using System.Linq;
using System.Text;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemRequirement : IItem
    {
        public byte[] Vocations => Metadata.Attributes.GetRequiredVocations();
        public ushort MinLevel => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);

        public string ValidationError
        {
            get
            {
                var text = new StringBuilder();
                text.Append("Only ");

                //for (int i = 0; i < Vocations.Length; i++)
                //{
                //    text.Append($"{VocationTypeParser.Parse(Vocations[i]).ToLower()}s");
                //    if (i + 1 < Vocations.Length)
                //    {
                //        text.Append(", ");
                //    }
                //}
                text.Append($" of level {MinLevel} or above may use or consume this item");
                return text.ToString();
            }
        }

        public bool CanBeUsed(IPlayer player)
        {
            var vocations = Vocations;
            if (vocations?.Length > 0)
                if (!vocations.Contains(player.VocationType))
                    return false;
            if (MinLevel > 0)
                if (player.Level < MinLevel)
                    return false;
            return true;
        }
    }
}