using System.ComponentModel;

namespace _11thLauncher.Model.Game
{
    public enum Priority
    {
        [Description("S_PRIORITY_NORMAL")]
        Normal,

        [Description("S_PRIORITY_ABOVENORMAL")]
        AboveNormal,

        [Description("S_PRIORITY_HIGH")]
        High 
    }
}
