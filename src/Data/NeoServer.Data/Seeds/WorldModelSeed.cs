using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Seeds;

public class WorldModelSeed
{
    public static void Seed(EntityTypeBuilder<WorldModel> builder)
    {
        builder.HasData
        (
            new WorldModel
            {
                Id = 1,
                Ip = "127.0.0.1",
                Name = "OpenCore"
            }
        );
    }
}