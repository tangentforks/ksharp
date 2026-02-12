using System;
using System.Collections.Generic;
using System.Linq;
using K3CSharp;

namespace K3CSharp.Serialization
{
    /// <summary>
    /// K List type for serialization support
    /// </summary>
    public class KList : K3Value
    {
        public List<object> Elements { get; set; } = new List<object>();
        
        public KList() 
        { 
            Type = ValueType.List;
        }
        
        public KList(params object[] elements)
        {
            Elements = elements.ToList();
            Type = ValueType.List;
        }
        
        public KList(IEnumerable<object> elements)
        {
            Elements = elements.ToList();
            Type = ValueType.List;
        }
        
        public override string ToString()
        {
            if (Elements.Count == 0) return "()";
            return "(" + string.Join(";", Elements.Select(e => e?.ToString() ?? "()")) + ")";
        }
        
        public override K3Value Add(K3Value other)
        {
            throw new NotSupportedException("List does not support Add operation");
        }
        
        public override K3Value Subtract(K3Value other)
        {
            throw new NotSupportedException("List does not support Subtract operation");
        }
        
        public override K3Value Multiply(K3Value other)
        {
            throw new NotSupportedException("List does not support Multiply operation");
        }
        
        public override K3Value Divide(K3Value other)
        {
            throw new NotSupportedException("List does not support Divide operation");
        }
    }
    
    /// <summary>
    /// K Dictionary type for serialization support
    /// </summary>
    public class KDictionary
    {
        public List<KeyValuePair<object, object>> Pairs { get; set; } = new List<KeyValuePair<object, object>>();
        
        public KDictionary() { }
        
        public KDictionary(params KeyValuePair<object, object>[] pairs)
        {
            Pairs = pairs.ToList();
        }
        
        public KDictionary(Dictionary<object, object> dict)
        {
            Pairs = dict.Select(kvp => kvp).ToList();
        }
        
        public void Add(object key, object value)
        {
            Pairs.Add(new KeyValuePair<object, object>(key, value));
        }
    }
    
    /// <summary>
    /// K Anonymous Function type for serialization support
    /// </summary>
    public class KFunction
    {
        public string Source { get; set; } = "";
        public bool HasParseErrors { get; set; } = false;
        
        public KFunction() { }
        
        public KFunction(string source, bool hasParseErrors = false)
        {
            Source = source;
            HasParseErrors = hasParseErrors;
        }
    }
    
    /// <summary>
    /// K Vector type for serialization support
    /// </summary>
    public class KVector
    {
        public KVectorType Type { get; set; }
        public int[] IntegerElements { get; set; } = Array.Empty<int>();
        public double[] FloatElements { get; set; } = Array.Empty<double>();
        public char[] CharacterElements { get; set; } = Array.Empty<char>();
        public string[] SymbolElements { get; set; } = Array.Empty<string>();
        
        public KVector() { }
        
        public KVector(int[] elements)
        {
            Type = KVectorType.Integer;
            IntegerElements = elements;
        }
        
        public KVector(double[] elements)
        {
            Type = KVectorType.Float;
            FloatElements = elements;
        }
        
        public KVector(char[] elements)
        {
            Type = KVectorType.Character;
            CharacterElements = elements;
        }
        
        public KVector(string[] elements)
        {
            Type = KVectorType.Symbol;
            SymbolElements = elements;
        }
    }
    
    public enum KVectorType
    {
        Integer,
        Float,
        Character,
        Symbol
    }
}
