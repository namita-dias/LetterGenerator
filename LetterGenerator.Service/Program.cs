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

            ILetterProcessor letterProcessor = new LetterProcessor();
            letterProcessor.dataFilePath = dataFilePath;
            letterProcessor.templateFilePath = templateFilePath;
            letterProcessor.outputFilePath = outputFilePath;
            letterProcessor.StartProcess();
        }
    }
}
