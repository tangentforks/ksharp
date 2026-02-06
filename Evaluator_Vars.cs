using System;

namespace K3CSharp
{
    public partial class Evaluator
    {
        // Internal variable and system information functions
        
        public K3Value DirFunction(K3Value operand)
        {
            // _d (directory) function - returns current directory/branch
            return kTree.CurrentBranch != null ? kTree.CurrentBranch : new NullValue();
        }

        private K3Value VarFunction(K3Value operand)
        {
            throw new Exception("_v (variable) operation reserved for future use");
        }

        private K3Value IndexFunction(K3Value operand)
        {
            throw new Exception("_i (index) operation reserved for future use");
        }

        private K3Value FunctionFunction(K3Value operand)
        {
            throw new Exception("_f (function) operation reserved for future use");
        }

        private K3Value SpaceFunction(K3Value operand)
        {
            throw new Exception("_s (space) operation reserved for future use");
        }

        
        private K3Value PortFunction(K3Value operand)
        {
            throw new Exception("_p (port) operation reserved for future use");
        }

        private K3Value WhoFunction(K3Value operand)
        {
            throw new Exception("_w (who) operation reserved for future use");
        }

        private K3Value UserFunction(K3Value operand)
        {
            throw new Exception("_u (user) operation reserved for future use");
        }

        private K3Value AddressFunction(K3Value operand)
        {
            throw new Exception("_a (address) operation reserved for future use");
        }

        private K3Value VersionFunction(K3Value operand)
        {
            throw new Exception("_k (version) operation reserved for future use");
        }
    }
}
