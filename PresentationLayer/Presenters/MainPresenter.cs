using DomainLayer;
using DomainLayer.Cameras;
using DomainLayer.Dto;
using DomainLayer.Enum;
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
        private readonly IlluminationService illuminationService;
        private readonly ProjectionParameters projectionParameters;
        private readonly IlluminationParameters illuminationParameters;
        private readonly DrawingParameters drawingParameters;
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
        public int Kd
        {
            set
            {
                illuminationParameters.Kd = value / 100f;
                Update();
            }
        }
        public int Ks
        {
            set
            {
                illuminationParameters.Ks = value / 100f;
                Update();
            }
        }

        public ShadingMode ShadingMode
        {
            set
            {
                if(drawingParameters.ShadingMode != value)
                {
                    drawingParameters.ShadingMode = value;
                    Update();
                }
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
            illuminationParameters = new();
            drawingParameters = new();
            illuminationService = new(illuminationParameters);
            drawingService = new DrawingService(illuminationService, drawingParameters);
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
            Update();
        }

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
        private float cubeYDelta = 0.03f;
        public void Update()
        {
            using Graphics g1 = Graphics.FromImage(bitmap);
            g1.Clear(Color.Black);
            scene.Cube.Center = new ModelPoint(scene.Cube.Center.X, scene.Cube.Center.Y + cubeYDelta, scene.Cube.Center.Z);
            if (scene.Cube.Center.Y > 1.95f || scene.Cube.Center.Y < 0.3f) cubeYDelta = -cubeYDelta;
            scene.Cube.Rotation += 10;
            //Camera activeCamera = cameras[activeCameraId];
            IFastBitmap fastBitmap = new ByteBitmap(bitmap);

            projectionService.ProjectScene(fastBitmap);
            bitmap = fastBitmap.Bitmap;
            view.CanvasImage = bitmap;
            view.RedrawCanvas();
        }

        public void LoadCanvasDimensions()
        {
            bitmap = new(view.CanvasWidth, view.CanvasHeight);
            projectionParameters.CanvasHeight = view.CanvasHeight;
            projectionParameters.CanvasWidth = view.CanvasWidth;
            view.CanvasImage = bitmap;
            Update();
        }
    }
}
