using NeoServer.Data.Interfaces;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Effect;
using NeoServer.Networking.Packets.Outgoing.Map;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Server.Events.Player;

public class PlayerLoggedInEventHandler : IEventHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IGameServer game;
    private readonly IMap map;

    public PlayerLoggedInEventHandler(IMap map, IGameServer game, IAccountRepository accountRepository)
    {
        this.map = map;
        this.game = game;
        _accountRepository = accountRepository;
    }

    public async void Execute(IWalkableCreature creature)
    {
        if (creature.IsNull()) return;

        if (creature is not IPlayer player) return;

        await _accountRepository.UpdatePlayerOnlineStatus(player.Id, true);
    }

    private void SendPacketsToPlayer(IPlayer player, IConnection connection)
    {
        connection.OutgoingPackets.Enqueue(new SelfAppearPacket(player));
        connection.OutgoingPackets.Enqueue(new MapDescriptionPacket(player, map));
        connection.OutgoingPackets.Enqueue(new MagicEffectPacket(player.Location, EffectT.BubbleBlue));
        connection.OutgoingPackets.Enqueue(new PlayerInventoryPacket(player.Inventory));
        connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));
        connection.OutgoingPackets.Enqueue(new PlayerSkillsPacket(player));

        connection.OutgoingPackets.Enqueue(new WorldLightPacket(game.LightLevel, game.LightColor));

        connection.OutgoingPackets.Enqueue(new CreatureLightPacket(player));

        ushort icons = 0;
        foreach (var condition in player.Conditions) icons |= (ushort)ConditionIconParser.Parse(condition.Key);

        connection.OutgoingPackets.Enqueue(new ConditionIconPacket(icons));
    }
}