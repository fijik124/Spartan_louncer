using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _11thLauncher
{
    class Version
    {
        //Version storage location
        private static readonly String versionURL = "http://raw.githubusercontent.com/11thmeu/launcher/master/bin/version";
        private static readonly String downloadBaseURL = "https://raw.githubusercontent.com/11thmeu/launcher/master/bin/";

        //Current version info
        private static readonly String currentBuildType = "dev";
        private static readonly String currentVersion = "121_dev30052015";

        //Latest version info
        public static String latestVersion = "";

        public static void checkVersion()
        {
            try
            {
                WebClient client = new WebClient();
                using (Stream stream = client.OpenRead(versionURL))
                using (StreamReader reader = new StreamReader(stream))
                {
                    String versionRaw = reader.ReadToEnd();
                    String[] versionData = versionRaw.Split('\n');
                    if (currentBuildType.Equals("stable"))
                    {
                        latestVersion = versionData[1];
                    }
                    else
                    {
                        latestVersion = versionData[3];
                    }
                }

                if (latestVersion != currentVersion && Program.form != null)
                {
                    Program.form.showUpdateNotification();
                }
            }
            catch (Exception) {}
        }

        public static String getLatestDownload()
        {
            return downloadBaseURL + "11thLauncher" + latestVersion + ".zip";
        }
    }
}