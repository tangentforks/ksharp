using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Hybrid parser that uses ParserPipeline for LRS-enabled parsing but falls back to original Parser
    /// This provides a smooth transition path for deprecating the old parser
    /// </summary>
    public class HybridParser
    {
        private readonly List<Token> tokens;
        private readonly string sourceText;
        private readonly ParserPipeline pipeline;
        private readonly Parser fallbackParser;

        public HybridParser(List<Token> tokens, string sourceText)
        {
            this.tokens = tokens;
            this.sourceText = sourceText;
            this.pipeline = new ParserPipeline(tokens, sourceText);
            this.fallbackParser = new Parser(tokens, sourceText);
        }

        /// <summary>
        /// Main parse method that tries ParserPipeline first, then falls back to original Parser
        /// </summary>
        public ASTNode? Parse()
        {
            // Try ParserPipeline first (with LRS integration)
            try
            {
                var pipelineResult = pipeline.Parse();
                if (pipelineResult != null)
                {
                    return pipelineResult;
                }
            }
            catch
            {
                // If ParserPipeline fails, fall back to original Parser
            }

            // Fall back to original Parser
            try
            {
                return fallbackParser.Parse();
            }
            catch
            {
                // If both fail, return null
                return null;
            }
        }

        /// <summary>
        /// Check if the pipeline can handle the current token
        /// </summary>
        public bool CanUsePipeline()
        {
            return pipeline.CanHandleCurrentToken();
        }

        /// <summary>
        /// Get statistics about which parser was used
        /// </summary>
        public (bool usedPipeline, bool usedFallback, ASTNode? result) ParseWithStats()
        {
            // Try ParserPipeline first
            try
            {
                if (pipeline.CanHandleCurrentToken())
                {
                    var result = pipeline.Parse();
                    if (result != null)
                    {
                        return (true, false, result);
                    }
                }
            }
            catch
            {
                // Pipeline failed, try fallback
            }

            // Fall back to original Parser
            try
            {
                var result = fallbackParser.Parse();
                return (false, true, result);
            }
            catch
            {
                return (false, false, null);
            }
        }
    }
}
