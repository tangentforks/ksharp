using System;
using System.IO;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class KWrapperTest
    {
        public static void TestMain(string[] args)
        {
            var wrapper = new KInterpreterWrapper();
            
            try
            {
                Console.WriteLine("Testing K Interpreter Wrapper");
                Console.WriteLine("=============================");
                
                // Test 1: Simple arithmetic
                TestScript(wrapper, "2+3", "5");
                
                // Test 2: Vector operations
                TestScript(wrapper, "1 2 3 + 4 5 6", "5 7 9");
                
                // Test 3: Function definition and call
                TestScript(wrapper, "f:{[x] x*2}\nf 5", "10");
                
                // Test 4: Vector indexing
                TestScript(wrapper, "v:10 20 30 40 50\nv[2]", "30");
                
                // Test 5: Multiple vector indexing
                TestScript(wrapper, "v:10 20 30 40 50\nv[0 2 4]", "10 30 50");
                
                Console.WriteLine("\nAll wrapper tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                wrapper.CleanupTempDirectory();
            }
        }
        
        private static void TestScript(KInterpreterWrapper wrapper, string script, string expectedContains)
        {
            Console.WriteLine($"\nTesting script: {script.Replace("\n", "; ")}");
            
            try
            {
                var result = wrapper.ExecuteScript(script);
                Console.WriteLine($"Result: {result}");
                
                if (result.Contains(expectedContains))
                {
                    Console.WriteLine("✓ Test passed");
                }
                else
                {
                    Console.WriteLine($"✗ Test failed - expected to contain: {expectedContains}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Test failed with exception: {ex.Message}");
            }
        }
    }
}
