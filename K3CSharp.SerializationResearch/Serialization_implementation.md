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
[4 bytes] Subtype Flag (1)
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

### **Element-wise 8-byte Alignment (Simplified)**
Mixed lists and dictionaries containing complex structures apply element-wise 8-byte alignment using the simplified rules above.

#### Raw Serialization (Simple Lists)
Lists containing only primitives use raw serialization:
```
[4 bytes] Type Flag (0)
[4 bytes] Element Count
[Serialized Elements] (no type flags, raw data only)
```

### Dictionaries (Type 5)

**Critical Insight:** Dictionaries use the **exact same serialization logic** as general mixed lists (Type 0) with simplified padding rules applied at the element level.

#### Dictionary Structure
Each dictionary entry is a triplet:
```
[Element 1] Symbol (key)
[Element 2] Value (any type)
[Element 3] Attributes (dictionary or null)
```

#### Serialization Process
```csharp
// Dictionaries use same processing as general lists with simplified padding
if (vectorType == 0 || vectorType == 5)
{
    // Apply simplified element-wise 8-byte alignment rules
    // Each element (leaf or simple vector) padded to 8-byte boundary
    // No complex pre/post padding calculations needed
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

### Function Message Structure
```
[4 bytes] Message Type (always 1 for serialized data)
[4 bytes] Message Length (excluding header)
[4 bytes] Function Flag (always 10)
[1 byte] Metadata Flag (0 or .k metadata)
[Variable] Function Source (UTF-8)
[1 byte] Null Terminator (0)
```

**Total Message:** `13 bytes + function_source_length` (without .k metadata)  
**Total Message:** `16 bytes + function_source_length` (with .k metadata)

### Function Data Format
```
[4 bytes] Function Flag (10)
[1 byte] Metadata Indicator:
  - 0x00: No undefined variables
  - 0x2E 0x6B 0x00: ".k\0" when undefined variables present
[Variable] Function Source Text (UTF-8, reconstructed from AST)
[1 byte] Null Terminator (0)
```

### Function Source Reconstruction
The function source is reconstructed as:
```
{[param1;param2;...] body_expression}
```

### Undefined Variable Detection (.k metadata)
The `.k\0` metadata is added when:
- Function contains undefined variables (variables not in parameter list)
- k.exe uses this to flag parsing errors or unresolved references

### Examples

**Simple Function (no undefined variables):**
```
{+} → "\001\000\000\000\016\000\000\000\n\000\000\000\000{+}\000"
```
Breakdown:
- `\001\000\000\000` - Message type (1)
- `\016\000\000\000` - Message length (24 bytes)
- `\n\000\000\000` - Function flag (10)
- `\000` - No undefined variables
- `{+}` - Function source
- `\000` - Null terminator

**Function with Undefined Variables:**
```
{[xyz] xy|3} → "\001\000\000\000\024\000\000\000\n\000\000\000.k\000{[xyz] xy|3}\000"
```
Breakdown:
- `\001\000\000\000` - Message type (1)
- `\024\000\000\000` - Message length (28 bytes)
- `\n\000\000\000` - Function flag (10)
- `.k\000` - Undefined variables present
- `{[xyz] xy|3}` - Function source
- `\000` - Null terminator

## Simplified Padding Rules

### **Core Padding Principles**

1. **Null Termination First**: Apply all required null termination rules before padding calculations
2. **8-byte Boundary Padding**: Serialized data of nulls must be padded to 8-byte boundaries
3. **Element-wise Padding**: When serializing mixed vectors or dictionaries, every leaf or simple vector (types -1, -2, -3, -4) must have its data padded to 8-byte boundaries
4. **No Deep Descent**: For padding purposes, do not descend into individual items of simple vectors (types -1, -2, -3, -4)

### **Implementation Rules**

```csharp
// Rule 1: Apply null termination before padding
// (Handled by individual serialization methods)

// Rule 2: Pad nulls to 8-byte boundaries
K3CSharp.NullValue => PadTo8ByteBoundary(SerializeNullData()),

// Rule 3: Element-wise 8-byte padding for mixed structures
K3CSharp.VectorValue nestedList => 
    GetVectorType(nestedList) <= -1 && GetVectorType(nestedList) >= -4 
    ? PadTo8ByteBoundary(SerializeListData(nestedList)) 
    : SerializeListData(nestedList),

// Rule 4: Simple vectors padded as whole units
IntegerValue, FloatValue, CharacterValue, SymbolValue => PadTo8ByteBoundary(SerializeXData())
```

### **Padding Algorithm**
```csharp
private byte[] PadTo8ByteBoundary(byte[] data)
{
    int currentLength = data.Length;
    int paddingNeeded = (8 - (currentLength % 8)) % 8;
    if (paddingNeeded == 0) return data;
    byte[] padded = new byte[currentLength + paddingNeeded];
    Array.Copy(data, 0, padded, 0, currentLength);
    return padded;
}
```

## Deserialization Implementation Guide

### Reading Process
1. **Read Message Header:** Extract length and verify type
2. **Read Vector Header:** Get vector type and element count
3. **Determine Serialization Strategy:**
   - Homogeneous vector → Use raw data reading
   - Mixed list/dictionary → Use element-wise parsing with alignment handling
4. **Parse Elements:** Apply appropriate type handlers
5. **Handle Alignment:** Skip padding bytes as needed

### Key Challenges
1. **Alignment Detection:** Determine when to apply 4-byte vs 8-byte alignment
2. **Padding Calculation:** Correctly calculate and skip padding bytes during deserialization
3. **Type Boundary Issues:** Ensure type IDs are read from correct positions, not padding bytes

### Current Issue
The serializer uses element-wise 8-byte alignment for mixed lists containing dictionaries, but the deserializer (`DeserializeList`) assumes simple sequential reading without alignment handling. This causes type IDs to be read from padding bytes, resulting in corrupted type values like 97, 25185, etc.

### Solution
Update `DeserializeList` to handle 8-byte alignment by:
1. Reading element count
2. For each element: calculate and skip pre-padding, read element, skip post-padding
3. Only apply to mixed lists (vectorType == 0) containing complex structures

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
