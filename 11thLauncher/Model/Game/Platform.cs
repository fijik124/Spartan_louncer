using System.ComponentModel;

namespace _11thLauncher.Model.Game
{
    public enum Platform
    {
        [Description("S_PLATFORM_ANY")]
        Any,

        [Description("S_PLATFORM_32")]
        X86,

        [Description("S_PLATFORM_64")]
        X64
    }
}