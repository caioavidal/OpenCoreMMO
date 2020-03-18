using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLua;

namespace neoserver
{
    class Program
    {
        static void Main(string[] args)
        {
            //   Lua state = new Lua();
            //  var res = state.DoString("return 10 + 3*(5 + 2)")[0];



            MessageQueue.Start();

            IoC.Load();
            RSA.LoadPem();

            //Database.Connect();

            // new AccountRepository().Create(new Account
            // {
            //     AccountName = "1",
            //     Password = "1",
            //     Players = new List<Player>(){
            //         new Player(){
            //              Name = "Caio 1"
            //         }, new Player(){
            //             Name = "Caio 2"
            //         }
            //      }
            // });

            //VocationConfig.Load();

            //OtbFile.LoadItems().Wait();

            // ItemConfig.Load();

            

            Task.Run(()=>LoginListener.StartListening());
            Task.Run(()=>GameListener.StartListening());


            Console.Read();
        }
    }
}
