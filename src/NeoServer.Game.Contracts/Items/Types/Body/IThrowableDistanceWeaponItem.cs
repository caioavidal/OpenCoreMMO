using System.Text;

namespace NeoServer.Game.Contracts.Items.Types.Body
{
    public interface IThrowableDistanceWeaponItem : ICumulative, IWeapon, IBodyEquipmentItem
    {
        byte Attack { get; }
        byte Range { get; }

        private string AttributesText
        {
            get
            {
                var range = Range > 0 ? $"Range:{Range}" : string.Empty;
                var atk = Attack > 0 ? $"Atk:{Attack}" : string.Empty;

                if (range == "" && atk == "") return string.Empty;

                var stringBuilder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(range)) stringBuilder.Append($"{range}, ");
                if (!string.IsNullOrWhiteSpace(atk)) stringBuilder.Append($"{atk}, ");

                stringBuilder?.Remove(stringBuilder.Length - 2, 2);

                return $"({stringBuilder})";
            }
        }

        string IThing.InspectionText => $"{LookText} {AttributesText}";

    }

}