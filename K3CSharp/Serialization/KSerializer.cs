using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K3CSharp.Serialization
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
                KList list => SerializeList(list),
                KDictionary dict => SerializeDictionary(dict),
                KFunction func => SerializeAnonymousFunction(func),
                K3CSharp.FunctionValue fv => SerializeAnonymousFunction(ToKFunction(fv)),
                KVector vec => SerializeVector(vec),
                _ => throw new NotSupportedException($"Unsupported type: {value.GetType()}")
            };
        }
        
        private byte[] SerializeMessage(int length)
        {
            var writer = new KBinaryWriter();
            writer.WriteByte(1);   // Architecture: little-endian
            writer.WriteByte(0);   // Message type: _bd serialization
            writer.WriteInt16(0);  // Reserved: 2 bytes
            writer.WriteInt32(length); // Length
            return writer.GetBuffer();
        }
        
        private byte[] SerializeIntegerData(int value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(1);   // Subtype
            writer.WriteInt32(value); // Value (little-endian)
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
            writer.WriteInt32(2);  // Subtype
            writer.WriteByte(1);   // Padding byte value 1
            writer.WritePadding(3); // Additional padding
            writer.WriteDouble(value); // IEEE 754 little-endian
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
            writer.WriteInt32(3);  // Character flag (3, not -1)
            writer.WriteByte((byte)value); // Character value
            writer.WritePadding(3); // Padding
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
            var symbolData = Encoding.UTF8.GetBytes(symbol.StartsWith("`") ? symbol[1..] : symbol);
            
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
            writer.WriteInt32(0);  // Length 0
            writer.WritePadding(3); // Padding
            writer.WriteByte(0);   // Extra null byte to match k.exe
            return writer.ToArray();
        }
        
        private byte[] SerializeNull()
        {
            var nullData = SerializeNullData();
            var message = SerializeMessage(nullData.Length);
            return message.Concat(nullData).ToArray();
        }
        
        private byte[] SerializeList(KList list)
        {
            var listData = SerializeListData(list);
            var message = SerializeMessage(listData.Length);
            return message.Concat(listData).ToArray();
        }
        
        private byte[] SerializeListData(KList list)
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
                // Handle KVector types (from nested list serialization) - return only data, not full message
                K3CSharp.VectorValue vv => SerializeVectorData(ToKVector(vv)),
                KVector vec => SerializeVectorData(vec),
                
                // Handle complex types that can be list elements - return only data, not full message
                KDictionary dict => SerializeDictionaryData(dict),
                KFunction func => SerializeAnonymousFunctionData(func),
                K3CSharp.FunctionValue fv => SerializeAnonymousFunctionData(ToKFunction(fv)),
                
                // Handle primitive K3Value objects
                K3CSharp.IntegerValue iv => SerializeIntegerData(iv.Value),
                K3CSharp.FloatValue fv => SerializeFloatData(fv.Value),
                K3CSharp.CharacterValue cv => SerializeCharacterData(cv.Value[0]),
                K3CSharp.SymbolValue sv => SerializeSymbolData(sv.Value),
                K3CSharp.NullValue => SerializeNullData(),
                
                _ => throw new NotSupportedException($"Unsupported element type: {element.GetType()}")
            };
        }
        
        private byte[] SerializeDictionary(KDictionary dict)
        {
            var dictData = SerializeDictionaryData(dict);
            var message = SerializeMessage(dictData.Length);
            return message.Concat(dictData).ToArray();
        }
        
        private byte[] SerializeDictionaryData(KDictionary dict)
        {
            var writer = new KBinaryWriter();
            var pairData = new List<byte>();
            
            foreach (var kvp in dict.Pairs)
            {
                pairData.AddRange(SerializeValue(kvp.Key));
                pairData.AddRange(SerializeValue(kvp.Value));
            }
            
            writer.WriteInt32(5);  // Dictionary flag
            writer.WriteInt32(dict.Pairs.Count); // Pair count
            writer.WriteBytes(pairData.ToArray()); // Pair data
            return writer.ToArray();
        }
        
        private byte[] SerializeAnonymousFunction(KFunction func)
        {
            var functionData = SerializeAnonymousFunctionData(func);
            var message = SerializeMessage(functionData.Length);
            return message.Concat(functionData).ToArray();
        }
        
        private byte[] SerializeAnonymousFunctionData(KFunction func)
        {
            var writer = new KBinaryWriter();
            var functionSource = Encoding.UTF8.GetBytes(func.Source);
            
            writer.WriteInt32(10); // Function flag
            writer.WriteByte(0); // Extra null byte to match k.exe
            
            // Add error metadata for pre-parsing failures
            if (func.HasParseErrors)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(".k\0"));
            }
            
            writer.WriteBytes(functionSource); // Function source
            writer.WriteByte(0); // Null terminator
            return writer.ToArray();
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
                KList list => SerializeList(list),
                KDictionary dict => SerializeDictionary(dict),
                KFunction func => SerializeAnonymousFunction(func),
                KVector vec => SerializeElementData(vec),
                
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
        
        private KVector ToKVector(K3CSharp.VectorValue vectorValue)
        {
            var kVector = new KVector();
            
            // Determine vector type based on element types
            if (vectorValue.Elements.All(e => e is K3CSharp.IntegerValue))
            {
                kVector.Type = KVectorType.Integer;
                kVector.IntegerElements = vectorValue.Elements.Cast<K3CSharp.IntegerValue>().Select(iv => iv.Value).ToArray();
            }
            else if (vectorValue.Elements.All(e => e is K3CSharp.FloatValue))
            {
                kVector.Type = KVectorType.Float;
                kVector.FloatElements = vectorValue.Elements.Cast<K3CSharp.FloatValue>().Select(fv => fv.Value).ToArray();
            }
            else if (vectorValue.Elements.All(e => e is K3CSharp.CharacterValue))
            {
                kVector.Type = KVectorType.Character;
                kVector.CharacterElements = vectorValue.Elements.Cast<K3CSharp.CharacterValue>().Select(cv => cv.Value[0]).ToArray();
            }
            else if (vectorValue.Elements.All(e => e is K3CSharp.SymbolValue))
            {
                kVector.Type = KVectorType.Symbol;
                kVector.SymbolElements = vectorValue.Elements.Cast<K3CSharp.SymbolValue>().Select(sv => sv.Value).ToArray();
            }
            else
            {
                // Mixed vector - not supported for now, fall back to general handling
                throw new NotSupportedException($"Mixed vectors not supported in element serialization");
            }
            
            return kVector;
        }
        
        private KFunction ToKFunction(K3CSharp.FunctionValue functionValue)
        {
            return new KFunction
            {
                Source = "{" + "[" + string.Join(";", functionValue.Parameters) + "] " + functionValue.BodyText + "}",
                HasParseErrors = false // Could be determined from functionValue.PreParsedTokens if needed
            };
        }
        
        private byte[] SerializeVector(KVector vector)
        {
            var vectorData = SerializeVectorData(vector);
            var message = SerializeMessage(vectorData.Length);
            return message.Concat(vectorData).ToArray();
        }
        
        private int GetVectorElementCount(KVector vector)
        {
            return vector.Type switch
            {
                KVectorType.Integer => vector.IntegerElements.Length,
                KVectorType.Float => vector.FloatElements.Length,
                KVectorType.Character => vector.CharacterElements.Length,
                KVectorType.Symbol => vector.SymbolElements.Length,
                _ => 0
            };
        }
        
        private byte[] SerializeVectorData(KVector vector)
        {
            var elementData = new List<byte>();
            var vectorTypeFlag = vector.Type switch
            {
                KVectorType.Integer => -1,
                KVectorType.Float => -2,
                KVectorType.Character => -3,
                KVectorType.Symbol => -4,
                _ => throw new NotSupportedException($"Unsupported vector type: {vector.Type}")
            };
            
            // Add vector type flag (4 bytes) and element count (4 bytes) at the beginning
            elementData.AddRange(BitConverter.GetBytes(vectorTypeFlag));
            elementData.AddRange(BitConverter.GetBytes(GetVectorElementCount(vector)));
            
            switch (vector.Type)
            {
                case KVectorType.Integer:
                    foreach (var element in vector.IntegerElements)
                    {
                        elementData.AddRange(BitConverter.GetBytes(element));
                    }
                    break;
                case KVectorType.Float:
                    foreach (var element in vector.FloatElements)
                    {
                        elementData.AddRange(BitConverter.GetBytes(element));
                    }
                    break;
                case KVectorType.Character:
                    foreach (var element in vector.CharacterElements)
                    {
                        elementData.Add((byte)element);
                    }
                    elementData.Add(0); // Null terminator for character vector
                    break;
                case KVectorType.Symbol:
                    foreach (var element in vector.SymbolElements)
                    {
                        var symbolBytes = Encoding.UTF8.GetBytes(element);
                        elementData.AddRange(symbolBytes);
                        elementData.Add(0); // Null terminator per symbol
                    }
                    break;
            }
            
            return elementData.ToArray();
        }
    }
}
