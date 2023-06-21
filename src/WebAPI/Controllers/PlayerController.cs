using Microsoft.AspNetCore.Mvc;
using NeoServer.API.Services;

namespace NeoServer.API.Controllers;

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

    [HttpGet()]
    public async Task<IActionResult> GetAllAsync()
        => Response(await _playerService.GetAll());

    [HttpGet("{playerId}")]
    public async Task<IActionResult> GetById([FromRoute] int playerId)
        => Response(await _playerService.GetById(playerId));

    #endregion
}