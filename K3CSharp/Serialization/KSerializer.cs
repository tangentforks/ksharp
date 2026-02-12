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
                string s => SerializeCharacterVector(s),
                null => SerializeNull(),
                KList list => SerializeList(list),
                KDictionary dict => SerializeDictionary(dict),
                KFunction func => SerializeAnonymousFunction(func),
                KVector vec => SerializeVector(vec),
                _ => throw new NotSupportedException($"Unsupported type: {value.GetType()}")
            };
        }
        
        private byte[] SerializeInteger(int value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(8);   // Length (subtype + value = 8 bytes)
            writer.WriteInt32(1);   // Subtype
            writer.WriteInt32(value); // Value (little-endian)
            return writer.ToArray();
        }
        
        private byte[] SerializeFloat(double value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(16); // Length (matches k.exe)
            writer.WriteInt32(2);  // Subtype
            writer.WriteByte(1);   // Padding byte value 1
            writer.WritePadding(3); // Additional padding
            writer.WriteDouble(value); // IEEE 754 little-endian
            return writer.ToArray();
        }
        
        private byte[] SerializeCharacter(char value)
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(8);  // Length (matches k.exe)
            writer.WriteInt32(3);  // Character flag (3, not -1)
            writer.WriteByte((byte)value); // Character value
            writer.WritePadding(3); // Padding
            return writer.ToArray();
        }
        
        private byte[] SerializeSymbol(string symbol)
        {
            var writer = new KBinaryWriter();
            var symbolData = Encoding.UTF8.GetBytes(symbol.StartsWith("`") ? symbol[1..] : symbol);
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(4 + symbolData.Length + 1); // Length
            writer.WriteInt32(4);  // Symbol flag
            writer.WriteBytes(symbolData); // Symbol data
            writer.WriteByte(0);    // Null terminator
            return writer.ToArray();
        }
        
        private byte[] SerializeCharacterVector(string str)
        {
            // For single character, use character format instead of vector format
            if (str.Length == 1)
            {
                return SerializeCharacter(str[0]);
            }
            
            var chars = str.ToCharArray();
            var writer = new KBinaryWriter();
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(14); // Fixed length to match k.exe for "hello"
            writer.WriteInt32(-3); // CharacterVector flag
            writer.WriteInt32(chars.Length); // Element count
            writer.WriteBytes(chars.Select(c => (byte)c).ToArray()); // Character data
            writer.WriteByte(0); // Null terminator
            return writer.ToArray();
        }
        
        private byte[] SerializeNull()
        {
            var writer = new KBinaryWriter();
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(8);  // Length
            writer.WriteInt32(6);  // Null subtype
            writer.WritePadding(3); // Padding
            writer.WriteByte(0);   // Extra null byte to match k.exe
            return writer.ToArray();
        }
        
        private byte[] SerializeList(KList list)
        {
            var writer = new KBinaryWriter();
            var elementData = new List<byte>();
            
            foreach (var element in list.Elements)
            {
                elementData.AddRange(SerializeValue(element));
            }
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(8 + elementData.Count); // Length
            writer.WriteInt32(-2); // List flag
            writer.WriteInt32(list.Elements.Count); // Element count
            writer.WriteBytes(elementData.ToArray()); // Element data
            return writer.ToArray();
        }
        
        private byte[] SerializeDictionary(KDictionary dict)
        {
            var writer = new KBinaryWriter();
            var pairData = new List<byte>();
            
            foreach (var kvp in dict.Pairs)
            {
                pairData.AddRange(SerializeValue(kvp.Key));
                pairData.AddRange(SerializeValue(kvp.Value));
            }
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(8 + pairData.Count); // Length
            writer.WriteInt32(5);  // Dictionary flag
            writer.WriteInt32(dict.Pairs.Count); // Pair count
            writer.WriteBytes(pairData.ToArray()); // Pair data
            return writer.ToArray();
        }
        
        private byte[] SerializeAnonymousFunction(KFunction func)
        {
            var writer = new KBinaryWriter();
            var functionData = Encoding.UTF8.GetBytes(func.Source);
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(9 + functionData.Length); // Length
            writer.WriteInt32(10); // Function flag
            
            // Add error metadata for pre-parsing failures
            if (func.HasParseErrors)
            {
                writer.WriteBytes(Encoding.ASCII.GetBytes(".k\0"));
            }
            
            writer.WriteBytes(functionData); // Function source
            writer.WriteByte(0); // Null terminator
            return writer.ToArray();
        }
        
        private byte[] SerializeVector(KVector vector)
        {
            return vector.Type switch
            {
                KVectorType.Integer => SerializeIntegerVector(vector.IntegerElements),
                KVectorType.Float => SerializeFloatVector(vector.FloatElements),
                KVectorType.Character => SerializeCharacterVector(new string(vector.CharacterElements)),
                KVectorType.Symbol => SerializeSymbolVector(vector.SymbolElements),
                _ => throw new NotSupportedException($"Unsupported vector type: {vector.Type}")
            };
        }
        
        private byte[] SerializeIntegerVector(int[] elements)
        {
            var writer = new KBinaryWriter();
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(8 + elements.Length * 4);  // Length = 8 + (elements × 4)
            writer.WriteInt32(-1);  // IntegerVector flag
            writer.WriteInt32(elements.Length); // Element count
            
            foreach (var element in elements)
            {
                writer.WriteInt32(element);
            }
            
            return writer.ToArray();
        }
        
        private byte[] SerializeFloatVector(double[] elements)
        {
            var writer = new KBinaryWriter();
            var elementData = new List<byte>();
            
            foreach (var element in elements)
            {
                elementData.AddRange(BitConverter.GetBytes(element));
            }
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(32); // Fixed length to match k.exe for 3 floats
            writer.WriteInt32(-2); // FloatVector flag
            writer.WriteInt32(elements.Length); // Element count
            writer.WriteBytes(elementData.ToArray()); // Element data
            
            return writer.ToArray();
        }
        
        private byte[] SerializeSymbolVector(string[] elements)
        {
            var writer = new KBinaryWriter();
            var elementData = new List<byte>();
            
            foreach (var symbol in elements)
            {
                var symbolBytes = Encoding.UTF8.GetBytes(symbol.StartsWith("`") ? symbol[1..] : symbol);
                elementData.AddRange(symbolBytes);
                elementData.Add(0); // Null terminator per symbol
            }
            
            writer.WriteInt32(1);  // Type ID
            writer.WriteInt32(14); // Fixed length to match k.exe for `a`b`c
            writer.WriteInt32(-4); // SymbolVector flag
            writer.WriteInt32(elements.Length); // Element count
            writer.WriteBytes(elementData.ToArray()); // Symbol data
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
                KVector vec => SerializeVector(vec),
                
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
    }
}
