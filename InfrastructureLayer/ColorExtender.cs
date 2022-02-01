using System.Drawing;
using System.Numerics;

namespace InfrastructureLayer
{
    public static class ColorExtender
    {
        public static Vector3 From255(this Color color)
        {
            var r = color.R < 0 ? 0 : color.R > 255 ? 1 : color.R / 255f;
            var g = color.G < 0 ? 0 : color.G > 255 ? 1 : color.G / 255f;
            var b = color.B < 0 ? 0 : color.B > 255 ? 1 : color.B / 255f;
            return new Vector3(r, g, b);
        }

        public static Color To255(this Vector3 color)
        {
            float r = color.X < 0 ? 0 : (color.X > 1 ? 255 : color.X * 255);
            float g = color.Y < 0 ? 0 : (color.Y > 1 ? 255 : color.Y * 255);
            float b = color.Z < 0 ? 0 : (color.Z > 1 ? 255 : color.Z * 255);
            if ((int)r < 0 || (int)r > 255)
                r = 1;
            if ((int)g < 0 || (int)g > 255)
                g = 1;
            if ((int)b < 0 || (int)b > 255)
                b = 1;
            return Color.FromArgb((int)r, (int)g, (int)b);
        }
    }
}
