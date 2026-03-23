using System;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Atomic token parsing for LRS parser
    /// Handles literals, identifiers, and basic atomic values without PrimaryParser dependency
    /// Provides verb-agnostic atomic token creation
    /// </summary>
    public class LRSAtomicParser
    {
        /// <summary>
        /// Parse atomic token and create appropriate AST node
        /// </summary>
        /// <param name="token">Token to parse</param>
        /// <returns>AST node representing atomic value</returns>
        public static ASTNode ParseAtomicToken(Token token)
        {
            return token.Type switch
            {
                TokenType.INTEGER => ParseInteger(token),
                TokenType.LONG => ParseLong(token),
                TokenType.FLOAT => ParseFloat(token),
                TokenType.CHARACTER => ParseCharacter(token),
                TokenType.CHARACTER_VECTOR => ParseString(token),
                TokenType.SYMBOL => ParseSymbol(token),
                TokenType.IDENTIFIER => ParseIdentifier(token),
                TokenType.NULL => ParseNull(token),
                _ => throw new Exception($"Unsupported atomic token type: {token.Type}({token.Lexeme})")
            };
        }

        /// <summary>
        /// Parse integer token with special value handling
        /// </summary>
        private static ASTNode ParseInteger(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Handle special integer values per K specification
            if (lexeme == "0I" || lexeme == "-0I")
                return ASTNode.MakeLiteral(new IntegerValue(lexeme));
            
            // Handle null integer literals (0N, -0N)
            if (lexeme == "0N" || lexeme == "-0N")
                return ASTNode.MakeLiteral(new IntegerValue(lexeme));
            
            if (int.TryParse(lexeme, out int intValue))
            {
                // Convert extreme values to special values per spec
                if (intValue >= 2147483647)
                    return ASTNode.MakeLiteral(new IntegerValue("0I"));
                return ASTNode.MakeLiteral(new IntegerValue(intValue));
            }
            
            throw new ArgumentException($"Invalid integer literal: {lexeme}");
        }

        /// <summary>
        /// Parse long integer token with bounds checking
        /// </summary>
        private static ASTNode ParseLong(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Parse with bounds checking
            var numberPart = lexeme.Substring(0, lexeme.Length - 1); // Remove 'j'
            double parsedValue = double.Parse(numberPart);
            
            if (parsedValue >= long.MaxValue)
                return ASTNode.MakeLiteral(new LongValue(long.MaxValue));
            else if (parsedValue <= -long.MaxValue)
                return ASTNode.MakeLiteral(new LongValue(-long.MaxValue));
            else
                return ASTNode.MakeLiteral(new LongValue(long.Parse(numberPart)));
        }

        /// <summary>
        /// Parse float token with special value handling
        /// </summary>
        private static ASTNode ParseFloat(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Handle special float values
            if (lexeme == "0i" || lexeme == "0n" || lexeme == "-0i")
                return ASTNode.MakeLiteral(new FloatValue(lexeme));
            
            return ASTNode.MakeLiteral(new FloatValue(double.Parse(lexeme)));
        }

        /// <summary>
        /// Parse character token with escape sequence handling
        /// </summary>
        private static ASTNode ParseCharacter(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Handle escape sequences
            if (lexeme.Length == 3 && lexeme[0] == '\'' && lexeme[2] == '\'')
            {
                char c = lexeme[1];
                var charValue = c switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '\\' => '\\',
                    '\'' => '\'',
                    _ => c
                };
                return ASTNode.MakeLiteral(new CharacterValue(charValue.ToString()));
            }
            
            throw new ArgumentException($"Invalid character literal: {lexeme}");
        }

        /// <summary>
        /// Parse string token (character vector)
        /// </summary>
        private static ASTNode ParseString(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Remove quotes and handle escape sequences
            if (lexeme.Length >= 2 && lexeme[0] == '"' && lexeme[^1] == '"')
            {
                var stringValue = lexeme.Substring(1, lexeme.Length - 2);
                return ASTNode.MakeLiteral(new SymbolValue(stringValue));
            }
            
            throw new ArgumentException($"Invalid string literal: {lexeme}");
        }

        /// <summary>
        /// Parse symbol token
        /// </summary>
        private static ASTNode ParseSymbol(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Remove backticks for symbol value
            var symbolValue = lexeme.Trim('`');
            return ASTNode.MakeLiteral(new SymbolValue(symbolValue));
        }

        /// <summary>
        /// Parse identifier token (variable)
        /// Uses verb-agnostic system function detection
        /// </summary>
        private static ASTNode ParseIdentifier(Token token)
        {
            var identifier = token.Lexeme;
            
            // Check if this identifier is a system function using verb-agnostic approach
            if (VerbQueryExtensions.IsSystemFunction(identifier))
            {
                // Create a system function node instead of a regular variable
                return ASTNode.MakeFunctionCall(ASTNode.MakeVariable(identifier), new List<ASTNode>());
            }
            
            return ASTNode.MakeVariable(identifier);
        }

        /// <summary>
        /// Parse null token
        /// </summary>
        private static ASTNode ParseNull(Token token)
        {
            return ASTNode.MakeLiteral(new NullValue());
        }

        /// <summary>
        /// Check if token type represents an atomic value
        /// </summary>
        public static bool IsAtomicToken(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.INTEGER or TokenType.LONG or TokenType.FLOAT or 
                TokenType.CHARACTER or TokenType.CHARACTER_VECTOR or 
                TokenType.SYMBOL or TokenType.IDENTIFIER or TokenType.NULL => true,
                _ => false
            };
        }

        /// <summary>
        /// Create AST node for operator symbols in parse trees
        /// </summary>
        public static ASTNode CreateOperatorNode(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.PLUS => ASTNode.MakeLiteral(new SymbolValue("+")),
                TokenType.MINUS => ASTNode.MakeLiteral(new SymbolValue("-")),
                TokenType.MULTIPLY => ASTNode.MakeLiteral(new SymbolValue("*")),
                TokenType.DIVIDE => ASTNode.MakeLiteral(new SymbolValue("%")),
                TokenType.POWER => ASTNode.MakeLiteral(new SymbolValue("^")),
                TokenType.MODULUS => ASTNode.MakeLiteral(new SymbolValue("!")),
                TokenType.JOIN => ASTNode.MakeLiteral(new SymbolValue(",")),
                TokenType.MATCH => ASTNode.MakeLiteral(new SymbolValue("~")),
                TokenType.NEGATE => ASTNode.MakeLiteral(new SymbolValue("~")),
                TokenType.DOLLAR => ASTNode.MakeLiteral(new SymbolValue("$")),
                TokenType.QUESTION => ASTNode.MakeLiteral(new SymbolValue("?")),
                TokenType.HASH => ASTNode.MakeLiteral(new SymbolValue("#")),
                TokenType.UNDERSCORE => ASTNode.MakeLiteral(new SymbolValue("_")),
                TokenType.GLOBAL_ASSIGNMENT => ASTNode.MakeLiteral(new SymbolValue("::")),
                TokenType.COLON => ASTNode.MakeLiteral(new SymbolValue(":")),
                TokenType.SEMICOLON => ASTNode.MakeLiteral(new SymbolValue(";")),
                
                // System verbs
                TokenType.CI => ASTNode.MakeLiteral(new SymbolValue("_ci")),
                TokenType.IC => ASTNode.MakeLiteral(new SymbolValue("_ic")),
                TokenType.SV => ASTNode.MakeLiteral(new SymbolValue("_sv")),
                TokenType.SS => ASTNode.MakeLiteral(new SymbolValue("_ss")),
                TokenType.SM => ASTNode.MakeLiteral(new SymbolValue("_sm")),
                TokenType.DRAW => ASTNode.MakeLiteral(new SymbolValue("_draw")),
                TokenType.GETENV => ASTNode.MakeLiteral(new SymbolValue("_getenv")),
                TokenType.SIZE => ASTNode.MakeLiteral(new SymbolValue("_size")),
                TokenType.DIRECTORY => ASTNode.MakeLiteral(new SymbolValue("_d")),
                TokenType.TIME => ASTNode.MakeLiteral(new SymbolValue("_t")),
                TokenType.EVAL => ASTNode.MakeLiteral(new SymbolValue("_eval")),
                TokenType.PARSE => ASTNode.MakeLiteral(new SymbolValue("_parse")),
                
                // Delimiters for parse tree representation
                TokenType.LEFT_PAREN => ASTNode.MakeLiteral(new SymbolValue("(")),
                TokenType.RIGHT_PAREN => ASTNode.MakeLiteral(new SymbolValue(")")),
                TokenType.LEFT_BRACKET => ASTNode.MakeLiteral(new SymbolValue("[")),
                TokenType.RIGHT_BRACKET => ASTNode.MakeLiteral(new SymbolValue("]")),
                TokenType.LEFT_BRACE => ASTNode.MakeLiteral(new SymbolValue("{")),
                TokenType.RIGHT_BRACE => ASTNode.MakeLiteral(new SymbolValue("}")),
                
                _ => throw new Exception($"Unsupported operator token type: {tokenType}")
            };
        }
    }
}
