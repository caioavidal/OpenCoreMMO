using NeoServer.Game.DataStore;
using NeoServer.Game.Effects;
using NeoServer.Loaders.Interfaces;
using System;
using System.Linq;
using System.Reflection;

namespace NeoServer.Loaders.Effects
{
    public class AreaTypeLoader: ICustomLoader
    {
        public void Load()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies();
            var fields = types.SelectMany(x => x.GetTypes()).SelectMany(x => x.GetFields()).Where(prop => prop.IsDefined(typeof(AreaTypeAttribute), false));

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<AreaTypeAttribute>();
                AreaTypeStore.Add(attr.Name, field);
            }
        }
    }
}
