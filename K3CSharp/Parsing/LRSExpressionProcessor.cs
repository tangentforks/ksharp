using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Expression processing for LRS parser
    /// Replaces ExpressionParser.ParsePrimary() calls with LRS-based processing
    /// Routes tokens to appropriate LRS parsers using verb-agnostic design
    /// </summary>
    public class LRSExpressionProcessor
    {
        private readonly List<Token> tokens;
        private readonly bool buildParseTree;
        private readonly ILRSParser? lrsParser;
        
        public LRSExpressionProcessor(List<Token> tokens, bool buildParseTree = false, ILRSParser? lrsParser = null)
        {
            this.tokens = tokens;
            this.buildParseTree = buildParseTree;
            this.lrsParser = lrsParser;
        }

        /// <summary>
        /// Process expression from current position
        /// </summary>
        /// <param name="position">Reference to current position, updated to end of expression</param>
        /// <returns>AST node representing processed expression</returns>
        public ASTNode? ProcessExpression(ref int position)
        {
            if (position >= tokens.Count)
                return null;

            var token = tokens[position];
            
            // Route to appropriate parser based on token type
            if (LRSAtomicParser.IsAtomicToken(token.Type))
            {
                return ProcessAtomicExpression(ref position);
            }
            
            switch (token.Type)
            {
                case TokenType.LEFT_PAREN:
                    return ProcessParenthesesExpression(ref position);
                    
                case TokenType.LEFT_BRACKET:
                    return ProcessBracketExpression(ref position);
                    
                case TokenType.LEFT_BRACE:
                    return ProcessBraceExpression(ref position);
                    
                // Handle unary operators
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                case TokenType.MODULUS:
                case TokenType.POWER:
                case TokenType.JOIN:
                case TokenType.MATCH:
                case TokenType.NOT:
                case TokenType.HASH:
                case TokenType.UNDERSCORE:
                case TokenType.QUESTION:
                case TokenType.DOLLAR:
                case TokenType.APPLY:
                    return ProcessUnaryExpression(ref position);
                    
                // Handle special functions
                case TokenType.PARSE:
                case TokenType.EVAL:
                    return ProcessFunctionExpression(ref position);
                    
                // Handle system functions using VerbRegistry
                default:
                    if (OperatorDetector.IsFunction(token.Type))
                    {
                        return ProcessFunctionExpression(ref position);
                    }
                    
                    // Handle binary operators (should be handled at higher level)
                    if (OperatorDetector.IsBinaryOperator(token.Type))
                    {
                        return null; // Let caller handle binary operations
                    }
                    
                    throw new Exception($"Unexpected token in expression: {token.Type}({token.Lexeme})");
            }
        }

        /// <summary>
        /// Process atomic expression (literals, identifiers)
        /// </summary>
        private ASTNode? ProcessAtomicExpression(ref int position)
        {
            if (position >= tokens.Count)
                return null;
                
            var token = tokens[position];
            position++;
            
            return LRSAtomicParser.ParseAtomicToken(token);
        }

        /// <summary>
        /// Process parentheses expression using LRSGroupingParser
        /// </summary>
        private ASTNode? ProcessParenthesesExpression(ref int position)
        {
            var groupingParser = new LRSGroupingParser(tokens, buildParseTree);
            return groupingParser.ParseParentheses(ref position);
        }

        /// <summary>
        /// Process bracket expression using LRSGroupingParser
        /// </summary>
        private ASTNode? ProcessBracketExpression(ref int position)
        {
            var groupingParser = new LRSGroupingParser(tokens, buildParseTree);
            return groupingParser.ParseBrackets(ref position);
        }

        /// <summary>
        /// Process brace expression using LRSGroupingParser
        /// </summary>
        private ASTNode? ProcessBraceExpression(ref int position)
        {
            var groupingParser = new LRSGroupingParser(tokens, buildParseTree);
            return groupingParser.ParseBraces(ref position);
        }

        /// <summary>
        /// Process unary expression using LRSUnaryParser
        /// </summary>
        private ASTNode? ProcessUnaryExpression(ref int position)
        {
            if (position >= tokens.Count)
                return null;
                
            // Extract tokens for unary expression
            var unaryTokens = new List<Token>();
            var startPos = position;
            
            // Add the unary operator
            unaryTokens.Add(tokens[position]);
            position++;
            
            // Add operand tokens until we hit a binary operator or delimiter
            while (position < tokens.Count)
            {
                var currentToken = tokens[position];
                
                // Stop if we hit a binary operator or expression delimiter
                if (OperatorDetector.IsBinaryOperator(currentToken.Type) ||
                    currentToken.Type == TokenType.SEMICOLON ||
                    currentToken.Type == TokenType.NEWLINE ||
                    currentToken.Type == TokenType.RIGHT_PAREN ||
                    currentToken.Type == TokenType.RIGHT_BRACKET ||
                    currentToken.Type == TokenType.RIGHT_BRACE)
                {
                    break;
                }
                
                unaryTokens.Add(currentToken);
                position++;
            }
            
            // Parse the unary expression
            if (unaryTokens.Count >= 2)
            {
                // Create unary operator node using injected LRSParser or fallback
                if (lrsParser != null)
                {
                    var unaryPosition = 0;
                    return lrsParser.ParseExpression(ref unaryPosition);
                }
                else
                {
                    // Fallback to direct creation without LRSParser dependency
                    return CreateUnaryOperatorNode(unaryTokens);
                }
            }
            
            // Fallback to atomic parsing if only operator
            if (unaryTokens.Count == 1)
            {
                return LRSAtomicParser.CreateOperatorNode(unaryTokens[0].Type);
            }
            
            return null;
        }

        /// <summary>
        /// Process function expression using LRSFunctionParser
        /// </summary>
        private ASTNode? ProcessFunctionExpression(ref int position)
        {
            if (position >= tokens.Count)
                return null;
                
            // Extract tokens for function call
            var funcTokens = new List<Token>();
            var startPos = position;
            
            // Add the function token
            funcTokens.Add(tokens[position]);
            position++;
            
            // Add argument tokens until we hit a delimiter
            while (position < tokens.Count)
            {
                var currentToken = tokens[position];
                
                // Stop if we hit an expression delimiter
                if (currentToken.Type == TokenType.SEMICOLON ||
                    currentToken.Type == TokenType.NEWLINE ||
                    currentToken.Type == TokenType.RIGHT_PAREN ||
                    currentToken.Type == TokenType.RIGHT_BRACKET ||
                    currentToken.Type == TokenType.RIGHT_BRACE)
                {
                    break;
                }
                
                funcTokens.Add(currentToken);
                position++;
            }
            
            // Parse the function call
            var functionParser = new LRSFunctionParser(tokens, buildParseTree);
            return functionParser.ParseFunctionCall(funcTokens);
        }

        /// <summary>
        /// Check if processor can handle the current token
        /// </summary>
        public bool CanHandle(TokenType tokenType)
        {
            return LRSAtomicParser.IsAtomicToken(tokenType) ||
                   tokenType == TokenType.LEFT_PAREN ||
                   tokenType == TokenType.LEFT_BRACKET ||
                   tokenType == TokenType.LEFT_BRACE ||
                   OperatorDetector.SupportsMonadic(tokenType) ||
                   OperatorDetector.IsFunction(tokenType) ||
                   tokenType == TokenType.PARSE ||
                   tokenType == TokenType.EVAL;
        }

        /// <summary>
        /// Process complex expression structure (multiple tokens)
        /// </summary>
        public ASTNode? ProcessComplexExpression(List<Token> expressionTokens)
        {
            if (expressionTokens.Count == 0)
                return null;
                
            if (expressionTokens.Count == 1)
                return LRSAtomicParser.ParseAtomicToken(expressionTokens[0]);
            
            // Use injected LRS parser for complex expressions or fallback
            if (lrsParser != null)
            {
                var complexPosition = 0;
                return lrsParser.ParseExpression(ref complexPosition);
            }
            else
            {
                // Fallback to ExpressionParser for complex cases
                return ProcessComplexExpressionFallback(expressionTokens);
            }
        }
        
        /// <summary>
        /// Create unary operator node from tokens
        /// </summary>
        private ASTNode? CreateUnaryOperatorNode(List<Token> unaryTokens)
        {
            if (unaryTokens.Count < 2)
                return null;
                
            var operatorToken = unaryTokens[0];
            var operandTokens = unaryTokens.Skip(1).ToList();
            
            // Parse operand
            if (operandTokens.Count == 1)
            {
                var operand = LRSAtomicParser.ParseAtomicToken(operandTokens[0]);
                if (operand != null)
                {
                    return CreateUnaryOperatorNode(operatorToken.Type, operand);
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Create unary operator node
        /// </summary>
        private ASTNode CreateUnaryOperatorNode(TokenType operatorType, ASTNode operand)
        {
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue(operatorType.ToString().ToLower());
            node.Children.Add(operand);
            return node;
        }
        
        /// <summary>
        /// Process complex expression without LRSParser dependency
        /// </summary>
        private ASTNode? ProcessComplexExpressionFallback(List<Token> tokens)
        {
            // Simple fallback - use ExpressionParser for complex cases
            var expressionParser = new ExpressionParser();
            var context = new ParseContext(tokens, "");
            return expressionParser.Parse(context);
        }
    }
}
