using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoServer.Data.Interfaces;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Configurations;
using Serilog;

namespace NeoServer.Server.Jobs.Persistence;

public class PlayerPersistenceJob
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IGameServer _gameServer;
    private readonly ILogger _logger;
    private readonly ServerConfiguration _serverConfiguration;
    private readonly Stopwatch _stopwatch = new();

    private int _saveInterval;

    public PlayerPersistenceJob(IGameServer gameServer, IPlayerRepository playerRepository, ILogger logger,
        ServerConfiguration serverConfiguration)
    {
        _gameServer = gameServer;
        _playerRepository = playerRepository;
        _logger = logger;
        _serverConfiguration = serverConfiguration;
    }

    public void Start(CancellationToken token)
    {
        _saveInterval = (int)(_serverConfiguration?.Save?.Players ?? (uint)_saveInterval);
        _saveInterval = (_saveInterval == 0 ? 3600 : _saveInterval) * 1000;
        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await SavePlayers();
                }
                catch (Exception ex)
                {
                    _logger.Error("Could not save players");
                    _logger.Debug(ex.Message);
                    _logger.Debug(ex.StackTrace);
                }

                await Task.Delay(_saveInterval, token);
            }
        }, token);
    }

    private async Task SavePlayers()
    {
        var players = _gameServer.CreatureManager.GetAllLoggedPlayers().ToList();

        if (players.Any())
        {
            _stopwatch.Restart();

            await _playerRepository.UpdatePlayers(players);

            _logger.Information("{NumPlayers} players saved in {Elapsed} ms", players.Count,
                _stopwatch.ElapsedMilliseconds);
        }
    }
}