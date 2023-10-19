using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TeamsDemoApp.Application.Infrastructure.Repositories;

namespace InvestchainApp.Application.Infrastructure.Repositories
{
    public class CurrencyRepository : Repository<Currency, int>
    {
        public CurrencyRepository(InvestchainContext db) : base(db)
        {
        }
    }
}
