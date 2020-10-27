using Microsoft.VisualBasic.CompilerServices;
using NeoServer.Game.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Server.Helpers
{
    public class ProbabilityRandom
    {
        static Random rand = new Random();

        public static IProbability Next(IProbability[] items)
        {
            double totalChance = 0;
            foreach (var item in items)
            {
                totalChance += item.Chance;
            }

            // pick a random number between 0 and u
            double r = rand.NextDouble() * 100;

            double sum = 0;
            foreach (var item in items)
            {
                // loop until the random number is less than our cumulative probability
                if (r <= (sum = sum + item.Chance))
                {
                    return item;
                }
            }
            // should never get here
            return default;
        }
    }
}
