using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    /// <summary>
    /// Parser pipeline that coordinates between multiple parser modules
    /// This provides a clean way to orchestrate parsing without replacing existing functionality
    /// </summary>
    public class ParserPipeline
    {
        private readonly List<IParserModule> modules;
        private readonly ParseContext context;

        public ParserPipeline(List<Token> tokens, string sourceText)
        {
            modules = new List<IParserModule>();
            context = new ParseContext(tokens, sourceText);
            InitializeModules();
        }

        private void InitializeModules()
        {
            modules.Add(new ExpressionParser());
            modules.Add(new PrimaryParser());
            modules.Add(new AdverbParser());
            modules.Add(new BracketParser());
            modules.Add(new FunctionParser());
            modules.Add(new AssignmentParser());
        }

        /// <summary>
        /// Main parse method that matches the Parser interface expected by test runner
        /// </summary>
        public ASTNode? Parse()
        {
            if (context.IsAtEnd())
            {
                return null;
            }

            var result = TryParseWithModules();
            
            // Handle case where TryParseWithModules returns null
            if (result == null)
            {
                return null;
            }

            // Handle multiple statements separated by semicolons or newlines
            var statements = new List<ASTNode>();
            statements.Add(result);
            
            // Parse additional statements separated by semicolons or newlines
            while (!context.IsAtEnd() && 
                  (context.CurrentToken().Type == TokenType.SEMICOLON || 
                   context.CurrentToken().Type == TokenType.NEWLINE))
            {
                context.Advance(); // Consume semicolon or newline
                
                // Skip empty lines
                while (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.NEWLINE)
                {
                    context.Advance();
                }
                
                if (!context.IsAtEnd())
                {
                    var stmt = TryParseWithModules();
                    if (stmt != null)
                    {
                        statements.Add(stmt);
                    }
                }
            }

            // If we have multiple statements, create a block
            if (statements.Count > 1)
            {
                var block = new ASTNode(ASTNodeType.Block);
                block.Children.AddRange(statements);
                return block;
            }

            return result;
        }

        /// <summary>
        /// Try to parse using the modular pipeline
        /// Returns null if no module can handle the current token
        /// </summary>
        public ASTNode? TryParseWithModules()
        {
            if (context.IsAtEnd())
            {
                return null;
            }

            var currentToken = context.CurrentToken();
            
            // Try each module in order to see which can handle the current token
            foreach (var module in modules)
            {
                if (module.CanHandle(currentToken.Type))
                {
                    try
                    {
                        var result = module.Parse(context);
                        return result;
                    }
                    catch
                    {
                        // If module fails, continue to next module
                        continue;
                    }
                }
            }
            
            // No module could handle this token
            return null;
        }

        /// <summary>
        /// Get current position for synchronization with main parser
        /// </summary>
        public int GetCurrentPosition() => context.Current;

        /// <summary>
        /// Set current position for synchronization with main parser
        /// </summary>
        public void SetCurrentPosition(int position) => context.Current = position;

        /// <summary>
        /// Check if pipeline can handle the current token
        /// </summary>
        public bool CanHandleCurrentToken()
        {
            if (context.IsAtEnd())
            {
                return false;
            }

            var currentToken = context.CurrentToken();
            return modules.Any(module => module.CanHandle(currentToken.Type));
        }

        /// <summary>
        /// Test specific module functionality
        /// </summary>
        public ASTNode? TestModule<T>() where T : IParserModule, new()
        {
            var module = new T();
            var currentToken = context.CurrentToken();
            
            if (module.CanHandle(currentToken.Type))
            {
                try
                {
                    return module.Parse(context);
                }
                catch
                {
                    return null;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Get module statistics for debugging
        /// </summary>
        public Dictionary<string, bool> GetModuleCapabilities()
        {
            var capabilities = new Dictionary<string, bool>();
            
            if (context.IsAtEnd())
            {
                return capabilities;
            }

            var currentToken = context.CurrentToken();
            
            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;
                capabilities[moduleName] = module.CanHandle(currentToken.Type);
            }
            
            return capabilities;
        }
    }
}
