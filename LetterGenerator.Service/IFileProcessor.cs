using LetterGenerator.Service.Models;
using System.Collections.Generic;

namespace LetterGenerator.Service
{
    public interface IFileProcessor
    {
        bool FileExists(string path);
        IEnumerable<Customer> ReadCSVFile(string path);
        string ReadTextFile(string path);
        void WriteFile(string text, string outputFilePath, string fileName);
    }
}
