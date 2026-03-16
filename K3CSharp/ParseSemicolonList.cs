namespace K3CSharp 
{
    /// <summary>
    /// Handles parsing of semicolon-separated lists
    /// </summary>
    partial class Parser
    {
        /// <summary>
        /// Parse semicolon-separated expressions into a vector
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
                
                // Handle binary operators with Long Right Scope for this element
                while (VerbRegistry.IsBinaryOperator(CurrentToken().Type))
                {
                    var op = CurrentToken().Type;
                    Match(op); // Consume the operator
                    
                    // Check if this is followed by an adverb (infix adverb)
                    if (VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        var adverbToken = CurrentToken();
                        Advance(); // Consume the adverb token
                        var adverbType = adverbToken.Type.ToString().Replace("TokenType.", "");
                        
                        // Convert the binary operator to a verb symbol
                        var verbName = VerbRegistry.GetBinaryOperatorSymbol(op);
                        var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                        
                        // Parse the right side of the adverb with LRS
                        var rightSide = ParseTerm();
                        
                        // Create the correct adverb structure: ADVERB(verb, left, right)
                        var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        adverbNode.Value = new SymbolValue(adverbType);
                        if (verbNode != null) adverbNode.Children.Add(verbNode);
                        if (next != null) adverbNode.Children.Add(next);
                        if (rightSide != null) adverbNode.Children.Add(rightSide);
                        
                        next = adverbNode;
                    }
                    else
                    {
                        // Regular binary operation with Long Right Scope
                        var right = ParseTerm();
                        if (next != null && right != null)
                            next = ASTNode.MakeBinaryOp(op, next, right);
                    }
                }
                
                // Handle empty positions: if next is null, add a null literal
                // This represents an empty position in a semicolon-separated list
                if (next != null)
                {
                    elements.Add(next);
                }
                else
                {
                    // Empty position becomes null in K semicolon-separated lists
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
            } while (Match(TokenType.SEMICOLON));
            
            // Always create mixed list (anonymous list) for semicolon-separated expressions
            return ASTNode.MakeVector(elements);
        }
    }
}
