using Chinook.Application.Interfaces;
using Chinook.Domain.Chinook.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Chinook.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ArtistsController(IArtistRepository _artistRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string sortBy = "artist_id", CancellationToken cancellationToken = default)
    {
        var artists = await _artistRepository.GetAllAsync(sortBy, cancellationToken);
        return Ok(artists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var artist = await _artistRepository.GetByIdAsync(id, cancellationToken);
        if (artist is null) return NotFound($"Исполнитель с ID: {id} не найден!");
        return Ok(artist);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Artist artist, CancellationToken cancellationToken)
    {
        // Dapper сам создаст объект Artist из JSON тела запроса
        var createdArtist = await _artistRepository.CreateAsync(artist, cancellationToken);

        // 201 Created - правильный статус для создания ресурса. 
        // Также возвращаем путь к новому ресурсу в заголовке Location.
        return CreatedAtAction(nameof(GetById), new { id = createdArtist.ArtistId }, createdArtist);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Artist artistDto, CancellationToken cancellationToken)

    {
        // Мы игнорируем artistDto.ArtistId и принудительно создаем новый record 
        // с ID из URL и Name из тела запроса. Это безопасно и чисто.
        var artistToUpdate = new Artist(ArtistId: id, Name: artistDto.Name);

        var isUpdated = await _artistRepository.UpdateAsync(artistToUpdate, cancellationToken);
        if (!isUpdated)
        {
            return NotFound(new { message = $"Исполнитель с ID {id} не найден для обновления." });
        }

        return NoContent(); // 204 No Content
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _artistRepository.DeleteAsync(id, cancellationToken);
        if (!isDeleted)
        {
            return NotFound(new { message = $"Исполнитель с ID {id} не найден для удаления." });
        }

        return NoContent(); // 204 No Content
    }

}
