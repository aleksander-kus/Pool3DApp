using InfrastructureLayer;
using InfrastructureLayer.Services;
using PresentationLayer.ViewLoaders;
using PresentationLayer.Views;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Linq;
using System.Collections.Generic;

namespace PresentationLayer.Presenters
{
    public class MainPresenter
    {
        private Bitmap bitmap;
        private readonly IMainView view;
        private readonly IViewLoader viewLoader;
        private readonly IDrawingService drawingService;
        private readonly MatrixService matrixService;
        private const int n = 1;
        private const double f = 100;
        double fov = Math.PI / 180 * 45; // kat patrzenia w stopniach

        double e;
        double a = 1; // aspect ratio picture boxa (H / W)
        double[,] ModelMatrix;
        double[,] ViewMatrix;
        double[,] ProjectionMatrix;
        double[][] Points;
        List<List<int>> connections;
        public MainPresenter(IMainView view, IViewLoader viewLoader)
        {
            this.view = view;
            this.viewLoader = viewLoader;
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            drawingService = new DrawingService();
            matrixService = new MatrixService();

            e = 1 / Math.Tan(fov / 2);
            a = (double)this.view.CanvasHeight / this.view.CanvasWidth;
            double[,] mm = { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            double[,] vm = { { 0, 1, 0, -0.5 }, { 0, 0, 1, -0.5 }, { 1, 0, 0, -3 }, { 0, 0, 0, 1 } };
            double[,] pm = { { e, 0, 0, 0 }, { 0, e / a, 0, 0 }, { 0, 0, -(f + n) / (f - n), -2 * f * n / (f - n) }, { 0, 0, -1, 0 } };
            ModelMatrix = mm;
            ViewMatrix = vm;
            ProjectionMatrix = pm;
            double[][] points = { new double[]{ 0, 0, 0, 1 }, new double[]{ 1, 0, 0, 1 }, new double[]{ 1, 1, 0, 1 }, new double[]{ 0, 1, 0, 1 },
            new double[]{ 0, 0, 1, 1 }, new double[]{ 1, 0, 1, 1 }, new double[]{ 1, 1, 1, 1 }, new double[]{ 0, 1, 1, 1 }};
            Points = points;
            List<List<int>> connectedWith = new();
            foreach(var point in points)
            {
                List<int> connected = new();
                for (int i = 0; i < points.Length; ++i)
                    if (points[i][0] == point[0] && points[i][1] == point[1] && points[i][2] != point[2] || points[i][1] == point[1] && points[i][2] == point[2] && points[i][0] != point[0] || points[i][0] == point[0] && points[i][2] == point[2] && points[i][1] != point[1])
                        connected.Add(i);
                connectedWith.Add(connected);
            }
            connections = connectedWith;
            Rotate();
            this.view.CanvasImage = bitmap;
            this.view.RedrawCanvas();
        }

        private double[] ProjectPoint(double[] point)
        {
            double[] pointPrim = matrixService.Multiply(ProjectionMatrix, matrixService.Multiply(ViewMatrix, matrixService.Multiply(ModelMatrix, point)));
            return pointPrim.Select(coord => coord / pointPrim[3]).ToArray();
        }

        private Point ConvertToCanvas(double[] point)
        {
            int x = (int)Math.Round(view.CanvasWidth / 2 * (point[0] + 1));
            int y = (int)Math.Round(view.CanvasHeight / 2 * (point[1] + 1));
            return new Point(x, y);
        }

        private void DrawPoints(Point[] points)
        {
            Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            for(int i = 0; i < points.Length; ++i)
            {
                g.FillRectangle(Brushes.Black, points[i].X, points[i].Y, 2, 2);
                for(int j = 0; j < connections[i].Count; ++j)
                    g.DrawLine(Pens.Black, points[i], points[connections[i][j]]);
            }
        }
        private int alpha = 0;
        public void Rotate()
        {
            double rad = Math.PI * alpha / 180;
            double[,] mm = { { Math.Cos(rad), -Math.Sin(rad), 0, 0 }, { Math.Sin(rad), Math.Cos(rad), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            ModelMatrix = mm;
            var hehe = Points.ToList()
                .Select(point => ConvertToCanvas(ProjectPoint(point)))
                .ToArray();
            DrawPoints(hehe);
            view.RedrawCanvas();
            alpha += 5;
        }
    }
}
