using CsvHelper;
using LetterGenerator.Service.Models;
using StringTokenFormatter;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LetterGenerator.Service
{
    public class LetterProcessor : ILetterProcessor
    {
        private string _dataFilePath;
        public string dataFilePath
        {
            get
            {
                return _dataFilePath;
            }
            set
            {
                _dataFilePath = value;
            }
        }

        private string _templateFilePath;
        public string templateFilePath
        {
            get
            {
                return _templateFilePath;
            }
            set
            {
                _templateFilePath = value;
            }
        }

        private string _outputFilePath;
        public string outputFilePath
        {
            get
            {
                return _outputFilePath;
            }
            set
            {
                _outputFilePath = value;
            }
        }

        IFileProcessor fileProcessor;

        public LetterProcessor()
        {
            fileProcessor = new FileProcessor();
        }

        public void StartProcess()
        {
            // If both the input files exist then process the .csv file
            if (fileProcessor.FileExists(dataFilePath)
                && fileProcessor.FileExists(templateFilePath))
                ProcessData();
        }

        /// <summary>
        /// Read the csv file using CsvReader and store the rows in a collection of 'Customer' model.
        /// </summary>
        public void ProcessData()
        {
            try
            {
                IEnumerable<Customer> data = null;
                using (var streamReader = new StreamReader(dataFilePath))
                using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    csvReader.Configuration.RegisterClassMap<CustomerMap>();
                    data = csvReader.GetRecords<Customer>();
                    ProcessCustomer(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not read the file - " + dataFilePath, ex.Message);
            }
        }

        /// <summary>
        /// Process the customer records.
        /// </summary>
        /// <param name="customers"></param>
        public void ProcessCustomer(IEnumerable<Customer> customers)
        {
            try
            {
                if (customers == null)
                {
                    Console.WriteLine("No data to process");
                    return;
                }
                else
                {
                    foreach (Customer customer in customers)
                    {
                        // Load the email template
                        string templateText = String.Empty;
                        templateText = LoadTemplate();

                        // Get values for the email template tokens
                        string outputText = String.Empty;
                        if (!String.IsNullOrEmpty(templateText))
                            outputText = LoadValues(templateText, customer);

                        // Write the output file 
                        if (!String.IsNullOrEmpty(outputText))
                            fileProcessor.WriteFile(outputText, outputFilePath,
                                                    $"{customer.Id}{customer.FirstName}{customer.Surname}.txt");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing data." + ex.Message);
                return;
            }
        }

        /// <summary>
        /// Read the email template file
        /// </summary>
        /// <returns></returns>
        public string LoadTemplate()
        {
            StreamReader sr = new StreamReader(templateFilePath);
            string templateText = sr.ReadToEnd();
            return templateText;
        }

        /// <summary>
        /// Calculate the total premium and monthly charges. Load values to the email template tokens.
        /// </summary>
        /// <param name="templateText"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public string LoadValues(string templateText, Customer customer)
        {
            try
            {
                // Invalid values
                if (!CheckValidAmount(customer.PayoutAmount.ToString())
                    || !CheckValidAmount(customer.AnnualPremium.ToString())
                    || customer.PayoutAmount <= 0
                    || customer.AnnualPremium <= 0)
                    return String.Empty;

                decimal creditCharge = CalculateCreditCharge(customer.AnnualPremium);
                decimal annualPremiumPlusCreditCharge = customer.AnnualPremium + creditCharge;
                decimal averageMonthlyPremium = annualPremiumPlusCreditCharge / 12;
                decimal[] monthlyPayments = CalculateMonthlyPayments(averageMonthlyPremium, annualPremiumPlusCreditCharge);
                decimal initialMonthlyPayment = monthlyPayments[0];
                decimal otherMonthlyPaymentsAmount = monthlyPayments[1];

                // Invalid calculated values
                if (creditCharge <= 0
                    || initialMonthlyPayment <= 0
                    || otherMonthlyPaymentsAmount <= 0)
                    return String.Empty;

                var tokenValues = new
                {
                    CurrentDate = DateTime.Today.ToShortDateString(),
                    FullName = $"{customer.Title} {customer.FirstName} {customer.Surname}",
                    Salutation = $"{customer.Title} {customer.Surname}",
                    ProductName = customer.ProductName,
                    PayoutAmount = customer.PayoutAmount.ToString("C"),
                    AnnualPremium = customer.AnnualPremium.ToString("C"),
                    CreditCharge = creditCharge.ToString("C"),
                    AnnualPremiumPlusCreditCharge = annualPremiumPlusCreditCharge.ToString("C"),
                    InitialMonthlyPaymentAmount = initialMonthlyPayment.ToString("C"),
                    OtherMonthlyPaymentsAmount = otherMonthlyPaymentsAmount.ToString("C")
                };
                return templateText.FormatToken(tokenValues);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading values" + ex.Message);
                return String.Empty;
            }
        }

        /// <summary>
        /// Check if the amount is numeric. Used double to check both int and decimal
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool CheckValidAmount(string amount)
        {
            double v;
            return (double.TryParse(amount, out v));
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
        /// Calculate the amount to be paid monthly.
        /// </summary>
        /// <param name="averageMonthlyPremium"></param>
        /// <param name="annualPremiumPlusCreditCharge"></param>
        /// <returns> The initial and other payment amounts</returns>
        public decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal annualPremiumPlusCreditCharge)
        {
            string averageMonthlyPremiumString = averageMonthlyPremium.ToString();
            decimal initialPayment = 0.0M;
            decimal otherPayments = 0.0M;

            if (averageMonthlyPremium <= 0 || annualPremiumPlusCreditCharge <= 0)
                Console.WriteLine("Invalid amount");

            // If the amount value has more than 2 numbers after the decimal then it's not a valid amount.
            else if ((averageMonthlyPremiumString.Substring(averageMonthlyPremiumString.IndexOf(".") + 1).Length) > 2)
            {
                // Get exactly 2 decimal places
                decimal truncatedValue = Math.Truncate(averageMonthlyPremium * 100) / 100;
                otherPayments = truncatedValue;
                initialPayment = Decimal.Round(annualPremiumPlusCreditCharge - (otherPayments * 11), 2);
            }

            else
                initialPayment = otherPayments = averageMonthlyPremium;

            return new decimal[] { initialPayment, otherPayments };
        }
    }
}
