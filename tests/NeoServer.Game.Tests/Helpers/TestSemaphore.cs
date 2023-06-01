using System.Threading;

namespace NeoServer.Game.Tests.Helpers;

public static class TestSemaphore
{
    public static readonly SemaphoreSlim Semaphore = new(2); // Set the initial count to 2
}