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
                    Console.WriteLine("\\  help (this page)");
                    Console.WriteLine("\\0 data types");
                    Console.WriteLine("\\+ arithmetic and basic verbs");
                    Console.WriteLine("\\' adverbs");
                    Console.WriteLine("\\. control flow, assignment and debug");
                    Console.WriteLine("\\_ underscore verbs (math, linear, etc.)");
                    Console.WriteLine("\\p precision");
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
                    Console.WriteLine("unary: - + * % & | < > ^ ! , # _ ? ~ @ . = $");
                    Console.WriteLine("binary: ! _ @ . :: $");
                    Console.WriteLine("indexing: @_n []");
                    Console.WriteLine("dict: !dict .dict");
                    Console.WriteLine("format: $value format${}expr");
                    Console.WriteLine("see \\_ for math, linear algebra, and other underscore verbs");
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
                    Console.WriteLine("  \\help        // show this help");
                    break;
                    
                case "\\_":
                    // Display underscore verbs information
                    Console.WriteLine("math functions:");
                    Console.WriteLine("  _log   _exp   _abs   _sqrt");
                    Console.WriteLine("  _sin   _cos   _tan");
                    Console.WriteLine();
                    Console.WriteLine("linear algebra:");
                    Console.WriteLine("  _dot   _mul   _inv");
                    Console.WriteLine();
                    Console.WriteLine("group:");
                    Console.WriteLine("  =      // group/ungroup");
                    Console.WriteLine();
                    Console.WriteLine("other underscore verbs:");
                    Console.WriteLine("  _do    _while _if    // internal control flow");
                    Console.WriteLine("see \\+ for basic arithmetic and binary verbs");
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
