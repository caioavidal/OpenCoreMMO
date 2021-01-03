using System.Collections.Generic;

namespace NeoServer.Game.Contracts
{
    public interface IFormula
    {
        public Dictionary<string, (double, double)> Variables { get; }
    }
}
