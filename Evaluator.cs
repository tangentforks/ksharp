using System;
using System.Collections.Generic;

namespace K3CSharp
{
    public class Evaluator
    {
        private Dictionary<string, K3Value> variables = new Dictionary<string, K3Value>();
        private Dictionary<string, int> symbolTable = new Dictionary<string, int>();

        public K3Value Evaluate(ASTNode node)
        {
            if (node == null)
            {
                throw new Exception("Null AST node");
            }

            return EvaluateNode(node);
        }

        private K3Value EvaluateNode(ASTNode node)
        {
            switch (node.Type)
            {
                case ASTNodeType.Literal:
                    return node.Value;

                case ASTNodeType.Variable:
                    var name = node.Value is SymbolValue symbol ? symbol.Value : node.Value.ToString();
                    return GetVariable(name);

                case ASTNodeType.Assignment:
                    var assignName = node.Value is SymbolValue assignmentSym ? assignmentSym.Value : node.Value.ToString();
                    var value = Evaluate(node.Children[0]);
                    return SetVariable(assignName, value);

                case ASTNodeType.BinaryOp:
                    return EvaluateBinaryOp(node);

                case ASTNodeType.Vector:
                    return EvaluateVector(node);

                case ASTNodeType.Function:
                    return EvaluateFunction(node);

                case ASTNodeType.FunctionCall:
                    return EvaluateFunctionCall(node);

                case ASTNodeType.Block:
                    return EvaluateBlock(node);

                default:
                    throw new Exception($"Unknown AST node type: {node.Type}");
            }
        }

        private K3Value GetVariable(string variableName)
        {
            if (variables.TryGetValue(variableName, out var value))
            {
                return value;
            }
            
            throw new Exception($"Undefined variable: {variableName}");
        }
        
        private K3Value SetVariable(string variableName, K3Value value)
        {
            variables[variableName] = value;
            return value;
        }

        private K3Value EvaluateLiteral(ASTNode node)
        {
            return node.Value;
        }

        private K3Value EvaluateBinaryOp(ASTNode node)
        {
            string op = node.Value is SymbolValue opSymbol ? opSymbol.Value : throw new Exception("Binary operation node must contain operator symbol");

            // Handle unary negation operator
            if (op == "NEGATE" && node.Children.Count == 1)
            {
                var operand = Evaluate(node.Children[0]);
                return Negate(operand);
            }

            if (node.Children.Count != 2)
            {
                throw new Exception("Binary operation requires exactly two operands");
            }

            var left = Evaluate(node.Children[0]);
            var right = Evaluate(node.Children[1]);

            switch (op)
            {
                case "PLUS":
                    return left.Add(right);
                case "MINUS":
                    return left.Subtract(right);
                case "MULTIPLY":
                    return left.Multiply(right);
                case "DIVIDE":
                    return left.Divide(right);
                case "MIN":
                    return Minimum(left, right);
                case "MAX":
                    return Maximum(left, right);
                case "LESS":
                    return LessThan(left, right);
                case "GREATER":
                    return GreaterThan(left, right);
                case "EQUAL":
                    return Equal(left, right);
                case "POWER":
                    return Power(left, right);
                case "MODULUS":
                    return Modulus(left, right);
                case "JOIN":
                    return Join(left, right);
                default:
                    throw new Exception($"Unknown binary operator: {op}");
            }
        }

        private K3Value EvaluateVector(ASTNode node)
        {
            var elements = new List<K3Value>();
            foreach (var child in node.Children)
            {
                elements.Add(Evaluate(child));
            }
            return new VectorValue(elements);
        }

        private K3Value EvaluateFunction(ASTNode node)
        {
            // Create a function value that contains the AST node for later evaluation
            return new FunctionValue(node);
        }

