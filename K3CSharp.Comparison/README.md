# K3CSharp.Comparison

This project provides comprehensive comparison testing between K3Sharp and k.exe to measure compatibility and identify differences.

## Features

- **Dynamic Test Discovery**: Automatically finds all `*.k` files in `K3CSharp.Tests/TestScripts/`
- **Single Test Mode**: Run individual test scripts for targeted debugging
- **Batch Processing**: Processes tests in batches to avoid timeouts
- **Intelligent Comparison**: Handles formatting differences (semicolons vs newlines, whitespace variations)
- **Long Integer Detection**: Automatically skips tests with 64-bit integers that k.exe 32-bit cannot handle
- **Comprehensive Reporting**: Generates detailed comparison tables with statistics

## Usage

### Run Full Comparison
```bash
dotnet run --project K3CSharp.Comparison
# or using the batch file
run_comparison.bat
```

### Run Single Test
```bash
dotnet run --project K3CSharp.Comparison -- <test_name>
# Examples:
dotnet run --project K3CSharp.Comparison -- format_symbol_pad_left
dotnet run --project K3CSharp.Comparison -- plus_over_empty
```

### Build Only
```bash
dotnet build K3CSharp.Comparison
```

## Output

The comparison generates `comparison_table.txt` in the project root with:

- **Summary Statistics**: Total tests, success rate, pass/fail breakdown
- **Detailed Results**: Individual test results with K3Sharp vs k.exe outputs
- **Failed Tests Details**: Specific differences for failed tests
- **Error Tests Details**: Error messages for tests that couldn't execute

For single test mode, results are displayed directly to console with full output without truncation.

## Comparison Logic

The comparison uses multiple normalization strategies:

1. **Exact Match**: Direct string comparison after basic normalization
2. **Formatting Equivalence**: Handles semicolon ↔ newline differences
3. **Whitespace Normalization**: Ignores spacing variations
4. **Structural Comparison**: Focuses on essential characters only

## Test Categories

- ✅ **Matched**: K3Sharp and k.exe produce equivalent results
- ❌ **Differed**: Results are meaningfully different
- ⚠️ **Skipped**: Contains unsupported long integers
- 💥 **Error**: Test execution failed

## Files

- `ComparisonRunner.cs` - Main comparison engine with single test support
- `KInterpreterWrapper.cs` - k.exe execution wrapper
- `comparison_table.txt` - Generated comparison report
- `known_differences.txt` - Known differences configuration file (in base folder)
- `run_comparison.bat` - Batch file for easy execution

## Dependencies

- K3CSharp core project
- k.exe (32-bit) must be available at `c:\k\k.exe`

## Notes

- Tests are processed in batches of 20 to prevent timeouts
- Progress is saved after each batch
- Long integer detection includes: `123L`, `456l`, `0IL`, `0NL`
- Formatting equivalence handles semicolon/newline differences intelligently
- Single test mode provides immediate feedback for debugging specific issues
