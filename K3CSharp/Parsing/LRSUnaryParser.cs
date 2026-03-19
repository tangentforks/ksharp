using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Monadic operator parsing for LRS parser
    /// Handles unary operations with proper verb-agnostic detection
    /// </summary>
    public class LRSUnaryParser
    {
        private readonly LRSParser parentParser;
        
        public LRSUnaryParser(LRSParser parentParser)
        {
            this.parentParser = parentParser;
        }
        
        /// <summary>
        /// Parse monadic operator expression from tokens
        /// Enhanced to handle all unary operations without delegation
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node representing monadic operation</returns>
        public ASTNode? ParseMonadicOperator(List<Token> tokens)
        {
            if (tokens.Count < 1) return null;
            
            var operatorToken = tokens[0];
            
            // Check if operator supports monadic operations
            if (!OperatorDetector.SupportsMonadic(operatorToken.Type))
                return null;
            
            // Handle projection case (no operand)
            if (tokens.Count == 1 || IsProjectionContext(tokens))
            {
                return CreateProjectionNode(operatorToken);
            }
            
            // Parse operand
            var operandTokens = tokens.GetRange(1, tokens.Count - 1);
            var operand = ParseOperandTokens(operandTokens);
            
            if (operand == null)
                return null;
            
            return CreateMonadicNode(operatorToken, operand);
        }
        
        /// <summary>
        /// Check if this is a projection context (no operand or ending with delimiter)
        /// </summary>
        private bool IsProjectionContext(List<Token> tokens)
        {
            if (tokens.Count <= 1) return true;
            
            // Check if the last token is a delimiter indicating projection
            var lastToken = tokens[tokens.Count - 1];
            return lastToken.Type == TokenType.RIGHT_PAREN ||
                   lastToken.Type == TokenType.RIGHT_BRACKET ||
                   lastToken.Type == TokenType.RIGHT_BRACE ||
                   lastToken.Type == TokenType.SEMICOLON ||
                   lastToken.Type == TokenType.NEWLINE;
        }
        
        /// <summary>
        /// Create projection node for unary operator
        /// </summary>
        private ASTNode CreateProjectionNode(Token operatorToken)
        {
            var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
            
            // Convert token type to operator symbol using VerbRegistry
            var operatorSymbol = VerbRegistry.GetBinaryOperatorSymbol(operatorToken.Type);
            
            projectedNode.Value = new SymbolValue(operatorSymbol);
            
            // Determine arity from VerbRegistry - use highest supported arity as default
            var verb = VerbRegistry.GetVerb(operatorSymbol);
            int defaultArity = verb?.SupportedArities?.Max() ?? 2;
            projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(defaultArity)));
            
            return projectedNode;
        }
        
        /// <summary>
        /// Parse operand tokens using LRS parsing
        /// </summary>
        private ASTNode? ParseOperandTokens(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) return CreateNodeFromToken(tokens[0]);
            
            // For multiple tokens, check if they form a vector (no operators)
            if (AllTokensAreAtomic(tokens))
            {
                // Create a vector from these atomic tokens
                return CreateVectorFromTokens(tokens);
            }
            
            // Check if the first token of the remaining expression is another monadic operator
            if (tokens.Count >= 1 && LRSUnaryParser.CouldBeMonadicOperator(tokens[0].Type))
            {
                // This is a nested monadic operation like ^,`a
                // Parse it recursively using parent parser's mode
                var position = 0;
                var nestedResult = parentParser.ParseSubExpressionForUnary(tokens, ref position);
                return nestedResult;
            }
            
            // Otherwise, use parent parser for binary operations
            var position2 = 0;
            return parentParser.ParseSubExpressionForUnary(tokens, ref position2);
        }
        
        /// <summary>
        /// Check if all tokens are atomic (no operators)
        /// </summary>
        private bool AllTokensAreAtomic(List<Token> tokens)
        {
            foreach (var token in tokens)
            {
                if (IsOperatorToken(token.Type))
                    return false;
            }
            return true;
        }
        
        /// <summary>
        /// Check if token is an operator
        /// </summary>
        private bool IsOperatorToken(TokenType tokenType)
        {
            return OperatorDetector.IsBinaryOperator(tokenType) || 
                   LRSUnaryParser.CouldBeMonadicOperator(tokenType);
        }
        
        /// <summary>
        /// Create vector AST node from multiple atomic tokens
        /// </summary>
        private ASTNode CreateVectorFromTokens(List<Token> tokens)
        {
            // For parse trees, we want to represent vectors as individual elements
            // not as a Vector node, so ConvertVector can handle them properly
            // Create a Block node that contains all the atomic elements
            var blockNode = new ASTNode(ASTNodeType.Block);
            foreach (var token in tokens)
            {
                var atomicNode = CreateNodeFromToken(token);
                if (atomicNode != null)
                    blockNode.Children.Add(atomicNode);
            }
            return blockNode;
        }
        
        /// <summary>
        /// Create AST node for monadic operation
        /// </summary>
        private ASTNode CreateMonadicNode(Token operatorToken, ASTNode operand)
        {
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue(GetOperatorSymbol(operatorToken.Type));
            node.Children.Add(operand);
            return node;
        }
        
        /// <summary>
        /// Get operator symbol for token type using VerbRegistry
        /// </summary>
        private string GetOperatorSymbol(TokenType tokenType)
        {
            // For monadic operations, we use the operator symbol
            return VerbRegistry.GetBinaryOperatorSymbol(tokenType);
        }
        
        /// <summary>
        /// Check if the first token could be a monadic operator
        /// </summary>
        public static bool CouldBeMonadicOperator(TokenType tokenType)
        {
            return OperatorDetector.SupportsMonadic(tokenType);
        }
        
        /// <summary>
        /// Create AST node from atomic token using LRSAtomicParser
        /// </summary>
        private ASTNode CreateNodeFromToken(Token token)
        {
            return LRSAtomicParser.ParseAtomicToken(token);
        }
    }
}
