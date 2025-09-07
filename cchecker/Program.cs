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
            using var fs = File.OpenRead(path);
            using var reader = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var input = new AntlrInputStream(reader);

            var lexer = new CSharpLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new CSharpParser(tokens);

            var tree = parser.prog();
            Console.WriteLine(Trees.ToStringTree(tree, parser.RuleNames));
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
}