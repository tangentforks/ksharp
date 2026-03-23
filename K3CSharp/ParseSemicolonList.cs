namespace K3CSharp 
{
    /// <summary>
    /// Handles parsing of expressions separated by semicolons according to K specification
    /// Semicolons are expression separators, not special "semicolon-separated lists"
    /// </summary>
    partial class Parser
    {
        /// <summary>
        /// Parse expressions separated by semicolons into a list according to K specification
        /// Semicolons are expression separators that create lists when multiple expressions are present
        /// </summary>
        private ASTNode? ParseSemicolonList(ASTNode left, bool insideDelimiters)
        {
            var elements = new List<ASTNode> { left };
            
            do
            {
                // Stop if we hit a closing delimiter - let the caller handle it
                if (IsAtEnd() || CurrentToken().Type == TokenType.RIGHT_PAREN ||
                    CurrentToken().Type == TokenType.RIGHT_BRACE ||
                    CurrentToken().Type == TokenType.RIGHT_BRACKET ||
                    CurrentToken().Type == TokenType.NEWLINE)
                {
                    break;
                }
                
                // Parse a single expression for next element (without semicolon handling)
                var next = ParseTerm();
                
                // Handle dyadic operators with Long Right Scope for this element
                while (VerbRegistry.IsDyadicOperator(CurrentToken().Type))
                {
                    var op = CurrentToken().Type;
                    Match(op); // Consume the operator
                    
                    // Check if this is followed by an adverb (infix adverb)
                    if (VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        var adverbToken = CurrentToken();
                        Advance(); // Consume the adverb token
                        var adverbType = adverbToken.Type.ToString().Replace("TokenType.", "");
                        
                        // Convert the dyadic operator to a verb symbol
                        var verbName = VerbRegistry.GetDyadicOperatorSymbol(op);
                        var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                        
                        // Parse the right side of the adverb with LRS
                        var rightSide = ParseTerm();
                        
                        // Create the correct adverb structure: ADVERB(verb, left, right)
                        var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
                        adverbNode.Value = new SymbolValue(adverbType);
                        if (verbNode != null) adverbNode.Children.Add(verbNode);
                        if (next != null) adverbNode.Children.Add(next);
                        if (rightSide != null) adverbNode.Children.Add(rightSide);
                        
                        next = adverbNode;
                    }
                    else
                    {
                        // Regular dyadic operation with Long Right Scope
                        var right = ParseTerm();
                        if (next != null && right != null)
                            next = ASTNode.MakeDyadicOp(op, next, right);
                    }
                }
                
                // Handle empty expressions (consecutive separators) - should be null values
                if (next != null)
                {
                    elements.Add(next);
                }
                else
                {
                    // Empty expression becomes null according to K specification
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
            } while (Match(TokenType.SEMICOLON));
            
            // Create list according to K specification rules
            // The evaluator will handle vector collapsing if all elements have the same type
            return ASTNode.MakeVector(elements);
        }
    }
}
