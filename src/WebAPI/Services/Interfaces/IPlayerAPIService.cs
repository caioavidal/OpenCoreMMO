using NeoServer.Web.Shared.ViewModels.Response;

namespace NeoServer.Web.API.Services.Interfaces;

public interface IPlayerAPIService
{
    Task<IEnumerable<PlayerResponseViewModel>> GetAll();
    Task<PlayerResponseViewModel> GetById(int playerId);
}