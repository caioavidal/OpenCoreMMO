using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Contracts
{
    public interface IStartup
    {
        void Run();
    }
    public interface IRunBeforeLoaders
    {
        void Run();
    }
}
