using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NeoServer.Server.Compiler
{
    public class ScriptRewriter : CSharpSyntaxRewriter
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

        private AttributeListSyntax CreateAttribute()
        {
            return SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(
                        SyntaxFactory.IdentifierName("Script")
                    )
                )
            );
        }
    }
}