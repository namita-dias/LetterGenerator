using LetterGenerator.Service.Models;
using StringTokenFormatter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LetterGenerator.Service
{
    public class LetterProcessor : ILetterProcessor
    {

        private IFileProcessor fileProcessor;

        public LetterProcessor(IFileProcessor processor)
        {
            fileProcessor = processor;
        }

        public void ProcessLetters(string csvFilePath, string letterTemplateFilePath, string outputFolderPath )
        {
            if (!fileProcessor.FileExists(csvFilePath)
                  || !fileProcessor.FileExists(letterTemplateFilePath))
            {
                Console.WriteLine("Input files not available");
                return;
            }

            IEnumerable<Customer> customers = fileProcessor.ReadCSVFile(csvFilePath);

            if (!customers.Any())
            {
                Console.WriteLine("No records in the file.");
                return;
            }

            foreach (Customer customer in customers)
            {
                ProcessCustomer(customer, letterTemplateFilePath, outputFolderPath);
            }
        }

        /// <summary>
        /// Process the customer
        /// </summary>
        /// <param name="customer"></param>
        public void ProcessCustomer(Customer customer, string letterTemplateFilePath, string outputFolderPath)
        {
            try
            {   // Load the email template
                string templateText = String.Empty;
                templateText = fileProcessor.ReadTextFile(letterTemplateFilePath);

                // Get values for the email template tokens
                string outputText = String.Empty;
                if (String.IsNullOrEmpty(templateText))
                {
                    Console.WriteLine("Email template file has no content");
                    return;
                }
                outputText = LoadValues(templateText, customer);

                // Write the output file 
                if (String.IsNullOrEmpty(outputText))
                {
                    Console.WriteLine("Output text not created for customer - " + customer.Id);
                    return;
                }
                fileProcessor.WriteFile(outputText, outputFolderPath,
                                            $"{customer.Id}{customer.FirstName}{customer.Surname}.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing data." + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Calculate the total premium and monthly charges. Load values to the email template tokens.
        /// </summary>
        /// <param name="templateText"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public string LoadValues(string templateText, Customer customer)
        {
            // Invalid values
            if (!CheckValidAmount(customer.PayoutAmount)
                || !CheckValidAmount(customer.AnnualPremium))
            {
                return String.Empty;
            }

            decimal creditCharge = CalculateCreditCharge(customer.AnnualPremium);
            decimal totalPremium = TotalPremium(customer.AnnualPremium, creditCharge);  
            decimal averageMonthlyPremium = AverageMonthlyPremium(totalPremium);
            decimal[] monthlyPayments = CalculateMonthlyPayments(averageMonthlyPremium, totalPremium);
            decimal initialMonthlyPayment = monthlyPayments[0];
            decimal otherMonthlyPaymentsAmount = monthlyPayments[1];

            // Invalid calculated values
            if (!CheckValidAmount(initialMonthlyPayment)
                || !CheckValidAmount(otherMonthlyPaymentsAmount))
            {
                return String.Empty;
            }

            var tokenValues = new
            {
                CurrentDate = DateTime.Today.ToShortDateString(),
                FullName = $"{customer.Title} {customer.FirstName} {customer.Surname}",
                Salutation = $"{customer.Title} {customer.Surname}",
                ProductName = customer.ProductName,
                PayoutAmount = customer.PayoutAmount.ToString("C"),
                AnnualPremium = customer.AnnualPremium.ToString("C"),
                CreditCharge = creditCharge.ToString("C"),
                AnnualPremiumPlusCreditCharge = totalPremium.ToString("C"),
                InitialMonthlyPaymentAmount = initialMonthlyPayment.ToString("C"),
                OtherMonthlyPaymentsAmount = otherMonthlyPaymentsAmount.ToString("C")
            };
            return templateText.FormatToken(tokenValues);
        }

        /// <summary>
        /// Check if the amount is numeric and greater than 0. Used double to check both int and decimal.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CheckValidAmount(decimal amount)
        {
            double v;
            return ((double.TryParse(amount.ToString(), out v)) && (amount > 0));
        }

        /// <summary>
        /// Calculate the credit charge. 
        /// creditCharge = 5% of annualPremium
        /// </summary>
        /// <param name="annualPremium"></param>
        /// <returns> Credit charge</returns>
        public decimal CalculateCreditCharge(decimal annualPremium)
        {
            if (annualPremium <= 0)
            {
                Console.WriteLine("Invalid value for annual premium.");
                return 0;
            }

            return Math.Round((5 * annualPremium) / 100, 2);
        }

        /// <summary>
        /// Calculate the total premium.
        /// </summary>
        /// <param name="annualPremium"></param>
        /// <param name="creditCharge"></param>
        /// <returns></returns>
        public decimal TotalPremium(decimal annualPremium, decimal creditCharge)
        {
            return annualPremium + creditCharge;
        }

        /// <summary>
        /// Calculate the average monthly premium.
        /// </summary>
        /// <param name="totalPremium"></param>
        /// <returns></returns>
        public decimal AverageMonthlyPremium(decimal totalPremium)
        {
            return totalPremium / 12;
        }

        /// <summary>
        /// Calculate the amount to be paid monthly.
        /// </summary>
        /// <param name="averageMonthlyPremium"></param>
        /// <param name="annualPremiumPlusCreditCharge"></param>
        /// <returns> The initial and other payment amounts</returns>
        public decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal totalPremium)
        {
            string averageMonthlyPremiumString = averageMonthlyPremium.ToString();
            decimal initialPayment = 0.0M;
            decimal otherPayments = 0.0M;

            if (averageMonthlyPremium <= 0 || totalPremium <= 0)
            {
                Console.WriteLine("Invalid amount");
            }

            // If the amount value has more than 2 numbers after the decimal then it's not a valid amount.
            else if ((averageMonthlyPremiumString.Substring(averageMonthlyPremiumString.IndexOf(".") + 1).Length) > 2)
            {
                // Get exactly 2 decimal places
                decimal truncatedValue = Math.Truncate(averageMonthlyPremium * 100) / 100;
                otherPayments = truncatedValue;
                initialPayment = Decimal.Round(totalPremium - (otherPayments * 11), 2);
            }

            else
            {
                initialPayment = otherPayments = averageMonthlyPremium;
            }

            return new decimal[] { initialPayment, otherPayments };
        }
    }
}
