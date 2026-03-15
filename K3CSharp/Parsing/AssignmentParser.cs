using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Assignment parser module for handling assignments and apply-and-assign operations
    /// </summary>
    public class AssignmentParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // Assignment parser handles assignment operators and apply-and-assign operations
            return currentToken == TokenType.ASSIGNMENT ||
                   currentToken == TokenType.GLOBAL_ASSIGNMENT ||
                   currentToken == TokenType.COLON ||
                   currentToken == TokenType.IDENTIFIER;
        }

        public ASTNode? Parse(ParseContext context)
        {
            var token = context.CurrentToken();
            
            return token.Type switch
            {
                TokenType.ASSIGNMENT => ParseAssignment(context),
                TokenType.GLOBAL_ASSIGNMENT => ParseGlobalAssignment(context),
                TokenType.COLON => ParseApplyAndAssign(context),
                TokenType.IDENTIFIER => ParseIdentifierAssignment(context),
                _ => throw new Exception($"Unexpected assignment token: {token.Type}({token.Lexeme})")
            };
        }

        private ASTNode ParseAssignment(ParseContext context)
        {
            context.Advance(); // Consume ':'
            
            // Check if this is a variable assignment
            if (context.Check(TokenType.IDENTIFIER))
            {
                var variableName = context.CurrentToken().Lexeme;
                context.Advance(); // Consume variable name
                
                // Check for apply-and-assign pattern: variable:op expression
                if (IsBinaryOperator(context.CurrentToken().Type))
                {
                    return ParseApplyAndAssignVariable(variableName, context);
                }
                
                // Parse the value to assign
                var value = ParseAssignmentValue(context);
                if (value == null)
                {
                    throw new Exception("Expected expression after assignment operator");
                }
                
                return ASTNode.MakeAssignment(variableName, value);
            }
            
            // Simple assignment expression
            var expression = ParseAssignmentValue(context);
            if (expression == null)
            {
                throw new Exception("Expected expression after assignment operator");
            }
            
            return expression;
        }

        private ASTNode ParseGlobalAssignment(ParseContext context)
        {
            context.Advance(); // Consume '::'
            
            if (!context.Check(TokenType.IDENTIFIER))
            {
                throw new Exception("Expected variable name after global assignment operator");
            }
            
            var variableName = context.CurrentToken().Lexeme;
            context.Advance(); // Consume variable name
            
            var value = ParseAssignmentValue(context);
            if (value == null)
            {
                throw new Exception("Expected expression after global assignment operator");
            }
            
            return ASTNode.MakeGlobalAssignment(variableName, value);
        }

        private ASTNode ParseApplyAndAssign(ParseContext context)
        {
            context.Advance(); // Consume ':'
            
            if (!context.Check(TokenType.IDENTIFIER))
            {
                throw new Exception("Expected variable name after apply-and-assign operator");
            }
            
            var variableName = context.CurrentToken().Lexeme;
            context.Advance(); // Consume variable name
            
            return ParseApplyAndAssignVariable(variableName, context);
        }

        private ASTNode ParseApplyAndAssignVariable(string variableName, ParseContext context)
        {
            if (!IsBinaryOperator(context.CurrentToken().Type))
            {
                throw new Exception("Expected binary operator after variable name in apply-and-assign");
            }
            
            var opToken = context.CurrentToken();
            context.Advance(); // Consume operator
            
            var rightArgument = ParseAssignmentValue(context);
            if (rightArgument == null)
            {
                throw new Exception("Expected right argument in apply-and-assign operation");
            }
            
            return ASTNode.MakeApplyAndAssign(variableName, opToken.Type, rightArgument);
        }

        private ASTNode ParseIdentifierAssignment(ParseContext context)
        {
            var identifier = context.CurrentToken().Lexeme;
            context.Advance(); // Consume identifier
            
            // Check if this is followed by an assignment operator
            if (context.Check(TokenType.ASSIGNMENT))
            {
                return ParseAssignmentWithIdentifier(identifier, context);
            }
            else if (context.Check(TokenType.GLOBAL_ASSIGNMENT))
            {
                return ParseGlobalAssignmentWithIdentifier(identifier, context);
            }
            else if (context.Check(TokenType.COLON))
            {
                return ParseApplyAndAssignWithIdentifier(identifier, context);
            }
            
            // Not an assignment, return as variable
            return ASTNode.MakeVariable(identifier);
        }

        private ASTNode ParseAssignmentWithIdentifier(string identifier, ParseContext context)
        {
            context.Advance(); // Consume ':'
            
            var value = ParseAssignmentValue(context);
            if (value == null)
            {
                throw new Exception("Expected expression after assignment operator");
            }
            
            return ASTNode.MakeAssignment(identifier, value);
        }

        private ASTNode ParseGlobalAssignmentWithIdentifier(string identifier, ParseContext context)
        {
            context.Advance(); // Consume '::'
            
            var value = ParseAssignmentValue(context);
            if (value == null)
            {
                throw new Exception("Expected expression after global assignment operator");
            }
            
            return ASTNode.MakeGlobalAssignment(identifier, value);
        }

        private ASTNode ParseApplyAndAssignWithIdentifier(string identifier, ParseContext context)
        {
            context.Advance(); // Consume ':'
            
            if (!IsBinaryOperator(context.CurrentToken().Type))
            {
                throw new Exception("Expected binary operator after variable name in apply-and-assign");
            }
            
            var opToken = context.CurrentToken();
            context.Advance(); // Consume operator
            
            var rightArgument = ParseAssignmentValue(context);
            if (rightArgument == null)
            {
                throw new Exception("Expected right argument in apply-and-assign operation");
            }
            
            return ASTNode.MakeApplyAndAssign(identifier, opToken.Type, rightArgument);
        }

        private ASTNode? ParseAssignmentValue(ParseContext context)
        {
            if (context.IsAtEnd())
            {
                return null;
            }
            
            // Use expression parser for assignment values
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

        private bool IsBinaryOperator(TokenType tokenType)
        {
            return tokenType switch
            {
                // Arithmetic operators
                TokenType.PLUS or TokenType.MINUS or TokenType.MULTIPLY or TokenType.DIVIDE or 
                TokenType.MODULUS or TokenType.POWER or
                // Comparison operators
                TokenType.LESS or TokenType.GREATER or TokenType.EQUAL or
                // Logical operators
                TokenType.MIN or TokenType.MAX or TokenType.MATCH or
                // System functions that are binary
                TokenType.DIV or TokenType.AND or TokenType.OR or TokenType.XOR or
                TokenType.ROT or TokenType.SHIFT or TokenType.DOT_PRODUCT or TokenType.MUL or
                TokenType.IN or TokenType.BIN or TokenType.BINL or TokenType.LIN or
                TokenType.DV or TokenType.DI or TokenType.SV or TokenType.SS or TokenType.SM or
                TokenType.LSQ or
                // I/O operators that are binary
                TokenType.IO_VERB_1 or TokenType.IO_VERB_2 or TokenType.IO_VERB_3 or
                // Special binary operators
                TokenType.JOIN or TokenType.COLON =>
                    true,
                _ => false
            };
        }

        /// <summary>
        /// Check if a token represents an assignment operation
        /// </summary>
        public static bool IsAssignmentOperator(TokenType tokenType)
        {
            return tokenType == TokenType.ASSIGNMENT ||
                   tokenType == TokenType.GLOBAL_ASSIGNMENT ||
                   tokenType == TokenType.COLON;
        }

        /// <summary>
        /// Parse assignment with left-hand side already parsed
        /// </summary>
        public static ASTNode? ParseAssignmentWithLHS(ASTNode? leftSide, ParseContext context)
        {
            if (leftSide == null || leftSide.Type != ASTNodeType.Variable)
            {
                return leftSide;
            }
            
            var variableName = leftSide.Value is SymbolValue symbol ? symbol.Value : leftSide.Value?.ToString() ?? "";
            
            if (context.Check(TokenType.ASSIGNMENT))
            {
                context.Advance(); // Consume ':'
                var assignmentParser = new AssignmentParser();
                var value = assignmentParser.ParseAssignmentValue(context);
                
                if (value != null)
                {
                    return ASTNode.MakeAssignment(variableName, value);
                }
            }
            else if (context.Check(TokenType.GLOBAL_ASSIGNMENT))
            {
                context.Advance(); // Consume '::'
                var assignmentParser = new AssignmentParser();
                var value = assignmentParser.ParseAssignmentValue(context);
                
                if (value != null)
                {
                    return ASTNode.MakeGlobalAssignment(variableName, value);
                }
            }
            else if (context.Check(TokenType.COLON))
            {
                context.Advance(); // Consume ':'
                return new AssignmentParser().ParseApplyAndAssignVariable(variableName, context);
            }
            
            return leftSide;
        }
    }
}
