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
        //Logs folder
        private static readonly string Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Arma 3";

        //Current file and last line
        //private static string file;
        private static int _lastLine = -1;

        public static string[] Readfile()
        {
            string[] lines = ReadAllLines(GetLatestFile().FullName, Encoding.Default, FileShare.ReadWrite);

            _lastLine = lines.Length;

            return lines;
        }

        public static string[] ReadNewLines()
        {
            string[] result = { };
            try
            {
                string[] lines = ReadAllLines(GetLatestFile().FullName, Encoding.Default, FileShare.ReadWrite);

                result = new string[lines.Length - _lastLine];
                Array.Copy(lines, _lastLine, result, 0, lines.Length - _lastLine);

                _lastLine = lines.Length;

                return result;
            }
            catch (OverflowException) { }

            return result;
        }

        public static FileInfo GetLatestFile()
        {
            FileInfo result = null;

            List<FileInfo> files = new DirectoryInfo(Path).GetFiles().OrderBy(f => f.LastWriteTime).ToList();

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
