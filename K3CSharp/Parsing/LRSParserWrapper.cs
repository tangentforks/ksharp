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
        private static readonly Dictionary<TokenType, (int paren, int bracket, int brace)> DelimiterAdjustments = new()
        {
            { TokenType.LEFT_PAREN, (1, 0, 0) },
            { TokenType.RIGHT_PAREN, (-1, 0, 0) },
            { TokenType.LEFT_BRACKET, (0, 1, 0) },
            { TokenType.RIGHT_BRACKET, (0, -1, 0) },
            { TokenType.LEFT_BRACE, (0, 0, 1) },
            { TokenType.RIGHT_BRACE, (0, 0, -1) }
        };

        private readonly LRSParser? lrsParser;
        private readonly List<Token> tokens;
        private readonly string sourceText;
        
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
        
        public LRSParserWrapper(List<Token> tokens, string sourceText)
        {
            // Preprocess tokens to combine K-tree dotted notation (.k.d -> single IDENTIFIER token)
            var processedTokens = PreprocessDottedPaths(tokens, sourceText);
            this.tokens = processedTokens;
            this.sourceText = sourceText;

            // Always use LRS parser (no legacy mode)
            this.lrsParser = new LRSParser(processedTokens);
            if (this.lrsParser != null)
            {
                this.lrsParser.PureLRSMode = true;
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
        /// Parse using LRS strategy (no fallback)
        /// </summary>
        public ASTNode? Parse()
        {
            // Debug: check which path we'll take
            if (sourceText.Contains("d:."))
            {
                Console.WriteLine($"[Parse] ENTER: source='{sourceText.Substring(0, Math.Min(30, sourceText.Length))}'");
            }

            // Pure LRS mode: Handle multiple semicolon-separated expressions
            if (sourceText.Contains("d:."))
                Console.WriteLine("[Parse] Taking Pure LRS path -> ParseMultipleExpressions");
            return ParseMultipleExpressions();
        }
        
        /// <summary>
        /// Check if expression is incomplete (matches Parser interface)
        /// Uses delimiter balancing instead of parsing to avoid dependency on parsing success
        /// </summary>
        public bool IsIncompleteExpression()
        {
            // Simple delimiter balancing check (same logic as legacy parser)
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
                else if (!inString && !inSymbol && DelimiterAdjustments.TryGetValue(token.Type, out var adjustment))
                {
                    parentheses += adjustment.paren;
                    brackets += adjustment.bracket;
                    braces += adjustment.brace;
                }
            }

            // Expression is incomplete if any brackets are unmatched
            return parentheses != 0 || brackets != 0 || braces != 0 || inString || inSymbol;
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
                UseLRSParser = true,
                EnableFallback = false
            };

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
