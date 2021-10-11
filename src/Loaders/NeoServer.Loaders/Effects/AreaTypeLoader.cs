using System;
using System.Linq;
using System.Reflection;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Effects;
using NeoServer.Loaders.Interfaces;

namespace NeoServer.Loaders.Effects
{
    public class AreaTypeLoader : IStartupLoader
    {
        private readonly IAreaTypeStore _areaTypeStore;

        public AreaTypeLoader(IAreaTypeStore areaTypeStore)
        {
            _areaTypeStore = areaTypeStore;
        }
        public void Load()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            var fields = types.SelectMany(x => x.GetTypes()).SelectMany(x => x.GetFields())
                .Where(prop => prop.IsDefined(typeof(AreaTypeAttribute), false));

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<AreaTypeAttribute>();
                if (attr is not null) _areaTypeStore.Add(attr.Name, field);
            }
        }
    }
}