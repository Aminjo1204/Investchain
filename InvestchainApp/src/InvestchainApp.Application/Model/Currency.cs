using Microsoft.EntityFrameworkCore;
using InvestchainApp.Application.Model;
using System;

namespace InvestchainApp.Application;

[Index(nameof(Name), IsUnique = true)]
public class Currency : IEntity<int>
{
    public Currency(string name)
    {
        Name = name;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Currency()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {

    }

    public int Id { get; private set; }
    public Guid Guid { get; private set; }
    public string Name { get; set; }
    public decimal Value { get; set; }


}

