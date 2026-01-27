using System;
using K3CSharp;

class TestLongIntegerDetection
{
    static void Main()
    {
        Console.WriteLine("Testing Long Integer Detection");
        Console.WriteLine("============================");
        
        var wrapper = new KInterpreterWrapper();
        
        // Test scripts with and without long integers
        var testCases = new[]
        {
            ("2+3", "Should work (no long integers)"),
            ("123L + 456", "Should be unsupported (long integer)"),
            ("10 20L 30", "Should be unsupported (long integer in vector)"),
            ("(1 2 3) + (4 5 6)", "Should work (no long integers)"),
            ("x: 42L", "Should be unsupported (assignment with long integer)")
        };
        
        foreach (var (script, description) in testCases)
        {
            Console.WriteLine($"\nTest: {description}");
            Console.WriteLine($"Script: {script}");
            
            try
            {
                var result = wrapper.ExecuteScript(script);
                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        wrapper.CleanupTempDirectory();
        Console.WriteLine("\n✓ Long integer detection test completed!");
    }
}
