using LetterGenerator.Service;
using LetterGenerator.Service.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace LetterGenerator.Tests
{
    public class Tests
    {
        private ILetterProcessor letterProcessor;
        private IEnumerable<Customer> customers;
        private string outputFileName;

        [SetUp]
        public void Setup()
        {
            customers = new Customer[] 
            { 
                new Customer
                {
                    Id = 1,
                    FirstName = "TestFirstName",
                    Surname = "TestSurname",
                    ProductName = "Test Product",
                    PayoutAmount = 200000,
                    AnnualPremium = 150
                }
            };

            letterProcessor = new LetterProcessor();

            letterProcessor.dataFilePath = "..\\..\\..\\InputFiles\\Customer.csv";
            letterProcessor.templateFilePath = "..\\..\\..\\InputFiles\\Email_Template.txt";
            letterProcessor.outputFilePath = "..\\..\\..\\OutputFiles";

            outputFileName = "1TestFirstNameTestSurname.txt";
        }

        [TestCase("125")]
        public void CheckValidAmount_ValidAmount_ReturnsTrue(string value)
        {
            //act
            bool result = letterProcessor.CheckValidAmount(value);

            //assert
            Assert.IsTrue(result);
        }

        [TestCase("125A")]
        public void CheckValidAmount_ValidAmount_ReturnsFalse(string value)
        {
            //act
            bool result = letterProcessor.CheckValidAmount(value);

            //assert
            Assert.IsFalse(result);
        }

        [TestCase(125.00)]
        public void CalculateCreditCharge_ValidAmount_ReturnsValidAmount(decimal value)
        {
            //act
            decimal result = letterProcessor.CalculateCreditCharge(value);

            //assert
            Assert.AreEqual(6.25, result);
        }

        [TestCase(-125.00)]
        public void CalculateCreditCharge_InValidAmount_ReturnsZero(decimal value)
        {
            //act
            decimal result = letterProcessor.CalculateCreditCharge(value);

            //assert
            Assert.AreEqual(0, result);
        }

        [TestCase(10.41666666, 125)]
        public void CalculateMonthlyPayments_ValidAmounts_ReturnsValidArrayOfAmount(decimal value1, decimal value2)
        {
            //act
            decimal[] result = letterProcessor.CalculateMonthlyPayments(value1, value2);

            //assert
            Assert.AreEqual(10.49, result[0]);
            Assert.AreEqual(10.41, result[1]);
        }

        [TestCase(-10.41666666, 125)]
        public void CalculateMonthlyPayments_InValidAmount_ReturnsZero(decimal value1, decimal value2)
        {
            //act
            decimal[] result = letterProcessor.CalculateMonthlyPayments(value1, value2);

            //assert
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [Test]
        public void ProcessCustomer_ValidCustomer_CreatesLetter()
        {
            //arrange
            string path = Path.Combine(letterProcessor.outputFilePath, outputFileName);
            // Delete the file if it exists.
            if (File.Exists(path))
                File.Delete(path);

            //act
            letterProcessor.ProcessCustomer(customers);

            //assert
            Assert.IsTrue(File.Exists(path));
        }

        [Test]
        public void ProcessCustomer_InValidCustomer_LetterNotCreated()
        {
            //arrange
            string path = Path.Combine(letterProcessor.outputFilePath, outputFileName);
            // Delete the file if it exists.
            if (File.Exists(path))
                File.Delete(path);

            //act
            letterProcessor.ProcessCustomer(null);

            //assert
            Assert.IsFalse(File.Exists(path));
        }
    }
}