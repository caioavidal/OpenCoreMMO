using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeoServer.Data.Model;

namespace NeoServer.Data.Seeds
{
    internal sealed class PlayerItemSeed
    {
        public static void Seed(EntityTypeBuilder<PlayerItemModel> builder)
        {
            builder.HasData(
                new PlayerItemModel
                {
                    Id = -1,
                    PlayerId = 1,
                    ParentId = 0,
                    ServerId = 1988,
                    Amount = 1,
                },
                new PlayerItemModel
                {
                    Id = -2,
                    PlayerId = 1,
                    ParentId = 0,
                    ServerId = 2666,
                    Amount = 10,
                },
                new PlayerItemModel
                {
                    Id = -3,
                    PlayerId = 1,
                    ParentId = 0,
                    ServerId = 7618,
                    Amount = 10,
                },
                new PlayerItemModel
                {
                    Id = -4,
                    PlayerId = 1,
                    ParentId = 0,
                    ServerId = 2311,
                    Amount = 10,
                },
                  new PlayerItemModel
                  {
                      Id = -5,
                      PlayerId = 1,
                      ParentId = -1,
                      ServerId = 2304,
                      Amount = 10,
                  }
            );
        }

    }
}
