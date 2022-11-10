using AutoMapper;
using CartingService.Contracts;
using CartingService.Domain.Entities;

namespace CartingService.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Item, ItemDto>();
            CreateMap<ItemDto, Item>();
            CreateMap<CreateItemDto, Item>();
            CreateMap<UpdateItemDto, Item>();
            CreateMap<Cart, CartDto>();
            CreateMap<CartDto, Cart>();
            CreateMap<CreateCartDto, Cart>();
        }
    }
}
