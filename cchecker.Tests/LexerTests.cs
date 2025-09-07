using System.Collections.Generic;
using Antlr4.Runtime;
using cchecker.Parser;
using Xunit;

namespace cchecker.Tests;

public class LexerTests
{
    private static List<int> LexTypes(string input)
    {
        var lexer = new CCalcLexer(new AntlrInputStream(input));
        var types = new List<int>();
        while (true)
        {
            var t = lexer.NextToken();
            if (t.Type == TokenConstants.EOF) break;
            types.Add(t.Type);
        }
        return types;
    }

    [Fact]
    public void Lexes_Integers_And_Operators()
    {
        var types = LexTypes("12+3*(4-5)/6");
        Assert.Equal(new[]
        {
            CCalcLexer.INT,
            CCalcLexer.ADD,
            CCalcLexer.INT,
            CCalcLexer.MUL,
            CCalcLexer.LPAREN,
            CCalcLexer.INT,
            CCalcLexer.SUB,
            CCalcLexer.INT,
            CCalcLexer.RPAREN,
            CCalcLexer.DIV,
            CCalcLexer.INT
        }, types);
    }

    [Fact]
    public void Skips_Whitespace()
    {
        var types = LexTypes("1 + 2\n  * 3\t");
        Assert.Equal(new[] { CCalcLexer.INT, CCalcLexer.ADD, CCalcLexer.INT, CCalcLexer.MUL, CCalcLexer.INT }, types);
    }
}