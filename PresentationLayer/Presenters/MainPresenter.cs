using DomainLayer;
using DomainLayer.Cameras;
using InfrastructureLayer;
using InfrastructureLayer.Services;
using PresentationLayer.ViewLoaders;
using PresentationLayer.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

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
                Update();
            }
        }
        private Matrix4x4 modelMatrix;
        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;
        private List<ModelTriangle> triangles = new();
        private List<ModelRectangle> rectangles = new();
        private MovingCube cube = new();
        private List<Camera> cameras = new();
        private int activeCameraId = 0;
        public MainPresenter(IMainView view, IViewLoader viewLoader)
        {
            this.view = view;
            this.viewLoader = viewLoader;
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            drawingService = new DrawingService();
            matrixService = new MatrixService();
            sceneService = new SceneService();
            (triangles, rectangles) = sceneService.GetScene();
            (cube.Triangles, cube.Rectangles) = sceneService.GetCube();
            cube.Center = new ModelPoint(0.5f, 1, .1f);
            cameras.Add(new StaticCamera(new ModelPoint(2, 2.5f, 2), new ModelPoint(0.5f, 1, 0), new Vector3(0, 0, 1)));
            cameras.Add(new CubeFollowingCamera(new ModelPoint(2, 2.5f, 2), new ModelPoint(0.5f, 1, 0), new Vector3(0, 0, 1), cube));
            cameras.Add(new CubeOnTopCamera(new ModelPoint(2, 2.5f, 2), new ModelPoint(0, 0, 0), new Vector3(0, 0, 1), cube));

            Update();
            this.view.CanvasImage = bitmap;
            this.view.RedrawCanvas();
        }

        private CanvasPoint ProjectPoint(ModelPoint point)
        {

            var vec = Vector4.Transform(Vector4.Transform(Vector4.Transform(point.Coordinates4, modelMatrix), viewMatrix), projectionMatrix);
            vec /= vec.W;
            return ConvertToCanvas(vec);
        }

        private CanvasTriangle ProjectTriangle(ModelTriangle triangle)
        {
            return new CanvasTriangle(triangle.Points.Select(point => ProjectPoint(point)).ToList(), triangle.Color);
        }

        private CanvasRectangle ProjectRectangle(ModelRectangle rectangle)
        {
            return new CanvasRectangle(rectangle.Points.Select(point => ProjectPoint(point)).ToList(), rectangle.Color);
        }

        private CanvasPoint ConvertToCanvas(Vector4 coords)
        {
            int x = (int)Math.Round(view.CanvasWidth / 2 * (coords.X + 1));
            int y = (int)Math.Round(view.CanvasHeight / 2 * (-coords.Y + 1));
            float z = coords.Z;
            return new CanvasPoint(x, y, z);
        }

        public void MoveCube(float x = 0, float y = 0, float z = 0, int angle = 0)
        {
            cube.Center = new ModelPoint(cube.Center.X + x, cube.Center.Y + y, cube.Center.Z + z);
            cube.Rotation += angle;
        }

        public void SwitchCamera()
        {
            activeCameraId = (activeCameraId + 1) % cameras.Count;
        }

        public void Update()
        {
            using Graphics g1 = Graphics.FromImage(bitmap);
            g1.Clear(Color.White);
            double[,] zbuffer = new double[view.CanvasWidth, view.CanvasHeight];
            for (int i = 0; i < zbuffer.GetLength(0); i++)
                for (int j = 0; j < zbuffer.GetLength(1); j++)
                    zbuffer[i, j] = double.PositiveInfinity;
            Camera activeCamera = cameras[activeCameraId];
            IFastBitmap fastBitmap = new ByteBitmap(bitmap);

            modelMatrix = Matrix4x4.Identity;
            viewMatrix = Matrix4x4.CreateLookAt(activeCamera.Position.Coordinates, activeCamera.Target.Coordinates, activeCamera.UpVector);
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView((float)fov, (float)view.CanvasHeight / view.CanvasWidth, n, (float)f);
            var projectedTriangles = triangles.Select(triangle => ProjectTriangle(triangle)).ToList();
            var projectedRectangles = rectangles.Select(rectangle => ProjectRectangle(rectangle)).ToList();
            drawingService.ColorTriangles(fastBitmap, projectedTriangles, zbuffer);
            drawingService.DrawContour(fastBitmap, projectedRectangles, Color.Black, zbuffer);

            modelMatrix = Matrix4x4.CreateTranslation(cube.Center.Coordinates) * Matrix4x4.CreateRotationZ(cube.Rotation * (float)Math.PI / 180, cube.Center.Coordinates);
            var projectedCubeTriangles = cube.Triangles.Select(triangle => ProjectTriangle(triangle)).ToList();
            var projectedCubeWalls = cube.Rectangles.Select(wall => ProjectRectangle(wall)).ToList();
            drawingService.ColorTriangles(fastBitmap, projectedCubeTriangles, zbuffer);
            drawingService.DrawContour(fastBitmap, projectedCubeWalls, Color.Black, zbuffer);
            bitmap = fastBitmap.Bitmap;
            view.CanvasImage = bitmap;
            view.RedrawCanvas();
        }

        public void LoadCanvasDimensions()
        {
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            view.CanvasImage = bitmap;
            Update();
        }
    }
}
