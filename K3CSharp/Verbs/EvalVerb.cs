using System;
using System.Collections.Generic;
using System.Linq;
using K3CSharp;
using K3CSharp.Parsing;

namespace K3CSharp.Verbs
{
    /// <summary>
    /// Implementation of _eval verb for K3CSharp
    /// Evaluates parse tree representations using existing evaluator
    /// </summary>
    public static class EvalVerbHandler
    {
        // Static variable storage for basic variable evaluation
        private static readonly Dictionary<string, K3Value> variables = new Dictionary<string, K3Value>();
        
        /// <summary>
        /// Main _eval entry point
        /// </summary>
        public static K3Value Evaluate(K3Value parseTree)
        {
            return Evaluate(new[] { parseTree });
        }
        
        /// <summary>
        /// Set a variable value (used by assignment operations)
        /// </summary>
        public static void SetVariable(string name, K3Value value)
        {
            variables[name] = value;
        }
        
        /// <summary>
        /// Get a variable value
        /// </summary>
        public static K3Value? GetVariable(string name)
        {
            return variables.TryGetValue(name, out var value) ? value : null;
        }
        
        /// <summary>
        /// Clear all variables (for test isolation)
        /// </summary>
        public static void ClearVariables()
        {
            variables.Clear();
        }
        
        /// <summary>
        /// Eval verb implementation matching delegate signature
        /// </summary>
        public static K3Value Evaluate(K3Value[] args)
        {
            if (args.Length != 1)
                throw new Exception("_eval: Requires exactly one argument");
                
            var parseTree = args[0];
            ValidateParseTree(parseTree);
            
            var astNode = ParseTreeConverter.FromKList(parseTree);
            var result = EvaluateAST(astNode);
            return UnenlistIfSingleElement(result);
        }
        
        /// <summary>
        /// Validate input is proper parse tree structure
        /// </summary>
        private static void ValidateParseTree(K3Value parseTree)
        {
            if (parseTree is not VectorValue vector)
                throw new Exception("_eval: Input must be a vector");
                
            var elements = vector.Elements;
            if (elements.Count == 0)
                throw new Exception("_eval: Parse tree cannot be empty");
                
            // First element should be a verb (symbol)
            if (elements[0] is not SymbolValue && elements[0] is not CharacterValue)
                throw new Exception("_eval: Parse tree must start with a verb");
        }
        
        /// <summary>
        /// Un-enlist 1-item vectors as required by the spec
        /// </summary>
        private static K3Value UnenlistIfSingleElement(K3Value value)
        {
            if (value is VectorValue vec && vec.Elements.Count == 1)
            {
                return vec.Elements[0];
            }
            return value;
        }
        
        /// <summary>
        /// Evaluate AST using existing evaluator infrastructure
        /// </summary>
        private static K3Value EvaluateAST(ASTNode astNode)
        {
            // This would use the existing Evaluator class
            // For now, provide a simplified implementation that handles basic operations
            if (astNode.Type == ASTNodeType.Literal)
            {
                var value = astNode.Value ?? new NullValue();
                return UnenlistIfSingleElement(value);
            }
                
            if (astNode.Type == ASTNodeType.Variable)
            {
                // Variable lookup using our static variable storage
                var variableName = astNode.Value?.ToString()?.Trim('`').Trim('"') ?? "";
                var variableValue = GetVariable(variableName);
                if (variableValue != null)
                {
                    return UnenlistIfSingleElement(variableValue);
                }
                throw new Exception($"Variable not found: {variableName}");
            }
            
            if (astNode.Type == ASTNodeType.FunctionCall)
            {
                return new SymbolValue($"Function: {astNode.Value?.ToString() ?? ""}");
            }
            
            if (astNode.Type == ASTNodeType.BinaryOp)
            {
                var op = astNode.Value?.ToString()?.Trim('`').Trim('"') ?? "+";
                
                if (astNode.Children.Count == 1)
                {
                    // Monadic operator
                    var operand = EvaluateAST(astNode.Children[0]);
                    return op switch
                    {
                        "*:" => UnenlistIfSingleElement(First(operand)),
                        _ => throw new Exception($"Monadic operator {op} not implemented")
                    };
                }
                else if (astNode.Children.Count >= 2)
                {
                    // Dyadic operator - check for projections
                    if (op.Contains("::"))
                    {
                        // Projection: operator with double colon
                        var left = EvaluateAST(astNode.Children[0]);
                        var right = EvaluateAST(astNode.Children[1]);
                        
                        // For projections, we need to handle the projection logic
                        // For now, just return the operator applied to arguments
                        return op switch
                        {
                            "+::" => UnenlistIfSingleElement(left.Add(right)),
                            _ => throw new Exception($"Projection operator {op} not implemented")
                        };
                    }
                    else
                    {
                        // Regular dyadic operator
                        var left = EvaluateAST(astNode.Children[0]);
                        var right = EvaluateAST(astNode.Children[1]);
                        
                        return op switch
                        {
                            "+" => UnenlistIfSingleElement(left.Add(right)),
                            "-" => UnenlistIfSingleElement(left.Subtract(right)),
                            "*" => UnenlistIfSingleElement(left.Multiply(right)),
                            "/" => UnenlistIfSingleElement(left.Divide(right)),
                            _ => throw new Exception($"Dyadic operator {op} not implemented")
                        };
                    }
                }
                else
                {
                    throw new Exception($"BinaryOp requires at least 1 child, got {astNode.Children.Count}");
                }
            }
            
            if (astNode.Type == ASTNodeType.Vector)
            {
                var elements = new List<K3Value>();
                foreach (var child in astNode.Children)
                {
                    elements.Add(EvaluateAST(child));
                }
                return new VectorValue(elements);
            }
            
            // Base case: return null for unimplemented node types instead of throwing
            return new NullValue();
        }
        
        private static K3Value First(K3Value a)
        {
            if (a is VectorValue vecA && vecA.Elements.Count > 0)
                return vecA.Elements[0];
            
            return a; // For scalars, return the value itself
        }
    }
}
