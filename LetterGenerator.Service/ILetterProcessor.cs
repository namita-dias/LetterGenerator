namespace LetterGenerator.Service
{
    public interface ILetterProcessor
    {
        string dataFilePath { get; set; }
        string templateFilePath { get; set; }
        string outputFilePath { get; set; }
        void StartProcess();
        decimal CalculateCreditCharge(decimal annualPremium);
        decimal[] CalculateMonthlyPayments(decimal averageMonthlyPremium, decimal annualPremiumPlusCreditCharge);
        bool CheckValidAmount(string amount);
    }
}