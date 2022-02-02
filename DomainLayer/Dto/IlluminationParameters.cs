using DomainLayer.Enum;

namespace DomainLayer.Dto
{
    public class IlluminationParameters
    {
        public float Ka { get; set; } = 0.4f;
        public float Kd { get; set; } = 0.5f;
        public float Ks { get; set; } = 0.5f;
        public int N { get; set; } = 5;
        public float MainLightHeight { set => MainLightPosition = new ModelPoint(MainLightPosition.X, MainLightPosition.Y, value); }
        public ModelPoint MainLightPosition { get; set; } = new ModelPoint(0.5f, 2, 1f);
        public LightSources LightSources { get; set; } = LightSources.Top;
    }
}
