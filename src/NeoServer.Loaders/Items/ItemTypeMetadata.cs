using System.Collections.Generic;

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

        public struct Attribute
        {
            public string Key { get; set; }
            public dynamic Value { get; set; }
            public IEnumerable<Attribute> Attributes { get; set; }

        }
        public struct Requirement
        {
            public string Vocation { get; set; }
            public ushort MinLevel { get; set; }
        }
    }
}