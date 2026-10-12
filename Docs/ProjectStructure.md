```text
Chinook/
├── BrunoCollection
│   └── Chinook API
│       ├── Albums
│       │   ├── CreateAlbum.bru
│       │   ├── folder.bru
│       │   ├── GetAllAlbums.bru
│       │   ├── GetAllAlbums_with_artistId.bru
│       │   └── GetAllbumById.bru
│       ├── Artists
│       │   ├── CreateArtist.bru
│       │   ├── DeleteArtist.bru
│       │   ├── folder.bru
│       │   ├── GetAllArtists.bru
│       │   ├── GetAllArtists_Sort_by_name.bru
│       │   ├── GetAtristById.bru
│       │   └── UpdateArtist.bru
│       ├── Tests
│       │   ├── API Test.bru
│       │   └── folder.bru
│       ├── .gitignore
│       └── bruno.json
├── Chinook.API
│   ├── Controllers
│   │   ├── AlbumsController.cs
│   │   ├── ArtistsController.cs
│   │   └── PingController.cs
│   ├── Properties
│   │   └── launchSettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── Chinook.API.csproj
│   ├── Chinook.API.http
│   ├── Dockerfile
│   └── Program.cs
├── Chinook.Application
│   ├── Interfaces
│   │   ├── IAlbumRepository.cs
│   │   └── IArtistRepository.cs
│   └── Chinook.Application.csproj
├── Chinook.Domain
│   ├── Entities
│   │   ├── Album.cs
│   │   └── Artist.cs
│   └── Chinook.Domain.csproj
├── Chinook.Infrastructure
│   ├── Repositories
│   │   ├── AlbumRepository.cs
│   │   └── ArtistRepository.cs
│   └── Chinook.Infrastructure.csproj
├── Docker
│   ├── SQL
│   │   └── init.sql
│   └── docker-compose.yml
├── Docs
│   ├── ProjectStructure.md
│   └── Todo.md
├── .gitattributes
├── .gitignore
├── Chinook.slnx
└── README.md
```
