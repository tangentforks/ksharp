---
description: Pre-commit checklist for K3CSharp project
---

# Pre-Commit Checklist for K3CSharp

This workflow ensures the K3CSharp project is ready for source control commits by validating code quality, test coverage, and documentation completeness.

## Usage

Run this workflow before committing changes to ensure:
- All code quality standards are met
- Tests are passing and up-to-date
- Documentation is synchronized
- File system is clean

## Steps

### 1. Development Documentation Updates
- [ ] Update recent insights into the K3CSharp implementation details document

### 2. Build Validation
- [ ] Run clean build on all three projects:
  - Main project: `K3CSharp.csproj`
  - Test project: `K3CSharp.Tests.csproj` 
  - Comparison project: `K3CSharp.Comparison.csproj`
- [ ] Verify zero warnings and zero errors across all projects

### 3. Test Cleanup and Validation
- [ ] Clean up any test cases outside `T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\TestScripts`
  - Either move them to the correct TestScripts directory
  - Or remove them if they are temporary/debug tests not needed
- [ ] Clean up file system by removing temporary and debug output files
- [ ] Run full test suite at `T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\`
  - Verify that `T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\results_table.txt` has been updated

### 4. Comparison Testing
- [ ] Run full comparison with 32-bit k.exe
  - Use tool at `T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\` 
  - Verify that `T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\results_table.txt` has been updated
- [ ] Allow sufficient time for completion (can run in background)

### 5. User Documentation Updates
- [ ] Update README.md (in project root: `T:\_src\github.com\ERufian\ksharp\README.md`) 
  - Add new information
  - Remove obsolete information
  - Update any outdated examples or instructions
  - Ensure all language features are mentioned
  - Update statistics, using the results from the Test Suite and the Comparison Tool from steps 3 and 4
  - Ensure there are no outdated statistics referenced in other sections
  - Avoid hype. Eliminate comments about being "production ready" or similar.
  - Preserve Installation Instructions
  - Preserve Authorship information

### 6. Final Checks
- [ ] Review git status for any untracked files
- [ ] Verify no sensitive or temporary files are staged

## Completion Criteria

The checklist is complete when:
- ✅ All documentation is current (Development & User docs)
- ✅ All projects build cleanly with zero warnings/errors
- ✅ Test suite passes with no regressions
- ✅ Comparison results are updated
- ✅ File system is clean and organized
- ✅ Git status is clean and ready for commit

## Notes

- The full comparison can take significant time to complete
- Test results should be reviewed for any unexpected failures
- Build warnings should be addressed before commit
- Consider running a quick smoke test after all checks pass
