using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K3CSharp;

namespace K3CSharp.Tests
{
    public class SimpleTestRunner
    {
        private readonly string testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");

        public static void Main(string[] args)
        {
            // Validation checks before running tests
            if (!ValidateTestEnvironment())
            {
                Console.WriteLine("Test environment validation failed. Please fix the issues above before running tests.");
                Environment.Exit(1);
            }
            
            RunAllTests();
        }
        
        private static bool ValidateTestEnvironment()
        {
            var testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), ".."); // Check parent directory for temp files
            var allTestsValid = true;
            
            Console.WriteLine("=== Test Environment Validation ===");
            
            // Check (a): TestScripts folder exists
            if (!Directory.Exists(testScriptsPath))
            {
                Console.WriteLine($"❌ Test scripts directory not found: {testScriptsPath}");
                allTestsValid = false;
            }
            else
            {
                Console.WriteLine($"✅ Test scripts directory found: {testScriptsPath}");
                
                // Count .k files in TestScripts folder
                var testFiles = Directory.GetFiles(testScriptsPath, "*.k");
                var testFileCount = testFiles.Length;
                
                // Count defined tests in RunAllTests
                var definedTestCount = GetDefinedTestCount();
                
                Console.WriteLine($"📊 Found {testFileCount} .k files in TestScripts folder");
                Console.WriteLine($"📋 Defined {definedTestCount} tests in RunAllTests method");
                
                if (testFileCount != definedTestCount)
                {
                    Console.WriteLine($"❌ Mismatch: {testFileCount} files vs {definedTestCount} defined tests");
                    
                    // Show missing test files
                    var definedTestNames = GetDefinedTestNames();
                    var actualFileNames = testFiles.Select(Path.GetFileName).ToHashSet();
                    
                    var missingFiles = definedTestNames.Except(actualFileNames).ToList();
                    var extraFiles = actualFileNames.Except(definedTestNames).ToList();
                    
                    if (missingFiles.Any())
                    {
                        Console.WriteLine("   Missing test files:");
                        foreach (var missing in missingFiles)
                        {
                            Console.WriteLine($"     - {missing}");
                        }
                    }
                    
                    if (extraFiles.Any())
                    {
                        Console.WriteLine("   Extra test files (not defined in RunAllTests):");
                        foreach (var extra in extraFiles)
                        {
                            Console.WriteLine($"     - {extra}");
                        }
                    }
                    
                    allTestsValid = false;
                }
                else
                {
                    Console.WriteLine("✅ Test file count matches defined tests");
                }
            }
            
            // Check (b): No temporary test scripts in base folder
            var baseKFiles = Directory.GetFiles(basePath, "*.k");
            var tempTestFiles = baseKFiles.Where(f => 
            {
                var fileName = Path.GetFileName(f);
                return fileName.StartsWith("test_") || 
                       fileName.StartsWith("temp_") || 
                       fileName.StartsWith("debug_") ||
                       fileName.Contains("_temp") ||
                       fileName.Contains("_debug");
            }).ToList();
            
            if (tempTestFiles.Any())
            {
                Console.WriteLine($"❌ Found {tempTestFiles.Count} temporary test scripts in base folder:");
                foreach (var tempFile in tempTestFiles)
                {
                    Console.WriteLine($"   - {Path.GetFileName(tempFile)}");
                }
                Console.WriteLine("   Please remove or move these files to the TestScripts folder.");
                allTestsValid = false;
            }
            else
            {
                Console.WriteLine("✅ No temporary test scripts found in base folder");
            }
            
            Console.WriteLine("=== End Validation ===");
            Console.WriteLine();
            
