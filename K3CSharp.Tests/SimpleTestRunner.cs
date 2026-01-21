using System;
using System.Collections.Generic;
using System.IO;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class SimpleTestRunner
    {
        private readonly string testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");

        public static void Main(string[] args)
        {
            RunAllTests();
        }
        
        public static void RunAllTests()
        {
            var tests = new[]
            {
                ("simple_addition.k", "3"),
                ("simple_subtraction.k", "2"),
                ("simple_multiplication.k", "12"),
                ("simple_division.k", "4"),
                ("vector_addition.k", "(4;6)"),
                ("vector_subtraction.k", "(-2;-2)"),
                ("vector_multiplication.k", "(3;8)"),
                ("vector_division.k", "(0;0)"),
                ("scalar_vector_addition.k", "(4;5)"),
                ("scalar_vector_multiplication.k", "(3;6)"),
                ("parenthesized_vector.k", "(1;2;3;4)"),
                ("variable_assignment.k", "7"),
                ("integer_types.k", "123456789L"),
                ("float_types.k", "170"),
                ("character_types.k", "\"hello\""),
                ("symbol_types.k", "`\"a symbol\""),
                ("variable_usage.k", "30"),
                ("variable_reassignment.k", "(7.2;4.5)"),
                ("minimum_operator.k", "3"),
                ("maximum_operator.k", "5"),
                ("less_than_operator.k", "1"),
                ("greater_than_operator.k", "0"),
                ("equal_operator.k", "0"),
                ("power_operator.k", "8"),
                ("modulus_operator.k", "1"),
                ("negate_operator.k", "1"),
                ("join_operator.k", "(3;5)"),
                ("anonymous_functions.k", "13"),
                ("function_application.k", "13"),
                ("complex_function.k", "205"),
                ("variable_scoping.k", "25")
            };

            int passed = 0;
            int total = tests.Length;

            foreach (var (scriptFile, expected) in tests)
            {
                var result = ExecuteK3Script(scriptFile);
                Console.WriteLine($"{(scriptFile)}: {result}");
                
                if (result == expected)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ {scriptFile}: {result}");
                    passed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {scriptFile}: Expected '{expected}', got '{result}'");
                }
                
                Console.ResetColor();
            }

            Console.WriteLine($"Test Results: {passed}/{total} passed");
        }

        private static string ExecuteK3Script(string scriptFileName)
        {
            var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts", scriptFileName);
            var scriptContent = File.ReadAllText(scriptPath);
            
            // Use single evaluator for entire script to maintain variable state
            var evaluator = new Evaluator();
            
            // Check if script contains multi-line constructs (blocks)
            if (scriptContent.Contains("{") && scriptContent.Contains("}"))
            {
                // For scripts with blocks, try to parse entire script first
                try
                {
                    var lexer = new Lexer(scriptContent);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens);
                    var ast = parser.Parse();
                    var result = evaluator.Evaluate(ast);
                    return result.ToString();
                }
                catch
                {
                    // If parsing fails, fall back to line-by-line evaluation
                    var lines = scriptContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    string lastResult = "";
                    
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (!string.IsNullOrEmpty(trimmedLine))
                        {
                            try
                            {
                                var lineLexer = new Lexer(trimmedLine);
                                var lineTokens = lineLexer.Tokenize();
                                var lineParser = new Parser(lineTokens);
                                var lineAst = lineParser.Parse();
                                var lineResult = evaluator.Evaluate(lineAst);
                                lastResult = lineResult.ToString();
                            }
                            catch
                            {
                                // Skip lines that cause parsing errors
                                continue;
                            }
                        }
                    }
                    
                    return lastResult;
                }
            }
            else
            {
                // For simple scripts, use line-by-line evaluation
                var lines = scriptContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                string lastResult = "";
                
                foreach (var line in lines)
                {
                    var trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine))
                    {
                        var lineLexer = new Lexer(trimmedLine);
                        var lineTokens = lineLexer.Tokenize();
                        var lineParser = new Parser(lineTokens);
                        var lineAst = lineParser.Parse();
                        var lineResult = evaluator.Evaluate(lineAst);
                        lastResult = lineResult.ToString();
                    }
                }
                
                return lastResult;
            }
        }
    }
}