## Folder structure

```
LibraryManagement.Domain/
├── Entities/
├── Aggregates/
│ └── Loan/ // Subfolder for Loan Aggregate
├── ValueObjects/
├── DomainEvents/
├── Enums/
├── Constants/
└── Exceptions/ // Optional: For Domain-Specific Exceptions
```

## Entities

- **Purpose**: Entities represent the core objects in your domain. They have a unique identity that persists over time and are mutable (their properties can change). Entities encapsulate business logic and data related to a specific concept.
- ### Create a file named **Author.cs** inside the Entities folder
- Author Entity: Represents an author of books.
- Id: Unique identifier.
- FirstName, LastName: Author's name.
- Biography: Optional biography.
- Constructor: Initializes an Author with required name information.
- UpdateBiography(): Example of domain behavior, allowing updates to the author's biography.
- ### Create a file named **Book.cs** inside the Entities folder
- Book Entity: Represents a book in the library.
- Properties: Includes ISBN, Title, AuthorId, Author (navigation), PublicationYear, Genre, IsAvailable.
- AuthorId & Author: Establishes a relationship with the Author entity.
- Genre (Enum): Uses the Genre enum we defined.
- IsAvailable: Tracks book availability.
- BorrowBook(Guid memberId, IPublisher publisher):
  - Domain behavior for borrowing a book.
  - Includes a check for availability.
  - IPublisher publisher Injection: We are now injecting IPublisher (from MediatR) into the BorrowBook method. Important: In a real Clean Architecture application, you would typically avoid injecting infrastructure concerns like IPublisher directly into Domain Entities. A better approach would be to raise the Domain Event within the Entity and then have an Application Service publish the event. However, for simplicity in this example and to demonstrate Domain Events, we're showing direct publishing from the Entity method. In a more complex application, consider using a Domain Event Dispatcher pattern or similar to decouple event publishing from the Entity.
  - publisher.Publish(new BookBorrowedEvent(...)): Publishes a BookBorrowedEvent when a book is successfully borrowed.
- ReturnBook(): Domain behavior for returning a book.
- UpdateDetails(): Example of another domain behavior to update book information.
- ### Create a file named **Member.cs** inside the Entities folder
- Member Entity: Represents a library member.
- Properties: MemberNumber (unique library ID), FirstName, LastName, Email, DateOfBirth, RegistrationDate, IsActive.
- \_loans (Collection of Loan Aggregates): A member can have multiple loans. This demonstrates a relationship between the Member entity and the Loan aggregate.
- BorrowLoan(LoanDate loanDate):
  - Domain behavior for a member to start a new loan.
  - Checks if the member is active and if they have reached the maximum loan limit (using DomainConstants.MaxLoanItemsPerMember).
  - Creates a new Loan aggregate root and adds it to the member's loans collection.
- DeactivateMembership(): Example of domain behavior to deactivate a member.

## Aggregates

- Purpose: Aggregates are clusters of entities and value objects that are treated as a single unit of consistency. An aggregate has a root entity (the aggregate root) that controls access to the internal entities and value objects. Aggregates enforce business rules and transactional consistency within their boundaries.
- ### Create a file named **LoanItem.cs** inside the Aggregates folder (or Aggregates/Loan subfolder if you prefer to group Loan-related classes together)
- LoanItem Value Object: Represents a single book within a loan. We already explained this Value Object in the previous response, so this is the same code. It is placed in the Aggregates folder (or Aggregates/Loan) as it's conceptually part of the Loan aggregate.
- ### Create a file named **Loan.cs** inside the Aggregates folder (or Aggregates/Loan subfolder)
- Loan Aggregate Root: Manages LoanItem value objects and represents a loan transaction.
- Properties: MemberId, Member (navigation), \_loanItems (collection of LoanItem), LoanDate, ReturnDate, Status.
- Constructor: Initializes a new loan with a member and loan date.
- AddLoanItem(Book book, LoanDate dueDate):
- Adds a LoanItem to the loan.
  - Checks loan status and book availability.
  - Calls book.BorrowBook() to update the Book entity's state.
  - Note on IPublisher: In this example, we are passing null! for the IPublisher in book.BorrowBook(MemberId, null!). This is for demonstration purposes only. In a real application, you would need to properly resolve and pass an IPublisher instance. A more robust Clean Architecture approach would be to use a Domain Event Dispatcher pattern or similar to decouple event publishing from the Entity. The Application Layer would be responsible for publishing Domain Events after interacting with the Domain.
