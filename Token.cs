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
        COLON,          // : conditional evaluation / assignment
        SEMICOLON,
        NEWLINE,
        QUOTE,
        BACKTICK,
        HASH,           // # count operator
        UNDERSCORE,     // _ floor operator
        QUESTION,       // ? unique operator
        APPLY,          // @ apply operator
        ATOM,           // @ atom operator (unary)
        DOT_APPLY,      // . dot-apply operator (binary)
        MAKE,           // . make operator (unary)
        ADVERB_SLASH,   // / adverb
        ADVERB_BACKSLASH, // \ adverb  
        ADVERB_TICK,    // ' adverb
        ADVERB_SLASH_COLON,   // /: adverb (each-right)
        ADVERB_BACKSLASH_COLON, // \: adverb (each-left)
        ADVERB_TICK_COLON,    // ': adverb (each-prior)
        NULL,           // _n null value
        TYPE,           // 4: type operator
        STRING_REPRESENTATION, // :: string representation operator
        GLOBAL_ASSIGNMENT, // :: global assignment operator
        DOLLAR,          // $ format/form operator
        
        // Mathematical floating point operations
        LOG,            // _log logarithm
        EXP,            // _exp exponential
        ABS,            // _abs absolute value
        SQR,            // _sqr square
        SQRT,           // _sqrt square root
        FLOOR_MATH,     // _floor mathematical floor (always returns float)
        SIN,            // _sin sine
        COS,            // _cos cosine
        TAN,            // _tan tangent
        ASIN,           // _asin arcsine
        ACOS,           // _acos arccosine
        ATAN,           // _atan arctangent
        SINH,           // _sinh hyperbolic sine
        COSH,           // _cosh hyperbolic cosine
        TANH,           // _tanh hyperbolic tangent
        
        // Linear algebra operations
        DOT,            // _dot dot product
        MUL,            // _mul matrix multiplication
        INV,            // _inv matrix inverse
        
        // System functions (missing from current implementation)
        TIME,           // _t current time
        DRAW,           // _draw random number generation
        IN,             // _in search/find function
        BIN,            // _bin binary search
        BINL,           // _binl binary search each-left
        LSQ,            // _lsq least squares
        LIN,            // _lin list intersection
        GTIME,          // _gtime GMT time conversion
        LTIME,          // _ltime local time conversion
        VS,             // _vs database function
        SV,             // _sv database function
        SS,             // _ss database function
        SM,             // _sm database function
        CI,             // _ci database function
        IC,             // _ic database function
        DV,             // _dv delete by value
        DI,             // _di delete by index
        DIRECTORY,      // _d directory operations
        DO,             // _do control flow
        WHILE,          // _while control flow
        IF_FUNC,        // _if control flow
        GOTO,           // _goto control flow
        EXIT,           // _exit control flow
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
