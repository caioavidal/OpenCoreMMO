﻿using NeoServer.Data.Entities;
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

public class TutorLoader : PlayerLoader
{
    public TutorLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
        ChatChannelFactory chatChannelFactory, IGuildStore guildStore,
        IVocationStore vocationStore, IMapTool mapTool,
        World world, ILogger logger) :
        base(itemFactory, creatureFactory, chatChannelFactory, guildStore, vocationStore, mapTool, world, logger)
    {
    }

    public override bool IsApplicable(PlayerEntity player)
    {
        return player?.PlayerType == 2;
    }

    public override IPlayer Load(PlayerEntity playerEntity)
    {
        if (Guard.IsNull(playerEntity)) return null;

        var town = GetTown(playerEntity);

        var newPlayer = new Tutor(
            (uint)playerEntity.PlayerId,
            playerEntity.Name,
            _vocationStore.Get(playerEntity.Vocation),
            playerEntity.Gender,
            playerEntity.Online,
            ConvertToSkills(playerEntity),
            new Outfit
            {
                Addon = (byte)playerEntity.LookAddons,
                Body = (byte)playerEntity.LookBody,
                Feet = (byte)playerEntity.LookFeet,
                Head = (byte)playerEntity.LookHead,
                Legs = (byte)playerEntity.LookLegs,
                LookType = (byte)playerEntity.LookType
            },
            playerEntity.Speed,
            new Location((ushort)playerEntity.PosX, (ushort)playerEntity.PosY, (byte)playerEntity.PosZ),
            _mapTool,
            town)
        {
            AccountId = (uint)playerEntity.AccountId,
            Guild = _guildStore.Get((ushort)(playerEntity.GuildMember?.GuildId ?? 0)),
            GuildLevel = (ushort)(playerEntity.GuildMember?.RankId ?? 0)
        };

        newPlayer.AddInventory(ConvertToInventory(newPlayer, playerEntity));
        SetCurrentTile(newPlayer);

        var tutor = _creatureFactory.CreatePlayer(newPlayer);
        return tutor;
    }
}