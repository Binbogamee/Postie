using AutoMapper;
using Postie.DAL.Entities;
using Postie.Dtos;
using Postie.Models;

namespace Postie.Infrastructure
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<AccountEntity, Account>().ReverseMap();
            CreateMap<Account, AccountDto>().ReverseMap().
                ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username.Trim()))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Trim()));

            CreateMap<PostEntity, Post>().ReverseMap();
            CreateMap<Post, CreatedPostDto>().ReverseMap();
        }
    }
}
