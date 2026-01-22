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
                    tokens.Add(new Token(TokenType.MINUS, "-", position));
                    Advance();
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
                    tokens.Add(new Token(TokenType.ADVERB_SLASH, "/", position));
                    Advance();
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
                    // Check for _n null value
                    if (position + 1 < input.Length && input[position + 1] == 'n')
                    {
                        tokens.Add(new Token(TokenType.NULL, "_n", position));
                        Advance(); // Skip _
                        Advance(); // Skip n
                    }
                    else
                    {
                        tokens.Add(new Token(TokenType.UNDERSCORE, "_", position));
                        Advance();
                    }
                }
                else if (c == '?')
                {
                    tokens.Add(new Token(TokenType.QUESTION, "?", position));
                    Advance();
                }
                else if (c == '@')
                {
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
                return new Token(TokenType.CHARACTER, value, start);
            }
            
            throw new Exception("Unterminated string literal");
        }

        private Token ReadSymbol()
        {
            int start = position;
            Advance(); // Skip backtick
            
            string value = "";
            while (currentChar != '`' && currentChar != '\0')
            {
                value += currentChar;
                Advance();
            }
            
            if (value.Length > 0)
            {
                return new Token(TokenType.SYMBOL, value, start);
            }
            
            throw new Exception("Invalid symbol");
        }

        private Token ReadNumber()
        {
            int start = position;
            string number = "";
            bool hasDecimal = false;
            bool hasExponent = false;
            
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
            
            // Handle negative special values
            if (currentChar == '-' && position + 2 < input.Length && 
                input[position + 1] == '0' && 
                (input[position + 2] == 'I' || input[position + 2] == 'i'))
            {
                if (position + 3 < input.Length && input[position + 3] == 'L')
                {
                    // -0IL
                    position += 4;
                    return new Token(TokenType.LONG, "-0IL", start);
                }
                else
                {
                    // -0I or -0i
                    string special = input.Substring(position, 3);
                    position += 3;
                    if (input[position - 1] == 'i')
                        return new Token(TokenType.FLOAT, special, start);
                    else
                        return new Token(TokenType.INTEGER, special, start);
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
