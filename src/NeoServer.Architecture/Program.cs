// See https://aka.ms/new-console-template for more information
using C4Sharp.Models.Plantuml.IO;
using NeoServer.Architecture.Diagrams;

var diagrams = new[]
{
    new ContainerDiagram().Build()
};

var path = "../../../c4";

new PlantumlContext()
    .UseDiagramImageBuilder()
    .UseDiagramSvgImageBuilder()
    .UseStandardLibraryBaseUrl()
    .UseHtmlPageBuilder()
    .Export(path, diagrams);