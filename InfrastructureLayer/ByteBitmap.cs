using DomainLayer;
using System.Drawing;
using System.Drawing.Imaging;

namespace InfrastructureLayer
{
    /// <summary>
    /// A bitmap represented as a list of bytes
    /// </summary>
    public class ByteBitmap : IFastBitmap
    {
        private readonly byte[] bitmap;
        public Bitmap Bitmap
        {
            get
            {
                Bitmap b = new(Width, Height);
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(bitmap, 0, bitmapData.Scan0, bitmap.Length);
                b.UnlockBits(bitmapData);
                return b;
            }
        }
        public int BytesPerPixel { get; set; } = 4;
        public int Width { get; set; }
        public int Height { get; set; }

        /// <param name="width">Width in pixels</param>
        /// <param name="height">Height in pixels</param>
        /// <param name="bytesPerPixel"></param>
        public ByteBitmap(int width, int height, int bytesPerPixel = 4)
        {
            Width = width;
            Height = height;
            BytesPerPixel = bytesPerPixel;
            bitmap = new byte[Width * Height * BytesPerPixel];
        }

        public ByteBitmap(Bitmap b)
        {
            BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, b.PixelFormat);
            Width = b.Width;
            Height = b.Height;
            BytesPerPixel = bitmapData.Stride / b.Width;
            bitmap = new byte[Width * Height * BytesPerPixel];
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, bitmap, 0, bitmap.Length);
            b.UnlockBits(bitmapData);
        }

        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;
            bitmap[y * Width * BytesPerPixel + x * BytesPerPixel] = color.B;
            bitmap[y * Width * BytesPerPixel + x * BytesPerPixel + 1] = color.G;
            bitmap[y * Width * BytesPerPixel + x * BytesPerPixel + 2] = color.R;
            if (BytesPerPixel == 4)
            {
                bitmap[y * Width * BytesPerPixel + x * BytesPerPixel + 3] = color.A;
            }
        }

        public Color GetPixel(int x, int y)
        {
            x %= Width;
            y %= Height;
            var r = bitmap[y * Width * BytesPerPixel + x * BytesPerPixel + 2];
            var g = bitmap[y * Width * BytesPerPixel + x * BytesPerPixel + 1];
            var b = bitmap[y * Width * BytesPerPixel + x * BytesPerPixel];
            if (BytesPerPixel == 4)
            {
                var a = bitmap[y * Width * BytesPerPixel + x * BytesPerPixel + 3];
                return Color.FromArgb(a, r, g, b);
            }
            return Color.FromArgb(r, g, b);
        }
    }
}
