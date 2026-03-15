using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Specialized parser for unary operators with consistent handling
    /// This handles unary +, -, *, %, ^, &, |, ~, !, #, _, ?, $, @ operators
    /// </summary>
    public class UnaryOperatorParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            return IsUnaryOperatorToken(currentToken);
        }

        public ASTNode? Parse(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            return token.Type switch
            {
                TokenType.PLUS => ParseUnaryPlus(context),
                TokenType.MINUS => ParseUnaryMinus(context),
                TokenType.MULTIPLY => ParseUnaryFirst(context),
                TokenType.DIVIDE => ParseUnaryReciprocal(context),
                TokenType.MODULUS => ParseUnaryReciprocal(context),
                TokenType.POWER => ParseUnaryPower(context),
                TokenType.MIN => ParseUnaryMin(context),
                TokenType.MAX => ParseUnaryMax(context),
                TokenType.MATCH => ParseUnaryMatch(context),
                TokenType.NOT => ParseUnaryNot(context),
                TokenType.HASH => ParseUnaryCount(context),
                TokenType.UNDERSCORE => ParseUnaryFloor(context),
                TokenType.QUESTION => ParseUnaryUnique(context),
                TokenType.DOLLAR => ParseUnaryFormat(context),
                TokenType.APPLY => ParseUnaryApply(context),
                _ => throw new Exception($"Unexpected unary operator: {token.Type}({token.Lexeme})")
            };
        }

        /// <summary>
        /// Parse unary operator with proper context checking
        /// </summary>
        public static ASTNode? ParseUnaryOperator(TokenType tokenType, ParseContext context, bool isAtExpressionStart)
        {
            context.Advance();
            
            return tokenType switch
            {
                TokenType.PLUS => ParseUnaryPlus(context),
                TokenType.MINUS => ParseUnaryMinus(context),
                TokenType.MULTIPLY => ParseUnaryFirst(context),
                TokenType.DIVIDE => ParseUnaryReciprocal(context),
                TokenType.MODULUS => ParseUnaryReciprocal(context),
                TokenType.POWER => ParseUnaryPower(context),
                TokenType.MIN => ParseUnaryMin(context),
                TokenType.MAX => ParseUnaryMax(context),
                TokenType.MATCH => ParseUnaryMatch(context),
                TokenType.NOT => ParseUnaryNot(context),
                TokenType.HASH => ParseUnaryCount(context),
                TokenType.UNDERSCORE => ParseUnaryFloor(context),
                TokenType.QUESTION => ParseUnaryUnique(context),
                TokenType.DOLLAR => ParseUnaryFormat(context),
                TokenType.APPLY => ParseUnaryApply(context),
                _ => throw new Exception($"Unexpected unary operator: {tokenType}")
            };
        }

        private static ASTNode ParseUnaryPlus(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("+");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryMinus(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("-");
            
            // Ensure we always have at least one child for unary operators
            if (operand != null)
            {
                node.Children.Add(operand);
            }
            else
            {
                // Create a placeholder operand for incomplete unary expression
                node.Children.Add(ASTNode.MakeLiteral(new NullValue()));
            }
            
            return node;
        }

        private static ASTNode ParseUnaryFirst(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("*");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryReciprocal(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
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

        private static ASTNode ParseUnaryPower(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("^");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryMin(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("&");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryMax(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("|");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryMatch(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("~");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryNot(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("~");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryCount(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("#");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryFloor(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("_");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryUnique(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("?");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryFormat(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("$");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private static ASTNode ParseUnaryApply(ParseContext context)
        {
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("@");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        /// <summary>
        /// Parse bracket argument for unary operators
        /// </summary>
        private static ASTNode? ParseBracketArgument(ParseContext context)
        {
            if (context.IsAtEnd())
            {
                return null;
            }
            
            // For now, use a simple approach - delegate to existing parsing
            // In a full implementation, this would use the ExpressionParser
            return ParseSimpleExpression(context);
        }

        /// <summary>
        /// Simple expression parsing for unary operator arguments
        /// </summary>
        private static ASTNode? ParseSimpleExpression(ParseContext context)
        {
            if (context.IsAtEnd())
            {
                return null;
            }

            var token = context.CurrentToken();
            
            return token.Type switch
            {
                TokenType.INTEGER => ParseInteger(context),
                TokenType.FLOAT => ParseFloat(context),
                TokenType.SYMBOL => ParseSymbol(context),
                TokenType.QUOTE => ParseString(context),
                TokenType.IDENTIFIER => ParseIdentifier(context),
                TokenType.LEFT_PAREN => ParseParenthesizedExpression(context),
                TokenType.LEFT_BRACE => ParseBraceExpression(context),
                TokenType.LEFT_BRACKET => ParseBracketExpression(context),
                _ => throw new Exception($"Unexpected token in unary operator argument: {token.Type}({token.Lexeme})")
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

        private static ASTNode ParseParenthesizedExpression(ParseContext context)
        {
            context.Advance(); // Consume '('
            
            var result = ParseSimpleExpression(context);
            
            if (!context.Match(TokenType.RIGHT_PAREN))
            {
                throw new Exception("Expected ')' after parenthesized expression");
            }
            
            return result ?? throw new Exception("Expected expression inside parentheses");
        }

        private static ASTNode ParseBraceExpression(ParseContext context)
        {
            context.Advance(); // Consume '{'
            
            var result = ParseSimpleExpression(context);
            
            if (!context.Match(TokenType.RIGHT_BRACE))
            {
                throw new Exception("Expected '}' after brace expression");
            }
            
            return result ?? throw new Exception("Expected expression inside braces");
        }

        private static ASTNode ParseBracketExpression(ParseContext context)
        {
            context.Advance(); // Consume '['
            
            var result = ParseSimpleExpression(context);
            
            if (!context.Match(TokenType.RIGHT_BRACKET))
            {
                throw new Exception("Expected ']' after bracket expression");
            }
            
            return result ?? throw new Exception("Expected expression inside brackets");
        }

        public static bool IsUnaryOperatorToken(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.PLUS or TokenType.MINUS or TokenType.MULTIPLY or TokenType.DIVIDE or 
                TokenType.MODULUS or TokenType.POWER or TokenType.MIN or TokenType.MAX or
                TokenType.MATCH or TokenType.NOT or TokenType.HASH or TokenType.UNDERSCORE or
                TokenType.QUESTION or TokenType.DOLLAR or TokenType.APPLY => true,
                _ => false
            };
        }

        /// <summary>
        /// Check if a token could be a unary operator in the current context
        /// </summary>
        public static bool CouldBeUnaryOperator(TokenType tokenType, bool isAtExpressionStart)
        {
            if (!IsUnaryOperatorToken(tokenType))
                return false;
            
            // Most unary operators are only valid at the start of an expression
            // except for specific cases like function arguments
            return isAtExpressionStart || tokenType == TokenType.PLUS;
        }
    }
}
