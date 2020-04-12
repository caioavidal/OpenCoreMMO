using System.Collections.Generic;

namespace NeoServer.Game.Items
{
    public class CipElement
    {
        public CipElement(int data)
        {
            Data = data;
            Attributes = new List<CipAttribute>();
        }

        public int Data { get; set; }

        public IList<CipAttribute> Attributes { get; set; }
    }

    public class CipAttribute
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
