using System;
using System.Linq;

namespace K3CSharp.SerializationResearch
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var options = ParseCommandLine(args);
                if (options == null)
                {
                    ShowUsage();
                    return 1;
                }

                var explorer = new SerializationExplorer(options.KExePath, options.OutputDir);
                
                try
                {
                    var outputPath = explorer.ExploreSerialization(
                        options.DataType!.Value, 
                        options.Count, 
                        options.EdgeCasesOnly);

                    // Return output file path to Cascade Agent
                    Console.WriteLine(outputPath);
                    return 0;
                }
                finally
                {
                    explorer.Cleanup();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        private static CommandLineOptions? ParseCommandLine(string[] args)
        {
            var options = new CommandLineOptions();

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "--type":
                    case "-t":
                        if (i + 1 >= args.Length) return null;
                        options.DataType = ParseDataType(args[++i]);
                        break;

                    case "--count":
                    case "-c":
                        if (i + 1 >= args.Length) return null;
                        if (!int.TryParse(args[++i], out var count) || count < 0)
                            return null;
                        options.Count = count;
                        break;

                    case "--edge-cases":
                    case "-e":
                        options.EdgeCasesOnly = true;
                        break;

                    case "--kexe":
                    case "-k":
                        if (i + 1 >= args.Length) return null;
                        options.KExePath = args[++i];
                        break;

                    case "--output":
                    case "-o":
                        if (i + 1 >= args.Length) return null;
                        options.OutputDir = args[++i];
                        break;

                    case "--help":
                    case "-h":
                    case "/?":
                        return null;

                    default:
                        return null;
                }
            }

            // Validate required parameters
            if (options.DataType == null)
                return null;

            // If edge cases are specified, count is ignored (per spec)
            if (options.EdgeCasesOnly)
                options.Count = 0;

            return options;
        }

        private static DataType ParseDataType(string typeStr)
        {
            return typeStr.ToLower() switch
            {
                "integer" => DataType.Integer,
                "int" => DataType.Integer,
                "float" => DataType.Float,
                "character" => DataType.Character,
                "char" => DataType.Character,
                "symbol" => DataType.Symbol,
                "dictionary" => DataType.Dictionary,
                "dict" => DataType.Dictionary,
                "null" => DataType.Null,
                "anonymous" => DataType.AnonymousFunction,
                "function" => DataType.AnonymousFunction,
                "intvector" => DataType.IntegerVector,
                "floatvector" => DataType.FloatVector,
                "charvector" => DataType.CharacterVector,
                "symbolvector" => DataType.SymbolVector,
                "list" => DataType.List,
                _ => throw new ArgumentException($"Unknown data type: {typeStr}")
            };
        }

        private static void ShowUsage()
        {
            Console.WriteLine("K3CSharp Serialization Research Tool");
            Console.WriteLine("Explores k.exe _bd serialization patterns for binary compatibility");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  SerializationResearch.exe --type <datatype> [--count <number>] [--edge-cases] [--kexe <path>] [--output <dir>]");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("  --type, -t        Data type to explore (required)");
            Console.WriteLine("  --count, -c       Number of random examples to generate (default: 100)");
            Console.WriteLine("  --edge-cases, -e  Generate edge cases only (ignores --count)");
            Console.WriteLine("  --kexe, -k        Path to k.exe executable (default: c:\\k\\k.exe)");
            Console.WriteLine("  --output, -o      Output directory (default: output)");
            Console.WriteLine("  --help, -h        Show this help message");
            Console.WriteLine();
            Console.WriteLine("Supported Data Types:");
            Console.WriteLine("  integer, int      - 32-bit integers with special values");
            Console.WriteLine("  float             - IEEE 754 doubles with special values");
            Console.WriteLine("  character, char   - ASCII characters (0-255)");
            Console.WriteLine("  symbol            - Unquoted and quoted symbols");
            Console.WriteLine("  dictionary, dict  - Simple and complex dictionaries");
            Console.WriteLine("  null              - Null value");
            Console.WriteLine("  anonymous, func   - Anonymous functions");
            Console.WriteLine("  intvector         - Integer vectors");
            Console.WriteLine("  floatvector       - Float vectors");
            Console.WriteLine("  charvector        - Character vectors");
            Console.WriteLine("  symbolvector      - Symbol vectors");
            Console.WriteLine("  list              - Mixed type lists");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  SerializationResearch.exe --type integer --count 50");
            Console.WriteLine("  SerializationResearch.exe --type float --edge-cases");
            Console.WriteLine("  SerializationResearch.exe -t symbol -c 100 -e");
            Console.WriteLine();
            Console.WriteLine("Output:");
            Console.WriteLine("  The tool returns the path to the generated output file.");
            Console.WriteLine("  Output format: '_bd <input> → <serialized_result>' per line");
        }
    }

    public class CommandLineOptions
    {
        public DataType? DataType { get; set; }
        public int Count { get; set; } = 100;
        public bool EdgeCasesOnly { get; set; } = false;
        public string KExePath { get; set; } = @"c:\nonexistent\k.exe";
        public string OutputDir { get; set; } = "output";
    }
}
