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
                            if (!OperatorDetector.IsBinaryOperator(nextToken.Type) &&
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
        /// Create AST node for adverb operation
        /// </summary>
        private ASTNode CreateAdverbNode(Token adverbToken, ASTNode verb, ASTNode? argument = null)
        {
            var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
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

        /// <summary>
        /// Handle nested adverb operations
        /// </summary>
        private ASTNode? HandleNestedAdverb(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            // Check for nested adverb pattern like '//' or '\\'
            if (position + 1 < tokens.Count && 
                VerbRegistry.IsAdverbToken(tokens[position].Type) &&
                VerbRegistry.IsAdverbToken(tokens[position + 1].Type))
            {
                var firstAdverb = tokens[position];
                var secondAdverb = tokens[position + 1];
                position += 2;

                // Parse the argument for the nested adverb
                var arg = ParseAdverbArgument(ref position);
                if (arg == null)
                    throw new Exception($"Expected expression after nested adverbs {firstAdverb.Lexeme}{secondAdverb.Lexeme}");

                // Create nested adverb structure
                var innerAdverb = CreateAdverbNode(secondAdverb, arg);
                return CreateAdverbNode(firstAdverb, innerAdverb);
            }

            return null;
        }
    }
}
