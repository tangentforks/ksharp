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
        
        public LRSParserWrapper(List<Token> tokens, string sourceText, bool enableFallback = true, bool useLRSParser = true)
        {
            this.tokens = tokens;
            this.sourceText = sourceText;
            this.enableFallback = enableFallback;
            this.useLRSParser = useLRSParser;
            
            if (useLRSParser)
            {
                this.lrsParser = new LRSParser(tokens);
            }
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
            
            try
            {
                // Try LRS parsing first
                var position = 0;
                var result = lrsParser?.ParseExpression(ref position);
                
                if (ParserConfig.EnableDebugging)
                {
                    var resultStr = result != null ? "SUCCESS" : "NULL";
                    Console.WriteLine($"[LRSParserWrapper] LRS parsing result: {resultStr}, consumed: {position}/{tokens.Count}");
                }
                
                // Validate result
                if (result != null && position >= tokens.Count)
                {
                    return result;
                }
                
                // If LRS parsing didn't consume all tokens, fall back
                if (enableFallback)
                {
                    return FallbackToLegacyParser();
                }
                
                throw new Exception($"LRS parser failed to parse complete expression: {sourceText}");
            }
            catch when (enableFallback)
            {
                // Fallback to legacy parser on any error
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
            var result = lrsParser.ParseExpression(ref position);
            
            if (result == null || position < tokens.Count)
            {
                throw new Exception($"LRS parser failed to parse complete expression: {sourceText}");
            }
            
            return result;
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
