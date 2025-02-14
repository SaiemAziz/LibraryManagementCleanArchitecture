## Folder Structure

```
LibraryManagement.Infrastructure/
├──Identity Services
├──File Storage Services
├──Queue Storage Services
├──Message Bus Services
├──Payment Services
├──Third-party Services
├──Notifications
│ └──Email Service
│ └──Sms Service
└──Persistence
  └──Data Context
  └──Repositories
  └──Data Seeding
  └──Data Migrations
  └──Caching (Distributed, In-Memory)
```

## Identity Services

- **Purpose**: Handles authentication and authorization. This might involve interacting with identity providers (like Azure AD, Okta, Auth0) or implementing your own identity management.
- Considerations for Identity Services:
  - Authentication Method: How will users be authenticated (e.g., username/password, OAuth, JWT)?
  - Authorization Policies: What are the authorization rules for different parts of the application?
  - Identity Provider: Will you use an external identity provider or manage identities internally?
  - Security: Security is paramount. Implement robust identity management practices.

## File Storage Services

- **Purpose**: Handles interactions with file storage systems (e.g., local file system, cloud storage like AWS S3, Azure Blob Storage, Google Cloud Storage).
- Considerations for File Storage Services:
  - Storage Location: Where will files be stored (local disk, cloud, network share)?
  - File Types: What types of files will be stored?
  - Access Control: How will file access be controlled and secured?
  - Scalability & Reliability: For cloud storage, consider scalability and reliability requirements.

## Queue Storage Services

- Purpose: Handles interactions with message queues (e.g., Azure Queue Storage, RabbitMQ, Amazon SQS). Message queues are used for asynchronous task processing, background jobs, and decoupling components.
- Considerations for Queue Storage Services:
  - Queue Provider: Which queue service will you use?
  - Message Format: How will messages be serialized and deserialized?
  - Message Handling: How will message processing be handled (e.g., error handling, retry policies)?
  - Scalability & Reliability: Consider queue service scalability and reliability.

## Message Bus Services

- **Purpose**: Handles communication using a message bus or message broker (e.g., RabbitMQ, Azure Service Bus, Kafka). Message buses are used for more complex asynchronous communication patterns, event-driven architectures, and microservices communication.
- Considerations for Message Bus Services:
  - Message Bus Provider: Which message bus will you use?
  - Message Broker Setup: Setting up and configuring the message broker infrastructure.
  - Message Routing & Exchange: How will messages be routed and exchanged between components?
  - Message Serialization & Deserialization: Message format and serialization.
  - Error Handling & Reliability: Message delivery guarantees, error handling, and retry mechanisms.

## Payment Services

- **Purpose**: Integrates with payment gateways (e.g., Stripe, PayPal, Square). Handles payment processing, refunds, subscriptions, etc.
- Considerations for Payment Services:
  - Payment Gateway: Which payment gateway will you use?
  - API Integration: Integrating with the payment gateway's API.
  - Security (PCI Compliance): Payment processing is highly sensitive. Security and PCI compliance are critical. Never handle sensitive payment data directly in your application if you can avoid it. Use secure payment gateways and tokenization.
  - Transaction Handling: Managing payment transactions, refunds, and error scenarios.

## Third-party Services

- **Purpose**: Integrations with any external services that don't fit neatly into other categories (e.g., Geocoding services, SMS gateways, analytics services, social media APIs, etc.).
- Considerations for Third-party Services:
  - Service API: Understanding the API of the third-party service.
  - Authentication & Authorization: How to authenticate with the service.
  - Data Mapping: Mapping data between your application and the third-party service.
  - Error Handling & Resilience: Handling API errors, rate limiting, and service outages.

## Notifications

- **Purpose**: Handles sending notifications to users through various channels (e.g., email, SMS, push notifications, in-app notifications).
- Subfolders:
  - EmailService: For email sending implementations.
  - SmsService: For SMS sending implementations.
  - You might add other subfolders for push notifications, in-app notifications, etc.

## Persistence

