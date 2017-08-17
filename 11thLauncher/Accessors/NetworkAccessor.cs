using System;
using System.Net;
using System.Reflection;
using _11thLauncher.Accessors.Contracts;

namespace _11thLauncher.Accessors
{
    public class NetworkAccessor : INetworkAccessor
    {
        public NetworkAccessor()
        {
            SetAllowUnsafeHeaderParsing();
        }

        public WebResponse GetWebResponse(WebRequest request)
        {
            return request.GetResponse();
        }

        public string DownloadString(WebClient webClient, string uri)
        {
            return webClient.DownloadString(uri);
        }

        //Set allow unsafe header parsing to ignore server protocol violations 
        private static void SetAllowUnsafeHeaderParsing()
        {
            //Get the assembly that contains the internal class
            Assembly aNetAssembly = Assembly.GetAssembly(typeof(System.Net.Configuration.SettingsSection));
            if (aNetAssembly == null) return;

            //Use the assembly in order to get the internal type for the internal class
            Type aSettingsType = aNetAssembly.GetType("System.Net.Configuration.SettingsSectionInternal");
            if (aSettingsType == null) return;

            //Use the internal static property to get an instance of the internal settings class.
            //If the static instance isn't created allready the property will create it for us.
            object anInstance = aSettingsType.InvokeMember("Section",
                BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
            if (anInstance == null) return;

            //Locate the private bool field that tells the framework is unsafe header parsing should be allowed or not
            FieldInfo aUseUnsafeHeaderParsing = aSettingsType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
            if (aUseUnsafeHeaderParsing != null)
            {
                aUseUnsafeHeaderParsing.SetValue(anInstance, true);
            }
        }
    }
}
