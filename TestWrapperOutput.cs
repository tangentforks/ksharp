using System;
using K3CSharp;

class TestWrapperOutput
{
    static void Main()
    {
        Console.WriteLine("Testing improved wrapper output...");
        var wrapper = new KInterpreterWrapper();
        
        try 
        {
            var result = wrapper.ExecuteScript("1 + 2");
            Console.WriteLine($"Cleaned output: '{result}'");
            Console.WriteLine($"Output length: {result.Length}");
            Console.WriteLine($"First 10 chars: '{result.Substring(0, Math.Min(10, result.Length))}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
        finally
        {
            wrapper.CleanupTempDirectory();
        }
    }
}
