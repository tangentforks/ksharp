using System;
using System.Collections.Generic;
using System.IO;

namespace K3CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var evaluator = new Evaluator();
            
            if (args.Length > 0)
            {
                // Execute file
                try
                {
                    var content = File.ReadAllText(args[0]);
                    ExecuteLine(content, evaluator);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return;
            }

            // REPL mode
            Console.WriteLine("K3 Interpreter - Version 1.0");
            Console.WriteLine("Type \\\\ or _exit to quit");
            Console.WriteLine();

            while (true)
            {
                Console.Write("K3> ");
                var input = Console.ReadLine();
                
                if (input == null) break;
                
                if (input == "\\\\" || input == "_exit")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                if (string.IsNullOrWhiteSpace(input)) continue;

                try
                {
                    var result = ExecuteLine(input, evaluator);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        static K3Value ExecuteLine(string input, Evaluator evaluator)
        {
            var lexer = new Lexer(input);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens);
            var ast = parser.Parse();
            return evaluator.Evaluate(ast);
        }
    }
}
