using NeoServer.Game.Chats;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Creature.Model;
using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Players;
using NeoServer.Server.Model.Players;

namespace NeoServer.Scripts.Players.Loaders
{
    public class GodLoader : PlayerLoader, IPlayerLoader
    {
        private readonly ICreatureFactory creatureFactory;
        public GodLoader(IItemFactory itemFactory, ICreatureFactory creatureFactory, ChatChannelFactory chatChannelFactory) : base(itemFactory, creatureFactory, chatChannelFactory)
        {
            this.creatureFactory = creatureFactory;
        }
        public override bool IsApplicable(PlayerModel player) => player.PlayerType == 3;
        public override IPlayer Load(PlayerModel playerModel)
        {
            var newPlayer = new God(
                (uint)playerModel.PlayerId,
                playerModel.Name,
                playerModel.Vocation,
                playerModel.Gender,
                playerModel.Online,
                ConvertToSkills(playerModel),
                new Outfit() { Addon = (byte)playerModel.LookAddons, Body = (byte)playerModel.LookBody, Feet = (byte)playerModel.LookFeet, Head = (byte)playerModel.LookHead, Legs = (byte)playerModel.LookLegs, LookType = (byte)playerModel.LookType },
                ConvertToInventory(playerModel),
                playerModel.Speed,
                new Location((ushort)playerModel.PosX, (ushort)playerModel.PosY, (byte)playerModel.PosZ)
                )
            {
                AccountId = (uint)playerModel.AccountId,
                GuildId = (ushort)(playerModel?.GuildMember?.GuildId ?? 0),
                GuildLevel = (ushort)(playerModel?.GuildMember?.RankId ?? 0)
            };

            var tutor = creatureFactory.CreatePlayer(newPlayer);

            return tutor;
        }
    }
}
