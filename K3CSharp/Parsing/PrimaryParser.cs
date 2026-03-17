using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Primary parser module for handling literals, identifiers, and unary operations
    /// </summary>
    public class PrimaryParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // Primary parser handles literals, identifiers, and unary operators
            return currentToken switch
            {
                TokenType.INTEGER or TokenType.LONG or TokenType.FLOAT or TokenType.CHARACTER or 
                TokenType.CHARACTER_VECTOR or TokenType.SYMBOL or TokenType.IDENTIFIER or
                TokenType.LEFT_PAREN or TokenType.LEFT_BRACE or TokenType.LEFT_BRACKET or
                TokenType.PLUS or TokenType.MINUS or TokenType.MULTIPLY or TokenType.DIVIDE or
                TokenType.MODULUS or TokenType.POWER or TokenType.MIN or TokenType.MAX or
                TokenType.MATCH or TokenType.NOT or TokenType.HASH or TokenType.UNDERSCORE or
                TokenType.QUESTION or TokenType.DOLLAR or TokenType.APPLY or
                TokenType.NULL => true,
                _ => false
            };
        }

        public ASTNode? Parse(ParseContext context)
        {
            var token = context.CurrentToken();
            
            return token.Type switch
            {
                TokenType.INTEGER => ParseInteger(context),
                TokenType.LONG => ParseLong(context),
                TokenType.FLOAT => ParseFloat(context),
                TokenType.CHARACTER => ParseCharacter(context),
                TokenType.CHARACTER_VECTOR => ParseString(context),
                TokenType.SYMBOL => ParseSymbol(context),
                TokenType.IDENTIFIER => ParseIdentifier(context),
                TokenType.LEFT_PAREN => ParseParentheses(context),
                TokenType.LEFT_BRACE => ParseBraces(context),
                TokenType.LEFT_BRACKET => ParseBrackets(context),
                TokenType.NULL => ParseNull(context),
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
                _ => throw new Exception($"Unexpected token: {token.Type}({token.Lexeme})")
            };
        }

        private ASTNode ParseInteger(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            
            // Handle special integer values per K specification
            if (lexeme == "0I" || lexeme == "-0I")
                return ASTNode.MakeLiteral(new IntegerValue(lexeme));
            
            if (int.TryParse(lexeme, out int intValue))
            {
                // Convert extreme values to special values per spec
                if (intValue >= 2147483647)
                    return ASTNode.MakeLiteral(new IntegerValue("0I"));
                return ASTNode.MakeLiteral(new IntegerValue(intValue));
            }
            
            throw new ArgumentException($"Invalid integer literal: {lexeme}");
        }

        private ASTNode ParseLong(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            
            // Parse with bounds checking
            var numberPart = lexeme.Substring(0, lexeme.Length - 1); // Remove 'j'
            double parsedValue = double.Parse(numberPart);
            
            if (parsedValue >= long.MaxValue)
                return ASTNode.MakeLiteral(new LongValue(long.MaxValue));
            else if (parsedValue <= -long.MaxValue)
                return ASTNode.MakeLiteral(new LongValue(-long.MaxValue));
            else
                return ASTNode.MakeLiteral(new LongValue(long.Parse(numberPart)));
        }

        private ASTNode ParseFloat(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            
            // Handle special float values
            if (lexeme == "0i" || lexeme == "0n" || lexeme == "-0i")
                return ASTNode.MakeLiteral(new FloatValue(lexeme));
            
            return ASTNode.MakeLiteral(new FloatValue(double.Parse(lexeme)));
        }

        private ASTNode ParseCharacter(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            
            // Handle escape sequences
            if (lexeme.Length == 3 && lexeme[0] == '\'' && lexeme[2] == '\'')
            {
                char c = lexeme[1];
                var charValue = c switch
                {
                    'n' => '\n',
                    't' => '\t',
                    'r' => '\r',
                    '\\' => '\\',
                    '\'' => '\'',
                    _ => c
                };
                return ASTNode.MakeLiteral(new CharacterValue(charValue.ToString()));
            }
            
            throw new ArgumentException($"Invalid character literal: {lexeme}");
        }

        private ASTNode ParseString(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            
            // Remove quotes and handle escape sequences
            if (lexeme.Length >= 2 && lexeme[0] == '"' && lexeme[^1] == '"')
            {
                var stringValue = lexeme.Substring(1, lexeme.Length - 2);
                return ASTNode.MakeLiteral(new SymbolValue(stringValue));
            }
            
            throw new ArgumentException($"Invalid string literal: {lexeme}");
        }

        private ASTNode ParseSymbol(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var lexeme = token.Lexeme;
            
            // Remove backticks for symbol value
            var symbolValue = lexeme.Trim('`');
            return ASTNode.MakeLiteral(new SymbolValue(symbolValue));
        }

        private ASTNode ParseIdentifier(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            return ASTNode.MakeVariable(token.Lexeme);
        }

        private ASTNode ParseParentheses(ParseContext context)
        {
            context.Advance(); // Consume '('

            // Parse expressions inside parentheses according to K specification
            // Semicolons and newlines are expression separators
            // Commas are OPERATORS (join/enlist), not separators
            var elements = new List<ASTNode>();
            
            // Handle empty parentheses () - should be treated as 1 expression of value null
            if (context.Check(TokenType.RIGHT_PAREN))
            {
                context.Advance(); // Consume ')'
                return ASTNode.MakeLiteral(new NullValue());
            }
            
            // Parse expressions sequentially from left to right
            while (!context.IsAtEnd() && !context.Check(TokenType.RIGHT_PAREN))
            {
                // Skip newlines as they are also expression separators
                while (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.NEWLINE)
                {
                    context.Advance();
                }
                
                // Check for empty expressions (consecutive separators)
                if (context.CurrentToken().Type == TokenType.SEMICOLON || 
                    context.CurrentToken().Type == TokenType.NEWLINE)
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                    context.Advance(); // Consume the separator
                    continue;
                }
                
                // Parse the expression (commas are operators, so they're handled as part of expressions)
                var expr = ParseExpressionInGrouping(context);
                if (expr != null)
                {
                    elements.Add(expr);
                }
                else
                {
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
                
                // Check for separator (semicolon or newline only - commas are operators!)
                if (context.Match(TokenType.SEMICOLON))
                {
                    continue; // Next expression
                }
                else if (context.Match(TokenType.NEWLINE))
                {
                    continue; // Next expression
                }
                else if (context.Check(TokenType.RIGHT_PAREN))
                {
                    break; // End of parentheses
                }
                else
                {
                    // No separator and not at end - this should not happen in valid K
                    break;
                }
            }
            
            context.Advance(); // Consume ')'
            
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

        private ASTNode? ParseBracketArgument(ParseContext context)
        {
            // Use expression parser for bracket arguments
            var expressionParser = new ExpressionParser();
            if (expressionParser.CanHandle(context.CurrentToken().Type))
            {
                return expressionParser.Parse(context);
            }
            
            throw new Exception("Expected expression in brackets");
        }

        private ASTNode? ParseExpressionInGrouping(ParseContext context)
        {
            // Parse a single expression within grouping constructs (parentheses, brackets, braces)
            // This method respects semicolons and newlines as expression separators
            // Commas are OPERATORS (join/enlist), not separators
            // but doesn't consume them unless they're part of current expression
            
            if (context.IsAtEnd())
                return null;
                
            var token = context.CurrentToken();
            
            // Handle primary expressions
            switch (token.Type)
            {
                case TokenType.INTEGER:
                case TokenType.LONG:
                case TokenType.FLOAT:
                case TokenType.CHARACTER:
                case TokenType.CHARACTER_VECTOR:
                case TokenType.SYMBOL:
                case TokenType.IDENTIFIER:
                case TokenType.NULL:
                    return ParsePrimaryToken(context);
                    
                case TokenType.LEFT_PAREN:
                    return ParseParentheses(context);
                    
                case TokenType.LEFT_BRACKET:
                    return ParseBrackets(context);
                    
                case TokenType.LEFT_BRACE:
                    return ParseBraces(context);
                    
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.MULTIPLY:
                case TokenType.DIVIDE:
                case TokenType.MODULUS:
                case TokenType.POWER:
                case TokenType.MIN:
                case TokenType.MAX:
                case TokenType.MATCH:
                case TokenType.NOT:
                case TokenType.HASH:
                case TokenType.UNDERSCORE:
                case TokenType.QUESTION:
                case TokenType.DOLLAR:
                case TokenType.APPLY:
                case TokenType.JOIN: // Comma is an operator, not a separator
                    return ParseUnaryOperator(context);
                    
                default:
                    // Stop at expression separators (semicolons and newlines only)
                    if (token.Type == TokenType.SEMICOLON || 
                        token.Type == TokenType.NEWLINE ||
                        token.Type == TokenType.RIGHT_PAREN ||
                        token.Type == TokenType.RIGHT_BRACKET ||
                        token.Type == TokenType.RIGHT_BRACE)
                    {
                        return null; // End of current expression
                    }
                    
                    throw new Exception($"Unexpected token in expression: {token.Type}({token.Lexeme})");
            }
        }
        
        private ASTNode ParsePrimaryToken(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            return token.Type switch
            {
                TokenType.INTEGER => ASTNode.MakeLiteral(new IntegerValue(int.Parse(token.Lexeme))),
                TokenType.LONG => ASTNode.MakeLiteral(new LongValue(long.Parse(token.Lexeme.Substring(0, token.Lexeme.Length - 1)))),
                TokenType.FLOAT => ASTNode.MakeLiteral(new FloatValue(double.Parse(token.Lexeme))),
                TokenType.CHARACTER => ASTNode.MakeLiteral(new CharacterValue(token.Lexeme)),
                TokenType.CHARACTER_VECTOR => ASTNode.MakeLiteral(new SymbolValue(token.Lexeme.Substring(1, token.Lexeme.Length - 2))),
                TokenType.SYMBOL => ASTNode.MakeLiteral(new SymbolValue(token.Lexeme.Trim('`'))),
                TokenType.IDENTIFIER => ASTNode.MakeVariable(token.Lexeme),
                TokenType.NULL => ASTNode.MakeLiteral(new NullValue()),
                _ => throw new Exception($"Unexpected primary token: {token.Type}")
            };
        }
        
        private ASTNode ParseUnaryOperator(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var operand = ParseExpressionInGrouping(context);
            if (operand == null)
            {
                throw new Exception($"Expected expression after unary operator {token.Lexeme}");
            }
            
            return ASTNode.MakeBinaryOp(token.Type, operand, ASTNode.MakeLiteral(new NullValue()));
        }

        private ASTNode ParseBraces(ParseContext context)
        {
            context.Advance(); // Consume '{'
            
            // Parse function body
            var body = ParseFunctionBody(context);
            
            context.Advance(); // Consume '}'
            
            return body;
        }

        private ASTNode ParseBrackets(ParseContext context)
        {
            context.Advance(); // Consume '['
            
            // Parse bracket contents with proper semicolon separation
            var elements = ParseBracketContentsAsCommaEnlisted(context);
            
            return elements;
        }

        private ASTNode ParseBracketContentsAsCommaEnlisted(ParseContext context)
        {
            var elements = new List<ASTNode>();
            
            if (context.Check(TokenType.RIGHT_BRACKET))
            {
                context.Advance(); // Consume ']'
                return ASTNode.MakeVector(elements);
            }
            
            // Parse first element
            var firstElement = ParseBracketArgument(context);
            if (firstElement == null)
            {
                throw new Exception("Expected expression in brackets");
            }
            
            // Check if this should be treated as indexing instead of function call
            if (firstElement.Type == ASTNodeType.Variable && !context.IsAtEnd() && context.CurrentToken().Type != TokenType.RIGHT_BRACKET)
            {
                // This is indexing: vector[index]
                var index = ParseBracketArgument(context);
                if (index == null)
                {
                    throw new Exception("Expected index in brackets");
                }
                
                context.Advance(); // Consume ']'
                return ASTNode.MakeFunctionCall(firstElement, new List<ASTNode> { index });
            }
            
            // Check if this should be treated as function call
            if (firstElement.Type == ASTNodeType.Variable && !context.IsAtEnd() && context.CurrentToken().Type != TokenType.RIGHT_BRACKET)
            {
                // This is a function call: func[args]
                var args = ParseBracketArguments(context);
                context.Advance(); // Consume ']'
                return ASTNode.MakeFunctionCall(firstElement, args);
            }
            
            // Otherwise, treat as vector construction
            elements.Add(firstElement);
            
            // Parse additional elements separated by semicolons
            while (!context.IsAtEnd() && context.CurrentToken().Type != TokenType.RIGHT_BRACKET)
            {
                if (context.Match(TokenType.SEMICOLON))
                {
                    // Skip empty lines
                    while (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.NEWLINE)
                    {
                        context.Match(TokenType.NEWLINE);
                    }
                    
                    if (!context.IsAtEnd() && context.CurrentToken().Type != TokenType.RIGHT_BRACKET)
                    {
                        elements.Add(ParseBracketArgument(context)!);
                    }
                }
                else
                {
                    elements.Add(ParseBracketArgument(context)!);
                }
            }
            
            context.Advance(); // Consume ']'
            return ASTNode.MakeVector(elements);
        }

        private List<ASTNode> ParseBracketArguments(ParseContext context)
        {
            var args = new List<ASTNode>();
            
            do
            {
                var arg = ParseBracketArgument(context);
                if (arg != null)
                {
                    args.Add(arg);
                }
                
                // Skip empty lines
                while (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.NEWLINE)
                {
                    context.Match(TokenType.NEWLINE);
                }
            } while (!context.IsAtEnd() && context.Match(TokenType.SEMICOLON));
            
            return args;
        }

        private ASTNode ParseFunctionBody(ParseContext context)
        {
            var parameters = new List<string>();
            
            // Parse parameter list if present
            if (context.Check(TokenType.LEFT_BRACKET))
            {
                context.Advance(); // Consume '['
                
                while (!context.IsAtEnd() && context.CurrentToken().Type != TokenType.RIGHT_BRACKET)
                {
                    if (context.CurrentToken().Type == TokenType.IDENTIFIER)
                    {
                        parameters.Add(context.CurrentToken().Lexeme);
                        context.Advance();
                        
                        if (context.Check(TokenType.SEMICOLON))
                        {
                            context.Advance(); // Consume ';'
                        }
                    }
                    else
                    {
                        throw new Exception("Expected identifier in parameter list");
                    }
                }
                
                context.Advance(); // Consume ']'
            }
            
            // Parse function body
            var bodyParser = new ExpressionParser();
            var body = bodyParser.Parse(context) ?? ASTNode.MakeLiteral(new NullValue());
            
            return ASTNode.MakeFunction(parameters, body);
        }

        // Unary operator parsing methods
        private ASTNode ParseUnaryPlus(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("+");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryMinus(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("-");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryReciprocal(ParseContext context)
        {
            context.Advance();
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

        private ASTNode ParseUnaryPower(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("^");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseNull(ParseContext context)
        {
            context.Advance();
            return ASTNode.MakeLiteral(new NullValue());
        }

        // Missing unary operator methods
        private ASTNode ParseUnaryFirst(ParseContext context)
        {
            context.Advance();
            
            // For monadic operators, try to parse a vector if multiple elements are present
            var elements = new List<ASTNode>();
            
            while (!context.IsAtEnd() && !IsExpressionTerminator(context.CurrentToken().Type))
            {
                var element = ParsePrimaryToken(context);
                if (element != null)
                {
                    elements.Add(element);
                }
                else
                {
                    break;
                }
            }
            
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("*");
            
            if (elements.Count == 1)
            {
                // Single element
                node.Children.Add(elements[0]);
            }
            else if (elements.Count > 1)
            {
                // Multiple elements - create a vector
                var vectorNode = ASTNode.MakeVector(elements);
                node.Children.Add(vectorNode);
            }
            else
            {
                // No elements - this shouldn't happen
                throw new Exception("Monadic * operator requires an operand");
            }
            
            return node;
        }
        
        private bool IsExpressionTerminator(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.SEMICOLON or TokenType.NEWLINE or TokenType.EOF or
                TokenType.RIGHT_PAREN or TokenType.RIGHT_BRACKET or TokenType.RIGHT_BRACE => true,
                _ => false
            };
        }

        private ASTNode ParseUnaryMin(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("&");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryMax(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("|");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryMatch(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("~");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryNot(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("~");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryCount(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("#");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryFloor(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("_");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryUnique(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("?");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryFormat(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("$");
            if (operand != null) node.Children.Add(operand);
            return node;
        }

        private ASTNode ParseUnaryApply(ParseContext context)
        {
            context.Advance();
            var operand = ParseBracketArgument(context);
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("@");
            if (operand != null) node.Children.Add(operand);
            return node;
        }
    }
}
