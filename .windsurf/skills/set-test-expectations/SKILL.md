---
name: set-test-expectations
description: Determine what the expected result for a test script should be
---

- Use k MCP Server
- Call tool: execute_k_script in the k MCP Server
- Find if an expectation already exists in K3CSharp.Tests\SimpleTestRunner.cs
- Upsert the expectation (upsert means: update if it exists, or insert if it doesn't exist)