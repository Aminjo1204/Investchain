using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestchainApp.Application.Model
{
    public class Stocks
    {
       

        public Stocks(string symbol, string name, decimal price, DateTime lastUpdated)
        {
            Symbol = symbol;
            Name = name;
            Price = price;
            LastUpdated = lastUpdated;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
