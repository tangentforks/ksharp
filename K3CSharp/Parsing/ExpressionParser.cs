using System;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Interface for parser modules
    /// </summary>
    public interface IParserModule
    {
        ASTNode? Parse(ParseContext context);
        bool CanHandle(TokenType currentToken);
    }

    /// <summary>
    /// Context object for parser operations
    /// </summary>
    public class ParseContext
    {
        public List<Token> Tokens { get; set; }
        public int Current { get; set; }
        public string SourceText { get; set; }
        public bool ParsingDotApplyArguments { get; set; }
        public int DelimiterDepth { get; set; }

        public ParseContext(List<Token> tokens, string sourceText)
        {
            Tokens = tokens;
            SourceText = sourceText;
            Current = 0;
            ParsingDotApplyArguments = false;
            DelimiterDepth = 0;
        }

        public Token CurrentToken() => Current < Tokens.Count ? Tokens[Current] : new Token(TokenType.EOF, "", Current);

        public Token PreviousToken() => Current > 0 ? Tokens[Current - 1] : new Token(TokenType.EOF, "", Current);

        public bool IsAtEnd() => Current >= Tokens.Count || (Current < Tokens.Count && Tokens[Current].Type == TokenType.EOF);

        public bool Match(TokenType tokenType)
        {
            if (Check(tokenType))
            {
                Current++;
                return true;
            }
            return false;
        }

        public bool Check(TokenType tokenType)
        {
            return !IsAtEnd() && CurrentToken().Type == tokenType;
        }

        public void Advance()
        {
            if (!IsAtEnd()) Current++;
        }
    }

    /// <summary>
    /// Expression parser module for handling ParseExpression, ParseTerm, and expression-level operations
    /// </summary>
    public class ExpressionParser : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // Enhanced to handle more cases for better coverage
            // Exclude true delimiters and operators that should be handled by other modules
            return currentToken != TokenType.EOF &&
                   currentToken != TokenType.NEWLINE &&
                   currentToken != TokenType.RIGHT_PAREN &&
                   currentToken != TokenType.RIGHT_BRACE &&
                   currentToken != TokenType.RIGHT_BRACKET &&
                   currentToken != TokenType.SEMICOLON &&
                   currentToken != TokenType.COLON;
        }

        public ASTNode? Parse(ParseContext context)
        {
            return ParseExpression(context);
        }

        private ASTNode? ParseExpression(ParseContext context)
        {
            var left = ParseTerm(context);

            // Handle case where ParseTerm returns null (e.g., due to NEWLINE at start)
            if (left == null)
            {
                return null;
            }

            // Enhanced vector formation - collect multiple elements
            var elements = new List<ASTNode>();
            if (left != null) elements.Add(left);
            var firstElementType = left?.Type ?? ASTNodeType.Literal;

            while (!context.IsAtEnd() && !ShouldStopParsing(context))
            {
                // Check if this would create a mixed-type vector or binary operation
                if (IsBinaryOperator(context.CurrentToken().Type))
                {
                    // Check if this might be an adverb context with the current vector
                    if (elements.Count > 0 && elements.Last().Type == ASTNodeType.Vector && 
                        IsPotentialAdverbOperator(context.CurrentToken().Type))
                    {
                        // This might be an adverb context - check if it's %/ pattern
                        if (context.CurrentToken().Type == TokenType.MODULUS)
                        {
                            // Look ahead to see if next token is DIVIDE
                            context.Advance(); // Consume the MODULUS token
                            if (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.DIVIDE)
                            {
                                // This is %/ pattern - break vector formation and let adverb handling take over
                                context.Current--; // Back up to MODULUS position
                                break;
                            }
                            context.Current--; // Back up if not DIVIDE
                        }
                    }
                    break; // Let binary operator handling take over
                }
                
                var nextElement = ParseTerm(context);
                if (nextElement != null)
                {
                    elements.Add(nextElement);
                }
                else
                {
                    break;
                }
            }

            // If we have multiple elements, create a vector
            if (elements.Count > 1)
            {
                var vectorNode = new ASTNode(ASTNodeType.Vector);
                vectorNode.Children.AddRange(elements);
                return vectorNode;
            }

            // Continue with binary operator handling
            while (!context.IsAtEnd() && IsBinaryOperator(context.CurrentToken().Type))
            {
                var opToken = context.CurrentToken();
                context.Advance();
                
                // Check if this might be an adverb context
                if (left != null && IsAdverbContext(left, opToken, context))
                {
                    // Handle as adverb operation instead of binary operation
                    return ParseAdverbOperation(left, opToken, context);
                }
                
                var right = ParseTerm(context);
                if (right == null)
                {
                    // This is a projection - create a ProjectedFunction node
                    var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                    
                    // Convert token type to operator symbol
                    var operatorSymbol = opToken.Type switch
                    {
                        TokenType.PLUS => "+",
                        TokenType.MINUS => "-",
                        TokenType.MULTIPLY => "*",
                        TokenType.DIVIDE => "%",
                        TokenType.MIN => "&",
                        TokenType.MAX => "|",
                        TokenType.LESS => "<",
                        TokenType.GREATER => ">",
                        TokenType.EQUAL => "=",
                        TokenType.IN => "_in",
                        TokenType.BIN => "_bin",
                        TokenType.BINL => "_binl",
                        TokenType.LIN => "_lin",
                        TokenType.DV => "_dv",
                        TokenType.DI => "_di",
                        TokenType.VS => "_vs",
                        TokenType.SV => "_sv",
                        TokenType.SS => "_ss",
                        TokenType.SM => "_sm",
                        TokenType.CI => "_ci",
                        TokenType.IC => "_ic",
                        TokenType.POWER => "^",
                        TokenType.MODULUS => "!",
                        TokenType.JOIN => ",",
                        TokenType.COLON => ":",
                        TokenType.HASH => "#",
                        TokenType.UNDERSCORE => "_",
                        TokenType.QUESTION => "?",
                        TokenType.DOLLAR => "$",
                        TokenType.TYPE => "@",
                        TokenType.STRING_REPRESENTATION => "$",
                        TokenType.APPLY => "@",
                        TokenType.DOT_APPLY => "_dot",
                        _ => opToken.Lexeme
                    };
                    
                    projectedNode.Value = new SymbolValue(operatorSymbol);
                    
                    // Determine arity from VerbRegistry - use highest supported arity as default
                    var verb = VerbRegistry.GetVerb(operatorSymbol);
                    int defaultArity = verb?.SupportedArities.Max() ?? 2;
                    projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(defaultArity)));
                    
                    // Add the left operand if we have one
                    if (left != null)
                    {
                        projectedNode.Children.Add(left);
                    }
                    
                    return projectedNode;
                }

                // Create binary operation node
                if (left != null && right != null)
                {
                    left = ASTNode.MakeBinaryOp(opToken.Type, left, right);
                }
                else if (left != null)
                {
                    // This is a projection - create a ProjectedFunction node
                    var projectedNode = new ASTNode(ASTNodeType.ProjectedFunction);
                    
                    // Determine operator symbol
                    var operatorSymbol = opToken.Type switch
                    {
                        TokenType.PLUS => "+",
                        TokenType.MINUS => "-",
                        TokenType.MULTIPLY => "*",
                        TokenType.DIVIDE => "%",
                        TokenType.MIN => "min",
                        TokenType.MAX => "max",
                        TokenType.LESS => "<",
                        TokenType.GREATER => ">",
                        TokenType.EQUAL => "=",
                        TokenType.IN => "_in",
                        TokenType.POWER => "^",
                        TokenType.MODULUS => "!",
                        TokenType.JOIN => ",",
                        TokenType.COLON => ":",
                        TokenType.HASH => "#",
                        TokenType.UNDERSCORE => "_",
                        TokenType.QUESTION => "?",
                        TokenType.DOLLAR => "$",
                        TokenType.TYPE => "@",
                        TokenType.STRING_REPRESENTATION => "$",
                        TokenType.APPLY => "@",
                        TokenType.DOT_APPLY => "_dot",
                        _ => opToken.Lexeme
                    };
                    
                    projectedNode.Value = new SymbolValue(operatorSymbol);
                    
                    // Determine arity from VerbRegistry
                    var verb = VerbRegistry.GetVerb(operatorSymbol);
                    int defaultArity = verb?.SupportedArities.Max() ?? 2;
                    projectedNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(defaultArity)));
                    
                    // Add the left operand
                    projectedNode.Children.Add(left);
                    
                    return projectedNode;
                }
            }

            return left;
        }

        private bool ShouldStopParsing(ParseContext context)
        {
            // Stop parsing for statement separators and end-of-input
            return context.IsAtEnd() || 
                   context.CurrentToken().Type == TokenType.SEMICOLON ||
                   context.CurrentToken().Type == TokenType.NEWLINE ||
                   context.CurrentToken().Type == TokenType.EOF;
        }

        private bool IsBinaryOperator(TokenType tokenType)
        {
            return VerbRegistry.IsBinaryOperator(tokenType);
        }

        private ASTNode? ParseTerm(ParseContext context, bool parseUntilEnd = false)
        {
            // Only return null for EOF, not for NEWLINE when parsing expressions
            // NEWLINE should be handled at higher levels as statement separators
            if (context.IsAtEnd())
            {
                return null;
            }

            ASTNode? result;
            
            // Check if current token is an adverb - if so, we're in a nesting context
            if (!context.IsAtEnd() && VerbRegistry.IsAdverbToken(context.CurrentToken().Type))
            {
                // Parse the primary expression normally - adverb will be handled in ParseTerm
                result = ParsePrimary(context);
            }
            else
            {
                result = ParsePrimary(context);
            }

            // Handle case where ParsePrimary returned null
            if (result == null)
            {
                return null;
            }

            // Handle adverbs
            if (!context.IsAtEnd() && VerbRegistry.IsAdverbToken(context.CurrentToken().Type))
            {
                var adverbType = context.CurrentToken().Type;
                context.Advance(); // Consume the adverb token
                
                // Parse the right argument for the adverb
                var rightArg = ParseTerm(context, parseUntilEnd);
                
                // Create adverb node: ADVERB(adverbType, verb, rightArg)
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue(adverbType.ToString());
                adverbNode.Children.Add(result!); // verb
                adverbNode.Children.Add(rightArg!); // right argument
                
                result = adverbNode;
            }

            return result;
        }

        private ASTNode? ParsePrimary(ParseContext context)
        {
            // Delegate to PrimaryParser for primary expression parsing
            var primaryParser = new PrimaryParser();
            if (primaryParser.CanHandle(context.CurrentToken().Type))
            {
                return primaryParser.Parse(context);
            }
            
            throw new Exception($"Unexpected token: {context.CurrentToken().Type}({context.CurrentToken().Lexeme})");
        }

        /// <summary>
        /// Check if the current context represents an adverb operation
        /// </summary>
        private bool IsAdverbContext(ASTNode? left, Token opToken, ParseContext context)
        {
            // Check if this is a potential adverb context
            // Pattern: (vector) operator scalar
            if (left != null && left.Type == ASTNodeType.Vector && IsPotentialAdverbOperator(opToken.Type))
            {
                // Look ahead to see if the right side is a scalar
                if (!context.IsAtEnd())
                {
                    var nextToken = context.CurrentToken();
                    return IsScalarToken(nextToken.Type);
                }
            }
            
            // Check for %/ pattern specifically
            if (left != null && left.Type == ASTNodeType.Vector && opToken.Type == TokenType.MODULUS)
            {
                // Look ahead to see if this is %/ pattern
                if (!context.IsAtEnd() && context.CurrentToken().Type == TokenType.DIVIDE)
                {
                    // This is %/ pattern - check if right side is scalar
                    context.Advance(); // Consume the / token
                    if (!context.IsAtEnd())
                    {
                        var nextToken = context.CurrentToken();
                        // Back up one position to let the main loop handle the /
                        context.Current--;
                        return IsScalarToken(nextToken.Type);
                    }
                    context.Current--; // Back up if no scalar found
                }
            }
            
            return false;
        }

        /// <summary>
        /// Check if the operator can be used as an adverb
        /// </summary>
        private bool IsPotentialAdverbOperator(TokenType tokenType)
        {
            return tokenType == TokenType.MULTIPLY ||  // * (each)
                   tokenType == TokenType.DIVIDE ||    // / (each-right)
                   tokenType == TokenType.MODULUS ||   // % (part of %/ pattern)
                   tokenType == TokenType.ADVERB_BACKSLASH || // \ (each-left)
                   tokenType == TokenType.ADVERB_TICK;       // ' (each)
        }

        /// <summary>
        /// Check if the token represents a scalar value
        /// </summary>
        private bool IsScalarToken(TokenType tokenType)
        {
            return tokenType == TokenType.INTEGER ||
                   tokenType == TokenType.FLOAT ||
                   tokenType == TokenType.SYMBOL ||
                   tokenType == TokenType.QUOTE ||
                   tokenType == TokenType.CHARACTER;
        }

        /// <summary>
        /// Parse an adverb operation
        /// </summary>
        private ASTNode? ParseAdverbOperation(ASTNode left, Token opToken, ParseContext context)
        {
            // Handle %/ pattern specifically
            if (opToken.Type == TokenType.MODULUS && !context.IsAtEnd() && context.CurrentToken().Type == TokenType.DIVIDE)
            {
                // This is %/ pattern - consume the / and handle as each-right adverb
                context.Advance(); // Consume the / token
                
                // Parse the right argument (scalar)
                var right = ParseTerm(context);
                if (right == null)
                {
                    throw new Exception("Expected scalar argument after %/ operator");
                }

                // Create %/ adverb node with proper structure
                var adverbNode = new ASTNode(ASTNodeType.BinaryOp);
                adverbNode.Value = new SymbolValue("ADVERB_SLASH_COLON"); // %/ maps to each-right
                
                // Structure: [verb, left-argument, right-argument]
                adverbNode.Children.Add(left); // verb (the vector)
                adverbNode.Children.Add(ASTNode.MakeLiteral(new IntegerValue(0))); // left argument (default)
                adverbNode.Children.Add(right); // right argument (the scalar)
                
                return adverbNode;
            }
            
            // Parse the right argument (scalar) for other adverb patterns
            var rightArg = ParseTerm(context);
            if (rightArg == null)
            {
                throw new Exception($"Expected scalar argument after adverb operator {opToken.Lexeme}");
            }

            // Create adverb node with proper structure
            var adverbNode2 = new ASTNode(ASTNodeType.BinaryOp);
            
            // Use original token type, don't convert - let evaluation handle context
            adverbNode2.Value = new SymbolValue(opToken.Type.ToString());
            
            // Structure: [verb, left-argument, right-argument]
            adverbNode2.Children.Add(left); // verb (the vector)
            adverbNode2.Children.Add(ASTNode.MakeLiteral(new IntegerValue(0))); // left argument (default)
            adverbNode2.Children.Add(rightArg); // right argument (the scalar)
            
            return adverbNode2;
        }

        }
}
