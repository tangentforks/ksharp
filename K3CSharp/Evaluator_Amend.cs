using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        private K3Value AmendFunction(List<K3Value> arguments)
        {
            // Amend operation: .[d; i; f; y] or .[d; i; f]
            // d: data structure to amend (list, dictionary, or atom)
            // i: indices to amend
            // f: function to apply
            // y: optional new value
            if (arguments.Count < 3)
            {
                throw new Exception("Amend operation requires at least 3 arguments: data, indices, function");
            }
            
            var data = arguments[0];
            var indices = arguments[1];
            var function = arguments[2];
            var value = arguments.Count > 3 ? arguments[3] : null;
            
            // Handle different data types
            if (data is VectorValue list)
            {
                return AmendList(list, indices, function, value ?? new NullValue());
            }
            else if (data is DictionaryValue dict)
            {
                return AmendDictionary(dict, indices, function, value ?? new NullValue());
            }
            else if (data is CharacterValue || data is IntegerValue || data is FloatValue)
            {
                // Handle atomic data
                if (!(indices is NullValue || (indices is VectorValue idxVec && idxVec.Elements.Count == 0)))
                {
                    throw new Exception("For atomic data, indices must be empty list");
                }
                return AmendAtom(data, function, value ?? new NullValue());
            }
            else
            {
                throw new Exception("Amend operation not supported for this data type");
            }
        }
        
        private K3Value AmendItemFunction(List<K3Value> arguments)
        {
            // Amend Item operation: @[d; i; f; y] or @[d; i; f]
            // Equivalent to .[d; ,i; f; y] - note the enlist (,) applied to indices
            // d: data structure to amend (list, dictionary, or atom)
            // i: indices or paths to amend (will be enlisted)
            // f: function to apply (monadic or dyadic)
            // y: optional value for dyadic function
            
            if (arguments.Count < 3)
            {
                throw new Exception("Amend Item operation requires at least 3 arguments: data, indices, function");
            }
            
            var data = arguments[0];
            var indices = arguments[1];
            var function = arguments[2];
            var value = arguments.Count > 3 ? arguments[3] : null;
            
            // Apply enlist to indices (equivalent to ,i)
            var enlistedIndices = Enlist(indices);
            
            // Handle different data types
            if (data is VectorValue list)
            {
                return AmendList(list, enlistedIndices, function, value ?? new NullValue());
            }
            else if (data is DictionaryValue dict)
            {
                return AmendDictionary(dict, enlistedIndices, function, value ?? new NullValue());
            }
            else if (data is CharacterValue || data is IntegerValue || data is FloatValue)
            {
                // Handle atomic data
                if (!(enlistedIndices is NullValue || (enlistedIndices is VectorValue idxVec && idxVec.Elements.Count == 0)))
                {
                    throw new Exception("For atomic data, indices must be empty list");
                }
                return AmendAtom(data, function, value ?? new NullValue());
            }
            else
            {
                throw new Exception("Amend Item operation not supported for this data type");
            }
        }
        
        private K3Value AmendList(VectorValue list, K3Value indices, K3Value function, K3Value value)
        {
            // Create a copy of the list to modify
            var result = new List<K3Value>(list.Elements);
            
            if (indices is VectorValue indexVec)
            {
                for (int i = 0; i < indexVec.Elements.Count; i++)
                {
                    var index = indexVec.Elements[i];
                    if (index is IntegerValue intIndex)
                    {
                        int idx = (int)intIndex.Value;
                        if (idx < 0 || idx >= result.Count)
                        {
                            throw new Exception($"Index {idx} out of bounds for list of length {result.Count}");
                        }
                        
                        // Apply function to current value
                        var currentValue = result[idx];
                        var newValue = ApplyAmendFunction(currentValue, function, value);
                        result[idx] = newValue;
                    }
                    else
                    {
                        throw new Exception("List indices must be integers");
                    }
                }
            }
            else if (indices is NullValue)
            {
                // Amend all items in the list
                for (int i = 0; i < result.Count; i++)
                {
                    var currentValue = result[i];
                    var newValue = ApplyAmendFunction(currentValue, function, value);
                    result[i] = newValue;
                }
            }
            else
            {
                throw new Exception("Indices must be a vector or null");
            }
            
            return new VectorValue(result);
        }
        
        private K3Value AmendDictionary(DictionaryValue dict, K3Value indices, K3Value function, K3Value value)
        {
            // Create a copy of the dictionary to modify
            var result = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue?)>(dict.Entries);
            
            if (indices is SymbolValue symbol)
            {
                // Single key amendment
                if (result.TryGetValue(symbol, out var current))
                {
                    var currentValue = current.Value;
                    var newValue = ApplyAmendFunction(currentValue, function, value);
                    result[symbol] = (newValue, current.Item2);
                }
                else
                {
                    throw new Exception($"Key '{symbol.Value}' not found in dictionary");
                }
            }
            else if (indices is VectorValue indexVec)
            {
                // Multiple key amendments
                for (int i = 0; i < indexVec.Elements.Count; i++)
                {
                    var index = indexVec.Elements[i];
                    if (index is SymbolValue keySymbol)
                    {
                        if (result.TryGetValue(keySymbol, out var current))
                        {
                            var currentValue = current.Value;
                            var newValue = ApplyAmendFunction(currentValue, function, value);
                            result[keySymbol] = (newValue, current.Item2);
                        }
                        else
                        {
                            throw new Exception($"Key '{keySymbol.Value}' not found in dictionary");
                        }
                    }
                    else
                    {
                        throw new Exception("Dictionary indices must be symbols");
                    }
                }
            }
            else
            {
                throw new Exception("Dictionary indices must be symbols or vector of symbols");
            }
            
            return new DictionaryValue(result);
        }
        
        private K3Value AmendAtom(K3Value atom, K3Value function, K3Value value)
        {
            // For atoms, just apply the function directly
            return ApplyAmendFunction(atom, function, value);
        }
        
        private K3Value ApplyAmendFunction(K3Value currentValue, K3Value function, K3Value value)
        {
            // Apply function to current value
            if (value != null)
            {
                // Dyadic function: f[currentValue; value]
                var arguments = new List<K3Value> { currentValue, value };
                return CallFunction(function, arguments);
            }
            else
            {
                // Monadic function: f[currentValue]
                var arguments = new List<K3Value> { currentValue };
                return CallFunction(function, arguments);
            }
        }
        
        private K3Value CallFunction(K3Value function, List<K3Value> arguments)
        {
            if (function is FunctionValue func)
            {
                var tempFunctionNode = new ASTNode(ASTNodeType.Function);
                tempFunctionNode.Value = func;
                return CallDirectFunction(tempFunctionNode, arguments);
            }
            else if (function is SymbolValue symbol)
            {
                return CallVariableFunction(symbol.Value, arguments);
            }
            else
            {
                throw new Exception("Function must be a FunctionValue or SymbolValue");
            }
        }
    }
}
