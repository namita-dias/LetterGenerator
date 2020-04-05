using LetterGenerator.Service;
using LetterGenerator.Service.Models;
using Moq;
using NUnit.Framework;

namespace LetterGenerator.Tests
{
    public class Tests
    {
        private ILetterProcessor letterProcessor;
        private Mock<IFileProcessor> mockFileProcessor;
        private Customer validCustomer;
        private Customer invalidCustomer;

        [SetUp]
        public void Setup()
        {
            validCustomer = new Customer
            {
                Id = 1,
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                ProductName = "Test Product",
                PayoutAmount = 200000,
                AnnualPremium = 150
            };

            invalidCustomer = new Customer
            {
                Id = 2,
                FirstName = "TestFirstName",
                Surname = "TestSurname",
                ProductName = "Test Product",
                PayoutAmount = 0,
                AnnualPremium = -15
            };

            mockFileProcessor = new Mock<IFileProcessor>();
            mockFileProcessor.Setup(s => s.ReadTextFile(It.IsAny<string>())).Returns("{FullName}");

            letterProcessor = new LetterProcessor(mockFileProcessor.Object);
        }

        [Test]
        public void CheckValidAmount_ValidAmount_ReturnsTrue()
        {
            //act
            bool result = letterProcessor.CheckValidAmount(validCustomer.PayoutAmount);

            //assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CheckValidAmount_InValidAmount_ReturnsFalse()
        {
            //act
            bool result = letterProcessor.CheckValidAmount(invalidCustomer.PayoutAmount);

            //assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CalculateCreditCharge_ValidAmount_ReturnsValidAmount()
        {
            //arrange
            decimal expected = 7.5M;

            //act
            decimal result = letterProcessor.CalculateCreditCharge(validCustomer.AnnualPremium);

            //assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CalculateCreditCharge_InValidAmount_ReturnsZero()
        {
            //arrange
            decimal expected = 0;

            //act
            decimal result = letterProcessor.CalculateCreditCharge(invalidCustomer.AnnualPremium);

            //assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CalculateMonthlyPayments_ValidAnnualPremium_ReturnsValidPaymentAmount()
        {
            //arrange
            decimal creditCharge = letterProcessor.CalculateCreditCharge(validCustomer.AnnualPremium);
            decimal totalPremium = letterProcessor.TotalPremium(validCustomer.AnnualPremium, creditCharge);
            decimal averageMonthlyPremium = letterProcessor.AverageMonthlyPremium(totalPremium);
            decimal[] expected = new decimal[] { 13.18M, 13.12M };

            //act
            decimal[] result = letterProcessor.CalculateMonthlyPayments(averageMonthlyPremium, totalPremium);

            //assert
            Assert.AreEqual(expected[0], result[0]);
            Assert.AreEqual(expected[1], result[1]);
        }

        [Test]
        public void CalculateMonthlyPayments_InvalidAnnualPremium_ReturnsZero()
        {
            //arrange
            decimal creditCharge = letterProcessor.CalculateCreditCharge(invalidCustomer.AnnualPremium);
            decimal totalPremium = letterProcessor.TotalPremium(invalidCustomer.AnnualPremium, creditCharge);
            decimal averageMonthlyPremium = letterProcessor.AverageMonthlyPremium(totalPremium);
            decimal[] expected = new decimal[] { 0, 0 };

            //act
            decimal[] result = letterProcessor.CalculateMonthlyPayments(averageMonthlyPremium, totalPremium);

            //assert
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(0, result[1]);
        }

        [Test]
        public void ProcessCustomer_ValidCustomer_LetterCreated()
        {
            //act
            letterProcessor.ProcessCustomer(validCustomer);

            //assert
            mockFileProcessor.Verify(x => x.WriteFile($" {validCustomer.FirstName} {validCustomer.Surname}",
                                                        mockFileProcessor.Object.outputFilePath,
                                                        $"{validCustomer.Id}{validCustomer.FirstName}{validCustomer.Surname}.txt"),
                                                        Times.Once);
        }

        [Test]
        public void ProcessCustomer_InValidCustomer_LetterNotCreated()
        {
            //act
            letterProcessor.ProcessCustomer(invalidCustomer);

            //assert
            mockFileProcessor.Verify(x => x.WriteFile($" {invalidCustomer.FirstName} {invalidCustomer.Surname}",
                                                        mockFileProcessor.Object.outputFilePath, 
                                                        $"{invalidCustomer.Id}{invalidCustomer.FirstName}{invalidCustomer.Surname}.txt"), 
                                                        Times.Never);
        }
    }
}