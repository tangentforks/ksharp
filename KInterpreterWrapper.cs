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

        public KInterpreterWrapper(string kExePath = @"c:\k\k.exe")
        {
            this.kExePath = kExePath;
            this.tempDirectory = Path.Combine(Path.GetTempPath(), "ksharp_wrapper");
            
            // Ensure temp directory exists
            if (!Directory.Exists(tempDirectory))
            {
                Directory.CreateDirectory(tempDirectory);
            }
        }

        public string ExecuteScript(string scriptContent)
        {
            string tempScriptPath = null;
            string tempOutputPath = null;

            try
            {
                // Create temporary script with double backslash to force exit
                tempScriptPath = CreateTempScriptWithExit(scriptContent);
                
                // Create temporary output file
                tempOutputPath = Path.Combine(tempDirectory, $"k_output_{Guid.NewGuid():N}.txt");
                
                // Execute k.exe with the temporary script
                var result = ExecuteKProcess(tempScriptPath, tempOutputPath);
                
                // Read and clean the output
                var output = ReadAndCleanOutput(tempOutputPath);
                
                return output;
            }
            finally
            {
                // Clean up temporary files
                CleanupTempFiles(tempScriptPath, tempOutputPath);
            }
        }

        private string CreateTempScriptWithExit(string scriptContent)
        {
            var tempScriptPath = Path.Combine(tempDirectory, $"k_script_{Guid.NewGuid():N}.k");
            
            // Add double backslash to force k.exe to exit
            var modifiedContent = scriptContent.TrimEnd() + "\n\\\\\n";
            
            File.WriteAllText(tempScriptPath, modifiedContent, Encoding.UTF8);
            return tempScriptPath;
        }

        private string ExecuteKProcess(string scriptPath, string outputPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = kExePath,
                Arguments = $"\"{scriptPath}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = startInfo };
            
            // Also redirect output to file for complete capture
            using var outputWriter = new StreamWriter(outputPath, false, Encoding.UTF8);
            
            process.OutputDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    outputWriter.WriteLine(e.Data);
                }
            };
            
            process.ErrorDataReceived += (sender, e) => {
                if (e.Data != null)
                {
                    outputWriter.WriteLine($"STDERR: {e.Data}");
                }
            };

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                // Wait for process to complete with timeout
                if (!process.WaitForExit(10000)) // 10 second timeout
                {
                    process.Kill();
                    throw new TimeoutException("k.exe execution timed out");
                }

                if (process.ExitCode != 0)
                {
                    throw new Exception($"k.exe exited with code {process.ExitCode}");
                }

                outputWriter.Flush();
                return outputPath;
            }
            catch (Exception ex)
            {
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
