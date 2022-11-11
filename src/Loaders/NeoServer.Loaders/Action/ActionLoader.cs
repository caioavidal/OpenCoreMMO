using System.Collections.Generic;
using System.IO;
using System.Linq;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Item;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace NeoServer.Loaders.Action;

public class ActionLoader
{
    private readonly IActionStore _actionStore;
    private readonly ILogger _logger;
    private readonly ServerConfiguration _serverConfiguration;

    public ActionLoader(ILogger logger,
        ServerConfiguration serverConfiguration, IActionStore actionStore)
    {
        _logger = logger;
        _serverConfiguration = serverConfiguration;
        _actionStore = actionStore;
    }

    public void Load()
    {
        _logger.Step("Loading actions...", "{n} actions loaded", () =>
        {
            var actions = GetActions();
            actions.AsParallel().ForAll(x => _actionStore.Add(x.FromId, x));

            return new object[] { actions.Count };
        });
    }

    private List<ItemAction> GetActions()
    {
        var basePath = $"{_serverConfiguration.Data}";
        var jsonString = File.ReadAllText(Path.Combine(basePath, "actions.json"));
        var actions = JsonConvert.DeserializeObject<List<ItemAction>>(jsonString);

        return actions;
    }
}