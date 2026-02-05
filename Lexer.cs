using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public class Lexer
    {
        private string input;
        private int position;
        private char currentChar => position < input.Length ? input[position] : '\0';

        public Lexer(string input)
        {
            this.input = input;
            position = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (position < input.Length)
            {
                char c = currentChar;

                if (char.IsWhiteSpace(c))
                {
                    if (c == '\n' && tokens.Count > 0)  // Only add NEWLINE if we have previous tokens
                    {
                        tokens.Add(new Token(TokenType.NEWLINE, "\n", position));
                    }
                    Advance();
                    SkipWhitespace();
                }
                else if (c == '(')
                {
                    tokens.Add(new Token(TokenType.LEFT_PAREN, "(", position));
                    Advance();
                }
                else if (c == ')')
                {
                    tokens.Add(new Token(TokenType.RIGHT_PAREN, ")", position));
                    Advance();
                }
                else if (c == '{')
                {
                    tokens.Add(new Token(TokenType.LEFT_BRACE, "{", position));
                    Advance();
                }
                else if (c == '}')
                {
                    tokens.Add(new Token(TokenType.RIGHT_BRACE, "}", position));
                    Advance();
                }
                else if (c == '`')
                {
                    var symbolToken = ReadSymbol();
                    tokens.Add(symbolToken);
                }
                else if (c == '[')
                {
                    tokens.Add(new Token(TokenType.LEFT_BRACKET, "[", position));
                    Advance();
                }
                else if (c == ']')
                {
                    tokens.Add(new Token(TokenType.RIGHT_BRACKET, "]", position));
                    Advance();
                }
                else if (c == '+')
                {
                    tokens.Add(new Token(TokenType.PLUS, "+", position));
                    Advance();
                }
                else if (c == '-')
                {
                    // Check if this is a negative special value
                    if (position + 2 < input.Length && 
                        input[position + 1] == '0' && 
                        (input[position + 2] == 'I' || input[position + 2] == 'i'))
                    {
                        if (position + 3 < input.Length && input[position + 3] == 'j')
                        {
                            // -0Ij
                            tokens.Add(new Token(TokenType.LONG, "-0Ij", position));
                            position += 4;
                        }
                        else
                        {
                            // -0I or -0i
                            string special = input.Substring(position, 3);
                            if (input[position + 2] == 'i')
                                tokens.Add(new Token(TokenType.FLOAT, special, position));
                            else
                                tokens.Add(new Token(TokenType.INTEGER, special, position));
                            position += 3;
                        }
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.MINUS, "-", position));
                        Advance();
                    }
                }
                else if (c == '*')
                {
                    tokens.Add(new Token(TokenType.MULTIPLY, "*", position));
                    Advance();
                }
                else if (c == '%')
                {
                    tokens.Add(new Token(TokenType.DIVIDE, "%", position));
                    Advance();
                }
                else if (c == '/')
                {
                    // Check if this is a comment (// or / at start of line or after space)
                    if (position + 1 < input.Length && input[position + 1] == '/')
                    {
                        // // comment - skip to end of line
                        position += 2; // Skip both slashes
                        while (position < input.Length && currentChar != '\n')
                        {
                            Advance();
                        }
                    }
                    else if (position == 0 || (position > 0 && char.IsWhiteSpace(input[position - 1])))
                    {
                        // / comment at start of line or after space - skip to end of line
                        while (position < input.Length && currentChar != '\n')
                        {
                            Advance();
                        }
                    }
                    else if (position + 1 < input.Length && input[position + 1] == ':')
                    {
                        // /: adverb (each-right)
                        tokens.Add(new Token(TokenType.ADVERB_SLASH_COLON, "/:", position));
                        Advance(); // Skip /
                        Advance(); // Skip :
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.ADVERB_SLASH, "/", position));
                        Advance();
                    }
                }
                else if (c == '\\')
                {
                    if (position + 1 < input.Length && input[position + 1] == ':')
                    {
                        // \: adverb (each-left)
                        tokens.Add(new Token(TokenType.ADVERB_BACKSLASH_COLON, "\\:", position));
                        Advance(); // Skip \
                        Advance(); // Skip :
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.ADVERB_BACKSLASH, "\\", position));
                        Advance();
                    }
                }
                else if (c == '\'')
                {
                    if (position + 1 < input.Length && input[position + 1] == ':')
                    {
                        // ': adverb (each-prior)
                        tokens.Add(new Token(TokenType.ADVERB_TICK_COLON, "':", position));
                        Advance(); // Skip '
                        Advance(); // Skip :
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.ADVERB_TICK, "'", position));
                        Advance();
                    }
                }
                else if (c == '&')
                {
                    tokens.Add(new Token(TokenType.MIN, "&", position));
                    Advance();
                }
                else if (c == '|')
                {
                    tokens.Add(new Token(TokenType.MAX, "|", position));
                    Advance();
                }
                else if (c == '<')
                {
                    tokens.Add(new Token(TokenType.LESS, "<", position));
                    Advance();
                }
                else if (c == '>')
                {
                    tokens.Add(new Token(TokenType.GREATER, ">", position));
                    Advance();
                }
                else if (c == '=')
                {
                    tokens.Add(new Token(TokenType.EQUAL, "=", position));
                    Advance();
                }
                else if (c == '^')
                {
                    tokens.Add(new Token(TokenType.POWER, "^", position));
                    Advance();
                }
                else if (c == '!')
                {
                    tokens.Add(new Token(TokenType.MODULUS, "!", position));
                    Advance();
                }
                else if (c == '~')
                {
                    tokens.Add(new Token(TokenType.NEGATE, "~", position));
                    Advance();
                }
                else if (c == ',')
                {
                    tokens.Add(new Token(TokenType.JOIN, ",", position));
                    Advance();
                }
                else if (c == '#')
                {
                    tokens.Add(new Token(TokenType.HASH, "#", position));
                    Advance();
                }
                else if (char.IsLetter(c))
                {
                    tokens.Add(ReadIdentifier());
                }
                else if (c == '_')
                {
                    // Check if underscore is part of a name (identifier or symbol)
                    // Look ahead to see if this could be part of a name
                    if (position > 0 && 
                        (char.IsLetterOrDigit(input[position - 1]) || input[position - 1] == '_') &&
                        position + 1 < input.Length && 
                        (char.IsLetterOrDigit(input[position + 1]) || input[position + 1] == '_'))
                    {
                        // Underscore is part of an identifier
                        tokens.Add(ReadIdentifier());
                    }
                    else
                    {
                        // Check for _n null value first
                        if (position + 1 < input.Length && input[position + 1] == 'n')
                        {
                            tokens.Add(new Token(TokenType.NULL, "_n", position));
                            Advance(); // Skip _
                            Advance(); // Skip n
                        }
                        else
                        {
                            // Check for mathematical operations
                            var mathOp = ReadMathOperation();
                            if (mathOp != null)
                            {
                                tokens.Add(mathOp);
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.UNDERSCORE, "_", position));
                                Advance();
                            }
                        }
                    }
                }
                else if (c == '?')
                {
                    tokens.Add(new Token(TokenType.QUESTION, "?", position));
                    Advance();
                }
                else if (c == '@')
                {
                    // Let parser disambiguate between unary ATOM and binary APPLY
                    tokens.Add(new Token(TokenType.APPLY, "@", position));
                    Advance();
                }
                else if (c == '$')
                {
                    tokens.Add(new Token(TokenType.DOLLAR, "$", position));
                    Advance();
                }
                else if (c == '.')
                {
                    tokens.Add(new Token(TokenType.DOT_APPLY, ".", position));
                    Advance();
                }
                else if (c == ':')
                {
                    // Check for 4: type operator
                    if (position > 0 && input[position - 1] == '4')
                    {
                        // Replace the previous token with TYPE operator
                        tokens.RemoveAt(tokens.Count - 1);
                        tokens.Add(new Token(TokenType.TYPE, "4:", position - 1));
                        Advance();
                    }
                    // Check for 5: string representation operator
                    else if (position > 0 && input[position - 1] == '5')
                    {
                        // Replace the previous token with STRING_REPRESENTATION operator
                        tokens.RemoveAt(tokens.Count - 1);
                        tokens.Add(new Token(TokenType.STRING_REPRESENTATION, "5:", position - 1));
                        Advance();
                    }
                    // Check for :: global assignment operator
                    else if (position + 1 < input.Length && input[position + 1] == ':')
                    {
                        tokens.Add(new Token(TokenType.GLOBAL_ASSIGNMENT, "::", position));
                        Advance();
                        Advance();
                    }
                    // Check for modified assignment operators (e.g., +:, -:, *:, etc.)
                    else if (position > 0 && IsOperatorChar(input[position - 1]) && input[position] == ':')
                    {
                        // Replace the previous token with modified assignment operator
                        var opChar = input[position - 1];
                        var opToken = tokens[tokens.Count - 1];
                        tokens.RemoveAt(tokens.Count - 1);
                        
                        // Create modified assignment token
                        var modifiedOp = $"{opChar}:";
                        tokens.Add(new Token(TokenType.ASSIGNMENT, modifiedOp, position - 1));
                        Advance();
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.COLON, ":", position));
                        Advance();
                    }
                }
                else if (c == ';')
                {
                    tokens.Add(new Token(TokenType.SEMICOLON, ";", position));
                    Advance();
                }
                else if (c == '"')
                {
                    tokens.Add(ReadString());
                }
                else if (c == '`')
                {
                    tokens.Add(ReadSymbol());
                }
                else if (char.IsDigit(c))
                {
                    tokens.Add(ReadNumber());
                }
                else
                {
                    throw new Exception($"Unexpected character: {c} at position {position}");
                }
            }

            tokens.Add(new Token(TokenType.EOF, "", position));
            return tokens;
        }

        private Token ReadString()
        {
            int start = position;
            Advance(); // Skip opening quote
            
            string value = "";
            while (currentChar != '"' && currentChar != '\0')
            {
                if (currentChar == '\\')
                {
                    Advance(); // Skip backslash
                    if (currentChar == '\0')
                    {
                        // End of input after backslash - treat backslash as literal
                        value += '\\';
                        break;
                    }
                    
                    // Handle escape sequences
                    switch (currentChar)
                    {
                        case '"':
                            value += '"';
                            break;
                        case '\\':
                            value += '\\';
                            break;
                        case 'n':
                            value += '\n';
                            break;
                        case 't':
                            value += '\t';
                            break;
                        case 'r':
                            value += '\r';
                            break;
                        default:
                            // Unknown escape sequence - treat backslash and character literally
                            value += '\\' + currentChar;
                            break;
                    }
                    Advance();
                }
                else
                {
                    value += currentChar;
                    Advance();
                }
            }
            
            if (currentChar == '"')
            {
                Advance(); // Skip closing quote
            }
            
            // Determine if this is a single character or character vector
            if (value.Length == 1)
            {
                return new Token(TokenType.CHARACTER, value, start);
            }
            else
            {
                return new Token(TokenType.CHARACTER_VECTOR, value, start);
            }
        }
        
        private Token ReadSymbol()
        {
            int start = position;
            Advance(); // Skip backtick
            
            // Handle quoted symbols
            if (currentChar == '"')
            {
                Advance(); // Skip opening quote
                string quotedValue = "";
                while (currentChar != '"' && currentChar != '\0')
                {
                    quotedValue += currentChar;
                    Advance();
                }
                if (currentChar == '"')
                {
                    Advance(); // Skip closing quote
                }
                return new Token(TokenType.SYMBOL, quotedValue, start);
            }
            
            string value = "";
            while (currentChar != '\0' && (char.IsLetterOrDigit(currentChar) || currentChar == '_' || currentChar == '.'))
            {
                value += currentChar;
                Advance();
            }
            
            return new Token(TokenType.SYMBOL, value, start);
        }
        
        private Token ReadIdentifier()
        {
            int start = position;
            string value = "";
            
            while (currentChar != '\0' && (char.IsLetterOrDigit(currentChar) || currentChar == '_'))
            {
                value += currentChar;
                Advance();
            }
            
            // Check for control flow keywords
            switch (value)
            {
                case "do":
                    return new Token(TokenType.DO, value, start);
                case "if":
                    return new Token(TokenType.IF_FUNC, value, start);
                case "while":
                    return new Token(TokenType.WHILE, value, start);
                default:
                    return new Token(TokenType.IDENTIFIER, value, start);
            }
        }

        private bool IsOperatorChar(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '^' || c == '!' || 
                   c == '<' || c == '>' || c == '=' || c == ',' || c == '&' || c == '|' || c == '#' || 
                   c == '_' || c == '?' || c == '$' || c == '@';
        }

        private Token ReadNumber()
        {
            int start = position;
            string number = "";
            bool hasDecimal = false;
            bool hasExponent = false;
            
            // Handle negative special values FIRST (before regular number parsing)
            if (currentChar == '-' && position + 2 < input.Length && 
                input[position + 1] == '0' && 
                (input[position + 2] == 'I' || input[position + 2] == 'i'))
            {
                if (position + 3 < input.Length && input[position + 3] == 'j')
                {
                    // -0Ij
                    position += 4;
                    var token = new Token(TokenType.LONG, "-0Ij", start);
                    return token;
                }
                else
                {
                    // -0I or -0i
                    string special = input.Substring(position, 3);
                    position += 3;
                    if (input[position - 1] == 'i')
                    {
                        var token = new Token(TokenType.FLOAT, special, start);
                        return token;
                    }
                    else
                    {
                        var token = new Token(TokenType.INTEGER, special, start);
                        return token;
                    }
                }
            }
            
            // Check for special values starting with 0
            if (currentChar == '0' && position + 1 < input.Length)
            {
                char nextChar = input[position + 1];
                if (nextChar == 'I' || nextChar == 'N' || nextChar == 'i' || nextChar == 'n')
                {
                    // Handle special values
                    if (position + 2 < input.Length && input[position + 2] == 'j')
                    {
                        // Long special values: 0Ij, 0Nj, -0Ij
                        string special = input.Substring(position, 3);
                        position += 3;
                        return new Token(TokenType.LONG, special, start);
                    }
                    else
                    {
                        // Integer or float special values: 0I, 0N, 0i, 0n
                        string special = input.Substring(position, 2);
                        position += 2;
                        if (nextChar == 'i' || nextChar == 'n')
                            return new Token(TokenType.FLOAT, special, start);
                        else
                            return new Token(TokenType.INTEGER, special, start);
                    }
                }
            }
            
            while (position < input.Length && (char.IsDigit(currentChar) || currentChar == '.' || currentChar == 'e' || currentChar == 'E'))
            {
                if (currentChar == '.')
                {
                    if (hasDecimal) break;
                    hasDecimal = true;
                }
                else if (currentChar == 'e' || currentChar == 'E')
                {
                    if (hasExponent) break;
                    hasExponent = true;
                    number += currentChar;
                    Advance();
                    if (currentChar == '+' || currentChar == '-')
                    {
                        number += currentChar;
                        Advance();
                    }
                    continue;
                }
                
                number += currentChar;
                Advance();
            }
            
            // Check for j suffix for long integers
            if (currentChar == 'j' && !hasDecimal && !hasExponent)
            {
                number += currentChar;
                Advance();
                return new Token(TokenType.LONG, number, start);
            }
            
            if (hasDecimal || hasExponent)
            {
                return new Token(TokenType.FLOAT, number, start);
            }
            
            return new Token(TokenType.INTEGER, number, start);
        }

        private Token? ReadMathOperation()
        {
            int start = position;
            
            if (currentChar != '_')
                return null;
                
            Advance(); // Skip _
            
            if (position >= input.Length)
            {
                position = start; // Reset position
                return null;
            }
            
            // Only proceed if next character is a letter (for valid math operations)
            if (!char.IsLetter(currentChar))
            {
                position = start; // Reset position
                return null;
            }
            
            string opName = "_" + currentChar;
            int charCount = 1;
            Advance();
            
            // Check for multi-character operations
            if (position < input.Length && char.IsLetter(currentChar))
            {
                while (position < input.Length && char.IsLetter(currentChar))
                {
                    opName += currentChar;
                    charCount++;
                    Advance();
                }
            }
            
            // Map operation names to tokens
            Token? result = opName switch
            {
                // Mathematical floating point operations
                "_log" => new Token(TokenType.LOG, opName, start),
                "_exp" => new Token(TokenType.EXP, opName, start),
                "_abs" => new Token(TokenType.ABS, opName, start),
                "_sqr" => new Token(TokenType.SQR, opName, start),
                "_sqrt" => new Token(TokenType.SQRT, opName, start),
                "_floor" => new Token(TokenType.FLOOR_MATH, opName, start),
                "_sin" => new Token(TokenType.SIN, opName, start),
                "_cos" => new Token(TokenType.COS, opName, start),
                "_tan" => new Token(TokenType.TAN, opName, start),
                "_asin" => new Token(TokenType.ASIN, opName, start),
                "_acos" => new Token(TokenType.ACOS, opName, start),
                "_atan" => new Token(TokenType.ATAN, opName, start),
                "_sinh" => new Token(TokenType.SINH, opName, start),
                "_cosh" => new Token(TokenType.COSH, opName, start),
                "_tanh" => new Token(TokenType.TANH, opName, start),
                
                // Linear algebra operations
                "_dot" => new Token(TokenType.DOT, opName, start),
                "_mul" => new Token(TokenType.MUL, opName, start),
                "_inv" => new Token(TokenType.INV, opName, start),
                
                // System functions (missing from current implementation)
                "_t" => new Token(TokenType.TIME, opName, start),
                "_draw" => new Token(TokenType.DRAW, opName, start),
                "_in" => new Token(TokenType.IN, opName, start),
                "_bin" => new Token(TokenType.BIN, opName, start),
                "_binl" => new Token(TokenType.BINL, opName, start),
                "_lsq" => new Token(TokenType.LSQ, opName, start),
                "_lin" => new Token(TokenType.LIN, opName, start),
                "_gtime" => new Token(TokenType.GTIME, opName, start),
                "_ltime" => new Token(TokenType.LTIME, opName, start),
                "_vs" => new Token(TokenType.VS, opName, start),
                "_sv" => new Token(TokenType.SV, opName, start),
                "_ss" => new Token(TokenType.SS, opName, start),
                "_ci" => new Token(TokenType.CI, opName, start),
                "_ic" => new Token(TokenType.IC, opName, start),
                "_d" => new Token(TokenType.DIRECTORY, opName, start),
                "_do" => new Token(TokenType.DO, opName, start),
                "_while" => new Token(TokenType.WHILE, opName, start),
                "_if" => new Token(TokenType.IF_FUNC, opName, start),
                "_goto" => new Token(TokenType.GOTO, opName, start),
                "_exit" => new Token(TokenType.EXIT, opName, start),
                
                // Control flow functions without underscores (K3 syntax)
                "do" => new Token(TokenType.DO, opName, start),
                "while" => new Token(TokenType.WHILE, opName, start),
                "if" => new Token(TokenType.IF_FUNC, opName, start),
                
                _ => null
            };
            
            // If no valid operation found, reset position
            if (result == null)
            {
                position = start;
            }
            
            return result;
        }

        private void SkipWhitespace()
        {
            while (position < input.Length && char.IsWhiteSpace(currentChar) && currentChar != '\n')
            {
                Advance();
            }
        }

        private void Advance()
        {
            position++;
        }
    }
}
