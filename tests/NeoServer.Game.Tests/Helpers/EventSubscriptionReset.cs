using System;
using System.Reflection;

namespace NeoServer.Game.Tests.Helpers;

public class EventSubscriptionReset
{
    public static void Clear<T>(string eventName)
    {
        var definedType = typeof(T);
        var eventField = definedType.GetField(eventName, BindingFlags.Static | BindingFlags.NonPublic);
        if (eventField == null)
            throw new Exception($"Event '{eventName}' not found in the type '{definedType.FullName}'.");

        eventField.SetValue(null, null);
    }
}