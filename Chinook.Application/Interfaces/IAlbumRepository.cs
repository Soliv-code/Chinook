using Chinook.Domain.Entities;

namespace Chinook.Application.Interfaces;

public interface IAlbumRepository
{
    Task<IEnumerable<Album>> GetAllAsync(int? artistId = null, CancellationToken cancellationToken = default);
    Task<Album?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Album> CreateAsync(Album album, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Album album, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
