using System;
using System.Collections.Generic;
using System.Linq;
using K3CSharp;
using K3CSharp.Parsing;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Convert between AST nodes and K list representations for parse tree functionality
    /// Implements conversion rules according to Parse.md speclet
    /// </summary>
    public static class ParseTreeConverter
    {
        /// <summary>
        /// Convert AST node to K list representation
        /// </summary>
        public static K3Value ToKList(ASTNode node)
        {
            if (node == null)
                return new NullValue();
                
            return node.Type switch
            {
                ASTNodeType.Literal => ToEnlistedVector(node.Value ?? new NullValue()),
                ASTNodeType.Variable => ToSymbolPath(node.Value?.ToString() ?? ""),
                ASTNodeType.BinaryOp => ConvertBinaryOp(node),
                ASTNodeType.FunctionCall => ConvertFunctionCall(node),
                ASTNodeType.Vector => ConvertVector(node),
                ASTNodeType.Assignment => ConvertAssignment(node),
                ASTNodeType.Block => ConvertBlock(node),
                _ => throw new NotSupportedException($"Node type {node.Type} not supported in parse tree conversion")
            };
        }
        
        /// <summary>
        /// Convert atomic values to enlisted vectors (according to Parse.md speclet)
        /// </summary>
        private static K3Value ToEnlistedVector(K3Value value)
        {
            if (value == null)
                return new NullValue();
                
            // Atomic values become enlisted vectors in parse trees
            if (value is IntegerValue || value is FloatValue || value is SymbolValue || value is CharacterValue)
            {
                var elements = new List<K3Value> { value };
                return new VectorValue(elements);
            }
            
            return value; // Pass through complex values as-is
        }
        
        /// <summary>
        /// Convert variables to symbol paths
        /// </summary>
        private static K3Value ToSymbolPath(string variableName)
        {
            // Variables become symbols with their K tree path
            return new SymbolValue(variableName);
        }
        
        /// <summary>
        /// Convert binary operation to K list representation
        /// </summary>
        private static K3Value ConvertBinaryOp(ASTNode node)
        {
            if (node.Children.Count < 2)
                throw new Exception($"Binary operation requires 2 children, got {node.Children.Count}");
                
            var opSymbol = node.Value as SymbolValue ?? new SymbolValue(node.Value?.ToString() ?? "+");
            var elements = new List<K3Value> { opSymbol };
            
            // Check if this is a monadic operator (only right operand)
            if (node.Children.Count == 1)
            {
                // For monadic operators, add disambiguating colon
                elements.Add(new SymbolValue(":"));
            }
            
            // Convert left operand
            elements.Add(ToKList(node.Children[0]));
            
            // Convert right operand
            elements.Add(ToKList(node.Children[1]));
            
            return new VectorValue(elements);
        }
        
        /// <summary>
        /// Convert function call to K list representation
        /// </summary>
        private static K3Value ConvertFunctionCall(ASTNode node)
        {
            var funcName = node.Value?.ToString() ?? "";
            var elements = new List<K3Value> { new SymbolValue(funcName) };
            
            // Convert all arguments
            foreach (var child in node.Children)
            {
                elements.Add(ToKList(child));
            }
            
            return new VectorValue(elements);
        }
        
        /// <summary>
        /// Convert vector to K list representation
        /// </summary>
        private static K3Value ConvertVector(ASTNode node)
        {
            var elements = new List<K3Value>();
            
            foreach (var child in node.Children)
            {
                elements.Add(ToKList(child));
            }
            
            return new VectorValue(elements);
        }
        
        /// <summary>
        /// Convert assignment to K list representation
        /// </summary>
        private static K3Value ConvertAssignment(ASTNode node)
        {
            var elements = new List<K3Value>();
            
            // Assignment operator
            elements.Add(new SymbolValue(node.Value?.ToString() ?? ":"));
            
            // Left side
            if (node.Children.Count > 0)
                elements.Add(ToKList(node.Children[0]));
                
            // Right side
            if (node.Children.Count > 1)
                elements.Add(ToKList(node.Children[1]));
                
            return new VectorValue(elements);
        }
        
        /// <summary>
        /// Convert block to K list representation
        /// </summary>
        private static K3Value ConvertBlock(ASTNode node)
        {
            var elements = new List<K3Value>();
            
            foreach (var child in node.Children)
            {
                elements.Add(ToKList(child));
            }
            
            return new VectorValue(elements);
        }
        
        /// <summary>
        /// Convert K list back to AST node (reverse conversion)
        /// </summary>
        public static ASTNode FromKList(K3Value kList)
        {
            if (kList is not VectorValue vector)
                throw new Exception("Parse tree must be a vector");
                
            var elements = vector.Elements;
            if (elements.Count == 0)
                throw new Exception("Parse tree cannot be empty");
                
            // First element should be a verb (symbol)
            // Temporarily remove validation to debug
            // if (elements[0] is not SymbolValue)
            //     throw new Exception("Parse tree must start with a verb");
                
            // Extract verb symbol properly
            string verbSymbol;
            if (elements[0] is SymbolValue sym)
                verbSymbol = sym.Value;
            else if (elements[0] is CharacterValue ch)
                verbSymbol = ch.Value;
            else
                verbSymbol = elements[0].ToString();
            
            // Check if this is an operator that should be BinaryOp
            if (IsOperatorVerb(verbSymbol) && elements.Count >= 3)
            {
                // Create binary operation node
                var binaryOp = new ASTNode(ASTNodeType.BinaryOp);
                binaryOp.Value = new SymbolValue(verbSymbol);
                
                // Handle disambiguating colon if present
                int argIndex = 1;
                if (elements.Count > 1 && elements[1] is SymbolValue && elements[1].ToString() == ":")
                {
                    // Skip the colon and move to next argument
                    argIndex = 2;
                }
                
                // Add remaining elements as operands
                for (int i = argIndex; i < elements.Count; i++)
                {
                    binaryOp.Children.Add(ConvertKListElementToAST(elements[i]));
                }
                
                return binaryOp;
            }
            else
            {
                // Create function call node
                var funcCall = new ASTNode(ASTNodeType.FunctionCall);
                funcCall.Value = new SymbolValue(verbSymbol);
                
                // Convert remaining elements as arguments
                for (int i = 1; i < elements.Count; i++)
                {
                    funcCall.Children.Add(ConvertKListElementToAST(elements[i]));
                }
                
                return funcCall;
            }
        }
        
        /// <summary>
        /// Check if a verb symbol represents an operator
        /// </summary>
        private static bool IsOperatorVerb(string verbSymbol)
        {
            // Strip quotes if present
            var cleanSymbol = verbSymbol.Trim('"');
            return cleanSymbol switch
            {
                "+" => true,
                "-" => true,
                "*" => true,
                "/" => true,
                "%" => true,
                "^" => true,
                "<" => true,
                ">" => true,
                "=" => true,
                "&" => true,
                "|" => true,
                "~" => true,
                "!" => true,
                "#" => true,
                "_" => true,
                "?" => true,
                "@" => true,
                "." => true,
                "," => true,
                _ => false
            };
        }
        
        /// <summary>
        /// Convert K list element to AST node
        /// </summary>
        private static ASTNode ConvertKListElementToAST(K3Value element)
        {
            Console.WriteLine($"DEBUG: ConvertKListElementToAST called with type: {element.GetType()}, value: {element}");
            return element switch
            {
                SymbolValue symbol => ASTNode.MakeVariable(symbol.ToString()),
                VectorValue vec => ConvertVectorFromKList(vec),
                IntegerValue num => ASTNode.MakeLiteral(num),
                FloatValue num => ASTNode.MakeLiteral(num),
                CharacterValue str => ASTNode.MakeLiteral(str),
                _ => throw new NotSupportedException($"Element type {element.GetType()} not supported in parse tree conversion")
            };
        }
        
        /// <summary>
        /// Convert vector from K list to AST vector node
        /// </summary>
        private static ASTNode ConvertVectorFromKList(VectorValue kListVector)
        {
            var vectorNode = new ASTNode(ASTNodeType.Vector);
            
            foreach (var element in kListVector.Elements)
            {
                vectorNode.Children.Add(ConvertKListElementToAST(element));
            }
            
            return vectorNode;
        }
        
        /// <summary>
        /// Handle monadic operator disambiguation (add colon)
        /// </summary>
        private static K3Value AddMonadicDisambiguation(K3Value verb, K3Value operand)
        {
            // Add colon for monadic operators in parse trees
            var elements = new List<K3Value> { verb };
            if (operand != new NullValue())
                elements.Add(new SymbolValue(":")); // Monadic disambiguation
            elements.Add(operand);
            
            return new VectorValue(elements);
        }
    }
}