- ReturnLoanItem(Guid bookId, LoanDate actualReturnDate):
  - Marks a LoanItem as returned.
  - Checks loan status.
  - In a real application, you would also need to fetch the Book entity and call book.ReturnBook().
- CloseLoan(): Closes the loan and sets the ReturnDate.
- MarkAsOverdue(): Illustrative method to change the loan status to overdue. In a real system, this would likely be triggered by a background process or an Application Service based on due dates.

## ValueObjects

- Purpose: Value Objects represent descriptive aspects of the domain. They are immutable and are identified by their properties, not by identity. Two value objects are considered the same if all their properties are equal. Immutability makes them safer to use and share.
- ### Create a file named **LoanDate.cs** inside the ValueObjects folder
- LoanDate Value Object: Represents a date specifically for loans.
- Constructor Validation: Ensures the LoanDate is not in the past and not too far in the future, enforcing domain rules.
- Immutability: Date property is read-only after construction.
- Value-Based Equality: Equals and GetHashCode are overridden for proper value comparison.
- ToString(): Provides a consistent date string format.
- Implicit Operators (Optional): For convenience in certain scenarios, allowing implicit conversion to and from DateTime. Use with caution as it can sometimes reduce clarity.

## DomainEvents

- Purpose: Domain Events are notifications that something significant has happened within the domain. They allow different parts of the application (within the domain or outside) to react to domain changes in a decoupled way. Domain Events promote loose coupling and can be used for tasks like auditing, sending notifications, or triggering integrations.
- ### Create a file named BookBorrowedEvent.cs inside the DomainEvents folder
- BookBorrowedEvent: Represents the domain event that occurs when a book is borrowed.
  INotification: Implements MediatR.INotification, making it a MediatR event.
  Event Data: Contains Book, MemberId, and BorrowDate – information relevant to the "book borrowed" event.

## Enums

- Purpose: Enums define a set of named constants, representing a fixed number of possible values for a property or state within your domain. They improve code readability and type safety compared to using magic strings or integers.
- ### Create a file named **Genre.cs** inside the Enums folder
- This enum defines the possible genres a book can belong to.
- Using an enum ensures type safety and provides a clear, readable list of allowed values for the Genre property of the Book entity.
- ### Create a file named **LoanStatus.cs** inside the Enums folder
- This enum defines the different statuses a Loan aggregate can be in.
- It helps manage the lifecycle of a loan and enforce business rules based on the loan's current status.

## Constants

- Purpose: Constants are used to define fixed values that are used throughout your domain. These can be strings, numbers, or any other immutable value that has a specific meaning in your domain. Using constants improves code maintainability and readability by giving meaningful names to values.
- ### Create a file named DomainConstants.cs inside the Constants folder
- This static class DomainConstants holds constant values relevant to our library domain.
- MaxLoanDays: Defines the maximum loan duration (e.g., 30 days).
- DefaultCurrency, OverdueFinePerDay: Examples of constants that might be used in loan calculations or financial aspects of the library (though we may not fully implement these in this example, they are good illustrations).
- MaxLoanItemsPerMember: Defines a rule on how many books a member can borrow concurrently.

## Exceptions (Optional): A good place to put custom exception classes specific to your domain (e.g., BookNotAvailableException.cs). We will skip this for now to keep the example focused, but in a real-world application, it's a good practice.
