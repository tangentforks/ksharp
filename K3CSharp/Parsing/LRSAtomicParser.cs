using System;
using System.Linq;

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
            return ParseAtomicToken(token, null);
        }
        
        /// <summary>
        /// Parse atomic token and create appropriate AST node with parser context
        /// </summary>
        /// <param name="token">Token to parse</param>
        /// <param name="parser">Optional parser reference for variable tracking</param>
        /// <returns>AST node representing atomic value</returns>
        public static ASTNode ParseAtomicToken(Token token, LRSParser? parser)
        {
            return token.Type switch
            {
                TokenType.INTEGER => ParseInteger(token),
                TokenType.LONG => ParseLong(token),
                TokenType.FLOAT => ParseFloat(token),
                TokenType.CHARACTER => ParseCharacter(token),
                TokenType.CHARACTER_VECTOR => ParseString(token),
                TokenType.SYMBOL => ParseSymbol(token),
                TokenType.IDENTIFIER => ParseIdentifier(token, parser),
                TokenType.NULL => ParseNull(token),
                TokenType.SS => ParseSystemVerb(token),
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
        /// Parse string token (character vector) with full K escape sequence support
        /// Supports: \\, \b, \t, \n, \r, \", and octal sequences \000-\377
        /// </summary>
        public static ASTNode ParseString(Token token)
        {
            var lexeme = token.Lexeme;
            
            // CHARACTER_VECTOR tokens from lexer contain raw string content (no quotes)
            // But we also need to handle cases where quotes might be present
            if (lexeme.Length >= 2 && lexeme[0] == '"' && lexeme[^1] == '"')
            {
                // Remove surrounding quotes and process escape sequences
                var content = lexeme.Substring(1, lexeme.Length - 2);
                var processedString = ProcessEscapeSequences(content);
                
                // Create a character vector (string) from the processed content
                var charValues = processedString.Select(c => (K3Value)new CharacterValue(c.ToString())).ToList();
                return ASTNode.MakeLiteral(new VectorValue(charValues));
            }
            else
            {
                // CHARACTER_VECTOR token with raw content - process escape sequences directly
                var processedString = ProcessEscapeSequences(lexeme);
                
                // Create a character vector (string) from the processed content
                var charValues = processedString.Select(c => (K3Value)new CharacterValue(c.ToString())).ToList();
                return ASTNode.MakeLiteral(new VectorValue(charValues));
            }
        }
        
        /// <summary>
        /// Process escape sequences in string according to K specification
        /// </summary>
        private static string ProcessEscapeSequences(string input)
        {
            var result = new System.Text.StringBuilder();
            int i = 0;
            
            while (i < input.Length)
            {
                if (input[i] == '\\' && i + 1 < input.Length)
                {
                    char nextChar = input[i + 1];
                    
                    // Check for octal escape sequence (\000-\377)
                    if (nextChar >= '0' && nextChar <= '7' && i + 3 < input.Length)
                    {
                        string octalDigits = input.Substring(i + 1, 3);
                        if (octalDigits.Length == 3 && 
                            octalDigits[0] >= '0' && octalDigits[0] <= '7' &&
                            octalDigits[1] >= '0' && octalDigits[1] <= '7' &&
                            octalDigits[2] >= '0' && octalDigits[2] <= '7')
                        {
                            // Convert octal to character
                            int octalValue = Convert.ToInt32(octalDigits, 8);
                            result.Append((char)octalValue);
                            i += 4; // Skip \xxx
                            continue;
                        }
                    }
                    
                    // Handle standard escape sequences
                    char escapedChar = nextChar switch
                    {
                        '\\' => '\\',
                        'b' => '\b',
                        't' => '\t',
                        'n' => '\n',
                        'r' => '\r',
                        '"' => '"',
                        _ => nextChar // Any other character after \ is interpreted as itself
                    };
                    
                    result.Append(escapedChar);
                    i += 2; // Skip \x
                }
                else
                {
                    result.Append(input[i]);
                    i++;
                }
            }
            
            return result.ToString();
        }

        /// <summary>
        /// Parse symbol token
        /// </summary>
        private static ASTNode ParseSymbol(Token token)
        {
            var lexeme = token.Lexeme;
            
            // Remove backticks for symbol value
            var symbolValue = lexeme.Trim('`');
            return ASTNode.MakeLiteral(new SymbolValue(token.Lexeme));
        }
        
        /// <summary>
        /// Parse system verb token
        /// </summary>
        private static ASTNode ParseSystemVerb(Token token)
        {
            // Create a literal node for the system verb
            return ASTNode.MakeLiteral(new SymbolValue(token.Lexeme));
        }

        /// <summary>
        /// Parse identifier token (variable)
        /// Uses verb-agnostic system function detection
        /// </summary>
        private static ASTNode ParseIdentifier(Token token, LRSParser? parser)
        {
            var identifier = token.Lexeme;
            
            // Check if this identifier is a system function using verb-agnostic approach
            if (VerbQueryExtensions.IsSystemFunction(identifier))
            {
                // Create a system function node instead of a regular variable
                return ASTNode.MakeFunctionCall(ASTNode.MakeVariable(identifier), new List<ASTNode>());
            }
            
            // Per spec: Check if variable is defined during parsing (for multi-line scripts)
            // This allows variables assigned in earlier expressions to be referenced in later ones
            // Note: We create the variable node regardless - the evaluator will validate at runtime
            // The parser just needs to know the variable is "expected to exist" when evaluation runs
            if (parser != null && parser.IsVariableDefined(identifier))
            {
                // Variable is registered as defined during parsing - safe to create variable node
                return ASTNode.MakeVariable(identifier);
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
                TokenType.SYMBOL or TokenType.IDENTIFIER or TokenType.NULL or
                TokenType.SS => true,
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
                TokenType.COLON => ASTNode.MakeLiteral(new SymbolValue(":")),
                TokenType.SEMICOLON => ASTNode.MakeLiteral(new SymbolValue(";")),
                TokenType.DOT_APPLY => ASTNode.MakeLiteral(new SymbolValue(".")),
                TokenType.APPLY => ASTNode.MakeLiteral(new SymbolValue("@")),
                
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
