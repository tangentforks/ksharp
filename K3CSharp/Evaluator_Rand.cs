using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public partial class Evaluator
    {
        private static int _randomSeed = -314159; // Default seed value
        
        private K3Value DrawFunction(K3Value operand)
        {
            // _draw is a function used to generate random numbers based on seed value
            // It has 3 different cases depending on input types: Select, Deal and Probability
            // The left argument must be either a nonnegative integer or a vector of nonnegative integers
            // The right argument must be an integer
            // There is no monadic form of _draw. If _draw is called with 1 argument it will produce a valence error.
            
            throw new Exception("_draw requires dyadic call (left and right arguments)");
        }
        
        private K3Value Draw(K3Value left, K3Value right)
        {
            // _draw function - dyadic implementation
            // It has 3 different cases depending on input types: Select, Deal and Probability
            // The left argument must be either a nonnegative integer or a vector of nonnegative integers
            // The right argument must be an integer
            
            // Handle scalar left operand
            if (left is IntegerValue leftInt && right is IntegerValue rightInt)
            {
                if (rightInt.Value > 0)
                {
                    // Select case: right argument is positive
                    return DrawSelect(leftInt.Value, rightInt.Value);
                }
                else if (rightInt.Value == 0)
                {
                    // Probability case: right argument is 0
                    return DrawProbability(leftInt, rightInt);
                }
                else
                {
                    // Deal case: right argument is negative
                    return DrawDeal(leftInt.Value, -rightInt.Value);
                }
            }
            // Handle vector left operand
            else if (left is VectorValue leftVec && right is IntegerValue rightVal)
            {
                if (leftVec.Elements.All(e => e is IntegerValue))
                {
                    if (rightVal.Value < 0 && leftVec.Elements.Count >= 2)
                    {
                        // Deal case with vector left: creates a matrix
                        // e.g., 2 3 _draw -10 creates 2 rows of 3 unique values from 0-9
                        var dims = leftVec.Elements.Select(e => ((IntegerValue)e).Value).ToList();
                        var rows = dims[0];
                        var cols = dims[dims.Count - 1];

                        var results = new List<K3Value>();
                        for (int r = 0; r < rows; r++)
                        {
                            results.Add(DrawDeal(cols, -rightVal.Value));
                        }
                        return new VectorValue(results);
                    }
                    else
                    {
                        // Select/Probability: apply to each element individually
                        var results = new List<K3Value>();
                        foreach (var element in leftVec.Elements)
                        {
                            var elementInt = (IntegerValue)element;
                            if (rightVal.Value > 0)
                                results.Add(DrawSelect(elementInt.Value, rightVal.Value));
                            else
                                results.Add(DrawProbability(elementInt, rightVal));
                        }
                        return new VectorValue(results);
                    }
                }
                throw new Exception("_draw requires integer or vector of integers");
            }
            else
            {
                throw new Exception("_draw requires integer or vector of integers");
            }
        }
        
        private K3Value DrawSelect(int count, int right)
        {
            // Select case: generate random integers between 0 and right (exclusive)
            if (count < 0)
            {
                throw new Exception("_draw requires nonnegative integer");
            }
            
            if (count == 0)
            {
                return new VectorValue(new List<K3Value>());
            }
            
            var random = new Random(_randomSeed);
            var results = new List<K3Value>();
            
            for (int i = 0; i < count; i++)
            {
                var randomValue = random.Next(0, right);
                results.Add(new IntegerValue(randomValue));
                
                // Update seed for next random number generation
                _randomSeed = Math.Abs(random.Next()) % 1000000;
            }
            
            return new VectorValue(results);
        }
        
        private K3Value DrawDeal(int count, int range)
        {
            // Deal case: right argument is a negative integer
            if (count < 0)
            {
                throw new Exception("_draw requires nonnegative integer");
            }
            
            if (count > range)
            {
                throw new Exception("_draw Deal case: count must be <= range for unique values");
            }
            
            var random = new Random(_randomSeed);
            var results = new List<K3Value>();
            
            // Generate unique random integers between 0 and range (exclusive)
            var possibleValues = new HashSet<int>();
            while (possibleValues.Count < count)
            {
                var randomValue = random.Next(0, range);
                if (!possibleValues.Contains(randomValue))
                {
                    possibleValues.Add(randomValue);
                }
            }
            
            // Use of generated unique values
            var possibleValuesList = possibleValues.ToList();
            for (int i = 0; i < count; i++)
            {
                if (i < possibleValuesList.Count)
                {
                    results.Add(new IntegerValue(possibleValuesList[i]));
                }
                else
                {
                    // Fallback: generate any random value
                    results.Add(new IntegerValue(random.Next(0, range)));
                }
                
                // Update seed for next random number generation
                _randomSeed = Math.Abs(random.Next()) % 1000000;
            }
            
            return new VectorValue(results);
        }
        
        private K3Value DrawProbability(K3Value left, K3Value right)
        {
            // Probability case: right argument is 0
            if (left is IntegerValue leftVal && right is IntegerValue rightVal && rightVal.Value == 0)
            {
                // Generate random floating point numbers between 0 and 1
                var random = new Random(_randomSeed);
                var results = new List<K3Value>();
                
                for (int i = 0; i < leftVal.Value; i++)
                {
                    var randomValue = random.NextDouble();
                    results.Add(new FloatValue(randomValue));
                    
                    // Update seed for next random number generation
                    _randomSeed = Math.Abs(random.Next()) % 1000000;
                }
                
                return new VectorValue(results);
            }
            
            throw new Exception("_draw Probability case requires left integer and right argument 0");
        }
        
        // Method to get/set random seed (for \r command)
        public static int RandomSeed
        {
            get { return _randomSeed; }
            set { _randomSeed = value; }
        }
    }
}
