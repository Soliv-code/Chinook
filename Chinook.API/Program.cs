using Chinook.Application.Interfaces;
using Chinook.Infrastructure.Repositories;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
{
    // 1. DI уже работает! Мы можем попросить у него что угодно через 'sp' (IServiceProvider)
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("DefaultConnection");

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

    // 2. Достаем фабрику логирования из DI и отдаем её в Npgsql
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    dataSourceBuilder.UseLoggerFactory(loggerFactory);

    dataSourceBuilder.EnableParameterLogging(true);

    // 3. И только теперь, когда всё настроено, мы делаем Build()
    return dataSourceBuilder.Build();
});

// Регистрируем наш репозиторий (Scoped - создается на каждый HTTP-запрос)
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
 
app.Run();
