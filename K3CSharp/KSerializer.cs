using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K3CSharp
{
    /// <summary>
    /// Main K serializer implementing unified serialization pattern
    /// </summary>
    public class KSerializer
    {
        public byte[] Serialize(object value)
        {
            return value switch
            {
                int i => SerializeInteger(i),
                double d => SerializeFloat(d),
                char c => SerializeCharacter(c),
                string s when s.StartsWith("`") => SerializeSymbol(s),
                string s when s.Length == 1 => SerializeCharacter(s[0]),
                string s => SerializeCharacterVector(s),
                null => SerializeNull(),
                K3CSharp.VectorValue list => SerializeList(list), // Use VectorValue for mixed lists
                K3CSharp.DictionaryValue dict => SerializeDictionary(dict), // Use DictionaryValue
                K3CSharp.FunctionValue func => SerializeAnonymousFunction(func), // Use FunctionValue directly
                _ => throw new NotSupportedException($"Unsupported type: {value.GetType()}")
            };
        }
        
        private byte[] SerializeMessage(int length)
        {
            var writer = new KBinaryWriter();
            writer.WriteByte(1);   // Architecture: little-endian
            writer.WriteByte(0);   // Message type: _bd serialization
            writer.WriteInt16(0);  // Reserved: 2 bytes
            writer.WriteInt32(length); // Data length
            return writer.ToArray();
        }
        
        private byte[] SerializeIntegerData(int value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(1);  // Integer flag
            writer.WriteInt32(value); // Integer value
            return writer.GetBuffer();
        }
        
        private byte[] SerializeInteger(int value)
        {
            var data = SerializeIntegerData(value);
            var message = SerializeMessage(data.Length);
            return message.Concat(data).ToArray();
        }
        
        private byte[] SerializeFloatData(double value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(2);  // Float flag
            writer.WriteInt32(1);  // Subtype field (required for floats in k.exe)
            writer.WriteDouble(value); // Float value
            return writer.GetBuffer();
        }
        
        private byte[] SerializeFloat(double value)
        {
            var data = SerializeFloatData(value);
            var message = SerializeMessage(data.Length);
            return message.Concat(data).ToArray();
        }
        
        private byte[] SerializeCharacterData(char value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(3);  // Character flag
            writer.WriteByte((byte)value); // Character value
            writer.WritePadding(3); // 3 bytes padding to align to 4-byte boundary
            return writer.GetBuffer();
        }
        
        private byte[] SerializeCharacter(char value)
        {
            var data = SerializeCharacterData(value);
            var message = SerializeMessage(data.Length);
            return message.Concat(data).ToArray();
        }
        
        private byte[] SerializeSymbolData(string symbol)
        {
            var writer = new KBinaryWriter();
            // Remove initial backtick for serialization (k.exe stores symbol value without backtick)
            var symbolValue = symbol.StartsWith("`") ? symbol.Substring(1) : symbol;
            var symbolData = Encoding.UTF8.GetBytes(symbolValue);
            writer.WriteInt32(4);  // Symbol flag
            writer.WriteBytes(symbolData); // Symbol data
            writer.WriteByte(0); // Null terminator
            return writer.GetBuffer();
        }
        
        private byte[] SerializeSymbol(string symbol)
        {
            var data = SerializeSymbolData(symbol);
            var message = SerializeMessage(data.Length);
            return message.Concat(data).ToArray();
        }
        
        public byte[] SerializeCharacterVector(string str)
        {
            var chars = str.ToCharArray();
            var writer = new KBinaryWriter();
            
            writer.WriteByte(1);   // Architecture: little-endian
            writer.WriteByte(0);   // Message type: _bd serialization
            writer.WriteInt16(0);  // Reserved: 2 bytes
            writer.WriteInt32(8 + chars.Length + 1); // Length: flag(4) + count(4) + data + null(1)
            writer.WriteInt32(-3); // CharacterVector flag
            writer.WriteInt32(chars.Length); // Element count
            writer.WriteBytes(chars.Select(c => (byte)c).ToArray()); // Character data
            writer.WriteByte(0); // Null terminator
            return writer.ToArray();
        }
        
        private byte[] SerializeNullData()
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(6);  // Null subtype
            writer.WritePadding(4); // 4 bytes padding to match k.exe
            return writer.ToArray();
        }
        
        private byte[] SerializeNull()
        {
            var nullData = SerializeNullData();
            var message = SerializeMessage(nullData.Length);
            return message.Concat(nullData).ToArray();
        }
        
        private byte[] SerializeList(K3CSharp.VectorValue list)
        {
            var listData = SerializeListData(list);
            var message = SerializeMessage(listData.Length);
            return message.Concat(listData).ToArray();
        }
        
        private byte[] SerializeListData(K3CSharp.VectorValue list)
        {
            var elementData = new List<byte>();
            
            foreach (var element in list.Elements)
            {
                var serialized = SerializeElementData(element);
                elementData.AddRange(serialized);
            }
            
            var writer = new KBinaryWriter();
            writer.WriteInt32(0);  // List type: 0 for mixed lists
            writer.WriteInt32(list.Elements.Count); // Element count
            writer.WriteBytes(elementData.ToArray()); // Element data
            return writer.ToArray();
        }
        
        private byte[] SerializeElementData(object element)
        {
            return element switch
            {
                // Handle nested lists (for dictionary triplets)
                K3CSharp.VectorValue nestedList => SerializeListData(nestedList),
                
                // Handle complex types that can be list elements - return only data, not full message
                K3CSharp.DictionaryValue dict => SerializeDictionaryData(dict),
                K3CSharp.FunctionValue func => SerializeAnonymousFunctionData(func),
                
                // Handle primitive K3Value objects
                K3CSharp.IntegerValue iv => SerializeIntegerData(iv.Value),
                K3CSharp.FloatValue fv => SerializeFloatData(fv.Value),
                K3CSharp.CharacterValue cv => SerializeCharacterData(cv.Value[0]),
                K3CSharp.SymbolValue sv => SerializeSymbolData(sv.Value),
                K3CSharp.NullValue => SerializeNullData(),
                
                _ => throw new NotSupportedException($"Unsupported element type: {element.GetType()}")
            };
        }
        
        private byte[] SerializeDictionary(K3CSharp.DictionaryValue dict)
        {
            var dictData = SerializeDictionaryData(dict);
            var message = SerializeMessage(dictData.Length);
            return message.Concat(dictData).ToArray();
        }
        
        private byte[] SerializeDictionaryData(K3CSharp.DictionaryValue dict)
        {
            // Convert dictionary to list of triplets using unmake functionality
            var triplets = new List<K3CSharp.K3Value>();
            
            foreach (var kvp in dict.Entries)
            {
                var key = kvp.Key;
                var (value, attr) = kvp.Value;
                
                // Create triplet as VectorValue (mixed list) with 3 elements: (key; value; null)
                var tripletElements = new List<K3CSharp.K3Value> { key, value, new K3CSharp.NullValue() };
                var tripletList = new K3CSharp.VectorValue(tripletElements);
                
                // Add triplet as element to main list
                triplets.Add(tripletList);
            }
            
            // Create list from triplets (type 0 list)
            var tripletListContainer = new K3CSharp.VectorValue(triplets);
            
            // Serialize as list data first
            var listData = SerializeListData(tripletListContainer);
            
            // Change type from 0 (list) to 5 (dictionary)
            if (listData.Length >= 4)
            {
                listData[0] = 5; // Dictionary flag instead of list flag
            }
            
            return listData;
        }
        
        private byte[] SerializeAnonymousFunction(K3CSharp.FunctionValue func)
        {
            var functionData = SerializeAnonymousFunctionData(func);
            var message = SerializeMessage(functionData.Length);
            return message.Concat(functionData).ToArray();
        }
        
        private byte[] SerializeAnonymousFunctionData(K3CSharp.FunctionValue func)
        {
            var writer = new KBinaryWriter();
            // Use the original source text if available, otherwise fall back to reconstructed text
            var functionSourceText = !string.IsNullOrEmpty(func.OriginalSourceText) 
                ? func.OriginalSourceText 
                : "{" + "[" + string.Join(";", func.Parameters) + "]" + func.BodyText + "}";
            
            // Check for undefined variables (k.exe adds .k metadata when there are parsing errors)
            var hasUndefinedVariables = HasUndefinedVariables(func);
            
            var functionSource = Encoding.UTF8.GetBytes(functionSourceText);
            
            writer.WriteInt32(10); // Function flag
            
            // Add .k metadata if there are undefined variables (like k.exe does)
            if (hasUndefinedVariables)
            {
                // k.exe writes exactly 3 bytes: ".k\0"
                var metadata = Encoding.ASCII.GetBytes(".k\0");
                writer.WriteBytes(metadata);
            }
            else
            {
                writer.WriteByte(0); // Extra null byte only when no metadata
            }
            
            writer.WriteBytes(functionSource); // Function source
            writer.WriteByte(0); // Null terminator
            
            return writer.ToArray();
        }
        
        private bool HasUndefinedVariables(K3CSharp.FunctionValue func)
        {
            if (func.PreParsedTokens == null || func.PreParsedTokens.Count == 0)
                return false;
                
            var parameterSet = new HashSet<string>(func.Parameters, StringComparer.Ordinal);
            
            foreach (var token in func.PreParsedTokens)
            {
                // Check for identifiers that are not parameters and not keywords
                if (token.Type == TokenType.IDENTIFIER && 
                    !parameterSet.Contains(token.Lexeme) &&
                    !IsKKeyword(token.Lexeme))
                {
                    return true; // Found undefined variable
                }
            }
            
            return false;
        }
        
        private bool IsKKeyword(string identifier)
        {
            // K keywords that should not be considered undefined variables
            var keywords = new HashSet<string>
            {
                "if", "do", "while", "for", "select", "exec", "exit", "return",
                "_in", "_sv", "_vs", "_ssr", "_bd", "_db", "_getenv", "_setenv",
                "_host", "_size", "_exit", "_d", "_cd", "_env", "_pid", "_uid",
                "_gid", "_time", "_date", "_year", "_month", "_day", "_hour",
                "_minute", "_second", "_millis", "_unix", "_gmt", "_local",
                "_abs", "_neg", "_sqrt", "_log", "_exp", "_sin", "_cos", "_tan",
                "_asin", "_acos", "_atan", "_floor", "_ceil", "_round", "_sign",
                "_recip", "_not", "_null", "_type", "_val", "_fill", "_rand",
                "_first", "_last", "_count", "_sum", "_avg", "_min", "_max",
                "_dev", "_var", "_med", "_mode", "_rank", "_unique", "_distinct",
                "_group", "_by", "_over", "_scan", "_each", "_prior", "_enlist",
                "_flatten", "_reverse", "_sort", "_grade", "_iasc", "_idesc",
                "_cross", "_join", "_union", "_intersect", "_except", "_in",
                "_like", "_match", "_split", "_drop", "_take", "_cut", "_vs",
                "_sv", "_ss", "_si", "_sc", "_sf", "_sl", "_sk", "_sp", "_sw",
                "_st", "_sd", "_sm", "_sn", "_sq", "_sr", "_sx", "_sy", "_sz"
            };
            
            return keywords.Contains(identifier);
        }
        
        private byte[] SerializeValue(object value)
        {
            return value switch
            {
                // Primitive types
                int i => BitConverter.GetBytes(i), // Compact 4-byte integer
                double d => BitConverter.GetBytes(d), // Compact 8-byte double
                char c => new byte[] { (byte)c }, // Compact 1-byte char
                string s when s.StartsWith("`") => Encoding.UTF8.GetBytes(s.Substring(1)), // Compact symbol without backtick
                string s => Encoding.UTF8.GetBytes(s), // Compact string
                null => new byte[0], // Compact null (empty)
                
                // K3Value objects
                K3CSharp.IntegerValue iv => BitConverter.GetBytes(iv.Value),
                K3CSharp.FloatValue fv => BitConverter.GetBytes(fv.Value),
                K3CSharp.CharacterValue cv => SerializeCompactCharacter(cv.Value),
                K3CSharp.SymbolValue sv => Encoding.UTF8.GetBytes(sv.Value),
                K3CSharp.NullValue => new byte[0],
                
                // Complex types
                K3CSharp.VectorValue list => SerializeList(list),
                K3CSharp.DictionaryValue dict => SerializeDictionary(dict),
                K3CSharp.FunctionValue func => SerializeAnonymousFunction(func),
                
                _ => throw new NotSupportedException($"Unsupported value type: {value.GetType()}")
            };
        }
        
        private byte[] SerializeCompactCharacter(string charValue)
        {
            // Compact character format used in vectors: flag(4) + char(1) + padding(3)
            var bytes = new byte[7];
            BitConverter.GetBytes(3).CopyTo(bytes, 0); // Character flag (3)
            bytes[4] = (byte)charValue[0]; // Character value
            // bytes[5], bytes[6] remain 0 (padding)
            return bytes;
        }
        
        private byte[] SerializeVectorData(K3CSharp.VectorValue vector)
        {
            var elementData = new List<byte>();
            
            // For VectorValue (mixed lists), we serialize as list type 0
            elementData.AddRange(BitConverter.GetBytes(0)); // List type: 0 for mixed lists
            elementData.AddRange(BitConverter.GetBytes(vector.Elements.Count)); // Element count
            
            foreach (var element in vector.Elements)
            {
                var serialized = SerializeElementData(element);
                elementData.AddRange(serialized);
            }
            
            return elementData.ToArray();
        }
    }
}
