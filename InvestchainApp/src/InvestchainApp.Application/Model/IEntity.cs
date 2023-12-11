using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestchainApp.Application.Model
{
    public interface IEntity
    {
        Guid Guid { get; }
    }

    public interface IEntity<Tkey> : IEntity where Tkey : struct
    {
        Tkey Id { get; }
    }
}
