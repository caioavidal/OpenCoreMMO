using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server
{
    public class Game
    {
        private static Game Instance;
        public Game()
        {
            Instance = new Game();
        }
        public DateTime CombatSynchronizationTime { get; private set; }
    }
}
