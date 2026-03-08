using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace K3CSharp
{
    /// <summary>
    /// Thread-safe global registry for tracking .NET object instances
    /// Provides unique handles and manages object lifecycle
    /// </summary>
    public static class ObjectRegistry
    {
        private static readonly ConcurrentDictionary<string, object> _objects = new ConcurrentDictionary<string, object>();
        private static readonly ConcurrentDictionary<object, string> _handles = new ConcurrentDictionary<object, string>();
        private static int _nextHandle = 1;

        /// <summary>
        /// Register an object and return its handle
        /// </summary>
        /// <param name="obj">Object to register</param>
        /// <returns>Unique handle string</returns>
        public static string RegisterObject(object obj)
        {
            if (obj == null)
                return "null";

            // Check if object is already registered
            if (_handles.TryGetValue(obj, out string? existingHandle))
                return existingHandle;

            // Generate new handle
            string handle = $"obj{_nextHandle++}";
            
            // Register in both directions
            _objects[handle] = obj;
            _handles[obj] = handle;
            
            return handle;
        }

        /// <summary>
        /// Get object by handle
        /// </summary>
        /// <param name="handle">Object handle</param>
        /// <returns>Registered object or null</returns>
        public static object? GetObject(string? handle)
        {
            if (handle == "null" || handle == null)
                return null;

            _objects.TryGetValue(handle, out object? obj);
            return obj;
        }

        /// <summary>
        /// Get handle for object
        /// </summary>
        /// <param name="obj">Object to find handle for</param>
        /// <returns>Handle string or null</returns>
        public static string? GetHandle(object? obj)
        {
            if (obj == null)
                return "null";

            _handles.TryGetValue(obj, out string? handle);
            return handle;
        }

        /// <summary>
        /// Unregister object
        /// </summary>
        /// <param name="handle">Handle to remove</param>
        /// <returns>True if object was removed</returns>
        public static bool UnregisterObject(string? handle)
        {
            if (handle == "null" || handle == null)
                return false;

            if (_objects.TryRemove(handle, out object? obj))
            {
                _handles.TryRemove(obj!, out _);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get all registered handles
        /// </summary>
        /// <returns>Array of all handles</returns>
        public static string[] GetAllHandles()
        {
            var handles = new string[_objects.Count];
            _objects.Keys.CopyTo(handles, 0);
            return handles;
        }

        /// <summary>
        /// Clear all registered objects
        /// </summary>
        public static void Clear()
        {
            _objects.Clear();
            _handles.Clear();
            _nextHandle = 1;
        }

        /// <summary>
        /// Get count of registered objects
        /// </summary>
        public static int Count => _objects.Count;
    }
}
