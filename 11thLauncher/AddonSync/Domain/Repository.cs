using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using _11thLauncher.Configuration;

namespace _11thLauncher.AddonSync.Domain
{
    class Repository
    {
        public string SchemaVersion { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public DateTime UpdateTime { get; set; }
        public string Changelog { get; set; }
        public HashMethod Method { get; set; }
        public Compression Compression { get; set; }
        public List<File> Files { get; set; }

        public void ReadLocalRepository()
        {
            string[] files = Directory.GetFiles(Settings.Arma3Path, "*.*", SearchOption.AllDirectories);
            Console.Write("");
        }
    }
}
