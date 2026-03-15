using System;

namespace K3CSharp
{
    /// <summary>
    /// Specialized parser for backtracking functionality
    /// Handles parser position backtracking for error recovery
    /// </summary>
    public class BacktrackHandler : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // This parser is used specifically for backtracking
            // It's not directly called by token type, but by the parser when needed
            return false;
        }

        public ASTNode? Parse(ParseContext context)
        {
            // This method is not used directly - Backtrack is the main entry point
            throw new NotImplementedException("Use Backtrack method instead");
        }

        /// <summary>
        /// Handle parser position backtracking for error recovery
        /// Moves the current position back if we're not at the beginning
        /// </summary>
        public static void Backtrack(ParseContext context)
        {
            // Move the current position back if we're not at the beginning
            if (context.Current > 0)
            {
                context.Current--;
            }
        }
    }
}
