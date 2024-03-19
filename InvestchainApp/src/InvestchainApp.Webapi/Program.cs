using InvestChainApp.Application.Infastructure;
using InvestChainApp.Application.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
// See appsettings.json for configuration.
builder.Services.AddDbContext<InvestChainContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
        o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));

builder.Services.AddScoped<UserRepository>();
// Add automapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.
builder.Services.AddControllers();
// Der Vue.JS Devserver l�uft auf einem anderen Port, deswegen brauchen wir diese Konfiguration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
}

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.OnAppendCookie = cookieContext =>
    {
        cookieContext.CookieOptions.Secure = true;
        cookieContext.CookieOptions.SameSite = builder.Environment.IsDevelopment() ? SameSiteMode.None : SameSiteMode.Strict;
    };
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return System.Threading.Tasks.Task.CompletedTask;
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    using (var db = scope.ServiceProvider.GetRequiredService<InvestChainContext>())
    {
        if (app.Environment.IsDevelopment())
            db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        if (app.Environment.IsDevelopment())
            db.Seed();
    }
}

app.UseHttpsRedirection();
if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Liefert die statischen Dateien, die von VueJS generiert werden, aus.
app.UseStaticFiles();
app.UseCookiePolicy();
// Ab hier werden alle calls bearbeitet, die an die api gehen.
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Wichtig f�r das clientseitige Routing, damit wir direkt an eine URL in der Client App steuern k�nnen.
app.MapFallbackToFile("index.html");
app.Run();
