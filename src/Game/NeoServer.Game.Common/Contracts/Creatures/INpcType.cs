using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Creatures
{
    public interface INpcType : ICreatureType
    {
        public string Script { get; set; }
        public INpcDialog[] Dialog { get; init; }
        public IDictionary<string, dynamic> CustomAttributes { get;  }
    }

    public interface INpcDialog
    {
        public string[] OnWords { get; init; }
        public string[] Answers { get; init; }
        public string Action { get; init; }
        public bool End { get; init; }
        public INpcDialog[] Then { get; init; }
    }
}
