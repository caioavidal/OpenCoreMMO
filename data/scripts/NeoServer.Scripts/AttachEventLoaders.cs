using NeoServer.Loaders.Interfaces;
using NeoServer.Loaders.Npcs;
using NeoServer.Scripts.Npcs;
using NeoServer.Server.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Scripts
{
    public class AttachEventLoaders: IRunBeforeLoaders
    {
        private readonly IEnumerable<IStartupLoader> loaders;

        public AttachEventLoaders(IEnumerable<IStartupLoader> loaders)
        {
            this.loaders = loaders;
        }

        public void Run()
        {
            if (loaders.SingleOrDefault(x => x is NpcLoader) is NpcLoader npcLoader)
            {
                npcLoader.OnLoad += ShopModule.LoadShopData;
            }
        }
    }
}
