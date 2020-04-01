namespace LetterGenerator.Service
{
    public interface ILetterProcessor
    {
        decimal CalculateCreditCharge(decimal annualPremium);
        decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal annualPremiumPlusCreditCharge);
        bool CheckValidAmount(string amount);
    }
}