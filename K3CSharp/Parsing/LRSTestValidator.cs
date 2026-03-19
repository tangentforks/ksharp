using System;
using System.Collections.Generic;
using K3CSharp;
using K3CSharp.Parsing;

/// <summary>
/// Test class to validate LRS parser functionality
/// </summary>
public static class LRSTestValidator
{
    /// <summary>
    /// Test the LRS parser with the target test case
    /// </summary>
    public static void TestParseMonadicShapeAtomic()
    {
        try
        {
            var expression = "^,`a";
            Console.WriteLine($"Testing LRS Parser with: {expression}");
            
            // Test the lexer
            var lexer = new Lexer(expression);
            var tokens = lexer.Tokenize();
            
            Console.WriteLine($"Tokens found: {tokens.Count}");
            for (int i = 0; i < tokens.Count; i++)
            {
                Console.WriteLine($"  {i}: {tokens[i].Type} = '{tokens[i].Lexeme}'");
            }
            
            // Test the modular LRS parser
            var lrsParser = new LRSParser(tokens);
            var position = 0;
            var astNode = lrsParser.ParseExpression(ref position);
            
            if (astNode == null)
            {
                Console.WriteLine("LRS Parser: Failed to parse expression");
                throw new Exception("LRS Parser failed to parse ^,`a");
            }
            else
            {
                Console.WriteLine("LRS Parser: Parse successful!");
                Console.WriteLine($"AST Node Type: {astNode.Type}");
                Console.WriteLine($"AST Node Value: {astNode.Value}");
                Console.WriteLine($"AST Node Children: {astNode.Children.Count}");
                
                // Print AST structure
                PrintAST(astNode, 0);
                
                // Convert to K list and verify structure
                var kList = ParseTreeConverter.ToKList(astNode);
                Console.WriteLine($"K List Result: {kList}");
            }
            
            // Test verb-agnostic operator detection
            Console.WriteLine("\nOperator Detection Tests:");
            TestOperatorDetection(TokenType.POWER);
            TestOperatorDetection(TokenType.JOIN);
            TestOperatorDetection(TokenType.PLUS);
            
            Console.WriteLine("\nLRS Parser test completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LRS Parser test failed: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            throw;
        }
    }
    
    /// <summary>
    /// Print AST structure for debugging
    /// </summary>
    private static void PrintAST(ASTNode node, int depth)
    {
        var indent = new string(' ', depth * 2);
        Console.WriteLine($"{indent}{node.Type}: {node.Value} (children: {node.Children.Count})");
        for (int i = 0; i < node.Children.Count; i++)
        {
            Console.WriteLine($"{indent}  Child {i}:");
            PrintAST(node.Children[i], depth + 1);
        }
    }
    
    /// <summary>
    /// Test verb-agnostic operator detection
    /// </summary>
    private static void TestOperatorDetection(TokenType tokenType)
    {
        Console.WriteLine($"  {tokenType}:");
        Console.WriteLine($"    IsBinaryOperator: {OperatorDetector.IsBinaryOperator(tokenType)}");
        Console.WriteLine($"    SupportsMonadic: {OperatorDetector.SupportsMonadic(tokenType)}");
        Console.WriteLine($"    SupportsDyadic: {OperatorDetector.SupportsDyadic(tokenType)}");
        Console.WriteLine($"    IsFunction: {OperatorDetector.IsFunction(tokenType)}");
    }
    
    /// <summary>
    /// Test that the LRS parser produces the expected AST structure
    /// </summary>
    public static void ValidateASTStructure(ASTNode node)
    {
        // Expected structure: ^ (, `a)
        // Should be BinaryOp("^") with child BinaryOp(",") with child Literal(`a)
        
        if (node.Type != ASTNodeType.BinaryOp)
            throw new Exception($"Expected BinaryOp, got {node.Type}");
            
        if (node.Value?.ToString() != "^")
            throw new Exception($"Expected ^ operator, got {node.Value}");
            
        if (node.Children.Count != 1)
            throw new Exception($"Expected 1 child for monadic ^, got {node.Children.Count}");
            
        var child = node.Children[0];
        if (child.Type != ASTNodeType.BinaryOp)
            throw new Exception($"Expected BinaryOp child, got {child.Type}");
            
        if (child.Value?.ToString() != ",")
            throw new Exception($"Expected , operator child, got {child.Value}");
            
        if (child.Children.Count != 1)
            throw new Exception($"Expected 1 child for monadic ,, got {child.Children.Count}");
            
        var grandchild = child.Children[0];
        if (grandchild.Type != ASTNodeType.Literal)
            throw new Exception($"Expected Literal grandchild, got {grandchild.Type}");
            
        Console.WriteLine("AST Structure validation passed!");
    }
}
