using System;
using K3CSharp;

namespace K3CSharp
{
    public class KWrapperDemo
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("K Interpreter Wrapper Demo");
            Console.WriteLine("===========================");
            
            var wrapper = new KInterpreterWrapper();
            
            // Demo scripts to test various K features
            var demos = new[]
            {
                ("Simple addition", "2 + 3"),
                ("Vector addition", "1 2 3 + 4 5 6"),
                ("Function definition", "f:{[x] x * 2}\nf 5"),
                ("Vector indexing", "v:10 20 30 40 50\nv[2]"),
                ("Multiple indexing", "v:10 20 30 40 50\nv[0 2 4]"),
                ("Square brackets (new syntax)", "div:{[x;y] x % y}\ndiv[10;2]"),
                ("Vector with square brackets", "v:100 200 300 400 500\nv[1 3]")
            };

            foreach (var (name, script) in demos)
            {
                Console.WriteLine($"\n--- {name} ---");
                Console.WriteLine($"Script: {script.Replace("\n", "; ")}");
                
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
            
            // Clean up
            wrapper.CleanupTempDirectory();
            Console.WriteLine("\nDemo completed. Temporary files cleaned up.");
        }
    }
}
