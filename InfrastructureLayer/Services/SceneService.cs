using DomainLayer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        public float CubeLength { get; set; } = 0.05f;

        public (List<ModelTriangle> triangles, List<ModelRectangle> rectangles) GetScene()
        {
            List<ModelTriangle> triangles = new();
            List<ModelRectangle> rectangles = new()
            {
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, 0, 0), new ModelPoint(0, TableLength, 0), new ModelPoint(TableWidth, TableLength, 0), new ModelPoint(TableWidth, 0, 0) }, Color.Green),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, 0, 0), new ModelPoint(0, 0, TableHeight), new ModelPoint(0, TableLength, TableHeight), new ModelPoint(0, TableLength, 0) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, TableLength, 0), new ModelPoint(0, TableLength, TableHeight), new ModelPoint(TableWidth, TableLength, TableHeight), new ModelPoint(TableWidth, TableLength, 0) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth, 0, 0), new ModelPoint(TableWidth, 0, TableHeight), new ModelPoint(TableWidth, TableLength, TableHeight), new ModelPoint(TableWidth, TableLength, 0) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, 0, 0), new ModelPoint(0, 0, TableHeight), new ModelPoint(TableWidth, 0, TableHeight), new ModelPoint(TableWidth, 0, 0) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, 0), new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, 0) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, 0), new ModelPoint(-SideWidth, -SideWidth, 0), new ModelPoint(-SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, -SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, 0, TableHeight), new ModelPoint(-SideWidth, 0, TableHeight), new ModelPoint(-SideWidth, -SideWidth, TableHeight) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-SideWidth, -SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(-SideWidth, -SideWidth, TableHeight) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(0, -SideWidth, TableHeight), new ModelPoint(0, TableLength + SideWidth, TableHeight), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(-SideWidth, -SideWidth, TableHeight) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, 0), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight) }, Color.Brown),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(TableWidth + SideWidth, TableLength + SideWidth, TableHeight), new ModelPoint(TableWidth + SideWidth, TableLength, TableHeight), new ModelPoint(-SideWidth, TableLength, TableHeight), new ModelPoint(-SideWidth, TableLength + SideWidth, TableHeight) }, Color.Brown),
            };
            rectangles.ForEach(rectangle => triangles.AddRange(rectangle.Split()));
            return (triangles, rectangles);
        }

        public (List<ModelTriangle> triangles, List<ModelRectangle> rectangles) GetCube()
        {
            float a = CubeLength / 2;
            List<ModelTriangle> triangles = new();
            List<ModelRectangle> rectangles = new()
            {
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-a, -a, 0), new ModelPoint(-a, a, 0), new ModelPoint(a, a, 0), new ModelPoint(a, -a, 0) }, Color.Blue),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-a, -a, a), new ModelPoint(-a, a, a), new ModelPoint(a, a, a), new ModelPoint(a, -a, a) }, Color.Blue),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(-a, -a, 0), new ModelPoint(-a, a, 0), new ModelPoint(-a, a, a), new ModelPoint(-a, -a, a) }, Color.Blue),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(a, -a, 0), new ModelPoint(a, a, 0), new ModelPoint(a, a, a), new ModelPoint(a, -a, a) }, Color.Blue),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(a, -a, 0), new ModelPoint(a, -a, a), new ModelPoint(-a, -a, a), new ModelPoint(-a, -a, 0) }, Color.Blue),
                new ModelRectangle(new List<ModelPoint> { new ModelPoint(a, a, 0), new ModelPoint(a, a, a), new ModelPoint(-a, a, a), new ModelPoint(-a, a, 0) }, Color.Blue),

            };
            rectangles.ForEach(rectangle => triangles.AddRange(rectangle.Split()));
            return (triangles, rectangles);
        }
    }
}
