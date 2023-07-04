using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.Entities;
using NeoServer.Loaders.Guilds;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Commands;
using NeoServer.Server.Common.Contracts.Network;
using Serilog;

namespace NeoServer.Server.Commands.Player;

public class PlayerLogInCommand : ICommand
{
    private readonly ILogger _logger;
    private readonly IGameServer game;
    private readonly GuildLoader guildLoader;
    private readonly IEnumerable<IPlayerLoader> playerLoaders;

    public PlayerLogInCommand(IGameServer game, IEnumerable<IPlayerLoader> playerLoaders, GuildLoader guildLoader,
        ILogger logger)
    {
        this.game = game;
        this.playerLoaders = playerLoaders;
        this.guildLoader = guildLoader;
        _logger = logger;
    }

    public void Execute(PlayerEntity playerRecord, IConnection connection)
    {
        if (playerRecord is null)
            //todo validations here
            return;

        if (!game.CreatureManager.TryGetLoggedPlayer((uint)playerRecord.Id, out var player))
        {
            if (playerLoaders.FirstOrDefault(x => x.IsApplicable(playerRecord)) is not { } playerLoader)
                return;

            guildLoader.Load(playerRecord.GuildMember?.Guild);
            player = playerLoader.Load(playerRecord);
        }

        game.CreatureManager.AddPlayer(player, connection);

        player.Login();
        player.Vip.LoadVipList(playerRecord.Account.VipList.Select(x => ((uint)x.PlayerId, x.Player?.Name)));
        _logger.Information("Player {PlayerName} logged in", player.Name);
    }
}