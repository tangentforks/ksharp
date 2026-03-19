using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Long Right Scope (LRS) parser for K expressions
    /// Implements right-first parsing strategy with proper K language rules
    /// Uses verb-agnostic design with modular components
    /// </summary>
    public class LRSParser
    {
        private readonly List<Token> tokens;
        private readonly LRSExpressionParser expressionParser;
        private readonly LRSBinaryParser binaryParser;
        private readonly LRSUnaryParser unaryParser;
        private readonly LRSFunctionParser functionParser;
        
        // Parse tree construction mode flag
        public bool BuildParseTree { get; set; }
        
        public LRSParser(List<Token> tokens, bool buildParseTree = false)
        {
            this.tokens = tokens;
            this.expressionParser = new LRSExpressionParser(tokens);
            this.binaryParser = new LRSBinaryParser(tokens, this);
            this.unaryParser = new LRSUnaryParser(this);
            this.functionParser = new LRSFunctionParser(tokens);
        }
        
        /// <summary>
        /// Parse expression using LRS strategy (right-first parsing)
        /// </summary>
        /// <param name="position">Starting position, updated to end of parsed expression</param>
        /// <returns>AST node representing the parsed expression</returns>
        public ASTNode? ParseExpression(ref int position)
        {
            // Step 1: Read entire expression until separator
            var expressionTokens = expressionParser.ReadExpressionTokens(ref position);
            
            if (expressionTokens.Count == 0)
                return null;
                
            // Step 2: Choose strategy based on mode
            if (BuildParseTree)
                return BuildParseTreeFromRight(expressionTokens);
            else
                return EvaluateFromRight(expressionTokens);
        }
        
        /// <summary>
        /// Build parse tree from rightmost subexpression (for _parse function)
        /// </summary>
        /// <param name="expressionTokens">Tokens to build parse tree from</param>
        /// <returns>AST node representing parse tree structure</returns>
        internal ASTNode? BuildParseTreeFromRight(List<Token> expressionTokens)
        {
            if (expressionTokens.Count == 0)
                return null;
                
            if (expressionTokens.Count == 1)
                return CreateNodeFromToken(expressionTokens[0]);
            
            // Check for special functions first (_parse, _eval)
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (LRSFunctionParser.CouldBeFunction(firstToken.Type))
                {
                    return functionParser.ParseFunctionCall(expressionTokens);
                }
            }
            
            // Check for monadic operations first (K LRS: monadic has lower precedence than binary)
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (LRSUnaryParser.CouldBeMonadicOperator(firstToken.Type))
                {
                    return unaryParser.ParseMonadicOperator(expressionTokens);
                }
            }
            
            // Handle binary operations
            return binaryParser.ParseBinaryOperation(expressionTokens);
        }
        
        /// <summary>
        /// Parse expression tokens from right to left (LRS strategy)
        /// </summary>
        /// <param name="expressionTokens">Tokens to parse</param>
        /// <returns>AST node representing parsed expression</returns>
        internal ASTNode? EvaluateFromRight(List<Token> expressionTokens)
        {
            if (expressionTokens.Count == 0)
                return null;
                
            if (expressionTokens.Count == 1)
                return CreateNodeFromToken(expressionTokens[0]);
            
            // Try binary operation first
            return binaryParser.ParseBinaryOperation(expressionTokens);
        }
        
        /// <summary>
        /// Parse sub-expression for unary parser (handles position parameter)
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="position">Reference to position parameter</param>
        /// <returns>AST node representing parsed expression</returns>
        internal ASTNode? ParseSubExpressionForUnary(List<Token> tokens, ref int position)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) return CreateNodeFromToken(tokens[0]);
            
            // Try binary operation first (unary parsing is handled at main LRS level)
            return binaryParser.ParseBinaryOperation(tokens);
        }
        
        /// <summary>
        /// Create AST node from token
        /// </summary>
        private ASTNode CreateNodeFromToken(Token token)
        {
            return token.Type switch
            {
                TokenType.INTEGER => ASTNode.MakeLiteral(new IntegerValue(int.Parse(token.Lexeme))),
                TokenType.FLOAT => ASTNode.MakeLiteral(new FloatValue(double.Parse(token.Lexeme))),
                TokenType.SYMBOL or TokenType.IDENTIFIER => ASTNode.MakeVariable(token.Lexeme.Trim('`')),
                
                // Delimiters for parse tree representation
                TokenType.LEFT_PAREN => ASTNode.MakeLiteral(new SymbolValue("(")),
                TokenType.RIGHT_PAREN => ASTNode.MakeLiteral(new SymbolValue(")")),
                TokenType.LEFT_BRACKET => ASTNode.MakeLiteral(new SymbolValue("[")),
                TokenType.RIGHT_BRACKET => ASTNode.MakeLiteral(new SymbolValue("]")),
                TokenType.LEFT_BRACE => ASTNode.MakeLiteral(new SymbolValue("{")),
                TokenType.RIGHT_BRACE => ASTNode.MakeLiteral(new SymbolValue("}")),
                TokenType.COLON => ASTNode.MakeLiteral(new SymbolValue(":")),
                TokenType.SEMICOLON => ASTNode.MakeLiteral(new SymbolValue(";")),
                
                // Operators that should be treated as symbols in parse trees
                TokenType.PLUS => ASTNode.MakeLiteral(new SymbolValue("+")),
                TokenType.MINUS => ASTNode.MakeLiteral(new SymbolValue("-")),
                TokenType.MULTIPLY => ASTNode.MakeLiteral(new SymbolValue("*")),
                TokenType.DIVIDE => ASTNode.MakeLiteral(new SymbolValue("%")),
                TokenType.POWER => ASTNode.MakeLiteral(new SymbolValue("^")),
                TokenType.MODULUS => ASTNode.MakeLiteral(new SymbolValue("!")),
                TokenType.JOIN => ASTNode.MakeLiteral(new SymbolValue(",")),
                TokenType.MATCH => ASTNode.MakeLiteral(new SymbolValue("~")),
                TokenType.NEGATE => ASTNode.MakeLiteral(new SymbolValue("~")),
                TokenType.DOLLAR => ASTNode.MakeLiteral(new SymbolValue("$")),
                TokenType.QUESTION => ASTNode.MakeLiteral(new SymbolValue("?")),
                TokenType.HASH => ASTNode.MakeLiteral(new SymbolValue("#")),
                TokenType.UNDERSCORE => ASTNode.MakeLiteral(new SymbolValue("_")),
                TokenType.GLOBAL_ASSIGNMENT => ASTNode.MakeLiteral(new SymbolValue("::")),
                
                _ => throw new Exception($"Unsupported token type: {token.Type}")
            };
        }
    }
}
