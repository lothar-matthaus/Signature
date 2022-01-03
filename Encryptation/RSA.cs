using Signature.Entity.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Signature.Encryptation {
    static class RSA {
        private static int MESSAGE_SIZE = 2048;

        public static bool ValidateKey(string stringKey, KeyType keyType) {
            try {
                byte[] key = Convert.FromBase64String(stringKey);
                stringKey = Encoding.UTF8.GetString(key);

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

                rsa.FromXmlString(stringKey);

                return true;
            }
            catch {
                throw new Exception($"Chave {keyType.GetDisplayName()} inválida. ");
            }
        }

        public static bool CheckSignature(string stringKey, byte[] signedMessageContent, byte[] signature) {
            try {
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(MESSAGE_SIZE);
                SHA1Managed hash = new SHA1Managed();
                byte[] hashedData;

                stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

                rSACryptoServiceProvider.FromXmlString(stringKey);

                bool isValidData = rSACryptoServiceProvider.VerifyData(signedMessageContent, CryptoConfig.MapNameToOID("SHA1"), signature);

                hashedData = hash.ComputeHash(signedMessageContent);

                bool isValidSignature = rSACryptoServiceProvider.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), signature);

                if (isValidData && isValidSignature) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Clear();
                    Console.Write("Assinatura verificado com sucesso. ");
                    return true;
                }
                else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Clear();
                    Console.Write("Assinatura inválida. Não foi possível verificar a autenticidade da mensagem. ");
                    return false;
                }
            }
            catch (Exception ex) {
                throw new Exception($"Erro ao verificar a assinatura da mensagem selecionada.\n Erro: {ex.Message}");
            }
        }

        public static string Sign(string stringKey, byte[] message) {
            RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(MESSAGE_SIZE);
            SHA1Managed shaManaged = new SHA1Managed();

            try {
                stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

                rSACryptoServiceProvider.FromXmlString(stringKey);

                byte[] hashedData = shaManaged.ComputeHash(message);

                byte[] signedContent = rSACryptoServiceProvider.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));

                return Convert.ToBase64String(signedContent);
            }
            catch (Exception ex) {

                throw new Exception($"Erro ao Assinar a mensagem.\nErro: {ex.Message}");
            }
        }

        public static string EncryptMessage(string stringKey, byte[] messageToEncrypt) {
            try {
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(MESSAGE_SIZE);

                stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

                rSACryptoServiceProvider.FromXmlString(stringKey);

                byte[] encryptedData = rSACryptoServiceProvider.Encrypt(messageToEncrypt, false);

                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex) {
                throw new Exception($"Erro ao cifrar a mensagem.\nErro: {ex.Message}");
            }
        }

        public static string DecrypteMessage(string stringKey, byte[] encryptedMessage) {
            try {
                RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider(MESSAGE_SIZE);

                stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

                rSACryptoServiceProvider.FromXmlString(stringKey);

                byte[] decryptedData = rSACryptoServiceProvider.Decrypt(encryptedMessage, false);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex) {
                throw new Exception($"Erro ao decifrar a mensagem.\nErro: {ex.Message}");
            }
        }
    }
}
