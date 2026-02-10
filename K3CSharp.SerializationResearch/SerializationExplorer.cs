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

        public string TestHypothesis(string testData)
        {
            if (string.IsNullOrEmpty(testData))
            {
                throw new ArgumentException("Test data cannot be null or empty");
            }

            var results = new List<SerializationResult>();
            
            // Process the test data as a single example
            var result = ProcessExample(testData, DataType.Integer); // Default to Integer type for hypothesis testing
            
            if (result != null)
            {
                results.Add(result);
            }

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var outputPath = Path.Combine(outputDirectory, $"hypothesis_test_{timestamp}.txt");
            
            WriteResultsFile(outputPath, DataType.Integer, results, false);
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
