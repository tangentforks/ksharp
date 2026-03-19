using System;
using System.Collections.Generic;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// LRS Parse Tree Evaluator - Direct evaluation of parse trees without re-parsing
    /// Supports verb-agnostic evaluation using VerbRegistry
    /// </summary>
    public class LRSParseTreeEvaluator
    {
        private readonly Evaluator evaluator;
        private readonly bool enableDebugging;
        
        public LRSParseTreeEvaluator(Evaluator evaluator, bool enableDebugging = false)
        {
            this.evaluator = evaluator;
            this.enableDebugging = enableDebugging;
        }
        
        /// <summary>
        /// Evaluate AST node directly without re-parsing
        /// </summary>
        public K3Value? EvaluateTree(ASTNode node)
        {
            if (enableDebugging)
            {
                Console.WriteLine($"Evaluating node: {node.Type} with value: {node.Value}");
            }
            
            return node.Type switch
            {
                ASTNodeType.Literal => EvaluateLiteral(node),
                ASTNodeType.Vector => EvaluateVector(node),
                ASTNodeType.BinaryOp => EvaluateBinaryOp(node),
                ASTNodeType.Variable => EvaluateVariable(node),
                ASTNodeType.Function => EvaluateFunction(node),
                ASTNodeType.FunctionCall => EvaluateFunctionCall(node),
                ASTNodeType.Assignment => EvaluateAssignment(node),
                ASTNodeType.Block => EvaluateBlock(node),
                _ => throw new NotSupportedException($"Node type {node.Type} not supported")
            };
        }
        
        /// <summary>
        /// Evaluate literal node
        /// </summary>
        private K3Value? EvaluateLiteral(ASTNode node)
        {
            return node.Value;
        }
        
        /// <summary>
        /// Evaluate vector node
        /// </summary>
        private K3Value? EvaluateVector(ASTNode node)
        {
            var elements = new List<K3Value>();
            foreach (var child in node.Children)
            {
                var element = EvaluateTree(child);
                if (element != null)
                {
                    elements.Add(element);
                }
            }
            return new VectorValue(elements);
        }
        
        /// <summary>
        /// Evaluate binary operation node (verb-agnostic)
        /// </summary>
        private K3Value? EvaluateBinaryOp(ASTNode node)
        {
            if (node.Value is not SymbolValue operatorSymbol)
                throw new Exception("Binary operation node must have symbol value");
                
            var operatorName = operatorSymbol.Value;
            
            // Get verb information from VerbRegistry
            var verbInfo = VerbRegistry.GetVerb(operatorName);
            if (verbInfo == null)
                throw new Exception($"Unknown operator: {operatorName}");
                
            // Evaluate operands
            if (node.Children.Count < 2)
                throw new Exception($"Binary operation {operatorName} requires 2 operands");
                
            var left = EvaluateTree(node.Children[0]);
            var right = EvaluateTree(node.Children[1]);
            
            if (left == null || right == null)
                throw new Exception($"Failed to evaluate operands for {operatorName}");
                
            // Create arguments array for verb execution
            var args = new K3Value[] { left, right };
            
            // Execute verb using VerbRegistry
            return ExecuteVerb(verbInfo, args);
        }
        
        /// <summary>
        /// Evaluate variable node
        /// </summary>
        private K3Value? EvaluateVariable(ASTNode node)
        {
            if (node.Value is not SymbolValue variableName)
                throw new Exception("Variable node must have symbol value");
                
            // Check if it's a system variable
            if (VerbRegistry.IsSystemVariable(variableName.Value))
            {
                return evaluator.GetVariableValuePublic(variableName.Value);
            }
            
            // Regular variable lookup
            return evaluator.GetVariableValuePublic(variableName.Value);
        }
        
        /// <summary>
        /// Evaluate function node
        /// </summary>
        private K3Value? EvaluateFunction(ASTNode node)
        {
            // Functions are typically evaluated when called
            // Return a function value that can be called later
            if (node.Value is not SymbolValue functionName)
                throw new Exception("Function node must have symbol value");
                
            var bodyText = node.Children.Count > 0 ? node.Children[0].Value?.ToString() ?? "" : "";
            return new FunctionValue(bodyText, new List<string> { functionName.Value });
        }
        
        /// <summary>
        /// Evaluate function call node
        /// </summary>
        private K3Value? EvaluateFunctionCall(ASTNode node)
        {
            if (node.Children.Count == 0)
                throw new Exception("Function call requires function");
                
            // Evaluate function
            var function = EvaluateTree(node.Children[0]);
            if (function == null)
                throw new Exception("Failed to evaluate function");
                
            // Evaluate arguments
            var args = new List<K3Value>();
            for (int i = 1; i < node.Children.Count; i++)
            {
                var arg = EvaluateTree(node.Children[i]);
                if (arg != null)
                {
                    args.Add(arg);
                }
            }
            
            // Call function
            if (function is FunctionValue func)
            {
                // For now, just return the function value itself
                // In a full implementation, we would evaluate the function with arguments
                return func;
            }
            
            throw new Exception($"Unsupported function type: {function.GetType()}");
        }
        
        /// <summary>
        /// Evaluate assignment node
        /// </summary>
        private K3Value? EvaluateAssignment(ASTNode node)
        {
            if (node.Value is not SymbolValue variableName)
                throw new Exception("Assignment node must have symbol value");
                
            if (node.Children.Count < 1)
                throw new Exception("Assignment requires value");
                
            var value = EvaluateTree(node.Children[0]);
            if (value == null)
                throw new Exception("Failed to evaluate assignment value");
                
            // Set variable value using reflection to access private SetVariable method
            var setVariableMethod = typeof(Evaluator).GetMethod("SetVariable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (setVariableMethod != null)
            {
                setVariableMethod.Invoke(evaluator, new object[] { variableName.Value, value });
            }
            else
            {
                throw new Exception("Cannot access SetVariable method");
            }
            
            return value;
        }
        
        /// <summary>
        /// Evaluate block node (multiple statements)
        /// </summary>
        private K3Value? EvaluateBlock(ASTNode node)
        {
            K3Value? result = null;
            foreach (var child in node.Children)
            {
                result = EvaluateTree(child);
            }
            return result; // Return value of last statement
        }
        
        /// <summary>
        /// Execute verb using VerbRegistry (verb-agnostic)
        /// </summary>
        private K3Value? ExecuteVerb(VerbInfo verbInfo, K3Value[] args)
        {
            // Check arity
            if (verbInfo.SupportedArities.Length == 0)
                throw new Exception($"Verb {verbInfo.Name} has no supported arities");
                
            // Find appropriate implementation
            var arity = args.Length;
            int arityIndex = Array.IndexOf(verbInfo.SupportedArities, arity);
            
            if (arityIndex < 0 || verbInfo.Implementations == null || arityIndex >= verbInfo.Implementations.Length)
                throw new Exception($"Verb {verbInfo.Name} does not support arity {arity}");
                
            var implementation = verbInfo.Implementations[arityIndex];
            if (implementation == null)
                throw new Exception($"Verb {verbInfo.Name} has no implementation for arity {arity}");
                
            // Execute verb
            try
            {
                return implementation(args);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing verb {verbInfo.Name} with arity {arity}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get evaluation statistics for debugging
        /// </summary>
        public EvaluationStats GetEvaluationStats(ASTNode node)
        {
            var stats = new EvaluationStats
            {
                NodeCount = CountNodes(node),
                NodeTypes = GetNodeTypes(node),
                VerbCount = CountVerbs(node)
            };
            
            return stats;
        }
        
        /// <summary>
        /// Count total nodes in tree
        /// </summary>
        private int CountNodes(ASTNode node)
        {
            int count = 1;
            foreach (var child in node.Children)
            {
                count += CountNodes(child);
            }
            return count;
        }
        
        /// <summary>
        /// Get all node types in tree
        /// </summary>
        private HashSet<ASTNodeType> GetNodeTypes(ASTNode node)
        {
            var types = new HashSet<ASTNodeType> { node.Type };
            foreach (var child in node.Children)
            {
                var childTypes = GetNodeTypes(child);
                types.UnionWith(childTypes);
            }
            return types;
        }
        
        /// <summary>
        /// Count verb operations in tree
        /// </summary>
        private int CountVerbs(ASTNode node)
        {
            int count = 0;
            
            if (node.Type == ASTNodeType.BinaryOp)
            {
                count++;
            }
            
            foreach (var child in node.Children)
            {
                count += CountVerbs(child);
            }
            
            return count;
        }
    }
    
    /// <summary>
    /// Evaluation statistics for debugging and monitoring
    /// </summary>
    public class EvaluationStats
    {
        public int NodeCount { get; set; }
        public HashSet<ASTNodeType> NodeTypes { get; set; } = new HashSet<ASTNodeType>();
        public int VerbCount { get; set; }
        
        public override string ToString()
        {
            return $"Nodes: {NodeCount}, Types: {string.Join(", ", NodeTypes)}, Verbs: {VerbCount}";
        }
    }
}
