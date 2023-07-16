using AutoMapper;
using NeoServer.Data.Entities;
using NeoServer.Web.Shared.ViewModels.Response;

namespace NeoServer.Web.API.AutoMapperProfiles;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        CreateMap<PlayerEntity, PlayerResponseViewModel>();
        CreateMap<PlayerResponseViewModel, PlayerEntity>();
    }
}