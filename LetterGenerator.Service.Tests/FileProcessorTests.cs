using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetterGenerator.Service.Tests
{
    public class Tests
    {
        private IFileProcessor fileprocessor;
        [SetUp]
        public void Setup()
        {
            fileprocessor = new FileProcessor();
        }

        [Test]
        public void Test1()
        {
            fileprocessor.ReadCSVFile("..\\..\\..\\InputFiles\\Customer.csv");

        }
    }
}
