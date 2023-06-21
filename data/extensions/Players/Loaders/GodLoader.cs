using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Player;
using NeoServer.Game.World;
using NeoServer.Loaders.Players;
using Serilog;

namespace NeoServer.Extensions.Players.Loaders;

public class GodLoader : PlayerLoader
{
    public GodLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
        ChatChannelFactory chatChannelFactory, IGuildStore guildStore,
        IVocationStore vocationStore, IMapTool mapTool, IWalkToMechanism walkToMechanism, World world, ILogger logger) :
        base(itemFactory, creatureFactory, chatChannelFactory, guildStore,
            vocationStore, mapTool, walkToMechanism, world, logger)
    {
    }

    public override bool IsApplicable(PlayerModel player)
    {
        return player?.PlayerType == 3;
    }

    public override IPlayer Load(PlayerModel playerModel)
    {
        if (Guard.IsNull(playerModel)) return null;
        
        if (!_world.TryGetTown((ushort)playerModel.TownId, out var town))
            _logger.Error("Town of player not found: {PlayerModelTownId}", playerModel.TownId);

        var newPlayer = new God(
            (uint)playerModel.PlayerId,
            playerModel.Name,
            _vocationStore.Get(playerModel.Vocation),
            playerModel.Gender,
            playerModel.Online,
            ConvertToSkills(playerModel),
            new Outfit
            {
                Addon = (byte)playerModel.LookAddons,
                Body = (byte)playerModel.LookBody,
                Feet = (byte)playerModel.LookFeet,
                Head = (byte)playerModel.LookHead,
                Legs = (byte)playerModel.LookLegs,
                LookType = (byte)playerModel.LookType
            },
            playerModel.Speed,
            new Location((ushort)playerModel.PosX, (ushort)playerModel.PosY, (byte)playerModel.PosZ),
            _mapTool,
            town)
        {
            AccountId = (uint)playerModel.AccountId,
            Guild = _guildStore.Get((ushort)(playerModel.GuildMember?.GuildId ?? 0)),
            GuildLevel = (ushort)(playerModel.GuildMember?.RankId ?? 0)
        };

        SetCurrentTile(newPlayer);

        newPlayer.AddInventory(ConvertToInventory(newPlayer, playerModel));
        var god = _creatureFactory.CreatePlayer(newPlayer);

        return god;
    }
}