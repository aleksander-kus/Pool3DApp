using DomainLayer.Enum;

namespace DomainLayer.Dto
{
    class IlluminationParameters
    {
        public float Ka { get; set; }
        public float Kd { get; set; }
        public float Ks { get; set; }
        public LightSources LightSources { get; set; }
    }
}
