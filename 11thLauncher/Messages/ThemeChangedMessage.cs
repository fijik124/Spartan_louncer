using _11thLauncher.Models;

namespace _11thLauncher.Messages
{
    public class ThemeChangedMessage
    {
        public ThemeStyle Theme;
        public AccentColor Accent;

        public ThemeChangedMessage(ThemeStyle theme, AccentColor accent)
        {
            Theme = theme;
            Accent = accent;
        }
    }
}
