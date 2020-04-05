using LetterGenerator.Service.Models;
using System.Collections.Generic;

namespace LetterGenerator.Service
{
    public interface IFileProcessor
    {
        string dataFilePath { get; set; }
        string templateFilePath { get; set; }
        string outputFilePath { get; set; }
        bool FileExists(string path);
        List<Customer> ReadCSVFile(string path);
        string ReadTextFile(string path);
        void WriteFile(string text, string outputFilePath, string fileName);
    }
}