- **Purpose**: Handles data persistence and retrieval. This typically involves:
- Data Context: Setting up an Object-Relational Mapper (ORM) like Entity Framework Core to interact with the database.
- Repositories: Implementing repository classes that use the Data Context to perform database operations on Domain Entities.
- Data Seeding: Populating the database with initial data.
- Data Migrations: Managing database schema changes.
- Caching: Implementing caching mechanisms to improve performance (e.g., in-memory cache, distributed cache like Redis).
- Subfolders:
  - DataContext: For Entity Framework Core DbContext setup.
  - Repositories: For repository implementations (e.g., BookRepository.cs, AuthorRepository.cs).
  - DataSeeding: For data seeding logic.
  - DataMigrations: (This folder is often created by Entity Framework Core migrations tooling, not manually).
  - Caching: For caching implementations (e.g., InMemoryCacheService.cs, RedisCacheService.cs).
- #### NuGet Package Installations (PostgreSQL and Entity Framework Core)
- #### Create a file named LibraryDbContext.cs inside the Persistence/DataContext folder
- LibraryDbContext: Entity Framework Core DbContext class for our library database.
- DbContextOptions\<LibraryDbContext\> options: Constructor takes DbContextOptions for configuration (connection string, database provider, etc.).
- DbSet\<Book\> Books, DbSet\<Author\> Authors, DbSet\<Member\> Members, DbSet\<Loan\> Loans: DbSet properties for each Domain Entity and Aggregate Root, allowing EF Core to track and query these entities.
- OnModelCreating(ModelBuilder modelBuilder): Method to configure the database model using Entity Framework Core's fluent API.
  - Example configurations for relationships (Book-Author, Loan-LoanItem) and owned entities (commented out example for LoanDate if it were an owned entity instead of a Value Object). You would configure all your entity relationships, indexes, constraints, and other database schema details here.
- #### Create a file named BookRepository.cs inside the Persistence/Repositories folder
- BookRepository: Implementation of IBookRepository interface using Entity Framework Core and LibraryDbContext.
- Dependency Injection: Takes LibraryDbContext in the constructor to access the database.
- GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync: Implement CRUD operations for Book entities using EF Core methods.
  - .Include(b => b.Author): Example of eager loading the Author navigation property to avoid lazy loading issues.
  - \_dbContext.SaveChangesAsync(): Saves changes to the PostgreSQL database.
- Dependency Inversion: BookRepository implements the IBookRepository interface defined in the Application Layer. The Application Layer depends on the interface, not the concrete BookRepository implementation.
- #### Create a file named DataSeeder.cs inside the Persistence/DataSeeding folder
- DataSeeder: Class to handle database seeding.
- SeedDataAsync(IServiceProvider serviceProvider): An async method to perform data seeding.
  - IServiceProvider: Takes IServiceProvider to resolve services from DI (like LibraryDbContext).
  - using var scope = serviceProvider.CreateScope(): Creates a service scope to ensure proper disposal of services.
  - var dbContext = scope.ServiceProvider.GetRequiredService\<LibraryDbContext\>(): Resolves LibraryDbContext from the service provider.
  - await dbContext.Database.MigrateAsync(): Crucially, applies pending database migrations before seeding. This ensures the database schema is up-to-date before data seeding.
  - if (!dbContext.Authors.Any()) and if (!dbContext.Books.Any()): Checks if Authors and Books tables are empty before seeding to avoid duplicate data on subsequent application starts.
  - Seed data creation: Creates lists of Author and Book entities with sample data.
  - await dbContext.Authors.AddRangeAsync(authors) and await dbContext.Books.AddRangeAsync(books): Adds the seed data to the DbContext.
  - await dbContext.SaveChangesAsync(): Saves the seeded data to the database.

## Migrations

- Data Migrations are typically managed using Entity Framework Core's CLI tools. You would use commands like:
  - `dotnet ef migrations add InitialCreate --project Infrastructure --startup-project WebApi`: To add a new migration (e.g., "InitialCreate"). --project Infrastructure specifies the Infrastructure project where migrations will be generated, and --startup-project WebApi (or your API project) is needed to find the application's configuration and DbContext registration.
  - `dotnet ef database update --project Infrastructure --startup-project WebApi`: To apply pending migrations to the database.
