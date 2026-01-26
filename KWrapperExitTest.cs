using System;
using System.IO;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class KWrapperExitTest
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Testing K Wrapper Exit Command Change");
            Console.WriteLine("=====================================");
            
            var wrapper = new KInterpreterWrapper();
            
            // Test script content
            var testScript = "2+3";
            Console.WriteLine($"Original script: {testScript}");
            
            // Create a temporary script to see what gets written
            var tempDir = Path.Combine(Path.GetTempPath(), "ksharp_test");
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            
            var tempScriptPath = Path.Combine(tempDir, "test_script.k");
            
            // Use reflection to test the private method (for testing purposes)
            var method = typeof(KInterpreterWrapper).GetMethod("CreateTempScriptWithExit", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                var result = method.Invoke(wrapper, new object[] { testScript });
                var createdPath = result as string;
                
                if (File.Exists(createdPath))
                {
                    var content = File.ReadAllText(createdPath);
                    Console.WriteLine($"Modified script content:");
                    Console.WriteLine(content);
                    
                    // Verify it contains _exit 0
                    if (content.Contains("_exit 0"))
                    {
                        Console.WriteLine("✓ SUCCESS: Script contains '_exit 0'");
                    }
                    else
                    {
                        Console.WriteLine("✗ FAIL: Script does not contain '_exit 0'");
                    }
                    
                    // Verify it doesn't contain double backslash
                    if (content.Contains("\\\\"))
                    {
                        Console.WriteLine("✗ FAIL: Script still contains double backslash");
                    }
                    else
                    {
                        Console.WriteLine("✓ SUCCESS: Script does not contain double backslash");
                    }
                    
                    // Clean up
                    File.Delete(createdPath);
                }
                else
                {
                    Console.WriteLine("✗ FAIL: Temporary script file was not created");
                }
            }
            else
            {
                Console.WriteLine("✗ FAIL: Could not access CreateTempScriptWithExit method");
            }
            
            // Clean up test directory
            try
            {
                Directory.Delete(tempDir, true);
            }
            catch { }
            
            wrapper.CleanupTempDirectory();
            Console.WriteLine("\nTest completed.");
        }
    }
}
