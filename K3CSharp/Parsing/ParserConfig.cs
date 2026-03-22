using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Parser configuration for gradual LRS migration
    /// Controls feature flags and parsing modes
    /// </summary>
    public static class ParserConfig
    {
        /// <summary>
        /// Enable LRS parser as primary parsing mechanism
        /// </summary>
        public static bool UseLRSParser { get; set; } = true;
        
        /// <summary>
        /// Enable fallback to legacy parser when LRS fails
        /// </summary>
        public static bool EnableFallback { get; set; } = false;
        
        /// <summary>
        /// Enable debugging output for parsing operations
        /// </summary>
        public static bool EnableDebugging { get; set; } = false;
        
        /// <summary>
        /// Enable parse tree construction mode
        /// </summary>
        public static bool BuildParseTree { get; set; } = false;
        
        /// <summary>
        /// Get current parsing mode
        /// </summary>
        public static ParsingMode CurrentMode => UseLRSParser ? 
            (EnableFallback ? ParsingMode.LRSWithFallback : ParsingMode.LRSOnly) : 
            ParsingMode.LegacyOnly;
        
        /// <summary>
        /// Parse with configuration-based mode selection
        /// </summary>
        public static ASTNode? ParseWithConfig(List<Token> tokens, string source)
        {
            if (UseLRSParser)
            {
                var wrapper = new LRSParserWrapper(tokens, source, EnableFallback, true);
                return wrapper.Parse();
            }
            else
            {
                return new Parser(tokens, source).Parse();
            }
        }
        
        /// <summary>
        /// Check if expression is incomplete using configuration
        /// </summary>
        public static bool IsIncompleteExpressionWithConfig(List<Token> tokens, string source)
        {
            if (UseLRSParser)
            {
                var wrapper = new LRSParserWrapper(tokens, source, EnableFallback, true);
                return wrapper.IsIncompleteExpression();
            }
            else
            {
                return new Parser(tokens, source).IsIncompleteExpression();
            }
        }
        
        /// <summary>
        /// Get parsing statistics for monitoring
        /// </summary>
        public static ParsingStats? GetParsingStats(List<Token> tokens, string source)
        {
            if (UseLRSParser)
            {
                var wrapper = new LRSParserWrapper(tokens, source, EnableFallback, true);
                return wrapper.GetParsingStats();
            }
            else
            {
                return null; // Legacy parser doesn't provide stats
            }
        }
        
        /// <summary>
        /// Enable LRS parser with safe configuration
        /// </summary>
        public static void EnableLRSSafely()
        {
            UseLRSParser = true;
            EnableFallback = true;
            EnableDebugging = false;
        }
        
        /// <summary>
        /// Disable LRS parser (revert to legacy)
        /// </summary>
        public static void DisableLRS()
        {
            UseLRSParser = false;
            EnableFallback = true;
        }
        
        /// <summary>
        /// Enable pure LRS mode (no fallback)
        /// </summary>
        public static void EnablePureLRS()
        {
            UseLRSParser = true;
            EnableFallback = false;
            EnableDebugging = true;
        }
        
        /// <summary>
        /// Get configuration summary
        /// </summary>
        public static string GetConfigSummary()
        {
            return $"LRS: {(UseLRSParser ? "Enabled" : "Disabled")}, " +
                   $"Fallback: {(EnableFallback ? "Enabled" : "Disabled")}, " +
                   $"Debug: {(EnableDebugging ? "Enabled" : "Disabled")}, " +
                   $"ParseTree: {(BuildParseTree ? "Enabled" : "Disabled")}";
        }
        
        /// <summary>
        /// Log configuration change
        /// </summary>
        public static void LogConfigChange(string operation)
        {
            if (EnableDebugging)
            {
                Console.WriteLine($"[ParserConfig] {operation}: {GetConfigSummary()}");
            }
        }
    }
}
