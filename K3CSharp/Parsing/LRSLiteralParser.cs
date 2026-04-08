using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// LRS Literal Parser for atomic values (int, float, char, symbol, etc.)
    /// Verb-agnostic design using VerbRegistry for type detection
    /// </summary>
    public class LRSLiteralParser
    {
        private readonly List<Token> tokens;
        
        public LRSLiteralParser(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        
        /// <summary>
        /// Check if token can be parsed as literal
        /// </summary>
        public static bool IsLiteralToken(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.INTEGER or TokenType.LONG or TokenType.FLOAT or 
                TokenType.CHARACTER or TokenType.CHARACTER_VECTOR or
                TokenType.SYMBOL or TokenType.NULL => true,
                _ => false
            };
        }
        
        /// <summary>
        /// Parse atomic token into AST node
        /// </summary>
        public static ASTNode? ParseAtomicToken(Token token)
        {
            return token.Type switch
            {
                TokenType.INTEGER => ParseInteger(token),
                TokenType.LONG => ParseLong(token),
                TokenType.FLOAT => ParseFloat(token),
                TokenType.CHARACTER => ParseCharacter(token),
                TokenType.CHARACTER_VECTOR => ParseCharacterVector(token),
                TokenType.SYMBOL => ParseSymbol(token),
                TokenType.NULL => ParseNull(token),
                _ => null
            };
        }
        
        /// <summary>
        /// Parse integer token
        /// </summary>
        private static ASTNode ParseInteger(Token token)
        {
            if (int.TryParse(token.Lexeme, out int intValue))
            {
                return ASTNode.MakeLiteral(new IntegerValue(intValue));
            }
            else if (long.TryParse(token.Lexeme, out long longValue))
            {
                return ASTNode.MakeLiteral(new LongValue(longValue));
            }
            throw new Exception($"Invalid integer literal: {token.Lexeme}");
        }
        
        /// <summary>
        /// Parse long token
        /// </summary>
        private static ASTNode ParseLong(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Handle special long values per K specification
            if (lexeme == "0Ij")
                return ASTNode.MakeLiteral(new LongValue(long.MaxValue));
            if (lexeme == "-0Ij")
                return ASTNode.MakeLiteral(new LongValue(-long.MaxValue));
            if (lexeme == "0Nj")
                return ASTNode.MakeLiteral(new LongValue(long.MinValue));
            
            // Strip the 'j' suffix and parse
            var numberPart = lexeme.Substring(0, lexeme.Length - 1);
            if (long.TryParse(numberPart, out long longValue))
            {
                return ASTNode.MakeLiteral(new LongValue(longValue));
            }
            throw new Exception($"Invalid long literal: {token.Lexeme}");
        }
        
        /// <summary>
        /// Parse float token
        /// </summary>
        private static ASTNode ParseFloat(Token token)
        {
            if (double.TryParse(token.Lexeme, out double floatValue))
            {
                return ASTNode.MakeLiteral(new FloatValue(floatValue));
            }
            throw new Exception($"Invalid float literal: {token.Lexeme}");
        }
        
        /// <summary>
        /// Parse character token
        /// </summary>
        private static ASTNode ParseCharacter(Token token)
        {
            if (token.Lexeme.Length >= 1)
            {
                return ASTNode.MakeLiteral(new CharacterValue(token.Lexeme));
            }
            throw new Exception($"Invalid character literal: {token.Lexeme}");
        }
        
        /// <summary>
        /// Parse character vector token
        /// </summary>
        private static ASTNode ParseCharacterVector(Token token)
        {
            return ASTNode.MakeLiteral(new CharacterValue(token.Lexeme));
        }
        
        /// <summary>
        /// Parse symbol token
        /// </summary>
        private static ASTNode ParseSymbol(Token token)
        {
            return ASTNode.MakeLiteral(new SymbolValue(token.Lexeme));
        }
        
        /// <summary>
        /// Parse string token (removed - handled as character vector)
        /// </summary>
        // private static ASTNode ParseString(Token token)
        // {
        //     return ASTNode.MakeLiteral(new CharacterValue(token.Lexeme));
        // }
        
        /// <summary>
        /// Parse null token
        /// </summary>
        private static ASTNode ParseNull(Token token)
        {
            return ASTNode.MakeLiteral(new NullValue());
        }
        
        /// <summary>
        /// Get literal type from token (for verb-agnostic processing)
        /// </summary>
        public static string GetLiteralType(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.INTEGER => "integer",
                TokenType.LONG => "long",
                TokenType.FLOAT => "float",
                TokenType.CHARACTER => "character",
                TokenType.CHARACTER_VECTOR => "string",
                TokenType.SYMBOL => "symbol",
                TokenType.NULL => "null",
                _ => "unknown"
            };
        }
    }
}
