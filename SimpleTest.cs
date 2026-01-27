using System;
using K3CSharp;

class SimpleTest
{
    static void Main()
    {
        Console.WriteLine("Testing wrapper...");
        var wrapper = new KInterpreterWrapper();
        
        var testCases = new[]
        {
            "1 + 2",
            "10 20 30",
            "count 1 2 3 4 5",
            "2 * 3"
        };
        
        foreach (var testCase in testCases)
        {
            try 
            {
                Console.WriteLine($"Testing: {testCase}");
                var result = wrapper.ExecuteScript(testCase);
                Console.WriteLine($"SUCCESS: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            Console.WriteLine("---");
        }
        
        wrapper.CleanupTempDirectory();
    }
}
