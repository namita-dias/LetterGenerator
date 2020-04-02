using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LetterGenerator.Service
{
    public class FileProcessor : IFileProcessor
    {
        public bool FileExists(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                Console.WriteLine("Path empty");
                return false;
            }
            return File.Exists(path);
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
