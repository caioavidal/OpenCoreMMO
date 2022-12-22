using System.Text;

namespace NeoServer.Game.Common.Helpers;

public static class StringBuilderExtension
{
    public static StringBuilder AppendNewLine(this StringBuilder stringBuilder) => stringBuilder.Append('\n');
    public static StringBuilder AppendNewLine(this StringBuilder stringBuilder, string value) => stringBuilder.Append($"{value}\n");
}