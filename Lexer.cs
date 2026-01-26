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
                    if (c == '\n')
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
                        if (position + 3 < input.Length && input[position + 3] == 'L')
                        {
                            // -0IL
                            tokens.Add(new Token(TokenType.LONG, "-0IL", position));
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
                    else
                    {
                        tokens.Add(new Token(TokenType.ADVERB_SLASH, "/", position));
                        Advance();
                    }
                }
                else if (c == '\\')
                {
                    tokens.Add(new Token(TokenType.ADVERB_BACKSLASH, "\\", position));
                    Advance();
                }
                else if (c == '\'')
                {
                    tokens.Add(new Token(TokenType.ADVERB_TICK, "'", position));
                    Advance();
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
                else if (c == '_')
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
                    else
                    {
                        tokens.Add(new Token(TokenType.ASSIGNMENT, ":", position));
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
                else if (char.IsLetter(c))
                {
                    tokens.Add(ReadIdentifier());
                }
                else
                {
                    tokens.Add(new Token(TokenType.UNKNOWN, c.ToString(), position));
                    Advance();
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
                value += currentChar;
                Advance();
            }
            
            if (currentChar == '"')
            {
                Advance(); // Skip closing quote
                
                // Check string length to determine token type
                if (value.Length == 1)
                {
                    return new Token(TokenType.CHARACTER, value, start);
                }
                else
                {
                    return new Token(TokenType.CHARACTER_VECTOR, value, start);
                }
            }
            
            throw new Exception("Unterminated string literal");
        }

        private Token ReadSymbol()
        {
            int start = position;
            Advance(); // Skip backtick
            
            // Check if next character is a quote - this should have precedence per spec
            if (currentChar == '"')
            {
                // Read the quoted string as part of the symbol
                var stringValue = ReadString();
                // The ReadString method already includes the quotes, so use that as the symbol value
                return new Token(TokenType.SYMBOL, stringValue.Lexeme, start);
            }
            
            string value = "";
            while (currentChar != '\0' && (char.IsLetterOrDigit(currentChar) || currentChar == '_' || currentChar == '.'))
            {
                value += currentChar;
                Advance();
            }
            
            // If we found a closing backtick, skip it
            if (currentChar == '`')
            {
                Advance();
            }
            
            return new Token(TokenType.SYMBOL, value, start);
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
                if (position + 3 < input.Length && input[position + 3] == 'L')
                {
                    // -0IL
                    position += 4;
                    var token = new Token(TokenType.LONG, "-0IL", start);
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
                    if (position + 2 < input.Length && input[position + 2] == 'L')
                    {
                        // Long special values: 0IL, 0NL, -0IL
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
            
            // Check for L suffix for long integers
            if (currentChar == 'L' && !hasDecimal && !hasExponent)
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

        private Token ReadIdentifier()
        {
            int start = position;
            string identifier = "";
            
            while (char.IsLetterOrDigit(currentChar) || currentChar == '_')
            {
                identifier += currentChar;
                Advance();
            }
            
            return new Token(TokenType.IDENTIFIER, identifier, start);
        }

        private Token ReadMathOperation()
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
            Token result = opName switch
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
