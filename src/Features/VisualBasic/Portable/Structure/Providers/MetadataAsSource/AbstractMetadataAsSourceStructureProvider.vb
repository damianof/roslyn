' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Threading
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.Structure
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Structure.MetadataAsSource

    Friend MustInherit Class AbstractMetadataAsSourceStructureProvider(Of TSyntaxNode As SyntaxNode)
        Inherits AbstractSyntaxNodeStructureProvider(Of TSyntaxNode)

        Protected Overrides Sub CollectBlockSpans(node As TSyntaxNode,
                                                      spans As ImmutableArray(Of BlockSpan).Builder,
                                                      cancellationToken As CancellationToken)
            Dim startToken = node.GetFirstToken()
            Dim endToken = GetEndToken(node)

            Dim firstComment = startToken.LeadingTrivia.FirstOrNullable(Function(t) t.Kind = SyntaxKind.CommentTrivia)

            Dim startPosition = If(firstComment.HasValue,
                                   firstComment.Value.SpanStart,
                                   startToken.SpanStart)

            Dim endPosition = endToken.SpanStart

            ' TODO (tomescht): Mark the regions to be collapsed by default.
            If startPosition <> endPosition Then
                Dim hintTextEndToken = GetHintTextEndToken(node)

                spans.Add(New BlockSpan(
                    isCollapsible:=True,
                    textSpan:=TextSpan.FromBounds(startPosition, endPosition),
                    hintSpan:=TextSpan.FromBounds(startPosition, hintTextEndToken.Span.End),
                    bannerText:=Ellipsis,
                    autoCollapse:=True))
            End If
        End Sub

        Protected Overrides Function SupportedInWorkspaceKind(kind As String) As Boolean
            Return kind = WorkspaceKind.MetadataAsSource
        End Function

        Protected Overridable Function GetHintTextEndToken(node As TSyntaxNode) As SyntaxToken
            Return node.GetLastToken()
        End Function

        Protected MustOverride Function GetEndToken(node As TSyntaxNode) As SyntaxToken
    End Class
End Namespace