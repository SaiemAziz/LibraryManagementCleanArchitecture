using System;
using AutoMapper;
using LibraryManagement.Application.Models;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Mappers;

public class BookMapper : Profile // Inherit from AutoMapper.Profile
{
    public BookMapper()
    {
        CreateMap<Book, BookDetailsDto>() // Define mapping from Book to BookDetailsDto
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => $"{src.Author.FirstName} {src.Author.LastName}")); // Flatten Author name
        // Add other mappings as needed
    }
}
