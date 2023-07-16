using NeoServer.Web.Shared.ViewModels.Request;

namespace NeoServer.Web.API.Services.Interfaces;

public interface IAccountApiService
{
    Task Create(AccountPostRequest request);
}