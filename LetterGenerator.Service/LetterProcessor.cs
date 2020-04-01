using System;
using System.Collections.Generic;
using System.Text;

namespace LetterGenerator.Service
{
    public class LetterProcessor : ILetterProcessor
    {

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
