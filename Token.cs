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
        CHARACTER_VECTOR,
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
        HASH,           // # count operator
        UNDERSCORE,     // _ floor operator
        QUESTION,       // ? unique operator
        APPLY,          // @ apply operator
        DOT_APPLY,      // . dot-apply operator
        ADVERB_SLASH,   // / adverb
        ADVERB_BACKSLASH, // \ adverb  
        ADVERB_TICK,    // ' adverb
        NULL,           // _n null value
        TYPE,           // 4: type operator
        GLOBAL_ASSIGNMENT, // :: global assignment operator
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
