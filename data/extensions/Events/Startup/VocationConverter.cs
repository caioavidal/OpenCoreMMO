using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Item;
using NeoServer.Game.DataStore;
using NeoServer.Server.Common.Contracts;
using Serilog.Core;

namespace NeoServer.Extensions.Events.Startup
{
    /// <summary>
    /// Converts vocations array string to byte type. ie: ["paladins"] -> [2] 
    /// </summary>
    public class VocationConverter : IStartup
    {
        private readonly ItemTypeStore _itemTypeStore;
        private readonly Logger _logger;

        public VocationConverter(ItemTypeStore itemTypeStore, Logger logger)
        {
            _itemTypeStore = itemTypeStore;
            _logger = logger;
        }
        public void Run()
        {
            foreach (var itemType in _itemTypeStore.All)
            {
                var vocationsAttr = itemType.Attributes.GetAttributeArray(ItemAttribute.Vocation);
                if (vocationsAttr is not string[] vocations) continue;


                var vocationsType = new List<byte>(vocations.Length);
                foreach (var vocation in vocations)
                {
                    var vocationFound = VocationStore.Data.All.FirstOrDefault(x =>
                        vocation.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase));
                    if (vocationFound is null) continue;

                    vocationsType.Add(vocationFound.VocationType);
                }
                
                itemType.Attributes.SetAttribute(ItemAttribute.Vocation, vocationsType.ToArray());
            }
            _logger.Debug("Extensions: Converting vocations...");
        }
    }
}