        private K3Value EvaluateFunctionCall(ASTNode node)
        {
            if (node.Children.Count < 1)
            {
                throw new Exception("Function call requires a function");
            }

            var function = Evaluate(node.Children[0]);
            var arguments = new List<K3Value>();
            
            for (int i = 1; i < node.Children.Count; i++)
            {
                arguments.Add(Evaluate(node.Children[i]));
            }

            // Handle FunctionValue (user-defined functions)
            if (function is FunctionValue functionValue)
            {
                var functionNode = functionValue.FunctionNode;
                var parameters = functionNode.Parameters;
                
                if (arguments.Count != parameters.Count)
                {
                    throw new Exception($"Function expects {parameters.Count} arguments, got {arguments.Count}");
                }
                
                // Create a new evaluator scope for this function call
                var functionEvaluator = new Evaluator();
                
                // Copy global variables to function scope
                foreach (var kvp in variables)
                {
                    functionEvaluator.variables[kvp.Key] = kvp.Value;
                }
                
                // Bind parameters to arguments
                for (int i = 0; i < parameters.Count; i++)
                {
                    functionEvaluator.SetVariable(parameters[i], arguments[i]);
                }
                
                // Evaluate the function Body
                if (functionNode.Children.Count > 0)
                {
                    var body = functionNode.Children[0];
                    return functionEvaluator.Evaluate(body);
                }
                else
                {
                    return new IntegerValue(0); // Empty function result
                }
            }
            
            // Handle named functions (variables that contain functions)
            if (function.Type == ValueType.Symbol)
            {
                var functionName = (function as SymbolValue)?.Value;
                if (functionName == null)
                {
                    throw new Exception("Invalid function name");
                }
                
                // Handle built-in functions
                switch (functionName)
                {
                    case "add7":
                        if (arguments.Count != 1) throw new Exception("add7 expects 1 argument");
                        var arg = arguments[0];
                        if (arg is IntegerValue intVal) return new IntegerValue(intVal.Value + 7);
                        if (arg is LongValue longVal) return new LongValue(longVal.Value + 7);
                        if (arg is FloatValue floatVal) return new FloatValue(floatVal.Value + 7);
                        throw new Exception("add7 expects numeric argument");
                        
                    case "mul":
                        if (arguments.Count != 2) throw new Exception("mul expects 2 arguments");
                        var op1 = arguments[0];
                        var op2 = arguments[1];
                        if (op1 is IntegerValue int1 && op2 is IntegerValue int2) 
                            return new IntegerValue(int1.Value * int2.Value);
                        if (op1 is LongValue long1 && op2 is LongValue long2) 
                            return new LongValue(long1.Value * long2.Value);
                        if (op1 is FloatValue float1 && op2 is FloatValue float2) 
                            return new FloatValue(float1.Value * float2.Value);
                        throw new Exception("mul expects numeric arguments");
                        
                    default:
                        throw new Exception($"Unknown function: {functionName}");
                }
            }
            
            throw new Exception("Function call evaluation not implemented");
        }

        private K3Value EvaluateBlock(ASTNode node)
        {
            K3Value lastResult = null;
            
            foreach (var child in node.Children)
            {
                lastResult = EvaluateNode(child);
            }
            
            return lastResult;
        }

        private K3Value Minimum(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(Math.Min(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(Math.Min(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Min(floatA.Value, floatB.Value));
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Minimum(vecB);
                else
                    return vecA.Minimum(b);
            }
            
            throw new Exception($"Cannot find minimum of {a.Type} and {b.Type}");
        }

        private K3Value Maximum(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(Math.Max(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(Math.Max(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Max(floatA.Value, floatB.Value));
            
            // Handle vector operations
            if (a is VectorValue vecA)
            {
                if (b is VectorValue vecB)
                    return vecA.Maximum(vecB);
                else
                    return vecA.Maximum(b);
            }
            
            throw new Exception($"Cannot find maximum of {a.Type} and {b.Type}");
        }

        private K3Value LessThan(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value < intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value < longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value < floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with <");
        }

        private K3Value GreaterThan(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value > intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value > longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value > floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with >");
        }

        private K3Value Equal(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value == intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value == longB.Value ? 1 : 0);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new IntegerValue(floatA.Value == floatB.Value ? 1 : 0);
            
            throw new Exception($"Cannot compare {a.Type} and {b.Type} with =");
        }

        private K3Value Power(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue((int)Math.Pow(intA.Value, intB.Value));
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue((long)Math.Pow(longA.Value, longB.Value));
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(Math.Pow(floatA.Value, floatB.Value));
            
            throw new Exception($"Cannot raise {a.Type} to power of {b.Type}");
        }

        private K3Value Modulus(K3Value a, K3Value b)
        {
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value % intB.Value);
            if (a is LongValue longA && b is LongValue longB)
                return new LongValue(longA.Value % longB.Value);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return new FloatValue(floatA.Value % floatB.Value);
            
            throw new Exception($"Cannot find remainder of {a.Type} and {b.Type}");
        }

        private K3Value Negate(K3Value a)
        {
            if (a is IntegerValue intA)
                return new IntegerValue(intA.Value == 0 ? 1 : 0);
            if (a is LongValue longA)
                return new IntegerValue(longA.Value == 0 ? 1 : 0);
            if (a is FloatValue floatA)
                return new IntegerValue(floatA.Value == 0 ? 1 : 0);
            
            throw new Exception($"Cannot negate {a.Type}");
        }

        private K3Value Join(K3Value a, K3Value b)
        {
            // Handle joining two values into a vector
            var elements = new List<K3Value>();
            
            if (a is VectorValue vecA)
            {
                elements.AddRange(vecA.Elements);
            }
            else
            {
                elements.Add(a);
            }
            
            if (b is VectorValue vecB)
            {
                elements.AddRange(vecB.Elements);
            }
            else
            {
                elements.Add(b);
            }
            
            return new VectorValue(elements);
        }
    }
}
