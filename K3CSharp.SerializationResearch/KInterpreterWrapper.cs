using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace K3CSharp
{
    public class KInterpreterWrapper
    {
        private readonly string kExePath;
        private readonly string tempDirectory;
        private readonly int timeoutMs;
        private readonly string runId;

        public KInterpreterWrapper(string kExePath = @"c:\k\e.exe", int timeoutMs = 10000)
        {
            if (!File.Exists(kExePath))
            {
                kExePath = @"c:\k\k.exe";
            }
            this.kExePath = kExePath;
            this.timeoutMs = timeoutMs;
            this.runId = $"{DateTime.Now:yyyyMMdd_HHmmss}_{Environment.ProcessId}_{Guid.NewGuid():N}";
            this.tempDirectory = Path.Combine(Path.GetTempPath(), "ksharp_wrapper", runId);
            
            // Ensure temp directory exists
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
        }

        public string ExecuteScript(string scriptContent)
        {
            // Check for unsupported long integers (32-bit k.exe limitation)
            if (this.kExePath.Contains("k.exe") && KInterpreterWrapper.ContainsLongInteger(scriptContent))
            {
                return "UNSUPPORTED: Script contains long integers (64-bit) - k.exe 32-bit does not support them";
            }
            
            string? tempScriptPath = null;
            string? outputPath = null;
            
            try
            {
                tempScriptPath = CreateTempScriptWithExit(scriptContent);
                outputPath = Path.Combine(tempDirectory, $"k_output_{Guid.NewGuid():N}_{DateTime.Now.Ticks}.txt");
                
                var processInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c \"\"{kExePath}\" \"{tempScriptPath}\" > \"{outputPath}\"\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                
                if (process != null)
                {
                    bool exited = process.WaitForExit(timeoutMs);
                    
                    if (!exited)
                    {
                        process.Kill();
                        throw new TimeoutException($"k.exe execution timed out after {timeoutMs}ms");
                    }
                    
                    if (process.ExitCode != 0)
                    {
                        throw new InvalidOperationException($"k.exe exited with code {process.ExitCode}");
                    }
                }

                if (File.Exists(outputPath))
                {
                    var output = File.ReadAllText(outputPath);
                    return CleanOutput(output);
                }
                
                return string.Empty;
            }
            finally
            {
                // Clean up temporary files
                if (tempScriptPath != null && File.Exists(tempScriptPath))
                {
                    File.Delete(tempScriptPath);
                }
                if (outputPath != null && File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
            }
        }

        private string CreateTempScriptWithExit(string scriptContent)
        {
            // Add double backslash to force k.exe to exit after execution
            var scriptWithExit = scriptContent.TrimEnd() + "\n\\\\";
            
            var tempScriptPath = Path.Combine(tempDirectory, $"k_script_{Guid.NewGuid():N}_{DateTime.Now.Ticks}.k");
            File.WriteAllText(tempScriptPath, scriptWithExit);
            
            return tempScriptPath;
        }

        private string CleanOutput(string output)
        {
            if (string.IsNullOrEmpty(output))
                return string.Empty;

            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var cleanedLines = lines.Where(line => 
                !line.Contains("WIN32") && 
                !line.Contains("CPU") && 
                !line.Contains("MB") &&
                !line.Contains("EVAL")).ToArray();

            return string.Join("\n", cleanedLines).Trim();
        }

        public void CleanupTempDirectory()
        {
            try
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        public static bool ContainsLongInteger(string script)
        {
            // Look for 'j' suffix indicating long integers
            return script.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Any(line => line.Trim().EndsWith("j") && 
                           char.IsDigit(line.Trim().SkipLast(1).LastOrDefault()));
        }
    }
}
