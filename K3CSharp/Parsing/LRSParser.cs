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
        private readonly LRSStatementParser statementParser;
        
        // Parse tree construction mode flag
        public bool BuildParseTree { get; set; }
        
        public LRSParser(List<Token> tokens, bool buildParseTree = false)
        {
            this.tokens = tokens;
            this.expressionParser = new LRSExpressionParser(tokens);
            this.binaryParser = new LRSBinaryParser(tokens, this);
            this.unaryParser = new LRSUnaryParser(this);
            this.functionParser = new LRSFunctionParser(tokens);
            this.statementParser = new LRSStatementParser(tokens, this);
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
            
            // Check for statements first (statements have lower precedence than verbs but higher than separators)
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (LRSStatementParser.CouldBeStatement(firstToken.Type))
                {
                    return statementParser.ParseStatement(expressionTokens);
                }
            }
            
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
            
            // Check for multi-arity operations first (triadic, tetradic, variadic)
            var multiAryResult = ParseMultiAryOperation(expressionTokens);
            if (multiAryResult != null)
                return multiAryResult;
            
            // Handle binary operations
            var binaryResult = binaryParser.ParseBinaryOperation(expressionTokens);
            if (binaryResult != null)
                return binaryResult;
                
            // If binary parsing fails, try monadic
            return unaryParser.ParseMonadicOperator(expressionTokens);
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
                
            // Check for statements first (statements have lower precedence than verbs but higher than separators)
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (LRSStatementParser.CouldBeStatement(firstToken.Type))
                {
                    return statementParser.ParseStatement(expressionTokens);
                }
            }
                
            // Check for multi-arity operations first (triadic, tetradic, variadic)
            var multiAryResult = ParseMultiAryOperation(expressionTokens);
            if (multiAryResult != null)
                return multiAryResult;
                
            // Try binary operation
            var binaryResult = binaryParser.ParseBinaryOperation(expressionTokens);
            if (binaryResult != null)
                return binaryResult;
                
            // If binary parsing fails, try monadic
            return unaryParser.ParseMonadicOperator(expressionTokens);
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
        /// Parse multi-arity operations (triadic, tetradic, variadic)
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <returns>AST node for multi-arity operation</returns>
        private ASTNode? ParseMultiAryOperation(List<Token> tokens)
        {
            if (tokens.Count < 2) return null;
            
            // Look for multi-arity operators
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                int arity = DetectOperationArity(tokens, i);
                
                if (arity >= 3)
                {
                    // Handle triadic operations
                    if (arity == 3)
                    {
                        return ParseTriadicOperation(tokens, i);
                    }
                    // Handle tetradic operations
                    else if (arity == 4)
                    {
                        return ParseTetradicOperation(tokens, i);
                    }
                }
                
                // Handle variadic adverbs
                if (token.Type == TokenType.VARIADIC_ADVERB_OVER ||
                    token.Type == TokenType.VARIADIC_ADVERB_SCAN ||
                    token.Type == TokenType.VARIADIC_ADVERB_EACH)
                {
                    return ParseVariadicAdverb(tokens, i);
                }
            }
            
            return null;
        }
        
        /// <summary>
        /// Detect operation arity based on token structure
        /// </summary>
        /// <param name="tokens">Tokens to analyze</param>
        /// <param name="position">Starting position</param>
        /// <returns>Detected arity (1=monadic, 2=dyadic, 3=triadic, 4=tetradic)</returns>
        private int DetectOperationArity(List<Token> tokens, int position)
        {
            if (position >= tokens.Count) return 1;
            
            var opToken = tokens[position];
            
            // Check for triadic/tetradic patterns - ONLY supported with brackets
            if (opToken.Type == TokenType.DOT_APPLY || opToken.Type == TokenType.APPLY)
            {
                // Look for bracket-based triadic/tetradic patterns
                // Format: .[arg1;arg2;arg3] or @[arg1;arg2;arg3;arg4]
                if (position + 1 < tokens.Count && tokens[position + 1].Type == TokenType.LEFT_BRACKET)
                {
                    // Count arguments inside brackets
                    int argCount = CountBracketArguments(tokens, position + 1);
                    if (argCount >= 3)
                        return Math.Min(argCount, 4); // Cap at tetradic
                }
            }
            
            if (opToken.Type == TokenType.QUESTION)
            {
                // Check for function inverse pattern
                if (position + 2 < tokens.Count && tokens[position + 1].Type == TokenType.COLON)
                {
                    return 3; // Triadic inverse function
                }
            }
            
            // Default to dyadic for binary operators
            if (IsBinaryOperator(opToken.Type))
                return 2;
            
            // Default to monadic
            return 1;
        }
        
        /// <summary>
        /// Count arguments inside brackets for triadic/tetradic operations
        /// </summary>
        /// <param name="tokens">Token list</param>
        /// <param name="bracketPos">Position of LEFT_BRACKET</param>
        /// <returns>Number of arguments found</returns>
        private int CountBracketArguments(List<Token> tokens, int bracketPos)
        {
            if (bracketPos >= tokens.Count || tokens[bracketPos].Type != TokenType.LEFT_BRACKET)
                return 0;
                
            int count = 0;
            int depth = 1;
            int i = bracketPos + 1;
            bool inArgument = false;
            
            while (i < tokens.Count && depth > 0)
            {
                var token = tokens[i];
                
                if (token.Type == TokenType.LEFT_BRACKET)
                {
                    depth++;
                    inArgument = true;
                }
                else if (token.Type == TokenType.RIGHT_BRACKET)
                {
                    depth--;
                    if (depth == 1 && inArgument)
                    {
                        count++;
                        inArgument = false;
                    }
                }
                else if (depth == 1)
                {
                    if (token.Type == TokenType.SEMICOLON)
                    {
                        if (inArgument)
                        {
                            count++;
                            inArgument = false;
                        }
                    }
                    else if (token.Type != TokenType.NEWLINE && 
                            token.Type != TokenType.LEFT_BRACKET &&
                            token.Type != TokenType.RIGHT_BRACKET)
                    {
                        inArgument = true;
                    }
                }
                
                i++;
            }
            
            // Count the last argument if we were in one
            if (inArgument && depth == 0)
                count++;
                
            return count;
        }
        
        /// <summary>
        /// Check if token type is a binary operator
        /// </summary>
        private bool IsBinaryOperator(TokenType tokenType)
        {
            return binaryParser.IsBinaryOperator(tokenType);
        }
        
        /// <summary>
        /// Parse triadic operation (3 arguments) - ONLY bracket-based
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="position">Starting position</param>
        /// <returns>AST node for triadic operation</returns>
        private ASTNode? ParseTriadicOperation(List<Token> tokens, int position)
        {
            if (position >= tokens.Count) return null;
            
            var opToken = tokens[position];
            
            // Must be followed by LEFT_BRACKET for triadic operations
            if (position + 1 >= tokens.Count || tokens[position + 1].Type != TokenType.LEFT_BRACKET)
                return null;
                
            var children = new List<ASTNode>();
            
            // Parse arguments inside brackets: .[arg1;arg2;arg3]
            var bracketEnd = FindMatchingBracket(tokens, position + 1);
            if (bracketEnd == -1) return null;
            
            var bracketTokens = tokens.GetRange(position + 2, bracketEnd - position - 2); // Skip . and [
            var splitArgs = SplitBracketArguments(bracketTokens, 3);
            
            if (splitArgs.Count >= 3)
            {
                foreach (var argTokens in splitArgs.Take(3))
                {
                    var argNode = BuildParseTreeFromTokens(argTokens);
                    if (argNode != null) children.Add(argNode);
                }
                
                return new ASTNode(ASTNodeType.TriadicOp, new SymbolValue(opToken.Lexeme), children);
            }
            
            return null;
        }
        
        /// <summary>
        /// Parse tetradic operation (4 arguments) - ONLY bracket-based
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="position">Starting position</param>
        /// <returns>AST node for tetradic operation</returns>
        private ASTNode? ParseTetradicOperation(List<Token> tokens, int position)
        {
            if (position >= tokens.Count) return null;
            
            var opToken = tokens[position];
            
            // Must be followed by LEFT_BRACKET for tetradic operations
            if (position + 1 >= tokens.Count || tokens[position + 1].Type != TokenType.LEFT_BRACKET)
                return null;
                
            var children = new List<ASTNode>();
            
            // Parse arguments inside brackets: @[arg1;arg2;arg3;arg4]
            var bracketEnd = FindMatchingBracket(tokens, position + 1);
            if (bracketEnd == -1) return null;
            
            var bracketTokens = tokens.GetRange(position + 2, bracketEnd - position - 2); // Skip @ and [
            var splitArgs = SplitBracketArguments(bracketTokens, 4);
            
            if (splitArgs.Count >= 4)
            {
                foreach (var argTokens in splitArgs.Take(4))
                {
                    var argNode = BuildParseTreeFromTokens(argTokens);
                    if (argNode != null) children.Add(argNode);
                }
                
                return new ASTNode(ASTNodeType.TetradicOp, new SymbolValue(opToken.Lexeme), children);
            }
            
            return null;
        }
        
        /// <summary>
        /// Find matching closing bracket
        /// </summary>
        private int FindMatchingBracket(List<Token> tokens, int leftBracketPos)
        {
            if (leftBracketPos >= tokens.Count || tokens[leftBracketPos].Type != TokenType.LEFT_BRACKET)
                return -1;
                
            int depth = 1;
            for (int i = leftBracketPos + 1; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.LEFT_BRACKET)
                    depth++;
                else if (tokens[i].Type == TokenType.RIGHT_BRACKET)
                    depth--;
                    
                if (depth == 0)
                    return i;
            }
            
            return -1;
        }
        
        /// <summary>
        /// Split bracket arguments by semicolons
        /// </summary>
        private List<List<Token>> SplitBracketArguments(List<Token> tokens, int expectedArgs)
        {
            var args = new List<List<Token>>();
            var currentArg = new List<Token>();
            
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                
                if (token.Type == TokenType.SEMICOLON)
                {
                    if (currentArg.Count > 0)
                    {
                        args.Add(currentArg);
                        currentArg = new List<Token>();
                    }
                }
                else
                {
                    currentArg.Add(token);
                }
            }
            
            // Add the last argument
            if (currentArg.Count > 0)
                args.Add(currentArg);
                
            return args;
        }
        
        /// <summary>
        /// Parse variadic adverb operation
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="position">Starting position</param>
        /// <returns>AST node for variadic adverb operation</returns>
        private ASTNode? ParseVariadicAdverb(List<Token> tokens, int position)
        {
            if (position >= tokens.Count) return null;
            
            var adverbToken = tokens[position];
            string adverbName = "";
            
            // Determine adverb type
            switch (adverbToken.Type)
            {
                case TokenType.VARIADIC_ADVERB_OVER:
                    adverbName = "over";
                    break;
                case TokenType.VARIADIC_ADVERB_SCAN:
                    adverbName = "scan";
                    break;
                case TokenType.VARIADIC_ADVERB_EACH:
                    adverbName = "each";
                    break;
                default:
                    return null;
            }
            
            // Return "not yet implemented" node
            var message = $"generalized {adverbName} is not yet implemented";
            return new ASTNode(ASTNodeType.NotImplemented, new CharacterValue(message), null);
        }
        
        /// <summary>
        /// Helper method to recursively build parse tree from tokens
        /// </summary>
        private ASTNode? BuildParseTreeFromTokens(List<Token> tokens)
        {
            var tempParser = new LRSParser(tokens, true);
            return tempParser.BuildParseTreeFromRight(tokens);
        }
        
        /// <summary>
        /// Create AST node from token using LRSAtomicParser
        /// </summary>
        private ASTNode CreateNodeFromToken(Token token)
        {
            if (LRSAtomicParser.IsAtomicToken(token.Type))
            {
                return LRSAtomicParser.ParseAtomicToken(token);
            }
            
            // Handle operator symbols for parse trees
            return LRSAtomicParser.CreateOperatorNode(token.Type);
        }
    }
}
