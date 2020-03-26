using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts;
using NeoServer.Networking;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Messages;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Server.Handlers.Authentication
{
    public class PlayerLogInEventHandler : PacketHandler
    {
        private readonly IAccountRepository _repository;
        private readonly ServerState _serverState;

        private readonly Game _game;
        private readonly IMap _map;

        public PlayerLogInEventHandler(IAccountRepository repository, ServerState serverState,
         Game game, IMap map)
        {
            _repository = repository;
            _serverState = serverState;
            _game = game;
            _map = map;
        }

        public async override void HandlerMessage(IReadOnlyNetworkMessage message, Connection connection)
        {
            var packet = new PlayerLogInPacket(message);

            connection.SetXtea(packet.Xtea);

            //todo: add more validations
            if (_serverState.Status == ServerStatus.Closed)
            {
                connection.Send(new GameServerDisconnectPacket("Server is currently closed.\nPlease try again later."));
                return;
            }

            var accountRecord = await _repository.FirstOrDefaultAsync(a => a.AccountName == packet.Account && a.Password == packet.Password);
            var playerRecord = accountRecord.Players.FirstOrDefault(p => p.CharacterName == packet.CharacterName);

            var player = _game.LogInPlayer(playerRecord, connection);

            connection.PlayerId = player.CreatureId;
            connection.IsAuthenticated = true;

            var outgoingPackets = new Queue<OutgoingPacket>();

            outgoingPackets.Enqueue(new SelfAppearPacket(player));
            outgoingPackets.Enqueue(new MapDescriptionPacket(player, _map));
            outgoingPackets.Enqueue(new MagicEffectPacket(player.Location, EffectT.BubbleBlue));
            outgoingPackets.Enqueue(new PlayerInventoryPacket(player.Inventory));
            outgoingPackets.Enqueue(new PlayerStatusPacket(player));
            outgoingPackets.Enqueue(new PlayerSkillsPacket(player));

            outgoingPackets.Enqueue(new WorldLightPacket(_game.LightLevel, _game.LightColor));

            outgoingPackets.Enqueue(new CreatureLightPacket(player));

            outgoingPackets.Enqueue(new PlayerConditionsPacket());

            connection.Send(outgoingPackets);
        }
    }
}
