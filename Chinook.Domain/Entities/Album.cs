namespace Chinook.Domain.Entities;

public record Album(
    int AlbumId, 
    string Title, 
    int ArtistId
);
