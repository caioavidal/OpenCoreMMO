using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Items.Items;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Loaders.Items
{
    public struct ItemTypeMetadata
    {
        public ushort? Id { get; set; }
        public string Name { get; set; }
        public ushort? Fromid { get; set; }
        public ushort? Toid { get; set; }
        public IEnumerable<Attribute> Attributes { get; set; }
        public IEnumerable<Attribute> OnUse { get; set; }

        public string Article { get; set; }
        public string Plural { get; set; }
        public string Editorsuffix { get; set; }
        public Requirement[] Requirements { get; set; }
        public string[] Flags { get; set; }

        public IItemRequirement[] ItemRequirements
        {
            get
            {
                if (Requirements is null) return null;

                return Requirements.Select(x => new ItemRequirement { Vocation = VocationTypeParser.Parse(x.Vocation), MinLevel = x.MinLevel } as IItemRequirement).ToArray();
            }
        }

        public struct Attribute
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public IEnumerable<Attribute> Attributes { get; set; }

        }
        public struct Requirement
        {
            public string Vocation { get; set; }
            public ushort MinLevel { get; set; }
        }
    }
}