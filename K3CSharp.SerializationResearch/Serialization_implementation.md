# K3CSharp Serialization Implementation Guide

## Overview

This document provides a complete specification for K3CSharp's serialization format, designed to enable implementation of corresponding deserialization functionality (`_db`). The format achieves byte-for-byte compatibility with k.exe's binary serialization.

## Message Structure

### Header Format
```
[4 bytes] Message Type (always 1 for serialized data)
[4 bytes] Message Length (excluding header)
[Variable] Serialized Data
```

**Total Message:** `8 bytes + serialized_data_length`

## Type System

### K Type Numbers
| Type | Description | K Type Number |
|------|-------------|---------------|
| `integer, int` | 32-bit integers with special values | 1 |
| `float` | IEEE 754 doubles with special values | 2 |
| `character, char` | ASCII characters (0-255) | 3 |
| `symbol` | Unquoted and quoted symbols | 4 |
| `dictionary, dict` | Simple and complex dictionaries | 5 |
| `null` | Null value | 6 |
| `anonymous, func` | Anonymous functions | 7 |
| `intvector` | Integer vectors | -1 |
| `floatvector` | Float vectors | -2 |
| `charvector` | Character vectors | -3 |
| `symbolvector` | Symbol vectors | -4 |
| `list` | Mixed type lists | 0 |

## Primitive Type Serialization

### Integer (Type 1)
```
[4 bytes] Type Flag (1)
[4 bytes] Integer Value (little-endian)
```

**Special Values:**
- `0x80000000` → Null integer
- `0x80000001` → Infinity (∞)
- `0x80000002` → Negative Infinity (-∞)
- `0x80000003` → Not a Number (NaN)

### Float (Type 2)
```
[4 bytes] Type Flag (2)
[8 bytes] Double Value (IEEE 754, little-endian)
```

**Special Values:**
- Standard IEEE 754 special values (NaN, ±Infinity)

### Character (Type 3)
```
[4 bytes] Type Flag (3)
[1 byte] ASCII Character Value (0-255)
```

### Symbol (Type 4)
```
[4 bytes] Type Flag (4)
[Variable] Symbol Data (UTF-8, without backtick)
[1 byte] Null Terminator (0)
[Padding] 4-byte alignment (only in mixed lists)
```

**Padding Calculation (Mixed Lists):**
```csharp
int totalSize = 5 + symbolData.Length; // 4 bytes flag + symbol data + null
int paddingNeeded = (4 - (totalSize % 4)) % 4;
```

### Null (Type 6)
```
[4 bytes] Type Flag (6)
```

## Vector Serialization

### Vector Header
```
[4 bytes] Vector Type (negative for homogeneous, 0 for mixed, 5 for dictionary)
[4 bytes] Element Count
[Variable] Element Data
```

### Homogeneous Vectors

#### Integer Vector (Type -1)
```
[4 bytes] Type Flag (-1)
[4 bytes] Element Count
[4 × count] Integer Values (little-endian)
```

#### Float Vector (Type -2)
```
[4 bytes] Type Flag (-2)
[4 bytes] Element Count
[8 × count] Double Values (IEEE 754, little-endian)
```

#### Character Vector (Type -3)
```
[4 bytes] Type Flag (-3)
[4 bytes] Element Count
[count] Character Values
[1 byte] Null Terminator (0)
```

#### Symbol Vector (Type -4)
```
[4 bytes] Type Flag (-4)
[4 bytes] Element Count
[Variable] Concatenated Symbol Data
[1 byte] Null Terminator (0)
```

**Symbol Vector Format:**
- Symbols concatenated without separators
- Each symbol includes its null terminator
- No individual symbol padding

## Complex Type Serialization

### Mixed Lists (Type 0)

#### Element-wise 8-byte Alignment
Mixed lists containing vectors or functions require element-wise 8-byte alignment:

```csharp
if (hasFunctions || hasVectors)
{
    var alignedElementData = new List<byte>();
    int currentOffset = 8; // Start after type+count header
    
    foreach (var element in list.Elements)
    {
        var serialized = SerializeElementData(element, true);
        
        // Pre-element padding
        int paddingNeeded = (8 - (currentOffset % 8)) % 8;
        if (paddingNeeded > 0)
        {
            alignedElementData.AddRange(new byte[paddingNeeded]);
            currentOffset += paddingNeeded;
        }
        
        alignedElementData.AddRange(serialized);
        currentOffset += serialized.Length;
        
        // Inter-element padding (except for last element)
        if (element != list.Elements.Last())
        {
            int postPaddingNeeded = (8 - (currentOffset % 8)) % 8;
            if (postPaddingNeeded > 0)
            {
                alignedElementData.AddRange(new byte[postPaddingNeeded]);
                currentOffset += postPaddingNeeded;
            }
        }
    }
    
    // Final 8-byte boundary alignment
    int finalPaddingNeeded = (8 - (currentOffset % 8)) % 8;
    if (finalPaddingNeeded > 0)
    {
        alignedElementData.AddRange(new byte[finalPaddingNeeded]);
    }
}
```

