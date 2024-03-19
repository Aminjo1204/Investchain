using AutoMapper;
using InvestCainApp.Webapi.Controllers;
using InvestChainApp.Application.Dto;
using InvestChainApp.Application.Infastructure;
using InvestChainApp.Application.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InvestChainApp.Webapi.Controllers
{
    public class UserController : EntityReadController<User>
    {
        private readonly UserRepository _repo;
        private readonly InvestChainContext _db;
        private readonly IConfiguration _config;

        public UserController(IMapper mapper, UserRepository repo, InvestChainContext db, IConfiguration config) : base(repo.Set, repo.Model, mapper)
        {
            _repo = repo;
            _db = db;
            _config = config;

        }

        [HttpGet]
        public async Task<IActionResult> GetUsers() => await GetAll<UserDto>();

        [HttpGet("{guid}")]
        public async Task<IActionResult> GetUser(Guid guid) => await GetByGuid(guid, u =>
        new
        {
            u.Guid,
            u.Mail,
            u.Username
        });

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto newUser)
        {
            var user = _mapper.Map<User>(newUser);
            user.Guid = Guid.NewGuid();
            try
            {
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                return BadRequest(e.Message);
            }
            return Ok("User added successfully. Guid: " + user.Guid);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto credentials)
        {
            var user = await _db.Users.FirstOrDefaultAsync(a => a.Mail == credentials.Email);
            if (user is null || !user.CheckPassword(credentials.Password)) return BadRequest("Password falsch oder User gibt es nicht");
            var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, credentials.Email),
            };
            var claimsIdentity = new ClaimsIdentity(
                claims,
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(3),
            };

            await HttpContext.SignInAsync(
                Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
                authProperties);
            return Ok("Sucessfully logged in");
        }

    }
}
