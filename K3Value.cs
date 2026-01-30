using System;
using System.Collections.Generic;

namespace K3CSharp
{
    public enum ValueType
    {
        Integer,
        Long,
        Float,
        Character,
        Symbol,
        Vector,
        Function,
        Null,
        Dictionary
    }

    public abstract class K3Value
    {
        public ValueType Type { get; protected set; }

        public abstract K3Value Add(K3Value other);
        public abstract K3Value Subtract(K3Value other);
        public abstract K3Value Multiply(K3Value other);
        public abstract K3Value Divide(K3Value other);
        public abstract override string ToString();
    }

    public class IntegerValue : K3Value
    {
        public int Value { get; }
        public bool IsSpecial { get; }
        public string SpecialName { get; }

        public IntegerValue(int value)
        {
            Value = value;
            Type = ValueType.Integer;
            
            // Check if this value matches any special integer patterns
            if (value == int.MaxValue)
            {
                IsSpecial = true;
                SpecialName = "0I";
            }
            else if (value == int.MinValue)
            {
                IsSpecial = true;
                SpecialName = "0N";
            }
            else if (value == int.MinValue + 1)
            {
                IsSpecial = true;
                SpecialName = "-0I";
            }
            else
            {
                IsSpecial = false;
                SpecialName = "";
            }
        }

        public IntegerValue(string specialName)
        {
            SpecialName = specialName;
            IsSpecial = true;
            Type = ValueType.Integer;
            
            // Set the actual integer values for special cases
            switch (specialName)
            {
                case "0I": Value = int.MaxValue; break;
                case "0N": Value = int.MinValue; break;
                case "-0I": Value = int.MinValue + 1; break;
                default: throw new ArgumentException($"Unknown special integer: {specialName}");
            }
        }

        public override K3Value Add(K3Value other)
        {
            if (other is IntegerValue intVal)
            {
                // Use unchecked arithmetic for all integers to allow natural overflow/underflow
                unchecked
                {
                    return new IntegerValue(Value + intVal.Value);
                }
            }
            if (other is LongValue longVal)
                return new LongValue(Value + longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value + floatVal.Value);
            
            throw new InvalidOperationException($"Cannot add Integer to {other.Type}");
        }

        public override K3Value Subtract(K3Value other)
        {
            if (other is IntegerValue intVal)
            {
                // Use unchecked arithmetic for all integers to allow natural overflow/underflow
                unchecked
                {
                    return new IntegerValue(Value - intVal.Value);
                }
            }
            if (other is LongValue longVal)
                return new LongValue(Value - longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value - floatVal.Value);
            
            throw new InvalidOperationException($"Cannot subtract Integer from {other.Type}");
        }

        public override K3Value Multiply(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new IntegerValue(Value * intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value * longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value * floatVal.Value);
            
            throw new InvalidOperationException($"Cannot multiply Integer by {other.Type}");
        }

        public override K3Value Divide(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new IntegerValue(Value / intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value / longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value / floatVal.Value);
            
            throw new InvalidOperationException($"Cannot divide Integer by {other.Type}");
        }

        public override string ToString()
        {
            if (IsSpecial)
                return SpecialName;
            return Value.ToString();
        }
    }

    public class LongValue : K3Value
    {
        public long Value { get; }
        public bool IsSpecial { get; }
        public string SpecialName { get; }

        public LongValue(long value)
        {
            Value = value;
            Type = ValueType.Long;
            
            // Check if this value matches any special long patterns
            if (value == long.MaxValue)
            {
                IsSpecial = true;
                SpecialName = "0IL";
            }
            else if (value == long.MinValue)
            {
                IsSpecial = true;
                SpecialName = "0NL";
            }
            else if (value == long.MinValue + 1)
            {
                IsSpecial = true;
                SpecialName = "-0IL";
            }
            else
            {
                IsSpecial = false;
                SpecialName = "";
            }
        }

        public LongValue(string specialName)
        {
            SpecialName = specialName;
            IsSpecial = true;
            Type = ValueType.Long;
            
            // Set the actual long values for special cases
            switch (specialName)
            {
                case "0IL": Value = long.MaxValue; break;
                case "0NL": Value = long.MinValue; break;
                case "-0IL": Value = long.MinValue + 1; break;
                default: throw new ArgumentException($"Unknown special long: {specialName}");
            }
        }

