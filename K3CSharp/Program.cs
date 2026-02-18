using System;
using System.Collections.Generic;
using System.IO;

namespace K3CSharp
{
    class Program
    {
        private static readonly List<string> commandHistory = new List<string>();
        //private static int historyIndex = -1;
        
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
            Console.WriteLine("Type \\\\ to quit, \\ to cancel input, or \\help for help");
            Console.WriteLine("Use arrow keys for history, Ctrl+C to clear line");
            Console.WriteLine();

            while (true)
            {
                Console.Write("  "); // Default prompt: two spaces
                
                var input = ReadMultiLineInput();
                
                if (input == null) break;
                
                if (input == "\\\\" || input == "_exit")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                if (string.IsNullOrWhiteSpace(input)) continue;

                // Add to history if not empty and not duplicate of last
                if (commandHistory.Count == 0 || commandHistory[^1] != input)
                {
                    commandHistory.Add(input);
                    if (commandHistory.Count > 100) // Keep last 100 commands
                    {
                        commandHistory.RemoveAt(0);
                    }
                }
                //historyIndex = -1;

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

        static string? ReadMultiLineInput()
        {
            return Console.ReadLine();
        }
        
        static void ReplaceCurrentLine(string newText)
        {
            Console.Write(new string(' ', newText.Length));
            Console.Write($"\r    {newText}");
        }

        public static K3Value ExecuteLine(string input, Evaluator evaluator)
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
            return evaluator.Evaluate(ast) ?? new NullValue();
        }

