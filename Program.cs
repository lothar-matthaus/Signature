using Signature.Encryptation;
using Signature.Entity;
using Signature.Entity.Enum;
using Signature.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signature
{
	internal class Program
	{

		static private Message SendSignedMessage()
		{
			Message message = new Message();
			Person fromPerson, toPerson;

			try
			{
				Console.WriteLine("Indique o remetente e destinatário.");

				Console.Write("CPF/CNPJ do Remetente: ");
				string personDocument = Console.ReadLine();

				fromPerson = PersonRepository.Get(personDocument);

				while (fromPerson == null)
				{
					Console.WriteLine("Pessoa não existe. Pressione uma tecla para continuar.");
					Console.ReadKey();

					Console.Write("CPF/CNPJ do Remetente: ");
					personDocument = Console.ReadLine();

					fromPerson = PersonRepository.Get(personDocument);
				}

				Console.Write("CPF/CNPJ do Destinatário: ");
				personDocument = Console.ReadLine();

				toPerson = PersonRepository.Get(personDocument);

				while (toPerson == null)
				{
					Console.WriteLine("Pessoa não existe. Pressione uma tecla para continuar.");
					Console.ReadKey();

					Console.Write("CPF/CNPJ do Destinatário: ");
					personDocument = Console.ReadLine();

					toPerson = PersonRepository.Get(personDocument);
				}

				Console.Write("Mensage: ");
				string messageContent = Console.ReadLine();

				message.FromPerson = new KeyValuePair<string, string>(fromPerson.DocumentNumber, fromPerson.Name);
				message.ToPerson = new KeyValuePair<string, string>(toPerson.DocumentNumber, toPerson.Name);
				message.Content = RSA.EncryptMessage(toPerson.PublicKey, Encoding.UTF8.GetBytes(messageContent));
				message.Signature = RSA.Sign(fromPerson.PrivateKey, Convert.FromBase64String(message.Content));
				message.MessageType = MessageType.Signed;

				return message;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		static private Message SendUnsignedMessage()
		{
			Message message = new Message();
			Person fromPerson, toPerson;

			try
			{
				Console.WriteLine("Indique o remetente e destinatário.");

				Console.Write("CPF/CNPJ do Remetente: ");
				string personDocument = Console.ReadLine();

				fromPerson = PersonRepository.Get(personDocument);

				while (fromPerson == null)
				{
					Console.WriteLine("Pessoa não existe. Pressione uma tecla para continuar.");
					Console.ReadKey();

					Console.Write("CPF/CNPJ do Remetente: ");
					personDocument = Console.ReadLine();

					fromPerson = PersonRepository.Get(personDocument);
				}

				Console.Write("CPF/CNPJ do Destinatário: ");
				personDocument = Console.ReadLine();

				toPerson = PersonRepository.Get(personDocument);

				while (toPerson == null)
				{
					Console.WriteLine("Pessoa não existe. Pressione uma tecla para continuar.");
					Console.ReadKey();

					Console.Write("CPF/CNPJ do Destinatário: ");
					personDocument = Console.ReadLine();

					toPerson = PersonRepository.Get(personDocument);
				}

				Console.Write("Mensage: ");
				string messageContent = Console.ReadLine();

				message.FromPerson = new KeyValuePair<string, string>( fromPerson.DocumentNumber, fromPerson.Name);
				message.ToPerson = new KeyValuePair<string, string>(toPerson.DocumentNumber, toPerson.Name);
				message.Content = RSA.EncryptMessage(toPerson.PublicKey, Encoding.UTF8.GetBytes(messageContent));
				message.Signature = String.Empty;
				message.MessageType = MessageType.Unsigned;

				return message;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		static void SendMessage()
		{
			int opc = 0;
			Console.WriteLine("------------------------------\n" +
							  $"-1 - {MessageType.Signed.GetDisplayName()}\n-2 - {MessageType.Unsigned.GetDisplayName()}");

			Console.Write("Opção: ");
			try
			{
				opc = int.Parse(Console.ReadLine());

				switch (opc)
				{
					case 1:
					{
						Message message = SendSignedMessage();
						MessageRepository.Save(message);
						break;
					}
					case 2:
					{
						Message message = SendUnsignedMessage();
						MessageRepository.Save(message);
						break;
					}
					default:
					{
						Console.WriteLine("Opção inválida...");
						break;
					}


				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}

		}

		private static void ProcessMessage(Message message)
		{
			if (message.MessageType == MessageType.Signed)
			{
				try
				{
					Console.Clear();

					ShowAllPersons();

					Console.Write("Digite a chave pública do remetente:");
					string publicKey = Console.ReadLine();

					bool isValidSignature = RSA.CheckSignature(publicKey, Convert.FromBase64String(message.Content), Convert.FromBase64String(message.Signature));

					if (isValidSignature)
					{
						Console.Clear();

						Console.WriteLine("A chave pública é válida.\nPressione uma tecla para ver a mensagem decifrada.");
						Console.ReadKey();

						string privateKey = PersonRepository.Get(message.ToPerson.Key).PrivateKey;

						string decodedMessage = RSA.DecrypteMessage(privateKey, Convert.FromBase64String(message.Content));

						Console.Clear();
						Console.WriteLine("Mensagem decodificada: {0}", decodedMessage + "\nPressione uma tecla para continuar...");
						Console.ReadKey();
						Console.Clear();
					}
					else
					{
						Console.Clear();
						Console.WriteLine("A assinatura é inválida.\nPressione uma tecla para continuar...");
						Console.ReadKey();
						Console.Clear();
						return;
					}
				}
				catch (Exception)
				{

					throw;
				}
			}
			else
			{
				try
				{
					string privateKey = PersonRepository.Get(message.ToPerson.Key).PrivateKey;
					string decodedMessage = RSA.DecrypteMessage(privateKey, Convert.FromBase64String(message.Content));

					Console.Clear();
					Console.WriteLine("Mensagem decodificada: {0}", decodedMessage + "\nPressione uma tecla para continuar...");
					Console.ReadKey();
					Console.Clear();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message + "\nPressione uma tecla para continuar...");
					Console.ReadKey();
					Console.Clear();
				}
			}
		}

		static void ShowMessages()
		{
			List<Message> messages = MessageRepository.Get();

			string document = "";

			Console.WriteLine("Entrar como: (CPF/CNPJ)");
			document = Console.ReadLine();

			Person person = PersonRepository.Get(document);

			if (person == null)
			{
				Console.WriteLine("Pessoa não existe.");
				return;
			}

			messages.RemoveAll(message => !message.ToPerson.Key.Equals(person.DocumentNumber));

			if (messages == null || messages.Count == 0)
			{
				Console.WriteLine("Não há mensagens para mostrar.");
				return;
			}

			messages.ForEach(message =>
			{
				Console.WriteLine(message);
			});

			Console.Write("ID da Mensagem: ");
			int id = int.Parse(Console.ReadLine());

			Message message = messages.Find(message => message.Id == id);

			while (message == null)
			{
				Console.Write("ID da Mensagem: ");
				id = int.Parse(Console.ReadLine());

				message = messages.Find(message => message.Id == id);
			}

			ProcessMessage(message);
		}

		static void ShowAllPersons()
		{
			List<Person> personList = PersonRepository.Get();

			Console.Clear();

			personList.ForEach(person =>
			{
				Console.WriteLine(person);
			});
		}

		static void CreatePerson()
		{
			Person person = new Person();

			try
			{
				Console.Write("Nome: ");
				person.Name = Console.ReadLine();

				Console.Write("CPF/CNPJ: ");
				person.DocumentNumber = Console.ReadLine();

				while (!person.CheckIfDocumentIsValid(person.DocumentNumber))
				{
					Console.Write("O documento digitado não é válido.\n");
					Console.Write("CPF/CNPJ: ");
					person.DocumentNumber = Console.ReadLine();
				}

				PersonRepository.Save(person);
			}
			catch (Exception ex)
			{
				Console.Write("Erro ao criar pessoa. {0}", ex.Message);
				Console.ReadKey();
				Menu();
			}
		}

		static void Menu()
		{
			int opc = 0;
			Console.Write("--------------------------------\n" +
				"-1 - Criar Pessoa\n-2 - Ver pessoas\n-3 - Enviar Mensagem\n-4 - Ver mensagens\n-5 - Sair\n" +
				"--------------------------------\nOpção: ");
			try
			{
				if (Console.KeyAvailable)
					Console.ReadKey(true);

				opc = int.Parse(Console.ReadLine());

				switch (opc)
				{
					case 1:
					{
						Console.Clear();
						CreatePerson();
						Console.Clear();
						Menu();
						break;
					}
					case 2:
					{
						Console.Clear();
						ShowAllPersons();
						Console.WriteLine("Pressione uma tecla para continuar...");
						Console.ReadKey();
						Console.Clear();
						Menu();
						break;
					}
					case 3:
					{
						Console.Clear();
						SendMessage();
						Console.WriteLine("Pressione uma tecla para continuar...");
						Console.ReadKey();
						Console.Clear();
						Menu();
						break;
					}
					case 4:
					{
						Console.Clear();
						ShowMessages();
						Console.WriteLine("Pressione uma tecla para continuar...");
						Console.ReadKey();
						Console.Clear();
						Menu();
						break;
					}
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				Console.Clear();
				Console.WriteLine(ex.Message);
				Console.WriteLine("Pressione uma tecla para continuar...");
				Console.ReadKey();
				Console.Clear();
				Menu();
			}

		}

		static void Main(string[] args)
		{
			PersonRepository.InitializePersonRepository();
			MessageRepository.InitializeMessageRepository();
			Menu();
			
			//Message message = new Message();

			//message.FromPerson = lothar.DocumentNumber;
			//message.ToPerson = mateus.DocumentNumber;
			//message.Content = RSA.EncryptMessage(lothar.PrivateKey, Encoding.UTF8.GetBytes(messageContent));
			//message.Signature = RSA.Sign(lothar.PrivateKey, Convert.FromBase64String(message.Content));

			//bool isValidSignature = RSA.CheckSignature(lothar.PrivateKey, Convert.FromBase64String(message.Content), Convert.FromBase64String(message.Signature));

			//Console.WriteLine($"Mensagem original: {messageContent}");

			//Console.WriteLine($"Mensagem cifrada: {message.Content}");

			//string decryptedMessage = RSA.DecrypteMessage(lothar.PublicKey, Convert.FromBase64String(message.Content));

			// Console.WriteLine($"Mensagem decifrada com a chave pública do remetente: {decryptedMessage}");
		}
	}
}
