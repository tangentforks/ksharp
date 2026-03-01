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
        return new VectorValue(charElements, -3); // Character vector
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
        try
        {
            string path;
            string separator = "\n"; // Default line separator
            
            // Handle operand types: symbol, character vector, or list with path and separator
            if (operand is SymbolValue sym)
            {
                path = sym.Value;
            }
            else if (operand is CharacterValue charVal)
            {
                path = charVal.Value.ToString();
            }
            else if (operand is VectorValue vec && vec.Elements.Count >= 2)
            {
                // First element is path, second is separator
                var pathElement = vec.Elements[0];
                var separatorElement = vec.Elements[1];
                
                path = pathElement switch
                {
                    SymbolValue s => s.Value,
                    CharacterValue c => c.Value.ToString(),
                    _ => throw new Exception("0: path must be symbol or character vector")
                };
                
                separator = separatorElement switch
                {
                    SymbolValue s => s.Value,
                    CharacterValue c => c.Value.ToString(),
                    VectorValue sepVec when sepVec.Elements.Count > 0 => string.Join("", sepVec.Elements.OfType<CharacterValue>().Select(cv => cv.Value)),
                    _ => throw new Exception("0: separator must be symbol or character vector")
                };
            }
            else
            {
                throw new Exception("0: argument must be symbol, character vector, or list with path and separator");
            }
            
            // Handle standard input
            if (string.IsNullOrEmpty(path))
            {
                return ReadFromStandardInput();
            }
            
            // Read file with UTF-8 encoding
            using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);
            
            var lines = new List<K3Value>();
            string? line;
            
            while ((line = streamReader.ReadLine()) != null)
            {
                // Convert line to character vector (single CharacterValue containing entire line)
                var charVector = new List<K3Value> { new CharacterValue(line) };
                lines.Add(new VectorValue(charVector, -3)); // -3 indicates character vector type
            }
            
            return new VectorValue(lines, 0); // 0 indicates generic list (list of character vectors)
        }
        catch (Exception ex)
        {
            // Convert exceptions to K signals
            throw new Exception($"0: {ex.Message}");
        }
    }
    
    private K3Value ReadFromStandardInput()
    {
        var lines = new List<K3Value>();
        
        try
        {
            while (true)
            {
                string? line = Console.ReadLine();
                if (line == null) break; // EOF reached
                
                // Convert line to character vector (single CharacterValue containing entire line)
                var charVector = new List<K3Value> { new CharacterValue(line) };
                lines.Add(new VectorValue(charVector, -3)); // -3 indicates character vector type
            }
        }
        catch (OperationCanceledException)
        {
            // Ctrl-C pressed - terminate gracefully
        }
        
        return new VectorValue(lines, 0); // 0 indicates generic list (list of character vectors)
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
        try
        {
            string path;
            
            // Handle left argument (path): symbol or character vector
            path = left switch
            {
                SymbolValue sym => sym.Value,
                CharacterValue charVal => charVal.Value.ToString(),
                _ => throw new Exception("0: output path must be symbol or character vector")
            };
            
            // Handle standard output
            if (string.IsNullOrEmpty(path))
            {
                WriteToStandardOutput(right);
                return new NullValue(); // Return null as specified
            }
            
            // Write to file with UTF-8 encoding and platform-specific line endings
            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            using var streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
            
            string lineEnding = Environment.NewLine; // Platform-specific line endings
            
            // Handle right argument (data to write)
            if (right is VectorValue vec)
            {
                // Check if this is a list of lists (structured data with separator)
                if (vec.Elements.Count > 0 && vec.Elements[0] is VectorValue)
                {
                    // This is structured data - write as fields with separators
                    WriteStructuredData(streamWriter, vec, lineEnding);
                }
                else
                {
                    // This is a simple list - write each item as a line
                    WriteSimpleList(streamWriter, vec, lineEnding);
                }
            }
            else
            {
                // Single item - write it and add line ending
                streamWriter.Write(right.ToString());
                streamWriter.Write(lineEnding);
            }
            
            streamWriter.Flush();
            return new NullValue(); // Return null as specified
        }
        catch (Exception ex)
        {
            // Convert exceptions to K signals
            throw new Exception($"0: {ex.Message}");
        }
    }
    
    private void WriteStructuredData(TextWriter writer, VectorValue data, string lineEnding)
    {
        // Default separator is comma (CSV)
        string separator = ",";
        
        // Check if first element is a list with separator specification
        if (data.Elements.Count >= 2 && data.Elements[0] is VectorValue firstVec && firstVec.Elements.Count == 1)
        {
            // Check if this is a separator specification (path, separator) pattern
            // For now, assume comma separator for CSV
            separator = ",";
        }
        
        foreach (var element in data.Elements)
        {
            if (element is VectorValue lineVec)
            {
                // Write each field in the line
                var fields = new List<string>();
                foreach (var field in lineVec.Elements)
                {
                    string fieldText = field.ToString();
                    
                    // Apply CSV escaping if separator is comma
                    if (separator == ",")
                    {
                        fieldText = EscapeCsvField(fieldText);
                    }
                    
                    fields.Add(fieldText);
                }
                
                writer.WriteLine(string.Join(separator, fields));
            }
            else
            {
                // Single item in line
                writer.WriteLine(element.ToString());
            }
        }
    }
    
    private void WriteSimpleList(TextWriter writer, VectorValue data, string lineEnding)
    {
        foreach (var item in data.Elements)
        {
            writer.Write(item.ToString());
            writer.Write(lineEnding);
        }
    }
    
    private string EscapeCsvField(string field)
    {
        // RFC 4180 CSV escaping
        if (field.Contains("\"") || field.Contains(",") || field.Contains("\n") || field.Contains("\r"))
        {
            // Escape quotes by doubling them and wrap in quotes
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
    
    private void WriteToStandardOutput(K3Value data)
    {
        try
        {
            string lineEnding = Environment.NewLine;
            
            if (data is VectorValue vec)
            {
                // Check if this is a list of lists (structured data)
                if (vec.Elements.Count > 0 && vec.Elements[0] is VectorValue)
                {
                    WriteStructuredData(Console.Out, vec, lineEnding);
                }
                else
                {
                    WriteSimpleList(Console.Out, vec, lineEnding);
                }
            }
            else
            {
                Console.WriteLine(data.ToString());
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"0: stdout write failed: {ex.Message}");
        }
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
