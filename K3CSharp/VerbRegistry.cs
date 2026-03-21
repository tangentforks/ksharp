using System;
using System.Collections.Generic;
using System.Linq;
using K3CSharp.Verbs;

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
            
            // Parse tree verbs
            RegisterVerb("_parse", VerbType.Operator, new[] { 1 }, new Func<K3Value[], K3Value>?[] { ParseVerbHandler.Parse, null });
            RegisterVerb("_eval", VerbType.Operator, new[] { 1 }, new Func<K3Value[], K3Value>?[] { EvalVerbHandler.Evaluate, null });
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
            RegisterVerb("DOT_MULTIPLY", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_dot", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("MUL", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_mul", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("INV", VerbType.Operator, new[] { 1 }, null);

            // System verbs (underscore functions)
            RegisterVerb("BIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_bin", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("BINL", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_binl", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("LIN", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_lin", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DV", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_dv", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("DI", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("_di", VerbType.Operator, new[] { 1, 2 }, null);
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
            RegisterVerb("LT", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("JD", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("DJ", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("CEIL", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("TIME", VerbType.Operator, new[] { 0 }, null);

            // Mathematical functions - register both symbol and token type names
            RegisterVerb("_log", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("LOG", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_exp", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("EXP", VerbType.Operator, new[] { 1 }, null);
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

            // Additional monadic system verbs
            RegisterVerb("_lt", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("LT", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_jd", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("JD", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_dj", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("DJ", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_T", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_in", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("IN", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_bd", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("BD", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_db", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("DB", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_not", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("NOT", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("_exit", VerbType.Operator, new[] { 1 }, null);
            RegisterVerb("EXIT", VerbType.Operator, new[] { 1 }, null);

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
            RegisterVerb("IO_VERB_4", VerbType.Operator, new[] { 1, 2 }, null);
            RegisterVerb("IO_VERB_5", VerbType.Operator, new[] { 1, 2 }, null);
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
            
            // Modified verbs (operators with adverbs)
            RegisterVerb("*:", VerbType.ProjectedFunction, new[] { 1, 2 }, null);
            RegisterVerb("/:", VerbType.ProjectedFunction, new[] { 1, 2 }, null);
            RegisterVerb("\\:", VerbType.ProjectedFunction, new[] { 1, 2 }, null);
        }

        private static void PopulateImplementationDelegates()
        {
            // Only implement basic arithmetic operators that have K3Value methods
            // All other operators will be handled by the evaluator
            UpdateVerbImplementations("+", new Func<K3Value[], K3Value>?[] { 
                null, // No monadic implementation for +
                args => args[0].Add(args[1])
            });
            
            UpdateVerbImplementations("-", new Func<K3Value[], K3Value>?[] { 
                null, // Monadic - will be handled by evaluator
                args => args[0].Subtract(args[1])
            });
            
            UpdateVerbImplementations("*", new Func<K3Value[], K3Value>?[] { 
                null, // No monadic implementation for *
                args => args[0].Multiply(args[1])
            });
            
            UpdateVerbImplementations("%", new Func<K3Value[], K3Value>?[] { 
                null, // Monadic % will be handled by evaluator
                args => args[0].Divide(args[1])
            });
            
            UpdateVerbImplementations(",", new Func<K3Value[], K3Value>?[] { 
                null, // No monadic implementation for ,
                args => {
                    // Implement join logic manually since it's commonly used
                    var elements = new List<K3Value>();
                    
                    if (args[0] is VectorValue vecA)
                    {
                        elements.AddRange(vecA.Elements);
                    }
                    else
                    {
                        elements.Add(args[0]);
                    }
                    
                    if (args[1] is VectorValue vecB)
                    {
                        elements.AddRange(vecB.Elements);
                    }
                    else
                    {
                        elements.Add(args[1]);
                    }
                    
                    return new VectorValue(elements);
                }
            });
            
            // Common modified verbs (operators with colons)
            UpdateVerbImplementations("*:", new Func<K3Value[], K3Value>?[] { 
                args => {
                    // Monadic *: is "first" - return first element
                    if (args[0] is VectorValue vec && vec.Elements.Count > 0)
                    {
                        return vec.Elements[0];
                    }
                    else
                    {
                        return args[0]; // First on scalar returns scalar
                    }
                },
                args => {
                    // Dyadic *: is "each" - apply to each element
                    if (args[0] is VectorValue vec)
                    {
                        var elements = new List<K3Value>();
                        foreach (var elem in vec.Elements)
                        {
                            elements.Add(elem.Multiply(args[1]));
                        }
                        return new VectorValue(elements);
                    }
                    else
                    {
                        return args[0].Multiply(args[1]);
                    }
                }
            });
            
            UpdateVerbImplementations("/:", new Func<K3Value[], K3Value>?[] { 
                args => {
                    // Monadic /: is "each-right" - identity
                    return args[0];
                },
                args => {
                    // Dyadic /: is "each-right" - apply right argument to each element of left
                    if (args[0] is VectorValue vec)
                    {
                        var elements = new List<K3Value>();
                        foreach (var elem in vec.Elements)
                        {
                            elements.Add(elem.Divide(args[1]));
                        }
                        return new VectorValue(elements);
                    }
                    else
                    {
                        return args[0].Divide(args[1]);
                    }
                }
            });
            
            UpdateVerbImplementations("\\:", new Func<K3Value[], K3Value>?[] { 
                args => {
                    // Monadic \: is "each-left" - identity
                    return args[0];
                },
                args => {
                    // Dyadic \: is "each-left" - apply left argument to each element of right
                    if (args[1] is VectorValue vec)
                    {
                        var elements = new List<K3Value>();
                        foreach (var elem in vec.Elements)
                        {
                            elements.Add(args[0].Divide(elem));
                        }
                        return new VectorValue(elements);
                    }
                    else
                    {
                        return args[0].Divide(args[1]);
                    }
                }
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
        /// Check if a verb can be used with adverbs
        /// </summary>
        public static bool SupportsAdverbs(string verbName, string? adverbType = null)
        {
            var verb = GetVerb(verbName);
            if (verb == null) 
                return false;
            
            // If no specific adverb type, check if verb supports any adverb-compatible operations
            if (string.IsNullOrEmpty(adverbType))
                return (verb.Type == VerbType.Operator || verb.Type == VerbType.Function) &&
                       verb.SupportedArities.Contains(1);
            
            // Check specific adverb requirements
            return adverbType switch
            {
                // Over (/) and Scan (\) - support monadic and dyadic verbs
                "ADVERB_SLASH" or "ADVERB_BACKSLASH" => verb.SupportedArities.Contains(1) || verb.SupportedArities.Contains(2),
                
                // Each (') - supports monadic verbs only
                "ADVERB_TICK" => verb.SupportedArities.Contains(1),
                
                // Each-right (/), Each-left (\), Each-prior (') - support dyadic verbs
                "ADVERB_SLASH_COLON" or "ADVERB_BACKSLASH_COLON" or "ADVERB_TICK_COLON" => verb.SupportedArities.Contains(2),
                
                _ => (verb.Type == VerbType.Operator || verb.Type == VerbType.Function) &&
                       verb.SupportedArities.Contains(1)
            };
        }

        /// <summary>
        /// Check if a token type is an adverb
        /// </summary>
        public static bool IsAdverbToken(TokenType tokenType)
        {
            return tokenType == TokenType.ADVERB_SLASH || tokenType == TokenType.ADVERB_BACKSLASH || tokenType == TokenType.ADVERB_TICK ||
                   tokenType == TokenType.ADVERB_SLASH_COLON || tokenType == TokenType.ADVERB_BACKSLASH_COLON || tokenType == TokenType.ADVERB_TICK_COLON;
        }

        /// <summary>
        /// Get the string representation of an adverb type
        /// </summary>
        public static string GetAdverbType(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.ADVERB_SLASH => "over",
                TokenType.ADVERB_BACKSLASH => "scan",
                TokenType.ADVERB_TICK => "each",
                TokenType.ADVERB_SLASH_COLON => "each-right",
                TokenType.ADVERB_BACKSLASH_COLON => "each-left",
                TokenType.ADVERB_TICK_COLON => "each-prior",
                _ => tokenType.ToString()
            };
        }

        /// <summary>
        /// Check if a token type is a binary operator
        /// </summary>
        public static bool IsBinaryOperatorToken(TokenType tokenType)
        {
            var verb = GetVerb(tokenType.ToString());
            return verb != null && verb.Type == VerbType.Operator;
        }

        /// <summary>
        /// Get tokens that should stop parsing until end of expression
        /// </summary>
        public static readonly TokenType[] ParseUntilEndStopTokens = {
            TokenType.RIGHT_PAREN, TokenType.RIGHT_BRACE, TokenType.SEMICOLON, TokenType.NEWLINE, TokenType.EOF
        };

        /// <summary>
        /// Get default stop tokens for expression parsing
        /// </summary>
        public static readonly TokenType[] DefaultStopTokens = {
            TokenType.PLUS, TokenType.MINUS, TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.DOT_PRODUCT, TokenType.MIN, TokenType.MAX,
            TokenType.LESS, TokenType.GREATER, TokenType.EQUAL, TokenType.IN, TokenType.POWER, TokenType.MODULUS,
            TokenType.COLON, TokenType.HASH, TokenType.UNDERSCORE, TokenType.QUESTION, TokenType.MATCH, TokenType.NEGATE, TokenType.DOLLAR, TokenType.RIGHT_PAREN,
            TokenType.RIGHT_BRACE, TokenType.RIGHT_BRACKET, TokenType.SEMICOLON, TokenType.NEWLINE, TokenType.ASSIGNMENT, TokenType.GLOBAL_ASSIGNMENT,
            TokenType.LEFT_BRACKET, TokenType.APPLY, TokenType.DOT_APPLY, TokenType.TYPE, TokenType.STRING_REPRESENTATION,
            TokenType.ADVERB_SLASH, TokenType.ADVERB_BACKSLASH, TokenType.ADVERB_TICK,
            TokenType.ADVERB_SLASH_COLON, TokenType.ADVERB_BACKSLASH_COLON, TokenType.ADVERB_TICK_COLON,
            TokenType.TIME, TokenType.IN, TokenType.BIN, TokenType.BINL, TokenType.LSQ, TokenType.LIN,
            TokenType.GTIME, TokenType.LTIME, TokenType.VS, TokenType.SV, TokenType.SS, TokenType.CI, TokenType.IC,
            TokenType.AND, TokenType.OR, TokenType.XOR, TokenType.ROT, TokenType.SHIFT,
            TokenType.DIRECTORY, TokenType.BD, TokenType.DB, TokenType.DO, TokenType.WHILE, TokenType.IF_FUNC, TokenType.EXIT, TokenType.EOF
        };

        /// <summary>
        /// Check if parsing should stop for given stop tokens
        /// </summary>
        public static bool ShouldStopParsing(TokenType currentToken, TokenType[] stopTokens)
        {
            return stopTokens.Contains(currentToken);
        }

        /// <summary>
        /// Get all binary operator token types
        /// </summary>
        public static readonly TokenType[] BinaryOperatorTokens = {
            TokenType.PLUS, TokenType.MINUS, TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.DOT_PRODUCT, TokenType.MUL, 
            TokenType.MIN, TokenType.MAX, TokenType.LESS, TokenType.GREATER, TokenType.EQUAL, TokenType.IN, 
            TokenType.POWER, TokenType.MODULUS, TokenType.JOIN, TokenType.COLON, TokenType.HASH, 
            TokenType.UNDERSCORE, TokenType.QUESTION, TokenType.MATCH, TokenType.NEGATE, TokenType.DOLLAR, 
            TokenType.LSQ, TokenType.AND, TokenType.OR, TokenType.XOR, TokenType.ROT, TokenType.SHIFT,
            TokenType.APPLY, TokenType.DOT_APPLY
        };

        /// <summary>
        /// Get string symbol for binary operator token
        /// </summary>
        public static string GetBinaryOperatorSymbol(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.PLUS => "+",
                TokenType.MINUS => "-",
                TokenType.MULTIPLY => "*",
                TokenType.DIVIDE => "%",
                TokenType.DOT_PRODUCT => "_dot",
                TokenType.MUL => ".*",
                TokenType.MIN => "&",
                TokenType.MAX => "|",
                TokenType.LESS => "<",
                TokenType.GREATER => ">",
                TokenType.EQUAL => "=",
                TokenType.IN => "in",
                TokenType.POWER => "^",
                TokenType.MODULUS => "!",
                TokenType.JOIN => ",",
                TokenType.COLON => ":",
                TokenType.HASH => "#",
                TokenType.UNDERSCORE => "_",
                TokenType.QUESTION => "?",
                TokenType.MATCH => "~",
                TokenType.NEGATE => "!",
                TokenType.DOLLAR => "$",
                TokenType.LSQ => "_lsq",
                TokenType.AND => "&",
                TokenType.OR => "|",
                TokenType.XOR => "^",
                TokenType.ROT => "rot",
                TokenType.SHIFT => "shift",
                TokenType.APPLY => "@",
                TokenType.DOT_APPLY => ".",
                TokenType.PARSE => "_parse",
                TokenType.EVAL => "_eval",
                TokenType.ADVERB_SLASH => "/",
                TokenType.ADVERB_BACKSLASH => "\\",
                TokenType.ADVERB_TICK => "'",
                TokenType.ADVERB_SLASH_COLON => "/:",
                TokenType.ADVERB_BACKSLASH_COLON => "\\:",
                TokenType.ADVERB_TICK_COLON => "':",
                _ => tokenType.ToString()
            };
        }

        /// <summary>
        /// Check if token type is a binary operator (comprehensive)
        /// </summary>
        public static bool IsBinaryOperator(TokenType tokenType)
        {
            return BinaryOperatorTokens.Contains(tokenType);
        }

        /// <summary>
        /// Get delimiter tokens (parentheses, brackets, braces)
        /// </summary>
        public static readonly TokenType[] DelimiterTokens = {
            TokenType.LEFT_PAREN, TokenType.RIGHT_PAREN,
            TokenType.LEFT_BRACE, TokenType.RIGHT_BRACE,
            TokenType.LEFT_BRACKET, TokenType.RIGHT_BRACKET
        };

        /// <summary>
        /// Check if a token is a delimiter
        /// </summary>
        public static bool IsDelimiterToken(TokenType tokenType)
        {
            return tokenType == TokenType.LEFT_PAREN || tokenType == TokenType.RIGHT_PAREN ||
                   tokenType == TokenType.LEFT_BRACE || tokenType == TokenType.RIGHT_BRACE ||
                   tokenType == TokenType.LEFT_BRACKET || tokenType == TokenType.RIGHT_BRACKET;
        }

        /// <summary>
        /// Get statement separator tokens
        /// </summary>
        public static readonly TokenType[] StatementSeparatorTokens = {
            TokenType.SEMICOLON, TokenType.NEWLINE
        };

        /// <summary>
        /// Check if a token is a statement separator
        /// </summary>
        public static bool IsStatementSeparatorToken(TokenType tokenType)
        {
            return tokenType == TokenType.SEMICOLON || tokenType == TokenType.NEWLINE;
        }

        /// <summary>
        /// Extract digit from I/O verb token
        /// </summary>
        public static int GetIOVerbDigit(TokenType tokenType)
        {
            return tokenType switch
            {
                TokenType.IO_VERB_0 => 0,
                TokenType.IO_VERB_1 => 1,
                TokenType.IO_VERB_2 => 2,
                TokenType.IO_VERB_3 => 3,
                TokenType.IO_VERB_4 => 4,
                TokenType.IO_VERB_5 => 5,
                TokenType.IO_VERB_6 => 6,
                TokenType.IO_VERB_7 => 7,
                TokenType.IO_VERB_8 => 8,
                TokenType.IO_VERB_9 => 9,
                _ => throw new ArgumentException($"Invalid I/O verb token: {tokenType}")
            };
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
