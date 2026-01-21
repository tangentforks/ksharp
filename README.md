# K3 Interpreter Implementation

A C# implementation of the K3 programming language, a high-performance array programming language from the APL family.

## Overview

K3 is version 3 of the K programming language, similar to A+, J, and Q. It's designed for data analysis, algorithmic trading, risk management, and other domains requiring high-performance array operations.

## Current Implementation Status

### ✅ Working Features

#### **Basic Data Types**
- **Integers** (32-bit signed): `0`, `42`, `-7`
- **Long Integers** (64-bit signed): `123456789L`
- **Floating Point Numbers**: `0.0`, `0.17e03`
- **Characters**: `"f"`, `"hello"`
- **Symbols**: `` `f ``, `` `"a symbol" ``

#### **Compound Types**
- **Vectors**: Space-separated or parenthesized/semicolon-separated
  - Integer vectors: `1 2 3 4` or `(1;2;3;4)`
  - Long vectors: `1L 2L 3L`
  - Float vectors: `0.0 1.1 2.2` or `(0.0;1.1;2.2)`
  - Character vectors: `"hello world"` or `("h";"e";"l";"l";"o")`
  - Symbol vectors: `` `a `symbol `vector ``
  - Mixed vectors: `"a" `mixed `type "vector" 0 5 1L 6L 7.7 8.9`

#### **Arithmetic Operations**
- **Basic Operations**: `+`, `-`, `*`, `%` (division)
- **Vector Operations**: Element-wise arithmetic on vectors
- **Scalar-Vector Operations**: Apply scalar operation to vector elements

#### **New Operators (from spec update)**
- **`&`** (minimum): Returns minimum of two arguments
- **`|`** (maximum): Returns maximum of two arguments
- **`<`** (less than): Returns 1 if first < second, otherwise 0
- **`>`** (greater than): Returns 1 if first > second, otherwise 0
- **`=`** (equal): Returns 1 if first == second, otherwise 0
- **`^`** (power): Returns first raised to power of second
- **`!`** (modulus): Returns remainder of division
- **`~`** (negation): Returns 1 for 0, 0 for any other value
- **`,`** (join): Creates vector by joining two arguments

#### **Variables**
- **Weak typing**: Type inferred at assignment time
- **Assignment operator**: `:` (colon)
- **Variable names**: Alphabetic characters, underscores, numbers (not at start)
- **Type reassignment**: Variables can change types on reassignment
- **Global scope**: Variables accessible from anywhere

#### **Functions**
- **Anonymous functions**: `{[param1;param2] body}`
- **Function parameters**: Optional parameter lists in square brackets
- **Function bodies**: Single expressions or blocks
- **Function calls**: `function[arg1;arg2]` syntax
- **Function assignment**: Can assign functions to variables

#### **REPL Interface**
- **Interactive mode**: Read-Eval-Print Loop
- **File execution**: Can run .k files as arguments
- **Error handling**: Basic error reporting

### 🚧 Work in Progress

#### **Function Evaluation**
- **Status**: Functions parse correctly but evaluation needs fixes
- **Issue**: Function calls return `<function>` instead of evaluated results
- **Tests**: 4/31 tests failing due to function evaluation issues
- **Examples needing fixes**:
  ```k3
  {[arg1] arg1+6}[7]      // Should return 13
  add7: {[arg1] arg1+6}
  add7[6]                 // Should return 13
  ```

#### **Variable Scoping**
- **Status**: Basic global variables work
- **Needed**: Local variable scoping within functions
- **Spec requirement**: Local variables should hide global variables

### ❌ Not Yet Implemented

#### **Symbol Table Optimization**
- **Spec requirement**: Global symbol table with reference equality
- **Current**: Basic string comparison
- **Impact**: Performance and memory efficiency

#### **Advanced Vector Operations**
- **Missing**: Some vector-specific operations for new operators
- **Status**: Basic vector-scalar operations work

#### **Error Handling**
- **Current**: Basic exception throwing
- **Needed**: More sophisticated error messages and recovery

#### **REPL Features**
- **Missing**: Exit commands (`\\`, `_exit`)
- **Missing**: Command history and editing features

## Test Coverage

### **Test Results**: 27/31 tests passing (87% success rate)

#### **Passing Tests** (27/31)
- All basic arithmetic operations
- All vector operations
- All new operators (`&`, `|`, `<`, `>`, `=`, `^`, `!`, `~`, `,`)
- Variable assignment and usage
- Type handling (integer, long, float, character, symbol)
- Function parsing (structure is correct)

#### **Failing Tests** (4/31)
- `anonymous_functions.k`: Expected `13`, got `<function>`
- `function_application.k`: Expected `13`, got `<function>`
- `complex_function.k`: Expected `205`, got `''`
- `variable_scoping.k`: Expected `25`, got `10`

## Architecture

### **Core Components**
- **Lexer.cs**: Tokenizes input into tokens
- **Parser.cs**: Recursive descent parser building AST
- **Evaluator.cs**: AST traversal and evaluation
- **K3Value.cs**: Type system and value operations
- **Program.cs**: REPL interface and file execution

### **Data Flow**
1. **Input** → **Lexer** → **Tokens**
2. **Tokens** → **Parser** → **AST**
3. **AST** → **Evaluator** → **Result**
4. **Result** → **Output**

## Building and Running

### **Prerequisites**
- .NET 6.0 or later
- Visual Studio 2022 or compatible C# compiler

### **Build**
```bash
dotnet build
```

### **Run Tests**
```bash
.\run_tests.bat
```

### **Run REPL**
```bash
dotnet run
```

### **Run Script File**
```bash
dotnet run script.k
```

## Next Development Priorities

1. **Fix Function Evaluation** (High Priority)
   - Resolve function call evaluation issues
   - Implement proper parameter binding
   - Fix 4 failing function tests

2. **Implement Variable Scoping** (Medium Priority)
   - Add local variable support in functions
   - Implement scope hiding rules

3. **Optimize Symbol Table** (Low Priority)
   - Implement global symbol table with reference equality
   - Improve memory efficiency

## Contributing

When adding new features:
1. Write test cases first (test-driven development)
2. Implement the feature
3. Ensure all tests pass
4. Update this README with status changes

## Authorship

This K3 interpreter implementation was written by **SWE-1.5** based on a specification and comments provided by **Eusebio Rufian-Zilbermann**.

### Development Approach
- **Test-Driven Development**: Every feature includes comprehensive test coverage
- **Iterative Implementation**: Features built incrementally with validation
- **Code Quality**: Clean, maintainable C# code following best practices
