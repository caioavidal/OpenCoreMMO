using System;
using System.IO;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.World.Map;
using NeoServer.Server.Configurations;

namespace NeoServer.Scripts.Lua
{
    public class ItemEventSubscriber : IItemEventSubscriber, IGameEventSubscriber
    {
        private readonly NLua.Lua lua;
        private readonly IMap map;
        private readonly ServerConfiguration serverConfiguration;

        public ItemEventSubscriber(ServerConfiguration serverConfiguration, NLua.Lua lua, IMap map)
        {
            this.serverConfiguration = serverConfiguration;
            this.lua = lua;
            this.map = map;
        }

        public void Subscribe(IItem item)
        {
            if (!item.Metadata.Attributes.HasAttribute(ItemAttribute.Script)) return;

            var script = item.Metadata.Attributes.GetAttribute(ItemAttribute.Script);

            var isLuaScript = script?.Trim()?.EndsWith(".lua", StringComparison.InvariantCultureIgnoreCase) ?? false;

            if (!isLuaScript) return;

            (item as IGround).OnCreatureWalkedThrough += (_, _) => { };
            var scriptPath = Path.Combine(serverConfiguration.Data, script);

            lua.DoFile(scriptPath);
            lua.GetFunction("init").Call(item);
        }

        public void Unsubscribe(IItem item)
        {
            throw new System.NotImplementedException();
        }
    }
}