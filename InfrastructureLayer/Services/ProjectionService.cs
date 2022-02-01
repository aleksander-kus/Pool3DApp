using DomainLayer;
using DomainLayer.ModelSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class ProjectionService
    {
        private readonly DrawingService drawingService;
        private readonly Scene scene;
        private readonly ProjectionParameters parameters;
        private Matrix4x4 modelMatrix;
        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;

        public ProjectionService(Scene scene, ProjectionParameters parameters, DrawingService drawingService)
        {
            this.scene = scene;
            this.parameters = parameters;
            this.drawingService = drawingService;
        }

        public void ProjectScene(IFastBitmap bitmap)
        {
            double[,] zbuffer = new double[parameters.CanvasWidth, parameters.CanvasHeight];
            for (int i = 0; i < zbuffer.GetLength(0); i++)
                for (int j = 0; j < zbuffer.GetLength(1); j++)
                    zbuffer[i, j] = double.PositiveInfinity;

            viewMatrix = Matrix4x4.CreateLookAt(parameters.Camera.Position.Coordinates, parameters.Camera.Target.Coordinates, parameters.Camera.UpVector);
            projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(parameters.FieldOfView, (float)parameters.CanvasHeight / parameters.CanvasWidth, parameters.NearPlaneDistance, parameters.FarPlaneDistance);

            ProjectTable(bitmap, zbuffer);
            ProjectCube(bitmap, zbuffer);
            ProjectSpheres(bitmap, zbuffer);
        }

        private void ProjectTable(IFastBitmap bitmap, double[,] zbuffer)
        {
            modelMatrix = Matrix4x4.Identity;
            var projectedTriangles = scene.TableTriangles.Select(triangle => ProjectTriangle(triangle)).ToList();
            drawingService.ColorTriangles(bitmap, projectedTriangles, zbuffer);
        }

        private void ProjectCube(IFastBitmap bitmap, double[,] zbuffer)
        {
            modelMatrix = Matrix4x4.CreateTranslation(scene.Cube.Center.Coordinates) * Matrix4x4.CreateRotationZ(scene.Cube.Rotation * (float)Math.PI / 180, scene.Cube.Center.Coordinates);
            var projectedTriangles = scene.Cube.Triangles.Select(triangle => ProjectTriangle(triangle)).ToList();
            drawingService.ColorTriangles(bitmap, projectedTriangles, zbuffer);
        }

        private void ProjectSpheres(IFastBitmap bitmap, double[,] zbuffer)
        {
            foreach(var sphere in scene.Spheres)
            {
                foreach (var triangle in sphere.Triangles)
                    triangle.SphereCenter = ProjectPoint(sphere.Center);
                modelMatrix = Matrix4x4.CreateTranslation(sphere.Center.Coordinates);
                //sphere.Center = ProjectPoint(sphere.Center);
                var projectedTriangles = sphere.Triangles.Select(triangle => ProjectTriangle(triangle)).ToList();
                drawingService.ColorTriangles(bitmap, projectedTriangles, zbuffer);
            }
        }

        private ModelTriangle ProjectTriangle(ModelTriangle triangle)
        {
            return triangle.NewFromPoints(triangle.Points.Select(point => ProjectPoint(point)).ToList());
        }

        private ModelPoint ProjectPoint(ModelPoint point)
        {
            var vec = Vector4.Transform(Vector4.Transform(Vector4.Transform(point.Coordinates4, modelMatrix), viewMatrix), projectionMatrix);
            vec /= vec.W;
            return ConvertToCanvas(vec);
        }

        private ModelPoint ConvertToCanvas(Vector4 coords)
        {
            int x = (int)Math.Round(parameters.CanvasWidth / 2 * (coords.X + 1));
            int y = (int)Math.Round(parameters.CanvasHeight / 2 * (-coords.Y + 1));
            float z = coords.Z;
            return new ModelPoint(x, y, z);
        }

    }
    
}
