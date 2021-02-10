using NeoServer.Game.Common;
using NeoServer.Server.Model.Players.Contracts;
using System.Linq;
using System.Text;

namespace NeoServer.Game.Contracts.Items
{
    public interface IItemRequirement : IItem
    {
        public byte[] Vocations => Metadata.Attributes.GetRequiredVocations();
        public ushort MinLevel => Metadata.Attributes.GetAttribute<ushort>(ItemAttribute.MinimumLevel);

        public bool CanBeUsed(IPlayer player)
        {
            var vocations = Vocations;
            if (vocations?.Length > 0)
            {
                if (!vocations.Contains(player.VocationType)) return false;
            }
            if (MinLevel > 0)
            {
                if (player.Level < MinLevel) return false;
            }
            return true;
        }

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
    }
}