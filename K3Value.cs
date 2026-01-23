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
        Null
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
            IsSpecial = false;
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
                return new IntegerValue(Value + intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value + longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value + floatVal.Value);
            
            throw new InvalidOperationException($"Cannot add Integer to {other.Type}");
        }

        public override K3Value Subtract(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new IntegerValue(Value - intVal.Value);
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
            IsSpecial = false;
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
                return new LongValue(Value + intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value + longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value + floatVal.Value);
            
            throw new InvalidOperationException($"Cannot add Long to {other.Type}");
        }

        public override K3Value Subtract(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new LongValue(Value - intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value - longVal.Value);
            if (other is FloatValue floatVal)
                return new FloatValue(Value - floatVal.Value);
            
            throw new InvalidOperationException($"Cannot subtract Long from {other.Type}");
        }

        public override K3Value Multiply(K3Value other)
        {
            if (other is IntegerValue intVal)
                return new LongValue(Value * intVal.Value);
            if (other is LongValue longVal)
                return new LongValue(Value * longVal.Value);
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
            
            // Use precision only if the value has many decimal places
            var str = Value.ToString();
            if (str.Contains('.') && str.Length > 10)
            {
                return Value.ToString($"F{Evaluator.floatPrecision}");
            }
            return str;
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
            return $"`{Value}";
        }
    }

    public class VectorValue : K3Value
    {
        public List<K3Value> Elements { get; }

        public VectorValue(List<K3Value> elements)
        {
            Elements = elements;
            Type = ValueType.Vector;
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
                
                var result = new List<K3Value>();
                for (int i = 0; i < Elements.Count; i++)
                {
                    result.Add(Elements[i].Divide(otherVec.Elements[i]));
                }
                return new VectorValue(result);
            }
            
            // Scalar division
            var scalarResult = new List<K3Value>();
            foreach (var element in Elements)
            {
                scalarResult.Add(element.Divide(other));
            }
            return new VectorValue(scalarResult);
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
            if (Elements.Count == 0)
                return "()";
            
            return "(" + string.Join(";", Elements.Select(e => e.ToString())) + ")";
        }
    }

    public class FunctionValue : K3Value
    {
        public string BodyText { get; }
        public List<string> Parameters { get; }
        public int Valence { get; }
        public List<Token> PreParsedTokens { get; }

        public FunctionValue(string bodyText, List<string> parameters, List<Token> preParsedTokens = null)
        {
            BodyText = bodyText;
            Parameters = parameters;
            Type = ValueType.Function;
            Valence = parameters.Count;
            PreParsedTokens = preParsedTokens;
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
}
