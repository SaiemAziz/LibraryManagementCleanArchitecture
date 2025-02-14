using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;
    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/books/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BookDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookDetails(Guid id)
    {
        var query = new GetBookDetailsQuery(id);
        var bookDetailsDto = await _mediator.Send(query);

        if (bookDetailsDto == null)
        {
            return NotFound();
        }

        return Ok(bookDetailsDto);
    }

    // POST: api/books/borrow
    [HttpPost("borrow")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BorrowBook([FromBody] BorrowBookCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidInputException ex)
        {
            return BadRequest(new { errors = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            // Log unexpected error (important in real applications)
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "An unexpected error occurred." });
        }
    }

    // ... Other API actions (e.g., POST api/books/return, GET api/books, etc.)
}
