using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        private K3Value Format(K3Value operand)
        {
            // Unary $ operator - convert to string representation
            // For vectors, preserve structure and convert each element to string
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
                        // Single character - enlist it
                        return new VectorValue(new List<K3Value> { vec.Elements[0] });
                    }
                    else
                    {
                        // Multiple characters - return as-is (already a character vector)
                        return vec;
                    }
                }
                
                // Regular vector - recursively format each element and enlist result
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    var formattedElement = FormatRecursive(element);
                    // Enlist the formatted element as a single-element vector
                    result.Add(new VectorValue(new List<K3Value> { formattedElement }));
                }
                return new VectorValue(result);
            }
            else
            {
                // For non-vector values, convert to string and create character vector
                string str;
                if (value is SymbolValue sym)
                    str = sym.Value;
                else if (value is CharacterValue charVal)
                    str = charVal.Value; // Use raw value, not ToString() which adds quotes
                else
                    str = value.ToString();
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
            // These only work on character vectors according to spec
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
                // Format specifier 0: converts non-character vectors
                // For homogeneous non-char vectors: string conversion per element
                // For mixed-type vectors: empty string per element
                // For scalars: empty string
                if (intFormat.Value == 0)
                {
                    if (right is VectorValue vec0 && vec0.Elements.Count > 0 &&
                        !vec0.Elements.All(e => e is CharacterValue))
                    {
                        var elementTypes = vec0.Elements.Select(e => e.GetType()).Distinct().ToList();
                        if (elementTypes.Count == 1)
                        {
                            // Homogeneous non-character vector: convert each to string
                            var result = new List<K3Value>();
                            foreach (var element in vec0.Elements)
                            {
                                result.Add(Format(element));
                            }
                            return new VectorValue(result);
                        }
                        else
                        {
                            // Mixed-type vector: return empty string per element
                            var result = new List<K3Value>();
                            foreach (var element in vec0.Elements)
                            {
                                result.Add(new VectorValue(new List<K3Value>(), -3)); // empty char vector
                            }
                            return new VectorValue(result);
                        }
                    }
                    return new CharacterValue("");
                }
                return FormatWithSpecifier(intFormat.Value, right);
            }
            else if (left is LongValue longFormat)
            {
                if (longFormat.Value == 0)
                {
                    return new CharacterValue("");
                }
                return FormatWithSpecifier((int)longFormat.Value, right);
            }
            else if (left is FloatValue floatFormat)
            {
                return FormatWithFloatSpecifier(floatFormat.Value, right);
            }
            else
            {
                throw new Exception($"Invalid format specifier: {left}");
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
                // Apply formatting to each element of vector and enlist each result
                var result = new List<K3Value>();
                foreach (var element in regularVec.Elements)
                {
                    var formattedElement = FormatElement(formatSpec, element);
                    // Enlist the formatted element as a single-element vector
                    if (formattedElement is CharacterValue charVal)
                    {
                        result.Add(new VectorValue(new List<K3Value> { charVal }));
                    }
                    else if (formattedElement is VectorValue formattedVec)
                    {
                        result.Add(new VectorValue(new List<K3Value> { formattedVec }));
                    }
                    else
                    {
                        result.Add(new VectorValue(new List<K3Value> { formattedElement }));
                    }
                }
                return new VectorValue(result);
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
            else if (value is FloatValue floatVal)
            {
                // Integer format spec: format float as integer (truncate decimals)
                if (floatVal.Value == Math.Floor(floatVal.Value) && !double.IsInfinity(floatVal.Value) && !double.IsNaN(floatVal.Value))
                {
                    str = ((long)floatVal.Value).ToString();
                }
                else
                {
                    str = value.ToString();
                }
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
                // If str.Length >= formatSpec, return as-is (no truncation)
            }
            else if (formatSpec < 0)
            {
                // Negative: pad with spaces on the right
                int targetLength = Math.Abs(formatSpec);
                if (str.Length < targetLength)
                {
                    str = str.PadRight(targetLength);
                }
                // If str.Length >= targetLength, return as-is (no truncation)
            }
            
            // According to K3 spec: single character results should be enlisted
            // to make them 1-item character vectors, e.g., ,"a" or ,"1"
            if (str.Length == 1)
            {
                return new VectorValue(new List<K3Value> { new CharacterValue(str) });
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
                // Apply formatting to each element of vector and enlist each result
                var result = new List<K3Value>();
                foreach (var element in regularVec.Elements)
                {
                    var formattedElement = FormatFloatElement(formatSpec, element);
                    // Enlist the formatted element as a single-element vector
                    if (formattedElement is CharacterValue charVal)
                    {
                        result.Add(new VectorValue(new List<K3Value> { charVal }));
                    }
                    else if (formattedElement is VectorValue formattedVec)
                    {
                        result.Add(new VectorValue(new List<K3Value> { formattedVec }));
                    }
                    else
                    {
                        result.Add(new VectorValue(new List<K3Value> { formattedElement }));
                    }
                }
                return new VectorValue(result);
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
                return new VectorValue(new List<K3Value> { new CharacterValue(str) });
            }
            
            return new CharacterValue(str);
        }
    }
}
