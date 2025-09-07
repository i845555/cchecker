using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Misc;
using cchecker.Parser;
using Xunit;
using AwesomeAssertions;

namespace cchecker.Tests;

public class CSharpParserTests
{
    private static CSharpParser MakeParser(string input, bool bail = false)
    {
        var inputStream = new AntlrInputStream(input);
        var lexer = new CSharpLexer(inputStream);
        if (bail)
        {
            lexer.RemoveErrorListeners();
        }
        var tokens = new CommonTokenStream(lexer);
        var parser = new CSharpParser(tokens);
        if (bail)
        {
            parser.RemoveErrorListeners();
            parser.ErrorHandler = new BailErrorStrategy();
        }
        return parser;
    }

    [Fact]
    public void Parses_Simple_Class_In_Namespace()
    {
        var code = @"using System;\nnamespace Demo { public class A { int x; public void M(int y) { return; } } }";
        var parser = MakeParser(code);
        var tree = parser.prog();
        var treeStr = Trees.ToStringTree(tree, parser.RuleNames);
        treeStr.Should().Contain("class");
        treeStr.Should().Contain("namespace");
        treeStr.Should().Contain("A");
    }

    [Fact]
    public void Parses_Using_And_Multiple_Members()
    {
        var code = @"using System.Text;\nnamespace N { public class C { int a = 1; int b = 2; public int Add(int x, int y) { return x; } } }";
        var parser = MakeParser(code);
        var tree = parser.prog();
        var treeStr = Trees.ToStringTree(tree, parser.RuleNames);
        treeStr.Should().Contain("using");
        treeStr.Should().Contain("class");
        treeStr.Should().Contain("Add");
    }

    [Fact]
    public void Invalid_Code_Throws_With_Bail_Strategy()
    {
        var bad = "namespace { class X { }"; // missing name and closing braces
        var parser = MakeParser(bad, bail: true);
        Action act = () => parser.prog();
        act.Should().Throw<ParseCanceledException>();
    }
}
