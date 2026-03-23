using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Adverb processing for LRS parser
    /// Handles adverb operations (each, over, scan, converge) using VerbRegistry categories
    /// Supports verb-agnostic adverb parsing without PrimaryParser dependency
    /// </summary>
    public class LRSAdverbParser
    {
        private readonly List<Token> tokens;
        private readonly bool buildParseTree;
        
        public LRSAdverbParser(List<Token> tokens, bool buildParseTree = false)
        {
            this.tokens = tokens;
            this.buildParseTree = buildParseTree;
        }

        /// <summary>
        /// Parse adverb operation from tokens
        /// </summary>
        /// <param name="position">Reference to current position, updated to end of adverb expression</param>
        /// <returns>AST node representing adverb operation</returns>
        public ASTNode? ParseAdverbOperation(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            var adverbToken = tokens[position];
            
            // Check if this is an adverb using VerbRegistry
            if (!VerbRegistry.IsAdverbToken(adverbToken.Type))
                return null;

            position++; // Consume adverb token

            // Parse the right argument for the adverb
            var rightArg = ParseAdverbArgument(ref position);
            if (rightArg == null)
                throw new Exception($"Expected expression after adverb {adverbToken.Lexeme}");

            // Parse the left argument (verb) if present
            var leftArg = ParseAdverbVerb(ref position);
            if (leftArg == null)
                throw new Exception($"Expected verb before adverb {adverbToken.Lexeme}");

            // Create adverb node: ADVERB(adverb, verb, rightArg)
            return CreateAdverbNode(adverbToken, leftArg, rightArg);
        }

        /// <summary>
        /// Parse adverb argument (right side of adverb)
        /// </summary>
        private ASTNode? ParseAdverbArgument(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            // Use LRS expression processor to parse the argument
            var expressionProcessor = new LRSExpressionProcessor(tokens, buildParseTree);
            return expressionProcessor.ProcessExpression(ref position);
        }

        /// <summary>
        /// Parse simple two-glyph adverb operation
        /// </summary>
        /// <param name="position">Reference to current position, updated to end of adverb expression</param>
        /// <returns>AST node representing two-glyph adverb operation</returns>
        public ASTNode? ParseSimpleTwoGlyphAdverb(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            var adverbToken = tokens[position];
            
            // Check if this is a simple two-glyph adverb
            if (!IsSimpleTwoGlyphAdverb(adverbToken.Type))
                return null;

            position++; // Consume adverb token

            // Parse the right argument for the adverb
            var rightArg = ParseAdverbArgument(ref position);
            if (rightArg == null)
                throw new Exception($"Expected expression after adverb {adverbToken.Lexeme}");

            // Parse the left argument (verb) - for two-glyph adverbs, the verb must be immediately to the left
            var leftArg = ParseVerbForAdverb(tokens, ref position, adverbToken);
            if (leftArg == null)
                throw new Exception($"Expected verb before adverb {adverbToken.Lexeme}");

            // Create two-glyph adverb node
            return CreateTwoGlyphAdverbNode(adverbToken, leftArg, rightArg);
        }
        
        /// <summary>
        /// Parse verb-immediate-left adverb pattern (verb on left side only)
        /// </summary>
        /// <param name="position">Reference to current position, updated to end of adverb expression</param>
        /// <returns>AST node representing verb-immediate-left adverb operation</returns>
        public ASTNode? ParseVerbImmediateLeftAdverb(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            var adverbToken = tokens[position];
            
            // Check if this is a single-glyph adverb that can have verb-immediate-left pattern
            if (!IsVerbImmediateLeftAdverb(adverbToken.Type))
                return null;

            position++; // Consume adverb token

            // Parse the right argument for the adverb
            var rightArg = ParseAdverbArgument(ref position);
            if (rightArg == null)
                throw new Exception($"Expected expression after adverb {adverbToken.Lexeme}");

            // Parse the left argument (verb) - must be immediately to the left
            var leftArg = ParseVerbForAdverb(tokens, ref position, adverbToken);
            if (leftArg == null)
                throw new Exception($"Expected verb before adverb {adverbToken.Lexeme}");

            // Create verb-immediate-left adverb node
            return CreateVerbImmediateLeftAdverbNode(adverbToken, leftArg, rightArg);
        }
        
        /// <summary>
        /// Check if token type is a simple two-glyph adverb
        /// </summary>
        private bool IsSimpleTwoGlyphAdverb(TokenType tokenType)
        {
            return tokenType == TokenType.ADVERB_SLASH_COLON ||
                   tokenType == TokenType.ADVERB_BACKSLASH_COLON ||
                   tokenType == TokenType.ADVERB_TICK_COLON;
        }
        
        /// <summary>
        /// Check if token type supports verb-immediate-left pattern
        /// </summary>
        private bool IsVerbImmediateLeftAdverb(TokenType tokenType)
        {
            return tokenType == TokenType.ADVERB_SLASH ||
                   tokenType == TokenType.ADVERB_BACKSLASH ||
                   tokenType == TokenType.ADVERB_TICK;
        }
        
        /// <summary>
        /// Parse verb that precedes an adverb
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="position">Current position (should be at verb)</param>
        /// <param name="adverbToken">The adverb token that follows</param>
        /// <returns>AST node for the verb, or null if not found</returns>
        private ASTNode? ParseVerbForAdverb(List<Token> tokens, ref int position, Token adverbToken)
        {
            if (position >= tokens.Count) return null;
            
            var verbToken = tokens[position];
            
            // Look backwards to find the verb immediately to the left
            // This is a simplified implementation - in practice, we'd need to scan backwards
            // For now, we'll handle the case where the verb is right before the adverb position
            
            // Handle system verbs and other registered verbs using VerbRegistry
            string? verbName = GetVerbNameFromToken(verbToken);
            if (verbName != null && VerbRegistry.HasVerb(verbName))
            {
                position++;
                return ASTNode.MakeLiteral(new SymbolValue(verbName));
            }
            
            // Handle atomic verbs (identifiers, symbols) as fallback
            if (LRSAtomicParser.IsAtomicToken(verbToken.Type))
            {
                position++;
                return LRSAtomicParser.ParseAtomicToken(verbToken);
            }
            
            // Handle operator verbs as fallback
            if (IsDyadicOperator(verbToken.Type))
            {
                position++;
                return CreateOperatorNode(verbToken);
            }
            
            return null;
        }
        
        /// <summary>
        /// Get verb name from token using VerbRegistry mapping
        /// </summary>
        /// <param name="token">Token to convert</param>
        /// <returns>Verb name or null if not a verb</returns>
        private string? GetVerbNameFromToken(Token token)
        {
            return token.Type switch
            {
                TokenType.CI => "_ci",
                TokenType.IC => "_ic",
                TokenType.SV => "_sv",
                TokenType.SS => "_ss",
                TokenType.SM => "_sm",
                TokenType.DRAW => "_draw",
                TokenType.GETENV => "_getenv",
                TokenType.SIZE => "_size",
                TokenType.DIRECTORY => "_d",
                TokenType.TIME => "_t",
                TokenType.EVAL => "_eval",
                TokenType.PARSE => "_parse",
                TokenType.PLUS => "+",
                TokenType.MINUS => "-",
                TokenType.MULTIPLY => "*",
                TokenType.DIVIDE => "%",
                TokenType.POWER => "^",
                TokenType.MODULUS => "!",
                TokenType.LESS => "<",
                TokenType.GREATER => ">",
                TokenType.EQUAL => "=",
                TokenType.MATCH => "~",
                TokenType.JOIN => ",",
                TokenType.HASH => "#",
                TokenType.DOLLAR => "$",
                TokenType.QUESTION => "?",
                TokenType.UNDERSCORE => "_",
                TokenType.ATOM => "@",
                TokenType.MAKE => ".",
                _ => null
            };
        }
        
        /// <summary>
        /// Check if token type is a dyadic operator
        /// </summary>
        private bool IsDyadicOperator(TokenType tokenType)
        {
            // Simplified check - in practice, this should use the same logic as LRSDyadicParser
            return tokenType == TokenType.PLUS ||
                   tokenType == TokenType.MINUS ||
                   tokenType == TokenType.MULTIPLY ||
                   tokenType == TokenType.DIVIDE ||
                   tokenType == TokenType.MODULUS ||
                   tokenType == TokenType.POWER ||
                   tokenType == TokenType.LESS ||
                   tokenType == TokenType.GREATER ||
                   tokenType == TokenType.EQUAL ||
                   tokenType == TokenType.MATCH ||
                   tokenType == TokenType.JOIN ||
                   tokenType == TokenType.HASH;
        }
        
        /// <summary>
        /// Create operator node from token
        /// </summary>
        private ASTNode CreateOperatorNode(Token token)
        {
            return new ASTNode(ASTNodeType.DyadicOp, new SymbolValue(token.Lexeme), new List<ASTNode>());
        }
        
        /// <summary>
        /// Create two-glyph adverb node
        /// </summary>
        private ASTNode CreateTwoGlyphAdverbNode(Token adverbToken, ASTNode leftArg, ASTNode rightArg)
        {
            var children = new List<ASTNode> { leftArg, rightArg };
            return new ASTNode(ASTNodeType.DyadicOp, new SymbolValue(adverbToken.Lexeme), children);
        }
        
        /// <summary>
        /// Create verb-immediate-left adverb node
        /// </summary>
        private ASTNode CreateVerbImmediateLeftAdverbNode(Token adverbToken, ASTNode leftArg, ASTNode rightArg)
        {
            var children = new List<ASTNode> { leftArg, rightArg };
            return new ASTNode(ASTNodeType.DyadicOp, new SymbolValue(adverbToken.Lexeme), children);
        }
        
        /// <summary>
        /// Parse adverb verb (left side of adverb)
        /// </summary>
        private ASTNode? ParseAdverbVerb(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            // Check if we have a verb before the adverb
            var currentToken = tokens[position];
            
            // Handle different types of verbs
            if (LRSAtomicParser.IsAtomicToken(currentToken.Type))
            {
                // Atomic value (literal, identifier, symbol)
                position++;
                return LRSAtomicParser.ParseAtomicToken(currentToken);
            }
            
            // Handle grouping constructs as verbs
            switch (currentToken.Type)
            {
                case TokenType.LEFT_PAREN:
                    var groupingParser = new LRSGroupingParser(tokens, buildParseTree);
                    return groupingParser.ParseParentheses(ref position);
                    
                case TokenType.LEFT_BRACKET:
                    var bracketParser = new LRSGroupingParser(tokens, buildParseTree);
                    return bracketParser.ParseBrackets(ref position);
                    
                case TokenType.LEFT_BRACE:
                    var braceParser = new LRSGroupingParser(tokens, buildParseTree);
                    return braceParser.ParseBraces(ref position);
                    
                // Handle unary operators as verbs
                default:
                    if (OperatorDetector.SupportsMonadic(currentToken.Type))
                    {
                        var unaryTokens = new List<Token> { currentToken };
                        
                        // Check if there are more tokens for the operand
                        if (position + 1 < tokens.Count)
                        {
                            var nextToken = tokens[position + 1];
                            if (!OperatorDetector.IsDyadicOperator(nextToken.Type) &&
                                !VerbRegistry.IsAdverbToken(nextToken.Type))
                            {
                                // Include the operand
                                unaryTokens.Add(nextToken);
                                position += 2;
                            }
                            else
                            {
                                position++;
                            }
                        }
                        else
                        {
                            position++;
                        }
                        
                        var unaryParser = new LRSUnaryParser(new LRSParser(tokens, buildParseTree));
                        return unaryParser.ParseMonadicOperator(unaryTokens);
                    }
                    
                    throw new Exception($"Unexpected token in adverb verb: {currentToken.Type}({currentToken.Lexeme})");
            }
        }

        /// <summary>
        /// Parse adverb chain (multiple adverbs applied to same verb)
        /// </summary>
        public ASTNode? ParseAdverbChain(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            var nodes = new List<ASTNode>();
            
            // Parse the first verb
            var verb = ParseAdverbVerb(ref position);
            if (verb == null)
                return null;

            nodes.Add(verb);

            // Parse subsequent adverbs
            while (position < tokens.Count && VerbRegistry.IsAdverbToken(tokens[position].Type))
            {
                var adverbToken = tokens[position];
                position++;

                // Parse adverb argument
                var arg = ParseAdverbArgument(ref position);
                if (arg == null)
                    throw new Exception($"Expected expression after adverb {adverbToken.Lexeme}");

                // Create adverb node with previous result as verb
                var adverbNode = CreateAdverbNode(adverbToken, nodes[nodes.Count - 1], arg);
                nodes[nodes.Count - 1] = adverbNode; // Replace with adverb result
            }

            return nodes.Count > 0 ? nodes[0] : null;
        }

        /// <summary>
        /// Validate that a verb is compatible with the given adverb
        /// </summary>
        /// <param name="verbNode">The verb AST node to validate</param>
        /// <param name="adverbType">The adverb type to check compatibility for</param>
        /// <returns>True if the verb is compatible with the adverb</returns>
        private bool ValidateVerbAdverbCompatibility(ASTNode verbNode, string adverbType)
        {
            // Extract verb name from the AST node
            var verbName = ExtractVerbName(verbNode);
            if (string.IsNullOrEmpty(verbName))
                return false;
            
            // Use VerbRegistry.SupportsAdverbs directly for more comprehensive validation
            return VerbRegistry.SupportsAdverbs(verbName, adverbType);
        }
        
        /// <summary>
        /// Extract verb name from AST node
        /// </summary>
        /// <param name="verbNode">The AST node containing the verb</param>
        /// <returns>Verb name or null if not found</returns>
        private string? ExtractVerbName(ASTNode verbNode)
        {
            if (verbNode.Value is SymbolValue symbol)
                return symbol.Value;
            
            // For variable nodes, check the node type and extract name differently
            if (verbNode.Type == ASTNodeType.Variable)
            {
                // Variable nodes store the name in a different way
                // For now, we'll handle this case by returning null
                // This could be enhanced based on how variable nodes are actually implemented
                return null;
            }
            
            return null;
        }
        
        /// <summary>
        /// Create AST node for adverb operation with validation
        /// </summary>
        private ASTNode CreateAdverbNode(Token adverbToken, ASTNode verb, ASTNode? argument = null)
        {
            var adverbType = VerbRegistry.GetAdverbType(adverbToken.Type);
            
            // Validate verb-adverb compatibility using verb-agnostic approach
            if (!ValidateVerbAdverbCompatibility(verb, adverbType))
            {
                var verbName = ExtractVerbName(verb) ?? "unknown";
                throw new Exception($"Verb '{verbName}' is not compatible with adverb '{adverbToken.Lexeme}' ({adverbType})");
            }
            
            var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
            adverbNode.Value = new SymbolValue(GetAdverbSymbol(adverbToken.Type));
            adverbNode.Children.Add(verb);
            if (argument != null)
                adverbNode.Children.Add(argument);
            return adverbNode;
        }

        /// <summary>
        /// Get adverb symbol for token type
        /// </summary>
        private string GetAdverbSymbol(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ADVERB_TICK => "'",
                TokenType.ADVERB_SLASH => "/",
                TokenType.ADVERB_BACKSLASH => "\\",
                TokenType.ADVERB_SLASH_COLON => "/:",
                TokenType.ADVERB_TICK_COLON => "':",
                TokenType.ADVERB_BACKSLASH_COLON => "\\:",
                _ => tokenType.ToString().ToLower()
            };
        }

        /// <summary>
        /// Check if token type represents an adverb
        /// </summary>
        public static bool CouldBeAdverb(TokenType tokenType)
        {
            return VerbRegistry.IsAdverbToken(tokenType);
        }

        /// <summary>
        /// Parse complex adverb expression with proper precedence
        /// </summary>
        public ASTNode? ParseComplexAdverbExpression(List<Token> adverbTokens)
        {
            if (adverbTokens.Count == 0)
                return null;

            var position = 0;
            return ParseAdverbChain(ref position);
        }
    }
}
