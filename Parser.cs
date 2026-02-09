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
        Block,
        FormSpecifier
    }

    public class ASTNode
    {
        public ASTNodeType Type { get; }
        public K3Value? Value { get; set; }
        public List<ASTNode> Children { get; }
        public List<string> Parameters { get; set; } = new List<string>();
        public int StartPosition { get; set; } = -1;
        public int EndPosition { get; set; } = -1;

        public ASTNode(ASTNodeType type, K3Value? value = null, List<ASTNode>? children = null)
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
            if (left != null) node.Children.Add(left);
            if (right != null) node.Children.Add(right);
            
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
                TokenType.IN => "_in",
                TokenType.BIN => "_bin",
                TokenType.BINL => "_binl",
                TokenType.LIN => "_lin",
                TokenType.DV => "_dv",
                TokenType.DI => "_di",
                TokenType.VS => "_vs",
                TokenType.SV => "_sv",
                TokenType.SS => "_ss",
                TokenType.SM => "_sm",
                TokenType.CI => "_ci",
                TokenType.IC => "_ic",
                TokenType.DRAW => "_draw",
                TokenType.GETENV => "_getenv",
                TokenType.SETENV => "_setenv",
                TokenType.SIZE => "_size",
                TokenType.POWER => "^",
                TokenType.MODULUS => "!",
                TokenType.JOIN => ",",
                TokenType.COLON => ":",
                TokenType.HASH => "#",
                TokenType.UNDERSCORE => "_",
                TokenType.QUESTION => "?",
                TokenType.NEGATE => "~",
                TokenType.DOLLAR => "$",
                TokenType.APPLY => "@",
                TokenType.DOT_APPLY => ".",
                TokenType.TYPE => "TYPE",
                TokenType.STRING_REPRESENTATION => "STRING_REPRESENTATION",
                TokenType.DO => "do",
                TokenType.WHILE => "while",
                TokenType.IF_FUNC => "if",
                _ => op.ToString()
            };
            
            node.Value = new SymbolValue(symbolValue);
            return node;
        }

        public static ASTNode MakeAssignment(string variableName, ASTNode value)
        {
            var node = new ASTNode(ASTNodeType.Assignment);
            node.Value = new SymbolValue(variableName);
            if (value != null) node.Children.Add(value);
            return node;
        }

        public static ASTNode MakeGlobalAssignment(string variableName, ASTNode value)
        {
            var node = new ASTNode(ASTNodeType.GlobalAssignment);
            node.Value = new SymbolValue(variableName);
            if (value != null) node.Children.Add(value);
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
            if (body != null) node.Children.Add(body);
            return node;
        }

        public static ASTNode MakeFunctionCall(ASTNode function, List<ASTNode> arguments)
        {
            var node = new ASTNode(ASTNodeType.FunctionCall);
            if (function != null) node.Children.Add(function);
            if (arguments != null) node.Children.AddRange(arguments);
            return node;
        }
    }

    public class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        private readonly string sourceText;
        #pragma warning disable CS0414
        private bool parsingDotApplyArguments = false; // Track context for dot-apply arguments
