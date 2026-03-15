using System;

namespace K3CSharp
{
    /// <summary>
    /// Specialized parser for creating binary operation AST nodes
    /// Handles converting token types to operator symbols and creating binary operation nodes
    /// </summary>
    public class BinaryOpFactory : IParserModule
    {
        public bool CanHandle(TokenType currentToken)
        {
            // This parser is used specifically for creating binary operation nodes
            // It's not directly called by token type, but by the parser when needed
            return false;
        }

        public ASTNode? Parse(ParseContext context)
        {
            // This method is not used directly - MakeBinaryOp is the main entry point
            throw new NotImplementedException("Use MakeBinaryOp method instead");
        }

        /// <summary>
        /// Create a binary operation AST node with the given operator and operands
        /// Converts token type to traditional operator symbol for the AST node value
        /// </summary>
        public static ASTNode MakeBinaryOp(TokenType op, ASTNode left, ASTNode right)
        {
            var node = new ASTNode(ASTNodeType.BinaryOp);
            if (left != null) node.Children.Add(left);
            if (right != null) node.Children.Add(right);
            
            // Convert token type to traditional operator symbol
            string symbolValue = op switch
            {
                TokenType.PLUS => "+",
                TokenType.MINUS => "-",
                TokenType.MULTIPLY => "*",
                TokenType.DIVIDE => "%",
                TokenType.DIV => "_div",
                TokenType.DOT_PRODUCT => "_dot",
                TokenType.MUL => "_mul",
                TokenType.LSQ => "_lsq",
                TokenType.MIN => "&",
                TokenType.MAX => "|",
                TokenType.LESS => "<",
                TokenType.GREATER => ">",
                TokenType.EQUAL => "=",
                TokenType.IN => "_in",
                TokenType.BIN => "_bin",
                TokenType.BINL => "_binl",
                TokenType.LIN => "_lin",
                TokenType.DV => "_dv",
                TokenType.DI => "_di",
                TokenType.VS => "_vs",
                TokenType.SV => "_sv",
                TokenType.SS => "_ss",
                TokenType.SM => "_sm",
                TokenType.CI => "_ci",
                TokenType.IC => "_ic",
                TokenType.DRAW => "_draw",
                TokenType.GETENV => "_getenv",
                TokenType.SETENV => "_setenv",
                TokenType.SIZE => "_size",
                TokenType.BD => "_bd",
                TokenType.DB => "_db",
                TokenType.LT => "_lt",
                TokenType.JD => "_jd",
                TokenType.DJ => "_dj",
                TokenType.GTIME => "_gtime",
                TokenType.LTIME => "_ltime",
                TokenType.MODULUS => "!",
                TokenType.JOIN => ",",
                TokenType.COLON => ":",
                TokenType.HASH => "#",
                TokenType.UNDERSCORE => "_",
                TokenType.QUESTION => "?",
                TokenType.MATCH => "~",
                TokenType.DOLLAR => "$",
                TokenType.APPLY => "@",
                TokenType.DOT_APPLY => ".",
                TokenType.TYPE => "TYPE",
                TokenType.STRING_REPRESENTATION => "STRING_REPRESENTATION",
                TokenType.AND => "_and",
                TokenType.OR => "_or",
                TokenType.XOR => "_xor",
                TokenType.ROT => "_rot",
                TokenType.SHIFT => "_shift",
                TokenType.NOT => "_not",
                TokenType.CEIL => "_ceil",
                TokenType.DO => "do",
                TokenType.WHILE => "while",
                TokenType.IF_FUNC => "if",
                _ => op.ToString()
            };
            
            node.Value = new SymbolValue(symbolValue);
            return node;
        }
    }
}
