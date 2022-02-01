using System;

namespace DomainLayer.Enum
{
    [Flags]
    public enum LightSources
    {
        None = 0,
        Top = 1,
        Reflector = 2,
        All = Top | Reflector
    }
}
