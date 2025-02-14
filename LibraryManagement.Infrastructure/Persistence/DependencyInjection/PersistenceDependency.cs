using System;
using LibraryManagement.Application.Contracts;
using LibraryManagement.Infrastructure.Persistence.DataContext;
using LibraryManagement.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure.Persistence.DependencyInjection;


public static class PersistenceDependency
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database Context (DbContext) Registration
        services.AddDbContext<LibraryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), // Assuming "DefaultConnection" is your PostgreSQL connection string name
                builder => builder.MigrationsAssembly(typeof(PersistenceDependency).Assembly.FullName))); // Configure PostgreSQL and Migrations Assembly

        // 2. Repository Registrations
        services.AddScoped<IBookRepository, BookRepository>();
        // services.AddScoped<IAuthorRepository, AuthorRepository>(); // Register other repositories similarly
        // services.AddScoped<IMemberRepository, MemberRepository>();


        return services;
    }
}