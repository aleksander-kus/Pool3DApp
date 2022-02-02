using System.Drawing;

namespace DomainLayer
{
    public interface IFastBitmap
    {
        public int Width { get; }
        public int Height { get; }
        public Bitmap Bitmap { get; }
        public void SetPixel(int x, int y, Color color);
        public Color GetPixel(int x, int y);
    }
}
