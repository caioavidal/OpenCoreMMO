using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Commands
{
    public class PlayerLogInCommand : ICommand
    {
        public PlayerLogInCommand(PlayerModel player, IConnection connection)
        {
            PlayerRecord = player;
            Connection = connection;
        }

        public PlayerModel PlayerRecord { get; }

        public IConnection Connection { get; }

        public string EventId => throw new System.NotImplementedException();

        public uint RequestorId => throw new System.NotImplementedException();

        public string ErrorMessage => throw new System.NotImplementedException();
    }
}