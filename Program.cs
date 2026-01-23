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
                    var result = ExecuteLine(content, evaluator);
                    Console.WriteLine(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                return;
            }

            // REPL mode
            Console.WriteLine("K3 Interpreter - Version 1.0");
            Console.WriteLine("Type \\\\ to quit, or \\ for help");
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
            // Handle REPL commands (prefixed with backslash)
            var trimmedInput = input.Trim().Trim('\uFEFF', '\u200B'); // Remove BOM and zero-width spaces
            if (trimmedInput.StartsWith("\\"))
            {
                return HandleReplCommand(trimmedInput, evaluator);
            }
            
            var lexer = new Lexer(input);
            var tokens = lexer.Tokenize();
            var parser = new Parser(tokens, input);
            var ast = parser.Parse();
            return evaluator.Evaluate(ast);
        }

        static K3Value HandleReplCommand(string command, Evaluator evaluator)
        {
            var parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            switch (parts[0])
            {
                case "\\p":
                    if (parts.Length == 1)
                    {
                        // Display current precision
                        Console.WriteLine($"Current precision: {Evaluator.floatPrecision}");
                    }
                    else if (parts.Length == 2)
                    {
                        // Set precision
                        if (int.TryParse(parts[1], out int newPrecision))
                        {
                            Evaluator.floatPrecision = newPrecision;
                            Console.WriteLine($"Precision set to: {newPrecision}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid precision value. Use: \\p <number>");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Usage: \\p [number]");
                    }
                    return new NullValue();
                    
                case "\\":
                    // Display top level help message
                    Console.WriteLine("K3 REPL Help Commands:");
                    Console.WriteLine("  \\       - Display this help message");
                    Console.WriteLine("  \\p [n]  - Set or display float precision (default: 7)");
                    Console.WriteLine("  \\0      - Display information about constants, literals and data types");
                    Console.WriteLine("  \\+      - Display information about verbs");
                    Console.WriteLine("  \\'      - Display information about adverbs");
                    Console.WriteLine("  \\.      - Display information about assignment");
                    Console.WriteLine("  \\\\      - Exit the REPL");
                    Console.WriteLine("");
                    Console.WriteLine("Note: This is a partial implementation. The K3 specification");
                    Console.WriteLine("is still being developed and additional commands may be added.");
                    break;
                    
                case "\\0":
                    // Display constants, literals and data types information
                    Console.WriteLine("Constants, Literals and Data Types:");
                    Console.WriteLine("  Integer literals: 42, 123L (64-bit)");
                    Console.WriteLine("  Float literals: 3.14, 2.5e10");
                    Console.WriteLine("  Character literals: \"a\", \"hello\"");
                    Console.WriteLine("  Symbol literals: `symbol, `\"symbol with spaces\"");
                    Console.WriteLine("  Null value: _n");
                    Console.WriteLine("  Special floats: 0i (inf), 0n (NaN), -0i (-inf)");
                    Console.WriteLine("  Vector literals: (1;2;3), (\"a\";\"b\";\"c\")");
                    Console.WriteLine("  Type operator 4: returns type code of value");
                    Console.WriteLine("");
                    Console.WriteLine("Note: This is a partial implementation of K3 data types.");
                    break;
                    
                case "\\+":
                    // Display verbs information
                    Console.WriteLine("Verbs (Operators):");
                    Console.WriteLine("  Arithmetic: + - * % (add, subtract, multiply, divide)");
                    Console.WriteLine("  Comparison: < > = (less, greater, equal)");
                    Console.WriteLine("  Min/Max: & | (minimum, maximum)");
                    Console.WriteLine("  Power: ^ (exponentiation)");
                    Console.WriteLine("  Modulus: ! (remainder)");
                    Console.WriteLine("  Join: , (concatenate)");
                    Console.WriteLine("  Unary: - ~ (negate, not)");
                    Console.WriteLine("  First: * (first of list/vector)");
                    Console.WriteLine("  Reciprocal: % (1/x)");
                    Console.WriteLine("  Generate: ! (zeros)");
                    Console.WriteLine("  Reverse: | (reverse list)");
                    Console.WriteLine("  Count: # (length of list)");
                    Console.WriteLine("  Enumerate: ! (range 0..n-1)");
                    Console.WriteLine("  Enlist: , (single-item list)");
                    Console.WriteLine("  Floor: _ (floor function)");
                    Console.WriteLine("  Unique: ? (unique values)");
                    Console.WriteLine("  Grade: < > (grade up/down)");
                    Console.WriteLine("  Shape: ^ (shape of array)");
                    Console.WriteLine("");
                    Console.WriteLine("Note: This is a partial implementation of K3 verbs.");
                    break;
                    
                case "\\'":
                    // Display adverbs information
                    Console.WriteLine("Adverbs (Higher-order functions):");
                    Console.WriteLine("  / (over/reduce):  +/ 1 2 3 4  -> 10");
                    Console.WriteLine("  \\ (scan):        +\\ 1 2 3 4  -> (1;3;6;10)");
                    Console.WriteLine("  ' (each):         +' 1 2 3 4  -> (1;2;3;4)");
                    Console.WriteLine("Adverbs can be chained: /:\\ (each-then-reduce)");
                    Console.WriteLine("Adverbs have higher precedence than unary operators");
                    Console.WriteLine("");
                    Console.WriteLine("Note: This is a partial implementation of K3 adverbs.");
                    break;
                    
                case "\\.":
                    // Display assignment information
                    Console.WriteLine("Assignment:");
                    Console.WriteLine("  Global:    name: value");
                    Console.WriteLine("  Local:     [name] value (in functions)");
                    Console.WriteLine("  Functions: name: {[params]} body");
                    Console.WriteLine("  Verb names: number:verb (e.g., 2:+ is a valid verb name)");
                    Console.WriteLine("Assignment has lower precedence than adverbs");
                    Console.WriteLine("");
                    Console.WriteLine("Note: This is a partial implementation of K3 assignment.");
                    break;
                    
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    Console.WriteLine("Type \\ for help on available commands");
                    break;
            }
            return new NullValue();
        }
    }
}