#pragma warning restore CS0414

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

        private int delimiterDepth = 0; // Track nesting depth of delimiters
        
        public ASTNode? Parse()
        {
            if (tokens.Count == 0)
            {
                throw new Exception("No tokens to parse");
            }

            delimiterDepth = 0; // Reset delimiter depth for each parse

            var result = ParseExpression();

            // Handle case where ParseExpression returns null (e.g., due to NEWLINE at start)
            if (result == null)
            {
                return null;
            }

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
                        var stmt = ParseExpression();
                        if (stmt != null)  // Only add non-null statements
                        {
                            statements.Add(stmt);
                        }
                    }
                }

                // If we have multiple statements, create a block
                if (statements.Count > 1)
                {
                    var block = new ASTNode(ASTNodeType.Block);
                    block.Children.AddRange(statements);
                    
                    // At top level, this block represents a niladic function
                    // According to K spec, it should return the value of the last expression
                    // We'll handle this in the evaluator by returning the last statement's value
                    return block;
                }
            }

            // Check if the result is a semicolon-separated vector (mixed list)
            // Only apply niladic wrapping if we're at true top level (no delimiters in original expression)
            // AND this is a semicolon-separated vector, not a space-separated vector
            if (result.Type == ASTNodeType.Vector && result.Children.Count > 1)
            {
                // Simple heuristic: if the original expression contains any delimiters, 
                // don't apply niladic wrapping
                bool hasDelimiters = false;
                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.LEFT_PAREN || token.Type == TokenType.RIGHT_PAREN ||
                        token.Type == TokenType.LEFT_BRACE || token.Type == TokenType.RIGHT_BRACE ||
                        token.Type == TokenType.LEFT_BRACKET || token.Type == TokenType.RIGHT_BRACKET)
                    {
                        hasDelimiters = true;
                        break;
                    }
                }
                
                // Additional check: only apply niladic wrapping if there are semicolons in the original tokens
                // This distinguishes semicolon-separated vectors from space-separated vectors
                bool hasSemicolons = false;
                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.SEMICOLON)
                    {
                        hasSemicolons = true;
                        break;
                    }
                }
                
                // Only apply niladic wrapping if no delimiters were found AND there are semicolons
                if (!hasDelimiters && hasSemicolons)
                {
                    // This represents a semicolon-separated list at top level
                    // According to K spec, evaluate as niladic function returning last element
                    var block = new ASTNode(ASTNodeType.Block);
                    block.Children.AddRange(result.Children);
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
                   type == TokenType.IN || type == TokenType.BIN || type == TokenType.BINL || type == TokenType.LIN ||
                   type == TokenType.DV || type == TokenType.DI || type == TokenType.VS || type == TokenType.SV || type == TokenType.SS || type == TokenType.SM || type == TokenType.CI || type == TokenType.IC ||
                   type == TokenType.POWER || type == TokenType.MODULUS || type == TokenType.JOIN ||
                   type == TokenType.COLON || type == TokenType.HASH || type == TokenType.UNDERSCORE || type == TokenType.QUESTION || 
                   type == TokenType.DOLLAR || type == TokenType.DRAW || type == TokenType.GETENV || type == TokenType.SETENV || type == TokenType.SIZE || type == TokenType.STRING_REPRESENTATION ||
                   type == TokenType.APPLY;
        }

        private static readonly TokenType[] ParseUntilEndStopTokens = {
            TokenType.RIGHT_PAREN, TokenType.RIGHT_BRACE, TokenType.SEMICOLON, TokenType.NEWLINE, TokenType.EOF
        };
        
        private static readonly TokenType[] DefaultStopTokens = {
            TokenType.PLUS, TokenType.MINUS, TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.MIN, TokenType.MAX,
            TokenType.LESS, TokenType.GREATER, TokenType.EQUAL, TokenType.IN, TokenType.POWER, TokenType.MODULUS, TokenType.JOIN,
            TokenType.COLON, TokenType.HASH, TokenType.UNDERSCORE, TokenType.QUESTION, TokenType.NEGATE, TokenType.DOLLAR, TokenType.RIGHT_PAREN,
            TokenType.RIGHT_BRACE, TokenType.RIGHT_BRACKET, TokenType.SEMICOLON, TokenType.NEWLINE, TokenType.ASSIGNMENT, TokenType.GLOBAL_ASSIGNMENT,
            TokenType.LEFT_BRACKET, TokenType.APPLY, TokenType.DOT_APPLY, TokenType.TYPE, TokenType.STRING_REPRESENTATION,
            TokenType.ADVERB_SLASH, TokenType.ADVERB_BACKSLASH, TokenType.ADVERB_TICK,
            TokenType.ADVERB_SLASH_COLON, TokenType.ADVERB_BACKSLASH_COLON, TokenType.ADVERB_TICK_COLON,
            TokenType.TIME, TokenType.IN, TokenType.BIN, TokenType.BINL, TokenType.LSQ, TokenType.LIN,
            TokenType.GTIME, TokenType.LTIME, TokenType.VS, TokenType.SV, TokenType.SS, TokenType.CI, TokenType.IC,
            TokenType.DIRECTORY, TokenType.DO, TokenType.WHILE, TokenType.IF_FUNC, TokenType.EXIT, TokenType.EOF
        };
        
        private bool ShouldStopParsing(TokenType[] stopTokens)
        {
            return IsAtEnd() || stopTokens.Contains(CurrentToken().Type);
        }
        
        private ASTNode? ParseTerm(bool parseUntilEnd = false)
        {
            // Only return null for EOF, not for NEWLINE when parsing expressions
            // NEWLINE should be handled at higher levels as statement separators
            if (IsAtEnd() || CurrentToken().Type == TokenType.EOF)
            {
                return null;
            }
            
            var result = ParsePrimary();

            // Handle case where ParsePrimary returned null
            if (result == null)
            {
                return null;
            }

            // Handle high-precedence adverb operations (verb-adverb binding has higher precedence than operators)
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                              CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                              CurrentToken().Type == TokenType.ADVERB_TICK ||
                              CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                              CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                              CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
            {
                var adverbToken = PreviousToken() ?? CurrentToken();
                var adverbType = CurrentToken().Type;
                Match(adverbType); // Consume the adverb token
                
                // Parse the right argument for the adverb
                var rightArg = ParseTerm(parseUntilEnd);
                
                // Create adverb node: ADVERB(adverbType, verb, rightArg)
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString());
                adverbNode.Children.Add(result); // verb
                adverbNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(0))); // left argument (default for dyadic adverbs)
                if (rightArg != null) adverbNode.Children.Add(rightArg); // right argument
                return adverbNode;
            }

            // Handle postfix operations: bracket notation for indexing or function calls
            while (!IsAtEnd() && CurrentToken().Type == TokenType.LEFT_BRACKET)
            {
                // Parse bracket notation: expression[index] or function[args]
                Match(TokenType.LEFT_BRACKET); // Consume '['
                
                // Parse the arguments expression (can be semicolon-separated)
                var argsExpression = ParseExpressionInsideDelimiters();
                if (argsExpression == null)
                {
                    throw new Exception("Expected arguments expression in brackets");
                }
                
                if (!Match(TokenType.RIGHT_BRACKET))
                {
                    throw new Exception("Expected ']' after arguments expression");
                }
                
                // Check if this is a control flow verb function call
                if (result != null && result.Type == ASTNodeType.Variable)
                {
                    var varName = result.Value is SymbolValue symbol ? symbol.Value : result.Value?.ToString() ?? "";
                    if (varName == "if" || varName == "while" || varName == "do")
                    {
                        // Create function call for control flow verbs
                        var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                        funcCall.Children.Add(result);
                        
                        // For control flow verbs, always add the arguments as a single expression
                        // The control flow functions will handle parsing the vector internally
                        if (argsExpression != null) funcCall.Children.Add(argsExpression);
                        
                        result = funcCall;
                    }
                    else
                    {
                        // Convert to dot-apply for regular indexing: expression . index
                        result = argsExpression != null && result != null ? ASTNode.MakeBinaryOp(TokenType.DOT_APPLY, result, argsExpression) : result;
                    }
                }
                else
                {
                    // Convert to dot-apply for regular indexing: expression . index
                    result = argsExpression != null && result != null ? ASTNode.MakeBinaryOp(TokenType.DOT_APPLY, result, argsExpression) : result;
                }
            }

            // If we found bracket notation, return the dot-apply result
            if (result != null && CurrentToken().Type == TokenType.LEFT_BRACKET)
            {
                return result;
            }

            // Continue with regular term parsing for vectors
            var elements = new List<ASTNode>();
            if (result != null) elements.Add(result);
            var firstElementType = result?.Type ?? ASTNodeType.Literal;
            var firstValueType = result?.Value?.GetType();
            var stopTokens = parseUntilEnd ? ParseUntilEndStopTokens : DefaultStopTokens;

            while (!ShouldStopParsing(stopTokens))
            {
                // Check if this would create a mixed-type vector
                // If the current token is an operator, it would create a mixed-type vector
                if (IsBinaryOperator(CurrentToken().Type))
                {
                    // This would create a mixed-type vector, so stop parsing and let ParseExpression handle it
                    break;
                }
                
                var nextElement = ParsePrimary();
                
                // Special case: function calls (variable followed by expression)
                // Check if the first element is a variable and the next element is any expression
                if (firstElementType == ASTNodeType.Variable && nextElement != null)
                {
                    // This is a function call: functionName argument
                    var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                    funcCall.Children.Add(elements[0]); // Function name
                    if (nextElement != null) funcCall.Children.Add(nextElement);  // Argument
                    return funcCall;
                }
                
                // Special case: compact symbol vectors (symbols back-to-back without spaces)
                // Check if the first element was a symbol and the current element is also a symbol
                if (firstElementType == ASTNodeType.Literal && 
                    firstValueType == typeof(SymbolValue) &&
                    nextElement?.Type == ASTNodeType.Literal && 
                    nextElement?.Value is SymbolValue)
                {
                    // Both are symbols, allow them to be combined regardless of uniform type check
                    if (nextElement != null) elements.Add(nextElement);
                    continue;
                }
                
                // Check for type uniformity
                if (nextElement?.Type != firstElementType || 
                    (nextElement?.Value?.GetType() != firstValueType && firstValueType != null))
                {
                    // Mixed types detected - this should be an arithmetic expression, not a vector
                    // Put the element back and let ParseExpression handle it
                    // For now, just break the vector parsing
                    break;
                }
                else
                {
                    if (nextElement != null) elements.Add(nextElement);
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

        private ASTNode? ParsePrimary()
        {
            ASTNode? result = null;

            // Handle NEWLINE tokens as statement separators (per NSL parser insight)
            if (Match(TokenType.NEWLINE))
            {
                // NEWLINE should be handled at higher levels as statement separator
                // Return null to indicate no primary expression found
                return null;
            }

            // Handle SEMICOLON tokens as expression separators (for empty positions)
            // Check without consuming so higher levels can handle it
            if (!IsAtEnd() && CurrentToken().Type == TokenType.SEMICOLON)
            {
                // SEMICOLON should be handled at higher levels as expression separator
                // Return null to indicate empty position in semicolon-separated list
                return null;
            }

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
                if (lexeme == "0Ij" || lexeme == "0Nj" || lexeme == "-0Ij")
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
                // Create a VectorValue containing individual CharacterValue objects
                var charVector = new List<K3Value>();
                foreach (char c in value)
                {
                    charVector.Add(new CharacterValue(c.ToString()));
                }
                result = ASTNode.MakeLiteral(new VectorValue(charVector));
            }
            else if (Match(TokenType.SYMBOL))
            {
                var symbol = PreviousToken().Lexeme;
                
                // Check if this is a dotted notation for K tree access
                if (symbol.Contains("."))
                {
                    // This is a K tree dotted notation variable
                    result = ASTNode.MakeVariable(symbol);
                }
                else
                {
                    // Regular symbol
                    result = ASTNode.MakeLiteral(new SymbolValue(symbol));
                }
            }
            else if (Match(TokenType.PLUS))
            {
                // Check if this is unary transpose (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("+"));
                    }
                    else
                    {
                        // Check if this is a standalone plus operator at the end of an expression
                        if (IsAtEnd() || CurrentToken().Type == TokenType.RIGHT_PAREN || 
                            CurrentToken().Type == TokenType.RIGHT_BRACKET || CurrentToken().Type == TokenType.RIGHT_BRACE ||
                            CurrentToken().Type == TokenType.SEMICOLON || CurrentToken().Type == TokenType.NEWLINE)
                        {
                            // This is just the plus operator itself - treat as variable
                            result = ASTNode.MakeVariable("+");
                        }
                        else
                        {
                            // This is unary transpose
                            var operand = ParsePrimary();
                            var node = new ASTNode(ASTNodeType.BinaryOp);
                            node.Value = new SymbolValue("+");
                            if (operand != null) node.Children.Add(operand);
                            return node;
                        }
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
                    if (lexeme == "0Ij" || lexeme == "0Nj" || lexeme == "-0Ij")
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
                                           CurrentToken().Type == TokenType.ADVERB_TICK ||
                                           CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                           CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                           CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                            if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                
                // Check if this is a dotted notation for K tree access
                if (identifier.Contains("."))
                {
                    // This is a K tree dotted notation variable
                    result = ASTNode.MakeVariable(identifier);
                }
                else
                {
                    // Regular identifier
                    result = ASTNode.MakeVariable(identifier);
                }
            }
            else if (Match(TokenType.DO))
            {
                result = ASTNode.MakeVariable("do");
            }
            else if (Match(TokenType.WHILE))
            {
                result = ASTNode.MakeVariable("while");
            }
            else if (Match(TokenType.IF_FUNC))
            {
                result = ASTNode.MakeVariable("if");
            }
            else if (Match(TokenType.COLON))
            {
                // Check if this is monadic colon (return operator) by looking ahead
                // If the next token is not an identifier or variable, it's likely monadic
                if (IsAtEnd() || (!IsIdentifierToken(CurrentToken().Type) && CurrentToken().Type != TokenType.LEFT_BRACKET))
                {
                    // Monadic colon - return operator
                    var operand = ParseExpression();
                    var node = new ASTNode(ASTNodeType.BinaryOp);
                    node.Value = new SymbolValue(":");
                    if (operand != null) node.Children.Add(operand);
                    return node;
                }
                else
                {
                    // This will be handled as binary operator (assignment)
                    result = ASTNode.MakeLiteral(new SymbolValue(":"));
                }
            }
            // Dyadic operators as primary expressions for bracket notation
            else if (Match(TokenType.PLUS))
            {
                result = ASTNode.MakeVariable("+");
            }
            else if (Match(TokenType.MINUS))
            {
                result = ASTNode.MakeVariable("-");
            }
            else if (Match(TokenType.MULTIPLY))
            {
                result = ASTNode.MakeVariable("*");
            }
            else if (Match(TokenType.DIVIDE))
            {
                result = ASTNode.MakeVariable("%");
            }
            else if (Match(TokenType.POWER))
            {
                result = ASTNode.MakeVariable("*");
            }
            else if (Match(TokenType.MODULUS))
            {
                result = ASTNode.MakeVariable("!");
            }
            else if (Match(TokenType.LESS))
            {
                result = ASTNode.MakeVariable("<");
            }
            else if (Match(TokenType.GREATER))
            {
                result = ASTNode.MakeVariable(">");
            }
            else if (Match(TokenType.EQUAL))
            {
                result = ASTNode.MakeVariable("=");
            }
            else if (Match(TokenType.JOIN))
            {
                result = ASTNode.MakeVariable(",");
            }
            else if (Match(TokenType.LEFT_PAREN))
            {
                // Check for empty parentheses () which should be an empty list
                if (Match(TokenType.RIGHT_PAREN))
                {
                    // Empty parentheses create an empty list
                    result = ASTNode.MakeVector(new List<ASTNode>());
                }
                else
                {
                    // Parse parenthesized expression - semicolon handling is at expression level
                    delimiterDepth++; // Enter delimiter
                    var expression = ParseExpressionInsideDelimiters();
                    delimiterDepth--; // Exit delimiter
                    
                    if (!Match(TokenType.RIGHT_PAREN))
                    {
                        throw new Exception("Expected ')' after expression");
                    }
                    
                    result = expression;
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary negate symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("~"));
                }
            }
            else if (Match(TokenType.DOLLAR))
            {
                // Check if this is unary format (at start of expression)
                if (result == null)
                {
                    // Look ahead to see if this is part of an adverb operation
                    if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
                    {
                        // This is a verb symbol for an adverb operation
                        result = ASTNode.MakeLiteral(new SymbolValue("$"));
                    }
                    else
                    {
                        // This is unary format
                        var operand = ParseExpression();
                        var node = new ASTNode(ASTNodeType.BinaryOp);
                        node.Value = new SymbolValue("$");
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary format symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("$"));
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
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
                                      CurrentToken().Type == TokenType.ADVERB_TICK ||
                                      CurrentToken().Type == TokenType.ADVERB_SLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_BACKSLASH_COLON || 
                                      CurrentToken().Type == TokenType.ADVERB_TICK_COLON))
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
                        if (operand != null) node.Children.Add(operand);
                        return node;
                    }
                }
                else
                {
                    // Binary question symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("?"));
                }
            }
            else if (Match(TokenType.MODULUS))
            {
                // Check if this is unary enumerate (at start of expression)
                if (result == null)
                {
                    // This is unary enumerate
                    var operand = ParsePrimary();
                    var node = new ASTNode(ASTNodeType.BinaryOp);
                    node.Value = new SymbolValue("!");
                    if (operand != null) node.Children.Add(operand);
                    return node;
                }
                else
                {
                    // Binary modulus symbol
                    result = ASTNode.MakeLiteral(new SymbolValue("!"));
                }
            }
            else if (Match(TokenType.ADVERB_SLASH))
            {
                // Standalone adverb slash - this should not happen in valid K3
                // According to spec, slash must be preceded by a verb without spaces
                throw new Exception("Adverb slash must be preceded by a verb");
            }
            else if (Match(TokenType.ADVERB_BACKSLASH))
            {
                // Standalone adverb backslash - this should not happen in valid K3
                throw new Exception("Adverb backslash must be preceded by a verb");
            }
            else if (Match(TokenType.ADVERB_TICK))
            {
                // Standalone adverb tick - this should not happen in valid K3
                throw new Exception("Adverb tick must be preceded by a verb");
            }
            else if (Match(TokenType.LOG))
            {
                // Mathematical logarithm operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_log");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.EXP))
            {
                // Mathematical exponential operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_exp");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ABS))
            {
                // Mathematical absolute value operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_abs");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SQR))
            {
                // Mathematical square operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sqr");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SQRT))
            {
                // Mathematical square root operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sqrt");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.FLOOR_MATH))
            {
                // Mathematical floor operation (always returns float)
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_floor");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DOT))
            {
                // Linear algebra dot product operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_dot");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.MUL))
            {
                // Linear algebra matrix multiplication operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_mul");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.INV))
            {
                // Linear algebra matrix inverse operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_inv");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SIN))
            {
                // Mathematical sine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sin");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.COS))
            {
                // Mathematical cosine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_cos");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TAN))
            {
                // Mathematical tangent operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_tan");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ASIN))
            {
                // Mathematical arcsine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_asin");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ACOS))
            {
                // Mathematical arccosine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_acos");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.ATAN))
            {
                // Mathematical arctangent operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_atan");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.SINH))
            {
                // Mathematical hyperbolic sine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_sinh");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.COSH))
            {
                // Mathematical hyperbolic cosine operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_cosh");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TANH))
            {
                // Mathematical hyperbolic tangent operation
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_tanh");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.TIME))
            {
                // Current time function - niladic, create function call
                var functionNode = ASTNode.MakeVariable("_t");
                return ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
            }
            else if (Match(TokenType.DIRECTORY))
            {
                // Directory function - niladic, create function call
                var functionNode = ASTNode.MakeVariable("_d");
                return ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
            }
            else if (Match(TokenType.LSQ))
            {
                // Least squares function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_lsq");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.GTIME))
            {
                // GMT time conversion function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_gtime");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LTIME))
            {
                // Local time conversion function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ltime");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.VS))
            {
                // _vs is dyadic, provide helpful error for monadic usage
                throw new Exception("_vs (vector from scalar) requires two arguments - use infix notation: base _vs scalar");
            }
            else if (Match(TokenType.DRAW))
            {
                // _draw is dyadic, provide helpful error for monadic usage
                throw new Exception("_draw requires dyadic call (left and right arguments)");
            }
            else if (Match(TokenType.GETENV))
            {
                // _getenv is monadic, create function call
                var functionNode = ASTNode.MakeVariable("_getenv");
                var operand = ParseExpression();
                var callNode = ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
                if (operand != null) callNode.Children.Add(operand);
                return callNode;
            }
            else if (Match(TokenType.SETENV))
            {
                result = ASTNode.MakeVariable("_setenv");
            }
            else if (Match(TokenType.SIZE))
            {
                // _size is monadic, create function call
                var functionNode = ASTNode.MakeVariable("_size");
                var operand = ParseExpression();
                var callNode = ASTNode.MakeFunctionCall(functionNode, new List<ASTNode>());
                if (operand != null) callNode.Children.Add(operand);
                return callNode;
            }
            else if (Match(TokenType.SS))
            {
                // Database function - dyadic, needs two operands
                var leftOperand = ParseExpression();
                if (!Match(TokenType.SEMICOLON))
                    throw new Exception("_ss: Expected semicolon after left operand");
                var rightOperand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ss");
                if (leftOperand != null) node.Children.Add(leftOperand);
                if (rightOperand != null) node.Children.Add(rightOperand);
                return node;
            }
            else if (Match(TokenType.CI))
            {
                // Database function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ci");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.IC))
            {
                // Database function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_ic");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.EXIT))
            {
                // Exit function
                // Control flow function
                var operand = ParseExpression();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("_exit");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.LEFT_BRACE))
            {
                delimiterDepth++; // Enter delimiter
                
                // Check if this is a form specifier (empty braces followed by $)
                if (CurrentToken().Type == TokenType.RIGHT_BRACE && 
                    !IsAtEnd() && PeekNext().Type == TokenType.DOLLAR)
                {
                    // This is {} form specifier
                    Match(TokenType.RIGHT_BRACE); // Consume the }
                    delimiterDepth--; // Exit delimiter
                    
                    // Create a special node for {} form specifier
                    var node = new ASTNode(ASTNodeType.FormSpecifier);
                    node.Value = new SymbolValue("{}");
                    return node;
                }
                
                // Parse function
                var parameters = new List<string>();
                
                // Check if there's a parameter list
                if (CurrentToken().Type == TokenType.LEFT_BRACKET)
                {
                    // Parse parameter list
                    Match(TokenType.LEFT_BRACKET); // Consume the [
                    
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
                        // Skip empty lines (multiple newlines) and whitespace
                        while (Match(TokenType.NEWLINE))
                        {
                            // Skip consecutive newlines
                        }
                        
                        // Check if we have content before the next separator or closing brace
                        if (!IsAtEnd() && CurrentToken().Type != TokenType.RIGHT_BRACE && 
                            CurrentToken().Type != TokenType.SEMICOLON && CurrentToken().Type != TokenType.NEWLINE)
                        {
                            var expr = ParseExpression();
                            if (expr != null)  // Only add non-null expressions
                            {
                                statements.Add(expr);
                            }
                        }
                        
                        // Skip the separator (semicolon or newline) if present
                        // Treat newlines and semicolons as equivalent per NSL parser insight
                        if (Match(TokenType.SEMICOLON) || Match(TokenType.NEWLINE))
                        {
                            // Skip additional consecutive newlines after the separator
                            while (Match(TokenType.NEWLINE))
                            {
                                // Skip consecutive newlines
                            }
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
                
                delimiterDepth--; // Exit delimiter
                
                // Extract the source text of the function body
                int rightBracePos = PreviousToken().Position; // Position of the closing brace
                string? bodyText = "";
                List<Token>? preParsedTokens = null;
                
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
                        
                        // For nested functions, we'll defer validation to runtime
                        // The main parser should handle nested function definitions correctly
                        // We only validate simple function bodies here
                        if (!bodyText.Contains("{[") && !bodyText.Contains("::"))
                        {
                            try
                            {
                                var testParser = new Parser(preParsedTokens, bodyText);
                                testParser.Parse();
                            }
                            catch
                            {
                                // Pre-parsing failed, defer to runtime parsing
                                preParsedTokens = null;
                            }
                        }
                        else
                        {
                            // For complex function bodies with nested functions, store as character vector
                            // and use dot execute at runtime for more robust evaluation
                            preParsedTokens = null; // Don't pre-parse complex bodies
                        }
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
                result.Value = new FunctionValue(bodyText ?? "", parameters, preParsedTokens ?? new List<Token>());
            }
            else if (Match(TokenType.TYPE))
            {
                // 4: operator is unary - parse the operand
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("TYPE");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.STRING_REPRESENTATION))
            {
                // 5: operator is unary - parse the operand
                var operand = ParsePrimary();
                var node = new ASTNode(ASTNodeType.BinaryOp);
                node.Value = new SymbolValue("STRING_REPRESENTATION");
                if (operand != null) node.Children.Add(operand);
                return node;
            }
            else if (Match(TokenType.DOT_APPLY))
            {
                // Check if this is followed by LEFT_BRACKET for bracket syntax .[...]
                if (Match(TokenType.LEFT_BRACKET))
                {
                    // This is bracket syntax: .[...] - implement equivalence .[x] is (.) .,(x)
                    var bracketContents = ParseBracketContentsAsCommaEnlisted();
                    
                    // Create dot-apply node: (.) .,(x)
                    var dotApplyNode = new ASTNode(ASTNodeType.BinaryOp);
                    dotApplyNode.Value = new SymbolValue(".");
                    
                    // Left side is the dot operator itself
                    var dotNode = ASTNode.MakeVariable(".");
                    dotApplyNode.Children.Add(dotNode);
                    
                    // Create comma node for the ,(x) part
                    var commaNode = new ASTNode(ASTNodeType.BinaryOp);
                    commaNode.Value = new SymbolValue(",");
                    commaNode.Children.Add(bracketContents);
                    
                    dotApplyNode.Children.Add(commaNode);
                    result = dotApplyNode;
                }
                else if (!IsAtEnd() && (CurrentToken().Type == TokenType.IDENTIFIER || CurrentToken().Type == TokenType.SYMBOL))
                {
                    // This is K tree dotted notation: .k, .k.foo, etc.
                    var identifier = CurrentToken().Lexeme;
                    Advance(); // Consume the identifier/symbol
                    
                    // Construct the dotted notation variable name
                    var dottedVariable = "." + identifier;
                    
                    // Check if there are more dotted parts
                    while (Match(TokenType.DOT_APPLY) && !IsAtEnd() && 
                           (CurrentToken().Type == TokenType.IDENTIFIER || CurrentToken().Type == TokenType.SYMBOL))
                    {
                        dottedVariable += "." + CurrentToken().Lexeme;
                        Advance(); // Consume the identifier/symbol
                    }
                    
                    result = ASTNode.MakeVariable(dottedVariable);
                }
                else
                {
                    // Check if this is a standalone dot operator at the end of an expression
                    if (IsAtEnd() || CurrentToken().Type == TokenType.RIGHT_PAREN || 
                        CurrentToken().Type == TokenType.RIGHT_BRACKET || CurrentToken().Type == TokenType.RIGHT_BRACE ||
                        CurrentToken().Type == TokenType.SEMICOLON || CurrentToken().Type == TokenType.NEWLINE)
                    {
                        // This is just the dot operator itself - treat as variable
                        result = ASTNode.MakeVariable(".");
                    }
                    else
                    {
                        // This is unary MAKE (.) operator
                        var operand = ParsePrimary();
                        var makeNode = new ASTNode(ASTNodeType.BinaryOp);
                        makeNode.Value = new SymbolValue(".");
                        if (operand != null) makeNode.Children.Add(operand);
                        return makeNode;
                    }
                }
            }
            else if (Match(TokenType.APPLY))
            {
                // Check if this is followed by LEFT_BRACKET for bracket syntax @[...]
                if (Match(TokenType.LEFT_BRACKET))
                {
                    // This is bracket syntax: @[...] - implement equivalence @[x] is (@) .,(x)
                    var bracketContents = ParseBracketContentsAsCommaEnlisted();
                    
                    // Create dot-apply node: (@) .,(x)
                    var dotApplyNode = new ASTNode(ASTNodeType.BinaryOp);
                    dotApplyNode.Value = new SymbolValue(".");
                    
                    // Left side is the at operator itself
                    var atNode = ASTNode.MakeVariable("@");
                    dotApplyNode.Children.Add(atNode);
                    
                    // Create comma node for the ,(x) part
                    var commaNode = new ASTNode(ASTNodeType.BinaryOp);
                    commaNode.Value = new SymbolValue(",");
                    commaNode.Children.Add(bracketContents);
                    
                    dotApplyNode.Children.Add(commaNode);
                    result = dotApplyNode;
                }
                else
                {
                    // This is unary ATOM (@) operator
                    var operand = ParseTerm();
                    var atomNode = new ASTNode(ASTNodeType.BinaryOp);
                    atomNode.Value = new SymbolValue("@");
                    if (operand != null) atomNode.Children.Add(operand);
                    return atomNode;
                }
            }
            else if (Match(TokenType.EOF))
            {
                // End of input - return null result
                return null;
            }
            else
            {
                var currentToken = CurrentToken();
                throw new Exception($"Unexpected token: {currentToken.Type}({currentToken.Lexeme})");
            }

            return result;
        }

        private ASTNode ParseBracketContentsAsCommaEnlisted()
        {
            // Parse bracket contents to handle the comma enlistment part of f[x] is (f) .,(x)
            
            if (Match(TokenType.RIGHT_BRACKET))
            {
                // Empty brackets [] - return null (which becomes _n when comma-enlisted)
                return ASTNode.MakeLiteral(new NullValue());
            }
            
            // Parse expressions separated by semicolons (for function arguments)
            var expressions = new List<ASTNode>();
            
            // Parse first argument using a method that doesn't stop at semicolons
            var firstArg = ParseBracketArgument();
            if (firstArg != null) expressions.Add(firstArg);
            
            // Handle semicolon-separated arguments
            while (Match(TokenType.SEMICOLON))
            {
                var nextArg = ParseBracketArgument();
                if (nextArg != null) expressions.Add(nextArg);
            }
            
            if (!Match(TokenType.RIGHT_BRACKET))
            {
                throw new Exception("Expected ']' after bracket expression");
            }
            
            // If multiple expressions, create a vector; otherwise return single expression
            if (expressions.Count > 1)
            {
                return ASTNode.MakeVector(expressions);
            }
            else
            {
                return expressions[0];
            }
        }

        private ASTNode? ParseBracketArgument()
        {
            // Parse an expression for bracket arguments, treating semicolons as separators
            // This is similar to ParseExpression but doesn't stop at semicolons
            
            var left = ParseTerm();
            if (left == null)
            {
                return null;
            }

            // Handle binary operators but stop at semicolon or right bracket
            while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.DIVIDE) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) || Match(TokenType.EQUAL) || Match(TokenType.IN) || Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.COLON) || Match(TokenType.HASH) || Match(TokenType.UNDERSCORE) || Match(TokenType.QUESTION) || Match(TokenType.DOLLAR) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION) ||
                   Match(TokenType.APPLY))
            {
                var op = PreviousToken().Type;
                
                // Check if this is followed by an adverb
                if (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK) ||
                    Match(TokenType.ADVERB_SLASH_COLON) || Match(TokenType.ADVERB_BACKSLASH_COLON) || Match(TokenType.ADVERB_TICK_COLON))
                {
                    var adverbType = PreviousToken().Type.ToString().Replace("TokenType.", "");
                    
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
                        "UNDERSCORE" => "_",
                        "BIN" => "_bin",
                        "BINL" => "_binl",
                        "LIN" => "_lin",
                        "DV" => "_dv",
                        "DI" => "_di",
                        "VS" => "_vs",
                        "SV" => "_sv",
                        "SS" => "_ss",
                "SM" => "_sm",
                        "CI" => "_ci",
                        "IC" => "_ic",
                        _ => op.ToString()
                    };
                    var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                    
                    // Parse the right side of the adverb
                    var rightSide = ParseBracketArgument();
                    
                    // Create the correct adverb structure: ADVERB(verb, left, right)
                    var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                    adverbNode.Value = new SymbolValue(adverbType);
                    if (verbNode != null) adverbNode.Children.Add(verbNode);
                    if (left != null) adverbNode.Children.Add(left);
                    if (rightSide != null) if (rightSide != null) adverbNode.Children.Add(rightSide);
                    
                    left = adverbNode;
                }
                else
                {
                    // Regular binary operation
                    var right = ParseTerm();
                    if (right == null)
                    {
                        throw new Exception($"Expected right operand after {op}");
                    }
                    left = ASTNode.MakeBinaryOp(op, left, right);
                }
            }

            return left;
        }

        private ASTNode? ParseElementForSemicolonVector()
        {
            // Parse an element for semicolon-separated vectors, handling nested structures properly
            // This method parses expressions but doesn't stop at semicolons at the top level of nested parentheses
            
            var left = ParseTerm();
            if (left == null)
            {
                return null;
            }

            // Handle binary operators but be careful about semicolons in nested structures
            while (!IsAtEnd() && 
                   CurrentToken().Type != TokenType.SEMICOLON && 
                   CurrentToken().Type != TokenType.RIGHT_PAREN &&
                   CurrentToken().Type != TokenType.RIGHT_BRACE &&
                   CurrentToken().Type != TokenType.RIGHT_BRACKET &&
                   IsBinaryOperator(CurrentToken().Type))
            {
                var op = MatchAndGetOperator();
                if (op == null) break;
                
                var right = ParseTerm();
                if (right == null) break;
                
                left = ASTNode.MakeBinaryOp(op.Type, left, right);
            }

            return left;
        }

        private ASTNode ParseParenthesizedForElement()
        {
            // Parse a parenthesized expression as a single element, handling internal semicolons properly
            // Reuse the existing parenthesized parsing logic from ParsePrimary
            
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
                        var beforeParsePos = current;
                        var expr = ParseExpression();
                        
                        // If we didn't make progress, break to avoid infinite loop
                        if (current == beforeParsePos)
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
                    
                    // Parse space-separated vector
                    var expression = ParseExpression();
                    
                    if (!Match(TokenType.RIGHT_PAREN))
                    {
                        throw new Exception("Expected ')' after expression");
                    }
                    
                    // If the expression is a vector, keep it as a vector
                    // Otherwise, return the expression as-is
                    return expression ?? ASTNode.MakeLiteral(new NullValue());
                }
                else
                {
                    // Parse space-separated vector
                    var expression = ParseExpression();
                    
                    if (!Match(TokenType.RIGHT_PAREN))
                    {
                        throw new Exception("Expected ')' after expression");
                    }
                    
                    // If the expression is a vector, keep it as a vector
                    // Otherwise, return the expression as-is
                    return expression ?? ASTNode.MakeLiteral(new NullValue());
                }
            }
            else
            {
                // Empty parentheses - create empty vector
                return ASTNode.MakeVector(new List<ASTNode>());
            }
        }

        private Token? MatchAndGetOperator()
        {
            if (IsAtEnd()) return null;
            
            var token = CurrentToken();
            if (IsBinaryOperator(token.Type))
            {
                Advance();
                return token;
            }
            return null;
        }

        private ASTNode SafeParsePrimary()
        {
            var result = ParsePrimary();
            if (result == null)
            {
                throw new Exception("Expected primary expression but found statement separator");
            }
            return result;
        }

        private ASTNode SafeParseExpression()
        {
            var result = ParseExpression();
            if (result == null)
            {
                throw new Exception("Expected expression but found statement separator");
            }
            return result;
        }

        private ASTNode ParseAdverbChain(string firstAdverb)
        {
            var adverbs = new List<string> { firstAdverb };
            
            // Collect additional adverbs for chaining
            while (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK) ||
                  Match(TokenType.ADVERB_SLASH_COLON) || Match(TokenType.ADVERB_BACKSLASH_COLON) || Match(TokenType.ADVERB_TICK_COLON))
            {
                adverbs.Add(PreviousToken().Type.ToString().Replace("TokenType.", ""));
            }
            
            // Parse the operand (the verb or data the adverbs apply to)
            var operand = ParsePrimary();
            
            // Create a chained adverb node
            var node = new ASTNode(ASTNodeType.BinaryOp);
            node.Value = new SymbolValue("ADVERB_CHAIN");
            if (operand != null) node.Children.Add(operand);
            
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
            if (IsAtEnd()) return new Token(TokenType.EOF, "", 0);
            return tokens[current];
        }

        private Token PreviousToken()
        {
            return tokens[current - 1];
        }

        private Token PeekNext()
        {
            var nextIndex = current + 1;
            if (nextIndex >= tokens.Count) return new Token(TokenType.EOF, "", 0);
            return tokens[nextIndex];
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

        public ASTNode? ParseExpression()
        {
            // Parse the first expression
            var result = ParseExpressionWithoutSemicolons();

            // Handle case where ParseExpressionWithoutSemicolons returns null (e.g., due to NEWLINE at start)
            if (result == null)
            {
                return null;
            }

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
                        var stmt = ParseExpressionWithoutSemicolons();
                        if (stmt != null)  // Only add non-null statements
                        {
                            statements.Add(stmt);
                        }
                    }
                }

                // If we have multiple statements, create a block
                if (statements.Count > 1)
                {
                    var block = new ASTNode(ASTNodeType.Block);
                    block.Children.AddRange(statements);
                    
                    // At top level, this block represents a niladic function
                    // According to K spec, it should return the value of the last expression
                    // We'll handle this in the evaluator by returning the last statement's value
                    return block;
                }
            }

            return result;
        }
        
        private ASTNode? ParseExpressionWithoutSemicolons()
        {
            // Check if we're at the start of an expression (no previous token or previous was a delimiter)
            var prevToken = current > 0 ? tokens[current - 1] : null;
            bool isAtExpressionStart = (prevToken == null || 
                                       prevToken.Type == TokenType.SEMICOLON || 
                                       prevToken.Type == TokenType.NEWLINE ||
                                       prevToken.Type == TokenType.LEFT_PAREN ||
                                       prevToken.Type == TokenType.LEFT_BRACE ||
                                       prevToken.Type == TokenType.LEFT_BRACKET ||
                                       prevToken.Type == TokenType.ASSIGNMENT ||
                                       prevToken.Type == TokenType.GLOBAL_ASSIGNMENT);
            
            ASTNode? left = null;
            
            // Check for operator[...] pattern at expression start
            if (isAtExpressionStart && !IsAtEnd() && IsBinaryOperator(CurrentToken().Type))
            {
                // Check if operator is followed by bracket notation
                if (current + 1 < tokens.Count && tokens[current + 1].Type == TokenType.LEFT_BRACKET)
                {
                    // This is operator[...] - treat as verb with bracket notation
                    // Create the operator variable directly instead of calling ParseTerm
                    var opToken = CurrentToken();
                    var opSymbol = opToken.Type.ToString() switch
                    {
                        "PLUS" => "+",
                        "MINUS" => "-",
                        "MULTIPLY" => "*",
                        "DIVIDE" => "%",
                        "POWER" => "^",
                        "MODULUS" => "!",
                        "MIN" => "&",
                        "MAX" => "|",
                        "LESS" => "<",
                        "GREATER" => ">",
                        "EQUAL" => "=",
                        "IN" => "in",
                        "JOIN" => ",",
                        "COLON" => ":",
                        "HASH" => "#",
                        "UNDERSCORE" => "_",
                        "QUESTION" => "?",
                        "DOLLAR" => "$",
                        "TYPE" => "@",
                        "STRING_REPRESENTATION" => "$",
                        "APPLY" => "@",
                        _ => opToken.Lexeme
                    };
                    left = ASTNode.MakeVariable(opSymbol);
                    Match(opToken.Type); // Consume the operator token
                    
                    if (left != null)
                    {
                        // Handle the bracket notation that follows
                        Match(TokenType.LEFT_BRACKET); // Consume '['
                        
                        // Parse the arguments expression (can be semicolon-separated)
                        var argsExpression = ParseExpressionInsideDelimiters();
                        if (argsExpression == null)
                        {
                            throw new Exception("Expected arguments expression in brackets");
                        }
                        
                        if (!Match(TokenType.RIGHT_BRACKET))
                        {
                            throw new Exception("Expected ']' after arguments expression");
                        }
                        
                        // Create dot-apply node: operator .,(args)
                        left = ASTNode.MakeBinaryOp(TokenType.DOT_APPLY, left, argsExpression);
                        
                        return left;
                    }
                }
            }
            
            if (isAtExpressionStart && !IsAtEnd())
            {
                // At expression start, try to parse verb with adverbs first
                // This handles prefix adverbs like %/, +/, {func}/', etc.
                var verbWithAdverbs = ParseVerbWithAdverbs();
                if (verbWithAdverbs != null)
                {
                    left = verbWithAdverbs;
                }
                else
                {
                    // Check for unary operators at expression start (like %4, -5, +3, etc.)
                    if (IsUnaryOperator(CurrentToken().Type))
                    {
                        var opToken = CurrentToken();
                        Match(opToken.Type); // Consume the unary operator
                        
                        // Parse the operand
                        var operand = ParseTerm();
                        if (operand == null)
                        {
                            throw new Exception($"Expected operand after unary operator {opToken.Type}");
                        }
                        
                        // Create unary operator node
                        var unaryNode = new ASTNode(ASTNodeType.BinaryOp);
                        var symbolValue = opToken.Type switch
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
                            TokenType.IN => "_in",
                            TokenType.BIN => "_bin",
                            TokenType.BINL => "_binl",
                            TokenType.LIN => "_lin",
                            TokenType.DV => "_dv",
                            TokenType.DI => "_di",
                            TokenType.VS => "_vs",
                            TokenType.SV => "_sv",
                            TokenType.SS => "_ss",
                            TokenType.SM => "_sm",
                            TokenType.CI => "_ci",
                            TokenType.IC => "_ic",
                            TokenType.DRAW => "_draw",
                            TokenType.POWER => "^",
                            TokenType.MODULUS => "!",
                            TokenType.JOIN => ",",
                            TokenType.COLON => ":",
                            TokenType.HASH => "#",
                            TokenType.UNDERSCORE => "_",
                            TokenType.QUESTION => "?",
                            TokenType.NEGATE => "~",
                            TokenType.DOLLAR => "$",
                            TokenType.APPLY => "@",
                            TokenType.DOT_APPLY => ".",
                            TokenType.TYPE => "TYPE",
                            TokenType.STRING_REPRESENTATION => "STRING_REPRESENTATION",
                            TokenType.DO => "do",
                            TokenType.WHILE => "while",
                            TokenType.IF_FUNC => "if",
                            _ => opToken.Type.ToString()
                        };
                        unaryNode.Value = new SymbolValue(symbolValue);
                        unaryNode.Children.Add(operand);
                        left = unaryNode;
                    }
                    else
                    {
                        left = ParseTerm();
                    }
                }
            }
            else
            {
                left = ParseTerm();
            }
            
            if (left == null) return null;
            
            // Handle binary operators with Long Right Scope (LRS)
            // In K, there's no precedence among operators - they're all right-associative
            while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.DIVIDE) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) || 
                   Match(TokenType.EQUAL) || Match(TokenType.IN) || Match(TokenType.BIN) || Match(TokenType.BINL) || Match(TokenType.LIN) ||
                   Match(TokenType.DV) || Match(TokenType.DI) || Match(TokenType.VS) || Match(TokenType.SV) || Match(TokenType.SS) || Match(TokenType.SM) || Match(TokenType.CI) || Match(TokenType.IC) ||
                   Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.COLON) || Match(TokenType.HASH) || Match(TokenType.UNDERSCORE) || Match(TokenType.QUESTION) || 
                   Match(TokenType.DOLLAR) || Match(TokenType.DRAW) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION) ||
                   Match(TokenType.APPLY))
            {
                var op = PreviousToken().Type;
                
                // Check if this is followed by an adverb (infix adverb)
                if (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK) ||
                    Match(TokenType.ADVERB_SLASH_COLON) || Match(TokenType.ADVERB_BACKSLASH_COLON) || Match(TokenType.ADVERB_TICK_COLON))
                {
                    var adverbType = PreviousToken().Type.ToString().Replace("TokenType.", "");
                    
                    // Convert the binary operator to a verb symbol
                    var verbName = op.ToString() switch
                    {
                        "PLUS" => "+",
                        "MINUS" => "-",
                        "MULTIPLY" => "*",
                        "DIVIDE" => "%",
                        "MIN" => "&",
                        "MAX" => "|",
                        "LESS" => "<",
                        "GREATER" => ">",
                        "EQUAL" => "=",
                        "IN" => "in",
                        "BIN" => "_bin",
                        "BINL" => "_binl",
                        "LIN" => "_lin",
                        "DV" => "_dv",
                        "DI" => "_di",
                        "VS" => "_vs",
                        "SV" => "_sv",
                        "SS" => "_ss",
                "SM" => "_sm",
                        "CI" => "_ci",
                        "IC" => "_ic",
                        "POWER" => "^",
                        "MODULUS" => "!",
                        "JOIN" => ",",
                        "COLON" => ":",
                        "HASH" => "#",
                        "UNDERSCORE" => "_",
                        "QUESTION" => "?",
                        "DOLLAR" => "$",
                        "TYPE" => "@",
                        "STRING_REPRESENTATION" => "$",
                        "APPLY" => "@",
                        _ => PreviousToken().Lexeme
                    };
                    var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                    
                    // Parse the right side of the adverb with LRS
                    var rightSide = ParseExpressionWithoutSemicolons();
                    
                    // Create the correct adverb structure: ADVERB(verb, left, right)
                    var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                    adverbNode.Value = new SymbolValue(adverbType);
                    adverbNode.Children.Add(verbNode);
                    adverbNode.Children.Add(left);
                    if (rightSide != null) if (rightSide != null) adverbNode.Children.Add(rightSide);
                    
                    return adverbNode;
                }
                else
                {
                    // Regular binary operation with Long Right Scope
                    // In K, the right argument is everything to the right (right-associative)
                    var right = ParseExpressionWithoutSemicolons();
                    if (right != null)
                        if (right != null) return ASTNode.MakeBinaryOp(op, left, right); return left;
                }
            }
            
            return left;
        }
        
        private ASTNode? ParseExpressionInsideDelimiters()
        {
            // Parse expression with the knowledge that we're inside delimiters
            // This affects semicolon behavior - they should create mixed lists
            var left = ParseTerm();
            
            // Handle case where first element is empty (e.g., "(;1;2)")
            if (left == null && !IsAtEnd() && CurrentToken().Type == TokenType.SEMICOLON)
            {
                // Empty first element becomes null in K semicolon-separated lists
                left = ASTNode.MakeLiteral(new NullValue());
            }
            
            if (left == null) return null;
            
            // Handle binary operators with Long Right Scope (LRS)
            if (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.DIVIDE) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) || 
                   Match(TokenType.EQUAL) || Match(TokenType.IN) || Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.COLON) || Match(TokenType.HASH) || Match(TokenType.UNDERSCORE) || Match(TokenType.QUESTION) || 
                   Match(TokenType.DOLLAR) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION) ||
                   Match(TokenType.APPLY))
            {
                var op = PreviousToken().Type;
                
                // Check if this is followed by an adverb (infix adverb)
                if (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK) ||
                    Match(TokenType.ADVERB_SLASH_COLON) || Match(TokenType.ADVERB_BACKSLASH_COLON) || Match(TokenType.ADVERB_TICK_COLON))
                {
                    var adverbType = PreviousToken().Type.ToString().Replace("TokenType.", "");
                    
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
                        "UNDERSCORE" => "_",
                        "BIN" => "_bin",
                        "BINL" => "_binl",
                        "LIN" => "_lin",
                        "TYPE" => "TYPE",
                        "APPLY" => "@",
                        _ => op.ToString()
                    };
                    var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                    
                    // Parse the right side of the adverb with LRS
                    var rightSide = ParseExpressionInsideDelimiters();
                    
                    // Create the correct adverb structure: ADVERB(verb, left, right)
                    var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                    adverbNode.Value = new SymbolValue(adverbType);
                    adverbNode.Children.Add(verbNode);
                    adverbNode.Children.Add(left);
                    if (rightSide != null) adverbNode.Children.Add(rightSide);
                    
                    return adverbNode;
                }
                else
                {
                    // Regular binary operation with Long Right Scope
                    var right = ParseExpressionInsideDelimiters();
                    if (right != null) return ASTNode.MakeBinaryOp(op, left, right); return left;
                }
            }
            
            // Handle semicolon-separated expressions - inside delimiters, create mixed lists
            if (Match(TokenType.SEMICOLON))
            {
                return ParseSemicolonList(left, true); // Inside delimiters = true
            }
            
            return left;
        }

        private ASTNode? ParseSemicolonList(ASTNode left, bool insideDelimiters)
        {
            var elements = new List<ASTNode> { left };
            
            do
            {
                // Stop if we hit a closing delimiter - let the caller handle it
                if (IsAtEnd() || CurrentToken().Type == TokenType.RIGHT_PAREN ||
                    CurrentToken().Type == TokenType.RIGHT_BRACE ||
                    CurrentToken().Type == TokenType.RIGHT_BRACKET ||
                    CurrentToken().Type == TokenType.NEWLINE)
                {
                    break;
                }
                
                // Parse a single expression for next element (without semicolon handling)
                var next = ParseTerm();
                
                // Handle binary operators with Long Right Scope for this element
                while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                       Match(TokenType.DIVIDE) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) || 
                       Match(TokenType.EQUAL) || Match(TokenType.IN) || Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                       Match(TokenType.COLON) || Match(TokenType.HASH) || Match(TokenType.UNDERSCORE) || Match(TokenType.QUESTION) || 
                       Match(TokenType.DOLLAR) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION) ||
                       Match(TokenType.APPLY))
                {
                    var op = PreviousToken().Type;
                    
                    // Check if this is followed by an adverb (infix adverb)
                    if (Match(TokenType.ADVERB_SLASH) || Match(TokenType.ADVERB_BACKSLASH) || Match(TokenType.ADVERB_TICK) ||
                    Match(TokenType.ADVERB_SLASH_COLON) || Match(TokenType.ADVERB_BACKSLASH_COLON) || Match(TokenType.ADVERB_TICK_COLON))
                    {
                        var adverbType = PreviousToken().Type.ToString().Replace("TokenType.", "");
                        
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
                            "UNDERSCORE" => "_",
                            "TYPE" => "TYPE",
                            _ => op.ToString()
                        };
                        var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                        
                        // Parse the right side of the adverb with LRS
                        var rightSide = ParseTerm();
                        
                        // Create the correct adverb structure: ADVERB(verb, left, right)
                        var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        adverbNode.Value = new SymbolValue(adverbType);
                        if (verbNode != null) adverbNode.Children.Add(verbNode);
                        if (next != null) adverbNode.Children.Add(next);
                        if (rightSide != null) adverbNode.Children.Add(rightSide);
                        
                        next = adverbNode;
                    }
                    else
                    {
                        // Regular binary operation with Long Right Scope
                        var right = ParseTerm();
                        if (next != null && right != null)
                            next = ASTNode.MakeBinaryOp(op, next, right);
                    }
                }
                
                // Handle empty positions: if next is null, add a null literal
                // This represents an empty position in a semicolon-separated list
                if (next != null)
                {
                    elements.Add(next);
                }
                else
                {
                    // Empty position becomes null in K semicolon-separated lists
                    elements.Add(ASTNode.MakeLiteral(new NullValue()));
                }
            } while (Match(TokenType.SEMICOLON));
            
            // Always create mixed list (anonymous list) for semicolon-separated expressions
            return ASTNode.MakeVector(elements);
        }

        private ASTNode? ParseAssignment()
        {
            return ParseTerm();
        }

        private ASTNode? ParseVerbWithAdverbs()
        {
            // Parse any expression that could be a verb
            var verb = ParseTerm();
            
            if (verb == null) return null;
            
            // Check if this verb is followed by an adverb (prefix adverb)
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                              CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                              CurrentToken().Type == TokenType.ADVERB_TICK))
            {
                var adverbType = CurrentToken().Type;
                Match(adverbType); // Consume the adverb
                
                // Parse the arguments for the adverb
                var arguments = ParseTerm();
                
                // Create the proper adverb structure: ADVERB(verb, 0, arguments)
                // Use 0 as initialization to signal "consume first element" for monadic derived verbs
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString().Replace("TokenType.", ""));
                if (verb != null) adverbNode.Children.Add(verb);
                adverbNode.Children.Add(new ASTNode(ASTNodeType.Literal, new IntegerValue(0))); // Use 0 for monadic derived verbs
                if (arguments != null) adverbNode.Children.Add(arguments);
                
                // Recursively check for more adverbs (derived verbs)
                return ParseVerbWithAdverbsRecursive(adverbNode);
            }
            
            return verb;
        }
        
        private ASTNode ParseVerbWithAdverbsRecursive(ASTNode derivedVerb)
        {
            // Check if this derived verb is followed by another adverb
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                              CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                              CurrentToken().Type == TokenType.ADVERB_TICK))
            {
                var adverbType = CurrentToken().Type;
                Match(adverbType); // Consume the adverb
                
                // Parse the arguments for the adverb
                var arguments = ParseTerm();
                
                // Create the proper adverb structure: ADVERB(derivedVerb, 0, arguments)
                // Use 0 as initialization to signal "consume first element" for monadic derived verbs
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString().Replace("TokenType.", ""));
                if (derivedVerb != null) adverbNode.Children.Add(derivedVerb);
                adverbNode.Children.Add(new ASTNode(ASTNodeType.Literal, new IntegerValue(0))); // Use 0 for monadic derived verbs
                if (arguments != null) adverbNode.Children.Add(arguments);
                
                // Recursively check for more adverbs
                return ParseVerbWithAdverbsRecursive(adverbNode);
            }
            
            return derivedVerb;
        }

        private bool IsIdentifierToken(TokenType type)
        {
            return type == TokenType.IDENTIFIER || 
                   type == TokenType.DO || 
                   type == TokenType.WHILE || 
                   type == TokenType.IF_FUNC ||
                   type == TokenType.DRAW ||
                   type == TokenType.GETENV ||
                   type == TokenType.SETENV ||
                   type == TokenType.SIZE ||
                   type == TokenType.IN ||
                   type == TokenType.BIN ||
                   type == TokenType.BINL ||
                   type == TokenType.LIN ||
                   type == TokenType.DV ||
                   type == TokenType.DI ||
                   type == TokenType.VS ||
                   type == TokenType.SV ||
                   type == TokenType.SS ||
                   type == TokenType.CI ||
                   type == TokenType.IC ||
                   type == TokenType.EXIT ||
                   type == TokenType.GTIME ||
                   type == TokenType.LTIME ||
                   type == TokenType.TYPE ||
                   type == TokenType.STRING_REPRESENTATION;
        }

        private bool IsUnaryOperator(TokenType type)
        {
            // Only operators that are commonly used as unary in K
            return type == TokenType.MINUS || 
                   type == TokenType.NEGATE ||
                   type == TokenType.DOLLAR ||
                   type == TokenType.DIVIDE ||  // % for reciprocal
                   type == TokenType.MULTIPLY || // * for first
                   type == TokenType.TYPE ||
                   type == TokenType.STRING_REPRESENTATION ||
                   type == TokenType.HASH ||
                   type == TokenType.UNDERSCORE ||
                   type == TokenType.QUESTION;
        }

        private bool IsAtEnd()
        {
            return current >= tokens.Count || (current < tokens.Count && tokens[current].Type == TokenType.EOF);
        }

        private ASTNode? HandlePrefixAdverb(string verbSymbol)
        {
            // Look ahead to see if this is part of an adverb operation
            if (!IsAtEnd() && (CurrentToken().Type == TokenType.ADVERB_SLASH || 
                              CurrentToken().Type == TokenType.ADVERB_BACKSLASH || 
                              CurrentToken().Type == TokenType.ADVERB_TICK))
            {
                // This is a verb symbol for an adverb operation
                var adverbType = CurrentToken().Type;
                Match(adverbType); // Consume the adverb
                
                // Create the verb node
                var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbSymbol));
                
                // Parse the arguments for the adverb
                var arguments = ParseTerm();
                
                // Create the proper adverb structure: ADVERB(verb, null, arguments)
                // For prefix adverbs, there's no left argument, so we use null
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString().Replace("TokenType.", ""));
                if (verbNode != null) adverbNode.Children.Add(verbNode);
                adverbNode.Children.Add(new ASTNode(ASTNodeType.Literal, new NullValue())); // Left argument is null for prefix adverbs
                if (arguments != null) adverbNode.Children.Add(arguments);
                
                return adverbNode;
            }
            else
            {
                // Not followed by an adverb, return null to indicate regular processing should continue
                return null;
            }
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
