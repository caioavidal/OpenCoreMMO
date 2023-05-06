using NeoServer.Web.Shared.ViewModels.Response;

namespace NeoServer.API.Services;

public interface IPlayerAPIService
{
    Task<IEnumerable<PlayerResponseViewModel>> GetAll();
    Task<PlayerResponseViewModel> GetById(int playerId);
}
