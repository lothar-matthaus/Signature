using Signature.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Signature.Repository
{
	static class MessageRepository
	{
		static string pathDirectory = @"C:/DataFiles/";
		static string filePath = @"C:/DataFiles/Messages.json";

		public static void InitializeMessageRepository()
		{
			if (!Directory.Exists(pathDirectory))
			{
				Directory.CreateDirectory(pathDirectory);

				if (!File.Exists(filePath))
					File.Create(filePath).Close();
			}
			else
				if (!File.Exists(filePath))
				File.Create(filePath).Close();
		}

		public static void Save(Message message)
		{
			FileStream fileStream = null;
			StreamWriter streamWriter = null;

			try
			{
				using (fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
				{
					using (streamWriter = new StreamWriter(fileStream))
					{
						streamWriter.WriteLine(JsonSerializer.Serialize<Message>(message));

						streamWriter.Close();
					}
					fileStream.Close();
				}
			}
			catch (IOException ex)
			{
				fileStream.Close();
				streamWriter.Close();

				throw ex;
			}
		}

		public static List<Message> Get()
		{
			FileStream fileStream = null;
			StreamReader streamReader = null;

			List<Message> personList = new List<Message>();

			try
			{
				using (fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					using (streamReader = new StreamReader(fileStream))
					{
						while (!streamReader.EndOfStream)
							personList.Add(JsonSerializer.Deserialize<Message>(streamReader.ReadLine()));

						streamReader.Close();
					}
					fileStream.Close();
				}
			}
			catch (IOException ex)
			{
				fileStream.Close();
				streamReader.Close();

				throw ex;
			}
			return personList;
		}
	}
}
