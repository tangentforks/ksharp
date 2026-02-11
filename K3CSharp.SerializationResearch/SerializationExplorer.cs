using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K3CSharp;

namespace K3CSharp.SerializationResearch
{
    public class SerializationExplorer
    {
        private readonly KInterpreterWrapper wrapper;
        private readonly DataGenerator dataGenerator;
        private readonly string outputDirectory;

        public SerializationExplorer(string kExePath = @"c:\k\k.exe", string outputDirectory = "output")
        {
            wrapper = new KInterpreterWrapper(kExePath, 10000); // 10 second timeout
            dataGenerator = new DataGenerator();
            this.outputDirectory = outputDirectory;
            
            // Ensure output directory exists
            if (!Directory.Exists(this.outputDirectory))
            {
                Directory.CreateDirectory(this.outputDirectory);
            }
        }

        public string ExploreSerialization(DataType dataType, int count, bool edgeCasesOnly)
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var filename = $"serialization_{dataType}_{timestamp}.txt";
            var outputPath = Path.Combine(outputDirectory, filename);

            try
            {
                var examples = dataGenerator.GenerateExamples(dataType, count, edgeCasesOnly);
                var results = new List<SerializationResult>();

                Console.WriteLine($"Generating {examples.Count} examples for {dataType}...");

                foreach (var example in examples)
                {
                    var result = ProcessExample(example, dataType);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                }

                // Write results to file
                WriteResultsFile(outputPath, dataType, results, edgeCasesOnly);
                
                Console.WriteLine($"Generated {results.Count} successful results out of {examples.Count} examples");
                Console.WriteLine($"Results saved to: {outputPath}");

                return outputPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during serialization exploration: {ex.Message}");
                throw;
            }
            finally
            {
                wrapper.CleanupTempDirectory();
            }
        }

        public string TestHypothesis(string testData, DataType dataType = DataType.Integer)
        {
            if (string.IsNullOrEmpty(testData))
            {
                throw new ArgumentException("Test data cannot be null or empty");
            }

            var results = new List<SerializationResult>();
            
            // Process the test data as a single example
            var result = ProcessExample(testData, dataType);
            
            if (result != null)
            {
                results.Add(result);
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var outputPath = Path.Combine(outputDirectory, $"hypothesis_test_{timestamp}.txt");
            
            WriteResultsFile(outputPath, dataType, results, false);
            return outputPath;
        }

        private SerializationResult? ProcessExample(string example, DataType dataType)
        {
            try
            {
                // Create _bd command
                var bdCommand = $"_bd {example}";
                
                // Execute with k.exe
                var result = wrapper.ExecuteScript(bdCommand);
                
                // Clean up result (remove licensing info, etc.)
                var cleanedResult = CleanResult(result);
                
                // Validate that the result type matches expected type
                if (!ValidateResultType(example, dataType))
                {
                    Console.WriteLine($"Generated example '{example}' does not match expected type {dataType}");
                    return null;
                }
                
                return new SerializationResult
                {
                    Input = example,
                    BdCommand = bdCommand,
                    SerializedOutput = cleanedResult,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to process example '{example}': {ex.Message}");
                return null; // Skip failed examples as per specification
            }
        }

        private bool ValidateResultType(string example, DataType expectedType)
        {
            try
            {
                // Use 4: operator to check type of generated example
                var typeCommand = $"4: {example}";
                var typeResult = wrapper.ExecuteScript(typeCommand);
                var cleanedType = CleanResult(typeResult).Trim();
                
                // Map K type names to our DataType enum
                var actualType = cleanedType switch
                {
                    "integer" or "1" => DataType.Integer,
                    "float" or "2" => DataType.Float,
                    "character" or "3" => DataType.Character,
                    "symbol" or "4" => DataType.Symbol,
                    "dictionary" or "5" => DataType.Dictionary,
                    "null" or "6" => DataType.Null,
                    "function" or "7" => DataType.AnonymousFunction, // Handle both "function" and numeric type code 7
                    "integer vector" or "-1" => DataType.IntegerVector,
                    "float vector" or "-2" => DataType.FloatVector,
                    "character vector" or "-3" => DataType.CharacterVector,
                    "symbol vector" or "-4" => DataType.SymbolVector,
                    "list" or "0" => DataType.List,
                    _ => (DataType?)null
                };
                
                return actualType == expectedType;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Type validation failed for '{example}': {ex.Message}");
                return false;
            }
        }

        private string CleanResult(string result)
        {
            if (string.IsNullOrEmpty(result))
                return string.Empty;

            // Remove licensing information lines
            var lines = result.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var cleanedLines = lines.Where(line => 
                !line.Contains("WIN32") && 
                !line.Contains("CPU") && 
                !line.Contains("MB") &&
                !line.Contains("EVAL")).ToArray();

            return string.Join("\n", cleanedLines).Trim();
        }

        private void WriteResultsFile(string outputPath, DataType dataType, List<SerializationResult> results, bool edgeCasesOnly)
        {
            using var writer = new StreamWriter(outputPath);
            
            // Write header
            writer.WriteLine($"# Serialization Research: {dataType} ({(edgeCasesOnly ? "edge cases" : "random examples")})");
            writer.WriteLine($"# Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"# Total examples: {results.Count}");
            writer.WriteLine();

            // Write results
            foreach (var result in results)
            {
                writer.WriteLine($"{result.BdCommand} → {result.SerializedOutput}");
            }

            // Write summary statistics
            writer.WriteLine();
            writer.WriteLine($"# Summary:");
            writer.WriteLine($"# Successful examples: {results.Count}");
            writer.WriteLine($"# Data type: {dataType}");
            writer.WriteLine($"# Mode: {(edgeCasesOnly ? "Edge cases" : "Random generation")}");
        }

        public void Cleanup()
        {
            wrapper.CleanupTempDirectory();
        }
    }

    public class SerializationResult
    {
        public string Input { get; set; } = string.Empty;
        public string BdCommand { get; set; } = string.Empty;
        public string SerializedOutput { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
