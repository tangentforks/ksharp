namespace K3CSharp 
{
    partial class Parser
    {
private ASTNode? ParsePrimary()
        {
            ASTNode? result = null;

            // Handle NEWLINE tokens as statement separators (per NSL parser insight)
            if (Match(TokenType.NEWLINE))
            {
                // NEWLINE should be handled at higher levels as statement separator
                // Return null to indicate no primary expression found
                return null;
            }

            // Handle SEMICOLON tokens as expression separators (for empty positions)
            // Check without consuming so higher levels can handle it
            if (!IsAtEnd() && CurrentToken().Type == TokenType.SEMICOLON)
            {
                // SEMICOLON should be handled at higher levels as expression separator
                // Return null to indicate empty position in semicolon-separated list
                return null;
            }

            if (Match(TokenType.INTEGER))
            {
                var lexeme = PreviousToken().Lexeme;
                // Check if it's a special value
                if (lexeme == "0I" || lexeme == "0N" || lexeme == "-0I")
                    result = ASTNode.MakeLiteral(new IntegerValue(lexeme));
                else
                {
                    int value = Int32.Parse(lexeme);
                    // Convert extreme values to special values per spec
                    if (value >= 2147483647)
                        result = ASTNode.MakeLiteral(new IntegerValue("0I"));
                    else if (value <= -2147483648)
                        result = ASTNode.MakeLiteral(new IntegerValue("-0I"));
                    else
                        result = ASTNode.MakeLiteral(new IntegerValue(value));
                }
            }
            else if (Match(TokenType.LONG))
            {
                var lexeme = PreviousToken().Lexeme;
                // Check if it's a special long value
                if (lexeme == "0Ij")
                    result = ASTNode.MakeLiteral(new LongValue(long.MaxValue));
                else if (lexeme == "0Nj")
                    result = ASTNode.MakeLiteral(new LongValue(long.MinValue));
                else if (lexeme == "-0Ij")
                    result = ASTNode.MakeLiteral(new LongValue(-long.MaxValue));
                else
                {
                    // Parse with bounds checking
                    var numberPart = lexeme.Substring(0, lexeme.Length - 1); // Remove 'j'
                    double parsedValue = double.Parse(numberPart);
                    
                    if (parsedValue >= long.MaxValue)
                        result = ASTNode.MakeLiteral(new LongValue(long.MaxValue));
                    else if (parsedValue <= -long.MaxValue)
                        result = ASTNode.MakeLiteral(new LongValue(-long.MaxValue));
                    else
                        result = ASTNode.MakeLiteral(new LongValue(long.Parse(numberPart)));
                }
            }
            else if (Match(TokenType.FLOAT))
            {
                var lexeme = PreviousToken().Lexeme;
                // Check if it's a special float value
                if (lexeme == "0i" || lexeme == "0n" || lexeme == "-0i")
                    result = ASTNode.MakeLiteral(new FloatValue(lexeme));
                else
                    result = ASTNode.MakeLiteral(new FloatValue(double.Parse(lexeme)));
            }
            else if (Match(TokenType.NULL))
            {
                result = ASTNode.MakeLiteral(new NullValue());
            }
            else if (Match(TokenType.CHARACTER))
            {
                var value = PreviousToken().Lexeme;
                result = ASTNode.MakeLiteral(new CharacterValue(value));
            }
            else if (Match(TokenType.CHARACTER_VECTOR))
            {
                var value = PreviousToken().Lexeme;
                
                // K type rules: length 1 = Character (type 3), length 0 or >1 = Character Vector (type -3)
                if (value.Length == 1)
                {
                    // Single character - create CharacterValue (type 3)
                    result = ASTNode.MakeLiteral(new CharacterValue(value));
                }
                else
                {
                    // Character vector - create VectorValue containing individual CharacterValue objects (type -3)
                    var charVector = new List<K3Value>();
                    foreach (char c in value)
                    {
                        charVector.Add(new CharacterValue(c.ToString()));
                    }
                    result = ASTNode.MakeLiteral(new VectorValue(charVector, -3)); // -3 = character vector type
                }
            }
            else if (Match(TokenType.SYMBOL))
            {
                var symbol = PreviousToken().Lexeme;
                
                // All symbols (including those with periods) should be treated as symbol literals
                // Symbol literals are created with backtick syntax and should not be variable references
                // The lexer already validates symbol names, so no additional validation needed here
                result = ASTNode.MakeLiteral(new SymbolValue(symbol));
            }
            else if (Match(TokenType.HINT))
            {
                // Handle _hint as a variable reference to the built-in function
                result = ASTNode.MakeVariable("_hint");
            }
            else if (Match(TokenType.GETHINT))
            {
                // Handle _gethint as a variable reference to the built-in function
                result = ASTNode.MakeVariable("_gethint");
            }
            else if (Match(TokenType.SETHINT))
            {
                // Handle _sethint as a variable reference to the built-in function
                result = ASTNode.MakeVariable("_sethint");
            }
            else if (Match(TokenType.IDENTIFIER))
            {
                var identifier = PreviousToken().Lexeme;
                
                // Check for special hint functions
                if (identifier == "_gethint" || identifier == "_sethint")
                {
                    // Handle as variable references to built-in functions
                    result = ASTNode.MakeVariable(identifier);
                }
                else
                {
                    // Handle as regular identifier (variable reference)
                    result = ASTNode.MakeVariable(identifier);
                }
            }
            else if (Match(TokenType.DISPOSE))
            {
                // Handle _dispose token as a symbol literal
                result = ASTNode.MakeLiteral(new SymbolValue("_dispose"));
            }
            else if (Match(TokenType.PLUS))
            {
                // Check if this is unary transpose (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("+"));
                    }
                    else if (!IsAtEnd() && CurrentToken().Type == TokenType.DOT_APPLY)
                    {
                        // This is a dyadic operator with dot-apply (e.g., + . (3; 5))
                        // Treat it as a dyadic operator symbol
                        result = ASTNode.MakeLiteral(new SymbolValue("+"));
                    }
                    else
                    {
                        // Check if this is a standalone plus operator at the end of an expression
                        if (IsAtEnd() || CurrentToken().Type == TokenType.RIGHT_PAREN || 
                            CurrentToken().Type == TokenType.RIGHT_BRACKET || CurrentToken().Type == TokenType.RIGHT_BRACE ||
                            CurrentToken().Type == TokenType.SEMICOLON || CurrentToken().Type == TokenType.NEWLINE)
                        {
                            // This is just the plus operator itself - treat as variable
                            result = ASTNode.MakeVariable("+");
                        }
                        else
                        {
                            // This is unary transpose
                            var operand = ParsePrimary();
                            var node = new ASTNode(ASTNodeType.BinaryOp);
                            node.Value = new SymbolValue("+");
                            if (operand != null) node.Children.Add(operand);
                            return node;
                        }
                    }
                }
                else
                {
                    // Binary plus symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("+"));
                }
            }
            else if (Match(TokenType.MINUS))
            {
                // Check if this is unary minus (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("-"));
                    }
                    else if (!IsAtEnd() && CurrentToken().Type == TokenType.DOT_APPLY)
                    {
                        // This is a dyadic operator with dot-apply (e.g., - . (10; 3))
                        // Treat it as a dyadic operator symbol
                        result = ASTNode.MakeLiteral(new SymbolValue("-"));
                    }
                    else
                    {
                        // This is unary minus
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("-");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary minus symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("-"));
                }
            }
            else if (Match(TokenType.DIVIDE))
            {
                // Check if this is unary reciprocal (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("%"));
                    }
                    else if (!IsAtEnd() && CurrentToken().Type == TokenType.DOT_APPLY)
                    {
                        // This is a dyadic operator with dot-apply (e.g., % . (20; 4))
                        // Treat it as a dyadic operator symbol
                        result = ASTNode.MakeLiteral(new SymbolValue("%"));
                    }
                    else
                    {
                        // This is unary reciprocal
                        var operand = ParsePrimary();
                        if (operand == null)
                        {
                            // Create a projected function instead of an empty binary op
                            var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                            projectedNode.Value = new SymbolValue("%");
                            projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1))); // Needs 1 more argument
                            return projectedNode;
                        }
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("%");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary division symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("%"));
                }
            }
            else if (Match(TokenType.MULTIPLY))
            {
                // Check if this is unary first (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("*"));
                    }
                    else if (!IsAtEnd() && CurrentToken().Type == TokenType.DOT_APPLY)
                    {
                        // This is a dyadic operator with dot-apply (e.g., * . (4; 6))
                        // Treat it as a dyadic operator symbol
                        result = ASTNode.MakeLiteral(new SymbolValue("*"));
                    }
                    else
                    {
                        // This is unary first
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("*");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary multiplication symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("*"));
                }
            }
            else if (Match(TokenType.MIN))
            {
                // Check if this is unary min (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("&"));
                    }
                    else
                    {
                        // This is unary where (&)
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("&");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary min symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("&"));
                }
            }
            else if (Match(TokenType.MAX))
            {
                // Check if this is unary max (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("|"));
                    }
                    else
                    {
                        // This is unary reverse (|)
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("|");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary max symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("|"));
                }
            }
            else if (Match(TokenType.LESS))
            {
                // Check if this is unary grade up (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("<"));
                    }
                    else
                    {
                        // This is unary grade up
                        var operand = ParseTerm(parseUntilEnd: true);
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("<");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary less symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("<"));
                }
            }
            else if (Match(TokenType.GREATER))
            {
                // Check if this is unary grade down (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue(">"));
                    }
                    else
                    {
                        // This is unary grade down
                        var operand = ParseTerm(parseUntilEnd: true);
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue(">");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary greater symbol
                    result = ASTNode.MakeLiteral(new SymbolValue(">"));
                }
            }
            else if (Match(TokenType.POWER))
            {
                // Check if this is unary shape (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("^"));
                    }
                    else
                    {
                        // This is unary shape
                        var operand = ParseTerm();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("^");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary power symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("^"));
                }
            }
            else if (Match(TokenType.JOIN))
            {
                // Check if this is unary enlist (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue(","));
                    }
                    else
                    {
                        // This is unary enlist
                        var operand = ParseTerm();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue(",");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary join symbol
                    result = ASTNode.MakeLiteral(new SymbolValue(","));
                }
            }
            else if (Match(TokenType.IDENTIFIER))
            {
                var identifier = PreviousToken().Lexeme;
                
                // Check if this is a dotted notation for K tree access
                if (identifier.Contains("."))
                {
                    // This is a K tree dotted notation variable
                    result = ASTNode.MakeVariable(identifier);
                }
                else
                {
                    // Regular identifier
                    result = ASTNode.MakeVariable(identifier);
                }
            }
            else if (Match(TokenType.COLON))
            {
                // Check if this is monadic colon (return operator) by looking ahead
                // If the next token is not an identifier or variable, it's likely monadic
                if (IsAtEnd() || (!IsIdentifierToken(CurrentToken().Type) && CurrentToken().Type != TokenType.LEFT_BRACKET))
                {
                    // Monadic colon - return operator
                    var operand = ParseExpression();
                    var node = new ASTNode(ASTNodeType.BinaryOp);
                    node.Value = new SymbolValue(":");
                    if (operand != null) node.Children.Add(operand);
                    return node;
                }
                else
                {
                    // This will be handled as binary operator (assignment)
                    result = ASTNode.MakeLiteral(new SymbolValue(":"));
                }
            }
            // Dyadic operators as primary expressions for bracket notation
            else if (Match(TokenType.PLUS))
            {
                result = ASTNode.MakeVariable("+");
            }
            else if (Match(TokenType.MINUS))
            {
                result = ASTNode.MakeVariable("-");
            }
            else if (Match(TokenType.MULTIPLY))
            {
                result = ASTNode.MakeVariable("*");
            }
            else if (Match(TokenType.DIVIDE))
            {
                result = ASTNode.MakeVariable("%");
            }
            else if (Match(TokenType.POWER))
            {
                result = ASTNode.MakeVariable("*");
            }
            else if (Match(TokenType.MODULUS))
            {
                result = ASTNode.MakeVariable("!");
            }
            else if (Match(TokenType.LESS))
            {
                result = ASTNode.MakeVariable("<");
            }
            else if (Match(TokenType.GREATER))
            {
                result = ASTNode.MakeVariable(">");
            }
            else if (Match(TokenType.EQUAL))
            {
                result = ASTNode.MakeVariable("=");
            }
            else if (Match(TokenType.JOIN))
            {
                result = ASTNode.MakeVariable(",");
            }
            else if (Match(TokenType.LEFT_PAREN))
            {
                // Check for empty parentheses () which should be an empty list
                if (Match(TokenType.RIGHT_PAREN))
                {
                    result = ASTNode.MakeVector(new List<ASTNode>());
                }
                else
                {
                    // Parse parenthesized expression - inside parentheses, semicolons create lists
                    // Handle leading semicolons for null elements: (;1;2)
                    ASTNode? firstExpr = null;
                    bool hasSemicolon = false;

                    if (CurrentToken().Type == TokenType.SEMICOLON)
                    {
                        // Leading semicolon: first element is null
                        firstExpr = ASTNode.MakeLiteral(new NullValue());
                        hasSemicolon = true;
                        Advance(); // consume the semicolon
                    }
                    else
                    {
                        firstExpr = ParseBracketArgument();
                        if (firstExpr == null)
                        {
                            throw new Exception("Expected expression after '('");
                        }
                        hasSemicolon = Match(TokenType.SEMICOLON);
                    }

                    if (hasSemicolon)
                    {
                        // Semicolon-separated list
                        var elements = new List<ASTNode> { firstExpr! };
                        do
                        {
                            // Check for empty position (consecutive semicolons or trailing)
                            if (!IsAtEnd() && (CurrentToken().Type == TokenType.SEMICOLON || CurrentToken().Type == TokenType.RIGHT_PAREN))
                            {
                                // Empty position → null element
                                elements.Add(ASTNode.MakeLiteral(new NullValue()));
                            }
                            else
                            {
                                var nextExpr = ParseBracketArgument();
                                if (nextExpr != null)
                                    elements.Add(nextExpr);
                                else
                                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                            }
                        } while (Match(TokenType.SEMICOLON));

                        if (!Match(TokenType.RIGHT_PAREN))
                        {
                            throw new Exception("Expected ')' after expression");
                        }
                        result = ASTNode.MakeVector(elements);
                    }
                    else
                    {
                        if (!Match(TokenType.RIGHT_PAREN))
                        {
                            throw new Exception("Expected ')' after expression");
                        }
                        result = firstExpr!;
                    }
                }
                
                // After parsing a parenthesized expression, check for dot-apply
                if (result != null && !IsAtEnd() && CurrentToken().Type == TokenType.DOT_APPLY)
                {
                    // This is a case of: (expression) . argument
                    // We need to parse the argument after the dot
                    Advance(); // Skip the DOT_APPLY token
                    var arg = ParseExpressionWithoutSemicolons();
                    if (arg != null)
                    {
                        result = ASTNode.MakeBinaryOp(TokenType.DOT_APPLY, result, arg);
                    }
                }
            }
            else if (Match(TokenType.MATCH))
            {
                // Check if this is unary match (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("~"));
                    }
                    else
                    {
                        // This is unary match (logical negate or attribute handle)
                        var operand = ParseExpression();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("~");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary match operator - create proper binary operation
                    var right = ParseTerm();
                    if (right != null)
                    {
                        result = ASTNode.MakeBinaryOp(TokenType.MATCH, result, right);
                    }
                }
            }
            else if (Match(TokenType.DOLLAR))
            {
                // Check if this is unary format (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("$"));
                    }
                    else
                    {
                        // This is unary format
                        var operand = ParseExpression();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("$");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary format symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("$"));
                }
            }
            else if (Match(TokenType.HASH))
            {
                // Check if this is unary count (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("#"));
                    }
                    // Check if this is monadic disambiguation (#:)
                    else if (!IsAtEnd() && CurrentToken().Type == TokenType.COLON)
                    {
                        // This is monadic disambiguation - consume the colon and treat as monadic #
                        Advance(); // Consume the COLON
                        result = ASTNode.MakeLiteral(new SymbolValue("#"));
                    }
                    else
                    {
                        // This is unary count
                        var operand = ParseTerm();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("#");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary hash symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("#"));
                }
            }
            else if (Match(TokenType.UNDERSCORE))
            {
                // Check if this is unary floor (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("_"));
                    }
                    else
                    {
                        // This is unary floor
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("_");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary underscore will be handled in ParseExpression, so treat as symbol here
                    result = ASTNode.MakeLiteral(new SymbolValue("_"));
                }
            }
            else if (Match(TokenType.QUESTION))
            {
                // Check if this is unary unique (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("?"));
                    }
                    else
                    {
                        // This is unary unique
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("?");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary question symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("?"));
                }
            }
            else if (Match(TokenType.MODULUS))
            {
                // Check if this is unary enumerate (at start of expression)
                if (result == null)
                {
                    // This is unary enumerate
                    var operand = ParsePrimary();
                    var node = new ASTNode(ASTNodeType.BinaryOp);
                    node.Value = new SymbolValue("!");
                    if (operand != null) node.Children.Add(operand);
                    return node;
                }
                else
                {
                    // Binary modulus symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("!"));
                }
            }
            else if (Match(TokenType.ADVERB_SLASH))
            {
                // Standalone adverb slash - this should not happen in valid K3
                // According to spec, slash must be preceded by a verb without spaces
                throw new Exception("Adverb slash must be preceded by a verb");
            }
            else if (Match(TokenType.ADVERB_BACKSLASH))
            {
                // Standalone adverb backslash - this should not happen in valid K3
                throw new Exception("Adverb backslash must be preceded by a verb");
            }
            else if (Match(TokenType.ADVERB_TICK))
            {
                // Adverb tick - this should be handled by ParseTerm
                // Don't throw an exception - let ParseTerm handle it
                result = ASTNode.MakeLiteral(new SymbolValue("'"));
            }
            else if (Match(TokenType.LOG))
            {
                // Mathematical logarithm operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_log");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.EXP))
            {
                // Mathematical exponential operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_exp");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ABS))
            {
                // Mathematical absolute value operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_abs");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SQR))
            {
                // Mathematical square operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sqr");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SQRT))
            {
                // Mathematical square root operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sqrt");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.FLOOR_MATH))
            {
                // Mathematical floor operation (always returns float)
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_floor");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.NOT))
            {
                // Bitwise NOT operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_not");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.CEIL))
            {
                // Mathematical ceiling operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ceil");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DOT_PRODUCT))
            {
                // Linear algebra dot product operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_dot");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MUL))
            {
                // Linear algebra matrix multiplication operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_mul");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.INV))
            {
                // Linear algebra matrix inverse operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_inv");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SIN))
            {
                // Mathematical sine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sin");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.COS))
            {
                // Mathematical cosine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_cos");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TAN))
            {
                // Mathematical tangent operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_tan");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ASIN))
            {
                // Mathematical arcsine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_asin");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ACOS))
            {
                // Mathematical arccosine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_acos");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ATAN))
            {
                // Mathematical arctangent operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_atan");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SINH))
            {
                // Mathematical hyperbolic sine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sinh");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.COSH))
            {
                // Mathematical hyperbolic cosine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_cosh");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TANH))
            {
                // Mathematical hyperbolic tangent operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_tanh");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TIME))
            {
                // Current time function - niladic, create function call
                var functionNode = ASTNode.MakeVariable("_t");
                return ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
            }
            else if (Match(TokenType.DIRECTORY))
            {
                // Directory function - niladic, create function call
                var functionNode = ASTNode.MakeVariable("_d");
                return ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
            }
            else if (Match(TokenType.BD))
            {
                // BD function - unary, create function call
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_bd");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DB))
            {
                // DB function - unary, create function call
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_db");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.GTIME))
            {
                // GMT time conversion function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_gtime");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LTIME))
            {
                // Local time conversion function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ltime");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LT))
            {
                // Local time offset function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_lt");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.JD))
            {
                // Julian date conversion function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_jd");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DJ))
            {
                // Date from Julian conversion function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_dj");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.VS))
            {
                // _vs is dyadic, provide helpful error for monadic usage
                throw new Exception("_vs (vector from scalar) requires two arguments - use infix notation: base _vs scalar");
            }
            else if (Match(TokenType.DRAW))
            {
                // _draw is dyadic, provide helpful error for monadic usage
                throw new Exception("_draw requires dyadic call (left and right arguments)");
            }
            else if (Match(TokenType.GETENV))
            {
                // _getenv is monadic, create function call
                var functionNode = ASTNode.MakeVariable("_getenv");
                var operand = ParseExpression();
                var callNode = ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
                if (operand != null) callNode.Children.Add(operand);
                return callNode;
            }
            else if (Match(TokenType.SETENV))
            {
                result = ASTNode.MakeVariable("_setenv");
            }
            else if (Match(TokenType.SIZE))
            {
                // _size is monadic, create function call
                var functionNode = ASTNode.MakeVariable("_size");
                var operand = ParseExpression();
                var callNode = ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
                if (operand != null) callNode.Children.Add(operand);
                return callNode;
            }
            else if (Match(TokenType.SS))
            {
                // Database function - dyadic, needs two operands
                var leftOperand = ParseExpression();
                if (!Match(TokenType.SEMICOLON))
                    throw new Exception("_ss: Expected semicolon after left operand");
                var rightOperand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ss");
                if (leftOperand != null) node.Children.Add(leftOperand);
                if (rightOperand != null) node.Children.Add(rightOperand);
                return node;
            }
            else if (Match(TokenType.CI))
            {
                // Database function - check if followed by an adverb
                if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                {
                    // This is a monadic verb followed by an adverb - create a verb node
                    var node = new ASTNode(ASTNodeType.Literal);
                    node.Value = new SymbolValue("_ci");
                    return node;
                }
                else
                {
                    // Regular monadic verb with operand
                    var operand = ParseExpression();
                    var node = new ASTNode(ASTNodeType.BinaryOp);
                    node.Value = new SymbolValue("_ci");
                    if (operand != null) node.Children.Add(operand);
                    return node;
                }
            }
            else if (Match(TokenType.IC))
            {
                // Database function - check if followed by an adverb
                if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
                {
                    // This is a monadic verb followed by an adverb - create a verb node
                    var node = new ASTNode(ASTNodeType.Literal);
                    node.Value = new SymbolValue("_ic");
                    return node;
                }
                else
                {
                    // Regular monadic verb with operand
                    var operand = ParseExpression();
                    var node = new ASTNode(ASTNodeType.BinaryOp);
                    node.Value = new SymbolValue("_ic");
                    if (operand != null) node.Children.Add(operand);
                    return node;
                }
            }
            else if (Match(TokenType.EXIT))
            {
                // Exit function
                // Control flow function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_exit");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (CurrentToken().Type == TokenType.LEFT_BRACE)
            {
                // Capture the opening brace position before matching (since Match advances the token)
                int leftBracePos;
                
                Match(TokenType.LEFT_BRACE); // Consume the opening brace
                leftBracePos = PreviousToken().Position; // Get position of the opening brace we just consumed
                
                // Check if this is a form specifier (empty braces followed by $)
                if (CurrentToken().Type == TokenType.RIGHT_BRACE && 
                    !IsAtEnd() && PeekNext().Type == TokenType.DOLLAR)
                {
                    // This is {} form specifier
                    Match(TokenType.RIGHT_BRACE); // Consume the }
                    
                    // Create a special node for {} form specifier
                    var node = new ASTNode(ASTNodeType.FormSpecifier);
                    node.Value = new SymbolValue("{}");
                    return node;
                }
                
                // Parse function
                var parameters = new List<string>();
                
                // Check if there's a parameter list
                if (CurrentToken().Type == TokenType.LEFT_BRACKET)
                {
                    // Parse parameter list
                    Match(TokenType.LEFT_BRACKET); // Consume the [
                    
                    if (!Match(TokenType.RIGHT_BRACKET))
                    {
                        // First parameter
                        if (Match(TokenType.IDENTIFIER))
                        {
                            parameters.Add(PreviousToken().Lexeme);
                        }
                        
                        while (Match(TokenType.SEMICOLON))
                        {
                            if (Match(TokenType.IDENTIFIER))
                            {
                                parameters.Add(PreviousToken().Lexeme);
                            }
                        }
                        
                        if (!Match(TokenType.RIGHT_BRACKET))
                        {
                            throw new Exception("Expected ']' after parameter list");
                        }
                    }
                }
                
                // Parse function body
                // leftBracePos was already captured before matching the opening brace
                int bodyStartTokenIndex = current; // Store the token index where body starts
                ASTNode body;
                if (CurrentToken().Type == TokenType.RIGHT_BRACE)
                {
                    // Empty function body - create a block with no statements
                    body = new ASTNode(ASTNodeType.Block);
                }
                else
                {
                    // Parse the function body - handle multiple statements separated by semicolons or newlines
                    var statements = new List<ASTNode>();
                    
                    // Parse all statements, skipping empty ones
                    while (!IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_BRACE)
                    {
                        // Skip empty lines (multiple newlines) and whitespace
                        while (Match(TokenType.NEWLINE))
                        {
                            // Skip consecutive newlines
                        }
                        
                        // Check if we have content before the next separator or closing brace
                        if (!IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_BRACE &&
                            CurrentToken().Type != TokenType.SEMICOLON && CurrentToken().Type != TokenType.NEWLINE)
                        {
                            // Use ParseExpressionWithoutSemicolons to avoid greedily consuming
                            // newlines/semicolons that the function body parser needs to manage
                            var expr = ParseExpressionWithoutSemicolons();
                            if (expr != null)  // Only add non-null expressions
                            {
                                statements.Add(expr);
                            }
                        }

                        // Skip the separator (semicolon or newline) if present
                        // Treat newlines and semicolons as equivalent per NSL parser insight
                        if (Match(TokenType.SEMICOLON) || Match(TokenType.NEWLINE))
                        {
                            // Skip additional consecutive newlines after the separator
                            while (Match(TokenType.NEWLINE))
                            {
                                // Skip consecutive newlines
                            }
                        }
                    }
                    
                    // If we have multiple statements, create a block
                    if (statements.Count > 1)
                    {
                        body = new ASTNode(ASTNodeType.Block);
                        body.Children.AddRange(statements);
                    }
                    else if (statements.Count == 1)
                    {
                        body = statements[0];
                    }
                    else
                    {
                        // Empty function body - create a block with no statements
                        body = new ASTNode(ASTNodeType.Block);
                    }
                }
                
                // Store the closing brace position before matching (since Match advances the token)
                int rightBracePos = CurrentToken().Position; // Position of the closing brace
                
                if (!Match(TokenType.RIGHT_BRACE))
                {
                    throw new Exception("Expected '}' after function body");
                }
                
                // Extract the source text of the function body
                string? bodyText = "";
                List<Token>? preParsedTokens = null;
                
                // Collect tokens from body start to the closing brace (forward direction)
                // This properly handles nested functions by tracking brace depth
                var bodyTokens = new List<Token>();
                for (int ti = bodyStartTokenIndex; ti < current - 1; ti++) // current-1 is the closing brace
                {
                    bodyTokens.Add(tokens[ti]);
                }
                
                // If no explicit parameters were provided, extract implicit parameters from the body
                if (parameters.Count == 0 && bodyTokens.Count > 0)
                {
                    parameters = ExtractImplicitParameters(bodyTokens);
                }
                
                // Reconstruct the function body text from tokens
                if (bodyTokens.Count > 0)
                {
                    // Reconstruct body text preserving original spacing exactly
                    var bodyParts = new List<string>();
                    for (int i = 0; i < bodyTokens.Count; i++)
                    {
                        var token = bodyTokens[i];
                        var lexeme = ReconstructTokenLexeme(token);

                        // Check if there should be a space before this token based on original positions
                        bool addSpaceBefore = false;
                        if (i > 0)
                        {
                            var prevToken = bodyTokens[i - 1];
                            int prevEnd = prevToken.Position + prevToken.Lexeme.Length;
                            addSpaceBefore = (token.Position > prevEnd);
                        }
                        
                        if (addSpaceBefore)
                            bodyParts.Add(" " + lexeme);
                        else
                            bodyParts.Add(lexeme);
                    }
                    
                    bodyText = string.Join("", bodyParts);
                    
                    // Pre-parse the function body for better performance
                    try
                    {
                        var bodyLexer = new Lexer(bodyText);
                        preParsedTokens = bodyLexer.Tokenize();
                        
                        // For nested functions, we'll defer validation to runtime
                        // The main parser should handle nested function definitions correctly
                        // We only validate simple function bodies here
                        if (!bodyText.Contains("{[") && !bodyText.Contains("::"))
                        {
                            try
                            {
                                var testParser = new Parser(preParsedTokens, bodyText);
                                testParser.Parse();
                            }
                            catch
                            {
                                // Pre-parsing failed, defer to runtime parsing
                                preParsedTokens = null;
                            }
                        }
                        else
                        {
                            // For complex function bodies with nested functions, store as character vector
                            // and use dot execute at runtime for more robust evaluation
                            preParsedTokens = null; // Don't pre-parse complex bodies
                        }
                    }
                    catch
                    {
                        // If pre-parsing fails, we'll defer to runtime (per spec)
                        // This is acceptable - validation is deferred until execution
                        preParsedTokens = null;
                    }
                }
                else
                {
                    // Empty function body
                    bodyText = "";
                }
                
                result = ASTNode.MakeFunction(parameters, body);
                result.StartPosition = leftBracePos;
                result.EndPosition = rightBracePos;
                
                // Extract original source text for serialization
                string originalSourceText = "";
                if (leftBracePos >= 0 && rightBracePos > leftBracePos && rightBracePos <= sourceText.Length)
                {
                    originalSourceText = sourceText.Substring(leftBracePos, rightBracePos - leftBracePos + 1);
                }
                
                result.Value = new FunctionValue(bodyText ?? "", parameters, preParsedTokens ?? new List<Token>(), originalSourceText);
            }
            else if (Match(TokenType.TYPE))
            {
                // 4: operator is unary - parse the operand with full expression parsing
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("TYPE");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.STRING_REPRESENTATION))
            {
                // 5: operator is unary - parse the operand with full expression parsing
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("STRING_REPRESENTATION");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.IO_VERB_0) || Match(TokenType.IO_VERB_1) || Match(TokenType.IO_VERB_2) || Match(TokenType.IO_VERB_3) || 
                     Match(TokenType.IO_VERB_6) || Match(TokenType.IO_VERB_7) || Match(TokenType.IO_VERB_8) || Match(TokenType.IO_VERB_9))
            {
                // General digit-colon operator - could be monadic or dyadic
                // We need to determine arity by context in the evaluator
                var matchedToken = PreviousToken(); // Get the I/O verb token before parsing operand
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                
                // Store the digit for later use in evaluator
                int digit = VerbRegistry.GetIOVerbDigit(matchedToken.Type);
                
                node.Value = new SymbolValue($"IO_VERB_{digit}");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DOT_APPLY))
            {
                // Check if this is followed by LEFT_BRACKET for bracket syntax .[...]
                if (Match(TokenType.LEFT_BRACKET))
                {
                    // This is bracket syntax: .[...] - implement equivalence .[x] is (.) .,(x)
                    var bracketContents = ParseBracketContentsAsCommaEnlisted();
                    
                    // Create dot-apply node: (.) .,(x)
                    var dotApplyNode = new ASTNode(ASTNodeType.BinaryOp);
                    dotApplyNode.Value = new SymbolValue(".");
                    
                    // Left side is the dot operator itself
                    var dotNode = ASTNode.MakeVariable(".");
                    dotApplyNode.Children.Add(dotNode);
                    
                    // Create comma node for the ,(x) part
                    var commaNode = new ASTNode(ASTNodeType.BinaryOp);
                    commaNode.Value = new SymbolValue(",");
                    commaNode.Children.Add(bracketContents);
                    
                    dotApplyNode.Children.Add(commaNode);
                    result = dotApplyNode;
                }
                else if (!IsAtEnd() && (CurrentToken().Type == TokenType.IDENTIFIER || CurrentToken().Type == TokenType.SYMBOL))
                {
                    // Check if dot and identifier are adjacent (no space) → dotted notation .d
                    // vs separated by space → monadic dot applied to d
                    var dotToken = PreviousToken();
                    var nextToken = CurrentToken();
                    bool isAdjacent = (dotToken.Position + dotToken.Lexeme.Length) == nextToken.Position;

                    if (isAdjacent)
                    {
                        // This is K tree dotted notation: .k, .k.foo, etc.
                        var identifier = CurrentToken().Lexeme;
                        Advance(); // Consume the identifier/symbol

                        // Construct the dotted notation variable name
                        var dottedVariable = "." + identifier;

                        // Check if there are more dotted parts
                        while (Match(TokenType.DOT_APPLY) && !IsAtEnd() &&
                               (CurrentToken().Type == TokenType.IDENTIFIER || CurrentToken().Type == TokenType.SYMBOL))
                        {
                            dottedVariable += "." + CurrentToken().Lexeme;
                            Advance(); // Consume the identifier/symbol
                        }

                        result = ASTNode.MakeVariable(dottedVariable);
                    }
                    else
                    {
                        // Space between . and identifier → monadic dot applied to the expression
                        result = ASTNode.MakeVariable(".");
                    }
                }
                else
                {
                    // Check if this is a standalone dot operator at the end of an expression
                    if (IsAtEnd() || CurrentToken().Type == TokenType.RIGHT_PAREN || 
                        CurrentToken().Type == TokenType.RIGHT_BRACKET || CurrentToken().Type == TokenType.RIGHT_BRACE ||
                        CurrentToken().Type == TokenType.SEMICOLON || CurrentToken().Type == TokenType.NEWLINE)
                    {
                        // This is just the dot operator itself - treat as variable
                        result = ASTNode.MakeVariable(".");
                    }
                    else
                    {
                        // This is unary MAKE (.) operator
                        var operand = ParsePrimary();
                        var makeNode = new ASTNode(ASTNodeType.BinaryOp);
                        makeNode.Value = new SymbolValue(".");
                        if (operand != null) makeNode.Children.Add(operand);
                        return makeNode;
                    }
                }
            }
            else if (Match(TokenType.APPLY))
            {
                // Check if this is followed by LEFT_BRACKET for bracket syntax @[...]
                if (Match(TokenType.LEFT_BRACKET))
                {
                    // This is bracket syntax: @[...] - implement equivalence @[x] is (@) .,(x)
                    var bracketContents = ParseBracketContentsAsCommaEnlisted();
                    
                    // Create dot-apply node: (@) .,(x)
                    var dotApplyNode = new ASTNode(ASTNodeType.BinaryOp);
                    dotApplyNode.Value = new SymbolValue(".");
                    
                    // Left side is the at operator itself
                    var atNode = ASTNode.MakeVariable("@");
                    dotApplyNode.Children.Add(atNode);
                    
                    // Create comma node for the ,(x) part
                    var commaNode = new ASTNode(ASTNodeType.BinaryOp);
                    commaNode.Value = new SymbolValue(",");
                    commaNode.Children.Add(bracketContents);
                    
                    dotApplyNode.Children.Add(commaNode);
                    result = dotApplyNode;
                }
                else
                {
                    // This is unary ATOM (@) operator
                    var operand = ParseTerm();
                    var atomNode = new ASTNode(ASTNodeType.BinaryOp);
                    atomNode.Value = new SymbolValue("@");
                    if (operand != null) atomNode.Children.Add(operand);
                    return atomNode;
                }
            }
            else if (Match(TokenType.EOF))
            {
                // End of input - return null result
                return null;
            }
            else if (Match(TokenType.LEFT_BRACKET))
            {
                // Handle standalone brackets - this shouldn't normally happen in valid K code
                // but we'll parse it to avoid crashing
                var bracketContents = ParseBracketContentsAsCommaEnlisted();
                return bracketContents;
            }
            else
            {
                var currentToken = CurrentToken();
                throw new Exception($"Unexpected token: {currentToken.Type}({currentToken.Lexeme})");
            }

            return result;
        }
    }
}