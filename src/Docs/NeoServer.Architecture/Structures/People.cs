using C4Sharp.Elements;
using C4Sharp.Elements.Relationships;

namespace NeoServer.Architecture.Structures;

public static class People
{
    private static Person? _customer;

    public static Person Customer => _customer ??= new Person("customer", "Personal")
    {
        Description = "A player of the tibia, with personal tibia accounts.",
        Boundary = Boundary.External,
        Label = "Player"
    };
}