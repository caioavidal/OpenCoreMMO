using System;
using System.Collections.Generic;
using System.Text;
using NeoServer.Server.World;

namespace NeoServer.Server
{
    public class ServerLoader
    {
        private readonly Game _game;
        private readonly WorldLoader _worldLoader;
        public ServerLoader(Game game, WorldLoader worldLoader)
        {
            _game = game;
            _worldLoader = worldLoader;
        }

        private void LoadWorld()
        {
            _worldLoader.Load();
        }   

    }
}
