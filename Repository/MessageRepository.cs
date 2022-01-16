using Signature.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Signature.Repository
{
    static class MessageRepository {
        static string pathDirectory = "./DataFiles/";
        static string filePath = "./DataFiles/Messages.json";

        public static void InitializeMessageRepository() {
            if (!Directory.Exists(pathDirectory))
                Directory.CreateDirectory(pathDirectory);

            if (!File.Exists(filePath))
                File.Create(filePath).Close();
        }

        public static void Save(Message message) {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;

            try {
                using (fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write)) {
                    using (streamWriter = new StreamWriter(fileStream)) {
                        streamWriter.WriteLine(JsonSerializer.Serialize<Message>(message));

                        streamWriter.Close();
                    }

                    fileStream.Close();
                }
            }
            catch(Exception ex) {
                throw new Exception($"Ocorreu um erro ao salvar a mensagem.\nErro: {ex.Message}");
            }
        }

        public static List<Message> Get() {
            FileStream fileStream = null;
            StreamReader streamReader = null;

            List<Message> messageList = new List<Message>();

            try {
                using (fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
                    using (streamReader = new StreamReader(fileStream)) {
                        while (!streamReader.EndOfStream)
                            messageList.Add(JsonSerializer.Deserialize<Message>(streamReader.ReadLine()));

                        streamReader.Close();
                    }
                    fileStream.Close();
                }
            }
            catch (Exception ex) {
                fileStream.Close();
                streamReader.Close();

                throw new Exception($"Ocorreu um problema ao carregar a lista de mensagens.\nErro: {ex.Message}");
            }
            return messageList;
        }
    }
}
