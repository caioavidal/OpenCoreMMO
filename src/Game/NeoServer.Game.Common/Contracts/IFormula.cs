using System.Collections.Generic;

namespace NeoServer.Game.Common.Contracts
{
    public interface IFormula
    {
        public Dictionary<string, (double, double)> Variables { get; }
    }
}