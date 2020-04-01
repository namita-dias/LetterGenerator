using LetterGenerator.Service;
using NUnit.Framework;

namespace LetterGenerator.Tests
{
    public class Tests
    {
        private ILetterProcessor letterProcessor;

        [SetUp]
        public void Setup()
        {
            letterProcessor = new LetterProcessor();
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
    }
}