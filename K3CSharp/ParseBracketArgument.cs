namespace K3CSharp 
{
    /// <summary>
    /// Handles parsing of bracket arguments and bracket contents
    /// </summary>
    partial class Parser
    {
        /// <summary>
        /// Parse bracket contents to handle the comma enlistment part of f[x] is (f) .,(x)
        /// </summary>
        private ASTNode ParseBracketContentsAsCommaEnlisted()
        {
            // Parse bracket contents to handle the comma enlistment part of f[x] is (f) .,(x)
            
            if (Match(TokenType.RIGHT_BRACKET))
            {
                // Empty brackets [] - return null (which becomes _n when comma-enlisted)
                return ASTNode.MakeLiteral(new NullValue());
            }
            
            // Parse expressions separated by semicolons (for function arguments)
            var expressions = new List<ASTNode>();
            
            // Parse first argument using a method that doesn't stop at semicolons
            var firstArg = ParseBracketArgument();
            if (firstArg != null) expressions.Add(firstArg);
            
            // Handle semicolon-separated arguments
            while (Match(TokenType.SEMICOLON))
            {
                var nextArg = ParseBracketArgument();
                if (nextArg != null) expressions.Add(nextArg);
            }
            
            if (!Match(TokenType.RIGHT_BRACKET))
            {
                throw new Exception("Expected ']' after bracket expression");
            }
            
            // If multiple expressions, create a vector; otherwise return single expression
            if (expressions.Count > 1)
            {
                return ASTNode.MakeVector(expressions);
            }
            else
            {
                return expressions[0];
            }
        }

        /// <summary>
        /// Parse an expression for bracket arguments, treating semicolons as separators
        /// This is similar to ParseExpression but doesn't stop at semicolons
        /// </summary>
        private ASTNode? ParseBracketArgument()
        {
            // Parse an expression for bracket arguments, treating semicolons as separators
            // This is similar to ParseExpression but doesn't stop at semicolons

            // Check for standalone operator as function reference (e.g., + in @[x; i; +; y])
            // or operator followed by adverb (e.g., +/ in @[+/; args; :])
            if (!IsAtEnd() && IsDyadicOperator(CurrentToken().Type))
            {
                var nextIdx = current + 1;
                var nextType = nextIdx < tokens.Count ? tokens[nextIdx].Type : TokenType.EOF;
                
                if (nextType == TokenType.SEMICOLON || nextType == TokenType.RIGHT_BRACKET || nextType == TokenType.RIGHT_PAREN || nextType == TokenType.EOF)
                {
                    // Standalone operator - treat as symbol
                    var opToken = CurrentToken();
                    var opSymbol = VerbRegistry.GetDyadicOperatorSymbol(opToken.Type);
                    Match(opToken.Type);
                    return ASTNode.MakeLiteral(new SymbolValue(opSymbol));
                }
                else if (VerbRegistry.IsAdverbToken(nextType))
                {
                    // Operator followed by adverb - create projected function
                    var opToken = CurrentToken();
                    var adverbToken = tokens[nextIdx];
                    
                    var opSymbol = VerbRegistry.GetDyadicOperatorSymbol(opToken.Type);
                    
                    var adverbType = VerbRegistry.GetAdverbType(adverbToken.Type);
                    
                    // Create projected function
                    var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                    projectedNode.Value = new SymbolValue(adverbType);
                    projectedNode.Children.Add(ASTNode.MakeLiteral(new SymbolValue(opSymbol))); // Store the verb
                    projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1))); // Needs 1 more argument
                    
                    // Consume both tokens
                    Match(opToken.Type);
                    Match(adverbToken.Type);
                    
                    return projectedNode;
                }
            }

            var left = ParseTerm();
            if (left == null)
            {
                return null;
            }

            // Handle assignment operators (including modified assignments like +:, -:, *:)
            if (!IsAtEnd() && CurrentToken().Type == TokenType.ASSIGNMENT)
            {
                var assignToken = CurrentToken();
                Match(TokenType.ASSIGNMENT);
                var right = ParseBracketArgument();
                if (right == null)
                {
                    throw new Exception("Expected expression after assignment operator");
                }
                // Extract variable name from left side
                if (left.Type == ASTNodeType.Variable)
                {
                    var variableName = left.Value is SymbolValue sym ? sym.Value : left.Value?.ToString() ?? "";
                    
                    // Validate variable name - cannot start with underscore
                    if (variableName.StartsWith("_"))
                    {
                        throw new Exception("Names starting with _ are reserved");
                    }
                    
                    if (assignToken.Lexeme.Length > 1)
                    {
                        // Modified assignment (e.g., +:, -:, *:)
                        var opChar = assignToken.Lexeme[0].ToString();
                        var opType = opChar switch
                        {
                            "+" => TokenType.PLUS,
                            "-" => TokenType.MINUS,
                            "*" => TokenType.MULTIPLY,
                            "%" => TokenType.DIVIDE,
                            _ => TokenType.PLUS
                        };
                        // x+: y => x: x + y
                        var currentVal = ASTNode.MakeVariable(variableName);
                        var modifiedValue = ASTNode.MakeDyadicOp(opType, currentVal, right);
                        return ASTNode.MakeAssignment(variableName, modifiedValue);
                    }
                    return ASTNode.MakeAssignment(variableName, right);
                }
                return ASTNode.MakeDyadicOp(TokenType.COLON, left, right);
            }

            // Handle global assignment operator (::)
            if (!IsAtEnd() && CurrentToken().Type == TokenType.GLOBAL_ASSIGNMENT)
            {
                Match(TokenType.GLOBAL_ASSIGNMENT);
                var right = ParseBracketArgument();
                if (right == null)
                {
                    throw new Exception("Expected expression after ::");
                }
                if (left.Type == ASTNodeType.Variable)
                {
                    var variableName = left.Value is SymbolValue sym ? sym.Value : left.Value?.ToString() ?? "";
                    return ASTNode.MakeGlobalAssignment(variableName, right);
                }
                return ASTNode.MakeDyadicOp(TokenType.GLOBAL_ASSIGNMENT, left, right);
            }

            // Check for "apply and assign" pattern: variable+: value
            // This happens when we have: variable operator: value
            // This must be checked BEFORE dyadic operator parsing to avoid consuming the operator token
            if (!IsAtEnd() && left.Type == ASTNodeType.Variable)
            {
                // Check if we have the pattern: variable+: value
                // Look ahead to see if there's an operator followed by colon
                var nextToken = PeekNext();
                
                if (nextToken != null && IsDyadicOperator(nextToken.Type))
                {
                    // Check if the token after the operator is a colon
                    var tokenAfterOperator = PeekToken(2); // Look two tokens ahead
                    
                    if (tokenAfterOperator != null && tokenAfterOperator.Type == TokenType.COLON)
                    {
                        // This is the "apply and assign" pattern
                        var variableName = left.Value is SymbolValue varSym ? varSym.Value : left.Value?.ToString() ?? "";
                        var operatorToken = nextToken.Type;
                        
                        // Skip the operator and colon
                        Advance(); // Skip the operator token
                        Advance(); // Skip the colon token
                        
                        // Parse the right argument after the colon
                        var rightArgument = ParseBracketArgument();
                        if (rightArgument == null)
                        {
                            throw new Exception("Expected expression after operator in apply and assign");
                        }
                        
                        // Create the apply and assign node
                        return ASTNode.MakeApplyAndAssign(variableName, operatorToken, rightArgument);
                    }
                }
            }

            // Handle dyadic operators but stop at semicolon or right bracket
            while (VerbRegistry.IsDyadicOperator(CurrentToken().Type))
            {
                var op = CurrentToken().Type;
                Match(op); // Consume the operator
                
                // Check if this is followed by an adverb
                if (VerbRegistry.IsAdverbToken(CurrentToken().Type))
                {
                    var adverbToken = CurrentToken();
                    Advance(); // Consume adverb token
                    var firstAdverbType = adverbToken.Type.ToString().Replace("TokenType.", "");
                    
                    // Convert the dyadic operator to a verb symbol
                    var verbName = VerbRegistry.GetDyadicOperatorSymbol(op);
                    var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                    
                    // Parse the right side of the adverb
                    var rightSide = ParseBracketArgument();
                    
                    // Check if this is an incomplete expression (should create a projected function)
                    if (rightSide == null)
                    {
                        // Create a projected function for the adverb operation
                        var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                        projectedNode.Value = new SymbolValue(firstAdverbType);
                        
                        // Add the verb as the first child
                        if (verbNode != null) projectedNode.Children.Add(verbNode);
                        
                        // Add the left operand if it exists
                        if (left != null) projectedNode.Children.Add(left);
                        
                        // Mark as needing 1 more argument (for the right side)
                        projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1)));
                        
                        left = projectedNode;
                    }
                    else
                    {
                        // Create the correct adverb structure: ADVERB(verb, left, right)
                        var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
                        adverbNode.Value = new SymbolValue(firstAdverbType);
                        if (verbNode != null) adverbNode.Children.Add(verbNode);
                        if (left != null) adverbNode.Children.Add(left);
                        if (rightSide != null) adverbNode.Children.Add(rightSide);
                        
                        left = adverbNode;
                    }
                }
                else
                {
                    // Regular dyadic operation with right-associativity
                    var right = ParseBracketArgument();
                    if (right == null)
                    {
                        throw new Exception($"Expected right operand after {op}");
                    }
                    return left != null ? ASTNode.MakeDyadicOp(op, left, right) : ASTNode.MakeDyadicOp(op, new ASTNode(ASTNodeType.Literal, new NullValue()), right);
                }
            }

            return left;
        }
    }
}