        public static K3Value HandleReplCommand(string command, Evaluator evaluator)
        {
            var parts = command.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            switch (parts[0])
            {
                case "\\d":
                    if (parts.Length == 1)
                    {
                        // Display current branch
                        var currentBranch = evaluator.DirFunction(new NullValue());
                        if (currentBranch is SymbolValue sym && !string.IsNullOrEmpty(sym.Value))
                        {
                            Console.WriteLine(sym.Value);
                        }
                        else
                        {
                            // Root branch - display nothing per spec
                        }
                    }
                    else if (parts.Length == 2)
                    {
                        // Set current branch
                        evaluator.SetCurrentBranch(parts[1]);
                        Console.WriteLine($"Current branch set to: {parts[1]}");
                    }
                    else
                    {
                        Console.WriteLine("Usage: \\d [branch_path]");
                    }
                    return new NullValue();
                    
                case "\\9":
                    // Reset K tree to default state (for testing purposes)
                    // Also reset random seed to -314159 for reproducible tests
                    evaluator.ResetKTree();
                    Evaluator.RandomSeed = -314159;
                    Console.WriteLine("K tree reset to default state");
                    return new NullValue();
                    
                case "\\^":
                    // Set current branch to parent
                    evaluator.SetParentBranch();
                    var parentBranch = evaluator.DirFunction(new NullValue());
                    if (parentBranch is SymbolValue parentSym && !string.IsNullOrEmpty(parentSym.Value))
                    {
                        Console.WriteLine($"Current branch set to: {parentSym.Value}");
                    }
                    else
                    {
                        Console.WriteLine("Current branch set to root");
                    }
                    return new NullValue();
                    
                case "\\p":
                    if (parts.Length == 1)
                    {
                        // Display current precision
                        Console.WriteLine($"Current precision: {Evaluator.floatPrecision} significant digits");
                        Console.WriteLine("Usage: \\p <number> to set precision");
                    }
                    else if (parts.Length == 2)
                    {
                        // Set precision
                        if (int.TryParse(parts[1], out int newPrecision))
                        {
                            Evaluator.floatPrecision = newPrecision;
                            Console.WriteLine($"Precision set to: {newPrecision} significant digits");
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
                    
                case "\\r":
                    // Handle random seed get/set
                    if (parts.Length == 1)
                    {
                        // Display current random seed
                        Console.WriteLine(Evaluator.RandomSeed);
                    }
                    else if (parts.Length == 2)
                    {
                        // Set random seed
                        if (int.TryParse(parts[1], out int newSeed))
                        {
                            Evaluator.RandomSeed = newSeed;
                        }
                        else
                        {
                            Console.WriteLine("Invalid seed value. Use: \\r <number>");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Usage: \\r [number]");
                    }
                    return new NullValue();
                    
                case "\\":
                    // Display top level help message
                    Console.WriteLine("\\  help (this page)");
                    Console.WriteLine("\\0 data types");
                    Console.WriteLine("\\+ arithmetic and basic verbs");
                    Console.WriteLine("\\' adverbs");
                    Console.WriteLine("\\. control flow, assignment and debug");
                    Console.WriteLine("\\_ underscore verbs (math, linear, etc.)");
                    Console.WriteLine("\\p precision");
                    Console.WriteLine("\\r random seed");
                    Console.WriteLine("\\9 reset K tree and random seed (testing)");
                    Console.WriteLine("\\\\ exit");
                    break;
                    
                case "\\0":
                    // Display constants, literals and data types information
                    Console.WriteLine("int long float char symbol vector dictionary null");
                    Console.WriteLine("4: type");
                    Console.WriteLine("0i 0n -0i inf NaN -inf");
                    break;
                    
                case "\\+":
                    // Display arithmetic and basic verbs information
                    Console.WriteLine("arithmetic: + - * % < > = & | ^");
                    Console.WriteLine("Monadic: - + * % & | < > ^ ! , # _ ? ~ @ . = $");
                    Console.WriteLine("Dyadic: ! _ @ . :: $");
                    Console.WriteLine("indexing: @_n []");
                    Console.WriteLine("dict: !dict .dict");
                    Console.WriteLine("format: $value format${}expr");
                    Console.WriteLine("assignment: :");
                    Console.WriteLine("see \\_ for math, linear algebra, and system information verbs");
                    break;
                    
                case "\\'":
                    // Display adverbs information
                    Console.WriteLine("/ over reduce");
                    Console.WriteLine("\\ scan");
                    Console.WriteLine("' each");
                    Console.WriteLine("/: each-right");
                    Console.WriteLine("\\: each-left");
                    Console.WriteLine("': each-prior");
                    break;
                    
                case "\\.":
                    // Display control flow, assignment and debug information
                    Console.WriteLine("assignment:");
                    Console.WriteLine("  name: value        // local assignment");
                    Console.WriteLine("  name:: value       // global assignment");
                    Console.WriteLine("  {[x;y] x+y}        // function definition");
                    Console.WriteLine();
                    Console.WriteLine("control flow (bracket notation):");
                    Console.WriteLine("  if[condition; expr]     // conditional execution");
                    Console.WriteLine("  while[condition; expr]   // loop while condition true");
                    Console.WriteLine("  do[count; expr]          // repeat count times");
                    Console.WriteLine("  :[cond; true; false]     // conditional evaluation");
                    Console.WriteLine();
                    Console.WriteLine("control flow (apply notation):");
                    Console.WriteLine("  if . (condition; expr)   // equivalent to bracket notation");
                    Console.WriteLine("  while . (condition; expr)");
                    Console.WriteLine("  do . (count; expr)");
                    Console.WriteLine("  : . (cond; true; false)");
                    Console.WriteLine();
                    Console.WriteLine("debug:");
                    Console.WriteLine("  \\p [number]  // set precision");
                    Console.WriteLine("  \\r [number]  // set/get random seed");
                    Console.WriteLine("  \\9            // reset K tree and random seed (testing)");
                    Console.WriteLine("  \\help        // show this help");
                    break;
                    
                case "\\_":
                    // Display underscore verbs information
                    Console.WriteLine("Math:");
                    Console.WriteLine("  _log   _exp   _abs   _sqrt");
                    Console.WriteLine("  _sin   _cos   _tan");
                    Console.WriteLine("  _dot   _mul   _inv");
                    Console.WriteLine();
                    Console.WriteLine("Rand:");
                    Console.WriteLine("  _draw");
                    Console.WriteLine();
                    Console.WriteLine("Time:");
                    Console.WriteLine("  _t     _gtime  _ltime  _lt  _jd  _dj");
                    Console.WriteLine();
                    Console.WriteLine("List:");
                    Console.WriteLine("  _sv    _vs    _ss     _dv    _di");
                    Console.WriteLine("  _getenv _setenv _size   _exit");
                    Console.WriteLine();
                    Console.WriteLine("Vars:");
                    Console.WriteLine("  _d    _v    _i    _f    _n    _s    _h    _p    _P");
                    Console.WriteLine("  _w    _u    _a    _k    _o    _c    _r    _m    _y");
                    Console.WriteLine();
                    Console.WriteLine("see \\+ for arithmetic and binary verbs");
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
