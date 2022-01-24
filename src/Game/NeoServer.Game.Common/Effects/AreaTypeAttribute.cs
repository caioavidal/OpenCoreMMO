using System;

namespace NeoServer.Game.Common.Effects;

[AttributeUsage(AttributeTargets.Field)]
public class AreaTypeAttribute : Attribute
{
    public AreaTypeAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}