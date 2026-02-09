# K3CSharp Serialization Research Tool

## Overview

The Serialization Research Tool is a specialized utility designed to explore and document the binary serialization formats used by k.exe for the `_bd` (bytes from data) and `_db` (data from bytes) verbs. This tool enables K3CSharp to achieve binary compatibility with k.exe's serialization format, which is essential for implementing robust I/O operations.

## Purpose

The K programming language's serialization format is not officially documented. This tool uses a scientific methodology to:

1. Generate comprehensive test data for all K data types
2. Execute `_bd` commands via k.exe to obtain serialized representations
3. Produce Cascade Agent-friendly output for pattern discovery
4. Enable systematic reverse engineering of the binary format

## Installation

### Prerequisites

- .NET 9.0 or later
- k.exe (32-bit K interpreter) available at `c:\k\k.exe` or specified path
- Windows operating system (k.exe is Windows-specific)

### Building

```bash
cd K3CSharp.SerializationResearch
dotnet build
```

## Usage

### Command Line Interface

```bash
SerializationResearch.exe --type <datatype> [--count <number>] [--edge-cases] [--kexe <path>] [--output <dir>]
```

### Parameters

- `--type, -t`: Data type to explore (required)
- `--count, -c`: Number of random examples to generate (default: 100)
- `--edge-cases, -e`: Generate edge cases only (ignores --count)
- `--kexe, -k`: Path to k.exe executable (default: c:\k\k.exe)
- `--output, -o`: Output directory (default: output)
- `--help, -h`: Show help message

### Supported Data Types

| Type | Description | K Type Number |
|------|-------------|---------------|
| `integer, int` | 32-bit integers with special values | 1 |
| `float` | IEEE 754 doubles with special values | 2 |
| `character, char` | ASCII characters (0-255) | 3 |
| `symbol` | Unquoted and quoted symbols | 4 |
| `dictionary, dict` | Simple and complex dictionaries | 5 |
| `null` | Null value | 6 |
| `anonymous, func` | Anonymous functions | 7 |
| `intvector` | Integer vectors | -1 |
| `floatvector` | Float vectors | -2 |
| `charvector` | Character vectors | -3 |
| `symbolvector` | Symbol vectors | -4 |
| `list` | Mixed type lists | 0 |

### Examples

```bash
# Generate 50 random integer examples
SerializationResearch.exe --type integer --count 50

# Generate edge cases for float type
SerializationResearch.exe --type float --edge-cases

# Generate 100 symbol examples with custom k.exe path
SerializationResearch.exe -t symbol -c 100 -k "d:\kdb\k.exe"

# Generate edge cases for complex types
SerializationResearch.exe --type dictionary --edge-cases --output research_output
```

## Output Format

The tool generates timestamped text files in the specified output directory. Each file contains:

```
# Serialization Research: Integer (edge cases)
# Generated: 2026-02-09 17:30:00
# Total examples: 8

_bd 0 → 1 0 0 0 0 0 0 0
_bd 1 → 1 1 0 0 0 0 0 0
_bd -1 → 1 255 255 255 255 255 255 255
_bd 0N → 1 128 0 0 0 0 0 128
_bd 0I → 1 127 255 255 255 255 255 255
_bd -0I → 1 255 0 0 0 0 0 128
_bd 2147483647 → 1 127 255 255 255 255 255 255
_bd -2147483648 → 1 128 0 0 0 0 0 0

# Summary:
# Successful examples: 8
# Data type: Integer
# Mode: Edge cases
```

## Scientific Method Integration

The tool is designed to work with the Cascade Agent using a systematic approach:

1. **Edge Case Generation**: Start with `--edge-cases` to establish baseline patterns
2. **Random Sampling**: Use `--count` to generate additional examples if needed
3. **Pattern Discovery**: Cascade Agent analyzes output to identify serialization patterns
4. **Hypothesis Testing**: Generate new examples to validate discovered patterns
5. **Iteration**: Repeat until 99% confidence is achieved

### Example Workflow

```bash
# Step 1: Generate edge cases
SerializationResearch.exe --type integer --edge-cases

# Step 2: If patterns unclear, generate more examples
SerializationResearch.exe --type integer --count 100

# Step 3: Cascade Agent analyzes output and formulates hypothesis

# Step 4: Test hypothesis with new examples
SerializationResearch.exe --type integer --count 50
```

## Data Type Coverage

### Integer (Type 1)
- **Range**: Full 32-bit integers (-2,147,483,648 to 2,147,483,647)
- **Special Values**: 0N (null), 0I (infinity), -0I (negative infinity)
- **Edge Cases**: 0, 1, -1, min/max values, special values

### Float (Type 2)
- **Range**: IEEE 754 double precision
- **Special Values**: 0n (NaN), 0i (infinity), -0i (negative infinity)
- **Edge Cases**: 0.0, ±1.0, min/max double, special values

### Character (Type 3)
- **Range**: ASCII 0-255
- **Formats**: Printable characters, octal escape sequences
- **Edge Cases**: Control characters, null, printable range

### Symbol (Type 4)
- **Formats**: Unquoted alphanumeric, quoted full ASCII
- **Constraints**: Unquoted symbols don't start with numbers
- **Edge Cases**: Empty symbol, quoted symbols with special chars

### Dictionary (Type 5)
- **Structures**: Empty, simple, nested, attributed dictionaries
- **Keys**: Symbol keys only (unquoted and quoted)
- **Values**: All data types including nested dictionaries

### Vectors (Types -1 to -4)
- **Lengths**: 0 to 10 elements
- **Content**: Full type range for each vector type
- **Edge Cases**: Empty vectors, single elements, special values

### List (Type 0)
- **Constraint**: Avoids uniform vectors (K collapses them)
- **Content**: Mixed types, nested structures, edge cases
- **Edge Cases**: Empty list, single elements, complex nesting

## Error Handling

The tool implements robust error handling:

- **Timeout Protection**: 10-second default timeout for k.exe execution
- **Graceful Degradation**: Failed examples are skipped and replaced
- **Output Cleaning**: Automatic removal of k.exe licensing information
- **Resource Management**: Automatic cleanup of temporary files and processes

## Integration with K3CSharp

Once serialization patterns are discovered, they can be implemented in K3CSharp:

1. **Pattern Analysis**: Use discovered patterns to understand binary format
2. **Implementation**: Code `_bd` and `_db` verbs in K3CSharp
3. **Validation**: Test against k.exe for binary compatibility
4. **Documentation**: Record format specifications for future reference

## Troubleshooting

### Common Issues

1. **k.exe not found**: Ensure k.exe is at default path or use `--kexe` parameter
2. **Timeout errors**: Increase timeout by modifying KInterpreterWrapper constructor
3. **Build errors**: Ensure .NET 9.0 SDK is installed
4. **Permission errors**: Run with appropriate file system permissions

### Debug Mode

For debugging, the tool provides detailed console output:
- Example generation progress
- Success/failure counts
- Output file paths
- Error messages for failed examples

## Future Extensions

The tool architecture supports future enhancements:

- **File Serialization**: `_bd` with file operations
- **IPC Serialization**: Inter-process communication patterns
- **Pattern Detection**: Automated pattern recognition
- **Validation Suite**: Binary compatibility testing framework

## Contributing

When extending the tool:

1. Maintain compatibility with existing output format
2. Add comprehensive test coverage for new features
3. Update documentation for new data types or options
4. Follow the established error handling patterns

## License

This tool is part of the K3CSharp project and follows the same licensing terms.
