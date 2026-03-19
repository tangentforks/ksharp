using System;
using System.Collections.Generic;
using K3CSharp.Parsing;

namespace K3CSharp
{
    /// <summary>
    /// Function parser module for handling functions, lambda expressions, and control flow
    /// </summary>
    public class FunctionParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // Function parser handles function-related tokens and control flow
            return currentToken == TokenType.IDENTIFIER ||
                   currentToken == TokenType.IF_FUNC ||
                   currentToken == TokenType.WHILE ||
                   currentToken == TokenType.DO ||
                   currentToken == TokenType.LEFT_BRACE;
        }

        public ASTNode? Parse(ParseContext context)
        {
            var token = context.CurrentToken();
            
            return token.Type switch
            {
                TokenType.IDENTIFIER => ParseIdentifierOrFunctionCall(context),
                TokenType.IF_FUNC => ParseIfStatement(context),
                TokenType.WHILE => ParseWhileStatement(context),
                TokenType.DO => ParseDoStatement(context),
                TokenType.LEFT_BRACE => ParseLambdaExpression(context),
                _ => throw new Exception($"Unexpected function token: {token.Type}({token.Lexeme})")
            };
        }

        private ASTNode ParseIdentifierOrFunctionCall(ParseContext context)
        {
            var token = context.CurrentToken();
            context.Advance();
            
            var identifier = token.Lexeme;
            
            // Check if this is a control flow verb
            if (identifier == "if" || identifier == "while" || identifier == "do")
            {
                return ParseControlFlowVerb(identifier, context);
            }
            
            // Check if followed by brackets for function call
            if (context.Check(TokenType.LEFT_BRACKET))
            {
                var args = ParseFunctionArguments(context);
                return ASTNode.MakeFunctionCall(ASTNode.MakeVariable(identifier), args);
            }
            
            // Simple variable reference
            return ASTNode.MakeVariable(identifier);
        }

        private ASTNode ParseControlFlowVerb(string verbName, ParseContext context)
        {
            if (!context.Check(TokenType.LEFT_BRACKET))
            {
                throw new Exception($"Expected '[' after control flow verb '{verbName}'");
            }
            
            var args = ParseFunctionArguments(context);
            var funcCall = ASTNode.MakeFunctionCall(ASTNode.MakeVariable(verbName), args);
            
            return funcCall;
        }

        private List<ASTNode> ParseFunctionArguments(ParseContext context)
        {
            var args = new List<ASTNode>();
            
            if (!context.Check(TokenType.LEFT_BRACKET))
            {
                return args;
            }
            
            context.Advance(); // Consume '['
            
            // Parse arguments separated by semicolons
            while (!context.IsAtEnd() && context.CurrentToken().Type != TokenType.RIGHT_BRACKET)
            {
                var arg = ParseFunctionArgument(context);
                if (arg != null)
                {
                    args.Add(arg);
                }
                
                // Skip empty lines
                while (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.NEWLINE)
                {
                    context.Match(TokenType.NEWLINE);
                }
                
                if (context.Match(TokenType.SEMICOLON))
                {
                    continue; // Parse next argument
                }
                else if (context.Check(TokenType.RIGHT_BRACKET))
                {
                    break; // End of arguments
                }
            }
            
            context.Advance(); // Consume ']'
            return args;
        }

        private ASTNode? ParseFunctionArgument(ParseContext context)
        {
            if (context.IsAtEnd())
            {
                return null;
            }
            
            // Use expression parser for function arguments
            // Try LRS Expression Processor integration first
            try
            {
                var lrsResult = TestLRSExpressionProcessor(context);
                if (lrsResult != null)
                {
                    return lrsResult;
                }
            }
            catch
            {
                // Fall back to original parsing if LRS fails
            }
            
            var expressionParser = new ExpressionParser();
            if (expressionParser.CanHandle(context.CurrentToken().Type))
            {
                return expressionParser.Parse(context);
            }
            
            // Fall back to primary parser
            var primaryParser = new PrimaryParser();
            if (primaryParser.CanHandle(context.CurrentToken().Type))
            {
                return primaryParser.Parse(context);
            }
            
            return null;
        }

        private ASTNode ParseIfStatement(ParseContext context)
        {
            context.Advance(); // Consume 'if'
            
            if (!context.Check(TokenType.LEFT_BRACKET))
            {
                throw new Exception("Expected '[' after 'if'");
            }
            
            var args = ParseFunctionArguments(context);
            
            if (args.Count < 2 || args.Count > 3)
            {
                throw new Exception("'if' requires 2 or 3 arguments: if[condition;true_expr;false_expr]");
            }
            
            return ASTNode.MakeFunctionCall(ASTNode.MakeVariable("if"), args);
        }

        private ASTNode ParseWhileStatement(ParseContext context)
        {
            context.Advance(); // Consume 'while'
            
            if (!context.Check(TokenType.LEFT_BRACKET))
            {
                throw new Exception("Expected '[' after 'while'");
            }
            
            var args = ParseFunctionArguments(context);
            
            if (args.Count != 2)
            {
                throw new Exception("'while' requires exactly 2 arguments: while[condition;body]");
            }
            
            return ASTNode.MakeFunctionCall(ASTNode.MakeVariable("while"), args);
        }

        private ASTNode ParseDoStatement(ParseContext context)
        {
            context.Advance(); // Consume 'do'
            
            if (!context.Check(TokenType.LEFT_BRACKET))
            {
                throw new Exception("Expected '[' after 'do'");
            }
            
            var args = ParseFunctionArguments(context);
            
            if (args.Count != 1)
            {
                throw new Exception("'do' requires exactly 1 argument: do[body]");
            }
            
            return ASTNode.MakeFunctionCall(ASTNode.MakeVariable("do"), args);
        }

        private ASTNode ParseLambdaExpression(ParseContext context)
        {
            context.Advance(); // Consume '{'
            
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
            
            context.Advance(); // Consume '}'
            
            return ASTNode.MakeFunction(parameters, body);
        }
        
        /// <summary>
        /// Test LRS Expression Processor integration for function arguments
        /// </summary>
        private ASTNode? TestLRSExpressionProcessor(ParseContext context)
        {
            // Extract tokens from current position to appropriate end
            var tokens = new List<Token>();
            var startPos = context.Current;
            var parenDepth = 0;
            var braceDepth = 0;
            
            while (context.Current < context.Tokens.Count)
            {
                var token = context.Tokens[context.Current];
                tokens.Add(token);
                
                if (token.Type == TokenType.LEFT_PAREN) parenDepth++;
                else if (token.Type == TokenType.RIGHT_PAREN) parenDepth--;
                else if (token.Type == TokenType.LEFT_BRACE) braceDepth++;
                else if (token.Type == TokenType.RIGHT_BRACE) braceDepth--;
                
                // Stop if we reach a separator or end of expression
                if ((parenDepth == 0 && braceDepth == 0) && 
                    (token.Type == TokenType.SEMICOLON || token.Type == TokenType.RIGHT_PAREN))
                {
                    break;
                }
                
                context.Current++;
            }
            
            // Remove the final separator token if present
            if (tokens.Count > 0 && (tokens.Last().Type == TokenType.SEMICOLON || tokens.Last().Type == TokenType.RIGHT_PAREN))
            {
                tokens.RemoveAt(tokens.Count - 1);
            }
            
            // Reset position for LRS processing
            context.Current = startPos;
            
            // Use factory to create LRS Expression Processor with dependency injection
            var processor = LRSParserFactory.CreateExpressionProcessor(tokens, false);
            var position = 0;
            var result = processor.ProcessExpression(ref position);
            
            // Update context position to after processed tokens
            context.Current = startPos + position;
            
            return result;
        }

        /// <summary>
        /// Parse projected function (partial application)
        /// </summary>
        public static ASTNode MakeProjectedFunction(string verbSymbol, int requiredArgs)
        {
            var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
            projectedNode.Value = new SymbolValue(verbSymbol);
            
            // Add placeholder arguments for missing parameters
            for (int i = 0; i < requiredArgs; i++)
            {
                projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1)));
            }
            
            return projectedNode;
        }
    }
}
