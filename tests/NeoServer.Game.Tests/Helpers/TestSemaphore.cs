using System.Threading;

namespace NeoServer.Game.Tests.Helpers;

public class TestSemaphore
{
    public static SemaphoreSlim Semaphore = new SemaphoreSlim(2); // Set the initial count to 2
}