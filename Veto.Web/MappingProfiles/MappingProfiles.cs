using AutoMapper;
using Veto.Application.Clients.UseCases;
using Veto.Web.Models;

namespace Veto.Web.MappingProfiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<CreateClientDto, CreateClientUseCaseRequest>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                ;
        }
    }
}
