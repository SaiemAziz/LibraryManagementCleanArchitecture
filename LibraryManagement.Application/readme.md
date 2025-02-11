## Folder structure

```
LibraryManagement.Application/
├── Contracts/
├── Services/
├── Requests/
├── Exceptions/
├── Models/ // DTOs
├── Mappers/
├── Validators/
├── Specifications/
└── Behaviors/
```

## Contracts

- Purpose: This folder defines interfaces that the Application Layer depends on, but the implementations of which reside in the Infrastructure Layer or even other Application Layer components. This is a key aspect of Dependency Inversion Principle in Clean Architecture. It decouples the Application Layer from concrete implementations.
- #### Create a file named IEmailService.cs inside the Abstractions folder
- IEmailService: Defines a contract for sending emails. The Application Layer will depend on this interface, not on a concrete email sending implementation.
- SendEmailAsync: An asynchronous method to send an email, taking recipient, subject, and body as parameters.
- #### Create a file named IBookRepository.cs inside the Abstractions folder
- IBookRepository: Defines a contract for interacting with book data storage.
- It outlines common CRUD (Create, Read, Update, Delete) operations for Book entities.
- The Application Layer will use this interface to access book data, without knowing how the data is actually stored (e.g., in a database, in memory, etc.).

## Services

- Purpose: This folder contains the core application logic. Application Services (or Handlers) orchestrate the use cases of your application. They receive requests from the Presentation Layer, interact with the Domain Layer to execute business logic, and use Infrastructure services (through interfaces defined in Abstractions) to perform tasks like data persistence, sending emails, etc.
- #### Create a file named BookBorrowingService.cs inside the Application Services folder
- BookBorrowingService: An Application Service responsible for the "Borrow Book" use case.
  - Dependencies (via Constructor Injection):
  - IBookRepository: To access and update book data (implementation will be in Infrastructure).
  - IEmailService: To send email notifications (implementation will be in Infrastructure).
  - Note on IPublisher: In the BookBorrowingService, we are not directly injecting IPublisher here. Instead, in a real application, the Application Service would be responsible for publishing Domain Events after interacting with the Domain. A proper Domain Event Dispatcher pattern would be used to decouple event publishing from Entities and move it to the Application Layer. For simplicity in this example and to align with the previous Domain Entity example, we are showing the Book.BorrowBook method accepting IPublisher (though this is not ideal in a full Clean Architecture implementation). In a more complete example, the BookBorrowingService would publish the BookBorrowedEvent after calling book.BorrowBook() (which would not take IPublisher as a parameter in a more decoupled design).
- BorrowBookAsync(Guid bookId, Guid memberId):
  - Receives a request to borrow a book.
  - Retrieves the Book entity using the IBookRepository.
  - Calls the book.BorrowBook() domain method (from the Domain Layer) to execute the core business logic.
  - Persists the updated Book entity using the IBookRepository.UpdateAsync().
  - Uses the IEmailService to send a confirmation email.
  - Transaction Management: In a real application, you would typically use a Unit of Work or similar pattern to manage transactions across repository and service operations, ensuring atomicity. Transaction handling is often done in Command Handlers or at a higher level than individual Application Services.

## Requests

- Purpose: This folder defines the input models for your Application Services (or Handlers). Commands represent actions that change the system's state (e.g., create, update, delete), while Queries represent requests for data retrieval without state changes. Using Commands and Queries promotes the Command-Query Responsibility Segregation (CQRS) principle at the application layer.
- #### Create a file named BorrowBookCommand.cs inside the Commands folder
- BorrowBookCommand: Represents a command to borrow a book.
  IRequest (MediatR): Implements MediatR.IRequest (typically IRequest\<TResponse\> for queries, IRequest for commands without a direct return value). This makes it a MediatR command that can be handled by a Command Handler.
- BookId, MemberId: Properties to hold the data needed for the borrow book operation.
- #### Create a file named GetBookDetailsQuery.cs inside the Queries folder
- GetBookDetailsQuery: Represents a query to retrieve details of a book.
- IRequest\<BookDetailsDto\> (MediatR): Implements MediatR.IRequest\<BookDetailsDto\>, indicating it's a query that returns a BookDetailsDto (Data Transfer Object - defined later).
- BookId: Property to specify which book's details to retrieve.

## Exceptions

- Purpose: This folder can contain custom exception classes specific to the Application Layer. These exceptions can be used to signal errors that occur during application logic execution, distinct from domain-level exceptions (which would reside in the Domain Layer, or you might choose to use a single set of exceptions and place them in a shared "Common" project if appropriate for your project scale).
- #### Let's create an InvalidInputException
- InvalidInputException: A custom exception class to represent invalid input data at the Application Layer level.
- It inherits from System.Exception.
- Provides constructors to set the error message and inner exception (optional).
- #### Use application example

  ```c#
  // ... inside BookBorrowingService.BorrowBookAsync() ...

  if (bookId == Guid.Empty || memberId == Guid.Empty)
  {
      throw new InvalidInputException("Book ID and Member ID must be valid GUIDs.");
  }

  // ... rest of the BorrowBookAsync logic ...
  ```

