using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestchainApp.Application.Model
{
    public class Transaktion
    {                   
            public Transaktion(string userId, string stockSymbol, int quantity, decimal pricePerShare, DateTime transactionDate, TransactionType type)
            {
                UserId = userId;
                StockSymbol = stockSymbol;
                Quantity = quantity;
                PricePerShare = pricePerShare;
                TransactionDate = transactionDate;
                Type = type;
            }
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; }

            public string UserId { get; set; }

            public string StockSymbol { get; set; }

            public int Quantity { get; set; }

            public decimal PricePerShare { get; set; }

            public DateTime TransactionDate { get; set; }

            public TransactionType Type { get; set; }

            public enum TransactionType
            {
                Buy,
                Sell
            }
        }

    }
