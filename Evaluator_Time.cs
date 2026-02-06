using System;

namespace K3CSharp
{
    public partial class Evaluator
    {
        // Time-related functions
        
        private K3Value TimeFunction(K3Value operand)
        {
            throw new Exception("_t (current time) operation reserved for future use");
        }

        private K3Value LtFunction(K3Value operand)
        {
            throw new Exception("_lt (local time) operation reserved for future use");
        }

        private K3Value GtimeFunction(K3Value operand)
        {
            throw new Exception("_gtime (GMT time conversion) operation reserved for future use");
        }

        private K3Value JdFunction(K3Value operand)
        {
            throw new Exception("_jd (Julian date) operation reserved for future use");
        }

        private K3Value DjFunction(K3Value operand)
        {
            throw new Exception("_dj (date from Julian) operation reserved for future use");
        }
    }
}
