using AutoMapper;
using InvestchainApp.Application;
using InvestchainApp.Application.Dto;
using InvestchainApp.Application.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace TeamsDemoApp.Webapi.Controllers
{
    public class CurrencyController : EntityReadController<Currency>
    {
        private readonly CurrencyRepository _repo;

        public CurrencyController(IMapper mapper, CurrencyRepository repo) :
            base(repo.Set, repo.Model, mapper)
        {
            _repo = repo;
        }

        [HttpGet]
        public Task<IActionResult> GetHandins() => GetAll<CurrencyDto>();

        [HttpGet("{guid}")]
        public Task<IActionResult> GetHandin(Guid guid) => GetByGuid<CurrencyDto>(guid);
    }
}
