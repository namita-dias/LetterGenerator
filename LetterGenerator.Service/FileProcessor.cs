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
    }
}
