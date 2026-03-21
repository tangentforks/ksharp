using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace K3CSharp.Parsing
{
    /// <summary>
    /// Shared JsonSerializer options for MCP server communication
    /// </summary>
    public static class JsonSerializerSettings
    {
        /// <summary>
        /// Default JSON serializer options with proper escape handling
        /// </summary>
        public static readonly JsonSerializerOptions Default = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        /// <summary>
        /// JSON serializer options for output (with proper escaping)
        /// </summary>
        public static readonly JsonSerializerOptions Output = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        /// <summary>
        /// JSON serializer options for input (lenient parsing)
        /// </summary>
        public static readonly JsonSerializerOptions Input = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
    }
}
