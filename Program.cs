using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace K3CSharp
{
    class Program
    {
        private static List<string> commandHistory = new List<string>();
        private static int historyIndex = -1;
        private static StringBuilder currentLine = new StringBuilder();
        private static int cursorPosition = 0;
        
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
                Console.Write("  "); // Default prompt: two spaces per spec
                
                var input = ReadMultiLineInput();
                
                if (input == null) break;
                
                if (input == "\\\\" || input == "_exit")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }

                if (string.IsNullOrWhiteSpace(input)) continue;

                // Add to history if not empty and not duplicate of last
                if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != input)
                {
                    commandHistory.Add(input);
                    if (commandHistory.Count > 100) // Keep last 100 commands
                    {
                        commandHistory.RemoveAt(0);
                    }
                }
                historyIndex = -1;

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

        static string ReadMultiLineInput()
        {
            var lines = new List<string>();
            int nestingLevel = 0;
            
            while (true)
            {
                // Build prompt based on nesting level
                string prompt = "  " + new string('>', nestingLevel);
                Console.Write(prompt);
                
                var line = ReadLineWithHistory();
                
                if (line == null) return null;
                
                // Handle cancellation
                if (line == "\\")
                {
                    Console.WriteLine("(cancelled)");
                    return "";
                }
                
                // Handle quit commands
                if (line == "\\\\" || line == "_exit")
                {
                    return line;
                }
                
                if (string.IsNullOrWhiteSpace(line)) 
                {
                    // Empty line - add newline if in multi-line mode
                    if (nestingLevel > 0)
                    {
                        lines.Add("");
                    }
                    continue;
                }
                
                lines.Add(line);
                
                // Check if the current input forms a complete expression
                var combinedInput = string.Join("\n", lines);
                var lexer = new Lexer(combinedInput);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens, combinedInput);
                
                if (parser.IsIncompleteExpression())
                {
                    // Calculate nesting level for prompt
                    nestingLevel = CalculateNestingLevel(combinedInput);
                }
                else
                {
                    // Expression is complete
                    break;
                }
            }
            
            return string.Join("\n", lines);
        }
        
        static int CalculateNestingLevel(string input)
        {
            int parentheses = 0;
            int brackets = 0;
            int braces = 0;
            bool inString = false;
            
            foreach (char c in input)
            {
                if (c == '"' && !inString)
                {
                    inString = true;
                }
                else if (c == '"' && inString)
                {
                    inString = false;
                }
                else if (!inString)
                {
                    switch (c)
                    {
                        case '(':
                            parentheses++;
                            break;
                        case ')':
                            parentheses--;
                            break;
                        case '[':
                            brackets++;
                            break;
                        case ']':
                            brackets--;
                            break;
                        case '{':
                            braces++;
                            break;
                        case '}':
                            braces--;
                            break;
                    }
                }
            }
            
            return Math.Max(0, Math.Max(parentheses, Math.Max(brackets, braces)));
        }

        static string ReadLineWithHistory()
        {
            currentLine.Clear();
            cursorPosition = 0;
            
            while (true)
            {
                var key = Console.ReadKey(true);
                
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        Console.WriteLine();
                        return currentLine.ToString();
                        
                    case ConsoleKey.Escape:
                        // Clear line
                        Console.Write(new string(' ', currentLine.Length));
                        Console.Write("\rK3> ");
                        currentLine.Clear();
                        cursorPosition = 0;
                        break;
                        
                    case ConsoleKey.UpArrow:
                        if (commandHistory.Count > 0)
                        {
                            if (historyIndex == -1)
                                historyIndex = commandHistory.Count - 1;
                            else if (historyIndex > 0)
                                historyIndex--;
                            
                            ReplaceCurrentLine(commandHistory[historyIndex]);
                        }
                        break;
                        
                    case ConsoleKey.DownArrow:
                        if (historyIndex != -1)
                        {
                            if (historyIndex < commandHistory.Count - 1)
                            {
                                historyIndex++;
                                ReplaceCurrentLine(commandHistory[historyIndex]);
                            }
                            else
                            {
                                historyIndex = -1;
                                ReplaceCurrentLine("");
                            }
                        }
                        break;
                        
                    case ConsoleKey.LeftArrow:
                        if (cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.Write("\b");
                        }
                        break;
                        
                    case ConsoleKey.RightArrow:
                        if (cursorPosition < currentLine.Length)
                        {
                            cursorPosition++;
                            Console.Write(currentLine[cursorPosition - 1]);
                        }
                        break;
                        
                    case ConsoleKey.Backspace:
                        if (cursorPosition > 0)
                        {
                            cursorPosition--;
                            currentLine.Remove(cursorPosition, 1);
                            Console.Write("\b" + currentLine.ToString().Substring(cursorPosition) + " ");
                            Console.Write($"\rK3> {currentLine}");
                            for (int i = 0; i < cursorPosition; i++)
                                Console.Write(new string('\b', 1));
                        }
                        break;
                        
                    case ConsoleKey.Delete:
                        if (cursorPosition < currentLine.Length)
                        {
                            currentLine.Remove(cursorPosition, 1);
                            Console.Write(currentLine.ToString().Substring(cursorPosition) + " ");
                            Console.Write($"\rK3> {currentLine}");
                            for (int i = 0; i < cursorPosition; i++)
                                Console.Write(new string('\b', 1));
                        }
                        break;
                        
                    case ConsoleKey.Home:
                        while (cursorPosition > 0)
                        {
                            cursorPosition--;
                            Console.Write("\b");
                        }
                        break;
                        
                    case ConsoleKey.End:
                        while (cursorPosition < currentLine.Length)
                        {
                            Console.Write(currentLine[cursorPosition]);
                            cursorPosition++;
                        }
                        break;
                        
                    case ConsoleKey.C when key.Modifiers == ConsoleModifiers.Control:
                        // Clear line
                        Console.Write(new string(' ', currentLine.Length));
                        Console.Write("\rK3> ");
                        currentLine.Clear();
                        cursorPosition = 0;
                        break;
                        
                    default:
                        if (!char.IsControl(key.KeyChar))
                        {
                            currentLine.Insert(cursorPosition, key.KeyChar);
                            Console.Write(currentLine.ToString().Substring(cursorPosition));
                            cursorPosition++;
                            
                            // Move cursor back to correct position
                            for (int i = cursorPosition; i < currentLine.Length; i++)
                                Console.Write("\b");
                        }
                        break;
                }
            }
        }

        static void ReplaceCurrentLine(string newText)
        {
            Console.Write(new string(' ', currentLine.Length));
            Console.Write($"\rK3> {newText}");
            currentLine.Clear();
            currentLine.Append(newText);
            cursorPosition = currentLine.Length;
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
                    
                case "\\":
                    // Display top level help message
                    Console.WriteLine("K3 REPL Help Commands:");
                    Console.WriteLine("  \\       - Display this help message");
                    Console.WriteLine("  \\p [n]  - Set or display float precision (default: 7)");
                    Console.WriteLine("  \\0      - Display information about data types");
                    Console.WriteLine("  \\+      - Display information about verbs/operators");
                    Console.WriteLine("  \\'      - Display information about adverbs");
                    Console.WriteLine("  \\.      - Display information about assignment");
                    Console.WriteLine("  \\\\      - Exit the REPL");
                    Console.WriteLine("");
                    Console.WriteLine("Note: Partial implementation of K3 specification.");
                    break;
                    
                case "\\0":
                    // Display constants, literals and data types information
                    Console.WriteLine("Data Types:");
                    Console.WriteLine("  Integer: 42, 123L (64-bit)");
                    Console.WriteLine("  Float: 3.14, 2.5e10");
                    Console.WriteLine("  Character: \"a\", \"hello\"");
                    Console.WriteLine("  Symbol: `symbol, `\"symbol with spaces\"");
                    Console.WriteLine("  Vector: (1;2;3), 1 2 3");
                    Console.WriteLine("  Dictionary: .(`a;1);(`b;2)");
                    Console.WriteLine("  Null: _n");
                    Console.WriteLine("  Special floats: 0i (inf), 0n (NaN), -0i (-inf)");
                    Console.WriteLine("  Type operator: 4: returns type code");
                    Console.WriteLine("");
                    Console.WriteLine("Note: Partial implementation of K3 data types.");
                    break;
                    
                case "\\+":
                    // Display verbs information
                    Console.WriteLine("Verbs (Operators):");
                    Console.WriteLine("  Arithmetic: + - * %");
                    Console.WriteLine("  Comparison: < > =");
                    Console.WriteLine("  Min/Max: & |");
                    Console.WriteLine("  Power: ^");
                    Console.WriteLine("  Unary: - + * % & | < > ^ ! , # _ ? ~ @ .");
                    Console.WriteLine("  Binary: ! _ @ . 4: ::");
                    Console.WriteLine("  Math: _log _exp _abs _sqrt _sin _cos _tan");
                    Console.WriteLine("  Linear: _dot _mul _inv");
                    Console.WriteLine("");
                    Console.WriteLine("Note: Partial implementation of K3 verbs.");
                    break;
                    
                case "\\'":
                    // Display adverbs information
                    Console.WriteLine("Adverbs (Higher-order functions):");
                    Console.WriteLine("  / (over/reduce):  +/ 1 2 3 4 -> 10");
                    Console.WriteLine("  \\ (scan):        +\\ 1 2 3 4 -> (1;3;6;10)");
                    Console.WriteLine("  ' (each):         +' 1 2 3 4 -> (1;2;3;4)");
                    Console.WriteLine("Adverbs have higher precedence than unary operators");
                    Console.WriteLine("");
                    Console.WriteLine("Note: Partial implementation of K3 adverbs.");
                    break;
                    
                case "\\.":
                    // Display assignment information
                    Console.WriteLine("Assignment:");
                    Console.WriteLine("  Global:    name: value");
                    Console.WriteLine("  Local:     [name] value (in functions)");
                    Console.WriteLine("  Functions: name: {[params]} body");
                    Console.WriteLine("  Global from function: name :: value");
                    Console.WriteLine("Assignment has lower precedence than adverbs");
                    Console.WriteLine("");
                    Console.WriteLine("Note: Partial implementation of K3 assignment.");
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
