using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        
        private K3Value ApplySymbolVerb(string verbName, K3Value left, K3Value right)
        {
            // Handle SymbolValue verbs by name
            
            // Check if it's a built-in operator (single character)
            if (verbName.Length == 1)
            {
                return ApplyBuiltInOperator(verbName[0], left, right);
            }
            
            // For all other verbs, use AST approach with preserved verb name
            var verbNode = new ASTNode(ASTNodeType.DyadicOp, new SymbolValue(verbName));
            verbNode.Children.Add(ASTNode.MakeLiteral(left));
            verbNode.Children.Add(ASTNode.MakeLiteral(right));
            
            return Evaluate(verbNode);
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
            // Use VerbRegistry to handle all monadic verbs dynamically
            if (VerbRegistry.HasVerb(verbName))
            {
                var verbInfo = VerbRegistry.GetVerb(verbName);
                if (verbInfo != null && verbInfo.SupportedArities.Contains(1))
                {
                    // Handle system verbs with dedicated methods
                    return verbName switch
                    {
                        "_ci" => Ci(operand),
                        "_ic" => Ic(operand),
                        // Basic operators
                        "+" => operand,  // Identity operation
                        "-" => Negate(operand),
                        "*" => First(operand),
                        "%" => Reciprocal(operand),
                        "&" => Where(operand),
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
                        _ => throw new Exception($"Verb '{verbName}' is registered as monadic but not implemented in ApplyUnaryVerb")
                    };
                }
            }
            
            throw new Exception($"Unknown unary verb: {verbName}");
        }

        private K3Value ApplySymbolVerbWithOperator(K3Value verb, K3Value left, K3Value right)
        {
            // Handle case where verb is a value (like 2 +/ 1 2 3)
            // This means we should use verb as left operand with operator
            if (verb is SymbolValue verbSymbol)
            {
                return ApplySymbolVerb(verbSymbol.Value, verb, right);
            }
            else if (verb is FunctionValue function)
            {
                // For functions, execute the function with left and right as arguments
                return ExecuteFunction(function, new List<K3Value> { left, right });
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

        private K3Value ApplyAdverbSlash(K3Value verb, K3Value left, K3Value right)
        {
            // For adverb slash /:
            // If left is dummy 0 and right is vector, use Over (e.g., +/ 1 2 3 4 5)
            // If left is vector and right is scalar, use Each (e.g., (1 2 3) %/ 2)
            // If left is vector and right is vector, use Each (e.g., (1 2 3) %/ (4 5 6))
            // If only right argument, use Over (e.g., %/ 1 2 3)
            
            // Check for "over" case: left is dummy 0 and right is vector
            if (left is IntegerValue leftInt && leftInt.Value == 0 && right is VectorValue)
            {
                return Over(verb, left, right);
            }
            
            // Check for "each" case: left is vector and right is scalar
            if (left is VectorValue && IsScalar(right))
            {
                return Each(verb, left, right);
            }
            
            // Check for vector-vector case
            if (left is VectorValue && right is VectorValue)
            {
                return Each(verb, left, right);
            }
            
            // Default case: use Over
            return Over(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
        }

        private K3Value ApplyAdverbBackslash(K3Value verb, K3Value left, K3Value right)
        {
            // Natural nested evaluation: call Scan with the verb and arguments
            return Scan(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
        }

        public K3Value HandleAdverbTick(K3Value verb, K3Value left, K3Value right)
        {
            // Special handling for system verbs with each adverb
            if (verb is SymbolValue verbSymbol)
            {
                var verbName = verbSymbol.Value;
                if (verbName == "_ci")
                {
                    // Handle _ci' pattern - apply _ci to each element
                    return HandleCiEach(left, right);
                }
                else if (verbName == "_ic")
                {
                    // Handle _ic' pattern - apply _ic to each element
                    return HandleIcEach(left, right);
                }
                // Add more system verbs as needed
            }
            
            // Check if this is a monadic verb with each (left is dummy value)
            if (left is IntegerValue leftInt && leftInt.Value == 0 && right is VectorValue dataVec)
            {
                // This is a monadic verb with each - call 2-argument Each
                return Each(verb, dataVec);
            }
            
            // Default to 3-argument Each for dyadic verbs
            return Each(verb, left, right);
        }
        
        /// <summary>
        /// Handle _ci' each adverb pattern
        /// </summary>
        /// <param name="left">Left argument (usually dummy value)</param>
        /// <param name="right">Right argument (data vector)</param>
        /// <returns>Vector of characters</returns>
        private K3Value HandleCiEach(K3Value left, K3Value right)
        {
            // Apply _ci to each element in the right argument
            if (right is VectorValue dataVec)
            {
                var result = new List<K3Value>();
                foreach (var element in dataVec.Elements)
                {
                    // Apply _ci monadic operation
                    var ciResult = Ci(element);
                    result.Add(ciResult);
                }
                
                // If all results are characters, return as character vector
                if (result.All(r => r is CharacterValue))
                {
                    return new VectorValue(result);
                }
                else
                {
                    return new VectorValue(result);
                }
            }
            
            // If right is not a vector, apply _ci to it directly
            return Ci(right);
        }
        
        /// <summary>
        /// Handle _ic' each adverb pattern
        /// </summary>
        /// <param name="left">Left argument (usually dummy value)</param>
        /// <param name="right">Right argument (data vector)</param>
        /// <returns>Vector of integers</returns>
        private K3Value HandleIcEach(K3Value left, K3Value right)
        {
            // Apply _ic to each element in the right argument
            if (right is VectorValue dataVec)
            {
                var result = new List<K3Value>();
                foreach (var element in dataVec.Elements)
                {
                    // Apply _ic monadic operation
                    var icResult = Ic(element);
                    result.Add(icResult);
                }
                
                return new VectorValue(result);
            }
            
            // If right is not a vector, apply _ic to it directly
            return Ic(right);
        }

        private K3Value ApplyAdverbTick(K3Value verb, K3Value left, K3Value right)
        {
            // Check if this is a monadic verb with each (left and right are dummy values)
            if ((left is IntegerValue leftInt && leftInt.Value == 0) && 
                (right is IntegerValue rightInt && rightInt.Value == 0) && 
                verb is SymbolValue)
            {
                // This is a monadic verb with each - call 2-argument Each
                // The data will be provided by the caller
                return Each(verb, new IntegerValue(0)); // Dummy data for now
            }
            
            // For dyadic verbs, call 3-argument Each
            return Each(verb, left, right);
        }

        private K3Value ApplyAdverbSlashColon(K3Value verb, K3Value left, K3Value right)
        {
            // One-adverb-at-a-time: consume just the outer adverb (/:), preserve inner verb for next step
            // Check if this is a nested adverb call (has verb, left, right arguments)
            // Use sentinel values to distinguish between "no arguments" and "actual arguments"
            bool hasLeftArg = !(left is IntegerValue leftInt && leftInt.Value == 0);
            bool hasRightArg = !(right is IntegerValue rightInt && rightInt.Value == 0);
            
            if (hasLeftArg && hasRightArg)
            {
                // One-adverb-at-a-time: apply just the outer adverb (/:) with preserved inner verb
                // This creates natural nested evaluation without complex chaining
                return EachRight(verb, left, right);
            }
            
            // For simple cases (nominalized adverb), return a function that represents "each-right of verb"
            // Store the verb in the function's BodyText for later use - this preserves the inner verb
            string verbStr = verb is SymbolValue sym ? sym.Value : verb.ToString();
            var lambda = new FunctionValue($"EACH_RIGHT:{verbStr}", new List<string> { "x", "y" });
            return lambda;
        }

        private K3Value ApplyAdverbBackslashColon(K3Value verb, K3Value left, K3Value right)
        {
            // One-adverb-at-a-time: consume just the outer adverb (\:), preserve inner verb for next step
            // Natural nested evaluation: call EachLeft with the verb and arguments
            // This creates natural nested evaluation without complex chaining
            return EachLeft(verb, left ?? new IntegerValue(0), right ?? new IntegerValue(0));
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
                // Check if this is a monadic verb with each (left is dummy value)
                if (left is IntegerValue leftInt && leftInt.Value == 0 && right is VectorValue dataVec)
                {
                    // This is a monadic verb with each - apply verb to each element of dataVec
                    var result = new List<K3Value>();
                    foreach (var element in dataVec.Elements)
                    {
                        result.Add(ApplyUnaryVerb(verbSymbol.Value, element));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                
                // Handle vector + scalar case (e.g., (1 2 3) %/ 2)
                if (left is VectorValue leftVec && IsScalar(right))
                {
                    // Apply dyadic operation element-wise with scalar right
                    var result = new List<K3Value>();
                    foreach (var leftElement in leftVec.Elements)
                    {
                        result.Add(ApplySymbolVerb(verbSymbol.Value, leftElement, right));
                    }
                    
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                
                // Handle vector + vector case (same length) - should behave like default operator
                if (left is VectorValue leftVec2 && right is VectorValue rightVec)
                {
                    // Check if vectors have different lengths - should throw length error
                    if (leftVec2.Elements.Count != rightVec.Elements.Count)
                    {
                        throw new Exception($"length error: {leftVec2.Elements.Count} != {rightVec.Elements.Count}");
                    }
                    
                    // Apply dyadic operation element-wise (same as default operator behavior)
                    var result = new List<K3Value>();
                    for (int i = 0; i < leftVec2.Elements.Count; i++)
                    {
                        var leftElement = leftVec2.Elements[i];
                        var rightElement = rightVec.Elements[i];
                        
                        result.Add(ApplySymbolVerb(verbSymbol.Value, leftElement, rightElement));
                    }
                    
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
            }
            
            throw new Exception($"Each not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachRight(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Right (/:): Apply verb to each element of right with entire left
            // One-adverb-at-a-time: consume just the outer adverb, preserve inner verb
            
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
                // This is a base verb - apply it directly using one-adverb-at-a-time
                if (right is VectorValue rightVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in rightVec.Elements)
                    {
                        // Apply the base verb with preserved verb name
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
            // One-adverb-at-a-time: consume just the outer adverb, preserve inner verb
            
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
                // This is a base verb - apply it directly using one-adverb-at-a-time
                if (left is VectorValue leftVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in leftVec.Elements)
                    {
                        // Apply the base verb with preserved verb name
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
            // Handle monadic verb with vector data (e.g., #:' 1 2 3)
            if (IsScalar(verb) && data is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    if (verb is SymbolValue verbSymbol)
                    {
                        // For each operations, apply verb as a unary operation to each element
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
            
            // Legacy 2-argument call for backward compatibility
            if (verb is VectorValue verbVec && data is VectorValue dataVec)
            {
                // Check if vectors have different lengths - should throw length error
                if (verbVec.Elements.Count != dataVec.Elements.Count)
                {
                    throw new Exception($"length error: {verbVec.Elements.Count} != {dataVec.Elements.Count}");
                }
                
                // Apply dyadic operation element-wise
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
            if (IsScalar(verb) && data is VectorValue dataVector)
            {
                var result = new List<K3Value>();
                foreach (var element in dataVector.Elements)
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
