using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace _11thLauncher.AddonSync
{
    class Cryptography
    {
        public static string computeSHA1(string path)
        {
            string hexHash;

            using (FileStream stream = File.OpenRead(path))
            {
                using (SHA1 sha1hash = SHA1.Create())
                {
                    //Compute SHA1 hash
                    byte[] rawHash = sha1hash.ComputeHash(stream);

                    //Convert hash to string
                    StringBuilder stringbuilder = new StringBuilder();
                    for (int i = 0; i < rawHash.Length; i++)
                    {
                        stringbuilder.Append(rawHash[i].ToString("x2"));
                    }
                    hexHash = stringbuilder.ToString();
                }
            }

            return hexHash;
        }
    }
}
