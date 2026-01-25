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
                        SetGlobalVariable(assignName, value); // Use global variables for K-style assignments
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
                    "UNARY_MINUS" => UnaryMinus(operand),
                    "TRANSPOSE" => Transpose(operand),
                    "FIRST" => First(operand),
                    "RECIPROCAL" => Reciprocal(operand),
                    "WHERE" => Where(operand),
                    "REVERSE" => Reverse(operand),
                    "TYPE" => GetTypeCode(operand),
                    "STRING_REPRESENTATION" => StringRepresentation(operand),
                    "." => Make(operand),
                    "GRADE_UP" => GradeUp(operand),
                    "GRADE_DOWN" => GradeDown(operand),
                    "SHAPE" => Shape(operand),
                    "ENUMERATE" => Enumerate(operand),
                    "ENLIST" => Enlist(operand),
                    "COUNT" => Count(operand),
                    "FLOOR" => Floor(operand),
                    "UNIQUE" => Unique(operand),
                    "NEGATE" => operand is SymbolValue || (operand is VectorValue vec && vec.Elements.All(e => e is SymbolValue))
                    ? AttributeHandle(operand)
                    : LogicalNegate(operand),
                    "@" => Atom(operand),
                    "~" => AttributeHandle(operand),
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

                return op.Value switch
                {
                    "PLUS" => Add(left, right),
                    "MINUS" => Subtract(left, right),
                    "MULTIPLY" => Multiply(left, right),
                    "DIVIDE" => Divide(left, right),
                    "MIN" => Min(left, right),
                    "MAX" => Max(left, right),
                    "LESS" => Less(left, right),
                    "GREATER" => Greater(left, right),
                    "EQUAL" => Equal(left, right),
                    "POWER" => Power(left, right),
                    "MODULUS" => Modulus(left, right),
                    "JOIN" => Join(left, right),
                    "HASH" => Take(left, right),
                    "UNDERSCORE" => FloorBinary(left, right),
                    "QUESTION" => UniqueBinary(left, right),
                    "NEGATE" => NegateBinary(left, right),
                    "APPLY" => VectorIndex(left, right),
                    "ADVERB_SLASH" => Over(left, right),
                    "ADVERB_BACKSLASH" => Scan(left, right),
                    "ADVERB_TICK" => Each(left, right),
                    "TYPE" => GetType(left, right),
                    _ => throw new Exception($"Unknown binary operator: {op.Value}")
                };
            }
            else if (op.Value == "ADVERB_CHAIN")
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
            
            // For anonymous functions with no parameters and non-empty body, evaluate immediately
            // This is K behavior: {4+5} evaluates to 9, not a function object
            // But {} should return a function object, and {[x] x+6} should return a function object
            if (functionValue.Parameters.Count == 0 && !string.IsNullOrEmpty(functionValue.BodyText.Trim()))
            {
                // No parameters but has body - evaluate the body directly
                var functionEvaluator = new Evaluator();
                return ExecuteFunctionBody(functionValue.BodyText, functionEvaluator, functionValue.PreParsedTokens);
            }
            else
            {
                // Function with parameters, or empty function - return the function object
                // For function calls, the execution happens in EvaluateFunctionCall
                return functionValue;
            }
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
                var function = Evaluate(functionNode);
                
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
            
            // Bind parameters to arguments (in local scope)
            for (int i = 0; i < parameters.Count; i++)
            {
                functionEvaluator.SetVariable(parameters[i], arguments[i]);
            }
            
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
                
                // Use pre-parsed tokens if available (better performance)
                if (preParsedTokens != null && preParsedTokens.Count > 0)
                {
                    var parser = new Parser(preParsedTokens, bodyText);
                    ast = parser.Parse();
                }
                else
                {
                    // Fall back to parsing from text (deferred validation per spec)
                    var lexer = new Lexer(bodyText);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens, bodyText);
                    ast = parser.Parse();
                }
                
                return functionEvaluator.Evaluate(ast);
            }
            catch (Exception ex)
            {
                // Runtime validation - function body errors are caught here (per spec)
                throw new Exception($"Function execution error: {ex.Message}");
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
                return new LongValue(((IntegerValue)a).Value + ((LongValue)b).Value);
            if (a is LongValue && b is IntegerValue)
                return new LongValue(((LongValue)a).Value + ((IntegerValue)b).Value);
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
                return new LongValue(((IntegerValue)a).Value - ((LongValue)b).Value);
            if (a is LongValue && b is IntegerValue)
                return new LongValue(((LongValue)a).Value - ((IntegerValue)b).Value);
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
                return new LongValue(((IntegerValue)a).Value * ((LongValue)b).Value);
            if (a is LongValue && b is IntegerValue)
                return new LongValue(((LongValue)a).Value * ((IntegerValue)b).Value);
            if (a is IntegerValue && b is FloatValue)
                return new FloatValue(((IntegerValue)a).Value * ((FloatValue)b).Value);
            if (a is FloatValue && b is IntegerValue)
                return new FloatValue(((FloatValue)a).Value * ((IntegerValue)b).Value);
            if (a is LongValue && b is FloatValue)
                return new FloatValue(((LongValue)a).Value * ((FloatValue)b).Value);
            if (a is FloatValue && b is LongValue)
                return new FloatValue(((FloatValue)a).Value * ((LongValue)b).Value);
            
            // Handle same type operations
            if (a is IntegerValue && b is IntegerValue)
                return new IntegerValue(((IntegerValue)a).Value * ((IntegerValue)b).Value);
            if (a is LongValue && b is LongValue)
                return new LongValue(((LongValue)a).Value * ((LongValue)b).Value);
            if (a is FloatValue && b is FloatValue)
                return new FloatValue(((FloatValue)a).Value * ((FloatValue)b).Value);
            
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
            if (a is IntegerValue intA)
            {
                var elements = new List<K3Value>();
                for (int i = 0; i < intA.Value; i++)
                {
                    elements.Add(new IntegerValue(0));
                }
                return new VectorValue(elements);
            }
            else if (a is VectorValue vecA)
            {
                var elements = new List<K3Value>();
                int value = 0;
                foreach (var element in vecA.Elements)
                {
                    if (element is IntegerValue length)
                    {
                        for (int i = 0; i < length.Value; i++)
                        {
                            elements.Add(new IntegerValue(value++));
                        }
                    }
                }
                return new VectorValue(elements);
            }
            
            throw new Exception($"Cannot generate from {a.Type}");
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
            
            return new VectorValue(new List<K3Value> { new IntegerValue(0) }); // For scalars
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
            
            return new VectorValue(new List<K3Value> { new IntegerValue(0) }); // For scalars
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
                return new IntegerValue(vecA.Elements.Count);
            
            return new IntegerValue(0); // For scalars
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
                for (int i = 0; i < longA.Value; i++)
                {
                    elements.Add(new LongValue(i));
                }
                return new VectorValue(elements, longA.Value == 0 ? "enumerate_long" : "standard");
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
                if (index is IntegerValue intIndex)
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
                // Handle dictionary indexing: dictionary @ key
                if (index is SymbolValue key)
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

        private K3Value Over(K3Value verb, K3Value data)
        {
            // Handle vector case (over)
            if (data is VectorValue dataVec && dataVec.Elements.Count > 0)
            {
                // Check if this is a mixed adverb operation (verb is a vector with [scalar, verbSymbol])
                if (verb is VectorValue mixedVerbVec && mixedVerbVec.Elements.Count == 2)
                {
                    var scalar = mixedVerbVec.Elements[0];
                    var verbSymbol = mixedVerbVec.Elements[1] as SymbolValue;
                    
                    if (verbSymbol != null)
                    {
                        // Mixed adverb: start with the scalar, then apply the verb to each vector element
                        var mixedResult = scalar;
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            mixedResult = ApplyVerb(verbSymbol.Value, mixedResult, dataVec.Elements[i]);
                        }
                        return mixedResult;
                    }
                }
                
                // For mixed adverb operations (scalar + over + vector), the scalar should be used only once
                // Example: 1 +/ 2 3 4 5 should be 1 + 2 + 3 + 4 + 5, not (1+2) + (1+3) + (1+4) + (1+5)
                K3Value result;
                
                // Check if this is a mixed adverb (scalar + over + vector)
                if (IsScalar(verb) && verb.Type != ValueType.Symbol)
                {
                    // Mixed adverb: start with the scalar verb, then apply to each vector element
                    result = verb;
                    for (int i = 0; i < dataVec.Elements.Count; i++)
                    {
                        result = ApplyVerbWithOperator(verb, result, dataVec.Elements[i]);
                    }
                }
                else
                {
                    // Regular over: start with first element and apply verb to accumulate
                    result = dataVec.Elements[0];
                    for (int i = 1; i < dataVec.Elements.Count; i++)
                    {
                        // Apply the verb to result and next element
                        if (verb is SymbolValue verbSymbol)
                        {
                            result = ApplyVerb(verbSymbol.Value, result, dataVec.Elements[i]);
                        }
                        else
                        {
                            // If verb is not a symbol, treat it as a value to apply with the operator
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
                "PLUS" => Add(left, right),
                "MINUS" => Subtract(left, right),
                "MULTIPLY" => Multiply(left, right),
                "DIVIDE" => Divide(left, right),
                "MIN" => Min(left, right),
                "MAX" => Max(left, right),
                "LESS" => Less(left, right),
                "GREATER" => Greater(left, right),
                "EQUAL" => Equal(left, right),
                "POWER" => Power(left, right),
                "MODULUS" => Modulus(left, right),
                "JOIN" => Join(left, right),
                "+" => Add(left, right),  // Add fallback for literal +
                "-" => Subtract(left, right),  // Add fallback for literal -
                "*" => Multiply(left, right),  // Add fallback for literal *
                "%" => Divide(left, right),  // Add fallback for literal %
                "&" => Min(left, right),  // Add fallback for literal &
                "|" => Max(left, right),  // Add fallback for literal |
                "^" => Power(left, right),  // Add fallback for literal ^
                _ => throw new Exception($"Unknown operator: {verbName}")
            };
        }

        private K3Value ApplyUnaryVerb(string verbName, K3Value operand)
        {
            return verbName switch
            {
                "PLUS" => operand,  // Identity operation
                "MINUS" => Negate(operand),
                "MULTIPLY" => operand,  // Identity operation  
                "DIVIDE" => Reciprocal(operand),
                "MIN" => operand,  // Identity operation
                "MAX" => operand,  // Identity operation
                "POWER" => operand,  // Identity operation
                "MODULUS" => operand,  // Identity operation
                "JOIN" => Enlist(operand),
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

        private K3Value Scan(K3Value verb, K3Value data)
        {
            // Handle mixed scan case where verb is actually a vector (scalar, verb)
            if (verb is VectorValue verbVec && verbVec.Elements.Count == 2)
            {
                var scalar = verbVec.Elements[0];
                var actualVerb = verbVec.Elements[1];
                
                if (data is VectorValue dataVec && dataVec.Elements.Count > 0)
                {
                    var result = new List<K3Value>();
                    var current = ApplyVerbToScalarAndVector(scalar, actualVerb, dataVec.Elements[0]);
                    result.Add(current);
                    
                    for (int i = 1; i < dataVec.Elements.Count; i++)
                    {
                        if (actualVerb is SymbolValue verbSymbol)
                        {
                            current = ApplyVerb(verbSymbol.Value, current, dataVec.Elements[i]);
                        }
                        else
                        {
                            // Check if verb is a glyph stored as non-symbol type
                            string verbStr = actualVerb.ToString();
                            if (verbStr.Length == 1 && "+-*/%^!&|<>=^,_?#~".Contains(verbStr))
                            {
                                current = ApplyVerb(verbStr, current, dataVec.Elements[i]);
                            }
                            else
                            {
                                // For non-glyph verbs, skip this iteration
                                continue;
                            }
                        }
                        result.Add(current);
                    }
                    
                    return new VectorValue(result);
                }
            }
            
            // Handle vector case (regular scan)
            if (data is VectorValue dataVec2 && dataVec2.Elements.Count > 0)
            {
                var result = new List<K3Value>();
                var current = dataVec2.Elements[0];
                result.Add(current);
                
                for (int i = 1; i < dataVec2.Elements.Count; i++)
                {
                    if (verb is SymbolValue verbSymbol)
                    {
                        current = ApplyVerb(verbSymbol.Value, current, dataVec2.Elements[i]);
                    }
                    else
                    {
                        // Check if verb is a glyph stored as non-symbol type
                        string verbStr = verb.ToString();
                        if (verbStr.Length == 1 && "+-*/%^!&|<>=^,_?#~".Contains(verbStr))
                        {
                            current = ApplyVerb(verbStr, current, dataVec2.Elements[i]);
                        }
                        else
                        {
                            // For non-glyph verbs, skip this iteration
                            continue;
                        }
                    }
                    result.Add(current);
                }
                
                return new VectorValue(result);
            }
            
            // Handle scalar case
            if (IsScalar(data))
            {
                return data;
            }
            
            throw new Exception($"Scan not implemented for types: {verb.Type}, {data.Type}");
        }

        private K3Value Each(K3Value verb, K3Value data)
        {
            // Handle vector + vector case (same length)
            if (verb is VectorValue verbVec && data is VectorValue dataVec)
            {
                // Check if vectors have different lengths - should throw length error
                if (verbVec.Elements.Count != dataVec.Elements.Count)
                {
                    throw new Exception($"length error: {verbVec.Elements.Count} != {dataVec.Elements.Count}");
                }
                
                // For now, throw an error as vector-vector each is not fully implemented
                throw new Exception("Vector-vector each not implemented");
            }
            
            // Handle scalar + vector case
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
            
            // Handle scalar + scalar case
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
            // 5: verb - produce string representation of the argument
            string representation = value.ToString();
            return new VectorValue(new List<K3Value> { new CharacterValue(representation) });
        }
        
        private K3Value Make(K3Value value)
        {
            // . (make) operator - create dictionary from mixed vector
            if (value is VectorValue vec)
            {
                var dict = new Dictionary<SymbolValue, (K3Value, DictionaryValue)>();
                
                foreach (var element in vec.Elements)
                {
                    if (element is VectorValue entryVec)
                    {
                        if (entryVec.Elements.Count == 2)
                        {
                            // Tuple (key; value) - attribute is null
                            if (entryVec.Elements[0] is SymbolValue key)
                            {
                                dict[key] = (entryVec.Elements[1], null);
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
                                dict[key] = (entryVec.Elements[1], attribute);
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
                
                return new DictionaryValue(dict);
            }
            else
            {
                throw new Exception("Make operator requires a vector");
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
                    
                    for (int i = 0; i < Math.Min(actualCount, dataVec.Elements.Count); i++)
                    {
                        result.Add(dataVec.Elements[i]);
                    }
                    
                    // If we need more elements than available, pad with the appropriate type
                    while (result.Count < actualCount)
                    {
                        if (dataVec.Elements.Count > 0)
                        {
                            // Pad with the same type as the first element
                            var firstElement = dataVec.Elements[0];
                            if (firstElement is IntegerValue)
                                result.Add(new IntegerValue(0));
                            else if (firstElement is LongValue)
                                result.Add(new LongValue(0L));
                            else if (firstElement is FloatValue)
                                result.Add(new FloatValue(0.0));
                            else if (firstElement is CharacterValue)
                                result.Add(new CharacterValue(" "));
                            else if (firstElement is SymbolValue)
                                result.Add(new SymbolValue(""));
                            else
                                result.Add(new NullValue());
                        }
                        else
                        {
                            // Empty vector - can't determine type, use null
                            result.Add(new NullValue());
                        }
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
                    
                    // Determine creation method for empty vectors from scalar
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
    }
}
