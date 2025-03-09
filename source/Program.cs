using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;

namespace TestEnvelopeCms
{
    internal class Program
    {        
        static void Main(string[] args)
        {
            string inputFile = "D:\\temp\\test.txt.enc"; // my envelopedCmd file created by another utility
            byte[] baEncryptedEnvelopedMessage = File.ReadAllBytes(inputFile);
            DecodeDirect(baEncryptedEnvelopedMessage);
        }

        public static byte[] DecodeDirect(byte[] encryptedMessage)
        {
            string aesEncrytionKeyFile = "D:\\temp\\decryptionkey.bin";
            EnvelopedCms envelopedCms = new EnvelopedCms();
            envelopedCms.Decode(encryptedMessage);

            byte[] aesKey;
            byte[] baEncKey = envelopedCms.RecipientInfos[0].EncryptedKey;

            aesKey = File.ReadAllBytes(aesEncrytionKeyFile);

            byte[] iv;
            byte[] baEncContentOnly;
            byte[] baEncContentPlusIV = envelopedCms.ContentInfo.Content;

            ExtractEncryptedContentAndIV(baEncContentPlusIV, aesKey, out iv, out baEncContentOnly);

            byte[] content = DecryptAes(baEncContentOnly, aesKey, iv); // this is not matching the original unecrypted bytes. This one has length: 415 whereas original content has length: 431

            string decryptedTextContent = Encoding.Default.GetString(content); // for debugging, this is not matching

            return content;
        }

        private static void ExtractEncryptedContentAndIV(byte[] encryptedContent, byte[] aesKey, out byte[] iv, out byte[] actualCipherText)
        {
            int ivLength = 16;
            iv = new byte[ivLength];
            Buffer.BlockCopy(encryptedContent, 0, iv, 0, ivLength);

            actualCipherText = new byte[encryptedContent.Length - ivLength];
            Buffer.BlockCopy(encryptedContent, ivLength, actualCipherText, 0, actualCipherText.Length);
        }

        private static byte[] DecryptAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor())
                {
                    return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                }
            }
        }
    }
}
