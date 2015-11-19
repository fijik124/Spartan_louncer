using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace _11thLauncher.AddonSync
{
    class Cryptography
    {
        public static string ComputeSha1(string path)
        {
            string hexHash;

            using (FileStream stream = File.OpenRead(path))
            {
                using (SHA1 sha1Hash = SHA1.Create())
                {
                    //Compute SHA1 hash
                    byte[] rawHash = sha1Hash.ComputeHash(stream);

                    //Convert hash to string
                    StringBuilder stringbuilder = new StringBuilder();
                    foreach (byte t in rawHash)
                    {
                        stringbuilder.Append(t.ToString("x2"));
                    }
                    hexHash = stringbuilder.ToString();
                }
            }

            return hexHash;
        }
    }
}
