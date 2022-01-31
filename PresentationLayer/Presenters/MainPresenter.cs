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
        private readonly SceneService sceneService;
        private readonly ProjectionService projectionService;
        private readonly ProjectionParameters projectionParameters;
        private const int n = 1;
        private const float f = 1.1f;
        public int Fov
        {
            set
            {
                projectionParameters.FieldOfView = (float)Math.PI / 180 * value;
                Update();
            }
        }

        private List<Camera> cameras = new();
        private int activeCameraId = 0;
        private readonly Scene scene;

        public MainPresenter(IMainView view, IViewLoader viewLoader)
        {
            this.view = view;
            this.viewLoader = viewLoader;
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            drawingService = new DrawingService();
            sceneService = new SceneService();
            scene = sceneService.GetScene();

            cameras.Add(new StaticCamera(new ModelPoint(2, 2.5f, 2), new ModelPoint(0.5f, 1, 0), new Vector3(0, 0, 1)));
            cameras.Add(new CubeFollowingCamera(new ModelPoint(2, 2.5f, 2), new ModelPoint(0.5f, 1, 0), new Vector3(0, 0, 1), scene.Cube));
            cameras.Add(new CubeOnTopCamera(new ModelPoint(2, 2.5f, 2), new ModelPoint(0, 0, 0), new Vector3(0, 0, 1), scene.Cube));
            projectionParameters = new()
            {
                Camera = cameras[activeCameraId],
                CanvasHeight = view.CanvasHeight,
                CanvasWidth = view.CanvasWidth,
                NearPlaneDistance = n,
                FarPlaneDistance = f,
                FieldOfView = (float)Math.PI / 180 * 60
            };

            projectionService = new(scene, projectionParameters, drawingService);

            //BALL = sceneService.GenerateSphere(25, 25, 0.1f, Color.Blue);
            Update();
        }



        //private CanvasRectangle ProjectRectangle(ModelRectangle rectangle)
        //{
        //    return new CanvasRectangle(rectangle.Points.Select(point => ProjectPoint(point)).ToList(), rectangle.Color);
        //}


        public void MoveCube(float x = 0, float y = 0, float z = 0, int angle = 0)
        {
            scene.Cube.Center = new ModelPoint(scene.Cube.Center.X + x, scene.Cube.Center.Y + y, scene.Cube.Center.Z + z);
            scene.Cube.Rotation += angle;
        }

        public void SwitchCamera()
        {
            activeCameraId = (activeCameraId + 1) % cameras.Count;
            projectionParameters.Camera = cameras[activeCameraId];
            Update();
        }

        public void Update()
        {
            using Graphics g1 = Graphics.FromImage(bitmap);
            g1.Clear(Color.White);
            
            //Camera activeCamera = cameras[activeCameraId];
            IFastBitmap fastBitmap = new ByteBitmap(bitmap);

            projectionService.ProjectScene(fastBitmap);
            //modelMatrix = Matrix4x4.Identity;
            //ProjectTriangles(triangles, fastBitmap, zbuffer);
            ////var projectedTriangles = triangles.Select(triangle => ProjectTriangle(triangle)).ToList();
            ////var projectedRectangles = rectangles.Select(rectangle => ProjectRectangle(rectangle)).ToList();
           
            ////drawingService.DrawContour(fastBitmap, projectedRectangles, Color.Black, zbuffer);

            ////var projectedBALL = BALL.Select(triangle => ProjectTriangle(triangle)).ToList();
            ////var projectedRectangles = rectangles.Select(rectangle => ProjectRectangle(rectangle)).ToList();
            ////drawingService.ColorTriangles(fastBitmap, projectedBALL, zbuffer);
            ////drawingService.DrawContour(fastBitmap, projectedRectangles, Color.Black, zbuffer);


            //ProjectTriangles(cube.Triangles, fastBitmap, zbuffer);
            ////var projectedCubeWalls = cube.Rectangles.Select(wall => ProjectRectangle(wall)).ToList();
            ////drawingService.DrawContour(fastBitmap, projectedCubeWalls, Color.Black, zbuffer);
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
