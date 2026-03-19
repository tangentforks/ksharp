using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Factory for creating LRS Parser instances to break circular dependencies
    /// </summary>
    public static class LRSParserFactory
    {
        /// <summary>
        /// Create LRS Parser instance without circular dependencies
        /// </summary>
        /// <param name="tokens">Tokens to parse</param>
        /// <param name="buildParseTree">Whether to build parse trees</param>
        /// <returns>LRS Parser instance</returns>
        public static ILRSParser CreateParser(List<Token> tokens, bool buildParseTree = false)
        {
            return new LRSParser(tokens, buildParseTree);
        }
        
        /// <summary>
        /// Create LRS Expression Processor with dependency injection
        /// </summary>
        /// <param name="tokens">Tokens to process</param>
        /// <param name="buildParseTree">Whether to build parse trees</param>
        /// <returns>LRS Expression Processor instance</returns>
        public static LRSExpressionProcessor CreateExpressionProcessor(List<Token> tokens, bool buildParseTree = false)
        {
            var lrsParser = CreateParser(tokens, buildParseTree);
            return new LRSExpressionProcessor(tokens, buildParseTree, lrsParser);
        }
    }
}
