using DomainLayer.Enum;

namespace DomainLayer.Dto
{
    public class IlluminationParameters
    {
        public float Ka { get; set; } = 0.4f;
        public float Kd { get; set; } = 0.5f;
        public float Ks { get; set; } = 0.5f;
        public int N { get; set; } = 5;
        public ModelPoint MainLightPosition { get; set; } = new ModelPoint(0.5f, 1, 1f);
        public LightSources LightSources { get; set; } = LightSources.Top;
    }
}
