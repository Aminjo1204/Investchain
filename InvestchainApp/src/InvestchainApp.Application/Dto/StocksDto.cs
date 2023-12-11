using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestchainApp.Application.Dto
{ public record StocksDto(
   int Id,
   string Symbol,
   string Name,
   decimal Price,
   DateTime LastUpdated
);
}
