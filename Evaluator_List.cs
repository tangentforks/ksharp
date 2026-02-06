using System;
using System.Collections.Generic;

namespace K3CSharp
{
    public partial class Evaluator
    {
        // List and system-related functions
        
        // Dyadic implementations
        private K3Value In(K3Value left, K3Value right)
        {
            // _in (Find) function - searches for left argument in right argument
            // Returns position (1-based) or 0 if not found
            // Uses tolerant comparison for floating-point numbers
            
            if (right is VectorValue rightVec)
            {
                // Search for left in right vector
                for (int i = 0; i < rightVec.Elements.Count; i++)
                {
                    var matchResult = Match(left, rightVec.Elements[i]);
                    if (matchResult is IntegerValue intVal && intVal.Value == 1)
                    {
                        return new IntegerValue(i + 1); // 1-based indexing
                    }
                }
                return new IntegerValue(0); // Not found
            }
            else
            {
                // Search for left in right scalar
                var matchResult = Match(left, right);
                if (matchResult is IntegerValue intVal2 && intVal2.Value == 1)
                {
                    return new IntegerValue(1); // Found at position 1
                }
                return new IntegerValue(0); // Not found
            }
        }

        private K3Value Bin(K3Value left, K3Value right)
        {
            // _bin (Binary Search) function - performs binary search on sorted list
            // Returns position (1-based) or 0 if not found
            // Assumes right argument is sorted in ascending order
            
            if (right is VectorValue rightVec)
            {
                int low = 0;
                int high = rightVec.Elements.Count - 1;
                
                while (low <= high)
                {
                    int mid = (low + high) / 2;
                    var midValue = rightVec.Elements[mid];
                    var comparison = CompareValues(left, midValue);
                    
                    if (comparison == 0)
                    {
                        return new IntegerValue(mid + 1); // 1-based indexing
                    }
                    else if (comparison < 0)
                    {
                        high = mid - 1;
                    }
                    else
                    {
                        low = mid + 1;
                    }
                }
                return new IntegerValue(0); // Not found
            }
            else
            {
                // Search for left in right scalar
                var comparison = CompareValues(left, right);
                if (comparison == 0)
                {
                    return new IntegerValue(1); // Found at position 1
                }
                return new IntegerValue(0); // Not found
            }
        }

        private K3Value Binl(K3Value left, K3Value right)
        {
            // _binl (Binary Search Each-Left) function
            // Returns 1 for each element of left that is found in right, 0 otherwise
            // Equivalent to left _in\: right but optimized
            
            if (left is VectorValue leftVec)
            {
                var results = new List<K3Value>();
                
                // For binary search each-left, we need to search each element of left in right
                foreach (var leftElement in leftVec.Elements)
                {
                    var result = Bin(leftElement, right);
                    // Convert position result to 1/0 (found/not found)
                    var found = result is IntegerValue intVal && intVal.Value != 0;
                    results.Add(new IntegerValue(found ? 1 : 0));
                }
                
                return new VectorValue(results);
            }
            else
            {
                // Single element case
                var result = Bin(left, right);
                var found = result is IntegerValue intVal && intVal.Value != 0;
                return new IntegerValue(found ? 1 : 0);
            }
        }

        private K3Value Lin(K3Value left, K3Value right)
        {
            // _lin (List Intersection) function
            // Returns 1 for each element of left that is in right, 0 otherwise
            // Equivalent to left _in\: right but optimized using HashSet
            
            if (left is VectorValue leftVec)
            {
                var results = new List<K3Value>();
                
                // Create a HashSet for efficient O(1) lookups of right argument elements
                var rightSet = CreateHashSet(right);
                
                foreach (var leftElement in leftVec.Elements)
                {
                    bool found = false;
                    
                    // Check if leftElement exists in rightSet
                    if (rightSet != null)
                    {
                        found = rightSet.Contains(leftElement);
                    }
                    else
                    {
                        // Fallback to linear search if HashSet creation failed
                        found = LinearSearchInRight(leftElement, right);
                    }
                    
                    results.Add(new IntegerValue(found ? 1 : 0));
                }
                
                return new VectorValue(results);
            }
            else
            {
                // Single element case - return 1 if found, 0 otherwise
                bool found = LinearSearchInRight(left, right);
                return new IntegerValue(found ? 1 : 0);
            }
        }

