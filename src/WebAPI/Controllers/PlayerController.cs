using Microsoft.AspNetCore.Mvc;
using NeoServer.Web.API.Services.Interfaces;
using NeoServer.Web.Shared.ViewModels.Request;

namespace NeoServer.Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : BaseController
{
    #region private members

    private readonly IPlayerApiService _playerService;

    #endregion

    #region constructor

    public PlayerController(IPlayerApiService playerService)
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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PlayerPostRequest player)
    {
        await _playerService.Create(player);
        return Ok();
    }

    #endregion
}