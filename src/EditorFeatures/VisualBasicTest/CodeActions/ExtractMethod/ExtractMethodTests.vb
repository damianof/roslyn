' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.CodeAnalysis.CodeRefactorings
Imports Microsoft.CodeAnalysis.CodeRefactorings.ExtractMethod

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.UnitTests.CodeRefactorings.ExtractMethod
    Public Class ExtractMethodTests
        Inherits AbstractVisualBasicCodeActionTest

        Protected Overrides Function CreateCodeRefactoringProvider(workspace As Workspace) As CodeRefactoringProvider
            Return New ExtractMethodCodeRefactoringProvider()
        End Function

        <WorkItem(540686, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540686")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestExtractReturnExpression() As Task
            Await TestAsync(
NewLines("Class Module1 \n Private Delegate Function Func(i As Integer) \n Shared Sub Main(args As String()) \n Dim temp As Integer = 2 \n Dim fnc As Func = Function(arg As Integer) \n temp = arg \n Return [|arg|] \n End Function \n End Sub \n End Class"),
NewLines("Class Module1 \n Private Delegate Function Func(i As Integer) \n Shared Sub Main(args As String()) \n Dim temp As Integer = 2 \n Dim fnc As Func = Function(arg As Integer) \n temp = arg \n Return {|Rename:GetArg|}(arg) \n End Function \n End Sub \n Private Shared Function GetArg(arg As Integer) As Integer \n Return arg \n End Function \n End Class"),
index:=0)
        End Function

        <WorkItem(540755, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/540755")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestExtractMultilineLambda() As Task
            Await TestAsync(
NewLines("Imports System \n Imports System.Collections.Generic \n Imports System.Linq \n Module Program \n Sub Main(args As String()) \n If True Then Dim q As Action = [|Sub() \n End Sub|] \n End Sub \n End Module"),
NewLines("Imports System \n Imports System.Collections.Generic \n Imports System.Linq \n Module Program \n Sub Main(args As String()) \n If True Then Dim q As Action = {|Rename:GetQ|}() \n End Sub \n Private Function GetQ() As Action \n Return Sub() \n End Sub \n End Function \n End Module"),
index:=0)
        End Function

        <WorkItem(541515, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/541515")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestCollectionInitializerInObjectCollectionInitializer() As Task
            Await TestAsync(
NewLines("Class Program \n Sub Main() \n [|Dim x As New List(Of Program) From {New Program}|] \n End Sub \n Public Property Name As String \n End Class"),
NewLines("Class Program \n Sub Main() \n {|Rename:NewMethod|}() \n End Sub \n Private Shared Sub NewMethod() \n Dim x As New List(Of Program) From {New Program} \n End Sub \n Public Property Name As String \n End Class"),
index:=0)
        End Function

        <WorkItem(542251, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542251")>
        <WorkItem(543030, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/543030")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestLambdaSelection() As Task
            Await TestAsync(
NewLines("Imports System \n Imports System.Collections.Generic \n Imports System.Linq \n Module Program \n Sub Main(args As String()) \n Dim q As Object \n If True Then q = [|Sub() \n End Sub|] \n End Sub \n End Module"),
NewLines("Imports System \n Imports System.Collections.Generic \n Imports System.Linq \n Module Program \n Sub Main(args As String()) \n Dim q As Object \n If True Then q = {|Rename:NewMethod|}() \n End Sub \n Private Function NewMethod() As Object \n Return Sub() \n End Sub \n End Function \n End Module"))
        End Function

        <WorkItem(542904, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/542904")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestFormatBeforeAttribute() As Task
            Await TestAsync(
<Text>Module Program
    Sub Main(args As String())
        Dim x = [|1 + 1|]
    End Sub

    &lt;Obsolete&gt;
    Sub Foo
    End Sub
End Module
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Module Program
    Sub Main(args As String())
        Dim x = {|Rename:GetX|}()
    End Sub

    Private Function GetX() As Integer
        Return 1 + 1
    End Function

    &lt;Obsolete&gt;
    Sub Foo
    End Sub
End Module
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(545262, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545262")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestInTernaryConditional() As Task
            Await TestAsync(
NewLines("Module Program \n Sub Main(args As String()) \n Dim p As Object = Nothing \n Dim Obj1 = If(New With {.a = True}.a, p, [|Nothing|]) \n End Sub \n End Module"),
NewLines("Module Program \n Sub Main(args As String()) \n Dim p As Object = Nothing \n Dim Obj1 = If(New With {.a = True}.a, p, {|Rename:NewMethod|}()) \n End Sub \n Private Function NewMethod() As Object \n Return Nothing \n End Function \n End Module"))
        End Function

        <WorkItem(545547, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545547")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestInRangeArgumentUpperBound() As Task
            Await TestAsync(
NewLines("Module Program \n Sub Main() \n Dim x(0 To [|1 + 2|]) ' Extract method \n End Sub \n End Module"),
NewLines("Module Program \n Sub Main() \n Dim x(0 To {|Rename:NewMethod|}()) ' Extract method \n End Sub \n Private Function NewMethod() As Integer \n Return 1 + 2 \n End Function \n End Module"))
        End Function

        <WorkItem(545655, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545655")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestInWhileUntilCondition() As Task
            Await TestAsync(
NewLines("Module M \n Sub Main() \n Dim x = 0 \n Do While [|x * x < 100|] \n x += 1 \n Loop \n End Sub \n End Module"),
NewLines("Module M \n Sub Main() \n Dim x = 0 \n Do While {|Rename:NewMethod|}(x) \n x += 1 \n Loop \n End Sub \n Private Function NewMethod(x As Integer) As Boolean \n Return x * x < 100 \n End Function \n End Module"))
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestInInterpolation1() As Task
            Await TestAsync(
NewLines("Module M \n Sub Main() \n Dim v As New Object \n [|System.Console.WriteLine($""{v}"")|] \n System.Console.WriteLine(v) \n End Sub \n End Module"),
NewLines("Module M
    Sub Main()
        Dim v As New Object
        {|Rename:NewMethod|}(v)
        System.Console.WriteLine(v)
    End Sub

    Private Sub NewMethod(v As Object)
        System.Console.WriteLine($""{v}"")
    End Sub
End Module"),
compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestInInterpolation2() As Task
            Await TestAsync(
NewLines("Module M \n Sub Main() \n Dim v As New Object \n System.Console.WriteLine([|$""{v}""|]) \n System.Console.WriteLine(v) \n End Sub \n End Module"),
NewLines("Module M
    Sub Main()
        Dim v As New Object
        System.Console.WriteLine({|Rename:NewMethod|}(v))
        System.Console.WriteLine(v)
    End Sub

    Private Function NewMethod(v As Object) As String
        Return $""{v}""
    End Function
End Module"),
compareTokens:=False)
        End Function

        <WorkItem(545829, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/545829")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestMissingOnImplicitMemberAccess() As Task
            Await TestAsync(
NewLines("Module Program \n Sub Main() \n With """""""" \n Dim x = [|.GetHashCode|] Xor &H7F3E ' Introduce Local \n End With \n End Sub \n End Module"),
NewLines("Module Program \n Sub Main() \n {|Rename:NewMethod|}() \n End Sub \n Private Sub NewMethod() \n With """""""" \n Dim x = .GetHashCode Xor &H7F3E ' Introduce Local \n End With \n End Sub \n End Module"))
        End Function

        <WorkItem(984831, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/984831")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestPreserveCommentsBeforeDeclaration_1() As Task
            Await TestAsync(
<Text>Class Program
    Sub Main(args As String())
        [|Dim one As Program = New Program()
        one.M()
        ' This is the comment
        Dim two As Program = New Program()
        two.M()|]
        one.M()
        two.M()
    End Sub

    Private Sub M()
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Class Program
    Sub Main(args As String())
        Dim one As Program = Nothing
        Dim two As Program = Nothing
        {|Rename:NewMethod|}(one, two)
        one.M()
        two.M()
    End Sub

    Private Shared Sub NewMethod(ByRef one As Program, ByRef two As Program)
        one = New Program()
        one.M()
        ' This is the comment
        two = New Program()
        two.M()
    End Sub

    Private Sub M()
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(984831, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/984831")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestPreserveCommentsBeforeDeclaration_2() As Task
            Await TestAsync(
<Text>Class Program
    Sub Main(args As String())
        [|Dim one As Program = New Program()
        one.M()
        ' This is the comment
        Dim two As Program = New Program(), three As Program = New Program()
        two.M()|]
        one.M()
        two.M()
        three.M()
    End Sub

    Private Sub M()
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Class Program
    Sub Main(args As String())
        Dim one As Program = Nothing
        Dim two As Program = Nothing
        Dim three As Program = Nothing
        {|Rename:NewMethod|}(one, two, three)
        one.M()
        two.M()
        three.M()
    End Sub

    Private Shared Sub NewMethod(ByRef one As Program, ByRef two As Program, ByRef three As Program)
        one = New Program()
        one.M()
        ' This is the comment
        two = New Program()
        three = New Program()
        two.M()
    End Sub

    Private Sub M()
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <WorkItem(984831, "http://vstfdevdiv:8080/DevDiv2/DevDiv/_workitems/edit/984831")>
        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod)>
        Public Async Function TestPreserveCommentsBeforeDeclaration_3() As Task
            Await TestAsync(
<Text>Class Program
    Sub Main(args As String())
        [|Dim one As Program = New Program()
        one.M()
        ' This is the comment
        Dim two As Program = New Program()
        two.M()
        ' Second Comment
        Dim three As Program = New Program()
        three.M()|]
        one.M()
        two.M()
        three.M()
    End Sub

    Private Sub M()
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
<Text>Class Program
    Sub Main(args As String())
        Dim one As Program = Nothing
        Dim two As Program = Nothing
        Dim three As Program = Nothing
        {|Rename:NewMethod|}(one, two, three)
        one.M()
        two.M()
        three.M()
    End Sub

    Private Shared Sub NewMethod(ByRef one As Program, ByRef two As Program, ByRef three As Program)
        one = New Program()
        one.M()
        ' This is the comment
        two = New Program()
        two.M()
        ' Second Comment
        three = New Program()
        three.M()
    End Sub

    Private Sub M()
        Throw New NotImplementedException()
    End Sub
End Class
</Text>.Value.Replace(vbLf, vbCrLf),
compareTokens:=False)
        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)>
        <WorkItem(13042, "https://github.com/dotnet/roslyn/issues/13042")>
        Public Async Function TestTuples() As Task

            Await TestAsync(
"Class Program
    Sub Main(args As String())
        [|Dim x = (1, 2)|]
        M(x)
    End Sub
    Private Sub M(x As (Integer, Integer))
    End Sub
End Class
Namespace System
    Structure ValueTuple(Of T1, T2)
    End Structure
End Namespace",
"Class Program
    Sub Main(args As String())
        Dim x As (Integer, Integer) = {|Rename:NewMethod|}()
        M(x)
    End Sub
    Private Shared Function NewMethod() As (Integer, Integer)
        Return (1, 2)
    End Function
    Private Sub M(x As (Integer, Integer))
    End Sub
End Class
Namespace System
    Structure ValueTuple(Of T1, T2)
    End Structure
End Namespace")

        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)>
        <WorkItem(11196, "https://github.com/dotnet/roslyn/issues/11196")>
        Public Async Function TestTupleDeclarationWithNames() As Task

            Await TestAsync(
"Class Program
    Sub Main(args As String())
        [|Dim x As (a As Integer, b As Integer) = (1, 2)|]
        System.Console.WriteLine(x.a)
    End Sub
End Class
Namespace System
    Structure ValueTuple(Of T1, T2)
    End Structure
End Namespace",
"Class Program
    Sub Main(args As String())
        Dim x As (a As Integer, b As Integer) = {|Rename:NewMethod|}()
        System.Console.WriteLine(x.a)
    End Sub
    Private Shared Function NewMethod() As (a As Integer, b As Integer)
        Return (1, 2)
    End Function
End Class
Namespace System
    Structure ValueTuple(Of T1, T2)
    End Structure
End Namespace")

        End Function

        <Fact, Trait(Traits.Feature, Traits.Features.CodeActionsExtractMethod), Test.Utilities.CompilerTrait(Test.Utilities.CompilerFeature.Tuples)>
        <WorkItem(11196, "https://github.com/dotnet/roslyn/issues/11196")>
        Public Async Function TestTupleDeclarationWithSomeNames() As Task

            Await TestAsync(
"Class Program
    Sub Main(args As String())
        [|Dim x As (a As Integer, Integer) = (1, 2)|]
        System.Console.WriteLine(x.a)
    End Sub
End Class
Namespace System
    Structure ValueTuple(Of T1, T2)
    End Structure
End Namespace",
"Class Program
    Sub Main(args As String())
        Dim x As (a As Integer, Integer) = {|Rename:NewMethod|}()
        System.Console.WriteLine(x.a)
    End Sub
    Private Shared Function NewMethod() As (a As Integer, Integer)
        Return (1, 2)
    End Function
End Class
Namespace System
    Structure ValueTuple(Of T1, T2)
    End Structure
End Namespace")

        End Function

    End Class
End Namespace
