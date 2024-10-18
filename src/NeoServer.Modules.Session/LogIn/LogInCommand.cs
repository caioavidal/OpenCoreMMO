using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Contracts.Loaders;
using NeoServer.BuildingBlocks.Application.Contracts.Repositories;
using NeoServer.BuildingBlocks.Application.Enums;
using NeoServer.BuildingBlocks.Application.Server;
using NeoServer.Data.Entities;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Network;
using Serilog;

namespace NeoServer.Modules.Session.LogIn;

public record LogInCommand(
    string Account,
    string Password,
    string CharacterName,
    int ClientVersion,
    IConnection Connection) : ICommand<InvalidLoginOperation>;

public class LogInCommandHandler : ICommandHandler<LogInCommand, InvalidLoginOperation>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IGameServer _gameServer;
    private readonly IGuildLoader _guildLoader;
    private readonly ILogger _logger;
    private readonly IEnumerable<IPlayerLoader> _playerLoaders;
    private readonly ServerConfiguration _serverConfiguration;

    public LogInCommandHandler(IAccountRepository accountRepository, IGameServer gameServer,
        IEnumerable<IPlayerLoader> playerLoaders, IGuildLoader guildLoader,
        ILogger logger, ServerConfiguration serverConfiguration)
    {
        _accountRepository = accountRepository;
        _gameServer = gameServer;
        _playerLoaders = playerLoaders;
        _guildLoader = guildLoader;
        _logger = logger;
        _serverConfiguration = serverConfiguration;
    }

    public async ValueTask<InvalidLoginOperation> Handle(LogInCommand command, CancellationToken cancellationToken)
    {
        command.Deconstruct(out var account, out var password, out var characterName,
            out var version, out var connection);

        var loggedPlayerRecord = await _accountRepository.GetOnlinePlayer(account);

        _gameServer.CreatureManager.TryGetLoggedPlayer((uint)(loggedPlayerRecord?.Id ?? 0), out var loggedPlayer);

        var inputValidationResult = ValidateInputs(account, version);

        if (!inputValidationResult.IsValid()) return inputValidationResult;

        var validationResult = ValidateOnlineStatus(loggedPlayerRecord, loggedPlayer, characterName);

        if (!validationResult.IsValid())
            if (validationResult is InvalidLoginOperation.PlayerAlreadyLoggedIn)
                loggedPlayer.Logout();

        var playerRecord =
            await _accountRepository.GetPlayer(account, password, characterName);

        return LoadPlayer(playerRecord, connection);
    }

    private InvalidLoginOperation LoadPlayer(PlayerEntity playerRecord,
        IConnection connection)
    {
        if (playerRecord is null) return InvalidLoginOperation.AccountOrPasswordIncorrect;
        if (playerRecord.Account.BanishedAt is not null) return InvalidLoginOperation.AccountIsBanned;

        if (!_gameServer.CreatureManager.TryGetLoggedPlayer((uint)playerRecord.Id, out var player))
        {
            if (_playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not { } playerLoader)
                return InvalidLoginOperation.PlayerTypeNotSupported;

            _guildLoader.Load(playerRecord.GuildMember?.Guild);
            player = playerLoader.Load(playerRecord);
        }

        _gameServer.CreatureManager.AddPlayer(player, connection);

        player.LogIn();
        player.Vip.LoadVipList(playerRecord.Account.VipList.Select(x => ((uint)x.PlayerId, x.Player?.Name)));
        _logger.Information("Player {PlayerName} logged in", player.Name);

        return InvalidLoginOperation.None;
    }

    private InvalidLoginOperation ValidateInputs(string account, int version)
    {
        if (string.IsNullOrWhiteSpace(account))
            return InvalidLoginOperation.InvalidAccountName;

        if (_serverConfiguration.Version != version) return InvalidLoginOperation.ClientVersionNotAllowed;

        return _gameServer.State switch
        {
            GameState.Opening => InvalidLoginOperation.GameWorldIsStartingUp,
            GameState.Maintaining => InvalidLoginOperation.GameWorldIsUnderMaintenance,
            GameState.Closed => InvalidLoginOperation.ServerIsCurrentlyClosed,
            _ => InvalidLoginOperation.None
        };
    }

    private InvalidLoginOperation ValidateOnlineStatus(PlayerEntity playerRecord,
        IPlayer loggedPlayer, string characterName)
    {
        if (loggedPlayer is null) return InvalidLoginOperation.None;

        if (loggedPlayer.Name == characterName)
            return InvalidLoginOperation.PlayerAlreadyLoggedIn;

        if (playerRecord.Account.AllowManyOnline) return InvalidLoginOperation.None;

        return InvalidLoginOperation.CannotLogInWithMultipleCharacters;
    }
}