using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
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

        private bool IsScalar(K3Value value)
        {
            return value is IntegerValue || value is LongValue || value is FloatValue || 
                   value is CharacterValue || value is SymbolValue || value is NullValue;
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
                            result.Add(ApplyVerbWithOperator(verb, element, new NullValue()));
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
    }
}
