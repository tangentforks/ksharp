using System;
using System.Collections.Generic;
using System.Linq;
using K3CSharp;
using K3CSharp.Parsing;

namespace K3CSharp.Verbs
{
    /// <summary>
    /// Implementation of _parse verb for K3CSharp
    /// Parses character vectors into AST nodes and converts to K list representations
    /// </summary>
    public static class ParseVerbHandler
    {
        /// <summary>
        /// Main _parse entry point
        /// </summary>
        public static K3Value Parse(string expressionText)
        {
            return ParseExpression(expressionText);
        }
        
        /// <summary>
        /// Parse verb implementation matching delegate signature
        /// </summary>
        public static K3Value Parse(K3Value[] arguments)
        {
            if (arguments.Length == 0)
                throw new Exception("_parse: requires an argument");
                
            var expressionText = arguments[0].ToString();
            return Parse(expressionText);
        }
        
        /// <summary>
        /// Tokenize character vector expression
        /// </summary>
        private static K3Value ParseExpression(string expressionText)
        {
            try
            {
                // Tokenize the expression
                var tokens = TokenizeExpression(expressionText);
                
                // Parse tokens to AST using existing parser
                var astNode = ParseTokensToAST(tokens);
                
                // Convert AST to K list representation
                return ParseTreeConverter.ToKList(astNode);
            }
            catch (Exception ex)
            {
                throw new Exception($"Parse error at {expressionText.Length}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Tokenize character vector expression
        /// </summary>
        private static List<Token> TokenizeExpression(string expression)
        {
            // For now, use a simple tokenization approach
            // In a full implementation, this would use the existing lexer
            var tokens = new List<Token>();
            var position = 0;
            
            while (position < expression.Length)
            {
                var c = expression[position];
                
                if (char.IsWhiteSpace(c))
                {
                    position++;
                    continue;
                }
                
                if (c == '"')
                {
                    var endPos = expression.IndexOf('"', position + 1);
                    if (endPos == -1)
                        throw new Exception("Unterminated string");
                    
                    var stringValue = expression.Substring(position + 1, endPos - position - 1);
                    tokens.Add(new Token(TokenType.CHARACTER_VECTOR, stringValue, position));
                    position = endPos + 1;
                }
                else if (char.IsDigit(c) || (c == '-' && position + 1 < expression.Length && char.IsDigit(expression[position + 1])))
                {
                    var numberStart = position;
                    while (position < expression.Length && (char.IsDigit(expression[position]) || (expression[position] == '.' && position + 1 < expression.Length && char.IsDigit(expression[position + 1]))))
                    {
                        position++;
                    }
                    
                    var numberString = expression.Substring(numberStart, position - numberStart);
                    if (numberString.Contains('.'))
                        tokens.Add(new Token(TokenType.FLOAT, numberString, numberStart));
                    else
                        tokens.Add(new Token(TokenType.INTEGER, numberString, numberStart));
                }
                else if (char.IsLetter(c) || c == '_')
                {
                    var start = position;
                    while (position < expression.Length && (char.IsLetterOrDigit(expression[position]) || expression[position] == '_'))
                    {
                        position++;
                    }
                    
                    var identifier = expression.Substring(start, position - start);
                    tokens.Add(new Token(TokenType.IDENTIFIER, identifier, start));
                }
                else if (c == '+' || c == '-' || c == '*' || c == '/' || c == '%' || c == '^')
                {
                    tokens.Add(new Token(TokenType.PLUS, c.ToString(), position));
                    position++;
                }
                else if (c == '(' || c == ')' || c == '[' || c == ']' || c == '{' || c == '}')
                {
                    var tokenType = c switch
                    {
                        '(' => TokenType.LEFT_PAREN,
                        ')' => TokenType.RIGHT_PAREN,
                        '[' => TokenType.LEFT_BRACKET,
                        ']' => TokenType.RIGHT_BRACKET,
                        '{' => TokenType.LEFT_BRACE,
                        '}' => TokenType.RIGHT_BRACE,
                        _ => TokenType.UNKNOWN
                    };
                    tokens.Add(new Token(tokenType, c.ToString(), position));
                    position++;
                }
                else
                {
                    tokens.Add(new Token(TokenType.UNKNOWN, c.ToString(), position));
                    position++;
                }
            }
            
            return tokens;
        }
        
        /// <summary>
        /// Parse tokens to AST using existing parser infrastructure
        /// </summary>
        private static ASTNode ParseTokensToAST(List<Token> tokens)
        {
            // This is a simplified implementation
            // In a full implementation, this would use the existing Parser class
            if (tokens.Count == 0)
                throw new Exception("Empty expression");
                
            if (tokens.Count == 1)
                return CreateNodeFromToken(tokens[0]);
                
            // For multiple tokens, create a simple binary operation
            // This is a placeholder - the full parser would handle this properly
            var left = CreateNodeFromToken(tokens[0]);
            var right = CreateNodeFromToken(tokens[tokens.Count - 1]);
            var op = tokens.Count > 2 ? tokens[1] : new Token(TokenType.PLUS, "+", 0);
            
            return ASTNode.MakeBinaryOp(op.Type, left, right);
        }
        
        /// <summary>
        /// Create AST node from token
        /// </summary>
        private static ASTNode CreateNodeFromToken(Token token)
        {
            switch (token.Type)
            {
                case TokenType.INTEGER:
                    return ASTNode.MakeLiteral(new IntegerValue(int.Parse(token.Lexeme)));
                case TokenType.FLOAT:
                    return ASTNode.MakeLiteral(new FloatValue(double.Parse(token.Lexeme)));
                case TokenType.IDENTIFIER:
                    return ASTNode.MakeVariable(token.Lexeme);
                case TokenType.CHARACTER_VECTOR:
                    // Remove quotes from the lexeme and create a proper string value
                    var content = token.Lexeme;
                    if (content.Length >= 2 && content[0] == '"' && content[content.Length - 1] == '"')
                    {
                        content = content.Substring(1, content.Length - 2);
                    }
                    return ASTNode.MakeLiteral(new SymbolValue(content));
                default:
                    throw new Exception($"Unsupported token type: {token.Type}");
            }
        }
    }
}
