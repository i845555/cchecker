using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace cchecker.Parser
{
    // Minimal hand-written parser that produces ANTLR-style parse trees (ParserRuleContext + TerminalNodeImpl)
    // Grammar (simplified):
    // prog: expr EOF;
    // expr: expr ('*'|'/') expr
    //     | expr ('+'|'-') expr
    //     | INT
    //     | '(' expr ')'
    //     ;
    public class CCalcParser
    {
        public const int RULE_prog = 0;
        public const int RULE_expr = 1;

        public static readonly string[] RuleNames = { "prog", "expr" };

        private readonly ITokenStream _input;

        public CCalcParser(ITokenStream input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        public ProgContext Prog()
        {
            var ctx = new ProgContext();
            var e = ParseExpr();
            ctx.AddChild(e);
            var eofToken = Match(TokenConstants.EOF, ctx);
            ctx.AddChild(new TerminalNodeImpl(eofToken));
            return ctx;
        }

        private ExprContext ParseExpr()
        {
            // Precedence climbing: factor (*|/) factor ... then (+|-) ...
            var left = ParseFactor();

            // Term level (*,/)
            while (true)
            {
                int la = _input.LA(1);
                if (la == CCalcLexer.MUL || la == CCalcLexer.DIV)
                {
                    var op = _input.LT(1);
                    _input.Consume();
                    var right = ParseFactor();
                    var parent = new ExprContext();
                    parent.AddChild(left);
                    parent.AddChild(new TerminalNodeImpl(op));
                    parent.AddChild(right);
                    left = parent;
                    continue;
                }
                break;
            }

            // Additive level (+,-)
            while (true)
            {
                int la = _input.LA(1);
                if (la == CCalcLexer.ADD || la == CCalcLexer.SUB)
                {
                    var op = _input.LT(1);
                    _input.Consume();
                    var right = ParseTerm();
                    var parent = new ExprContext();
                    parent.AddChild(left);
                    parent.AddChild(new TerminalNodeImpl(op));
                    parent.AddChild(right);
                    left = parent;
                    continue;
                }
                break;
            }

            return left;
        }

        private ExprContext ParseTerm()
        {
            var left = ParseFactor();
            while (true)
            {
                int la = _input.LA(1);
                if (la == CCalcLexer.MUL || la == CCalcLexer.DIV)
                {
                    var op = _input.LT(1);
                    _input.Consume();
                    var right = ParseFactor();
                    var parent = new ExprContext();
                    parent.AddChild(left);
                    parent.AddChild(new TerminalNodeImpl(op));
                    parent.AddChild(right);
                    left = parent;
                    continue;
                }
                break;
            }
            return left;
        }

        private ExprContext ParseFactor()
        {
            int la = _input.LA(1);
            if (la == CCalcLexer.INT)
            {
                var ctx = new ExprContext();
                var t = Match(CCalcLexer.INT, ctx);
                ctx.AddChild(new TerminalNodeImpl(t));
                return ctx;
            }
            if (la == CCalcLexer.LPAREN)
            {
                var ctx = new ExprContext();
                Match(CCalcLexer.LPAREN, ctx);
                var inner = ParseExpr();
                ctx.AddChild(inner);
                Match(CCalcLexer.RPAREN, ctx);
                return ctx;
            }
            throw new ParseCanceledException($"Unexpected token type {la} at: '{_input.LT(1).Text}'");
        }

        private IToken Match(int ttype, ParserRuleContext current)
        {
            var t = _input.LT(1);
            if (t.Type != ttype)
            {
                throw new ParseCanceledException($"Mismatched token {t.Text}, expecting type {ttype}");
            }
            _input.Consume();
            return t;
        }

        // Context classes with RuleIndex values for ToStringTree
        public class ProgContext : ParserRuleContext
        {
            public override int RuleIndex => RULE_prog;
        }

        public class ExprContext : ParserRuleContext
        {
            public override int RuleIndex => RULE_expr;
        }
    }
}
