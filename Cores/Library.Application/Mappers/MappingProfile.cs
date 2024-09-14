using AutoMapper;
using Library.Application.DTOs;
using Library.Domain.Entities;

namespace Library.Application.Mappers;
public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<Book, BookDTO>()
            .ForMember(dest => dest.ISBN, opt => opt.MapFrom(src => src.ISBN))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageFileName, opt => opt.MapFrom(src => src.ImageFileName))
            .ForMember(dest => dest.ImageFile, opt => opt.Ignore());
        CreateMap<BookDTO, Book>()
            .ForMember(dest => dest.ISBN, opt => opt.MapFrom(src => src.ISBN))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageFileName, opt => opt.MapFrom(src => src.ImageFileName))
            .ForMember(dest => dest.Author, opt => opt.Ignore())
            .ForMember(dest => dest.StartRentDateTime, opt => opt.Ignore())
            .ForMember(dest => dest.EndRentDateTime, opt => opt.Ignore());
       
        CreateMap<Author, AuthorDTO>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country));
        CreateMap<AuthorDTO, Author>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => src.Birthday))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.Books, opt => opt.Ignore()); 
    }
}