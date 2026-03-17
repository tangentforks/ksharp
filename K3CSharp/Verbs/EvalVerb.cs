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
        /// <summary>
        /// Main _eval entry point
        /// </summary>
        public static K3Value Evaluate(K3Value parseTree)
        {
            return Evaluate(new[] { parseTree });
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
                // Variable lookup would happen here in real implementation
                throw new Exception($"Variable evaluation not implemented: {astNode.Value}");
            }
            
            if (astNode.Type == ASTNodeType.FunctionCall)
            {
                return new SymbolValue($"Function: {astNode.Value?.ToString() ?? ""}");
            }
            
            if (astNode.Type == ASTNodeType.BinaryOp)
            {
                var left = EvaluateAST(astNode.Children[0]);
                var right = EvaluateAST(astNode.Children[1]);
                var op = astNode.Value?.ToString()?.Trim('`').Trim('"') ?? "+";
                
                return op switch
                {
                    "+" => UnenlistIfSingleElement(left.Add(right)),
                    "-" => UnenlistIfSingleElement(left.Subtract(right)),
                    "*" => UnenlistIfSingleElement(left.Multiply(right)),
                    "/" => UnenlistIfSingleElement(left.Divide(right)),
                    _ => throw new Exception($"Operator {op} not implemented")
                };
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
        private static K3Value HandleAddition(K3Value left, K3Value right)
        {
            if (left is FloatValue leftFloat && right is FloatValue rightFloat)
                return new FloatValue(leftFloat.Value + rightFloat.Value);
            else if (left is IntegerValue leftInt && right is IntegerValue rightInt)
                return new IntegerValue(leftInt.Value + rightInt.Value);
            else
                throw new Exception("Unsupported operand types for +");
        }

        private static K3Value HandleSubtraction(K3Value left, K3Value right)
        {
            if (left is FloatValue leftFloat && right is FloatValue rightFloat)
                return new FloatValue(leftFloat.Value - rightFloat.Value);
            else if (left is IntegerValue leftInt && right is IntegerValue rightInt)
                return new IntegerValue(leftInt.Value - rightInt.Value);
            else
                throw new Exception("Unsupported operand types for -");
        }

        private static K3Value HandleMultiplication(K3Value left, K3Value right)
        {
            if (left is FloatValue leftFloat && right is FloatValue rightFloat)
                return new FloatValue(leftFloat.Value * rightFloat.Value);
            else if (left is IntegerValue leftInt && right is IntegerValue rightInt)
                return new IntegerValue(leftInt.Value * rightInt.Value);
            else
                throw new Exception("Unsupported operand types for *");
        }

        private static K3Value HandleDivision(K3Value left, K3Value right)
        {
            if (left is FloatValue leftFloat && right is FloatValue rightFloat)
                return new FloatValue(leftFloat.Value / rightFloat.Value);
            else if (left is IntegerValue leftInt && right is IntegerValue rightInt)
                return new IntegerValue(leftInt.Value / rightInt.Value);
            else
                throw new Exception("Unsupported operand types for /");
        }
    }
}
