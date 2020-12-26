using NeoServer.Game.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Contracts
{
    public interface IFormula
    {
        public string Formula { get; }
        public Dictionary<string, (double, double)> Variables { get; }
    }
}
