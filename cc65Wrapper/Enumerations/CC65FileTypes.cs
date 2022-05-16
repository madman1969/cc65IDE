using System;

namespace cc65Wrapper.Enumerations
{
    [Flags]
    public enum CC65FileTypes
    {
        None = 0,
        SourceFile = 2,
        IncludeFile = 4
    }
}
