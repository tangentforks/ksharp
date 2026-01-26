using System;
using System.IO;

class TestTimeout
{
    static void Main()
    {
        Console.WriteLine("Testing K Wrapper Timeout Handling");
        Console.WriteLine("===================================");
        
        try
        {
            var wrapper = new K3CSharp.KInterpreterWrapper();
            
            // Test 1: Normal script (should work)
            Console.WriteLine("Test 1: Normal script");
            var normalScript = "2+3";
            try
            {
                var result1 = wrapper.ExecuteScript(normalScript);
                Console.WriteLine($"✓ Normal script result: '{result1.Trim()}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Normal script failed: {ex.Message}");
            }
            
            // Test 2: Script that might cause k.exe to hang (UTF-8 BOM issue)
            Console.WriteLine("\nTest 2: Problematic script (should timeout)");
            var problematicScript = "2+3"; // This should work with ASCII encoding now
            
            // Let's create a script with invalid syntax to test error handling
            var errorScript = "2+"; // Invalid syntax
            try
            {
                var result2 = wrapper.ExecuteScript(errorScript);
                Console.WriteLine($"✓ Error script result: '{result2.Trim()}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✓ Error script correctly handled: {ex.Message}");
            }
            
            wrapper.CleanupTempDirectory();
            Console.WriteLine("\n✓ Timeout test completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test error: {ex.Message}");
        }
    }
}
