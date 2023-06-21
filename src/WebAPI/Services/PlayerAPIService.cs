using AutoMapper;
using NeoServer.Data.Interfaces;
using NeoServer.Web.API.Services.Interfaces;
using NeoServer.Web.Shared.ViewModels.Response;

namespace NeoServer.Web.API.Services;

public class PlayerAPIService : BaseAPIService, IPlayerAPIService
{
    #region private members

    private readonly IPlayerRepository _playeRepository;

    #endregion

    #region constructors

    public PlayerAPIService(
        IMapper mapper,
        IPlayerRepository playeRepository) : base(mapper)
    {
        _playeRepository = playeRepository;
    }

    #endregion constructors

    #region public methods implementations

    public async Task<IEnumerable<PlayerResponseViewModel>> GetAll()
    {
        var players = await _playeRepository.GetAllAsync();
        var response = _mapper.Map<IEnumerable<PlayerResponseViewModel>>(players);
        return response;
    }

    public async Task<PlayerResponseViewModel> GetById(int playerId)
    {
        var player = await _playeRepository.GetAsync(playerId);
        var response = _mapper.Map<PlayerResponseViewModel>(player);
        return response;
    }

    #endregion
}