using Signature.Entity.Enum;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Signature.Entity
{
	internal class Person
	{
		public string Name { get; set; }
		public string DocumentNumber { get; set; }
		public PersonType PersonType { get; set; }

		// Conjunto de chaves da pessoa
		public string PublicKey { get; set; }

		private void CheckPersonType(string documentNumber)
		{
			if (documentNumber.Length == 11)
				this.PersonType = PersonType.Individual;
			else
				this.PersonType = PersonType.Company;
		}

		public void CreateKeys()
		{
			try
			{
				RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);

				byte[] publicKey = Encoding.UTF8.GetBytes(rsa.ToXmlString(false));
				byte[] privateKey = Encoding.UTF8.GetBytes(rsa.ToXmlString(true));

				this.PublicKey = Convert.ToBase64String(publicKey);

				string exportPrivateKey = Convert.ToBase64String(privateKey);
				
				FileStream fileStream = null;
				StreamWriter streamWriter = null;
				
				try
				{
					string fileName = this.Name.Replace(" ", "_");

					fileStream = new FileStream("./DataFiles/Person/Private Keys/" + fileName + ".pem", FileMode.Create, FileAccess.Write);
					streamWriter = new StreamWriter(fileStream);

					streamWriter.Write(exportPrivateKey);

					streamWriter.Close();
					fileStream.Close();
				}
				catch
				{
					throw new Exception($"Ocorreu um erro ao salvar a chave privada do usuário.");
				}
				
			}
			catch (Exception ex)
			{
				throw new Exception($"Ocorreu um erro ao criar o par de chaves do usuário.\nErro: {ex.Message}");
			}	
		}

		public override string ToString()
		{
			return $"--------------------------------------------------\n" +
				$"Nome: {this.Name}\nTipo: {this.PersonType.GetDisplayName()}\nCPF/CNPJ: {this.DocumentNumber}\nChave Pública: {this.PublicKey}" +
				$"\n--------------------------------------------------";
		}
	}
}
