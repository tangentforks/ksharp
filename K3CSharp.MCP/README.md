K3CSharp MCP Server — Specification
====================================

1. Overview
-----------

An MCP (Model Context Protocol) server that exposes a persistent K3 interpreter
session to AI coding agents. The server manages a K3CSharp child process,
supports hot-reload after recompilation, and provides tools for executing K code
and inspecting interpreter state.

The server speaks MCP (JSON-RPC 2.0 over stdio) directly, with no SDK dependency.


2. Architecture
---------------

  Claude Code <--MCP/JSON-RPC--> MCP Server <--stdin/stdout--> K3CSharp.exe (shadow copy)

- MCP Server: Standalone .NET 8 console app (zero NuGet deps). Speaks JSON-RPC
  2.0 over stdio to the MCP client (Claude Code).
- K3CSharp process: Launched as a child process in --mcp REPL mode from a
  shadow copy of the built executable (avoids locking build outputs).
- Shadow copy directory: <temp>/k3mcp/ — the MCP server copies K3CSharp.exe +
  deps here before launching.


3. Tools
--------

k_eval
~~~~~~
Execute K code in the running session.

  Parameter  Type    Required  Description
  code       string  yes       K expression(s) to evaluate

Returns the printed output from the interpreter. Supports multi-line input.
Errors are returned as-is (K error format like "length error").

k_reset
~~~~~~~
Reset the K session to a clean state (equivalent to \9).

No parameters. Clears all variables, resets KTree and random seed.

k_reload
~~~~~~~~
Hot-reload the interpreter from a fresh build.

  Parameter       Type  Required             Description
  preserve_state  bool  no (default: false)  If true, attempt to save and
                                             restore session state across reload

Sequence:
  1. If preserve_state: capture state via k_inspect snapshot (not yet implemented).
  2. Terminate current K3CSharp child process.
  3. Copy new build artifacts from K3CSharp/bin/Debug/net8.0/ to shadow
     directory.
  4. Launch new child process from shadow copy.
  5. If preserve_state: replay variable assignments to restore state
     (best-effort, not yet implemented).

k_inspect
~~~~~~~~~
Inspect the K namespace tree.

  Parameter  Type    Required            Description
  path       string  no (default: ".")   K tree path to inspect (e.g. .k, .k.foo)
  depth      int     no (default: 1)     How many levels deep to enumerate

Returns variable and function names at the given path.
Currently uses \v command in K3CSharp, which is NOT YET IMPLEMENTED.
Once \v is added, this tool will work. Until then it returns an error.

k_status
~~~~~~~~
Report server and session health.

No parameters. Returns:
  - Whether a K process is running
  - Path to shadow copy in use
  - Path to source build directory
  - Session uptime / command count
  - running_hash: MD5 of the K3CSharp.dll that the current session was launched from
  - build_hash:   MD5 of the K3CSharp.dll in the source build directory
  - stale: true if running_hash != build_hash (interpreter has been recompiled
    since this session was launched and a k_reload is recommended)


4. Child Process Management
---------------------------

Launch protocol:
  1. Copy K3CSharp/bin/Debug/net8.0/ contents to shadow directory.
  2. Compute and store the MD5 hash of the shadow copy of K3CSharp.dll.
     This becomes the "running_hash" for the lifetime of this session.
  3. Start K3CSharp.exe --mcp from shadow directory, redirecting
     stdin/stdout/stderr.
  4. Session is ready when the sentinel is received on stdout.

Command protocol:
  1. Write K expression + newline to child stdin.
  2. Read stdout until the sentinel (\x01\x02) is received.
  3. Everything before the sentinel is the result.

The --mcp flag on K3CSharp (implemented) enables sentinel-based framing and
suppresses the interactive banner.

Hash staleness detection:
  - On launch, the MD5 of the shadow K3CSharp.dll is stored as running_hash.
  - k_status computes build_hash (from source build dir) on demand and compares.
  - If running_hash != build_hash, the session is stale and k_reload is advised.

Process lifecycle:
  - If the child process crashes, k_eval returns an error and the next call
    auto-relaunches.
  - On MCP server shutdown, the child process is terminated.


5. K3CSharp Changes
--------------------

Changes made to K3CSharp to support MCP integration:

  1. --mcp mode (DONE): Emits a machine-readable sentinel (\x01\x02) after each
     result instead of the two-space prompt. Suppresses the startup banner.
     Reads lines from stdin, flushes stdout after each response.

  2. \v command (TODO): Dump all variable names and types in the current scope
     as structured output. This will power k_inspect.


6. Configuration
----------------

Register via Claude Code CLI:

  claude mcp add ksharp -- dotnet run --project /path/to/KSharp/K3CSharp.MCP

Or via claude_desktop_config.json / Claude Code MCP settings:

  {
    "mcpServers": {
      "ksharp": {
        "command": "dotnet",
        "args": ["run", "--project", "/path/to/KSharp/K3CSharp.MCP"]
      }
    }
  }

The K3_BUILD_DIR environment variable can optionally override the path to the
K3CSharp build output. If not set, it defaults to
K3CSharp/bin/Debug/net8.0/ relative to the repo.


7. Error Handling
-----------------

- Build not found: If shadow copy fails (no build artifacts), return clear error
  suggesting "dotnet build".
- Process crash: Detect via exit code, return error, auto-relaunch on next call.
- Timeout: Not yet implemented. Long-running K expressions will block.
