using System.Collections.Generic;
using NeoServer.Loaders.Interfaces;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Extensions
{
    public class AttachEventLoaders : IRunBeforeLoaders
    {
        private readonly IEnumerable<IStartupLoader> loaders;

        public AttachEventLoaders(IEnumerable<IStartupLoader> loaders)
        {
            this.loaders = loaders;
        }

        public void Run()
        {
            //if (loaders.SingleOrDefault(x => x is NpcLoader) is NpcLoader npcLoader)
            //{
            //}
        }
    }
}