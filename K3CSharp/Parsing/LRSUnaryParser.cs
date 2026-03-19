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
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node representing monadic operation</returns>
        public ASTNode? ParseMonadicOperator(List<Token> tokens)
        {
            if (tokens.Count < 2) return null;
            
            var operatorToken = tokens[0];
            var operandTokens = tokens.GetRange(1, tokens.Count - 1);
            
            // Check if operator supports monadic operations
            if (!OperatorDetector.SupportsMonadic(operatorToken.Type))
                return null;
            
            // Parse operand recursively using the main LRS parser
            var position = 0; // Start after operator
            var operand = ParseOperandTokens(operandTokens, ref position);
            
            if (operand == null)
                return null;
            
            return CreateMonadicNode(operatorToken, operand);
        }
        
        /// <summary>
        /// Parse operand tokens using LRS parsing
        /// </summary>
        private ASTNode? ParseOperandTokens(List<Token> tokens, ref int position)
        {
            if (position >= tokens.Count) return null;
            
            // For single operand, create atomic node
            if (position == tokens.Count - 1)
            {
                return CreateNodeFromToken(tokens[position]);
            }
            
            // For multiple tokens, check if they form a vector (no operators)
            var remainingTokens = tokens.GetRange(position, tokens.Count - position);
            
            // Check if these tokens form a vector (no operators among them)
            if (AllTokensAreAtomic(remainingTokens))
            {
                // Create a vector from these atomic tokens
                return CreateVectorFromTokens(remainingTokens);
            }
            
            // Check if the first token of the remaining expression is another monadic operator
            if (remainingTokens.Count >= 1 && LRSUnaryParser.CouldBeMonadicOperator(remainingTokens[0].Type))
            {
                // This is a nested monadic operation like ^,`a
                // Parse it recursively using parent parser's mode
                var nestedResult = parentParser.ParseSubExpressionForUnary(remainingTokens, ref position);
                return nestedResult;
            }
            
            // Otherwise, use parent parser for binary operations
            return parentParser.ParseSubExpressionForUnary(remainingTokens, ref position);
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
        /// Create AST node from atomic token
        /// </summary>
        private ASTNode CreateNodeFromToken(Token token)
        {
            return token.Type switch
            {
                TokenType.INTEGER => ASTNode.MakeLiteral(new IntegerValue(int.Parse(token.Lexeme))),
                TokenType.FLOAT => ASTNode.MakeLiteral(new FloatValue(double.Parse(token.Lexeme))),
                TokenType.SYMBOL or TokenType.IDENTIFIER => ASTNode.MakeVariable(token.Lexeme.Trim('`')),
                _ => throw new Exception($"Unsupported token type: {token.Type}")
            };
        }
    }
}
