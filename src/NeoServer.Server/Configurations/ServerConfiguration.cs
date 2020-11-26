using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Standalone
{
    public record ServerConfiguration (int Version, string OTBM, string OTB, string Data);
}
