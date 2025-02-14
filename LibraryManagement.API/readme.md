## Folder Structure

```
LibraryManagement.API/
├── Controllers/
├── Middlewares/
├── Filters/
├── Utilities/
├── Program.cs
├── appsettings.json
├── WebApi.csproj
└── [OtherProjectFiles]
```

## Contollers

- Purpose: To create endpoints and map with terminal middlewares.
- #### Create BooksController.cs in Controller folder.
- Entry Point for Book-Related Requests: Handles HTTP requests related to books (e.g., getting book details, borrowing books).
  Orchestration: Receives requests, translates them into Commands or Queries, and uses MediatR to send them to the Application Layer for processing.
- Response Handling: Receives results (DTOs or void for commands) from the Application Layer and constructs appropriate HTTP responses (JSON data, status codes).
- Error Handling: Catches exceptions from the Application Layer and translates them into relevant HTTP error responses (e.g., 400 Bad Request for validation errors, 404 Not Found, 500 Internal Server Error for unexpected issues).
- Clean Architecture Role: Part of the Presentation Layer. It depends on the Application Layer (through MediatR and DTOs) but has no direct dependency on the Domain or Infrastructure Layers. It's responsible for the presentation logic - how the application interacts with the outside world via HTTP.
- #### Create AuthorsController.cs in Controller folder.
- Entry Point for Author-Related Requests: Handles HTTP requests related to authors (e.g., getting author details, listing authors).
- Similar Structure to BooksController: Follows the same pattern as BooksController: using IMediator for sending Queries and Commands, handling responses, and basic error handling.
- Demonstrates Multiple Actions: Shows both GetAuthorDetails (GET by ID) and ListAuthors (GET all) actions.
- Clean Architecture Role: Also part of the Presentation Layer, adhering to the same principles as BooksController.

## Middlewares

- Purpose: Create non-terminal middlewares.
- #### Create a file named ExceptionHandlingMiddleware.cs inside the Middlewares folder
- Global Exception Handling: Provides a centralized way to catch and handle exceptions that occur during the processing of any HTTP request in your API.
- HTTP Pipeline Component: Middleware is part of the ASP.NET Core HTTP request pipeline. It intercepts requests and responses.
- Logging: Logs unhandled exceptions using ILogger for debugging and monitoring.
- Error Response Formatting: Formats exceptions into consistent JSON error responses for clients (in this example, a generic 500 Internal Server Error with a user-friendly message).
- Clean Architecture Role: Part of the Presentation Layer. It deals with a cross-cutting concern (exception handling) at the API level, ensuring consistent error responses without cluttering controllers with repetitive try-catch blocks for generic exceptions.
- #### Program.cs (Example - updated to include middleware registration)

  ```csharp
  var app = builder.Build();

  // **Register Exception Handling Middleware - register it EARLY in the pipeline**
  app.UseMiddleware<ExceptionHandlingMiddleware>(); // Register custom middleware
  ```

## Filters

- Purpose: manipulating data received from the Application Layer to ensure it's presented to the user interface in the correct format.
- #### Create a file named ValidationFilter.cs inside the Filters folder
- Request Body Validation at API Level: Provides a way to perform model validation (e.g., using Data Annotations or FluentValidation) directly within the API pipeline, before the action method is executed.
- IActionFilter Interface: Implements the IActionFilter interface, allowing it to run code before (OnActionExecuting) and after (OnActionExecuted) an action method.
- OnActionExecuting Implementation:
  - if (!context.ModelState.IsValid): Checks if the ModelState is valid. ModelState is populated by ASP.NET Core's model binding process, which includes validation based on Data Annotations on your request models (or FluentValidation if you've set it up).
  - If ModelState is invalid:
  - Extracts validation errors from context.ModelState.
  - Formats the errors into a dictionary (property name -> array of error messages).
  - Creates a BadRequestObjectResult (HTTP 400) with the error response.
  - Sets context.Result to this BadRequestObjectResult. This short-circuits the action method execution, and the filter directly returns the 400 response.
- Clean Architecture Role: Part of the Presentation Layer. It handles input validation at the API level, ensuring that controllers receive valid data. While validation can also be done in the Application Layer (e.g., using Validation Behaviors), using a Filter provides another option to handle it specifically at the API entry point.
- #### Example - Applying ValidationFilter to the BorrowBook action in BooksController.cs
  ```csharp
  [HttpPost("borrow")]
  [ServiceFilter(typeof(ValidationFilter))] // Apply ValidationFilter using ServiceFilter
  public async Task<IActionResult> BorrowBook([FromBody] BorrowBookCommand command)
  {
      // ... (BorrowBook action logic - same as before)
  }
  ```
- #### Register ValidationFilter as a Service in Program.cs

  ```csharp
  // **Register Validation Filter as a service**
  builder.Services.AddScoped<ValidationFilter>(); // Register ValidationFilter in DI


  var app = builder.Build();
  ```

  - builder.Services.AddScoped\<ValidationFilter\>();: This line registers the ValidationFilter class with the DI container, making it available to be used with [ServiceFilter].

## Utilities

- Purpose: reusable helper functions or classes that handle common tasks related to user interface display and interaction, without containing any core business logic
- #### Create a file named APIResponseHelper.cs inside the Utilities folder
- Consistent API Responses: Provides helper methods to create consistent and structured JSON responses for your API. This can improve API clarity and make it easier for clients to consume your API.
- Success and Error Responses: Includes methods for creating both success responses (with data and optional message) and error responses (with error message and status code).
- Reduced Code Duplication: Avoids repeating the same response formatting logic in multiple controllers.
- Clean Architecture Role: Part of the Presentation Layer (Utilities folder). It's a helper class to simplify response formatting within the API project.
- #### Example - Using APIResponseHelper in BooksController.cs

  ```csharp
  // GET: api/books/{id}
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetBookDetails(Guid id)
  {
      var query = new GetBookDetailsQuery(id);
      var bookDetailsDto = await _mediator.Send(query);

      if (bookDetailsDto == null)
      {
          return APIResponseHelper.CreateErrorResponse("Book not found.", StatusCodes.Status404NotFound); // Use helper for error
      }

      return APIResponseHelper.CreateSuccessResponse(bookDetailsDto); // Use helper for success
  }
  ```

## Program.cs add persistence Dependency

```csharp
// **Register Persistence Infrastructure Services using extension method**
builder.Services.AddPersistence(builder.Configuration); //  <-- Using AddPersistence extension method
```
