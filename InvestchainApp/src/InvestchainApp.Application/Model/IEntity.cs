﻿using System;

namespace InvestChainApp.Application.Model
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