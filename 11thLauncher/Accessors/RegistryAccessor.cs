using Microsoft.Win32;
using _11thLauncher.Accessors.Contracts;

namespace _11thLauncher.Accessors
{
    public class RegistryAccessor : IRegistryAccessor
    {
        public object GetValue(string keyName, string valueName, object defaultValue)
        {
            return Registry.GetValue(keyName, valueName, defaultValue);
        }

        public object GetKeyValue(RegistryKey key, string name, object defaultValue)
        {
            return key.GetValue(name, defaultValue);
        }

        public RegistryKey OpenBaseKey(RegistryHive hkey, RegistryView view)
        {
            return RegistryKey.OpenBaseKey(hkey, view);
        }

        public RegistryKey OpenSubKey(RegistryKey key, string name)
        {
            return key.OpenSubKey(name);
        }
    }
}
