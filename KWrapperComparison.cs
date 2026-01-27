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
            Console.WriteLine("K Wrapper Comparison - All Test Scripts");
            var wrapper = new KInterpreterWrapper();
            var testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "K3CSharp.Tests", "TestScripts");
            
            try
            {
                var testFiles = Directory.GetFiles(testScriptsPath, "*.k").Select(Path.GetFileName).OrderBy(f => f).ToList();
                Console.WriteLine($"Found {testFiles.Count} test scripts\n");
                
                int skipped = 0, matched = 0, differed = 0, errors = 0;
                
                foreach (var fileName in testFiles)
                {
                    var result = CompareTestFile(wrapper, fileName, testScriptsPath);
                    
                    switch (result.Status)
                    {
                        case ComparisonStatus.Skipped:
                            skipped++; Console.WriteLine($"⚠️  SKIP {fileName}: {result.Message}"); break;
                        case ComparisonStatus.Matched:
                            matched++; Console.WriteLine($"✅ PASS {fileName}"); break;
                        case ComparisonStatus.Differed:
                            differed++; Console.WriteLine($"❌ FAIL {fileName}: K3Sharp='{result.K3SharpOutput}' vs k.exe='{result.KOutput}'"); break;
                        case ComparisonStatus.Error:
                            errors++; Console.WriteLine($"💥 ERROR {fileName}: {result.Message}"); break;
                    }
                }
                
                Console.WriteLine($"\nSUMMARY: Total {testFiles.Count}, ✅ {matched}, ❌ {differed}, ⚠️ {skipped}, 💥 {errors}");
                Console.WriteLine($"Success rate: {(matched * 100.0 / (testFiles.Count - skipped)):F1}%");
            }
            catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
            finally { wrapper.CleanupTempDirectory(); }
        }
        
        private static TestComparison CompareTestFile(KInterpreterWrapper wrapper, string fileName, string testScriptsPath)
        {
            var comparison = new TestComparison { FileName = fileName };
            try
            {
                var scriptContent = File.ReadAllText(Path.Combine(testScriptsPath, fileName));
                
                if (System.Text.RegularExpressions.Regex.IsMatch(scriptContent, @"\b\d+[Ll]\b"))
                {
                    comparison.Status = ComparisonStatus.Skipped;
                    comparison.Message = "Contains long integers";
                    return comparison;
                }
                
                comparison.K3SharpOutput = ExecuteK3Sharp(scriptContent);
                comparison.KOutput = wrapper.ExecuteScript(scriptContent);
                
                if (comparison.KOutput.StartsWith("UNSUPPORTED:"))
                {
                    comparison.Status = ComparisonStatus.Skipped;
                    comparison.Message = comparison.KOutput;
                    return comparison;
                }
                
                comparison.Status = AreResultsEquivalent(comparison.K3SharpOutput, comparison.KOutput) 
                    ? ComparisonStatus.Matched : ComparisonStatus.Differed;
            }
            catch (Exception ex) { comparison.Status = ComparisonStatus.Error; comparison.Message = ex.Message; }
            return comparison;
        }
        
        private static string ExecuteK3Sharp(string scriptContent)
        {
            var lexer = new Lexer(scriptContent);
            var parser = new Parser(lexer.Tokenize(), scriptContent);
            return new Evaluator().Evaluate(parser.Parse()).ToString();
        }
        
        private static bool AreResultsEquivalent(string k3Sharp, string kExe)
        {
            var normalize = (string s) => 
            {
                if (string.IsNullOrEmpty(s)) return "";
                
                // Remove k.exe copyright header
                s = System.Text.RegularExpressions.Regex.Replace(s, @"K\s+\d+\.\d+\s+\d{4}-\d{2}-\d{2}\s+Copyright\s+\(C\)\s+\d{4}-\d{4}\s+Kx\s+Systems\s*", "");
                
                // Normalize float formatting (10.0 -> 10)
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(\d+)\.0\b", "$1");
                
                // Normalize whitespace and trim
                s = System.Text.RegularExpressions.Regex.Replace(s.Trim(), @"\s+", " ");
                
                // Remove trailing semicolons and spaces
                s = s.TrimEnd(';', ' ');
                
                return s;
            };
            
            return normalize(k3Sharp) == normalize(kExe);
        }
    }
    
    public class TestComparison
    {
        public string FileName { get; set; } = "";
        public string K3SharpOutput { get; set; } = "";
        public string KOutput { get; set; } = "";
        public ComparisonStatus Status { get; set; }
        public string Message { get; set; } = "";
    }
    
    public enum ComparisonStatus { Matched, Differed, Skipped, Error }
}
