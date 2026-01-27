using System;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class TestWrapperFileLocking
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing KInterpreterWrapper file locking improvements...");
            
            var wrapper = new KInterpreterWrapper();
            
            // Test simple cases that should work
            var testCases = new[]
            {
                "1 + 2",
                "10 20 30", 
                "count 1 2 3 4 5",
                "2 * 3"
            };
            
            int successCount = 0;
            int errorCount = 0;
            
            foreach (var testCase in testCases)
            {
                try
                {
                    Console.WriteLine($"Testing: {testCase}");
                    var result = wrapper.ExecuteScript(testCase);
                    Console.WriteLine($"Result: {result}");
                    successCount++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    errorCount++;
                }
                Console.WriteLine("---");
            }
            
            Console.WriteLine($"Summary: {successCount} succeeded, {errorCount} failed");
            wrapper.CleanupTempDirectory();
        }
    }
}
