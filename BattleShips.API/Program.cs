using BattleShips.API.Data.Access;
using BattleShips.API.Data.Access.Repositories;
using BattleShips.API.Data.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepository<Game>, GameRepository>();
builder.Services.AddScoped<IRepository<Board>, BoardRepository>();
builder.Services.AddScoped<IRepository<Player>, PlayerRepository>();
builder.Services.AddScoped<IRepository<Ship>, ShipRepository>();
builder.Services.AddScoped<IRepository<ShipType>, ShipTypeRepository>();

builder.Services.AddDbContext<BattleShipsContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Game-DbContext"));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BattleShipsContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
