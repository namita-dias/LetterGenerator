using LetterGenerator.Service.Models;
using System.Collections.Generic;

namespace LetterGenerator.Service
{
    public interface ILetterProcessor
    {
        void ProcessLetters(string csvFilePath, string letterTemplateFilePath, string outputFolderPath);
        void ProcessCustomer(Customer customer, string letterTemplateFilePath, string outputFolderPath);
        string LoadValues(string templateText, Customer customer);
        decimal CalculateCreditCharge(decimal annualPremium);
        decimal TotalPremium(decimal annualPremium, decimal creditCharge);
        decimal AverageMonthlyPremium(decimal totalPremium);
        decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal totalPremium);
        bool CheckValidAmount(decimal amount);
    }
}