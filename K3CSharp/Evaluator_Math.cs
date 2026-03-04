using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        private K3Value MathLog(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < 0)
                    return new FloatValue("0n"); // log(negative) = 0n
                else if (intValue.Value == 0)
                    return new FloatValue("-0i"); // log(0) = -0i
                return new FloatValue(Math.Log((double)intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < 0)
                    return new FloatValue("0n"); // log(negative) = 0n
                else if (longValue.Value == 0)
                    return new FloatValue("-0i"); // log(0) = -0i
                return new FloatValue(Math.Log((double)longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < 0)
                    return new FloatValue("0n"); // log(negative) = 0n
                else if (floatValue.Value == 0)
                    return new FloatValue("-0i"); // log(0) = -0i
                return new FloatValue(Math.Log(floatValue.Value));
            }
            else if (operand is VectorValue vector)
            {
                var result = new List<K3Value>();
                foreach (var element in vector.Elements)
                {
                    result.Add(MathLog(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception($"MathLog operation not supported for type {operand.Type}");
            }
        }
        
        private K3Value MathExp(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Exp(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Exp(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Exp(floatValue.Value));
            }
            else if (operand is VectorValue vector)
            {
                var result = new List<K3Value>();
                foreach (var element in vector.Elements)
                {
                    result.Add(MathExp(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_exp can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathAbs(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new IntegerValue(Math.Abs(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new LongValue(Math.Abs(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Abs(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAbs(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_abs can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathSqr(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(intValue.Value * intValue.Value);
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(longValue.Value * longValue.Value);
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(floatValue.Value * floatValue.Value);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSqr(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_sqr can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathSqrt(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Sqrt(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Sqrt(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < 0)
                    return new FloatValue(double.NaN); // 0n
                return new FloatValue(Math.Sqrt(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSqrt(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_sqrt can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathFloor(K3Value operand)
        {
            // Mathematical floor operation that always returns floating point values
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Floor((double)intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Floor((double)longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Floor(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathFloor(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_floor can only be applied to numeric values or vectors");
            }
        }
        
        private double GetNumericValue(K3Value value)
        {
            if (value is IntegerValue intVal)
                return intVal.Value;
            else if (value is LongValue longVal)
                return longVal.Value;
            else if (value is FloatValue floatVal)
                return floatVal.Value;
            else
                throw new Exception("Numeric value required");
        }
        
        private K3Value MathDot(K3Value left, K3Value right)
        {
            // Linear algebra dot product operation (binary)
            if (left is VectorValue leftVec && right is VectorValue rightVec)
            {
                if (leftVec.Elements.Count != rightVec.Elements.Count)
                    throw new Exception("_dot requires vectors of the same length");
                
                double sum = 0.0;
                for (int i = 0; i < leftVec.Elements.Count; i++)
                {
                    var leftElement = leftVec.Elements[i];
                    var rightElement = rightVec.Elements[i];
                    
                    double leftVal = GetNumericValue(leftElement);
                    double rightVal = GetNumericValue(rightElement);
                    
                    sum += leftVal * rightVal;
                }
                
                return new FloatValue(sum);
            }
            else if (left is VectorValue vec)
            {
                // Unary case: dot product of vector with itself
                double sum = 0.0;
                foreach (var element in vec.Elements)
                {
                    double val = GetNumericValue(element);
                    sum += val * val;
                }
                return new FloatValue(sum);
            }
            else
            {
                throw new Exception("_dot requires vector arguments");
            }
        }
        
        private K3Value MathMul(K3Value left, K3Value right)
        {
            // Generic matrix multiplication for any size matrices
            if (left is VectorValue leftVec && right is VectorValue rightVec)
            {
                // Check if both are matrices (vectors of vectors)
                if (IsMatrix(leftVec) && IsMatrix(rightVec))
                {
                    var leftMatrix = ExtractMatrix(leftVec);
                    var rightMatrix = ExtractMatrix(rightVec);
                    
                    // Check dimensions: left columns must equal right rows
                    if (leftMatrix[0].Length != rightMatrix.Length)
                    {
                        throw new Exception($"Matrix multiplication dimensions incompatible: {leftMatrix.Length}x{leftMatrix[0].Length} cannot be multiplied by {rightMatrix.Length}x{rightMatrix[0].Length}");
                    }
                    
                    // Perform matrix multiplication: C = A * B
                    var result = new double[leftMatrix.Length][];
                    for (int i = 0; i < leftMatrix.Length; i++)
                    {
                        result[i] = new double[rightMatrix[0].Length];
                        for (int j = 0; j < rightMatrix[0].Length; j++)
                        {
                            result[i][j] = 0;
                            for (int k = 0; k < leftMatrix[0].Length; k++)
                            {
                                result[i][j] += leftMatrix[i][k] * rightMatrix[k][j];
                            }
                        }
                    }
                    
                    // Convert result back to K3Sharp matrix format
                    var resultRows = new List<K3Value>();
                    for (int i = 0; i < result.Length; i++)
                    {
                        var rowElements = new List<K3Value>();
                        for (int j = 0; j < result[i].Length; j++)
                        {
                            rowElements.Add(new FloatValue(result[i][j]));
                        }
                        resultRows.Add(new VectorValue(rowElements));
                    }
                    
                    return new VectorValue(resultRows);
                }
            }
            
            throw new Exception("_mul requires matrices represented as nested vectors");
        }
        
        private bool IsMatrix(VectorValue vec)
        {
            // Check if this is a matrix (vector of vectors with consistent dimensions)
            if (vec.Elements.Count == 0) return false;
            
            // All elements must be vectors
            foreach (var element in vec.Elements)
            {
                if (!(element is VectorValue))
                    return false;
            }
            
            // All rows must have the same length
            var firstRowLength = ((VectorValue)vec.Elements[0]).Elements.Count;
            for (int i = 1; i < vec.Elements.Count; i++)
            {
                if (((VectorValue)vec.Elements[i]).Elements.Count != firstRowLength)
                    return false;
            }
            
            return true;
        }
        
        private double[][] ExtractMatrix(VectorValue matrixVec)
        {
            var rows = matrixVec.Elements.Count;
            var cols = ((VectorValue)matrixVec.Elements[0]).Elements.Count;
            var matrix = new double[rows][];
            
            for (int i = 0; i < rows; i++)
            {
                matrix[i] = new double[cols];
                var rowVec = (VectorValue)matrixVec.Elements[i];
                for (int j = 0; j < cols; j++)
                {
                    matrix[i][j] = GetNumericValue(rowVec.Elements[j]);
                }
            }
            
            return matrix;
        }
        
        private K3Value MathInv(K3Value left, K3Value right)
        {
            // Linear algebra matrix inverse - for now implement as identity
            // Full matrix inverse would require proper matrix representation
            return right;
        }
        
        private K3Value MathInv(K3Value operand)
        {
            // Generic matrix inverse for any size square matrix
            if (operand is VectorValue matrix && IsMatrix(matrix))
            {
                var matrixData = ExtractMatrix(matrix);
                var n = matrixData.Length;
                
                // Check if it's a square matrix
                if (n != matrixData[0].Length)
                {
                    throw new Exception($"Matrix inverse requires square matrix, got {n}x{matrixData[0].Length}");
                }
                
                // Create augmented matrix [A|I]
                var augmented = new double[n][];
                for (int i = 0; i < n; i++)
                {
                    augmented[i] = new double[2 * n];
                    // Copy matrix A
                    for (int j = 0; j < n; j++)
                    {
                        augmented[i][j] = matrixData[i][j];
                    }
                    // Copy identity matrix I
                    augmented[i][n + i] = 1.0;
                }
                
                // Gaussian elimination to get [I|A^-1]
                for (int i = 0; i < n; i++)
                {
                    // Find pivot
                    int pivotRow = i;
                    for (int j = i + 1; j < n; j++)
                    {
                        if (Math.Abs(augmented[j][i]) > Math.Abs(augmented[pivotRow][i]))
                            pivotRow = j;
                    }
                    
                    // Swap rows if needed
                    if (pivotRow != i)
                    {
                        for (int k = 0; k < 2 * n; k++)
                        {
                            var temp = augmented[i][k];
                            augmented[i][k] = augmented[pivotRow][k];
                            augmented[pivotRow][k] = temp;
                        }
                    }
                    
                    // Check for singular matrix
                    if (Math.Abs(augmented[i][i]) < 1e-10)
                    {
                        throw new Exception("Matrix is singular (determinant = 0)");
                    }
                    
                    // Eliminate column i
                    for (int j = i + 1; j < n; j++)
                    {
                        var factor = augmented[j][i] / augmented[i][i];
                        for (int k = i; k < 2 * n; k++)
                        {
                            augmented[j][k] -= factor * augmented[i][k];
                        }
                    }
                }
                
                // Back substitution
                for (int i = n - 1; i >= 0; i--)
                {
                    for (int j = n; j < 2 * n; j++)
                    {
                        augmented[i][j] /= augmented[i][i];
                    }
                    
                    for (int j = i - 1; j >= 0; j--)
                    {
                        for (int k = n; k < 2 * n; k++)
                        {
                            augmented[j][k] -= augmented[i][k] * augmented[j][i];
                        }
                    }
                }
                
                // Extract inverse matrix
                var resultRows = new List<K3Value>();
                for (int i = 0; i < n; i++)
                {
                    var rowElements = new List<K3Value>();
                    for (int j = n; j < 2 * n; j++)
                    {
                        rowElements.Add(new FloatValue(augmented[i][j]));
                    }
                    resultRows.Add(new VectorValue(rowElements));
                }
                
                return new VectorValue(resultRows);
            }
            
            // For scalars, return reciprocal
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value == 0)
                    return new FloatValue(double.NaN); // 0n for singular matrix
                return new FloatValue(1.0 / intValue.Value);
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value == 0)
                    return new FloatValue(double.NaN); // 0n for singular matrix
                return new FloatValue(1.0 / longValue.Value);
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value == 0)
                    return new FloatValue(double.NaN); // 0n for singular matrix
                return new FloatValue(1.0 / floatValue.Value);
            }
            
            throw new Exception("_inv requires square matrix or scalar");
        }
        
        private K3Value MathSin(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Sin(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Sin(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Sin(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSin(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_sin can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathCos(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Cos(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Cos(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Cos(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathCos(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_cos can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathTan(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Tan(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Tan(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Tan(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathTan(element));
                }
                return new VectorValue(result, -2); // Float vector
            }
            else
            {
                throw new Exception("_tan can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathAsin(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < -1 || intValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Asin(intValue.Value);
                return new FloatValue(result);
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < -1 || longValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Asin(longValue.Value);
                return new FloatValue(result);
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < -1 || floatValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Asin(floatValue.Value);
                return new FloatValue(result);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAsin(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_asin can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathAcos(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                if (intValue.Value < -1 || intValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Acos(intValue.Value);
                return new FloatValue(result);
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value < -1 || longValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Acos(longValue.Value);
                return new FloatValue(result);
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value < -1 || floatValue.Value > 1)
                    return new FloatValue(double.NaN); // 0n
                var result = Math.Acos(floatValue.Value);
                return new FloatValue(result);
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAcos(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_acos can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathAtan(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Atan(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Atan(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Atan(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathAtan(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_atan can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathSinh(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Sinh(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Sinh(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Sinh(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathSinh(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_sinh can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathCosh(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Cosh(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Cosh(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Cosh(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathCosh(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_cosh can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathTanh(K3Value operand)
        {
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(Math.Tanh(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Tanh(longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                return new FloatValue(Math.Tanh(floatValue.Value));
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathTanh(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_tanh can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathDiv(K3Value left, K3Value right)
        {
            // Integer division operation - only works with int/long types
            if (left is IntegerValue intLeft && right is IntegerValue intRight)
            {
                if (intRight.Value == 0)
                    throw new Exception("Division by zero");
                return new IntegerValue(intLeft.Value / intRight.Value);
            }
            else if (left is LongValue longLeft && right is LongValue longRight)
            {
                if (longRight.Value == 0)
                    throw new Exception("Division by zero");
                return new LongValue(longLeft.Value / longRight.Value);
            }
            else if (left is VectorValue leftVec && (right is IntegerValue || right is LongValue))
            {
                var result = new List<K3Value>();
                foreach (var element in leftVec.Elements)
                {
                    result.Add(MathDiv(element, right));
                }
                return new VectorValue(result, -1); // Integer vector
            }
            else if (left is VectorValue vecLeft && right is VectorValue vecRight)
            {
                if (vecLeft.Elements.Count != vecRight.Elements.Count)
                    throw new Exception("Vector lengths must match for integer division");
                
                var result = new List<K3Value>();
                for (int i = 0; i < vecLeft.Elements.Count; i++)
                {
                    result.Add(MathDiv(vecLeft.Elements[i], vecRight.Elements[i]));
                }
                return new VectorValue(result, -1); // Integer vector
            }
            else
            {
                throw new Exception("_div can only be applied to int/long values or int/long vectors");
            }
        }
        
        private K3Value MathAnd(K3Value left, K3Value right)
        {
            // Bitwise AND operation
            if (left is IntegerValue intLeft && right is IntegerValue intRight)
            {
                return new IntegerValue(intLeft.Value & intRight.Value);
            }
            else if (left is LongValue longLeft && right is LongValue longRight)
            {
                return new LongValue(longLeft.Value & longRight.Value);
            }
            else if (left is VectorValue vecLeft && right is VectorValue vecRight)
            {
                if (vecLeft.Elements.Count != vecRight.Elements.Count)
                    throw new Exception("Vector lengths must match for bitwise AND");
                
                var result = new List<K3Value>();
                for (int i = 0; i < vecLeft.Elements.Count; i++)
                {
                    result.Add(MathAnd(vecLeft.Elements[i], vecRight.Elements[i]));
                }
                return new VectorValue(result, -1); // Integer vector
            }
            else
            {
                throw new Exception("_and can only be applied to int/long values or vectors");
            }
        }
        
        private K3Value MathOr(K3Value left, K3Value right)
        {
            // Bitwise OR operation
            if (left is IntegerValue intLeft && right is IntegerValue intRight)
            {
                return new IntegerValue(intLeft.Value | intRight.Value);
            }
            else if (left is LongValue longLeft && right is LongValue longRight)
            {
                return new LongValue(longLeft.Value | longRight.Value);
            }
            else if (left is VectorValue vecLeft && right is VectorValue vecRight)
            {
                if (vecLeft.Elements.Count != vecRight.Elements.Count)
                    throw new Exception("Vector lengths must match for bitwise OR");
                
                var result = new List<K3Value>();
                for (int i = 0; i < vecLeft.Elements.Count; i++)
                {
                    result.Add(MathOr(vecLeft.Elements[i], vecRight.Elements[i]));
                }
                return new VectorValue(result, -1); // Integer vector
            }
            else
            {
                throw new Exception("_or can only be applied to int/long values or vectors");
            }
        }
        
        private K3Value MathXor(K3Value left, K3Value right)
        {
            // Bitwise XOR operation
            if (left is IntegerValue intLeft && right is IntegerValue intRight)
            {
                return new IntegerValue(intLeft.Value ^ intRight.Value);
            }
            else if (left is LongValue longLeft && right is LongValue longRight)
            {
                return new LongValue(longLeft.Value ^ longRight.Value);
            }
            else if (left is VectorValue vecLeft && right is VectorValue vecRight)
            {
                if (vecLeft.Elements.Count != vecRight.Elements.Count)
                    throw new Exception("Vector lengths must match for bitwise XOR");
                
                var result = new List<K3Value>();
                for (int i = 0; i < vecLeft.Elements.Count; i++)
                {
                    result.Add(MathXor(vecLeft.Elements[i], vecRight.Elements[i]));
                }
                return new VectorValue(result, -1); // Integer vector
            }
            else
            {
                throw new Exception("_xor can only be applied to int/long values or vectors");
            }
        }
        
        private K3Value MathNot(K3Value left)
        {
            // Bitwise NOT operation (unary)
            if (left is IntegerValue intLeft)
            {
                return new IntegerValue(~intLeft.Value);
            }
            else if (left is LongValue longLeft)
            {
                return new LongValue(~longLeft.Value);
            }
            else if (left is VectorValue vecLeft)
            {
                var result = new List<K3Value>();
                foreach (var element in vecLeft.Elements)
                {
                    result.Add(MathNot(element));
                }
                return new VectorValue(result, -1); // Integer vector
            }
            else
            {
                throw new Exception("_not can only be applied to int/long values or vectors");
            }
        }
        
        private K3Value MathLsq(K3Value left, K3Value right)
        {
            // Least squares regression: x _lsq y returns w where yw = x
            // x is a floating-point vector or matrix
            // y is a list of floating-point vectors of the same length as x
            // The items of y are considered to be the columns of the matrix.
            // We need to solve: y * w = x for w
            
            if (left is VectorValue xVec && right is VectorValue yVec)
            {
                // Extract x vector (right-hand side)
                var x = ExtractVector(xVec);
                var m = x.Length; // length of x vector
                
                // Extract y matrix (rows are vectors in yVec)
                // yVec contains row vectors, so y[i] is the i-th row
                var y = new double[yVec.Elements.Count][];
                for (int i = 0; i < yVec.Elements.Count; i++)
                {
                    if (yVec.Elements[i] is VectorValue rowVec)
                    {
                        y[i] = ExtractVector(rowVec);
                        if (y[i].Length != m)
                        {
                            throw new Exception($"Incompatible dimensions: x has length {m}, but row {i} of y has length {y[i].Length}");
                        }
                    }
                    else
                    {
                        throw new Exception("y must be a list of vectors representing matrix rows");
                    }
                }
                
                var n = y.Length; // number of rows in y (and length of result w)
                
                // Solve y * w = x using least squares: w = (y^T * y)^-1 * y^T * x
                // First, compute y^T * y (n x n matrix)
                var yTy = new double[n][];
                for (int i = 0; i < n; i++)
                {
                    yTy[i] = new double[n];
                    for (int j = 0; j < n; j++)
                    {
                        yTy[i][j] = 0;
                        for (int k = 0; k < m; k++)
                        {
                            yTy[i][j] += y[i][k] * y[j][k];  // Fixed: use y[i][k] and y[j][k]
                        }
                    }
                }
                
                // Compute y^T * x (n-dimensional vector)
                var yTx = new double[n];
                for (int i = 0; i < n; i++)
                {
                    yTx[i] = 0;
                    for (int k = 0; k < m; k++)
                    {
                        yTx[i] += y[i][k] * x[k];  // Fixed: use y[i][k]
                    }
                }
                
                // Solve the system: yTy * w = yTx
                var w = SolveLinearSystem(yTy, yTx);
                
                // Convert result back to K3Sharp format
                var resultElements = new List<K3Value>();
                for (int i = 0; i < w.Length; i++)
                {
                    resultElements.Add(new FloatValue(w[i]));
                }
                
                return new VectorValue(resultElements);
            }
            
            throw new Exception("_lsq requires x to be a vector and y to be a list of vectors");
        }
        
        private double[] SolveLinearSystem(double[][] A, double[] b)
        {
            var n = A.Length;
            var augmented = new double[n][];
            
            // Create augmented matrix [A|b]
            for (int i = 0; i < n; i++)
            {
                augmented[i] = new double[n + 1];
                for (int j = 0; j < n; j++)
                {
                    augmented[i][j] = A[i][j];
                }
                augmented[i][n] = b[i];
            }
            
            // Gaussian elimination
            for (int i = 0; i < n; i++)
            {
                // Find pivot
                int pivotRow = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (Math.Abs(augmented[j][i]) > Math.Abs(augmented[pivotRow][i]))
                        pivotRow = j;
                }
                
                // Swap rows if needed
                if (pivotRow != i)
                {
                    for (int k = 0; k <= n; k++)
                    {
                        var temp = augmented[i][k];
                        augmented[i][k] = augmented[pivotRow][k];
                        augmented[pivotRow][k] = temp;
                    }
                }
                
                // Check for singular matrix
                if (Math.Abs(augmented[i][i]) < 1e-10)
                {
                    throw new Exception("Matrix is singular, cannot compute least squares solution");
                }
                
                // Eliminate column i
                for (int j = i + 1; j < n; j++)
                {
                    var factor = augmented[j][i] / augmented[i][i];
                    for (int k = i; k <= n; k++)
                    {
                        augmented[j][k] -= factor * augmented[i][k];
                    }
                }
            }
            
            // Back substitution
            var x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                x[i] = augmented[i][n];
                for (int j = i + 1; j < n; j++)
                {
                    x[i] -= augmented[i][j] * x[j];
                }
                x[i] /= augmented[i][i];
            }
            
            return x;
        }
        
        private K3Value MathLsq(K3Value operand)
        {
            // Legacy monadic version - should not be used
            throw new Exception("_lsq requires two arguments (x and y)");
        }
        
        private double[] ExtractVector(VectorValue vec)
        {
            var result = new double[vec.Elements.Count];
            for (int i = 0; i < vec.Elements.Count; i++)
            {
                result[i] = GetNumericValue(vec.Elements[i]);
            }
            return result;
        }
        
        private K3Value MathCeil(K3Value operand)
        {
            // Ceiling function - rounds up to nearest integer
            if (operand is IntegerValue intValue)
            {
                return new FloatValue(intValue.Value); // Convert to float
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(longValue.Value); // Convert to float
            }
            else if (operand is FloatValue floatValue)
            {
                var result = Math.Ceiling(floatValue.Value);
                return new FloatValue(result); // Always return float for float inputs
            }
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathCeil(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_ceil can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathRot(K3Value left, K3Value right)
        {
            // Bitwise rotation operation
            if (left is IntegerValue intLeft && right is IntegerValue intRight)
            {
                int shift = intRight.Value % 32; // Ensure shift is within 32 bits
                return new IntegerValue((intLeft.Value << shift) | (intLeft.Value >> (32 - shift)));
            }
            else if (left is LongValue longLeft && right is LongValue longRight)
            {
                int shift = (int)(longRight.Value % 64); // Ensure shift is within 64 bits
                return new LongValue((longLeft.Value << shift) | (longLeft.Value >> (64 - shift)));
            }
            else
            {
                throw new Exception("_rot can only be applied to int/long values");
            }
        }
        
        private K3Value MathShift(K3Value left, K3Value right)
        {
            // Bitwise shift operation
            if (left is IntegerValue intLeft && right is IntegerValue intRight)
            {
                if (intRight.Value >= 0)
                {
                    return new IntegerValue(intLeft.Value << intRight.Value);
                }
                else
                {
                    return new IntegerValue(intLeft.Value >> -intRight.Value);
                }
            }
            else if (left is LongValue longLeft && right is LongValue longRight)
            {
                if (longRight.Value >= 0)
                {
                    return new LongValue(longLeft.Value << (int)longRight.Value);
                }
                else
                {
                    return new LongValue(longLeft.Value >> -(int)longRight.Value);
                }
            }
            else
            {
                throw new Exception("_shift can only be applied to int/long values");
            }
        }
    }
}
