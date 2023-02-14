using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NeoServer.Server.Compiler.Compilers;

namespace NeoServer.Server.Compiler;

public class Source
{
    public Source(string path, string code)
    {
        Path = path;
        Code = code;

        var buffer = GetSourceBuffer();
        SourceText = SourceText.From(buffer, buffer.Length, Encoding.UTF8, canBeEmbedded: true);
    }

    public string Code { get; }
    public string Path { get; }
    public SourceText SourceText { get; }

    public SyntaxTree GetSourceTree(CSharpParseOptions options)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText, options, Path);

        var syntaxRootNode = syntaxTree.GetRoot() as CSharpSyntaxNode;
        return CSharpSyntaxTree.Create(syntaxRootNode, null, Path, Encoding.UTF8);
    }

    private static string AddAttribute(string source, ScriptRewriter rewriter)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var newNode = rewriter.Visit(syntaxTree.GetRoot());

        return newNode.ToFullString();
    }

    private byte[] GetSourceBuffer()
    {
        var rewriter = new ScriptRewriter();
        var newSource = AddAttribute(Code, rewriter);
        var buffer = Encoding.UTF8.GetBytes(newSource);
        return buffer;
    }
}