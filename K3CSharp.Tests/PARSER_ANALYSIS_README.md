# K3CSharp Parser Analysis Report System

## Overview

The K3CSharp Parser Analysis Report System provides comprehensive analysis of LRS (Long Right Scope) parser performance and failures. This system replaces the previous `[LRSParserWrapper]` debug messages with detailed, configurable reports.

## Configuration

The system is controlled by `parser_config.json` in the `K3CSharp.Tests` directory:

```json
{
  "ParserAnalysis": {
    "EnableReportGeneration": true,
    "ReportOutputPath": "T:\\_src\\github.com\\ERufian\\ksharp\\K3CSharp.Tests\\parser_results",
    "MaxReportEntries": 1000,
    "IncludeParseTrees": true,
    "IncludeLegacyParserAST": true,
    "ReportFormat": "Markdown",
    "GenerateDetailedFailureAnalysis": true,
    "GroupFailuresByType": true
  },
  "Logging": {
    "EnableDebugMessages": false,
    "EnableLRSWrapperMessages": false,
    "LogLevel": "Info"
  }
}
```

### Configuration Options

#### ParserAnalysis Settings
- **EnableReportGeneration**: Set to `true` to generate reports, `false` to disable (default: false)
- **ReportOutputPath**: Directory where reports will be generated
- **MaxReportEntries**: Maximum number of entries per report section (default: 1000)
- **IncludeParseTrees**: Include detailed parse trees in reports (default: true)
- **IncludeLegacyParserAST**: Include legacy parser AST for comparison (default: true)
- **ReportFormat**: Output format (currently only "Markdown" is supported)
- **GenerateDetailedFailureAnalysis**: Generate detailed failure analysis (default: true)
- **GroupFailuresByType**: Group failures by type in analysis (default: true)

#### Logging Settings
- **EnableDebugMessages**: Enable general debug messages (default: false)
- **EnableLRSWrapperMessages**: Enable LRS wrapper debug messages (default: false)
- **LogLevel**: Logging level (default: "Info")

## Generated Reports

When enabled, the system generates the following reports in the `parser_results` directory:

### 1. Main Analysis Report (`parser_analysis_report.md`)

**A) Non-Null AST with Incorrect Results**
For test cases that produce a parse tree but generate incorrect results:
- Complete input
- Generated parse tree
- Expected result
- Actual result
- Legacy parser result (if available)

**B) LRS Parser NULL Results**
For test cases that cause the LRS parser to return NULL:
- Complete input
- Failure location (e.g., "After PLUS (position 3/5)")
- Consumed tokens information
- Legacy parser AST (if enabled)

### 2. Detailed Failure Analysis (`detailed_failure_analysis.md`)

- Token type analysis
- Failure point analysis
- Statistical breakdown of failures

### 3. Summary Statistics (`summary_statistics.txt`)

- Overall test results
- LRS parser success rate
- Top failure patterns
- Key metrics

## Usage

### Running Tests with Report Generation

1. **Enable Report Generation**: Set `EnableReportGeneration` to `true` in `parser_config.json`
2. **Run Tests**: Execute the test runner as usual:
   ```bash
   dotnet run --project K3CSharp.Tests
   ```
3. **View Reports**: Check the `parser_results` directory for generated reports

### Disabling Report Generation

To disable report generation and return to the default behavior:
- Set `EnableReportGeneration` to `false` in `parser_config.json`
- Or delete the configuration file (defaults to disabled)

### Enabling Debug Messages

To restore the previous `[LRSParserWrapper]` debug messages:
- Set `EnableLRSWrapperMessages` to `true` in `parser_config.json`

## Architecture

### Components

1. **ParserAnalysisConfig**: Configuration management with JSON file support
2. **LRSFailureAnalyzer**: Tracks and analyzes LRS parser failures
3. **ParserReportGenerator**: Generates comprehensive reports
4. **LRSParserWrapper**: Integrated with failure tracking (debug messages removed)

### Data Flow

1. **Test Execution**: SimpleTestRunner runs tests as usual
2. **Failure Tracking**: LRSParserWrapper records failures during parsing
3. **Report Generation**: ParserReportGenerator creates reports after test completion
4. **Output**: Reports written to configured directory

## Benefits

- **Comprehensive Analysis**: Detailed breakdown of parser failures
- **Configurable**: Easy to enable/disable and customize output
- **Clean Output**: Replaces verbose debug messages with structured reports
- **Historical Tracking**: Records can be analyzed over time
- **Performance Impact**: Minimal overhead when disabled

## Example Report Output

### Main Analysis Report Example

```markdown
# K3CSharp Parser Analysis Report

**Generated:** 2026-03-20 17:30:45
**Test Results:** 805/831 passed (96.9%)

## Executive Summary

- **Total LRS Parser Failures**: 26
- **Total Incorrect Results:** 0
- **LRS Parser Success Rate:** 96.9%

## Failure Analysis

### A) Non-Null AST with Incorrect Results
*No cases found*

### B) LRS Parser NULL Results

#### 1. `complex_expression.k`
**Complete Input:**
```k
a + b * c / d - e
```
**Failure Location:**
- After DIVIDE (position 7/9)
- Consumed: 7/9 tokens
```

## Recommendations

🔍 **Focus Area:** After DIVIDE (position 7/9) (12 occurrences)

📋 **Suggested Actions:**
1. Prioritize fixing the most common failure patterns identified above
2. Review the NULL result cases to understand parsing boundaries
```

## Integration Notes

- **Zero Breaking Changes**: System is completely optional and defaults to disabled
- **Performance**: Minimal overhead when disabled
- **Compatibility**: Works with existing test infrastructure
- **Extensibility**: Easy to add new report types and analysis features

## Troubleshooting

### Reports Not Generated
- Check that `EnableReportGeneration` is set to `true`
- Verify the `ReportOutputPath` directory is writable
- Check for any error messages in the test runner output

### Missing Debug Messages
- Set `EnableLRSWrapperMessages` to `true` to restore debug output
- Check the `Logging` section in configuration

### Build Errors
- Ensure all configuration files are valid JSON
- Check that required directories exist and are accessible
