using LetterGenerator.Service;
using LetterGenerator.Service.Models;
using Moq;
using NUnit.Framework;
using System;

namespace LetterGenerator.Tests
{
    public class LetterProcessorTests
    {
        private ILetterProcessor letterProcessor;
        private Mock<IFileProcessor> mockFileProcessor;
        private Customer validCustomer;
        private Customer invalidCustomer;
        private string emailTemplateText;
        private string expectedOutPutForValidCustomer;

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

            emailTemplateText = "{CurrentDate} FAO: {FullName} RE: Your Renewal Dear {Salutation}We hereby invite you to renew your Insurance Policy, subject to the following terms.Your chosen insurance product is {ProductName}. The amount payable to you in the event of a valid claim will be {PayoutAmount}. Your annual premium will be {AnnualPremium}. If you choose to pay by Direct Debit, we will add a credit charge of {CreditCharge}, bringing the total to {AnnualPremiumPlusCreditCharge}. This is payable by an initial payment of {InitialMonthlyPaymentAmount}, followed by 11 payments of {OtherMonthlyPaymentsAmount} each.Please get in touch with us to arrange your renewal by visiting https://www.regallutoncodingtest.co.uk/renew or calling us on 01625 123456. Kind Regards Regal Luton";

            mockFileProcessor = new Mock<IFileProcessor>();
            mockFileProcessor.Setup(s => s.ReadTextFile(It.IsAny<string>())).Returns(emailTemplateText);

            letterProcessor = new LetterProcessor(mockFileProcessor.Object);

            expectedOutPutForValidCustomer = DateTime.Today.ToShortDateString() + " FAO:  TestFirstName TestSurname RE: Your Renewal Dear  TestSurnameWe hereby invite you to renew your Insurance Policy, subject to the following terms.Your chosen insurance product is Test Product. The amount payable to you in the event of a valid claim will be £200,000.00. Your annual premium will be £150.00. If you choose to pay by Direct Debit, we will add a credit charge of £7.50, bringing the total to £157.50. This is payable by an initial payment of £13.18, followed by 11 payments of £13.12 each.Please get in touch with us to arrange your renewal by visiting https://www.regallutoncodingtest.co.uk/renew or calling us on 01625 123456. Kind Regards Regal Luton";
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
        public void LoadValues_ValidCustomer_EmailContentGeneratedMatchesExpected()
        {
            //act
            string result = letterProcessor.LoadValues(emailTemplateText, validCustomer);

            //assert
            Assert.AreEqual(result, expectedOutPutForValidCustomer);
        }

        [Test]
        public void LoadValue_InvalidCustomer_EmailContentEmpty()
        {
            //act
            string result = letterProcessor.LoadValues(emailTemplateText, invalidCustomer);

            //assert
            Assert.AreEqual(result, string.Empty);
        }

        [Test]
        public void ProcessCustomer_ValidCustomer_LetterCreated()
        {
            //act
            letterProcessor.ProcessCustomer(validCustomer, It.IsAny<string>(), It.IsAny<string>());

            //assert
            mockFileProcessor.Verify(x => x.WriteFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                                                        Times.Once);
        }

        [Test]
        public void ProcessCustomer_InValidCustomer_LetterNotCreated()
        {
            //act
            letterProcessor.ProcessCustomer(invalidCustomer, It.IsAny<string>(), It.IsAny<string>());

            //assert
            mockFileProcessor.Verify(x => x.WriteFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                                                        Times.Never);
        }
    }
}