        public override K3Value Add(K3Value other)
        {
            if (other is IntegerValue intVal)
            {
                // Use unchecked arithmetic to allow natural overflow/underflow
                unchecked
                {
                    return new LongValue(Value + intVal.Value);
                }
            }
            if (other is LongValue longVal)
            {
                // Use unchecked arithmetic to allow natural overflow/underflow
                unchecked
                {
                    return new LongValue(Value + longVal.Value);
                }
            }
            if (other is FloatValue floatVal)
                return new FloatValue(Value + floatVal.Value);
            
            throw new InvalidOperationException($"Cannot add Long to {other.Type}");
        }

        public override K3Value Subtract(K3Value other)
        {
            if (other is IntegerValue intVal)
            {
                // Use unchecked arithmetic to allow natural overflow/underflow
                unchecked
                {
                    return new LongValue(Value - intVal.Value);
                }
            }
            if (other is LongValue longVal)
            {
                // Use unchecked arithmetic to allow natural overflow/underflow
                unchecked
                {
                    return new LongValue(Value - longVal.Value);
                }
            }
            if (other is FloatValue floatVal)
                return new FloatValue(Value - floatVal.Value);
            
            throw new InvalidOperationException($"Cannot subtract from Long");
        }

        public override K3Value Multiply(K3Value other)
        {
            if (other is IntegerValue intVal)
            {
                // Use unchecked arithmetic to allow natural overflow/underflow
                unchecked
                {
                    return new LongValue(Value * intVal.Value);
                }
            }
            if (other is LongValue longVal)
            {
                // Use unchecked arithmetic to allow natural overflow/underflow
                unchecked
                {
                    return new LongValue(Value * longVal.Value);
                }
            }
            if (other is FloatValue floatVal)
                return new FloatValue(Value * floatVal.Value);
            
            throw new InvalidOperationException($"Cannot multiply Long by {other.Type}");
        }

        public override K3Value Divide(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new LongValue(Value / intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value / longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value / floatVal.Value);
            
            throw new InvalidOperationException($"Cannot divide Long by {other.Type}");
        }

        public override string ToString()
        {
            if (IsSpecial)
                return SpecialName;
            return Value.ToString() + "L";
        }
    }

    public class FloatValue : K3Value
    {
        public double Value { get; }
        public bool IsSpecial { get; }
        public string SpecialName { get; }

        public FloatValue(double value)
        {
            Value = value;
            Type = ValueType.Float;
            IsSpecial = false;
            
            // Check if this value should be treated as special
            if (double.IsNaN(value))
            {
                IsSpecial = true;
                SpecialName = "0n";
            }
            else if (double.IsPositiveInfinity(value))
            {
                IsSpecial = true;
                SpecialName = "0i";
            }
            else if (double.IsNegativeInfinity(value))
            {
                IsSpecial = true;
                SpecialName = "-0i";
            }
        }

        public FloatValue(string specialName)
        {
            SpecialName = specialName;
            IsSpecial = true;
            Type = ValueType.Float;
            
            // Set the actual double values for special cases
            switch (specialName)
            {
                case "0i": Value = double.PositiveInfinity; break;
                case "0n": Value = double.NaN; break;
                case "-0i": Value = double.NegativeInfinity; break;
                default: throw new ArgumentException($"Unknown special float: {specialName}");
            }
        }

        public override K3Value Add(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new FloatValue(Value + intVal.Value);
            if (other is LongValue longVal)
                return new FloatValue(Value + longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value + floatVal.Value);
            
            throw new InvalidOperationException($"Cannot add Float to {other.Type}");
        }

        public override K3Value Subtract(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new FloatValue(Value - intVal.Value);
            if (other is LongValue longVal)
                return new FloatValue(Value - longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value - floatVal.Value);
            
            throw new InvalidOperationException($"Cannot subtract Float from {other.Type}");
        }

        public override K3Value Multiply(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new FloatValue(Value * intVal.Value);
            if (other is LongValue longVal)
                return new FloatValue(Value * longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value * floatVal.Value);
            
            throw new InvalidOperationException($"Cannot multiply Float by {other.Type}");
        }

