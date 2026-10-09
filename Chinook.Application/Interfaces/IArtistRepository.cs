using Chinook.Domain.Chinook.Domain.Entities;

namespace Chinook.Application.Interfaces;

public interface IArtistRepository
{
    Task<IEnumerable<Artist>> GetAllAsync(string sortBy = "artist_id", CancellationToken cancellationToken = default);
    Task<Artist?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    // Новые методы:
    Task<Artist> CreateAsync(Artist artist, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Artist artist, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
