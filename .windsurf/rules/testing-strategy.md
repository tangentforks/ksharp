---
trigger: always_on
---
## Testing Strategy

- Use Test Driven Development
- Write tests before implementing functionality
- Determine expected results by running the comparison tool for the specific test and getting the k.exe result.
- Do not run the test suite from the base folder.
- The comparison tool must be run from its own folder

#### **Test Naming Conventions**
- **Descriptive names**: `format_symbol_string_mixed_vector.k`
- **Consistent prefixes**: `format_`, `form_`, `variable_scoping_`
- **Clear purpose**: Name should indicate what is being tested

#### **Test File Structure**
- **Ideal format**: One line of code whenever possible. If the purpose is clear in the name there is no need to add comments.
- **Inline comments**: If comments are appropriate, they should be added at the end of the line.
- **Avoid unnecessary comments**: If the purpose is clear in the file name there is no need to add comments.
```k
4 _in 1 7 2 4 6 3
```

#### **Test Script Design Principles**

##### **One Test Per File**
- **Single focus**: Each test script should test exactly one aspect of functionality
- **One output**: Each test script should expect only one output value
- **Multiple lines only**: When variable assignments are required before the test (this including assigning anonymous functions to give them a name)

#### **Setting Test Expectations**
Try not to guess what the expected result should be. 
Use the comparison tool to call k.exe, and determine the expectation from the k.exe results. 
Note that tests that result in a dictionary, nested vectors or vectors of symbols will require first adding a known difference in T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\known_differences.txt for Compact representation, with regex 
^\s+|\s+$|\s*\n\s*:\;
before running the comparison tool to determine expectations, if we don't add this known difference, the result will have newlines which is undesired

##### **Test Structure Guidelines**
```k
// ✅ GOOD: Single line test
in_basic.k:
4 _in 1 7 2 4 6 3

// ✅ GOOD: Multiple lines only when necessary
in_with_variables.k:
x: 1 2 3 4 5
4 _in x

// ❌ AVOID: Multiple tests in one file
in_comprehensive.k:
4 _in 1 7 2 4 6 3  // Test 1
3 _in 1 7 2 4 6 3  // Test 2  
10 _in 1 7 2 4 6 3 // Test 3
```

##### **Why This Approach?**
1. **Clear failure attribution**: Each test failure points to specific functionality
2. **Isolated debugging**: Can debug individual test cases without interference
3. **Focused test names**: File names clearly indicate what's being tested
4. **Simplified maintenance**: Easy to update or remove specific test cases
5. **Better test organization**: Clear separation of different test scenarios

##### **Naming Convention for Focused Tests**
- Use descriptive names without redundant prefixes:
  - `in_basic.k` - basic functionality
  - `in_notfound.k` - edge case (not found)
  - `in_scalar.k` - scalar arguments
  - `in_symbols.k` - symbol vector testing
  - `in_strings.k` - string vector testing

### **Test Development Workflow**

#### **Test-First Development**
1. **Write failing test**: Create test that demonstrates the issue. 
2. **Canonical results**: Use the k MCP Server to generate the expected output 
3. **Implement minimal fix**: Get test passing with simplest solution
4. **Refactor**: Clean up implementation while keeping test passing
5. **Add edge cases**: Test boundary conditions and error cases

#### **Test Coverage Guidelines**
- **Core operations**: 100% coverage required
- **Edge cases**: Test all boundary conditions
- **Error paths**: Avoid creating tests that make the the k MCP Server to return an error or timeout
- **Integration**: Test complex expressions combining operations

### **Test Maintenance**

#### **Keeping Tests Current**
- Remove tests for deprecated functionality
- Update tests when behavior changes (obtain results again from the k MCP Server)
- Add tests for new features immediately
- Run the test suite to update T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\results_table.txt after each step of development is completed

#### **Test Data Management**
- Use meaningful test data that covers real scenarios

## **🧪 Test Expectation Modification Notice**

**Before modifying any test expectations in `SimpleTestRunner.cs` or test files, you must get the expected result from the k MCP Server. If the modified expectation differs from k.exe's actual output, you must ask for explicit confirmation before proceeding.**

### **Required Verification Process:**
1. **Run the k MCP Server**: 
2. **Compare results**: Check if K3Sharp output matches k.exe output
3. **If different**: Ask for user confirmation before changing test expectations
4. **Document**: If difference is expected, add entry to `known_differences.txt`

This ensures test expectations remain aligned with the reference k.exe implementation.

#### **Solving discrepancies in test counts**
The test suite has a verification to ensure thare aren't test cases in the file system that are missing from the test suite. If errors are reported then the test cases missing from the test runner need to be added to the test runner or removed:
- [ ] Check if the test cases are duplicates and if they are then remove them from the file system. 
- [ ] If the test cases are not duplicates then use the individual test functionality in the comparison tool to determine the results from using k.exe. 
- [ ] If k.exe succeeds (meaning that either the comparison passed, the comparison failed or the comparison resulted in an error only in K3Sharp) then add the test case to the test runner and use the results from k.exe as the expectation
- [ ] If the comparison tool reports that it was skipped (because of k.exe 32-bit limitation) then add it to the test suite.
- [ ] If the comparison tool reports an error and k.exe failed and timed out, then remove the test from the file system. 

---

#### **Difference Management**
```csharp
// Format: TestName :: Tweak1&Tweak2&Tweak3 :: Notes
format_symbol_string_mixed_vector :: regex:\s+: :: Compact representation
symbol_vector_compact :: regex:\s+: :: Compact representation
```

#### **Known Differences**
- Document all expected differences in `known_differences.txt`
- Categorize differences by purpose (e.g., compact representation, smart integer conversion), not by operation performed (removing spaces, removing nulls, etc.)
- Review differences regularly for potential fixes

#### **Regression Prevention**
- Never reduce k.exe compatibility without explicit reason
- Test that fixes don't break existing functionality
- Maintain comprehensive test coverage

#### **Continuous Validation**
- Automated comparison runs on major changes
- Execute the full test suite regularly, including on minor changes
- Monitor performance regressions

## Test file management
- *.k Test scripts are located in `T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\test_files`
- Never create test files in the base folder
- Always clean up temporary/debug tests when the associated troubleshooting is complete.
- If older temporery/debug tests are found (e.g., from a previous troubleshooting task that was interrupted) then remove them as well.
- When removing tests, remove them both from the test runner and the file system
- Always run the test suite from its own folder T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests
- Always run the comparison tool from its own folder T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison
- The canonical reference for test results is T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\test_results.txt.
- Never delete T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\test_results.txt when cleaning up
- Other files may be used temporarily for outputting test results but they must be cleaned up when the associated troubleshooting is complete.
- The canonical reference for comparison results is T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\comparison_table.txt
- Never delete T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\comparison_table.txt when cleaning up
- Other files may be used temporarily for outputting comparison results but they must be cleaned up when the associated troubleshooting is complete.
