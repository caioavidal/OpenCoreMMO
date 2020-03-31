using NeoServer.Server.Contracts.Commands;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Commands
{
    public class PlayerLogOutCommand : ICommand
    {
        public PlayerLogOutCommand(IPlayer player)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public string EventId => throw new System.NotImplementedException();

        public uint RequestorId => throw new System.NotImplementedException();

        public string ErrorMessage => throw new System.NotImplementedException();
    }
}