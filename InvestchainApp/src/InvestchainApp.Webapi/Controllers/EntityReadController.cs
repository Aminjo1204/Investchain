using AutoMapper;
using InvestchainApp.Application.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace TeamsDemoApp.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class EntityReadController<TEntity> : ControllerBase where TEntity : class, IEntity
    {
        protected readonly IQueryable<TEntity> _set;
        protected readonly IModel _model;     // For dynamic query expansion
        protected readonly IMapper _mapper;

        protected EntityReadController(IQueryable<TEntity> set, IModel model, IMapper mapper)
        {
            _set = set;
            _model = model;
            _mapper = mapper;
        }

        protected async Task<IActionResult> GetAll<TDto>(Expression<Func<TEntity, TDto>> projection)
        {
            var result = await _set
                .Select(projection)
                .ToListAsync();
            return Ok(result);
        }

        protected async Task<IActionResult> GetAll<TDto>()
        {
            var query = _set;
            query = ExpandQueryByParam(query);  // Add includes
            var data = await ExpandQueryByParam(query).ToListAsync();
            return Ok(_mapper.Map<IList<TDto>>(data));
        }

        protected async Task<IActionResult> GetByGuid<TDto>(Guid guid)
        {
            var query = _set;
            query = ExpandQueryByParam(query);  // Add includes
            var data = await ExpandQueryByParam(query).FirstOrDefaultAsync();
            if (data is null) return NotFound();
            return Ok(_mapper.Map<TDto>(data));
        }

        protected async Task<IActionResult> GetByGuid<TDto>(Guid guid, Expression<Func<TEntity, TDto>> projection)
        {
            var result = await _set
                .Where(e => e.Guid == guid)
                .Select(projection)
                .FirstOrDefaultAsync();
            if (result is null) return NotFound();
            return Ok(result);
        }

        protected IQueryable<TEntity> ExpandQueryByParam(IQueryable<TEntity> query)
        {
            // Suche z. B. den Entity Type Handin
            var entity = _model.FindEntityType(typeof(TEntity));
            if (entity is null) { throw new ApplicationException($"Entity {typeof(TEntity).Name} not found."); }

            // HTTP Request im Controller analysiere die Parameter
            // $expand=Student,Task soll ausgelesen werden.
            if (!HttpContext.Request.Query.TryGetValue("$expand", out var paramValues))
                return query;
            // values wäre dann [Student, Task]
            var values = paramValues.SelectMany(v => v.Split(",")).ToList();

            var expandNavigations = entity.GetNavigations()
                .Where(n => values.Contains(n.Name) || values.Contains(n.Name.ToLower())).Select(n => n.Name);
            foreach (var navigation in expandNavigations)
                query = query.Include(navigation);                // _db.Set<Handin>().Include(h=>h.Student)
            return query;
        }
    }
}
