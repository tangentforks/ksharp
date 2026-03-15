---
name: run-test-suite
description: Run the test suite to evaluate progress and identify regressions
---

[] Read original file: T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\results_table.txt
[] Identify original line with pattern: "SUMMARY: (0|[1-9]\d*)/(0|[1-9]\d*) tests passed (\d+(\.\d+)?%)"
[] Note the nubers in the original identified line
[] run terminal command: cd T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests
[] run terminal command: dotnet run
[] Read file (updated): T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\results_table.txt
[] if the number of tests in the original identified file is higher than 20 
    - [] Identify line with pattern: "SUMMARY: (0|[1-9]\d*)/(0|[1-9]\d*) tests passed (\d+(\.\d+)?%)"
    - [] Compare updated numbers to previous numbers 
[] Identify Section titled FAILING TESTS DETAILS
[] Note the names of tests in lines with pattern "^Test: *"
