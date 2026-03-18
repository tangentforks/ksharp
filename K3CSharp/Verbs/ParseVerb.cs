using System;
using System.Collections.Generic;
using System.Linq;
using K3CSharp;
using K3CSharp.Parsing;

namespace K3CSharp.Verbs
{
    /// <summary>
    /// Implementation of _parse verb for K3CSharp
    /// Parses character vectors into AST nodes and converts to K list representations
    /// </summary>
    public static class ParseVerbHandler
    {
        /// <summary>
        /// Main _parse entry point
        /// </summary>
        public static K3Value Parse(string expressionText)
        {
            return ParseExpression(expressionText);
        }
        
        /// <summary>
        /// Parse verb implementation matching delegate signature
        /// </summary>
        public static K3Value Parse(K3Value[] arguments)
        {
            if (arguments.Length == 0)
                throw new Exception("_parse: requires an argument");
                
            var expressionText = arguments[0].ToString();
            return Parse(expressionText);
        }
        
        /// <summary>
        /// Parse character vector expression using proper K parser
        /// </summary>
        private static K3Value ParseExpression(string expressionText)
        {
            try
            {
                // Use same pattern as Program.cs
                var lexer = new Lexer(expressionText);
                var tokens = lexer.Tokenize();
                
                // Check if we have a single CHARACTER_VECTOR token (from a quoted string)
                if (tokens.Count == 2 && tokens[0].Type == TokenType.CHARACTER_VECTOR && tokens[1].Type == TokenType.EOF)
                {
                    // Extract the content from the quoted string and parse it
                    var content = tokens[0].Lexeme;
                    var contentLexer = new Lexer(content);
                    var contentTokens = contentLexer.Tokenize();
                    var pipeline = new ParserPipeline(contentTokens, content);
                    var astNode = pipeline.TryParseWithModules();
                    
                    if (astNode == null)
                        throw new Exception("Failed to parse expression");
                    
                    // Convert AST to K list representation
                    return ParseTreeConverter.ToKList(astNode);
                }
                else
                {
                    // Parse normally for other cases
                    var pipeline = new ParserPipeline(tokens, expressionText);
                    var astNode = pipeline.TryParseWithModules();
                    
                    if (astNode == null)
                        throw new Exception("Failed to parse expression");
                    
                    // Convert AST to K list representation
                    return ParseTreeConverter.ToKList(astNode);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Parse error: {ex.Message}");
            }
        }
    }
}
