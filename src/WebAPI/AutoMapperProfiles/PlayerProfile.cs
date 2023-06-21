using AutoMapper;
using NeoServer.Data.Model;
using NeoServer.Web.Shared.ViewModels.Response;

namespace NeoServer.Web.API.AutoMapperProfiles;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        CreateMap<PlayerModel, PlayerResponseViewModel>();
        CreateMap<PlayerResponseViewModel, PlayerModel>();
    }
}
