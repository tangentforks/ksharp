namespace K3CSharp
{
    public partial class Parser
    {
        /// <summary>
        /// Parse an expression without stopping at semicolons (used within brackets and other contexts)
        /// </summary>
        private ASTNode? ParseExpressionWithoutSemicolons()
        {
            // Check for conditional statements at the start of an expression
            if (!IsAtEnd())
            {
                var tokenType = CurrentToken().Type;
                if (tokenType == TokenType.DO || tokenType == TokenType.IF_FUNC || tokenType == TokenType.WHILE)
                {
                    return ParseConditionalStatement(tokenType);
                }
            }

            return ParseExpressionInsideDelimiters();
        }
        
        /// <summary>
        /// Parse expression with the knowledge that we're inside delimiters
        /// This affects semicolon behavior - they should create mixed lists
        /// </summary>
        private ASTNode? ParseExpressionInsideDelimiters()
        {
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
