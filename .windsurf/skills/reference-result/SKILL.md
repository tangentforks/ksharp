---
name: reference-result
description: Get behavior of reference (canonical) implementation
---

- The ksharp MCP will produce implementation behavior, it does not return reference behavior
- The k (k.exe) MCP is used to determine the canonical behavior
- Prefer tool execute_k_script, because it is easier to get reliable results
- The tool execute_k_command is also available but it needs escape sequences for characters like quotation marks and newlines. If errors happen after using execute_k_command fall back to using execute_k_script
