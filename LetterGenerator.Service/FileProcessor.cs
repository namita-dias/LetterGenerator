using CsvHelper;
using LetterGenerator.Service.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LetterGenerator.Service
{
    public class FileProcessor : IFileProcessor
    {
        public bool FileExists(string path)
        {
            try
            {
                if (String.IsNullOrEmpty(path))
                {
                    Console.WriteLine("Path empty");
                    return false;
                }
                return File.Exists(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error checking file." + ex.Message);
                return false;
            }
        }

        public IEnumerable<Customer> ReadCSVFile(string path)
        {
            TextReader textReader = null;
            try
            {
               textReader = File.OpenText(path);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error reading csv file." + ex.Message);
            }

            CsvReader csvReader = new CsvReader(textReader, CultureInfo.InvariantCulture);
            csvReader.Configuration.Delimiter = ",";
            csvReader.Configuration.RegisterClassMap<CustomerMap>();
            while (csvReader.Read())
            {
                yield return csvReader.GetRecord<Customer>();
            }
        }

        public string ReadTextFile(string path)
        {
            StreamReader sr = new StreamReader(path);
            string text = sr.ReadToEnd();
            return text;
        }

        public void WriteFile(string text, string outputFilePath, string fileName)
        {
            try
            {
                if (!Directory.Exists(outputFilePath))
                {
                    Directory.CreateDirectory(outputFilePath);
                }
                    
                string path = Path.Combine(outputFilePath, fileName);
                if (FileExists(path))
                {
                    Console.WriteLine("Output file not created. File with name - " + fileName + " already exists.");
                    return;
                }

                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(text);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing file -" + fileName + ex.Message);
            }
        }
    }
}
