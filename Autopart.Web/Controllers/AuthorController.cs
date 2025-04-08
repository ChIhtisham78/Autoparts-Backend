using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
    [Route("api/Author")]
    [ApiController]
   
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [Authorize]
        [HttpPost("author")]
        public async Task<ActionResult<Author>> CreateAuthor([FromQuery] RequestAuthorDto request)
        {
            var createdAuthor = await _authorService.CreateAuthorAsync(request);

            return CreatedAtAction(
                    nameof(GetAuthorById),
                    new { id = createdAuthor.Id },
                    createdAuthor
            );
        }

        [HttpGet("authors/{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return Ok();
            }
            return Ok(author);
        }

        [HttpGet("slug/{slug}")]
        public async Task<ActionResult<AuthorDto>> GetAuthorBySlug(string slug)
        {
            var author = await _authorService.GetAuthorBySlugAsync(slug);
            if (author == null)
            {
                return Ok();
            }
            return Ok(author);
        }

        [HttpGet("author")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAllAuthors()
        {
            var authors = await _authorService.GetAuthorsAsync();
            return Ok(authors);
        }


        [Authorize]
        [HttpPut("author")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromQuery] RequestAuthorDto request)
        {
            request.authorDto.Id = id;
            await _authorService.UpdateAuthorAsync(request);
            return Ok();
        }


        [Authorize]
        [HttpDelete("author/{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorService.RemoveAuthorAsync(id);
            return Ok();
        }
    }
}
