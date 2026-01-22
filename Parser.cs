using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public enum ASTNodeType
    {
        Literal,
        Vector,
        BinaryOp,
        Assignment,
        GlobalAssignment,
        Variable,
        Function,
        FunctionCall,
        Block
    }

    public class ASTNode
    {
        public ASTNodeType Type { get; }
        public K3Value Value { get; set; }
        public List<ASTNode> Children { get; }
        public List<string> Parameters { get; set; } = new List<string>();
        public int StartPosition { get; set; } = -1;
        public int EndPosition { get; set; } = -1;

        public ASTNode(ASTNodeType type, K3Value value = null, List<ASTNode> children = null)
        {
            Type = type;
            Value = value;
            Children = children ?? new List<ASTNode>();
        }

        public static ASTNode MakeLiteral(K3Value value)
        {
            return new ASTNode(ASTNodeType.Literal, value, null);
        }

        public static ASTNode MakeVector(List<ASTNode> elements)
        {
            return new ASTNode(ASTNodeType.Vector, null, elements);
        }

        public static ASTNode MakeBinaryOp(TokenType op, ASTNode left, ASTNode right)
        {
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Children.Add(left);
            node.Children.Add(right);
            
            // Handle special case for TYPE operator
            string symbolValue = op switch
            {
                TokenType.TYPE => "TYPE",
                _ => op.ToString()
            };
            
            node.Value = new SymbolValue(symbolValue);
            return node;
        }

        public static ASTNode MakeAssignment(string variableName, ASTNode value)
        {
            var node = new ASTNode(ASTNodeType.Assignment);
            node.Value = new SymbolValue(variableName);
            node.Children.Add(value);
            return node;
        }

        public static ASTNode MakeGlobalAssignment(string variableName, ASTNode value)
        {
            var node = new ASTNode(ASTNodeType.GlobalAssignment);
            node.Value = new SymbolValue(variableName);
            node.Children.Add(value);
            return node;
        }

        public static ASTNode MakeVariable(string variableName)
        {
            return new ASTNode(ASTNodeType.Variable, new SymbolValue(variableName));
        }

        public static ASTNode MakeFunction(List<string> parameters, ASTNode body)
        {
            var node = new ASTNode(ASTNodeType.Function);
            node.Parameters = parameters;
            node.Children.Add(body);
            return node;
        }

        public static ASTNode MakeFunctionCall(ASTNode function, List<ASTNode> arguments)
        {
            var node = new ASTNode(ASTNodeType.FunctionCall);
            node.Children.Add(function);
            node.Children.AddRange(arguments);
            return node;
        }
    }

    public class Parser
    {
        private List<Token> tokens;
        private int current;
        private string sourceText;

        public Parser(List<Token> tokens, string sourceText = "")
        {
            this.tokens = tokens;
            this.current = 0;
            this.sourceText = sourceText;
        }

        public ASTNode Parse()
        {
            if (tokens.Count == 0)
            {
                throw new Exception("No tokens to parse");
            }

            var result = ParseExpression();

            // Only look for additional statements if we haven't consumed all tokens
            // and the next token is a semicolon or newline (indicating multiple statements)
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.SEMICOLON || CurrentToken().Type == TokenType.NEWLINE))
            {
                var statements = new List<ASTNode>();
                statements.Add(result);
                
                // Parse additional statements separated by semicolons or newlines
                while (Match(TokenType.SEMICOLON) || Match(TokenType.NEWLINE))
                {
                    // Skip empty lines
                    while (!IsAtEnd() && CurrentToken().Type == TokenType.NEWLINE)
                    {
                        Match(TokenType.NEWLINE);
                    }
                    
                    if (!IsAtEnd())
                    {
                        statements.Add(ParseExpression());
                    }
                }

                // If we have multiple statements, create a block
                if (statements.Count > 1)
                {
                    var block = new ASTNode(ASTNodeType.Block);
                    block.Children.AddRange(statements);
                    return block;
                }
            }

            return result;
        }

        private ASTNode ParseExpression()
        {
            var left = ParseTerm();

            // Check for function call using @ or . operators (higher precedence than binary ops)
            if (Match(TokenType.APPLY) || Match(TokenType.DOT_APPLY))
            {
                var op = PreviousToken().Type;
                var arguments = new List<ASTNode>();
                
                // For @ operator: parse a single expression (vector or scalar)
                if (op == TokenType.APPLY)
                {
                    arguments.Add(ParseExpression());
                }
                // For . operator: parse space-separated arguments
                else if (op == TokenType.DOT_APPLY)
                {
                    arguments.Add(ParseExpression());
                    while (!IsAtEnd() && CurrentToken().Type != TokenType.SEMICOLON && 
                           CurrentToken().Type != TokenType.NEWLINE && CurrentToken().Type != TokenType.RIGHT_PAREN &&
                           CurrentToken().Type != TokenType.RIGHT_BRACE && CurrentToken().Type != TokenType.RIGHT_BRACKET)
                    {
                        arguments.Add(ParseExpression());
                    }
                }
                
                // Check if this is a function call or vector indexing
                // If left is a function or variable that could be a function, treat as function call
                // If left is a vector literal, treat as binary operation for indexing
                if (left.Type == ASTNodeType.Function || 
                    (left.Type == ASTNodeType.Variable && !IsVectorLiteral(left)))
                {
                    left = ASTNode.MakeFunctionCall(left, arguments);
                }
                else
                {
                    // This is vector indexing, treat as binary operation
                    var applyToken = new Token(TokenType.APPLY, "@", 0);
                    return ASTNode.MakeBinaryOp(TokenType.APPLY, left, arguments[0]);
                }
            }

            while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) ||
                   Match(TokenType.EQUAL) || Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK) ||
                   Match(TokenType.TYPE))
            {
                var op = PreviousToken().Type;
                var right = ParseTerm();
                
                left = ASTNode.MakeBinaryOp(op, left, right);
            }

            // Check for assignment
            if (Match(TokenType.ASSIGNMENT))
            {
                if (left.Type != ASTNodeType.Variable)
                {
                    throw new Exception("Assignment target must be a variable");
                }
                var value = ParseExpression();
                var variableName = left.Value is SymbolValue symbol ? symbol.Value : throw new Exception("Variable node must contain variable name");
                return ASTNode.MakeAssignment(variableName, value);
            }

            // Check for global assignment
            if (Match(TokenType.GLOBAL_ASSIGNMENT))
            {
                if (left.Type != ASTNodeType.Variable)
                {
                    throw new Exception("Global assignment target must be a variable");
                }
                var value = ParseExpression();
                var variableName = left.Value is SymbolValue symbol ? symbol.Value : throw new Exception("Variable node must contain variable name");
                return ASTNode.MakeGlobalAssignment(variableName, value);
            }

            return left;
        }

        private ASTNode ParseTerm()
        {
            // Check for space-separated vector
            var elements = new List<ASTNode>();
            elements.Add(ParsePrimary());

            while (!IsAtEnd() &&
                   CurrentToken().Type != TokenType.PLUS &&
                   CurrentToken().Type != TokenType.MINUS &&
                   CurrentToken().Type != TokenType.MULTIPLY &&
                   CurrentToken().Type != TokenType.MIN &&
                   CurrentToken().Type != TokenType.MAX &&
                   CurrentToken().Type != TokenType.LESS &&
                   CurrentToken().Type != TokenType.GREATER &&
                   CurrentToken().Type != TokenType.EQUAL &&
                   CurrentToken().Type != TokenType.POWER &&
                   CurrentToken().Type != TokenType.MODULUS &&
                   CurrentToken().Type != TokenType.JOIN &&
                   CurrentToken().Type != TokenType.HASH &&
                   CurrentToken().Type != TokenType.UNDERSCORE &&
                   CurrentToken().Type != TokenType.QUESTION &&
                   CurrentToken().Type != TokenType.NEGATE &&
                   CurrentToken().Type != TokenType.RIGHT_PAREN &&
                   CurrentToken().Type != TokenType.RIGHT_BRACE &&
                   CurrentToken().Type != TokenType.SEMICOLON &&
                   CurrentToken().Type != TokenType.NEWLINE &&
                   CurrentToken().Type != TokenType.ASSIGNMENT &&
                   CurrentToken().Type != TokenType.LEFT_BRACKET &&
                   CurrentToken().Type != TokenType.APPLY &&
                   CurrentToken().Type != TokenType.DOT_APPLY &&
                   CurrentToken().Type != TokenType.TYPE &&
                   CurrentToken().Type != TokenType.IDENTIFIER &&
                   CurrentToken().Type != TokenType.EOF)
            {
                elements.Add(ParsePrimary());
            }

            if (elements.Count > 1)
            {
                return ASTNode.MakeVector(elements);
            }

            return elements[0];
        }

        private bool IsVectorLiteral(ASTNode node)
        {
            // A vector literal is a node that was parsed as a vector
            // This is a simple heuristic - if it has multiple children and they're all literals/variables
            if (node.Type == ASTNodeType.Vector && node.Children.Count > 0)
            {
                return true;
            }
            
            // Also check if it's a variable that we know represents a vector from context
            // For now, we'll be conservative and only treat explicit vectors as vector literals
            return false;
        }

        private ASTNode ParsePrimary()
        {
            ASTNode result = null;

            if (Match(TokenType.INTEGER))
            {
                var lexeme = PreviousToken().Lexeme;
                // Check if it's a special value
                if (lexeme == "0I" || lexeme == "0N" || lexeme == "-0I")
                    result = ASTNode.MakeLiteral(new IntegerValue(lexeme));
                else
                    result = ASTNode.MakeLiteral(new IntegerValue(int.Parse(lexeme)));
            }
            else if (Match(TokenType.LONG))
            {
                var lexeme = PreviousToken().Lexeme;
                // Check if it's a special long value
                if (lexeme == "0IL" || lexeme == "0NL" || lexeme == "-0IL")
                    result = ASTNode.MakeLiteral(new LongValue(lexeme));
                else
                    result = ASTNode.MakeLiteral(new LongValue(long.Parse(lexeme.Substring(0, lexeme.Length - 1))));
            }
            else if (Match(TokenType.FLOAT))
            {
                var lexeme = PreviousToken().Lexeme;
                // Check if it's a special float value
                if (lexeme == "0i" || lexeme == "0n" || lexeme == "-0i")
                    result = ASTNode.MakeLiteral(new FloatValue(lexeme));
                else
                    result = ASTNode.MakeLiteral(new FloatValue(double.Parse(lexeme)));
            }
            else if (Match(TokenType.NULL))
            {
                result = ASTNode.MakeLiteral(new NullValue());
            }
            else if (Match(TokenType.CHARACTER))
            {
                var value = PreviousToken().Lexeme;
                result = ASTNode.MakeLiteral(new CharacterValue(value));
            }
            else if (Match(TokenType.SYMBOL))
            {
                result = ASTNode.MakeLiteral(new SymbolValue(PreviousToken().Lexeme));
            }
            else if (Match(TokenType.IDENTIFIER))
            {
                var identifier = PreviousToken().Lexeme;
                result = ASTNode.MakeVariable(identifier);
            }
            // Handle unary operators
            else if (Match(TokenType.NEGATE))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("NEGATE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MINUS))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("UNARY_MINUS");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.PLUS))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("TRANSPOSE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MULTIPLY))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("FIRST");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MIN))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("GENERATE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DIVIDE))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("RECIPROCAL");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MAX))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("REVERSE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LESS))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("GRADE_UP");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.GREATER))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("GRADE_DOWN");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.POWER))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("SHAPE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MODULUS))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("ENUMERATE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.JOIN))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("ENLIST");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.HASH))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("COUNT");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.UNDERSCORE))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("FLOOR");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.QUESTION))
            {
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("UNIQUE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LEFT_PAREN))
            {
                var elements = new List<ASTNode>();
                
                if (!Match(TokenType.RIGHT_PAREN))
                {
                    elements.Add(ParsePrimary());
                    
                    while (Match(TokenType.SEMICOLON))
                    {
                        elements.Add(ParsePrimary());
                    }
                    
                    // Check for additional space-separated elements (not semicolon-separated)
                    while (!IsAtEnd() && 
                           CurrentToken().Type != TokenType.RIGHT_PAREN &&
                           CurrentToken().Type != TokenType.RIGHT_BRACE &&
                           CurrentToken().Type != TokenType.SEMICOLON &&
                           CurrentToken().Type != TokenType.NEWLINE &&
                           CurrentToken().Type != TokenType.EOF)
                    {
                        elements.Add(ParsePrimary());
                    }
                    
                    if (!Match(TokenType.RIGHT_PAREN))
                    {
                        throw new Exception("Expected ')' after vector elements");
                    }
                }
                
                result = ASTNode.MakeVector(elements);
            }
            else if (Match(TokenType.LEFT_BRACE))
            {
                // Parse function
                var parameters = new List<string>();
                
                // Check if there's a parameter list
                if (CurrentToken().Type == TokenType.LEFT_BRACKET)
                {
                    // Parse parameter list
                    Match(TokenType.LEFT_BRACKET); // Consume the '['
                    
                    if (!Match(TokenType.RIGHT_BRACKET))
                    {
                        // First parameter
                        if (Match(TokenType.IDENTIFIER))
                        {
                            parameters.Add(PreviousToken().Lexeme);
                        }
                        
                        while (Match(TokenType.SEMICOLON))
                        {
                            if (Match(TokenType.IDENTIFIER))
                            {
                                parameters.Add(PreviousToken().Lexeme);
                            }
                        }
                        
                        if (!Match(TokenType.RIGHT_BRACKET))
                        {
                            throw new Exception("Expected ']' after parameter list");
                        }
                    }
                }
                
                // Parse function body
                int leftBracePos = CurrentToken().Position; // Position of the opening brace
                int bodyStartTokenIndex = current; // Store the token index where body starts
                ASTNode body;
                if (CurrentToken().Type == TokenType.RIGHT_BRACE)
                {
                    // Empty function body - create a block with no statements
                    body = new ASTNode(ASTNodeType.Block);
                }
                else
                {
                    // Parse the function body as a block or single expression
                    body = ParseExpression();
                }
                
                if (!Match(TokenType.RIGHT_BRACE))
                {
                    throw new Exception("Expected '}' after function body");
                }
                
                // Extract the source text of the function body
                int rightBracePos = PreviousToken().Position; // Position of the closing brace
                string bodyText = "";
                List<Token> preParsedTokens = null;
                
                // Reconstruct function body text from tokens between the braces
                var bodyTokens = new List<Token>();
                int tokenIndex = current - 1; // Start from the token before the closing brace
                
                // Work backwards to collect tokens from the function body (excluding braces)
                while (tokenIndex >= bodyStartTokenIndex && tokens[tokenIndex].Type != TokenType.LEFT_BRACE)
                {
                    bodyTokens.Insert(0, tokens[tokenIndex]); // Insert at beginning to maintain order
                    tokenIndex--;
                }
                
                // Reconstruct the function body text from tokens
                if (bodyTokens.Count > 0)
                {
                    bodyText = string.Join(" ", bodyTokens.Select(t => t.Lexeme));
                    
                    // Pre-parse the function body for better performance
                    try
                    {
                        var bodyLexer = new Lexer(bodyText);
                        preParsedTokens = bodyLexer.Tokenize();
                    }
                    catch
                    {
                        // If pre-parsing fails, we'll defer to runtime (per spec)
                        // This is acceptable - validation is deferred until execution
                        preParsedTokens = null;
                    }
                }
                else
                {
                    // Empty function body
                    bodyText = "";
                }
                
                result = ASTNode.MakeFunction(parameters, body);
                result.StartPosition = leftBracePos;
                result.EndPosition = rightBracePos;
                result.Value = new FunctionValue(bodyText, parameters, preParsedTokens);
            }
            else if (Match(TokenType.ADVERB_SLASH))
            {
                return ParseAdverbChain("ADVERB_SLASH");
            }
            else if (Match(TokenType.ADVERB_BACKSLASH))
            {
                return ParseAdverbChain("ADVERB_BACKSLASH");
            }
            else if (Match(TokenType.ADVERB_TICK))
            {
                return ParseAdverbChain("ADVERB_TICK");
            }
            else
            {
                // Debug: Let's see what token we're getting
                var currentToken = CurrentToken();
                throw new Exception($"Unexpected token: {currentToken.Type}({currentToken.Lexeme})");
            }

            return result;
        }

        private ASTNode ParseAdverbChain(string firstAdverb)
        {
            var adverbs = new List<string> { firstAdverb };
            
            // Collect additional adverbs for chaining
            while (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK))
            {
                adverbs.Add(PreviousToken().Type.ToString().Replace("TokenType.", ""));
            }
            
            // Parse the operand (the verb or data the adverbs apply to)
            var operand = ParsePrimary();
            
            // Create a chained adverb node
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("ADVERB_CHAIN");
            node.Children.Add(operand);
            
            // Add adverbs as metadata
            foreach (var adverb in adverbs)
            {
                var adverbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(adverb));
                node.Children.Add(adverbNode);
            }
            
            return node;
        }

        private Token CurrentToken()
        {
            return current < tokens.Count ? tokens[current] : new Token(TokenType.EOF, "", -1);
        }

        private Token PreviousToken()
        {
            return current > 0 ? tokens[current - 1] : new Token(TokenType.EOF, "", -1);
        }

        private bool IsAtEnd()
        {
            return current >= tokens.Count || CurrentToken().Type == TokenType.EOF;
        }

        private bool Match(TokenType type)
        {
            if (CurrentToken().Type == type)
            {
                Advance();
                return true;
            }
            return false;
        }

        private void Advance()
        {
            current++;
        }
    }
}
