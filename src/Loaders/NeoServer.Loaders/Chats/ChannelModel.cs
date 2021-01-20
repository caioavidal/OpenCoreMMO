using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Loaders.Chats
{
    public class ChannelModel
    {
        public string Name { get; set; }
        [JsonProperty("allowed-vocations")]
        public byte[] Vocations { get; set; }
        [JsonProperty("allowed-levels")]
        public LevelModel Level { get; set; }
        [JsonProperty("write-rules")]
        public WriteModel Write { get; set; }
        public ColorModel Color { get; set; }
        public class LevelModel
        {
            [JsonProperty("bigger-than")]
            public ushort BiggerThan { get; set; }
            [JsonProperty("lower-than")]
            public ushort LowerThan { get; set; }
        }
        public class WriteModel
        {
            [JsonProperty("allowed-vocations")]
            public byte[] Vocations { get; set; }
            [JsonProperty("allowed-levels")]
            public LevelModel Level { get; set; }
        }
        public class ColorModel
        {
            public int Default { get; set; }
            [JsonProperty("by-vocation")]
            public Dictionary<int, int> ByVocation { get; set; }
        }
        
        
    }
}
