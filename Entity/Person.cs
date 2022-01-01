using Signature.Entity.Enum;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace Signature.Entity
{
	internal class Person
	{
		public string Name { get; set; }
		public string DocumentNumber { get; set; }
		public PersonType PersonType { get; set; }

		// Conjunto de chaves da pessoa
		public string PrivateKey { get; set; }
		public string PublicKey { get; set; }

		public Person()
		{
			CreateKeys();
		}


		private void CheckPersonType(string documentNumber)
		{
			if (documentNumber.Length == 11)
				this.PersonType = PersonType.Individual;
			else
				this.PersonType = PersonType.Company;

		}

		public bool CheckIfDocumentIsValid(string documentNumber)
		{
			if (documentNumber.Length != 11)
			{
				if (documentNumber.Length != 14)
					return false;
			}
			else
			{
				CheckPersonType(documentNumber);
				return true;
			}

			CheckPersonType(documentNumber);
			return true;
		}

		private void CreateKeys()
		{
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

			byte[] publicKey = Encoding.UTF8.GetBytes(rsa.ToXmlString(false));
			byte[] privateKey = Encoding.UTF8.GetBytes(rsa.ToXmlString(true));

			this.PrivateKey = Convert.ToBase64String(privateKey);
			this.PublicKey = Convert.ToBase64String(publicKey);
		}

		public override string ToString()
		{
			return $"--------------------------------------------------\n" +
				$"Nome {this.Name}\nTipo: {this.PersonType.GetDisplayName()}\nCPF/CNPJ: {this.DocumentNumber}\nChave Pública: {this.PublicKey}" +
				$"\n--------------------------------------------------";
		}
	}
}
