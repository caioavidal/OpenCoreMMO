using System.Collections.Generic;

namespace NeoServer.Game.Common.Location.Structs.Helpers
{
    public sealed class DuplicateComparer : IComparer<int>
    {
        /// <summary>
        /// Compares an int to another.
        /// </summary>
        /// <param name="x">The first integer to compare.</param>
        /// <param name="y">The second integer to compare.c</param>
        /// <returns>-1 if first is less than or equal to second, 1 otherwise.</returns>
        public int Compare(int x, int y)
        {
            return (x <= y) ? -1 : 1;
        }
    }
}
