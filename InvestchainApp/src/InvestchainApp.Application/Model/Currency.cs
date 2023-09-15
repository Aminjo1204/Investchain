using Microsoft.EntityFrameworkCore;

namespace InvestchainApp.Application;

[Index(nameof(Name), IsUnique = true)]
public class Currency
{
    public Currency(string name, decimal value)
    {
        Name = name;
        Value = value;
    }

    public int Id { get; private set; }
    public Guid Guid { get; private set; }
    public string Name { get; set; }
    public decimal Value { get; set; }


}

