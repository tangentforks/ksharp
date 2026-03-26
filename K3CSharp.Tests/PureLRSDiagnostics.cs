using System;
using K3CSharp;
using K3CSharp.Parsing;

namespace K3CSharp.Tests
{
    public class PureLRSDiagnosticsRunner
    {
        public static void RunDiagnostics()
        {
            Console.WriteLine("=== Pure LRS Diagnostics ===");
            
            // Enable Pure LRS mode without debugging for production testing
            ParserConfig.EnablePureLRS();
            ParserConfig.EnableDebugging = false;
            ParserConfig.LogConfigChange("Pure LRS Production Testing - Debug disabled");
            
            // Test case 1: Simple vector (should create implicit vector)
            Console.WriteLine("\n--- Test 1: Simple vector '1 2 3 4 5' ---");
            TestExpression("1 2 3 4 5");
            
            // Test case 2: Simple assignment (should parse assignment)
            Console.WriteLine("\n--- Test 2: Simple assignment 'a:5' ---");
            TestExpression("a:5");
            
            // Test case 3: String assignment (should parse assignment)
            Console.WriteLine("\n--- Test 3: String assignment 's:\"hello\"' ---");
            TestExpression("s:\"hello\"");
            
            // Test case 4: Mixed types (should create list)
            Console.WriteLine("\n--- Test 4: Mixed types '1 2.5 \"hello\"' ---");
            TestExpression("1 2.5 \"hello\"");
            
            // Test case 5: Special values
            Console.WriteLine("\n--- Test 5: Special values '0I 0N -0I' ---");
            TestExpression("0I 0N -0I");
            
            // Test case 6: Symbol vector
            Console.WriteLine("\n--- Test 6: Symbol vector '`a`b`c' ---");
            TestExpression("`a`b`c");
            
            // Test case 7: Empty vector
            Console.WriteLine("\n--- Test 7: Empty vector '()' ---");
            TestExpression("()");
            
            // Test case 8: Simple arithmetic
            Console.WriteLine("\n--- Test 8: Simple arithmetic '1 + 2' ---");
            TestExpression("1 + 2");
            
            // Test case 9: Multi-line script
            Console.WriteLine("\n--- Test 9: Multi-line script ---");
            TestMultiLineScript("a:5\na+3");
            
            Console.WriteLine("\n=== Diagnostics Complete ===");
        }
        
        static void TestExpression(string expression)
        {
            try
            {
                var lexer = new Lexer(expression);
                var tokens = lexer.Tokenize();
                
                Console.WriteLine($"Tokens: {string.Join(", ", tokens.Select(t => $"{t.Type}({t.Lexeme})"))}");
                
                var parser = new LRSParser(tokens);
                parser.PureLRSMode = true;
                
                int position = 0;
                var result = parser.ParseExpression(ref position);
                
                Console.WriteLine($"Result: {(result == null ? "NULL" : result.Type.ToString())}");
                if (result != null)
                {
                    var evaluator = new Evaluator();
                    var value = evaluator.Evaluate(result);
                    Console.WriteLine($"Value: {value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
        }
        
        static void TestMultiLineScript(string script)
        {
            try
            {
                var lexer = new Lexer(script);
                var tokens = lexer.Tokenize();
                
                Console.WriteLine($"Multi-line tokens: {string.Join(", ", tokens.Select(t => $"{t.Type}({t.Lexeme})"))}");
                
                // Use LRSParserWrapper for multi-line scripts (Pure LRS mode)
                var wrapper = new LRSParserWrapper(tokens, script, enableFallback: false, useLRSParser: true);
                
                var result = wrapper.Parse();
                
                Console.WriteLine($"Multi-line result: {(result == null ? "NULL" : result.Type.ToString())}");
                if (result != null)
                {
                    var evaluator = new Evaluator();
                    var value = evaluator.Evaluate(result);
                    Console.WriteLine($"Multi-line value: {value}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Multi-line ERROR: {ex.Message}");
            }
        }
    }
}
