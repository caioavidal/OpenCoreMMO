using NeoServer.Data.Model;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creatures.Model;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Players;
using Serilog;

namespace NeoServer.Extensions.Players.Loaders
{
    public class GodLoader : PlayerLoader, IPlayerLoader
    {
        private readonly ICreatureFactory _creatureFactory;
        private readonly IGuildStore _guildStore;
        private readonly IVocationStore _vocationStore;
        private readonly IPathFinder _pathFinder;
        private readonly IWalkToMechanism _walkToMechanism;

        public GodLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory,
            ChatChannelFactory chatChannelFactory,
            IChatChannelStore chatChannelStore, IGuildStore guildStore,
            IVocationStore vocationStore, IPathFinder pathFinder, IWalkToMechanism walkToMechanism, ILogger logger) :
            base(itemFactory, creatureFactory, chatChannelFactory, guildStore,
                vocationStore, pathFinder, walkToMechanism,logger)
        {
            _creatureFactory = creatureFactory;
            _guildStore = guildStore;
            _vocationStore = vocationStore;
            _pathFinder = pathFinder;
            _walkToMechanism = walkToMechanism;
        }

        public override bool IsApplicable(PlayerModel player)
        {
            return player.PlayerType == 3;
        }

        public override IPlayer Load(PlayerModel playerModel)
        {
            var newPlayer = new God(
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
                _pathFinder,
                _walkToMechanism
            )
            {
                AccountId = (uint)playerModel.AccountId,
                Guild = _guildStore.Get((ushort)(playerModel?.GuildMember?.GuildId ?? 0)),
                GuildLevel = (ushort)(playerModel?.GuildMember?.RankId ?? 0)
            };

            var god = _creatureFactory.CreatePlayer(newPlayer);
            god.AddInventory(ConvertToInventory(god, playerModel));
            return god;
        }
    }
}