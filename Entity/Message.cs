using Signature.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Signature.Entity
{
	public class Message
	{
		public int Id { get; set; }
		public string Content { get; set; }
		public string Signature { get; set; }
		public DateTime CreationDate { get; set; }
		public KeyValuePair<string, string> FromPerson { get; set; }
		public KeyValuePair<string, string> ToPerson { get; set; }
		public MessageType MessageType { get; set; }

		public Message()
		{
			this.Id = new Random().Next(100000, 999999);
			this.CreationDate = DateTime.Now;
		}

		public override string ToString()
		{
			return "=============================\n" +
				  $"Id: {this.Id}\nTipo: {this.MessageType.GetDisplayName()}\nRemetente: {this.FromPerson.Value}\nDestinatário: {this.ToPerson.Value}\nMensagem: {this.Content}\nData de Envio: {this.CreationDate}" +
				   "\n=============================\n";
		}
	}
}
