using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace NeoServer.Game.Common.Helpers;

[ExcludeFromCodeCoverage]
public static class StringBuilderExtension
{
    public static StringBuilder AppendNewLine(this StringBuilder stringBuilder) => stringBuilder.Append('\n');
    public static StringBuilder AppendNewLine(this StringBuilder stringBuilder, string value) => stringBuilder.Append($"{value}\n");
}