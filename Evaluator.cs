using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace K3CSharp
{
    public class Evaluator
    {
        private Dictionary<string, K3Value> globalVariables = new Dictionary<string, K3Value>();
        private Dictionary<string, K3Value> localVariables = new Dictionary<string, K3Value>();
        private Dictionary<string, int> symbolTable = new Dictionary<string, int>();
        public bool isInFunctionCall = false; // Track if we're evaluating a function call
        public static int floatPrecision = 7; // Default precision for floating point display
        
        // K Tree for global namespace management
        private KTree kTree = new KTree();
        
        // Reference to the current function being executed (for AST caching)
        public FunctionValue currentFunctionValue = null;

        // Reference to parent evaluator for global scope access
        private Evaluator parentEvaluator = null;

        // Public methods for K tree operations
        public K3Value GetCurrentBranch()
        {
            return kTree.CurrentBranch;
        }
        
        public void SetCurrentBranch(string branchPath)
        {
            kTree.CurrentBranch = new SymbolValue(branchPath);
        }
        
        public void SetParentBranch()
        {
            kTree.CurrentBranch = kTree.GetParentBranch();
        }
        
        /// <summary>
        /// Resets the K tree to its default state (for testing purposes)
        /// </summary>
        public void ResetKTree()
        {
            kTree = new KTree();
        }

        public K3Value Evaluate(ASTNode node)
        {
            if (node == null)
                return new NullValue();
                
            return EvaluateNode(node);
        }

        private K3Value EvaluateNode(ASTNode node)
        {
            switch (node.Type)
            {
                case ASTNodeType.Literal:
                    return node.Value;

                case ASTNodeType.Variable:
                    var name = node.Value is SymbolValue symbol ? symbol.Value : node.Value.ToString();
                    return GetVariable(name);

                case ASTNodeType.Assignment:
                    {
                        var assignName = node.Value is SymbolValue assignmentSym ? assignmentSym.Value : node.Value.ToString();
                        var value = Evaluate(node.Children[0]);
                        SetVariable(assignName, value); // Use local variables for regular assignments
                        return value; // Return the assigned value
                    }

                case ASTNodeType.GlobalAssignment:
                    {
                        var globalAssignName = node.Value is SymbolValue globalAssignmentSym ? globalAssignmentSym.Value : node.Value.ToString();
                        var globalValue = Evaluate(node.Children[0]);
                        SetGlobalVariable(globalAssignName, globalValue);
                        return globalValue; // Return the assigned value
                    }

                case ASTNodeType.BinaryOp:
                    return EvaluateBinaryOp(node);

                case ASTNodeType.Vector:
                    return EvaluateVector(node);

                case ASTNodeType.Function:
                    return EvaluateFunction(node);

                case ASTNodeType.FunctionCall:
                    return EvaluateFunctionCall(node);

                case ASTNodeType.Block:
                    return EvaluateBlock(node);

                case ASTNodeType.FormSpecifier:
                    // {} form specifier - return a special value that will be handled in binary form operations
                    return new SymbolValue("{}");

                default:
                    throw new Exception($"Unknown AST node type: {node.Type}");
            }
        }

        private bool IsBuiltInOperator(string operatorName)
        {
            // List of built-in operators that can be used as functions
            return operatorName == "+" || operatorName == "-" || operatorName == "*" || operatorName == "%" ||
                   operatorName == "^" || operatorName == "<" || operatorName == ">" || operatorName == "=" ||
                   operatorName == "," || operatorName == "." || operatorName == "!" || operatorName == "@" ||
                   operatorName == "#" || operatorName == "_" || operatorName == "?" || operatorName == "$";
        }

        private bool IsColon(K3Value value)
        {
            // Check if the value represents a colon (:)
            return value is SymbolValue symbol && symbol.Value == ":";
        }
        
        private K3Value GetVariable(string variableName)
        {
            // Check if this is a K tree dotted notation variable
            if (variableName.Contains("."))
            {
                var kTreeValue = kTree.GetValue(variableName);
                if (kTreeValue != null)
                {
                    return kTreeValue;
                }
            }
            
            // Check if this is a relative path in the current K tree branch
            if (!variableName.Contains("."))
            {
                var currentBranch = kTree.CurrentBranch.Value;
                if (!string.IsNullOrEmpty(currentBranch))
                {
                    // Try relative path from current branch
                    var relativePath = currentBranch + "." + variableName;
                    var kTreeValue = kTree.GetValue(relativePath);
                    if (kTreeValue != null)
                    {
                        return kTreeValue;
                    }
                }
                else
                {
                    // Debug: current branch is empty (root), so no relative resolution
                    // This means we should fall back to regular variable lookup
                }
                
                // Also check function's associated K tree for relative paths
                if (string.IsNullOrEmpty(currentBranch) && currentFunctionValue != null && currentFunctionValue.AssociatedKTree != null)
                {
                    var functionKTreeValue = currentFunctionValue.AssociatedKTree.GetValue(variableName);
                    if (functionKTreeValue != null)
                    {
                        return functionKTreeValue;
                    }
                }
            }
            
            // Check local scope first
            if (localVariables.TryGetValue(variableName, out var localValue))
            {
                return localValue;
            }
            
            // Check global scope
            if (globalVariables.TryGetValue(variableName, out var globalValue))
            {
                return globalValue;
            }
            
            // Check if this is a built-in operator that can be used as a function
            if (IsBuiltInOperator(variableName))
            {
                return new SymbolValue(variableName);
            }
            
            // Check parent evaluator (for nested function calls)
            if (parentEvaluator != null)
            {
                return parentEvaluator.GetVariable(variableName);
            }
            
            throw new Exception($"Undefined variable: {variableName}");
        }
        
        private K3Value SetVariable(string variableName, K3Value value)
        {
            // Check if this is a K tree dotted notation variable
            if (variableName.Contains("."))
            {
                if (kTree.SetValue(variableName, value))
                {
                    return value;
                }
                // If K tree assignment fails, fall back to local assignment
            }
            
            // Local assignment - always set in local scope
            localVariables[variableName] = value;
            return value;
        }

        private K3Value SetGlobalVariable(string variableName, K3Value value)
        {
            // Check if this is a K tree dotted notation variable
            if (variableName.Contains("."))
            {
                if (kTree.SetValue(variableName, value))
                {
                    return value;
                }
                // If K tree assignment fails, fall back to global assignment
            }
            
            // Global assignment - always set in global scope
            if (parentEvaluator != null)
            {
                // If we have a parent, set the global variable there
                return parentEvaluator.SetGlobalVariable(variableName, value);
            }
            else
            {
                // We're the root evaluator, set in our global scope
                globalVariables[variableName] = value;
                return value;
            }
        }

        private K3Value EvaluateLiteral(ASTNode node)
        {
            return node.Value;
        }

        private K3Value EvaluateBinaryOp(ASTNode node)
        {
            var op = node.Value as SymbolValue;
            if (op == null) throw new Exception("Binary operator must have a symbol value");

            // Handle unary operators (which are implemented as binary ops with one child)
            if (node.Children.Count == 1)
            {
                var operand = Evaluate(node.Children[0]);
                return op.Value switch
                {
                    "-" => UnaryMinus(operand),
                    "+" => Transpose(operand),
                    "*" => First(operand),
                    "%" => Reciprocal(operand),
                    "&" => Where(operand),
                    "|" => Reverse(operand),
                    "TYPE" => GetTypeCode(operand),
                    "STRING_REPRESENTATION" => StringRepresentation(operand),
                    "." => Make(operand),
                    "<" => GradeUp(operand),
                    ">" => GradeDown(operand),
                    "^" => Shape(operand),
                    "!" => Enumerate(operand),
                    "," => Enlist(operand),
                    "#" => Count(operand),
                    "_" => Floor(operand),
                    "?" => Unique(operand),
                    "=" => Group(operand),
                    "$" => Format(operand),
                    "DIRECTORY" => GetCurrentBranch(),
                    "NEGATE" => operand is SymbolValue || (operand is VectorValue vec && vec.Elements.All(e => e is SymbolValue))
                    ? AttributeHandle(operand)
                    : LogicalNegate(operand),
                    "@" => Atom(operand),
                    "~" => AttributeHandle(operand),
                    "_log" => MathLog(operand),
                    "_exp" => MathExp(operand),
                    "_abs" => MathAbs(operand),
                    "_sqr" => MathSqr(operand),
                    "_sqrt" => MathSqrt(operand),
                    "_floor" => MathFloor(operand),
                    "_sin" => MathSin(operand),
                    "_cos" => MathCos(operand),
                    "_tan" => MathTan(operand),
                    "_asin" => MathAsin(operand),
                    "_acos" => MathAcos(operand),
                    "_atan" => MathAtan(operand),
                    "_sinh" => MathSinh(operand),
                    "_cosh" => MathCosh(operand),
                    "_tanh" => MathTanh(operand),
                    "_dot" => MathDot(operand),
                    "_mul" => MathMul(operand),
                    "_inv" => MathInv(operand),
                    "_t" => TimeFunction(operand),
                    "_draw" => DrawFunction(operand),
                    "_in" => InFunction(operand),
                    "_bin" => BinFunction(operand),
                    "_binl" => BinlFunction(operand),
                    "_lin" => LinFunction(operand),
                    "_lsq" => LsqFunction(operand),
                    "_gtime" => GtimeFunction(operand),
                    "_ltime" => LtimeFunction(operand),
                    "_vs" => VsFunction(operand),
                    "_sv" => SvFunction(operand),
                    "_ss" => SsFunction(operand),
                    "_ci" => CiFunction(operand),
                    "_ic" => IcFunction(operand),
                    "_do" => DoFunction(operand),
                    "_while" => WhileFunction(operand),
                    "_if" => IfFunction(operand),
                    "_d" => GetCurrentBranch(),
                    "do" => DoFunction(operand),
                    "while" => WhileFunction(operand),
                    "if" => IfFunction(operand),
                    "_goto" => GotoFunction(operand),
                    "_exit" => ExitFunction(operand),
                    "MIN" => operand, // Identity operation for unary min
                    "MAX" => operand, // Identity operation for unary max
                    "ADVERB_SLASH" => operand, // Return operand as-is for now
                    "ADVERB_BACKSLASH" => operand, // Return operand as-is for now
                    "ADVERB_TICK" => operand, // Return operand as-is for now
                    _ => throw new Exception($"Unknown unary operator: {op.Value}")
                };
            }

            // Handle binary operators
            if (node.Children.Count == 2)
            {
                // Special handling for colon operator to avoid evaluating left side as variable lookup
                if (op.Value.ToString() == ":")
                {
                    var leftNode = node.Children[0];
                    var rightValue = Evaluate(node.Children[1]);
                    
                    // For assignment, the left side should be treated as a variable name, not evaluated
                    if (leftNode.Type == ASTNodeType.Variable)
                    {
                        var variableName = leftNode.Value is SymbolValue symbol ? symbol.Value : leftNode.Value.ToString();
                        return Assignment(variableName, rightValue);
                    }
                    else
                    {
                        // If left side is not a variable, evaluate it normally
                        var leftValue = Evaluate(leftNode);
                        return ColonOperator(leftValue, rightValue);
                    }
                }
                
                var left = Evaluate(node.Children[0]);
                var right = Evaluate(node.Children[1]);

                return op.Value.ToString() switch
                    {
                        "+" => Plus(left, right),
                        "-" => Minus(left, right),
                        "*" => Times(left, right),
                        "%" => Divide(left, right),
                        "^" => Power(left, right),
                        "!" => ModRotate(left, right),
                        "&" => Min(left, right),
                        "|" => Max(left, right),
                        "<" => LessThan(left, right),
                        ">" => More(left, right),
                        "=" => Match(left, right),
                        "," => Join(left, right),
                        "#" => Take(left, right),
                        "_" => FloorBinary(left, right),
                        "@" => AtIndex(left, right),
                        "." => DotApply(left, right),
                        "$" => Format(left, right),
                        "::" => GlobalAssignment(left, right),
                        "ADVERB_SLASH" => Over(new SymbolValue("+"), left, right),
                        "ADVERB_BACKSLASH" => Scan(new SymbolValue("+"), left, right),
                        "ADVERB_TICK" => Each(left, right),
                        "_in" => In(left, right),
                        "_bin" => Bin(left, right),
                        "_binl" => Binl(left, right),
                        "_lin" => Lin(left, right),
                        "?" => Find(left, right),
                        "TYPE" => GetType(left, right),
                        _ => throw new Exception($"Unknown binary operator: {op.Value}")
                    };
            }
            // Handle 3-argument adverb structure: ADVERB(verb, left, right)
            else if (node.Children.Count == 3 && 
                    (op.Value.ToString() == "ADVERB_SLASH" || op.Value.ToString() == "ADVERB_BACKSLASH" || op.Value.ToString() == "ADVERB_TICK" ||
                     op.Value.ToString() == "ADVERB_SLASH_COLON" || op.Value.ToString() == "ADVERB_BACKSLASH_COLON" || op.Value.ToString() == "ADVERB_TICK_COLON"))
            {
                var verb = Evaluate(node.Children[0]);
                var left = Evaluate(node.Children[1]);
                var right = Evaluate(node.Children[2]);

                return op.Value.ToString() switch
                {
                    "ADVERB_SLASH" => Over(verb, left, right),
                    "ADVERB_BACKSLASH" => Scan(verb, left, right),
                    "ADVERB_TICK" => Each(verb, left, right),
                    "ADVERB_SLASH_COLON" => EachRight(verb, left, right),
                    "ADVERB_BACKSLASH_COLON" => EachLeft(verb, left, right),
                    "ADVERB_TICK_COLON" => EachPrior(verb, left, right),
                    _ => throw new Exception($"Unknown adverb: {op.Value}")
                };
            }
            else if (op.Value.ToString() == "ADVERB_CHAIN")
            {
                return EvaluateAdverbChain(node);
            }
            else
            {
                throw new Exception($"Binary operator must have exactly 2 children, got {node.Children.Count}");
            }
        }
        
        private K3Value EvaluateVector(ASTNode node)
        {
            var elements = new List<K3Value>();
            foreach (var child in node.Children)
            {
                elements.Add(Evaluate(child));
            }
            // Use "mixed" creation method for empty vectors created from parentheses
            return new VectorValue(elements, elements.Count == 0 ? "mixed" : "standard");
        }
        private K3Value EvaluateFunction(ASTNode node)
        {
            // The function value should already be stored in node.Value from the parser
            var functionValue = node.Value as FunctionValue;
            if (functionValue == null)
            {
                throw new Exception("Function node must contain a FunctionValue");
            }
            
            // According to updated spec: niladic functions should remain as functions and not be
            // automatically evaluated. They should only be evaluated when explicitly applied.
            // All functions (including niladic) should return the function object.
            return functionValue;
        }

        private K3Value EvaluateFunctionCall(ASTNode node)
        {
            if (node.Children.Count < 1)
            {
                throw new Exception("Function call requires a function");
            }

            var functionNode = node.Children[0];
            var arguments = new List<K3Value>();
            
            for (int i = 1; i < node.Children.Count; i++)
            {
                arguments.Add(Evaluate(node.Children[i]));
            }

            // Handle variable function calls first (to avoid evaluating built-in functions as variables)
            if (functionNode.Type == ASTNodeType.Variable)
            {
                // Variable function call: functionName[args]
                var functionName = functionNode.Value is SymbolValue symbol ? symbol.Value : functionNode.Value.ToString();
                return CallVariableFunction(functionName, arguments);
            }

            // First evaluate the left side to see if it's a vector or dictionary
            var leftValue = Evaluate(functionNode);
            
            // Check if this should be treated as indexing instead of function call
            if (leftValue is VectorValue || leftValue is DictionaryValue)
            {
                // This is indexing: vector[index] or dictionary[index]
                if (arguments.Count != 1)
                {
                    throw new Exception("Indexing requires exactly one argument");
                }
                return VectorIndex(leftValue, arguments[0]);
            }

            // Handle function calls differently based on the function node type
            if (functionNode.Type == ASTNodeType.Function)
            {
                // Direct function call: {[params] body}[args]
                return CallDirectFunction(functionNode, arguments);
            }
            else
            {
                // Evaluate the function expression and call it
                var function = leftValue;
                
                if (function is FunctionValue functionValue)
                {
                    // Create a temporary AST node for the function to reuse CallDirectFunction
                    var tempFunctionNode = new ASTNode(ASTNodeType.Function);
                    tempFunctionNode.Value = functionValue;
                    return CallDirectFunction(tempFunctionNode, arguments);
                }
                else if (function.Type == ValueType.Symbol)
                {
                    var functionName = (function as SymbolValue)?.Value;
                    if (functionName == null)
                    {
                        throw new Exception("Invalid function name");
                    }
                    return CallVariableFunction(functionName, arguments);
                }
                
                throw new Exception($"Cannot call non-function: {function.Type}");
            }
        }

        private K3Value CreateProjectedFunction(FunctionValue originalFunction, List<K3Value> providedArguments)
        {
            // Create a new function with reduced valence
            var remainingParameters = originalFunction.Parameters.Skip(providedArguments.Count).ToList();
            var projectedBody = GenerateProjectedBody(originalFunction, providedArguments);
            
            return new FunctionValue(projectedBody, remainingParameters);
        }

        private string GenerateProjectedBody(FunctionValue originalFunction, List<K3Value> providedArguments)
        {
            // For a simpler implementation, we'll create a closure-like approach
            // Store the provided arguments and create a function that takes the remaining ones
            
            if (originalFunction.Parameters.Count <= providedArguments.Count)
            {
                // No remaining parameters, just evaluate the original function
                return originalFunction.BodyText;
            }
            
            // Create a new function body with argument substitution
            var bodyText = originalFunction.BodyText;
            
            // Substitute provided arguments in the body
            for (int i = 0; i < providedArguments.Count && i < originalFunction.Parameters.Count; i++)
            {
                var paramName = originalFunction.Parameters[i];
                var argValue = providedArguments[i].ToString();
                
                // Replace parameter name with its value in the body
                bodyText = bodyText.Replace(paramName, argValue);
            }
            
            return bodyText;
        }

        private K3Value EvaluateAdverbChain(ASTNode node)
        {
            if (node.Children.Count < 2)
            {
                throw new Exception("Adverb chain requires at least an operand and one adverb");
            }
            
            var operand = Evaluate(node.Children[0]);
            var adverbs = new List<string>();
            
            // Extract adverbs from the remaining children
            for (int i = 1; i < node.Children.Count; i++)
            {
                var adverbNode = node.Children[i];
                if (adverbNode.Value is SymbolValue adverbSymbol)
                {
                    adverbs.Add(adverbSymbol.Value);
                }
            }
            
            // Apply adverbs in reverse order (right-to-left evaluation)
            K3Value result = operand;
            for (int i = adverbs.Count - 1; i >= 0; i--)
            {
                var adverb = adverbs[i];
                switch (adverb)
                {
                    case "ADVERB_SLASH":
                        result = ApplyAdverbSlash(result);
                        break;
                    case "ADVERB_BACKSLASH":
                        result = ApplyAdverbBackslash(result);
                        break;
                    case "ADVERB_TICK":
                        result = ApplyAdverbTick(result);
                        break;
                    default:
                        throw new Exception($"Unknown adverb in chain: {adverb}");
                }
            }
            
            return result;
        }

        private K3Value ApplyAdverbSlash(K3Value operand)
        {
            // For now, just return the operand (reduce needs a verb and data)
            // In a full implementation, this would need to handle the verb/data distinction
            return operand;
        }

        private K3Value ApplyAdverbBackslash(K3Value operand)
        {
            // For now, just return the operand (scan needs a verb and data)
            // In a full implementation, this would need to handle the verb/data distinction
            return operand;
        }

        private K3Value ApplyAdverbTick(K3Value operand)
        {
            // For now, just return the operand (each needs a verb and data)
            // In a full implementation, this would need to handle the verb/data distinction
            return operand;
        }

        private K3Value CallDirectFunction(ASTNode functionNode, List<K3Value> arguments)
        {
            var functionValue = functionNode.Value as FunctionValue;
            if (functionValue == null)
            {
                throw new Exception("Function node must contain a FunctionValue");
            }
            
            var parameters = functionValue.Parameters;
            var bodyText = functionValue.BodyText;
            
            // Vector argument unpacking: if we have 1 vector argument but need multiple parameters, unpack it
            // Only unpack if the vector has multiple elements (for single-param functions, keep as is)
            if (arguments.Count == 1 && parameters.Count > 1 && arguments[0] is VectorValue vectorArg && vectorArg.Elements.Count > 1)
            {
                var unpackedArgs = new List<K3Value>();
                foreach (var element in vectorArg.Elements)
                {
                    unpackedArgs.Add(element);
                }
                arguments = unpackedArgs;
            }
            
            // Check for projection: fewer arguments than expected valence
            if (arguments.Count < parameters.Count)
            {
                return CreateProjectedFunction(functionValue, arguments);
            }
            
            if (arguments.Count != parameters.Count)
            {
                throw new Exception($"Function expects {parameters.Count} arguments, got {arguments.Count}");
            }
            
            // Create a new evaluator scope for this function call
            var functionEvaluator = new Evaluator();
            functionEvaluator.parentEvaluator = this; // Set parent for global access
            
            // Copy global variables to function scope
            foreach (var kvp in globalVariables)
            {
                functionEvaluator.globalVariables[kvp.Key] = kvp.Value;
            }
            
            // Copy local variables to function scope (for nested functions)
            foreach (var kvp in localVariables)
            {
                functionEvaluator.localVariables[kvp.Key] = kvp.Value;
            }
            
            // Bind parameters to arguments (in local scope)
            for (int i = 0; i < parameters.Count; i++)
            {
                functionEvaluator.SetVariable(parameters[i], arguments[i]);
            }
            
            // Set the associated K tree for anonymous functions
            functionEvaluator.kTree = functionValue.AssociatedKTree; // Pass the associated K tree
            
            // Bind parameters to arguments (in local scope)
            for (int i = 0; i < parameters.Count; i++)
            {
                functionEvaluator.SetVariable(parameters[i], arguments[i]);
            }
            
            // Set the current function value for AST caching optimization
            functionEvaluator.currentFunctionValue = functionValue;
            
            // Execute the function body using recursive text evaluation
            return ExecuteFunctionBody(bodyText, functionEvaluator, functionValue.PreParsedTokens);
        }

        private K3Value ExecuteFunctionBody(string bodyText, Evaluator functionEvaluator, List<Token> preParsedTokens = null)
        {
            if (string.IsNullOrWhiteSpace(bodyText))
            {
                return new IntegerValue(0); // Empty function result
            }
            
            try
            {
                ASTNode ast;
                
                // Try to get cached AST from the function value if available
                // This is a performance optimization to avoid re-parsing the same function
                if (functionEvaluator.currentFunctionValue != null)
                {
                    ast = functionEvaluator.currentFunctionValue.GetCachedAst();
                }
                else
                {
                    // Fallback to parsing from text (deferred validation per spec)
                    if (preParsedTokens != null && preParsedTokens.Count > 0)
                    {
                        var parser = new Parser(preParsedTokens, bodyText);
                        ast = ParseFunctionBodyStatements(parser, bodyText);
                    }
                    else
                    {
                        // For complex function bodies with nested functions, use enhanced parsing
                        if (bodyText.Contains("{[") || bodyText.Contains("::"))
                        {
                            // Use enhanced parsing for complex function bodies
                            // Try manual parsing first, fall back to dot execute if needed
                            try
                            {
                                var lexer = new Lexer(bodyText);
                                var tokens = lexer.Tokenize();
                                var parser = new Parser(tokens, bodyText);
                                ast = ParseFunctionBodyStatements(parser, bodyText);
                            }
                            catch
                            {
                                // If parsing fails, use dot execute as a robust fallback
                                // This preserves variable context and handles complex nested scenarios
                                var chars = bodyText.Select(c => (K3Value)new CharacterValue(c.ToString())).ToList();
                                var charVector = new VectorValue(chars, "standard");
                                return Make(charVector);
                            }
                        }
                        else
                        {
                            var lexer = new Lexer(bodyText);
                            var tokens = lexer.Tokenize();
                            var parser = new Parser(tokens, bodyText);
                            ast = ParseFunctionBodyStatements(parser, bodyText);
                        }
                    }
                }
                
                return functionEvaluator.Evaluate(ast);
            }
            catch (Exception ex)
            {
                // Runtime validation - function body errors are caught here (per spec)
                throw new Exception($"Function execution error: {ex.Message}", ex);
            }
        }
        
        private ASTNode ParseFunctionBodyStatements(Parser parser, string bodyText)
        {
            // For function bodies, we need to handle multiple statements separated by semicolons or newlines
            // The main parser.Parse() method should handle this correctly for function bodies
            try
            {
                return parser.Parse();
            }
            catch (Exception ex)
            {
                // If parsing fails, it might be due to nested function definitions
                // Let's try a more robust approach by parsing the function body manually
                return ParseFunctionBodyManually(bodyText);
            }
        }
        
        private ASTNode ParseFunctionBodyManually(string bodyText)
        {
            // Manual parsing for function bodies with nested functions
            // This is a simplified version that focuses on the core issue
            var lexer = new Lexer(bodyText);
            var tokens = lexer.Tokenize();
            var statements = new List<ASTNode>();
            
            int current = 0;
            while (current < tokens.Count)
            {
                var token = tokens[current];
                
                // Skip whitespace and newlines
                if (token.Type == TokenType.NEWLINE || token.Type == TokenType.SEMICOLON)
                {
                    current++;
                    continue;
                }
                
                // Check for assignment: variable : expression
                if (token.Type == TokenType.SYMBOL && current + 1 < tokens.Count && tokens[current + 1].Type == TokenType.ASSIGNMENT)
                {
                    var varName = token.Lexeme;
                    current += 2; // Skip variable and assignment
                    
                    // Parse the right side of the assignment
                    var rightSide = ParseRightSide(tokens, ref current);
                    
                    if (rightSide != null)
                    {
                        var assignment = ASTNode.MakeAssignment(varName, rightSide);
                        statements.Add(assignment);
                    }
                }
                else
                {
                    // For now, just skip unknown tokens to avoid crashes
                    current++;
                }
            }
            
            // Create a block node for the function body
            var block = new ASTNode(ASTNodeType.Block);
            foreach (var statement in statements)
            {
                block.Children.Add(statement);
            }
            
            return block;
        }
        
        private ASTNode ParseRightSide(List<Token> tokens, ref int current)
        {
            if (current >= tokens.Count) return null;
            
            var token = tokens[current];
            
            // Check for function definition: {[params] body}
            if (token.Type == TokenType.LEFT_BRACE)
            {
                return ParseFunctionDefinitionFromTokens(tokens, ref current);
            }
            
            // Parse as regular expression
            return ParseExpressionFromTokens(tokens, ref current);
        }
        
        private ASTNode ParseFunctionDefinitionFromTokens(List<Token> tokens, ref int current)
        {
            // Parse function definition: {[params] body}
            if (current >= tokens.Count || tokens[current].Type != TokenType.LEFT_BRACE)
                return null;
                
            current++; // Skip LEFT_BRACE
            
            var parameters = new List<string>();
            
            // Check for parameter list
            if (current < tokens.Count && tokens[current].Type == TokenType.LEFT_BRACKET)
            {
                current++; // Skip LEFT_BRACKET
                
                // Parse parameters
                while (current < tokens.Count && tokens[current].Type != TokenType.RIGHT_BRACKET)
                {
                    if (tokens[current].Type == TokenType.IDENTIFIER)
                    {
                        parameters.Add(tokens[current].Lexeme);
                    }
                    current++;
                    
                    // Skip semicolons between parameters
                    if (current < tokens.Count && tokens[current].Type == TokenType.SEMICOLON)
                    {
                        current++;
                    }
                }
                
                if (current < tokens.Count && tokens[current].Type == TokenType.RIGHT_BRACKET)
                {
                    current++; // Skip RIGHT_BRACKET
                }
            }
            
            // Parse function body (everything until RIGHT_BRACE)
            var bodyStart = current;
            var braceLevel = 1;
            
            while (current < tokens.Count && braceLevel > 0)
            {
                if (tokens[current].Type == TokenType.LEFT_BRACE)
                    braceLevel++;
                else if (tokens[current].Type == TokenType.RIGHT_BRACE)
                    braceLevel--;
                    
                if (braceLevel > 0)
                    current++;
            }
            
            if (braceLevel == 0)
            {
                current--; // Back up to the RIGHT_BRACE
                
                // Extract body tokens
                var bodyTokens = new List<Token>();
                for (int i = bodyStart; i < current; i++)
                {
                    bodyTokens.Add(tokens[i]);
                }
                
                current++; // Skip the RIGHT_BRACE
                
                // Create function body text
                var bodyText = string.Join(" ", bodyTokens.Select(t => t.Lexeme));
                
                // Create function value
                var functionValue = new FunctionValue(bodyText, parameters, bodyTokens);
                
                // Create function AST node
                var functionNode = new ASTNode(ASTNodeType.Function);
                functionNode.Value = functionValue;
                functionNode.Parameters = parameters;
                
                return functionNode;
            }
            
            return null;
        }
        
        private ASTNode ParseExpressionFromTokens(List<Token> tokens, ref int current)
        {
            // Simple expression parsing - this is a simplified version
            // In a full implementation, this would be more sophisticated
            if (current >= tokens.Count) return null;
            
            var token = tokens[current];
            
            // Handle literals
            if (token.Type == TokenType.INTEGER || token.Type == TokenType.FLOAT || token.Type == TokenType.SYMBOL)
            {
                current++;
                return ASTNode.MakeLiteral(CreateK3Value(token));
            }
            
            // Handle function calls: variable . argument (dot-apply)
            if (token.Type == TokenType.SYMBOL && current + 2 < tokens.Count && tokens[current + 1].Type == TokenType.DOT_APPLY)
            {
                var funcName = token.Lexeme;
                var argToken = tokens[current + 2];
                
                // Check if the next token could be an argument
                if (IsArgumentToken(argToken))
                {
                    current += 3; // Skip function, dot, and argument
                    
                    var funcVar = ASTNode.MakeVariable(funcName);
                    var argValue = ASTNode.MakeLiteral(CreateK3Value(argToken));
                    
                    var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                    funcCall.Children.Add(funcVar);
                    funcCall.Children.Add(argValue);
                    
                    return funcCall;
                }
            }
            
            // Handle function calls: variable argument (space-separated)
            if (token.Type == TokenType.SYMBOL && current + 1 < tokens.Count)
            {
                var funcName = token.Lexeme;
                var argToken = tokens[current + 1];
                
                // Check if the next token could be an argument
                if (IsArgumentToken(argToken))
                {
                    current += 2;
                    
                    var funcVar = ASTNode.MakeVariable(funcName);
                    var argValue = ASTNode.MakeLiteral(CreateK3Value(argToken));
                    
                    var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                    funcCall.Children.Add(funcVar);
                    funcCall.Children.Add(argValue);
                    
                    return funcCall;
                }
            }
            
            // Skip unknown tokens
            current++;
            return null;
        }
        
        private bool IsArgumentToken(Token token)
        {
            return token.Type == TokenType.INTEGER || token.Type == TokenType.FLOAT || 
                   token.Type == TokenType.SYMBOL;
        }
        
        private K3Value CreateK3Value(Token token)
        {
            switch (token.Type)
            {
                case TokenType.INTEGER:
                    return new IntegerValue(int.Parse(token.Lexeme));
                case TokenType.FLOAT:
                    return new FloatValue(double.Parse(token.Lexeme));
                case TokenType.SYMBOL:
                    return new SymbolValue(token.Lexeme);
                default:
                    return new NullValue();
            }
        }

        private K3Value CallVariableFunction(string functionName, List<K3Value> arguments)
        {
            // Check if it's a built-in function first
            switch (functionName)
            {
                case "!":
                    // Handle unary enumerate operator
                    if (arguments.Count == 1)
                    {
                        // Special case: !` enumerates keys in root dictionary
                        if (arguments[0] is SymbolValue sym && sym.Value == "")
                        {
                            return kTree.GetRootKeys();
                        }
                        return Enumerate(arguments[0]);
                    }
                    else if (arguments.Count >= 2)
                    {
                        return ModRotate(arguments[0], arguments[1]);
                    }
                    throw new Exception("! operator requires at least 1 argument");
                case "do":
                case "_do":
                    return DoFunction(arguments.Count > 0 ? new VectorValue(arguments) : new NullValue());
                case "while":
                case "_while":
                    return WhileFunction(arguments.Count > 0 ? new VectorValue(arguments) : new NullValue());
                case "if":
                case "_if":
                    return IfFunction(arguments.Count > 0 ? new VectorValue(arguments) : new NullValue());
                case ":":
                    // Check if this is conditional evaluation (3+ arguments) or regular assignment
                    if (arguments.Count >= 3)
                    {
                        // Conditional evaluation: :[cond; true; false]
                        return ConditionalEvaluation(arguments);
                    }
                    else if (arguments.Count == 2)
                    {
                        // Assignment: variable : value
                        if (arguments[0] is SymbolValue variableName)
                        {
                            return Assignment(variableName.Value, arguments[1]);
                        }
                        else
                        {
                            throw new Exception("Assignment requires a variable name on the left side");
                        }
                    }
                    else if (arguments.Count == 1)
                    {
                        // Monadic colon - return value from function (not implemented yet)
                        throw new Exception("Monadic colon (return from function) is not yet implemented");
                    }
                    else
                    {
                        throw new Exception("Colon operator requires at least 1 argument");
                    }
                case "@":
                    // Check if this is amend-item (3+ arguments) or regular @ operator
                    if (arguments.Count >= 3)
                    {
                        // Check for special case: triadic @ with colon (trapped apply to enlisted argument)
                        if (arguments.Count == 3 && IsColon(arguments[1]))
                        {
                            // Trapped apply to enlisted argument: (d; :; y) -> trapped apply with enlisted y
                            var enlistedArgument = Enlist(arguments[2]);
                            return TrappedApply(arguments[0], enlistedArgument);
                        }
                        else
                        {
                            // Regular amend-item operation with enlisted second argument
                            var enlistedArguments = new List<K3Value> { arguments[0] };
                            enlistedArguments.Add(Enlist(arguments[1]));
                            for (int i = 2; i < arguments.Count; i++)
                            {
                                enlistedArguments.Add(arguments[i]);
                            }
                            return AmendItemFunction(enlistedArguments);
                        }
                    }
                    else
                    {
                        // Regular @ operator - handle based on argument count
                        if (arguments.Count == 1)
                        {
                            // Monadic @ is ATOM
                            return Atom(arguments[0]);
                        }
                        else if (arguments.Count == 2)
                        {
                            // Dyadic @ is AT - use AtIndex
                            return AtIndex(arguments[0], arguments[1]);
                        }
                        else
                        {
                            throw new Exception("@ operator with invalid number of arguments");
                        }
                    }
                case ".":
                    // Check if this is amend (3+ arguments) or regular . operator
                    if (arguments.Count >= 3)
                    {
                        // Check for special case: triadic dot with colon (trapped apply)
                        if (arguments.Count == 3 && IsColon(arguments[1]))
                        {
                            // Trapped apply: (d; :; y) - behave like dyadic dot apply but never throw exceptions
                            return TrappedApply(arguments[0], arguments[2]);
                        }
                        else
                        {
                            // Regular amend operation
                            return AmendFunction(arguments);
                        }
                    }
                    else
                    {
                        // Regular . operator - handle based on argument count
                        if (arguments.Count == 1)
                        {
                            // Monadic . is MAKE - use Make function
                            return Make(arguments[0]);
                        }
                        else if (arguments.Count == 2)
                        {
                            // Dyadic . is APPLY - use DotApply
                            return DotApply(arguments[0], arguments[1]);
                        }
                        else
                        {
                            throw new Exception(". operator with 2+ arguments is not supported (use .[...] for amend)");
                        }
                    }
                // Dyadic operators
                case "+":
                    if (arguments.Count == 1)
                    {
                        // Plus as a function (transpose)
                        return Transpose(arguments[0]);
                    }
                    else if (arguments.Count >= 2) 
                    {
                        return Plus(arguments[0], arguments[1]);
                    }
                    throw new Exception("+ operator requires 1 or 2 arguments");
                case "-":
                    if (arguments.Count >= 2) return Minus(arguments[0], arguments[1]);
                    throw new Exception("- operator requires 2 arguments");
                case "*":
                    if (arguments.Count >= 2) return Times(arguments[0], arguments[1]);
                    throw new Exception("* operator requires 2 arguments");
                case "/":
                    if (arguments.Count >= 2) return Divide(arguments[0], arguments[1]);
                    Console.WriteLine($"DEBUG: Divide operator called with symbol: {arguments[0]}"); // Debug output
                    throw new Exception("/ operator requires 2 arguments");
                case "^":
                    if (arguments.Count >= 2) return Power(arguments[0], arguments[1]);
                    throw new Exception("^ operator requires 2 arguments");
                case "<":
                    if (arguments.Count >= 2) return LessThan(arguments[0], arguments[1]);
                    throw new Exception("< operator requires 2 arguments");
                case ">":
                    if (arguments.Count >= 2) return GreaterThan(arguments[0], arguments[1]);
                    throw new Exception("> operator requires 2 arguments");
                case "=":
                    if (arguments.Count >= 2) return Match(arguments[0], arguments[1]);
                    if (arguments.Count == 1) return Group(arguments[0]);
                    throw new Exception("= operator requires 1 or 2 arguments");
                case ",":
                    if (arguments.Count >= 2) return Join(arguments[0], arguments[1]);
                    throw new Exception(", operator requires 2 arguments");
            }
            
            // Check if it's a user-defined function stored in a variable
            var functionValue = GetVariable(functionName);
            
            if (functionValue is FunctionValue userFunction)
            {
                // Create a temporary AST node for the function to reuse CallDirectFunction
                var tempFunctionNode = new ASTNode(ASTNodeType.Function);
                tempFunctionNode.Value = userFunction;
                return CallDirectFunction(tempFunctionNode, arguments);
            }
            else if (functionValue is VectorValue vectorValue && arguments.Count == 1)
            {
                // This is vector indexing using square bracket syntax: vector[index]
                return VectorIndex(vectorValue, arguments[0]);
            }
            throw new Exception($"Variable '{functionName}' is not a function");
        }

        private K3Value EvaluateBlock(ASTNode node)
        {
            K3Value lastResult = null;
            
            foreach (var child in node.Children)
            {
                lastResult = EvaluateNode(child);
            }
            
            return lastResult;
        }

        private K3Value In(K3Value left, K3Value right)
        {
            // _in (Find) function - searches for left argument in right argument
            // Returns position (1-based) or 0 if not found
            // Uses tolerant comparison for floating-point numbers
            
            if (right is VectorValue rightVec)
            {
                // Search for left in right vector
                for (int i = 0; i < rightVec.Elements.Count; i++)
                {
                    var matchResult = Match(left, rightVec.Elements[i]);
                    if (matchResult is IntegerValue intVal && intVal.Value == 1)
                    {
                        return new IntegerValue(i + 1); // 1-based indexing
                    }
                }
                return new IntegerValue(0); // Not found
            }
            else
            {
                // Search for left in right scalar
                var matchResult = Match(left, right);
                if (matchResult is IntegerValue intVal2 && intVal2.Value == 1)
                {
                    return new IntegerValue(1); // Found at position 1
                }
                return new IntegerValue(0); // Not found
            }
        }

        private K3Value Bin(K3Value left, K3Value right)
        {
            // _bin (Binary Search) function - performs binary search on sorted list
            // Returns position (1-based) or 0 if not found
            // Assumes right argument is sorted in ascending order
            
            if (right is VectorValue rightVec)
            {
                int low = 0;
                int high = rightVec.Elements.Count - 1;
                
                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    var midValue = rightVec.Elements[mid];
                    var comparison = CompareValues(left, midValue);
                    
                    if (comparison == 0)
                    {
                        return new IntegerValue(mid + 1); // 1-based indexing
                    }
                    else if (comparison < 0)
                    {
                        high = mid - 1;
                    }
                    else
                    {
                        low = mid + 1;
                    }
                }
                return new IntegerValue(0); // Not found
            }
            else
            {
                // Search for left in right scalar
                var comparison = CompareValues(left, right);
                if (comparison == 0)
                {
                    return new IntegerValue(1); // Found at position 1
                }
                return new IntegerValue(0); // Not found
            }
        }

        private K3Value Binl(K3Value left, K3Value right)
        {
            // _binl (Binary Search Each-Left) function
            // Returns 1 for each element of left that is found in right, 0 otherwise
            // Equivalent to left _in\: right but optimized
            
            if (left is VectorValue leftVec)
            {
                var results = new List<K3Value>();
                
                // For binary search each-left, we need to search each element of left in right
                foreach (var leftElement in leftVec.Elements)
                {
                    var result = Bin(leftElement, right);
                    // Convert position result to 1/0 (found/not found)
                    var found = result is IntegerValue intVal && intVal.Value != 0;
                    results.Add(new IntegerValue(found ? 1 : 0));
                }
                
                return new VectorValue(results);
            }
            else
            {
                // Single element case
                var result = Bin(left, right);
                var found = result is IntegerValue intVal && intVal.Value != 0;
                return new IntegerValue(found ? 1 : 0);
            }
        }

        private K3Value Lin(K3Value left, K3Value right)
        {
            // _lin (List Intersection) function
            // Returns 1 for each element of left that is in right, 0 otherwise
            // Equivalent to left _in\: right but optimized using HashSet
            
            if (left is VectorValue leftVec)
            {
                var results = new List<K3Value>();
                
                // Create a HashSet for efficient O(1) lookups of right argument elements
                var rightSet = CreateHashSet(right);
                
                foreach (var leftElement in leftVec.Elements)
                {
                    bool found = false;
                    
                    // Check if leftElement exists in rightSet
                    if (rightSet != null)
                    {
                        found = rightSet.Contains(leftElement);
                    }
                    else
                    {
                        // Fallback to linear search if HashSet creation failed
                        found = LinearSearchInRight(leftElement, right);
                    }
                    
                    results.Add(new IntegerValue(found ? 1 : 0));
                }
                
                return new VectorValue(results);
            }
            else
            {
                // Single element case - return 1 if found, 0 otherwise
                bool found = LinearSearchInRight(left, right);
                return new IntegerValue(found ? 1 : 0);
            }
        }

        private HashSet<K3Value> CreateHashSet(K3Value value)
        {
            // Create a HashSet from a K3Value for efficient lookups
            try
            {
                var set = new HashSet<K3Value>(new K3ValueComparer());
                
                if (value is VectorValue vec)
                {
                    foreach (var element in vec.Elements)
                    {
                        set.Add(element);
                    }
                }
                else
                {
                    set.Add(value);
                }
                
                return set;
            }
            catch
            {
                return null; // Return null if HashSet creation fails
            }
        }

        private bool LinearSearchInRight(K3Value leftElement, K3Value right)
        {
            // Linear search for leftElement in right
            if (right is VectorValue rightVec)
            {
                foreach (var rightElement in rightVec.Elements)
                {
                    var matchResult = Match(leftElement, rightElement);
                    if (matchResult is IntegerValue intVal && intVal.Value == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                var matchResult = Match(leftElement, right);
                return matchResult is IntegerValue intVal && intVal.Value == 1;
            }
        }

        private K3Value Find(K3Value left, K3Value right)
        {
            // Find operator: d ? y
            // If y occurs among the items of d then d?y is the smallest index of all occurrences
            // Otherwise, d?y is #d (the smallest nonnegative integer that is not a valid index of d)
            // When d is nil, the result is y
            // Uses Match for comparing items (tolerant comparison)
            
            // Handle nil case: when d is nil, result is y
            if (left is NullValue)
            {
                return right;
            }
            
            // Handle list case: d is a list
            if (left is VectorValue leftVec)
            {
                // Search for right in left vector
                for (int i = 0; i < leftVec.Elements.Count; i++)
                {
                    var matchResult = Match(leftVec.Elements[i], right);
                    if (matchResult is IntegerValue intVal && intVal.Value == 1)
                    {
                        return new IntegerValue(i); // 0-based indexing (K3 standard)
                    }
                }
                // Not found, return #d (count of d)
                return new IntegerValue(leftVec.Elements.Count);
            }
            else
            {
                // Handle scalar case: d is an atom
                var matchResult = Match(left, right);
                if (matchResult is IntegerValue intVal2 && intVal2.Value == 1)
                {
                    return new IntegerValue(0); // Found at index 0 (K3 0-based)
                }
                // Not found, return #d (count of scalar is 1)
                return new IntegerValue(1);
            }
        }

        private K3Value Assignment(string variableName, K3Value value)
        {
            // Assignment: variable : value
            // Uses local variable assignment
            SetVariable(variableName, value);
            return value;
        }

        private K3Value ColonOperator(K3Value left, K3Value right)
        {
            // Colon operator: left : right
            // Can be either:
            // 1. Assignment: variable : value (when left is a variable name symbol)
            // 2. Conditional evaluation: :[cond; true; false] (when left is null from bracket parsing)
            
            // Check if this is conditional evaluation (left is null from bracket parsing)
            if (left is NullValue)
            {
                // This is conditional evaluation: right should be a vector of arguments
                if (right is VectorValue args)
                {
                    return ConditionalEvaluation(args.Elements);
                }
                else
                {
                    throw new Exception("Conditional evaluation requires a list of arguments");
                }
            }
            else
            {
                // This is assignment: left : right
                if (left is SymbolValue variableName)
                {
                    return Assignment(variableName.Value, right);
                }
                else
                {
                    throw new Exception($"Assignment requires a variable name on the left side, but got {left.GetType().Name} with value {left}");
                }
            }
        }

        private K3Value ConditionalEvaluation(List<K3Value> arguments)
        {
            // Conditional evaluation: [cond; true; false] or [cond1;true1; cond2;true2; ; condN;trueN; false]
            // Arguments alternate between conditions and expressions to execute
            // Returns the result of the first true expression, or nil if all conditions are false
            
            if (arguments.Count < 3)
            {
                throw new Exception("Conditional evaluation requires at least 3 arguments");
            }
            
            // Process arguments in pairs: (condition, expression)
            for (int i = 0; i < arguments.Count - 1; i += 2)
            {
                var condition = arguments[i];
                var expression = arguments[i + 1];
                
                // Check if this is the final "else" case (no condition)
                if (i == arguments.Count - 2 && arguments.Count % 2 == 1)
                {
                    // This is the default case - execute it
                    return EvaluateExpression(expression);
                }
                
                // Evaluate condition
                var conditionResult = EvaluateExpression(condition);
                
                // Check if condition is a non-zero integer
                if (IsNonZeroInteger(conditionResult))
                {
                    // Condition is true, execute the expression
                    return EvaluateExpression(expression);
                }
            }
            
            // All conditions were false, return nil
            return new NullValue();
        }
        
        private bool IsNonZeroInteger(K3Value value)
        {
            if (value is IntegerValue intValue)
            {
                return intValue.Value != 0;
            }
            else if (value is LongValue longValue)
            {
                return longValue.Value != 0;
            }
            else
            {
                throw new Exception("Condition must be an integer atom");
            }
        }
        
        private K3Value EvaluateExpression(K3Value expression)
        {
            // If the expression is already evaluated, return it
            if (!(expression is FunctionValue))
            {
                return expression;
            }
            
            // If it's a function value, we need to evaluate it
            // For now, this is a simplified implementation
            // In a full implementation, we'd need to handle function evaluation properly
            return expression;
        }

        private K3Value Plus(K3Value a, K3Value b)
        {
            // Handle mixed type promotion
            if (a is IntegerValue && b is LongValue)
            {
                unchecked
                {
                    return new LongValue(((IntegerValue)a).Value + ((LongValue)b).Value);
                }
            }
            if (a is LongValue && b is IntegerValue)
            {
                unchecked
                {
                    return new LongValue(((LongValue)a).Value + ((IntegerValue)b).Value);
                }
            }
            if (a is IntegerValue && b is FloatValue)
                return new FloatValue(((IntegerValue)a).Value + ((FloatValue)b).Value);
            if (a is FloatValue && b is IntegerValue)
                return new FloatValue(((FloatValue)a).Value + ((IntegerValue)b).Value);
            if (a is LongValue && b is FloatValue)
                return new FloatValue(((LongValue)a).Value + ((FloatValue)b).Value);
            if (a is FloatValue && b is LongValue)
                return new FloatValue(((FloatValue)a).Value + ((LongValue)b).Value);
            
            // Handle same type operations - use the K3Value Add method for proper overflow handling
            if (a is IntegerValue && b is IntegerValue)
                return ((IntegerValue)a).Add((IntegerValue)b);
            if (a is LongValue && b is LongValue)
                return ((LongValue)a).Add((LongValue)b);
            if (a is FloatValue && b is FloatValue)
                return ((FloatValue)a).Add((FloatValue)b);
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Add(vecB);
                else
                    return vecA.Add(b);
            }
            
            // Handle scalar + vector operations
            if (b is VectorValue vectorB)
                return vectorB.Add(a);
            
            throw new Exception($"Cannot add {a.Type} and {b.Type}");
        }

        private K3Value Minus(K3Value a, K3Value b)
        {
            // Handle mixed type promotion
            if (a is IntegerValue && b is LongValue)
            {
                unchecked
                {
                    return new LongValue(((IntegerValue)a).Value - ((LongValue)b).Value);
                }
            }
            if (a is LongValue && b is IntegerValue)
            {
                unchecked
                {
                    return new LongValue(((LongValue)a).Value - ((IntegerValue)b).Value);
                }
            }
            if (a is IntegerValue && b is FloatValue)
                return new FloatValue(((IntegerValue)a).Value - ((FloatValue)b).Value);
            if (a is FloatValue && b is IntegerValue)
                return new FloatValue(((FloatValue)a).Value - ((IntegerValue)b).Value);
            if (a is LongValue && b is FloatValue)
                return new FloatValue(((LongValue)a).Value - ((FloatValue)b).Value);
            if (a is FloatValue && b is LongValue)
                return new FloatValue(((FloatValue)a).Value - ((LongValue)b).Value);
            
            // Handle same type operations - use the K3Value Subtract method for proper overflow handling
            if (a is IntegerValue && b is IntegerValue)
                return ((IntegerValue)a).Subtract((IntegerValue)b);
            if (a is LongValue && b is LongValue)
                return ((LongValue)a).Subtract((LongValue)b);
            if (a is FloatValue && b is FloatValue)
                return ((FloatValue)a).Subtract((FloatValue)b);
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Subtract(vecB);
                else
                    return vecA.Subtract(b);
            }
            
            // Handle scalar - vector operations
            if (b is VectorValue vectorB)
            {
                // For scalar - vector, we need to subtract each element from the scalar
                var result = new List<K3Value>();
                foreach (var element in vectorB.Elements)
                {
                    result.Add(Minus(a, element));
                }
                return new VectorValue(result);
            }
            
            throw new Exception($"Cannot subtract {a.Type} and {b.Type}");
        }

        private K3Value Times(K3Value a, K3Value b)
        {
            // Handle mixed type promotion
            if (a is IntegerValue && b is LongValue)
            {
                unchecked
                {
                    return new LongValue(((IntegerValue)a).Value * ((LongValue)b).Value);
                }
            }
            if (a is LongValue && b is IntegerValue)
            {
                unchecked
                {
                    return new LongValue(((LongValue)a).Value * ((IntegerValue)b).Value);
                }
            }
            if (a is IntegerValue && b is FloatValue)
                return new FloatValue(((IntegerValue)a).Value * ((FloatValue)b).Value);
            if (a is FloatValue && b is IntegerValue)
                return new FloatValue(((FloatValue)a).Value * ((IntegerValue)b).Value);
            if (a is LongValue && b is FloatValue)
                return new FloatValue(((LongValue)a).Value * ((FloatValue)b).Value);
            if (a is FloatValue && b is LongValue)
                return new FloatValue(((FloatValue)a).Value * ((LongValue)b).Value);
            
            // Handle same type operations - use the K3Value Multiply method for proper overflow handling
            if (a is IntegerValue && b is IntegerValue)
                return ((IntegerValue)a).Multiply((IntegerValue)b);
            if (a is LongValue && b is LongValue)
                return ((LongValue)a).Multiply((LongValue)b);
            if (a is FloatValue && b is FloatValue)
                return ((FloatValue)a).Multiply((FloatValue)b);
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Multiply(vecB);
                else
                    return vecA.Multiply(b);
            }
            
            // Handle scalar * vector operations
            if (b is VectorValue vectorB)
            {
                return vectorB.Multiply(a);
            }
            
            throw new Exception($"Cannot multiply {a.Type} and {b.Type}");
        }

        private K3Value Divide(K3Value a, K3Value b)
        {
            // Handle integer division with modulo check
            if (a is IntegerValue && b is IntegerValue)
            {
                int divisor = ((IntegerValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                
                int dividend = ((IntegerValue)a).Value;
                // Check modulo - if zero, do integer division
                if (dividend % divisor == 0)
                    return new IntegerValue(dividend / divisor);
                else
                    return new FloatValue((double)dividend / divisor);
            }
            
            // Handle long division with modulo check
            if (a is LongValue && b is LongValue)
            {
                long divisor = ((LongValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                
                long dividend = ((LongValue)a).Value;
                // Check modulo - if zero, do integer division
                if (dividend % divisor == 0)
                    return new LongValue(dividend / divisor);
                else
                    return new FloatValue((double)dividend / divisor);
            }
            
            // Handle mixed type promotion
            if (a is IntegerValue && b is LongValue)
            {
                long divisor = ((LongValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue((double)((IntegerValue)a).Value / divisor);
            }
            if (a is LongValue && b is IntegerValue)
            {
                int divisor = ((IntegerValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue((double)((LongValue)a).Value / divisor);
            }
            if (a is IntegerValue && b is FloatValue)
            {
                double divisor = ((FloatValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue(((IntegerValue)a).Value / divisor);
            }
            if (a is FloatValue && b is IntegerValue)
            {
                int divisor = ((IntegerValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue(((FloatValue)a).Value / divisor);
            }
            if (a is LongValue && b is FloatValue)
            {
                double divisor = ((FloatValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue(((LongValue)a).Value / divisor);
            }
            if (a is FloatValue && b is LongValue)
            {
                long divisor = ((LongValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue(((FloatValue)a).Value / divisor);
            }
            
            // Handle same type float operations
            if (a is FloatValue && b is FloatValue)
            {
                double divisor = ((FloatValue)b).Value;
                if (divisor == 0)
                    throw new Exception("Division by zero");
                return new FloatValue(((FloatValue)a).Value / divisor);
            }
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Divide(vecB);
                else
                    return vecA.Divide(b);
            }
            
            // Handle scalar / vector operations
            if (b is VectorValue vectorB)
            {
                // For scalar / vector, we need to divide scalar by each element
                var result = new List<K3Value>();
                foreach (var element in vectorB.Elements)
                {
                    result.Add(Divide(a, element));
                }
                return new VectorValue(result);
            }
            
            throw new Exception($"Cannot divide {a.Type} and {b.Type}");
        }

        private K3Value Min(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(Math.Min(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(Math.Min(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Min(floatA.Value, floatB.Value));
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Minimum(vecB);
                else
                    return vecA.Minimum(b);
            }
            
            throw new Exception($"Cannot find minimum of {a.Type} and {b.Type}");
        }

        private K3Value Max(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(Math.Max(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(Math.Max(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Max(floatA.Value, floatB.Value));
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Maximum(vecB);
                else
                    return vecA.Maximum(b);
            }
            
            throw new Exception($"Cannot find maximum of {a.Type} and {b.Type}");
        }

        private K3Value Less(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value < intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value < longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value < floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with <");
        }

        private K3Value More(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value > intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value > longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value > floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with >");
        }

        private K3Value NegateBinary(K3Value a, K3Value b)
        {
            return Negate(a);
        }

        private K3Value Minimum(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(Math.Min(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(Math.Min(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Min(floatA.Value, floatB.Value));
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Minimum(vecB);
                else
                    return vecA.Minimum(b);
            }
            
            throw new Exception($"Cannot find minimum of {a.Type} and {b.Type}");
        }

        private K3Value Maximum(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(Math.Max(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(Math.Max(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Max(floatA.Value, floatB.Value));
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Maximum(vecB);
                else
                    return vecA.Maximum(b);
            }
            
            throw new Exception($"Cannot find maximum of {a.Type} and {b.Type}");
        }

        private K3Value LessThan(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value < intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value < longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value < floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with <");
        }

        private K3Value GreaterThan(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value > intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value > longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value > floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with >");
        }

        public K3Value Match(K3Value a, K3Value b)
        {
            // For match comparison (~ operator)
            if (a is FloatValue floatA && b is FloatValue floatB)
            {
                double maxAbs = Math.Max(Math.Abs(floatA.Value), Math.Abs(floatB.Value));
                double threshold = maxAbs * 0.00001; // 0.001 percent
                return new IntegerValue(Math.Abs(floatA.Value - floatB.Value) < threshold ? 1 : 0);
            }
            
            if (a is VectorValue vecA && b is VectorValue vecB)
            {
                if (vecA.Elements.Count != vecB.Elements.Count)
                    return new IntegerValue(0);
                
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    var result = Match(vecA.Elements[i], vecB.Elements[i]);
                    if (result is IntegerValue intResult && intResult.Value == 0)
                        return new IntegerValue(0);
                }
                return new IntegerValue(1);
            }
            
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value == intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value == longB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with =");
        }

        private K3Value Power(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue((int)Math.Pow(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue((long)Math.Pow(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Pow(floatA.Value, floatB.Value));
            
            throw new Exception($"Cannot raise {a.Type} to power of {b.Type}");
        }

        private K3Value ModRotate(K3Value left, K3Value right)
        {
            // Enhanced ! operator with multiple behaviors
            if (left is IntegerValue leftInt && right is IntegerValue rightInt)
            {
                // Integer mod: remainder of division
                return new IntegerValue(leftInt.Value % rightInt.Value);
            }
            else if (left is VectorValue leftVec && right is IntegerValue rightIntVal)
            {
                // Vector mod: remainder for each element
                var result = new List<K3Value>();
                foreach (var element in leftVec.Elements)
                {
                    if (element is IntegerValue intVal)
                    {
                        result.Add(new IntegerValue(intVal.Value % rightIntVal.Value));
                    }
                    else
                    {
                        throw new Exception("Vector mod requires all elements to be integers");
                    }
                }
                return new VectorValue(result);
            }
            else if (left is IntegerValue leftIntVal && right is VectorValue rightVec)
            {
                // Vector rotation: rotate vector by integer
                int rotation = leftIntVal.Value;
                int size = rightVec.Elements.Count;
                
                if (size == 0)
                    return new VectorValue(new List<K3Value>());
                
                // Normalize rotation to be within vector bounds
                rotation = ((rotation % size) + size) % size;
                
                var result = new List<K3Value>();
                for (int i = 0; i < size; i++)
                {
                    result.Add(rightVec.Elements[(i + rotation) % size]);
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("Modulus operator requires integer arguments or vector+integer combinations");
            }
        }

        private K3Value Negate(K3Value a)
        {
            if (a is IntegerValue intA)
                return new IntegerValue(-intA.Value);
            if (a is LongValue longA)
                return new LongValue(-longA.Value);
            if (a is FloatValue floatA)
                return new FloatValue(-floatA.Value);
            
            throw new Exception($"Cannot negate {a.Type}");
        }

        private K3Value LogicalNegate(K3Value a)
        {
            if (a is IntegerValue intA)
                return new IntegerValue(intA.Value == 0 ? 1 : 0);
            if (a is LongValue longA)
                return new IntegerValue(longA.Value == 0 ? 1 : 0);
            if (a is FloatValue floatA)
                return new IntegerValue(floatA.Value == 0 ? 1 : 0);
            
            throw new Exception($"Cannot logically negate {a.Type}");
        }

        private K3Value Join(K3Value a, K3Value b)
        {
            // Handle joining two values into a vector
            var elements = new List<K3Value>();
            
            if (a is VectorValue vecA)
            {
                elements.AddRange(vecA.Elements);
            }
            else
            {
                elements.Add(a);
            }
            
            if (b is VectorValue vecB)
            {
                elements.AddRange(vecB.Elements);
            }
            else
            {
                elements.Add(b);
            }
            
            return new VectorValue(elements);
        }

        // New unary operator implementations
        private K3Value UnaryMinus(K3Value a)
        {
            if (a is IntegerValue intA)
                return new IntegerValue(-intA.Value);
            if (a is LongValue longA)
                return new LongValue(-longA.Value);
            if (a is FloatValue floatA)
                return new FloatValue(-floatA.Value);
            
            throw new Exception($"Cannot apply unary minus to {a.Type}");
        }

        private K3Value Transpose(K3Value a)
        {
            // Flip/transpose operation: +(`a`b`c;1 2 3) -> ((`a;1);(`b;2);(`c;3))
            if (a is VectorValue vec && vec.Elements.Count == 2)
            {
                var first = vec.Elements[0];
                var second = vec.Elements[1];
                
                if (first is VectorValue firstVec && second is VectorValue secondVec)
                {
                    // Check if both vectors have the same length
                    if (firstVec.Elements.Count != secondVec.Elements.Count)
                    {
                        throw new Exception("Flip requires vectors of equal length");
                    }
                    
                    // Create the flipped structure: ((first[i];second[i]);...)
                    var result = new List<K3Value>();
                    for (int i = 0; i < firstVec.Elements.Count; i++)
                    {
                        var pair = new List<K3Value> { firstVec.Elements[i], secondVec.Elements[i] };
                        result.Add(new VectorValue(pair));
                    }
                    
                    return new VectorValue(result);
                }
            }
            
            // For other cases, return as-is (matrix transpose not implemented)
            return a;
        }

        private K3Value First(K3Value a)
        {
            if (a is VectorValue vecA && vecA.Elements.Count > 0)
                return vecA.Elements[0];
            
            return a; // For scalars, return the value itself
        }

        private K3Value Reciprocal(K3Value a)
        {
            if (a is IntegerValue intA)
                return new FloatValue(1.0 / intA.Value);
            if (a is LongValue longA)
                return new FloatValue(1.0 / longA.Value);
            if (a is FloatValue floatA)
                return new FloatValue(1.0 / floatA.Value);
            
            throw new Exception($"Cannot find reciprocal of {a.Type}");
        }

        private K3Value Where(K3Value a)
        {
            // Convert scalar to single-element vector for consistent processing
            VectorValue vecA;
            if (a is IntegerValue intA)
            {
                vecA = new VectorValue(new List<K3Value> { intA });
            }
            else if (a is VectorValue vectorA)
            {
                vecA = vectorA;
            }
            else
            {
                throw new Exception($"Cannot apply where to {a.Type}");
            }
            
            // Generate indices repeated according to count values
            var elements = new List<K3Value>();
            for (int i = 0; i < vecA.Elements.Count; i++)
            {
                var element = vecA.Elements[i];
                int count = 0;
                
                // Get the count value from the element
                if (element is IntegerValue intVal)
                {
                    count = intVal.Value;
                }
                else if (element is FloatValue floatVal)
                {
                    count = (int)floatVal.Value;
                }
                
                // Add the index repeated 'count' times
                for (int j = 0; j < count; j++)
                {
                    elements.Add(new IntegerValue(i));
                }
            }
            return new VectorValue(elements);
        }

        private K3Value Reverse(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var reversed = new List<K3Value>(vecA.Elements);
                reversed.Reverse();
                return new VectorValue(reversed);
            }
            
            return a; // For scalars, return the value itself
        }

        private K3Value GradeUp(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var indices = new List<int>();
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    indices.Add(i);
                }
                
                // Simple stable sort implementation
                indices.Sort((i, j) => CompareValues(vecA.Elements[i], vecA.Elements[j]));
                
                var result = new List<K3Value>();
                foreach (var index in indices)
                {
                    result.Add(new IntegerValue(index));
                }
                return new VectorValue(result);
            }
            
            throw new Exception("Rank error: grade-up operator '<' requires a vector argument");
        }

        private K3Value GradeDown(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var indices = new List<int>();
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    indices.Add(i);
                }
                
                // Simple stable sort implementation (descending)
                indices.Sort((i, j) => CompareValues(vecA.Elements[j], vecA.Elements[i]));
                
                var result = new List<K3Value>();
                foreach (var index in indices)
                {
                    result.Add(new IntegerValue(index));
                }
                return new VectorValue(result);
            }
            
            throw new Exception("Rank error: grade-down operator '>' requires a vector argument");
        }

        private int CompareValues(K3Value a, K3Value b)
        {
            // Handle all K3Value types
            if (a is IntegerValue intA && b is IntegerValue intB)
                return intA.Value.CompareTo(intB.Value);
            if (a is LongValue longA && b is LongValue longB)
                return longA.Value.CompareTo(longB.Value);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return floatA.Value.CompareTo(floatB.Value);
            if (a is CharacterValue charA && b is CharacterValue charB)
                return charA.Value.CompareTo(charB.Value);
            if (a is SymbolValue symA && b is SymbolValue symB)
                return string.Compare(symA.Value, symB.Value, StringComparison.Ordinal);
            
            // For vectors and other types, use ToString comparison
            int comparison = string.Compare(a.ToString(), b.ToString(), StringComparison.Ordinal);
            
            // For stable sorting: if equal, preserve original order
            if (comparison == 0)
                return -1; // a comes before b in original order
            
            return comparison;
        }

        private K3Value Shape(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                // Check if this is a simple vector (no nested vectors)
                var hasNestedVectors = vecA.Elements.Any(e => e is VectorValue);
                
                if (!hasNestedVectors)
                {
                    // Simple vector - return its length as a 1-element vector
                    return new VectorValue(new List<K3Value> { new IntegerValue(vecA.Elements.Count) });
                }
                else
                {
                    // Matrix or tensor - compute dimensions
                    var dimensions = new List<int>();
                    var current = vecA;
                    
                    // First dimension is the number of top-level elements
                    dimensions.Add(current.Elements.Count);
                    
                    // Check if we have a regular matrix/tensor
                    if (current.Elements.Count > 0 && current.Elements[0] is VectorValue)
                    {
                        var firstElement = (VectorValue)current.Elements[0];
                        var isUniform = true;
                        var uniformLength = firstElement.Elements.Count;
                        
                        // Check if all elements have the same structure
                        foreach (var element in current.Elements)
                        {
                            if (element is VectorValue vec)
                            {
                                if (vec.Elements.Count != uniformLength)
                                {
                                    isUniform = false;
                                    break;
                                }
                            }
                            else
                            {
                                isUniform = false;
                                break;
                            }
                        }
                        
                        if (isUniform && uniformLength > 0)
                        {
                            // Check if we have nested vectors (3D tensor)
                            if (firstElement.Elements[0] is VectorValue)
                            {
                                // 3D tensor - check uniformity of third dimension
                                var thirdDimUniform = true;
                                var thirdDimLength = ((VectorValue)firstElement.Elements[0]).Elements.Count;
                                
                                foreach (var element in current.Elements)
                                {
                                    var vec = (VectorValue)element;
                                    foreach (var subElement in vec.Elements)
                                    {
                                        if (subElement is VectorValue subVec)
                                        {
                                            if (subVec.Elements.Count != thirdDimLength)
                                            {
                                                thirdDimUniform = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            thirdDimUniform = false;
                                            break;
                                        }
                                    }
                                    if (!thirdDimUniform) break;
                                }
                                
                                if (thirdDimUniform)
                                {
                                    dimensions.Add(uniformLength);
                                    dimensions.Add(thirdDimLength);
                                }
                                else
                                {
                                    // Jagged in third dimension - only add dimensions that are uniform
                                    dimensions.Add(uniformLength);
                                }
                            }
                            else
                            {
                                // 2D matrix - add second dimension
                                dimensions.Add(uniformLength);
                            }
                        }
                        else
                        {
                            // Jagged matrix - only add first dimension (rows)
                            // According to spec: "shape will be a vector of the lengths of the dimensions that do have uniform length"
                        }
                    }
                    
                    return new VectorValue(dimensions.Select(d => (K3Value)new IntegerValue(d)).ToList());
                }
            }
            
            return new VectorValue(new List<K3Value>(), "enumerate_int"); // For scalars - return !0 (empty integer vector)
        }

        private K3Value Enumerate(K3Value a)
        {
            if (a is IntegerValue intA)
            {
                var elements = new List<K3Value>();
                for (int i = 0; i < intA.Value; i++)
                {
                    elements.Add(new IntegerValue(i));
                }
                return new VectorValue(elements, intA.Value == 0 ? "enumerate_int" : "standard");
            }
            else if (a is LongValue longA)
            {
                var elements = new List<K3Value>();
                for (long i = 0; i < longA.Value; i++)
                {
                    elements.Add(new LongValue(i));
                }
                return new VectorValue(elements, longA.Value == 0 ? "enumerate_long" : "standard");
            }
            else if (a is DictionaryValue dict)
            {
                // Enumerate operator on dictionary returns list of keys
                var keys = new List<K3Value>();
                foreach (var key in dict.Entries.Keys)
                {
                    keys.Add(key);
                }
                return new VectorValue(keys, "standard");
            }
            
            throw new Exception($"Cannot enumerate {a.Type}");
        }

        private K3Value Enlist(K3Value a)
        {
            var elements = new List<K3Value> { a };
            return new VectorValue(elements);
        }

        private K3Value Count(K3Value a)
        {
            if (a is VectorValue vecA)
                return new IntegerValue(vecA.Elements.Count);
            
            return new IntegerValue(1); // For scalars
        }

        private K3Value Floor(K3Value a)
        {
            if (a is IntegerValue intA)
                return intA;
            if (a is LongValue longA)
                return longA;
            if (a is FloatValue floatA)
                return new FloatValue(Math.Floor(floatA.Value));
            
            throw new Exception($"Cannot floor {a.Type}");
        }

        private K3Value Unique(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var uniqueElements = new List<K3Value>();
                var seen = new HashSet<string>();
                
                foreach (var element in vecA.Elements)
                {
                    var key = element.ToString();
                    if (seen.Add(key))
                    {
                        uniqueElements.Add(element);
                    }
                }
                
                return new VectorValue(uniqueElements);
            }
            
            return a; // For scalars, return the value itself
        }

        private K3Value Group(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var groups = new Dictionary<string, List<int>>();
                
                // First pass: collect indices for each unique value
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    var element = vecA.Elements[i];
                    var key = element.ToString();
                    
                    if (!groups.ContainsKey(key))
                    {
                        groups[key] = new List<int>();
                    }
                    groups[key].Add(i);
                }
                
                // Second pass: create group vectors in order of first appearance
                var result = new List<K3Value>();
                var seenKeys = new HashSet<string>();
                
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    var element = vecA.Elements[i];
                    var key = element.ToString();
                    
                    if (seenKeys.Add(key)) // First time seeing this value
                    {
                        // Create vector of indices for this group
                        var indices = groups[key].Select(idx => (K3Value)new IntegerValue(idx)).ToList();
                        result.Add(new VectorValue(indices));
                    }
                }
                
                return new VectorValue(result);
            }
            
            return a; // For scalars, return the value itself
        }

        // Binary versions for operators that can be both unary and binary
        private K3Value CountBinary(K3Value a, K3Value b)
        {
            return Count(a);
        }

        private K3Value FloorBinary(K3Value left, K3Value right)
        {
            // Enhanced _ operator with multiple behaviors
            if (left is VectorValue leftVec && right is VectorValue rightVec)
            {
                // Cut operation: cut vector at specified indices
                var result = new List<K3Value>();
                int prevIndex = 0;
                
                foreach (var element in leftVec.Elements)
                {
                    if (element is IntegerValue cutPoint)
                    {
                        if (cutPoint.Value < 0)
                            throw new Exception("Cut operation cannot contain negative indices");
                        
                        if (cutPoint.Value > prevIndex && cutPoint.Value <= rightVec.Elements.Count)
                        {
                            var subVector = new List<K3Value>();
                            for (int i = prevIndex; i < cutPoint.Value && i < rightVec.Elements.Count; i++)
                            {
                                subVector.Add(rightVec.Elements[i]);
                            }
                            result.Add(new VectorValue(subVector));
                        }
                        prevIndex = cutPoint.Value;
                    }
                    else
                    {
                        throw new Exception("Cut operation requires integer indices");
                    }
                }
                
                // Add remaining elements
                if (prevIndex < rightVec.Elements.Count)
                {
                    var subVector = new List<K3Value>();
                    for (int i = prevIndex; i < rightVec.Elements.Count; i++)
                    {
                        subVector.Add(rightVec.Elements[i]);
                    }
                    result.Add(new VectorValue(subVector));
                }
                
                return new VectorValue(result);
            }
            else if (left is IntegerValue leftInt && right is VectorValue dropVec)
            {
                // Drop operation: drop N elements from start or end
                int dropCount = leftInt.Value;
                int size = dropVec.Elements.Count;
                
                if (dropCount >= 0)
                {
                    // Drop from start
                    if (dropCount >= size)
                        return new VectorValue(new List<K3Value>());
                    
                    var result = new List<K3Value>();
                    for (int i = dropCount; i < size; i++)
                    {
                        result.Add(dropVec.Elements[i]);
                    }
                    return new VectorValue(result);
                }
                else
                {
                    // Drop from end (negative count)
                    int dropFromEnd = -dropCount;
                    if (dropFromEnd >= size)
                        return new VectorValue(new List<K3Value>());
                    
                    var result = new List<K3Value>();
                    for (int i = 0; i < size - dropFromEnd; i++)
                    {
                        result.Add(dropVec.Elements[i]);
                    }
                    return new VectorValue(result);
                }
            }
            else
            {
                throw new Exception("Drop/Cut operation requires vector arguments or integer+vector");
            }
        }

        private K3Value UniqueBinary(K3Value a, K3Value b)
        {
            return Unique(a);
        }

        private K3Value VectorIndex(K3Value vector, K3Value index)
        {
            // Handle vector indexing: vector @ index
            // If vector is null, return the index (spec: _n@x returns x)
            if (vector is NullValue)
            {
                return index;
            }
            
            if (vector is VectorValue vec)
            {
                // Handle null indexing (_n) - "all" operation
                if (index is NullValue)
                {
                    // Return all elements of the vector
                    return new VectorValue(vec.Elements);
                }
                else if (index is IntegerValue intIndex)
                {
                    // Single index: return element at position
                    int idx = intIndex.Value;
                    if (idx < 0 || idx >= vec.Elements.Count)
                    {
                        throw new Exception($"Index {idx} out of bounds for vector of length {vec.Elements.Count}");
                    }
                    return vec.Elements[idx];
                }
                else if (index is VectorValue indexVec)
                {
                    // Multiple indices: return vector of elements at specified positions
                    var result = new List<K3Value>();
                    foreach (var idxValue in indexVec.Elements)
                    {
                        if (idxValue is IntegerValue intIdx)
                        {
                            int idx = intIdx.Value;
                            if (idx < 0 || idx >= vec.Elements.Count)
                            {
                                throw new Exception($"Index {idx} out of bounds for vector of length {vec.Elements.Count}");
                            }
                            result.Add(vec.Elements[idx]);
                        }
                        else
                        {
                            throw new Exception($"Vector indices must be integers, got {idxValue.Type}");
                        }
                    }
                    return new VectorValue(result);
                }
                else
                {
                    throw new Exception($"Index must be integer or vector of integers, got {index.Type}");
                }
            }
            else if (vector is DictionaryValue dict)
            {
                // Handle null indexing (_n) - "all" operation for dictionaries
                if (index is NullValue)
                {
                    // Return all values of the dictionary
                    var values = new List<K3Value>();
                    foreach (var entry in dict.Entries)
                    {
                        values.Add(entry.Value.Value);
                    }
                    return new VectorValue(values);
                }
                // Handle dictionary indexing: dictionary @ key
                else if (index is SymbolValue key)
                {
                    // Check if key ends with period for attribute retrieval
                    bool getAttribute = key.Value.EndsWith(".");
                    string lookupKey = getAttribute ? key.Value.Substring(0, key.Value.Length - 1) : key.Value;
                    var lookupSymbol = new SymbolValue(lookupKey);
                    
                    // Single key lookup
                    if (dict.Entries.TryGetValue(lookupSymbol, out var entry))
                    {
                        if (getAttribute)
                        {
                            // Return attributes (null if no attributes)
                            return entry.Attribute ?? new DictionaryValue();
                        }
                        else
                        {
                            // Return value
                            return entry.Value;
                        }
                    }
                    else
                    {
                        throw new Exception($"Key '{lookupKey}' not found in dictionary");
                    }
                }
                else if (index is VectorValue keyVec)
                {
                    // Multiple keys lookup: return vector of values or attributes
                    var result = new List<K3Value>();
                    foreach (var keyElement in keyVec.Elements)
                    {
                        if (keyElement is SymbolValue symbolKey)
                        {
                            // Check if key ends with period for attribute retrieval
                            bool getAttribute = symbolKey.Value.EndsWith(".");
                            string lookupKey = getAttribute ? symbolKey.Value.Substring(0, symbolKey.Value.Length - 1) : symbolKey.Value;
                            var lookupSymbol = new SymbolValue(lookupKey);
                            
                            if (dict.Entries.TryGetValue(lookupSymbol, out var entry))
                            {
                                if (getAttribute)
                                {
                                    // Return attributes (null if no attributes)
                                    result.Add(entry.Attribute ?? new DictionaryValue());
                                }
                                else
                                {
                                    // Return value
                                    result.Add(entry.Value);
                                }
                            }
                            else
                            {
                                throw new Exception($"Key '{lookupKey}' not found in dictionary");
                            }
                        }
                        else
                        {
                            throw new Exception($"Dictionary keys must be symbols, got {keyElement.Type}");
                        }
                    }
                    return new VectorValue(result);
                }
                else
                {
                    throw new Exception($"Dictionary index must be symbol or vector of symbols, got {index.Type}");
                }
            }
            else
            {
                throw new Exception($"Cannot index into type: {vector.Type}");
            }
        }

        private K3Value Over(K3Value verb, K3Value initialization, K3Value data)
        {
            // Handle vector case (over)
            if (data is VectorValue dataVec)
            {
                // Special case: empty vector
                if (dataVec.Elements.Count == 0)
                {
                    // For +/!0 and +/!0L, return 0 (identity element for addition)
                    if (verb is SymbolValue verbSymbol && verbSymbol.Value == "+")
                    {
                        return new IntegerValue(0);
                    }
                    // For */!0 and */!0L, return 1 (identity element for multiplication)
                    else if (verb is SymbolValue verbSymbolMul && verbSymbolMul.Value == "*")
                    {
                        return new IntegerValue(1);
                    }
                    // For other verbs with empty vectors, return the initialization value
                    else
                    {
                        return initialization;
                    }
                }
                
                K3Value result;
                
                // If initialization is 0, use first element as initialization (K behavior for / without explicit init)
                if (initialization is IntegerValue intInit && intInit.Value == 0 && dataVec.Elements.Count > 0)
                {
                    result = dataVec.Elements[0]; // Use first element as starting point
                    var startIndex = 1; // Start from second element
                    
                    if (verb is SymbolValue verbSymbol)
                    {
                        // Apply verb to remaining elements
                        for (int i = startIndex; i < dataVec.Elements.Count; i++)
                        {
                            result = ApplyVerb(verbSymbol.Value, result, dataVec.Elements[i]);
                        }
                    }
                    else
                    {
                        // If verb is not a symbol, treat it as a value to apply with the operator
                        for (int i = startIndex; i < dataVec.Elements.Count; i++)
                        {
                            result = ApplyVerbWithOperator(verb, result, dataVec.Elements[i]);
                        }
                    }
                }
                else
                {
                    // Use provided initialization value
                    result = initialization;
                    
                    if (verb is SymbolValue verbSymbol)
                    {
                        // Apply verb to each element of the vector, accumulating the result
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            result = ApplyVerb(verbSymbol.Value, result, dataVec.Elements[i]);
                        }
                    }
                    else
                    {
                        // If verb is not a symbol, treat it as a value to apply with the operator
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            result = ApplyVerbWithOperator(verb, result, dataVec.Elements[i]);
                        }
                    }
                }
                
                return result;
            }
            
            // Handle scalar case
            if (IsScalar(data))
            {
                return data;
            }
            
            throw new Exception($"Over not implemented for types: {verb.Type}, {data.Type}");
        }

        private K3Value ApplyVerb(string verbName, K3Value left, K3Value right)
        {
            return verbName switch
            {
                "+" => Plus(left, right),
                "-" => Minus(left, right),
                "*" => Times(left, right),
                "%" => Divide(left, right),
                "&" => Min(left, right),
                "|" => Max(left, right),
                "<" => Less(left, right),
                ">" => More(left, right),
                "=" => Match(left, right),
                "^" => Power(left, right),
                "!" => ModRotate(left, right),
                "," => Join(left, right),
                _ => throw new Exception($"Unknown operator: {verbName}")
            };
        }

        private K3Value ApplyUnaryVerb(string verbName, K3Value operand)
        {
            return verbName switch
            {
                "+" => operand,  // Identity operation
                "-" => Negate(operand),
                "*" => First(operand),
                "%" => Reciprocal(operand),
                "&" => operand,  // Identity operation
                "|" => Reverse(operand),
                "^" => Shape(operand),
                "!" => Enumerate(operand),
                "," => Enlist(operand),
                "#" => Count(operand),
                "_" => Floor(operand),
                "?" => Unique(operand),
                "=" => Group(operand),
                "~" => operand is SymbolValue || (operand is VectorValue vec && vec.Elements.All(e => e is SymbolValue)) 
                    ? AttributeHandle(operand) 
                    : LogicalNegate(operand),
                _ => throw new Exception($"Unknown unary verb: {verbName}")
            };
        }

        private K3Value ApplyVerbWithOperator(K3Value verb, K3Value left, K3Value right)
        {
            // Handle case where verb is a value (like 2 +/ 1 2 3)
            // This means we should use the verb as the left operand with the operator
            if (verb is SymbolValue verbSymbol)
            {
                return ApplyVerb(verbSymbol.Value, verb, right);
            }
            else
            {
                // For numeric verbs, assume addition by default
                // But check if this is actually a glyph verb stored as a different type
                if (verb.Type == ValueType.Symbol || 
                    (verb.Type == ValueType.Integer && verb.ToString().Length == 1 && "+-*/%^!&|<>=^,_?#~".Contains(verb.ToString())))
                {
                    return ApplyVerb(verb.ToString(), verb, right);
                }
                else
                {
                    return Plus(verb, right);
                }
            }
        }

        private K3Value ApplyVerbToScalarAndVector(K3Value scalar, K3Value verb, K3Value vectorElement)
        {
            if (verb is SymbolValue verbSymbol)
            {
                return ApplyVerb(verbSymbol.Value, scalar, vectorElement);
            }
            else
            {
                // Check if verb is a glyph stored as non-symbol type
                string verbStr = verb.ToString();
                if (verbStr.Length == 1 && "+-*/%^!&|<>=^,_?#~".Contains(verbStr))
                {
                    return ApplyVerb(verbStr, scalar, vectorElement);
                }
                else
                {
                    throw new Exception($"Cannot apply verb {verb} to scalar and vector element");
                }
            }
        }

        private K3Value Scan(K3Value verb, K3Value initialization, K3Value data)
        {
            // Handle vector case with initialization
            if (data is VectorValue dataVec && dataVec.Elements.Count > 0)
            {
                var result = new List<K3Value>();
                K3Value current;
                
                // If initialization is 0, use first element as initialization (K behavior for \ without explicit init)
                if (initialization is IntegerValue intInit && intInit.Value == 0 && dataVec.Elements.Count > 0)
                {
                    current = dataVec.Elements[0]; // Use first element as starting point
                    result.Add(current); // Add first element to result
                    
                    var startIndex = 1; // Start from second element
                    
                    if (verb is SymbolValue verbSymbol)
                    {
                        // Apply verb to remaining elements
                        for (int i = startIndex; i < dataVec.Elements.Count; i++)
                        {
                            current = ApplyVerb(verbSymbol.Value, current, dataVec.Elements[i]);
                            result.Add(current);
                        }
                    }
                    else
                    {
                        // If verb is not a symbol, treat it as a value to apply with the operator
                        for (int i = startIndex; i < dataVec.Elements.Count; i++)
                        {
                            current = ApplyVerbWithOperator(verb, current, dataVec.Elements[i]);
                            result.Add(current);
                        }
                    }
                }
                else
                {
                    // Use provided initialization value
                    current = initialization;
                    
                    // Add the initialization value as the first element
                    result.Add(current);
                    
                    if (verb is SymbolValue verbSymbol)
                    {
                        // Apply verb to each element, accumulating the result
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            current = ApplyVerb(verbSymbol.Value, current, dataVec.Elements[i]);
                            result.Add(current);
                        }
                    }
                    else
                    {
                        // If verb is not a symbol, treat it as a value to apply with the operator
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            current = ApplyVerbWithOperator(verb, current, dataVec.Elements[i]);
                            result.Add(current);
                        }
                    }
                }
                
                return new VectorValue(result, "standard");
            }
            
            return data;
        }

        private K3Value Each(K3Value verb, K3Value left, K3Value right)
        {
            // New structure: Each(verbSymbol, leftVector, rightVector)
            if (verb is SymbolValue verbSymbol)
            {
                // Handle vector + vector case (same length) - should behave like default operator
                if (left is VectorValue leftVec && right is VectorValue rightVec)
                {
                    // Check if vectors have different lengths - should throw length error
                    if (leftVec.Elements.Count != rightVec.Elements.Count)
                    {
                        throw new Exception($"length error: {leftVec.Elements.Count} != {rightVec.Elements.Count}");
                    }
                    
                    // Apply binary operation element-wise (same as default operator behavior)
                    var result = new List<K3Value>();
                    for (int i = 0; i < leftVec.Elements.Count; i++)
                    {
                        var leftElement = leftVec.Elements[i];
                        var rightElement = rightVec.Elements[i];
                        
                        result.Add(ApplyVerb(verbSymbol.Value, leftElement, rightElement));
                    }
                    return new VectorValue(result);
                }
                
                // Handle scalar + vector case
                if (IsScalar(left) && right is VectorValue vec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in vec.Elements)
                    {
                        result.Add(ApplyVerb(verbSymbol.Value, left, element));
                    }
                    return new VectorValue(result);
                }
                
                // Handle scalar + scalar case
                if (IsScalar(left) && IsScalar(right))
                {
                    return ApplyVerb(verbSymbol.Value, left, right);
                }
            }
            
            throw new Exception($"Each not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachRight(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Right (/:): Apply verb to each element of left with entire right
            if (verb is SymbolValue verbSymbol)
            {
                if (left is VectorValue leftVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in leftVec.Elements)
                    {
                        result.Add(ApplyVerb(verbSymbol.Value, element, right));
                    }
                    return new VectorValue(result);
                }
                else if (IsScalar(left))
                {
                    return ApplyVerb(verbSymbol.Value, left, right);
                }
            }
            
            throw new Exception($"EachRight not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachLeft(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Left (\:): Apply verb to entire left with each element of right
            if (verb is SymbolValue verbSymbol)
            {
                if (right is VectorValue rightVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in rightVec.Elements)
                    {
                        result.Add(ApplyVerb(verbSymbol.Value, left, element));
                    }
                    return new VectorValue(result);
                }
                else if (IsScalar(right))
                {
                    return ApplyVerb(verbSymbol.Value, left, right);
                }
            }
            
            throw new Exception($"EachLeft not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachPrior(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Prior (':): Apply verb to each element with previous element
            if (verb is SymbolValue verbSymbol)
            {
                if (right is VectorValue rightVec)
                {
                    var result = new List<K3Value>();
                    for (int i = 0; i < rightVec.Elements.Count; i++)
                    {
                        var current = rightVec.Elements[i];
                        var prior = i > 0 ? rightVec.Elements[i - 1] : left;
                        // Only add to result if i > 0 (skip first element)
                        if (i > 0)
                        {
                            result.Add(ApplyVerb(verbSymbol.Value, current, prior));
                        }
                    }
                    return new VectorValue(result);
                }
                else if (IsScalar(right))
                {
                    return ApplyVerb(verbSymbol.Value, right, left);
                }
            }
            
            throw new Exception($"EachPrior not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value Each(K3Value verb, K3Value data)
        {
            // Legacy 2-argument call for backward compatibility
            if (verb is VectorValue verbVec && data is VectorValue dataVec)
            {
                // Check if vectors have different lengths - should throw length error
                if (verbVec.Elements.Count != dataVec.Elements.Count)
                {
                    throw new Exception($"length error: {verbVec.Elements.Count} != {dataVec.Elements.Count}");
                }
                
                // Apply binary operation element-wise
                var result = new List<K3Value>();
                for (int i = 0; i < verbVec.Elements.Count; i++)
                {
                    var left = verbVec.Elements[i];
                    var right = dataVec.Elements[i];
                    
                    // Determine the operation based on the verb type
                    if (verb is SymbolValue verbSymbol)
                    {
                        result.Add(ApplyVerb(verbSymbol.Value, left, right));
                    }
                    else
                    {
                        // Handle case where verb is a scalar value (for mixed operations)
                        result.Add(ApplyVerbWithOperator(verb, left, right));
                    }
                }
                return new VectorValue(result);
            }
            
            // Handle scalar + vector case (legacy)
            if (IsScalar(verb) && data is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    if (verb is SymbolValue verbSymbol)
                    {
                        // For each operations, apply the verb as a unary operation to each element
                        result.Add(ApplyUnaryVerb(verbSymbol.Value, element));
                    }
                    else
                    {
                        // Check if verb is a glyph stored as non-vector type
                        string verbStr = verb.ToString();
                        if (verbStr.Length == 1 && "+-*/%^!&|<>=^,_?#~".Contains(verbStr))
                        {
                            result.Add(ApplyUnaryVerb(verbStr, element));
                        }
                        else
                        {
                            result.Add(ApplyVerbWithOperator(verb, element, null));
                        }
                    }
                }
                return new VectorValue(result);
            }
            
            // Handle scalar + scalar case (legacy)
            if (IsScalar(verb) && IsScalar(data))
            {
                if (verb is SymbolValue verbSymbol)
                {
                    return ApplyVerb(verbSymbol.Value, verb, data);
                }
                else
                {
                    return ApplyVerbWithOperator(verb, verb, data);
                }
            }
            
            throw new Exception($"Each not implemented for types: {verb.Type}, {data.Type}");
        }

        private bool IsScalar(K3Value value)
        {
            return value is IntegerValue || value is LongValue || value is FloatValue || 
                   value is CharacterValue || value is SymbolValue || value is NullValue;
        }

        private K3Value GetType(K3Value left, K3Value right)
        {
            // 4: operator - right operand is ignored, returns type code of left operand
            return GetTypeCode(left);
        }

        private K3Value GetTypeCode(K3Value value)
        {
            if (value is IntegerValue)
                return new IntegerValue(1);
            if (value is LongValue)
                return new IntegerValue(64);
            if (value is FloatValue)
                return new IntegerValue(2);
            if (value is CharacterValue)
                return new IntegerValue(3);
            if (value is SymbolValue)
                return new IntegerValue(4);
            if (value is DictionaryValue)
                return new IntegerValue(5);
            if (value is NullValue)
                return new IntegerValue(6);
            if (value is FunctionValue)
                return new IntegerValue(7);
            if (value is VectorValue vec)
            {
                // Check vector type
                if (vec.Elements.Count == 0)
                    return new IntegerValue(0); // Empty vector is generic list
                
                // Check if all elements are the same type
                var firstType = vec.Elements[0].Type;
                bool allSameType = true;
                bool hasNulls = false;
                
                foreach (var element in vec.Elements)
                {
                    if (element.Type != firstType)
                    {
                        allSameType = false;
                        break;
                    }
                    if (element.Type == ValueType.Null)
                    {
                        hasNulls = true;
                    }
                }
                
                if (!allSameType)
                    return new IntegerValue(0); // Mixed type vector
                
                if (hasNulls)
                    return new IntegerValue(0); // Vector with nulls is generic list
                
                // Return vector type code
                return firstType switch
                {
                    ValueType.Integer => new IntegerValue(-1),
                    ValueType.Long => new IntegerValue(-64),
                    ValueType.Float => new IntegerValue(-2),
                    ValueType.Character => new IntegerValue(-3),
                    ValueType.Symbol => new IntegerValue(-4),
                    ValueType.Function => new IntegerValue(-7),
                    _ => new IntegerValue(0)
                };
            }
            
            return new IntegerValue(0); // Default to generic list
        }
        
        private K3Value StringRepresentation(K3Value value)
        {
            // 5: verb - produce string representation of the argument with proper escaping
            string representation = ToStringWithEscaping(value);
            return new VectorValue(new List<K3Value> { new CharacterValue(representation) }, "string_representation");
        }
        
        private string ToStringWithEscaping(K3Value value)
        {
            if (value is CharacterValue charVal)
            {
                // For character values, escape the result of ToString() which already includes quotes
                return EscapeString(charVal.ToString());
            }
            else if (value is VectorValue vec)
            {
                // Handle vector representation with escaping
                return VectorToStringWithEscaping(vec);
            }
            else
            {
                // For other types, use regular ToString
                return value.ToString();
            }
        }
        
        private string EscapeString(string input)
        {
            var result = new System.Text.StringBuilder();
            
            foreach (char c in input)
            {
                switch (c)
                {
                    case '"':
                        result.Append("\\\"");
                        break;
                    case '\\':
                        result.Append("\\\\");
                        break;
                    case '\n':
                        result.Append("\\n");
                        break;
                    case '\r':
                        result.Append("\\r");
                        break;
                    case '\t':
                        result.Append("\\t");
                        break;
                    case '\b':
                        result.Append("\\b");
                        break;
                    case '\f':
                        result.Append("\\f");
                        break;
                    default:
                        if (c < ' ' || c > '~')
                        {
                            // Non-printable characters as octal escape
                            result.Append($"\\{(int)c:o3}");
                        }
                        else
                        {
                            result.Append(c);
                        }
                        break;
                }
            }
            
            return result.ToString();
        }
        
        private string VectorToStringWithEscaping(VectorValue vec)
        {
            if (vec.Elements.Count == 0)
            {
                return "()";
            }
            
            // Check if this is a character vector - display as quoted string with escaping
            if (vec.Elements.All(e => e is CharacterValue))
            {
                var chars = vec.Elements.Select(e => ((CharacterValue)e).Value);
                var content = string.Concat(chars);
                return $"\"{EscapeString(content)}\"";
            }
            
            // Check if this is a symbol vector - display in compact format without spaces
            if (vec.Elements.All(e => e is SymbolValue))
            {
                var symbols = vec.Elements.Select(e => ((SymbolValue)e).ToString());
                return string.Concat(symbols);
            }
            
            // Check if this is a mixed vector - keep parentheses and semicolons for clarity
            var elementTypes = vec.Elements.Select(e => e.GetType()).Distinct().ToList();
            var hasNestedVectors = vec.Elements.Any(e => e is VectorValue);
            var hasNullValues = vec.Elements.Any(e => e is NullValue);
            
            // Check if this is truly mixed (different types) OR has nested vectors OR has null values
            var isTrulyMixed = elementTypes.Count > 1 || hasNestedVectors || hasNullValues;
            
            if (isTrulyMixed)
            {
                var elementsStr = string.Join(";", vec.Elements.Select(e => 
                {
                    if (e is NullValue)
                    {
                        return ""; // Display null as empty position
                    }
                    else if (e is VectorValue innerVec)
                    {
                        // For character vectors in mixed context, treat as single string element with escaping
                        if (innerVec.Elements.All(x => x is CharacterValue))
                        {
                            var chars = innerVec.Elements.Select(x => ((CharacterValue)x).Value);
                            var content = string.Concat(chars);
                            return $"\"{EscapeString(content)}\"";
                        }
                        // For simple homogeneous vectors, don't add inner parentheses
                        else if (innerVec.Elements.All(x => x is IntegerValue) || 
                            innerVec.Elements.All(x => x is FloatValue) ||
                            innerVec.Elements.All(x => x is LongValue))
                        {
                            return VectorToStringWithEscaping(innerVec);
                        }
                        return VectorToStringWithEscaping(innerVec);
                    }
                    return ToStringWithEscaping(e);
                }));
                return "(" + elementsStr + ")";
            }
            
            // For homogeneous vectors (except characters), use space-separated format
            var vectorStr = string.Join(" ", vec.Elements.Select(e => e.ToString()));
            
            // Add enlist comma for single-element vectors of integer, symbol, and character types
            if (vec.Elements.Count == 1 && 
                (vec.Elements[0] is IntegerValue || vec.Elements[0] is SymbolValue || vec.Elements[0] is CharacterValue))
            {
                return "," + vectorStr;
            }
            
            return vectorStr;
        }
        
        private K3Value Make(K3Value value)
        {
            // . (make) operator - create dictionary from mixed vector OR execute character vector
            // IMPORTANT: Check for character vector FIRST, then handle dictionaries and other vectors
            
            // Check if this is specifically a character vector (all elements are CharacterValue)
            if (value is VectorValue charVec && charVec.Elements.Count > 0 && charVec.Elements.All(e => e is CharacterValue))
            {
                // Execute operation: ."text" executes the text as if it was entered in the parser
                // IMPORTANT: Dot execute works in the current variable context (no new variable space)
                var chars = charVec.Elements.Select(e => ((CharacterValue)e).Value);
                var executeText = string.Concat(chars);
                
                // Create a new parser instance but use the current evaluator (current variable context)
                var lexer = new Lexer(executeText);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens, executeText);
                
                try
                {
                    var ast = parser.Parse();
                    // Use current evaluator (this) to maintain variable context
                    return Evaluate(ast);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Execute operation failed: {ex.Message}", ex);
                }
            }
            else if (value is DictionaryValue dict)
            {
                // Unmake operation: .dictionary returns list of triplets
                var triplets = new List<K3Value>();
                
                foreach (var kvp in dict.Entries)
                {
                    var key = kvp.Key;
                    var (val, attr) = kvp.Value;
                    
                    // Create triplet vector: (key; value; attribute)
                    var tripletElements = new List<K3Value> { key, val };
                    
                    // Add attribute (null if no attribute)
                    if (attr != null)
                    {
                        tripletElements.Add(attr);
                    }
                    else
                    {
                        // Add null for missing attribute
                        tripletElements.Add(new NullValue());
                    }
                    
                    triplets.Add(new VectorValue(tripletElements));
                }
                
                return new VectorValue(triplets);
            }
            else if (value is VectorValue vec)
            {
                // Make operation: .() creates empty dictionary, .(elements) creates dictionary
                var newDict = new Dictionary<SymbolValue, (K3Value, DictionaryValue)>();
                
                foreach (var element in vec.Elements)
                {
                    if (element is VectorValue entryVec)
                    {
                        if (entryVec.Elements.Count == 2)
                        {
                            // Tuple (key; value) - attribute is null
                            if (entryVec.Elements[0] is SymbolValue key)
                            {
                                newDict[key] = (entryVec.Elements[1], null);
                            }
                            else
                            {
                                throw new Exception("Dictionary key must be a symbol");
                            }
                        }
                        else if (entryVec.Elements.Count == 3)
                        {
                            // Triplet (key; value; attribute)
                            if (entryVec.Elements[0] is SymbolValue key)
                            {
                                var attribute = entryVec.Elements[2] as DictionaryValue;
                                newDict[key] = (entryVec.Elements[1], attribute);
                            }
                            else
                            {
                                throw new Exception("Dictionary key must be a symbol");
                            }
                        }
                        else
                        {
                            throw new Exception("Dictionary entry must be a tuple (2 elements) or triplet (3 elements)");
                        }
                    }
                    else
                    {
                        throw new Exception("Dictionary entries must be vectors");
                    }
                }
                
                return new DictionaryValue(newDict);
            }
            else
            {
                throw new Exception("Make operator requires a vector or dictionary");
            }
        }
        
        private K3Value Take(K3Value count, K3Value data)
        {
            if (count is IntegerValue intCount)
            {
                if (data is VectorValue dataVec)
                {
                    // Take from vector
                    var actualCount = Math.Max(0, intCount.Value);
                    var result = new List<K3Value>();
                    
                    if (dataVec.Elements.Count == 0)
                    {
                        // Empty source vector - return empty result
                        return new VectorValue(result, "standard");
                    }
                    
                    // Repeat the source vector periodically to fill the requested count
                    for (int i = 0; i < actualCount; i++)
                    {
                        var sourceIndex = i % dataVec.Elements.Count;
                        result.Add(dataVec.Elements[sourceIndex]);
                    }
                    
                    // Determine creation method for empty vectors
                    string creationMethod = "standard";
                    if (result.Count == 0)
                    {
                        if (dataVec.Elements.Count > 0 && dataVec.Elements[0] is FloatValue)
                            creationMethod = "take_float";
                        else if (dataVec.Elements.Count > 0 && dataVec.Elements[0] is SymbolValue)
                            creationMethod = "take_symbol";
                    }
                    
                    return new VectorValue(result, creationMethod);
                }
                else
                {
                    // Take from scalar - create vector with the scalar repeated
                    var actualCount = Math.Max(0, intCount.Value);
                    var result = new List<K3Value>();
                    
                    for (int i = 0; i < actualCount; i++)
                    {
                        result.Add(data);
                    }
                    
                    // Determine creation method
                    string creationMethod = "standard";
                    if (result.Count == 0)
                    {
                        if (data is FloatValue)
                            creationMethod = "take_float";
                        else if (data is SymbolValue)
                            creationMethod = "take_symbol";
                    }
                    
                    return new VectorValue(result, creationMethod);
                }
            }
            else if (count is LongValue longCount)
            {
                // Handle long count by converting to integer
                return Take(new IntegerValue((int)longCount.Value), data);
            }
            else
            {
                throw new Exception("Take count must be an integer");
            }
        }

        private K3Value AtIndex(K3Value left, K3Value right)
        {
            // Check if this is Amend Item operation: @[d; i; f; y] or @[d; i; f]
            // This happens when left is null (from bracket notation) or when left is the at symbol
            if ((left is NullValue || (left is SymbolValue sym && sym.Value == "@")) && 
                right is VectorValue args && args.Elements.Count >= 3)
            {
                return AmendItemFunction(args.Elements);
            }
            
            // @ operator for indexing: data @ index
            // If data is null, return the index (spec: _n@x returns x)
            if (left is NullValue)
            {
                return right;
            }
            
            // Regular indexing operation
            return AtIndexOperation(left, right);
        }

        private K3Value AtIndexOperation(K3Value data, K3Value index)
        {
            // Handle dictionary indexing
            if (data is DictionaryValue dict)
            {
                if (index is SymbolValue symbol)
                {
                    // Check if this is attribute access (symbol ends with .)
                    if (symbol.Value.EndsWith("."))
                    {
                        // Remove the trailing . to get the key name
                        var keyName = symbol.Value.Substring(0, symbol.Value.Length - 1);
                        var keySymbol = new SymbolValue(keyName);
                        
                        foreach (var entry in dict.Entries)
                        {
                            if (entry.Key.Equals(keySymbol))
                            {
                                return entry.Value.Attribute; // Return Attribute from tuple
                            }
                        }
                        throw new Exception($"Key '{keyName}' not found in dictionary");
                    }
                    else
                    {
                        // Dictionary @ symbol - get value by key
                        foreach (var entry in dict.Entries)
                        {
                            if (entry.Key.Equals(symbol))
                            {
                                return entry.Value.Value; // Extract Value from tuple
                            }
                        }
                        throw new Exception($"Key '{symbol.Value}' not found in dictionary");
                    }
                }
                else if (index is VectorValue indexVec)
                {
                    // Dictionary @ vector of symbols - get multiple values
                    var result = new List<K3Value>();
                    foreach (var idx in indexVec.Elements)
                    {
                        if (idx is SymbolValue sym)
                        {
                            bool found = false;
                            foreach (var entry in dict.Entries)
                            {
                                if (entry.Key.Equals(sym))
                                {
                                    result.Add(entry.Value.Value); // Extract Value from tuple
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                throw new Exception($"Key '{sym.Value}' not found in dictionary");
                            }
                        }
                        else
                        {
                            throw new Exception($"Dictionary indices must be symbols, got {idx.Type}");
                        }
                    }
                    return new VectorValue(result);
                }
                else if (index is NullValue)
                {
                    // Handle null indexing (_n) - "all" operation for dictionaries
                    var values = new List<K3Value>();
                    foreach (var entry in dict.Entries)
                    {
                        values.Add(entry.Value.Value);
                    }
                    return new VectorValue(values);
                }
                else
                {
                    throw new Exception($"Dictionary index must be symbol, vector of symbols, or null, got {index.Type}");
                }
            }
            
            // Handle vector indexing
            if (data is VectorValue vec)
            {
                return VectorIndex(vec, index);
            }
            else
            {
                throw new Exception("Index must be integer, symbol, or vector of integers/symbols");
            }
        }

        private K3Value Atom(K3Value operand)
        {
            // @ operator: returns 1 if scalar, 0 if vector
            if (operand is VectorValue)
                return new IntegerValue(0);
            else
                return new IntegerValue(1);
        }

        private K3Value AttributeHandle(K3Value operand)
        {
            // ~ operator for symbols: adds period suffix
            if (operand is SymbolValue symbol)
            {
                return new SymbolValue(symbol.Value + ".");
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    if (element is SymbolValue sym)
                    {
                        result.Add(new SymbolValue(sym.Value + "."));
                    }
                    else
                    {
                        throw new Exception("Attribute handle can only be applied to symbols or vectors of symbols");
                    }
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("Attribute handle can only be applied to symbols or vectors of symbols");
            }
        }

        // Mathematical floating point operations
        private K3Value MathLog(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value == 0)
                    return new FloatValue(double.NegativeInfinity); // -0i
                if (intValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Log(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value == 0)
                    return new FloatValue(double.NegativeInfinity); // -0i
                if (longValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Log(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value == 0)
                    return new FloatValue(double.NegativeInfinity); // -0i
                if (floatValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Log(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathLog(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_log can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathExp(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Exp(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Exp(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Exp(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathExp(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_exp can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathAbs(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Abs(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Abs(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Abs(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAbs(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_abs can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathSqr(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(intValue.Value * intValue.Value);
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(longValue.Value * longValue.Value);
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(floatValue.Value * floatValue.Value);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSqr(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_sqr can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathSqrt(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Sqrt(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Sqrt(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Sqrt(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSqrt(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_sqrt can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathFloor(K3Value operand)
        {
            // Mathematical floor operation that always returns floating point values
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Floor((double)intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Floor((double)longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Floor(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathFloor(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_floor can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathDot(K3Value operand)
        {
            // Linear algebra dot product operation
            if (operand is VectorValue vec)
            {
                if (vec.Elements.Count == 0)
                    return new FloatValue(0.0);
                
                double sum = 0.0;
                foreach (var element in vec.Elements)
                {
                    if (element is IntegerValue intVal)
                        sum += intVal.Value * intVal.Value;
                    else if (element is LongValue longVal)
                        sum += longVal.Value * longVal.Value;
                    else if (element is FloatValue floatVal)
                        sum += floatVal.Value * floatVal.Value;
                    else
                        throw new Exception("_dot requires numeric vector elements");
                }
                return new FloatValue(sum);
            }
            else if (operand is IntegerValue intValue)
            {
                return new FloatValue(intValue.Value * intValue.Value);
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(longValue.Value * longValue.Value);
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(floatValue.Value * floatValue.Value);
            }
            else
            {
                throw new Exception("_dot can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathMul(K3Value operand)
        {
            // Linear algebra matrix multiplication - for now implement as identity
            // Full matrix multiplication would require proper matrix representation
            return operand;
        }

        private K3Value MathInv(K3Value operand)
        {
            // Linear algebra matrix inverse - for now implement as element-wise reciprocal
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value == 0)
                    return new FloatValue(double.NaN); // 0n for singular matrix
                return new FloatValue(1.0 / intValue.Value);
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value == 0)
                    return new FloatValue(double.NaN); // 0n for singular matrix
                return new FloatValue(1.0 / longValue.Value);
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value == 0)
                    return new FloatValue(double.NaN); // 0n for singular matrix
                return new FloatValue(1.0 / floatValue.Value);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathInv(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_inv can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathSin(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Sin(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Sin(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Sin(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSin(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_sin can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathCos(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Cos(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Cos(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Cos(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathCos(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_cos can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathTan(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Tan(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Tan(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Tan(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathTan(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_tan can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathAsin(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < -1 || intValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Asin(intValue.Value);
                return new FloatValue(result);
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < -1 || longValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Asin(longValue.Value);
                return new FloatValue(result);
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < -1 || floatValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Asin(floatValue.Value);
                return new FloatValue(result);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAsin(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_asin can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathAcos(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < -1 || intValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Acos(intValue.Value);
                return new FloatValue(result);
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < -1 || longValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Acos(longValue.Value);
                return new FloatValue(result);
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < -1 || floatValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Acos(floatValue.Value);
                return new FloatValue(result);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAcos(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_acos can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathAtan(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Atan(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Atan(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Atan(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAtan(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_atan can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathSinh(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Sinh(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Sinh(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Sinh(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSinh(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_sinh can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathCosh(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Cosh(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Cosh(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Cosh(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathCosh(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_cosh can only be applied to numeric values or vectors");
            }
        }

        private K3Value MathTanh(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Tanh(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Tanh(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Tanh(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathTanh(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_tanh can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value DotApply(K3Value left, K3Value right)
        {
            // Check if this is Amend operation: .[d; i; f; y] or .[d; i; f]
            // This happens when left is null (from bracket notation) or when left is the dot symbol
            if ((left is NullValue || (left is SymbolValue sym && sym.Value == ".")) && 
                right is VectorValue args && args.Elements.Count >= 3)
            {
                return AmendFunction(args.Elements);
            }
            
            // Dot-apply operator: function . argument
            // Similar to function application but with different precedence
            // If left is null, return the right (spec: _n . x returns x)
            if (left is NullValue)
            {
                return right;
            }
            
            // Handle dictionary dot-apply with symbol vectors (spec: d@`v is equivalent to d .,`v)
            if (left is DictionaryValue dict)
            {
                if (right is SymbolValue symbol)
                {
                    // Single symbol lookup - same as dictionary indexing
                    return VectorIndex(dict, symbol);
                }
                else if (right is VectorValue rightVec && rightVec.Elements.Count == 1 && rightVec.Elements[0] is VectorValue symbolVec)
                {
                    // Single-item list containing a vector of symbols
                    return VectorIndex(dict, symbolVec);
                }
                else if (right is VectorValue vec && vec.Elements.All(e => e is SymbolValue))
                {
                    // Vector of symbols - same as dictionary indexing with vector
                    return VectorIndex(dict, vec);
                }
            }
            
            if (left is FunctionValue function)
            {
                // Create a temporary AST node for the function to reuse existing logic
                var tempFunctionNode = new ASTNode(ASTNodeType.Function);
                tempFunctionNode.Value = function;
                
                // Create arguments list
                var arguments = new List<K3Value> { right };
                
                return CallDirectFunction(tempFunctionNode, arguments);
            }
            else if (left is VectorValue vector)
            {
                // Vector indexing: vector . indices
                return VectorIndex(vector, right);
            }
            else if (left.Type == ValueType.Symbol)
            {
                var functionName = (left as SymbolValue)?.Value;
                if (functionName == null)
                {
                    throw new Exception("Invalid function name for dot-apply");
                }
                
                var arguments = new List<K3Value> { right };
                return CallVariableFunction(functionName, arguments);
            }
            else
            {
                throw new Exception("Dot-apply operator requires a function, vector, or dictionary on the left side");
            }
        }
        
        private K3Value AmendFunction(List<K3Value> arguments)
        {
            // Amend operation: .[d; i; f; y] or .[d; i; f]
            // d: data structure to amend (list, dictionary, or atom)
            // i: indices or paths to amend
            // f: function to apply (monadic or dyadic)
            // y: optional value for dyadic function
            
            if (arguments.Count < 3)
            {
                throw new Exception("Amend operation requires at least 3 arguments: data, indices, function");
            }
            
            var data = arguments[0];
            var indices = arguments[1];
            var function = arguments[2];
            var value = arguments.Count > 3 ? arguments[3] : null;
            
            // Handle different data types
            if (data is VectorValue list)
            {
                return AmendList(list, indices, function, value);
            }
            else if (data is DictionaryValue dict)
            {
                return AmendDictionary(dict, indices, function, value);
            }
            else if (data is CharacterValue || data is IntegerValue || data is FloatValue)
            {
                // For atoms, indices must be empty list
                if (!(indices is NullValue || (indices is VectorValue idxVec && idxVec.Elements.Count == 0)))
                {
                    throw new Exception("For atomic data, indices must be empty list");
                }
                return AmendAtom(data, function, value);
            }
            else
            {
                throw new Exception("Amend operation not supported for this data type");
            }
        }

        private K3Value AmendItemFunction(List<K3Value> arguments)
        {
            // Amend Item operation: @[d; i; f; y] or @[d; i; f]
            // Equivalent to .[d; ,i; f; y] - note the enlist (,) applied to indices
            // d: data structure to amend (list, dictionary, or atom)
            // i: indices or paths to amend (will be enlisted)
            // f: function to apply (monadic or dyadic)
            // y: optional value for dyadic function
            
            if (arguments.Count < 3)
            {
                throw new Exception("Amend Item operation requires at least 3 arguments: data, indices, function");
            }
            
            var data = arguments[0];
            var indices = arguments[1];
            var function = arguments[2];
            var value = arguments.Count > 3 ? arguments[3] : null;
            
            // Apply enlist to indices (equivalent to ,i)
            var enlistedIndices = Enlist(indices);
            
            // Handle different data types
            if (data is VectorValue list)
            {
                return AmendList(list, enlistedIndices, function, value);
            }
            else if (data is DictionaryValue dict)
            {
                return AmendDictionary(dict, enlistedIndices, function, value);
            }
            else if (data is CharacterValue || data is IntegerValue || data is FloatValue)
            {
                // Handle atomic data
                if (!(enlistedIndices is NullValue || (enlistedIndices is VectorValue idxVec && idxVec.Elements.Count == 0)))
                {
                    throw new Exception("For atomic data, indices must be empty list");
                }
                return AmendAtom(data, function, value);
            }
            else
            {
                throw new Exception("Amend Item operation not supported for this data type");
            }
        }

        private K3Value AmendList(VectorValue list, K3Value indices, K3Value function, K3Value value)
        {
            // Create a copy of the list to modify
            var result = new List<K3Value>(list.Elements);
            
            if (indices is VectorValue indexVec)
            {
                for (int i = 0; i < indexVec.Elements.Count; i++)
                {
                    var index = indexVec.Elements[i];
                    if (index is IntegerValue intIndex)
                    {
                        int idx = (int)intIndex.Value;
                        if (idx < 0 || idx >= result.Count)
                        {
                            throw new Exception($"Index {idx} out of bounds for list of length {result.Count}");
                        }
                        
                        // Apply function to current value
                        var currentValue = result[idx];
                        var newValue = ApplyAmendFunction(currentValue, function, value);
                        result[idx] = newValue;
                    }
                    else
                    {
                        throw new Exception("List indices must be integers");
                    }
                }
            }
            else if (indices is NullValue)
            {
                // Amend all items in the list
                for (int i = 0; i < result.Count; i++)
                {
                    var currentValue = result[i];
                    var newValue = ApplyAmendFunction(currentValue, function, value);
                    result[i] = newValue;
                }
            }
            else
            {
                throw new Exception("Indices must be a vector or null");
            }
            
            return new VectorValue(result);
        }
        
        private K3Value AmendDictionary(DictionaryValue dict, K3Value indices, K3Value function, K3Value value)
        {
            // Create a copy of the dictionary to modify
            var result = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue Attribute)>(dict.Entries);
            
            if (indices is SymbolValue symbol)
            {
                // Single key amendment
                if (result.ContainsKey(symbol))
                {
                    var currentValue = result[symbol].Value;
                    var newValue = ApplyAmendFunction(currentValue, function, value);
                    result[symbol] = (newValue, result[symbol].Attribute);
                }
                else
                {
                    throw new Exception($"Key '{symbol.Value}' not found in dictionary");
                }
            }
            else if (indices is VectorValue indexVec)
            {
                // Multiple key amendments
                for (int i = 0; i < indexVec.Elements.Count; i++)
                {
                    var index = indexVec.Elements[i];
                    if (index is SymbolValue keySymbol)
                    {
                        if (result.ContainsKey(keySymbol))
                        {
                            var currentValue = result[keySymbol].Value;
                            var newValue = ApplyAmendFunction(currentValue, function, value);
                            result[keySymbol] = (newValue, result[keySymbol].Attribute);
                        }
                        else
                        {
                            throw new Exception($"Key '{keySymbol.Value}' not found in dictionary");
                        }
                    }
                    else
                    {
                        throw new Exception("Dictionary indices must be symbols");
                    }
                }
            }
            else
            {
                throw new Exception("Dictionary indices must be symbols or vector of symbols");
            }
            
            return new DictionaryValue(result);
        }
        
        private K3Value AmendAtom(K3Value atom, K3Value function, K3Value value)
        {
            // For atoms, just apply the function directly
            return ApplyAmendFunction(atom, function, value);
        }
        
        private K3Value ApplyAmendFunction(K3Value currentValue, K3Value function, K3Value value)
        {
            // Apply function to current value
            if (value != null)
            {
                // Dyadic function: f[currentValue; value]
                var arguments = new List<K3Value> { currentValue, value };
                return CallFunction(function, arguments);
            }
            else
            {
                // Monadic function: f[currentValue]
                var arguments = new List<K3Value> { currentValue };
                return CallFunction(function, arguments);
            }
        }
        
        private K3Value CallFunction(K3Value function, List<K3Value> arguments)
        {
            if (function is FunctionValue func)
            {
                var tempFunctionNode = new ASTNode(ASTNodeType.Function);
                tempFunctionNode.Value = func;
                return CallDirectFunction(tempFunctionNode, arguments);
            }
            else if (function is SymbolValue symbol)
            {
                return CallVariableFunction(symbol.Value, arguments);
            }
            else
            {
                throw new Exception("Function must be a FunctionValue or SymbolValue");
            }
        }
        
        private K3Value GlobalAssignment(K3Value left, K3Value right)
        {
            // Global assignment operator: variable :: value
            // Assigns to global variable regardless of current scope
            if (left.Type != ValueType.Symbol)
            {
                throw new Exception("Global assignment requires a variable name on the left side");
            }
            
            var variableName = (left as SymbolValue)?.Value;
            if (variableName == null)
            {
                throw new Exception("Invalid variable name for global assignment");
            }
            
            // Evaluate the right side
            var value = right;
            
            // Store in global variables (access parent evaluator if available)
            if (parentEvaluator != null)
            {
                parentEvaluator.globalVariables[variableName] = value;
            }
            else
            {
                globalVariables[variableName] = value;
            }
            
            return value;
        }

        private K3Value Format(K3Value operand)
        {
            // Unary $ operator - convert to string representation
            // For vectors, preserve structure and convert each element to string
            // Use consistent recursion like {} expressions
            return FormatRecursive(operand);
        }

        private K3Value FormatRecursive(K3Value value)
        {
            // Handle vectors with consistent recursion
            if (value is VectorValue vec)
            {
                // Special handling for character vectors (strings) - split into individual characters
                if (vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
                {
                    if (vec.Elements.Count == 1)
                    {
                        // Single character - return as enlisted form with comma
                        return new VectorValue(new List<K3Value> { vec.Elements[0] });
                    }
                    else
                    {
                        // Character vector with multiple characters - return as-is (identity operation)
                        return vec;
                    }
                }
                
                // Regular vector - recursively format each element
                var vecResult = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    vecResult.Add(FormatRecursive(element));
                }
                return new VectorValue(vecResult);
            }
            else
            {
                // Convert single value to string (character vector)
                // Per spec: result is expected to be always a character vector and never a single character
                // If the result has a single character it must be returned in enlisted form
                
                if (value is CharacterValue charVal)
                {
                    // Single character - return as enlisted form
                    return new VectorValue(new List<K3Value> { charVal });
                }
                else
                {
                    // Convert to string and create proper character vector with individual characters
                    string str = value is SymbolValue sym ? sym.Value : value.ToString();
                    var charElements = str.Select(c => (K3Value)new CharacterValue(c.ToString())).ToList();
                    return new VectorValue(charElements);
                }
            }
        }

        private K3Value ConvertToString(K3Value value)
        {
            // Convert value to string representation without unary format enlisting
            if (value is VectorValue vec && vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
            {
                // Character vector - return as-is
                return vec;
            }
            else
            {
                // Convert to string and create proper character vector with individual characters
                string str = value is SymbolValue sym ? sym.Value : value.ToString();
                var charElements = str.Select(c => (K3Value)new CharacterValue(c.ToString())).ToList();
                return new VectorValue(charElements);
            }
        }

        private K3Value Format(K3Value left, K3Value right)
        {
            // Binary $ operator - form/format according to updated K3 specification
            
            // Handle {} form specifier for evaluating string expressions
            if (left is SymbolValue leftSym && leftSym.Value == "{}")
            {
                return EvaluateStringExpression(right);
            }
            
            // Check if this is a type conversion case (0, 0L, 0.0, `, " ", {})
            // These only work on character vectors according to the spec
            // Type conversion happens ONLY when:
            // 1. First argument is a type conversion specifier AND
            // 2. Second argument is a character vector
            if (IsTypeConversionSpecifier(left) && IsCharacterVectorOrList(right))
            {
                return PerformTypeConversion(left, right);
            }
            
            // Otherwise, this is a format operation with numeric specifier
            if (left is IntegerValue intFormat)
            {
                // Special case: k.exe treats format specifier 0 as always returning empty string
                if (intFormat.Value == 0)
                {
                    return new CharacterValue("");
                }
                return FormatWithSpecifier(intFormat.Value, right);
            }
            else if (left is LongValue longFormat)
            {
                // Special case: k.exe treats format specifier 0j as always returning empty string
                if (longFormat.Value == 0)
                {
                    return new CharacterValue("");
                }
                return FormatWithSpecifier((int)longFormat.Value, right);
            }
            else if (left is FloatValue floatFormat)
            {
                // Special case: k.exe treats format specifier 0.0 as always returning empty string
                if (floatFormat.Value == 0.0)
                {
                    return new CharacterValue("");
                }
                return FormatWithFloatSpecifier(floatFormat.Value, right);
            }
            else
            {
                throw new Exception($"Invalid format specifier: {left}");
            }
        }
        
        private bool IsTypeConversionSpecifier(K3Value left)
        {
            return (left is IntegerValue intValue && intValue.Value == 0) ||
                   (left is LongValue longValue && longValue.Value == 0) ||
                   (left is FloatValue floatValue && floatValue.Value == 0.0) ||
                   (left is SymbolValue symValue && symValue.Value == "") ||
                   (left is CharacterValue charValue && charValue.Value == " ");
        }
        
        private K3Value PerformTypeConversion(K3Value left, K3Value right)
        {
            // Type conversions only work on character vectors according to the spec
            if (!IsCharacterVectorOrList(right))
            {
                throw new Exception($"Type conversion requires character vector input, got {right.Type}");
            }
            
            return ConvertType(left, right);
        }
        
        private bool IsCharacterVectorOrList(K3Value value)
        {
            if (value is CharacterValue)
                return true;
                
            if (value is VectorValue vec)
            {
                // Check if all leaf elements are character vectors
                return AllLeafElementsAreCharacterVectors(vec);
            }
            
            return false;
        }
        
        private bool AllLeafElementsAreCharacterVectors(VectorValue vec)
        {
            foreach (var element in vec.Elements)
            {
                if (element is VectorValue nestedVec)
                {
                    if (!AllLeafElementsAreCharacterVectors(nestedVec))
                        return false;
                }
                else if (!(element is CharacterValue))
                {
                    return false;
                }
            }
            return true;
        }

        private K3Value EvaluateStringExpression(K3Value value)
        {
            // {} form specifier - evaluate each leaf input expression and preserve structure
            // This is similar to the consistent recursion approach used in Format
            return EvaluateStringExpressionRecursive(value);
        }

        private K3Value EvaluateStringExpressionRecursive(K3Value value)
        {
            // Handle vectors with consistent recursion
            if (value is VectorValue vec)
            {
                // Check if this is a character vector (string) - should be a leaf node
                if (vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
                {
                    // Character vector - evaluate as string expression using dot execute
                    var str = string.Join("", vec.Elements.Cast<CharacterValue>().Select(c => c.Value));
                    return ExecuteStringExpression(str);
                }
                
                // Regular vector - recursively evaluate each element
                var vecResult = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    vecResult.Add(EvaluateStringExpressionRecursive(element));
                }
                return new VectorValue(vecResult);
            }
            else
            {
                // For non-vector values, convert to string and evaluate as expression
                var str = value is SymbolValue sym ? sym.Value : value.ToString();
                return ExecuteStringExpression(str);
            }
        }

        private K3Value ExecuteStringExpression(string expression)
        {
            // Execute the string expression using dot execute
            // This evaluates the expression in the current variable context
            try
            {
                var lexer = new Lexer(expression);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens, expression);
                var ast = parser.Parse();
                return Evaluate(ast);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error evaluating string expression '{expression}': {ex.Message}");
            }
        }

        private K3Value FormatWithSpecifier(int formatSpec, K3Value value)
        {
            // Check if this is a character vector (string) - treat as leaf element per spec
            if (value is VectorValue vec && vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
            {
                // Character vector should be treated as a leaf element, not descended into
                return FormatElement(formatSpec, value);
            }
            else if (value is VectorValue regularVec)
            {
                // Apply formatting to each element of the vector
                var result = new List<K3Value>();
                foreach (var element in regularVec.Elements)
                {
                    result.Add(FormatElement(formatSpec, element));
                }
                return new VectorValue(result, "formatted");
            }
            else
            {
                return FormatElement(formatSpec, value);
            }
        }
        
        private K3Value FormatElement(int formatSpec, K3Value value)
        {
            string str;
            
            // Handle character vectors (strings) properly
            if (value is VectorValue charVec && charVec.Elements.Count > 0 && charVec.Elements.All(e => e is CharacterValue))
            {
                // Extract the raw string content from character vector
                var chars = charVec.Elements.Select(e => ((CharacterValue)e).Value);
                str = string.Concat(chars);
            }
            else if (value is SymbolValue symValue)
            {
                // For symbols, format just the name without the backtick
                str = symValue.Value;
            }
            else
            {
                str = value.ToString();
            }
            
            if (formatSpec > 0)
            {
                // Positive: pad with spaces on the left
                if (str.Length < formatSpec)
                {
                    str = str.PadLeft(formatSpec);
                }
                else if (str.Length > formatSpec)
                {
                    // Length overflow: return asterisks
                    str = new string('*', formatSpec);
                }
            }
            else if (formatSpec < 0)
            {
                // Negative: pad with spaces on the right
                int targetLength = Math.Abs(formatSpec);
                if (str.Length < targetLength)
                {
                    str = str.PadRight(targetLength);
                }
                else if (str.Length > targetLength)
                {
                    // Length overflow: return asterisks
                    str = new string('*', targetLength);
                }
            }
            
            // According to K3 spec: single character results should be enlisted
            // to make them 1-item character vectors, e.g., ,"a" or ,"1"
            if (str.Length == 1)
            {
                return new VectorValue(new List<K3Value> { new CharacterValue(str) }, "string");
            }
            
            return new CharacterValue(str);
        }

        private K3Value FormatWithFloatSpecifier(double formatSpec, K3Value value)
        {
            // Check if this is a character vector (string) - treat as leaf element per spec
            if (value is VectorValue vec && vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
            {
                // Character vector should be treated as a leaf element, not descended into
                return FormatFloatElement(formatSpec, value);
            }
            else if (value is VectorValue regularVec)
            {
                // Apply formatting to each element of the vector
                var result = new List<K3Value>();
                foreach (var element in regularVec.Elements)
                {
                    result.Add(FormatFloatElement(formatSpec, element));
                }
                return new VectorValue(result, "formatted");
            }
            else
            {
                return FormatFloatElement(formatSpec, value);
            }
        }
        
        private K3Value FormatFloatElement(double formatSpec, K3Value value)
        {
            // Extract width and decimal places from format specifier
            // For example: 8.2 means width 8 with 2 decimal places
            string formatSpecStr = formatSpec.ToString("F10").TrimEnd('0').TrimEnd('.');
            string[] parts = formatSpecStr.Split('.');
            int totalWidth = (int)Math.Truncate(formatSpec);
            int decimalPlaces = parts.Length > 1 ? int.Parse(parts[1]) : 0;
            
            // Get the numeric value
            double numericValue;
            if (value is FloatValue fv)
            {
                numericValue = fv.Value;
            }
            else if (value is IntegerValue iv)
            {
                numericValue = (double)iv.Value;
            }
            else if (value is LongValue lv)
            {
                numericValue = (double)lv.Value;
            }
            else
            {
                return new CharacterValue(value.ToString());
            }
            
            // Use string.Format for clean formatting with width and precision
            string formatString = totalWidth > 0 
                ? $"{{0,{totalWidth}:F{decimalPlaces}}}"  // e.g., "{0,8:F2}"
                : $"{{0:F{decimalPlaces}}}";             // e.g., "{0:F2}"
            
            string str = string.Format(formatString, numericValue);
            
            // Handle negative width (right padding) - string.Format only handles left padding
            if (totalWidth < 0 && str.Length < Math.Abs(totalWidth))
            {
                str = str.PadRight(Math.Abs(totalWidth));
            }
            
            // According to K3 spec: single character results should be enlisted
            // to make them 1-item character vectors, e.g., ,"a" or ,"1"
            if (str.Length == 1)
            {
                return new VectorValue(new List<K3Value> { new CharacterValue(str) }, "string");
            }
            
            return new CharacterValue(str);
        }

        private K3Value ConvertType(K3Value typeSpec, K3Value value)
        {
            // Check for special type specifiers: 0, 0L, 0.0, `, " ", {}
            if (typeSpec is IntegerValue intSpec && intSpec.Value == 0)
            {
                // Convert to integer
                return ConvertToInteger(value);
            }
            else if (typeSpec is LongValue longSpec && longSpec.Value == 0)
            {
                // Convert to long integer
                return ConvertToLong(value);
            }
            else if (typeSpec is FloatValue floatSpec && floatSpec.Value == 0.0)
            {
                // Convert to float
                return ConvertToFloat(value);
            }
            else if (typeSpec is SymbolValue symSpec && symSpec.Value == "")
            {
                // Convert to symbol (empty backtick)
                return ConvertToSymbol(value);
            }
            else if (typeSpec is CharacterValue charSpec && charSpec.Value == " ")
            {
                // Convert to string representation
                return new CharacterValue(value.ToString());
            }
            else if (typeSpec is FunctionValue funcSpec && funcSpec.BodyText == "")
            {
                // Evaluate expressions ({} case) - evaluate each leaf expression
                return EvaluateExpressions(value);
            }
            else if (typeSpec is VectorValue vecSpec && vecSpec.Elements.Count == 0)
            {
                // Evaluate expressions ({} case) - evaluate each leaf expression
                return EvaluateExpressions(value);
            }
            else
            {
                throw new Exception($"Invalid type specifier: {typeSpec}");
            }
        }

        private K3Value EvaluateExpressions(K3Value value)
        {
            // {} format specifier - evaluate each leaf expression
            if (value is VectorValue vec)
            {
                // Check if this is a character vector (string) that should be evaluated
                if (vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
                {
                    // Reconstruct the string from individual characters
                    var chars = vec.Elements.Select(e => ((CharacterValue)e).Value);
                    var code = string.Concat(chars);
                    
                    // Remove outer quotes if present
                    if (code.StartsWith("\"") && code.EndsWith("\"") && code.Length > 1)
                    {
                        code = code.Substring(1, code.Length - 1);
                    }
                    
                    // Evaluate the reconstructed string using full parser for consistency
                    var lexer = new Lexer(code);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens, code);
                    var ast = parser.Parse();
                    return Evaluate(ast);
                }
                
                // Regular vector - recurse on elements
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(EvaluateExpressions(element));
                }
                return new VectorValue(result);
            }
            else if (value is CharacterValue charVal)
            {
                // For character values (strings), evaluate as K code using full parser
                var code = charVal.Value;
                if (code.StartsWith("\"") && code.EndsWith("\""))
                {
                    code = code.Substring(1, code.Length - 2);
                }
                
                // Use full parser for consistency
                var lexer = new Lexer(code);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens, code);
                var ast = parser.Parse();
                return Evaluate(ast);
            }
            else
            {
                // For non-string scalars, return as-is
                return value;
            }
        }
        
        private K3Value ConvertToInteger(K3Value value)
        {
            if (value is VectorValue vec)
            {
                // Check if this is a character vector (string) - treat as leaf element
                if (vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
                {
                    // Extract the string content from character vector
                    var str = string.Join("", vec.Elements.Cast<CharacterValue>().Select(c => c.Value));
                    if (int.TryParse(str, out int parseResult))
                    {
                        return new IntegerValue(parseResult);
                    }
                    else
                    {
                        throw new Exception($"Cannot convert '{str}' to integer");
                    }
                }
                
                // Regular vector - recursively convert each element
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(ConvertToInteger(element));
                }
                return new VectorValue(result);
            }
            else if (value is CharacterValue charVal)
            {
                // Single character - try to parse as integer first, fallback to ASCII
                if (int.TryParse(charVal.Value, out int parseResult))
                {
                    return new IntegerValue(parseResult);
                }
                else
                {
                    return new IntegerValue((int)charVal.Value[0]);
                }
            }
            else if (value is IntegerValue)
            {
                return value;
            }
            else if (value is LongValue lv)
            {
                return new IntegerValue((int)lv.Value);
            }
            else if (value is FloatValue fv)
            {
                return new IntegerValue((int)fv.Value);
            }
            else if (value is SymbolValue sv)
            {
                // Try to parse symbol as integer
                if (int.TryParse(sv.Value, out int result))
                {
                    return new IntegerValue(result);
                }
                throw new Exception($"Cannot convert symbol '{sv.Value}' to integer");
            }
            else
            {
                throw new Exception($"Cannot convert {value.Type} to integer");
            }
        }

        private K3Value ConvertToLong(K3Value value)
        {
            if (value is VectorValue vec)
            {
                // Check if this is a character vector (string) - treat as leaf element
                if (vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
                {
                    // Extract the string content from character vector
                    var str = string.Join("", vec.Elements.Cast<CharacterValue>().Select(c => c.Value));
                    if (long.TryParse(str, out long parseResult))
                    {
                        return new LongValue(parseResult);
                    }
                    else
                    {
                        throw new Exception($"Cannot convert '{str}' to long");
                    }
                }
                
                // Regular vector - recursively convert each element
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(ConvertToLong(element));
                }
                return new VectorValue(result);
            }
            else if (value is CharacterValue charVal)
            {
                // Single character - convert to long
                if (long.TryParse(charVal.Value, out long parseResult))
                {
                    return new LongValue(parseResult);
                }
                else
                {
                    throw new Exception($"Cannot convert '{charVal.Value}' to long");
                }
            }
            else if (value is LongValue)
            {
                return value;
            }
            else if (value is IntegerValue iv)
            {
                return new LongValue(iv.Value);
            }
            else if (value is FloatValue fv)
            {
                return new LongValue((long)fv.Value);
            }
            else
            {
                throw new Exception($"Cannot convert {value.Type} to long");
            }
        }

        private K3Value ConvertToFloat(K3Value value)
        {
            if (value is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(ConvertToFloat(element));
                }
                return new VectorValue(result);
            }
            else if (value is FloatValue)
            {
                return value;
            }
            else if (value is IntegerValue iv)
            {
                return new FloatValue(iv.Value);
            }
            else if (value is LongValue lv)
            {
                return new FloatValue(lv.Value);
            }
            else
            {
                throw new Exception($"Cannot convert {value.Type} to float");
            }
        }

        private K3Value ConvertToSymbol(K3Value value)
        {
            // Check if this is a character vector (string) - treat as leaf element per spec
            if (value is VectorValue vec && vec.Elements.Count > 0 && vec.Elements.All(e => e is CharacterValue))
            {
                // Character vector should be treated as a leaf element, not descended into
                // Extract the string content and convert to a single symbol
                var chars = vec.Elements.Select(e => ((CharacterValue)e).Value);
                var symbolName = string.Concat(chars);
                return new SymbolValue(symbolName);
            }
            else if (value is VectorValue regularVec)
            {
                var result = new List<K3Value>();
                foreach (var element in regularVec.Elements)
                {
                    result.Add(ConvertToSymbol(element));
                }
                return new VectorValue(result);
            }
            else if (value is SymbolValue)
            {
                return value;
            }
            else if (value is CharacterValue charVal)
            {
                return new SymbolValue(charVal.Value);
            }
            else
            {
                return new SymbolValue(value.ToString());
            }
        }

        // Placeholder functions for missing underscore functions
        private K3Value TimeFunction(K3Value operand)
        {
            throw new Exception("_t (current time) operation reserved for future use");
        }

        private K3Value DrawFunction(K3Value operand)
        {
            throw new Exception("_draw (random number generation) operation reserved for future use");
        }

        private K3Value InFunction(K3Value operand)
        {
            // _in function should be handled as dyadic in binary operations
            // This unary case should not be reached in normal operation
            throw new Exception("_in (Find) function requires two arguments - use infix notation: x _in y");
        }

        private K3Value BinFunction(K3Value operand)
        {
            throw new Exception("_bin (binary search) operation reserved for future use");
        }

        private K3Value BinlFunction(K3Value operand)
        {
            // _binl function should be handled as dyadic in binary operations
            // This unary case should not be reached in normal operation
            throw new Exception("_binl (binary search each-left) function requires two arguments - use infix notation: x _binl y");
        }

        private K3Value LinFunction(K3Value operand)
        {
            // _lin function should be handled as dyadic in binary operations
            // This unary case should not be reached in normal operation
            throw new Exception("_lin (list intersection) function requires two arguments - use infix notation: x _lin y");
        }

        private K3Value LsqFunction(K3Value operand)
        {
            throw new Exception("_lsq (least squares) operation reserved for future use");
        }

        private K3Value GtimeFunction(K3Value operand)
        {
            throw new Exception("_gtime (GMT time conversion) operation reserved for future use");
        }

        private K3Value LtimeFunction(K3Value operand)
        {
            throw new Exception("_ltime (local time conversion) operation reserved for future use");
        }

        private K3Value VsFunction(K3Value operand)
        {
            throw new Exception("_vs (database) operation reserved for future use");
        }

        private K3Value SvFunction(K3Value operand)
        {
            throw new Exception("_sv (database) operation reserved for future use");
        }

        private K3Value SsFunction(K3Value operand)
        {
            throw new Exception("_ss (database) operation reserved for future use");
        }

        private K3Value CiFunction(K3Value operand)
        {
            throw new Exception("_ci (database) operation reserved for future use");
        }

        private K3Value IcFunction(K3Value operand)
        {
            throw new Exception("_ic (database) operation reserved for future use");
        }

        private int ToInteger(K3Value value)
        {
            if (value is IntegerValue intValue)
            {
                return intValue.Value;
            }
            else if (value is LongValue longValue)
            {
                return (int)longValue.Value;
            }
            else if (value is FloatValue floatValue)
            {
                return (int)floatValue.Value;
            }
            else
            {
                throw new Exception("Cannot convert to integer");
            }
        }

        private K3Value DoFunction(K3Value operand)
        {
            // Do function: do[count; expression] or do[count; expression1; ; expressionN]
            // Execute expressions count times, return empty string (matching k.exe behavior)
            
            if (operand is VectorValue args && args.Elements.Count >= 2)
            {
                var countValue = args.Elements[0] is FunctionValue countFunc 
                    ? Evaluate(new Parser(countFunc.PreParsedTokens ?? new List<Token>()).Parse())
                    : EvaluateExpression(args.Elements[0]);
                var count = ToInteger(countValue);
                
                if (count < 0)
                {
                    throw new Exception("Do count must be non-negative");
                }
                
                var expressions = args.Elements.Skip(1).ToList();
                
                for (int i = 0; i < count; i++)
                {
                    foreach (var expr in expressions)
                    {
                        // Handle FunctionValue (contains AST to evaluate) vs regular K3Value
                        if (expr is FunctionValue func)
                        {
                            // Parse and evaluate the function body
                            var parser = new Parser(func.PreParsedTokens ?? new List<Token>());
                            var ast = parser.Parse();
                            Evaluate(ast); // Execute but don't store result
                        }
                        else
                        {
                            EvaluateExpression(expr); // Execute but don't store result
                        }
                    }
                }
                
                // Return empty string to match k.exe behavior
                return new SymbolValue("");
            }
            else
            {
                throw new Exception("Do function requires at least 2 arguments: count and expression(s)");
            }
        }

        private K3Value WhileFunction(K3Value operand)
        {
            // While function: while[condition; expression] or while[condition; expression1; ; expressionN]
            // Execute expressions while condition is not equal to 0
            
            if (operand is VectorValue args && args.Elements.Count >= 2)
            {
                var condition = args.Elements[0];
                var expressions = args.Elements.Skip(1).ToList();
                K3Value result = new SymbolValue(""); // Empty symbol for when loop doesn't execute
                
                while (true)
                {
                    // Evaluate condition
                    var conditionResult = EvaluateExpression(condition);
                    
                    // Check if condition is zero (false)
                    if (!IsNonZeroInteger(conditionResult))
                    {
                        break;
                    }
                    
                    // Execute all expressions
                    foreach (var expr in expressions)
                    {
                        result = EvaluateExpression(expr);
                    }
                }
                
                return result;
            }
            else
            {
                throw new Exception("While function requires at least 2 arguments: condition and expression(s)");
            }
        }

        private K3Value IfFunction(K3Value operand)
        {
            // If function: if[condition; expression] or if[condition; expression1; ; expressionN]
            // Execute expressions if condition is not equal to 0, return empty string (matching k.exe behavior)
            
            if (operand is VectorValue args && args.Elements.Count >= 2)
            {
                var condition = args.Elements[0];
                var expressions = args.Elements.Skip(1).ToList();
                
                // Evaluate condition
                var conditionResult = EvaluateExpression(condition);
                
                // Check if condition is non-zero (true)
                if (IsNonZeroInteger(conditionResult))
                {
                    // Execute all expressions but don't store result
                    foreach (var expr in expressions)
                    {
                        EvaluateExpression(expr);
                    }
                }
                
                // Return empty string to match k.exe behavior
                return new SymbolValue("");
            }
            else
            {
                throw new Exception("If function requires at least 2 arguments: condition and expression(s)");
            }
        }

        private K3Value GotoFunction(K3Value operand)
        {
            throw new Exception("_goto (control flow) operation reserved for future use");
        }

        private K3Value ExitFunction(K3Value operand)
        {
            throw new Exception("_exit (control flow) operation reserved for future use");
        }

        private K3Value TrappedApply(K3Value data, K3Value argument)
        {
            // Trapped apply: behave like dyadic dot apply but never throw exceptions
            // Always return a 2-item vector: [success_flag; result_or_error]
            try
            {
                // Try to perform the regular dot apply operation
                var result = DotApply(data, argument);
                
                // Success: return (0; result)
                var successFlag = new IntegerValue(0);
                var resultVector = new VectorValue(new List<K3Value> { successFlag, result });
                return resultVector;
            }
            catch (Exception ex)
            {
                // Error: return (1; error_message)
                var errorFlag = new IntegerValue(1);
                var errorMessage = new CharacterValue(ex.Message);
                var errorVector = new VectorValue(new List<K3Value> { errorFlag, errorMessage });
                return errorVector;
            }
        }
    }
    
    // Custom comparer for K3Value to use in HashSet operations
    public class K3ValueComparer : IEqualityComparer<K3Value>
    {
        public bool Equals(K3Value x, K3Value y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            
            // Use Match function for comparison (same as _in uses)
            var evaluator = new Evaluator();
            var matchResult = evaluator.Match(x, y);
            
            if (matchResult is IntegerValue intVal)
            {
                return intVal.Value == 1;
            }
            
            return false;
        }

        public int GetHashCode(K3Value obj)
        {
            if (obj is null) return 0;
            
            // Generate hash code based on type and value
            return obj.Type switch
            {
                ValueType.Integer => obj is IntegerValue iv ? iv.Value.GetHashCode() : 0,
                ValueType.Long => obj is LongValue lv ? lv.Value.GetHashCode() : 0,
                ValueType.Float => obj is FloatValue fv ? fv.Value.GetHashCode() : 0,
                ValueType.Character => obj is CharacterValue cv ? cv.Value.GetHashCode() : 0,
                ValueType.Symbol => obj is SymbolValue sv ? sv.Value.GetHashCode() : 0,
                ValueType.Vector => obj is VectorValue vv ? vv.Elements.Count.GetHashCode() : 0,
                _ => obj.ToString().GetHashCode()
            };
        }
    }
}
