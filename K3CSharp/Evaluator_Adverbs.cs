using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        private K3Value ApplyVerb(K3Value verb, K3Value left, K3Value right)
        {
            // Generic verb application that works with K3Value verbs
            
            // Handle SymbolValue verbs (most common case)
            if (verb is SymbolValue verbSymbol)
            {
                return ApplySymbolVerb(verbSymbol.Value, left, right);
            }
            
            // Handle FunctionValue verbs
            if (verb is FunctionValue func)
            {
                return ApplyFunctionValue(func, left, right);
            }
            
            // Handle VectorValue verbs (composite verbs)
            if (verb is VectorValue verbVec)
            {
                return ApplyVectorVerb(verbVec, left, right);
            }
            
            // Handle other verb types by treating them as values with operators
            return ApplySymbolVerbWithOperator(verb, left, right);
        }
        
        private K3Value ApplySymbolVerb(string verbName, K3Value left, K3Value right)
        {
            // Handle SymbolValue verbs by name
            
            // Check if it's a built-in operator (single character)
            if (verbName.Length == 1)
            {
                return ApplyBuiltInOperator(verbName[0], left, right);
            }
            
            // Check if it's a system function (starts with underscore)
            if (verbName.StartsWith("_"))
            {
                return ApplySystemFunction(verbName, left, right);
            }
            
            // Check if it's a variable name (lookup in global scope)
            var variableValue = GetVariableValue(verbName);
            if (variableValue != null)
            {
                return ApplyFunctionValue(variableValue, left, right);
            }
            
            // Check if it's an anonymous function (in the form {body})
            if (verbName.StartsWith("{") && verbName.EndsWith("}"))
            {
                return ApplyAnonymousFunction(verbName, left, right);
            }
            
            throw new Exception($"Unknown verb: {verbName}");
        }
        
        private K3Value ApplyVectorVerb(VectorValue verbVec, K3Value left, K3Value right)
        {
            // Handle VectorValue verbs (composite verbs)
            if (verbVec.Elements.Count == 1)
            {
                // Single element vector - check if this is an adverb-created composite verb
                var verbElement = verbVec.Elements[0];
                
                // Check if this is a composite verb from adverb processing
                // Adverb-created composite verbs have vector type -1 and contain a verb
                if (verbVec.VectorType == -1)
                {
                    // This is a composite verb created by an adverb
                    // Apply the underlying verb with adverb semantics
                    return ApplyVerb(verbElement, left, right);
                }
                else
                {
                    // Regular single element vector - apply the verb directly
                    return ApplyVerb(verbElement, left, right);
                }
            }
            else if (verbVec.Elements.Count >= 2)
            {
                // Multi-element vector - this might be a modified verb or function
                var firstElement = verbVec.Elements[0];
                if (firstElement is SymbolValue symbol)
                {
                    // Apply the symbol verb with the remaining elements as modifiers
                    return ApplySymbolVerb(symbol.Value, left, right);
                }
                else
                {
                    // Treat as a composite function
                    return ApplyFunctionValue(verbVec, left, right);
                }
            }
            
            throw new Exception($"Invalid verb vector: {verbVec}");
        }
        
                
        private K3Value ApplyBuiltInOperator(char op, K3Value left, K3Value right)
        {
            return op switch
            {
                '+' => Plus(left, right),
                '-' => Minus(left, right),
                '*' => Times(left, right),
                '%' => Divide(left, right),
                '&' => Min(left, right),
                '|' => Max(left, right),
                '<' => Less(left, right),
                '>' => More(left, right),
                '=' => Match(left, right),
                '^' => Power(left, right),
                '!' => ModRotate(left, right),
                ',' => Join(left, right),
                '#' => Take(left, right),
                '_' => FloorBinary(left, right),
                '@' => AtIndex(left, right),
                '.' => DotApply(left, right),
                '$' => Format(left, right),
                '~' => Match(left, right),
                '?' => Find(left, right),
                _ => throw new Exception($"Unknown operator: {op}")
            };
        }
        
        private K3Value ApplySystemFunction(string functionName, K3Value left, K3Value right)
        {
            return functionName switch
            {
                "_in" => In(left, right),
                "_draw" => Draw(left, right),
                "_bin" => Bin(left, right),
                "_div" => MathDiv(left, right),
                "_dot" => MathDot(left, right),
                "_mul" => MathMul(left, right),
                "_inv" => MathInv(left, right),
                "_lsq" => MathLsq(left, right),
                "_and" => MathAnd(left, right),
                "_or" => MathOr(left, right),
                "_xor" => MathXor(left, right),
                "_rot" => MathRot(left, right),
                "_shift" => MathShift(left, right),
                "_binl" => Binl(left, right),
                "_lin" => Lin(left, right),
                "_dv" => Dv(left, right),
                "_di" => Di(left, right),
                "_ci" => CiFunction(left),
                "_ic" => IcFunction(left),
                "_sm" => Sm(left, right),
                "_sv" => Sv(left, right),
                "_vs" => Vs(left, right),
                "_ss" => SsFunction(left, right),
                "_setenv" => SetenvFunction(left, right),
                _ => throw new Exception($"Unknown system function: {functionName}")
            };
        }
        
        private K3Value? GetVariableValue(string variableName)
        {
            // Look up variable in global scope
            if (globalVariables.TryGetValue(variableName, out var value))
            {
                return value;
            }
            return null;
        }
        
        private K3Value ApplyFunctionValue(K3Value functionValue, K3Value left, K3Value right)
        {
            if (functionValue is FunctionValue func)
            {
                // Execute the function with the provided arguments
                var arguments = new List<K3Value> { left, right };
                return ExecuteFunction(func, arguments);
            }
            else if (functionValue is SymbolValue symbol)
            {
                // Symbol might be a function name
                return ApplySymbolVerb(symbol.Value, left, right);
            }
            else
            {
                throw new Exception($"Variable '{functionValue}' is not a function");
            }
        }
        
        private K3Value ApplyAnonymousFunction(string functionBody, K3Value left, K3Value right)
        {
            // Parse and execute anonymous function
            // For now, this is a simplified implementation
            // In a full implementation, we'd parse the function body and execute it
            throw new NotImplementedException($"Anonymous function execution not yet implemented: {functionBody}");
        }
        
        private K3Value ExecuteFunction(FunctionValue func, List<K3Value> arguments)
        {
            // Execute the function with the provided arguments
            // This is a simplified implementation - in a full K interpreter,
            // we'd evaluate the function body with the arguments bound to parameters
            if (func.BodyText.Contains(","))
            {
                // Handle comma functions specially
                return ApplySymbolVerb(",", arguments[0], arguments.Count > 1 ? arguments[1] : new IntegerValue(0));
            }
            else
            {
                throw new Exception($"Function execution not yet implemented: {func.BodyText}");
            }
        }

        private K3Value ApplyUnaryVerb(string verbName, K3Value operand)
        {
            return verbName switch
            {
                "+" => operand,  // Identity operation
                "-" => Negate(operand),
                "*" => First(operand),
                "%" => Reciprocal(operand),
                "&" => Where(operand),  // Where operator
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

        private K3Value ApplySymbolVerbWithOperator(K3Value verb, K3Value left, K3Value right)
        {
            // Handle case where verb is a value (like 2 +/ 1 2 3)
            // This means we should use verb as left operand with operator
            if (verb is SymbolValue verbSymbol)
            {
                return ApplySymbolVerb(verbSymbol.Value, verb, right);
            }
            else
            {
                // For numeric verbs, assume addition by default
                // But check if this is actually a glyph verb stored as a different type
                if (verb.Type == ValueType.Symbol || 
                    (verb.Type == ValueType.Integer && verb.ToString().Length == 1 && "+-*/%^!&|<>=^,_?#~".Contains(verb.ToString())))
                {
                    return ApplySymbolVerb(verb.ToString(), verb, right);
                }
                else
                {
                    return Plus(verb, right);
                }
            }
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
                        result = ApplyAdverbSlash(result, new IntegerValue(0), new IntegerValue(0));
                        break;
                    case "ADVERB_BACKSLASH":
                        result = ApplyAdverbBackslash(result, new IntegerValue(0), new IntegerValue(0));
                        break;
                    case "ADVERB_TICK":
                        result = ApplyAdverbTick(result, new IntegerValue(0), new IntegerValue(0));
                        break;
                    case "ADVERB_SLASH_COLON":
                        result = ApplyAdverbSlashColon(result, new IntegerValue(0), new IntegerValue(0));
                        break;
                    case "ADVERB_BACKSLASH_COLON":
                        result = ApplyAdverbBackslashColon(result, new IntegerValue(0), new IntegerValue(0));
                        break;
                    case "ADVERB_TICK_COLON":
                        result = ApplyAdverbTickColon(result, new IntegerValue(0), new IntegerValue(0));
                        break;
                    default:
                        throw new Exception($"Unknown adverb in chain: {adverb}");
                }
            }
            
            return result;
        }

        private K3Value CreateVerbDataStructure(K3Value verb, K3Value left, K3Value right)
        {
            // Create a vector structure representing [verb, left, right] for adverb processing
            var result = new VectorValue(new List<K3Value> { verb, left, right }, -1);
            return result;
        }

        private K3Value ApplyAdverbSlash(K3Value verb, K3Value left, K3Value right)
        {
            // Natural nested evaluation: call Over with the verb and arguments
            return Over(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
        }

        private K3Value ApplyAdverbBackslash(K3Value verb, K3Value left, K3Value right)
        {
            // Natural nested evaluation: call Scan with the verb and arguments
            return Scan(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
        }

        private K3Value ApplyAdverbTick(K3Value verb, K3Value left, K3Value right)
        {
            // Natural nested evaluation: call Each with the verb and arguments
            return Each(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
        }

        private K3Value ApplyAdverbSlashColon(K3Value verb, K3Value left, K3Value right)
        {
            // Check if this is a nested adverb call (has verb, left, right arguments)
            // Use sentinel values to distinguish between "no arguments" and "actual arguments"
            bool hasLeftArg = !(left is IntegerValue leftInt && leftInt.Value == 0);
            bool hasRightArg = !(right is IntegerValue rightInt && rightInt.Value == 0);
            
            if (hasLeftArg && hasRightArg)
            {
                // Natural nested evaluation: call EachRight with the verb and arguments
                return EachRight(verb, left, right);
            }
            
            // For simple cases (nominalized adverb), return a function that represents "each-right of verb"
            // Store the verb in the function's BodyText for later use
            string verbStr = verb is SymbolValue sym ? sym.Value : verb.ToString();
            var lambda = new FunctionValue($"EACH_RIGHT:{verbStr}", new List<string> { "x", "y" });
            return lambda;
        }

        private K3Value ApplyAdverbBackslashColon(K3Value verb, K3Value left, K3Value right)
        {
            // Natural nested evaluation: call EachLeft with the verb and arguments
            return EachLeft(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
        }
        
        private K3Value? HandleVerbDataStructureForBackslashColon(K3Value operand)
        {
            return null;
        }
        
        private K3Value? HandleNestedAdverbCase(K3Value verb, K3Value left, K3Value right)
        {
            if (!(right is VectorValue rightVec))
                return null;
            
            var leftElements = left is VectorValue leftVec ? leftVec.Elements : new List<K3Value> { left };
            var rightElements = rightVec.Elements;
            
            // No special heuristic - let natural type determination handle everything
            // The resulting structure should emerge from the operation results and type detection
            return PerformOriginalSimpleEachLeftOperation(verb, leftElements, rightElements);
        }
        
        private K3Value PerformOriginalSimpleEachLeftOperation(K3Value verb, List<K3Value> leftElements, List<K3Value> rightElements)
        {
            var result = new List<K3Value>();
            var rightVector = new VectorValue(rightElements, DetermineVectorType(rightElements));
            
            foreach (var leftElement in leftElements)
            {
                // Each-left: apply verb with left element and the ENTIRE right vector
                var operationResult = ApplyVerb(verb, leftElement, rightVector);
                result.Add(operationResult);
            }
            
            return new VectorValue(result, DetermineVectorType(result));
        }
        
                
        private K3Value PerformNestedAdverbEachLeft(K3Value verb, List<K3Value> leftElements, List<K3Value> rightElements)
        {
            var result = new List<K3Value>();
            
            foreach (var leftElement in leftElements)
            {
                var operationResults = new List<K3Value>();
                foreach (var rightElement in rightElements)
                {
                    var operationResult = ApplyVerb(verb, leftElement, rightElement);
                    operationResults.Add(operationResult);
                }
                result.Add(new VectorValue(operationResults, DetermineVectorType(operationResults)));
            }
            
            return new VectorValue(result, DetermineVectorType(result));
        }
        
        private K3Value PerformSimpleEachLeftOperation(K3Value verb, List<K3Value> leftElements, List<K3Value> rightElements)
        {
            var result = new List<K3Value>();
            
            foreach (var leftElement in leftElements)
            {
                // Check if this is a composite verb from adverb processing
                
                if (verb is VectorValue compositeVerb && ((VectorValue)verb).VectorType == -1)
                {
                    // This is a composite verb created by an adverb
                    // Apply it to left element with entire right vector
                    var rightVector = new VectorValue(rightElements, DetermineVectorType(rightElements));
                    var operationResult = ApplyVerb(verb, leftElement, rightVector);
                    result.Add(operationResult);
                }
                else
                {
                    // Regular verb - each-left: for each left element, join with each right element individually
                    var operationResults = new List<K3Value>();
                    foreach (var rightElement in rightElements)
                    {
                        var operationResult = ApplyVerb(verb, leftElement, rightElement);
                        operationResults.Add(operationResult);
                    }
                    int operationVectorType = DetermineVectorType(operationResults);
                    result.Add(new VectorValue(operationResults, operationVectorType));
                }
            }
            var vectorType = DetermineVectorType(result);
            return new VectorValue(result, vectorType);
        }
        
        private K3Value? HandleFunctionValueForBackslashColon(K3Value operand)
        {
            if (!(operand is FunctionValue func))
                return null;
            
            // Create a composite function that represents a verb+adverb combination
            var adverbSymbol = new SymbolValue("\\:");
            var composedElements = new List<K3Value> { func, adverbSymbol };
            
            var composedFunction = new VectorValue(composedElements, -1);
            
            return composedFunction;
        }
        
        private K3Value HandleFallbackForBackslashColon(K3Value operand)
        {
            return new VectorValue(new List<K3Value> { operand }, -1);
        }

        private K3Value ApplyAdverbTickColon(K3Value verb, K3Value left, K3Value right)
        {
            // Natural nested evaluation: call EachPrior with the verb and arguments
            return EachPrior(verb, left, right);
        }
        

        private K3Value Over(K3Value verb, K3Value initialization, K3Value data)
        {
            // Handle vector case (over)
            if (data is VectorValue dataVec)
            {
                return OverVector(verb, initialization, dataVec);
            }
            
            // Handle matrix case (VectorValue of VectorValues)
            if (data is VectorValue matrixData && matrixData.Elements.Count > 0 && matrixData.Elements[0] is VectorValue)
            {
                return OverMatrix(verb, initialization, matrixData);
            }
            
            // Handle scalar case
            if (IsScalar(data))
            {
                return data;
            }
            
            throw new Exception($"Over not implemented for types: {verb.Type}, {data.Type}");
        }
        
        private K3Value OverVector(K3Value verb, K3Value initialization, VectorValue dataVec)
        {
            // Special case: empty vector
            if (dataVec.Elements.Count == 0)
            {
                return HandleEmptyVectorOver(verb, initialization);
            }
            
            // If initialization is 0, use first element as initialization (K behavior for / without explicit init)
            if (initialization is IntegerValue intInit && intInit.Value == 0 && dataVec.Elements.Count > 0)
            {
                return OverVectorWithFirstElementInit(verb, dataVec);
            }
            else
            {
                return OverVectorWithProvidedInit(verb, initialization, dataVec);
            }
        }
        
        private K3Value HandleEmptyVectorOver(K3Value verb, K3Value initialization)
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
            // For other verbs with empty vectors, return initialization value
            else
            {
                return initialization;
            }
        }
        
        private K3Value OverVectorWithFirstElementInit(K3Value verb, VectorValue dataVec)
        {
            var result = dataVec.Elements[0]; // Use first element as starting point
            var startIndex = 1; // Start from second element
            
            if (verb is SymbolValue verbSymbol)
            {
                // Apply verb to remaining elements
                for (int i = startIndex; i < dataVec.Elements.Count; i++)
                {
                    result = ApplySymbolVerb(verbSymbol.Value, result, dataVec.Elements[i]);
                }
            }
            else
            {
                // If verb is not a symbol, treat it as a value to apply with operator
                for (int i = startIndex; i < dataVec.Elements.Count; i++)
                {
                    result = ApplySymbolVerbWithOperator(verb, result, dataVec.Elements[i]);
                }
            }
            
            return result;
        }
        
        private K3Value OverVectorWithProvidedInit(K3Value verb, K3Value initialization, VectorValue dataVec)
        {
            var result = initialization;
            
            if (verb is SymbolValue verbSymbol)
            {
                // Apply verb to each element of vector, accumulating result
                for (int i = 0; i < dataVec.Elements.Count; i++)
                {
                    result = ApplySymbolVerb(verbSymbol.Value, result, dataVec.Elements[i]);
                }
            }
            else
            {
                // If verb is not a symbol, treat it as a value to apply with operator
                for (int i = 0; i < dataVec.Elements.Count; i++)
                {
                    result = ApplySymbolVerbWithOperator(verb, result, dataVec.Elements[i]);
                }
            }
            
            return result;
        }
        
        private K3Value OverMatrix(K3Value verb, K3Value initialization, VectorValue matrixData)
        {
            var result = new List<K3Value>();
            
            if (verb is SymbolValue verbSymbol)
            {
                return OverMatrixWithSymbolVerb(verbSymbol, initialization, matrixData);
            }
            else
            {
                return OverMatrixWithValueVerb(verb, initialization, matrixData);
            }
        }
        
        private K3Value OverMatrixWithSymbolVerb(SymbolValue verbSymbol, K3Value initialization, VectorValue matrixData)
        {
            var result = new List<K3Value>();
            
            // For each row in the matrix, apply the verb over that row
            for (int i = 0; i < matrixData.Elements.Count; i++)
            {
                var row = (VectorValue)matrixData.Elements[i];
                var rowResult = OverVector(verbSymbol, initialization, row);
                result.Add(rowResult);
            }
            
            return new VectorValue(result);
        }
        
        private K3Value OverMatrixWithValueVerb(K3Value verb, K3Value initialization, VectorValue matrixData)
        {
            var result = new List<K3Value>();
            
            for (int i = 0; i < matrixData.Elements.Count; i++)
            {
                var row = (VectorValue)matrixData.Elements[i];
                var rowResult = OverVector(verb, initialization, row);
                result.Add(rowResult);
            }
            
            return new VectorValue(result);
        }

        private K3Value Scan(K3Value verb, K3Value initialization, K3Value data)
        {
            // Handle vector case with initialization
            if (data is VectorValue dataVec && dataVec.Elements.Count > 0)
            {
                return ScanVector(verb, initialization, dataVec);
            }
            
            return data;
        }
        
        private K3Value ScanVector(K3Value verb, K3Value initialization, VectorValue dataVec)
        {
            // If initialization is 0, use first element as initialization (K behavior for \ without explicit init)
            if (initialization is IntegerValue intInit && intInit.Value == 0 && dataVec.Elements.Count > 0)
            {
                return ScanVectorWithFirstElementInit(verb, dataVec);
            }
            else
            {
                return ScanVectorWithProvidedInit(verb, initialization, dataVec);
            }
        }
        
        private K3Value ScanVectorWithFirstElementInit(K3Value verb, VectorValue dataVec)
        {
            var result = new List<K3Value>();
            var current = dataVec.Elements[0]; // Use first element as starting point
            result.Add(current); // Add first element to result
            
            var startIndex = 1; // Start from second element
            
            if (verb is SymbolValue verbSymbol)
            {
                // Apply verb to remaining elements
                for (int i = startIndex; i < dataVec.Elements.Count; i++)
                {
                    current = ApplySymbolVerb(verbSymbol.Value, current, dataVec.Elements[i]);
                    result.Add(current);
                }
            }
            else
            {
                // If verb is not a symbol, treat it as a value to apply with operator
                for (int i = startIndex; i < dataVec.Elements.Count; i++)
                {
                    current = ApplySymbolVerbWithOperator(verb, current, dataVec.Elements[i]);
                    result.Add(current);
                }
            }
            
            return new VectorValue(result, DetermineVectorType(result));
        }
        
        private K3Value ScanVectorWithProvidedInit(K3Value verb, K3Value initialization, VectorValue dataVec)
        {
            var result = new List<K3Value>();
            var current = initialization;
            
            // Add initialization value as first element
            result.Add(current);
            
            if (verb is SymbolValue verbSymbol)
            {
                // Apply verb to each element, accumulating result
                for (int i = 0; i < dataVec.Elements.Count; i++)
                {
                    current = ApplySymbolVerb(verbSymbol.Value, current, dataVec.Elements[i]);
                    result.Add(current);
                }
            }
            else
            {
                // If verb is not a symbol, treat it as a value to apply with operator
                for (int i = 0; i < dataVec.Elements.Count; i++)
                {
                    current = ApplySymbolVerbWithOperator(verb, current, dataVec.Elements[i]);
                    result.Add(current);
                }
            }
            
            return new VectorValue(result, DetermineVectorType(result));
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
                        
                        result.Add(ApplySymbolVerb(verbSymbol.Value, leftElement, rightElement));
                    }
                    
                    return ApplySymbolVerb(verbSymbol.Value, left, right);
                }
            }
            
            throw new Exception($"Each not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachRight(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Right (/:): Apply verb to each element of right with entire left
            // Natural nested evaluation: produce inputs for next inner level
            
            if (verb is FunctionValue func)
            {
                // This is a nested adverb function - call it with left and each right element
                if (right is VectorValue rightVec)
                {
                    var result = new List<K3Value>();
                    foreach (var rightElement in rightVec.Elements)
                    {
                        // Call the function with left argument and right element
                        var funcResult = ExecuteFunction(func, new List<K3Value> { left, rightElement });
                        result.Add(funcResult);
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (IsScalar(right))
                {
                    return ExecuteFunction(func, new List<K3Value> { left, right });
                }
            }
            else if (verb is SymbolValue verbSymbol)
            {
                // This is a base verb - apply it directly
                if (right is VectorValue rightVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in rightVec.Elements)
                    {
                        result.Add(ApplySymbolVerb(verbSymbol.Value, left, element));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (left is VectorValue leftVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in leftVec.Elements)
                    {
                        result.Add(ApplySymbolVerb(verbSymbol.Value, element, right));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (IsScalar(left))
                {
                    return ApplySymbolVerb(verbSymbol.Value, left, right);
                }
            }
            
            throw new Exception($"EachRight not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachLeft(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Left (\:): Apply verb to entire right with each element of left
            // Natural nested evaluation: produce inputs for next inner level
            
            if (verb is FunctionValue func)
            {
                // Check if this is a nested each-right function
                if (func.BodyText.StartsWith("EACH_RIGHT:"))
                {
                    // Extract the original verb from the function body
                    var verbStr = func.BodyText.Substring("EACH_RIGHT:".Length);
                    
                    // Parse the verb back to a K3Value
                    K3Value originalVerb;
                    if (verbStr.StartsWith("`") && verbStr.EndsWith("`"))
                    {
                        originalVerb = new SymbolValue(verbStr.Substring(1, verbStr.Length - 2));
                    }
                    else
                    {
                        // For now, assume it's a symbol (without backticks)
                        originalVerb = new SymbolValue(verbStr);
                    }
                    
                    // Apply each-right behavior with each left element
                    if (left is VectorValue leftVec)
                    {
                        var result = new List<K3Value>();
                        foreach (var leftElement in leftVec.Elements)
                        {
                            var eachRightResult = EachRight(originalVerb, leftElement, right);
                            result.Add(eachRightResult);
                        }
                        int vectorType = DetermineVectorType(result);
                        return new VectorValue(result, vectorType);
                    }
                    else if (IsScalar(left))
                    {
                        return EachRight(originalVerb, left, right);
                    }
                }
                else
                {
                    // This is a regular function - call it with each left element and right
                    if (left is VectorValue leftVec)
                    {
                        var result = new List<K3Value>();
                        foreach (var leftElement in leftVec.Elements)
                        {
                            var funcResult = ExecuteFunction(func, new List<K3Value> { leftElement, right });
                            result.Add(funcResult);
                        }
                        int vectorType = DetermineVectorType(result);
                        return new VectorValue(result, vectorType);
                    }
                    else if (IsScalar(left))
                    {
                        return ExecuteFunction(func, new List<K3Value> { left, right });
                    }
                }
            }
            else if (verb is SymbolValue verbSymbol)
            {
                // This is a base verb - apply it directly
                if (left is VectorValue leftVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in leftVec.Elements)
                    {
                        result.Add(ApplySymbolVerb(verbSymbol.Value, element, right));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (IsScalar(left))
                {
                    return ApplySymbolVerb(verbSymbol.Value, left, right);
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
                            result.Add(ApplySymbolVerb(verbSymbol.Value, current, prior));
                        }
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (IsScalar(right))
                {
                    return ApplySymbolVerb(verbSymbol.Value, right, left);
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
                        result.Add(ApplySymbolVerb(verbSymbol.Value, left, right));
                    }
                    else
                    {
                        // Handle case where verb is a scalar value (for mixed operations)
                        result.Add(ApplySymbolVerbWithOperator(verb, left, right));
                    }
                }
                int vectorType = DetermineVectorType(result);
                return new VectorValue(result, vectorType);
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
                            result.Add(ApplySymbolVerbWithOperator(verb, element, new NullValue()));
                        }
                    }
                }
                int vectorType = DetermineVectorType(result);
                return new VectorValue(result, vectorType);
            }
            
            // Handle scalar + scalar case (legacy)
            if (IsScalar(verb) && IsScalar(data))
            {
                if (verb is SymbolValue verbSymbol)
                {
                    return ApplySymbolVerb(verbSymbol.Value, verb, data);
                }
                else
                {
                    return ApplySymbolVerbWithOperator(verb, verb, data);
                }
            }
            
            throw new Exception($"Each not implemented for types: {verb.Type}, {data.Type}");
        }

        private K3Value EvaluateAdverbNode(string adverbType, K3Value left, K3Value right)
        {
            Console.WriteLine($"DEBUG EvaluateAdverbNode: adverbType={adverbType}, left={left}, right={right}");
            return adverbType switch
            {
                "ADVERB_SLASH" => ApplyAdverbSlash(new SymbolValue("+"), left, right),
                "ADVERB_BACKSLASH" => ApplyAdverbBackslash(new SymbolValue("+"), left, right),
                "ADVERB_TICK" => ApplyAdverbTick(new SymbolValue("+"), left, right),
                "ADVERB_SLASH_COLON" => ApplyAdverbSlashColon(new SymbolValue("+"), left, right),
                "ADVERB_BACKSLASH_COLON" => ApplyAdverbBackslashColon(new SymbolValue("+"), left, right),
                "ADVERB_TICK_COLON" => ApplyAdverbTickColon(new SymbolValue("+"), left, right),
                _ => throw new Exception($"Unknown adverb: {adverbType}")
            };
        }

        private int DetermineVectorType(List<K3Value> elements)
        {
            if (elements.Count == 0) return 0; // Empty list
            
            // Check if all elements are the same type
            var firstType = elements[0].Type;
            var allSameType = elements.All(e => e.Type == firstType);
            
            if (!allSameType) return 0; // Mixed types = generic list
            
            return firstType switch
            {
                ValueType.Integer => -1,   // Integer vector
                ValueType.Long => -64,     // Long vector  
                ValueType.Float => -2,     // Float vector
                ValueType.Character => -3, // Character vector
                ValueType.Symbol => -4,    // Symbol vector
                _ => 0                      // Default to generic list
            };
        }
    }
}
