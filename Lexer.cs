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
                    tokens.Add(new Token(TokenType.UNDERSCORE, "_", position));
                    Advance();
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
                    tokens.Add(new Token(TokenType.ASSIGNMENT, ":", position));
                    Advance();
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
