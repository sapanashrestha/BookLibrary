using AutoMapper;
using BookLibrary.DTO;
using BookLibrary.Model;

namespace BookLibrary.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Books, GetBooksDTO>();
        }
    }
}
