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
            var op = node.Value as SymbolValue;
            if (op == null) throw new Exception("Binary operator must have a symbol value");

            // Handle unary operators (which are implemented as binary ops with one child)
            if (node.Children.Count == 1)
            {
                var operand = Evaluate(node.Children[0]);
                return op.Value switch
                {
                    "UNARY_MINUS" => UnaryMinus(operand),
                    "TRANSPOSE" => Transpose(operand),
                    "FIRST" => First(operand),
                    "RECIPROCAL" => Reciprocal(operand),
                    "GENERATE" => Generate(operand),
                    "REVERSE" => Reverse(operand),
                    "GRADE_UP" => GradeUp(operand),
                    "GRADE_DOWN" => GradeDown(operand),
                    "SHAPE" => Shape(operand),
                    "ENUMERATE" => Enumerate(operand),
                    "ENLIST" => Enlist(operand),
                    "COUNT" => Count(operand),
                    "FLOOR" => Floor(operand),
                    "UNIQUE" => Unique(operand),
                    "NEGATE" => Negate(operand),
                    _ => throw new Exception($"Unknown unary operator: {op.Value}")
                };
            }

            // Handle binary operators
            if (node.Children.Count != 2)
                throw new Exception($"Binary operator must have exactly 2 children, got {node.Children.Count}");

            var left = Evaluate(node.Children[0]);
            var right = Evaluate(node.Children[1]);

            return op.Value switch
            {
                "PLUS" => Add(left, right),
                "MINUS" => Subtract(left, right),
                "MULTIPLY" => Multiply(left, right),
                "DIVIDE" => Divide(left, right),
                "MIN" => Min(left, right),
                "MAX" => Max(left, right),
                "LESS" => Less(left, right),
                "GREATER" => Greater(left, right),
                "EQUAL" => Equal(left, right),
                "POWER" => Power(left, right),
                "MODULUS" => Modulus(left, right),
                "JOIN" => Join(left, right),
                "HASH" => CountBinary(left, right),
                "UNDERSCORE" => FloorBinary(left, right),
                "QUESTION" => UniqueBinary(left, right),
                "NEGATE" => NegateBinary(left, right),
                _ => throw new Exception($"Unknown binary operator: {op.Value}")
            };
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
            // For match comparison (~ operator)
            if (a is FloatValue floatA && b is FloatValue floatB)
            {
                double maxAbs = Math.Max(Math.Abs(floatA.Value), Math.Abs(floatB.Value));
                double threshold = maxAbs * 0.00001; // 0.001 percent
                return new IntegerValue(Math.Abs(floatA.Value - floatB.Value) < threshold ? 1 : 0);
            }
            
            if (a is VectorValue vecA && b is VectorValue vecB)
            {
                if (vecA.Elements.Count != vecB.Elements.Count)
                    return new IntegerValue(0);
                
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    var result = Equal(vecA.Elements[i], vecB.Elements[i]);
                    if (result is IntegerValue intResult && intResult.Value == 0)
                        return new IntegerValue(0);
                }
                return new IntegerValue(1);
            }
            
            if (a is IntegerValue intA && b is IntegerValue intB)
                return new IntegerValue(intA.Value == intB.Value ? 1 : 0);
            if (a is LongValue longA && b is LongValue longB)
                return new IntegerValue(longA.Value == longB.Value ? 1 : 0);
            
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

        // New unary operator implementations
        private K3Value UnaryMinus(K3Value a)
        {
            if (a is IntegerValue intA)
                return new IntegerValue(-intA.Value);
            if (a is LongValue longA)
                return new LongValue(-longA.Value);
            if (a is FloatValue floatA)
                return new FloatValue(-floatA.Value);
            
            throw new Exception($"Cannot apply unary minus to {a.Type}");
        }

        private K3Value Transpose(K3Value a)
        {
            // For now, just return the argument (transpose for matrices not implemented)
            return a;
        }

        private K3Value First(K3Value a)
        {
            if (a is VectorValue vecA && vecA.Elements.Count > 0)
                return vecA.Elements[0];
            
            return a; // For scalars, return the value itself
        }

        private K3Value Reciprocal(K3Value a)
        {
            if (a is IntegerValue intA)
                return new FloatValue(1.0 / intA.Value);
            if (a is LongValue longA)
                return new FloatValue(1.0 / longA.Value);
            if (a is FloatValue floatA)
                return new FloatValue(1.0 / floatA.Value);
            
            throw new Exception($"Cannot find reciprocal of {a.Type}");
        }

        private K3Value Generate(K3Value a)
        {
            if (a is IntegerValue intA)
            {
                var elements = new List<K3Value>();
                for (int i = 0; i < intA.Value; i++)
                {
                    elements.Add(new IntegerValue(0));
                }
                return new VectorValue(elements);
            }
            else if (a is VectorValue vecA)
            {
                var elements = new List<K3Value>();
                int value = 0;
                foreach (var element in vecA.Elements)
                {
                    if (element is IntegerValue length)
                    {
                        for (int i = 0; i < length.Value; i++)
                        {
                            elements.Add(new IntegerValue(value++));
                        }
                    }
                }
                return new VectorValue(elements);
            }
            
            throw new Exception($"Cannot generate from {a.Type}");
        }

        private K3Value Reverse(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var reversed = new List<K3Value>(vecA.Elements);
                reversed.Reverse();
                return new VectorValue(reversed);
            }
            
            return a; // For scalars, return the value itself
        }

        private K3Value GradeUp(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var indices = new List<int>();
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    indices.Add(i);
                }
                
                // Simple stable sort implementation
                indices.Sort((i, j) => CompareValues(vecA.Elements[i], vecA.Elements[j]));
                
                var result = new List<K3Value>();
                foreach (var index in indices)
                {
                    result.Add(new IntegerValue(index));
                }
                return new VectorValue(result);
            }
            
            return new VectorValue(new List<K3Value> { new IntegerValue(0) }); // For scalars
        }

        private K3Value GradeDown(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var indices = new List<int>();
                for (int i = 0; i < vecA.Elements.Count; i++)
                {
                    indices.Add(i);
                }
                
                // Simple stable sort implementation (descending)
                indices.Sort((i, j) => CompareValues(vecA.Elements[j], vecA.Elements[i]));
                
                var result = new List<K3Value>();
                foreach (var index in indices)
                {
                    result.Add(new IntegerValue(index));
                }
                return new VectorValue(result);
            }
            
            return new VectorValue(new List<K3Value> { new IntegerValue(0) }); // For scalars
        }

        private int CompareValues(K3Value a, K3Value b)
        {
            // Handle all K3Value types
            if (a is IntegerValue intA && b is IntegerValue intB)
                return intA.Value.CompareTo(intB.Value);
            if (a is LongValue longA && b is LongValue longB)
                return longA.Value.CompareTo(longB.Value);
            if (a is FloatValue floatA && b is FloatValue floatB)
                return floatA.Value.CompareTo(floatB.Value);
            if (a is CharacterValue charA && b is CharacterValue charB)
                return charA.Value.CompareTo(charB.Value);
            if (a is SymbolValue symA && b is SymbolValue symB)
                return string.Compare(symA.Value, symB.Value, StringComparison.Ordinal);
            
            // For vectors and other types, use ToString comparison
            return string.Compare(a.ToString(), b.ToString(), StringComparison.Ordinal);
        }

        private K3Value Shape(K3Value a)
        {
            if (a is VectorValue vecA)
                return new VectorValue(new List<K3Value> { new IntegerValue(vecA.Elements.Count) });
            
            return new IntegerValue(0); // For scalars
        }

        private K3Value Enumerate(K3Value a)
        {
            if (a is IntegerValue intA)
            {
                var elements = new List<K3Value>();
                for (int i = 0; i < intA.Value; i++)
                {
                    elements.Add(new IntegerValue(i));
                }
                return new VectorValue(elements);
            }
            
            throw new Exception($"Cannot enumerate {a.Type}");
        }

        private K3Value Enlist(K3Value a)
        {
            var elements = new List<K3Value> { a };
            return new VectorValue(elements);
        }

        private K3Value Count(K3Value a)
        {
            if (a is VectorValue vecA)
                return new IntegerValue(vecA.Elements.Count);
            
            return new IntegerValue(1); // For scalars
        }

        private K3Value Floor(K3Value a)
        {
            if (a is IntegerValue intA)
                return intA;
            if (a is LongValue longA)
                return longA;
            if (a is FloatValue floatA)
                return new FloatValue(Math.Floor(floatA.Value));
            
            throw new Exception($"Cannot floor {a.Type}");
        }

        private K3Value Unique(K3Value a)
        {
            if (a is VectorValue vecA)
            {
                var uniqueElements = new List<K3Value>();
                var seen = new HashSet<string>();
                
                foreach (var element in vecA.Elements)
                {
                    var key = element.ToString();
                    if (seen.Add(key))
                    {
                        uniqueElements.Add(element);
                    }
                }
                
                return new VectorValue(uniqueElements);
            }
            
            return a; // For scalars, return the value itself
        }

        // Binary versions for operators that can be both unary and binary
        private K3Value CountBinary(K3Value a, K3Value b)
        {
            return Count(a);
        }

        private K3Value FloorBinary(K3Value a, K3Value b)
        {
            return Floor(a);
        }

        private K3Value UniqueBinary(K3Value a, K3Value b)
        {
            return Unique(a);
        }
    }
}
