using Chinook.Application.Interfaces;
using Chinook.Infrastructure.Repositories;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 1. Читаем строку подключения
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Создаем DataSource (менеджер пула соединений)
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();

// 3. Регистрируем DataSource в DI (Singleton)
builder.Services.AddSingleton(dataSource);

// 4. Регистрируем наш репозиторий (Scoped - создается на каждый HTTP-запрос)
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