## Models (DTOs)

- Purpose: This folder contains Data Transfer Objects (DTOs). DTOs are simple classes used to transfer data between layers. In the Application Layer, DTOs are typically used to:
  - Define the shape of data returned by Queries to the Presentation Layer (API).
  - Receive data from the Presentation Layer in Commands (though Commands themselves can also act as input DTOs).
  - Isolate the Presentation Layer from the Domain Model. DTOs are flat data structures and do not contain domain logic.
- #### Let's create a BookDetailsDto
- BookDetailsDto: A DTO to represent book details for presentation.
- Properties: Includes relevant book information. Note AuthorName is flattened – instead of exposing the entire Author entity, we just include the author's name as a string, simplifying the data for the Presentation Layer.
- Plain Properties: DTOs typically have simple public properties with getters and setters. They are data containers, not behavior containers.

## Mappers

- Purpose: This folder contains classes responsible for mapping between different object types, especially between Domain Entities and DTOs. Mappers help to keep the layers decoupled and avoid direct dependencies between Domain Entities and Presentation Layer models.
- **NuGet Package Installation (AutoMapper - Optional but Recommended)**
- #### Let's create a mapper to convert Book entity to BookDetailsDto
- BookMapper: A class to define mappings related to Book entities.
- Profile (AutoMapper): Inherits from AutoMapper.Profile to configure mappings.
- CreateMap\<Book, BookDetailsDto\>(): Defines a mapping from the Book entity to the BookDetailsDto.
- .ForMember(...): Shows how to customize mapping. Here, we flatten the Author entity's FirstName and LastName into the AuthorName property of the BookDetailsDto.
- #### Use application example

  ```c#
  // ... inside BookBorrowingService (or a BookRetrievalService) ...
  using AutoMapper; // Import AutoMapper

  public class BookBorrowingService
  {
      private readonly IBookRepository _bookRepository;
      private readonly IEmailService _emailService;
      private readonly IMapper _mapper; // Inject AutoMapper's IMapper

      public BookBorrowingService(IBookRepository bookRepository, IEmailService emailService, IMapper mapper)
      {
          _bookRepository = bookRepository;
          _emailService = emailService;
          _mapper = mapper; // Get IMapper via DI
      }

      public async Task<BookDetailsDto> GetBookDetailsAsync(Guid bookId)
      {
          var book = await _bookRepository.GetByIdAsync(bookId);
          if (book == null)
          {
              return null; // Or throw exception, or return default DTO, depending on requirements
          }

          // Map Book entity to BookDetailsDto using AutoMapper
          var bookDetailsDto = _mapper.Map<BookDetailsDto>(book);
          return bookDetailsDto;
      }
  }
  ```

- IMapper \_mapper is injected into the Application Service via constructor injection.
- \_mapper.Map\<BookDetailsDto\>(book): Uses AutoMapper to perform the mapping from a Book entity instance to a BookDetailsDto instance, based on the mappings configured in BookMapper.

## Validators

- Purpose: This folder contains classes responsible for validating input data in Commands and Queries (or DTOs in general). Validators ensure that data coming into the Application Layer is valid before it's processed further by Application Services or passed to the Domain Layer.
- **NuGet Package Installation (FluentValidation - Recommended)**
- #### Let's create a validator for the BorrowBookCommand
- BorrowBookCommandValidator: A class to validate BorrowBookCommand instances.
- AbstractValidator<BorrowBookCommand> (FluentValidation): Inherits from AbstractValidator to define validation rules for BorrowBookCommand.
- RuleFor(command => command.BookId): Starts defining rules for the BookId property of the command.
  - .NotEmpty().WithMessage(...): Ensures BookId is not empty and sets an error message if it is.
  - .NotEqual(Guid.Empty).WithMessage(...): Ensures BookId is not the default empty GUID value.
- Similar rules are defined for MemberId
- #### Use application example

  ```c#
  // ... inside BookBorrowingService.BorrowBookAsync() ...
    using FluentValidation;

    public async Task BorrowBookAsync(Guid bookId, Guid memberId)
    {
        var command = new BorrowBookCommand(bookId, memberId);
        var validator = new BorrowBookCommandValidator();
        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            // Handle validation errors
            var errorMessages = validationResult.Errors.Select(error => error.ErrorMessage);
            throw new InvalidInputException(string.Join(", ", errorMessages)); // Or return validation result to Presentation Layer
        }

        // ... rest of BorrowBookAsync logic ...
    }
  ```

## Behaviors

