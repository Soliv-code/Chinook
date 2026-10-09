using Chinook.Application.Interfaces;
using Chinook.Domain.Chinook.Domain.Entities;
using Dapper;
using Npgsql;

namespace Chinook.Infrastructure.Repositories;

// DI сам подставит сюда наш Singleton DataSource из Program.cs
public class ArtistRepository(NpgsqlDataSource _dataSource) : IArtistRepository
{

    //private readonly NpgsqlDataSource _dataSource = dataSource;
    public async Task<IEnumerable<Artist>> GetAllAsync(string sortBy = "artist_id", CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        // БЕЗОПАСНАЯ сортировка: мы явно разрешаем только известные нам колонки.
        // Никакой пользовательский ввод напрямую в SQL не попадет.
        var orderByClause = sortBy.ToLower() switch
        {
            "name" => "ORDER BY name",
            _ => "ORDER BY artist_id" // Дефолтное значение
        };

        var sql = $"""
            SELECT artist_id AS ArtistId, 
                   name AS Name 
            FROM artist 
            {orderByClause}
            """;

        return await connection.QueryAsync<Artist>(sql);
    }

    public async Task<Artist?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        const string sql = """
            SELECT artist_id AS ArtistId, name AS Name 
            FROM artist 
            WHERE artist_id = @Id
            """;

        // Передаем параметры через анонимный объект new { Id = id } - Dapper сам подставит их безопасно
        return await connection.QueryFirstOrDefaultAsync<Artist>(sql, new { Id = id });
    }

    public async Task<Artist> CreateAsync(Artist artist, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        // RETURNING artist_id, name позволяет сразу получить созданную сущность с новым ID
        const string sql = """
            INSERT INTO artist (name) 
            VALUES (@Name) 
            RETURNING artist_id AS ArtistId, name AS Name
            """;

        // QueryFirstAsync выполнит вставку и сразу вернет заполненный объект Artist
        return await connection.QueryFirstAsync<Artist>(sql, artist);
    }

    public async Task<bool> UpdateAsync(Artist artist, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        
        const string sql = """
            UPDATE artist 
            SET name = @Name 
            WHERE artist_id = @ArtistId
            """;

        // ExecuteAsync возвращает количество затронутых строк. 
        // Если > 0, значит обновление прошло успешно.
        var rowsAffected = await connection.ExecuteAsync(sql, artist);
        return rowsAffected > 0;

    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
            DELETE 
            FROM artist 
            WHERE artist_id = @Id
            """;

        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
        return rowsAffected > 0;
    }
}
