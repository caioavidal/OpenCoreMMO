using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.Containers.Container.Operations.Update;

internal static class ItemsLocationOperation
{
    public static void Update(Container container, byte? containerId = null)
    {
        var index = 0;
        foreach (var item in container.Items)
        {
            containerId ??= container.Id ?? 0;
            var newLocation = Location.Container(containerId.Value, (byte)index++);

            item.SetNewLocation(newLocation);
        }
    }
}