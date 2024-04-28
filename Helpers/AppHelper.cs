using System.Security.Cryptography;
using System.Text;

namespace ReportService.Helpers
{
    public class AppHelper
    {
        public string GetNewGuid()
        {
            var guid = Guid.NewGuid();
            return guid.ToString();
        }
        public string GetNewUniqueId()
        {
            return DateTime.Now.ToFileTime().ToString();
        }
        public string GetRandomNumber()
        {
            try
            {
                var rNum = new Random();
                var sCode = rNum.Next(1000, 9999).ToString();
                return sCode;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        public static string GetUnique8ByteKey()
        {
            try
            {
                var maxSize = 10;
                var chars = new char[62];
                var a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
                chars = a.ToCharArray();
                var size = maxSize;
                var data = new byte[1];
                var crypto = new RNGCryptoServiceProvider();
                crypto.GetNonZeroBytes(data);
                size = maxSize;
                data = new byte[size];
                crypto.GetNonZeroBytes(data);
                var result = new StringBuilder(size);
                foreach (var b in data)
                { result.Append(chars[b % (chars.Length - 1)]); }
                return result.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        public static string GetUniqueKeyOriginal_BIASED(int size)
        {
            var chars =
                "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            byte[] data = new byte[size];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            var result = new StringBuilder(size);
            foreach (var b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }



        public static string Encrypt(string encryptString , string key)
        {
            var clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(key, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(clearBytes, 0, clearBytes.Length);
                cs.Close();
            }
            encryptString = Convert.ToBase64String(ms.ToArray());
            return encryptString;
        }


        public static string Decrypt(string cipherText, string key)
        {
            
            cipherText = cipherText.Replace(" ", "+");
            var cipherBytes = Convert.FromBase64String(cipherText);
            using var encryptor = Aes.Create();
            var pdb = new Rfc2898DeriveBytes(key, new byte[] {
                0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
            });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(cipherBytes, 0, cipherBytes.Length);
                cs.Close();
            }
            cipherText = Encoding.Unicode.GetString(ms.ToArray());

            return cipherText;
        }


    }
}
