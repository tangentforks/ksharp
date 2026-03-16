namespace K3CSharp
{
    public partial class Parser
    {
        /// <summary>
        /// Parse a term - handles primary expressions, adverbs, and vector parsing
        /// </summary>
        private ASTNode? ParseTerm(bool parseUntilEnd = false)
        {
            // Only return null for EOF, not for NEWLINE when parsing expressions
            // NEWLINE should be handled at higher levels as statement separators
            if (IsAtEnd() || CurrentToken().Type == TokenType.EOF)
            {
                return null;
            }
            
            ASTNode? result;
            
            // Check if current token is an adverb - if so, we're in a nesting context
            // Don't create placeholder for adverb operations - let ParseTerm handle them
            if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
            {
                // Parse the primary expression normally - adverb will be handled in ParseTerm
                result = ParsePrimary();
            }
            else
            {
                result = ParsePrimary();
            }

            // Handle case where ParsePrimary returned null
            if (result == null)
            {
                return null;
            }

            // Handle high-precedence adverb operations (verb-adverb binding has higher precedence than operators)
            if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
            {
                var adverbToken = PreviousToken() ?? CurrentToken();
                var adverbType = CurrentToken().Type;
                Match(adverbType); // Consume the adverb token
                
                // Parse the right argument for the adverb
                var rightArg = ParseTerm(parseUntilEnd);
                
                // Create adverb node: ADVERB(adverbType, verb, rightArg)
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString());
                adverbNode.Children.Add(result); // verb
                adverbNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(0))); // left argument (default for dyadic adverbs)
                if (rightArg != null) adverbNode.Children.Add(rightArg); // right argument
                result = adverbNode;
            }

            // Handle postfix operations: bracket notation for indexing or function calls
            while (!IsAtEnd() && CurrentToken().Type == TokenType.LEFT_BRACKET)
            {
                // Parse bracket notation: expression[index] or function[args]
                Match(TokenType.LEFT_BRACKET); // Consume '['

                // Parse bracket arguments with proper semicolon separation
                // ParseBracketContentsAsCommaEnlisted handles ']' consumption internally
                var argsExpression = ParseBracketContentsAsCommaEnlisted();

                // Special case: if the result is a unary operator (like _sqrt), treat brackets as grouping
                // So _sqrt[3] becomes _sqrt 3, not _sqrt @ [3]
                if (result != null && result.Type == ASTNodeType.BinaryOp && result.Value is SymbolValue opSymbol && 
                    opSymbol.Value.ToString().StartsWith("_") && result.Children.Count == 1)
                {
                    // This is a unary operator with brackets - treat as grouping
                    // Replace the unary operator's child with the bracket contents
                    if (argsExpression != null)
                    {
                        result.Children[0] = argsExpression;
                    }
                    // Don't consume more brackets - break the loop
                    break;
                }

                // Special case: if the result is a literal underscore function (like _ic), treat brackets as grouping
                // So _ic[x] becomes _ic x, not _ic @ [x]
                if (result != null && result.Type == ASTNodeType.Literal && result.Value is SymbolValue literalSymbol && 
                    literalSymbol.Value.ToString().StartsWith("_"))
                {
                    // This is a literal underscore function with brackets - convert to unary operator
                    var unaryNode = new ASTNode(ASTNodeType.BinaryOp);
                    unaryNode.Value = literalSymbol;
                    if (argsExpression != null)
                    {
                        unaryNode.Children.Add(argsExpression);
                    }
                    result = unaryNode;
                    // Don't consume more brackets - break the loop
                    break;
                }

                // Check if this is a control flow verb function call
                if (result != null && result.Type == ASTNodeType.Variable)
                {
                    var varName = result.Value is SymbolValue symbol ? symbol.Value : result.Value?.ToString() ?? "";
                    if (varName == "if" || varName == "while" || varName == "do")
                    {
                        // Create function call for control flow verbs
                        var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                        funcCall.Children.Add(result);

                        // For control flow verbs, always add the arguments as a single expression
                        // The control flow functions will handle parsing the vector internally
                        if (argsExpression != null) funcCall.Children.Add(argsExpression);

                        result = funcCall;
                    }
                    else
                    {
                        // Convert to apply operation: expression @ index
                        result = argsExpression != null && result != null ? ASTNode.MakeBinaryOp(TokenType.APPLY, result, argsExpression) : result;
                    }
                }
                else
                {
                    // Convert to apply operation: expression @ index
                    result = argsExpression != null && result != null ? ASTNode.MakeBinaryOp(TokenType.APPLY, result, argsExpression) : result;
                }
            }

            // If we processed bracket notation, return the result (don't continue with vector parsing)
            if (result != null && result.Type == ASTNodeType.BinaryOp && result.Value is SymbolValue symbolValue && symbolValue.Value == ".")
            {
                return result;
            }

            // Continue with regular term parsing for vectors
            var elements = new List<ASTNode>();
            if (result != null) elements.Add(result);
            var firstElementType = result?.Type ?? ASTNodeType.Literal;
            var firstValueType = result?.Value?.GetType();
            var stopTokens = parseUntilEnd ? ParseUntilEndStopTokens : DefaultStopTokens;

            while (!ShouldStopParsing(stopTokens))
            {
                // Check if this would create a mixed-type vector
                // If current token is an operator, it would create a mixed-type vector
                // So stop parsing and let ParseExpression handle it
                if (IsBinaryOperator(CurrentToken().Type) ||
                    CurrentToken().Type == TokenType.DV || CurrentToken().Type == TokenType.DI ||
                    CurrentToken().Type == TokenType.SETENV || CurrentToken().Type == TokenType.SM ||
                    CurrentToken().Type == TokenType.DIV || CurrentToken().Type == TokenType.DRAW ||
                    CurrentToken().Type == TokenType.IO_VERB_0 || CurrentToken().Type == TokenType.IO_VERB_1 ||
                    CurrentToken().Type == TokenType.IO_VERB_2 || CurrentToken().Type == TokenType.IO_VERB_3 ||
                    CurrentToken().Type == TokenType.IO_VERB_6 || CurrentToken().Type == TokenType.IO_VERB_7 ||
                    CurrentToken().Type == TokenType.IO_VERB_8 || CurrentToken().Type == TokenType.IO_VERB_9)
                {
                    break;
                }
                
                if (CurrentToken().Type == TokenType.SYMBOL && CurrentToken().Lexeme == "_dv" || CurrentToken().Lexeme == "_di")
                {
                    var symbol = CurrentToken().Lexeme;
                    Match(TokenType.SYMBOL);
                    var rightArg = ParseTerm(parseUntilEnd);
                    var binaryOp = new ASTNode(ASTNodeType.BinaryOp);
                    binaryOp.Value = new SymbolValue(symbol);
                    if (result != null) binaryOp.Children.Add(result);
                    if (rightArg != null) binaryOp.Children.Add(rightArg);
                    result = binaryOp;
                    break;
                }
                
                var nextElement = ParsePrimary();
                
                // Special case: function calls (variable followed by expression)
                // Check if the first element is a variable and the next element is any expression
                if (firstElementType == ASTNodeType.Variable && nextElement != null)
                {
                    // This is a function call: functionName argument
                    var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                    funcCall.Children.Add(elements[0]); // Function name
                    if (nextElement != null) funcCall.Children.Add(nextElement);  // Argument
                    return funcCall;
                }
                
                // Special case: compact symbol vectors (symbols back-to-back without spaces)
                // Check if the first element was a symbol and the current element is also a symbol
                bool firstIsSymbol = (firstElementType == ASTNodeType.Literal && firstValueType == typeof(SymbolValue)) ||
                                  (firstElementType == ASTNodeType.Variable && firstValueType == typeof(SymbolValue));
                bool nextIsSymbol = (nextElement?.Type == ASTNodeType.Literal && nextElement?.Value is SymbolValue) ||
                                  (nextElement?.Type == ASTNodeType.Variable && nextElement?.Value is SymbolValue);
                
                if (firstIsSymbol && nextIsSymbol)
                {
                    // Both are symbols, allow them to be combined regardless of uniform type check
                    if (nextElement != null) elements.Add(nextElement);
                    continue;
                }
                
                // Check for type uniformity
                if (nextElement?.Type != firstElementType || 
                    (nextElement?.Value?.GetType() != firstValueType && firstValueType != null))
                {
                    // Check if this is int/float mixing - allow it as it will be converted to float
                    bool firstIsNumber = elements[0]?.Value is IntegerValue || elements[0]?.Value is LongValue || elements[0]?.Value is FloatValue;
                    bool nextIsNumber = nextElement?.Value is IntegerValue || nextElement?.Value is LongValue || nextElement?.Value is FloatValue;
                    
                    if (firstIsNumber && nextIsNumber)
                    {
                        if (nextElement != null) elements.Add(nextElement);
                    }
                    else
                    {
                        // Mixed types detected - this should be an arithmetic expression, not a vector
                        // Put the element back and let ParseExpression handle it
                        // For now, just break the vector parsing
                        break;
                    }
                }
                else
                {
                    if (nextElement != null) elements.Add(nextElement);
                }
            }

            if (elements.Count > 1)
            {
                // Check if this might be a function call (variable followed by arguments)
                if (elements[0].Type == ASTNodeType.Variable)
                {
                    // Treat as function call: variable is the function, rest are arguments
                    var functionNode = elements[0];
                    var arguments = elements.Skip(1).ToList();
                    return ASTNode.MakeFunctionCall(functionNode, arguments);
                }
                
                return ASTNode.MakeVector(elements);
            }

            return elements[0];
        }

        /// <summary>
        /// Check if a node represents a vector literal
        /// </summary>
        private bool IsVectorLiteral(ASTNode node)
        {
            // A vector literal is a node that was parsed as a vector
            // This is a simple heuristic - if it has multiple children and they're all literals/variables
            if (node.Type == ASTNodeType.Vector && node.Children.Count > 0)
            {
                return true;
            }
            
            // Also check if it's a variable that we know represents a vector from context
            // For now, we'll be conservative and only treat explicit vectors as vector literals
            return false;
        }
    }
}
