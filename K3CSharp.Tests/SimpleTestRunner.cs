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
            RunAllTests();
        }
        
        private static void WriteResultsTable(List<TestResult> testResults)
        {
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "test_results_persistent.txt");
            
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
            public string FileName { get; set; } = "";
            public string ActualOutput { get; set; } = "";
            public string Expected { get; set; } = "";
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
                ("parentheses_nested.k", "7"),
                ("parentheses_nested1.k", "21"),
                ("parentheses_nested2.k", "0.7142857"),
                ("parentheses_nested3.k", "24"),
                ("parentheses_nested4.k", "15"),
                ("parentheses_nested5.k", "29"),
                ("parentheses_precedence.k", "7"),
                
                // K-style precedence tests (no parentheses)
                ("precedence_spec1.k", "11.5714286"),
                ("precedence_spec2.k", "6"),
                ("precedence_chain1.k", "1410"),
                ("precedence_chain2.k", "0.0709220"),
                ("precedence_mixed1.k", "0"),
                ("precedence_mixed2.k", "25"),
                ("precedence_mixed3.k", "11"),
                ("precedence_power1.k", "128"),
                ("precedence_power2.k", "83"),
                ("precedence_complex1.k", "0.7142857"),
                ("precedence_complex2.k", "10.0166667"),
                
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
                ("integer_types.k", "123456789L"),
                ("float_types.k", "3.14"),
                ("float_exponential.k", "170.0"),
                ("float_decimal_point.k", "10.0"),
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
                ("generate_operator.k", "0 0 0 0"),
                ("reverse_operator.k", "3 2 1"),
                ("count_operator.k", "3"),
                ("enumerate_operator.k", "0 1 2 3 4"),
                ("enlist_operator.k", ",5"),
                ("floor_operator.k", "3.0"),
                ("unique_operator.k", "1 2 3"),
                ("grade_up_operator.k", "0 4 2 3 1"),
                ("grade_down_operator.k", "1 2 3 4 0"),
                ("shape_operator.k", "3"),
                ("shape_operator_matrix.k", "3 3"),
                ("shape_operator_jagged.k", ",3"),
                ("shape_operator_scalar.k", "0"),
                ("shape_operator_vector.k", "5"),
                ("shape_operator_matrix_2x3.k", "2 3"),
                ("shape_operator_matrix_3x3.k", "3 3"),
                ("shape_operator_jagged_matrix.k", ",3"),
                ("shape_operator_tensor_3d.k", "3 2 2"),
                ("shape_operator_tensor_2x2x3.k", "2 2 3"),
                ("shape_operator_jagged_3d.k", "2 2"),
                ("shape_operator_empty_vector.k", "0"),
                ("where_operator.k", "0 2 3"),
                ("where_vector_counts.k", "0 0 0 1 1 2"),
                
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
                
                // Division tests
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
                
                // Special values
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
                
                // Overflow tests
                ("overflow_int_pos_inf.k", "0N"),
                ("overflow_int_pos_inf_plus2.k", "-0I"),
                ("overflow_int_neg_inf.k", "0N"),
                ("overflow_int_neg_inf_minus2.k", "0I"),
                ("overflow_int_max_plus1.k", "0N"),
                ("overflow_int_null_minus1.k", "0I"),
                ("overflow_long_max_plus1.k", "0NL"),
                ("overflow_long_min_minus1.k", "0IL"),
                ("overflow_long_neg_inf.k", "0NL"),
                ("overflow_long_neg_inf_minus2.k", "0IL"),
                ("overflow_long_pos_inf.k", "0NL"),
                ("overflow_long_pos_inf_plus2.k", "-0IL"),
                ("overflow_regular_int.k", "-2147483639"),
                ("underflow_regular_int.k", "2147483617"),
                
                // Vector tests with null
                ("vector_with_null.k", "(;1;2)"),
                ("vector_with_null_middle.k", "(1;;3)"),
                ("nested_vector_test.k", "(1 2 3;4 5 6)"),
                
                // Take operator
                ("take_operator_scalar.k", "42 42 42"),
                ("take_operator_overflow.k", "1 2 3 0 0 0 0 0 0 0"),
                
                // Function tests
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
                
                // Special arithmetic tests
                ("special_values_arithmetic.k", "-2147483646"),
                ("test_special_underflow.k", "2147483622"),
                ("test_special_underflow_2.k", "2147483549"),
                ("test_special_underflow_3.k", "2147482649"),
                ("test_special_0i_plus_1.k", "0N"),
                ("test_special_0n_plus_1.k", "-0I"),
                ("test_special_1_plus_neg0i.k", "-2147483646"),
                ("test_special_neg0i_plus_1.k", "-2147483646"),
                ("enumerate_empty_int.k", "!0"),
                ("enumerate_empty_long.k", "!0L"),
                
                // Symbol vectors
                ("symbol_vector_compact.k", "`a`b`c"),
                ("symbol_vector_spaces.k", "`a`b`c"),
                ("empty_mixed_vector.k", "()"),
                
                // Dictionary tests
                ("dictionary_empty.k", ".()"),
                ("dictionary_single.k", ".((`a;`b;))"),
                ("dictionary_multiple.k", ".((`a;1;);(`b;2;))"),
                ("dictionary_null_attributes.k", ".((`a;1;);(`b;2;))"),
                ("dictionary_with_null_value.k", ".((`a;1;);(`c;3;))"),
                ("mixed_list_with_null.k", "(1;;`test;42.5)"),
                ("dictionary_type.k", "5"),
                
                // Atom operator tests
                ("atom_scalar.k", "1"),
                ("atom_vector.k", "0"),
                
                // Attribute handle tests
                ("attribute_handle_symbol.k", "`a."),
                ("attribute_handle_vector.k", "`a.`b.`c."),
                
                // Mod operator tests
                ("mod_integer.k", "1"),
                ("mod_vector.k", "1 0 1 0"),
                
                // Grade tests without parentheses
                ("test_grade_up_no_parens.k", "0 4 2 3 1"),
                ("test_grade_down_no_parens.k", "1 2 3 4 0")
            };

            var testResults = new List<TestResult>();
            var testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");

            Console.WriteLine("Running K3CSharp Tests...");
            Console.WriteLine("=========================");

            foreach (var (fileName, expected) in tests)
            {
                try
                {
                    var scriptPath = Path.Combine(testScriptsPath, fileName);
                    if (!File.Exists(scriptPath))
                    {
                        Console.WriteLine($"✗ {fileName}: File not found");
                        testResults.Add(new TestResult { FileName = fileName, ActualOutput = "File not found", Expected = expected, Passed = false });
                        continue;
                    }

                    var script = File.ReadAllText(scriptPath);
                    var lexer = new Lexer(script);
                    var tokens = lexer.Tokenize();
                    var parser = new Parser(tokens);
                    var ast = parser.Parse();
                    var evaluator = new Evaluator();
                    var actualOutput = evaluator.Evaluate(ast).ToString().Trim();
                    var passed = actualOutput == expected;

                    if (passed)
                    {
                        Console.WriteLine($"✓ {fileName}: {actualOutput}");
                    }
                    else
                    {
                        Console.WriteLine($"✗ {fileName}: Expected '{expected}', got '{actualOutput}'");
                    }

                    testResults.Add(new TestResult { FileName = fileName, ActualOutput = actualOutput, Expected = expected, Passed = passed });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ {fileName}: Error - {ex.Message}");
                    testResults.Add(new TestResult { FileName = fileName, ActualOutput = $"Error: {ex.Message}", Expected = expected, Passed = false });
                }
            }

            var passedCount = testResults.Count(t => t.Passed);
            var totalCount = testResults.Count;
            
            Console.WriteLine();
            Console.WriteLine($"Test Results: {passedCount}/{totalCount} passed");
            
            WriteResultsTable(testResults);
        }
    }
}
