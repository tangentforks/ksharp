using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Function call parsing for LRS parser
    /// Handles function calls and special verbs like _parse and _eval
    /// </summary>
    public class LRSFunctionParser
    {
        private readonly List<Token> tokens;
        
        public LRSFunctionParser(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        
        /// <summary>
        /// Parse function call from tokens
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node representing function call</returns>
        public ASTNode? ParseFunctionCall(List<Token> tokens)
        {
            if (tokens.Count < 2) return null;
            
            var funcToken = tokens[0];
            var argTokens = tokens.GetRange(1, tokens.Count - 1);
            
            // Check if this is a special function (_parse, _eval)
            if (IsSpecialFunction(funcToken.Type))
            {
                return HandleSpecialFunction(funcToken, argTokens);
            }
            
            // Parse as regular function call
            return ParseRegularFunction(funcToken, argTokens);
        }
        
        /// <summary>
        /// Handle special functions like _parse and _eval
        /// </summary>
        private ASTNode HandleSpecialFunction(Token funcToken, List<Token> argTokens)
        {
            // Parse argument using LRS parser
            var argument = ParseArgumentWithLRS(argTokens);
            
            var funcCall = new ASTNode(ASTNodeType.FunctionCall);
            funcCall.Children.Add(ASTNode.MakeVariable(funcToken.Lexeme));
            if (argument != null)
                funcCall.Children.Add(argument);
                
            return funcCall;
        }
        
        /// <summary>
        /// Parse regular function call
        /// </summary>
        private ASTNode ParseRegularFunction(Token funcToken, List<Token> argTokens)
        {
            // Parse argument using LRS parser
            var argument = ParseArgumentWithLRS(argTokens);
            
            var funcCall = new ASTNode(ASTNodeType.FunctionCall);
            funcCall.Children.Add(ASTNode.MakeVariable(funcToken.Lexeme));
            if (argument != null)
                funcCall.Children.Add(argument);
                
            return funcCall;
        }
        
        /// <summary>
        /// Parse argument tokens using LRS parser
        /// </summary>
        private ASTNode? ParseArgumentWithLRS(List<Token> argTokens)
        {
            if (argTokens.Count == 0) return null;
            if (argTokens.Count == 1) return CreateNodeFromToken(argTokens[0]);
            
            // Use LRS parser for complex arguments
            var lrsParser = new LRSParser(argTokens);
            var position = 0;
            return lrsParser.ParseExpression(ref position);
        }
        
        /// <summary>
        /// Check if token type represents a special function
        /// </summary>
        private bool IsSpecialFunction(TokenType tokenType)
        {
            return tokenType == TokenType.PARSE || tokenType == TokenType.EVAL;
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
                TokenType.SYMBOL or TokenType.IDENTIFIER => ASTNode.MakeVariable(token.Lexeme),
                TokenType.CHARACTER_VECTOR => ASTNode.MakeLiteral(new CharacterValue(token.Lexeme)),
                _ => throw new Exception($"Unsupported token type: {token.Type}")
            };
        }
        
        /// <summary>
        /// Check if token could be a function
        /// </summary>
        public static bool CouldBeFunction(TokenType tokenType)
        {
            return OperatorDetector.IsFunction(tokenType) || 
                   tokenType == TokenType.PARSE || 
                   tokenType == TokenType.EVAL;
        }
    }
}
