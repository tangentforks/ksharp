using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Binary operator parsing for LRS parser
    /// Handles dyadic operations with right-associative LRS semantics
    /// </summary>
    public class LRSBinaryParser
    {
        private readonly List<Token> tokens;
        private readonly LRSParser? parentParser;
        
        public LRSBinaryParser(List<Token> tokens, LRSParser? parentParser = null)
        {
            this.tokens = tokens;
            this.parentParser = parentParser;
        }
        
        /// <summary>
        /// Find the rightmost binary operator in token list (LRS strategy)
        /// </summary>
        /// <param name="tokens">Tokens to search</param>
        /// <returns>Index of rightmost binary operator, or -1 if none found</returns>
        public int FindRightmostOperator(List<Token> tokens)
        {
            // In K LRS, all binary operators have the same precedence and are right-associative
            // Simply find the rightmost binary operator
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (OperatorDetector.IsBinaryOperator(tokens[i].Type))
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        /// <summary>
        /// Parse binary operation using LRS right-associative strategy
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node representing binary operation</returns>
        public ASTNode? ParseBinaryOperation(List<Token> tokens)
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
                
                return CreateBinaryNode(opToken, leftNode, rightNode);
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
                
                return CreateBinaryNode(opToken, leftNode, rightNode);
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
            
            // Try binary operation first (unary parsing is handled at main LRS level)
            var result = ParseBinaryOperation(tokens);
            if (result == null)
                return null;
            return result;
        }
        
        /// <summary>
        /// Parse sub-expression (could be unary, binary, or atomic)
        /// </summary>
        private ASTNode? ParseSubExpression(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) 
            {
                var nodeResult = CreateNodeFromToken(tokens[0]);
                return nodeResult;
            }
            
            // Try binary operation (unary parsing is handled at main LRS level)
            return ParseBinaryOperation(tokens);
        }
        
        /// <summary>
        /// Create AST node for binary operation
        /// </summary>
        private ASTNode CreateBinaryNode(Token opToken, ASTNode left, ASTNode right)
        {
            return ASTNode.MakeBinaryOp(opToken.Type, left, right);
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
        /// Check if token could be a binary operator
        /// </summary>
        public static bool CouldBeBinaryOperator(TokenType tokenType)
        {
            return OperatorDetector.IsBinaryOperator(tokenType);
        }
    }
}
