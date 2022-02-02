using System;

namespace DomainLayer.Enum
{
    [Flags]
    public enum LightSources
    {
        None = 0,
        Main = 1,
        Reflector = 2,
        All = Main | Reflector
    }
}
