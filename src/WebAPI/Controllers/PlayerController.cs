using Microsoft.AspNetCore.Mvc;
using NeoServer.Web.API.Services.Interfaces;

namespace NeoServer.Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : BaseController
{
    #region private members

    private readonly IPlayerAPIService _playerService;

    #endregion

    #region constructor

    public PlayerController(IPlayerAPIService playerService)
    {
        _playerService = playerService;
    }

    #endregion

    #region public methods

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Response(await _playerService.GetAll());
    }

    [HttpGet("{playerId}")]
    public async Task<IActionResult> GetById([FromRoute] int playerId)
    {
        return Response(await _playerService.GetById(playerId));
    }

    #endregion
}