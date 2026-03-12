using System;
using System.Collections.Generic;
using System.Linq;

namespace K3CSharp
{
    public enum VerbType
    {
        Operator,
        SystemVariable,
        Function,
        ProjectedFunction
    }

    public class VerbInfo
    {
        public string Name { get; set; } = "";
        public int[] SupportedArities { get; set; } = Array.Empty<int>();
        public Func<K3Value[], K3Value>?[]? Implementations { get; set; }
        public VerbType Type { get; set; }
        public bool IsSystemVariable { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// Centralized registry for all verb metadata and implementations
    /// </summary>
    public static class VerbRegistry
    {
        private static readonly Dictionary<string, VerbInfo> verbs = new Dictionary<string, VerbInfo>();
        
        // Performance optimization: caching for frequently accessed operations
        private static readonly Dictionary<string, VerbType> verbTypeCache = new Dictionary<string, VerbType>();
        private static readonly Dictionary<string, bool> systemVariableCache = new Dictionary<string, bool>();
        private static readonly Dictionary<string, int[]> supportedAritiesCache = new Dictionary<string, int[]>();
        
        // Performance monitoring
        private static int lookupCount = 0;
        private static int cacheHits = 0;

        static VerbRegistry()
        {
            InitializeBasicOperators();
            PopulateImplementationDelegates();
        }

        public static void RegisterVerb(string name, VerbType type, int[] arities, Func<K3Value[], K3Value>?[]? implementations, bool isSystemVariable = false, string? description = null)
        {
            verbs[name] = new VerbInfo
            {
                Name = name,
                SupportedArities = arities,
                Implementations = implementations,
                Type = type,
                IsSystemVariable = isSystemVariable,
                Description = description
            };
        }

        public static VerbInfo? GetVerb(string name)
        {
            lookupCount++;
            return verbs.TryGetValue(name, out var verbInfo) ? verbInfo : null;
        }

        /// <summary>
        /// Get verb type with caching optimization
        /// </summary>
        public static VerbType GetVerbType(string verbName)
        {
            if (verbTypeCache.TryGetValue(verbName, out var cachedType))
            {
                cacheHits++;
                return cachedType;
            }
            
            var verb = GetVerb(verbName);
            var type = verb?.Type ?? VerbType.Function;
            verbTypeCache[verbName] = type;
            return type;
        }

        /// <summary>
        /// Check if verb is system variable with caching optimization
        /// </summary>
        public static bool IsSystemVariable(string verbName)
        {
            if (systemVariableCache.TryGetValue(verbName, out var cachedResult))
            {
                cacheHits++;
                return cachedResult;
            }
            
            var verb = GetVerb(verbName);
            var isSystemVar = verb?.Type == VerbType.SystemVariable;
            systemVariableCache[verbName] = isSystemVar;
            return isSystemVar;
        }

        /// <summary>
        /// Get supported arities with caching optimization
        /// </summary>
        public static int[] GetSupportedArities(string verbName)
        {
            if (supportedAritiesCache.TryGetValue(verbName, out var cachedArities))
            {
                cacheHits++;
                return cachedArities;
            }
            
            var verb = GetVerb(verbName);
            var arities = verb?.SupportedArities ?? new int[0];
            supportedAritiesCache[verbName] = arities;
            return arities;
        }

        /// <summary>
        /// Clear all caches (useful for testing or memory management)
        /// </summary>
        public static void ClearCaches()
        {
            verbTypeCache.Clear();
            systemVariableCache.Clear();
            supportedAritiesCache.Clear();
            lookupCount = 0;
            cacheHits = 0;
        }

        /// <summary>
        /// Get cache performance statistics
        /// </summary>
        public static Dictionary<string, object> GetCacheStats()
        {
            var hitRate = lookupCount > 0 ? (double)cacheHits / lookupCount * 100 : 0;
            return new Dictionary<string, object>
            {
                ["LookupCount"] = lookupCount,
                ["CacheHits"] = cacheHits,
                ["HitRate"] = $"{hitRate:F2}%",
                ["VerbTypeCacheSize"] = verbTypeCache.Count,
                ["SystemVariableCacheSize"] = systemVariableCache.Count,
                ["SupportedAritiesCacheSize"] = supportedAritiesCache.Count
            };
        }

        public static IEnumerable<VerbInfo> GetAllVerbs()
        {
            return verbs.Values;
        }

        public static bool IsVerb(string tokenName)
        {
            return verbs.ContainsKey(tokenName);
        }

        public static VerbInfo? GetVerb(TokenType tokenType)
        {
            var tokenName = tokenType.ToString();
            return GetVerb(tokenName);
        }

        private static void InitializeBasicOperators()
        {
            // Basic mathematical operators - register both symbol and token type names
            RegisterVerb("+", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("PLUS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("-", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MINUS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("*", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MULTIPLY", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("%", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DIVIDE", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("&", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("|", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MAX", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("^", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("POWER", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("!", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MODULUS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb(",", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("JOIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("#", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("HASH", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("UNDERSCORE", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("?", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("QUESTION", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("=", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("EQUAL", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("$", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DOLLAR", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("~", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MATCH", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("@", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("APPLY", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb(".", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DOT", VerbType.Operator, new[] { 1, 2 }, null);

            // Additional mathematical operators
            RegisterVerb("DIV", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MUL", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("AND", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("OR", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("XOR", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("ROT", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("SHIFT", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MAX", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("LESS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("GREATER", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MATCH", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("POWER", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MODULUS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("JOIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("COLON", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("HASH", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("UNDERSCORE", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("QUESTION", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DOLLAR", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DRAW", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("TYPE", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("STRING_REPRESENTATION", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("APPLY", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DOT_APPLY", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("LSQ", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DOT", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MUL", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("INV", VerbType.Operator, new[] { 1 }, null);

            // System verbs (underscore functions)
            RegisterVerb("BIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("BINL", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("LIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DV", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DI", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("VS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("SV", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("SS", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_ss", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("SM", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_sm", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("CI", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_ci", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IC", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_ic", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("GETENV", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("SETENV", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("SIZE", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DIRECTORY", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("BD", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DB", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DO", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("WHILE", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IF", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("EXIT", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_dispose", VerbType.Function, new[] { 1 }, null);
            RegisterVerb("HINT", VerbType.Operator, new[] { 1, 2 }, null);
            
            // Also register lowercase versions for identifier recognition
            RegisterVerb("do", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("while", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("if", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("exit", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("dispose", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("hint", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("NULL", VerbType.Operator, new[] { 0 }, null);
            RegisterVerb("GTIME", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("LTIME", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("LT", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("JD", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("DJ", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("CEIL", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("TIME", VerbType.Operator, new[] { 0 }, null);

            // Mathematical functions - register both symbol and token type names
            RegisterVerb("_abs", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("ABS", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_sqr", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("SQR", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_sqrt", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("SQRT", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_floor", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("FLOOR_MATH", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_sin", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("SIN", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_cos", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("COS", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_tan", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("TAN", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_asin", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("ASIN", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_acos", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("ACOS", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_atan", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("ATAN", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_sinh", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("SINH", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_cosh", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("COSH", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_tanh", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("TANH", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("NOT", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("NEGATE", VerbType.Operator, new[] { 1 }, null);

            // Adverbs
            RegisterVerb("ADVERB_SLASH", VerbType.Operator, new[] { 2 }, null);
            RegisterVerb("ADVERB_BACKSLASH", VerbType.Operator, new[] { 2 }, null);
            RegisterVerb("ADVERB_TICK", VerbType.Operator, new[] { 2 }, null);
            RegisterVerb("ADVERB_SLASH_COLON", VerbType.Operator, new[] { 2 }, null);
            RegisterVerb("ADVERB_BACKSLASH_COLON", VerbType.Operator, new[] { 2 }, null);
            RegisterVerb("ADVERB_TICK_COLON", VerbType.Operator, new[] { 2 }, null);

            // I/O verbs (0:, 1:, 2:, 3:, 4:, 5:, 6:, 7:, 8:, 9:)
            for (int i = 0; i <= 9; i++)
            {
                var ioNum = i.ToString();
                RegisterVerb(ioNum, VerbType.Operator, new[] { 1, 2 }, null);
            }
            
            // Also register with the TokenType names that the evaluator expects
            RegisterVerb("IO_VERB_0", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_1", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_2", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_3", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_6", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_7", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_8", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_9", VerbType.Operator, new[] { 1, 2 }, null);

            // System variables (0-arity)
            RegisterVerb("_d", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_v", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_i", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_f", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_n", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_s", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_h", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_p", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_P", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_w", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_u", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_a", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_k", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_o", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_c", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_r", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_m", VerbType.SystemVariable, new[] { 0 }, null);
            RegisterVerb("_y", VerbType.SystemVariable, new[] { 0 }, null);
            
            // Also register NULL for TokenType.NULL
            RegisterVerb("NULL", VerbType.SystemVariable, new[] { 0 }, null);
            
            // Hint system functions
            RegisterVerb("_gethint", VerbType.Function, new[] { 1 }, null);
            RegisterVerb("GETHINT", VerbType.Function, new[] { 1 }, null);
            RegisterVerb("_sethint", VerbType.Operator, new[] { 2 }, null);
            RegisterVerb("SETHINT", VerbType.Operator, new[] { 2 }, null);
        }

        private static void PopulateImplementationDelegates()
        {
            // Create an evaluator instance to get method references
            var evaluator = new Evaluator();
            
            // Basic mathematical operators - use CallVariableFunction as unified entry point
            UpdateVerbImplementations("+", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("+", args.ToList()),
                args => evaluator.CallVariableFunction("+", args.ToList())
            });
            
            UpdateVerbImplementations("-", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("-", args.ToList()),
                args => evaluator.CallVariableFunction("-", args.ToList())
            });
            
            UpdateVerbImplementations("*", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("*", args.ToList()),
                args => evaluator.CallVariableFunction("*", args.ToList())
            });
            
            UpdateVerbImplementations("%", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("%", args.ToList()),
                args => evaluator.CallVariableFunction("%", args.ToList())
            });
            
            // Mathematical functions (monadic only)
            UpdateVerbImplementations("_abs", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_abs", args.ToList())
            });
            
            UpdateVerbImplementations("_sin", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_sin", args.ToList())
            });
            
            UpdateVerbImplementations("_cos", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_cos", args.ToList())
            });
            
            UpdateVerbImplementations("_tan", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_tan", args.ToList())
            });
            
            UpdateVerbImplementations("_atan", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_atan", args.ToList())
            });
            
            // Comparison operators
            UpdateVerbImplementations("<", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("<", args.ToList())
            });
            
            UpdateVerbImplementations(">", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction(">", args.ToList())
            });
            
            UpdateVerbImplementations("=", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("=", args.ToList())
            });
            
            UpdateVerbImplementations("~", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("~", args.ToList())
            });
            
            // Additional mathematical operators
            UpdateVerbImplementations("^", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("^", args.ToList())
            });
            
            UpdateVerbImplementations(",", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction(",", args.ToList())
            });
            
            UpdateVerbImplementations("#", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("#", args.ToList())
            });
            
            // System functions
            UpdateVerbImplementations("_ic", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_ic", args.ToList())
            });
            
            UpdateVerbImplementations("_ci", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_ci", args.ToList())
            });
            
            UpdateVerbImplementations("_val", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_val", args.ToList())
            });
            
            // Additional mathematical functions
            UpdateVerbImplementations("_log", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_log", args.ToList())
            });
            
            UpdateVerbImplementations("_exp", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_exp", args.ToList())
            });
            
            UpdateVerbImplementations("_sqrt", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_sqrt", args.ToList())
            });
            
            UpdateVerbImplementations("_ceil", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_ceil", args.ToList())
            });
            
            UpdateVerbImplementations("_floor", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_floor", args.ToList())
            });
            
            UpdateVerbImplementations("_neg", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_neg", args.ToList())
            });
            
            UpdateVerbImplementations("_not", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_not", args.ToList())
            });
            
            // Vector operations
            UpdateVerbImplementations("_sv", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_sv", args.ToList())
            });
            
            UpdateVerbImplementations("_vs", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_vs", args.ToList())
            });
            
            UpdateVerbImplementations("_dv", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_dv", args.ToList())
            });
            
            UpdateVerbImplementations("_di", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_di", args.ToList())
            });
            
            // String operations
            UpdateVerbImplementations("_string", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_string", args.ToList())
            });
            
            UpdateVerbImplementations("_type", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_type", args.ToList())
            });
            
            // Conditional operations
            UpdateVerbImplementations("$", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("$", args.ToList())
            });
            
            UpdateVerbImplementations("if", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("if", args.ToList())
            });
            
            UpdateVerbImplementations("while", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("while", args.ToList())
            });
            
            // Hint system functions
            UpdateVerbImplementations("_gethint", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_gethint", args.ToList())
            });
            
            UpdateVerbImplementations("_sethint", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_sethint", args.ToList())
            });
            
            // FFI functions
            UpdateVerbImplementations("_dispose", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("_dispose", args.ToList())
            });
            
            UpdateVerbImplementations("do", new Func<K3Value[], K3Value>?[] { 
                args => evaluator.CallVariableFunction("do", args.ToList())
            });
        }

        private static void UpdateVerbImplementations(string name, Func<K3Value[], K3Value>?[]? implementations)
        {
            if (verbs.TryGetValue(name, out var verb))
            {
                verb.Implementations = implementations;
            }
        }

        /// <summary>
        /// Get the preferred arity for an operator based on context and position
        /// </summary>
        public static int GetPreferredArity(string verbName, bool hasLeftOperand, bool hasRightOperand = false)
        {
            var verb = GetVerb(verbName);
            if (verb == null) return 0;
            
            // For multi-arity operators, determine preferred arity based on context
            if (verb.SupportedArities.Length > 1)
            {
                // Hash (#) is context-sensitive: monadic for count, dyadic for take
                if (verbName == "#")
                {
                    return hasLeftOperand && hasRightOperand ? 2 : 1;
                }
                
                // Underscore (_) is context-sensitive: monadic for floor, dyadic for conditional
                if (verbName == "_")
                {
                    return hasLeftOperand && hasRightOperand ? 2 : 1;
                }
                
                // Dollar ($) is context-sensitive: monadic for count, dyadic for each
                if (verbName == "$")
                {
                    return hasLeftOperand && hasRightOperand ? 2 : 1;
                }
                
                // At (@) is context-sensitive: monadic for type, dyadic for apply
                if (verbName == "@")
                {
                    return hasLeftOperand && hasRightOperand ? 2 : 1;
                }
                
                // For other multi-arity operators, prefer dyadic if we have both operands
                if (hasLeftOperand && hasRightOperand && verb.SupportedArities.Contains(2))
                    return 2;
                
                // Prefer monadic if we only have one operand or no left operand
                if (!hasLeftOperand && verb.SupportedArities.Contains(1))
                    return 1;
            }
            
            // Return the highest supported arity as fallback
            return verb.SupportedArities.Length > 0 ? verb.SupportedArities.Max() : 0;
        }

        /// <summary>
        /// Check if an operator supports a specific arity
        /// </summary>
        public static bool SupportsArity(string verbName, int arity)
        {
            var verb = GetVerb(verbName);
            return verb != null && verb.SupportedArities.Contains(arity);
        }

        /// <summary>
        /// Validate verb arity and provide helpful error message
        /// </summary>
        public static string ValidateVerbArity(string verbName, int arity)
        {
            var verb = GetVerb(verbName);
            if (verb == null)
                return $"Unknown verb: {verbName}";
            
            if (!verb.SupportedArities.Contains(arity))
            {
                var aritiesStr = string.Join(", ", verb.SupportedArities);
                return $"Verb '{verbName}' does not support {arity} argument{(arity == 1 ? "" : "s")}. Supported arities: [{aritiesStr}]";
            }
            
            return string.Empty; // No error
        }

        /// <summary>
        /// Fast check if verb exists - optimized for performance
        /// </summary>
        public static bool HasVerb(string verbName)
        {
            return verbs.ContainsKey(verbName);
        }

        /// <summary>
        /// Get all verbs of a specific type
        /// </summary>
        public static IEnumerable<string> GetVerbsByType(VerbType type)
        {
            return verbs.Where(kvp => kvp.Value.Type == type).Select(kvp => kvp.Key);
        }

        /// <summary>
        /// Get verb description for debugging/help
        /// </summary>
        public static string GetVerbDescription(string verbName)
        {
            var verb = GetVerb(verbName);
            return verb?.Description ?? $"Unknown verb: {verbName}";
        }

        /// <summary>
        /// Check if a verb is a projected function (adverb-modified)
        /// </summary>
        public static bool IsProjectedFunction(string verbName)
        {
            var verb = GetVerb(verbName);
            return verb?.Type == VerbType.ProjectedFunction;
        }

        /// <summary>
        /// Get the remaining required arguments for a projected function
        /// </summary>
        public static int GetRemainingArity(string verbName)
        {
            var verb = GetVerb(verbName);
            if (verb?.Type == VerbType.ProjectedFunction)
            {
                // For projected functions, return the remaining arity
                // This is a simplified implementation - in a full version, we'd track projection state
                return verb.SupportedArities.Length > 0 ? verb.SupportedArities.Max() - 1 : 0;
            }
            return verb?.SupportedArities.Length > 0 ? verb.SupportedArities.Max() : 0;
        }

        /// <summary>
        /// Register a new projected function dynamically
        /// </summary>
        public static void RegisterProjectedFunction(string name, int[] supportedArities, string description)
        {
            var projectedVerb = new VerbInfo
            {
                Name = name,
                SupportedArities = supportedArities,
                Type = VerbType.ProjectedFunction,
                Description = description
            };
            
            verbs[name] = projectedVerb;
        }

        /// <summary>
        /// Get verbs that support higher-order operations (adverbs)
        /// </summary>
        public static IEnumerable<string> GetHigherOrderVerbs()
        {
            return verbs.Where(kvp => 
                kvp.Value.Type == VerbType.Operator && 
                kvp.Value.SupportedArities.Contains(1))
                .Select(kvp => kvp.Key);
        }

        /// <summary>
        /// Check if a verb can be used with adverbs
        /// </summary>
        public static bool SupportsAdverbs(string verbName)
        {
            var verb = GetVerb(verbName);
            return verb != null && 
                   (verb.Type == VerbType.Operator || verb.Type == VerbType.Function) &&
                   verb.SupportedArities.Contains(1);
        }

        /// <summary>
        /// Get performance statistics for the VerbRegistry
        /// </summary>
        public static Dictionary<string, object> GetPerformanceStats()
        {
            var cacheStats = GetCacheStats();
            return new Dictionary<string, object>
            {
                ["TotalVerbs"] = verbs.Count,
                ["Operators"] = verbs.Count(kvp => kvp.Value.Type == VerbType.Operator),
                ["SystemVariables"] = verbs.Count(kvp => kvp.Value.Type == VerbType.SystemVariable),
                ["Functions"] = verbs.Count(kvp => kvp.Value.Type == VerbType.Function),
                ["ProjectedFunctions"] = verbs.Count(kvp => kvp.Value.Type == VerbType.ProjectedFunction),
                ["VerbsWithImplementations"] = verbs.Count(kvp => kvp.Value.Implementations != null),
                ["MultiArities"] = verbs.Count(kvp => kvp.Value.SupportedArities.Length > 1)
            }.Union(cacheStats).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Export verb registry information for debugging/documentation
        /// </summary>
        public static string ExportVerbInfo()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("=== Verb Registry Information ===");
            sb.AppendLine($"Total verbs: {verbs.Count}");
            sb.AppendLine();
            
            foreach (var type in Enum.GetValues<VerbType>())
            {
                var verbsByType = verbs.Where(kvp => kvp.Value.Type == type).OrderBy(kvp => kvp.Key);
                sb.AppendLine($"{type} ({verbsByType.Count()}):");
                
                foreach (var kvp in verbsByType)
                {
                    var verb = kvp.Value;
                    var arities = string.Join(",", verb.SupportedArities);
                    var hasImpl = verb.Implementations != null ? "✓" : "✗";
                    sb.AppendLine($"  {verb.Name} [{arities}] {hasImpl} - {verb.Description}");
                }
                sb.AppendLine();
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Validate the integrity of the VerbRegistry
        /// </summary>
        public static List<string> ValidateRegistry()
        {
            var issues = new List<string>();
            
            foreach (var kvp in verbs)
            {
                var verb = kvp.Value;
                
                // Check for empty name
                if (string.IsNullOrEmpty(verb.Name))
                {
                    issues.Add("Verb with empty name found");
                }
                
                // Check for empty supported arities
                if (verb.SupportedArities.Length == 0)
                {
                    issues.Add($"Verb '{verb.Name}' has no supported arities");
                }
                
                // Check for invalid arities (negative or zero)
                if (verb.SupportedArities.Any(a => a <= 0))
                {
                    issues.Add($"Verb '{verb.Name}' has invalid arities: [{string.Join(",", verb.SupportedArities)}]");
                }
                
                // Check for duplicate arities
                var uniqueArities = verb.SupportedArities.Distinct().ToArray();
                if (uniqueArities.Length != verb.SupportedArities.Length)
                {
                    issues.Add($"Verb '{verb.Name}' has duplicate arities");
                }
                
                // Check if implementations array matches supported arities
                if (verb.Implementations != null)
                {
                    if (verb.Implementations.Length < verb.SupportedArities.Max())
                    {
                        issues.Add($"Verb '{verb.Name}' implementations array is too small for max arity");
                    }
                }
            }
            
            return issues;
        }
    }
}
