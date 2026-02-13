# Integer Vector Serialization Hypothesis

## **Format Structure**
```
[1 byte] Data Architecture: \001 (little-endian)
[1 byte] Serialization Type: \000 (_bd serialization)
[2 bytes] Reserved: \000\000
[4 bytes] Length: 8 + (elements × 4)
[4 bytes] Vector Flag: \377\377\377\377 (-1)
[4 bytes] Element Count: number of elements
[4×n bytes] Element Data: each integer as little-endian
```

**📖 Source**: Header information obtained from https://code.kx.com/q/kb/serialization/

## **Length Formula**
```
Length = 8 + (elements × 4)
```

### **Validation Examples:**

**Empty Vector (`!0`):**
- Elements: 0
- Length: 8 + (0×4) = 8
- Expected: `\001\000\000\000\b\000\000\000\377\377\377\377\000\000\000\000`
- Actual: `\001\000\000\000\b\000\000\000\377\377\377\377\000\000\000\000` ✅

**Single-Element Vector (`,1`):**
- Elements: 1
- Length: 8 + (1×4) = 12
- Expected: `\001\000\000\000\014\000\000\000\377\377\377\377\001\000\000\000\001\000\000\000`
- Actual: `\001\000\000\000\014\000\000\000\377\377\377\377\001\000\000\000\001\000\000\000` ✅

**Multi-Element Vector (`1 2 3`):**
- Elements: 3
- Length: 8 + (3×4) = 20
- Expected: `\001\000\000\000\024\000\000\000\377\377\377\377\003\000\000\000\001\000\000\000\002\000\000\000\003\000\000\000`
- Actual: `\001\000\000\000\024\000\000\000\377\377\377\377\003\000\000\000\001\000\000\000\002\000\000\000\003\000\000\000` ✅

## **Key Insights**

1. **Length Field**: Represents the size of data that comes after the length field itself
2. **Consistent Structure**: All vectors (empty, single, multi-element) follow the same format
3. **No Special Cases**: The same formula works for all vector sizes
4. **Data Boundary**: Length field accurately describes subsequent data size

## **Implementation Strategy**

```csharp
private byte[] SerializeIntegerVector(int[] elements)
{
    var writer = new KBinaryWriter();
    
    writer.WriteInt32(1);  // Type ID
    writer.WriteInt32(8 + elements.Length * 4);  // Length = 8 + (elements × 4)
    writer.WriteInt32(-1);  // Vector flag
    writer.WriteInt32(elements.Length);  // Element count
    
    foreach (var element in elements)
    {
        writer.WriteInt32(element);
    }
    
    return writer.ToArray();
}
```

## **Test Cases Covered**

- ✅ Empty vectors (`!0`)
- ✅ Single-element vectors (`,1`)
- ✅ Multi-element vectors (`1 2 3`)
- ✅ Special integer values (`0N`, `0I`, `-0I`)

## **Research Data Sources**

- `serialization_IntegerVector_20260212_115255.txt` (corrected edge cases)
- `serialization_IntegerVector_20260211_035418.txt` (100 random examples)
- `serialization_IntegerVector_20260210_011338.txt` (10 random examples)

## **Confidence Level**

**HIGH** - Hypothesis validated against 100+ examples with no contradictions found.
