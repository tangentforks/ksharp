using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K3CSharp
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
            var messageType = reader.ReadInt32();  // First 4 bytes: Message Type (always 1)
            var messageLength = reader.ReadInt32(); // Next 4 bytes: Message Length
            
            // Message Type should always be 1 for serialized data from _bd
            if (messageType != 1) 
                throw new ArgumentException($"Invalid message type: {messageType}");
            
            // Read the actual serialized data payload
            var payloadData = reader.ReadBytes(messageLength);
            var payloadReader = new KSerializationReader(payloadData);
            
            return DeserializeValue(payloadReader);
        }
        
        private object DeserializeScalar(KSerializationReader reader)
        {
            return new IntegerValue(reader.ReadInt32()); // Integer value as K3Value
        }
        
        private object DeserializeFloat(KSerializationReader reader, int typeId)
        {
            // typeId is already read and should be 2
            if (typeId != 2)
                throw new ArgumentException($"Invalid type flag for float: {typeId}");
            
            // Read subtype flag (4 bytes)
            var subtypeFlag = reader.ReadInt32();
            if (subtypeFlag != 1)
                throw new ArgumentException($"Invalid subtype flag for float: {subtypeFlag}");
            
            // Read the actual double value (8 bytes)
            var doubleValue = reader.ReadDouble();
            
            return new FloatValue(doubleValue);
        }
        
        private object DeserializeCharacter(KSerializationReader reader)
        {
            return new CharacterValue(((char)reader.ReadByte()).ToString()); // Character as K3Value
        }
        
        private object DeserializeSymbol(KSerializationReader reader)
        {
            var bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }
            return new SymbolValue(Encoding.UTF8.GetString(bytes.ToArray())); // Symbol as K3Value
        }
        
        private string ReadSymbol(KSerializationReader reader, int length)
        {
            var symbolLength = length - 8; // Subtract header (type_id + length + subtype)
            var symbolBytes = reader.ReadBytes(symbolLength);
            return Encoding.UTF8.GetString(symbolBytes);
        }
        
        private object DeserializeIntegerVector(KSerializationReader reader)
        {
            var elementCount = reader.ReadInt32();
            var elements = new List<int>();
            
            for (int i = 0; i < elementCount; i++)
            {
                elements.Add(reader.ReadInt32());
            }
            
            return new VectorValue(elements.Select(x => (K3Value)new IntegerValue(x)).ToList());
        }
        
        private object DeserializeFloatVector(KSerializationReader reader)
        {
            var elementCount = reader.ReadInt32();
            var elements = new List<double>();
            
            for (int i = 0; i < elementCount; i++)
            {
                elements.Add(reader.ReadDouble());
            }
            
            return new VectorValue(elements.Select(x => (K3Value)new FloatValue(x)).ToList());
        }
        
        private object DeserializeCharacterVector(KSerializationReader reader)
        {
            var elementCount = reader.ReadInt32();
            var elements = new List<char>();
            
            for (int i = 0; i < elementCount; i++)
            {
                elements.Add((char)reader.ReadByte());
            }
            
            // Read and discard null terminator
            if (reader.HasMoreData) reader.ReadByte();
            
            return new VectorValue(elements.Select(x => (K3Value)new CharacterValue(x.ToString())).ToList());
        }
        
        private object DeserializeSymbolVector(KSerializationReader reader)
        {
            var elementCount = reader.ReadInt32();
            var elements = new List<string>();
            
            // Symbol vectors store symbols concatenated with null terminators
            for (int i = 0; i < elementCount; i++)
            {
                var bytes = new List<byte>();
                byte b;
                while ((b = reader.ReadByte()) != 0)
                {
                    bytes.Add(b);
                }
                var symbol = Encoding.UTF8.GetString(bytes.ToArray());
                elements.Add(symbol); // Don't add backtick here - SymbolValue.ToString() will add it
            }
            
            return new VectorValue(elements.Select(x => (K3Value)new SymbolValue(x)).ToList());
        }
        
        private object DeserializeList(KSerializationReader reader)
        {
            // Note: typeId (listFlag) was already read by DeserializeValue
            // We only need to read the elementCount here
            var elementCount = reader.ReadInt32();
            var elements = new List<K3Value>();
            
            // For empty lists, just return an empty VectorValue
            for (int i = 0; i < elementCount; i++)
            {
                var element = (K3Value)DeserializeValue(reader);
                elements.Add(element);
            }
            
            return new VectorValue(elements, "mixed"); // Mark as mixed vector
        }
        
        private int GetElementSize(K3Value element)
        {
            // For mixed lists, we need to calculate the actual serialized size
            // This is complex because elements can be nested structures
            return element switch
            {
                IntegerValue => 8, // 4 bytes type + 4 bytes value
                FloatValue => 12, // 4 bytes type + 8 bytes value
                CharacterValue => 5, // 4 bytes type + 1 byte value
                SymbolValue sv => 5 + sv.Value.Length - 1, // 4 bytes type + symbol data + null
                NullValue => 4, // 4 bytes type only
                VectorValue vv => 8 + GetVectorSize(vv), // 8 bytes header + element data
                DictionaryValue dv => 8 + GetDictionarySize((DictionaryValue)element),
                _ => 8 // Default for complex nested structures
            };
        }
        
        private int GetVectorSize(VectorValue vector)
        {
            int size = 0;
            foreach (var element in vector.Elements)
            {
                size += GetElementSize(element);
            }
            return size;
        }
        
        private int GetDictionarySize(DictionaryValue dict)
        {
            // Simplified - dictionaries are serialized as lists of triplets
            var elements = dict.Entries.SelectMany(kvp => 
            {
                var (val, attr) = kvp.Value;
                var elementList = new List<K3Value> { kvp.Key, val };
                // Only add attribute if it's not null
                if (attr != null)
                {
                    elementList.Add(attr);
                }
                return elementList;
            }).ToList();
            return GetVectorSize(new VectorValue(elements));
        }
        private object DeserializeDictionary(KSerializationReader reader)
        {
            // The reader is positioned after typeId (5) has been read by DeserializeValue
            // Dictionaries are serialized as: [type(5)][count][element1][element2][element3][element4]...
            // So we need to read the elementCount directly
            var elementCount = reader.ReadInt32();
            var entries = new Dictionary<SymbolValue, (K3Value, DictionaryValue)>();
            
            // Dictionaries are serialized as lists of triplet vectors: [(key;value;attr), (key;value;attr), ...]
            // elementCount is the number of triplet vectors (same as list.Elements.Count in serialization)
            for (int i = 0; i < elementCount; i++)
            {
                // Each element is a triplet vector (key; value; attributes)
                // The triplet is serialized as a list, so deserialize it as such
                var triplet = (K3Value)DeserializeValue(reader);
                
                if (triplet is VectorValue vector && vector.Elements.Count >= 2)
                {
                    var key = vector.Elements[0];
                    var value = vector.Elements[1];
                    var attributes = vector.Elements.Count > 2 ? vector.Elements[2] : new NullValue();
                    
                    if (key is SymbolValue symbolKey)
                    {
                        var attrDict = attributes as DictionaryValue ?? new DictionaryValue();
                        entries[symbolKey] = (value, attrDict);
                    }
                }
            }
            
            return new DictionaryValue(entries);
        }
        
        private object DeserializeAnonymousFunction(KSerializationReader reader, int typeId)
        {
            // Check for .k metadata (undefined variables)
            var hasErrorMetadata = false;
            if (reader.HasMoreData)
            {
                var nextByte = reader.ReadByte();
                if (nextByte == 0x2E) // '.' character
                {
                    // Read the rest of .k metadata
                    var kByte = reader.ReadByte();
                    var nullByte = reader.ReadByte();
                    hasErrorMetadata = (kByte == 0x6B && nullByte == 0x00); // "k\0"
                }
                else if (nextByte == 0x00)
                {
                    // This is the null byte for functions without undefined variables
                    // Continue to read function source
                }
                else
                {
                    // This is the start of the function source, put it back
                    reader.Position -= 1;
                }
            }
            
            // Read function source until null terminator
            var functionBytes = new List<byte>();
            byte b;
            while (reader.HasMoreData && (b = reader.ReadByte()) != 0)
            {
                functionBytes.Add(b);
            }
            var functionSource = Encoding.UTF8.GetString(functionBytes.ToArray());
            
            return new FunctionValue(functionSource, new List<string>(), null!, functionSource);
        }
        
        private object DeserializeValue(KSerializationReader reader)
        {
            // Save current position to peek at type
            var startPos = reader.Position;
            var typeId = reader.ReadInt32();
            
            return typeId switch
            {
                1 => DeserializeScalar(reader),
                2 => DeserializeFloat(reader, typeId),
                3 => DeserializeCharacter(reader),
                4 => DeserializeSymbol(reader),
                6 => new NullValue(),
                -1 => DeserializeIntegerVector(reader),
                -2 => DeserializeFloatVector(reader),
                -3 => DeserializeCharacterVector(reader),
                -4 => DeserializeSymbolVector(reader),
                0 => DeserializeList(reader),
                5 => DeserializeDictionary(reader),
                7 => DeserializeAnonymousFunction(reader, typeId), // Legacy type 7
                10 => DeserializeAnonymousFunction(reader, typeId), // Functions use type 10
                _ => throw new NotSupportedException($"Unsupported type: {typeId}")
            };
        }
    }
}
