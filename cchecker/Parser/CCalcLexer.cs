using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace cchecker.Parser
{
    // Lightweight lexer compatible with Antlr4.Runtime, no Java/codegen required
    // Tokens: INT, PLUS, MINUS, MUL, DIV, LPAREN, RPAREN, EOF
    public class CCalcLexer : ITokenSource
    {
        public const int INT = 1;
        public const int MUL = 2;
        public const int DIV = 3;
        public const int ADD = 4;
        public const int SUB = 5;
        public const int LPAREN = 6;
        public const int RPAREN = 7;

        private readonly ICharStream _input;
        private int _line = 1;
        private int _col = 0;
        private readonly ITokenFactory _tokenFactory = CommonTokenFactory.Default;

        private static readonly string[] LiteralNames = new string[]
        {
            null, null, "*", "/", "+", "-", "(", ")"
        };

        private static readonly string[] SymbolicNames = new string[]
        {
            null,
            nameof(INT), nameof(MUL), nameof(DIV), nameof(ADD), nameof(SUB), nameof(LPAREN), nameof(RPAREN)
        };

        private static readonly IVocabulary _vocabulary = new Vocabulary(LiteralNames, SymbolicNames);

        public CCalcLexer(ICharStream input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        public IToken NextToken()
        {
            while (true)
            {
                int c = _input.LA(1);
                if (c == IntStreamConstants.EOF)
                {
                    return _tokenFactory.Create(new Tuple<ITokenSource, ICharStream>(this, _input), TokenConstants.EOF, "<EOF>", TokenConstants.DefaultChannel, _input.Index, _input.Index, _line, _col);
                }

                char ch = (char)c;
                if (ch == ' ' || ch == '\t' || ch == '\r')
                {
                    Consume();
                    continue;
                }
                if (ch == '\n')
                {
                    ConsumeNewLine();
                    continue;
                }

                if (char.IsDigit(ch))
                {
                    int start = _input.Index;
                    int startCol = _col;
                    int startLine = _line;
                    while (char.IsDigit((char)_input.LA(1)))
                    {
                        Consume();
                    }
                    string text = GetText(start, _input.Index - 1);
                    return _tokenFactory.Create(new Tuple<ITokenSource, ICharStream>(this, _input), INT, text, TokenConstants.DefaultChannel, start, _input.Index - 1, startLine, startCol);
                }

                switch (ch)
                {
                    case '*': return SingleCharToken(MUL, "*");
                    case '/': return SingleCharToken(DIV, "/");
                    case '+': return SingleCharToken(ADD, "+");
                    case '-': return SingleCharToken(SUB, "-");
                    case '(': return SingleCharToken(LPAREN, "(");
                    case ')': return SingleCharToken(RPAREN, ")");
                    default:
                        throw new ParseCanceledException($"Unknown character '{ch}' at line {_line}, col {_col}");
                }
            }
        }

        private IToken SingleCharToken(int type, string text)
        {
            int start = _input.Index;
            int startCol = _col;
            int startLine = _line;
            Consume();
            return _tokenFactory.Create(new Tuple<ITokenSource, ICharStream>(this, _input), type, text, TokenConstants.DefaultChannel, start, start, startLine, startCol);
        }

        private void Consume()
        {
            _input.Consume();
            _col++;
        }

        private void ConsumeNewLine()
        {
            _input.Consume();
            _line++;
            _col = 0;
        }

        private string GetText(int startIdx, int stopIdx)
        {
            return _input.GetText(new Interval(startIdx, stopIdx));
        }

        public string SourceName => _input.SourceName;
        public ICharStream InputStream => _input;
        public ITokenFactory TokenFactory { get => _tokenFactory; set { } }
        public IVocabulary Vocabulary => _vocabulary;
        public int Line => _line;
        public int Column => _col;
    }
}
