using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = @"t:/_src/github.com/ERufian/vibe-docs/ksharp/cleaned";
            string outputDir = @"t:/_src/github.com/ERufian/vibe-docs/ksharp/cleaned";
            
            Console.WriteLine("Merging K3 reference HTML files...");
            
            // Merge Part 1: kref_part1_*, kref_part2_*, kref_part3_* -> kref_part1.htm
            MergeFiles(
                inputDir,
                outputDir,
                new string[] { "kref_part1_Basics.htm", "kref_Part2_VerbsAG.htm", "kref_Part3_VerbsIW.htm" },
                "kref_part1.htm",
                "K3 Reference Manual - Part 1: Basics, Verbs A-G, Verbs I-W"
            );
            
            // Merge Part 2: kref_part4_*, kref_part5_*, kref_part6_* -> kref_part2.htm
            MergeFiles(
                inputDir,
                outputDir,
                new string[] { "kref_Part4_Adverbs_Assign_Amend_Apply.htm", "kref_Part5_Functions_Attributes_Control_IO.htm", "kref_Part6_Commands_System_UI.htm" },
                "kref_part2.htm",
                "K3 Reference Manual - Part 2: Adverbs, Functions, Commands"
            );
            
            Console.WriteLine("HTML merging completed!");
        }
        
        static void MergeFiles(string inputDir, string outputDir, string[] inputFiles, string outputFile, string title)
        {
            try
            {
                Console.WriteLine($"\nMerging into {outputFile}:");
                
                StringBuilder mergedContent = new StringBuilder();
                
                // Start HTML document
                mergedContent.AppendLine("<html>");
                mergedContent.AppendLine("<head>");
                mergedContent.AppendLine($"<title>{title}</title>");
                mergedContent.AppendLine(@"
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; margin: 40px; }
        h1 { color: #333; border-bottom: 2px solid #333; padding-bottom: 10px; }
        h2 { color: #666; border-bottom: 1px solid #666; padding-bottom: 5px; }
        h3 { color: #999; }
        h4, h5, h6 { color: #aaa; }
        p { margin: 10px 0; }
        code { background-color: #f4f4f4; padding: 2px 4px; border-radius: 3px; }
        pre { background-color: #f4f4f4; padding: 10px; border-radius: 5px; overflow-x: auto; }
        table { border-collapse: collapse; width: 100%; margin: 20px 0; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
        th { background-color: #f2f2f2; }
        ul, ol { margin: 10px 0; padding-left: 20px; }
        li { margin: 5px 0; }
        .file-section { border: 1px solid #ddd; margin: 30px 0; padding: 20px; border-radius: 5px; }
        .file-title { color: #333; font-size: 1.2em; font-weight: bold; margin-bottom: 15px; }
    </style>");
                mergedContent.AppendLine("</head>");
                mergedContent.AppendLine("<body>");
                
                // Add main title
                mergedContent.AppendLine($"<h1>{title}</h1>");
                
                foreach (string inputFile in inputFiles)
                {
                    string inputPath = Path.Combine(inputDir, inputFile);
                    
                    if (!File.Exists(inputPath))
                    {
                        Console.WriteLine($"  Warning: {inputFile} not found, skipping...");
                        continue;
                    }
                    
                    Console.WriteLine($"  Processing: {inputFile}");
                    
                    string content = File.ReadAllText(inputPath, Encoding.UTF8);
                    
                    // Extract body content
                    string bodyContent = ExtractBodyContent(content);
                    
                    // Add section divider
                    string fileName = Path.GetFileNameWithoutExtension(inputFile);
                    string sectionTitle = GetSectionTitle(fileName);
                    
                    mergedContent.AppendLine("<div class=\"file-section\">");
                    mergedContent.AppendLine($"<div class=\"file-title\">{sectionTitle}</div>");
                    mergedContent.AppendLine(bodyContent);
                    mergedContent.AppendLine("</div>");
                }
                
                // Close HTML document
                mergedContent.AppendLine("</body>");
                mergedContent.AppendLine("</html>");
                
                // Write merged file
                string outputPath = Path.Combine(outputDir, outputFile);
                File.WriteAllText(outputPath, mergedContent.ToString(), Encoding.UTF8);
                
                Console.WriteLine($"  Created: {outputFile}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error merging files: {e.Message}");
            }
        }
        
        static string ExtractBodyContent(string html)
        {
            // Extract content between <body> and </body> tags
            Match bodyMatch = Regex.Match(html, @"<body[^>]*>(.*?)</body>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            
            if (bodyMatch.Success)
            {
                return bodyMatch.Groups[1].Value;
            }
            
            // If no body tags found, return the content as-is
            return html;
        }
        
        static string GetSectionTitle(string fileName)
        {
            return fileName switch
            {
                "kref_part1_Basics" => "Chapter 1: Basics",
                "kref_Part2_VerbsAG" => "Chapter 2: Verbs A-G",
                "kref_Part3_VerbsIW" => "Chapter 3: Verbs I-W",
                "kref_Part4_Adverbs_Assign_Amend_Apply" => "Chapter 4: Adverbs, Assignment, Amend, Apply",
                "kref_Part5_Functions_Attributes_Control_IO" => "Chapter 5: Functions, Attributes, Control, I/O",
                "kref_Part6_Commands_System_UI" => "Chapter 6: Commands, System, UI",
                _ => fileName
            };
        }
    }
}
