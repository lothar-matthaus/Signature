using Signature.Encryptation;
using Signature.Entity;
using Signature.Entity.Enum;
using Signature.Repository;
using Signature.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Signature {
    internal class Program {

        static private void SendMessage(ref Message message) {
            Person fromPerson, toPerson;
            Console.WriteLine("----- Enviar nova mensagem -----");

            try {
                Console.WriteLine("Indique o remetente e destinatário.");

                Console.Write("CPF/CNPJ do Remetente: ");
                string personDocument = Console.ReadLine();

                fromPerson = PersonRepository.Get(personDocument);

                while (fromPerson == null) {
                    Pause("A pessoa não existe.");

                    Console.Write("CPF/CNPJ do Remetente: ");
                    personDocument = Console.ReadLine();

                    fromPerson = PersonRepository.Get(personDocument);
                }

                Console.Write("CPF/CNPJ do Destinatário: ");
                personDocument = Console.ReadLine();

                toPerson = PersonRepository.Get(personDocument);

                while (toPerson == null) {
                    Pause("A pessoa não existe.");

                    Console.Write("CPF/CNPJ do Destinatário: ");
                    personDocument = Console.ReadLine();

                    toPerson = PersonRepository.Get(personDocument);
                }

                Console.Write("Mensagem: ");
                string messageContent = Console.ReadLine();

                if (message.MessageType == MessageType.Signed) {
                    message.FromPerson = new KeyValuePair<string, string>(fromPerson.DocumentNumber, fromPerson.Name);
                    message.ToPerson = new KeyValuePair<string, string>(toPerson.DocumentNumber, toPerson.Name);
                    message.Content = RSA.EncryptMessage(toPerson.PublicKey, Encoding.UTF8.GetBytes(messageContent));

                    Console.Write("Insira a chave privada do Remetente: ");
                    string privateKey = Console.ReadLine();

                    if (RSA.ValidateKey(privateKey, KeyType.PrivateKey)) {
                        message.Signature = RSA.Sign(privateKey, Convert.FromBase64String(message.Content));
                    }
                }
                else {
                    message.FromPerson = new KeyValuePair<string, string>(fromPerson.DocumentNumber, fromPerson.Name);
                    message.ToPerson = new KeyValuePair<string, string>(toPerson.DocumentNumber, toPerson.Name);
                    message.Content = RSA.EncryptMessage(toPerson.PublicKey, Encoding.UTF8.GetBytes(messageContent));
                    message.Signature = String.Empty;
                    message.MessageType = MessageType.Unsigned;
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        static void SendMessage() {
            int opc = 0;
            Console.WriteLine("------------------------------\n" +
                              $"-1 - {MessageType.Signed.GetDisplayName()}\n-2 - {MessageType.Unsigned.GetDisplayName()}" +
                              $"\n------------------------------");

            Console.Write("Opção: ");
            try {
                opc = int.Parse(Console.ReadLine());

                Message message = new Message();

                switch (opc) {
                    case 1: {
                            Console.Clear();
                            message.MessageType = MessageType.Signed;
                            SendMessage(ref message);
                            MessageRepository.Save(message);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Pause("Mensagem enviada com sucesso. ");
                            break;
                        }
                    case 2: {
                            Console.Clear();
                            message.MessageType = MessageType.Unsigned;
                            SendMessage(ref message);
                            MessageRepository.Save(message);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Pause("Mensagem enviada com sucesso. ");
                            break;
                        }
                    default: {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Pause("Opção inválida. ");
                            break;
                        }
                }
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Pause(ex.Message + " ");
                return;
            }
        }

        private static void ProcessMessage(Message message) {

            try {
                if (message.MessageType == MessageType.Signed) {

                    Console.Clear();

                    ShowAllPersons();

                    Console.Write("Digite a chave pública do remetente:");
                    string publicKey = Console.ReadLine();

                    if (RSA.ValidateKey(publicKey, KeyType.PublicKey)) {
                        if (RSA.CheckSignature(publicKey, Convert.FromBase64String(message.Content), Convert.FromBase64String(message.Signature))) {
                            Console.Clear();

                            Console.ForegroundColor = ConsoleColor.Green;
                            Pause("A chave pública é válida. \n");

                            Console.Write("Digite a chave privada do destinatário:");
                            string privateKey = Console.ReadLine();

                            if (RSA.ValidateKey(privateKey, KeyType.PrivateKey)) {
                                message.Content = RSA.DecrypteMessage(privateKey, Convert.FromBase64String(message.Content));

                                Console.Clear();
                                Pause($"Mensagem decodificada:\n{message}");
                            }
                        }
                        else {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Red;
                            Pause("A assinatura é inválida.\n");
                            return;
                        }
                    }
                }
            }
            catch {
                throw new Exception($"Ocorreu um erro ao processar a mensagem de ID '{message.Id}'");
            }
        }

        static void ShowMessages() {
            List<Message> messages = MessageRepository.Get();

            try {
                if (messages == null || messages.Count == 0) {
                    Console.Clear();
                    Pause($"----------------------------------------\nNão há mensagens para exibir.\n----------------------------------------\n");
                    return;
                }

                messages.ForEach(message => {
                    Console.Write(message);
                });

                Console.Write("ID da Mensagem: ");
                int id = int.Parse(Console.ReadLine());

                Message message = messages.Find(message => message.Id == id);

                while (message == null) {
                    Console.Clear();
                    Pause($"Não há mensagens com o ID '{id}' informado. ");

                    messages.ForEach(message => {
                        Console.Write(message);
                    });

                    Console.Write("ID da Mensagem: ");
                    id = int.Parse(Console.ReadLine());

                    message = messages.Find(message => message.Id == id);
                }

                ProcessMessage(message);
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Pause(ex.Message);
                return;
            }
        }

        static void ShowAllPersons() {
            List<Person> personList = PersonRepository.Get();

            Console.Clear();

            personList.ForEach(person => {
                Console.WriteLine(person);
            });
        }

        static void CreatePerson() {
            Person person = new Person();
            Console.WriteLine("----- Criar nova pessoa ------");
            try {
                Console.Write("Nome: ");
                person.Name = Console.ReadLine();

                Console.Write("CPF/CNPJ: ");
                person.DocumentNumber = Console.ReadLine();

                person.CreateKeys();

                if (PersonValidation.ValidatePersonData(person))
                    PersonRepository.Save(person);
                    
                Console.ForegroundColor = ConsoleColor.Green;
                Pause("Pessoa criada com sucesso. ");
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Pause($"Erro ao criar pessoa. {ex.Message}");
                return;
            }
        }

        static void Pause(string message = null) {
            Console.WriteLine($"{message}Pressione uma tecla para continuar...");

            Console.ForegroundColor = ConsoleColor.Gray;

            if (Console.KeyAvailable)
                Console.ReadKey(true);

            Console.ReadKey();
            Console.Clear();
        }

        static void Menu() {
            int opc = 0;
            Console.Write("--------------------------------\n" +
                "-1 - Criar Pessoa\n-2 - Ver pessoas\n-3 - Enviar Mensagem\n-4 - Ver mensagens\n-5 - Sair\n" +
                "--------------------------------\nOpção: ");
            try {
                if (Console.KeyAvailable)
                    Console.ReadKey(true);

                opc = int.Parse(Console.ReadLine());

                switch (opc) {
                    case 1: {
                            Console.Clear();
                            CreatePerson();
                            Menu();
                            break;
                        }
                    case 2: {
                            Console.Clear();
                            ShowAllPersons();
                            Pause();
                            Menu();
                            break;
                        }
                    case 3: {
                            Console.Clear();
                            SendMessage();
                            Menu();
                            break;
                        }
                    case 4: {
                            Console.Clear();
                            ShowMessages();
                            Menu();
                            break;
                        }
                    default:
                        Console.Clear();
                        Pause("Aplicação finalizada. ");
                        break;
                }
            }
            catch (Exception ex) {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Pause(ex.Message + " ");
                Menu();
            }

        }

        static void Main(string[] args) {

            PersonRepository.InitializePersonRepository();
            MessageRepository.InitializeMessageRepository();

            try {
                Person alice = new Person();

                alice.Name = "Alice Maria da Silva";
                alice.DocumentNumber = "88273341341";
                alice.PersonType = PersonType.Individual;

                Person bob = new Person();

                bob.Name = "Bob Joelson Mota";
                bob.DocumentNumber = "82341131321";
                bob.PersonType = PersonType.Individual;

                if (PersonRepository.Get(alice.DocumentNumber) == null) {
                    alice.CreateKeys();
                    PersonRepository.Save(alice);
                }

                if (PersonRepository.Get(bob.DocumentNumber) == null) {
                    bob.CreateKeys();
                    PersonRepository.Save(bob);
                }

                Menu();
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
            }
        }
    }
}
