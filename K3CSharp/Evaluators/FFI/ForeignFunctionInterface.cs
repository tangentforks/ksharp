using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace K3CSharp
{
    /// <summary>
    /// Foreign Function Interface for .NET interoperability
    /// </summary>
    public static class ForeignFunctionInterface
    {
        /// <summary>
        /// Global _dotnet tree structure for storing loaded assemblies and types
        /// </summary>
        private static DictionaryValue _dotnetTree = new DictionaryValue(new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>());

        /// <summary>
        /// Create a character vector from a string
        /// </summary>
        private static VectorValue CreateCharacterVectorFromString(string s)
        {
            var charElements = new List<K3Value>();
            foreach (char c in s)
            {
                charElements.Add(new CharacterValue(c.ToString()));
            }
            return new VectorValue(charElements, -3);
        }

        /// <summary>
        /// Create a constructor function for a .NET type
        /// </summary>
        /// <param name="type">The type to create constructor for</param>
        /// <returns>Function that creates instances of the type</returns>
        public static FunctionValue CreateConstructorFunction(Type type)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            
            // Create parameter list for first constructor (simplified)
            var firstConstructor = constructors.FirstOrDefault();
            var parameters = firstConstructor?.GetParameters().Select(p => p.Name ?? "").ToList() ?? new List<string>();
            
            return new FunctionValue(
                $"constructor:{type.Name}",
                parameters,
                null!,
                "",
                new SymbolValue("method"));
        }

        /// <summary>
        /// Create an instance of a .NET type with given arguments
        /// </summary>
        /// <param name="type">The type to instantiate</param>
        /// <param name="args">Constructor arguments</param>
        /// <returns>Object dictionary for the created instance</returns>
        public static DictionaryValue CreateInstance(Type type, List<K3Value> args)
        {
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            
            // Find best constructor match
            var bestConstructor = FindBestConstructorMatch(constructors, args);
            
            try
            {
                // Convert arguments to .NET types
                var parameters = bestConstructor.GetParameters();
                var netArgs = new object?[parameters.Length];
                
                for (int i = 0; i < Math.Min(args.Count, parameters.Length); i++)
                {
                    netArgs[i] = TypeMarshalling.K3ToNet(args[i], parameters[i].ParameterType);
                }
                
                // Create instance
                var instance = bestConstructor.Invoke(netArgs!);
                
                // Register instance and return object dictionary
                var result = TypeMarshalling.NetToK3(instance, new SymbolValue("object"));
                return result as DictionaryValue ?? throw new Exception("Failed to create object dictionary");
            }
            catch (Exception ex)
            {
                throw new Exception($"Instance creation failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Find the best constructor match for given arguments
        /// </summary>
        /// <param name="constructors">Available constructors</param>
        /// <param name="args">Arguments to match</param>
        /// <returns>Best matching constructor</returns>
        private static ConstructorInfo FindBestConstructorMatch(ConstructorInfo[] constructors, List<K3Value> args)
        {
            // Filter out constructors with complex types that we can't marshal
            var filteredConstructors = constructors.Where(c => 
            {
                foreach (var param in c.GetParameters())
                {
                    // Skip unsafe pointer types
                    if (param.ParameterType.IsPointer)
                        return false;
                    
                    // Skip span and readonly span types
                    if (param.ParameterType.IsGenericType && 
                        (param.ParameterType.GetGenericTypeDefinition() == typeof(Span<>) ||
                         param.ParameterType.GetGenericTypeDefinition() == typeof(ReadOnlySpan<>)))
                        return false;
                }
                return true;
            }).ToArray();
            
            // First try to find exact parameter count match
            var exactCountMatches = filteredConstructors.Where(c => c.GetParameters().Length == args.Count).ToList();
            
            if (exactCountMatches.Count == 1)
                return exactCountMatches[0];
            
            if (exactCountMatches.Count > 1)
            {
                // Multiple constructors with same parameter count - score and rank them
                var scoredConstructors = exactCountMatches.Select(ctor => new
                {
                    Constructor = ctor,
                    Score = ScoreConstructor(ctor, args)
                }).OrderByDescending(x => x.Score).First();
                
                return scoredConstructors.Constructor;
            }
            
            // No exact count match - find closest parameter count
            return filteredConstructors.OrderBy(c => Math.Abs(c.GetParameters().Length - args.Count)).First();
        }
        
        /// <summary>
        /// Score a constructor based on how well it matches the arguments
        /// </summary>
        private static int ScoreConstructor(ConstructorInfo ctor, List<K3Value> args)
        {
            var parameters = ctor.GetParameters();
            int score = 0;
            
            for (int i = 0; i < parameters.Length && i < args.Count; i++)
            {
                var paramType = parameters[i].ParameterType;
                var arg = args[i];
                
                // Heavily penalize unsafe pointer types (char*, void*, etc.)
                if (paramType.IsPointer)
                {
                    score -= 1000; // Never select unsafe constructors
                    continue;
                }
                
                // Prefer string over char[] for character vectors
                if (arg is VectorValue vector && vector.VectorType == -3)
                {
                    if (paramType == typeof(string))
                        score += 100; // Strong preference for string
                    else if (paramType == typeof(char[]))
                        score -= 50; // Penalize char[]
                }
                
                // Type compatibility bonus
                if (IsTypeCompatible(arg, paramType))
                    score += 10;
            }
            
            return score;
        }
        
        /// <summary>
        /// Check if a K3Value is compatible with a .NET type
        /// </summary>
        private static bool IsTypeCompatible(K3Value kValue, Type targetType)
        {
            // Character vectors (strings) are compatible with string type
            if (kValue is VectorValue charVector && charVector.VectorType == -3 && targetType == typeof(string))
                return true;
            
            // Character vectors are also compatible with char[]
            if (kValue is VectorValue vector && vector.VectorType == -3 && targetType == typeof(char[]))
                return true;
            
            // Integers are compatible with numeric types
            if (kValue is IntegerValue && (targetType == typeof(int) || targetType == typeof(long) || 
                targetType == typeof(float) || targetType == typeof(double)))
                return true;
            
            // Floats are compatible with numeric types
            if (kValue is FloatValue && (targetType == typeof(float) || targetType == typeof(double)))
                return true;
            
            return false;
        }

        /// <summary>
        /// Get the global _dotnet tree
        /// </summary>
        /// <returns>The _dotnet dictionary containing all loaded .NET assemblies</returns>
        public static DictionaryValue GetDotNetTree()
        {
            return _dotnetTree;
        }

        /// <summary>
        /// Store an assembly in the _dotnet tree
        /// </summary>
        /// <param name="assembly">The assembly to store</param>
        public static void StoreAssemblyInDotNetTree(Assembly assembly)
        {
            var assemblyName = assembly.GetName().Name ?? "";
            var assemblySymbol = new SymbolValue(assemblyName);
            
            // Create assembly dictionary
            var assemblyDict = CreateAssemblyDictionary(assembly);
            
            // Store in _dotnet tree
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>(_dotnetTree.Entries);
            entries[assemblySymbol] = (assemblyDict, null);
            _dotnetTree = new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a .NET assembly
        /// </summary>
        /// <param name="assembly">The assembly to represent</param>
        /// <returns>K dictionary with assembly information and types</returns>
        private static DictionaryValue CreateAssemblyDictionary(Assembly assembly)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            // Add basic assembly information
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(assembly.GetName().Name ?? ""), null);
            entries[new SymbolValue("fullname")] = (CreateCharacterVectorFromString(assembly.FullName ?? ""), null);
            entries[new SymbolValue("location")] = (CreateCharacterVectorFromString(assembly.Location ?? ""), null);
            entries[new SymbolValue("version")] = (CreateCharacterVectorFromString(assembly.GetName().Version?.ToString() ?? ""), null);
            
            // Add all defined types
            var types = assembly.GetTypes();
            if (types.Length > 0)
            {
                var typeList = new List<K3Value>();
                foreach (var type in types)
                {
                    if (type.IsPublic) // Only include public types
                    {
                        var typeDict = CreateNetTypeDictionary(type);
                        typeList.Add(typeDict);
                    }
                }
                entries[new SymbolValue("types")] = (new VectorValue(typeList), null);
            }
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a .NET type with hierarchical namespace
        /// </summary>
        /// <param name="type">The type to represent</param>
        /// <returns>K dictionary with type information</returns>
        public static DictionaryValue CreateNetTypeDictionary(Type type)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            // Add basic type information
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(type.Name ?? ""), null);
            entries[new SymbolValue("fullname")] = (CreateCharacterVectorFromString(type.FullName ?? type.Name ?? ""), null);
            entries[new SymbolValue("namespace")] = (CreateCharacterVectorFromString(type.Namespace ?? ""), null);
            entries[new SymbolValue("assembly")] = (CreateCharacterVectorFromString(type.Assembly.GetName().Name ?? ""), null);
            
            // Add type characteristics
            entries[new SymbolValue("isclass")] = (new IntegerValue(type.IsClass ? 1 : 0), null);
            entries[new SymbolValue("isinterface")] = (new IntegerValue(type.IsInterface ? 1 : 0), null);
            entries[new SymbolValue("isenum")] = (new IntegerValue(type.IsEnum ? 1 : 0), null);
            entries[new SymbolValue("isabstract")] = (new IntegerValue(type.IsAbstract ? 1 : 0), null);
            entries[new SymbolValue("ispublic")] = (new IntegerValue(type.IsPublic ? 1 : 0), null);
            entries[new SymbolValue("isstatic")] = (new IntegerValue(type.IsAbstract && type.IsSealed ? 1 : 0), null);
            
            // Add constructors
            var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (constructors.Length > 0)
            {
                foreach (var constructor in constructors)
                {
                    var constructorDict = CreateConstructorDictionary(constructor);
                    entries[new SymbolValue(constructor.Name)] = (constructorDict, null);
                }
                // Add main constructor entry
                entries[new SymbolValue("constructor")] = (CreateConstructorFunction(type), null);
            }
            
            // Add static methods
            var staticMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public);
            if (staticMethods.Length > 0)
            {
                foreach (var method in staticMethods)
                {
                    var methodDict = CreateMethodDictionary(method);
                    entries[new SymbolValue(method.Name)] = (methodDict, null);
                }
            }
            
            // Add instance methods
            var instanceMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
            if (instanceMethods.Length > 0)
            {
                foreach (var method in instanceMethods)
                {
                    var methodDict = CreateMethodDictionary(method);
                    entries[new SymbolValue(method.Name)] = (methodDict, null);
                }
            }
            
            // Add properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (properties.Length > 0)
            {
                foreach (var property in properties)
                {
                    var propertyDict = CreatePropertyDictionary(property);
                    entries[new SymbolValue(property.Name)] = (propertyDict, null);
                }
            }
            
            // Add fields
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (fields.Length > 0)
            {
                foreach (var field in fields)
                {
                    var fieldDict = CreateFieldDictionary(field);
                    entries[new SymbolValue(field.Name)] = (fieldDict, null);
                }
            }
            
            // Add events
            var events = type.GetEvents(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (events.Length > 0)
            {
                foreach (var eventInfo in events)
                {
                    var eventDict = CreateEventDictionary(eventInfo);
                    entries[new SymbolValue(eventInfo.Name)] = (eventDict, null);
                }
            }
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a constructor
        /// </summary>
        private static DictionaryValue CreateConstructorDictionary(ConstructorInfo constructor)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(constructor.Name), null);
            entries[new SymbolValue("ispublic")] = (new IntegerValue(constructor.IsPublic ? 1 : 0), null);
            entries[new SymbolValue("isstatic")] = (new IntegerValue(constructor.IsStatic ? 1 : 0), null);
            
            var parameters = constructor.GetParameters();
            if (parameters.Length > 0)
            {
                var paramList = new List<K3Value>();
                foreach (var param in parameters)
                {
                    var paramDict = CreateParameterDictionary(param);
                    paramList.Add(paramDict);
                }
                entries[new SymbolValue("parameters")] = (new VectorValue(paramList), null);
            }
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a method
        /// </summary>
        private static DictionaryValue CreateMethodDictionary(MethodInfo method)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(method.Name), null);
            entries[new SymbolValue("isstatic")] = (new IntegerValue(method.IsStatic ? 1 : 0), null);
            entries[new SymbolValue("ispublic")] = (new IntegerValue(method.IsPublic ? 1 : 0), null);
            entries[new SymbolValue("isvirtual")] = (new IntegerValue(method.IsVirtual ? 1 : 0), null);
            entries[new SymbolValue("isabstract")] = (new IntegerValue(method.IsAbstract ? 1 : 0), null);
            entries[new SymbolValue("returntype")] = (CreateCharacterVectorFromString(method.ReturnType.Name), null);
            
            var parameters = method.GetParameters();
            if (parameters.Length > 0)
            {
                var paramList = new List<K3Value>();
                foreach (var param in parameters)
                {
                    var paramDict = CreateParameterDictionary(param);
                    paramList.Add(paramDict);
                }
                entries[new SymbolValue("parameters")] = (new VectorValue(paramList), null);
            }
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a property
        /// </summary>
        private static DictionaryValue CreatePropertyDictionary(PropertyInfo property)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(property.Name), null);
            entries[new SymbolValue("canread")] = (new IntegerValue(property.CanRead ? 1 : 0), null);
            entries[new SymbolValue("canwrite")] = (new IntegerValue(property.CanWrite ? 1 : 0), null);
            entries[new SymbolValue("propertytype")] = (CreateCharacterVectorFromString(property.PropertyType.Name), null);
            entries[new SymbolValue("isstatic")] = (new IntegerValue(
                property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic ?? false ? 1 : 0), null);
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a field
        /// </summary>
        private static DictionaryValue CreateFieldDictionary(FieldInfo field)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(field.Name), null);
            entries[new SymbolValue("fieldtype")] = (CreateCharacterVectorFromString(field.FieldType.Name), null);
            entries[new SymbolValue("isstatic")] = (new IntegerValue(field.IsStatic ? 1 : 0), null);
            entries[new SymbolValue("ispublic")] = (new IntegerValue(field.IsPublic ? 1 : 0), null);
            entries[new SymbolValue("isreadonly")] = (new IntegerValue(field.IsInitOnly ? 1 : 0), null);
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing an event
        /// </summary>
        private static DictionaryValue CreateEventDictionary(EventInfo eventInfo)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(eventInfo.Name), null);
            entries[new SymbolValue("eventtype")] = (CreateCharacterVectorFromString(eventInfo.EventHandlerType?.Name ?? ""), null);
            entries[new SymbolValue("isstatic")] = (new IntegerValue(
                eventInfo.GetAddMethod()?.IsStatic ?? eventInfo.GetRemoveMethod()?.IsStatic ?? false ? 1 : 0), null);
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Create a K dictionary representing a method parameter
        /// </summary>
        private static DictionaryValue CreateParameterDictionary(ParameterInfo parameter)
        {
            var entries = new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>();
            
            entries[new SymbolValue("name")] = (CreateCharacterVectorFromString(parameter.Name ?? ""), null);
            entries[new SymbolValue("type")] = (CreateCharacterVectorFromString(parameter.ParameterType.Name), null);
            entries[new SymbolValue("position")] = (new IntegerValue(parameter.Position), null);
            entries[new SymbolValue("isoptional")] = (new IntegerValue(parameter.IsOptional ? 1 : 0), null);
            
            return new DictionaryValue(entries);
        }

        /// <summary>
        /// Get a specific assembly from the _dotnet tree
        /// </summary>
        /// <param name="assemblyName">The assembly name to retrieve</param>
        /// <returns>The assembly dictionary or null if not found</returns>
        public static DictionaryValue? GetAssemblyFromDotNetTree(string assemblyName)
        {
            var assemblySymbol = new SymbolValue(assemblyName);
            if (_dotnetTree.Entries.TryGetValue(assemblySymbol, out var assemblyEntry))
            {
                return assemblyEntry.Value as DictionaryValue;
            }
            return null;
        }

        /// <summary>
        /// Clear the _dotnet tree (useful for testing)
        /// </summary>
        public static void ClearDotNetTree()
        {
            _dotnetTree = new DictionaryValue(new Dictionary<SymbolValue, (K3Value Value, DictionaryValue? Attribute)>());
        }
    }
}
