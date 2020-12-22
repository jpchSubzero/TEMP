using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using OtpNet;

namespace satelite.Controllers
{
    public class ManagedAes
    {

        protected AesManaged myRijndael;
        private string encryptionKey;
        private static byte[] salt = new byte[] { 172, 137, 25, 56, 156, 100, 136, 211, 84, 67, 96, 10, 24, 111, 112, 137, 3 };
        //private static byte[] initialisationVector = new byte[16];
        private string initialisationVector;

        //private static byte[] initialisationVector = Encoding.UTF8.GetBytes("l4iG63jN9Dcg6537");
        private static byte[] secretKey = GetSecretKey("cjcxzcxz");

        // Singleton pattern used here with ensured thread safety
        protected static readonly ManagedAes _instance = new ManagedAes();


        public static ManagedAes Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Constructor vacio
        /// </summary>
        public ManagedAes()
        {

        }

        /// <summary>
        /// Contructor con datos
        /// </summary>
        /// <param name="vector">Verto de codificacion</param>
        /// <param name="key">Clave de encriptacion</param>
        public ManagedAes(string vector, string key)
        {
            this.initialisationVector = vector;
            this.encryptionKey = key;
        }



        /// <summary>
        /// Metodo para la decodificacion usando algorito AES y formato 32
        /// </summary>
        /// <param name="encryptedString">Cadena codificada</param>
        /// <returns>Cadena decodificada</returns>
        public string DecryptText(string encryptedString)
        {
            using (myRijndael = new AesManaged())
            {
                Byte[] ourEnc;

                if (AbstManagerConfig.GetCodificationMode64() == "SI")
                {
                    myRijndael.Key = Convert.FromBase64String(encryptionKey);
                    myRijndael.IV = Convert.FromBase64String(initialisationVector);//new byte[16];

                    myRijndael.Mode = CipherMode.CBC;
                    myRijndael.Padding = PaddingMode.PKCS7;
                    
                    ourEnc = Convert.FromBase64String(encryptedString);
                }
                else
                {
                    myRijndael.Key = Base32Encoding.ToBytes(encryptionKey);
                    myRijndael.IV = Base32Encoding.ToBytes(initialisationVector);

                    myRijndael.Mode = CipherMode.CBC;
                    myRijndael.Padding = PaddingMode.PKCS7;

                    ourEnc = Base32Encoding.ToBytes(encryptedString);
                }

                string ourDec = DecryptStringFromBytes(ourEnc, myRijndael.Key, myRijndael.IV);

                return ourDec;
            }
        }


        public string EncryptText(string plainText, string initialisationVector)
        {
            using (myRijndael = new AesManaged())
            {

                myRijndael.Key = secretKey;
                myRijndael.IV = Convert.FromBase64String(initialisationVector);
                myRijndael.Mode = CipherMode.CBC;
                myRijndael.Padding = PaddingMode.PKCS7;

                byte[] encrypted = EncryptStringToBytes(plainText, myRijndael.Key, myRijndael.IV);
                string encString = Convert.ToBase64String(encrypted);

                return encString;
            }
        }


