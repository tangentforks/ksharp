using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Interface for LRS Parser to break circular dependencies
    /// </summary>
    public interface ILRSParser
    {
        /// <summary>
        /// Parse expression using LRS strategy
        /// </summary>
        /// <param name="position">Starting position, updated to end of parsed expression</param>
        /// <returns>AST node representing the parsed expression</returns>
        ASTNode? ParseExpression(ref int position);
        
        /// <summary>
        /// Build parse tree flag
        /// </summary>
        bool BuildParseTree { get; set; }
    }
}
