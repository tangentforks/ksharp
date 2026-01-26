using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class KWrapperComparison
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("K Wrapper Comparison - First 10 Test Scripts");
            Console.WriteLine("==============================================");
            
            var wrapper = new KInterpreterWrapper();
            var testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "K3CSharp.Tests", "TestScripts");
            
            try
            {
                // Get the first 10 test files
                var testFiles = GetFirstTestFiles(testScriptsPath, 10);
                
                if (testFiles.Count == 0)
                {
                    Console.WriteLine("No test files found in TestScripts directory");
                    return;
                }
                
                Console.WriteLine($"Found {testFiles.Count} test files to compare\n");
                
                var results = new List<TestComparison>();
                
                foreach (var testFile in testFiles)
                {
                    var result = CompareTestFile(wrapper, testFile, testScriptsPath);
                    results.Add(result);
                    PrintComparisonResult(result);
                }
                
                PrintSummary(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                wrapper.CleanupTempDirectory();
            }
        }
        
        private static List<string> GetFirstTestFiles(string testScriptsPath, int count)
        {
            if (!Directory.Exists(testScriptsPath))
            {
                Console.WriteLine($"TestScripts directory not found: {testScriptsPath}");
                return new List<string>();
            }

            return Directory.GetFiles(testScriptsPath, "*.k")
                           .Select(Path.GetFileName)
                           .OrderBy(f => f)
                           .Take(count)
                           .ToList();
        }
        
        private static TestComparison CompareTestFile(KInterpreterWrapper wrapper, string fileName, string testScriptsPath)
        {
            var comparison = new TestComparison { FileName = fileName };
            
            try
            {
                // Read the test file
                var scriptPath = Path.Combine(testScriptsPath, fileName);
                var scriptContent = File.ReadAllText(scriptPath);
                var lines = scriptContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var testInput = lines.Length > 0 ? lines[0].Trim() : "";
                
                comparison.TestInput = testInput;
                
                // Get expected result from our test runner
                comparison.ExpectedResult = GetExpectedResult(fileName);
                
                // Execute with K3Sharp
                comparison.K3SharpResult = ExecuteK3Sharp(testInput);
                
                // Execute with K wrapper
                try
                {
                    comparison.KResult = wrapper.ExecuteScript(testInput);
                    comparison.KSuccess = true;
                }
                catch (Exception ex)
                {
                    comparison.KResult = $"Error: {ex.Message}";
                    comparison.KSuccess = false;
                }
                
                // Compare results
                comparison.K3SharpMatchesExpected = comparison.K3SharpResult == comparison.ExpectedResult;
                comparison.KMatchesExpected = comparison.KResult == comparison.ExpectedResult;
                comparison.KMatchesK3Sharp = AreResultsAcceptablyMatched(comparison.K3SharpResult, comparison.KResult);
            }
            catch (Exception ex)
            {
                comparison.Error = ex.Message;
            }
            
            return comparison;
        }
        
        private static string GetExpectedResult(string fileName)
        {
            // Map test files to their expected results from SimpleTestRunner
            // Updated with actual k.exe results from manual testing
            var expectedResults = new Dictionary<string, string>
            {
                { "simple_addition.k", "3" },
                { "simple_subtraction.k", "2" },
                { "simple_multiplication.k", "12" },
                { "simple_division.k", "4" },
                { "vector_addition.k", "4 6" },
                { "vector_subtraction.k", "-2 -2" },
                { "vector_multiplication.k", "3 8" },
                { "vector_division.k", "0.3333333 0.5" },
                { "test_vector.k", "1 2 3" },
                { "scalar_vector_addition.k", "4 5" },
                // Adverb tests - UPDATED with actual k.exe results
                { "adverb_over_plus.k", "15" },           // ✅ Matches expected
                { "adverb_over_multiply.k", "24" },       // ✅ Matches expected  
                { "adverb_over_minus.k", "4" },           // ✅ Matches expected
                { "adverb_over_divide.k", "10" },          // ✅ Matches expected
                { "adverb_over_min.k", "1" },              // ✅ Matches expected
                { "adverb_over_max.k", "5" },              // ✅ Matches expected
                { "adverb_over_power.k", "64" },           // ✅ Matches expected
                { "adverb_each_vector_plus.k", "6 8 10 12" },      // ❌ Different: k.exe returns element-wise result
                { "adverb_each_vector_multiply.k", "10 20 30" },   // ❌ Different: k.exe returns element-wise result  
                { "adverb_each_vector_minus.k", "9 18 27" }         // ❌ Different: k.exe returns element-wise result
            };
            
            return expectedResults.GetValueOrDefault(fileName, "UNKNOWN");
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
        
        private static bool AreResultsAcceptablyMatched(string result1, string result2)
        {
            if (result1 == result2) return true;
            
            // Handle acceptable discrepancies
            if (result1.Contains("Error") && result2.Contains("Error")) return true;
            if (result1.Contains("help") && result2.Contains("help")) return true;
            
            // Floating point precision
            if (ContainsFloatingPoint(result1) && ContainsFloatingPoint(result2))
            {
                return CompareFloatingPointResults(result1, result2);
            }
            
            return false;
        }
        
        private static bool ContainsFloatingPoint(string result)
        {
            return result.Contains(".") && result.Any(char.IsDigit);
        }
        
        private static bool CompareFloatingPointResults(string result1, string result2)
        {
            var nums1 = ExtractNumbers(result1);
            var nums2 = ExtractNumbers(result2);
            
            if (nums1.Count != nums2.Count) return false;
            
            for (int i = 0; i < nums1.Count; i++)
            {
                if (Math.Abs(nums1[i] - nums2[i]) > 0.0001)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private static List<double> ExtractNumbers(string result)
        {
            var numbers = new List<double>();
            var parts = result.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var part in parts)
            {
                if (double.TryParse(part, out double num))
                {
                    numbers.Add(num);
                }
            }
            
            return numbers;
        }
        
        private static void PrintComparisonResult(TestComparison comparison)
        {
            Console.WriteLine($"\n{comparison.FileName}:");
            Console.WriteLine($"  Input:    {comparison.TestInput}");
            Console.WriteLine($"  Expected: {comparison.ExpectedResult}");
            Console.WriteLine($"  K3Sharp:  {comparison.K3SharpResult} {(comparison.K3SharpMatchesExpected ? "✓" : "✗")}");
            Console.WriteLine($"  K.exe:    {comparison.KResult} {(comparison.KMatchesExpected ? "✓" : "✗")}");
            Console.WriteLine($"  Match:    {(comparison.KMatchesK3Sharp ? "✓" : "✗")}");
            
            if (comparison.Error != null)
            {
                Console.WriteLine($"  Error:    {comparison.Error}");
            }
        }
        
        private static void PrintSummary(List<TestComparison> results)
        {
            var total = results.Count;
            var k3SharpMatches = results.Count(r => r.K3SharpMatchesExpected);
            var kMatches = results.Count(r => r.KMatchesExpected);
            var kMatchesK3Sharp = results.Count(r => r.KMatchesK3Sharp);
            var kErrors = results.Count(r => !r.KSuccess);
            
            Console.WriteLine("\n" + new string('=', 60));
            Console.WriteLine("COMPARISON SUMMARY");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"Total tests compared: {total}");
            Console.WriteLine($"K3Sharp matches expected: {k3SharpMatches}/{total} ({(k3SharpMatches * 100.0 / total):F1}%)");
            Console.WriteLine($"K.exe matches expected: {kMatches}/{total} ({(kMatches * 100.0 / total):F1}%)");
            Console.WriteLine($"K.exe matches K3Sharp: {kMatchesK3Sharp}/{total} ({(kMatchesK3Sharp * 100.0 / total):F1}%)");
            Console.WriteLine($"K.exe errors: {kErrors}");
            
            Console.WriteLine("\nDetailed Analysis:");
            foreach (var result in results)
            {
                var status = result.KSuccess ? 
                    (result.KMatchesExpected ? "MATCH" : "DIFF") : 
                    "ERROR";
                Console.WriteLine($"  {result.FileName,-25} {status}");
            }
        }
        
        private class TestComparison
        {
            public string FileName { get; set; } = "";
            public string TestInput { get; set; } = "";
            public string ExpectedResult { get; set; } = "";
            public string K3SharpResult { get; set; } = "";
            public string KResult { get; set; } = "";
            public bool K3SharpMatchesExpected { get; set; }
            public bool KMatchesExpected { get; set; }
            public bool KMatchesK3Sharp { get; set; }
            public bool KSuccess { get; set; }
            public string Error { get; set; } = "";
        }
    }
}
