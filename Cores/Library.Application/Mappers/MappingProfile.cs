using AutoMapper;
using Library.Domain.DTOs;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Library.Application.Mappers;
public class MappingProfile : Profile
{

    public MappingProfile()
    {

        CreateMap<Book, BookDTO>();
        CreateMap<BookDTO, Book>();
    }
}