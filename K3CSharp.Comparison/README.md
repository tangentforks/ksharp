# K3CSharp.Comparison

This project provides comprehensive comparison testing between K3Sharp and k.exe to measure compatibility and identify differences.

## Features

- **Dynamic Test Discovery**: Automatically finds all `*.k` files in `K3CSharp.Tests/TestScripts/`
- **Batch Processing**: Processes tests in batches to avoid timeouts
- **Intelligent Comparison**: Handles formatting differences (semicolons vs newlines, whitespace variations)
- **Long Integer Detection**: Automatically skips tests with 64-bit integers that k.exe 32-bit cannot handle
- **Comprehensive Reporting**: Generates detailed comparison tables with statistics

## Usage

### Run Full Comparison
```bash
dotnet run --project K3CSharp.Comparison
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

- `ComparisonRunner.cs` - Main comparison engine
- `KInterpreterWrapper.cs` - k.exe execution wrapper
- `comparison_table.txt` - Generated comparison report
- `K_WRAPPER_README.md` - Detailed wrapper documentation
- `K_WRAPPER_SUMMARY.md` - Implementation summary and test results

## Documentation

For detailed information about the KInterpreterWrapper implementation, see:
- `K_WRAPPER_README.md` - Complete usage guide and API documentation
- `K_WRAPPER_SUMMARY.md` - Implementation summary and test analysis

## Dependencies

- K3CSharp core project
- k.exe (32-bit) must be available at `c:\k\k.exe`

## Notes

- Tests are processed in batches of 20 to prevent timeouts
- Progress is saved after each batch
- Long integer detection includes: `123L`, `456l`, `0IL`, `0NL`
- Formatting equivalence handles semicolon/newline differences intelligently
