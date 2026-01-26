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

## Key Requirements Handled

1. **Exit Handling**: When k.exe is invoked with a script, it normally doesn't exit the REPL. The wrapper automatically adds `_exit 0` to the end of scripts to force exit.

2. **Licensing Information**: Lines like `WIN32 32CPU 4095MB rufeu01-hx.rufian.zilbermann.com 0 EVAL` are automatically filtered from output.

3. **Cleanup**: All temporary files (script copies, output files) are automatically removed after execution.

4. **Acceptable Discrepancies**: The comparison logic handles known differences such as:
   - Help command messages
   - Floating point precision differences
   - Special value representations (0I vs 0N, etc.)

## Classes

### KInterpreterWrapper

Main wrapper class for executing K scripts.

**Methods:**
- `ExecuteScript(string scriptContent)` - Executes a K script and returns cleaned output
- `CleanupTempDirectory()` - Manually cleanup temporary files

### KComparisonTestRunner

Utility for comparing K3Sharp output with k.exe output across test suites.

**Methods:**
- `RunComparisonTests()` - Runs comparison tests on all test files
- `CompareTestFile(string fileName)` - Compares a single test file

### KWrapperDemo

Demonstration program showing wrapper usage with various K features.

## Example Output

```csharp
var wrapper = new KInterpreterWrapper();

// Simple arithmetic
string result1 = wrapper.ExecuteScript("2 + 3");
// Returns: "5"

// Vector operations
string result2 = wrapper.ExecuteScript("1 2 3 + 4 5 6");
// Returns: "5 7 9"

// Function definition and call
string result3 = wrapper.ExecuteScript("f:{[x] x*2}\nf 5");
// Returns: "10"

// Vector indexing
string result4 = wrapper.ExecuteScript("v:10 20 30 40 50\nv[2]");
// Returns: "30"
```

## Error Handling

The wrapper includes comprehensive error handling:

- **Timeout Protection**: 10-second timeout for k.exe execution
- **Process Management**: Proper process cleanup and termination
- **File I/O Errors**: Graceful handling of temporary file issues
- **Encoding Support**: UTF-8 encoding for proper character handling

## Integration with Test Suite

The wrapper can be integrated with the existing test suite to validate K3Sharp behavior against the reference K implementation:

```csharp
var runner = new KComparisonTestRunner();
runner.RunComparisonTests();
```

This will run comparison tests and show:
- Total tests compared
- Match percentage
- Acceptable discrepancies
- Error counts for both implementations