        public override K3Value Divide(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new FloatValue(Value / intVal.Value);
            if (other is LongValue longVal)
                return new FloatValue(Value / longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value / floatVal.Value);
            
            throw new InvalidOperationException($"Cannot divide Float by {other.Type}");
        }

        public override string ToString()
        {
            if (IsSpecial)
                return SpecialName;
            
            // Use exponential notation for very large or very small numbers
            var absValue = Math.Abs(Value);
            if (absValue >= 1e15 || (absValue > 0 && absValue < 1e-10))
            {
                var expFormat = $"E{Evaluator.floatPrecision}";
                var formatted = Value.ToString(expFormat);
                // Convert to lowercase 'e'
                formatted = formatted.Replace('E', 'e');
                
                // Handle trailing zeroes in exponential notation
                var eIndex = formatted.IndexOf('e');
                if (eIndex > 0)
                {
                    var mantissa = formatted.Substring(0, eIndex);
                    var exponent = formatted.Substring(eIndex);
                    
                    // Remove trailing zeroes from mantissa
                    if (mantissa.Contains('.'))
                    {
                        mantissa = mantissa.TrimEnd('0');
                        // If decimal portion is zero, remove decimal point
                        if (mantissa.EndsWith('.'))
                        {
                            mantissa = mantissa.TrimEnd('.');
                        }
                    }
                    
                    formatted = mantissa + exponent;
                }
                
                return formatted;
            }
            
            // Use significant digits precision for regular floating point numbers
            if (Value != 0)
            {
                // Use G format with significant digits, then ensure consistent decimal notation
                var precision = Evaluator.floatPrecision;
                var formatted = Math.Round(Value, precision, MidpointRounding.AwayFromZero).ToString("G15");
                
                // Convert to scientific notation if it's too long or has too many decimal places
                if (formatted.Contains('E') || formatted.Length > 15)
                {
                    var expFormat = $"E{precision}";
                    formatted = Value.ToString(expFormat);
                    // Convert to lowercase 'e'
                    formatted = formatted.Replace('E', 'e');
                    
                    // Handle trailing zeroes in exponential notation
                    var eIndex = formatted.IndexOf('e');
                    if (eIndex > 0)
                    {
                        var mantissa = formatted.Substring(0, eIndex);
                        var exponent = formatted.Substring(eIndex);
                        
                        // Remove trailing zeroes from mantissa
                        if (mantissa.Contains('.'))
                        {
                            mantissa = mantissa.TrimEnd('0');
                            // If decimal portion is zero, remove decimal point
                            if (mantissa.EndsWith('.'))
                            {
                                mantissa = mantissa.TrimEnd('.');
                            }
                        }
                        
                        formatted = mantissa + exponent;
                    }
                    
                    return formatted;
                }
                
                // Ensure we have the right number of significant digits
                var significantDigits = CountSignificantDigits(Value);
                if (significantDigits > precision)
                {
                    // Round to the correct number of significant digits
                    var scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(Value))) - precision + 1);
                    var rounded = Math.Round(Value / scale) * scale;
                    formatted = rounded.ToString("G15");
                }
                
                // Handle decimal notation trailing zeroes
                if (formatted.Contains('.'))
                {
                    var decimalIndex = formatted.IndexOf('.');
                    var integerPart = formatted.Substring(0, decimalIndex);
                    var decimalPart = formatted.Substring(decimalIndex + 1);
                    
                    // Remove trailing zeroes from decimal part
                    decimalPart = decimalPart.TrimEnd('0');
                    
                    // If all decimal digits were zeroes, preserve one zero
                    if (decimalPart.Length == 0)
                    {
                        decimalPart = "0";
                    }
                    
                    // Reconstruct
                    formatted = integerPart + "." + decimalPart;
                    
                    // If decimal part is just "0", we might want to keep it for float numbers
                    // But if the original number was a whole number that was converted to float, keep .0
                }
                
