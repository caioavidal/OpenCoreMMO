using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Common.Helpers
{
    public class ConditionEvaluation
    {
        public static bool And(params bool[] conditions)
        {
            foreach (var condition in conditions)
            {
                if (condition is false) return false;
            }
            return true;
        }
    }
}
