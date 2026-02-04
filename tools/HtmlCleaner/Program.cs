using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputDir = @"t:/_src/github.com/ERufian/vibe-docs/ksharp";
            string outputDir = @"t:/_src/github.com/ERufian/vibe-docs/ksharp/cleaned";
            
            // Create output directory
            Directory.CreateDirectory(outputDir);
            
            // Find all HTML files
            string[] htmlFiles = Directory.GetFiles(inputDir, "*.htm");
            
            Console.WriteLine($"Found {htmlFiles.Length} HTML files to clean");
            
            int cleanedCount = 0;
            foreach (string inputFile in htmlFiles)
            {
                string fileName = Path.GetFileName(inputFile);
                string outputFile = Path.Combine(outputDir, fileName);
                
                if (CleanHtmlFile(inputFile, outputFile))
                {
                    cleanedCount++;
                }
            }
            
            Console.WriteLine($"Successfully cleaned {cleanedCount}/{htmlFiles.Length} files");
            Console.WriteLine($"Cleaned files saved to: {outputDir}");
        }
        
        static bool CleanHtmlFile(string inputPath, string outputPath)
        {
            try
            {
                // Try different encodings, prioritize UTF-8
                string[] encodings = { "utf-8", "windows-1252", "iso-8859-1", "latin-1" };
                string content = null;
                
                foreach (string encoding in encodings)
                {
                    try
                    {
                        content = File.ReadAllText(inputPath, Encoding.GetEncoding(encoding));
                        // Check if content contains replacement characters
                        if (!content.Contains("\uFFFD"))
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                
                // If still has replacement characters, try to clean them
                if (content != null && content.Contains("\uFFFD"))
                {
                    // Replace replacement character with space or appropriate entity
                    content = content.Replace("\uFFFD", " ");
                }
                
                if (content == null)
                {
                    Console.WriteLine($"Could not decode {inputPath}");
                    return false;
                }
                
                // Replace non-ASCII characters with HTML entities
                content = ReplaceSpecialCharacters(content);
                
                // Basic HTML cleaning while preserving structure
                
                // Remove style tags and their content
                content = Regex.Replace(content, @"<style[^>]*>.*?</style>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                
                // Remove all style attributes
                content = Regex.Replace(content, @"style=""[^""]*""", "", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"style='[^']*'", "", RegexOptions.IgnoreCase);
                
                // Remove class attributes
                content = Regex.Replace(content, @"class=""[^""]*""", "", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"class='[^']*'", "", RegexOptions.IgnoreCase);
                
                // Convert common Word formatting to simple HTML
                content = Regex.Replace(content, @"<p[^>]*class=""MsoNormal""[^>]*>", "<p>", RegexOptions.IgnoreCase);
                
                // Remove span tags that are just for formatting
                content = Regex.Replace(content, @"<span[^>]*>(.*?)</span>", "$1", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                
                // Convert font size patterns to headings
                content = Regex.Replace(content, @"<b><span[^>]*font-size:14\.0pt[^>]*>(.*?)</span></b>", "<h1>$1</h1>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"<b><span[^>]*font-size:13\.5pt[^>]*>(.*?)</span></b>", "<h2>$1</h2>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"<b><span[^>]*font-size:12\.0pt[^>]*>(.*?)</span></b>", "<h3>$1</h3>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                
                // Clean up other common patterns
                content = Regex.Replace(content, @"<div[^>]*>", "<div>", RegexOptions.IgnoreCase);
                content = Regex.Replace(content, @"<p[^>]*>", "<p>", RegexOptions.IgnoreCase);
                
                // Remove empty tags
                content = Regex.Replace(content, @"<(\w+)>\s*</\1>", "", RegexOptions.Singleline);
                
                // Add basic CSS for readability
                string css = @"
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
    </style>";
                
                // Insert CSS after <head> or add head section
                if (content.Contains("<head>", StringComparison.OrdinalIgnoreCase))
                {
                    content = Regex.Replace(content, @"<head>", $"<head>{css}", RegexOptions.IgnoreCase);
                }
                else if (content.Contains("<html>", StringComparison.OrdinalIgnoreCase))
                {
                    content = Regex.Replace(content, @"<html>", $"<html><head>{css}</head>", RegexOptions.IgnoreCase);
                }
                else
                {
                    content = $"<html><head>{css}</head><body>{content}</body></html>";
                }
                
                // Write cleaned content
                File.WriteAllText(outputPath, content, Encoding.UTF8);
                
                Console.WriteLine($"Cleaned: {inputPath} -> {outputPath}");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error cleaning {inputPath}: {e.Message}");
                return false;
            }
        }
        
        static string ReplaceSpecialCharacters(string content)
        {
            // Replace common non-ASCII characters with HTML entities
            var replacements = new Dictionary<string, string>
            {
                // Em dash and en dash
                { "\u2014", "&mdash;" },  // em dash
                { "\u2013", "&ndash;" },  // en dash
                { "\u0097", "&ndash;" },  // en dash (Windows-1252 encoding)
                
                // Smart quotes
                { "\u201C", "&ldquo;" },  // left double quote
                { "\u201D", "&rdquo;" },  // right double quote
                { "\u2018", "&lsquo;" },  // left single quote
                { "\u2019", "&rsquo;" },  // right single quote
                
                // Common punctuation
                { "\u2026", "&hellip;" }, // ellipsis
                { "\u2022", "&bull;" },   // bullet
                { "\u2020", "&dagger;" }, // dagger
                { "\u2021", "&Dagger;" }, // double dagger
                { "\u2039", "&lsaquo;" }, // left single guillemet
                { "\u203A", "&rsaquo;" }, // right single guillemet
                { "\u00AB", "&laquo;" },  // left double guillemet
                { "\u00BB", "&raquo;" },  // right double guillemet
                
                // Currency symbols
                { "\u20AC", "&euro;" },   // euro
                { "\u00A3", "&pound;" },  // pound
                { "\u00A5", "&yen;" },    // yen
                { "\u00A2", "&cent;" },   // cent
                
                // Mathematical symbols
                { "\u00D7", "&times;" },  // multiplication
                { "\u00F7", "&divide;" }, // division
                { "\u00B1", "&plusmn;" }, // plus-minus
                { "\u2260", "&ne;" },     // not equal
                { "\u2264", "&le;" },     // less than or equal
                { "\u2265", "&ge;" },     // greater than or equal
                { "\u221E", "&infin;" },  // infinity
                { "\u2211", "&sum;" },    // summation
                { "\u220F", "&prod;" },   // product
                { "\u221A", "&radic;" },  // square root
                { "\u2202", "&part;" },   // partial differential
                { "\u2207", "&nabla;" },  // nabla
                { "\u222B", "&int;" },    // integral
                
                // Accented characters (common ones)
                { "\u00E1", "&aacute;" }, // á
                { "\u00C1", "&Aacute;" }, // Á
                { "\u00E9", "&eacute;" }, // é
                { "\u00C9", "&Eacute;" }, // É
                { "\u00ED", "&iacute;" }, // í
                { "\u00CD", "&Iacute;" }, // Í
                { "\u00F3", "&oacute;" }, // ó
                { "\u00D3", "&Oacute;" }, // Ó
                { "\u00FA", "&uacute;" }, // ú
                { "\u00DA", "&Uacute;" }, // Ú
                { "\u00F1", "&ntilde;" }, // ñ
                { "\u00D1", "&Ntilde;" }, // Ñ
                { "\u00FC", "&uuml;" },   // ü
                { "\u00DC", "&Uuml;" },   // Ü
                { "\u00E7", "&ccedil;" }, // ç
                { "\u00C7", "&Ccedil;" }, // Ç
                
                // Special spaces and dashes
                { "\u00A0", "&nbsp;" },   // non-breaking space
                { "\u2011", "&hyphen;" }, // non-breaking hyphen
                
                // Other common special characters
                { "\u00A9", "&copy;" },   // copyright
                { "\u00AE", "&reg;" },    // registered
                { "\u2122", "&trade;" },  // trademark
                { "\u00A7", "&sect;" },   // section
                { "\u00B6", "&para;" },   // paragraph
                { "\u00B0", "&deg;" },    // degree
                { "\u00B5", "&micro;" },  // micro
                { "\u03A9", "&Omega;" },  // Omega
                { "\u03B1", "&alpha;" },  // alpha
                { "\u03B2", "&beta;" },   // beta
                { "\u03B3", "&gamma;" },  // gamma
                { "\u03B4", "&delta;" },  // delta
                { "\u03B5", "&epsilon;" }, // epsilon
                { "\u03B8", "&theta;" },  // theta
                { "\u03BB", "&lambda;" }, // lambda
                { "\u03C0", "&pi;" },     // pi
                { "\u03C3", "&sigma;" },  // sigma
                { "\u03C6", "&phi;" },    // phi
                { "\u03C8", "&psi;" },    // psi
                { "\u03C7", "&chi;" },    // chi
                { "\u03C9", "&omega;" }   // omega
            };
            
            string result = content;
            
            // First, replace the Unicode replacement character with space
            result = result.Replace("\uFFFD", " ");
            
            // Apply all replacements
            foreach (var replacement in replacements)
            {
                result = result.Replace(replacement.Key, replacement.Value);
            }
            
            // Handle any remaining non-ASCII characters by converting to numeric entities
            StringBuilder sb = new StringBuilder();
            foreach (char c in result)
            {
                if (c > 127)
                {
                    sb.Append($"&#{(int)c};");
                }
                else
                {
                    sb.Append(c);
                }
            }
            
            return sb.ToString();
        }
    }
}
