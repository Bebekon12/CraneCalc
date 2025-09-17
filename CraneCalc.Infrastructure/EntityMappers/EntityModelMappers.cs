using AutoMapper;
using CraneCalc.Domain.Models;
using CraneCalc.Infrastructure.Entities;

namespace CraneCalc.Infrastructure.EntityMappers;

public class EntityModelMappers : Profile
{
    public EntityModelMappers()
    {
        CreateMap<Cart, CartEntity>();
        CreateMap<CartEntity, Cart>();
        
        CreateMap<Cargo, CargoEntity>();
        CreateMap<CargoEntity, Cargo>();
        
        CreateMap<CartCargo, CartCargoEntity>()
            .ForMember(dest=>dest.Cargo, opt=>opt.Ignore());
        CreateMap<CartCargoEntity, CartCargo>()
            .ForMember(dest=>dest.Cargo, opt=>opt.Ignore());

        CreateMap<UserEntity, User>();
        CreateMap<User, UserEntity>();
    }
}