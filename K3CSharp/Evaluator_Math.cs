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
                if (intValue.Value <= 0)
                    return new FloatValue(double.NaN);
                return new FloatValue(Math.Log((double)intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                if (longValue.Value <= 0)
                    return new FloatValue(double.NaN);
                return new FloatValue(Math.Log((double)longValue.Value));
            }
            else if (operand is FloatValue floatValue)
            {
                if (floatValue.Value <= 0)
                    return new FloatValue(double.NaN);
                return new FloatValue(Math.Log(floatValue.Value));
            }
            else if (operand is VectorValue vector)
            {
                var result = new List<K3Value>();
                foreach (var element in vector.Elements)
                {
                    result.Add(MathLog(element));
                }
                return new VectorValue(result);
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
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathExp(element));
                }
                return new VectorValue(result);
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
                return new FloatValue(Math.Abs(intValue.Value));
            }
            else if (operand is LongValue longValue)
            {
                return new FloatValue(Math.Abs(longValue.Value));
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
                return new VectorValue(result);
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
                return new VectorValue(result);
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
                return new VectorValue(result);
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
        
        private K3Value MathDot(K3Value operand)
        {
            // Linear algebra dot product operation
            if (operand is VectorValue vec)
            {
                if (vec.Elements.Count == 0)
                    return new FloatValue(0.0);
                
                double sum = 0.0;
                foreach (var element in vec.Elements)
                {
                    if (element is IntegerValue intVal)
                        sum += intVal.Value * intVal.Value;
                    else if (element is LongValue longVal)
                        sum += longVal.Value * longVal.Value;
                    else if (element is FloatValue floatVal)
                        sum += floatVal.Value * floatVal.Value;
                    else
                        throw new Exception("_dot requires numeric vector elements");
                }
                return new FloatValue(sum);
            }
            else if (operand is IntegerValue intValue)
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
            else
            {
                throw new Exception("_dot can only be applied to numeric values or vectors");
            }
        }
        
        private K3Value MathMul(K3Value operand)
        {
            // Linear algebra matrix multiplication - for now implement as identity
            // Full matrix multiplication would require proper matrix representation
            return operand;
        }
        
        private K3Value MathInv(K3Value operand)
        {
            // Linear algebra matrix inverse - for now implement as element-wise reciprocal
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
            else if (operand is VectorValue vec)
            {
                var result = new List<K3Value>();
                foreach (var element in vec.Elements)
                {
                    result.Add(MathInv(element));
                }
                return new VectorValue(result);
            }
            else
            {
                throw new Exception("_inv can only be applied to numeric values or vectors");
            }
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
                return new VectorValue(result);
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
                return new VectorValue(result);
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
                return new VectorValue(result);
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
    }
}
