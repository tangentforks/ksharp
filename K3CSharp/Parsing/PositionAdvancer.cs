using System;

namespace K3CSharp
{
    /// <summary>
    /// Specialized parser for position advancement functionality
    /// Handles advancing the parser's current token position
    /// </summary>
    public class PositionAdvancer : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // This parser is used specifically for position advancement
            // It's not directly called by token type, but by the parser when needed
            return false;
        }

        public ASTNode? Parse(ParseContext context)
        {
            // This method is not used directly - Advance is the main entry point
            throw new NotImplementedException("Use Advance method instead");
        }

        /// <summary>
        /// Advance the parser's current position by one
        /// Moves to the next token in the token stream
        /// </summary>
        public static void Advance(ParseContext context)
        {
            context.Current++;
        }
    }
}
