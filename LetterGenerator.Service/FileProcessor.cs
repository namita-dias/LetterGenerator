using CsvHelper;
using LetterGenerator.Service.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LetterGenerator.Service
{
    public class FileProcessor : IFileProcessor
    {
        private string _dataFilePath;
        public string dataFilePath
        {
            get
            {
                return _dataFilePath;
            }
            set
            {
                _dataFilePath = value;
            }
        }

        private string _templateFilePath;
        public string templateFilePath
        {
            get
            {
                return _templateFilePath;
            }
            set
            {
                _templateFilePath = value;
            }
        }

        private string _outputFilePath;
        public string outputFilePath
        {
            get
            {
                return _outputFilePath;
            }
            set
            {
                _outputFilePath = value;
            }
        }

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

        public List<Customer> ReadCSVFile(string path)
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                using (TextReader reader = File.OpenText(path))
                {
                    CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csvReader.Configuration.Delimiter = ",";
                    csvReader.Configuration.RegisterClassMap<CustomerMap>();
                    while (csvReader.Read())
                    {
                        Customer customer = csvReader.GetRecord<Customer>();
                        customers.Add(customer);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error reading csv file." + ex.Message);
            }
            return customers;
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
                    Directory.CreateDirectory(outputFilePath);

                string path = Path.Combine(outputFilePath, fileName);

                if (!FileExists(path))
                {
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.WriteLine(text);
                    }
                }
                else
                    Console.WriteLine("Output file not created. File with name - " + fileName + " already exists.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing file -" + fileName + ex.Message);
            }
        }
    }
}
