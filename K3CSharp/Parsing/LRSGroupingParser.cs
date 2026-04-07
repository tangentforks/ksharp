using System;
using System.Collections.Generic;
using System.Text;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Grouping construct parsing for LRS parser
    /// Handles parentheses, brackets, and braces without PrimaryParser dependency
    /// Supports expression lists, vectors, projections, and function parameters
    /// </summary>
    public class LRSGroupingParser
    {
        private List<Token> tokens;
        private readonly bool buildParseTree;
        private readonly LRSParser? parentParser;
        
        public LRSGroupingParser(List<Token> tokens, bool buildParseTree = false, LRSParser? parentParser = null)
        {
            this.tokens = tokens;
            this.buildParseTree = buildParseTree;
            this.parentParser = parentParser;
        }

        /// <summary>
        /// Parse parentheses expression (expression lists)
        /// </summary>
        public ASTNode ParseParentheses(ref int position)
        {
            if (position >= tokens.Count || tokens[position].Type != TokenType.LEFT_PAREN)
                throw new Exception("Expected '('");
                
            position++; // Consume '('

            var elements = new List<ASTNode>();
            
            // Handle empty parentheses () - should be treated as empty list (0 items), not null
            // Per K spec: () is an empty vector of mixed type (type 0)
            if (position < tokens.Count && tokens[position].Type == TokenType.RIGHT_PAREN)
            {
                position++; // Consume ')'
                return ASTNode.MakeVector(new List<ASTNode>()); // Empty list
            }
            
            // Parse expressions sequentially from left to right
            while (position < tokens.Count && tokens[position].Type != TokenType.RIGHT_PAREN)
            {
                // Skip newlines as they are expression separators
                while (position < tokens.Count && tokens[position].Type == TokenType.NEWLINE)
                {
                    position++;
                }
                
                // Check for empty expressions (consecutive separators)
                if (position < tokens.Count && (tokens[position].Type == TokenType.SEMICOLON || 
                    tokens[position].Type == TokenType.NEWLINE))
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                    position++; // Consume the separator
                    continue;
                }
                
                // Parse the expression (commas are operators, so they're handled as part of expressions)
                var expr = ParseExpressionInGrouping(ref position);
                if (expr != null)
                {
                    elements.Add(expr);
                }
                else
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
                
                // Check for separator (semicolon or newline only - commas are operators!)
                if (position < tokens.Count && tokens[position].Type == TokenType.SEMICOLON)
                {
                    position++; // Consume semicolon
                    continue; // Next expression
                }
                else if (position < tokens.Count && tokens[position].Type == TokenType.NEWLINE)
                {
                    position++; // Consume newline
                    continue; // Next expression
                }
                else if (position < tokens.Count && tokens[position].Type == TokenType.RIGHT_PAREN)
                {
                    break; // End of parentheses
                }
                else
                {
                    // No separator and not at end - this should not happen in valid K
                    break;
                }
            }
            
            if (position >= tokens.Count)
                throw new Exception("Unclosed parentheses - expected ')'");
                
            position++; // Consume ')'
            
            // Apply K specification rules for the result
            if (elements.Count == 1)
            {
                // If the content has 1 expression, return the value of the expression
                return elements[0];
            }
            else if (elements.Count > 1)
            {
                // If the content has more than 1 expression, generate a list
                // The evaluator will handle vector collapsing if all elements have the same type
                // For semicolon-separated expressions, create an ExpressionList (return all values)
                return new ASTNode(ASTNodeType.ExpressionList, null, elements);
            }
            else
            {
                // Should not reach here due to empty parentheses handling above
                return ASTNode.MakeLiteral(new NullValue());
            }
        }

        /// <summary>
        /// Parse brackets expression (vectors and projections)
        /// </summary>
        public ASTNode ParseBrackets(ref int position)
        {
            if (position >= tokens.Count || tokens[position].Type != TokenType.LEFT_BRACKET)
                throw new Exception("Expected '['");
                
            position++; // Consume '['
            
            var elements = new List<ASTNode>();
            var projectionSlots = new List<ASTNode?>();
            var hasSemicolon = false;
            
            if (position < tokens.Count && tokens[position].Type == TokenType.RIGHT_BRACKET)
            {
                position++; // Consume ']'
                // For semicolon-separated expressions, create a Block (list), not a Vector
                return new ASTNode(ASTNodeType.Block, null, elements);
            }
            
            // Check if the first token is a semicolon (indicating a projection)
            if (position < tokens.Count && tokens[position].Type == TokenType.SEMICOLON)
            {
                position++; // Consume semicolon
                hasSemicolon = true;
                projectionSlots.Add(null); // Missing first argument
                
                // Skip empty lines
                while (position < tokens.Count && tokens[position].Type == TokenType.NEWLINE)
                {
                    position++;
                }
            }
            
            // Parse first element if not at end
            if (position < tokens.Count && tokens[position].Type != TokenType.RIGHT_BRACKET)
            {
                var firstElement = ParseBracketArgument(ref position);
                if (firstElement == null)
                {
                    throw new Exception("Expected expression in brackets");
                }
                
                if (hasSemicolon)
                {
                    projectionSlots.Add(firstElement);
                }
                else
                {
                    elements.Add(firstElement);
                }
            }
            
            // Parse additional elements separated by semicolons
            int bracketDepth = 0;
            while (position < tokens.Count)
            {
                var token = tokens[position];
                
                // Track nesting depth for brackets
                if (token.Type == TokenType.LEFT_BRACKET)
                {
                    bracketDepth++;
                }
                else if (token.Type == TokenType.RIGHT_BRACKET)
                {
                    if (bracketDepth == 0)
                    {
                        break; // Found our closing bracket
                    }
                    bracketDepth--;
                }
                
                if (token.Type == TokenType.SEMICOLON)
                {
                    position++; // Consume semicolon
                    hasSemicolon = true;
                    projectionSlots.Add(null); // Missing argument
                    
                    // Skip empty lines
                    while (position < tokens.Count && tokens[position].Type == TokenType.NEWLINE)
                    {
                        position++;
                    }
                    
                    if (position < tokens.Count && tokens[position].Type != TokenType.RIGHT_BRACKET)
                    {
                        // Check if we're still at depth 0 for this bracket level
                        var element = ParseBracketArgument(ref position);
                        if (element != null)
                        {
                            projectionSlots[projectionSlots.Count - 1] = element; // Replace the null
                        }
                    }
                }
                else
                {
                    var element = ParseBracketArgument(ref position);
                    if (element != null)
                    {
                        if (hasSemicolon)
                        {
                            projectionSlots.Add(element);
                        }
                        else
                        {
                            elements.Add(element);
                        }
                    }
                }
            }
            
            if (position >= tokens.Count)
                throw new Exception("Unclosed brackets - expected ']'");
                
            position++; // Consume ']'
            
            // If we detected semicolons (projection syntax), create a projection node
            if (hasSemicolon)
            {
                // For projections, we need to create a vector with the projection arguments
                var projectionElements = new List<ASTNode>();
                
                // Add all non-null projection arguments
                foreach (var slot in projectionSlots)
                {
                    if (slot != null)
                    {
                        projectionElements.Add(slot);
                    }
                }
                
                return ASTNode.MakeVector(projectionElements);
            }
            
            // For semicolon-separated expressions, create a Block (list), not a Vector
                return new ASTNode(ASTNodeType.Block, null, elements);
        }
        
        /// <summary>
        /// Parse brackets from a token list (used by BuildParseTreeFromRight)
        /// </summary>
        public ASTNode? ParseBrackets(List<Token> bracketTokens)
        {
            if (bracketTokens.Count == 0 || bracketTokens[0].Type != TokenType.LEFT_BRACKET)
                return null;
            
            // Temporarily swap tokens to use the provided list
            var originalTokens = tokens;
            try
            {
                tokens = bracketTokens;
                int position = 0;
                return ParseBrackets(ref position);
            }
            finally
            {
                tokens = originalTokens;
            }
        }

        /// <summary>
        /// Parse braces expression (function definitions)
        /// </summary>
        public ASTNode ParseBraces(ref int position)
        {
            if (position >= tokens.Count || tokens[position].Type != TokenType.LEFT_BRACE)
                throw new Exception("Expected '{'");
                
            int braceStartPos = position; // Remember start position for original text extraction
            position++; // Consume '{'
            
            // Check if this is a form specifier (empty braces followed by $)
            // Per K spec: {}$ pattern should be treated as form specifier, not empty function
            if (position < tokens.Count && tokens[position].Type == TokenType.RIGHT_BRACE)
            {
                int nextPos = position + 1;
                // Skip any whitespace/newline tokens to find the next significant token
                while (nextPos < tokens.Count && 
                       (tokens[nextPos].Type == TokenType.NEWLINE || 
                        tokens[nextPos].Type == TokenType.SEMICOLON))
                {
                    nextPos++;
                }
                
                // Check if next significant token is $
                if (nextPos < tokens.Count && tokens[nextPos].Type == TokenType.DOLLAR)
                {
                    position++; // Consume '}'
                    
                    // Create a special node for {} form specifier
                    var formSpecifierNode = new ASTNode(ASTNodeType.FormSpecifier);
                    formSpecifierNode.Value = new SymbolValue("{}");
                    return formSpecifierNode;
                }
            }
            
            // Parse function body
            var body = ParseFunctionBody(ref position, braceStartPos);
            
            if (position >= tokens.Count || tokens[position].Type != TokenType.RIGHT_BRACE)
                throw new Exception("Unclosed braces - expected '}'");
                
            position++; // Consume '}'
            
            // Update the function's original source text to include the closing brace
            if (body.Type == ASTNodeType.Function && body.Value is FunctionValue funcVal)
            {
                // Create new FunctionValue with updated source text that includes closing brace
                string updatedSourceText = funcVal.OriginalSourceText + "}";
                body.Value = new FunctionValue(funcVal.BodyText, funcVal.Parameters, funcVal.PreParsedTokens, updatedSourceText, funcVal.Hint);
            }
            
            return body;
        }

        /// <summary>
        /// Parse expression within grouping constructs
        /// Collects all tokens including nested parentheses and delegates to LRS parser
        /// </summary>
        private ASTNode? ParseExpressionInGrouping(ref int position)
        {
            if (position >= tokens.Count)
            {
                return ASTNode.MakeLiteral(new NullValue()); // Return K NullValue for empty
            }
            
            var token = tokens[position];
            
            // Check for expression separators or closing delimiters at start
            if (token.Type == TokenType.SEMICOLON || token.Type == TokenType.NEWLINE ||
                token.Type == TokenType.RIGHT_PAREN || token.Type == TokenType.RIGHT_BRACKET || 
                token.Type == TokenType.RIGHT_BRACE)
            {
                return ASTNode.MakeLiteral(new NullValue()); // Empty expression
            }
            
            // Collect all tokens for this expression, including nested parentheses
            // Track depth to know when we hit the end of this expression
            // We start at depth 0 (outside any grouping constructs)
            var exprTokens = new List<Token>();
            int depth = 0;
            
            while (position < tokens.Count)
            {
                token = tokens[position];
                
                // Check for closing delimiter at depth 0 - this belongs to our parent
                if (depth == 0 && (token.Type == TokenType.RIGHT_PAREN || 
                                   token.Type == TokenType.RIGHT_BRACKET || 
                                   token.Type == TokenType.RIGHT_BRACE))
                {
                    break; // End of this expression (closing delimiter of parent)
                }
                
                // Check for separator at depth 0
                if (depth == 0 && (token.Type == TokenType.SEMICOLON || token.Type == TokenType.NEWLINE))
                {
                    break; // Expression separator at base level
                }
                
                // Track nesting depth for grouping constructs
                if (token.Type == TokenType.LEFT_PAREN || token.Type == TokenType.LEFT_BRACKET || 
                    token.Type == TokenType.LEFT_BRACE)
                {
                    depth++;
                }
                else if (token.Type == TokenType.RIGHT_PAREN || token.Type == TokenType.RIGHT_BRACKET || 
                         token.Type == TokenType.RIGHT_BRACE)
                {
                    depth--;
                }
                
                exprTokens.Add(token);
                position++;
            }
            
            // If no tokens collected, return null
            if (exprTokens.Count == 0)
            {
                return ASTNode.MakeLiteral(new NullValue());
            }
            
            // Delegate to parent LRS parser for proper expression parsing
            // Parent parser will handle nested parentheses recursively
            ASTNode? result = null;
            
            if (parentParser != null)
            {
                if (buildParseTree)
                    result = parentParser.BuildParseTreeFromRight(exprTokens);
                else
                    result = parentParser.EvaluateFromRight(exprTokens);
            }
            else
            {
                // Fallback: create new parser instance (should not happen in normal flow)
                var lrsParser = new LRSParser(exprTokens, buildParseTree);
                int pos = 0;
                result = lrsParser.ParseExpression(ref pos);
            }
            
            // Phase 2.1: Ensure we return K NullValue instead of C# null
            if (result == null)
            {
                return ASTNode.MakeLiteral(new NullValue());
            }
            
            return result;
        }
        
        /// <summary>
        /// Parse bracket argument (single expression within brackets)
        /// </summary>
        private ASTNode? ParseBracketArgument(ref int position)
        {
            if (position >= tokens.Count)
                return ASTNode.MakeLiteral(new NullValue()); // Phase 2.1: Return K NullValue
                
            // Simple case: parse the contents as a single expression
            var result = ParseExpressionInGrouping(ref position);
            
            // Phase 2.1: Ensure we return K NullValue instead of C# null
            if (result == null)
                return ASTNode.MakeLiteral(new NullValue());
                
            return result;
        }

        /// <summary>
        /// Parse function body with optional parameters
        /// Creates proper Function AST node with FunctionValue
        /// </summary>
        private ASTNode ParseFunctionBody(ref int position, int braceStartPos)
        {
            var parameters = new List<string>();
            var bodyStartPos = position; // Remember start position for body text extraction
            
            // Parse parameter list if present
            if (position < tokens.Count && tokens[position].Type == TokenType.LEFT_BRACKET)
            {
                position++; // Consume '['
                
                while (position < tokens.Count && tokens[position].Type != TokenType.RIGHT_BRACKET)
                {
                    if (tokens[position].Type == TokenType.IDENTIFIER)
                    {
                        parameters.Add(tokens[position].Lexeme);
                        position++;
                        
                        if (position < tokens.Count && tokens[position].Type == TokenType.SEMICOLON)
                        {
                            position++; // Consume ';'
                        }
                    }
                    else
                    {
                        throw new Exception("Expected identifier in parameter list");
                    }
                }
                
                if (position >= tokens.Count)
                    throw new Exception("Unclosed parameter list - expected ']'");
                    
                position++; // Consume ']'
            }
            
            // Parse function body
            var body = ParseExpressionInGrouping(ref position);
            var bodyNode = body ?? ASTNode.MakeLiteral(new NullValue());
            
            // Extract complete original source text from opening brace to current position
            string originalSourceText = ExtractOriginalSourceText(braceStartPos, position);
            
            // Create Function AST node with FunctionValue
            var functionNode = new ASTNode(ASTNodeType.Function);
            
            // Extract only the function body part (after parameters) from original source
            string bodyText = ExtractBodyFromOriginalSource(originalSourceText);
            var functionValue = new FunctionValue(bodyText, parameters, null!, originalSourceText);
            functionNode.Value = functionValue;
            
            // Pre-cache the parsed body AST to avoid re-parsing issues
            functionValue.CacheAst(bodyNode);
            
            // Store the actual parsed body AST as a child node for evaluation
            functionNode.Children.Add(bodyNode);
            
            return functionNode;
        }
        
        /// <summary>
        /// Extract function body from original source text (excluding parameter list)
        /// </summary>
        /// <param name="originalSourceText">Original source including parameters</param>
        /// <returns>Function body text only</returns>
        private string ExtractBodyFromOriginalSource(string originalSourceText)
        {
            // Find the closing bracket ']' of the parameter list
            var bracketIndex = originalSourceText.IndexOf(']');
            if (bracketIndex >= 0)
            {
                // Return everything after the closing bracket
                return originalSourceText.Substring(bracketIndex + 1).Trim();
            }
            
            // If no parameter list found, strip any leading '{' and return the body
            var body = originalSourceText.Trim();
            if (body.StartsWith("{"))
                body = body.Substring(1).Trim();
            return body;
        }
        
        /// <summary>
        /// Extract body text from AST node for execution
        /// </summary>
        /// <param name="bodyNode">The parsed body AST node</param>
        /// <returns>Body text reconstructed from AST</returns>
        private string ExtractBodyTextFromAST(ASTNode bodyNode)
        {
            // Debug: Print the AST structure
            System.Diagnostics.Debug.WriteLine($"[DEBUG] ExtractBodyTextFromAST: Type={bodyNode.Type}, Children={bodyNode.Children.Count}");
            
            if (bodyNode.Type == ASTNodeType.Literal && bodyNode.Value != null)
            {
                return bodyNode.Value.ToString();
            }
            else if (bodyNode.Type == ASTNodeType.DyadicOp && bodyNode.Children.Count >= 2)
            {
                var left = ExtractBodyTextFromAST(bodyNode.Children[0]);
                var op = bodyNode.Value?.ToString() ?? "";
                var right = ExtractBodyTextFromAST(bodyNode.Children[1]);
                return $"{left}{op}{right}";
            }
            else if (bodyNode.Type == ASTNodeType.MonadicOp && bodyNode.Children.Count >= 1)
            {
                var op = bodyNode.Value?.ToString() ?? "";
                var operand = ExtractBodyTextFromAST(bodyNode.Children[0]);
                return $"{op}{operand}";
            }
            else if (bodyNode.Type == ASTNodeType.Variable && bodyNode.Value != null)
            {
                return bodyNode.Value.ToString();
            }
            else if (bodyNode.Type == ASTNodeType.Block && bodyNode.Children.Count > 0)
            {
                // For block nodes, join all children with spaces
                var parts = new List<string>();
                foreach (var child in bodyNode.Children)
                {
                    parts.Add(ExtractBodyTextFromAST(child));
                }
                return string.Join(" ", parts);
            }
            
            return "";
        }
        
        /// <summary>
        /// Extract original source text from token range
        /// </summary>
        /// <param name="startPos">Start position (including opening brace)</param>
        /// <param name="endPos">End position (before closing brace)</param>
        /// <returns>Original source text with proper K formatting</returns>
        private string ExtractOriginalSourceText(int startPos, int endPos)
        {
            var sourceText = new StringBuilder();
            
            for (int i = startPos; i < endPos; i++)
            {
                var token = tokens[i];
                var nextToken = i < endPos - 1 ? tokens[i + 1] : null;
                
                sourceText.Append(token.Lexeme);
                
                // Add space only in specific cases for K syntax:
                // 1. After identifiers/numbers, except before brackets, semicolons, or closing brace
                // 2. After operators, except before brackets, semicolons, or closing brace
                // 3. After closing bracket, except before closing brace
                // 4. NOT after opening brace
                // 5. NOT after semicolons
                // 6. NOT before closing bracket or brace
                if (nextToken != null)
                {
                    bool addSpace = false;
                    
                    if (token.Type == TokenType.IDENTIFIER || 
                        token.Type == TokenType.INTEGER || 
                        token.Type == TokenType.FLOAT ||
                        token.Type == TokenType.LONG)
                    {
                        // Add space after identifiers/numbers, except before brackets, semicolons, or closing brace
                        addSpace = nextToken.Type != TokenType.LEFT_BRACKET &&
                                  nextToken.Type != TokenType.RIGHT_BRACKET &&
                                  nextToken.Type != TokenType.RIGHT_BRACE &&
                                  nextToken.Type != TokenType.RIGHT_PAREN &&
                                  nextToken.Type != TokenType.SEMICOLON;
                    }
                    else if (IsOperator(token.Type))
                    {
                        // Add space after operators, except before brackets, semicolons, or closing brace
                        addSpace = nextToken.Type != TokenType.LEFT_BRACKET &&
                                  nextToken.Type != TokenType.RIGHT_BRACKET &&
                                  nextToken.Type != TokenType.RIGHT_BRACE &&
                                  nextToken.Type != TokenType.RIGHT_PAREN &&
                                  nextToken.Type != TokenType.SEMICOLON;
                    }
                    else if (token.Type == TokenType.RIGHT_BRACKET)
                    {
                        // Add space after closing bracket, except before closing brace
                        addSpace = nextToken.Type != TokenType.RIGHT_BRACE &&
                                  nextToken.Type != TokenType.RIGHT_PAREN;
                    }
                    
                    if (addSpace)
                    {
                        sourceText.Append(' ');
                    }
                }
            }
            
            return sourceText.ToString();
        }
        
        /// <summary>
        /// Check if a token type represents an operator
        /// </summary>
        private bool IsOperator(TokenType tokenType)
        {
            return tokenType == TokenType.PLUS ||
                   tokenType == TokenType.MINUS ||
                   tokenType == TokenType.MULTIPLY ||
                   tokenType == TokenType.DIVIDE ||
                   tokenType == TokenType.MODULUS ||
                   tokenType == TokenType.POWER ||
                   tokenType == TokenType.DOT_APPLY ||
                   tokenType == TokenType.APPLY;
        }
        
        }
}
