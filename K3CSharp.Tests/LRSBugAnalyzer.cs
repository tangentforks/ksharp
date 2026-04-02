using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K3CSharp;
using K3CSharp.Parsing;

namespace K3CSharp.Tests
{
    /// <summary>
    /// Analyzes LRS parser bugs by comparing LRS vs Legacy parser results
    /// </summary>
    public class LRSBugAnalyzer
    {
        public static void AnalyzeLRSBugs()
        {
            Console.WriteLine("=== LRS Bug Analysis ===\n");
            
            var testDir = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");
            var testFiles = Directory.GetFiles(testDir, "*.k").OrderBy(f => f).ToList();
            
            var bugCategories = new Dictionary<string, List<string>>
            {
                ["OperatorPrecedence"] = new List<string>(),
                ["FunctionCalls"] = new List<string>(),
                ["VectorOperations"] = new List<string>(),
                ["DictionaryOperations"] = new List<string>(),
                ["AssignmentIssues"] = new List<string>(),
                ["ParsingErrors"] = new List<string>(),
                ["EvaluationErrors"] = new List<string>(),
                ["Other"] = new List<string>()
            };
            
            int analyzed = 0;
            int maxToAnalyze = 50; // Analyze first 50 tests
            
            foreach (var testFile in testFiles)
            {
                if (analyzed >= maxToAnalyze) break;
                
                var fileName = Path.GetFileName(testFile);
                var script = File.ReadAllText(testFile);
                
                try
                {
                    // Test with LRS parser (Pure mode)
                    ParserConfig.UseLRSParser = true;
                    ParserConfig.EnableFallback = false;
                    ParserConfig.EnableDebugging = false;
                    
                    var lexerLRS = new Lexer(script);
                    var tokensLRS = lexerLRS.Tokenize();
                    var astLRS = ParserConfig.ParseWithConfig(tokensLRS, script);
                    var evaluatorLRS = new Evaluator();
                    K3Value? resultLRS = null;
                    string? errorLRS = null;
                    
                    try
                    {
                        resultLRS = evaluatorLRS.Evaluate(astLRS);
                    }
                    catch (Exception ex)
                    {
                        errorLRS = ex.Message;
                    }
                    
                    // Test with Legacy parser
                    ParserConfig.UseLRSParser = false;
                    
                    var lexerLegacy = new Lexer(script);
                    var tokensLegacy = lexerLegacy.Tokenize();
                    var parserLegacy = new Parser(tokensLegacy, script);
                    var astLegacy = parserLegacy.Parse();
                    var evaluatorLegacy = new Evaluator();
                    K3Value? resultLegacy = null;
                    string? errorLegacy = null;
                    
                    try
                    {
                        resultLegacy = evaluatorLegacy.Evaluate(astLegacy);
                    }
                    catch (Exception ex)
                    {
                        errorLegacy = ex.Message;
                    }
                    
                    // Compare results
                    var lrsOutput = errorLRS ?? resultLRS?.ToString() ?? "NULL";
                    var legacyOutput = errorLegacy ?? resultLegacy?.ToString() ?? "NULL";
                    
                    if (lrsOutput != legacyOutput)
                    {
                        analyzed++;
                        
                        // Categorize the bug
                        var category = CategorizeBug(script, lrsOutput, legacyOutput, errorLRS, errorLegacy);
                        bugCategories[category].Add(fileName);
                        
                        Console.WriteLine($"\n[{analyzed}] {fileName}");
                        Console.WriteLine($"Script: {script.Substring(0, Math.Min(60, script.Length))}...");
                        Console.WriteLine($"LRS:    {lrsOutput.Substring(0, Math.Min(60, lrsOutput.Length))}");
                        Console.WriteLine($"Legacy: {legacyOutput.Substring(0, Math.Min(60, legacyOutput.Length))}");
                        Console.WriteLine($"Category: {category}");
                    }
                }
                catch (Exception)
                {
                    // Skip tests that fail to parse
                }
            }
            
            // Print summary
            Console.WriteLine("\n\n=== Bug Category Summary ===");
            foreach (var category in bugCategories.OrderByDescending(kv => kv.Value.Count))
            {
                if (category.Value.Count > 0)
                {
                    Console.WriteLine($"\n{category.Key}: {category.Value.Count} tests");
                    foreach (var test in category.Value.Take(5))
                    {
                        Console.WriteLine($"  - {test}");
                    }
                    if (category.Value.Count > 5)
                    {
                        Console.WriteLine($"  ... and {category.Value.Count - 5} more");
                    }
                }
            }
            
            // Restore default config
            ParserConfig.UseLRSParser = true;
            ParserConfig.EnableFallback = false;
        }
        
        private static string CategorizeBug(string script, string lrsOutput, string legacyOutput, 
            string? errorLRS, string? errorLegacy)
        {
            // Check for parsing errors
            if (errorLRS != null && errorLegacy == null)
                return "ParsingErrors";
            
            // Check for evaluation errors
            if (errorLRS != null && errorLegacy != null)
                return "EvaluationErrors";
            
            // Check for operator precedence issues (contains operators)
            if (script.Contains("+") || script.Contains("-") || script.Contains("*") || script.Contains("%"))
            {
                if (script.Contains(" - ") || script.Contains("+ ") || script.Contains("* "))
                    return "OperatorPrecedence";
            }
            
            // Check for function calls
            if (script.Contains("[") && script.Contains("]"))
                return "FunctionCalls";
            
            // Check for vector operations
            if (script.Contains("(") && script.Contains(";"))
                return "VectorOperations";
            
            // Check for dictionary operations
            if (script.Contains(".") && script.Contains("("))
                return "DictionaryOperations";
            
            // Check for assignment issues
            if (script.Contains(":") && !script.Contains("::"))
                return "AssignmentIssues";
            
            return "Other";
        }
    }
}
