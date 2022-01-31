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
        private readonly DrawingService drawingService;
        private readonly MatrixService matrixService;
        private readonly SceneService sceneService;
        private const int n = 1;
        private const double f = 100;
        private double fov = Math.PI / 180 * 60; // kat patrzenia w stopniach
        public int Fov
        {
            set
            {
                fov = Math.PI / 180 * value;
                Rotate();
            }
        }
        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;
        private List<ModelTriangle> triangles = new();
        private List<ModelRectangle> rectangles = new();
        public MainPresenter(IMainView view, IViewLoader viewLoader)
        {
            this.view = view;
            this.viewLoader = viewLoader;
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            drawingService = new DrawingService();
            matrixService = new MatrixService();
            sceneService = new SceneService();

            //e = 1 / Math.Tan(fov / 2);
            //a = (double)this.view.CanvasHeight / this.view.CanvasWidth;
            //double[,] mm = { { 1, 0, 0, 0 }, { 0, 1, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            //double[,] vm = { { 0, 1, 0, -0.5 }, { 0, 0, 1, -0.5 }, { 1, 0, 0, -10 }, { 0, 0, 0, 1 } };
            //double[,] pm = { { e, 0, 0, 0 }, { 0, e / a, 0, 0 }, { 0, 0, -(f + n) / (f - n), -2 * f * n / (f - n) }, { 0, 0, -1, 0 } };
            //ModelMatrix = mm;
            //double[][] points = { new double[]{ 0, 0, 0, 1 }, new double[]{ 1, 0, 0, 1 }, new double[]{ 1, 1, 0, 1 }, new double[]{ 0, 1, 0, 1 },
            //new double[]{ 0, 0, 1, 1 }, new double[]{ 1, 0, 1, 1 }, new double[]{ 1, 1, 1, 1 }, new double[]{ 0, 1, 1, 1 }};
            //Points = points;
            //List<List<int>> connectedWith = new();
            //foreach(var point in points)
            //{
            //    List<int> connected = new();
            //    for (int i = 0; i < points.Length; ++i)
            //        //if (points[i][0] == point[0] && points[i][1] == point[1] && points[i][2] != point[2] || 
            //        //    points[i][1] == point[1] && points[i][2] == point[2] && points[i][0] != point[0] || 
            //        //    points[i][0] == point[0] && points[i][2] == point[2] && points[i][1] != point[1])
            //        if (points[i][0] == point[0] ||
            //            points[i][1] == point[1] ||
            //            points[i][2] == point[2])
            //            connected.Add(i);
            //    connectedWith.Add(connected);
            //}
            //connections = connectedWith;
            //ExtractTriangles();
            (triangles, rectangles) = sceneService.GetScene();
            Rotate();
            this.view.CanvasImage = bitmap;
            this.view.RedrawCanvas();
        }

        private CanvasPoint ProjectPoint(ModelPoint point)
        {
            var vec = Vector4.Transform(Vector4.Transform(point.Coordinates4, viewMatrix), projectionMatrix);
            vec /= vec.W;
            return ConvertToCanvas(vec);
        }

        private CanvasTriangle ProjectTriangle(ModelTriangle triangle)
        {
            return new CanvasTriangle(triangle.Points.Select(point => ProjectPoint(point)).ToList(), triangle.Color);
        }

        private CanvasPoint ConvertToCanvas(Vector4 coords)
        {
            int x = (int)Math.Round(view.CanvasWidth / 2 * (coords.X + 1));
            int y = (int)Math.Round(view.CanvasHeight / 2 * (-coords.Y + 1));
            float z = coords.Z;
            return new CanvasPoint(x, y, z);
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
            //triangles.Clear();
            //for(int i = 0;i <= 1; ++i)
            //{
            //    triangles.Add(new List<double[]> { new double[] { 0, 0, i, 1 }, new double[] { 0, 1, i, 1 }, new double[] { 1, 1, i, 1 } });
            //    triangles.Add(new List<double[]> { new double[] { 0, 0, i, 1 }, new double[] { 1, 0, i, 1 }, new double[] { 1, 1, i, 1 } });
            //}
            //for (int i = 0; i <= 1; ++i)
            //{
            //    triangles.Add(new List<double[]> { new double[] { 0, i, 0, 1 }, new double[] { 0, i, 1, 1 }, new double[] { 1, i, 1, 1 } });
            //    triangles.Add(new List<double[]> { new double[] { 0, i, 0, 1 }, new double[] { 1, i, 0, 1 }, new double[] { 1, i, 1, 1 } });
            //}
            //for (int i = 0; i <= 1; ++i)
            //{
            //    triangles.Add(new List<double[]> { new double[] { i, 0, 0, 1 }, new double[] { i, 1, 0, 1 }, new double[] { i, 1, 1, 1 } });
            //    triangles.Add(new List<double[]> { new double[] { i, 0, 0, 1 }, new double[] { i, 0, 1, 1 }, new double[] { i, 1, 1, 1 } });
            //}
        }

        private int alpha = 0;
        public void Rotate(int degree = 5)
        {
            using Graphics g1 = Graphics.FromImage(bitmap);
            g1.Clear(Color.White);

            viewMatrix = Matrix4x4.CreateLookAt(new Vector3(2, 2.5f, 2), new Vector3(0.5f, 1, 0), new Vector3(0, 0, 1));
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)fov, (float)view.CanvasHeight / view.CanvasWidth, n, (float)f);
            //triangles[0].Points[0] = new ModelPoint(triangles[0].Points[0].Coordinates.X + 0.05f, triangles[0].Points[0].Coordinates.Y, triangles[0].Points[0].Coordinates.Z);
            var projectedTriangles = triangles.Select(triangle => ProjectTriangle(triangle)).ToList();

            //double rad = Math.PI * alpha / 180;
            //e = 1 / Math.Tan(fov / 2);
            //a = (double)this.view.CanvasHeight / this.view.CanvasWidth;
            //double[,] mm = { { Math.Cos(rad), -Math.Sin(rad), 0, 0 }, { Math.Sin(rad), Math.Cos(rad), 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } };
            //double[,] pm = { { e, 0, 0, 0 }, { 0, e / a, 0, 0 }, { 0, 0, -(f + n) / (f - n), -2 * f * n / (f - n) }, { 0, 0, -1, 0 } };
            //ModelMatrix = mm;
            //ProjectionMatrix = pm;
            //var projected_triangles = triangles.Select(triangle => triangle.Select(point => ConvertToCanvas(ProjectPoint(point))).ToList()).ToList();
            //ModelMatrix = mm;
            double[,] zbuffer = new double[view.CanvasWidth, view.CanvasHeight];
            for (int i = 0; i < zbuffer.GetLength(0); i++)
                for (int j = 0; j < zbuffer.GetLength(1); j++)
                    zbuffer[i, j] = double.PositiveInfinity;
            //DrawTriangles(projected_triangles);
            var point = ProjectPoint(new ModelPoint(0.5f, 1, 0));
            var center = ProjectPoint(new ModelPoint(0, 0, 0));
            //var p1 = ProjectPoint(new ModelPoint(-0.1f, -0.1f, 0.1f));
            //var p2 = ProjectPoint(new ModelPoint(-0.1f, 2.1f, 0.1f));
            //var p3 = ProjectPoint(new ModelPoint(1.1f, 2.1f, 0.1f));
            //var p4 = ProjectPoint(new ModelPoint(1.1f, -0.1f, 0.1f));
            var p1 = ProjectPoint(new ModelPoint(0f, 0f, 0.1f));
            var p2 = ProjectPoint(new ModelPoint(0f, 2f, 0.1f));
            var p3 = ProjectPoint(new ModelPoint(1f, 2f, 0.1f));
            var p4 = ProjectPoint(new ModelPoint(1f, 0f, 0.1f));
            IFastBitmap fastBitmap = new ByteBitmap(bitmap);
            //drawingService.ColorTriangles(fastBitmap, projected_triangles, zbuffer, 2000);
            //rad *= 2;
            //double[,] mm2 = { { 1, 0, 0, 0 }, { 0, Math.Cos(rad), -Math.Sin(rad), 0 }, { 0, Math.Sin(rad), Math.Cos(rad), 0 }, { 0, 0, 0, 1 } };

            //ModelMatrix = mm2;

            //projected_triangles = triangles.Select(triangle => triangle.Select(point => ConvertToCanvas(ProjectPoint(point))).ToList()).ToList();
            //DrawTriangles(projected_triangles);
            drawingService.ColorTriangles(fastBitmap, projectedTriangles, zbuffer, 4321);
            bitmap = fastBitmap.Bitmap;
            using Graphics g = Graphics.FromImage(bitmap);
            //g.FillRectangle(Brushes.Black, point.Coordinates.X, point.Coordinates.Y, 5, 5);
            //g.FillRectangle(Brushes.Black, center.Coordinates.X, center.Coordinates.Y, 5, 5);
            g.DrawPolygon(new Pen(Brushes.Black, 1), new PointF[] { new PointF(p1.Coordinates.X, p1.Coordinates.Y), new PointF(p2.Coordinates.X, p2.Coordinates.Y), new PointF(p3.Coordinates.X, p3.Coordinates.Y), new PointF(p4.Coordinates.X, p4.Coordinates.Y) });
            view.CanvasImage = bitmap;
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
