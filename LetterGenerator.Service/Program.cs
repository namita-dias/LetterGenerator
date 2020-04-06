using System;

namespace LetterGenerator.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvFilePath = "..\\..\\..\\InputFiles\\Customer.csv";
            string letterTemplateFilePath = "..\\..\\..\\InputFiles\\Email_Template.txt";
            string outputFolderPath = "..\\..\\..\\OutputFiles";

            IFileProcessor fileProcessor = new FileProcessor();

            ILetterProcessor letterProcessor = new LetterProcessor(fileProcessor);

            letterProcessor.ProcessLetters(csvFilePath, letterTemplateFilePath, outputFolderPath);
        }
    }
}
