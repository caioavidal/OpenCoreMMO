using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.World.Algorithms;
using NeoServer.Game.World.Map;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Players;
using Serilog;

namespace NeoServer.Extensions.Players.Loaders
{
    public class TutorLoader : PlayerLoader, IPlayerLoader
    {
        private readonly ICreatureFactory _creatureFactory;
        private readonly IGuildStore _guildStore;
        private readonly IVocationStore _vocationStore;
        private readonly IMapTool _mapTool;
        private readonly IPathFinder _pathFinder;
        private readonly IWalkToMechanism _walkToMechanism;

        public TutorLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
            ChatChannelFactory chatChannelFactory,
            IChatChannelStore chatChannelStore, IGuildStore guildStore,
            IVocationStore vocationStore, IMapTool mapTool, IWalkToMechanism walkToMechanism, ILogger logger) :
            base(itemFactory, creatureFactory, chatChannelFactory, guildStore, vocationStore,mapTool,
                walkToMechanism, logger)
        {
            _creatureFactory = creatureFactory;
            _guildStore = guildStore;
            _vocationStore = vocationStore;
            _mapTool = mapTool;
            _walkToMechanism = walkToMechanism;
        }

        public override bool IsApplicable(PlayerModel player)
        {
            return player.PlayerType == 2;
        }

        public override IPlayer Load(PlayerModel playerModel)
        {
            var newPlayer = new Tutor(
                (uint)playerModel.PlayerId,
                playerModel.Name,
                _vocationStore.Get(playerModel.Vocation),
                playerModel.Gender,
                playerModel.Online,
                ConvertToSkills(playerModel),
                new Outfit
                {
                    Addon = (byte)playerModel.LookAddons, Body = (byte)playerModel.LookBody,
                    Feet = (byte)playerModel.LookFeet, Head = (byte)playerModel.LookHead,
                    Legs = (byte)playerModel.LookLegs, LookType = (byte)playerModel.LookType
                },
                playerModel.Speed,
                new Location((ushort)playerModel.PosX, (ushort)playerModel.PosY, (byte)playerModel.PosZ),
                _mapTool,
                _walkToMechanism)
            {
                AccountId = (uint)playerModel.AccountId,
                Guild = _guildStore.Get((ushort)(playerModel?.GuildMember?.GuildId ?? 0)),
                GuildLevel = (ushort)(playerModel?.GuildMember?.RankId ?? 0)
            };

            newPlayer.AddInventory(ConvertToInventory(newPlayer, playerModel));

            var tutor = _creatureFactory.CreatePlayer(newPlayer);
            return tutor;
        }
    }
}