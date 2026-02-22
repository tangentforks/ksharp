using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace K3CSharp
{
    public class KInterpreterWrapper
    {
        private readonly string kExePath;
        private readonly int timeoutMs;
        private readonly string runId;
        private readonly string tempDirectory;

        public KInterpreterWrapper(string kExePath = @"c:\k\e.exe", int timeoutMs = 10000)
        {
            this.kExePath = File.Exists(kExePath) ? kExePath : @"c:\k\k.exe";
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
            if (this.kExePath.Contains("k.exe") && ContainsLongInteger(scriptContent))
            {
                return "UNSUPPORTED: Script contains long integers (64-bit) - k.exe 32-bit does not support them";
            }
            
            return ExecuteScriptWithTimeout(scriptContent, timeoutMs);
        }

        private string ExecuteScriptWithTimeout(string scriptContent, int executionTimeoutMs)
        {
            string? tempScriptPath = null;
            string? outputPath = null;
            
            try
            {
                tempScriptPath = CreateTempScriptWithExit(scriptContent);
                outputPath = Path.Combine(tempDirectory, $"k_output_{Guid.NewGuid():N}_{DateTime.Now.Ticks}.txt");
                
                return ExecuteScriptWithRetryAndErrorCheck(tempScriptPath, outputPath, executionTimeoutMs);
            }
            finally
            {
                CleanupTempFilesWithRetry(tempScriptPath ?? "", outputPath ?? "");
            }
        }

        private string ExecuteScriptWithRetryAndErrorCheck(string scriptPath, string outputPath, int executionTimeoutMs)
        {
            const int maxAttempts = 3;
            const int retryDelayMs = 1000;
            
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    // Use shell execution to properly redirect stdout and stderr
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c \"\"{kExePath}\" \"{scriptPath}\" > \"{outputPath}\" 2>&1\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = false, // Don't redirect - using shell redirection
                        RedirectStandardError = false,  // Don't redirect - using shell redirection
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    
                    // Wait for process to complete with timeout
                    if (!process.WaitForExit(executionTimeoutMs))
                    {
                        process.Kill();
                        if (attempt == maxAttempts)
                        {
                            throw new TimeoutException($"k.exe execution timed out after {executionTimeoutMs}ms");
                        }
                        // Wait before retry
                        System.Threading.Thread.Sleep(retryDelayMs);
                        continue;
                    }

                    // Check if k.exe exited with code 0
                    if (process.ExitCode == 0)
                    {
                        // Add delay after exit to allow file operations to complete
                        System.Threading.Thread.Sleep(100); // 100ms delay
                        return ReadAndCleanOutput(outputPath);
                    }
                    else
                    {
                        // k.exe exited with error code, check stderr for messages
                        if (File.Exists(outputPath))
                        {
                            var errorOutput = File.ReadAllText(outputPath, Encoding.UTF8);
                            throw new Exception($"k.exe exited with code {process.ExitCode}. Output: {errorOutput}");
                        }
                        throw new Exception($"k.exe exited with code {process.ExitCode}");
                    }
                }
                catch (Exception ex)
                {
                    if (attempt == maxAttempts)
                    {
                        throw new Exception($"Failed to execute k.exe after {maxAttempts} attempts: {ex.Message}", ex);
                    }
                    // Wait before retry
                    System.Threading.Thread.Sleep(retryDelayMs);
                }
            }
            
            throw new Exception("k.exe execution failed after maximum retry attempts");
        }

        public static bool ContainsLongInteger(string scriptContent)
        {
            // Pattern 1: Regular long integers: digits followed by 'j' (case insensitive)
            // This matches K long integer notation like 123j, 456j, etc.
            var regularLongPattern = @"\b\d+[Jj]\b";
            
            // Pattern 2: Special K long integers: 0Ij (integer long) and 0Nj (null long)
            var specialLongPattern = @"\b0[ILN][Jj]\b";
            
            // Combine both patterns
            var combinedPattern = $"({regularLongPattern}|{specialLongPattern})";
            return Regex.IsMatch(scriptContent, combinedPattern);
        }

        private string CreateTempScriptWithExit(string scriptContent)
        {
            var tempScriptPath = Path.Combine(tempDirectory, $"k_script_{Guid.NewGuid():N}_{DateTime.Now.Ticks}.k");
            
            // Add _exit 0 to force k.exe to exit with status code 0
            var modifiedContent = scriptContent.TrimEnd() + "\n_exit 0\n";
            
            // Use atomic write with file sharing to prevent locking issues
            using var fileStream = new FileStream(tempScriptPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(fileStream, Encoding.ASCII);
            {
                writer.Write(modifiedContent);
            }
            
            return tempScriptPath;
        }

        private static string ReadAndCleanOutput(string outputPath)
        {
            if (!File.Exists(outputPath))
            {
                return "";
            }

            // Use retry mechanism for file access
            const int maxRetries = 3;
            const int retryDelayMs = 100;
            
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    using var fileStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, FileOptions.SequentialScan);
                    using var reader = new StreamReader(fileStream, Encoding.UTF8);
                    {
                        var content = reader.ReadToEnd();
                        
                        // Filter out licensing and debug information
                        var lines = content.Split('\n');
                        var filteredLines = lines.Where(line => 
                            !string.IsNullOrWhiteSpace(line) &&
                            !line.StartsWith("WIN32") && !line.Contains("EVAL") &&
                            !line.StartsWith("w64") && !line.Contains("PROD") &&
                            !line.StartsWith("K 3.") && !line.Contains("Copyright")).ToArray();
                        
                        return string.Join("\n", filteredLines).TrimEnd();
                    }
                }
                catch (IOException) when (attempt < maxRetries - 1)
                {
                    System.Threading.Thread.Sleep(retryDelayMs);
                    continue;
                }
                catch
                {
                    break;
                }
            }
            
            return "";
        }

        private void CleanupTempFilesWithRetry(params string[] filePaths)
        {
            const int maxRetries = 3;
            const int retryDelayMs = 50;
            
            foreach (var filePath in filePaths)
            {
                if (string.IsNullOrEmpty(filePath)) continue;
                
                for (int attempt = 0; attempt < maxRetries; attempt++)
                {
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }
                        break;
                    }
                    catch (IOException) when (attempt < maxRetries - 1)
                    {
                        System.Threading.Thread.Sleep(retryDelayMs);
                        continue;
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        public bool IsUsingKExe()
        {
            return kExePath.Contains("k.exe");
        }

        ~KInterpreterWrapper()
        {
            CleanupTempFilesWithRetry(tempDirectory);
        }
    }
}
