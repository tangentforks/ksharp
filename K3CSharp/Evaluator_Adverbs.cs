using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        private K3Value ApplyVerb(string verbName, K3Value left, K3Value right)
        {
            // Special handling for _dot with scalar arguments
            if (verbName == "_dot")
            {
                // If left is scalar, wrap it in a single-element vector
                if (IsScalar(left))
                {
                    left = new VectorValue(new List<K3Value> { left });
                }
                // If right is scalar, wrap it in a single-element vector  
                if (IsScalar(right))
                {
                    right = new VectorValue(new List<K3Value> { right });
                }
            }
            
            return verbName switch
            {
                // Basic operators
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
                "#" => Take(left, right),
                "_" => FloorBinary(left, right),
                "@" => AtIndex(left, right),
                "." => DotApply(left, right),
                "$" => Format(left, right),
                "~" => Match(left, right),
                "?" => Find(left, right),
                
                // System verbs
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

        private K3Value ApplyVerbWithOperator(K3Value verb, K3Value left, K3Value right)
        {
            // Handle case where verb is a value (like 2 +/ 1 2 3)
            // This means we should use verb as left operand with operator
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
                    case "ADVERB_SLASH_COLON":
                        result = ApplyAdverbSlashColon(result);
                        break;
                    case "ADVERB_BACKSLASH_COLON":
                        result = ApplyAdverbBackslashColon(result);
                        break;
                    case "ADVERB_TICK_COLON":
                        result = ApplyAdverbTickColon(result);
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
            return new VectorValue(new List<K3Value> { verb, left, right }, -1);
        }

        private K3Value ApplyAdverbSlash(K3Value operand)
        {
            // Over/Reduce (/): Apply verb to reduce over data
            // Check if operand has verb/data structure from adverb processing
            if (operand is VectorValue vec && vec.Elements.Count >= 3)
            {
                // Structure: [verb, left, right] from adverb processing
                var verb = vec.Elements[0];
                var left = vec.Elements[1];
                var right = vec.Elements[2];
                return Over(verb, left, right);
            }
            
            // For simple cases, execute the adverb operation directly
            // This represents a nominalized adverb that can be used in chains
            return new VectorValue(new List<K3Value> { operand }, -1); // Wrap in vector to indicate nominalized adverb
        }

        private K3Value ApplyAdverbBackslash(K3Value operand)
        {
            // Scan (\): Apply verb to scan over data
            // Check if operand has verb/data structure from adverb processing
            if (operand is VectorValue vec && vec.Elements.Count >= 3)
            {
                // Structure: [verb, left, right] from adverb processing
                var verb = vec.Elements[0];
                var left = vec.Elements[1];
                var right = vec.Elements[2];
                return Scan(verb, left, right);
            }
            
            // For simple cases, execute the adverb operation directly
            // This represents a nominalized adverb that can be used in chains
            return new VectorValue(new List<K3Value> { operand }, -1); // Wrap in vector to indicate nominalized adverb
        }

        private K3Value ApplyAdverbTick(K3Value operand)
        {
            // Each ('): Apply verb to each element of data
            // Check if operand has verb/data structure from adverb processing
            if (operand is VectorValue vec && vec.Elements.Count >= 3)
            {
                // Structure: [verb, left, right] from adverb processing
                var verb = vec.Elements[0];
                var left = vec.Elements[1];
                var right = vec.Elements[2];
                return Each(verb, left, right);
            }
            
            // For simple cases, execute the adverb operation directly
            // This represents a nominalized adverb that can be used in chains
            return new VectorValue(new List<K3Value> { operand }, -1); // Wrap in vector to indicate nominalized adverb
        }

        private K3Value ApplyAdverbSlashColon(K3Value operand)
        {
            // Each-Right (/:): Apply verb to each element of left with entire right
            // Check if operand has verb/data structure from previous adverb processing
            if (operand is VectorValue vec && vec.Elements.Count >= 3)
            {
                // Structure: [verb, left, right] from adverb processing
                var verb = vec.Elements[0];
                var left = vec.Elements[1];
                var right = vec.Elements[2];
                
                // Special handling for comma operator in adverb chains
                if (verb is SymbolValue commaSymbol && commaSymbol.Value == ",")
                {
                    Console.WriteLine($"DEBUG ApplyAdverbSlashColon: Creating comma /: function for chaining");
                    // For comma /: right, create the function {x ,/: y} that will be used by the next adverb
                    var functionBody = "x ,/: y";
                    var commaFunction = new FunctionValue(functionBody, new List<string>{"x", "y"}, new List<Token>(), functionBody);
                    
                    return commaFunction;
                }
                
                return EachRight(verb, left, right);
            }
            
            // For simple cases, create a composite verb structure that can be used by subsequent adverbs
            // This represents a "verb+adverb" structure that can be further modified
            return new VectorValue(new List<K3Value> { operand }, -1); // Wrap the verb in a vector to indicate it's a composite verb
        }

        private K3Value ApplyAdverbBackslashColon(K3Value operand)
        {
            // Each-Left (\:): Apply verb to entire left with each element of right
            
            // Check if operand is a function from previous adverb processing
            if (operand is FunctionValue prevFunc)
            {
                Console.WriteLine($"DEBUG ApplyAdverbBackslashColon: operand is function from previous adverb with body '{prevFunc.BodyText}'");
                
                // This is a function from the previous adverb, we need to apply \: to it
                // For {x ,/: y} \: left right, we need to apply the function to each element of left with right
                // But we need to get the left and right arguments from the calling context
                // The calling context should pass us a vector with [function, left, right]
                
                // For adverb chaining, we need to execute the function immediately
                // The issue is that we need to get the actual arguments from the parser structure
                Console.WriteLine($"DEBUG ApplyAdverbBackslashColon: attempting to execute chained adverb function");
                
                // For now, we'll create a special case for comma adverb chaining
                if (prevFunc.BodyText == "x ,/: y")
                {
                    Console.WriteLine($"DEBUG ApplyAdverbBackslashColon: detected comma /: function, attempting execution");
                    
                    // This is the comma /: function, we need to execute it with the arguments
                    // But we need to get the arguments from somewhere - they should be in the parent context
                    // For now, let's return a special marker that the evaluator can recognize
                    return new VectorValue(new List<K3Value> { prevFunc }, -1);
                }
                
                // For now, let's create a composed function that represents {x ,/: y}\:
                string composedBody = $"{prevFunc.BodyText}\\:";
                Console.WriteLine($"DEBUG ApplyAdverbBackslashColon: creating composed function with body '{composedBody}'");
                
                var composedFunction = new FunctionValue(
                    composedBody,
                    new List<string>(),
                    new List<Token>(),
                    composedBody
                );
                
                return composedFunction;
            }
            
            // Check if operand has verb/data structure from previous adverb processing
            if (operand is VectorValue vec && vec.Elements.Count >= 3)
            {
                // Structure: [verb, left, right] from adverb processing
                var verb = vec.Elements[0];
                var left = vec.Elements[1];
                var right = vec.Elements[2];
                
                // Special handling for comma operator in adverb chains
                if (verb is SymbolValue commaSymbol && commaSymbol.Value == ",")
                {
                    Console.WriteLine($"DEBUG ApplyAdverbBackslashColon: Executing comma \\: directly");
                    // For comma \: right, apply comma to each element of left with entire right
                    // This is the direct execution of {x ,\: y}
                    if (left is VectorValue leftVec && right is VectorValue rightVec)
                    {
                        var results = new List<K3Value>();
                        foreach (var leftElement in leftVec.Elements)
                        {
                            // Apply comma to left element and entire right vector: leftElement , right
                            var result = Join(leftElement, right);
                            results.Add(result);
                        }
                        return new VectorValue(results, 0); // mixed list
                    }
                    else
                    {
                        // Fallback for non-vector arguments
                        return Join(left, right);
                    }
                }
                
                return EachLeft(verb, left, right);
            }
            
            // For simple cases, create a composite verb structure that can be used by subsequent adverbs
            return new VectorValue(new List<K3Value> { operand }, -1);
        }

        private K3Value ApplyAdverbTickColon(K3Value operand)
        {
            // Each-Prior (':): Apply verb to each element with previous element
            // Check if operand has verb/data structure from previous adverb processing
            if (operand is VectorValue vec && vec.Elements.Count >= 3)
            {
                // Structure: [verb, left, right] from adverb processing
                var verb = vec.Elements[0];
                var left = vec.Elements[1];
                var right = vec.Elements[2];
                return EachPrior(verb, left, right);
            }
            
            // For simple cases, create a composite verb structure that can be used by subsequent adverbs
            return new VectorValue(new List<K3Value> { operand }, -1);
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
                    // For other verbs with empty vectors, return initialization value
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
                        // If verb is not a symbol, treat it as a value to apply with operator
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
                        // Apply verb to each element of vector, accumulating result
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            result = ApplyVerb(verbSymbol.Value, result, dataVec.Elements[i]);
                        }
                    }
                    else
                    {
                        // If verb is not a symbol, treat it as a value to apply with operator
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            result = ApplyVerbWithOperator(verb, result, dataVec.Elements[i]);
                        }
                    }
                }
                
                return result;
            }
            
            // Handle matrix case (VectorValue of VectorValues)
            if (data is VectorValue matrixData && matrixData.Elements.Count > 0 && matrixData.Elements[0] is VectorValue)
            {
                // This is a matrix - apply verb to each row separately
                var result = new List<K3Value>();
                
                if (verb is SymbolValue verbSymbol)
                {
                    // For each row in the matrix, apply the verb over that row
                    for (int i = 0; i < matrixData.Elements.Count; i++)
                    {
                        var row = (VectorValue)matrixData.Elements[i];
                        K3Value current = initialization;
                        
                        // If initialization is 0, use first element as initialization (K behavior for / without explicit init)
                        if (initialization is IntegerValue intInit && intInit.Value == 0 && row.Elements.Count > 0)
                        {
                            current = row.Elements[0]; // Use first element as starting point
                            var startIndex = 1; // Start from second element
                            
                            // Apply verb to remaining elements of this row
                            for (int j = startIndex; j < row.Elements.Count; j++)
                            {
                                current = ApplyVerb(verbSymbol.Value, current, row.Elements[j]);
                            }
                        }
                        else
                        {
                            // Use provided initialization value
                            current = initialization;
                            for (int j = 0; j < row.Elements.Count; j++)
                            {
                                current = ApplyVerb(verbSymbol.Value, current, row.Elements[j]);
                            }
                        }
                        
                        result.Add(current);
                    }
                }
                else
                {
                    // If verb is not a symbol, treat it as a value to apply with operator
                    for (int i = 0; i < matrixData.Elements.Count; i++)
                    {
                        var row = (VectorValue)matrixData.Elements[i];
                        K3Value current = initialization;
                        
                        // If initialization is 0, use first element as initialization
                        if (initialization is IntegerValue intInit && intInit.Value == 0 && row.Elements.Count > 0)
                        {
                            current = row.Elements[0];
                            var startIndex = 1;
                            
                            for (int j = startIndex; j < row.Elements.Count; j++)
                            {
                                current = ApplyVerbWithOperator(verb, current, row.Elements[j]);
                            }
                        }
                        else
                        {
                            current = initialization;
                            for (int j = 0; j < row.Elements.Count; j++)
                            {
                                current = ApplyVerbWithOperator(verb, current, row.Elements[j]);
                            }
                        }
                        
                        result.Add(current);
                    }
                }
                
                return new VectorValue(result);
            }
            
            // Handle scalar case
            if (IsScalar(data))
            {
                return data;
            }
            
            throw new Exception($"Over not implemented for types: {verb.Type}, {data.Type}");
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
                        // If verb is not a symbol, treat it as a value to apply with operator
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
                    
                    // Add initialization value as first element
                    result.Add(current);
                    
                    if (verb is SymbolValue verbSymbol)
                    {
                        // Apply verb to each element, accumulating result
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            current = ApplyVerb(verbSymbol.Value, current, dataVec.Elements[i]);
                            result.Add(current);
                        }
                    }
                    else
                    {
                        // If verb is not a symbol, treat it as a value to apply with operator
                        for (int i = 0; i < dataVec.Elements.Count; i++)
                        {
                            current = ApplyVerbWithOperator(verb, current, dataVec.Elements[i]);
                            result.Add(current);
                        }
                    }
                }
                
                int vectorType = DetermineVectorType(result);
                return new VectorValue(result, vectorType);
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
                    
                    return ApplyVerb(verbSymbol.Value, left, right);
                }
            }
            
            throw new Exception($"Each not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachRight(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Right (/:): Apply verb to each element of left with entire right
            // When the argument to an adverbialized verb is a complex structure,
            // the verb+adverb will be applied to the outermost level of the structure.
            
            // Check if this is a chaining context: if right is a single scalar, 
            // this might be part of a chained adverb operation
            bool isChainingContext = IsScalar(right) && !(verb is VectorValue);
            
            if (verb is SymbolValue verbSymbol)
            {
                if (left is VectorValue leftVec && IsMatrix(leftVec) && !(verbSymbol.Value == "_dot" && right is VectorValue rightMatrix && IsMatrix(rightMatrix)))
                {
                                        var result = new List<K3Value>();
                    foreach (var element in leftVec.Elements)
                    {
                        // For complex structures, we need to apply the verb+adverb recursively
                        result.Add(EachRight(verb, element, right));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (right is VectorValue rightVec && IsMatrix(rightVec))
                {
                    var result = new List<K3Value>();
                    foreach (var element in rightVec.Elements)
                    {
                        // Special handling for _dot to use fallback operation
                        if (verbSymbol.Value == "_dot")
                        {
                            var fallbackResult = DotProductFallback(left, element);
                            result.Add(fallbackResult);
                        }
                        else
                        {
                            result.Add(ApplyVerb(verbSymbol.Value, left, element));
                        }
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (left is VectorValue leftVec2 && right is VectorValue rightVec2)
                {
                    // Special case for +/: to create matrix addition
                    if (verbSymbol.Value == "+")
                    {
                        // Create matrix where each row is left element + each right element
                        var result = new List<K3Value>();
                        foreach (var leftElement in leftVec2.Elements)
                        {
                            var row = new List<K3Value>();
                            foreach (var rightElement in rightVec2.Elements)
                            {
                                row.Add(Plus(leftElement, rightElement));
                            }
                            int rowVectorType = DetermineVectorType(row);
                            result.Add(new VectorValue(row, rowVectorType));
                        }
                        int matrixVectorType = DetermineVectorType(result);
                        return new VectorValue(result, matrixVectorType);
                    }
                    else
                    {
                        // For other verbs, use the regular ApplyVerb
                        var result = new List<K3Value>();
                        foreach (var element in leftVec2.Elements)
                        {
                            result.Add(ApplyVerb(verbSymbol.Value, element, right));
                        }
                        int vectorType = DetermineVectorType(result);
                        return new VectorValue(result, vectorType);
                    }
                }
                else if (left is VectorValue leftVec3)
                {
                    var result = new List<K3Value>();
                    foreach (var element in leftVec3.Elements)
                    {
                        result.Add(ApplyVerb(verbSymbol.Value, element, right));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (IsScalar(left))
                {
                    if (isChainingContext)
                    {
                        // In chaining context, create a composite verb structure
                        // This represents "verb+adverb" that can be further modified
                        return new VectorValue(new List<K3Value> { verb }, -1);
                    }
                    return ApplyVerb(verbSymbol.Value, left, right);
                }
            }
            else if (verb is VectorValue verbVec && verbVec.Elements.Count >= 1)
            {
                // Handle complex verb structures from chained adverbs
                var actualVerb = verbVec.Elements[0];
                if (actualVerb is SymbolValue)
                {
                    var symbolValue = (SymbolValue)actualVerb;
                    if (left is VectorValue leftVec)
                    {
                        var result = new List<K3Value>();
                        foreach (var element in leftVec.Elements)
                        {
                            result.Add(ApplyVerb(symbolValue.Value, element, right));
                        }
                        int vectorType = DetermineVectorType(result);
                        return new VectorValue(result, vectorType);
                    }
                    else if (IsScalar(left))
                    {
                        return ApplyVerb(symbolValue.Value, left, right);
                    }
                }
            }
            
            throw new Exception($"EachRight not implemented for types: {verb.Type}, {left.Type}, {right.Type}");
        }

        private K3Value EachLeft(K3Value verb, K3Value left, K3Value right)
        {
            // Each-Left (\:): Apply verb to entire right with each element of left
            
            // Check if this is a chaining context: if right is a single scalar, 
            // this might be part of a chained adverb operation
            bool isChainingContext = IsScalar(right) && !(verb is VectorValue);
            
            if (verb is SymbolValue verbSymbol)
            {
                if (left is VectorValue leftVec)
                {
                    var result = new List<K3Value>();
                    foreach (var element in leftVec.Elements)
                    {
                        result.Add(ApplyVerb(verbSymbol.Value, element, right));
                    }
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
                }
                else if (IsScalar(left))
                {
                    if (isChainingContext)
                    {
                        // In chaining context, create a composite verb structure
                        // This represents "verb+adverb" that can be further modified
                        return new VectorValue(new List<K3Value> { verb }, -1);
                    }
                    return ApplyVerb(verbSymbol.Value, left, right);
                }
            }
            else if (verb is VectorValue verbVec && verbVec.Elements.Count == 1)
            {
                // Handle composite verb structures from chained adverbs
                // The verb is wrapped in a vector to indicate it's a composite verb
                var actualVerb = verbVec.Elements[0];
                if (actualVerb is SymbolValue symbolValue)
                {
                    if (left is VectorValue leftVec)
                    {
                        var result = new List<K3Value>();
                        foreach (var element in leftVec.Elements)
                        {
                            result.Add(ApplyVerb(symbolValue.Value, element, right));
                        }
                        int vectorType = DetermineVectorType(result);
                        return new VectorValue(result, vectorType);
                    }
                    else if (IsScalar(left))
                    {
                        return ApplyVerb(symbolValue.Value, left, right);
                    }
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
                    int vectorType = DetermineVectorType(result);
                    return new VectorValue(result, vectorType);
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
                            result.Add(ApplyVerbWithOperator(verb, element, new NullValue()));
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
                    return ApplyVerb(verbSymbol.Value, verb, data);
                }
                else
                {
                    return ApplyVerbWithOperator(verb, verb, data);
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
