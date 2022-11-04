using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Server.Configurations;
using NeoServer.Server.Helpers.Extensions;
using Newtonsoft.Json;

namespace NeoServer.Loaders.Action;

public class ActionLoader
{
    private readonly ILogger _logger;
    private readonly ServerConfiguration _serverConfiguration;
    private readonly IActionStore _actionStore;

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
            actions.AsParallel().ForAll(x=> _actionStore.Add(x.FromId, x));

            return new object[] { actions.Count };
        });
    }
    private List<Game.Common.Item.ItemAction> GetActions()
    {
        var basePath = $"{_serverConfiguration.Data}";
        var jsonString = File.ReadAllText(Path.Combine(basePath, "actions.json"));
        var actions = JsonConvert.DeserializeObject<List<Game.Common.Item.ItemAction>>(jsonString);
        
        return actions;
    }
}