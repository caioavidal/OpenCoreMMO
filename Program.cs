using System;
using NLua;

namespace neoserver
{
    class Program
    {
        static void Main(string[] args)
        {
            Lua state = new Lua();
            var res = state.DoString("return 10 + 3*(5 + 2)")[0];



            MessageQueue.Start();

            IoC.Load();
            RSA.LoadPem();

            Database.Connect();

            VocationConfig.Load();

            var loadItemsTask = OtbFile.LoadItems();

            ItemConfig.Load();

            LoginListener.StartListening();


            Console.Read();
        }
    }
}
