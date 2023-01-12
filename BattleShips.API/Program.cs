using BattleShips.API.Data.Access;
using BattleShips.API.Data.Access.Repositories;
using BattleShips.API.Data.Models;
using BattleShips.API.Library;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepository<Game>, GameRepository>();
builder.Services.AddScoped<IRepository<Board>, BoardRepository>();
builder.Services.AddScoped<IRepository<Player>, PlayerRepository>();
builder.Services.AddScoped<IRepository<Ship>, ShipRepository>();
builder.Services.AddScoped<IRepository<ShipType>, ShipTypeRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

builder.Services.AddHealthChecks();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddDbContext<BattleShipsContext>(options =>
{
    options.UseSqlServer("name=ConnectionStrings:Db-ConnString");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowSpecificOriginPolicy",
        policy =>
        {
            policy
            .WithOrigins(
                "https://bsstaticstorage.z6.web.core.windows.net", 
                "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BattleShipsContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOriginPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
