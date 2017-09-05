using System;
using System.ComponentModel;

namespace _11thLauncher.Models
{
    public enum RepositoryStatus
    {
        Unknown,
        Checking,
        Outdated,
        Updated
    }

    [Flags]
    public enum LaunchGameResult
    {
        GameLaunched = 1,
        NoElevation = 2,
        NoSteam = 4,
        UndefinedPath = 8,
        LaunchError = 16
    }

    public enum UpdateCheckResult
    {
        UpdateAvailable,
        NoUpdateAvailable,
        ErrorCheckingUpdates,
        ErrorRateExceeded
    }

    public enum LoadSettingsResult
    {
        NoExistingSettings,
        LoadedExistingSettings,
        LoadedLegacySettings,
        ErrorLoadingSettings,
        ErrorLoadingLegacySettings
    }

    public enum AsyncAction
    {
        [Description("S_ACTION_CHECK_SERVER_STATUS")]
        CheckServerStatus,

        [Description("S_ACTION_CHECK_REPOSITORY_STATUS")]
        CheckRepositoryStatus
    }

    public enum LaunchPlatform
    {
        [Description("S_PLATFORM_32")]
        X86,

        [Description("S_PLATFORM_64")]
        X64
    }

    public enum LaunchOption
    {
        [Description("S_LAUNCH_NORMAL")]
        Normal,

        [Description("S_LAUNCH_JOIN_SERVER")]
        JoinServer
    }

    public enum ParameterPlatform
    {
        [Description("S_PLATFORM_32")]
        X86,

        [Description("S_PLATFORM_64")]
        X64,

        [Description("S_PLATFORM_ANY")]
        Any
    }

    public enum ParameterType
    {
        Boolean,
        Selection,
        Text,
        Numerical
    }

    public enum ServerStatus
    {
        Unknown,
        Checking,
        Online,
        Offline
    }

    public enum StartAction
    {
        [Description("S_START_ACTION_NONE")]
        Nothing,

        [Description("S_START_ACTION_MINIMIZE")]
        Minimize,

        [Description("S_START_ACTION_CLOSE")]
        Close
    }

    public enum ThemeStyle
    {
        [Description("S_THEME_LIGHT")]
        BaseLight,

        [Description("S_THEME_DARK")]
        BaseDark
    }

    public enum AccentColor
    {
        [Description("S_ACCENT_RED")]
        Red,

        [Description("S_ACCENT_GREEN")]
        Green,

        [Description("S_ACCENT_BLUE")]
        Blue,

        [Description("S_ACCENT_PURPLE")]
        Purple,

        [Description("S_ACCENT_ORANGE")]
        Orange,

        [Description("S_ACCENT_LIME")]
        Lime,

        [Description("S_ACCENT_EMERALD")]
        Emerald,

        [Description("S_ACCENT_TEAL")]
        Teal,

        [Description("S_ACCENT_CYAN")]
        Cyan,

        [Description("S_ACCENT_COBALT")]
        Cobalt,

        [Description("S_ACCENT_INDIGO")]
        Indigo,

        [Description("S_ACCENT_VIOLET")]
        Violet,

        [Description("S_ACCENT_PINK")]
        Pink,

        [Description("S_ACCENT_MAGENTA")]
        Magenta,

        [Description("S_ACCENT_CRIMSON")]
        Crimson,

        [Description("S_ACCENT_AMBER")]
        Amber,

        [Description("S_ACCENT_YELLOW")]
        Yellow,

        [Description("S_ACCENT_BROWN")]
        Brown,

        [Description("S_ACCENT_OLIVE")]
        Olive,

        [Description("S_ACCENT_STEEL")]
        Steel,

        [Description("S_ACCENT_MAUVE")]
        Mauve,

        [Description("S_ACCENT_TAUPE")]
        Taupe,

        [Description("S_ACCENT_SIENNA")]
        Sienna
    }

    public enum LogLevel
    {
        NONE = 0,
        ERROR = 1,
        INFO = 2,
        DEBUG = 3
    }
}
