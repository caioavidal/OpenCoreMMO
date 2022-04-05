using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Models.Spawns;

namespace NeoServer.Loaders.Spawns;

public static class SpawnConverter
{
    public static Spawn Convert(SpawnData spawnData)
    {
        var spawn = new Spawn
        {
            Location = new Location(spawnData.CenterX, spawnData.CenterY, spawnData.CenterZ),
            Radius = spawnData.Radius
        };

        AddCreatureToSpawn(spawnData.Npcs?.ToList(), spawn);
        AddCreatureToSpawn(spawnData.Monsters?.ToList(), spawn);
        
        return spawn;
    }

    private static void AddCreatureToSpawn(IList<SpawnData.Creature> creatures, ISpawn spawn)
    {
        if (creatures is null) return;

        spawn.Npcs = new ISpawn.ICreature[creatures.Count];

        var i = 0;
        foreach (var creature in creatures)
            spawn.Npcs[i++] = new Spawn.Creature
            {
                Name = creature.Name,
                Spawn = new SpawnPoint(
                    new Location((ushort)(creature.X + spawn.Location.X),
                        (ushort)(creature.Y + spawn.Location.Y), creature.Z), creature.SpawnTime)
            };
    }
}