using Signature.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Signature.Repository
{
	static class PersonRepository
	{
		static string pathDirectory = "./DataFiles/";
		static string filePath = "./DataFiles/Person.json";
		static string privateKeysDirectory = "./DataFiles/Person/Private Keys/";

		public static void InitializePersonRepository()
		{
			if (!Directory.Exists(pathDirectory))
				Directory.CreateDirectory(pathDirectory);

			if (!File.Exists(filePath))
				File.Create(filePath).Close();
			
			if (!Directory.Exists(privateKeysDirectory))
				Directory.CreateDirectory(privateKeysDirectory);
		}

		public static List<Person> Get()
		{
			FileStream fileStream = null;
			StreamReader streamReader = null;

			List<Person> personList = new List<Person>();

			try
			{
				using (fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					using (streamReader = new StreamReader(fileStream))
					{
						while (!streamReader.EndOfStream)
							personList.Add(JsonSerializer.Deserialize<Person>(streamReader.ReadLine()));

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

		public static Person Get(string documentNumber)
		{
			FileStream fileStream = null;
			StreamReader streamReader = null;

			Person person;

			try
			{
				using (fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				{
					using (streamReader = new StreamReader(fileStream))
					{
						while (!streamReader.EndOfStream)
						{
							person = JsonSerializer.Deserialize<Person>(streamReader.ReadLine());

							if (person.DocumentNumber == documentNumber)
								return person;
						}

						streamReader.Close();
					}
					fileStream.Close();

					return null;
				}
			}
			catch (IOException ex)
			{
				fileStream.Close();
				streamReader.Close();

				throw ex;
			}
		}

		public static void Save(Person person)
		{
			FileStream fileStream = null;
			StreamWriter streamWriter = null;

			if (Get(person.DocumentNumber) != null)
			{
				throw new Exception("Não é possível cadastrar uma pessoa com o mesmo documento.");
			}

			try
			{
				using (fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write))
				{
					using (streamWriter = new StreamWriter(fileStream))
					{
						streamWriter.WriteLine(JsonSerializer.Serialize<Person>(person));

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
	}
}
