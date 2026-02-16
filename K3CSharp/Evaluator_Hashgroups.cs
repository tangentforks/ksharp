using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
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
            
            return a; // For scalars, return value itself
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
            
            return a; // For scalars, return value itself
        }

        // Binary versions for operators that can be both unary and binary
        private K3Value UniqueBinary(K3Value a, K3Value b)
        {
            return Unique(a);
        }
    }
}
