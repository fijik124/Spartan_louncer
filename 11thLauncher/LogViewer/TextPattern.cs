using System.Drawing;
using System.Text.RegularExpressions;

namespace _11thLauncher.LogViewer
{
    class TextPattern
    {
        private string pattern;
        public string Pattern
        {
            get { return pattern; }
            set { pattern = value; }
        }

        private Color color;
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        private Font font;
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        public TextPattern(string pattern, Color color, Font font)
        {
            this.pattern = pattern;
            this.color = color;
            this.font = font;
        }

        public MatchCollection getMatches(string text)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            return reg.Matches(text);
        }
    }
}