#### Raw Serialization (Simple Lists)
Lists containing only primitives use raw serialization:
```
[4 bytes] Type Flag (0)
[4 bytes] Element Count
[Serialized Elements] (no type flags, raw data only)
```

### Dictionaries (Type 5)

**Critical Insight:** Dictionaries use the **exact same serialization logic** as general mixed lists (Type 0). They are internally represented as lists of triplets.

#### Dictionary Structure
Each dictionary entry is a triplet:
```
[Element 1] Symbol (key)
[Element 2] Value (any type)
[Element 3] Attributes (dictionary or null)
```

#### Serialization Process
```csharp
// Dictionaries use same processing as general lists
if (vectorType == 0 || vectorType == 5)
{
    bool hasFunctions = list.Elements.Any(e => e is FunctionValue);
    bool hasVectors = list.Elements.Any(e => e is VectorValue);
    
    if (hasFunctions || hasVectors)
    {
        // Apply element-wise 8-byte alignment (identical to mixed lists)
        // ... same alignment logic as Type 0 lists
    }
}
```

#### Dictionary Entry Examples

**Simple Entry:**
```
.,(`key;value) → Serializes as list with 2 elements:
- Element 1: Symbol "key"
- Element 2: Value (with implicit null attribute)
```

**Entry with Attributes:**
```
.,(`key;value;.,(`attr1;val1;`attr2;val2)) → Serializes as list with 3 elements:
- Element 1: Symbol "key"
- Element 2: Value
- Element 3: Attribute dictionary
```

**Multiple Entries:**
```
.((`a;1);(`b;2);(`c;3)) → Serializes as list with 3 triplet elements
```

## Anonymous Functions (Type 7)

### Function Structure
```
[4 bytes] Type Flag (7)
[4 bytes] Function Type (1 = verb, 2 = adverb, 3 = projection)
[Variable] Function Data (implementation-specific)
```

**Note:** Function serialization is complex and implementation-dependent. Focus on core types for deserialization.

## Alignment Rules Summary

### 4-byte Alignment
- **Symbols in mixed lists:** Always aligned to 4-byte boundaries
- **Vector headers:** Always aligned
- **Primitive types:** Naturally aligned

### 8-byte Alignment
- **Mixed lists with vectors/functions:** Element-wise alignment
- **Mixed lists with dictionaries:** Element-wise alignment  
- **Dictionaries:** Element-wise alignment (same as mixed lists)

### Alignment Algorithm
```csharp
// Element-wise 8-byte alignment for complex structures
int currentOffset = 8; // After type+count header

foreach (var element)
{
    // Pre-element alignment
    int prePadding = (8 - (currentOffset % 8)) % 8;
    
    // Add serialized element
    currentOffset += prePadding + elementSize;
    
    // Inter-element alignment (except last)
    if (!isLastElement)
    {
        int postPadding = (8 - (currentOffset % 8)) % 8;
        currentOffset += postPadding;
    }
}

// Final alignment
int finalPadding = (8 - (currentOffset % 8)) % 8;
```

## Deserialization Implementation Guide

### Reading Process
1. **Read Message Header:** Extract length and verify type
2. **Read Vector Header:** Get vector type and element count
3. **Determine Serialization Strategy:**
   - Homogeneous vector → Use raw data reading
   - Mixed list/dictionary → Use element-wise parsing
4. **Parse Elements:** Apply appropriate type handlers
5. **Handle Alignment:** Skip padding bytes as needed

### Key Challenges
1. **Alignment Detection:** Determine when to apply 4-byte vs 8-byte alignment
2. **Dictionary Recognition:** Type 5 indicates dictionary structure
3. **Element Boundary Detection:** Use alignment rules to find element boundaries
4. **Implicit Attributes:** 2-element dictionary entries need implicit null attribute

### Implementation Order
1. **Primitive Types:** Integer, Float, Character, Symbol, Null
2. **Homogeneous Vectors:** All vector types
3. **Simple Mixed Lists:** Without complex alignment
4. **Complex Mixed Lists:** With element-wise alignment
5. **Dictionaries:** Using same logic as mixed lists
6. **Functions:** Last (complex, implementation-dependent)

## Testing Strategy

### Validation Approach
1. **Round-trip Testing:** Serialize → Deserialize → Compare
2. **k.exe Compatibility:** Compare byte output with reference implementation
3. **Edge Cases:** Null values, empty structures, special numbers
4. **Alignment Testing:** Verify correct padding handling

### Test Cases
- All primitive types with special values
- Empty and single-element vectors
- Complex nested structures
- Dictionaries with and without attributes
- Mixed lists with various element combinations

## Performance Considerations

### Optimization Opportunities
1. **Buffer Management:** Pre-allocate buffers for known sizes
2. **Alignment Calculation:** Use bit operations for modulo
3. **Type Detection:** Cache type information for repeated elements
4. **Memory Pooling:** Reuse byte arrays for common operations

### Memory Usage
- **Primitive Types:** Fixed small overhead
- **Vectors:** Linear with element count
- **Complex Structures:** Quadratic in worst case (deep nesting)

---

**This specification provides complete information for implementing `_db` deserialization with full compatibility to the K3CSharp serialization format.**
