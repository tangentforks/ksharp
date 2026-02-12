using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K3CSharp.Serialization
{
    /// <summary>
    /// Main K deserializer implementing unified serialization pattern
    /// </summary>
    public class KDeserializer
    {
        public object Deserialize(byte[] data)
        {
            if (data.Length < 8) throw new ArgumentException("Invalid data - minimum 8 bytes required");
            
            var reader = new KSerializationReader(data);
            var typeId = reader.ReadInt32();
            var length = reader.ReadInt32();
            
            return typeId switch
            {
                1 => DeserializeScalar(reader, length),
                -1 => DeserializeIntegerVector(reader, length),
                -2 => DeserializeFloatVector(reader, length),
                -3 => DeserializeCharacterVector(reader, length),
                -4 => DeserializeSymbolVector(reader, length),
                0 => DeserializeList(reader, length),
                5 => DeserializeDictionary(reader, length),
                7 => DeserializeAnonymousFunction(reader, length),
                _ => throw new NotSupportedException($"Unsupported type: {typeId}")
            };
        }
        
        private object DeserializeScalar(KSerializationReader reader, int length)
        {
            var subtype = reader.ReadInt32();
            return subtype switch
            {
                1 => reader.ReadInt32(), // Integer
                2 => reader.ReadDouble(), // Float
                3 => (char)reader.ReadByte(), // Character
                4 => ReadSymbol(reader, length), // Symbol
                6 => new NullValue(), // Null
                _ => throw new NotSupportedException($"Unsupported scalar subtype: {subtype}")
            };
        }
        
        private string ReadSymbol(KSerializationReader reader, int length)
        {
            var symbolLength = length - 8; // Subtract header (type_id + length + subtype)
            var symbolBytes = reader.ReadBytes(symbolLength);
            return Encoding.UTF8.GetString(symbolBytes);
        }
        
        private object DeserializeIntegerVector(KSerializationReader reader, int length)
        {
            var vectorFlag = reader.ReadInt32();
            var elementCount = reader.ReadInt32();
            var elements = new List<int>();
            
            for (int i = 0; i < elementCount; i++)
            {
                elements.Add(reader.ReadInt32());
            }
            
            return elements.ToArray();
        }
        
        private object DeserializeFloatVector(KSerializationReader reader, int length)
        {
            var vectorFlag = reader.ReadInt32();
            var elementCount = reader.ReadInt32();
            var elements = new List<double>();
            
            for (int i = 0; i < elementCount; i++)
            {
                elements.Add(reader.ReadDouble());
            }
            
            return elements.ToArray();
        }
        
        private object DeserializeCharacterVector(KSerializationReader reader, int length)
        {
            var vectorFlag = reader.ReadInt32();
            var elementCount = reader.ReadInt32();
            var elements = new List<char>();
            
            for (int i = 0; i < elementCount; i++)
            {
                elements.Add((char)reader.ReadByte());
            }
            
            // Read and discard null terminator
            if (reader.HasMoreData) reader.ReadByte();
            
            return elements.ToArray();
        }
        
        private object DeserializeSymbolVector(KSerializationReader reader, int length)
        {
            var vectorFlag = reader.ReadInt32();
            var elementCount = reader.ReadInt32();
            var elements = new List<string>();
            
            for (int i = 0; i < elementCount; i++)
            {
                var symbol = ReadSymbol(reader, length);
                elements.Add(symbol);
            }
            
            return elements.ToArray();
        }
        
        private object DeserializeList(KSerializationReader reader, int length)
        {
            var listFlag = reader.ReadInt32();
            var elementCount = reader.ReadInt32();
            var elements = new List<object>();
            
            for (int i = 0; i < elementCount; i++)
            {
                // Recursively deserialize each element
                var element = DeserializeValue(reader);
                elements.Add(element);
            }
            
            return elements.ToArray();
        }
        
        private object DeserializeDictionary(KSerializationReader reader, int length)
        {
            var dictFlag = reader.ReadInt32();
            var pairCount = reader.ReadInt32();
            var pairs = new List<KeyValuePair<object, object>>();
            
            for (int i = 0; i < pairCount; i++)
            {
                var key = DeserializeValue(reader);
                var value = DeserializeValue(reader);
                pairs.Add(new KeyValuePair<object, object>(key, value));
            }
            
            return pairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        
        private object DeserializeAnonymousFunction(KSerializationReader reader, int length)
        {
            var functionFlag = reader.ReadInt32();
            var functionLength = length - 8; // Subtract header
            
            // Check for error metadata
            var hasErrorMetadata = false;
            if (functionLength >= 4)
            {
                var currentPos = reader.Position;
                var errorBytes = reader.ReadBytes(4);
                var errorStr = Encoding.ASCII.GetString(errorBytes);
                hasErrorMetadata = errorStr == ".k\0";
                functionLength -= 4; // Adjust for metadata
            }
            
            var functionBytes = reader.ReadBytes(functionLength);
            var functionSource = Encoding.UTF8.GetString(functionBytes);
            
            return new { Source = functionSource, HasParseErrors = hasErrorMetadata };
        }
        
        private object DeserializeValue(KSerializationReader reader)
        {
            // Save current position to peek at type
            var startPos = reader.Position;
            var typeId = reader.ReadInt32();
            var length = reader.ReadInt32();
            
            // Reset position to read the actual value
            reader.Position = startPos;
            
            return typeId switch
            {
                1 => DeserializeScalar(reader, length),
                -1 => DeserializeIntegerVector(reader, length),
                -2 => DeserializeFloatVector(reader, length),
                -3 => DeserializeCharacterVector(reader, length),
                -4 => DeserializeSymbolVector(reader, length),
                0 => DeserializeList(reader, length),
                5 => DeserializeDictionary(reader, length),
                7 => DeserializeAnonymousFunction(reader, length),
                _ => throw new NotSupportedException($"Unsupported type: {typeId}")
            };
        }
    }
}
