using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Complex vector parsing logic extracted from main Parser class
    /// Handles nested list structures with semicolons
    /// </summary>
    public partial class Parser
    {
        /// <summary>
        /// Parse complex nested vector structures like ((9 8 7);(6 5 4);(3 2 1))
        /// This handles multiple levels of nesting with semicolon separators
        /// </summary>
        private ASTNode ParseComplexVector()
        {
            var elements = new List<ASTNode>();
            
            Console.WriteLine($"DEBUG ParseComplexVector: Starting at position {current}");
            
            // Parse until we hit the closing parenthesis
            while (!IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_PAREN)
            {
                Console.WriteLine($"DEBUG ParseComplexVector: Parsing element at position {current}, token: {CurrentToken().Type}");
                
                // Parse element (could be simple or complex)
                var element = ParseComplexVectorElement();
                if (element != null)
                {
                    elements.Add(element);
                    Console.WriteLine($"DEBUG ParseComplexVector: Added element, total count: {elements.Count}");
                }
                
                // Handle semicolon separators
                if (Match(TokenType.SEMICOLON))
                {
                    Console.WriteLine("DEBUG ParseComplexVector: Found semicolon, continuing to next element");
                    continue; // Next element
                }
                else if (CurrentToken().Type != TokenType.RIGHT_PAREN)
                {
                    Console.WriteLine($"DEBUG ParseComplexVector: Breaking on token: {CurrentToken().Type}");
                    break; // End of vector or unexpected token
                }
            }
            
            if (!Match(TokenType.RIGHT_PAREN))
            {
                throw new Exception("Expected ')' after complex vector");
            }
            
            Console.WriteLine($"DEBUG ParseComplexVector: Completed with {elements.Count} elements");
            return ASTNode.MakeVector(elements);
        }
        
        /// <summary>
        /// Parse a single element within a complex vector
        /// Element could be a nested vector or a simple expression
        /// </summary>
        private ASTNode ParseComplexVectorElement()
        {
            if (CurrentToken().Type == TokenType.LEFT_PAREN)
            {
                // Nested vector - use existing method to avoid recursion
                Console.WriteLine("DEBUG ParseComplexVectorElement: Found nested vector, using ParseParenthesizedForElement");
                return ParseParenthesizedForElement();
            }
            else
            {
                // Simple expression
                Console.WriteLine("DEBUG ParseComplexVectorElement: Parsing simple expression");
                return ParseExpression();
            }
        }
        
        /// <summary>
        /// Enhanced version of ParseParenthesizedForElement that can handle complex structures
        /// Falls back to complex parsing when needed
        /// </summary>
        private ASTNode ParseEnhancedParenthesizedElement()
        {
            Console.WriteLine($"DEBUG ParseEnhancedParenthesizedElement: Starting at position {current}, token: {CurrentToken().Type}");
            
            if (CurrentToken().Type != TokenType.LEFT_PAREN)
            {
                throw new Exception("Expected '(' at start of parenthesized element");
            }
            
            // Look ahead to determine if this is a complex structure
            var lookaheadPos = current;
            bool hasSemicolon = false;
            int parenDepth = 1;
            
            Console.WriteLine($"DEBUG ParseEnhancedParenthesizedElement: Looking ahead from position {lookaheadPos}");
            while (lookaheadPos < tokens.Count && parenDepth > 0)
            {
                var token = tokens[lookaheadPos];
                if (token.Type == TokenType.LEFT_PAREN) parenDepth++;
                else if (token.Type == TokenType.RIGHT_PAREN) parenDepth--;
                else if (token.Type == TokenType.SEMICOLON) hasSemicolon = true; // Detect semicolons at any nesting level
                lookaheadPos++;
            }
            
            Console.WriteLine($"DEBUG ParseEnhancedParenthesizedElement: hasSemicolon={hasSemicolon}, parenDepth ended at {parenDepth}");
            
            if (hasSemicolon)
            {
                // Complex structure - use complex parsing
                Console.WriteLine("DEBUG ParseEnhancedParenthesizedElement: Using complex parsing");
                return ParseComplexVector();
            }
            else
            {
                // Simple structure - use existing logic
                Console.WriteLine("DEBUG ParseEnhancedParenthesizedElement: Using simple parsing");
                return ParseParenthesizedForElement();
            }
        }
    }
}
