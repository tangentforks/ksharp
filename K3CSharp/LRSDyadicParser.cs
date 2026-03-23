using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Dyadic operator parsing for LRS parser
    /// Handles dyadic operations with right-associative LRS semantics
    /// </summary>
    public class LRSDyadicParser
    {
        private readonly List<Token> tokens;
        private readonly LRSParser? parentParser;
        
        public LRSDyadicParser(List<Token> tokens, LRSParser? parentParser = null)
        {
            this.tokens = tokens;
            this.parentParser = parentParser;
        }
        
        /// <summary>
        /// Find the rightmost (or leftmost in Pure LRS) dyadic operator in token list
        /// Safe LRS mode: Find rightmost operator (original behavior for fallback compatibility)
        /// Pure LRS mode: Find leftmost operator with grouping depth tracking (improved behavior)
        /// </summary>
        /// <param name="tokens">Tokens to search</param>
        /// <returns>Index of dyadic operator, or -1 if none found</returns>
        public int FindRightmostOperator(List<Token> tokens)
        {
            bool pureLRSMode = parentParser?.PureLRSMode ?? false;
            
            if (pureLRSMode)
            {
                // PURE LRS MODE: Find LEFTMOST operator with grouping depth tracking
                // K uses right-to-left evaluation, so we split at the leftmost operator
                // to ensure the right side is evaluated first.
                // For example: 1 + 2 * 3 splits at + (leftmost), giving 1 + (2*3) = 1 + 6 = 7
                
                int depth = 0;
                
                for (int i = 0; i < tokens.Count; i++)
                {
                    var currentToken = tokens[i];
                    
                    // Track grouping depth
                    if (currentToken.Type == TokenType.LEFT_PAREN || 
                        currentToken.Type == TokenType.LEFT_BRACKET || 
                        currentToken.Type == TokenType.LEFT_BRACE)
                    {
                        depth++;
                        continue;
                    }
                    else if (currentToken.Type == TokenType.RIGHT_PAREN || 
                             currentToken.Type == TokenType.RIGHT_BRACKET || 
                             currentToken.Type == TokenType.RIGHT_BRACE)
                    {
                        depth--;
                        continue;
                    }
                    
                    // Only consider operators at depth 0 (not inside grouping constructs)
                    if (depth == 0 && IsDyadicOperatorDirect(currentToken.Type))
                    {
                        // NEW: Check if next token is an adverb (verb+adverb pattern)
                        if (i + 1 < tokens.Count && IsAdverbToken(tokens[i + 1].Type))
                        {
                            // This is a verb+adverb pattern - skip both
                            i++; // Skip the adverb too
                            continue;
                        }
                        
                        // This is a standalone dyadic operator at depth 0 - return it
                        return i;
                    }
                }
                
                return -1;
            }
            else
            {
                // SAFE LRS MODE (with fallback): Find RIGHTMOST operator (original behavior)
                // This ensures compatibility with legacy parser fallback
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    if (IsDyadicOperatorDirect(tokens[i].Type))
                    {
                        return i;
                    }
                }
                
                return -1;
            }
        }
        
        /// <summary>
        /// Direct dyadic operator detection without VerbRegistry (temporary fix)
        /// </summary>
        private static bool IsDyadicOperatorDirect(TokenType tokenType)
        {
            return tokenType == TokenType.PLUS ||
                   tokenType == TokenType.MINUS ||
                   tokenType == TokenType.MULTIPLY ||
                   tokenType == TokenType.DIVIDE ||
                   tokenType == TokenType.DOT_PRODUCT ||
                   tokenType == TokenType.MIN ||
                   tokenType == TokenType.MAX ||
                   tokenType == TokenType.LESS ||
                   tokenType == TokenType.GREATER ||
                   tokenType == TokenType.EQUAL ||
                   tokenType == TokenType.IN ||
                   tokenType == TokenType.POWER ||
                   tokenType == TokenType.MODULUS ||
                   tokenType == TokenType.MATCH ||
                   tokenType == TokenType.NEGATE ||
                   tokenType == TokenType.DOLLAR ||
                   tokenType == TokenType.QUESTION ||
                   // I/O verbs (digit-colon operators)
                   tokenType == TokenType.IO_VERB_0 ||
                   tokenType == TokenType.IO_VERB_1 ||
                   tokenType == TokenType.IO_VERB_2 ||
                   tokenType == TokenType.IO_VERB_3 ||
                   tokenType == TokenType.IO_VERB_4 ||
                   tokenType == TokenType.IO_VERB_5 ||
                   tokenType == TokenType.IO_VERB_6 ||
                   tokenType == TokenType.IO_VERB_7 ||
                   tokenType == TokenType.IO_VERB_8 ||
                   tokenType == TokenType.IO_VERB_9 ||
                   // Additional operators
                   tokenType == TokenType.JOIN ||
                   tokenType == TokenType.HASH ||
                   tokenType == TokenType.UNDERSCORE ||
                   tokenType == TokenType.APPLY ||
                   tokenType == TokenType.DOT_APPLY ||
                   tokenType == TokenType.TYPE ||
                   tokenType == TokenType.STRING_REPRESENTATION ||
                   // Mathematical functions (dyadic)
                   tokenType == TokenType.DIV ||
                   tokenType == TokenType.DOT_PRODUCT ||
                   tokenType == TokenType.MUL ||
                   tokenType == TokenType.AND ||
                   tokenType == TokenType.OR ||
                   tokenType == TokenType.XOR ||
                   tokenType == TokenType.ROT ||
                   tokenType == TokenType.SHIFT ||
                   tokenType == TokenType.LSQ ||
                   tokenType == TokenType.LIN ||
                   tokenType == TokenType.BIN ||
                   tokenType == TokenType.BINL ||
                   tokenType == TokenType.DV ||
                   tokenType == TokenType.DI ||
                   tokenType == TokenType.VS ||
                   tokenType == TokenType.SV ||
                   tokenType == TokenType.SS ||
                   tokenType == TokenType.SM ||
                   tokenType == TokenType.CI ||
                   tokenType == TokenType.IC ||
                   tokenType == TokenType.BD ||
                   tokenType == TokenType.DB ||
                   // System functions (dyadic)
                   tokenType == TokenType.IN ||
                   tokenType == TokenType.DRAW ||
                   tokenType == TokenType.GETENV ||
                   tokenType == TokenType.SETENV ||
                   tokenType == TokenType.SIZE;
        }
        
        /// <summary>
        /// Public method to check if token type is a dyadic operator
        /// </summary>
        /// <param name="tokenType">Token type to check</param>
        /// <returns>True if dyadic operator</returns>
        public bool IsDyadicOperator(TokenType tokenType)
        {
            return IsDyadicOperatorDirect(tokenType);
        }
        
        /// <summary>
        /// Parse dyadic operation using LRS right-associative strategy
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node representing dyadic operation</returns>
        public ASTNode? ParseDyadicOperation(List<Token> tokens)
        {
            if (tokens.Count < 3) return null; // Need at least: left op right
            
            var rightmostOpIndex = FindRightmostOperator(tokens);
            if (rightmostOpIndex == -1) return null;
            
            // Split at rightmost operator
            var leftTokens = tokens.GetRange(0, rightmostOpIndex);
            var rightTokens = tokens.GetRange(rightmostOpIndex + 1, tokens.Count - rightmostOpIndex - 1);
            var opToken = tokens[rightmostOpIndex];
            
            // Choose strategy based on parent parser mode
            if (parentParser?.BuildParseTree == true)
            {
                // Build parse tree: recursively parse left and right without evaluation
                var leftNode = BuildParseTreeFromTokens(leftTokens);
                var rightNode = BuildParseTreeFromTokens(rightTokens);
                
                // Handle null nodes by creating appropriate literals
                if (leftNode == null)
                    leftNode = ASTNode.MakeLiteral(new NullValue());
                if (rightNode == null)
                    rightNode = ASTNode.MakeLiteral(new NullValue());
                
                return CreateDyadicNode(opToken, leftNode, rightNode);
            }
            else
            {
                // Original evaluation logic
                var leftNode = ParseSubExpression(leftTokens);
                var rightNode = ParseSubExpression(rightTokens);
                
                // Handle null nodes by creating appropriate literals
                if (leftNode == null)
                    leftNode = ASTNode.MakeLiteral(new NullValue());
                if (rightNode == null)
                    rightNode = ASTNode.MakeLiteral(new NullValue());
                
                return CreateDyadicNode(opToken, leftNode, rightNode);
            }
        }
        
        /// <summary>
        /// Build parse tree from tokens (recursive)
        /// </summary>
        /// <param name="tokens">Tokens to build parse tree from</param>
        /// <returns>AST node representing parse tree structure</returns>
        private ASTNode? BuildParseTreeFromTokens(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) 
            {
                var nodeResult = CreateNodeFromToken(tokens[0]);
                return nodeResult;
            }
            
            // Try dyadic operation first (unary parsing is handled at main LRS level)
            var result = ParseDyadicOperation(tokens);
            if (result == null)
                return null;
            return result;
        }
        
        /// <summary>
        /// Parse sub-expression (could be unary, dyadic, or atomic)
        /// </summary>
        private ASTNode? ParseSubExpression(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) 
            {
                var nodeResult = CreateNodeFromToken(tokens[0]);
                return nodeResult;
            }
            
            // Try dyadic operation (unary parsing is handled at main LRS level)
            return ParseDyadicOperation(tokens);
        }
        
        /// <summary>
        /// Create AST node for dyadic operation
        /// </summary>
        private ASTNode CreateDyadicNode(Token opToken, ASTNode left, ASTNode right)
        {
            return ASTNode.MakeDyadicOp(opToken.Type, left, right);
        }
        
        /// <summary>
        /// Create AST node from atomic token using LRSAtomicParser
        /// </summary>
        private ASTNode? CreateNodeFromToken(Token token)
        {
            if (LRSAtomicParser.IsAtomicToken(token.Type))
            {
                return LRSAtomicParser.ParseAtomicToken(token);
            }
            
            // Handle operator symbols for parse trees
            return LRSAtomicParser.CreateOperatorNode(token.Type);
        }
        
        /// <summary>
        /// Check if token could be a dyadic operator
        /// </summary>
        public static bool CouldBeDyadicOperator(TokenType tokenType)
        {
            return IsDyadicOperatorDirect(tokenType);
        }
        
        /// <summary>
        /// Check if token is an adverb
        /// </summary>
        /// <param name="tokenType">Token type to check</param>
        /// <returns>True if token is an adverb</returns>
        private bool IsAdverbToken(TokenType tokenType)
        {
            return VerbRegistry.IsAdverbToken(tokenType);
        }
    }
}
