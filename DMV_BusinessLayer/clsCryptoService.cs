using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_Buisness
{
    public class clsCryptoService
    {
        public static byte[] GenerateSalt()  // Generate a random salt (16 bytes = 128 bits)
        {

            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);

            }
            return salt;
        }
        //   
        public static byte[] HashPasswordAndSalt(string password, byte[] salt)
        // Hash the password using SHA256 and a salt
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            // combine password bytes + salt bytes
            byte[] combinedBytes = passwordBytes.Concat(salt).ToArray();

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(combinedBytes);
                return hashBytes;
                // return Convert.ToBase64String(hashBytes); // Store hash as Base64

            }
        }

        // Verify the password by comparing hashes
        public static bool VerifyPassword(byte[] inputPassword, byte[] StoredPassword)
        {


            return inputPassword.SequenceEqual(StoredPassword);
        }

        public static string GenerateToken(int length = 32)
        {

            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }
        private static readonly byte[] key = Encoding.UTF8.GetBytes("Key1234567890123"); // 32 bytes

        public static string Encrypt(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.GenerateIV(); // new random IV

                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // First, write the IV at the beginning of the memory stream
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    byte[] encryptedBytes = ms.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string Decrypt(string encryptedBase64)
        {
            byte[] cipherBytes = Convert.FromBase64String(encryptedBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;

                // Extract the IV (first 16 bytes)
                byte[] iv = new byte[16];
                Array.Copy(cipherBytes, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipherBytes, 16, cipherBytes.Length - 16))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

    }
}