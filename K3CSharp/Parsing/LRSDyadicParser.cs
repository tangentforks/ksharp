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
            
            // Debug for DOT_APPLY
            if (tokenType == TokenType.DOT_APPLY)
            {
                Console.WriteLine($"[IsDyadicOperatorDirect] DOT_APPLY: verbName='{verbName}', verb={verb?.Name}, result={result}");
            }
            
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
                
                // Handle null nodes by creating appropriate literals
                if (leftNode == null)
                    leftNode = ASTNode.MakeLiteral(new NullValue());
                if (rightNode == null)
                    rightNode = ASTNode.MakeLiteral(new NullValue());
                
                // Check if this should be a monadic operation (no left operand)
                if (leftTokens.Count == 0 && OperatorDetector.SupportsMonadic(opToken.Type))
                {
                    // Create monadic node when there's no left operand
                    return CreateMonadicNode(opToken, rightNode);
                }
                else
                {
                    // Create dyadic node when there are both operands
                    return CreateDyadicNode(opToken, leftNode, rightNode);
                }
            }
            else
            {
                // Original evaluation logic
                var leftNode = ParseSubExpression(leftTokens);
                var rightNode = ParseSubExpression(rightTokens);
                
                // Handle null nodes by creating appropriate literals
                if (leftNode == null)
                    leftNode = ASTNode.MakeLiteral(new NullValue());
                if (rightNode == null)
                    rightNode = ASTNode.MakeLiteral(new NullValue());
                
                // Check if this should be a monadic operation (no left operand)
                if (leftTokens.Count == 0 && OperatorDetector.SupportsMonadic(opToken.Type))
                {
                    // Create monadic node when there's no left operand
                    return CreateMonadicNode(opToken, rightNode);
                }
                else
                {
                    // Create dyadic node when there are both operands
                    return CreateDyadicNode(opToken, leftNode, rightNode);
                }
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
                var nodeResult = CreateNodeFromToken(tokens[0]);
                return nodeResult;
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
            // This handles inline assignment where assignment is a sub-expression
            if (tokens.Count >= 3 && IsSimpleAssignment(tokens))
            {
                var statementParser = parentParser?.GetStatementParser();
                if (statementParser != null)
                {
                    return statementParser.ParseStatement(tokens);
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
            return ParseDyadicOperation(tokens);
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
