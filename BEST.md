# K3Sharp Best Practices

This document captures best practices for developing and maintaining K3Sharp, a K3 interpreter implementation in C#.

## **🚨 IMPORTANT: File Access Notice**

**Before attempting to modify this BEST.md file, always check if it is listed in `.gitignore` (and not commented out). If BEST.md is ignored by git, you must first ask the user to temporarily modify `.gitignore` to allow access to this file, and wait for explicit confirmation before proceeding with any modifications.**

This prevents accidental attempts to modify files that are intentionally excluded from version control.

## Table of Contents

1. [Code Development Practices](#code-development-practices)
2. [Error Handling](#error-handling)
3. [Performance Considerations](#performance-considerations)
4. [Specification Compliance](#specification-compliance)
5. [Development Workflow](#development-workflow)
6. [Documentation Standards](#documentation-standards)
7. [Continuous Improvement](#continuous-improvement)

---

## Code Development Practices

### **Code Organization Principles**

#### **Single Responsibility**
- Each class should have one clear, well-defined purpose
- Methods should be focused on a single task
- Avoid God classes or overly complex methods

#### **Naming Conventions**
- **Classes**: PascalCase (e.g., `VectorValue`, `ComparisonRunner`)
- **Methods**: PascalCase with descriptive names (e.g., `PerformTypeConversion`)
- **Variables**: camelCase with meaningful names (e.g., `rightVector`)
- **Constants**: UPPER_SNAKE_CASE (e.g., `DEFAULT_TOLERANCE`)
- **Traditional Names**: Names should use K traditional naming described in spec instead of naming conventions from other languages, e.g., "over" instead of "reduce", "monadic" instead of "unary", "dyadic" instead of "binary", "character vector" (possibly abbreviated to "charvec") instead of string (which should be reserved for referring to string literals)

#### **Method Design**
```csharp
// Good: Clear purpose, descriptive name, proper documentation
private K3Value ConvertCharacterVectorToSymbol(VectorValue charVector)
{
    // Implementation with clear logic
}

// Avoid: Vague names, unclear purpose
private K3Value Process(K3Value val)
{
    // Unclear what this does
}
```

### **Code Quality Standards**

#### **Readability First**
- Write code that explains itself
- Use meaningful variable names
- Add comments for complex logic, not obvious operations
- Keep methods under 50 lines when possible

#### **Consistent Patterns**
- Follow established patterns in the codebase
- Use similar error handling approaches throughout
- Maintain consistent formatting and structure

#### **Simplify**
- Avoid unnecessary complexity
- Avoid special cases whenever possible, check if the general case can handle all the cases and if it can, use it instead of adding special cases.
- If the general case is updated, check if there are any special cases that can be eliminated. If there are, remove them.

#### **Proactive Optimization**
- Regularly identify and remove any unused or redundant code.

#### **Quality Prevents Noise**
- Compiler warnings do help prevent potential problems
- Warnings have lower priority than errors
- We can tolerate warnings during troubleshooting
- Warnings must be addressed and fixed before a task is finished 
- Having many warnings should be avoided because they make it harder to analyze compiler output and focus on the actual problems

### **Implementing functions described in kref and kusr**
- The documents kref* and kusr* are used as a reference description of how many verbs operate.
These documents use bracket notation 
    verb\[arguments\]
However for implementation purposes these verbs should be implemented first using apply notation
    verb . arguments
We have already implemented logic for brackets being handled as an alternative notation for apply, this logic should handle the change to using bracket notation automatically, and all that should be needed is creating test cases that confirm it

---

### **Special Value Handling**

#### **Null Values**
- Preserve K3 null semantics throughout operations
- Handle null propagation correctly
- Document null behavior in method comments

#### **Infinity and Special Numbers**
- Implement proper overflow/underflow behavior
- Handle positive/negative infinity correctly
- Test edge cases around numeric limits

---

## Error Handling

### **Error Handling Principles**

#### **Robust Error Handling**
- Handle errors explicitly
- Avoid swallowing exceptions
- Document error behavior in method comments

#### **Error Types**
- **ArgumentException**: Thrown when input is invalid
- **InvalidOperationException**: Thrown when operation is invalid
- **NotSupportedException**: Thrown when operation is not supported

---

## Performance Considerations

### **Efficient Coding Practices**

#### **Vector Operations**
```csharp
// Good: Pre-allocate capacity
var result = new List<K3Value>(vector.Elements.Count);

// Avoid: Repeated allocations
List<K3Value> result = new List<K3Value>();
foreach (var element in vector.Elements)
{
    result.Add(Process(element)); // Multiple reallocations
}
```

#### **Memory Management**
- Minimize object allocations in hot paths
- Use object pooling for frequently created objects
- Prefer structs for small, frequently used data structures

#### **Algorithmic Efficiency**
- Choose appropriate data structures for operations
- Consider time complexity for vector operations
- Implement fast paths for common cases

#### **Profiling Guidelines**
- Profile before optimizing
- Focus on actual bottlenecks, not perceived ones
- Measure impact of optimizations

#### **Benchmarking**
- Create benchmarks for critical operations
- Track performance over time
- Set performance regression alerts

---

## Specification Compliance

### **K3 Specification Adherence**

#### **Critical Requirements**
- **Character vectors as leaf elements**: Essential for FORM/FORMAT operations
- **Type conversion rules**: Exact implementation of K3 type system
- **Operator semantics**: Precise behavior matching K3 reference
- **Special values**: Correct handling of null, infinity, etc.

#### **Compliance Verification**
```csharp
// Document spec compliance with comments
// According to K3 spec: single character results should be enlisted
if (str.Length == 1)
{
    return new VectorValue(new List<K3Value> { new CharacterValue(str) }, "string");
}
```

#### **Specification References**
- Keep `speck#.txt` as authoritative reference
- Document deviations with clear reasoning
- Regular comparison with k.exe for validation

### **Implementation Standards**

#### **Type System Compliance**
- Implement exact K3 type promotion rules
- Handle special values according to specification
- Maintain proper type semantics

#### **Operator Behavior**
- Follow K3 operator precedence exactly
- Implement proper associativity rules
- Handle edge cases as specified

---

## Development Workflow

### **Pre-Commit Validation**
Use the workflow skill at `.windsurf/workflows/pre_commit_checklist.md` for systematic pre-commit validation:
- Documentation updates
- Test cleanup and validation  
- Comparison testing
- Build validation
- Final checks

### **Manual Pre-Commit Checklist for T:\_src\github.com\ERufian\ksharp**

- [ ] Update recent insights into T:\_src\github.com\ERufian\vibe-docs\ksharp\K3CSharp_implementation_details.md
- [ ] Clean up any test case outside T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\TestScripts, either move it to T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\TestScripts and add it to the test runner, or remove it from the file system if it is a temporary or debug test case that is not needed
- [ ] Clean up the file system by removing other temporary and debug output files
- [ ] Run the full test suite at T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\ to update T:\_src\github.com\ERufian\ksharp\K3CSharp.Tests\results_table.txt 
- [ ] Run a full comparison with k.exe in 32-bit mode with the tool at T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\ to update T:\_src\github.com\ERufian\ksharp\K3CSharp.Comparison\results_table.txt
- [ ] The full comparison can take a while to run. Either allow sufficient time for it to complete or run it in the background and monitor that it is producing output regularly
- [ ] Update T:\_src\github.com\ERufian\ksharp\README.md by adding new information, removing obsolete information and updating statistics. Preserve sections related to installation instructions and authorship.
- [ ] Ensure this BEST.md is not commented out in .gitignore. Ask me to remove the comment mark if present.

### **Commit Best Practices**

#### **Commit Message Format**
```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

#### **Types**
- **feat**: New feature or functionality
- **fix**: Bug fix or regression
- **docs**: Documentation changes
- **refactor**: Code refactoring without functional changes
- **test**: Test additions or modifications
- **chore**: Maintenance tasks

#### **Examples**
```
fix(format): Handle character vectors as leaf elements in FORM operations

- Implement proper character vector handling in ConvertToSymbol
- Add known difference entry for symbol vector spacing
- Update test expectations to match K3Sharp output

Fixes #123

feat(testing): Add comprehensive test coverage for _in function

- Add test cases for basic find functionality
- Include edge cases (not found, scalar arguments)
- Update SimpleTestRunner with new test expectations
```

### **Code Review Guidelines**

#### **Review Checklist**
- [ ] Specification compliance verified
- [ ] Test coverage adequate
- [ ] Performance considered
- [ ] Error handling robust
- [ ] Documentation updated
- [ ] No breaking changes without justification

#### **Review Focus Areas**
- Correctness of implementation
- Adherence to project patterns
- Performance implications
- Test quality and coverage
- Documentation clarity

---

## Documentation Standards

### **Code Documentation**

#### **XML Documentation**
```csharp
/// <summary>
/// Converts a character vector to a symbol value according to K3 specification.
/// Character vectors are treated as leaf elements in FORM operations.
/// </summary>
/// <param name="charVector">The character vector to convert</param>
/// <returns>A symbol value containing the concatenated characters</returns>
/// <exception cref="ArgumentException">Thrown when input is not a character vector</exception>
private K3Value ConvertCharacterVectorToSymbol(VectorValue charVector)
{
    // Implementation
}
```

#### **Inline Comments**
- Document complex algorithms
- Explain non-obvious business logic
- Reference specification sections
- Note performance considerations

### **Project Documentation**

#### **README Maintenance**
- Keep statistics current
- Update feature status
- Document recent changes
- Provide clear build/run instructions

#### **Technical Documentation**
- Update implementation details for major changes
- Document architectural decisions
- Maintain API documentation
- Keep troubleshooting guides current

---

## Continuous Improvement

### **Regular Reviews**

#### **Code Quality**
- Monthly code quality assessments
- Performance regression monitoring
- Test coverage analysis
- Documentation completeness checks

#### **Process Improvement**
- Review development workflow efficiency
- Identify bottlenecks in build/test process
- Evaluate tooling effectiveness
- Gather team feedback

### **Learning and Adaptation**

#### **Stay Current**
- Monitor K3 specification changes
- Track .NET best practices
- Learn from similar projects
- Attend relevant conferences/webinars

#### **Knowledge Sharing**
- Document lessons learned
- Share implementation insights
- Create troubleshooting guides
- Mentor new contributors

---

## Conclusion

These best practices represent the collective experience gained while developing K3Sharp. Following these guidelines ensures:

- **Quality**: Robust, well-tested code
- **Maintainability**: Clean, understandable structure
- **Compliance**: Adherence to K3 specification
- **Performance**: Efficient implementation
- **Collaboration**: Effective team development

Regular review and updates of these practices will ensure continued code quality and project success.

---

## References to Existing Rules

This document complements the existing rules in `.windsurf/rules/`:

- **Testing Strategy**: See `testing-strategy.md` for comprehensive testing guidelines
- **Terminology**: See `terminology.md` for K-specific terminology differences  
- **Documentation Locations**: See `locating-information.md` for specification and reference locations
- **Pre-commit Workflow**: See `pre_commit_checklist.md` for systematic validation procedures

These rules should be considered authoritative for their respective domains and are not duplicated here to avoid maintenance overhead.
