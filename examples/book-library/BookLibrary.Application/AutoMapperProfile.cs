using AutoMapper;
using BookLibrary.Application.Models;
using BookLibrary.Domain;

namespace BookLibrary.Application;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AddBookRequest, Book>();
        CreateMap<PutBookRequest, Book>();
        
        CreateMap<AddPublisherRequest, Publisher>();
        CreateMap<PutPublisherRequest, Publisher>();
        
        CreateMap<AddAuthorRequest, Author>();
        CreateMap<PutAuthorRequest, Author>();
    }
}
