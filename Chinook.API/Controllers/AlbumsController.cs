using Chinook.Application.Interfaces;
using Chinook.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AlbumsController(IAlbumRepository _albumRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? artistId = null, CancellationToken cancellationToken = default)
    {
        var albums = await _albumRepository.GetAllAsync(artistId, cancellationToken);

        // Возвращаем 200 OK даже если список пустой (правильный REST)
        return Ok(albums);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var album = await _albumRepository.GetByIdAsync(id, cancellationToken);
        if (album is null)
            // Not found стандарт для поиска по Id 
            return NotFound(new { message = $"Альбом с ID {id} не найден." }); 

        return Ok(album);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Album album, CancellationToken cancellationToken = default)
    {
        try
        {
            var createdAlbum = await _albumRepository.CreateAsync(album, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdAlbum.AlbumId }, createdAlbum);
        }
        catch (InvalidOperationException ex)
        {
            // Возвращаем чистый 400 Bad Request с понятным сообщением
            return BadRequest(new { message = ex.Message });
        }
    }

}
