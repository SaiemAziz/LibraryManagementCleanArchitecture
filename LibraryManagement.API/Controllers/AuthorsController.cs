using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthorsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    // GET: api/authors/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAuthorDetails(Guid id)
    {
        // Assuming you have a query like GetAuthorDetailsQuery
        var query = new GetAuthorDetailsQuery(id);
        var authorDto = await _mediator.Send(query);

        if (authorDto == null)
        {
            return NotFound();
        }

        return Ok(authorDto);
    }

    // GET: api/authors
    [HttpGet]
    public async Task<IActionResult> ListAuthors()
    {
        // Assuming you have a query like ListAuthorsQuery
        var query = new ListAuthorsQuery();
        var authorsDtoList = await _mediator.Send(query);

        return Ok(authorsDtoList);
    }

    // ... (You would add POST, PUT, DELETE actions for Author management - e.g., AddAuthor, UpdateAuthor, DeleteAuthor, using Commands)
}
