using AutoMapper;
using NeoServer.Web.Shared.ViewModels.Response;
using NeoServer.Data.Model;

namespace NeoServer.API.AutoMapperProfiles;

public class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        CreateMap<PlayerModel, PlayerResponseViewModel>();
        CreateMap<PlayerResponseViewModel, PlayerModel>();
    }
}
