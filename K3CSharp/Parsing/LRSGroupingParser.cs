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
        private readonly List<Token> tokens;
        private readonly bool buildParseTree;
        
        public LRSGroupingParser(List<Token> tokens, bool buildParseTree = false)
        {
            this.tokens = tokens;
            this.buildParseTree = buildParseTree;
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
            
            // Handle empty parentheses () - should be treated as 1 expression of value null
            if (position < tokens.Count && tokens[position].Type == TokenType.RIGHT_PAREN)
            {
                position++; // Consume ')'
                return ASTNode.MakeLiteral(new NullValue());
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
                return ASTNode.MakeVector(elements);
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
                return ASTNode.MakeVector(elements);
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
            while (position < tokens.Count && tokens[position].Type != TokenType.RIGHT_BRACKET)
            {
                if (tokens[position].Type == TokenType.SEMICOLON)
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
            
            return ASTNode.MakeVector(elements);
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
        /// </summary>
        private ASTNode? ParseExpressionInGrouping(ref int position)
        {
            if (position >= tokens.Count)
                return null;
                
            var token = tokens[position];
            
            // Handle atomic expressions
            if (LRSAtomicParser.IsAtomicToken(token.Type))
            {
                position++;
                return LRSAtomicParser.ParseAtomicToken(token);
            }
            
            // Handle grouping constructs
            switch (token.Type)
            {
                case TokenType.LEFT_PAREN:
                    return ParseParentheses(ref position);
                    
                case TokenType.LEFT_BRACKET:
                    return ParseBrackets(ref position);
                    
                case TokenType.LEFT_BRACE:
                    return ParseBraces(ref position);
                    
                // Handle monadic operators
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                case TokenType.MODULUS:
                case TokenType.POWER:
                case TokenType.JOIN:
                case TokenType.MATCH:
                case TokenType.NOT:
                case TokenType.HASH:
                case TokenType.UNDERSCORE:
                case TokenType.QUESTION:
                case TokenType.DOLLAR:
                case TokenType.APPLY:
                    return ParseMonadicOperator(ref position);
                    
                // Stop at expression separators
                case TokenType.SEMICOLON:
                case TokenType.NEWLINE:
                case TokenType.RIGHT_PAREN:
                case TokenType.RIGHT_BRACKET:
                case TokenType.RIGHT_BRACE:
                    return null; // End of current expression
                    
                default:
                    throw new Exception($"Unexpected token in expression: {token.Type}({token.Lexeme})");
            }
        }

        /// <summary>
        /// Parse bracket argument (single expression within brackets)
        /// </summary>
        private ASTNode? ParseBracketArgument(ref int position)
        {
            if (position >= tokens.Count)
                return null;
                
            // Simple case: parse the contents as a single expression
            return ParseExpressionInGrouping(ref position);
        }

        /// <summary>
        /// Parse function body with optional parameters
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
            return body ?? ASTNode.MakeLiteral(new NullValue());
        }

        /// <summary>
        /// Parse monadic operator within grouping context
        /// </summary>
        private ASTNode ParseMonadicOperator(ref int position)
        {
            var token = tokens[position];
            position++;
            
            // Check if this is a projection (no operand before closing parenthesis/separator)
            var isProjection = position >= tokens.Count || 
                tokens[position].Type == TokenType.RIGHT_PAREN ||
                tokens[position].Type == TokenType.RIGHT_BRACKET ||
                tokens[position].Type == TokenType.RIGHT_BRACE ||
                tokens[position].Type == TokenType.SEMICOLON ||
                tokens[position].Type == TokenType.NEWLINE;
                
            if (isProjection)
            {
                // This is a projection - create a ProjectedFunction node
                var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                
                // Convert token type to operator symbol
                var operatorSymbol = VerbRegistry.GetDyadicOperatorSymbol(token.Type);
                
                projectedNode.Value = new SymbolValue(operatorSymbol);
                
                // Determine arity from VerbRegistry - use highest supported arity as default
                var verb = VerbRegistry.GetVerb(operatorSymbol);
                int defaultArity = verb?.SupportedArities?.Max() ?? 2;
                projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(defaultArity)));
                
                return projectedNode;
            }
            
            var operand = ParseExpressionInGrouping(ref position);
            if (operand == null)
            {
                throw new Exception($"Expected expression after monadic operator {token.Lexeme}");
            }
            
            return ASTNode.MakeDyadicOp(token.Type, operand, ASTNode.MakeLiteral(new NullValue()));
        }
    }
}
