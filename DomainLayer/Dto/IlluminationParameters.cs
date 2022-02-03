using DomainLayer.Enum;
using System.Numerics;

namespace DomainLayer.Dto
{
    public class IlluminationParameters
    {
        public float Ka { get; set; } = 0.4f;
        public float Kd { get; set; } = 0.5f;
        public float Ks { get; set; } = 0.5f;
        public int N { get; set; } = 5;
        public float MainLightX { set => MainLightPosition = new ModelPoint(value, MainLightPosition.Y, MainLightPosition.Z); }
        public float MainLightY { set => MainLightPosition = new ModelPoint(MainLightPosition.X, value, MainLightPosition.Z); }
        public float MainLightZ { set => MainLightPosition = new ModelPoint(MainLightPosition.X, MainLightPosition.Y, value); }
        public ModelPoint MainLightPosition { get; set; } = new ModelPoint(0.5f, 1, 1f);
        public ModelPoint ReflectorPosition { get; set; } = new ModelPoint(0, 0, 0);
        public Vector3 BaseReflectorDirection { get; set; } = new Vector3(-1, 0, 0);
        public Vector3 ModifiedReflectorDirection { get; set; } = new Vector3(-1, 0, 0);
        public int ReflectorMr { get; set; } = 10;
        public LightSources LightSources { get; set; } = LightSources.Main;
        public bool Fog { get; set; } = false;
        public float FogDensity { get; set; } = 0.15f;
    }
}
