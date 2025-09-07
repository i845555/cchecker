using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using cchecker.Parser;
using Xunit;
using AwesomeAssertions;

namespace cchecker.Tests;

public class ParserTests
{
    private static CCalcParser.ProgContext Parse(string input)
    {
        var lexer = new CCalcLexer(new AntlrInputStream(input));
        var tokens = new CommonTokenStream(lexer);
        var parser = new CCalcParser(tokens);
        return parser.Prog();
    }

    [Fact]
    public void Parses_Simple_Expression_And_Contains_Operators_In_Tree()
    {
        var tree = Parse("1+2*3");
        var treeStr = Trees.ToStringTree(tree, CCalcParser.RuleNames);
        treeStr.Should().Contain("+");
        treeStr.Should().Contain("*");
        treeStr.Should().EndWith("<EOF>)");
    }

    [Fact]
    public void Parses_Parentheses_With_Precedence()
    {
        var tree = Parse("(1+2)*3");
        var treeStr = Trees.ToStringTree(tree, CCalcParser.RuleNames);
        // Should still contain both '+' and '*'
        treeStr.Should().Contain("+");
        treeStr.Should().Contain("*");
    }
}