using System;
using System.IO;

class TestWrapper
{
    static void Main()
    {
        Console.WriteLine("Testing K Interpreter Wrapper");
        Console.WriteLine("==============================");
        
        try
        {
            var wrapper = new K3CSharp.KInterpreterWrapper();
            
            // Test with simple script
            var script = "2+3";
            Console.WriteLine($"Testing script: {script}");
            
            var result = wrapper.ExecuteScript(script);
            Console.WriteLine($"Result: '{result}'");
            
            // Test with first test script
            Console.WriteLine("\nTesting first test script:");
            var testScriptPath = "K3CSharp.Tests/TestScripts/adverb_each_vector_minus.k";
            var testScriptContent = File.ReadAllText(testScriptPath);
            Console.WriteLine($"Script content: {testScriptContent.Trim()}");
            
            var testResult = wrapper.ExecuteScript(testScriptContent);
            Console.WriteLine($"Test result: '{testResult}'");
            
            wrapper.CleanupTempDirectory();
            Console.WriteLine("\n✓ Wrapper test completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
    }
}
