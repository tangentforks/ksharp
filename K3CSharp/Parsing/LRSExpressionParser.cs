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
        public List<Token> ReadExpressionTokens(ref int position, int initialParenLevel = 0, int initialBracketLevel = 0, int initialBraceLevel = 0)
        {
            var expressionTokens = new List<Token>();
            var delimiterDepth = new DelimiterDepth(initialParenLevel, initialBracketLevel, initialBraceLevel);
            
            while (position < tokens.Count)
            {
                var token = tokens[position];
                
                // Always stop at EOF - it marks end of input, not start of new expression
                if (token.Type == TokenType.EOF)
                {
                    break;
                }
                
                // Stop at other separators when at base level, but only if we have some tokens already
                if (delimiterDepth.IsAtBaseLevel() && 
                    (token.Type == TokenType.SEMICOLON || token.Type == TokenType.NEWLINE) && 
                    expressionTokens.Count > 0)
                {
                    break;
                }
                
                // Stop at adverb boundaries when at base level, but NOT if preceded by a verb, function, or identifier (verb-immediate-left pattern)
                if (delimiterDepth.IsAtBaseLevel() && VerbRegistry.IsAdverbToken(token.Type))
                {
                    // Check if this adverb is preceded by a verb, function closing brace, or identifier in the collected expression tokens
                    // Use expressionTokens.Count to check if we have collected any tokens before this adverb
                    bool precededByVerbOrFunction = expressionTokens.Count > 0 && 
                        (VerbRegistry.IsVerbToken(expressionTokens[expressionTokens.Count - 1].Type) ||
                         expressionTokens[expressionTokens.Count - 1].Type == TokenType.RIGHT_BRACE ||
                         expressionTokens[expressionTokens.Count - 1].Type == TokenType.RIGHT_PAREN ||
                         expressionTokens[expressionTokens.Count - 1].Type == TokenType.IDENTIFIER);
                    
                    if (!precededByVerbOrFunction)
                    {
                        break;
                    }
                }
                    
                // Update delimiter depth
                delimiterDepth.UpdateDepth(token.Type);
                
                // Return if we've closed too many delimiters
                if (delimiterDepth.HasNegativeDepth())
                {
                    return expressionTokens;
                }
                
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
            
            public DelimiterDepth(int initialParenLevel = 0, int initialBracketLevel = 0, int initialBraceLevel = 0)
            {
                ParenLevel = initialParenLevel;
                BracketLevel = initialBracketLevel;
                BraceLevel = initialBraceLevel;
            }
            
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
