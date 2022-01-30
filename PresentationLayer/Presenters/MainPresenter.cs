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
using DomainLayer;

namespace PresentationLayer.Presenters
{
    public class MainPresenter
    {
        private Bitmap bitmap;
        private readonly IMainView view;
        private readonly IViewLoader viewLoader;
        private readonly IDrawingService drawingService;
        private readonly MatrixService matrixService;
        private const int n = 5;
        private const double f = 100;
        private double fov = Math.PI / 180 * 100; // kat patrzenia w stopniach
        public int Fov
        {
            set
            {
                fov = Math.PI / 180 * value;
            }
        }

        private double e;
        private double a = 1; // aspect ratio picture boxa (H / W)
        private double[,] ModelMatrix;
        private double[,] ViewMatrix;
        private double[,] ProjectionMatrix;
        private double[][] Points;
        private List<List<int>> connections;
        private List<List<double[]>> triangles = new();
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
            double[,] vm = { { 0, 1, 0, -0.5 }, { 0, 0, 1, -0.5 }, { 1, 0, 0, -10 }, { 0, 0, 0, 1 } };
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
                    //if (points[i][0] == point[0] && points[i][1] == point[1] && points[i][2] != point[2] || 
                    //    points[i][1] == point[1] && points[i][2] == point[2] && points[i][0] != point[0] || 
                    //    points[i][0] == point[0] && points[i][2] == point[2] && points[i][1] != point[1])
                    if (points[i][0] == point[0] ||
                        points[i][1] == point[1] ||
                        points[i][2] == point[2])
                        connected.Add(i);
                connectedWith.Add(connected);
            }
            connections = connectedWith;
            ExtractTriangles();
            Rotate();
            this.view.CanvasImage = bitmap;
            this.view.RedrawCanvas();
        }

        private double[] ProjectPoint(double[] point)
        {
            double[] pointPrim = matrixService.Multiply(ProjectionMatrix, matrixService.Multiply(ViewMatrix, matrixService.Multiply(ModelMatrix, point)));
            return pointPrim.Select(coord => coord / pointPrim[3]).ToArray();
        }

        private Vector3 ConvertToCanvas(double[] point)
        {
            int x = (int)Math.Round(view.CanvasWidth / 2 * (point[0] + 1));
            int y = (int)Math.Round(view.CanvasHeight / 2 * (point[1] + 1));
            float z = (float)point[2];
            return new Vector3(x, y, z);
        }

        private void DrawPoints(Vector3[] points)
        {
            Graphics g = Graphics.FromImage(bitmap);
            for(int i = 0; i < points.Length; ++i)
            {
                g.FillRectangle(Brushes.Black, points[i].X, points[i].Y, 2, 2);
                for(int j = 0; j < connections[i].Count; ++j)
                    g.DrawLine(Pens.Black, new Point((int)points[i].X, (int)points[i].Y), new Point((int)points[connections[i][j]].X, (int)points[connections[i][j]].Y));
            }
        }

        private void DrawTriangles(List<List<Vector3>> triangles)
        {
            Graphics g = Graphics.FromImage(bitmap);
            foreach(var triangle in triangles)
            {
                g.DrawPolygon(Pens.Black, triangle.Select(point => new Point((int)point.X, (int)point.Y)).ToArray());
            }

        }
        private void ExtractTriangles()
        {
            triangles.Clear();
            for(int i = 0;i <= 1; ++i)
            {
                triangles.Add(new List<double[]> { new double[] { 0, 0, i, 1 }, new double[] { 0, 1, i, 1 }, new double[] { 1, 1, i, 1 } });
                triangles.Add(new List<double[]> { new double[] { 0, 0, i, 1 }, new double[] { 1, 0, i, 1 }, new double[] { 1, 1, i, 1 } });
            }
            for (int i = 0; i <= 1; ++i)
            {
                triangles.Add(new List<double[]> { new double[] { 0, i, 0, 1 }, new double[] { 0, i, 1, 1 }, new double[] { 1, i, 1, 1 } });
                triangles.Add(new List<double[]> { new double[] { 0, i, 0, 1 }, new double[] { 1, i, 0, 1 }, new double[] { 1, i, 1, 1 } });
            }
            for (int i = 0; i <= 1; ++i)
            {
                triangles.Add(new List<double[]> { new double[] { i, 0, 0, 1 }, new double[] { i, 1, 0, 1 }, new double[] { i, 1, 1, 1 } });
                triangles.Add(new List<double[]> { new double[] { i, 0, 0, 1 }, new double[] { i, 0, 1, 1 }, new double[] { i, 1, 1, 1 } });
            }
        }

        private int alpha = 0;
        public void Rotate(int degree = 5)
        {
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.White);
            double rad = Math.PI * alpha / 180;
            e = 1 / Math.Tan(fov / 2);
            a = (double)this.view.CanvasHeight / this.view.CanvasWidth;
            double[,] mm = { { Math.Cos(rad), -Math.Sin(rad), 0, 0 }, { Math.Sin(rad), Math.Cos(rad), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            double[,] pm = { { e, 0, 0, 0 }, { 0, e / a, 0, 0 }, { 0, 0, -(f + n) / (f - n), -2 * f * n / (f - n) }, { 0, 0, -1, 0 } };
            ModelMatrix = mm;
            ProjectionMatrix = pm;
            var projected_triangles = triangles.Select(triangle => triangle.Select(point => ConvertToCanvas(ProjectPoint(point))).ToList()).ToList();
            ModelMatrix = mm;
            double[,] zbuffer = new double[view.CanvasWidth, view.CanvasHeight];
            for (int i = 0; i < zbuffer.GetLength(0); i++)
                for (int j = 0; j < zbuffer.GetLength(1); j++)
                    zbuffer[i, j] = double.PositiveInfinity;
            DrawTriangles(projected_triangles);
            IFastBitmap fastBitmap = new ByteBitmap(bitmap);
            drawingService.ColorTriangles(fastBitmap, projected_triangles, zbuffer, 2000);
            rad *= 2;
            double[,] mm2 = { { 1, 0, 0, 0 }, { 0, Math.Cos(rad), -Math.Sin(rad), 0 }, { 0, Math.Sin(rad), Math.Cos(rad), 0 }, { 0, 0, 0, 1 } };

            ModelMatrix = mm2;

            projected_triangles = triangles.Select(triangle => triangle.Select(point => ConvertToCanvas(ProjectPoint(point))).ToList()).ToList();
            DrawTriangles(projected_triangles);
            drawingService.ColorTriangles(fastBitmap, projected_triangles, zbuffer, 4321);

            view.CanvasImage = bitmap = fastBitmap.Bitmap;
            view.RedrawCanvas();
            alpha += degree;
        }

        public void LoadCanvasDimensions()
        {
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            view.CanvasImage = bitmap;
            Rotate(0);

        }
    }
}