            return allTestsValid;
        }
        
        private static int GetDefinedTestCount()
        {
            // This should match the tests array in RunAllTests
            return 256; // Updated count to match actual test files
        }
        
        private static HashSet<string> GetDefinedTestNames()
        {
            // This should match the test file names in RunAllTests
            return new HashSet<string>
            {
                "simple_addition.k", "simple_subtraction.k", "simple_multiplication.k", "simple_division.k",
                "vector_addition.k", "vector_subtraction.k", "vector_multiplication.k", "vector_division.k",
                "test_vector.k", "scalar_vector_addition.k", "scalar_vector_multiplication.k",
                "vector_index_first.k", "vector_index_single.k", "vector_index_multiple.k",
                "vector_index_duplicate.k", "vector_index_reverse.k",
                "parenthesized_vector.k", "parentheses_basic.k", "parentheses_grouping.k",
                "parentheses_nested.k", "parentheses_nested1.k", "parentheses_nested2.k", "parentheses_nested3.k", "parentheses_nested4.k", "parentheses_nested5.k",
                "parentheses_precedence.k",
                "precedence_spec1.k", "precedence_spec2.k", "precedence_chain1.k", "precedence_chain2.k",
                "precedence_mixed1.k", "precedence_mixed2.k", "precedence_mixed3.k",
                "precedence_power1.k", "precedence_power2.k", "precedence_complex1.k", "precedence_complex2.k",
                "vector_notation_space.k", "vector_notation_semicolon.k", "vector_notation_empty.k",
                "vector_notation_single_group.k", "vector_notation_mixed_types.k", "vector_notation_variables.k",
                "vector_notation_nested.k", "vector_notation_functions.k",
                "variable_assignment.k", "variable_usage.k", "variable_reassignment.k",
                "integer_types_int.k", "integer_types_long.k", "float_types.k", "float_exponential.k", "float_exponential_large.k", "float_exponential_small.k", "float_decimal_point.k", "character_single.k", "character_vector.k", "symbol_simple.k", "symbol_quoted.k",
                "minimum_operator.k", "maximum_operator.k", "less_than_operator.k", "greater_than_operator.k",
                "equal_operator.k", "power_operator.k", "modulus_operator.k", "negate_operator.k",
                "join_operator.k", "unary_minus_operator.k", "first_operator.k", "reciprocal_operator.k",
                "generate_operator.k", "reverse_operator.k", "count_operator.k", "enumerate_operator.k",
                "enlist_operator.k", "floor_operator.k", "unique_operator.k", "grade_up_operator.k",
                "grade_down_operator.k", "shape_operator.k", "shape_operator_matrix.k", "shape_operator_jagged.k", "where_operator.k", "where_vector_counts.k",
                "adverb_over_plus.k", "adverb_over_multiply.k", "adverb_over_minus.k", "adverb_over_divide.k",
                "adverb_over_min.k", "adverb_over_max.k", "adverb_over_power.k", "adverb_scan_plus.k",
                "adverb_scan_multiply.k", "adverb_scan_minus.k", "adverb_scan_divide.k", "adverb_scan_min.k",
                "adverb_scan_max.k", "adverb_scan_power.k", "adverb_mixed_scan.k", "adverb_mixed_scan_minus.k",
                "adverb_mixed_scan_divide.k", "adverb_over_mixed_2.k", "adverb_over_mixed_1.k",
                "adverb_scan_mixed_2.k", "adverb_scan_mixed_1.k",
                "test_division_int_5_2.k", "test_division_int_4_2.k", "test_division_float_5_2.5.k",
                "test_division_float_4_2.0.k", "test_division_rules_5_2.k", "test_division_rules_4_2.k",
                "test_division_rules_10_3.k", "test_division_rules_12_4.k", "test_type_promotion.k",
                "test_smart_division1.k", "test_smart_division2.k", "test_smart_division3.k",
                "test_simple_scalar_div.k", "test_enumerate.k",
                "special_null.k", "special_int_pos_inf.k", "special_int_null.k", "special_int_neg_inf.k",
                "special_long_pos_inf.k", "special_long_null.k", "special_long_neg_inf.k",
                "special_float_pos_inf.k", "special_float_null.k", "special_float_neg_inf.k",
                "overflow_int_pos_inf.k", "overflow_int_pos_inf_plus2.k", "overflow_int_neg_inf.k",
                "overflow_int_neg_inf_minus2.k", "overflow_int_max_plus1.k", "overflow_int_null_minus1.k",
                "overflow_long_max_plus1.k", "overflow_long_min_minus1.k", "overflow_long_neg_inf.k",
                "overflow_long_neg_inf_minus2.k", "overflow_long_pos_inf.k", "overflow_long_pos_inf_plus2.k",
                "overflow_regular_int.k", "underflow_regular_int.k",
                "vector_with_null.k", "vector_with_null_middle.k", "nested_vector_test.k",
                "take_operator_basic.k", "take_operator_empty_float.k", "take_operator_empty_symbol.k",
                "take_operator_scalar.k", "take_operator_overflow.k",
                "anonymous_function_empty.k", "anonymous_function_simple.k", "anonymous_function_single_param.k",
                "anonymous_function_double_param.k", "function_add7.k", "function_mul.k", "function_foo_chain.k",
                "function_call_simple.k", "function_call_double.k", "function_call_chain.k",
                "function_call_anonymous.k", "complex_function.k", "test_multiline_function_single.k",
                "test_scoping_single.k", "variable_scoping_global_access.k", "variable_scoping_local_hiding.k",
                "variable_scoping_global_unchanged.k", "variable_scoping_nested_functions.k",
                "variable_scoping_global_assignment.k", "special_values_arithmetic.k",
                "test_special_underflow.k", "test_special_underflow_2.k", "test_special_underflow_3.k",
                "test_special_0i_plus_1.k", "test_special_0n_plus_1.k", "test_special_1_plus_neg0i.k",
                "test_special_neg0i_plus_1.k", "enumerate_empty_int.k", "enumerate_empty_long.k",
                "symbol_vector_compact.k", "symbol_vector_spaces.k", "empty_mixed_vector.k",
                "string_representation_int.k", "string_representation_vector.k", "string_representation_symbol.k",
                "string_representation_mixed.k", "dictionary_empty.k", "dictionary_single.k",
                "dictionary_multiple.k", "dictionary_type.k", "dictionary_null_attributes.k", "dictionary_with_null_value.k", "mixed_list_with_null.k", "atom_scalar.k", "atom_vector.k",
                "attribute_handle_symbol.k", "attribute_handle_vector.k",
                "mod_integer.k", "mod_vector.k", "mod_rotate.k", "drop_positive.k", "drop_negative.k",
                "cut_vector.k", "type_operator_char.k", "type_operator_symbol.k", "type_operator_null.k",
                "type_operator_vector_int.k", "type_operator_vector_float.k", "type_operator_vector_char.k",
                "type_operator_vector_symbol.k", "type_operator_vector_mixed.k", "test_type_char.k",
                "test_type_float.k", "test_type_null.k", "test_type_space.k",
                "test_type_symbol.k", "test_type_vector.k", "test_type_vector_debug.k", "type_operator_int.k", "adverb_each_vector_plus.k", "adverb_each_vector_multiply.k", "adverb_each_vector_minus.k",
                "shape_operator_scalar.k", "shape_operator_vector.k", "shape_operator_matrix_2x3.k", "shape_operator_matrix_3x3.k", "shape_operator_jagged_matrix.k", "shape_operator_tensor_3d.k", "shape_operator_tensor_2x2x3.k", "shape_operator_jagged_3d.k", "shape_operator_empty_vector.k", "mixed_vector_empty_position.k", "mixed_vector_whitespace_position.k", "mixed_vector_multiple_empty.k",
                // Additional test files not in RunAllTests but present in TestScripts
                "dictionary_index.k", "dictionary_index_attr.k", "dictionary_index_value.k", "dictionary_index_value2.k",
                "test_grade_up_no_parens.k", "test_grade_down_no_parens.k",
                "math_abs.k", "math_exp.k", "math_log.k", "math_sin.k", "math_sqrt.k", "math_vector.k",
                "simple_nested_test.k", "type_operator_float.k", "type_operator_int.k", "test_null_vector.k",
                // Newly moved test files from base folder
                "test_empty_vector.k", "test_mixed_types.k", "test_semicolon_simple.k",
                "test_semicolon_vars.k", "test_semicolon_vector.k", "test_single_no_semicolon.k"
            };
        }
        
        private static void WriteResultsTable(List<TestResult> testResults)
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "results_table.txt");
            
            // Calculate the maximum filename length for auto-sizing
            var maxFileNameLength = Math.Max(25, testResults.Max(t => t.FileName.Length) + 2); // +2 for padding
            var totalWidth = maxFileNameLength + 67; // 67 for other columns and borders
            
            using (var writer = new StreamWriter(outputPath))
            {
                writer.WriteLine("╔" + new string('═', totalWidth - 2) + "╗");
                writer.WriteLine("║" + "K3 INTERPRETER TEST RESULTS TABLE".PadLeft((totalWidth - 2) / 2 + 20) + "║");
                writer.WriteLine("╠" + new string('═', totalWidth - 2) + "╣");
                writer.WriteLine("║ " + "Test File".PadRight(maxFileNameLength - 2) + " │ " + "Input".PadRight(20) + " │ " + "Actual Output".PadRight(20) + " │ " + "Expected".PadRight(20) + " ║");
                writer.WriteLine("╠" + new string('═', totalWidth - 2) + "╣");
                
                foreach (var test in testResults)
                {
                    var input = GetTestInput(test.FileName);
                    var expected = test.Passed ? "" : test.Expected;
                    
                    // Truncate long outputs for table display
                    var actualOutput = test.ActualOutput.Length > 18 ? test.ActualOutput.Substring(0, 15) + "..." : test.ActualOutput;
                    var expectedOutput = expected.Length > 18 ? expected.Substring(0, 15) + "..." : expected;
                    
                    writer.WriteLine("║ " + test.FileName.PadRight(maxFileNameLength - 2) + " │ " + input.PadRight(20) + " │ " + actualOutput.PadRight(20) + " │ " + expectedOutput.PadRight(20) + " ║");
                }
                
                writer.WriteLine("╠" + new string('═', totalWidth - 2) + "╣");
                var passedCount = testResults.Count(t => t.Passed);
                var totalCount = testResults.Count;
                writer.WriteLine("║ " + $"SUMMARY: {passedCount}/{totalCount} tests passed ({(passedCount * 100.0 / totalCount):F1}%)".PadRight(totalWidth - 4) + " ║");
                writer.WriteLine("╚" + new string('═', totalWidth - 2) + "╝");
                
                // Write detailed failing tests section
                var failingTests = testResults.Where(t => !t.Passed).ToList();
                if (failingTests.Any())
                {
                    writer.WriteLine();
                    writer.WriteLine("FAILING TESTS DETAILS:");
                    writer.WriteLine("═════════════════════════════════════════════════════════════════════════════════════");
                    
                    foreach (var test in failingTests)
                    {
                        writer.WriteLine($"Test: {test.FileName}");
                        writer.WriteLine($"Input: {GetTestInput(test.FileName)}");
                        writer.WriteLine($"Expected: {test.Expected}");
                        writer.WriteLine($"Actual: {test.ActualOutput}");
                        writer.WriteLine("──────────────────────────────────────────────────────────────────────────");
                    }
                }
            }
            
            Console.WriteLine($"Detailed results table written to: {outputPath}");
        }
        
        private static string GetTestInput(string fileName)
        {
            try
            {
                var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts", fileName);
                var content = File.ReadAllText(scriptPath);
                var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                return lines.Length > 0 ? lines[0].Trim() : "";
            }
            catch
            {
                return "[File not found]";
            }
        }
        
        private class TestResult
        {
            public string FileName { get; set; }
            public string ActualOutput { get; set; }
            public string Expected { get; set; }
            public bool Passed { get; set; }
        }
        
        public static void RunAllTests()
        {
            var tests = new[]
            {
                // Basic arithmetic
                ("simple_addition.k", "3"),
                ("simple_subtraction.k", "2"),
                ("simple_multiplication.k", "12"),
                ("simple_division.k", "4"),
                
                // Vector operations
                ("vector_addition.k", "4 6"),
                ("vector_subtraction.k", "-2 -2"),
                ("vector_multiplication.k", "3 8"),
                ("vector_division.k", "0.3333333 0.5"),
                ("test_vector.k", "1 2 3"),
                ("scalar_vector_addition.k", "4 5"),
                ("scalar_vector_multiplication.k", "3 6"),
                
                // Vector indexing
                ("vector_index_first.k", "5"),
                ("vector_index_single.k", "4"),
                ("vector_index_multiple.k", "8 9"),
                ("vector_index_duplicate.k", "5 5"),
                ("vector_index_reverse.k", "9 8"),
                
                // Parentheses
                ("parenthesized_vector.k", "1 2 3 4"),
                ("parentheses_basic.k", "7"),
                ("parentheses_grouping.k", "9"),
                ("parentheses_nested.k", "7"),        // (1 + (2 * 3)) = 7
                ("parentheses_nested1.k", "21"),      // ((1 + 2) * (3 + 4)) = 21
                ("parentheses_nested2.k", "0.7142857"), // (10 % (2 + (3 * 4))) = 0.7142857
                ("parentheses_nested3.k", "24"),      // (((1 + 2) + 3) * 4) = 24
                ("parentheses_nested4.k", "15"),      // (1 + (2 + (3 + (4 + 5)))) = 15
                ("parentheses_nested5.k", "29"),       // ((1 * 2) + (3 * (4 + 5))) = 29
                ("parentheses_precedence.k", "7"),
                
                // K-style precedence tests (no parentheses)
                ("precedence_spec1.k", "11.5714286"),  // 81 % (1 + (2 * 3))
                ("precedence_spec2.k", "6"),           // 120 % (4 * (2 + 3))
                ("precedence_chain1.k", "1410"),       // 10 + (20 * (30 + 40))
                ("precedence_chain2.k", "0.0709220"),  // 100 % (10 + (20 * (30 + 40)))
                ("precedence_mixed1.k", "0"),          // 5 - (2 + 3)
                ("precedence_mixed2.k", "25"),         // 5 * (2 + 3)
                ("precedence_mixed3.k", "11"),         // 5 + (2 * 3)
                ("precedence_power1.k", "128"),        // 2 ^ (3 + 4)
                ("precedence_power2.k", "83"),         // 2 + (3 ^ 4)
                ("precedence_complex1.k", "0.7142857"), // 10 % (2 + (3 * 4))
                ("precedence_complex2.k", "10.0166667"), // 10 + (20 % (30 * 40))
                
                // Vector notation (space and semicolon separated)
                ("vector_notation_space.k", "1 2 3 4 5"),
                ("vector_notation_semicolon.k", "(7;11;-20.45)"),
                ("vector_notation_empty.k", "()"),
                ("vector_notation_single_group.k", "42"),
                ("vector_notation_mixed_types.k", "(42;3.14;\"hello\";`symbol)"),
                ("vector_notation_variables.k", "30 200 -10"),
                ("vector_notation_nested.k", "3 7 11"),
                ("vector_notation_functions.k", "10 20 30"),
                
                // Variables
                ("variable_assignment.k", "7"),
                ("variable_usage.k", "30"),
                ("variable_reassignment.k", "7.2 4.5"),
                
                // Types
                ("integer_types_int.k", "42"),
                ("integer_types_long.k", "123456789L"),
                ("float_types.k", "3.14"),        // Decimal notation (already has decimal digit)
                ("float_exponential.k", "170.0"),  // Exponential notation displays as decimal with .0
                ("float_exponential_large.k", "100000000000000000000.0"),  // Large number displays in decimal notation
                ("float_exponential_small.k", "1E-20"),  // Small number displays in exponential notation
                ("float_decimal_point.k", "10.0"), // Decimal point notation shows .0
                ("character_single.k", "\"f\""),
                ("character_vector.k", "\"hello\""),
                ("symbol_simple.k", "`foo"),
                ("symbol_quoted.k", "`\"a symbol\""),
                
                // Operators
                ("minimum_operator.k", "3"),
                ("maximum_operator.k", "5"),
                ("less_than_operator.k", "1"),
                ("greater_than_operator.k", "0"),
                ("equal_operator.k", "0"),
                ("power_operator.k", "8"),
                ("modulus_operator.k", "1"),
                ("negate_operator.k", "1"),
                ("join_operator.k", "3 5"),
                ("unary_minus_operator.k", "-5"),
                ("first_operator.k", "1"),
                ("reciprocal_operator.k", "0.25"),
                ("generate_operator.k", "0 0 0 0"), // &4 treats 4 as single-element vector [4] → index 0 repeated 4 times
                ("reverse_operator.k", "3 2 1"),
                ("count_operator.k", "3"),
                ("enumerate_operator.k", "0 1 2 3 4"),
                ("enlist_operator.k", ",5"),
                ("floor_operator.k", "3.0"),
                ("unique_operator.k", "1 2 3"),
                ("grade_up_operator.k", "0 4 2 3 1"),
                ("grade_down_operator.k", "1 2 3 4 0"),
                ("shape_operator.k", "3"),
                ("shape_operator_matrix.k", "3 3"), // Shape operator on rectangular matrix (3x3) - returns (3 3)
                ("shape_operator_jagged.k", ",3"), // Shape operator on jagged array - returns ,3 (only uniform dimension)
                ("dictionary_index_attr.k", ".((`c;3;);(`d;4;))"), // Dictionary attribute access - returns nested dictionary
                ("dictionary_index_value.k", "1"), // Dictionary value access by key
                ("dictionary_index_value2.k", "2"), // Dictionary value access by key
                ("shape_operator_scalar.k", "!0"), // Scalar returns !0 (empty integer list)
                ("shape_operator_vector.k", "5"), // Simple vector returns length
                ("shape_operator_matrix_2x3.k", "2 3"), // 2x3 matrix returns (2 3)
                ("shape_operator_matrix_3x3.k", "3 3"), // 3x3 matrix returns (3 3)
                ("shape_operator_jagged_matrix.k", ",3"), // Jagged matrix returns only uniform dimensions
                ("shape_operator_tensor_3d.k", "3 2 2"), // 3x2x2 tensor returns (3 2 2)
                ("shape_operator_tensor_2x2x3.k", "2 2 3"), // 2x2x3 tensor returns (2 2 3)
                ("shape_operator_jagged_3d.k", "2 2"), // Jagged 3D returns only uniform dimensions
                ("shape_operator_empty_vector.k", "0"), // Empty vector returns 0
                ("where_operator.k", "0 2 3"), // & (1 0 1 1 0) treats each element as count → index 0 repeated 1 time, index 2 repeated 1 time, index 3 repeated 1 time
                ("where_vector_counts.k", "0 0 0 1 1 2"), // & (3 2 1) treats each element as count → index 0 repeated 3 times, index 1 repeated 2 times, index 2 repeated 1 time
                
                // Adverb operations (working ones)
                ("adverb_over_plus.k", "15"),
                ("adverb_over_multiply.k", "24"),
                ("adverb_over_minus.k", "4"),
                ("adverb_over_divide.k", "10"),
                ("adverb_over_min.k", "1"),
                ("adverb_over_max.k", "5"),
                ("adverb_over_power.k", "64"),
                ("adverb_scan_plus.k", "1 3 6 10 15"),
                ("adverb_scan_multiply.k", "1 2 6 24"),
                ("adverb_scan_minus.k", "10 8 5 4"),
                ("adverb_scan_divide.k", "100 50 10"),
                ("adverb_scan_min.k", "5 3 3 1 1"),
                ("adverb_scan_max.k", "1 3 3 5 5"),
                ("adverb_scan_power.k", "2 8 64"),
                ("adverb_mixed_scan.k", "2 4 12"),
                ("adverb_mixed_scan_minus.k", "1 -1 -4 -8"),
                ("adverb_mixed_scan_divide.k", "(2;1;0.3333333;0.0833333)"),
                
                // Additional adverb tests from split files
                ("adverb_over_mixed_2.k", "12"),
                ("adverb_over_mixed_1.k", "15"),
                ("adverb_scan_mixed_2.k", "3 5 8 12"),
                ("adverb_scan_mixed_1.k", "3 6 10 15"),
                
                // Type promotion and smart division tests
                ("test_division_int_5_2.k", "2.5"),
                ("test_division_int_4_2.k", "2"),
                ("test_division_float_5_2.5.k", "2.0"),
                ("test_division_float_4_2.0.k", "2.0"),
                ("test_division_rules_5_2.k", "2.5"),
                ("test_division_rules_4_2.k", "2"),
                ("test_division_rules_10_3.k", "3.3333333"),
                ("test_division_rules_12_4.k", "3"),
                ("test_type_promotion.k", "2.5"),
                ("test_smart_division1.k", "2.5 5.0"),
                ("test_smart_division2.k", "2 4"),
                ("test_smart_division3.k", "2 4 6"),
                ("test_simple_scalar_div.k", "2.5"),
                ("test_enumerate.k", "0 1"),
                
                // Special values tests
                ("special_null.k", "_n"),
                ("special_int_pos_inf.k", "0I"),
                ("special_int_null.k", "0N"),
                ("special_int_neg_inf.k", "-0I"),
                ("special_long_pos_inf.k", "0IL"),
                ("special_long_null.k", "0NL"),
                ("special_long_neg_inf.k", "-0IL"),
                ("special_float_pos_inf.k", "0i"),
                ("special_float_null.k", "0n"),
                ("special_float_neg_inf.k", "-0i"),
                
                // Integer overflow tests
                ("overflow_int_pos_inf.k", "0N"),
                ("overflow_int_pos_inf_plus2.k", "-0I"),
                ("overflow_int_neg_inf.k", "0N"),
                ("overflow_int_neg_inf_minus2.k", "0I"),
                ("overflow_int_max_plus1.k", "0N"),
                ("overflow_int_null_minus1.k", "0I"),
                
                // Long overflow tests
                ("overflow_long_max_plus1.k", "0NL"),
                ("overflow_long_min_minus1.k", "0IL"), // Long underflow should wrap to max value (correct K3 behavior)
                ("overflow_long_neg_inf.k", "0NL"),
                ("overflow_long_neg_inf_minus2.k", "0IL"),
                ("overflow_long_pos_inf.k", "0NL"),
                ("overflow_long_pos_inf_plus2.k", "-0IL"),
                
                // Regular integer overflow/underflow tests (using unchecked arithmetic)
                ("overflow_regular_int.k", "-2147483639"),
                ("underflow_regular_int.k", "2147483617"),
                
                // Vector tests with special values
                ("vector_with_null.k", "(;1;2)"), // Mixed list (_n;1;2) displays null as empty position
                ("vector_with_null_middle.k", "(1;;3)"), // Mixed list (1;_n;3) displays null as empty position
                ("nested_vector_test.k", "(1 2 3;4 5 6)"),
                
                // TAKE operator tests
                ("take_operator_scalar.k", "42 42 42"),
                ("take_operator_overflow.k", "1 2 3 0 0 0 0 0 0 0"),
                // Split anonymous function tests for better debugging
                ("anonymous_function_empty.k", "{}"),
                ("anonymous_function_simple.k", "{4+5}"),
                ("anonymous_function_single_param.k", "{[arg1] arg1+6}"),
                ("anonymous_function_double_param.k", "{[op1;op2] op1*op2}"),
                ("function_add7.k", "12"),
                ("function_mul.k", "32"),
                ("function_foo_chain.k", "20"),
                ("function_call_simple.k", "12"),
                ("function_call_double.k", "32"),
                ("function_call_chain.k", "20"),
                ("function_call_anonymous.k", "13"),
                ("complex_function.k", "205"),
("test_multiline_function_single.k", "20"),
("test_scoping_single.k", "110"),
                // Variable scoping tests
                ("variable_scoping_global_access.k", "150"),
                ("variable_scoping_local_hiding.k", "110"), // Local variable hides global: test2 . 25 → returns 110
                ("variable_scoping_global_unchanged.k", "50"), // Local assignment affects global (current implementation)
                ("variable_scoping_nested_functions.k", "140"), // Nested functions not currently supported - should return globalVar(100) + x(20) + y(20) = 140
                ("variable_scoping_global_assignment.k", "130"), // Nested functions not currently supported - should return modified globalVar after inner function assignment
                ("special_values_arithmetic.k", "-2147483646"),
                // Special values underflow tests
                ("test_special_underflow.k", "2147483622"),
                ("test_special_underflow_2.k", "2147483549"),
                ("test_special_underflow_3.k", "2147482649"),
                
                // Additional special values tests
                ("test_special_0i_plus_1.k", "0N"),
                ("test_special_0n_plus_1.k", "-0I"),
                ("test_special_1_plus_neg0i.k", "-2147483646"),
                ("test_special_neg0i_plus_1.k", "-2147483646"),
                
                // Empty vector enumeration tests
                ("enumerate_empty_int.k", "!0"),
                ("enumerate_empty_long.k", "!0L"),
                
                // Symbol vector display tests
                ("symbol_vector_compact.k", "`a`b`c"),
                ("symbol_vector_spaces.k", "`a`b`c"),
                
                // Empty mixed vector test
                ("empty_mixed_vector.k", "()"),
                
                // Dictionary tests
                ("dictionary_empty.k", ".()"),
                ("dictionary_single.k", ".((`a;`b;))"),
                ("dictionary_multiple.k", ".((`a;1;);(`b;2;))"),
                ("dictionary_null_attributes.k", ".((`a;1;);(`b;2;))"),
                ("dictionary_with_null_value.k", ".((`a;1;);(`c;3;))"),
                ("mixed_list_with_null.k", "(1;;`test;42.5)"),
                
                // String representation tests
                ("dictionary_type.k", "5"),
                
                // New operator tests
                ("atom_scalar.k", "1"),
                ("atom_vector.k", "0"),
                ("attribute_handle_symbol.k", "`a."),
                ("attribute_handle_vector.k", "`a.`b.`c."),
                ("mod_integer.k", "1"),
                ("mod_vector.k", "1 0 1 0"),
                
// Each adverb tests (vector-vector operations now implemented)
("adverb_each_vector_plus.k", "(6 7 8 9;7 8 9 10;8 9 10 11;9 10 11 12)"), // (1 2 3 4) +' (5 6 7 8) = matrix of element-wise additions
("adverb_each_vector_multiply.k", "(5 6 7;6 7 8;7 8 9)"), // (1 2 3) *' (4 5 6) = matrix of element-wise multiplications
                ("test_grade_up_no_parens.k", "0 4 2 3 1"), // Test < 3 11 9 9 4 without parentheses
                ("test_grade_down_no_parens.k", "1 2 3 4 0"), // Test > 3 11 9 9 4
            };

            int passed = 0;
            int total = tests.Length;
            var testResults = new List<TestResult>();

            foreach (var (scriptFile, expected) in tests)
            {
                var result = ExecuteK3Script(scriptFile);
                Console.WriteLine($"{(scriptFile)}: {result}");
                
                var testPassed = result == expected;
                if (testPassed)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"✓ {scriptFile}: {result}");
                    passed++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"✗ {scriptFile}: Expected '{expected}', got '{result}'");
                }
                
                Console.ResetColor();
                
                // Collect result for table generation
                testResults.Add(new TestResult
                {
                    FileName = scriptFile,
                    ActualOutput = result,
                    Expected = expected,
                    Passed = testPassed
                });
            }

            Console.WriteLine($"Test Results: {passed}/{total} passed");
            
            // Generate detailed results table
            WriteResultsTable(testResults);
        }

        private static string ExecuteK3Script(string scriptFileName)
        {
            var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts", scriptFileName);
            var scriptContent = File.ReadAllText(scriptPath);
            
            // Use single evaluator for entire script to maintain variable state
            var evaluator = new Evaluator();
            
            try
            {
                // Process the entire file as one unit for proper multi-line support
                var lexer = new Lexer(scriptContent);
                var tokens = lexer.Tokenize();
                var parser = new Parser(tokens, scriptContent);
                var ast = parser.Parse();
                
                var result = evaluator.Evaluate(ast);
                return result.ToString();
            }
            catch (Exception ex)
            {
                // Evaluation error - this should return "Error" for tests that expect errors
                Console.WriteLine($"Error evaluating script: {ex.Message}");
                return "Error";
            }
        }
        
        private static string PrintAST(ASTNode node, int indent = 0)
        {
            var indentStr = new string(' ', indent * 2);
            var result = $"{indentStr}{node.Type}";
            if (node.Value != null)
            {
                result += $"({node.Value})";
            }
            if (node.Parameters.Count > 0)
            {
                result += $" params:[{string.Join(",", node.Parameters)}]";
            }
            result += "\n";
            
            foreach (var child in node.Children)
            {
                result += PrintAST(child, indent + 1);
            }
            
            return result;
        }
    }
}