                // Ensure decimal notation for display consistency
                if (formatted.Contains('.') || formatted.Contains('e'))
                {
                    return formatted;
                }
                else
                {
                    // Add .0 for whole numbers that were originally floats
                    return formatted + ".0";
                }
            }
            
            return "0.0";
        }
        
        private static int CountSignificantDigits(double value)
        {
            if (value == 0) return 1;
            
            var absValue = Math.Abs(value);
            var log10 = Math.Log10(absValue);
            var integerDigits = (int)Math.Floor(log10) + 1;
            
            // Count digits in string representation, excluding decimal point and leading zeros
            var str = absValue.ToString("G15");
            var count = 0;
            var seenNonZero = false;
            
            foreach (char c in str)
            {
                if (c == '.' || c == 'E' || c == '-') continue;
                if (c != '0') seenNonZero = true;
                if (seenNonZero) count++;
            }
            
            return count;
        }
    }

    public class CharacterValue : K3Value
    {
        public string Value { get; }

        public CharacterValue(string value)
        {
            Value = value;
            Type = ValueType.Character;
        }

        public override K3Value Add(K3Value other)
        {
            throw new InvalidOperationException("Cannot add Character values");
        }

        public override K3Value Subtract(K3Value other)
        {
            throw new InvalidOperationException("Cannot subtract Character values");
        }

        public override K3Value Multiply(K3Value other)
        {
            throw new InvalidOperationException("Cannot multiply Character values");
        }

        public override K3Value Divide(K3Value other)
        {
            throw new InvalidOperationException("Cannot divide Character values");
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }

    public class SymbolValue : K3Value
    {
        public string Value { get; }

        public SymbolValue(string value)
        {
            Value = value;
            Type = ValueType.Symbol;
        }

        public override K3Value Add(K3Value other)
        {
            throw new InvalidOperationException("Cannot add Symbol values");
        }

        public override K3Value Subtract(K3Value other)
        {
            throw new InvalidOperationException("Cannot subtract Symbol values");
        }

        public override K3Value Multiply(K3Value other)
        {
            throw new InvalidOperationException("Cannot multiply Symbol values");
        }

        public override K3Value Divide(K3Value other)
        {
            throw new InvalidOperationException("Cannot divide Symbol values");
        }

        public override string ToString()
        {
            // Check if the symbol contains spaces or special characters
            if (ContainsSpecialCharacters(Value))
            {
                return $"`\"{Value}\"";
            }
            else
            {
                return $"`{Value}";
            }
        }
        
        public string ToStringForFormat()
        {
            // For unary $ formatting, return symbol name in quotes without backtick
            return $"\"{Value}\"";
        }
        
        private bool ContainsSpecialCharacters(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
                
            foreach (char c in value)
            {
                // If character is not alphanumeric, underscore, or period, it's a special character
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '.')
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is SymbolValue other)
            {
                return Value == other.Value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }

    public class VectorValue : K3Value
    {
        public List<K3Value> Elements { get; }
        public string CreationMethod { get; } // Track how the vector was created

        public VectorValue(List<K3Value> elements) : this(elements, "standard")
        {
        }
        
        public VectorValue(List<K3Value> elements, string creationMethod)
        {
            Elements = elements;
            Type = ValueType.Vector;
            CreationMethod = creationMethod;
        }

        public override K3Value Add(K3Value other)
        {
            if (other is VectorValue otherVec)
            {
                if (Elements.Count != otherVec.Elements.Count)
                    throw new InvalidOperationException("Vector size mismatch for addition");
                
                var result = new List<K3Value>();
                for (int i = 0; i < Elements.Count; i++)
                {
                    result.Add(Elements[i].Add(otherVec.Elements[i]));
                }
                return new VectorValue(result);
            }
            
            // Scalar addition
            var scalarResult = new List<K3Value>();
            foreach (var element in Elements)
            {
                scalarResult.Add(element.Add(other));
            }
            return new VectorValue(scalarResult);
        }

        public override K3Value Subtract(K3Value other)
        {
            if (other is VectorValue otherVec)
            {
                if (Elements.Count != otherVec.Elements.Count)
                    throw new InvalidOperationException("Vector size mismatch for subtraction");
                
                var result = new List<K3Value>();
                for (int i = 0; i < Elements.Count; i++)
                {
                    result.Add(Elements[i].Subtract(otherVec.Elements[i]));
                }
                return new VectorValue(result);
            }
            
            // Scalar subtraction
            var scalarResult = new List<K3Value>();
            foreach (var element in Elements)
            {
                scalarResult.Add(element.Subtract(other));
            }
            return new VectorValue(scalarResult);
        }

        public override K3Value Multiply(K3Value other)
        {
            if (other is VectorValue otherVec)
            {
                if (Elements.Count != otherVec.Elements.Count)
                    throw new InvalidOperationException("Vector size mismatch for multiplication");
                
                var result = new List<K3Value>();
                for (int i = 0; i < Elements.Count; i++)
                {
                    result.Add(Elements[i].Multiply(otherVec.Elements[i]));
                }
                return new VectorValue(result);
            }
            
            // Scalar multiplication
            var scalarResult = new List<K3Value>();
            foreach (var element in Elements)
            {
                scalarResult.Add(element.Multiply(other));
            }
            return new VectorValue(scalarResult);
        }

        public override K3Value Divide(K3Value other)
        {
            if (other is VectorValue otherVec)
            {
                if (Elements.Count != otherVec.Elements.Count)
                    throw new InvalidOperationException("Vector size mismatch for division");
                
                // Check if all elements are integers and all divisions are exact
                bool allIntegerDivision = true;
                bool allExactDivision = true;
                
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (Elements[i] is IntegerValue intElem && otherVec.Elements[i] is IntegerValue intDiv)
                    {
                        if (intDiv.Value == 0)
                            throw new InvalidOperationException("Division by zero");
                        if (intElem.Value % intDiv.Value != 0)
                            allExactDivision = false;
                    }
                    else if (Elements[i] is LongValue longElem && otherVec.Elements[i] is LongValue longDiv)
                    {
                        if (longDiv.Value == 0)
                            throw new InvalidOperationException("Division by zero");
                        if (longElem.Value % longDiv.Value != 0)
                            allExactDivision = false;
                    }
                    else
                    {
                        allIntegerDivision = false;
                        break;
                    }
                }
                
                var result = new List<K3Value>();
                for (int i = 0; i < Elements.Count; i++)
                {
                    if (allIntegerDivision && allExactDivision)
                    {
                        // All exact integer division - return integer vector
                        if (Elements[i] is IntegerValue intElem && otherVec.Elements[i] is IntegerValue intDiv)
                            result.Add(new IntegerValue(intElem.Value / intDiv.Value));
                        else if (Elements[i] is LongValue longElem && otherVec.Elements[i] is LongValue longDiv)
                            result.Add(new LongValue(longElem.Value / longDiv.Value));
                    }
                    else if (allIntegerDivision && !allExactDivision)
                    {
                        // Integer division but not all exact - return float vector
                        if (Elements[i] is IntegerValue intElem && otherVec.Elements[i] is IntegerValue intDiv)
                            result.Add(new FloatValue((double)intElem.Value / intDiv.Value));
                        else if (Elements[i] is LongValue longElem && otherVec.Elements[i] is LongValue longDiv)
                            result.Add(new FloatValue((double)longElem.Value / longDiv.Value));
                    }
                    else
                    {
                        // Mixed types - use element-wise division
                        result.Add(Elements[i].Divide(otherVec.Elements[i]));
                    }
                }
                return new VectorValue(result);
            }
            
            // Scalar division with smart division rules
            if (other is IntegerValue intScalar)
            {
                if (intScalar.Value == 0)
                    throw new InvalidOperationException("Division by zero");
                
                // Check if all elements are integers
                bool allIntegerElements = true;
                bool allExactDivision = true;
                
                foreach (var element in Elements)
                {
                    if (element is IntegerValue intElem)
                    {
                        if (intElem.Value % intScalar.Value != 0)
                            allExactDivision = false;
                    }
                    else if (element is LongValue longElem)
                    {
                        if (longElem.Value % intScalar.Value != 0)
                            allExactDivision = false;
                    }
                    else
                    {
                        allIntegerElements = false;
                        break;
                    }
                }
                
                var scalarResult = new List<K3Value>();
                foreach (var element in Elements)
                {
                    if (allIntegerElements && allExactDivision)
                    {
                        // All exact integer division
                        if (element is IntegerValue intElem)
                            scalarResult.Add(new IntegerValue(intElem.Value / intScalar.Value));
                        else if (element is LongValue longElem)
                            scalarResult.Add(new LongValue(longElem.Value / intScalar.Value));
                    }
                    else if (allIntegerElements && !allExactDivision)
                    {
                        // Integer division but not all exact - convert to float
                        if (element is IntegerValue intElem)
                            scalarResult.Add(new FloatValue((double)intElem.Value / intScalar.Value));
                        else if (element is LongValue longElem)
                            scalarResult.Add(new FloatValue((double)longElem.Value / intScalar.Value));
                    }
                    else
                    {
                        // Mixed types - use element-wise division
                        scalarResult.Add(element.Divide(other));
                    }
                }
                return new VectorValue(scalarResult);
            }
            
            // Default scalar division for other types
            var defaultScalarResult = new List<K3Value>();
            foreach (var element in Elements)
            {
                defaultScalarResult.Add(element.Divide(other));
            }
            return new VectorValue(defaultScalarResult);
        }

        public K3Value Minimum(VectorValue other)
        {
            if (Elements.Count != other.Elements.Count)
                throw new InvalidOperationException("Vector size mismatch for minimum");
            
            var result = new List<K3Value>();
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] is IntegerValue intA && other.Elements[i] is IntegerValue intB)
                    result.Add(new IntegerValue(Math.Min(intA.Value, intB.Value)));
                else if (Elements[i] is LongValue longA && other.Elements[i] is LongValue longB)
                    result.Add(new LongValue(Math.Min(longA.Value, longB.Value)));
                else if (Elements[i] is FloatValue floatA && other.Elements[i] is FloatValue floatB)
                    result.Add(new FloatValue(Math.Min(floatA.Value, floatB.Value)));
                else
                    throw new InvalidOperationException("Cannot find minimum of mixed types");
            }
            return new VectorValue(result);
        }

        public K3Value Minimum(K3Value scalar)
        {
            var result = new List<K3Value>();
            foreach (var element in Elements)
            {
                if (element is IntegerValue intA && scalar is IntegerValue intB)
                    result.Add(new IntegerValue(Math.Min(intA.Value, intB.Value)));
                else if (element is LongValue longA && scalar is LongValue longB)
                    result.Add(new LongValue(Math.Min(longA.Value, longB.Value)));
                else if (element is FloatValue floatA && scalar is FloatValue floatB)
                    result.Add(new FloatValue(Math.Min(floatA.Value, floatB.Value)));
                else
                    throw new InvalidOperationException("Cannot find minimum of mixed types");
            }
            return new VectorValue(result);
        }

        public K3Value Maximum(VectorValue other)
        {
            if (Elements.Count != other.Elements.Count)
                throw new InvalidOperationException("Vector size mismatch for maximum");
            
            var result = new List<K3Value>();
            for (int i = 0; i < Elements.Count; i++)
            {
                if (Elements[i] is IntegerValue intA && other.Elements[i] is IntegerValue intB)
                    result.Add(new IntegerValue(Math.Max(intA.Value, intB.Value)));
                else if (Elements[i] is LongValue longA && other.Elements[i] is LongValue longB)
                    result.Add(new LongValue(Math.Max(longA.Value, longB.Value)));
                else if (Elements[i] is FloatValue floatA && other.Elements[i] is FloatValue floatB)
                    result.Add(new FloatValue(Math.Max(floatA.Value, floatB.Value)));
                else
                    throw new InvalidOperationException("Cannot find maximum of mixed types");
            }
            return new VectorValue(result);
        }

        public K3Value Maximum(K3Value scalar)
        {
            var result = new List<K3Value>();
            foreach (var element in Elements)
            {
                if (element is IntegerValue intA && scalar is IntegerValue intB)
                    result.Add(new IntegerValue(Math.Max(intA.Value, intB.Value)));
                else if (element is LongValue longA && scalar is LongValue longB)
                    result.Add(new LongValue(Math.Max(longA.Value, longB.Value)));
                else if (element is FloatValue floatA && scalar is FloatValue floatB)
                    result.Add(new FloatValue(Math.Max(floatA.Value, floatB.Value)));
                else
                    throw new InvalidOperationException("Cannot find maximum of mixed types");
            }
            return new VectorValue(result);
        }

        public override string ToString()
        {
            return ToString(false);
        }
        
        public string ToString(bool asElement)
        {
            return ToString(asElement, false);
        }
        
        public string ToString(bool asElement, bool skipComma)
        {
            if (Elements.Count == 0)
            {
                // Handle special empty vector display formats
                if (CreationMethod == "enumerate_int")
                    return "!0";
                else if (CreationMethod == "enumerate_long")
                    return "!0L";
                else if (CreationMethod == "take_float")
                    return "0#0.0";
                else if (CreationMethod == "take_symbol")
                    return "0#`";
                else if (CreationMethod == "enumerate_char" || (Elements.Count > 0 && Elements.All(e => e is CharacterValue)))
                    return "\"\"";
                else if (CreationMethod == "standard")
                    return "0"; // Empty standard integer vector
                else if (asElement || CreationMethod == "mixed")
                    return "()"; // Empty mixed vector
                else
                    return "()";
            }
            
            // Check if this is a character vector - display as vector of quoted strings
            if (Elements.All(e => e is CharacterValue))
            {
                // For formatted vectors (like from $ operator), return vector of character vectors
                // Only apply this to actual formatting operations, not type conversions
                if (CreationMethod == "formatted")
                {
                    var elementsStr = string.Join(";", Elements.Select(e => e.ToString()));
                    return $"({elementsStr})";
                }
                
                // For simple character vectors, display as quoted string
                var chars = Elements.Select(e => ((CharacterValue)e).Value);
                var result = $"\"{string.Concat(chars)}\"";
                
                // Add enlist comma for single-element character vectors (unless skipped or string representation)
                if (Elements.Count == 1 && !skipComma && CreationMethod != "string_representation")
                {
                    result = "," + result;
                }
                
                return result;
            }
            
            // Check if this is a symbol vector - display in compact format without spaces
            if (Elements.All(e => e is SymbolValue))
            {
                var symbols = Elements.Select(e => ((SymbolValue)e).ToString());
                return string.Concat(symbols);
            }
            
            // Check if this is a mixed vector - keep parentheses and semicolons for clarity
            var elementTypes = Elements.Select(e => e.GetType()).Distinct().ToList();
            var hasNestedVectors = Elements.Any(e => e is VectorValue);
            var hasNullValues = Elements.Any(e => e is NullValue);
            
            // Check if this is truly mixed (different types) OR has nested vectors OR has null values
            // For homogeneous vectors with nulls, we should display as mixed with semicolons
            var isTrulyMixed = elementTypes.Count > 1 || hasNestedVectors || hasNullValues;
            
            if (isTrulyMixed)
            {
                var elementsStr = string.Join(";", Elements.Select(e => 
                {
                    if (e is NullValue)
                    {
                        return ""; // Display null as empty position
                    }
                    else if (e is VectorValue vec)
                    {
                        // Special handling for single-element vectors
                        if (vec.Elements.Count == 1)
                        {
                            return "," + vec.ToString(false, true); // Add comma prefix, but skip inner comma logic
                        }
                        // For simple homogeneous vectors (like integer vectors), don't add inner parentheses
                        if (vec.Elements.All(x => x is IntegerValue) || 
                            vec.Elements.All(x => x is FloatValue) ||
                            vec.Elements.All(x => x is LongValue))
                        {
                            return vec.ToString(false); // Don't add parentheses for simple vectors
                        }
                        return vec.ToString(true); // Add parentheses for complex vectors
                    }
                    return e.ToString();
                }));
                return "(" + elementsStr + ")";
            }
            
            // For homogeneous vectors (except characters), use space-separated format
            var vectorStr = string.Join(" ", Elements.Select(e => e.ToString()));
            
            // Add enlist comma for any single-element vector (unless skipped)
            if (Elements.Count == 1 && !skipComma)
            {
                return "," + vectorStr;
            }
            
            // If this vector is an element of another vector, wrap it in parentheses
            if (asElement)
            {
                return "(" + vectorStr + ")";
            }
            
            return vectorStr;
        }
    }

    public class FunctionValue : K3Value
    {
        public string BodyText { get; }
        public List<string> Parameters { get; }
        public int Valence { get; }
        public List<Token> PreParsedTokens { get; }
        
        // AST cache for performance optimization
        private ASTNode _cachedAst;
        private readonly object _astCacheLock = new object();

        public FunctionValue(string bodyText, List<string> parameters, List<Token> preParsedTokens = null)
        {
            BodyText = bodyText;
            Parameters = parameters;
            Type = ValueType.Function;
            Valence = parameters.Count;
            PreParsedTokens = preParsedTokens;
        }
        
        // Get or create cached AST (thread-safe)
        public ASTNode GetCachedAst()
        {
            if (_cachedAst != null)
            {
                return _cachedAst;
            }
            
            lock (_astCacheLock)
            {
                // Double-check pattern for thread safety
                if (_cachedAst != null)
                {
                    return _cachedAst;
                }
                
                // Parse and cache the AST
                ASTNode ast;
                if (PreParsedTokens != null && PreParsedTokens.Count > 0)
                {
                    var parser = new Parser(PreParsedTokens, BodyText);
                    ast = parser.Parse();
                }
                else
                {
                    var lexer = new Lexer(BodyText);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens, BodyText);
                    ast = parser.Parse();
                }
                
                _cachedAst = ast;
                return _cachedAst;
            }
        }

        public override K3Value Add(K3Value other)
        {
            throw new InvalidOperationException("Cannot add Function values");
        }

        public override K3Value Subtract(K3Value other)
        {
            throw new InvalidOperationException("Cannot subtract Function values");
        }

        public override K3Value Multiply(K3Value other)
        {
            throw new InvalidOperationException("Cannot multiply Function values");
        }

        public override K3Value Divide(K3Value other)
        {
            throw new InvalidOperationException("Cannot divide Function values");
        }

        public override string ToString()
        {
            // Generate representative textual representation
            var paramsStr = Parameters.Count > 0 ? "[" + string.Join(";", Parameters) + "] " : "";
            return "{" + paramsStr + BodyText + "}";
        }
    }

    public class NullValue : K3Value
    {
        public NullValue()
        {
            Type = ValueType.Null;
        }

        public override K3Value Add(K3Value other)
        {
            throw new InvalidOperationException("Cannot add Null values");
        }

        public override K3Value Subtract(K3Value other)
        {
            throw new InvalidOperationException("Cannot subtract Null values");
        }

        public override K3Value Multiply(K3Value other)
        {
            throw new InvalidOperationException("Cannot multiply Null values");
        }

        public override K3Value Divide(K3Value other)
        {
            throw new InvalidOperationException("Cannot divide Null values");
        }

        public override string ToString()
        {
            return "_n";
        }
    }

    public class DictionaryValue : K3Value
    {
        public Dictionary<SymbolValue, (K3Value Value, DictionaryValue Attribute)> Entries { get; }

        public DictionaryValue()
        {
            Type = ValueType.Dictionary;
            Entries = new Dictionary<SymbolValue, (K3Value, DictionaryValue)>();
        }

        public DictionaryValue(Dictionary<SymbolValue, (K3Value, DictionaryValue)> entries)
        {
            Type = ValueType.Dictionary;
            Entries = entries;
        }

        public override K3Value Add(K3Value other)
        {
            throw new InvalidOperationException("Cannot add Dictionary values");
        }

        public override K3Value Subtract(K3Value other)
        {
            throw new InvalidOperationException("Cannot subtract Dictionary values");
        }

        public override K3Value Multiply(K3Value other)
        {
            throw new InvalidOperationException("Cannot multiply Dictionary values");
        }

        public override K3Value Divide(K3Value other)
        {
            throw new InvalidOperationException("Cannot divide Dictionary values");
        }

        public override string ToString()
        {
            if (Entries.Count == 0)
                return ".()";
            
            var entries = new List<string>();
            foreach (var kvp in Entries)
            {
                var key = kvp.Key.ToString();
                var value = kvp.Value.Value;
                var attr = kvp.Value.Attribute;
                
                var valueStr = value is NullValue ? "" : value.ToString();
                
                if (attr != null && attr.Entries.Count > 0)
                {
                    entries.Add($"({key};{valueStr};{attr})");
                }
                else
                {
                    // Always show the semicolon for attributes, even when null
                    entries.Add($"({key};{valueStr};)");
                }
            }
            
            // Handle single-element case with comma prefix (per specification)
            if (entries.Count == 1)
            {
                // Remove the outer parentheses from the single entry to avoid double parentheses
                var singleEntry = entries[0];
                if (singleEntry.StartsWith("(") && singleEntry.EndsWith(")"))
                {
                    singleEntry = singleEntry.Substring(1, singleEntry.Length - 2);
                }
                return ".,(" + singleEntry + ")";
            }
            
            return ".(" + string.Join(";", entries) + ")";
        }
    }
}