        // Unary placeholder functions
        private K3Value InFunction(K3Value operand)
        {
            // _in function should be handled as dyadic in binary operations
            // This unary case should not be reached in normal operation
            throw new Exception("_in (Find) function requires two arguments - use infix notation: x _in y");
        }

        private K3Value BinFunction(K3Value operand)
        {
            throw new Exception("_bin (binary search) operation reserved for future use");
        }

        private K3Value BinlFunction(K3Value operand)
        {
            // _binl function should be handled as dyadic in binary operations
            // This unary case should not be reached in normal operation
            throw new Exception("_binl (binary search each-left) function requires two arguments - use infix notation: x _binl y");
        }

        private K3Value LinFunction(K3Value operand)
        {
            // _lin function should be handled as dyadic in binary operations
            // This unary case should not be reached in normal operation
            throw new Exception("_lin (list intersection) function requires two arguments - use infix notation: x _lin y");
        }

        // Database and system functions (placeholders)
        private K3Value DvFunction(K3Value operand)
        {
            throw new Exception("_dv (delete by value) operation reserved for future use");
        }

        private K3Value DiFunction(K3Value operand)
        {
            throw new Exception("_di (delete by index) operation reserved for future use");
        }

        private K3Value SvFunction(K3Value operand)
        {
            throw new Exception("_sv (scalar from vector) operation reserved for future use");
        }

        private K3Value VsFunction(K3Value operand)
        {
            throw new Exception("_vs (vector from scalar) operation reserved for future use");
        }

        private K3Value CiFunction(K3Value operand)
        {
            throw new Exception("_ci (character from integer) operation reserved for future use");
        }

        private K3Value IcFunction(K3Value operand)
        {
            throw new Exception("_ic (integer from character) operation reserved for future use");
        }

        private K3Value SmFunction(K3Value operand)
        {
            throw new Exception("_sm (string match) operation reserved for future use");
        }

        private K3Value SsFunction(K3Value operand)
        {
            throw new Exception("_ss (string search) operation reserved for future use");
        }

        private K3Value SsrFunction(K3Value operand)
        {
            throw new Exception("_ssr (string search and replace) operation reserved for future use");
        }

        private K3Value BdFunction(K3Value operand)
        {
            throw new Exception("_bd (bytes from data) operation reserved for future use");
        }

        private K3Value DbFunction(K3Value operand)
        {
            throw new Exception("_db (data from bytes) operation reserved for future use");
        }

        private K3Value GetenvFunction(K3Value operand)
        {
            throw new Exception("_getenv (get environment variable) operation reserved for future use");
        }

        private K3Value SetenvFunction(K3Value operand)
        {
            throw new Exception("_setenv (set environment variable) operation reserved for future use");
        }

        private K3Value HostFunction(K3Value operand)
        {
            throw new Exception("_host (host information) operation reserved for future use");
        }

        private K3Value SizeFunction(K3Value operand)
        {
            throw new Exception("_size (size information) operation reserved for future use");
        }

        private K3Value ExitFunction(K3Value operand)
        {
            throw new Exception("_exit (control flow) operation reserved for future use");
        }

        // Helper methods for list operations
        private HashSet<K3Value>? CreateHashSet(K3Value value)
        {
            // Create a HashSet from a K3Value for efficient lookups
            try
            {
                var set = new HashSet<K3Value>(new K3ValueComparer());
                
                if (value is VectorValue vec)
                {
                    foreach (var element in vec.Elements)
                    {
                        set.Add(element);
                    }
                }
                else
                {
                    set.Add(value);
                }
                
                return set;
            }
            catch
            {
                return null; // Return null if HashSet creation fails
            }
        }

        private bool LinearSearchInRight(K3Value leftElement, K3Value right)
        {
            // Linear search for leftElement in right
            if (right is VectorValue rightVec)
            {
                foreach (var rightElement in rightVec.Elements)
                {
                    var matchResult = Match(leftElement, rightElement);
                    if (matchResult is IntegerValue intVal && intVal.Value == 1)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                var matchResult = Match(leftElement, right);
                return matchResult is IntegerValue intVal && intVal.Value == 1;
            }
        }
    }
}
