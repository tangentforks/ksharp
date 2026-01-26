using System;
using System.IO;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class KWrapperBasicTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("K Wrapper Basic Test");
            Console.WriteLine("=====================");
            
            var wrapper = new KInterpreterWrapper();
            
            // Test basic arithmetic that should work
            var basicTests = new[]
            {
                ("2+3", "5"),
                ("1+2+3", "6"),
                ("10-5", "5"),
                ("2*3", "6"),
                ("8/2", "4")
            };
            
            Console.WriteLine("Testing K3Sharp execution (without K.exe comparison):");
            Console.WriteLine();
            
            foreach (var (script, expected) in basicTests)
            {
                Console.WriteLine($"Script: {script}");
                Console.WriteLine($"Expected: {expected}");
                
                try
                {
                    var k3Result = ExecuteK3Sharp(script);
                    Console.WriteLine($"K3Sharp: {k3Result}");
                    Console.WriteLine($"Match: {(k3Result == expected ? "✓" : "✗")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                Console.WriteLine();
            }
            
            // Test K.exe wrapper (will likely fail if k.exe not installed)
            Console.WriteLine("Testing K.exe wrapper (may fail if k.exe not installed):");
            Console.WriteLine();
            
            foreach (var (script, expected) in basicTests)
            {
                Console.WriteLine($"Script: {script}");
                Console.WriteLine($"Expected: {expected}");
                
                try
                {
                    var kResult = wrapper.ExecuteScript(script);
                    Console.WriteLine($"K.exe: {kResult}");
                    Console.WriteLine($"Match: {(kResult == expected ? "✓" : "✗")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"K.exe Error: {ex.Message}");
                }
                
                Console.WriteLine();
            }
            
            wrapper.CleanupTempDirectory();
            Console.WriteLine("Test completed. Temporary files cleaned up.");
        }
        
        private static string ExecuteK3Sharp(string script)
        {
            try
            {
                var lexer = new Lexer(script);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens, script);
                var ast = parser.Parse();
                var evaluator = new Evaluator();
                var result = evaluator.Evaluate(ast);
                return result.ToString();
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
