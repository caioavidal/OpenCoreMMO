using AutoMapper;
using NeoServer.Data.Entities;
using NeoServer.Data.Interfaces;
using NeoServer.Web.API.Services.Interfaces;
using NeoServer.Web.Shared.ViewModels.Request;

namespace NeoServer.Web.API.Services;

public class AccountApiService : BaseApiService, IAccountApiService
{
    private readonly IAccountRepository _accountRepository;

    public AccountApiService(IMapper mapper, IAccountRepository accountRepository) : base(mapper)
    {
        _accountRepository = accountRepository;
    }

    public async Task Create(AccountPostRequest request)
    {
        await _accountRepository.Insert(new AccountEntity
        {
            Password = request.Password,
            CreatedAt = DateTime.UtcNow,
            EmailAddress = request.Email,
            PremiumTime = request.PremiumDays,
            AllowManyOnline = false
        });
    }
}