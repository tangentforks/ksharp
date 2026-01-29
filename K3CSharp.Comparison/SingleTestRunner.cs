using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace K3CSharp.Comparison
{
    /// <summary>
    /// Simple interpreter wrapper for running single test comparisons
    /// </summary>
    public class SingleTestRunner
    {
        private readonly string k3SharpPath;
        private readonly string kExePath;
        private readonly string testScriptsPath;

        public SingleTestRunner()
        {
            // Get paths relative to this project
            var currentDir = Directory.GetCurrentDirectory();
            var solutionRoot = Path.GetFullPath(Path.Combine(currentDir, ".."));
            
            k3SharpPath = Path.Combine(solutionRoot, "bin", "Debug", "net6.0", "K3CSharp.dll");
            kExePath = "k.exe"; // Assume k.exe is in PATH
            testScriptsPath = Path.Combine(solutionRoot, "K3CSharp.Tests", "TestScripts");
        }

        public void RunSingleTest(string testFileName)
        {
            var testFilePath = Path.Combine(testScriptsPath, testFileName);
            
            if (!File.Exists(testFilePath))
            {
                Console.WriteLine($"❌ Test file not found: {testFilePath}");
                return;
            }

            Console.WriteLine($"🔍 Running comparison for: {testFileName}");
            Console.WriteLine("=" + new string('=', 50));

            try
            {
                // Run K3Sharp
                var k3SharpResult = RunK3Sharp(testFilePath);
                Console.WriteLine($"K3Sharp: {k3SharpResult}");

                // Try to run k.exe (may not be available)
                try
                {
                    var kResult = RunKExe(testFilePath);
                    Console.WriteLine($"k.exe:   {kResult}");

                    // Compare results
                    var match = k3SharpResult.Trim() == kResult.Trim();
                    Console.WriteLine(match ? "✅ PASS" : "❌ FAIL");
                    
                    if (!match)
                    {
                        Console.WriteLine($"📊 Difference detected:");
                        Console.WriteLine($"   K3Sharp: '{k3SharpResult}'");
                        Console.WriteLine($"   k.exe:   '{kResult}'");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"k.exe:   ⚠️  Not available ({ex.Message})");
                    Console.WriteLine($"ℹ️  K3Sharp result: {k3SharpResult}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERROR: {ex.Message}");
            }
            
            Console.WriteLine();
        }

        private string RunK3Sharp(string testFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"\"{k3SharpPath}\" \"{testFilePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return $"ERROR: {error.Trim()}";
            }

            return output.Trim();
        }

        private string RunKExe(string testFilePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = kExePath,
                Arguments = $"\"{testFilePath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                return $"ERROR: {error.Trim()}";
            }

            return output.Trim();
        }

        public static void Main(string[] args)
        {
            var runner = new SingleTestRunner();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: SingleTestRunner <testFileName>");
                Console.WriteLine("Example: SingleTestRunner test_form_braces_arith.k");
                Console.WriteLine();
                Console.WriteLine("Available tests:");
                
                var testDir = Path.Combine(Directory.GetCurrentDirectory(), "..", "K3CSharp.Tests", "TestScripts");
                if (Directory.Exists(testDir))
                {
                    var tests = Directory.GetFiles(testDir, "*.k")
                        .Select(Path.GetFileName)
                        .OrderBy(f => f)
                        .Take(20); // Show first 20 as examples
                    
                    foreach (var test in tests)
                    {
                        Console.WriteLine($"  {test}");
                    }
                    Console.WriteLine($"  ... and {Directory.GetFiles(testDir, "*.k").Length - 20} more");
                }
                return;
            }

            var testFileName = args[0];
            if (!testFileName.EndsWith(".k"))
            {
                testFileName += ".k";
            }

            runner.RunSingleTest(testFileName);
        }
    }
}
