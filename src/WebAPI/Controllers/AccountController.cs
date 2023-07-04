using Microsoft.AspNetCore.Mvc;
using NeoServer.Web.API.Services.Interfaces;
using NeoServer.Web.Shared.ViewModels.Request;

namespace NeoServer.Web.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : BaseController
{
    private readonly IAccountApiService _accountApiService;

    public AccountController(IAccountApiService accountApiService)
    {
        _accountApiService = accountApiService;
    }

    [HttpPost]
    public async Task<IActionResult> Post(AccountPostRequest request)
    {
        await _accountApiService.Create(request);
        return Ok();
    }
}