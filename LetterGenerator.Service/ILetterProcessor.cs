using LetterGenerator.Service.Models;
using System.Collections.Generic;

namespace LetterGenerator.Service
{
    public interface ILetterProcessor
    {
        void StartProcess();
        void ProcessCustomer(Customer customer);
        decimal CalculateCreditCharge(decimal annualPremium);
        decimal TotalPremium(decimal annualPremium, decimal creditCharge);
        decimal AverageMonthlyPremium(decimal totalPremium);
        decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal totalPremium);
        bool CheckValidAmount(decimal amount);
    }
}