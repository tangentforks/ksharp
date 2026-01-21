using System;
using System.Collections.Generic;

namespace K3CSharp
{
    public enum TokenType
    {
        INTEGER,
        LONG,
        FLOAT,
        CHARACTER,
        SYMBOL,
        IDENTIFIER,
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        PLUS,
        MINUS,
        MULTIPLY,
        DIVIDE,
        MIN,
        MAX,
        LESS,
        GREATER,
        EQUAL,
        POWER,
        MODULUS,
        NEGATE,
        JOIN,
        ASSIGNMENT,
        SEMICOLON,
        NEWLINE,
        QUOTE,
        BACKTICK,
        EOF,
        UNKNOWN
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public int Position { get; }

        public Token(TokenType type, string lexeme, int position)
        {
            Type = type;
            Lexeme = lexeme;
            Position = position;
        }

        public override string ToString()
        {
            return $"{Type}({Lexeme})";
        }
    }
}
