using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Tree evaluator for LRS parser
    /// Evaluates AST nodes directly without re-parsing
    /// Supports lazy evaluation and tree optimization
    /// </summary>
    public class LRSTreeEvaluator
    {
        private readonly Evaluator evaluator;
        
        public LRSTreeEvaluator()
        {
            this.evaluator = new Evaluator();
        }

        /// <summary>
        /// Evaluate AST node directly
        /// </summary>
        /// <param name="node">AST node to evaluate</param>
        /// <returns>Evaluated K3Value</returns>
        public K3Value? EvaluateTree(ASTNode? node)
        {
            if (node == null)
                return null;
                
            return EvaluateNodeRecursive(node);
        }

        /// <summary>
        /// Evaluate node recursively with optimization
        /// </summary>
        private K3Value? EvaluateNodeRecursive(ASTNode node)
        {
            return node.Type switch
            {
                ASTNodeType.Literal => node.Value,
                
                ASTNodeType.Variable => EvaluateVariable(node),
                
                ASTNodeType.BinaryOp => EvaluateBinaryOperation(node),
                
                ASTNodeType.Vector => EvaluateVector(node),
                
                ASTNodeType.Function => EvaluateFunction(node),
                
                ASTNodeType.FunctionCall => EvaluateFunctionCall(node),
                
                ASTNodeType.ProjectedFunction => EvaluateProjectedFunction(node),
                
                _ => throw new Exception($"Unsupported AST node type: {node.Type}")
            };
        }

        /// <summary>
        /// Evaluate variable node
        /// </summary>
        private K3Value? EvaluateVariable(ASTNode node)
        {
            if (node.Value is SymbolValue symbol)
            {
                return evaluator.GetVariableValuePublic(symbol.Value);
            }
            
            throw new Exception($"Invalid variable node: {node.Value}");
        }

        /// <summary>
        /// Evaluate binary operation
        /// </summary>
        private K3Value? EvaluateBinaryOperation(ASTNode node)
        {
            if (node.Children.Count < 2)
                throw new Exception($"Binary operation requires 2 operands, got {node.Children.Count}");
                
            var left = EvaluateNodeRecursive(node.Children[0]);
            var right = EvaluateNodeRecursive(node.Children[1]);
            
            if (left == null || right == null)
                return null;
                
            // Get operator symbol from node value
            var operatorSymbol = GetOperatorSymbol(node);
            var verb = VerbRegistry.GetVerb(operatorSymbol);
            
            if (verb?.Implementations?[2] != null)
            {
                return verb.Implementations[2]!(new K3Value[] { left, right });
            }
            
            throw new Exception($"Unknown binary operator: {operatorSymbol}");
        }

        /// <summary>
        /// Evaluate vector node
        /// </summary>
        private K3Value? EvaluateVector(ASTNode node)
        {
            var elements = new List<K3Value>();
            
            foreach (var child in node.Children)
            {
                var evaluated = EvaluateNodeRecursive(child);
                if (evaluated != null)
                    elements.Add(evaluated);
            }
            
            return new VectorValue(elements);
        }

        /// <summary>
        /// Evaluate function node
        /// </summary>
        private K3Value? EvaluateFunction(ASTNode node)
        {
            // For now, delegate to evaluator
            // In a more complete implementation, this would handle lambda evaluation
            return evaluator.Evaluate(node);
        }

        /// <summary>
        /// Evaluate function call node
        /// </summary>
        private K3Value? EvaluateFunctionCall(ASTNode node)
        {
            if (node.Children.Count == 0)
                throw new Exception("Function call requires function name");
                
            var functionName = GetFunctionName(node);
            var arguments = new List<K3Value>();
            
            // Evaluate arguments
            for (int i = 1; i < node.Children.Count; i++)
            {
                var arg = EvaluateNodeRecursive(node.Children[i]);
                if (arg != null)
                    arguments.Add(arg);
            }
            
            // Call function through evaluator
            return evaluator.CallVariableFunction(functionName, arguments);
        }

        /// <summary>
        /// Evaluate projected function
        /// </summary>
        private K3Value? EvaluateProjectedFunction(ASTNode node)
        {
            // For now, delegate to evaluator
            // In a more complete implementation, this would handle projection evaluation
            return evaluator.Evaluate(node);
        }

        /// <summary>
        /// Get operator symbol from node
        /// </summary>
        private string GetOperatorSymbol(ASTNode node)
        {
            if (node.Value is SymbolValue symbol)
            {
                // Remove precedence annotations if present
                var value = symbol.Value;
                var bracketIndex = value.IndexOf('[');
                if (bracketIndex != -1)
                {
                    value = value.Substring(0, bracketIndex);
                }
                return value;
            }
            
            throw new Exception($"Cannot extract operator symbol from: {node.Value}");
        }

        /// <summary>
        /// Get function name from function call node
        /// </summary>
        private string GetFunctionName(ASTNode node)
        {
            if (node.Children.Count > 0 && node.Children[0].Value is SymbolValue symbol)
            {
                return symbol.Value;
            }
            
            throw new Exception($"Cannot extract function name from: {node.Value}");
        }

        /// <summary>
        /// Optimize tree for evaluation
        /// </summary>
        public ASTNode? OptimizeTree(ASTNode? node)
        {
            if (node == null)
                return null;
                
            // Apply basic optimizations
            return OptimizeNodeRecursive(node);
        }

        /// <summary>
        /// Recursively optimize nodes
        /// </summary>
        private ASTNode? OptimizeNodeRecursive(ASTNode node)
        {
            switch (node.Type)
            {
                case ASTNodeType.Literal:
                    // Literals are already optimal
                    return node;
                    
                case ASTNodeType.Vector:
                    return OptimizeVector(node);
                    
                case ASTNodeType.BinaryOp:
                    return OptimizeBinaryOperation(node);
                    
                default:
                    // For other types, try to optimize children
                    var optimizedChildren = new List<ASTNode>();
                    foreach (var child in node.Children)
                    {
                        var optimized = OptimizeNodeRecursive(child);
                        optimizedChildren.Add(optimized ?? child);
                    }
                    
                    if (optimizedChildren.SequenceEqual(node.Children))
                        return node; // No optimization needed
                        
                    var optimizedNode = new ASTNode(node.Type, node.Value, optimizedChildren);
                    optimizedNode.StartPosition = node.StartPosition;
                    optimizedNode.EndPosition = node.EndPosition;
                    return optimizedNode;
            }
        }

        /// <summary>
        /// Optimize vector node
        /// </summary>
        private ASTNode OptimizeVector(ASTNode node)
        {
            // Check if vector can be collapsed to a simpler form
            if (node.Children.Count == 0)
                return node;
                
            // Check if all children are literals of the same type
            if (node.Children.All(child => child.Type == ASTNodeType.Literal))
            {
                var values = node.Children.Select(child => child.Value).Where(v => v != null).ToList();
                return ASTNode.MakeVector(values.Select(v => ASTNode.MakeLiteral(v!)).ToList());
            }
            
            return node;
        }

        /// <summary>
        /// Optimize binary operation
        /// </summary>
        private ASTNode OptimizeBinaryOperation(ASTNode node)
        {
            // Check for constant folding
            if (node.Children.Count >= 2 &&
                node.Children[0].Type == ASTNodeType.Literal &&
                node.Children[1].Type == ASTNodeType.Literal)
            {
                // Try to evaluate constant operation
                try
                {
                    var result = EvaluateBinaryOperation(node);
                    if (result != null)
                    {
                        return ASTNode.MakeLiteral(result);
                    }
                }
                catch
                {
                    // If evaluation fails, return original node
                    return node;
                }
            }
            
            return node;
        }

        /// <summary>
        /// Get evaluation statistics
        /// </summary>
        public Dictionary<string, object> GetEvaluationStatistics(ASTNode? root)
        {
            var stats = new Dictionary<string, object>
            {
                ["nodeCount"] = 0,
                ["evaluationCount"] = 0,
                ["optimizationCount"] = 0,
                ["cacheHits"] = 0
            };
            
            if (root == null)
                return stats;
                
            CalculateEvaluationStats(root, stats);
            return stats;
        }

        /// <summary>
        /// Calculate evaluation statistics
        /// </summary>
        private void CalculateEvaluationStats(ASTNode node, Dictionary<string, object> stats)
        {
            stats["nodeCount"] = (int)stats["nodeCount"] + 1;
            stats["evaluationCount"] = (int)stats["evaluationCount"] + 1;
            
            foreach (var child in node.Children)
            {
                CalculateEvaluationStats(child, stats);
            }
        }
    }
}
