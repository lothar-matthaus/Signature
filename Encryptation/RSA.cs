using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Signature.Encryptation
{
	static class RSA
	{

		public static bool CheckSignature(string stringKey, byte[] signedMessageContent, byte[] signature)
		{
			try
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
				SHA1Managed hash = new SHA1Managed();
				byte[] hashedData;

				stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

				rSACryptoServiceProvider.FromXmlString(stringKey);
				bool isValidData = rSACryptoServiceProvider.VerifyData(signedMessageContent, CryptoConfig.MapNameToOID("SHA1"), signature);
				hashedData = hash.ComputeHash(signedMessageContent);

				bool isValidSignature = rSACryptoServiceProvider.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), signature);

				if (isValidData && isValidSignature)
					return true;

				return false;
			}
			catch (Exception ex)
			{
				throw new Exception($"Erro ao checar a assinatura da mensagem.\nErro: {ex.Message}"); ;
			}
		}

		public static string Sign(string stringKey, byte[] message)
		{
			try
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
				SHA1Managed shaManaged = new SHA1Managed();

				stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

				rSACryptoServiceProvider.FromXmlString(stringKey);

				byte[] hashedData = shaManaged.ComputeHash(message);

				byte[] signedContent = rSACryptoServiceProvider.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));

				return Convert.ToBase64String(signedContent);
			}
			catch (Exception ex)
			{

				throw new Exception($"Erro ao Assinar a mensagem.\nErro: {ex.Message}");
			}
		}

		public static string EncryptMessage(string stringKey, byte[] messageToEncrypt)
		{
			try
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();

				stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

				rSACryptoServiceProvider.FromXmlString(stringKey);

				byte[] encryptedData = rSACryptoServiceProvider.Encrypt(messageToEncrypt, false);

				return Convert.ToBase64String(encryptedData);
			}
			catch (Exception ex)
			{
				throw new Exception($"Erro ao cifrar a mensagem.\nErro: {ex.Message}");
			}
		}

		public static string DecrypteMessage(string stringKey, byte[] encryptedMessage)
		{
			try
			{
				RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();

				stringKey = Encoding.UTF8.GetString(Convert.FromBase64String(stringKey));

				rSACryptoServiceProvider.FromXmlString(stringKey);

				byte[] decryptedData = rSACryptoServiceProvider.Decrypt(encryptedMessage, false);

				return Encoding.UTF8.GetString(decryptedData);
			}
			catch(Exception ex)
			{
				throw new Exception($"Erro ao decifrar a mensagem.\nErro: {ex.Message}");
			}
		}
	}
}
