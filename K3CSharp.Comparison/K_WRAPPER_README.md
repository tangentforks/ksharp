# K Interpreter Wrapper

The K3Sharp project includes a wrapper for invoking an external K interpreter (`k.exe`) for comparison testing and validation purposes.

## Features

- **Automatic Exit Handling**: Adds double backslash (`\\`) to scripts to force k.exe to exit after execution
- **Licensing Filter**: Automatically removes WIN32 licensing information from output
- **Temporary File Management**: Creates and cleans up temporary script and output files
- **Error Handling**: Robust error handling with timeouts and process management
- **Acceptable Discrepancies**: Handles known differences between K3Sharp and k.exe output

## Usage

### Basic Usage

```csharp
using K3CSharp;

var wrapper = new KInterpreterWrapper();
string result = wrapper.ExecuteScript("2 + 3");
Console.WriteLine(result); // Output: 5
wrapper.CleanupTempDirectory();
```

### Custom K.exe Path

```csharp
var wrapper = new KInterpreterWrapper(@"d:\kdb\k.exe");
```

## Integration Status

The KInterpreterWrapper is fully integrated into the K3CSharp.Comparison project and provides:

- **Reliable k.exe Execution**: Successfully executes K scripts with proper exit handling
- **Output Cleaning**: Removes licensing information and normalizes output
- **Error Management**: Handles timeouts, process failures, and cleanup automatically
- **Comparison Ready**: Designed specifically for K3Sharp vs k.exe compatibility testing

## Key Requirements Handled

1. **Exit Handling**: When k.exe is invoked with a script, it normally doesn't exit the REPL. The wrapper automatically adds `\\` to the end of scripts to force exit.

2. **Licensing Information**: Lines like `WIN32 32CPU 4095MB rufeu01-hx.rufian.zilbermann.com 0 EVAL` are automatically filtered from output.

3. **Cleanup**: All temporary files (script copies, output files) are automatically removed after execution.

4. **Acceptable Discrepancies**: The comparison logic handles known differences such as:
   - Help command messages
   - Floating point precision differences
   - Special value representations (0I vs 0N, etc.)

## Classes

### KInterpreterWrapper

Main wrapper class for executing K scripts.

#### Key Methods
- `ExecuteScript(string script)` - Execute a K script and return cleaned output
- `CleanupTempDirectory()` - Clean up temporary files
- Constructor accepts optional k.exe path parameter

#### Properties
- `KExecutablePath` - Path to k.exe executable (default: `c:\k\k.exe`)
- `TempDirectory` - Temporary directory for script execution
- `TimeoutMs` - Execution timeout in milliseconds (default: 10000)

## Error Handling

The wrapper provides comprehensive error handling:
- **Timeout Protection**: 10-second default timeout prevents hanging
- **Process Management**: Proper process cleanup and resource management
- **File System Errors**: Graceful handling of temporary file issues
- **K.exe Errors**: Clear error messages for k.exe execution failures

## Dependencies

- k.exe (32-bit) must be available at the specified path
- .NET 6.0 or later
- Windows operating system (k.exe is Windows-specific)

## Notes

- The wrapper is specifically designed for comparison testing, not general K execution
- All temporary files are automatically cleaned up after execution
- The wrapper handles both script execution and interactive mode scenarios
- Output is normalized to remove k.exe-specific artifacts
