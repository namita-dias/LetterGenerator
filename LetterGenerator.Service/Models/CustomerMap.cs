using System;
using CsvHelper.Configuration;

namespace LetterGenerator.Service.Models
{
    public class CustomerMap : ClassMap<Customer>
    {
        public CustomerMap()
        {
            Map(m => m.Id).Name("ID");
            Map(m => m.Title).Name("Title");
            Map(m => m.FirstName).Name("FirstName");
            Map(m => m.Surname).Name("Surname");
            Map(m => m.ProductName).Name("ProductName");
            Map(m => m.PayoutAmount).Name("PayoutAmount");
            Map(m => m.AnnualPremium).Name("AnnualPremium");
        }
    }
}
