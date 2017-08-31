using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using _11thLauncher.Services.Contracts;
using _11thLauncher.Util;

namespace _11thLauncher.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly ILogger _logger;

        public SecurityService(ILogger logger)
        {
            _logger = logger;
        }

        public string EncryptPassword(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            try
            {
                byte[] original = Encoding.Unicode.GetBytes(text);
                byte[] encrypted = ProtectedData.Protect(original, GetEntropy(), DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(encrypted);
            }
            catch (Exception e)
            {
                _logger.LogException("SecurityService", "Error encrypting password", e);
                return string.Empty;
            }
        }

        public string DecryptPassword(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            try
            {
                byte[] encrypted = Convert.FromBase64String(text);
                byte[] original = ProtectedData.Unprotect(encrypted, GetEntropy(), DataProtectionScope.CurrentUser);
                return Encoding.Unicode.GetString(original);
            }
            catch (Exception e)
            {
                _logger.LogException("SecurityService", "Error decrypting password", e);
                return string.Empty;
            }
        }

        private static byte[] GetEntropy()
        {
            var assembly = typeof(Program).Assembly;
            var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            var guid = attribute.Value;

            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(guid);
            byte[] hash = sha512.ComputeHash(bytes);

            return hash;
        }
    }
}
