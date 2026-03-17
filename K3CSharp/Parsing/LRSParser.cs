using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Long Right Scope (LRS) parser for K expressions
    /// Implements right-first parsing strategy with proper K language rules
    /// </summary>
    public class LRSParser
    {
        private readonly List<Token> tokens;
        
        public LRSParser(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        
        /// <summary>
        /// Parse expression using LRS strategy (right-first parsing)
        /// </summary>
        /// <param name="position">Starting position, updated to end of parsed expression</param>
        /// <returns>AST node representing the parsed expression</returns>
        public ASTNode? ParseExpression(ref int position)
        {
            // Step 1: Read entire expression until separator
            var expressionTokens = ReadExpressionTokens(ref position);
            
            if (expressionTokens.Count == 0)
                return null;
                
            // Step 2: Evaluate from rightmost subexpression
            return EvaluateFromRight(expressionTokens);
        }
        
        /// <summary>
        /// Read tokens until we hit a separator (semicolon, newline, EOF, or closing delimiter)
        /// </summary>
        private List<Token> ReadExpressionTokens(ref int position)
        {
            var expressionTokens = new List<Token>();
            var parenLevel = 0;
            var bracketLevel = 0;
            var braceLevel = 0;
            
            while (position < tokens.Count)
            {
                var token = tokens[position];
                
                // Stop at separators
                if (token.Type == TokenType.SEMICOLON || token.Type == TokenType.NEWLINE || token.Type == TokenType.EOF)
                    break;
                    
                // Handle delimiters
                switch (token.Type)
                {
                    case TokenType.LEFT_PAREN:
                        parenLevel++;
                        break;
                    case TokenType.RIGHT_PAREN:
                        parenLevel--;
                        if (parenLevel < 0) return expressionTokens;
                        break;
                    case TokenType.LEFT_BRACKET:
                        bracketLevel++;
                        break;
                    case TokenType.RIGHT_BRACKET:
                        bracketLevel--;
                        if (bracketLevel < 0) return expressionTokens;
                        break;
                    case TokenType.LEFT_BRACE:
                        braceLevel++;
                        break;
                    case TokenType.RIGHT_BRACE:
                        braceLevel--;
                        if (braceLevel < 0) return expressionTokens;
                        break;
                }
                
                expressionTokens.Add(token);
                position++;
            }
            
            return expressionTokens;
        }
        
        /// <summary>
        /// Evaluate expression tokens from right to left (LRS strategy)
        /// </summary>
        private ASTNode? EvaluateFromRight(List<Token> expressionTokens)
        {
            if (expressionTokens.Count == 0)
                return null;
                
            if (expressionTokens.Count == 1)
                return CreateNodeFromToken(expressionTokens[0]);
            
            // Special handling for PARSE and EVAL tokens - treat as regular function calls
            if (expressionTokens[0].Type == TokenType.PARSE || expressionTokens[0].Type == TokenType.EVAL)
            {
                return HandleParseEvalFunction(expressionTokens);
            }
            
            // Find rightmost binary operator with lowest precedence
            var rightmostOpIndex = FindRightmostOperator(expressionTokens);
            
            if (rightmostOpIndex == -1)
                return CreateNodeFromToken(expressionTokens[0]);
                
            // Split at rightmost operator
            var leftTokens = expressionTokens.GetRange(0, rightmostOpIndex);
            var rightTokens = expressionTokens.GetRange(rightmostOpIndex + 1, expressionTokens.Count - rightmostOpIndex - 1);
            
            var leftNode = EvaluateFromRight(leftTokens);
            var rightNode = EvaluateFromRight(rightTokens);
            var opToken = expressionTokens[rightmostOpIndex];
            
            // Handle null nodes by creating appropriate literals
            if (leftNode == null)
                leftNode = ASTNode.MakeLiteral(new NullValue());
            if (rightNode == null)
                rightNode = ASTNode.MakeLiteral(new NullValue());
            
            return ASTNode.MakeBinaryOp(opToken.Type, leftNode, rightNode);
        }
        
        /// <summary>
        /// Handle _parse and _eval function calls
        /// </summary>
        private ASTNode HandleParseEvalFunction(List<Token> expressionTokens)
        {
            if (expressionTokens.Count < 2)
                throw new Exception($"Parse error at {expressionTokens[0].Position}: {expressionTokens[0].Lexeme}");
                
            var funcToken = expressionTokens[0];
            var argTokens = expressionTokens.GetRange(1, expressionTokens.Count - 1);
            
            // Parse argument using standard parser (not LRS)
            var argNode = ParseArgumentWithStandardParser(argTokens);
            
            var funcCall = new ASTNode(ASTNodeType.FunctionCall);
            funcCall.Children.Add(ASTNode.MakeVariable(funcToken.Lexeme));
            if (argNode != null)
                funcCall.Children.Add(argNode);
                
            return funcCall;
        }
        
        /// <summary>
        /// Parse argument tokens using standard parser (fallback for parse tree functions)
        /// </summary>
        private ASTNode? ParseArgumentWithStandardParser(List<Token> argTokens)
        {
            // This would need to be implemented by calling back into the main parser
            // For now, create a simple expression from the tokens
            if (argTokens.Count == 0)
                return null;
                
            if (argTokens.Count == 1)
                return CreateNodeFromToken(argTokens[0]);
                
            // For multiple tokens, create a simple binary operation or function call
            // This is a simplified implementation - the full parser would handle this properly
            var left = CreateNodeFromToken(argTokens[0]);
            var right = CreateNodeFromToken(argTokens[argTokens.Count - 1]);
            var op = argTokens.Count > 2 ? argTokens[1] : new Token(TokenType.PLUS, "+", 0);
            
            return ASTNode.MakeBinaryOp(op.Type, left, right);
        }
        
        /// <summary>
        /// Find the rightmost binary operator (LRS: all operators have same precedence)
        /// </summary>
        private int FindRightmostOperator(List<Token> tokens)
        {
            // In K LRS, all binary operators have the same precedence and are right-associative
            // Simply find the rightmost binary operator
            for (int i = tokens.Count - 1; i >= 0; i--)
            {
                if (IsBinaryOperator(tokens[i].Type))
                {
                    return i;
                }
            }
            
            return -1;
        }
        
        /// <summary>
        /// Check if token is a binary operator
        /// </summary>
        private bool IsBinaryOperator(TokenType tokenType)
        {
            return VerbRegistry.IsBinaryOperatorToken(tokenType);
        }
        
        /// <summary>
        /// Create AST node from token
        /// </summary>
        private ASTNode CreateNodeFromToken(Token token)
        {
            return token.Type switch
            {
                TokenType.INTEGER => ASTNode.MakeLiteral(new IntegerValue(int.Parse(token.Lexeme))),
                TokenType.FLOAT => ASTNode.MakeLiteral(new FloatValue(double.Parse(token.Lexeme))),
                TokenType.SYMBOL or TokenType.IDENTIFIER => ASTNode.MakeVariable(token.Lexeme),
                TokenType.CHARACTER_VECTOR => ASTNode.MakeLiteral(new CharacterValue(token.Lexeme)),
                _ => throw new Exception($"Unsupported token type: {token.Type}")
            };
        }
    }
}
