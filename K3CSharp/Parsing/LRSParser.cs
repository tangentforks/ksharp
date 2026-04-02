using System;
using System.Collections.Generic;
using System.Linq;

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
        private readonly LRSDyadicParser dyadicParser;
        private readonly LRSMonadicParser monadicParser;
        private readonly LRSFunctionParser functionParser;
        private readonly LRSStatementParser statementParser;
        private readonly LRSGroupingParser groupingParser;
        
        // Parse tree construction mode flag
        public bool BuildParseTree { get; set; }
        
        // Pure LRS mode flag (enables improved parsing logic when no fallback)
        public bool PureLRSMode { get; set; }
        
        // Defined variables tracker for parser-time variable registration
        // Per spec: allows expressions to be parsed before evaluation by tracking
        // variables defined in assignment operations during AST construction
        private readonly HashSet<string> definedVariables = new HashSet<string>();
        
        /// <summary>
        /// Register a variable as defined during parsing (for assignment targets)
        /// </summary>
        public void RegisterDefinedVariable(string variableName)
        {
            definedVariables.Add(variableName);
            if (ParserConfig.EnableDebugging)
            {
                Console.WriteLine($"[VARIABLE TRACKING] Registered variable: {variableName}");
            }
        }
        
        /// <summary>
        /// Check if a variable is defined (either in evaluator or registered during parsing)
        /// </summary>
        public bool IsVariableDefined(string variableName)
        {
            return definedVariables.Contains(variableName);
        }
        
        /// <summary>
        /// Clear defined variables tracker (called after parsing completes)
        /// </summary>
        public void ClearDefinedVariables()
        {
            definedVariables.Clear();
        }
        
        public LRSParser(List<Token> tokens, bool buildParseTree = false)
        {
            this.tokens = tokens;
            this.BuildParseTree = buildParseTree;
            this.PureLRSMode = false; // Default to Safe LRS mode
            this.expressionParser = new LRSExpressionParser(tokens);
            this.dyadicParser = new LRSDyadicParser(tokens, this);
            this.monadicParser = new LRSMonadicParser(this);
            this.functionParser = new LRSFunctionParser(tokens);
            this.statementParser = new LRSStatementParser(tokens, this);
            this.groupingParser = new LRSGroupingParser(tokens, buildParseTree, this);
        }
        
        /// <summary>
        /// Parse expression using LRS strategy (right-first parsing)
        /// </summary>
        /// <param name="position">Starting position, updated to end of parsed expression</param>
        /// <returns>AST node representing the parsed expression</returns>
        public ASTNode? ParseExpression(ref int position)
        {
            if (PureLRSMode && ParserConfig.EnableDebugging)
            {
                Console.WriteLine($"[PURE LRS DEBUG] ParseExpression called at position {position}/{tokens.Count}");
            }
            
            // Step 1: Read entire expression until separator
            var expressionTokens = expressionParser.ReadExpressionTokens(ref position);
            
            if (expressionTokens.Count == 0)
                return null;
            
            // Check for bracket function call pattern BEFORE delegating to specific parsers
            // Brackets bind left-to-right with stronger binding the closer they are to the base.
            // e.g., c1[`Abs][] => (c1[`Abs])[] and f[1][4] => (f[1])[4]
            // Find the first bracket position (prefix is everything before it)
            {
                int firstBracket = -1;
                for (int i = 1; i < expressionTokens.Count; i++)
                {
                    if ((int)expressionTokens[i].Type == (int)TokenType.LEFT_BRACKET)
                    {
                        firstBracket = i;
                        break;
                    }
                }
                
                if (firstBracket != -1)
                {
                    // Verify the token sequence from firstBracket to end is entirely consecutive bracket groups
                    // i.e., tokens[firstBracket..end] = [arg1][arg2]...[argN]
                    bool allBrackets = true;
                    int pos = firstBracket;
                    var bracketGroups = new List<(int start, int end)>();
                    while (pos < expressionTokens.Count)
                    {
                        if ((int)expressionTokens[pos].Type != (int)TokenType.LEFT_BRACKET)
                        {
                            allBrackets = false;
                            break;
                        }
                        var groupEnd = FindMatchingBracket(expressionTokens, pos);
                        if (groupEnd == -1)
                        {
                            allBrackets = false;
                            break;
                        }
                        bracketGroups.Add((pos, groupEnd));
                        pos = groupEnd + 1;
                    }
                    
                    if (allBrackets && bracketGroups.Count > 0)
                    {
                        // Parse the base (prefix before first bracket)
                        var prefixTokens = expressionTokens.GetRange(0, firstBracket);
                        ASTNode? currentNode;
                        if (BuildParseTree)
                            currentNode = BuildParseTreeFromRight(prefixTokens);
                        else
                            currentNode = EvaluateFromRight(prefixTokens);
                        
                        if (currentNode != null)
                        {
                            // Apply each bracket group left-to-right
                            foreach (var (groupStart, groupEnd) in bracketGroups)
                            {
                                var argTokens = expressionTokens.GetRange(groupStart + 1, groupEnd - groupStart - 1);
                                
                                if (argTokens.Count == 0)
                                {
                                    // niladic call: f[]
                                    currentNode = ASTNode.MakeDyadicOp(TokenType.APPLY, currentNode, ASTNode.MakeVector(new List<ASTNode>()));
                                }
                                else
                                {
                                    // Split arguments by semicolons and parse each one
                                    var splitArgs = SplitBracketArguments(argTokens, int.MaxValue);
                                    var argNodes = new List<ASTNode>();
                                    
                                    foreach (var splitArgTokens in splitArgs)
                                    {
                                        ASTNode? argNode;
                                        if (BuildParseTree)
                                            argNode = BuildParseTreeFromRight(splitArgTokens);
                                        else
                                            argNode = EvaluateFromRight(splitArgTokens);
                                        
                                        if (argNode != null)
                                            argNodes.Add(argNode);
                                    }
                                    
                                    if (argNodes.Count > 0)
                                    {
                                        var argVector = argNodes.Count == 1 ? argNodes[0] : ASTNode.MakeVector(argNodes);
                                        currentNode = ASTNode.MakeDyadicOp(TokenType.APPLY, currentNode, argVector);
                                    }
                                }
                            }
                            return currentNode;
                        }
                    }
                }
            }
                
            // Step 2: Choose strategy based on mode
            ASTNode? result;
            if (BuildParseTree)
                result = BuildParseTreeFromRight(expressionTokens);
            else
                result = EvaluateFromRight(expressionTokens);
                
            if (PureLRSMode && ParserConfig.EnableDebugging)
            {
                Console.WriteLine($"[PURE LRS DEBUG] ParseExpression result: {(result == null ? "NULL" : result.Type.ToString())}");
            }
            
            return result;
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
            
            // Check for grouping constructs (parentheses, braces, brackets)
            var firstTok = expressionTokens[0];
            if (firstTok.Type == TokenType.LEFT_PAREN || firstTok.Type == TokenType.LEFT_BRACE || firstTok.Type == TokenType.LEFT_BRACKET)
            {
                var groupingResult = groupingParser.ParseBrackets(expressionTokens);
                if (groupingResult != null)
                    return groupingResult;
            }
            
            // Check for atomic-only sequences (implicit vector creation)
            if (expressionTokens.Count > 1 && expressionTokens.All(t => LRSAtomicParser.IsAtomicToken(t.Type)))
            {
                return TryCreateImplicitVector(expressionTokens);
            }
            
            // Handle multi-token expressions using dyadic parser FIRST (right-to-left evaluation)
            // This ensures expressions like "_bd ,5" are parsed as: _bd (monadic) applied to (,5 monadic enlist)
            // rather than as a function call _bd(,5)
            if (expressionTokens.Count > 2)
            {
                var dyadicResult = dyadicParser.ParseDyadicOperation(expressionTokens);
                if (dyadicResult != null)
                    return dyadicResult;
            }
            
            // Handle monadic operations (2 tokens: verb + argument)
            if (expressionTokens.Count == 2)
            {
                return monadicParser.ParseMonadicOperator(expressionTokens);
            }
            
            // Try function call parsing only if dyadic/monadic parsing didn't handle it
            // This handles explicit function calls like f[x] or lambda applications
            if (LRSFunctionParser.CouldBeFunction(firstTok.Type))
            {
                var functionResult = functionParser.ParseFunctionCall(expressionTokens);
                if (functionResult != null)
                    return functionResult;
            }
            
            // Fallback to dyadic parser for any remaining cases
            return dyadicParser.ParseDyadicOperation(expressionTokens);
        }
        
        /// <summary>
        /// Parse expression tokens from right to left (LRS strategy)
        /// </summary>
        /// <param name="expressionTokens">Tokens to parse</param>
        /// <returns>AST node representing parsed expression</returns>
        internal ASTNode? EvaluateFromRight(List<Token> expressionTokens)
        {
            if (PureLRSMode && ParserConfig.EnableDebugging)
            {
                Console.WriteLine($"[PURE LRS DEBUG] EvaluateFromRight processing {expressionTokens.Count} tokens: {string.Join(" ", expressionTokens.Select(t => $"{t.Type}({t.Lexeme})"))}");
            }
            
            if (expressionTokens.Count == 0)
            {
                if (PureLRSMode && ParserConfig.EnableDebugging)
                {
                    Console.WriteLine($"[PURE LRS DEBUG] NULL RETURN: EvaluateFromRight called with empty token list");
                }
                return null;
            }
                
            if (expressionTokens.Count == 1)
            {
                if (ParserConfig.EnableDebugging)
                    Console.WriteLine($"[EvaluateFromRight] Single token, returning: {expressionTokens[0].Type}");
                return CreateNodeFromToken(expressionTokens[0]);
            }
            
            // Check for disambiguating colon pattern: verb + colon + adverb
            // Pattern: #:' args (e.g., #:' (1 2;3 4) for count each)
            // This MUST be checked FIRST because other verb-related checks would otherwise intercept
            if (expressionTokens.Count >= 3)
            {
                Console.WriteLine($"[LRS DEBUG] Checking disambiguating pattern: token0={expressionTokens[0].Type}, token1={expressionTokens[1].Type}, token2={expressionTokens[2].Type}");
                Console.WriteLine($"[LRS DEBUG] IsVerbToken(token0)={VerbRegistry.IsVerbToken(expressionTokens[0].Type)}");
                
                if (VerbRegistry.IsVerbToken(expressionTokens[0].Type) &&
                    expressionTokens[1].Type == TokenType.COLON &&
                    (expressionTokens[2].Type == TokenType.ADVERB_SLASH ||
                     expressionTokens[2].Type == TokenType.ADVERB_BACKSLASH ||
                     expressionTokens[2].Type == TokenType.ADVERB_TICK))
                {
                    Console.WriteLine($"[LRS DEBUG] Found disambiguating colon pattern: {expressionTokens[0].Lexeme}:{expressionTokens[2].Lexeme}");
                    return ParseGenericVerbAdverbWithColon(expressionTokens);
                }
            }
            
            // Check for statements first (statements have lower precedence than verbs but higher than separators)
            // This is critical for assignment parsing in Pure LRS mode
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (LRSStatementParser.CouldBeStatement(firstToken.Type))
                {
                    if (PureLRSMode && ParserConfig.EnableDebugging)
                    {
                        Console.WriteLine($"[PURE LRS DEBUG] EvaluateFromRight delegating to statement parser for {firstToken.Type}");
                    }
                    return statementParser.ParseStatement(expressionTokens);
                }
                
                // Also check for assignment operators elsewhere in the expression
                // This handles cases like 'foo:7' where first token is IDENTIFIER, not COLON
                // But DON'T treat as statement if assignment is embedded in larger expression
                // e.g., '1 + a: 42' should parse as '1 + (a: 42)' not as a statement
                for (int i = 1; i < expressionTokens.Count; i++)
                {
                    if (LRSStatementParser.CouldBeStatement(expressionTokens[i].Type))
                    {
                        // Only treat as statement if:
                        // 1. The assignment is at position 1 (e.g., 'a: 42' where a is at 0, : at 1)
                        // 2. The token before assignment is a simple identifier (variable name)
                        // This ensures '1 + a: 42' is NOT treated as a statement
                        bool isSimpleAssignment = i == 1 && expressionTokens[0].Type == TokenType.IDENTIFIER;
                        bool isAssignmentOnly = expressionTokens[i].Type == TokenType.COLON && 
                                                i == expressionTokens.Count - 2 && 
                                                expressionTokens[0].Type == TokenType.IDENTIFIER;
                        
                        if (isSimpleAssignment || isAssignmentOnly)
                    {
                        if (PureLRSMode && ParserConfig.EnableDebugging)
                        {
                                Console.WriteLine($"[PURE LRS DEBUG] EvaluateFromRight found simple assignment at position {i}: {expressionTokens[i].Type}");
                        }
                        return statementParser.ParseStatement(expressionTokens);
                        }
                        // Otherwise, let dyadic parser handle colon as operator (e.g., inline assignment)
                    }
                }
            }
                
            // Check for grouping constructs FIRST (highest precedence per K spec)
            // Only if the ENTIRE expression is wrapped in grouping constructs
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (firstToken.Type == TokenType.LEFT_PAREN || 
                    firstToken.Type == TokenType.LEFT_BRACKET || 
                    firstToken.Type == TokenType.LEFT_BRACE)
                {
                    if (ParserConfig.EnableDebugging)
                        Console.WriteLine($"[EvaluateFromRight] First token is grouping construct: {firstToken.Type}");
                    
                    // Check if the entire expression is wrapped in this grouping construct
                    // CRITICAL FIX: We need to verify that the FIRST opening delimiter
                    // is closed by the LAST closing delimiter, not just any closing delimiter
                    int depth = 0;
                    bool firstOpenClosed = false;
                    TokenType openType = firstToken.Type;
                    TokenType closeType = openType == TokenType.LEFT_PAREN ? TokenType.RIGHT_PAREN :
                                         openType == TokenType.LEFT_BRACKET ? TokenType.RIGHT_BRACKET :
                                         TokenType.RIGHT_BRACE;
                    
                    for (int i = 0; i < expressionTokens.Count; i++)
                    {
                        if (expressionTokens[i].Type == openType) depth++;
                        else if (expressionTokens[i].Type == closeType) depth--;
                        
                        // Track when the first opening delimiter gets closed
                        if (i == 0) depth = 1; // First token is the opening delimiter
                        if (depth == 0 && !firstOpenClosed)
                        {
                            firstOpenClosed = true;
                            // If the first opening closes at the last position, entire expression is wrapped
                            if (i == expressionTokens.Count - 1 && expressionTokens[i].Type == closeType)
                            {
                                if (ParserConfig.EnableDebugging)
                                    Console.WriteLine($"[EvaluateFromRight] Entire expression wrapped in {openType}, delegating to grouping parser");
                                
                                // Delegate to grouping parser for the entire wrapped expression
                                var tempGroupingParser = new LRSGroupingParser(expressionTokens, BuildParseTree, this);
                                int pos = 0;
                                if (openType == TokenType.LEFT_PAREN)
                                    return tempGroupingParser.ParseParentheses(ref pos);
                                else if (openType == TokenType.LEFT_BRACKET)
                                    return tempGroupingParser.ParseBrackets(ref pos);
                                else if (openType == TokenType.LEFT_BRACE)
                                    return tempGroupingParser.ParseBraces(ref pos);
                            }
                            // If first opening closes before the end, expression is NOT fully wrapped
                            break;
                        }
                    }
                    
                    if (ParserConfig.EnableDebugging)
                        Console.WriteLine($"[EvaluateFromRight] Grouping does not wrap entire expression, continuing to dyadic parsing");
                }
                // Period expressions (dictionaries) handled by DOT_APPLY/MAKE operators
                // These will be caught by the dyadic/monadic parsing below
            }
                
            // Check for implicit vector creation (sequences of atomic literals like "1 2 3 4 5")
            // This applies to both Pure and Safe LRS modes
            if (expressionTokens.Count >= 2)
            {
                var implicitVector = TryCreateImplicitVector(expressionTokens);
                if (implicitVector != null)
                    return implicitVector;
            }
                
            // Check for statements first (statements have lower precedence than verbs but higher than separators)
            if (expressionTokens.Count >= 2)
            {
                var firstToken = expressionTokens[0];
                if (LRSStatementParser.CouldBeStatement(firstToken.Type))
                {
                    return statementParser.ParseStatement(expressionTokens);
                }
            }
            
            // Check for disambiguating colon pattern: verb + colon + adverb
            // Pattern: #:' args (e.g., #:' (1 2;3 4) for count each)
            // This MUST be checked BEFORE the system operators check because
            // the verb token check would otherwise dispatch to the function parser
            if (expressionTokens.Count >= 3)
            {
                if (VerbRegistry.IsVerbToken(expressionTokens[0].Type) &&
                    expressionTokens[1].Type == TokenType.COLON &&
                    (expressionTokens[2].Type == TokenType.ADVERB_SLASH ||
                     expressionTokens[2].Type == TokenType.ADVERB_BACKSLASH ||
                     expressionTokens[2].Type == TokenType.ADVERB_TICK))
                {
                    return ParseGenericVerbAdverbWithColon(expressionTokens);
                }
            }
            
            // NOTE: Removed premature verb token check that was delegating to function parser
            // Verbs should be handled naturally through right-to-left dyadic/monadic parsing
            // This allows "_bd ,5" to parse correctly as: _bd (monadic) applied to (,5 monadic enlist)
            
            // Check for assignments (variable:value pattern)
            // Assignments can appear anywhere in the token list, not just at the start
            // BUT only treat as statement if it's a simple assignment (identifier: value)
            // For inline assignments like '1 + a: 42', let dyadic parser handle the colon as part of expression
            for (int i = 1; i < expressionTokens.Count; i++)
            {
                if (expressionTokens[i].Type == TokenType.COLON ||
                    expressionTokens[i].Type == TokenType.ASSIGNMENT ||
                    expressionTokens[i].Type == TokenType.GLOBAL_ASSIGNMENT)
                {
                    // Only treat as statement if it's a simple assignment (identifier: value)
                    // where the identifier is at position i-1 and position 0 is also an identifier
                    // This handles 'a: 42' but not '1 + a: 42'
                    bool isSimpleAssignment = i == 1 && 
                                              expressionTokens[0].Type == TokenType.IDENTIFIER &&
                                              expressionTokens[i].Type == TokenType.COLON;
                    
                    if (isSimpleAssignment)
                    {
                        // Found a simple assignment operator - parse as assignment statement
                    return statementParser.ParseStatement(expressionTokens);
                    }
                    // Otherwise, let dyadic parser handle it (inline assignment in expression)
                }
            }
                
            // Check for multi-arity operations first (triadic, tetradic, variadic)
            var multiAryResult = ParseMultiAryOperation(expressionTokens);
            if (multiAryResult != null)
                return multiAryResult;
                
            // Check for adverb operations
            if (ParseAdverbExpression(expressionTokens) is ASTNode adverbResult)
            {
                return adverbResult;
            }
            
            // Check for bracket indexing pattern: identifier[expression]
            // This is equivalent to identifier @ expression (index at depth)
            if (expressionTokens.Count >= 4 &&
                expressionTokens[0].Type == TokenType.IDENTIFIER &&
                expressionTokens[1].Type == TokenType.LEFT_BRACKET)
            {
                var bracketEnd = FindMatchingBracket(expressionTokens, 1);
                if (bracketEnd != -1 && bracketEnd == expressionTokens.Count - 1)
                {
                    // Valid bracket indexing pattern: d[`keyB] -> d @ `keyB
                    var leftNode = ASTNode.MakeVariable(expressionTokens[0].Lexeme);
                    var indexTokens = expressionTokens.GetRange(2, bracketEnd - 2);
                    var indexNode = BuildParseTreeFromTokens(indexTokens);
                    
                    if (indexNode != null)
                    {
                        return ASTNode.MakeDyadicOp(TokenType.APPLY, leftNode, indexNode);
                    }
                }
            }
            
            // Check for bracket function call pattern: expression[args] (e.g., {lambda}[], func[arg1;arg2])
            // Find the first LEFT_BRACKET that could be a function call
            for (int i = 1; i < expressionTokens.Count; i++)
            {
                if (expressionTokens[i].Type == TokenType.LEFT_BRACKET)
                {
                    if (ParserConfig.EnableDebugging)
                    {
                        Console.WriteLine($"[EvaluateFromRight] Found LEFT_BRACKET at position {i}, checking for function call pattern");
                    }
                    
                    var bracketEnd = FindMatchingBracket(expressionTokens, i);
                    
                    if (ParserConfig.EnableDebugging)
                    {
                        Console.WriteLine($"[EvaluateFromRight] Matching bracket at {bracketEnd}, token count = {expressionTokens.Count}");
                    }
                    
                    if (bracketEnd != -1 && bracketEnd == expressionTokens.Count - 1)
                    {
                        // The bracket is at the end - this is a function call pattern
                        // Parse the left side as the function, bracket content as arguments
                        var funcTokens = expressionTokens.GetRange(0, i);
                        var argTokens = expressionTokens.GetRange(i + 1, bracketEnd - i - 1);
                        
                        if (ParserConfig.EnableDebugging)
                        {
                            Console.WriteLine($"[EvaluateFromRight] Bracket function call: funcTokens={funcTokens.Count}, argTokens={argTokens.Count}");
                        }
                        
                        var funcNode = BuildParseTreeFromTokens(funcTokens);
                        if (funcNode != null)
                        {
                            if (argTokens.Count == 0)
                            {
                                // No arguments: {lambda}[] -> call with empty args
                                return ASTNode.MakeDyadicOp(TokenType.APPLY, funcNode, ASTNode.MakeVector(new List<ASTNode>()));
                            }
                            else
                            {
                                // Parse arguments and create apply operation
                                var argNode = BuildParseTreeFromTokens(argTokens);
                                if (argNode != null)
                                {
                                    return ASTNode.MakeDyadicOp(TokenType.APPLY, funcNode, argNode);
                                }
                            }
                        }
                    }
                    break; // Only check the first bracket
                }
            }
                
            // Try dyadic operation
            var dyadicResult = dyadicParser.ParseDyadicOperation(expressionTokens);
            if (dyadicResult != null)
                return dyadicResult;
                
            // If dyadic parsing fails, try monadic
            var monadicResult = monadicParser.ParseMonadicOperator(expressionTokens);
            
            if (PureLRSMode && ParserConfig.EnableDebugging)
            {
                Console.WriteLine($"[PURE LRS DEBUG] Final monadic parsing result: {(monadicResult == null ? "NULL" : monadicResult.Type.ToString())}");
                if (monadicResult == null)
                {
                    Console.WriteLine($"[PURE LRS DEBUG] NULL RETURN: All parsing strategies failed for {expressionTokens.Count} tokens");
                }
            }

            return monadicResult;
        }
        
        /// <summary>
        /// Parse sub-expression for monadic parser (handles position parameter)
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="position">Reference to position parameter</param>
        /// <returns>AST node representing parsed expression</returns>
        internal ASTNode? ParseSubExpressionForMonadic(List<Token> tokens, ref int position)
        {
            if (tokens.Count == 0) return null;
            if (tokens.Count == 1) return CreateNodeFromToken(tokens[0]);
            
            // Handle parenthesized expressions using grouping parser
            if (tokens[0].Type == TokenType.LEFT_PAREN)
            {
                var tempGroupingParser = new LRSGroupingParser(tokens, BuildParseTree, this);
                return tempGroupingParser.ParseParentheses(ref position);
            }
            
            // Handle bracket expressions using grouping parser
            if (tokens[0].Type == TokenType.LEFT_BRACKET)
            {
                var tempGroupingParser = new LRSGroupingParser(tokens, BuildParseTree, this);
                return tempGroupingParser.ParseBrackets(ref position);
            }
            
            // Handle brace expressions using grouping parser
            if (tokens[0].Type == TokenType.LEFT_BRACE)
            {
                var tempGroupingParser = new LRSGroupingParser(tokens, BuildParseTree, this);
                return tempGroupingParser.ParseBraces(ref position);
            }
            
            // Try dyadic operation first (monadic parsing is handled at main LRS level)
            return dyadicParser.ParseDyadicOperation(tokens);
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
            
            // Default to dyadic for dyadic operators
            if (IsDyadicOperator(opToken.Type))
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
        /// Check if token type is a dyadic operator
        /// </summary>
        private bool IsDyadicOperator(TokenType tokenType)
        {
            return dyadicParser.IsDyadicOperator(tokenType);
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
            // Handle adverb tokens in Pure LRS mode
            if (PureLRSMode && VerbRegistry.IsAdverbToken(token.Type))
            {
                // In Pure LRS mode, try to parse adverb operations
                return ParseAdverbOperation(token);
            }
            
            if (LRSAtomicParser.IsAtomicToken(token.Type))
            {
                return LRSAtomicParser.ParseAtomicToken(token, this);
            }
            
            // Handle operator symbols for parse trees
            return LRSAtomicParser.CreateOperatorNode(token.Type);
        }
        
        /// <summary>
        /// Parse adverb operation in Pure LRS mode
        /// </summary>
        /// <param name="adverbToken">The adverb token</param>
        /// <returns>AST node representing the adverb operation</returns>
        private ASTNode ParseAdverbOperation(Token adverbToken)
        {
            // Find the position of this adverb token in the original token list
            int adverbPosition = -1;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == adverbToken.Type && tokens[i].Lexeme == adverbToken.Lexeme)
                {
                    adverbPosition = i;
                    break;
                }
            }
            
            if (adverbPosition == -1)
            {
                throw new Exception($"Adverb token not found in token list: {adverbToken.Lexeme}");
            }
            
            // Use LRSAdverbParser for proper adverb handling
            var adverbParser = new LRSAdverbParser(tokens, BuildParseTree);
            int position = adverbPosition;
            
            // Try to parse verb-immediate-left adverb pattern (verb on left side only)
            var result = adverbParser.ParseVerbImmediateLeftAdverb(ref position);
            if (result != null)
            {
                return result;
            }
            
            // Fallback: create a simple node for basic adverb handling
            var children = new List<ASTNode>();
            return new ASTNode(ASTNodeType.DyadicOp, new SymbolValue(adverbToken.Lexeme), children);
        }
        
        /// <summary>
        /// Parse adverb expression in Safe LRS mode
        /// </summary>
        /// <param name="expressionTokens">Tokens to parse for adverb operations</param>
        /// <returns>AST node representing the adverb operation, or null if no adverb found</returns>
        private ASTNode? ParseAdverbExpression(List<Token> expressionTokens)
        {
            Console.WriteLine($"[ParseAdverbExpression] Called with {expressionTokens.Count} tokens");
            if (expressionTokens.Count > 0)
            {
                for (int i = 0; i < Math.Min(expressionTokens.Count, 5); i++)
                {
                    Console.WriteLine($"  Token[{i}]: {expressionTokens[i].Type}({expressionTokens[i].Lexeme})");
                }
            }
                        
            // CONSERVATIVE APPROACH: Only handle very specific, simple cases
            // Start with the most basic adverb patterns to avoid breaking the system
            
            // CONSERVATIVE APPROACH: Only handle very specific, simple cases
            // Start with the most basic adverb patterns to avoid breaking the system
            
            // Case 1: Verb + two-glyph adverb patterns
            // Pattern: verb/: arguments, verb\: arguments, etc.
            if (expressionTokens.Count >= 3)
            {
                // Check for verb + two-glyph adverb at the beginning
                if (IsVerbToken(expressionTokens[0].Type) &&
                    (expressionTokens[1].Type == TokenType.ADVERB_SLASH_COLON ||
                     expressionTokens[1].Type == TokenType.ADVERB_BACKSLASH_COLON ||
                     expressionTokens[1].Type == TokenType.ADVERB_TICK_COLON))
                {
                    // Check that remaining tokens are valid arguments
                    bool hasValidArgs = true;
                    for (int i = 2; i < expressionTokens.Count; i++)
                    {
                        if (!LRSAtomicParser.IsAtomicToken(expressionTokens[i].Type) &&
                            expressionTokens[i].Type != TokenType.LEFT_PAREN &&
                            expressionTokens[i].Type != TokenType.LEFT_BRACE &&
                            expressionTokens[i].Type != TokenType.LEFT_BRACKET)
                        {
                            hasValidArgs = false;
                            break;
                        }
                    }
                    
                    if (hasValidArgs)
                    {
                        return ParseVerbTwoGlyphAdverb(expressionTokens);
                    }
                }
            }
            
            // Case 2: Simple two-glyph adverb with simple left and right sides
            // Pattern: (vector) %\: number
            if (expressionTokens.Count >= 4)
            {
                // Check for pattern: (1 2 3) %\: 2
                if (expressionTokens[0].Type == TokenType.LEFT_PAREN &&
                    expressionTokens[2].Type == TokenType.DIVIDE &&
                    expressionTokens[3].Type == TokenType.ADVERB_BACKSLASH_COLON &&
                    expressionTokens[4].Type == TokenType.INTEGER)
                {
                    return ParseSimpleTwoGlyphAdverb(expressionTokens);
                }
            }
            
            // Case 2: Simple single-glyph adverb with system verb
            // Pattern: _ci' number number number
            if (expressionTokens.Count >= 3)
            {
                // Check for pattern: _ci' 97 94 80 (or any number of arguments)
                if (expressionTokens[0].Type == TokenType.CI &&
                    expressionTokens[1].Type == TokenType.ADVERB_TICK)
                {
                    // Check that remaining tokens are arguments (integers, floats, etc.)
                    bool hasValidArgs = true;
                    for (int i = 2; i < expressionTokens.Count; i++)
                    {
                        if (!LRSAtomicParser.IsAtomicToken(expressionTokens[i].Type))
                        {
                            hasValidArgs = false;
                            break;
                        }
                    }
                    
                    if (hasValidArgs)
                    {
                        return ParseSimpleSingleGlyphAdverb(expressionTokens);
                    }
                }
            }
            
            // Case 3a: Disambiguating colon pattern - verb + colon + adverb
            // Pattern: verb:' args (e.g., #:' (1 2;3 4) for count each)
            if (expressionTokens.Count >= 3)
            {
                if (IsVerbToken(expressionTokens[0].Type) &&
                    expressionTokens[1].Type == TokenType.COLON &&
                    (expressionTokens[2].Type == TokenType.ADVERB_SLASH ||
                     expressionTokens[2].Type == TokenType.ADVERB_BACKSLASH ||
                     expressionTokens[2].Type == TokenType.ADVERB_TICK))
                {
                    return ParseGenericVerbAdverbWithColon(expressionTokens);
                }
            }
            
            // Case 3: Generic verb + single-glyph adverb pattern
            // Pattern: verb/ args, verb\ args, verb' args (e.g., -/ 10 2 3 1, +\ 1 2 3, #' (1 2;3 4))
            if (expressionTokens.Count >= 2)
            {
                // Check for verb followed by single-glyph adverb
                if (IsVerbToken(expressionTokens[0].Type) &&
                    (expressionTokens[1].Type == TokenType.ADVERB_SLASH ||
                     expressionTokens[1].Type == TokenType.ADVERB_BACKSLASH ||
                     expressionTokens[1].Type == TokenType.ADVERB_TICK))
                {
                    return ParseGenericVerbAdverb(expressionTokens);
                }
            }
            
            return null; // No simple adverb pattern found
        }
        
        /// <summary>
        /// Check if a token represents a verb (operator or system function)
        /// Uses VerbRegistry to include all verbs, not just hardcoded operators
        /// </summary>
        private bool IsVerbToken(TokenType tokenType)
        {
            // Use VerbRegistry to check all registered verbs (operators + system functions)
            return VerbRegistry.IsVerbToken(tokenType);
        }
        
        /// <summary>
        /// Parse generic verb + single-glyph adverb pattern (verb/ verb\ verb')
        /// Handles common patterns like: -/ 10 2 3 1, +\ 1 2 3, #' (1 2;3 4)
        /// </summary>
        private ASTNode ParseGenericVerbAdverb(List<Token> expressionTokens)
        {
            var verbToken = expressionTokens[0];
            var adverbToken = expressionTokens[1];
            
            // Create verb node
            var verbNode = CreateNodeFromToken(verbToken);
            
            // Parse arguments (everything after the adverb)
            var argTokens = expressionTokens.Skip(2).ToList();
            ASTNode? argNode = null;
            
            if (argTokens.Count > 0)
            {
                // Parse the arguments as a sub-expression
                int argPosition = 0;
                var argParser = new LRSExpressionProcessor(argTokens, BuildParseTree, this);
                argNode = argParser.ProcessExpression(ref argPosition);
                Console.WriteLine($"[LRS DEBUG] ProcessExpression returned: {(argNode == null ? "NULL" : argNode.Type.ToString())}");
                
                // If we couldn.t parse the arguments, try using the LRSParser directly
                // This handles complex expressions like (1 2 3;4 5 6) with semicolons
                if (argNode == null && argTokens.Count > 0)
                {
                    argNode = BuildParseTreeFromTokens(argTokens);
                }
                
                // Final fallback: create a vector from atomic tokens only
                if (argNode == null && argTokens.Count > 0)
                if (argNode == null && argTokens.Count > 0)
                {
                    // Create a vector node from the argument tokens
                    var argNodes = new List<ASTNode>();
                    foreach (var token in argTokens)
                    {
                        if (LRSAtomicParser.IsAtomicToken(token.Type))
                        {
                            argNodes.Add(LRSAtomicParser.ParseAtomicToken(token, this));
                        }
                    }
                    
                    if (argNodes.Count > 0)
                    {
                        argNode = new ASTNode(ASTNodeType.Vector, null, argNodes);
                    }
                }
            }
            
            // Create adverb node: DyadicOp(adverb_symbol, verb, arguments)
            var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
            adverbNode.Value = new SymbolValue(GetAdverbName(adverbToken.Type));
            adverbNode.Children.Add(verbNode);
            
            // Add dummy left argument (0) to match legacy parser AST structure
            adverbNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(0)));
            
            if (argNode != null)
            {
                adverbNode.Children.Add(argNode);
            }
            
            return adverbNode;
        }
        
        /// <summary>
        /// Parse generic verb + colon + single-glyph adverb pattern (verb:' verb:/ verb:\)
        /// Handles disambiguating colon patterns like #:' (count each), +:/ (sum over)
        /// The colon indicates monadic interpretation of the verb
        /// </summary>
        private ASTNode ParseGenericVerbAdverbWithColon(List<Token> expressionTokens)
        {
            var verbToken = expressionTokens[0];
            var colonToken = expressionTokens[1];  // COLON - disambiguating
            var adverbToken = expressionTokens[2];
            
            // Create verb node (the colon indicates monadic interpretation, but we still use the verb as-is)
            var verbNode = CreateNodeFromToken(verbToken);
            
            // Parse arguments (everything after the adverb)
            var argTokens = expressionTokens.Skip(3).ToList(); Console.WriteLine($"[LRS DEBUG] argTokens count={argTokens.Count}, types={string.Join(",", argTokens.Select(t => t.Type))}");
                Console.WriteLine($"[LRS DEBUG] argTokens count={argTokens.Count}, first={argTokens[0].Type}, last={argTokens[argTokens.Count-1].Type}");
            ASTNode? argNode = null;
            
            if (argTokens.Count > 0)
            {
                // Parse the arguments as a sub-expression
                int argPosition = 0;
                var argParser = new LRSExpressionProcessor(argTokens, BuildParseTree, this);
                argNode = argParser.ProcessExpression(ref argPosition);
                Console.WriteLine($"[LRS DEBUG] ProcessExpression returned: {(argNode == null ? "NULL" : argNode.Type.ToString())}");
                
                // If we couldn.t parse the arguments, try using the LRSParser directly
                // This handles complex expressions like (1 2 3;4 5 6) with semicolons
                if (argNode == null && argTokens.Count > 0)
                {
                    argNode = BuildParseTreeFromTokens(argTokens);
                }
                
                // Final fallback: create a vector from atomic tokens only
                if (argNode == null && argTokens.Count > 0)
                {
                    // Create a vector node from the argument tokens
                    var argNodes = new List<ASTNode>();
                    foreach (var token in argTokens)
                    {
                        if (LRSAtomicParser.IsAtomicToken(token.Type))
                        {
                            argNodes.Add(LRSAtomicParser.ParseAtomicToken(token, this));
                        }
                    }
                    
                    if (argNodes.Count > 0)
                    {
                        argNode = new ASTNode(ASTNodeType.Vector, null, argNodes);
                    }
                }
            }
            
            // Create adverb node: DyadicOp(adverb_symbol, verb, arguments)
            var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
            adverbNode.Value = new SymbolValue(GetAdverbName(adverbToken.Type));
            adverbNode.Children.Add(verbNode);
            
            // Add dummy left argument (0) to match legacy parser AST structure
            adverbNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(0)));
            
            if (argNode != null)
            {
                adverbNode.Children.Add(argNode);
            }
            
            return adverbNode;
        }
        
        /// <summary>
        /// Parse verb + two-glyph adverb pattern
        /// </summary>
        /// <param name="expressionTokens">Tokens to parse</param>
        /// <returns>AST node representing the adverb operation</returns>
        private ASTNode ParseVerbTwoGlyphAdverb(List<Token> expressionTokens)
        {
            var verbToken = expressionTokens[0];
            var adverbToken = expressionTokens[1];
            
            // Create verb node
            var verbNode = new ASTNode(ASTNodeType.Literal, new SymbolValue(GetVerbName(verbToken.Type)));
            
            // Parse arguments after the adverb
            var arguments = new List<ASTNode>();
            for (int i = 2; i < expressionTokens.Count; i++)
            {
                var arg = LRSAtomicParser.ParseAtomicToken(expressionTokens[i], this);
                if (arg != null)
                {
                    arguments.Add(arg);
                }
            }
            
            // Create adverb node: ADVERB(verb, 0, args)
            var adverbNode = new ASTNode(ASTNodeType.DyadicOp);
            adverbNode.Value = new SymbolValue(GetAdverbName(adverbToken.Type));
            adverbNode.Children.Add(verbNode);
            adverbNode.Children.Add(new ASTNode(ASTNodeType.Literal, new IntegerValue(0))); // left argument (dummy)
            
            // Add all arguments as the right argument (as a vector)
            if (arguments.Count > 0)
            {
                if (arguments.Count == 1)
                {
                    adverbNode.Children.Add(arguments[0]);
                }
                else
                {
                    var vectorNode = new ASTNode(ASTNodeType.Vector);
                    foreach (var arg in arguments)
                    {
                        vectorNode.Children.Add(arg);
                    }
                    adverbNode.Children.Add(vectorNode);
                }
            }
            else
            {
                // No arguments, add dummy
                adverbNode.Children.Add(new ASTNode(ASTNodeType.Literal, new IntegerValue(0)));
            }
            
            return adverbNode;
        }
        
        /// <summary>
        /// Get verb name from token type
        /// </summary>
        private string GetVerbName(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.PLUS => "+",
                TokenType.MINUS => "-",
                TokenType.MULTIPLY => "*",
                TokenType.DIVIDE => "%",
                TokenType.DOT_PRODUCT => "_dot",
                _ => tokenType.ToString()
            };
        }
        
        /// <summary>
        /// Get adverb name from token type
        /// </summary>
        private string GetAdverbName(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ADVERB_SLASH_COLON => "ADVERB_SLASH_COLON",
                TokenType.ADVERB_BACKSLASH_COLON => "ADVERB_BACKSLASH_COLON",
                TokenType.ADVERB_TICK_COLON => "ADVERB_TICK_COLON",
                _ => tokenType.ToString()
            };
        }
        
        /// <summary>
        /// Parse simple two-glyph adverb (conservative approach)
        /// </summary>
        /// <param name="expressionTokens">Tokens to parse</param>
        /// <returns>AST node representing the adverb operation</returns>
        private ASTNode ParseSimpleTwoGlyphAdverb(List<Token> expressionTokens)
        {
            // Create a simple placeholder node that represents %\: operation
            // The actual evaluation will be handled by the existing evaluator
            var children = new List<ASTNode>();
            
            // Left side: (1 2 3) - create a simple vector node
            var vectorElements = new List<ASTNode>();
            for (int i = 1; i < expressionTokens.Count - 2; i++)
            {
                if (expressionTokens[i].Type == TokenType.INTEGER)
                {
                    vectorElements.Add(ASTNode.MakeLiteral(new IntegerValue(int.Parse(expressionTokens[i].Lexeme))));
                }
            }
            var leftNode = ASTNode.MakeVector(vectorElements);
            
            // Right side: 2
            var rightNode = ASTNode.MakeLiteral(new IntegerValue(int.Parse(expressionTokens[4].Lexeme)));
            
            children.Add(leftNode);
            children.Add(rightNode);
            
            // Create node with %\: symbol
            return new ASTNode(ASTNodeType.DyadicOp, new SymbolValue("%\\:"), children);
        }
        
        /// <summary>
        /// Parse simple single-glyph adverb (conservative approach)
        /// </summary>
        /// <param name="expressionTokens">Tokens to parse</param>
        /// <returns>AST node for the adverb operation</returns>
        private ASTNode ParseSimpleSingleGlyphAdverb(List<Token> expressionTokens)
        {
            // Create a DyadicOp node with the ' adverb
            // The evaluator will handle this as a special case for adverb evaluation
            var children = new List<ASTNode>();
            
            // Verb: _ci
            var verbNode = ASTNode.MakeLiteral(new SymbolValue("_ci"));
            children.Add(verbNode);
            
            // Add dummy left argument (0) to match legacy parser AST structure
            children.Add(ASTNode.MakeLiteral(new IntegerValue(0)));
            
            // Arguments: 97 94 80
            for (int i = 2; i < expressionTokens.Count; i++)
            {
                if (expressionTokens[i].Type == TokenType.INTEGER)
                {
                    children.Add(ASTNode.MakeLiteral(new IntegerValue(int.Parse(expressionTokens[i].Lexeme))));
                }
            }
            
            // Create DyadicOp with ' symbol - evaluator will handle this specially
            return new ASTNode(ASTNodeType.DyadicOp, new SymbolValue("'"), children);
        }
        
        /// <summary>
        /// Try to create an implicit vector from a sequence of atomic literals
        /// Returns null if tokens don't form a valid implicit vector
        /// </summary>
        private ASTNode? TryCreateImplicitVector(List<Token> tokens)
        {
            if (tokens.Count < 2)
                return null;
                
            // Check if all tokens are atomic literals and collect them
            var elements = new List<ASTNode>();
            
            foreach (var token in tokens)
            {
                // Check if token is an atomic literal
                if (!LRSAtomicParser.IsAtomicToken(token.Type))
                    return null; // Not all atomic - can't be implicit vector
                
                // Parse the token and add to elements
                var node = LRSAtomicParser.ParseAtomicToken(token, this);
                if (node == null)
                    return null;
                    
                elements.Add(node);
            }
            
            // Create vector for all implicit collections
            // K semantics: space-separated literals create vectors (homogeneous or mixed)
            // The evaluator will determine the proper K3Value type based on element types
            return ASTNode.MakeVector(elements);
        }
        
        /// <summary>
        /// Get the statement parser for handling assignment statements in sub-expressions
        /// </summary>
        /// <returns>The LRS statement parser instance</returns>
        public LRSStatementParser? GetStatementParser()
        {
            return statementParser;
        }
    }
}
