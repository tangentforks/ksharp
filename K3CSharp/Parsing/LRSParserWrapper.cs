using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// LRS Parser Wrapper - Drop-in replacement for Parser class
    /// Provides gradual migration path with fallback to legacy parser
    /// </summary>
    public class LRSParserWrapper
    {
        private readonly LRSParser? lrsParser;
        private readonly List<Token> tokens;
        private readonly string sourceText;
        private readonly bool enableFallback;
        private readonly bool useLRSParser;
        
        // Static failure analyzer for tracking all LRS failures
        private static readonly object failureAnalyzerLock = new object();
        private static readonly List<ParserFailureRecord> failureRecords = new();
        private static string currentTestName = "";
        
        /// <summary>
        /// Set the current test name for failure tracking
        /// </summary>
        public static void SetCurrentTestName(string testName)
        {
            currentTestName = testName ?? "";
        }
        
        /// <summary>
        /// Clear the current test name
        /// </summary>
        public static void ClearCurrentTestName()
        {
            currentTestName = "";
        }
        
        public LRSParserWrapper(List<Token> tokens, string sourceText, bool enableFallback = true, bool useLRSParser = true)
        {
            // Preprocess tokens to combine K-tree dotted notation (.k.d -> single IDENTIFIER token)
            var processedTokens = PreprocessDottedPaths(tokens, sourceText);
            this.tokens = processedTokens;
            this.sourceText = sourceText;
            this.enableFallback = enableFallback;
            this.useLRSParser = useLRSParser;
            
            if (useLRSParser)
            {
                this.lrsParser = new LRSParser(processedTokens);
                // Enable Pure LRS mode when fallback is disabled
                if (this.lrsParser != null)
                {
                    this.lrsParser.PureLRSMode = !enableFallback;
                }
            }
        }
        
        /// <summary>
        /// Preprocess tokens to combine adjacent DOT_APPLY + IDENTIFIER sequences
        /// into single IDENTIFIER tokens with dotted notation (e.g., .k.d becomes a single Variable ".k.d")
        /// Only applies when the DOT_APPLY and IDENTIFIER are adjacent (no space between them).
        /// </summary>
        private static List<Token> PreprocessDottedPaths(List<Token> tokens, string sourceText)
        {
            var result = new List<Token>();
            int i = 0;
            
            while (i < tokens.Count)
            {
                // Check for DOT_APPLY followed by adjacent IDENTIFIER pattern
                if (tokens[i].Type == TokenType.DOT_APPLY && 
                    i + 1 < tokens.Count &&
                    (tokens[i + 1].Type == TokenType.IDENTIFIER || tokens[i + 1].Type == TokenType.SYMBOL))
                {
                    // Check adjacency: dot position + 1 == next token position
                    bool isAdjacent = (tokens[i].Position + tokens[i].Lexeme.Length) == tokens[i + 1].Position;
                    
                    if (isAdjacent)
                    {
                        // Build the dotted path
                        var dottedPath = "." + tokens[i + 1].Lexeme;
                        int startPos = tokens[i].Position;
                        int j = i + 2;
                        
                        // Continue consuming .identifier sequences
                        while (j + 1 < tokens.Count && 
                               tokens[j].Type == TokenType.DOT_APPLY &&
                               (tokens[j + 1].Type == TokenType.IDENTIFIER || tokens[j + 1].Type == TokenType.SYMBOL) &&
                               (tokens[j].Position + tokens[j].Lexeme.Length) == tokens[j + 1].Position)
                        {
                            dottedPath += "." + tokens[j + 1].Lexeme;
                            j += 2;
                        }
                        
                        // Create a single IDENTIFIER token with the full dotted path
                        result.Add(new Token(TokenType.IDENTIFIER, dottedPath, startPos));
                        i = j;
                        continue;
                    }
                }
                
                result.Add(tokens[i]);
                i++;
            }
            
            return result;
        }
        
        /// <summary>
        /// Check if expression should use legacy parser based on complexity patterns
        /// </summary>
        private bool ShouldFallbackToLegacy(string sourceText, List<Token> tokens)
        {
            // Pure LRS mode: no fallback to legacy parser
            return false;
        }

        /// <summary>
        /// Parse using LRS strategy with fallback to legacy parser
        /// </summary>
        public ASTNode? Parse()
        {
            if (!useLRSParser)
            {
                // Direct to legacy parser
                return new Parser(tokens, sourceText).Parse();
            }
            
            // Check if we should fallback to legacy based on complexity patterns
            if (ShouldFallbackToLegacy(sourceText, tokens))
            {
                return FallbackToLegacyParser();
            }
            
            try
            {
                // Debug: check which path we'll take
                if (sourceText.Contains("d:."))
                {
                    Console.WriteLine($"[Parse] ENTER: enableFallback={enableFallback}, source='{sourceText.Substring(0, Math.Min(30, sourceText.Length))}'");
                }
                
                // Pure LRS mode: Handle multiple semicolon-separated expressions
                if (!enableFallback)
                {
                    if (sourceText.Contains("d:."))
                        Console.WriteLine("[Parse] Taking Pure LRS path -> ParseMultipleExpressions");
                    return ParseMultipleExpressions();
                }
                
                // Safe LRS mode: Handle multiple expressions with fallback
                if (sourceText.Contains("d:."))
                    Console.WriteLine("[Parse] Taking Safe LRS path");
                var position = 0;
                ASTNode? result = null;
                var expressions = new List<ASTNode>();
                
                // Check if this contains multiple TOP-LEVEL expressions (semicolons not inside parentheses/brackets/braces)
                // Semicolons inside grouping constructs are handled by the grouping parser, not as expression separators
                bool hasMultipleExpressions = false;
                int nestingLevel = 0;
                foreach (var token in tokens)
                {
                    if (token.Type == TokenType.LEFT_PAREN || token.Type == TokenType.LEFT_BRACKET || token.Type == TokenType.LEFT_BRACE)
                    {
                        nestingLevel++;
                    }
                    else if (token.Type == TokenType.RIGHT_PAREN || token.Type == TokenType.RIGHT_BRACKET || token.Type == TokenType.RIGHT_BRACE)
                    {
                        nestingLevel--;
                    }
                    else if (token.Type == TokenType.SEMICOLON && nestingLevel == 0)
                    {
                        // Semicolon at top level indicates multiple expressions
                        hasMultipleExpressions = true;
                        break;
                    }
                }
                
                if (hasMultipleExpressions)
                {
                    // Parse multiple expressions for semicolon-separated statements
                    while (position < tokens.Count)
                    {
                        // Skip whitespace and separators
                        while (position < tokens.Count && 
                               (tokens[position].Type == TokenType.NEWLINE || 
                                tokens[position].Type == TokenType.SEMICOLON))
                        {
                            if (tokens[position].Type == TokenType.SEMICOLON)
                            {
                                // Add null for empty expression between semicolons
                                expressions.Add(ASTNode.MakeLiteral(new NullValue()));
                            }
                            position++;
                        }
                        
                        if (position >= tokens.Count) break;
                        
                        // Parse single expression
                        if (sourceText.Contains("d:."))
                        {
                            Console.WriteLine($"[SafeLRS] Parsing expression at position {position}, token={tokens[position].Type}({tokens[position].Lexeme})");
                        }
                        var exprResult = lrsParser?.ParseExpression(ref position);
                        if (sourceText.Contains("d:."))
                        {
                            Console.WriteLine($"[SafeLRS] ParseExpression returned: {exprResult?.Type} (position now {position})");
                        }
                        if (exprResult != null)
                        {
                            expressions.Add(exprResult);
                        }
                        else
                        {
                            // Failed to parse expression, break out
                            if (sourceText.Contains("d:."))
                            {
                                Console.WriteLine($"[SafeLRS] ParseExpression returned NULL, breaking");
                            }
                            break;
                        }
                    }
                    
                    // Create result following K semantics: wrap multiple expressions in Block node
                    // for sequential evaluation. The evaluator will execute each expression
                    // in order and return the last value.
                    if (sourceText.Contains("d:."))
                    {
                        Console.WriteLine($"[SafeLRS] expressions.Count = {expressions.Count}");
                    }
                    if (expressions.Count == 1)
                    {
                        result = expressions[0]; // Single expression - return directly
                    }
                    else if (expressions.Count > 1)
                    {
                        // Multiple expressions: wrap in Block node for sequential evaluation
                        if (sourceText.Contains("d:."))
                            Console.WriteLine("[SafeLRS] Creating Block node for multiple expressions");
                        var blockNode = new ASTNode(ASTNodeType.Block);
                        foreach (var expr in expressions)
                        {
                            blockNode.Children.Add(expr);
                        }
                        result = blockNode;
                    }
                }
                else
                {
                    // Single expression parsing
                    result = lrsParser?.ParseExpression(ref position);
                }
                
                // Record failure if LRS parsing failed
                if (result == null)
                {
                    var lastTokenType = position > 0 && position <= tokens.Count 
                        ? tokens[position - 1].Type 
                        : (TokenType?)null;
                    
                    // Record failure for later analysis
                    lock (failureAnalyzerLock)
                    {
                        var record = new ParserFailureRecord
                        {
                            Timestamp = DateTime.Now,
                            TestName = currentTestName,
                            SourceText = sourceText,
                            ConsumedTokens = position,
                            TotalTokens = tokens.Count,
                            LastTokenType = lastTokenType,
                            FailurePoint = DetermineFailurePoint(position, tokens.Count, lastTokenType)
                        };
                        failureRecords.Add(record);
                        
                        // Limit records to prevent memory issues
                        if (failureRecords.Count > 10000)
                        {
                            failureRecords.RemoveAt(0);
                        }
                    }
                }
                
                // Validate result
                // Check if remaining tokens are just whitespace/newlines
                bool onlyWhitespaceRemaining = true;
                if (position < tokens.Count)
                {
                    for (int i = position; i < tokens.Count; i++)
                    {
                        if (tokens[i].Type != TokenType.NEWLINE && tokens[i].Type != TokenType.EOF)
                        {
                            onlyWhitespaceRemaining = false;
                            break;
                        }
                    }
                }
                
                if (result != null && (position >= tokens.Count || onlyWhitespaceRemaining))
                {
                    return result;
                }
                
                // Debug logging for Pure LRS mode failures
                if (!enableFallback && ParserConfig.EnableDebugging)
                {
                    Console.WriteLine($"[DEBUG] Pure LRS parsing incomplete:");
                    Console.WriteLine($"  Source: {sourceText}");
                    Console.WriteLine($"  Result: {(result != null ? "NOT NULL" : "NULL")}");
                    Console.WriteLine($"  Position: {position}/{tokens.Count}");
                    if (position < tokens.Count)
                    {
                        Console.WriteLine($"  Remaining tokens: {string.Join(" ", tokens.Skip(position).Select(t => t.Type))}");
                    }
                }
                
                // If LRS parsing didn't consume all tokens, fall back
                if (enableFallback)
                {
                    // Debug for ktree tests
                    if (sourceText.Contains("d:."))
                    {
                        Console.WriteLine($"[SafeLRS] Checking fallback: position={position}, tokens.Count={tokens.Count}, result={result?.Type}");
                    }
                    
                    // Track incomplete consumption cases separately
                    if (result != null && position < tokens.Count)
                    {
                        lock (failureAnalyzerLock)
                        {
                            var record = new ParserFailureRecord
                            {
                                Timestamp = DateTime.Now,
                                TestName = currentTestName,
                                SourceText = sourceText,
                                ConsumedTokens = position,
                                TotalTokens = tokens.Count,
                                LastTokenType = position < tokens.Count ? tokens[position].Type : null,
                                FailurePoint = $"Incomplete consumption (position {position}/{tokens.Count})"
                            };
                            failureRecords.Add(record);
                            
                            if (failureRecords.Count > 10000)
                            {
                                failureRecords.RemoveAt(0);
                            }
                        }
                    }
                    return FallbackToLegacyParser();
                }
                
                throw new Exception($"LRS parser failed to parse complete expression: {sourceText}");
            }
            catch (Exception ex) when (enableFallback)
            {
                // Fallback to legacy parser on any error
                Console.WriteLine($"[DEBUG] LRS parser failed with error: {ex.Message}");
                Console.WriteLine($"[DEBUG] Stack trace: {ex.StackTrace}");
                return FallbackToLegacyParser();
            }
        }
        
        /// <summary>
        /// Fallback to legacy parser
        /// </summary>
        private ASTNode? FallbackToLegacyParser()
        {
            try
            {
                var legacyParser = new Parser(tokens, sourceText);
                return legacyParser.Parse();
            }
            catch (Exception ex)
            {
                throw new Exception($"Both LRS and legacy parsers failed for: {sourceText}. Legacy error: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Check if expression is incomplete (matches Parser interface)
        /// </summary>
        public bool IsIncompleteExpression()
        {
            if (!useLRSParser)
            {
                // Delegate to legacy parser
                return new Parser(tokens, sourceText).IsIncompleteExpression();
            }
            
            try
            {
                var position = 0;
                var result = lrsParser?.ParseExpression(ref position);
                
                // If LRS parsing failed, fall back to legacy parser
                if (result == null)
                {
                    return new Parser(tokens, sourceText).IsIncompleteExpression();
                }
                
                // Expression is incomplete if we didn't consume all tokens
                return position < tokens.Count;
            }
            catch when (enableFallback)
            {
                // Fallback to legacy parser for incomplete expression detection
                return new Parser(tokens, sourceText).IsIncompleteExpression();
            }
        }
        
        /// <summary>
        /// Parse with explicit mode selection
        /// </summary>
        public ASTNode? ParseWithMode(ParsingMode mode)
        {
            return mode switch
            {
                ParsingMode.LRSOnly => ParseLRSOnly(),
                ParsingMode.LegacyOnly => ParseLegacyOnly(),
                ParsingMode.LRSWithFallback => Parse(),
                _ => Parse()
            };
        }
        
        /// <summary>
        /// Parse using only LRS parser (no fallback)
        /// </summary>
        private ASTNode? ParseLRSOnly()
        {
            if (!useLRSParser || lrsParser == null)
            {
                throw new InvalidOperationException("LRS parser is disabled");
            }
            
            var position = 0;
            ASTNode? lastResult = null;
            
            // Parse all expressions in the script sequentially
            while (position < tokens.Count)
            {
                var expr = lrsParser.ParseExpression(ref position);
                
                if (expr != null)
                {
                    lastResult = expr;
                }
                
                // Skip any remaining newlines or semicolons
                while (position < tokens.Count && 
                       (tokens[position].Type == TokenType.NEWLINE || 
                        tokens[position].Type == TokenType.SEMICOLON))
                {
                    position++;
                }
            }
            
            return lastResult;
        }
        
        /// <summary>
        /// Parse using only legacy parser
        /// </summary>
        private ASTNode? ParseLegacyOnly()
        {
            var legacyParser = new Parser(tokens, sourceText);
            return legacyParser.Parse();
        }
        
        /// <summary>
        /// Get parsing statistics for debugging
        /// </summary>
        public ParsingStats GetParsingStats()
        {
            var stats = new ParsingStats
            {
                TokenCount = tokens.Count,
                SourceText = sourceText,
                UseLRSParser = useLRSParser,
                EnableFallback = enableFallback
            };
            
            if (useLRSParser)
            {
                try
                {
                    var position = 0;
                    var result = lrsParser?.ParseExpression(ref position);
                    stats.LRSSuccess = result != null && position >= tokens.Count;
                    stats.LRSConsumedTokens = position;
                }
                catch
                {
                    stats.LRSSuccess = false;
                }
            }
            
            return stats;
        }
        
        /// <summary>
        /// Get failure records for analysis
        /// </summary>
        public static List<ParserFailureRecord> GetFailureRecords()
        {
            lock (failureAnalyzerLock)
            {
                return new List<ParserFailureRecord>(failureRecords);
            }
        }
        
        /// <summary>
        /// Clear all failure records
        /// </summary>
        public static void ClearFailureRecords()
        {
            lock (failureAnalyzerLock)
            {
                failureRecords.Clear();
            }
        }
        
        /// <summary>
        /// Determine failure point description
        /// </summary>
        private static string DetermineFailurePoint(int consumedTokens, int totalTokens, TokenType? lastTokenType)
        {
            if (consumedTokens == 0)
                return "Start of expression";
            if (consumedTokens >= totalTokens)
                return "End of expression";
            if (lastTokenType.HasValue)
                return $"After {lastTokenType.Value} (position {consumedTokens}/{totalTokens})";
            return $"Position {consumedTokens}/{totalTokens}";
        }
        
        /// <summary>
        /// Parse multiple semicolon-separated expressions in Pure LRS mode
        /// Returns the result of the last expression (K semantics)
        /// Follows K spec: sequential left-to-right evaluation, newlines vs semicolons distinction
        /// </summary>
        private ASTNode? ParseMultipleExpressions()
        {
            var position = 0;
            var expressions = new List<ASTNode>();
            
            // Skip leading whitespace-only lines (top-level only)
            // Per K spec: whitespace-only lines at top level are ignored
            while (position < tokens.Count && 
                   (tokens[position].Type == TokenType.NEWLINE || 
                    tokens[position].Type == TokenType.SEMICOLON))
            {
                position++;
            }
            
            // Parse expressions sequentially left-to-right
            // CRITICAL: Sequential evaluation for stateful operations (assignments, I/O, side effects)
            while (position < tokens.Count)
            {
                // Debug for ktree tests
                if (sourceText.Contains("d:.((`keyA"))
                {
                    Console.WriteLine($"[ParseMultiple] Position {position}/{tokens.Count}, Token: {tokens[position].Type}({tokens[position].Lexeme})");
                    Console.WriteLine($"[ParseMultiple] expressions.Count = {expressions.Count}");
                }
                // Check for EOF
                if (tokens[position].Type == TokenType.EOF)
                    break;
                
                // Check for empty expression (consecutive separators with semicolon)
                // Per K spec: semicolons create null elements, newlines don't
                if (tokens[position].Type == TokenType.SEMICOLON)
                {
                    // This is an empty expression - add null
                    expressions.Add(ASTNode.MakeLiteral(new NullValue()));
                    position++;
                    continue;
                }
                
                // Skip newlines (but not semicolons - they mark empty expressions)
                // Per K spec: newlines are separators but don't create null elements at top level
                if (tokens[position].Type == TokenType.NEWLINE)
                {
                    position++;
                    continue;
                }
                
                // Parse one expression
                var result = lrsParser?.ParseExpression(ref position);
                
                if (result == null)
                {
                    // Check if this is a comment-only line (no meaningful tokens to parse)
                    // If we haven't advanced position, it's likely a comment-only line
                    var startPosition = position;
                    
                    // Try to skip ahead to next newline/semicolon/EOF to see if there's more content
                    while (position < tokens.Count && 
                           tokens[position].Type != TokenType.NEWLINE && 
                           tokens[position].Type != TokenType.SEMICOLON &&
                           tokens[position].Type != TokenType.EOF)
                    {
                        position++;
                    }
                    
                    // If we didn't advance at all (or only hit EOF), this was comment-only
                    // Skip it and continue rather than failing
                    if (position == startPosition || 
                        (position < tokens.Count && tokens[position].Type == TokenType.EOF))
                    {
                        // Skip the EOF and continue
                        if (position < tokens.Count && tokens[position].Type == TokenType.EOF)
                            position++;
                        
                        // Skip any following newlines
                        while (position < tokens.Count && 
                               tokens[position].Type == TokenType.NEWLINE)
                        {
                            position++;
                        }
                        continue;
                    }
                    
                    // Otherwise, it's a real parsing failure
                    if (ParserConfig.EnableDebugging)
                    {
                        Console.WriteLine($"[DEBUG] Pure LRS multi-expression parsing failed at position {position}");
                        Console.WriteLine($"  Source: {sourceText}");
                        if (position < tokens.Count)
                        {
                            Console.WriteLine($"  Failed at token: {tokens[position].Type}({tokens[position].Lexeme})");
                        }
                    }
                    return null;
                }
                
                expressions.Add(result);
                
                // After expression, skip trailing newlines but preserve semicolons
                while (position < tokens.Count && 
                       tokens[position].Type == TokenType.NEWLINE)
                {
                    position++;
                }
            }
            
            // Skip final EOF token if present
            if (position < tokens.Count && tokens[position].Type == TokenType.EOF)
            {
                position++;
            }
            
            // Validate that we consumed all tokens
            if (position < tokens.Count)
            {
                // Debug logging for incomplete parsing
                if (ParserConfig.EnableDebugging)
                {
                    Console.WriteLine($"[DEBUG] Pure LRS multi-expression parsing incomplete:");
                    Console.WriteLine($"  Source: {sourceText}");
                    Console.WriteLine($"  Position: {position}/{tokens.Count}");
                    Console.WriteLine($"  Remaining tokens: {string.Join(" ", tokens.Skip(position).Select(t => t.Type))}");
                }
                return null; // Incomplete parse
            }
            
            // Apply K top-level semantics
            // Per K spec: 0 expressions → null, 1 expression → its value, multiple → last value
            if (expressions.Count == 0)
                return ASTNode.MakeLiteral(new NullValue());
            
            if (expressions.Count == 1)
                return expressions[0]; // Single expression - return its value
            
            // Multiple expressions: wrap in Block node for sequential evaluation
            // The evaluator will execute each expression in order and return the last value
            // This is critical for assignments and stateful operations
            var blockNode = new ASTNode(ASTNodeType.Block);
            foreach (var expr in expressions)
            {
                blockNode.Children.Add(expr);
            }
            
            // Debug for ktree tests
            if (sourceText.Contains("d:.((`keyA"))
            {
                Console.WriteLine($"[ParseMultiple] RETURNING Block with {expressions.Count} expressions");
            }
            
            return blockNode;
        }
    }
    
    /// <summary>
    /// Record of a parser failure (simplified for cross-namespace use)
    /// </summary>
    public class ParserFailureRecord
    {
        public DateTime Timestamp { get; set; }
        public string TestName { get; set; } = "";
        public string SourceText { get; set; } = "";
        public int ConsumedTokens { get; set; }
        public int TotalTokens { get; set; }
        public TokenType? LastTokenType { get; set; }
        public string FailurePoint { get; set; } = "";
    }
    
    /// <summary>
    /// Parsing mode enumeration
    /// </summary>
    public enum ParsingMode
    {
        LRSWithFallback,
        LRSOnly,
        LegacyOnly
    }
    
    /// <summary>
    /// Parsing statistics for debugging and monitoring
    /// </summary>
    public class ParsingStats
    {
        public int TokenCount { get; set; }
        public string SourceText { get; set; } = "";
        public bool UseLRSParser { get; set; }
        public bool EnableFallback { get; set; }
        public bool LRSSuccess { get; set; }
        public int LRSConsumedTokens { get; set; }
        
        public override string ToString()
        {
            return $"Tokens: {TokenCount}, LRS: {(UseLRSParser ? (LRSSuccess ? "✓" : "✗") : "Disabled")}, " +
                   $"Fallback: {(EnableFallback ? "Enabled" : "Disabled")}, " +
                   $"Consumed: {LRSConsumedTokens}/{TokenCount}";
        }
    }
}
