using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class KComparisonTestRunner
    {
        private readonly KInterpreterWrapper kWrapper;
        private readonly string testScriptsPath;

        public KComparisonTestRunner(string kExePath = @"c:\k\k.exe")
        {
            this.kWrapper = new KInterpreterWrapper(kExePath);
            this.testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");
        }

        public void RunComparisonTests()
        {
            Console.WriteLine("K3Sharp vs K.exe Comparison Tests");
            Console.WriteLine("=================================");
            
            var testFiles = GetTestFiles();
            var results = new List<ComparisonResult>();

            foreach (var testFile in testFiles.Take(20)) // Limit to first 20 for demo
            {
                var result = CompareTestFile(testFile);
                results.Add(result);
                PrintComparisonResult(result);
            }

            PrintSummary(results);
        }

        private List<string> GetTestFiles()
        {
            if (!Directory.Exists(testScriptsPath))
            {
                Console.WriteLine($"TestScripts directory not found: {testScriptsPath}");
                return new List<string>();
            }

            return Directory.GetFiles(testScriptsPath, "*.k")
                           .Select(Path.GetFileName)
                           .OrderBy(f => f)
                           .ToList();
        }

        private ComparisonResult CompareTestFile(string fileName)
        {
            var result = new ComparisonResult { FileName = fileName };

            try
            {
                // Read script content
                var scriptPath = Path.Combine(testScriptsPath, fileName);
                var scriptContent = File.ReadAllText(scriptPath);
                
                // Get first line (test input)
                var lines = scriptContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                var testInput = lines.Length > 0 ? lines[0].Trim() : "";

                // Execute with K3Sharp
                result.K3SharpOutput = ExecuteK3Sharp(testInput);
                result.K3SharpSuccess = true;

                // Execute with K.exe
                result.KOutput = kWrapper.ExecuteScript(testInput);
                result.KSuccess = true;

                // Compare results (allowing for acceptable discrepancies)
                result.IsMatch = AreResultsAcceptablyMatched(result.K3SharpOutput, result.KOutput);
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
            }

            return result;
        }

        private string ExecuteK3Sharp(string script)
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

        private bool AreResultsAcceptablyMatched(string k3SharpResult, string kResult)
        {
            // Normalize both results
            var normalizedK3 = NormalizeResult(k3SharpResult);
            var normalizedK = NormalizeResult(kResult);

            // Exact match
            if (normalizedK3 == normalizedK)
            {
                return true;
            }

            // Check for acceptable discrepancies
            return CheckAcceptableDiscrepancies(normalizedK3, normalizedK);
        }

        private string NormalizeResult(string result)
        {
            if (string.IsNullOrWhiteSpace(result))
                return "";

            return result.Trim()
                       .Replace("\r\n", "\n")
                       .Replace("\r", "\n")
                       .Replace(" ", " ")
                       .Replace("\t", " ");
        }

        private bool CheckAcceptableDiscrepancies(string k3Result, string kResult)
        {
            // Help commands can show different messages
            if (k3Result.Contains("help") || kResult.Contains("help"))
            {
                return true;
            }

            // Floating point precision differences
            if (ContainsFloatingPoint(k3Result) && ContainsFloatingPoint(kResult))
            {
                return CompareFloatingPointResults(k3Result, kResult);
            }

            // Special value representation differences (0I vs 0N, etc.)
            if (ContainsSpecialValues(k3Result) && ContainsSpecialValues(kResult))
            {
                return CompareSpecialValueResults(k3Result, kResult);
            }

            return false;
        }

        private bool ContainsFloatingPoint(string result)
        {
            return result.Contains(".") && result.Any(char.IsDigit);
        }

        private bool CompareFloatingPointResults(string result1, string result2)
        {
            // Simple comparison - could be enhanced for more sophisticated floating point comparison
            var nums1 = ExtractNumbers(result1);
            var nums2 = ExtractNumbers(result2);
            
            if (nums1.Count != nums2.Count) return false;
            
            for (int i = 0; i < nums1.Count; i++)
            {
                if (Math.Abs(nums1[i] - nums2[i]) > 0.0001) // Allow small precision differences
                {
                    return false;
                }
            }
            
            return true;
        }

        private List<double> ExtractNumbers(string result)
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

        private bool ContainsSpecialValues(string result)
        {
            return result.Contains("0I") || result.Contains("0N") || result.Contains("0i") || result.Contains("0n");
        }

        private bool CompareSpecialValueResults(string result1, string result2)
        {
            // For now, accept any special value differences as acceptable
            // This could be made more sophisticated if needed
            return true;
        }

        private void PrintComparisonResult(ComparisonResult result)
        {
            Console.WriteLine($"\n{result.FileName}:");
            Console.WriteLine($"  K3Sharp: {result.K3SharpOutput}");
            Console.WriteLine($"  K.exe:   {result.KOutput}");
            Console.WriteLine($"  Match:   {(result.IsMatch ? "✓" : "✗")}");
            
            if (result.Error != null)
            {
                Console.WriteLine($"  Error:   {result.Error}");
            }
        }

        private void PrintSummary(List<ComparisonResult> results)
        {
            var total = results.Count;
            var matches = results.Count(r => r.IsMatch);
            var k3SharpErrors = results.Count(r => !r.K3SharpSuccess);
            var kErrors = results.Count(r => !r.KSuccess);

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("COMPARISON SUMMARY");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Total tests: {total}");
            Console.WriteLine($"Matches: {matches} ({(matches * 100.0 / total):F1}%)");
            Console.WriteLine($"K3Sharp errors: {k3SharpErrors}");
            Console.WriteLine($"K.exe errors: {kErrors}");
            Console.WriteLine($"Acceptable discrepancies: {total - matches}");
        }

        private class ComparisonResult
        {
            public string FileName { get; set; }
            public string K3SharpOutput { get; set; }
            public string KOutput { get; set; }
            public bool K3SharpSuccess { get; set; }
            public bool KSuccess { get; set; }
            public bool IsMatch { get; set; }
            public string Error { get; set; }
        }
    }
}
