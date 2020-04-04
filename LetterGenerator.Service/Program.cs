using System;

namespace LetterGenerator.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            string dataFilePath = "..\\..\\..\\InputFiles\\Customer.csv";
            string templateFilePath = "..\\..\\..\\InputFiles\\Email_Template.txt";
            string outputFilePath = "..\\..\\..\\OutputFiles";

            IFileProcessor fileProcessor = new FileProcessor();

            fileProcessor.dataFilePath = dataFilePath;
            fileProcessor.templateFilePath = templateFilePath;
            fileProcessor.outputFilePath = outputFilePath;

            ILetterProcessor letterProcessor = new LetterProcessor(fileProcessor);

            letterProcessor.StartProcess();
        }
    }
}
