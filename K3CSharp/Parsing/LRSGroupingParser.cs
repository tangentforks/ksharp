using System;
using System.Collections.Generic;

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
                // For semicolon-separated expressions, create a Block (list), not a Vector
                return new ASTNode(ASTNodeType.Block, null, elements);
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
                
            position++; // Consume '{'
            
            // Parse function body
            var body = ParseFunctionBody(ref position);
            
            if (position >= tokens.Count || tokens[position].Type != TokenType.RIGHT_BRACE)
                throw new Exception("Unclosed braces - expected '}'");
                
            position++; // Consume '}'
            
            return body;
        }

        /// <summary>
        /// Parse expression within grouping constructs
        /// Collects all tokens including nested parentheses and delegates to LRS parser
        /// </summary>
        private ASTNode? ParseExpressionInGrouping(ref int position)
        {
            if (position >= tokens.Count)
                return ASTNode.MakeLiteral(new NullValue()); // Return K NullValue for empty
            
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
                return ASTNode.MakeLiteral(new NullValue());
            
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
            
            Console.WriteLine($"[LRS DEBUG] ParseExpressionInGrouping returning: {(result == null ? "NULL" : result.Type.ToString())}");
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
                
            Console.WriteLine($"[LRS DEBUG] ParseExpressionInGrouping returning: {(result == null ? "NULL" : result.Type.ToString())}");
            return result;
        }

        /// <summary>
        /// Parse function body with optional parameters
        /// Creates proper Function AST node with FunctionValue
        /// </summary>
        private ASTNode ParseFunctionBody(ref int position)
        {
            var parameters = new List<string>();
            
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
            
            // Create Function AST node with FunctionValue
            // Note: FunctionValue stores body as text, but we also store the parsed AST in the node's Children
            var functionNode = new ASTNode(ASTNodeType.Function);
            
            // Store the body text (simplified representation for now)
            string bodyText = "parsed_lambda_body";
            functionNode.Value = new FunctionValue(bodyText, parameters, null!, bodyText);
            
            // Store the actual parsed body AST as a child node for evaluation
            functionNode.Children.Add(bodyNode);
            
            return functionNode;
        }
    }
}
