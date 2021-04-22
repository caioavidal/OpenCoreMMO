using NeoServer.Data.Interfaces;
using NeoServer.Server.Contracts;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoServer.Server.Jobs.Persistance
{
    public class PlayerPersistenceJob
    {
        private readonly IGameServer gameServer;
        private readonly IAccountRepository accountRepository;
        private readonly Logger logger;
        private readonly Stopwatch stopwatch = new Stopwatch();

        private const int SAVE_INTERVAL = 30000;
        public PlayerPersistenceJob(IGameServer gameServer, IAccountRepository accountRepository, Logger logger)
        {
            this.gameServer = gameServer;
            this.accountRepository = accountRepository;
            this.logger = logger;
        }
        public void Start(CancellationToken token)
        {

            Task.Run(async () =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) return;

                    await SavePlayers();
                    await Task.Delay(SAVE_INTERVAL, token);
                }
            });
        }

        public async Task SavePlayers()
        {
            var players = gameServer.CreatureManager.GetAllLoggedPlayers().ToList();

            if (players.Any())
            {
                try
                {
                    stopwatch.Restart();

                    await accountRepository.UpdatePlayers(players);

                    logger.Information("{numPlayers} players saved in {elapsed} ms", players.Count, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    logger.Error("Could not save players");
                    logger.Debug(ex.Message);
                    logger.Debug(ex.StackTrace);

                }
            }
        }
    }
}