        protected byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }
        protected string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments. 
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = null;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream 
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }

        public static void GenerateKeyAndIV()
        {
            // This code is only here for an example
            AesManaged myRijndaelManaged = new AesManaged();
            myRijndaelManaged.Mode = CipherMode.CBC;
            myRijndaelManaged.Padding = PaddingMode.PKCS7;

            myRijndaelManaged.GenerateIV();
            myRijndaelManaged.GenerateKey();
            string newKey = ByteArrayToHexString(myRijndaelManaged.Key);
            string newinitVector = ByteArrayToHexString(myRijndaelManaged.IV);
        }

        protected static byte[] HexStringToByte(string hexString)
        {
            try
            {
                int bytesCount = (hexString.Length) / 2;
                byte[] bytes = new byte[bytesCount];
                for (int x = 0; x < bytesCount; ++x)
                {
                    bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
                }
                return bytes;
            }
            catch
            {
                throw;
            }
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private static byte[] GetSecretKey(string encryptionKey)
        {
            string hashedKey = GetHashedKey(encryptionKey);
            byte[] saltBytes = salt;//Encoding.UTF8.GetBytes(salt); // builder.mCharsetName = "UTF8";
            int iterations = 1; // builder.mIterationCount = 1
            byte[] secretKey = null;
            using (Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(hashedKey, saltBytes, iterations)) // builder.mSecretKeyType = "PBKDF2WithHmacSHA1";
            {
                secretKey = rfc2898.GetBytes(16); // builder.mKeyLength = 128;
                                                  //Console.WriteLine("Key: " + ByteArrayToHexString(secretKey));
            }
            return secretKey;
        }

        private static string GetHashedKey(string encryptionKey)
        {
            string hashBase64 = String.Empty;
            using (SHA1Managed sha1 = new SHA1Managed()) // builder.mDigestAlgorithm = "SHA1";
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey)); // builder.mCharsetName = "UTF8";

                hashBase64 = Base64ThirdPartAndroid(hash, true);
                //hashBase64 = Base64ThirdPartAndroid(hash, true);
                //Console.WriteLine("Hash (base64): " + hashBase64);
            }
            return hashBase64;
        }

        private static string Base64ThirdPartAndroid(byte[] arr, bool withoutPadding)
        {
            string base64String = System.Convert.ToBase64String(arr);
            if (withoutPadding) base64String = base64String.TrimEnd('='); // Remove trailing "="-characters
            base64String += "\n"; // Append LF (10)
                                  //Console.WriteLine("Array as base64 encoded string: " + base64String);
            return base64String;
        }

        static void EncryptAesManaged()
        {
            try
            {
                // Create Aes that generates a new key and initialization vector (IV).    
                // Same key must be used in encryption and decryption    
                //using (AesManaged aes = new AesManaged())
                //{
                // Encrypt string    
                //byte[] encrypted = Encrypt(raw, aes.Key, aes.IV);
                //byte[] encrypted = Encoding.ASCII.GetBytes(raw);
                // Print encrypted string    
                Console.WriteLine($"Encrypted data: ");
                // Decrypt the bytes to a string.   
                //string decrypted = Decrypt(encrypted, aesKye, aesIV);
                string decrypted = decryptAes();
                // Print decrypted string. It should be same as raw data    
                Console.WriteLine($"Decrypted data: {decrypted}");
                //}
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
            Console.ReadKey();
        }

        public static string Desencriptar(string textoEncriptado)
        {
            try
            {
                string key = "qualityinfosolutions";
                byte[] keyArray;
                byte[] Array_a_Descifrar = Convert.FromBase64String(textoEncriptado);

                //algoritmo MD5
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();

                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));

                hashmd5.Clear();

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();

                tdes.Key = keyArray;
                //tdes.Mode = CipherMode.ECB;
                //tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();

                byte[] resultArray = cTransform.TransformFinalBlock(Array_a_Descifrar, 0, Array_a_Descifrar.Length);

                tdes.Clear();
                textoEncriptado = UTF8Encoding.UTF8.GetString(resultArray);

            }
            catch (Exception)
            {

            }
            return textoEncriptado;
        }
        

        public static string decryptAes()
        {
            //byte[] salt = Encoding.UTF8.GetBytes("cdWSu23E9BLbNXWUTnrznFgc");
            byte[] salt = new byte[] { 172, 137, 25, 56, 156, 100, 136, 211, 84, 67, 96, 10, 24, 111, 112, 137, 3 };
            int iterations = 128;
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes("gygp6yevKWKBUwFy4GXpuFwT", salt, iterations);
            byte[] key = rfc2898.GetBytes(16);


            AesManaged aesCipher = new AesManaged();
            aesCipher.KeySize = 256;
            aesCipher.BlockSize = 128;
            aesCipher.Mode = CipherMode.CBC;
            aesCipher.Padding = PaddingMode.PKCS7;
            aesCipher.Key = key;

            String cipherB64 = "0kg0kIL0A+e1Sw+HPnsaYw==";
            String ivB64 = "v/JzYejNwPAwgPqbwjoV0A==";
            byte[] cipherText = Convert.FromBase64String(cipherB64);
            aesCipher.IV = Convert.FromBase64String(ivB64);

            byte[] plainText = null;
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aesCipher.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherText, 0, cipherText.Length);
                }

                plainText = ms.ToArray();
            }
            return System.Text.Encoding.Unicode.GetString(plainText);
        }


        static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new AesManaged.    
            using (AesManaged aes = new AesManaged())
            {
                // Create encryptor    
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }

        static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }





    }
}
 