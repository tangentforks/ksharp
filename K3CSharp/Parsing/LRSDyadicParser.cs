using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Dyadic operator parsing for LRS parser
    /// Handles dyadic operations with right-associative LRS semantics
    /// </summary>
    public class LRSDyadicParser
    {
        private readonly List<Token> tokens;
        private readonly LRSParser? parentParser;
        private readonly LRSGroupingParser? groupingParser;
        
        public LRSDyadicParser(List<Token> tokens, LRSParser? parentParser = null)
        {
            this.tokens = tokens;
            this.parentParser = parentParser;
            this.groupingParser = new LRSGroupingParser(tokens, parentParser?.BuildParseTree ?? false);
        }
        
        /// <summary>
        /// Find the rightmost (or leftmost in Pure LRS) dyadic operator in token list
        /// Safe LRS mode: Find rightmost operator (original behavior for fallback compatibility)
        /// Pure LRS mode: Find leftmost operator with grouping depth tracking (improved behavior)
        /// </summary>
        /// <param name="tokens">Tokens to search</param>
        /// <returns>Index of dyadic operator, or -1 if none found</returns>
        public int FindRightmostOperator(List<Token> tokens)
        {
            // LRS RULE: Always find LEFTMOST operator for true right-to-left evaluation
            // This ensures expressions like "5 + 2 * 3" parse as "5 + (2 * 3)" = 11
            // The leftmost operator splits: left side is atomic, right side is recursively parsed
            
            int depth = 0;
            
            for (int i = 0; i < tokens.Count; i++)
            {
                var currentToken = tokens[i];
                
                // Track grouping depth
                if (currentToken.Type == TokenType.LEFT_PAREN || 
                    currentToken.Type == TokenType.LEFT_BRACKET || 
                    currentToken.Type == TokenType.LEFT_BRACE)
                {
                    depth++;
                    continue;
                }
                else if (currentToken.Type == TokenType.RIGHT_PAREN || 
                         currentToken.Type == TokenType.RIGHT_BRACKET || 
                         currentToken.Type == TokenType.RIGHT_BRACE)
                {
                    depth--;
                    continue;
                }
                
                // Only consider operators at depth 0 (not inside grouping constructs)
                if (depth == 0 && IsDyadicOperatorDirect(currentToken.Type))
                {
                    // Skip verb+adverb patterns
                    if (i + 1 < tokens.Count && IsAdverbToken(tokens[i + 1].Type))
                    {
                        i++; // Skip the adverb too
                        continue;
                    }
                    
                    // Return leftmost operator for LRS right-to-left evaluation
                    return i;
                }
            }
            
            return -1;
        }
        
        /// <summary>
        /// Check if token type represents a verb that supports dyadic arity
        /// Uses VerbRegistry for verb-agnostic detection
        /// </summary>
        private static bool IsDyadicOperatorDirect(TokenType tokenType)
        {
            // Use VerbRegistry to check if the verb supports dyadic arity
            // This ensures any registered verb (operator, system function, etc.) 
            // with dyadic support is automatically recognized
            var verbName = VerbRegistry.TokenTypeToVerbName(tokenType);
            var verb = VerbRegistry.GetVerb(verbName);
            var result = verb?.SupportedArities.Contains(2) ?? false;
            
            return result;
        }
        
        /// <summary>
        /// Public method to check if token type is a dyadic operator
        /// </summary>
        /// <param name="tokenType">Token type to check</param>
        /// <returns>True if dyadic operator</returns>
        public bool IsDyadicOperator(TokenType tokenType)
        {
            return IsDyadicOperatorDirect(tokenType);
        }
        
        /// <summary>
        /// Parse dyadic operation using LRS right-associative strategy
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node representing dyadic operation</returns>
        public ASTNode? ParseDyadicOperation(List<Token> tokens)
        {
            if (tokens.Count < 3) return null; // Need at least: left op right
            
            // Check for verb+adverb patterns anywhere in the token list
            // This handles cases like (1 2 3) +' (4 5 6) where the verb+adverb is in the middle
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (IsDyadicOperatorDirect(tokens[i].Type) && IsAdverbToken(tokens[i + 1].Type))
                {
                    // Extract the tokens for the adverb operation
                    // Everything before the verb is the left operand
                    var adverbLeftTokens = tokens.GetRange(0, i);
                    // Everything after the adverb is the right operand
                    var adverbRightTokens = tokens.GetRange(i + 2, tokens.Count - i - 2);
                    var verbToken = tokens[i];
                    var adverbToken = tokens[i + 1];
                    
                    // Build parse tree for left and right operands
                    var leftNode = adverbLeftTokens.Count > 0 ? BuildParseTreeFromTokens(adverbLeftTokens) : null;
                    var rightNode = adverbRightTokens.Count > 0 ? BuildParseTreeFromTokens(adverbRightTokens) : null;
                    
                    // Create verb node
                    var verbNode = CreateNodeFromToken(verbToken);
                    if (verbNode == null)
                    {
                        throw new Exception($"Failed to create verb node from token: {verbToken.Type}({verbToken.Lexeme})");
                    }
                    
                    // Create adverb node with the structure expected by the evaluator
                    // The evaluator expects: DyadicOp(adverb_symbol, left, right)
                    // For adverbs with verbs, we need to embed the verb info in the structure
                    var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
                    
                    // For the ' (each) adverb with dyadic verbs like +', the structure should be:
                    // The verb becomes the "left" operand (evaluator extracts it)
                    // The right operand is the actual right argument
                    // If there's a left operand in the expression (e.g., 1 2 3 +' 4 5 6), 
                    // we need a more complex structure
                    
                    if (leftNode != null && rightNode != null)
                    {
                        // Both operands: e.g., (1 2 3) +' (4 5 6)
                        // Create a 3-child structure for dyadic verb with each adverb
                        adverbNode.Value = new SymbolValue(VerbRegistry.GetAdverbType(adverbToken.Type));
                        // First child is the verb
                        adverbNode.Children.Add(verbNode);
                        // Second child is the left operand
                        adverbNode.Children.Add(leftNode);
                        // Third child is the right operand
                        adverbNode.Children.Add(rightNode);
                    }
                    else if (rightNode != null)
                    {
                        // Only right operand: e.g., +' (1 2 3) - unusual but handle it
                        adverbNode.Value = new SymbolValue(VerbRegistry.GetAdverbType(adverbToken.Type));
                        adverbNode.Children.Add(verbNode);
                        adverbNode.Children.Add(rightNode);
                    }
                    else if (leftNode != null)
                    {
                        // Only left operand: e.g., (1 2 3) +' - unusual but handle it
                        adverbNode.Value = new SymbolValue(VerbRegistry.GetAdverbType(adverbToken.Type));
                        adverbNode.Children.Add(verbNode);
                        adverbNode.Children.Add(leftNode);
                    }
                    else
                    {
                        // No operands - just the modified verb
                        adverbNode.Value = new SymbolValue(VerbRegistry.GetAdverbType(adverbToken.Type));
                        adverbNode.Children.Add(verbNode);
                        adverbNode.Children.Add(ASTNode.MakeLiteral(new NullValue()));
                    }
                    
                    return adverbNode;
                }
            }
            
            var rightmostOpIndex = FindRightmostOperator(tokens);
            if (rightmostOpIndex == -1) return null;
            
            // Split at rightmost operator
            var leftTokens = tokens.GetRange(0, rightmostOpIndex);
            var rightTokens = tokens.GetRange(rightmostOpIndex + 1, tokens.Count - rightmostOpIndex - 1);
            var opToken = tokens[rightmostOpIndex];
            
            // Choose strategy based on parent parser mode
            if (parentParser?.BuildParseTree == true)
            {
                // Build parse tree: recursively parse left and right without evaluation
                var leftNode = BuildParseTreeFromTokens(leftTokens);
                var rightNode = BuildParseTreeFromTokens(rightTokens);
                
                // Check if this should be a monadic operation (no left operand)
                // MUST check this BEFORE replacing null nodes, as null leftNode is valid for monadic
                if (leftTokens.Count == 0 && OperatorDetector.SupportsMonadic(opToken.Type))
                {
                    // Create monadic node when there's no left operand
                    // Handle null right node
                    if (rightNode == null)
                        rightNode = ASTNode.MakeLiteral(new NullValue());
                    return CreateMonadicNode(opToken, rightNode);
                }
                
                // Handle null nodes by creating appropriate literals (for dyadic operations)
                if (leftNode == null)
                    leftNode = ASTNode.MakeLiteral(new NullValue());
                if (rightNode == null)
                    rightNode = ASTNode.MakeLiteral(new NullValue());
                
                // Create dyadic node when there are both operands
                return CreateDyadicNode(opToken, leftNode, rightNode);
            }
            else
            {
                // Original evaluation logic
                var leftNode = ParseSubExpression(leftTokens);
                var rightNode = ParseSubExpression(rightTokens);
                
                // Check if this should be a monadic operation (no left operand)
                // MUST check this BEFORE replacing null nodes, as null leftNode is valid for monadic
                if (leftTokens.Count == 0 && OperatorDetector.SupportsMonadic(opToken.Type))
                {
                    // Create monadic node when there's no left operand
                    // Handle null right node
                    if (rightNode == null)
                        rightNode = ASTNode.MakeLiteral(new NullValue());
                    return CreateMonadicNode(opToken, rightNode);
                }
                
                // Handle null nodes by creating appropriate literals (for dyadic operations)
                if (leftNode == null)
                    leftNode = ASTNode.MakeLiteral(new NullValue());
                if (rightNode == null)
                    rightNode = ASTNode.MakeLiteral(new NullValue());
                
                // Create dyadic node when there are both operands
                return CreateDyadicNode(opToken, leftNode, rightNode);
            }
        }
        
        /// <summary>
        /// Build parse tree from tokens (recursive)
        /// </summary>
        /// <param name="tokens">Tokens to build parse tree from</param>
        /// <returns>AST node representing parse tree structure</returns>
        private ASTNode? BuildParseTreeFromTokens(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) 
            {
                return CreateNodeFromToken(tokens[0]);
            }
            
            // Check if this is a parenthesized expression and use grouping parser
            if (tokens.Count >= 2 && 
                tokens[0].Type == TokenType.LEFT_PAREN && 
                tokens[tokens.Count - 1].Type == TokenType.RIGHT_PAREN)
            {
                // Use grouping parser for parenthesized expressions
                var subGroupingParser = new LRSGroupingParser(tokens, parentParser?.BuildParseTree ?? false, parentParser);
                int pos = 0;
                try
                {
                    return subGroupingParser.ParseParentheses(ref pos);
                }
                catch
                {
                    // Fall through to dyadic parsing if grouping parser fails
                }
            }
            
            // Check if all tokens are atomic - if so, create a vector
            bool allAtomic = tokens.All(t => LRSAtomicParser.IsAtomicToken(t.Type));
            if (allAtomic)
            {
                var argNodes = new List<ASTNode>();
                foreach (var token in tokens)
                {
                    argNodes.Add(LRSAtomicParser.ParseAtomicToken(token));
                }
                return new ASTNode(ASTNodeType.Vector, null, argNodes);
            }
            
            // Try dyadic operation first (monadic parsing is handled at main LRS level)
            var result = ParseDyadicOperation(tokens);
            if (result == null)
                return null;
            return result;
        }
        
        /// <summary>
        /// Parse sub-expression (could be monadic, dyadic, or atomic)
        /// Pure LRS mode: Enhanced with grouping parser support
        /// </summary>
        private ASTNode? ParseSubExpression(List<Token> tokens)
        {
            if (tokens.Count == 0) return null;
            
            // Check for simple assignment statement (e.g., 'a: 42' in '1 + a: 42')
            // This handles inline assignment where assignment is a sub-expression.
            // Use ParseInlineStatement so the assignment returns the value (not Null).
            if (tokens.Count >= 3 && IsSimpleAssignment(tokens))
            {
                var statementParser = parentParser?.GetStatementParser();
                if (statementParser != null)
                {
                    return statementParser.ParseInlineStatement(tokens);
                }
            }
            
            bool pureLRSMode = parentParser?.PureLRSMode ?? false;
            
            // Check for nested grouping constructs with semicolon-separated expressions
            // This handles dictionary creation cases like ((`a;1);(`b;2)) where semicolons
            // appear at depth >= 1 (inside parentheses), not simple cases like (1;2;3)
            if (tokens.Count >= 7)  // Minimum: ( ( x ; y ) ; ( z ; w ) )
            {
                var firstToken = tokens[0];
                
                if (firstToken.Type == TokenType.LEFT_PAREN || 
                    firstToken.Type == TokenType.LEFT_BRACKET || 
                    firstToken.Type == TokenType.LEFT_BRACE)
                {
                    // Check for semicolons at depth > 1 (inside nested groupings)
                    bool hasDeepSemicolon = false;
                    int depth = 0;
                    TokenType openType = firstToken.Type;
                    TokenType closeType = openType == TokenType.LEFT_PAREN ? TokenType.RIGHT_PAREN :
                                         openType == TokenType.LEFT_BRACKET ? TokenType.RIGHT_BRACKET :
                                         TokenType.RIGHT_BRACE;
                    
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        var token = tokens[i];
                        
                        if (token.Type == openType) 
                        {
                            depth++;
                        }
                        else if (token.Type == closeType) 
                        {
                            depth--;
                        }
                        else if (token.Type == TokenType.SEMICOLON && depth >= 1)
                        {
                            // Semicolon at depth >= 1 means it's inside parentheses (for nested structures like matrices)
                            hasDeepSemicolon = true;
                        }
                        
                        // Early exit if we find a deep semicolon
                        if (hasDeepSemicolon) break;
                    }
                    
                    // Verify it's a complete grouping and has deep semicolons
                    if (hasDeepSemicolon)
                    {
                        // Recalculate depth to verify structure
                        depth = 0;
                        for (int i = 0; i < tokens.Count; i++)
                        {
                            var token = tokens[i];
                            
                            if (token.Type == openType) depth++;
                            else if (token.Type == closeType) depth--;
                            
                            // If we close at the last token, this is a complete grouping
                            if (depth == 0 && i == tokens.Count - 1)
                            {
                                // Use grouping parser for nested semicolon-containing expressions
                                var subGroupingParser = new LRSGroupingParser(tokens, parentParser?.BuildParseTree ?? false, parentParser);
                                int pos = 0;
                                try
                                {
                                    if (openType == TokenType.LEFT_PAREN)
                                        return subGroupingParser.ParseParentheses(ref pos);
                                    else if (openType == TokenType.LEFT_BRACKET)
                                        return subGroupingParser.ParseBrackets(ref pos);
                                    else if (openType == TokenType.LEFT_BRACE)
                                        return subGroupingParser.ParseBraces(ref pos);
                                }
                                catch
                                {
                                    // Fall through to default handling
                                }
                                break;
                            }
                        }
                    }
                }
            }
            
            // Pure LRS mode: Check for single-token grouping constructs
            if (pureLRSMode && tokens.Count == 1)
            {
                var token = tokens[0];
                
                // Handle grouping constructs using LRSGroupingParser
                if (token.Type == TokenType.LEFT_PAREN || 
                    token.Type == TokenType.LEFT_BRACKET || 
                    token.Type == TokenType.LEFT_BRACE)
                {
                    // Create new grouping parser with the sub-expression tokens
                    var subGroupingParser = new LRSGroupingParser(tokens, parentParser?.BuildParseTree ?? false, parentParser);
                    int pos = 0;
                    try
                    {
                        if (token.Type == TokenType.LEFT_PAREN)
                            return subGroupingParser.ParseParentheses(ref pos);
                        else if (token.Type == TokenType.LEFT_BRACKET)
                            return subGroupingParser.ParseBrackets(ref pos);
                        else if (token.Type == TokenType.LEFT_BRACE)
                            return subGroupingParser.ParseBraces(ref pos);
                    }
                    catch
                    {
                        // Fall through to default handling
                    }
                }
            }
            
            if (tokens.Count == 1) 
            {
                var nodeResult = CreateNodeFromToken(tokens[0]);
                return nodeResult;
            }
            
            // Check for empty braces {} form specifier (used with $ operator for string expression evaluation)
            // Pattern: {} followed by $ - must be detected before other parsing
            if (tokens.Count == 2 && 
                tokens[0].Type == TokenType.LEFT_BRACE && 
                tokens[1].Type == TokenType.RIGHT_BRACE)
            {
                // Create a form specifier node for empty braces
                var formSpecifierNode = new ASTNode(ASTNodeType.FormSpecifier);
                formSpecifierNode.Value = new SymbolValue("{}");
                return formSpecifierNode;
            }
            
            // Handle grouping constructs (parentheses, brackets, braces) that wrap the entire expression
            // This ensures proper parsing of parenthesized sub-expressions regardless of pureLRSMode
            if (tokens.Count > 2)
            {
                var firstToken = tokens[0];
                
                if (firstToken.Type == TokenType.LEFT_PAREN || 
                    firstToken.Type == TokenType.LEFT_BRACKET || 
                    firstToken.Type == TokenType.LEFT_BRACE)
                {
                    // Check if this is a complete grouping construct
                    int depth = 0;
                    TokenType openType = firstToken.Type;
                    TokenType closeType = openType == TokenType.LEFT_PAREN ? TokenType.RIGHT_PAREN :
                                         openType == TokenType.LEFT_BRACKET ? TokenType.RIGHT_BRACKET :
                                         TokenType.RIGHT_BRACE;
                    
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i].Type == openType) depth++;
                        else if (tokens[i].Type == closeType) depth--;
                        
                        // If we close at the last token, this is a complete grouping
                        if (depth == 0 && i == tokens.Count - 1)
                        {
                            // Create new grouping parser with the sub-expression tokens
                            var subGroupingParser = new LRSGroupingParser(tokens, parentParser?.BuildParseTree ?? false, parentParser);
                            int pos = 0;
                            try
                            {
                                ASTNode? result;
                                if (openType == TokenType.LEFT_PAREN)
                                    result = subGroupingParser.ParseParentheses(ref pos);
                                else if (openType == TokenType.LEFT_BRACKET)
                                    result = subGroupingParser.ParseBrackets(ref pos);
                                else
                                    result = subGroupingParser.ParseBraces(ref pos);
                                return result;
                            }
                            catch
                            {
                                // Fall through to dyadic parsing
                            }
                            break;
                        }
                    }
                }
            }
            
            // Check for implicit vector creation (sequences of atomic literals like "1 2 3")
            // This must happen before dyadic parsing to handle vector left arguments to operators
            if (tokens.Count >= 2)
            {
                var implicitVector = TryCreateImplicitVector(tokens);
                if (implicitVector != null)
                    return implicitVector;
            }
            
            // Try dyadic operation (monadic parsing is handled at main LRS level)
            var dyadicResult = ParseDyadicOperation(tokens);
            if (dyadicResult != null)
                return dyadicResult;
            
            // If dyadic parsing failed and we have exactly 2 tokens (potential monadic: op + operand)
            // try monadic parsing directly
            if (tokens.Count == 2 && OperatorDetector.SupportsMonadic(tokens[0].Type))
            {
                var opToken = tokens[0];
                var operandNode = CreateNodeFromToken(tokens[1]);
                if (operandNode != null)
                {
                    return CreateMonadicNode(opToken, operandNode);
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Try to create an implicit vector from a sequence of atomic literals
        /// Returns null if tokens don't form a valid implicit vector
        /// </summary>
        private ASTNode? TryCreateImplicitVector(List<Token> tokens)
        {
            if (tokens.Count < 2)
                return null;
            
            var elements = new List<ASTNode>();
            
            foreach (var token in tokens)
            {
                // Check if token is an atomic literal
                if (!LRSAtomicParser.IsAtomicToken(token.Type))
                    return null; // Not all atomic - can't be implicit vector
                
                // Parse the token and add to elements
                var node = LRSAtomicParser.ParseAtomicToken(token);
                if (node == null)
                    return null;
                
                elements.Add(node);
            }
            
            // Create vector for all implicit collections
            // K semantics: space-separated literals create vectors (homogeneous or mixed)
            // The evaluator will determine the proper K3Value type based on element types
            return ASTNode.MakeVector(elements);
        }
        
        /// <summary>
        /// Check if tokens represent a simple assignment (e.g., 'a: 42')
        /// Pattern: IDENTIFIER COLON expression
        /// </summary>
        private bool IsSimpleAssignment(List<Token> tokens)
        {
            // Must have at least: identifier, colon, value
            if (tokens.Count < 3)
                return false;
            
            // First token must be an identifier (variable name)
            if (tokens[0].Type != TokenType.IDENTIFIER)
                return false;
            
            // Second token must be colon
            if (tokens[1].Type != TokenType.COLON)
                return false;
            
            // Must not contain any operators before the colon (simple variable name only)
            // e.g., 'a: 42' is simple, but '1 + a: 42' is not
            return true;
        }
        
        /// <summary>
        /// Create AST node for dyadic operation
        /// </summary>
        private ASTNode CreateDyadicNode(Token opToken, ASTNode left, ASTNode right)
        {
            return ASTNode.MakeDyadicOp(opToken.Type, left, right);
        }
        
        /// <summary>
        /// Create AST node for monadic operation
        /// </summary>
        private ASTNode CreateMonadicNode(Token opToken, ASTNode operand)
        {
            var node = new ASTNode(ASTNodeType.MonadicOp);
            node.Value = new SymbolValue(VerbRegistry.TokenTypeToVerbName(opToken.Type));
            node.Children.Add(operand);
            return node;
        }
        
        /// <summary>
        /// Create AST node from atomic token using LRSAtomicParser
        /// </summary>
        private ASTNode? CreateNodeFromToken(Token token)
        {
            if (LRSAtomicParser.IsAtomicToken(token.Type))
            {
                return LRSAtomicParser.ParseAtomicToken(token);
            }
            
            // Handle operator symbols for parse trees
            return LRSAtomicParser.CreateOperatorNode(token.Type);
        }
        
        /// <summary>
        /// Check if token could be a dyadic operator
        /// </summary>
        public static bool CouldBeDyadicOperator(TokenType tokenType)
        {
            return IsDyadicOperatorDirect(tokenType);
        }
        
        /// <summary>
        /// Check if token is an adverb
        /// </summary>
        /// <param name="tokenType">Token type to check</param>
        /// <returns>True if token is an adverb</returns>
        private bool IsAdverbToken(TokenType tokenType)
        {
            return VerbRegistry.IsAdverbToken(tokenType);
        }
    }
}
