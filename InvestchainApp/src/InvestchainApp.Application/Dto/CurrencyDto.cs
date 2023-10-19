using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestchainApp.Application.Dto
{
    public record CurrencyDto(
        Guid Guid,
        string Name);
}
