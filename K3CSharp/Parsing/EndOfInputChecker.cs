using System;

namespace K3CSharp
{
    /// <summary>
    /// Specialized parser for end-of-input checking functionality
    /// Handles checking if the parser has reached the end of the token stream
    /// </summary>
    public class EndOfInputChecker : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // This parser is used specifically for end-of-input checking
            // It's not directly called by token type, but by the parser when needed
            return false;
        }

        public ASTNode? Parse(ParseContext context)
        {
            // This method is not used directly - IsAtEnd is the main entry point
            throw new NotImplementedException("Use IsAtEnd method instead");
        }

        /// <summary>
        /// Check if the parser has reached the end of the token stream
        /// Returns true if at or beyond the last token, or if current token is EOF
        /// </summary>
        public static bool IsAtEnd(ParseContext context)
        {
            return context.Current >= context.Tokens.Count || 
                   (context.Current < context.Tokens.Count && context.Tokens[context.Current].Type == TokenType.EOF);
        }
    }
}
