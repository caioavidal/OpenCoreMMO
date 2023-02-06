using C4Sharp.Diagrams;
using C4Sharp.Elements;
using C4Sharp.Elements.Boundaries;
using C4Sharp.Elements.Relationships;
using NeoServer.Architecture.Structures;

namespace NeoServer.Architecture.Diagrams;

public class ContainerDiagram : DiagramBuildRunner
{
    protected override string Title => "Container diagram for OpenCoreMMO";
    protected override DiagramType DiagramType => DiagramType.Container;
    protected override DiagramLayout FlowVisualization => DiagramLayout.LeftRight;
    protected override bool ShowLegend => true;
    protected override bool LayoutAsSketch => false;

    protected override IEnumerable<Structure> Structures =>
        new Structure[]
        {
            People.Customer,
            new SoftwareSystemBoundary("c1", "OpenCoreMMO.Networking", Containers.NetworkingServer),
            new SoftwareSystemBoundary("c2", "OpenCoreMMO.Game",
                Containers.GameServer,
                Containers.JobsServer,
                Containers.LoadServer),
            new SoftwareSystemBoundary("c3", "OpenCoreMMO.Data",
                Containers.SqlDatabase,
                Containers.MemoryDatabase),
            new SoftwareSystemBoundary("c4", "OpenCoreMMO.SystemFile",
                Containers.FileSystemToLoader)
        };


    protected override IEnumerable<Relationship> Relationships =>
        new[]
        {
            (People.Customer > Containers.NetworkingServer)["Uses", "TCP"][Position.Down],

            (Containers.NetworkingServer > Containers.GameServer)["Uses", "Events"][Position.Up],
            (Containers.GameServer > Containers.SqlDatabase)["Uses", "Events"][Position.Neighbor],
            (Containers.GameServer > Containers.MemoryDatabase)["Uses", "Events"][Position.Neighbor],
            (Containers.JobsServer > Containers.SqlDatabase)["Uses", "TCP"][Position.Neighbor],
            (Containers.JobsServer > Containers.GameServer)["Uses", "Events"],
            (Containers.LoadServer > Containers.FileSystemToLoader)["Uses", "I/O"][Position.Up],
            (Containers.LoadServer > Containers.MemoryDatabase)["Uses"][Position.Up],
            (Containers.LoadServer > Containers.SqlDatabase)["Uses", "TCP"][Position.Up]
        };
}

