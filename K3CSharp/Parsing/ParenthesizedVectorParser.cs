using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Specialized parser for parenthesized expressions in semicolon vectors
    /// Handles parsing of (a;b;c) semicolon-separated vectors and (a b c) space-separated vectors
    /// </summary>
    public class ParenthesizedVectorParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // This parser is used specifically for parsing parenthesized expressions in semicolon vectors
            // It's not directly called by token type, but by the parser when needed
            return false;
        }

        public ASTNode? Parse(ParseContext context)
        {
            // This method is not used directly - ParseParenthesizedForElement is the main entry point
            throw new NotImplementedException("Use ParseParenthesizedForElement method instead");
        }

        /// <summary>
        /// Parse a parenthesized expression as a single element, handling internal semicolons properly
        /// This method handles both semicolon-separated vectors (a;b;c) and space-separated vectors (a b c)
        /// </summary>
        public static ASTNode ParseParenthesizedForElement(ParseContext context)
        {
            // Check if this is an empty parenthesized expression
            if (context.Match(TokenType.RIGHT_PAREN))
            {
                // Empty parentheses - create empty vector
                return ASTNode.MakeVector(new List<ASTNode>());
            }
            
            // Look ahead to see if we have semicolons
            var hasSemicolons = false;
            var tokenCount = 0;
            var currentPos = context.Current;
            var parenLevel = 1; // Start with 1 because we're already inside one (
            
            while (!context.IsAtEnd() && parenLevel > 0)
            {
                if (context.CurrentToken().Type == TokenType.LEFT_PAREN)
                {
                    parenLevel++;
                }
                else if (context.CurrentToken().Type == TokenType.RIGHT_PAREN)
                {
                    parenLevel--;
                }
                else if (context.CurrentToken().Type == TokenType.SEMICOLON && parenLevel == 1)
                {
                    hasSemicolons = true;
                    break;
                }
                context.Advance();
                tokenCount++;
                if (tokenCount > 20) break; // Safety check
            }
            
            // Reset position
            context.Current = currentPos;
            
            if (hasSemicolons)
            {
                return ParseSemicolonSeparatedVector(context);
            }
            else
            {
                return ParseSpaceSeparatedVector(context);
            }
        }

        private static ASTNode ParseSemicolonSeparatedVector(ParseContext context)
        {
            // Parse semicolon-separated vector
            var elements = new List<ASTNode>();
            
            // Check if vector starts with empty position
            if (context.CurrentToken().Type == TokenType.SEMICOLON)
            {
                elements.Add(ASTNode.MakeLiteral(new NullValue()));
                context.Match(TokenType.SEMICOLON);
                
                // Handle multiple consecutive semicolons at start
                while (context.CurrentToken().Type == TokenType.SEMICOLON)
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                    context.Match(TokenType.SEMICOLON);
                }
            }
            
            // Parse first element if not at end
            if (context.CurrentToken().Type != TokenType.RIGHT_PAREN)
            {
                var beforeParsePos = context.Current;
                var expr = ParseExpression(context);
                
                // If we didn't make progress, break to avoid infinite loop
                if (context.Current == beforeParsePos)
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
                else if (expr != null)
                {
                    elements.Add(expr);
                }
                else
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
            }
            
            // Parse remaining elements separated by semicolons
            while (context.Match(TokenType.SEMICOLON))
            {
                // Handle empty position (consecutive semicolons)
                if (context.CurrentToken().Type == TokenType.RIGHT_PAREN)
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
                else
                {
                    var beforeParsePos = context.Current;
                    var expr = ParseExpression(context);
                    
                    // If we didn't make progress, break to avoid infinite loop
                    if (context.Current == beforeParsePos)
                    {
                        elements.Add(ASTNode.MakeLiteral(new NullValue()));
                    }
                    else if (expr != null)
                    {
                        elements.Add(expr);
                    }
                    else
                    {
                        elements.Add(ASTNode.MakeLiteral(new NullValue()));
                    }
                }
            }
            
            if (!context.Match(TokenType.RIGHT_PAREN))
            {
                throw new Exception("Expected ')' after expression");
            }
            
            // Return the vector
            return ASTNode.MakeVector(elements);
        }

        private static ASTNode ParseSpaceSeparatedVector(ParseContext context)
        {
            // Parse space-separated vector
            var expression = ParseExpression(context);
            
            if (!context.Match(TokenType.RIGHT_PAREN))
            {
                throw new Exception("Expected ')' after expression");
            }
            
            // If the expression is a vector, keep it as a vector
            // Otherwise, return the expression as-is
            return expression ?? ASTNode.MakeLiteral(new NullValue());
        }

        private static ASTNode? ParseExpression(ParseContext context)
        {
            // Simplified expression parsing for parenthesized vectors
            // In a full implementation, this would delegate to ExpressionParser
            return ParseTerm(context);
        }

        private static ASTNode? ParseTerm(ParseContext context)
        {
            // Simplified term parsing
            // In a full implementation, this would delegate to PrimaryParser
            return ParsePrimary(context);
        }

        private static ASTNode? ParsePrimary(ParseContext context)
        {
            if (context.IsAtEnd())
            {
                return null;
            }

            var token = context.CurrentToken();
            
            // Handle basic literals and identifiers
            return token.Type switch
            {
                TokenType.INTEGER => ParseInteger(context),
                TokenType.FLOAT => ParseFloat(context),
                TokenType.SYMBOL => ParseSymbol(context),
                TokenType.QUOTE => ParseString(context),
                TokenType.IDENTIFIER => ParseIdentifier(context),
                TokenType.LEFT_PAREN => ParseNestedParenthesizedExpression(context),
                TokenType.LEFT_BRACE => ParseBraceExpression(context),
                TokenType.LEFT_BRACKET => ParseBracketExpression(context),
                _ => throw new Exception($"Unexpected token in parenthesized expression: {token.Type}({token.Lexeme})")
            };
        }

        private static ASTNode ParseInteger(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            if (lexeme == "0I" || lexeme == "0N" || lexeme == "-0I")
                return ASTNode.MakeLiteral(new IntegerValue(lexeme));
            
            if (int.TryParse(lexeme, out int intValue))
            {
                if (intValue >= 2147483647)
                    return ASTNode.MakeLiteral(new IntegerValue("0I"));
                else if (intValue <= -2147483648)
                    return ASTNode.MakeLiteral(new IntegerValue("-0I"));
                else
                    return ASTNode.MakeLiteral(new IntegerValue(intValue));
            }
            
            throw new ArgumentException($"Invalid integer literal: {lexeme}");
        }

        private static ASTNode ParseFloat(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            if (double.TryParse(lexeme, out double floatValue))
            {
                return ASTNode.MakeLiteral(new FloatValue(floatValue));
            }
            
            throw new ArgumentException($"Invalid float literal: {lexeme}");
        }

        private static ASTNode ParseSymbol(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            var symbolValue = lexeme.Trim('`');
            return ASTNode.MakeLiteral(new SymbolValue(symbolValue));
        }

        private static ASTNode ParseString(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            if (lexeme.Length >= 2 && lexeme[0] == '"' && lexeme[^1] == '"')
            {
                var stringValue = lexeme.Substring(1, lexeme.Length - 2);
                return ASTNode.MakeLiteral(new SymbolValue(stringValue));
            }
            
            throw new ArgumentException($"Invalid string literal: {lexeme}");
        }

        private static ASTNode ParseIdentifier(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            return ASTNode.MakeVariable(token.Lexeme);
        }

        private static ASTNode ParseNestedParenthesizedExpression(ParseContext context)
        {
            context.Advance(); // Consume '('
            
            var result = ParseExpression(context);
            
            if (!context.Match(TokenType.RIGHT_PAREN))
            {
                throw new Exception("Expected ')' after parenthesized expression");
            }
            
            return result ?? throw new Exception("Expected expression inside parentheses");
        }

        private static ASTNode ParseBraceExpression(ParseContext context)
        {
            context.Advance(); // Consume '{'
            
            var result = ParseExpression(context);
            
            if (!context.Match(TokenType.RIGHT_BRACE))
            {
                throw new Exception("Expected '}' after brace expression");
            }
            
            return result ?? throw new Exception("Expected expression inside braces");
        }

        private static ASTNode ParseBracketExpression(ParseContext context)
        {
            context.Advance(); // Consume '['
            
            var result = ParseExpression(context);
            
            if (!context.Match(TokenType.RIGHT_BRACKET))
            {
                throw new Exception("Expected ']' after bracket expression");
            }
            
            return result ?? throw new Exception("Expected expression inside brackets");
        }
    }
}
