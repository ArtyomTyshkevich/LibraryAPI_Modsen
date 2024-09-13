using AutoMapper;
using Library.Application.DTOs;
using Library.Domain.Entities;

namespace Library.Application.Mappers;
public class MappingProfile : Profile
{

    public MappingProfile()
    {

        CreateMap<Book, BookDTO>();
        CreateMap<BookDTO, Book>();
    }
}