- Purpose: Behaviors are used to implement cross-cutting concerns in your Application Layer using the Mediator pattern's pipeline feature (if you are using MediatR). Behaviors are like middleware for your Application Layer. They can intercept requests (Commands and Queries) before they reach their handlers and perform actions like:
  - Validation: Automatically validate Commands and Queries using Validators.
  - Logging: Log request and response information.
  - Authorization: Check user permissions before executing handlers.
  - Transaction Management: Wrap handler execution in a transaction.
  - Caching: Implement caching for query results.
- **NuGet Package Installation (MediatR already installed)**
- #### Create a file named ValidationBehavior.cs inside the Behaviors folder
- ValidationBehavior\<TRequest, TResponse\>: A generic class implementing IPipelineBehavior\<TRequest, TResponse\> from MediatR. This is MediatR's pipeline behavior interface.
- Generic Constraints: where TRequest : IRequest ensures this behavior applies only to MediatR requests (Commands and Queries).
- Dependency Injection of Validators: The constructor takes IEnumerable\<IValidator\<TRequest\>> \_validators. This allows MediatR's dependency injection to automatically inject all registered validators for the specific TRequest type.
- Handle(...) Method: This is the core of the behavior. It's executed before the actual handler for the request.
  - if (\_validators.Any()): Checks if any validators are registered for the request type.
  - ValidationContext, validator.ValidateAsync(): Uses FluentValidation to perform validation.
  - failures.Any(): Checks if there are any validation failures.
  - throw new InvalidInputException(...): If validation fails, throws an InvalidInputException (or a custom exception to encapsulate validation errors).
  - await next(): If validation passes (or no validators are registered), it calls next() to continue the pipeline execution, which will eventually invoke the actual Command or Query Handler.
- #### Registration of Behaviors (in Application Layer's Dependency Injection Setup) in Program.cs file
  ```c#
  // Example in Startup.cs or Program.cs (Application Layer)
  builder.Services.AddMediatR(cfg => {
      cfg.RegisterServicesFromAssembly(typeof(ApplicationLayerAssemblyMarker).Assembly); // Register handlers
      cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); // Register ValidationBehavior
      // Add other behaviors here (e.g., LoggingBehavior, TransactionBehavior)
  });
  ```
  - cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)): Registers the ValidationBehavior as a pipeline behavior with MediatR. The typeof(IPipelineBehavior<,>) indicates that we are registering a behavior that implements the IPipelineBehavior interface, and typeof(ValidationBehavior<,>) specifies the concrete behavior class to use. MediatR will now automatically apply this ValidationBehavior to all requests that go through its pipeline.

## Specifications

- Purpose: Specifications are used to encapsulate complex query logic or business rules that determine whether an object satisfies certain criteria. They are particularly useful for building reusable and composable query filters and business rules, especially when dealing with collections of entities. Specifications can be used in Repositories to create dynamic queries or in Domain Services to encapsulate complex business rules.
- **NuGet Package Installation (CSharpFunctionalExtensions - Optional but Helpful)**
- #### Create a file named AvailableBooksOfGenreSpecification.cs inside the Specifications folder
- AvailableBooksOfGenreSpecification: A class to represent the specification for finding available books of a given genre.
- Genre: Property to hold the genre to filter by.
- Criteria(): Returns an Expression\<Func\<Book, bool\>\>. This is a lambda expression that represents the filtering criteria. It checks if a Book is both IsAvailable and has the specified Genre.
- #### Usage Example (in Application Service or Repository - more commonly in Repository)

  ```c#
  // ... inside BookRepository (Infrastructure Layer - see Infrastructure section later) ...

  public async Task<List<Book>> GetAvailableBooksByGenreAsync(Genre genre)
  {
      var specification = new AvailableBooksOfGenreSpecification(genre);
      return await _dbContext.Books
          .Include(b => b.Author) // Example: Eager loading Author
          .Where(specification.Criteria()) // Apply the specification criteria
          .ToListAsync();
  }

  // Usage in Application Service:
  public class BookRetrievalService
  {
      private readonly IBookRepository _bookRepository;

      public BookRetrievalService(IBookRepository bookRepository)
      {
          _bookRepository = bookRepository;
      }

      public async Task<List<BookDetailsDto>> GetAvailableFictionBooksAsync()
      {
          var books = await _bookRepository.GetAvailableBooksByGenreAsync(Genre.Fiction);
          // ... map to DTOs using Mapper ...
          return _mapper.Map<List<BookDetailsDto>>(books);
      }
  }
  ```

  - Specification in Repository: The GetAvailableBooksByGenreAsync method in a Repository (example shown in BookRepository - Infrastructure Layer) takes a Genre and uses the AvailableBooksOfGenreSpecification to build a dynamic WHERE clause for the database query.
  - Specification in Application Service: The GetAvailableFictionBooksAsync method in an Application Service uses the BookRepository method that utilizes the specification to retrieve books.
