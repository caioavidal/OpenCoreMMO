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

        private const byte PROCESS_CHUNCKS = 20;
        private const int SAVE_INTERVAL = 3000;
        private int lastIndexProcessed = 0;

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

        public async Task SavePlayers(bool full = false)
        {
            var players = gameServer.CreatureManager.GetAllLoggedPlayers().ToArray();
            if (players.Any())
            {
                try
                {
                    stopwatch.Restart();

                    var end = lastIndexProcessed + PROCESS_CHUNCKS > players.Length ? players.Length : lastIndexProcessed + PROCESS_CHUNCKS;

                    var playersChunk = full ? players : players[lastIndexProcessed..end];

                    await accountRepository.UpdatePlayers(playersChunk);
                    logger.Information("{numPlayers} players saved in {elapsed} ms", playersChunk.Length, stopwatch.ElapsedMilliseconds);

                    lastIndexProcessed = lastIndexProcessed + PROCESS_CHUNCKS > players.Length ? 0 : end;
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
