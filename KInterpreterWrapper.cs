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

        public KInterpreterWrapper(string kExePath = @"c:\k\k.exe", int timeoutMs = 10000)
        {
            this.kExePath = kExePath;
            this.timeoutMs = timeoutMs;
            this.tempDirectory = Path.Combine(Path.GetTempPath(), "ksharp_wrapper");
            
            // Ensure temp directory exists
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
        }

        public string ExecuteScript(string scriptContent)
        {
            var tempScriptPath = CreateTempScriptWithExit(scriptContent);
            var outputPath = Path.Combine(tempDirectory, $"k_output_{Guid.NewGuid():N}.txt");
            
            try
            {
                // Check for unsupported long integers (32-bit k.exe limitation)
                if (ContainsLongInteger(scriptContent))
                {
                    return "UNSUPPORTED: Script contains long integers (64-bit) - k.exe 32-bit does not support them";
                }
                
                ExecuteKProcess(tempScriptPath, outputPath);
                return ReadAndCleanOutput(outputPath);
            }
            finally
            {
                CleanupTempFiles(tempScriptPath, outputPath);
            }
        }

        private bool ContainsLongInteger(string scriptContent)
        {
            // Pattern: digits followed by 'L' (case insensitive)
            // This matches K long integer notation like 123L, 456l, etc.
            var longIntegerPattern = @"\b\d+[Ll]\b";
            return System.Text.RegularExpressions.Regex.IsMatch(scriptContent, longIntegerPattern);
        }

        private string CreateTempScriptWithExit(string scriptContent)
        {
            var tempScriptPath = Path.Combine(tempDirectory, $"k_script_{Guid.NewGuid():N}.k");
            
            // Add _exit 0 to force k.exe to exit with status code 0
            // This is more explicit than double backslash and provides a clean exit
            var modifiedContent = scriptContent.TrimEnd() + "\n_exit 0\n";
            
            // Use ASCII encoding to avoid UTF-8 BOM issues with k.exe
            File.WriteAllText(tempScriptPath, modifiedContent, Encoding.ASCII);
            return tempScriptPath;
        }

        private string ExecuteKProcess(string scriptPath, string outputPath)
        {
            // Use shell execution to properly redirect stdout
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{kExePath}\" \"{scriptPath}\" > \"{outputPath}\" 2>&1",
                UseShellExecute = false,
                RedirectStandardOutput = false, // Don't redirect - using shell redirection
                RedirectStandardError = false,  // Don't redirect - using shell redirection
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            
            try
            {
                process.Start();
                
                // Monitor output file with timeout
                var checkIntervalMs = 500; // Check every 500ms
                var elapsedMs = 0;
                var lastFileSize = 0L;
                var lastModified = DateTime.MinValue;
                
                while (!process.HasExited && elapsedMs < timeoutMs)
                {
                    System.Threading.Thread.Sleep(checkIntervalMs);
                    elapsedMs += checkIntervalMs;
                    
                    // Check if output file exists and is being written to
                    if (File.Exists(outputPath))
                    {
                        var fileInfo = new FileInfo(outputPath);
                        var currentSize = fileInfo.Length;
                        var currentModified = fileInfo.LastWriteTime;
                        
                        // If file hasn't changed in the last check interval, consider it hung
                        if (currentSize == lastFileSize && currentModified == lastModified)
                        {
                            // File hasn't changed - might be hung
                            if (elapsedMs >= 2000) // Give at least 2 seconds before considering hung
                            {
                                Console.WriteLine($"k.exe appears to be hung (no output for {checkIntervalMs}ms)");
                                process.Kill();
                                throw new TimeoutException($"k.exe execution timed out - no output for {elapsedMs}ms");
                            }
                        }
                        else
                        {
                            // File is being written to, reset timeout
                            lastFileSize = currentSize;
                            lastModified = currentModified;
                        }
                    }
                }
                
                // Final check for process completion
                if (!process.HasExited)
                {
                    process.Kill();
                    throw new TimeoutException($"k.exe execution timed out after {timeoutMs}ms");
                }

                if (process.ExitCode != 0)
                {
                    // Read error from output file if it exists
                    if (File.Exists(outputPath))
                    {
                        var errorOutput = File.ReadAllText(outputPath, Encoding.UTF8);
                        throw new Exception($"k.exe exited with code {process.ExitCode}. Output: {errorOutput}");
                    }
                    throw new Exception($"k.exe exited with code {process.ExitCode}");
                }
                
                return outputPath;
            }
            catch (Exception ex)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
                throw new Exception($"Failed to execute k.exe: {ex.Message}", ex);
            }
        }

        private string ReadAndCleanOutput(string outputPath)
        {
            if (!File.Exists(outputPath))
            {
                return "";
            }

            var lines = File.ReadAllLines(outputPath, Encoding.UTF8);
            var cleanedLines = new List<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Skip licensing information lines that start with WIN32 and end with EVAL
                if (trimmedLine.StartsWith("WIN32") && trimmedLine.EndsWith("EVAL"))
                {
                    continue;
                }
                
                // Skip stderr markers
                if (trimmedLine.StartsWith("STDERR:"))
                {
                    continue;
                }
                
                // Add the cleaned line
                cleanedLines.Add(line);
            }

            return string.Join("\n", cleanedLines).TrimEnd();
        }

        private void CleanupTempFiles(params string[] filePaths)
        {
            foreach (var filePath in filePaths)
            {
                try
                {
                    if (filePath != null && File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
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

        ~KInterpreterWrapper()
        {
            CleanupTempDirectory();
        }
    }
}
