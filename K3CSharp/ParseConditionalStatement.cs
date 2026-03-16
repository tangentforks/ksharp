namespace K3CSharp 
{
    /// <summary>
    /// Handles parsing of conditional statements (do, if, while)
    /// </summary>
    partial class Parser
    {
        /// <summary>
        /// Parse conditional statements: do[count; expressions], if[condition; expressions], while[condition; expressions]
        /// These are "true statements" with variable argument counts
        /// </summary>
        private ASTNode ParseConditionalStatement(TokenType statementType)
        {
            // Consume the statement keyword token
            var keywordToken = CurrentToken();
            Advance();
            
            // Expect LEFT_BRACKET after the keyword
            if (!Match(TokenType.LEFT_BRACKET))
            {
                throw new Exception($"Expected '[' after {keywordToken.Lexeme}");
            }
            
            // Parse the arguments inside the brackets
            var arguments = new List<ASTNode>();
            
            // Parse first argument using ParseBracketArgument (doesn't stop at semicolons)
            var firstArg = ParseBracketArgument();
            if (firstArg == null)
            {
                throw new Exception($"Expected first argument after {keywordToken.Lexeme}[");
            }
            arguments.Add(firstArg);
            
            // Handle semicolon-separated arguments
            while (Match(TokenType.SEMICOLON))
            {
                var nextArg = ParseBracketArgument();
                if (nextArg != null)
                {
                    arguments.Add(nextArg);
                }
            }
            
            // Expect RIGHT_BRACKET to close the statement
            if (!Match(TokenType.RIGHT_BRACKET))
            {
                throw new Exception($"Expected ']' to close {keywordToken.Lexeme} statement");
            }
            
            // Create the conditional statement AST node
            var node = new ASTNode(ASTNodeType.ConditionalStatement);
            node.Value = new SymbolValue(keywordToken.Lexeme.ToLower());
            node.Children.AddRange(arguments);
            
            return node;
        }
    }
}
