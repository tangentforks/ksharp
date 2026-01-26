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
            
            // Convert token type to traditional operator symbol
            string symbolValue = op switch
            {
                TokenType.PLUS => "+",
                TokenType.MINUS => "-",
                TokenType.MULTIPLY => "*",
                TokenType.DIVIDE => "%",
                TokenType.MIN => "&",
                TokenType.MAX => "|",
                TokenType.LESS => "<",
                TokenType.GREATER => ">",
                TokenType.EQUAL => "=",
                TokenType.POWER => "^",
                TokenType.MODULUS => "!",
                TokenType.JOIN => ",",
                TokenType.HASH => "#",
                TokenType.UNDERSCORE => "_",
                TokenType.QUESTION => "?",
                TokenType.NEGATE => "~",
                TokenType.APPLY => "@",
                TokenType.DOT_APPLY => ".",
                TokenType.TYPE => "TYPE",
                TokenType.STRING_REPRESENTATION => "STRING_REPRESENTATION",
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

        public bool IsExpressionComplete()
        {
            // Skip any trailing whitespace and newlines
            while (!IsAtEnd() && CurrentToken().Type == TokenType.NEWLINE)
            {
                Advance();
            }
            
            // Expression is complete if we're at EOF or have a statement separator
            return IsAtEnd() || CurrentToken().Type == TokenType.SEMICOLON || 
                   CurrentToken().Type == TokenType.NEWLINE;
        }

        public bool IsIncompleteExpression()
        {
            // Check for unmatched brackets, parentheses, or braces
            int parentheses = 0;
            int brackets = 0;
            int braces = 0;
            bool inString = false;
            bool inSymbol = false;
            
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                
                if (token.Type == TokenType.QUOTE && !inString)
                {
                    inString = true;
                }
                else if (token.Type == TokenType.QUOTE && inString)
                {
                    inString = false;
                }
                else if (token.Type == TokenType.BACKTICK && !inSymbol)
                {
                    inSymbol = true;
                }
                else if ((token.Type == TokenType.SYMBOL || token.Type == TokenType.BACKTICK) && inSymbol)
                {
                    inSymbol = false;
                }
                else if (!inString && !inSymbol)
                {
                    switch (token.Type)
                    {
                        case TokenType.LEFT_PAREN:
                            parentheses++;
                            break;
                        case TokenType.RIGHT_PAREN:
                            parentheses--;
                            break;
                        case TokenType.LEFT_BRACKET:
                            brackets++;
                            break;
                        case TokenType.RIGHT_BRACKET:
                            brackets--;
                            break;
                        case TokenType.LEFT_BRACE:
                            braces++;
                            break;
                        case TokenType.RIGHT_BRACE:
                            braces--;
                            break;
                    }
                }
            }
            
            // Expression is incomplete if any brackets are unmatched
            return parentheses != 0 || brackets != 0 || braces != 0 || inString || inSymbol;
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

        private bool IsBinaryOperator(TokenType type)
        {
            return type == TokenType.PLUS || type == TokenType.MINUS || type == TokenType.MULTIPLY ||
                   type == TokenType.DIVIDE || type == TokenType.MIN || type == TokenType.MAX || 
                   type == TokenType.LESS || type == TokenType.GREATER || type == TokenType.EQUAL || 
                   type == TokenType.POWER || type == TokenType.MODULUS || type == TokenType.JOIN ||
                   type == TokenType.HASH || type == TokenType.UNDERSCORE || type == TokenType.QUESTION || 
                   type == TokenType.NEGATE;
        }

        private ASTNode ParseTerm(bool parseUntilEnd = false)
        {
            // Check for space-separated vector
            var elements = new List<ASTNode>();
            elements.Add(ParsePrimary());

            // Track the type of the first element to enforce uniform type requirement
            var firstElementType = elements[0].Type;
            var firstValueType = elements[0].Value?.GetType();

            while (!IsAtEnd() &&
                   (parseUntilEnd ? (
                       CurrentToken().Type != TokenType.RIGHT_PAREN &&
                       CurrentToken().Type != TokenType.RIGHT_BRACE &&
                       CurrentToken().Type != TokenType.SEMICOLON &&
                       CurrentToken().Type != TokenType.NEWLINE &&
                       CurrentToken().Type != TokenType.EOF
                   ) : (
                   CurrentToken().Type != TokenType.PLUS &&
                   CurrentToken().Type != TokenType.MINUS &&
                   CurrentToken().Type != TokenType.MULTIPLY &&
                   CurrentToken().Type != TokenType.DIVIDE &&
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
                   CurrentToken().Type != TokenType.GLOBAL_ASSIGNMENT &&
                   CurrentToken().Type != TokenType.LEFT_BRACKET &&
                   CurrentToken().Type != TokenType.APPLY &&
                   CurrentToken().Type != TokenType.DOT_APPLY &&
                   CurrentToken().Type != TokenType.TYPE &&
                   CurrentToken().Type != TokenType.STRING_REPRESENTATION &&
                   CurrentToken().Type != TokenType.ADVERB_SLASH &&
                   CurrentToken().Type != TokenType.ADVERB_BACKSLASH &&
                   CurrentToken().Type != TokenType.ADVERB_TICK &&
                   CurrentToken().Type != TokenType.EOF)))
            {
                // Check if this would create a mixed-type vector
                // If the current token is an operator, it would create a mixed-type vector
                if (IsBinaryOperator(CurrentToken().Type))
                {
                    // This would create a mixed-type vector, so stop parsing and let ParseExpression handle it
                    break;
                }
                
                var nextElement = ParsePrimary();
                
                // Special case: compact symbol vectors (symbols back-to-back without spaces)
                // Check if the first element was a symbol and the current element is also a symbol
                if (firstElementType == ASTNodeType.Literal && 
                    firstValueType == typeof(SymbolValue) &&
                    nextElement.Type == ASTNodeType.Literal && 
                    nextElement.Value is SymbolValue)
                {
                    // Both are symbols, allow them to be combined regardless of uniform type check
                    elements.Add(nextElement);
                    continue;
                }
                
                // Check for type uniformity
                if (nextElement.Type != firstElementType || 
                    (nextElement.Value?.GetType() != firstValueType && firstValueType != null))
                {
                    // Mixed types detected - this should be an arithmetic expression, not a vector
                    // Put the element back and let ParseExpression handle it
                    // For now, just break the vector parsing
                    break;
                }
                else
                {
                    elements.Add(nextElement);
                }
            }

            if (elements.Count > 1)
            {
                // Check if this might be a function call (variable followed by arguments)
                if (elements[0].Type == ASTNodeType.Variable)
                {
                    // Treat as function call: variable is the function, rest are arguments
                    var functionNode = elements[0];
                    var arguments = elements.Skip(1).ToList();
                    return ASTNode.MakeFunctionCall(functionNode, arguments);
                }
                
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
            else if (Match(TokenType.CHARACTER_VECTOR))
            {
                var value = PreviousToken().Lexeme;
                var elements = new List<K3Value>();
                foreach (char c in value)
                {
                    elements.Add(new CharacterValue(c.ToString()));
                }
                result = ASTNode.MakeLiteral(new VectorValue(elements));
            }
            else if (Match(TokenType.SYMBOL))
            {
                result = ASTNode.MakeLiteral(new SymbolValue(PreviousToken().Lexeme));
            }
            else if (Match(TokenType.PLUS))
            {
                // Check if this is unary transpose (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("+"));
                    }
                    else
                    {
                        // This is unary transpose
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("+");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary plus symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("+"));
                }
            }
            else if (Match(TokenType.MINUS))
            {
                // Check if this is a negative long literal (for long.MinValue)
                if (!IsAtEnd() && CurrentToken().Type == TokenType.LONG)
                {
                    var longToken = CurrentToken();
                    Advance(); // Consume the LONG token
                    var lexeme = longToken.Lexeme;
                    // Check if it's a special negative long value
                    if (lexeme == "0IL" || lexeme == "0NL" || lexeme == "-0IL")
                        result = ASTNode.MakeLiteral(new LongValue(lexeme));
                    else
                    {
                        // Parse the positive part and negate it
                        var positivePart = lexeme.Substring(0, lexeme.Length - 1);
                        long positiveValue;
                        
                        // Handle the special case where the positive part is long.MaxValue + 1
                        if (positivePart == "9223372036854775808")
                        {
                            // This is long.MaxValue + 1, which will overflow if parsed directly
                            // We know this should become long.MinValue when negated
                            result = ASTNode.MakeLiteral(new LongValue(long.MinValue));
                        }
                        else
                        {
                            positiveValue = long.Parse(positivePart);
                            result = ASTNode.MakeLiteral(new LongValue(-positiveValue));
                        }
                    }
                }
                else
                {
                    // Check if this is unary minus (at start of expression)
                    if (result == null)
                    {
                        // Look ahead to see if this is part of an adverb operation
                        if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                           CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                           CurrentToken().Type == TokenType.ADVERB_TICK))
                        {
                            // This is a verb symbol for an adverb operation
                            result = ASTNode.MakeLiteral(new SymbolValue("-"));
                        }
                        else
                        {
                            // This is unary minus
                            var operand = ParsePrimary();
                            var node = new ASTNode(ASTNodeType.BinaryOp);
                            node.Value = new SymbolValue("-");
                            node.Children.Add(operand);
                            return node;
                        }
                    }
                    else
                    {
                        // Binary minus symbol
                        result = ASTNode.MakeLiteral(new SymbolValue("-"));
                    }
                }
            }
            else if (Match(TokenType.DIVIDE))
            {
                // Check if this is unary reciprocal (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("%"));
                    }
                    else
                    {
                        // This is unary reciprocal
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("%");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary division symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("%"));
                }
            }
            else if (Match(TokenType.MULTIPLY))
            {
                // Check if this is unary first (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("*"));
                    }
                    else
                    {
                        // This is unary first
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("*");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary multiplication symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("*"));
                }
            }
            else if (Match(TokenType.MIN))
            {
                // Check if this is unary min (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("&"));
                    }
                    else
                    {
                        // This is unary where (&)
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("&");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary min symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("&"));
                }
            }
            else if (Match(TokenType.MAX))
            {
                // Check if this is unary max (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("|"));
                    }
                    else
                    {
                        // This is unary reverse (|)
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("|");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary max symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("|"));
                }
            }
            else if (Match(TokenType.LESS))
            {
                // Check if this is unary grade up (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("<"));
                    }
                    else
                    {
                        // This is unary grade up
                        var operand = ParseTerm(parseUntilEnd: true);
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("<");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary less symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("<"));
                }
            }
            else if (Match(TokenType.GREATER))
            {
                // Check if this is unary grade down (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue(">"));
                    }
                    else
                    {
                        // This is unary grade down
                        var operand = ParseTerm(parseUntilEnd: true);
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue(">");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary greater symbol
                    result = ASTNode.MakeLiteral(new SymbolValue(">"));
                }
            }
            else if (Match(TokenType.POWER))
            {
                // Check if this is unary shape (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("^"));
                    }
                    else
                    {
                        // This is unary shape
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("^");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary power symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("^"));
                }
            }
            else if (Match(TokenType.JOIN))
            {
                // Check if this is unary enlist (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue(","));
                    }
                    else
                    {
                        // This is unary enlist
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue(",");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary join symbol
                    result = ASTNode.MakeLiteral(new SymbolValue(","));
                }
            }
            else if (Match(TokenType.IDENTIFIER))
            {
                var identifier = PreviousToken().Lexeme;
                result = ASTNode.MakeVariable(identifier);
            }
            else if (Match(TokenType.LEFT_PAREN))
            {
                // Check if this is a semicolon-separated vector
                if (!Match(TokenType.RIGHT_PAREN))
                {
                    // Look ahead to see if we have semicolons
                    var hasSemicolons = false;
                    var tokenCount = 0;
                    var currentPos = current;
                    var parenLevel = 1; // Start with 1 because we're already inside one (
                    
                    while (!IsAtEnd() && parenLevel > 0)
                    {
                        if (CurrentToken().Type == TokenType.LEFT_PAREN)
                        {
                            parenLevel++;
                        }
                        else if (CurrentToken().Type == TokenType.RIGHT_PAREN)
                        {
                            parenLevel--;
                        }
                        else if (CurrentToken().Type == TokenType.SEMICOLON && parenLevel == 1)
                        {
                            hasSemicolons = true;
                            break;
                        }
                        Advance();
                        tokenCount++;
                        if (tokenCount > 20) break; // Safety check
                    }
                    
                    // Reset position
                    current = currentPos;
                    
                    if (hasSemicolons)
                    {
                        // Parse semicolon-separated vector
                        var elements = new List<ASTNode>();
                        
                        // Check if vector starts with empty position
                        if (CurrentToken().Type == TokenType.SEMICOLON)
                        {
                            elements.Add(ASTNode.MakeLiteral(new NullValue()));
                            Match(TokenType.SEMICOLON);
                            
                            // Handle multiple consecutive semicolons at start
                            while (CurrentToken().Type == TokenType.SEMICOLON)
                            {
                                elements.Add(ASTNode.MakeLiteral(new NullValue()));
                                Match(TokenType.SEMICOLON);
                            }
                        }
                        
                        // Parse first element if not at end
                        if (CurrentToken().Type != TokenType.RIGHT_PAREN)
                        {
                            elements.Add(ParseExpression());
                        }
                        
                        // Parse semicolon-separated elements
                        while (Match(TokenType.SEMICOLON) && !IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_PAREN)
                        {
                            elements.Add(ParseExpression());
                        }
                        
                        if (!Match(TokenType.RIGHT_PAREN))
                        {
                            throw new Exception("Expected ')' after expression");
                        }
                        
                        // If the expression is a vector, keep it as a vector
                        // Otherwise, return the expression as-is
                        result = elements.Count == 1 ? elements[0] : ASTNode.MakeVector(elements);
                    }
                    else
                    {
                        // Parse space-separated vector
                        Console.WriteLine($"DEBUG: Parsing space-separated vector in parentheses, current token: {CurrentToken().Type} = {CurrentToken().Lexeme}");
                        var expression = ParseExpression();
                        Console.WriteLine($"DEBUG: Parsed expression result: {expression.Type} = {expression.Value}");
                        
                        if (!Match(TokenType.RIGHT_PAREN))
                        {
                            throw new Exception("Expected ')' after expression");
                        }
                        
                        // If the expression is a vector, keep it as a vector
                        // Otherwise, return the expression as-is
                        result = expression;
                    }
                }
                else
                {
                    // Empty parentheses - create empty vector
                    result = ASTNode.MakeVector(new List<ASTNode>());
                }
            }
            else if (Match(TokenType.NEGATE))
            {
                // Check if this is unary negate (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("~"));
                    }
                    else
                    {
                        // This is unary negate
                        var operand = ParseExpression();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("NEGATE");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary negate symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("~"));
                }
            }
            else if (Match(TokenType.HASH))
            {
                // Check if this is unary count (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("#"));
                    }
                    else
                    {
                        // This is unary count
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("#");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary hash symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("#"));
                }
            }
            else if (Match(TokenType.UNDERSCORE))
            {
                // Check if this is unary floor (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("_"));
                    }
                    else
                    {
                        // This is unary floor
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("_");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary underscore will be handled in ParseExpression, so treat as symbol here
                    result = ASTNode.MakeLiteral(new SymbolValue("_"));
                }
            }
            else if (Match(TokenType.QUESTION))
            {
                // Check if this is unary unique (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                       CurrentToken().Type == TokenType.ADVERB_TICK))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("?"));
                    }
                    else
                    {
                        // This is unary unique
                        var operand = ParsePrimary();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("?");
                        node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary question symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("?"));
                }
            }
            else if (Match(TokenType.ADVERB_SLASH))
            {
                // Standalone adverb slash - this should not happen in valid K3
                // According to spec, slash must be preceded by a verb without spaces
                throw new Exception("Adverb slash must follow a verb without spaces");
            }
            else if (Match(TokenType.LOG))
            {
                // Mathematical logarithm operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_log");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.EXP))
            {
                // Mathematical exponential operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_exp");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ABS))
            {
                // Mathematical absolute value operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_abs");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SQR))
            {
                // Mathematical square operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sqr");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SQRT))
            {
                // Mathematical square root operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sqrt");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.FLOOR_MATH))
            {
                // Mathematical floor operation (always returns float)
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_floor");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DOT))
            {
                // Linear algebra dot product operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_dot");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MUL))
            {
                // Linear algebra matrix multiplication operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_mul");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.INV))
            {
                // Linear algebra matrix inverse operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_inv");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SIN))
            {
                // Mathematical sine operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sin");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.COS))
            {
                // Mathematical cosine operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_cos");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TAN))
            {
                // Mathematical tangent operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_tan");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ASIN))
            {
                // Mathematical arcsine operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_asin");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ACOS))
            {
                // Mathematical arccosine operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_acos");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ATAN))
            {
                // Mathematical arctangent operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_atan");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SINH))
            {
                // Mathematical hyperbolic sine operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sinh");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.COSH))
            {
                // Mathematical hyperbolic cosine operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_cosh");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TANH))
            {
                // Mathematical hyperbolic tangent operation
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_tanh");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LEFT_PAREN))
            {
                // Parentheses are for grouping - parse as complete expression
                // Space-separated lists are inherently vectors in K3, parentheses don't change that
                if (!Match(TokenType.RIGHT_PAREN))
                {
                    var expression = ParseExpression();
                    
                    if (!Match(TokenType.RIGHT_PAREN))
                    {
                        throw new Exception("Expected ')' after expression");
                    }
                    
                    result = expression;
                }
                else
                {
                    // Empty parentheses - create empty vector
                    result = ASTNode.MakeVector(new List<ASTNode>());
                }
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
                    // Parse the function body - handle multiple statements separated by semicolons or newlines
                    var statements = new List<ASTNode>();
                    
                    // Parse all statements, skipping empty ones
                    while (!IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_BRACE)
                    {
                        // Skip empty lines (multiple newlines)
                        while (Match(TokenType.NEWLINE))
                        {
                            // Skip consecutive newlines
                        }
                        
                        // Check if we have content before the next separator or closing brace
                        if (!IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_BRACE && CurrentToken().Type != TokenType.SEMICOLON)
                        {
                            statements.Add(ParseExpression());
                        }
                        
                        // Skip the separator (semicolon or newline) if present
                        Match(TokenType.SEMICOLON);
                        while (Match(TokenType.NEWLINE))
                        {
                            // Skip consecutive newlines after separator
                        }
                    }
                    
                    // If we have multiple statements, create a block
                    if (statements.Count > 1)
                    {
                        body = new ASTNode(ASTNodeType.Block);
                        body.Children.AddRange(statements);
                    }
                    else if (statements.Count == 1)
                    {
                        body = statements[0];
                    }
                    else
                    {
                        // Empty function body - create a block with no statements
                        body = new ASTNode(ASTNodeType.Block);
                    }
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
                    // Skip the closing brace - don't include it in the body
                    if (tokens[tokenIndex].Type != TokenType.RIGHT_BRACE)
                    {
                        bodyTokens.Insert(0, tokens[tokenIndex]); // Insert at beginning to maintain order
                    }
                    tokenIndex--;
                }
                
                // Reconstruct the function body text from tokens
                if (bodyTokens.Count > 0)
                {
                    // Reconstruct body text preserving original spacing as much as possible
                    var bodyParts = new List<string>();
                    for (int i = 0; i < bodyTokens.Count; i++)
                    {
                        var token = bodyTokens[i];
                        var lexeme = token.Lexeme;
                        
                        // Handle operators that should be attached to operands (no spaces)
                        if ((lexeme == "+" || lexeme == "-" || lexeme == "*" || lexeme == "/" || 
                             lexeme == "^" || lexeme == "!" || lexeme == "=" || lexeme == "<" || 
                             lexeme == ">" || lexeme == "&" || lexeme == "|" || lexeme == ","))
                        {
                            // Check if this operator should be attached to previous token (no space before)
                            if (i > 0)
                            {
                                var prevToken = bodyTokens[i - 1];
                                if (prevToken.Type == TokenType.IDENTIFIER || prevToken.Type == TokenType.INTEGER ||
                                    prevToken.Type == TokenType.FLOAT || prevToken.Type == TokenType.RIGHT_PAREN ||
                                    prevToken.Type == TokenType.RIGHT_BRACKET)
                                {
                                    // Attach operator to previous token (no space before)
                                    bodyParts[bodyParts.Count - 1] += lexeme;
                                    
                                    // Check if next token should also be attached (no space after)
                                    if (i + 1 < bodyTokens.Count)
                                    {
                                        var nextToken = bodyTokens[i + 1];
                                        if (nextToken.Type == TokenType.IDENTIFIER || nextToken.Type == TokenType.INTEGER ||
                                            nextToken.Type == TokenType.FLOAT || nextToken.Type == TokenType.LEFT_PAREN ||
                                            nextToken.Type == TokenType.LEFT_BRACKET)
                                        {
                                            // Attach next token to operator as well (no space after)
                                            if (i + 1 < bodyTokens.Count)
                                            {
                                                bodyParts[bodyParts.Count - 1] += nextToken.Lexeme;
                                                i++; // Skip the next token since we already processed it
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                        
                        bodyParts.Add(lexeme);
                    }
                    
                    bodyText = string.Join(" ", bodyParts);
                    
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
            else if (Match(TokenType.TYPE))
            {
                // 4: operator is unary - parse the operand
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("TYPE");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.STRING_REPRESENTATION))
            {
                // 5: operator is unary - parse the operand
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("STRING_REPRESENTATION");
                node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DOT_APPLY))
            {
                // This is unary MAKE (.) operator
                var operand = ParsePrimary();
                var makeNode = new ASTNode(ASTNodeType.BinaryOp);
                makeNode.Value = new SymbolValue(".");
                makeNode.Children.Add(operand);
                return makeNode;
            }
            else if (Match(TokenType.APPLY))
            {
                // This is unary ATOM (@) operator
                var operand = ParseTerm();
                var atomNode = new ASTNode(ASTNodeType.BinaryOp);
                atomNode.Value = new SymbolValue("@");
                atomNode.Children.Add(operand);
                return atomNode;
            }
            else if (Match(TokenType.EOF))
            {
                // End of input - return null result
                return null;
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

        private bool IsScalar(ASTNode node)
        {
            return node.Type == ASTNodeType.Literal && 
                   (node.Value is IntegerValue || node.Value is LongValue || 
                    node.Value is FloatValue || node.Value is CharacterValue || 
                    node.Value is SymbolValue || node.Value is NullValue);
        }

        private bool IsBinaryOperatorContext()
        {
            // Check if the current token suggests we're in a binary operator context
            // This is a simplified check - in a full implementation we'd need more sophisticated parsing
            return CurrentToken().Type == TokenType.IDENTIFIER ||
                   CurrentToken().Type == TokenType.INTEGER ||
                   CurrentToken().Type == TokenType.LONG ||
                   CurrentToken().Type == TokenType.FLOAT ||
                   CurrentToken().Type == TokenType.CHARACTER ||
                   CurrentToken().Type == TokenType.SYMBOL ||
                   CurrentToken().Type == TokenType.LEFT_PAREN ||
                   CurrentToken().Type == TokenType.LEFT_BRACE ||
                   CurrentToken().Type == TokenType.LEFT_BRACKET;
        }

        private bool IsUnaryOperatorContext()
        {
            // Check if we're at the start of an expression or after a binary operator
            // This is a simplified check - in a full implementation we'd need more sophisticated parsing
            return CurrentToken().Type == TokenType.EOF ||
                   CurrentToken().Type == TokenType.SEMICOLON ||
                   CurrentToken().Type == TokenType.NEWLINE ||
                   CurrentToken().Type == TokenType.RIGHT_PAREN ||
                   CurrentToken().Type == TokenType.RIGHT_BRACE ||
                   CurrentToken().Type == TokenType.RIGHT_BRACKET ||
                   CurrentToken().Type == TokenType.ASSIGNMENT;
        }

        private void Backtrack()
        {
            if (current > 0)
                current--;
        }

        private ASTNode ParseExpression()
        {
            // Handle unary enumerate at the very start (before any left operand parsing)
            if (Match(TokenType.MODULUS))
            {
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("!");
                node.Children.Add(operand);
                return node;
            }

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
                    var applyToken = new Token(op, op == TokenType.APPLY ? "@" : ".", 0);
                    return ASTNode.MakeBinaryOp(op, left, arguments[0]);
                }
            }

            while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.DIVIDE) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) ||
                   Match(TokenType.EQUAL) || Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.HASH) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION))
            {
                var op = PreviousToken().Type;
                
                // Check if this is followed by an adverb
                if (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK))
                {
                    var adverbType = PreviousToken().Type.ToString().Replace("TokenType.", "");
                    
                    // Check if this is a vector-vector each operation
                    if (left.Type == ASTNodeType.Vector && adverbType == "ADVERB_TICK")
                    {
                        // This is a vector-vector each operation, which should throw a length error
                        // Parse the right side of the adverb
                        var rightSide = ParseExpression();
                        
                        // Create adverb node with vector verb
                        var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        adverbNode.Value = new SymbolValue(adverbType);
                        adverbNode.Children.Add(left);
                        adverbNode.Children.Add(rightSide);
                        
                        left = adverbNode;
                        continue;
                    }
                    
                // Convert the binary operator to a verb symbol
                    var verbName = op.ToString() switch
                    {
                        "PLUS" => "+",
                        "MINUS" => "-",
                        "MULTIPLY" => "*",
                        "DIVIDE" => "%",
                        "MIN" => "&",
                        "MAX" => "|",
                        "POWER" => "^",
                        "MODULUS" => "!",
                        "JOIN" => ",",
                        "HASH" => "#",
                        "TYPE" => "TYPE",
                        _ => op.ToString()
                    };
                    var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                    
                    // For mixed scan operations (scalar verb\ vector), create a vector containing the scalar and verb
                    // that the Scan method can recognize as a mixed scan
                    if (IsScalar(left) && adverbType == "ADVERB_BACKSLASH")
                    {
                        var mixedScanVector = new List<ASTNode>();
                        mixedScanVector.Add(left);
                        mixedScanVector.Add(verbNode);
                        var mixedScanNode = ASTNode.MakeVector(mixedScanVector);
                        
                        // Parse the right side of the adverb
                        var right = ParseExpression();
                        
                        // Create adverb node with the mixed scan vector
                        var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        adverbNode.Value = new SymbolValue(adverbType);
                        adverbNode.Children.Add(mixedScanNode);
                        adverbNode.Children.Add(right);
                        
                        left = adverbNode;
                    }
                    else
                    {
                        // Regular adverb operation (including mixed over operations)
                        // Parse the right side of the adverb
                        var right = ParseExpression();
                        
                        // Create adverb node with both scalar and verb information
                        var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        adverbNode.Value = new SymbolValue(adverbType);
                        
                        // Create a vector containing the scalar and verb symbol for mixed adverbs
                        var mixedAdverbVector = new List<ASTNode>();
                        mixedAdverbVector.Add(left);  // Add the scalar value
                        mixedAdverbVector.Add(verbNode);  // Add the verb symbol
                        var mixedAdverbNode = ASTNode.MakeVector(mixedAdverbVector);
                        
                        adverbNode.Children.Add(mixedAdverbNode);
                        adverbNode.Children.Add(right);
                        
                        left = adverbNode;
                    }
                }
                else
                {
                    // Long Right Scope: parse the right operand as a full expression for LRS
                    var rightExpr = ParseExpression();
                    left = ASTNode.MakeBinaryOp(op, left, rightExpr);
                }
            }

            // Handle standalone adverbs (like +' 1 2 3)
            if (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK))
            {
                var adverbToken = PreviousToken();
                var right = ParseTerm();
                return ASTNode.MakeBinaryOp(adverbToken.Type, left, right);
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
