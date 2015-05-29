using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _11thLauncher.LogViewer
{
    class TextPattern
    {
        private String pattern;
        public String Pattern
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

        public TextPattern(String pattern, Color color, Font font)
        {
            this.pattern = pattern;
            this.color = color;
            this.font = font;
        }

        public MatchCollection getMatches(String text)
        {
            Regex reg = new Regex(pattern, RegexOptions.IgnoreCase);
            return reg.Matches(text);
        }
    }
}
