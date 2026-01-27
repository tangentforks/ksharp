using System;
using K3CSharp;

class TestEachBehavior
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Testing Each Adverb Behavior");
        Console.WriteLine("============================");
        
        try
        {
            var evaluator = new Evaluator();
            
            // Test the each adverb behavior
            var script = "(10 20 30) -' (1 2 3)";
            Console.WriteLine($"Script: {script}");
            
            // Parse the script first
            var lexer = new Lexer(script);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens, script);
            var ast = parser.Parse();
            
            var result = evaluator.Evaluate(ast);
            Console.WriteLine($"K3Sharp result: {result}");
            
            // Test what k.exe should produce (element-wise)
            Console.WriteLine($"Expected k.exe result: 9 18 27");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
    }
}
