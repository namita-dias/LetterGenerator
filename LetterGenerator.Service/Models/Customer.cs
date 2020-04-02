using System.ComponentModel.DataAnnotations;

namespace LetterGenerator.Service.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string FirstName { get; set; }

        public string Surname { get; set; }

        public string ProductName { get; set; }

        public decimal PayoutAmount { get; set; }

        public decimal AnnualPremium { get; set; }
    }
}
