using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using cchecker.Parser;

namespace cchecker;

internal static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error.WriteLine("Usage: cchecker <input-file>");
            return 1;
        }

        var path = args[0];
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"File not found: {path}");
            return 2;
        }

        try
        {
            // Read entire file to keep original text for slicing
            var codeText = File.ReadAllText(path, Encoding.UTF8);
            var input = new AntlrInputStream(codeText);

            var lexer = new CSharpLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);

            var tree = parser.prog();

            // Traverse tree to find methods longer than 5 lines and print them
            PrintLongMethods(tree, codeText, minLinesExclusive: 5);

            return 0;
        }
        catch (ParseCanceledException pce)
        {
            Console.Error.WriteLine($"Parse error: {pce.Message}");
            return 3;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 4;
        }
    }

    private static void PrintLongMethods(IParseTree tree, string sourceText, int minLinesExclusive)
    {
        void Walk(IParseTree node)
        {
            if (node is ParserRuleContext prc)
            {
                if (prc.RuleIndex == CSharpParser.RULE_method_declaration)
                {
                    var start = prc.Start;
                    var stop = prc.Stop;
                    if (start != null && stop != null)
                    {
                        var lineCount = (stop.Line - start.Line + 1);
                        if (lineCount > minLinesExclusive)
                        {
                            var startIdx = start.StartIndex;
                            var stopIdx = stop.StopIndex;
                            if (startIdx >= 0 && stopIdx >= startIdx && stopIdx < sourceText.Length)
                            {
                                var snippet = sourceText.Substring(startIdx, stopIdx - startIdx + 1);
                                Console.WriteLine(snippet);
                            }
                        }
                    }
                }
                // Recurse into children
                for (int i = 0; i < prc.ChildCount; i++)
                {
                    Walk(prc.GetChild(i));
                }
            }
            else
            {
                // Non-rule nodes: still walk if they have children
                for (int i = 0; i < node.ChildCount; i++)
                {
                    Walk(node.GetChild(i));
                }
            }
        }

        Walk(tree);
    }

        public void Parses_Parentheses_With_Precedence()
    {        var tree = Parse("(1+2)*3");
        // Should still contain both '+' and '*'

    }
}