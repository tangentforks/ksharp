using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Expression boundary detection for LRS parser
    /// Handles delimiter tracking and expression boundary identification
    /// </summary>
    public class LRSExpressionParser
    {
        private readonly List<Token> tokens;
        
        public LRSExpressionParser(List<Token> tokens)
        {
            this.tokens = tokens;
        }
        
        /// <summary>
        /// Read tokens until we hit a separator (semicolon, newline, EOF, or closing delimiter)
        /// </summary>
        public List<Token> ReadExpressionTokens(ref int position)
        {
            var expressionTokens = new List<Token>();
            var delimiterDepth = new DelimiterDepth();
            
            while (position < tokens.Count)
            {
                var token = tokens[position];
                
                // Always stop at EOF - it marks end of input, not start of new expression
                if (token.Type == TokenType.EOF)
                    break;
                
                // Stop at other separators when at base level, but only if we have some tokens already
                if (delimiterDepth.IsAtBaseLevel() && 
                    (token.Type == TokenType.SEMICOLON || token.Type == TokenType.NEWLINE) && 
                    expressionTokens.Count > 0)
                    break;
                    
                // Update delimiter depth
                delimiterDepth.UpdateDepth(token.Type);
                
                // Return if we've closed too many delimiters
                if (delimiterDepth.HasNegativeDepth())
                    return expressionTokens;
                
                expressionTokens.Add(token);
                position++;
            }
            
            return expressionTokens;
        }
        
        /// <summary>
        /// Helper class to track delimiter nesting depth
        /// </summary>
        private class DelimiterDepth
        {
            public int ParenLevel { get; private set; }
            public int BracketLevel { get; private set; }
            public int BraceLevel { get; private set; }
            
            public bool IsAtBaseLevel() => ParenLevel == 0 && BracketLevel == 0 && BraceLevel == 0;
            public bool HasNegativeDepth() => ParenLevel < 0 || BracketLevel < 0 || BraceLevel < 0;
            
            public void UpdateDepth(TokenType tokenType)
            {
                switch (tokenType)
                {
                    case TokenType.LEFT_PAREN:
                        ParenLevel++;
                        break;
                    case TokenType.RIGHT_PAREN:
                        ParenLevel--;
                        break;
                    case TokenType.LEFT_BRACKET:
                        BracketLevel++;
                        break;
                    case TokenType.RIGHT_BRACKET:
                        BracketLevel--;
                        break;
                    case TokenType.LEFT_BRACE:
                        BraceLevel++;
                        break;
                    case TokenType.RIGHT_BRACE:
                        BraceLevel--;
                        break;
                }
            }
        }
    }
}
