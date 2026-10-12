using Chinook.Application.Interfaces;
using Chinook.Domain.Entities;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Chinook.Infrastructure.Repositories;

public class AlbumRepository(
    NpgsqlDataSource _dataSource,
    ILogger<AlbumRepository> _logger
    ) : IAlbumRepository
{
    public async Task<IEnumerable<Album>> GetAllAsync(int? artistId = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        const string sql = """
            SELECT album_id AS AlbumId, 
                   title AS Title, 
                   artist_id AS ArtistId 
            FROM album 
            WHERE (@ArtistId IS NULL OR artist_id = @ArtistId)
            ORDER BY album_id
            """;
        return await connection.QueryAsync<Album>(sql, new { ArtistId = artistId });
    }
    public async Task<Album?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        
        const string sql = """
            SELECT  album_id AS AlbumId, 
                    title AS Title, 
                    artist_id AS ArtistId 
            FROM album WHERE album_id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<Album>(sql, new { Id = id });
    }

    public async Task<Album> CreateAsync(Album album, CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        const string sql = """
            INSERT INTO album (title, artist_id) 
            VALUES (@Title, @ArtistId) 
            RETURNING album_id AS AlbumId, title AS Title, artist_id AS ArtistId
            """;
        try
        {
            return await connection.QueryFirstAsync<Album>(sql, album);
        }
        // Превращаем сырую ошибку БД в понятную бизнес-ошибку
        catch (PostgresException ex) when (ex.SqlState == "23503") // Foreign Key Violation
        {
            _logger.LogWarning("Попытка создать альбом для несуществующего исполнителя. ArtistId: {ArtistId}", album.ArtistId);
            throw new InvalidOperationException($"Не удалось создать альбом: исполнитель с ID {album.ArtistId} не существует.");
        }
        catch (PostgresException ex) when (ex.SqlState == "23505") // Unique Violation
        {
            _logger.LogWarning("Попытка создать дубликат альбома. Title: '{Title}', ArtistId: {ArtistId}", album.Title, album.ArtistId);
            throw new InvalidOperationException($"Альбом с названием '{album.Title}' уже существует у этого исполнителя.");
        }
    }

    public Task<bool> UpdateAsync(Album album, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
