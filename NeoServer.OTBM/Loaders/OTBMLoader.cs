using System;
using System.Linq;
using NeoServer.OTBM.Structure;

namespace NeoServer.OTBM
{
    public class OTBMLoader
    {
        public OTBMLoader()
        {
            
        }
     
        public Structure.OTBM Load()
        {
            var otbmStream = OTBMConvert.Serialize("");
            var otbmNode = OTBMConvert.Deserialize(otbmStream);
            var otbm = new OTBMNodeParser().Parse(otbmNode);


            return otbm;
        }
    }
}