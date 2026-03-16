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
        FormSpecifier,
        ProjectedFunction,
        ApplyAndAssign,
        ConditionalStatement
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
                TokenType.DIV => "_div",
                TokenType.DOT_PRODUCT => "_dot",

                TokenType.MUL => "_mul",
                TokenType.LSQ => "_lsq",
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
                TokenType.BD => "_bd",
                TokenType.DB => "_db",
                TokenType.LT => "_lt",
                TokenType.JD => "_jd",
                TokenType.DJ => "_dj",
                TokenType.GTIME => "_gtime",
                TokenType.LTIME => "_ltime",
                TokenType.MODULUS => "!",
                TokenType.JOIN => ",",
                TokenType.COLON => ":",
                TokenType.HASH => "#",
                TokenType.UNDERSCORE => "_",
                TokenType.QUESTION => "?",
                TokenType.MATCH => "~",
                TokenType.DOLLAR => "$",
                TokenType.APPLY => "@",
                TokenType.DOT_APPLY => ".",
                TokenType.TYPE => "TYPE",
                TokenType.STRING_REPRESENTATION => "STRING_REPRESENTATION",
                TokenType.AND => "_and",
                TokenType.OR => "_or",
                TokenType.XOR => "_xor",
                TokenType.ROT => "_rot",
                TokenType.SHIFT => "_shift",
                TokenType.NOT => "_not",
                TokenType.CEIL => "_ceil",
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

        public static ASTNode MakeApplyAndAssign(string variableName, TokenType op, ASTNode rightArgument)
        {
            var node = new ASTNode(ASTNodeType.ApplyAndAssign);
            node.Value = new SymbolValue(variableName);
            node.Children.Add(ASTNode.MakeLiteral(new SymbolValue(op.ToString())));
            node.Children.Add(rightArgument);
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

    public partial class Parser
    {
        private readonly List<Token> tokens;
        private int current = 0;
        private readonly string sourceText;
        #pragma warning disable CS0414
        private bool parsingDotApplyArguments = false; // Track context for dot-apply arguments
#pragma warning restore CS0414

        public Parser(List<Token> tokens, string sourceText)
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

                
        public ASTNode? Parse()
        {
            if (tokens.Count == 0)
            {
                throw new Exception("No tokens to parse");
            }

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

        private bool IsUnaryOperator(TokenType type)
        {
            var verb = VerbRegistry.GetVerb(type.ToString());
            return verb != null && verb.Type == VerbType.Operator && 
                   verb.SupportedArities != null && verb.SupportedArities.Contains(1);
        }

        private static readonly TokenType[] ParseUntilEndStopTokens = {
            TokenType.RIGHT_PAREN, TokenType.RIGHT_BRACE, TokenType.SEMICOLON, TokenType.NEWLINE, TokenType.EOF
        };
        
        private static readonly TokenType[] DefaultStopTokens = {
            TokenType.PLUS, TokenType.MINUS, TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.MIN, TokenType.MAX,
            TokenType.LESS, TokenType.GREATER, TokenType.EQUAL, TokenType.IN, TokenType.POWER, TokenType.MODULUS, TokenType.JOIN,
            TokenType.COLON, TokenType.HASH, TokenType.UNDERSCORE, TokenType.QUESTION, TokenType.MATCH, TokenType.NEGATE, TokenType.DOLLAR, TokenType.RIGHT_PAREN,
            TokenType.RIGHT_BRACE, TokenType.RIGHT_BRACKET, TokenType.SEMICOLON, TokenType.NEWLINE, TokenType.ASSIGNMENT, TokenType.GLOBAL_ASSIGNMENT,
            TokenType.LEFT_BRACKET, TokenType.APPLY, TokenType.DOT_APPLY, TokenType.TYPE, TokenType.STRING_REPRESENTATION,
            TokenType.ADVERB_SLASH, TokenType.ADVERB_BACKSLASH, TokenType.ADVERB_TICK,
            TokenType.ADVERB_SLASH_COLON, TokenType.ADVERB_BACKSLASH_COLON, TokenType.ADVERB_TICK_COLON,
            TokenType.TIME, TokenType.IN, TokenType.BIN, TokenType.BINL, TokenType.LSQ, TokenType.LIN,
            TokenType.GTIME, TokenType.LTIME, TokenType.VS, TokenType.SV, TokenType.SS, TokenType.CI, TokenType.IC,
            TokenType.AND, TokenType.OR, TokenType.XOR, TokenType.ROT, TokenType.SHIFT,
            TokenType.DIRECTORY, TokenType.BD, TokenType.DB, TokenType.DO, TokenType.WHILE, TokenType.IF_FUNC, TokenType.EXIT, TokenType.EOF
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
            
            ASTNode? result;
            
            // Check if current token is an adverb - if so, we're in a nesting context
            // Don't create placeholder for adverb operations - let ParseTerm handle them
            if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
            {
                // Parse the primary expression normally - adverb will be handled in ParseTerm
                result = ParsePrimary();
            }
            else
            {
                result = ParsePrimary();
            }

            // Handle case where ParsePrimary returned null
            if (result == null)
            {
                return null;
            }

            // Handle high-precedence adverb operations (verb-adverb binding has higher precedence than operators)
            if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
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
                result = adverbNode;
            }

            // Handle postfix operations: bracket notation for indexing or function calls
            while (!IsAtEnd() && CurrentToken().Type == TokenType.LEFT_BRACKET)
            {
                // Parse bracket notation: expression[index] or function[args]
                Match(TokenType.LEFT_BRACKET); // Consume '['

                // Parse bracket arguments with proper semicolon separation
                // ParseBracketContentsAsCommaEnlisted handles ']' consumption internally
                var argsExpression = ParseBracketContentsAsCommaEnlisted();

                // Special case: if the result is a unary operator (like _sqrt), treat brackets as grouping
                // So _sqrt[3] becomes _sqrt 3, not _sqrt @ [3]
                if (result != null && result.Type == ASTNodeType.BinaryOp && result.Value is SymbolValue opSymbol && 
                    opSymbol.Value.ToString().StartsWith("_") && result.Children.Count == 1)
                {
                    // This is a unary operator with brackets - treat as grouping
                    // Replace the unary operator's child with the bracket contents
                    if (argsExpression != null)
                    {
                        result.Children[0] = argsExpression;
                    }
                    // Don't consume more brackets - break the loop
                    break;
                }

                // Special case: if the result is a literal underscore function (like _ic), treat brackets as grouping
                // So _ic[x] becomes _ic x, not _ic @ [x]
                if (result != null && result.Type == ASTNodeType.Literal && result.Value is SymbolValue literalSymbol && 
                    literalSymbol.Value.ToString().StartsWith("_"))
                {
                    // This is a literal underscore function with brackets - convert to unary operator
                    var unaryNode = new ASTNode(ASTNodeType.BinaryOp);
                    unaryNode.Value = literalSymbol;
                    if (argsExpression != null)
                    {
                        unaryNode.Children.Add(argsExpression);
                    }
                    result = unaryNode;
                    // Don't consume more brackets - break the loop
                    break;
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
                        // Convert to apply operation: expression @ index
                        result = argsExpression != null && result != null ? ASTNode.MakeBinaryOp(TokenType.APPLY, result, argsExpression) : result;
                    }
                }
                else
                {
                    // Convert to apply operation: expression @ index
                    result = argsExpression != null && result != null ? ASTNode.MakeBinaryOp(TokenType.APPLY, result, argsExpression) : result;
                }
            }

            // If we processed bracket notation, return the result (don't continue with vector parsing)
            if (result != null && result.Type == ASTNodeType.BinaryOp && result.Value is SymbolValue symbolValue && symbolValue.Value == ".")
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
                // If current token is an operator, it would create a mixed-type vector
                // So stop parsing and let ParseExpression handle it
                if (IsBinaryOperator(CurrentToken().Type) ||
                    CurrentToken().Type == TokenType.DV || CurrentToken().Type == TokenType.DI ||
                    CurrentToken().Type == TokenType.SETENV || CurrentToken().Type == TokenType.SM ||
                    CurrentToken().Type == TokenType.DIV || CurrentToken().Type == TokenType.DRAW ||
                    CurrentToken().Type == TokenType.IO_VERB_0 || CurrentToken().Type == TokenType.IO_VERB_1 ||
                    CurrentToken().Type == TokenType.IO_VERB_2 || CurrentToken().Type == TokenType.IO_VERB_3 ||
                    CurrentToken().Type == TokenType.IO_VERB_6 || CurrentToken().Type == TokenType.IO_VERB_7 ||
                    CurrentToken().Type == TokenType.IO_VERB_8 || CurrentToken().Type == TokenType.IO_VERB_9)
                {
                    break;
                }
                
                if (CurrentToken().Type == TokenType.SYMBOL && CurrentToken().Lexeme == "_dv" || CurrentToken().Lexeme == "_di")
                {
                    var symbol = CurrentToken().Lexeme;
                    Match(TokenType.SYMBOL);
                    var rightArg = ParseTerm(parseUntilEnd);
                    var binaryOp = new ASTNode(ASTNodeType.BinaryOp);
                    binaryOp.Value = new SymbolValue(symbol);
                    if (result != null) binaryOp.Children.Add(result);
                    if (rightArg != null) binaryOp.Children.Add(rightArg);
                    result = binaryOp;
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
                bool firstIsSymbol = (firstElementType == ASTNodeType.Literal && firstValueType == typeof(SymbolValue)) ||
                                  (firstElementType == ASTNodeType.Variable && firstValueType == typeof(SymbolValue));
                bool nextIsSymbol = (nextElement?.Type == ASTNodeType.Literal && nextElement?.Value is SymbolValue) ||
                                  (nextElement?.Type == ASTNodeType.Variable && nextElement?.Value is SymbolValue);
                
                if (firstIsSymbol && nextIsSymbol)
                {
                    // Both are symbols, allow them to be combined regardless of uniform type check
                    if (nextElement != null) elements.Add(nextElement);
                    continue;
                }
                
                // Check for type uniformity
                if (nextElement?.Type != firstElementType || 
                    (nextElement?.Value?.GetType() != firstValueType && firstValueType != null))
                {
                    // Check if this is int/float mixing - allow it as it will be converted to float
                    bool firstIsNumber = elements[0]?.Value is IntegerValue || elements[0]?.Value is LongValue || elements[0]?.Value is FloatValue;
                    bool nextIsNumber = nextElement?.Value is IntegerValue || nextElement?.Value is LongValue || nextElement?.Value is FloatValue;
                    
                    if (firstIsNumber && nextIsNumber)
                    {
                        if (nextElement != null) elements.Add(nextElement);
                    }
                    else
                    {
                        // Mixed types detected - this should be an arithmetic expression, not a vector
                        // Put the element back and let ParseExpression handle it
                        // For now, just break the vector parsing
                        break;
                    }
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

            // Check for standalone operator as function reference (e.g., + in @[x; i; +; y])
            // or operator followed by adverb (e.g., +/ in @[+/; args; :])
            if (!IsAtEnd() && IsBinaryOperator(CurrentToken().Type))
            {
                var nextIdx = current + 1;
                var nextType = nextIdx < tokens.Count ? tokens[nextIdx].Type : TokenType.EOF;
                
                if (nextType == TokenType.SEMICOLON || nextType == TokenType.RIGHT_BRACKET || nextType == TokenType.RIGHT_PAREN || nextType == TokenType.EOF)
                {
                    // Standalone operator - treat as symbol
                    var opToken = CurrentToken();
                    var opSymbol = VerbRegistry.GetBinaryOperatorSymbol(opToken.Type);
                    Match(opToken.Type);
                    return ASTNode.MakeLiteral(new SymbolValue(opSymbol));
                }
                else if (VerbRegistry.IsAdverbToken(nextType))
                {
                    // Operator followed by adverb - create projected function
                    var opToken = CurrentToken();
                    var adverbToken = tokens[nextIdx];
                    
                    var opSymbol = VerbRegistry.GetBinaryOperatorSymbol(opToken.Type);
                    
                    var adverbType = VerbRegistry.GetAdverbType(adverbToken.Type);
                    
                    // Create projected function
                    var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                    projectedNode.Value = new SymbolValue(adverbType);
                    projectedNode.Children.Add(ASTNode.MakeLiteral(new SymbolValue(opSymbol))); // Store the verb
                    projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1))); // Needs 1 more argument
                    
                    // Consume both tokens
                    Match(opToken.Type);
                    Match(adverbToken.Type);
                    
                    return projectedNode;
                }
            }

            var left = ParseTerm();
            if (left == null)
            {
                return null;
            }

            // Handle assignment operators (including modified assignments like +:, -:, *:)
            if (!IsAtEnd() && CurrentToken().Type == TokenType.ASSIGNMENT)
            {
                var assignToken = CurrentToken();
                Match(TokenType.ASSIGNMENT);
                var right = ParseBracketArgument();
                if (right == null)
                {
                    throw new Exception("Expected expression after assignment operator");
                }
                // Extract variable name from left side
                if (left.Type == ASTNodeType.Variable)
                {
                    var variableName = left.Value is SymbolValue sym ? sym.Value : left.Value?.ToString() ?? "";
                    
                    // Validate variable name - cannot start with underscore
                    if (variableName.StartsWith("_"))
                    {
                        throw new Exception("Names starting with _ are reserved");
                    }
                    
                    if (assignToken.Lexeme.Length > 1)
                    {
                        // Modified assignment (e.g., +:, -:, *:)
                        var opChar = assignToken.Lexeme[0].ToString();
                        var opType = opChar switch
                        {
                            "+" => TokenType.PLUS,
                            "-" => TokenType.MINUS,
                            "*" => TokenType.MULTIPLY,
                            "%" => TokenType.DIVIDE,
                            _ => TokenType.PLUS
                        };
                        // x+: y => x: x + y
                        var currentVal = ASTNode.MakeVariable(variableName);
                        var modifiedValue = ASTNode.MakeBinaryOp(opType, currentVal, right);
                        return ASTNode.MakeAssignment(variableName, modifiedValue);
                    }
                    return ASTNode.MakeAssignment(variableName, right);
                }
                return ASTNode.MakeBinaryOp(TokenType.COLON, left, right);
            }

            // Handle global assignment operator (::)
            if (!IsAtEnd() && CurrentToken().Type == TokenType.GLOBAL_ASSIGNMENT)
            {
                Match(TokenType.GLOBAL_ASSIGNMENT);
                var right = ParseBracketArgument();
                if (right == null)
                {
                    throw new Exception("Expected expression after ::");
                }
                if (left.Type == ASTNodeType.Variable)
                {
                    var variableName = left.Value is SymbolValue sym ? sym.Value : left.Value?.ToString() ?? "";
                    return ASTNode.MakeGlobalAssignment(variableName, right);
                }
                return ASTNode.MakeBinaryOp(TokenType.GLOBAL_ASSIGNMENT, left, right);
            }

            // Check for "apply and assign" pattern: variable+: value
            // This happens when we have: variable operator: value
            // This must be checked BEFORE binary operator parsing to avoid consuming the operator token
            if (!IsAtEnd() && left.Type == ASTNodeType.Variable)
            {
                // Check if we have the pattern: variable+: value
                // Look ahead to see if there's an operator followed by colon
                var nextToken = PeekNext();
                
                if (nextToken != null && IsBinaryOperator(nextToken.Type))
                {
                    // Check if the token after the operator is a colon
                    var tokenAfterOperator = PeekToken(2); // Look two tokens ahead
                    
                    if (tokenAfterOperator != null && tokenAfterOperator.Type == TokenType.COLON)
                    {
                        // This is the "apply and assign" pattern
                        var variableName = left.Value is SymbolValue varSym ? varSym.Value : left.Value?.ToString() ?? "";
                        var operatorToken = nextToken.Type;
                        
                        // Skip the operator and colon
                        Advance(); // Skip the operator token
                        Advance(); // Skip the colon token
                        
                        // Parse the right argument after the colon
                        var rightArgument = ParseBracketArgument();
                        if (rightArgument == null)
                        {
                            throw new Exception("Expected expression after operator in apply and assign");
                        }
                        
                        // Create the apply and assign node
                        return ASTNode.MakeApplyAndAssign(variableName, operatorToken, rightArgument);
                    }
                }
            }

            // Handle binary operators but stop at semicolon or right bracket
            while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.DIVIDE) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) || Match(TokenType.EQUAL) || Match(TokenType.IN) || Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.COLON) || Match(TokenType.HASH) || Match(TokenType.UNDERSCORE) || Match(TokenType.QUESTION) || Match(TokenType.DOLLAR) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION) ||
                   Match(TokenType.LSQ) || Match(TokenType.AND) || Match(TokenType.OR) || Match(TokenType.XOR) || Match(TokenType.ROT) || Match(TokenType.SHIFT) || Match(TokenType.APPLY) || Match(TokenType.DOT_APPLY))
            {
                var op = PreviousToken().Type;
                
                // Check if this is followed by an adverb
                if (VerbRegistry.IsAdverbToken(CurrentToken().Type))
                {
                    var adverbToken = CurrentToken();
                    Advance(); // Consume adverb token
                    var firstAdverbType = adverbToken.Type.ToString().Replace("TokenType.", "");
                    
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
                        "DRAW" => "_draw",
                        "COLON" => ":",
                        "APPLY" => "@",
                        "DOT_APPLY" => ".",
                        "TYPE" => "TYPE",
                        "STRING_REPRESENTATION" => "STRING_REPRESENTATION",
                        "DO" => "do",
                        "WHILE" => "while",
                        "IF_FUNC" => "if",
                        _ => op.ToString()
                    };
                    var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                    
                    // Parse the right side of the adverb
                    var rightSide = ParseBracketArgument();
                    
                    // Check if this is an incomplete expression (should create a projected function)
                    if (rightSide == null)
                    {
                        // Create a projected function for the adverb operation
                        var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                        projectedNode.Value = new SymbolValue(firstAdverbType);
                        
                        // Add the verb as the first child
                        if (verbNode != null) projectedNode.Children.Add(verbNode);
                        
                        // Add the left operand if it exists
                        if (left != null) projectedNode.Children.Add(left);
                        
                        // Mark as needing 1 more argument (for the right side)
                        projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1)));
                        
                        left = projectedNode;
                    }
                    else
                    {
                        // Create the correct adverb structure: ADVERB(verb, left, right)
                        var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        adverbNode.Value = new SymbolValue(firstAdverbType);
                        if (verbNode != null) adverbNode.Children.Add(verbNode);
                        if (left != null) adverbNode.Children.Add(left);
                        if (rightSide != null) adverbNode.Children.Add(rightSide);
                        
                        left = adverbNode;
                    }
                }
                else
                {
                    // Regular binary operation with right-associativity
                    var right = ParseBracketArgument();
                    if (right == null)
                    {
                        throw new Exception($"Expected right operand after {op}");
                    }
                    return left != null ? ASTNode.MakeBinaryOp(op, left, right) : ASTNode.MakeBinaryOp(op, new ASTNode(ASTNodeType.Literal, new NullValue()), right);
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
                    
                    // Parse remaining elements separated by semicolons
                    while (Match(TokenType.SEMICOLON))
                    {
                        // Handle empty position (consecutive semicolons)
                        if (CurrentToken().Type == TokenType.RIGHT_PAREN)
                        {
                            elements.Add(ASTNode.MakeLiteral(new NullValue()));
                        }
                        else
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
                    }
                    
                    if (!Match(TokenType.RIGHT_PAREN))
                    {
                        throw new Exception("Expected ')' after expression");
                    }
                    
                    // Return the vector
                    return ASTNode.MakeVector(elements);
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

        private Token PeekToken(int offset)
        {
            var targetIndex = current + offset;
            if (targetIndex >= tokens.Count) return new Token(TokenType.EOF, "", 0);
            return tokens[targetIndex];
        }

        private bool IsScalar(ASTNode node)
        {
            return node.Type == ASTNodeType.Literal && 
                   (node.Value is IntegerValue || node.Value is LongValue || 
                    node.Value is FloatValue || node.Value is CharacterValue || 
                    node.Value is SymbolValue || node.Value is NullValue);
        }

        private bool IsBinaryOperator(TokenType type)
        {
            return VerbRegistry.IsBinaryOperator(type);
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

            return result;
        }
        
        private ASTNode? ParseExpressionWithoutSemicolons()
        {
            // Check for conditional statements at the start of an expression
            if (!IsAtEnd())
            {
                var currentToken = CurrentToken();
                if (currentToken.Type == TokenType.DO || currentToken.Type == TokenType.IF_FUNC || currentToken.Type == TokenType.WHILE)
                {
                    return ParseConditionalStatement(currentToken.Type);
                }
            }
            
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
                    var opSymbol = VerbRegistry.GetBinaryOperatorSymbol(opToken.Type);
                    left = ASTNode.MakeLiteral(new SymbolValue(opSymbol));
                    Match(opToken.Type); // Consume the operator token
                    
                    if (left != null)
                    {
                        // Handle the bracket notation that follows
                        Match(TokenType.LEFT_BRACKET); // Consume '['

                        // Parse bracket arguments with proper semicolon separation
                        // This ensures operators like < don't consume past semicolons
                        var argsExpression = ParseBracketContentsAsCommaEnlisted();

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
                            // Create a projected function (partial application) instead of throwing error
                            var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
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
                                TokenType.MATCH => "~",
                                TokenType.NEGATE => "~",
                                TokenType.DOLLAR => "$",
                                TokenType.APPLY => "@",
                                TokenType.STRING_REPRESENTATION => "STRING_REPRESENTATION",
                                TokenType.DO => "do",
                                TokenType.WHILE => "while",
                                TokenType.IF_FUNC => "if",
                                _ => opToken.Type.ToString()
                            };
                            projectedNode.Value = new SymbolValue(symbolValue);
                            // Mark this as a unary projected function (needs 1 more argument)
                            projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(1))); // Arity: needs 1 more argument
                            left = projectedNode;
                        }
                        else
                        {
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
                                TokenType.MATCH => "~",
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

            // Handle global assignment operator (::)
            if (!IsAtEnd() && CurrentToken().Type == TokenType.GLOBAL_ASSIGNMENT)
            {
                Match(TokenType.GLOBAL_ASSIGNMENT);
                var right = ParseExpressionWithoutSemicolons();
                if (right == null)
                {
                    throw new Exception("Expected expression after ::");
                }
                // Extract variable name from left side
                if (left.Type == ASTNodeType.Variable)
                {
                    var variableName = left.Value is SymbolValue symbol ? symbol.Value : left.Value?.ToString() ?? "";
                    
                    // Validate variable name - cannot start with underscore
                    if (variableName.StartsWith("_"))
                    {
                        throw new Exception("parse error");
                    }
                    
                    return ASTNode.MakeGlobalAssignment(variableName, right);
                }
                else
                {
                    // Fallback: create binary op for ::
                    return ASTNode.MakeBinaryOp(TokenType.GLOBAL_ASSIGNMENT, left, right);
                }
            }

            // Handle binary operators with Long Right Scope (LRS)
            // In K, there's no precedence among operators - they're all right-associative
            while (Match(TokenType.PLUS) || Match(TokenType.MINUS) || Match(TokenType.MULTIPLY) ||
                   Match(TokenType.DIVIDE) || Match(TokenType.DIV) || Match(TokenType.DOT_PRODUCT) || Match(TokenType.DOT_APPLY) || Match(TokenType.MUL) || Match(TokenType.AND) || Match(TokenType.OR) || Match(TokenType.XOR) || Match(TokenType.ROT) || Match(TokenType.SHIFT) || Match(TokenType.MIN) || Match(TokenType.MAX) || Match(TokenType.LESS) || Match(TokenType.GREATER) ||
                   Match(TokenType.EQUAL) || Match(TokenType.MATCH) || Match(TokenType.IN) || Match(TokenType.BIN) || Match(TokenType.BINL) || Match(TokenType.LIN) ||
                   Match(TokenType.DV) || Match(TokenType.DI) || Match(TokenType.VS) || Match(TokenType.SV) || Match(TokenType.SS) || Match(TokenType.SM) || Match(TokenType.CI) || Match(TokenType.IC) ||
                   Match(TokenType.GETENV) || Match(TokenType.SETENV) || Match(TokenType.SIZE) ||
                   Match(TokenType.POWER) || Match(TokenType.MODULUS) || Match(TokenType.JOIN) ||
                   Match(TokenType.COLON) || Match(TokenType.HASH) || Match(TokenType.UNDERSCORE) || Match(TokenType.QUESTION) ||
                   Match(TokenType.DOLLAR) || Match(TokenType.DRAW) || Match(TokenType.TYPE) || Match(TokenType.STRING_REPRESENTATION) ||
                   Match(TokenType.IO_VERB_0) || Match(TokenType.IO_VERB_1) || Match(TokenType.IO_VERB_2) || Match(TokenType.IO_VERB_3) ||
                   Match(TokenType.IO_VERB_6) || Match(TokenType.IO_VERB_7) || Match(TokenType.IO_VERB_8) || Match(TokenType.IO_VERB_9) ||
                   Match(TokenType.LSQ) || Match(TokenType.APPLY) || Match(TokenType.DOT_APPLY) ||
                   Match(TokenType.DO) || Match(TokenType.WHILE) || Match(TokenType.IF_FUNC))
            {
                var op = PreviousToken().Type;
                
                // Check if this is followed by an adverb (infix adverb)
                if (VerbRegistry.IsAdverbToken(CurrentToken().Type))
                {
                    var adverbToken = CurrentToken();
                    Advance(); // Consume the adverb token
                    var adverbType = adverbToken.Type.ToString().Replace("TokenType.", "");
                    
                    // Check if this adverb is followed by another adverb (adverb nesting)
                    if (VerbRegistry.IsAdverbToken(CurrentToken().Type))
                    {
                        // This is adverb nesting (e.g., ,/:\:)
                        // Create a composite function representing the nested adverbs
                        var secondAdverbToken = CurrentToken();
                        Advance(); // Consume the second adverb token
                        var secondAdverbType = secondAdverbToken.Type.ToString().Replace("TokenType.", "");
                        
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
                            "GETENV" => "_getenv",
                            "SETENV" => "_setenv",
                            "SIZE" => "_size",
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
                            "DOT" => "_dot",
                            _ => PreviousToken().Lexeme
                        };
                        var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(verbName));
                        
                        // For adverb nesting, parse the right side as a vector (multiple terms)
                        // This prevents LRS from interfering with the nested adverbs
                        
                        // Parse a vector by collecting multiple terms until we hit a delimiter or end
                        var rightElements = new List<ASTNode>();
                        while (!IsAtEnd() && CurrentToken().Type != TokenType.SEMICOLON && CurrentToken().Type != TokenType.NEWLINE && CurrentToken().Type != TokenType.RIGHT_PAREN && CurrentToken().Type != TokenType.RIGHT_BRACE && CurrentToken().Type != TokenType.RIGHT_BRACKET)
                        {
                             // Parse individual tokens as literals
                            ASTNode? term = null;
                            var token = CurrentToken();
                            
                            if (token.Type == TokenType.INTEGER)
                            {
                                term = ASTNode.MakeLiteral(new IntegerValue(int.Parse(token.Lexeme)));
                                Advance(); // Consume the token
                            }
                            else if (token.Type == TokenType.LONG)
                            {
                                term = ASTNode.MakeLiteral(new LongValue(long.Parse(token.Lexeme)));
                                Advance(); // Consume the token
                            }
                            else if (token.Type == TokenType.FLOAT)
                            {
                                term = ASTNode.MakeLiteral(new FloatValue(double.Parse(token.Lexeme)));
                                Advance(); // Consume the token
                            }
                            else if (token.Type == TokenType.CHARACTER)
                            {
                                term = ASTNode.MakeLiteral(new CharacterValue(token.Lexeme));
                                Advance(); // Consume token
                            }
                            else if (token.Type == TokenType.CHARACTER_VECTOR)
                            {
                                // Handle string literals - convert to proper K types
                                // K type rules: length 1 = Character (type 3), length 0 or >1 = Character Vector (type -3)
                                if (token.Lexeme.Length == 1)
                                {
                                    // Single character - create CharacterValue (type 3)
                                    term = ASTNode.MakeLiteral(new CharacterValue(token.Lexeme));
                                }
                                else
                                {
                                    // Character vector - create VectorValue containing individual CharacterValue objects (type -3)
                                    var charVector = new List<K3Value>();
                                    foreach (char c in token.Lexeme)
                                    {
                                        charVector.Add(new CharacterValue(c.ToString()));
                                    }
                                    term = ASTNode.MakeLiteral(new VectorValue(charVector, -3)); // -3 = character vector type
                                }
                                Advance(); // Consume token
                            }
                            else if (token.Type == TokenType.LEFT_PAREN)
                            {
                                // Parse nested list structure using existing logic for delimiters
                                term = ParseExpressionInsideDelimiters();
                            }
                            else if (token.Type == TokenType.SYMBOL)
                            {
                                term = ASTNode.MakeLiteral(new SymbolValue(token.Lexeme));
                                Advance(); // Consume token
                            }
                            else
                            {
                                break;
                            }
                            
                            if (term != null)
                            {
                                rightElements.Add(term);
                            }
                        }
                        
                        ASTNode? rightSide = null;
                        if (rightElements.Count == 1)
                        {
                            rightSide = rightElements[0];
                        }
                        else if (rightElements.Count > 1)
                        {
                            rightSide = new ASTNode(ASTNodeType.Vector);
                            rightSide.Children.AddRange(rightElements);
                            // Set the Value property to show vector content
                            rightSide.Value = new SymbolValue($"vector({rightElements.Count})");
                        }
                        else
                        {
                            // No elements found, create empty vector
                            rightSide = new ASTNode(ASTNodeType.Vector);
                        }
                        
                        // Create the correct adverb nesting structure according to K specification
                        // For {x verb y} adverb1 adverb2, the structure should be:
                        // ADVERB2(ADVERB1(verb), x, y) where ADVERB1(verb) creates a composite verb
                        
                        // For nested adverbs like 1 2 3 ,/:\: 4 5 6:
                        // First create ADVERB_SLASH_COLON(,) as a composite verb
                        // Then create ADVERB_BACKSLASH_COLON(ADVERB_SLASH_COLON(,), left, right)
                        
                        // Create the inner adverb structure (composite verb)
                        var innerAdverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        innerAdverbNode.Value = new SymbolValue(adverbType); // First adverb (e.g., ADVERB_SLASH_COLON)
                        innerAdverbNode.Children.Add(verbNode); // The verb (,)
                        
                        // Create the outer adverb structure
                        var nestedAdverbNode = new ASTNode(ASTNodeType.BinaryOp);
                        nestedAdverbNode.Value = new SymbolValue(secondAdverbType); // Second adverb (e.g., ADVERB_BACKSLASH_COLON)
                        nestedAdverbNode.Children.Add(innerAdverbNode); // The composite verb
                        nestedAdverbNode.Children.Add(left); // Left side (1 2 3)
                        nestedAdverbNode.Children.Add(rightSide); // Right side (4 5 6)
                        
                        return nestedAdverbNode;
                    }
                    else
                    {
                        // Single adverb case
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
                            "GETENV" => "_getenv",
                            "SETENV" => "_setenv",
                            "SIZE" => "_size",
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
                            "DOT" => "_dot",
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
                        if (rightSide != null) adverbNode.Children.Add(rightSide);
                        
                        return adverbNode;
                    }
                }
                else
                {
                    // Regular binary operation with Long Right Scope
                    // In K, the right argument is everything to the right (right-associative)
                    var right = ParseExpressionWithoutSemicolons();
                    if (right != null)
                        return ASTNode.MakeBinaryOp(op, left, right);
                    return left;
                }
            }
            
            return left;
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
            if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
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
        
        private ASTNode? ParseVerbWithAdverbsRecursive(ASTNode derivedVerb)
        {
            // Check if this derived verb is followed by another adverb
            if (!IsAtEnd() && VerbRegistry.IsAdverbToken(CurrentToken().Type))
            {
                var adverbToken = CurrentToken();
                Match(adverbToken.Type); // Consume adverb
                var adverbType = adverbToken.Type.ToString().Replace("TokenType.", "");
                
                // Parse the arguments for the adverb
                var arguments = ParseTerm();
                
                // Create the proper adverb structure: ADVERB(verb, null, arguments)
                // Use 0 as initialization to signal "consume first element" for monadic derived verbs
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType);
                if (derivedVerb != null) adverbNode.Children.Add(derivedVerb);
                adverbNode.Children.Add(new ASTNode(ASTNodeType.Literal, new IntegerValue(0))); // Use 0 for monadic derived verbs
                if (arguments != null) adverbNode.Children.Add(arguments);
                
                // Recursively check for more adverbs
                return ParseVerbWithAdverbsRecursive(adverbNode);
            }
            else
            {
                // Not followed by an adverb, return null to indicate regular processing should continue
                return null;
            }
        }

        private bool IsIdentifierToken(TokenType type)
        {
            return type == TokenType.IDENTIFIER || 
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

        /// Get the preferred arity for an operator based on context
        /// </summary>
        private int GetPreferredArity(TokenType type, bool hasLeftOperand)
        {
            // K doesn't use precedence by type - all operators are right-associative
            // Default to dyadic (arity 2) for binary operators
            return 2;
        }

        /// <summary>
        /// Check if an operator should be treated as unary in the current context
        /// </summary>
        private bool ShouldTreatAsUnary(TokenType type, bool hasLeftOperand)
        {
            var preferredArity = GetPreferredArity(type, hasLeftOperand);
            return preferredArity == 1;
        }

        /// <summary>
        /// Convert TokenType to its symbol representation
        /// </summary>
        private string GetTokenSymbol(TokenType type)
        {
            return VerbRegistry.GetBinaryOperatorSymbol(type);
        }
        

        private bool IsAtEnd()
        {
            return current >= tokens.Count || (current < tokens.Count && tokens[current].Type == TokenType.EOF);
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

        /// <summary>
        /// Reconstruct a token's source representation including delimiters that the lexer strips.
        /// CHARACTER and CHARACTER_VECTOR tokens lose their surrounding quotes,
        /// and SYMBOL tokens lose their leading backtick.
        /// </summary>
        private static string ReconstructTokenLexeme(Token token)
        {
            switch (token.Type)
            {
                case TokenType.CHARACTER:
                case TokenType.CHARACTER_VECTOR:
                    // Re-add surrounding double quotes, escaping any embedded quotes or backslashes
                    return "\"" + token.Lexeme.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
                case TokenType.SYMBOL:
                    // Re-add the leading backtick
                    return "`" + token.Lexeme;
                default:
                    return token.Lexeme;
            }
        }

        /// <summary>
        /// Extract implicit parameters from function body tokens.
        /// In K, implicit parameters are single lowercase letters that appear in the function body.
        /// The parameters are extracted in alphabetical order of first appearance.
        /// </summary>
        private List<string> ExtractImplicitParameters(List<Token> bodyTokens)
        {
            var parameters = new List<string>();
            var seenParameters = new HashSet<string>();
            
            foreach (var token in bodyTokens)
            {
                // Look for identifier tokens that are single lowercase letters (a-z)
                if (token.Type == TokenType.IDENTIFIER && 
                    token.Lexeme.Length == 1 && 
                    char.IsLower(token.Lexeme[0]) &&
                    !seenParameters.Contains(token.Lexeme))
                {
                    parameters.Add(token.Lexeme);
                    seenParameters.Add(token.Lexeme);
                }
            }
            
            return parameters;
        }
        
            }
}
