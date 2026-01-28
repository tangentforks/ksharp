using System;
using System.Collections.Generic;

namespace K3CSharp
{
    public class Evaluator
    {
        private Dictionary<string, K3Value> globalVariables = new Dictionary<string, K3Value>();
        private Dictionary<string, K3Value> localVariables = new Dictionary<string, K3Value>();
        private Dictionary<string, int> symbolTable = new Dictionary<string, int>();
        public bool isInFunctionCall = false; // Track if we're evaluating a function call
        public static int floatPrecision = 7; // Default precision for floating point display
        
        // Reference to the current function being executed (for AST caching)
        public FunctionValue currentFunctionValue = null;

        // Reference to parent evaluator for global scope access
        private Evaluator parentEvaluator = null;

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

                default:
                    throw new Exception($"Unknown AST node type: {node.Type}");
            }
        }

        private K3Value GetVariable(string variableName)
        {
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
            
            // Check parent evaluator (for nested function calls)
            if (parentEvaluator != null)
            {
                return parentEvaluator.GetVariable(variableName);
            }
            
            throw new Exception($"Undefined variable: {variableName}");
        }
        
        private K3Value SetVariable(string variableName, K3Value value)
        {
            // Local assignment - always set in local scope
            localVariables[variableName] = value;
            return value;
        }

        private K3Value SetGlobalVariable(string variableName, K3Value value)
        {
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
                var left = Evaluate(node.Children[0]);
                var right = Evaluate(node.Children[1]);

                return op.Value.ToString() switch
                    {
                        "+" => Add(left, right),
                        "-" => Subtract(left, right),
                        "*" => Multiply(left, right),
                        "%" => Divide(left, right),
                        "^" => Power(left, right),
                        "!" => Modulus(left, right),
                        "&" => Min(left, right),
                        "|" => Max(left, right),
                        "<" => LessThan(left, right),
                        ">" => GreaterThan(left, right),
                        "=" => Equal(left, right),
                        "," => Join(left, right),
                        "#" => Take(left, right),
                        "_" => FloorBinary(left, right),
                        "@" => AtIndex(left, right),
                        "." => DotApply(left, right),
                        "::" => GlobalAssignment(left, right),
                        "ADVERB_SLASH" => Over(new SymbolValue("+"), left, right),
                        "ADVERB_BACKSLASH" => Scan(new SymbolValue("+"), left, right),
                        "ADVERB_TICK" => Each(left, right),
                        "TYPE" => GetType(left, right),
                        _ => throw new Exception($"Unknown binary operator: {op.Value}")
                    };
            }
            // Handle 3-argument adverb structure: ADVERB(verb, left, right)
            else if (node.Children.Count == 3 && 
                    (op.Value.ToString() == "ADVERB_SLASH" || op.Value.ToString() == "ADVERB_BACKSLASH" || op.Value.ToString() == "ADVERB_TICK"))
            {
                var verb = Evaluate(node.Children[0]);
                var left = Evaluate(node.Children[1]);
                var right = Evaluate(node.Children[2]);

                return op.Value.ToString() switch
                {
                    "ADVERB_SLASH" => Over(verb, left, right),
                    "ADVERB_BACKSLASH" => Scan(verb, left, right),
                    "ADVERB_TICK" => Each(verb, left, right),
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
            else if (functionNode.Type == ASTNodeType.Variable)
            {
                // Variable function call: functionName[args]
                var functionName = functionNode.Value is SymbolValue symbol ? symbol.Value : functionNode.Value.ToString();
                return CallVariableFunction(functionName, arguments);
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

        private K3Value Add(K3Value a, K3Value b)
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

        private K3Value Subtract(K3Value a, K3Value b)
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
                    result.Add(Subtract(a, element));
                }
                return new VectorValue(result);
            }
            
            throw new Exception($"Cannot subtract {a.Type} and {b.Type}");
        }

        private K3Value Multiply(K3Value a, K3Value b)
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

        private K3Value Greater(K3Value a, K3Value b)
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

        private K3Value Equal(K3Value a, K3Value b)
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
                    var result = Equal(vecA.Elements[i], vecB.Elements[i]);
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

        private K3Value Modulus(K3Value left, K3Value right)
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
            // For now, just return the argument (transpose for matrices not implemented)
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
            if (data is VectorValue dataVec && dataVec.Elements.Count > 0)
            {
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
                "+" => Add(left, right),
                "-" => Subtract(left, right),
                "*" => Multiply(left, right),
                "%" => Divide(left, right),
                "&" => Min(left, right),
                "|" => Max(left, right),
                "<" => Less(left, right),
                ">" => Greater(left, right),
                "=" => Equal(left, right),
                "^" => Power(left, right),
                "!" => Modulus(left, right),
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
                    return Add(verb, right);
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
            return new VectorValue(new List<K3Value> { new CharacterValue(representation) });
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

        private K3Value AtIndex(K3Value data, K3Value index)
        {
            // @ operator for indexing: data @ index
            
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
                            throw new Exception("Dictionary indices must be symbols");
                        }
                    }
                    return new VectorValue(result, "standard");
                }
                else
                {
                    throw new Exception("Dictionary indices must be symbols or vector of symbols");
                }
            }
            
            // Handle vector indexing
            if (index is IntegerValue intIndex)
            {
                if (data is VectorValue dataVec)
                {
                    // Vector @ integer
                    var actualIndex = intIndex.Value;
                    if (actualIndex < 0)
                    {
                        actualIndex = dataVec.Elements.Count + actualIndex;
                    }
                    
                    if (actualIndex < 0 || actualIndex >= dataVec.Elements.Count)
                    {
                        throw new Exception($"Index {intIndex.Value} out of bounds for vector of length {dataVec.Elements.Count}");
                    }
                    
                    return dataVec.Elements[actualIndex];
                }
                else
                {
                    // Scalar @ integer - treat scalar as single-element vector
                    if (intIndex.Value == 0)
                    {
                        return data;
                    }
                    else
                    {
                        throw new Exception($"Index {intIndex.Value} out of bounds for scalar");
                    }
                }
            }
            else if (index is VectorValue indexVec)
            {
                if (data is VectorValue dataVec)
                {
                    // Vector @ vector of indices
                    var result = new List<K3Value>();
                    foreach (var idx in indexVec.Elements)
                    {
                        if (idx is IntegerValue intIdx)
                        {
                            var actualIndex = intIdx.Value;
                            if (actualIndex < 0)
                            {
                                actualIndex = dataVec.Elements.Count + actualIndex;
                            }
                            
                            if (actualIndex < 0 || actualIndex >= dataVec.Elements.Count)
                            {
                                throw new Exception($"Index {intIdx.Value} out of bounds for vector of length {dataVec.Elements.Count}");
                            }
                            
                            result.Add(dataVec.Elements[actualIndex]);
                        }
                        else
                        {
                            throw new Exception("Vector indices must be integers");
                        }
                    }
                    return new VectorValue(result, "standard");
                }
                else
                {
                    // Scalar @ vector of indices
                    var result = new List<K3Value>();
                    foreach (var idx in indexVec.Elements)
                    {
                        if (idx is IntegerValue intIdx && intIdx.Value == 0)
                        {
                            result.Add(data);
                        }
                        else
                        {
                            throw new Exception($"Index out of bounds for scalar");
                        }
                    }
                    return new VectorValue(result, "standard");
                }
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
            // Dot-apply operator: function . argument
            // Similar to function application but with different precedence
            if (left is FunctionValue function)
            {
                // Create a temporary AST node for the function to reuse existing logic
                var tempFunctionNode = new ASTNode(ASTNodeType.Function);
                tempFunctionNode.Value = function;
                
                // Create arguments list
                var arguments = new List<K3Value> { right };
                
                return CallDirectFunction(tempFunctionNode, arguments);
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
                throw new Exception("Dot-apply operator requires a function on the left side");
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
    }
}
