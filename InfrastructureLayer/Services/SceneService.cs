using DomainLayer;
using DomainLayer.ModelSpace;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Services
{
    public class SceneService
    {
        public float TableHeight { get; set; } = 0.1f;
        public float TableWidth { get; set; } = 1;
        public float TableLength { get; set; } = 2;
        public float SideWidth { get; set; } = 0.1f;
        public float CubeLength { get; set; } = 0.1f;
        public int CubeMeridians { get; set; } = 20;
        public int CubeParallels { get; set; } = 20;
        public float SphereRadius { get; set; } = 0.05f;

        public Scene GetScene()
        {
            return new Scene()
            {
                TableTriangles = GenerateTableTriangles(),
                Cube = new MovingCube()
                {
                    Triangles = GenerateCube(),
                    Center = new ModelPoint(TableWidth / 3, TableLength / 2, CubeLength * 2),
                    Rotation = 0
                },
                Spheres = new List<ModelSphere>
                {
                    new ModelSphere()
                    {
                        Center = new ModelPoint(0.5f, 0.5f, SphereRadius),
                        Triangles = GenerateSphere(CubeMeridians, CubeParallels, SphereRadius, Color.White)
                    },
                    new ModelSphere()
                    {
                        Center = new ModelPoint(0.5f + 2 * SphereRadius, 1.5f + 2 * SphereRadius, SphereRadius),
                        Triangles = GenerateSphere(CubeMeridians, CubeParallels, SphereRadius, Color.Black)
                    },
                    new ModelSphere()
                    {
                        Center = new ModelPoint(0.5f - 2 * SphereRadius, 1.5f + 2 * SphereRadius, SphereRadius),
                        Triangles = GenerateSphere(CubeMeridians, CubeParallels, SphereRadius, Color.Blue)
                    },
                    new ModelSphere()
                    {
                        Center = new ModelPoint(0.5f, 1.5f, SphereRadius),
                        Triangles = GenerateSphere(CubeMeridians, CubeParallels, SphereRadius, Color.Red)
                    },
                }
            };
        }

        private List<FlatTriangle> GenerateTableTriangles()
        {
            List<FlatTriangle> triangles = new();
            List<ModelRectangle> rectangles = new()
            {
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, 0, 0), new ModelPoint(0, TableLength, 0), new ModelPoint(TableWidth, TableLength, 0), new ModelPoint(TableWidth, 0, 0) }, Color.Green, new Vector3(0, 0, 1)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, 0, 0), new ModelPoint(0, 0, TableHeight), new ModelPoint(0, TableLength, TableHeight), new ModelPoint(0, TableLength, 0) }, Color.Brown, new Vector3(1,0,0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, TableLength, 0), new ModelPoint(0, TableLength, TableHeight), new ModelPoint(TableWidth, TableLength, TableHeight), new ModelPoint(TableWidth, TableLength, 0) }, Color.Brown, new Vector3(0, -1, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth, 0, 0), new ModelPoint(TableWidth, 0, TableHeight), new ModelPoint(TableWidth, TableLength, TableHeight), new ModelPoint(TableWidth, TableLength, 0) }, Color.Brown, new Vector3(-1, 0 ,0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, 0, 0), new ModelPoint(0, 0, TableHeight), new ModelPoint(TableWidth, 0, TableHeight), new ModelPoint(TableWidth, 0, 0) }, Color.Brown, new Vector3(0, 1, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, 0), new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, 0) }, Color.Brown, new Vector3(1, 0, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight) }, Color.Brown, new Vector3(0,0,1)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, 0), new ModelPoint(-SideWidth, -SideWidth, 0), new ModelPoint(-SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight) }, Color.Brown, new Vector3(0, -1, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, 0, TableHeight), new ModelPoint(-SideWidth, 0, TableHeight), new ModelPoint(-SideWidth, -SideWidth, TableHeight) }, Color.Brown, new Vector3(0,0,1)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-SideWidth, -SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(-SideWidth, -SideWidth, TableHeight) }, Color.Brown, new Vector3(-1,0,0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, -SideWidth, TableHeight), new ModelPoint(0, TableLength + SideWidth, TableHeight), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(-SideWidth, -SideWidth, TableHeight) }, Color.Brown, new Vector3(0,0,1)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight) }, Color.Brown, new Vector3(0, 1, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength, TableHeight), new ModelPoint(-SideWidth, TableLength, TableHeight), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight) }, Color.Brown, new Vector3(0,0,1))
            };
            rectangles.ForEach(rectangle => triangles.AddRange(rectangle.Split()));
            return triangles;
        }

        private List<FlatTriangle> GenerateCube()
        {
            float a = CubeLength / 2;
            List<FlatTriangle> triangles = new();
            List<ModelRectangle> rectangles = new()
            {
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-a, -a, 0), new ModelPoint(-a, a, 0), new ModelPoint(a, a, 0), new ModelPoint(a, -a, 0) }, Color.Blue, new Vector3(0,0, -1)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-a, -a, a), new ModelPoint(-a, a, a), new ModelPoint(a, a, a), new ModelPoint(a, -a, a) }, Color.Blue, new Vector3(0,0,1)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-a, -a, 0), new ModelPoint(-a, a, 0), new ModelPoint(-a, a, a), new ModelPoint(-a, -a, a) }, Color.Blue, new Vector3(-1, 0, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(a, -a, 0), new ModelPoint(a, a, 0), new ModelPoint(a, a, a), new ModelPoint(a, -a, a) }, Color.Blue, new Vector3(1, 0, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(a, -a, 0), new ModelPoint(a, -a, a), new ModelPoint(-a, -a, a), new ModelPoint(-a, -a, 0) }, Color.Blue, new Vector3(0, -1, 0)),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(a, a, 0), new ModelPoint(a, a, a), new ModelPoint(-a, a, a), new ModelPoint(-a, a, 0) }, Color.Blue, new Vector3(0, 1, 0)),

            };
            rectangles.ForEach(rectangle => triangles.AddRange(rectangle.Split()));
            return triangles;
        }

        private List<SphereTriangle> GenerateSphere(int meridians, int parallels, float r, Color col)
        {
            var sphere = new List<(Vector3 v1, Vector3 v2, Vector3 v3, Color col)>();
            List<Vector3> vertices = new();

            vertices.Add(new Vector3(0f, r, 0f));

            for (int j = 0; j < parallels - 1; j++)
            {
                double polar = Math.PI * (double)(j + 1) / (double)(parallels);
                double sp = Math.Sin(polar);
                double cp = Math.Cos(polar);

                for (int i = 0; i < meridians; i++)
                {
                    double azimuth = 2.0 * Math.PI * (double)(i) / (double)(meridians);
                    double sa = Math.Sin(azimuth);
                    double ca = Math.Cos(azimuth);

                    float x = (float)(r * sp * ca);
                    float y = (float)(r * cp);
                    float z = (float)(r * sp * sa);
                    vertices.Add(new Vector3(x, y, z));
                }
            }

            vertices.Add(new Vector3(0f, -r, 0f));

            for (int i = 0; i < meridians; i++)
            {
                int a = i + 1;
                int b = (i + 1) % meridians + 1;
                sphere.Add((vertices[0], vertices[b], vertices[a], col));
            }

            for (int j = 0; j < parallels - 2; j++)
            {
                int aStart = j * meridians + 1;
                int bStart = (j + 1) * meridians + 1;

                for (int i = 0; i < meridians; i++)
                {
                    int a = aStart + i;
                    int a1 = aStart + (i + 1) % meridians;
                    int b = bStart + i;
                    int b1 = bStart + (i + 1) % meridians;
                    //Add Quad
                    sphere.Add((vertices[a], vertices[a1], vertices[b1], col));
                    sphere.Add((vertices[a], vertices[b1], vertices[b], col));
                }
            }

            for (int i = 0; i < meridians; i++)
            {
                int a = i + meridians * (parallels - 2) + 1;
                int b = (i + 1) % meridians + meridians * (parallels - 2) + 1;
                sphere.Add((vertices[vertices.Count - 1], vertices[a], vertices[b], col));
            }

            return sphere.Select(triangle => new SphereTriangle(new List<ModelPoint> { new ModelPoint(triangle.v1), new ModelPoint(triangle.v2), new ModelPoint(triangle.v3) }, col)).ToList();
        }
    }
}
