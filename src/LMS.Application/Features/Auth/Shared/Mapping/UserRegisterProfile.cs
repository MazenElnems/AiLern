using AutoMapper;
using LMS.Application.Features.Auth.Commands.Register;
using LMS.Domain.Entities.Users;

namespace LMS.Application.Features.Auth.Shared.Mapping;

public class UserRegisterProfile : Profile
{
    public UserRegisterProfile()
    {
        CreateMap<RegisterUserCommand, ApplicationUser>()
            .ForMember(d => d.Role, opt => opt.MapFrom(s => s.Role.ToString()));
    }
}
