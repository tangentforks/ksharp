using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class KComparisonReporter
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("K3Sharp vs k.exe Full Comparison Report");
            Console.WriteLine("=========================================");
            
            var wrapper = new KInterpreterWrapper();
            var testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "K3CSharp.Tests", "TestScripts");
            var reportPath = "comparison_table.txt";
            
            try
            {
                var testFiles = Directory.GetFiles(testScriptsPath, "*.k").Select(Path.GetFileName).OrderBy(f => f).ToList();
                Console.WriteLine($"Found {testFiles.Count} test scripts");
                
                var results = new List<TestComparison>();
                int batchSize = 20; // Process in smaller batches
                int processed = 0;
                
                // Process in batches to avoid timeouts
                for (int i = 0; i < testFiles.Count; i += batchSize)
                {
                    var batch = testFiles.Skip(i).Take(batchSize).ToList();
                    Console.WriteLine($"\nProcessing batch {Math.Min(i + batchSize, testFiles.Count)}/{testFiles.Count}...");
                    
                    foreach (var fileName in batch)
                    {
                        processed++;
                        Console.Write($"{processed:D3}. {fileName,-40} ");
                        
                        var result = CompareTestFile(wrapper, fileName, testScriptsPath);
                        results.Add(result);
                        
                        var statusSymbol = result.Status switch
                        {
                            ComparisonStatus.Matched => "✅",
                            ComparisonStatus.Differed => "❌", 
                            ComparisonStatus.Skipped => "⚠️",
                            ComparisonStatus.Error => "💥",
                            _ => "❓"
                        };
                        Console.WriteLine(statusSymbol);
                    }
                    
                    // Save progress after each batch
                    Console.WriteLine("Saving progress...");
                    GenerateComparisonReport(results, reportPath, false);
                }
                
                // Generate final complete report
                Console.WriteLine("\nGenerating final report...");
                GenerateComparisonReport(results, reportPath, true);
                
                // Print summary
                var summary = results.GroupBy(r => r.Status).ToDictionary(g => g.Key, g => g.Count());
                Console.WriteLine($"\n🎉 COMPARISON COMPLETE!");
                Console.WriteLine($"================================");
                Console.WriteLine($"Total Tests:     {testFiles.Count}");
                Console.WriteLine($"✅ Matched:       {summary.GetValueOrDefault(ComparisonStatus.Matched, 0)}");
                Console.WriteLine($"❌ Differed:      {summary.GetValueOrDefault(ComparisonStatus.Differed, 0)}");
                Console.WriteLine($"⚠️  Skipped:       {summary.GetValueOrDefault(ComparisonStatus.Skipped, 0)}");
                Console.WriteLine($"💥 Errors:        {summary.GetValueOrDefault(ComparisonStatus.Error, 0)}");
                
                var successRate = summary.GetValueOrDefault(ComparisonStatus.Matched, 0) * 100.0 / (testFiles.Count - summary.GetValueOrDefault(ComparisonStatus.Skipped, 0));
                Console.WriteLine($"Success Rate:    {successRate:F1}%");
                Console.WriteLine($"Report saved to:  {reportPath}");
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
                
                // Check if this is a semicolon/newline formatting match
                if (comparison.Status == ComparisonStatus.Matched)
                {
                    var normalize = (string s) => 
                    {
                        if (string.IsNullOrEmpty(s)) return "";
                        s = System.Text.RegularExpressions.Regex.Replace(s, @"(\d+)\.0\b", "$1");
                        s = System.Text.RegularExpressions.Regex.Replace(s.Trim(), @"\s+", " ");
                        s = s.TrimEnd(';', ' ');
                        return s;
                    };
                    
                    var normalizedK3Sharp = normalize(comparison.K3SharpOutput);
                    var normalizedKExe = normalize(comparison.KOutput);
                    
                    if (normalizedK3Sharp != normalizedKExe)
                    {
                        comparison.Message = "Matched via semicolon/newline formatting equivalence";
                    }
                }
            }
            catch (Exception ex) { 
                comparison.Status = ComparisonStatus.Error; 
                comparison.Message = ex.Message; 
            }
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
                
                // Normalize float formatting (10.0 -> 10)
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(\d+)\.0\b", "$1");
                
                // Normalize whitespace and trim
                s = System.Text.RegularExpressions.Regex.Replace(s.Trim(), @"\s+", " ");
                
                // Remove trailing semicolons and spaces
                s = s.TrimEnd(';', ' ');
                
                return s;
            };
            
            var normalizedK3Sharp = normalize(k3Sharp);
            var normalizedKExe = normalize(kExe);
            
            // If they match exactly, return true
            if (normalizedK3Sharp == normalizedKExe)
            {
                return true;
            }
            
            // If they don't match, check if the difference is only semicolons vs newlines
            return AreSemicolonNewlineEquivalent(k3Sharp, kExe);
        }
        
        private static bool AreSemicolonNewlineEquivalent(string str1, string str2)
        {
            // Normalize both strings by replacing semicolons with newlines and vice versa
            var str1WithNewlines = str1.Replace(';', '\n');
            var str2WithNewlines = str2.Replace(';', '\n');
            
            var str1WithSemicolons = str1.Replace('\n', ';');
            var str2WithSemicolons = str2.Replace('\n', ';');
            
            // Normalize both versions for comparison
            var normalize = (string s) => 
            {
                if (string.IsNullOrEmpty(s)) return "";
                
                // Normalize float formatting (10.0 -> 10)
                s = System.Text.RegularExpressions.Regex.Replace(s, @"(\d+)\.0\b", "$1");
                
                // Normalize whitespace and trim
                s = System.Text.RegularExpressions.Regex.Replace(s.Trim(), @"\s+", " ");
                
                // Remove trailing semicolons and spaces
                s = s.TrimEnd(';', ' ');
                
                return s;
            };
            
            // Check if they match when we normalize semicolons to newlines
            var normalized1 = normalize(str1WithNewlines);
            var normalized2 = normalize(str2WithNewlines);
            if (normalized1 == normalized2)
            {
                return true;
            }
            
            // Check if they match when we normalize newlines to semicolons
            normalized1 = normalize(str1WithSemicolons);
            normalized2 = normalize(str2WithSemicolons);
            if (normalized1 == normalized2)
            {
                return true;
            }
            
            // Check character-by-character differences
            return AreDifferencesOnlySemicolonNewline(str1, str2);
        }
        
        private static bool AreDifferencesOnlySemicolonNewline(string str1, string str2)
        {
            var maxLen = Math.Max(str1.Length, str2.Length);
            
            for (int i = 0; i < maxLen; i++)
            {
                char c1 = i < str1.Length ? str1[i] : '\0';
                char c2 = i < str2.Length ? str2[i] : '\0';
                
                // If characters are the same, continue
                if (c1 == c2)
                {
                    continue;
                }
                
                // Check if the difference is semicolon vs newline (in either direction)
                bool isSemicolonNewlineDiff = 
                    (c1 == ';' && c2 == '\n') ||
                    (c1 == '\n' && c2 == ';') ||
                    (c1 == ';' && c2 == '\0') ||
                    (c1 == '\0' && c2 == ';') ||
                    (c1 == '\n' && c2 == '\0') ||
                    (c1 == '\0' && c2 == '\n');
                
                if (!isSemicolonNewlineDiff)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private static void GenerateComparisonReport(List<TestComparison> results, string reportPath, bool isFinal)
        {
            using (var writer = new StreamWriter(reportPath, append: !isFinal))
            {
                if (isFinal)
                {
                    writer.WriteLine("K3Sharp vs k.exe Comparison Report");
                    writer.WriteLine("=================================");
                    writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine();
                    
                    // Summary table
                    var summary = results.GroupBy(r => r.Status).ToDictionary(g => g.Key, g => g.Count());
                    writer.WriteLine("SUMMARY:");
                    writer.WriteLine("--------");
                    writer.WriteLine($"Total Tests:     {results.Count}");
                    writer.WriteLine($"✅ Matched:       {summary.GetValueOrDefault(ComparisonStatus.Matched, 0)}");
                    writer.WriteLine($"❌ Differed:      {summary.GetValueOrDefault(ComparisonStatus.Differed, 0)}");
                    writer.WriteLine($"⚠️  Skipped:       {summary.GetValueOrDefault(ComparisonStatus.Skipped, 0)}");
                    writer.WriteLine($"💥 Errors:        {summary.GetValueOrDefault(ComparisonStatus.Error, 0)}");
                    
                    var successRate = summary.GetValueOrDefault(ComparisonStatus.Matched, 0) * 100.0 / (results.Count - summary.GetValueOrDefault(ComparisonStatus.Skipped, 0));
                    writer.WriteLine($"Success Rate:    {successRate:F1}%");
                    writer.WriteLine();
                    
                    // Detailed results table
                    writer.WriteLine("DETAILED RESULTS:");
                    writer.WriteLine("-----------------");
                    writer.WriteLine("Status".PadRight(8) + " " + "Test File".PadRight(40) + " " + "K3Sharp Output".PadRight(25) + " " + "k.exe Output".PadRight(25) + " " + "Notes");
                    writer.WriteLine(new string('-', 110));
                    
                    foreach (var result in results.OrderBy(r => r.FileName))
                    {
                        var status = result.Status switch
                        {
                            ComparisonStatus.Matched => "✅ PASS",
                            ComparisonStatus.Differed => "❌ FAIL", 
                            ComparisonStatus.Skipped => "⚠️  SKIP",
                            ComparisonStatus.Error => "💥 ERROR",
                            _ => "❓ UNKNOWN"
                        };
                        
                        var k3SharpOutput = TruncateString(result.K3SharpOutput, 23);
                        var kOutput = TruncateString(result.KOutput, 23);
                        var notes = TruncateString(result.Message, 20);
                        
                        writer.WriteLine(status.PadRight(8) + " " + result.FileName.PadRight(40) + " " + 
                                       k3SharpOutput.PadRight(25) + " " + kOutput.PadRight(25) + " " + notes);
                    }
                    
                    writer.WriteLine();
                    writer.WriteLine("FAILED TESTS DETAILS:");
                    writer.WriteLine("--------------------");
                    
                    var failedTests = results.Where(r => r.Status == ComparisonStatus.Differed).ToList();
                    if (failedTests.Any())
                    {
                        foreach (var test in failedTests.Take(10)) // Limit to first 10 failed tests
                        {
                            writer.WriteLine($"File: {test.FileName}");
                            writer.WriteLine($"K3Sharp: {test.K3SharpOutput}");
                            writer.WriteLine($"k.exe:   {test.KOutput}");
                            writer.WriteLine();
                        }
                        if (failedTests.Count > 10)
                        {
                            writer.WriteLine($"... and {failedTests.Count - 10} more failed tests");
                        }
                    }
                    else
                    {
                        writer.WriteLine("No failed tests found!");
                    }
                    
                    writer.WriteLine();
                    writer.WriteLine("ERROR TESTS DETAILS:");
                    writer.WriteLine("-------------------");
                    
                    var errorTests = results.Where(r => r.Status == ComparisonStatus.Error).ToList();
                    if (errorTests.Any())
                    {
                        foreach (var test in errorTests.Take(10)) // Limit to first 10 error tests
                        {
                            writer.WriteLine($"File: {test.FileName}");
                            writer.WriteLine($"Error: {test.Message}");
                            writer.WriteLine();
                        }
                        if (errorTests.Count > 10)
                        {
                            writer.WriteLine($"... and {errorTests.Count - 10} more error tests");
                        }
                    }
                    else
                    {
                        writer.WriteLine("No error tests found!");
                    }
                }
                else
                {
                    // Progress update
                    writer.WriteLine($"Progress update: {results.Count} tests processed at {DateTime.Now:HH:mm:ss}");
                }
            }
        }
        
        private static string TruncateString(string s, int maxLength)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Length > maxLength ? s.Substring(0, maxLength - 3) + "..." : s;
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
