---
name: write-test-case
description: Write a K script that can be used as a test case
---

1 - Use examples in the documentation. Extrapolate from those examples if more tests are desired
2 - Avoid creating a test that already exists. Search K3CSharp.Tests\\TestScripts\\*.k
3 - Follow @Rules:testing-strategy (e.g., avoid unnecessary comments, use meaningful file names)
4 - Determine if the test is for standard K functionality or for ksharp special functionality
    ksharp special functionality can be FFI or _parse/_eval:
    - ✅ **Parse and eval**: e.g. `_parse "1 + 2"` ``_eval (`"+";1;2)``
    - ✅ **.NET type loading**: e.g., `2:` loads Assemblies into `` ._dotnet`` tree 
    - ✅ **.NET type conversion hints**: e.g., `_sethint` and `_gethint`
5 - If the test uses standard K, always use the k MCP to verify that the syntax is correct. If the K MCP produces an error, keep extrapolating examples based on the documentation until you find a test that k.exe can run successfully
6 - If the test is expected to return a symbol vector, a mixed list or a dictionary, add an entry in known_differences.txt to account for compact representation
7 - Reload the known differences for the k MCP and after reloading, determine the expected result using the k MCP. Add the test with expected result to the test runner