namespace DomainLayer
{
    public class ProjectionParameters
    {
        public Camera Camera { get; set; }
        public float FieldOfView { get; set; }
        public float AspectRatio { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
    }
}
