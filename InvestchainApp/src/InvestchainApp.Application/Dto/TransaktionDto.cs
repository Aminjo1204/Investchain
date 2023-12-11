using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InvestchainApp.Application.Model.Transaktion;

namespace InvestchainApp.Application.Dto
{
    public record TransaktionDto(
    int Id,
    string UserId,
    string StockSymbol,
    int Quantity,
    decimal PricePerShare,
    DateTime TransactionDate,
    TransactionType Type
);
}
