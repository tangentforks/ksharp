using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Statement parsing for LRS parser
    /// Handles assignment and control flow statements with proper precedence
    /// Statements have lower precedence than verbs but higher precedence than separators
    /// </summary>
    public class LRSStatementParser
    {
        private readonly List<Token> tokens;
        private readonly LRSParser parentParser;
        
        public LRSStatementParser(List<Token> tokens, LRSParser parentParser)
        {
            this.tokens = tokens;
            this.parentParser = parentParser;
        }
        
        /// <summary>
        /// Check if the first token could be a statement
        /// </summary>
        public static bool CouldBeStatement(TokenType tokenType)
        {
            return tokenType == TokenType.COLON ||           // Assignment / conditional evaluation
                   tokenType == TokenType.ASSIGNMENT ||     // Assignment operator
                   tokenType == TokenType.GLOBAL_ASSIGNMENT || // Global assignment
                   tokenType == TokenType.DO ||              // do statement
                   tokenType == TokenType.WHILE ||           // while statement
                   tokenType == TokenType.IF_FUNC;           // if statement
        }
        
        /// <summary>
        /// Parse statement from tokens
        /// </summary>
        /// <param name="statementTokens">Tokens representing the statement</param>
        /// <returns>AST node representing the statement</returns>
        public ASTNode? ParseStatement(List<Token> statementTokens)
        {
            if (statementTokens.Count == 0)
                return null;
                
            var firstToken = statementTokens[0];
            
            switch (firstToken.Type)
            {
                case TokenType.COLON:
                    return ParseColonStatement(statementTokens);
                case TokenType.ASSIGNMENT:
                case TokenType.GLOBAL_ASSIGNMENT:
                    return ParseAssignmentStatement(statementTokens);
                case TokenType.DO:
                    return ParseDoStatement(statementTokens);
                case TokenType.WHILE:
                    return ParseWhileStatement(statementTokens);
                case TokenType.IF_FUNC:
                    return ParseIfStatement(statementTokens);
                default:
                    return null;
            }
        }
        
        /// <summary>
        /// Parse colon statement (either assignment or conditional evaluation)
        /// </summary>
        private ASTNode? ParseColonStatement(List<Token> tokens)
        {
            // Check if this is conditional evaluation :[...]
            if (tokens.Count >= 2 && tokens[1].Type == TokenType.LEFT_BRACKET)
            {
                return ParseConditionalEvaluation(tokens);
            }
            
            // Otherwise, treat as assignment
            return ParseAssignmentStatement(tokens);
        }
        
        /// <summary>
        /// Parse assignment statement with pure vs inline behavior
        /// </summary>
        private ASTNode? ParseAssignmentStatement(List<Token> tokens)
        {
            // Find the assignment operator
            int assignmentIndex = FindAssignmentOperator(tokens);
            if (assignmentIndex == -1)
                return null;
                
            var assignmentToken = tokens[assignmentIndex];
            
            // Split into left and right parts
            var leftTokens = tokens.GetRange(0, assignmentIndex);
            var rightTokens = tokens.GetRange(assignmentIndex + 1, tokens.Count - assignmentIndex - 1);
            
            if (leftTokens.Count == 0 || rightTokens.Count == 0)
                return null;
            
            // Check if this is a modified assignment operator (apply and assign)
            if (assignmentToken.Lexeme.Length > 1 && assignmentToken.Lexeme.EndsWith(":"))
            {
                return ParseApplyAndAssignStatement(leftTokens, assignmentToken, rightTokens);
            }
            
            // Parse left side (variable name)
            var variableNode = ParseVariableName(leftTokens);
            if (variableNode == null)
                return null;
            
            // Parse right side (expression)
            var rightNode = ParseRightSideExpression(rightTokens);
            if (rightNode == null)
                return null;
            
            // Create assignment node
            var assignmentNode = new ASTNode(ASTNodeType.Assignment);
            assignmentNode.Value = variableNode.Value; // Variable name
            assignmentNode.Children.Add(rightNode);
            
            // Mark whether this is a terminal (pure) or inline assignment
            bool isTerminalAssignment = IsTerminalAssignment(tokens);
            assignmentNode.IsTerminalAssignment = isTerminalAssignment;
            
            return assignmentNode;
        }
        
        /// <summary>
        /// Parse apply and assign statement (e.g., i+:1, x:*2, etc.)
        /// </summary>
        private ASTNode? ParseApplyAndAssignStatement(List<Token> leftTokens, Token assignmentToken, List<Token> rightTokens)
        {
            // Parse left side (variable name)
            var variableNode = ParseVariableName(leftTokens);
            if (variableNode == null)
                return null;
            
            // Parse right side (expression)
            var rightNode = ParseRightSideExpression(rightTokens);
            if (rightNode == null)
                return null;
            
            // Extract the operator (everything except the colon)
            string operatorSymbol = assignmentToken.Lexeme.Substring(0, assignmentToken.Lexeme.Length - 1);
            
            // Create apply and assign node
            var applyAndAssignNode = new ASTNode(ASTNodeType.ApplyAndAssign);
            applyAndAssignNode.Value = variableNode.Value; // Variable name
            applyAndAssignNode.Children.Add(ASTNode.MakeLiteral(new SymbolValue(operatorSymbol)));
            applyAndAssignNode.Children.Add(rightNode);
            
            return applyAndAssignNode;
        }
        
        /// <summary>
        /// Parse conditional evaluation :[...]
        /// </summary>
        private ASTNode? ParseConditionalEvaluation(List<Token> tokens)
        {
            if (tokens.Count < 3 || tokens[1].Type != TokenType.LEFT_BRACKET)
                return null;
            
            // Find the matching right bracket
            int bracketDepth = 1;
            int rightBracketIndex = -1;
            
            for (int i = 2; i < tokens.Count && bracketDepth > 0; i++)
            {
                if (tokens[i].Type == TokenType.LEFT_BRACKET)
                    bracketDepth++;
                else if (tokens[i].Type == TokenType.RIGHT_BRACKET)
                    bracketDepth--;
                    
                if (bracketDepth == 0)
                {
                    rightBracketIndex = i;
                    break;
                }
            }
            
            if (rightBracketIndex == -1)
                return null;
            
            // Extract the content inside brackets
            var contentTokens = tokens.GetRange(2, rightBracketIndex - 2);
            var arguments = ParseBracketArguments(contentTokens);
            
            // Create conditional evaluation node
            var conditionalNode = new ASTNode(ASTNodeType.ConditionalStatement);
            conditionalNode.Value = new SymbolValue(":");
            
            // Add parsed arguments as children
            foreach (var arg in arguments)
            {
                conditionalNode.Children.Add(arg);
            }
            
            return conditionalNode;
        }
        
        /// <summary>
        /// Parse do[...] statement
        /// </summary>
        private ASTNode? ParseDoStatement(List<Token> tokens)
        {
            return ParseControlFlowStatement(tokens, "do");
        }
        
        /// <summary>
        /// Parse if[...] statement
        /// </summary>
        private ASTNode? ParseIfStatement(List<Token> tokens)
        {
            return ParseControlFlowStatement(tokens, "if");
        }
        
        /// <summary>
        /// Parse while[...] statement
        /// </summary>
        private ASTNode? ParseWhileStatement(List<Token> tokens)
        {
            return ParseControlFlowStatement(tokens, "while");
        }
        
        /// <summary>
        /// Parse control flow statement (do, if, while) with bracket arguments
        /// </summary>
        private ASTNode? ParseControlFlowStatement(List<Token> tokens, string statementType)
        {
            if (tokens.Count < 3 || tokens[1].Type != TokenType.LEFT_BRACKET)
                return null;
            
            // Find the matching right bracket
            int bracketDepth = 1;
            int rightBracketIndex = -1;
            
            for (int i = 2; i < tokens.Count && bracketDepth > 0; i++)
            {
                if (tokens[i].Type == TokenType.LEFT_BRACKET)
                    bracketDepth++;
                else if (tokens[i].Type == TokenType.RIGHT_BRACKET)
                    bracketDepth--;
                    
                if (bracketDepth == 0)
                {
                    rightBracketIndex = i;
                    break;
                }
            }
            
            if (rightBracketIndex == -1)
                return null;
            
            // Extract the content inside brackets
            var contentTokens = tokens.GetRange(2, rightBracketIndex - 2);
            var arguments = ParseBracketArguments(contentTokens);
            
            // Create control flow node
            var controlFlowNode = new ASTNode(ASTNodeType.ConditionalStatement);
            controlFlowNode.Value = new SymbolValue(statementType);
            
            // Add parsed arguments as children
            foreach (var arg in arguments)
            {
                controlFlowNode.Children.Add(arg);
            }
            
            return controlFlowNode;
        }
        
        /// <summary>
        /// Parse arguments inside brackets (separated by semicolons)
        /// </summary>
        private List<ASTNode> ParseBracketArguments(List<Token> contentTokens)
        {
            var arguments = new List<ASTNode>();
            var currentArgTokens = new List<Token>();
            
            foreach (var token in contentTokens)
            {
                if (token.Type == TokenType.SEMICOLON)
                {
                    // End of current argument
                    if (currentArgTokens.Count > 0)
                    {
                        var argNode = ParseArgument(currentArgTokens);
                        if (argNode != null)
                            arguments.Add(argNode);
                        currentArgTokens.Clear();
                    }
                }
                else
                {
                    currentArgTokens.Add(token);
                }
            }
            
            // Add the last argument
            if (currentArgTokens.Count > 0)
            {
                var argNode = ParseArgument(currentArgTokens);
                if (argNode != null)
                    arguments.Add(argNode);
            }
            
            return arguments;
        }
        
        /// <summary>
        /// Parse a single argument expression
        /// </summary>
        private ASTNode? ParseArgument(List<Token> argTokens)
        {
            if (argTokens.Count == 0)
                return null;
                
            if (argTokens.Count == 1)
                return CreateNodeFromToken(argTokens[0]);
            
            // Use the parent parser to handle complex expressions
            return parentParser.BuildParseTreeFromRight(argTokens);
        }
        
        /// <summary>
        /// Find the assignment operator in the token list
        /// </summary>
        private int FindAssignmentOperator(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == TokenType.COLON ||
                    tokens[i].Type == TokenType.ASSIGNMENT ||
                    tokens[i].Type == TokenType.GLOBAL_ASSIGNMENT)
                {
                    return i;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Parse variable name from left side tokens
        /// </summary>
        private ASTNode? ParseVariableName(List<Token> leftTokens)
        {
            // Variable name should be a single identifier or symbol
            if (leftTokens.Count == 1)
            {
                var token = leftTokens[0];
                if (token.Type == TokenType.IDENTIFIER || token.Type == TokenType.SYMBOL)
                {
                    return new ASTNode(ASTNodeType.Variable)
                    {
                        Value = new SymbolValue(token.Lexeme)
                    };
                }
            }
            return null;
        }
        
        /// <summary>
        /// Parse right side expression of assignment
        /// </summary>
        private ASTNode? ParseRightSideExpression(List<Token> rightTokens)
        {
            if (rightTokens.Count == 1)
                return CreateNodeFromToken(rightTokens[0]);
            
            // Use the parent parser to handle complex expressions
            return parentParser.BuildParseTreeFromRight(rightTokens);
        }
        
        /// <summary>
        /// Check if this is a terminal (pure) assignment
        /// Terminal assignment returns null, inline assignment returns the assigned value
        /// </summary>
        private bool IsTerminalAssignment(List<Token> tokens)
        {
            // For a proper implementation, we need to analyze the context
            // to determine if there are verbs to the left of this assignment.
            // 
            // Terminal (pure) assignment: no verbs to the left between assignment and separator
            // Inline assignment: one or more verbs to the left between assignment and separator
            //
            // Since we don't have the full context here, we'll use a heuristic:
            // If the assignment starts at the beginning of the token list or is preceded by a separator,
            // it's likely terminal. Otherwise, it's likely inline.
            
            // For now, we'll implement a simplified check based on position in the expression
            // TODO: Implement proper context analysis using parent parser context
            
            // Simplified heuristic: if the assignment is the first token in a simple expression,
            // treat it as terminal. This is not perfect but handles common cases.
            return tokens.Count <= 3; // Simple heuristic: var: value is terminal
        }
        
        /// <summary>
        /// Create AST node from atomic token
        /// </summary>
        private ASTNode CreateNodeFromToken(Token token)
        {
            return LRSAtomicParser.ParseAtomicToken(token);
        }
    }
}
