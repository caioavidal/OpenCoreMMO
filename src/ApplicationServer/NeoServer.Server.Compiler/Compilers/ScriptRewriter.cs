using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NeoServer.Server.Attributes;

namespace NeoServer.Server.Compiler.Compilers;

internal class ScriptRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var attributes = AddAttribute(node);
        return node.WithAttributeLists(attributes);
    }

    private SyntaxList<AttributeListSyntax> AddAttribute(MemberDeclarationSyntax node)
    {
        var attributes = node.AttributeLists;

        attributes = attributes.Add(CreateAttribute()
            .WithLeadingTrivia(node.GetLeadingTrivia())
            .WithTrailingTrivia(node.GetTrailingTrivia()));
        return attributes;
    }

    private static AttributeListSyntax CreateAttribute()
    {
        var attributeType = typeof(ExtensionAttribute);
        return SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(
                        SyntaxFactory.QualifiedName(
                            SyntaxFactory.IdentifierName(attributeType.Namespace ?? string.Empty),
                            SyntaxFactory.IdentifierName(attributeType.Name?.Replace("Attribute", string.Empty))))))
            .NormalizeWhitespace();
    }
}