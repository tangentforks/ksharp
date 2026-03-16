namespace K3CSharp 
{
    /// <summary>
    /// Handles parsing of expressions inside delimiters
    /// </summary>
    partial class Parser
    {
        /// <summary>
        /// Parse expression with the knowledge that we're inside delimiters
        /// This affects semicolon behavior - they should create mixed lists
        /// </summary>
        private ASTNode? ParseExpressionInsideDelimiters()
        {
            // Parse expression with the knowledge that we're inside delimiters
            // This affects semicolon behavior - they should create mixed lists
            var left = ParseTerm();
            
            // Handle case where first element is empty (e.g., "(;1;2)")
            if (left == null && !IsAtEnd() && CurrentToken().Type == TokenType.SEMICOLON)
            {
                // Empty first element becomes null in K semicolon-separated lists
                // Just continue to semicolon handling
            }
            
            // Handle semicolon-separated expressions - inside delimiters, create mixed lists
            if (Match(TokenType.SEMICOLON))
            {
                return ParseSemicolonList(left ?? ASTNode.MakeLiteral(new NullValue()), true); // Inside delimiters = true
            }
            
            return left;
        }
    }
}
