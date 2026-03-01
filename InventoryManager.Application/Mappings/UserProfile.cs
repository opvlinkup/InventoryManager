using AutoMapper;
using InventoryManager.Application.DTO;
using InventoryManager.Domain.Models;

namespace InventoryManager.Application.Mappings;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.Name} {src.Surname}"))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest =>  dest.LastSeenAt, opt => opt.MapFrom(src => src.Sessions.OrderByDescending(s => s.LastUsedAt).FirstOrDefault()!.LastUsedAt))
            .ForMember(dest => dest.Theme, opt => opt.MapFrom(src => src.Theme.ToString()))
            .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language.ToString()));
    }
}