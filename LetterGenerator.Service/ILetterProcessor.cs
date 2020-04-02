using LetterGenerator.Service.Models;
using System.Collections.Generic;

namespace LetterGenerator.Service
{
    public interface ILetterProcessor
    {
        string dataFilePath { get; set; }
        string templateFilePath { get; set; }
        string outputFilePath { get; set; }
        void StartProcess();
        void ProcessCustomer(IEnumerable<Customer> customers);
        decimal CalculateCreditCharge(decimal annualPremium);
        decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal annualPremiumPlusCreditCharge);
        bool CheckValidAmount(string amount);
    }
}