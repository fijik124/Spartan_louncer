using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace _11thLauncher.LogViewer
{
    static class Logger
    {
        //Text patterns
        public static List<TextPattern> patterns;

        //Pattern colors
        private static Color commentColor = Color.Green;
        private static Color stringColor = Color.Green;
        private static Color infoColor = Color.DarkGray;
        private static Color pathColor = Color.Black;
        private static Color methodColor = Color.Blue;
        private static Color attributeColor = Color.FromArgb(0, 69, 131, 131);
        private static Color warningColor = Color.OrangeRed;
        private static Color errorColor = Color.Red;

        //Pattern fonts
        private static Font defaultFont = new Font("Courier New", 9F, FontStyle.Regular);
        private static Font boldFont = new Font("Courier New", 9F, FontStyle.Bold);
        private static Font italicFont = new Font("Courier New", 9F, FontStyle.Italic);

        //Logs folder
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Arma 3";

        //Current file and last line
        //private static string file;
        private static int lastLine = -1;

        public static void init()
        {
            patterns = new List<TextPattern>();

            //Info
            patterns.Add(new TextPattern(@"\d+/\d+/\d+", infoColor, defaultFont)); //date
            patterns.Add(new TextPattern(@"\d+:\d+:\d+", infoColor, defaultFont)); //time
            patterns.Add(new TextPattern("by", infoColor, defaultFont));

            //File paths
            patterns.Add(new TextPattern(@"[A-Z,a-z]{1}:\\.*\.[a-z,A-Z]+", pathColor, italicFont));

            //Low level Warnings
            patterns.Add(new TextPattern("Attempt to override final function.*", warningColor, defaultFont));
            patterns.Add(new TextPattern("No owner", warningColor, defaultFont));
            patterns.Add(new TextPattern(@"Some of magazines weren't stored in soldier Vest or Uniform\?", warningColor, defaultFont));
            patterns.Add(new TextPattern(@"Unsupported language \w* in stringtable", warningColor, defaultFont));
            patterns.Add(new TextPattern(@"Item \w* listed twice", warningColor, defaultFont));

            //Errors
            patterns.Add(new TextPattern("Error:", errorColor, boldFont));
            patterns.Add(new TextPattern("Warning Message", errorColor, boldFont));
            patterns.Add(new TextPattern("Cannot register unknown string", errorColor, defaultFont));
            patterns.Add(new TextPattern("Unexpected stringtable format inside", errorColor, defaultFont));
            patterns.Add(new TextPattern(@"Cannot delete class \w+, it is referenced somewhere \(used as a base class probably\).", errorColor, defaultFont));
            patterns.Add(new TextPattern("Cannot update non class from class", errorColor, defaultFont));
            patterns.Add(new TextPattern("Addon .* requires addon .*", errorColor, defaultFont));

            //Strings
            patterns.Add(new TextPattern("\'[^\']*\'", stringColor, defaultFont)); //single quote
            patterns.Add(new TextPattern("\"[^\"]*\"", stringColor, defaultFont)); //double quote
            patterns.Add(new TextPattern(@"STR_\w*", commentColor, defaultFont)); //localization strings

            //Comments
            patterns.Add(new TextPattern("[=]{2,}", commentColor, defaultFont));

            //Methods
            patterns.Add(new TextPattern("Updating base class", methodColor, defaultFont));
            patterns.Add(new TextPattern("Deinitialized shape", methodColor, defaultFont));
            patterns.Add(new TextPattern(@"\w+\(\)", methodColor, defaultFont));

            //Classes/Attributes
            patterns.Add(new TextPattern("Class:", attributeColor, defaultFont));
            patterns.Add(new TextPattern("Shape:", attributeColor, defaultFont));
            patterns.Add(new TextPattern(@"\w*->\w*", attributeColor, defaultFont));
            patterns.Add(new TextPattern(@"\[\w*\]", attributeColor, defaultFont));
            patterns.Add(new TextPattern(@"\w*\\.*\/{1}", attributeColor, defaultFont));
        }

        public static string[] readfile()
        {
            string[] lines = ReadAllLines(getLatestFile().FullName, Encoding.Default, FileShare.ReadWrite);

            lastLine = lines.Length;

            return lines;
        }

        public static string[] readNewLines()
        {
            string[] result = new string[] { };
            try
            {
                string[] lines = ReadAllLines(getLatestFile().FullName, Encoding.Default, FileShare.ReadWrite);

                result = new string[lines.Length - lastLine];
                Array.Copy(lines, lastLine, result, 0, lines.Length - lastLine);

                lastLine = lines.Length;

                return result;
            }
            catch (OverflowException) { }

            return result;
        }

        public static FileInfo getLatestFile()
        {
            FileInfo result = null;

            List<FileInfo> files = new DirectoryInfo(path).GetFiles().OrderBy(f => f.LastWriteTime).ToList();

            if (files.Count != 0)
            {
                result = files.Last();
            }

            return result;
        }

        private static string[] ReadAllLines(string file, Encoding encoding, FileShare share)
        {
            using (FileStream stream = File.Open(file, FileMode.Open, FileAccess.Read, share))
            {
                string line;
                List<string> lines = new List<string>();

                using (StreamReader sr = new StreamReader(stream, encoding))
                    while ((line = sr.ReadLine()) != null)
                        lines.Add(line);

                return lines.ToArray();
            }
        }
    }
}
