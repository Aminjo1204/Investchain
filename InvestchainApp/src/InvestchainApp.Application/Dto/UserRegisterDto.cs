using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestChainApp.Application.Dto
{
    public record UserRegisterDto(
        string Username,
        string Mail,
        string InitialPassword
        );
}
