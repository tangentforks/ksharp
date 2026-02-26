using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp;

public partial class Evaluator
{
    // I/O Verbs - digit-colon operators (0: through 9:)
    // Based on io_verbs.txt speclet
    
    private K3Value IoVerb(K3Value left, K3Value right)
    {
        // Extract the digit from the operator (stored in the AST node value)
        // This will be called from the main evaluator with the appropriate digit
        throw new NotImplementedException($"I/O verb {left}:{right} not yet implemented");
    }
    
    // Monadic I/O verbs (single argument)
    private K3Value IoVerbMonadic(K3Value operand, int digit)
    {
        return digit switch
        {
            0 => ReadText(operand),           // READ TEXT
            1 => ReadMemoryMappedKData(operand), // READ MEMORY MAPPED K DATA
            2 => ReadRawKData(operand),       // READ RAW K DATA
            3 => OpenClosePort(operand),       // OPEN/CLOSE PORT
            4 => GetTypeCode(operand),         // TYPE (existing implementation)
            5 => StringRepresentation(operand), // STRING REPRESENTATION (existing implementation)
            6 => ReadBytes(operand),          // READ BYTES
            7 => throw new NotImplementedException("7: reserved for future use (direct memory access and P/Invoke)"),
            8 => throw new NotImplementedException("8: reserved for future use (shared memory, fork and create process)"),
            9 => throw new NotImplementedException("9: reserved for future use (threads and fibers)"),
            _ => throw new ArgumentException($"Invalid I/O verb digit: {digit}")
        };
    }
    
    // Dyadic I/O verbs (two arguments)
    private K3Value IoVerbDyadic(K3Value left, K3Value right, int digit)
    {
        return digit switch
        {
            0 => WriteText(left, right),          // WRITE TEXT
            1 => WriteMemoryMappedKData(left, right), // WRITE MEMORY MAPPED K DATA
            2 => WriteData(left, right),          // WRITE DATA (and FFI dynamic load)
            3 => IpcGet(left, right),            // IPC GET
            4 => IpcSet(left, right),            // IPC SET
            5 => AppendData(left, right),          // APPEND DATA
            6 => WriteBytes(left, right),          // WRITE BYTES
            7 => throw new NotImplementedException("7: reserved for future use (direct memory access and P/Invoke)"),
            8 => throw new NotImplementedException("8: reserved for future use (shared memory, fork and create process)"),
            9 => throw new NotImplementedException("9: reserved for future use (threads and fibers)"),
            _ => throw new ArgumentException($"Invalid I/O verb digit: {digit}")
        };
    }
    
    // Existing implementations moved from Evaluator.cs
    private K3Value GetTypeCode(K3Value value)
    {
        if (value is IntegerValue)
            return new IntegerValue(1);
        if (value is LongValue)
            return new IntegerValue(64);
        if (value is FloatValue)
            return new IntegerValue(2);
        if (value is CharacterValue)
            return new IntegerValue(3);
        if (value is SymbolValue)
            return new IntegerValue(4);
        if (value is DictionaryValue)
            return new IntegerValue(5);
        if (value is NullValue)
            return new IntegerValue(6);
        if (value is VectorValue vector)
        {
            if (vector.Elements.Count == 0)
                return new IntegerValue(-1); // Empty vector (assume integer vector by default)
            if (vector.Elements.All(x => x is IntegerValue))
                return new IntegerValue(-1); // Integer vector
            if (vector.Elements.All(x => x is LongValue))
                return new IntegerValue(-64); // Long vector
            if (vector.Elements.All(x => x is FloatValue))
                return new IntegerValue(-2); // Float vector
            if (vector.Elements.All(x => x is CharacterValue))
                return new IntegerValue(-3); // Character vector
            if (vector.Elements.All(x => x is SymbolValue))
                return new IntegerValue(-4); // Symbol vector
            if (vector.Elements.All(x => x is DictionaryValue))
                return new IntegerValue(-5); // Dictionary vector
            if (vector.Elements.All(x => x is VectorValue))
                return new IntegerValue(0); // Nested vector (generic list)
        }
        
        return new IntegerValue(0); // Default to generic list
    }
    
    private K3Value StringRepresentation(K3Value value)
    {
        // 5: verb - produce string representation of argument with proper escaping
        string representation = ToStringWithEscaping(value);
        // Create character vector with individual characters
        var charElements = representation.Select(c => (K3Value)new CharacterValue(c.ToString())).ToList();
        return new VectorValue(charElements);
    }
    
    private string ToStringWithEscaping(K3Value value)
    {
        if (value is CharacterValue charVal)
        {
            // For character values, escape the result of ToString() which already includes quotes
            return EscapeString(charVal.ToString());
        }
        else
        {
            // For other types, convert to string and escape
            return EscapeString(value.ToString());
        }
    }
    
    private string EscapeString(string input)
    {
        var result = new System.Text.StringBuilder();
        foreach (char c in input)
        {
            switch (c)
            {
                case '\n':
                    result.Append("\\n");
                    break;
                case '\t':
                    result.Append("\\t");
                    break;
                case '\r':
                    result.Append("\\r");
                    break;
                case '"':
                    result.Append("\\\"");
                    break;
                case '\\':
                    result.Append("\\\\");
                    break;
                default:
                    result.Append(c);
                    break;
            }
        }
        return result.ToString();
    }
    
    // Stub implementations for new I/O verbs
    private K3Value ReadText(K3Value operand)
    {
        throw new NotImplementedException("0: READ TEXT not yet implemented");
    }
    
    private K3Value ReadMemoryMappedKData(K3Value operand)
    {
        throw new NotImplementedException("1: READ MEMORY MAPPED K DATA not yet implemented");
    }
    
    private K3Value ReadRawKData(K3Value operand)
    {
        throw new NotImplementedException("2: READ RAW K DATA not yet implemented");
    }
    
    private K3Value OpenClosePort(K3Value operand)
    {
        throw new NotImplementedException("3: OPEN/CLOSE PORT not yet implemented");
    }
    
    private K3Value ReadBytes(K3Value operand)
    {
        throw new NotImplementedException("6: READ BYTES not yet implemented");
    }
    
    private K3Value WriteText(K3Value left, K3Value right)
    {
        throw new NotImplementedException("0: WRITE TEXT not yet implemented");
    }
    
    private K3Value WriteMemoryMappedKData(K3Value left, K3Value right)
    {
        throw new NotImplementedException("1: WRITE MEMORY MAPPED K DATA not yet implemented");
    }
    
    private K3Value WriteData(K3Value left, K3Value right)
    {
        throw new NotImplementedException("2: WRITE DATA not yet implemented");
    }
    
    private K3Value IpcGet(K3Value left, K3Value right)
    {
        throw new NotImplementedException("3: IPC GET not yet implemented");
    }
    
    private K3Value IpcSet(K3Value left, K3Value right)
    {
        throw new NotImplementedException("4: IPC SET not yet implemented");
    }
    
    private K3Value AppendData(K3Value left, K3Value right)
    {
        throw new NotImplementedException("5: APPEND DATA not yet implemented");
    }
    
    private K3Value WriteBytes(K3Value left, K3Value right)
    {
        throw new NotImplementedException("6: WRITE BYTES not yet implemented");
    }
}
