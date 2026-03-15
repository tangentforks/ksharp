using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Interface for parser modules
    /// </summary>
    public interface IParserModule
    {
        ASTNode? Parse(ParseContext context);
        bool CanHandle(TokenType currentToken);
    }

    /// <summary>
    /// Context object for parser operations
    /// </summary>
    public class ParseContext
    {
        public List<Token> Tokens { get; set; }
        public int Current { get; set; }
        public string SourceText { get; set; }
        public bool ParsingDotApplyArguments { get; set; }
        public int DelimiterDepth { get; set; }

        public ParseContext(List<Token> tokens, string sourceText)
        {
            Tokens = tokens;
            SourceText = sourceText;
            Current = 0;
            ParsingDotApplyArguments = false;
            DelimiterDepth = 0;
        }

        public Token CurrentToken() => Current < Tokens.Count ? Tokens[Current] : new Token(TokenType.EOF, "", Current);

        public Token PreviousToken() => Current > 0 ? Tokens[Current - 1] : new Token(TokenType.EOF, "", Current);

        public bool IsAtEnd() => Current >= Tokens.Count || (Current < Tokens.Count && Tokens[Current].Type == TokenType.EOF);

        public bool Match(TokenType tokenType)
        {
            if (Check(tokenType))
            {
                Current++;
                return true;
            }
            return false;
        }

        public bool Check(TokenType tokenType)
        {
            return !IsAtEnd() && CurrentToken().Type == tokenType;
        }

        public void Advance()
        {
            if (!IsAtEnd()) Current++;
        }
    }

    /// <summary>
    /// Expression parser module for handling ParseExpression, ParseTerm, and expression-level operations
    /// </summary>
    public class ExpressionParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // Expression parser handles most tokens except delimiters that start primary expressions
            return currentToken != TokenType.LEFT_PAREN && 
                   currentToken != TokenType.LEFT_BRACE && 
                   currentToken != TokenType.LEFT_BRACKET &&
                   currentToken != TokenType.RIGHT_PAREN &&
                   currentToken != TokenType.RIGHT_BRACE &&
                   currentToken != TokenType.RIGHT_BRACKET &&
                   currentToken != TokenType.EOF &&
                   currentToken != TokenType.NEWLINE;
        }

        public ASTNode? Parse(ParseContext context)
        {
            return ParseExpression(context);
        }

        private ASTNode? ParseExpression(ParseContext context)
        {
            var left = ParseTerm(context);

            // Handle case where ParseTerm returns null (e.g., due to NEWLINE at start)
            if (left == null)
            {
                return null;
            }

            // Handle binary operators with Long Right Scope (right-associative, equal precedence)
            while (!context.IsAtEnd() && IsBinaryOperator(context.CurrentToken().Type))
            {
                var opToken = context.CurrentToken();
                context.Advance();
                
                var right = ParseTerm(context);
                if (right == null)
                {
                    throw new Exception($"Expected expression after operator {opToken.Lexeme}");
                }

                // Create binary operation node
                left = ASTNode.MakeBinaryOp(opToken.Type, left, right);
            }

            return left;
        }

        private ASTNode? ParseTerm(ParseContext context, bool parseUntilEnd = false)
        {
            // Only return null for EOF, not for NEWLINE when parsing expressions
            // NEWLINE should be handled at higher levels as statement separators
            if (context.IsAtEnd())
            {
                return null;
            }

            ASTNode? result;
            
            // Check if current token is an adverb - if so, we're in a nesting context
            if (!context.IsAtEnd() && VerbRegistry.IsAdverbToken(context.CurrentToken().Type))
            {
                // Parse the primary expression normally - adverb will be handled in ParseTerm
                result = ParsePrimary(context);
            }
            else
            {
                result = ParsePrimary(context);
            }

            // Handle case where ParsePrimary returned null
            if (result == null)
            {
                return null;
            }

            // Handle adverbs
            if (!context.IsAtEnd() && VerbRegistry.IsAdverbToken(context.CurrentToken().Type))
            {
                var adverbType = context.CurrentToken().Type;
                context.Advance(); // Consume the adverb token
                
                // Parse the right argument for the adverb
                var rightArg = ParseTerm(context, parseUntilEnd);
                
                // Create adverb node: ADVERB(adverbType, verb, rightArg)
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString());
                adverbNode.Children.Add(result!); // verb
                adverbNode.Children.Add(rightArg!); // right argument
                
                result = adverbNode;
            }

            return result;
        }

        private ASTNode? ParsePrimary(ParseContext context)
        {
            // Delegate to PrimaryParser for primary expression parsing
            var primaryParser = new PrimaryParser();
            if (primaryParser.CanHandle(context.CurrentToken().Type))
            {
                return primaryParser.Parse(context);
            }
            
            throw new Exception($"Unexpected token: {context.CurrentToken().Type}({context.CurrentToken().Lexeme})");
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

        }
}
