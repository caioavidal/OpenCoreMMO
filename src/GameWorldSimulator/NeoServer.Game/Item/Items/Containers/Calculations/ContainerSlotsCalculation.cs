﻿using NeoServer.Game.Common.Contracts.Items.Types.Containers;

namespace NeoServer.Game.Item.Items.Containers.Calculations;

internal static class ContainerSlotsCalculation
{
    public static uint CalculateFreeSlots(Container container)
    {
        uint total = 0;
        foreach (var item in container.Items)
            if (item is IContainer innerContainer)
                total += innerContainer.TotalOfFreeSlots;

        total += container.FreeSlotsCount;
        return total;
    }
}