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
            public string FileName { get; set; } = "";
            public string ActualOutput { get; set; } = "";
            public string Expected { get; set; } = "";
            public bool Passed { get; set; }
        }
        
        public static void RunAllTests()
        {
            // Comprehensive test list with expected results from K.exe
            var tests = new[]
            {
                // Adverb Each tests (from K.exe results)
                ("adverb_each_vector_minus.k", "9 18 27"),
                ("adverb_each_vector_multiply.k", "4 10 18"),
                ("adverb_each_vector_plus.k", "6 8 10 12"),
                
                // Adverb Over tests
                ("adverb_over_divide.k", "10"),
                ("adverb_over_max.k", "5"),
                ("adverb_over_min.k", "1"),
                ("adverb_over_minus.k", "4"),
                ("adverb_over_multiply.k", "24"),
                ("adverb_over_plus.k", "15"),
                ("plus_over_empty.k", "0"),
                ("multiply_over_empty.k", "1"),
                ("adverb_over_power.k", "64"),
                ("adverb_over_with_initialization_1.k", "15"),
                ("adverb_over_with_initialization_2.k", "12"),
                
                // Adverb Scan tests
                ("adverb_scan_divide.k", "100 50 10"),
                ("adverb_scan_max.k", "1 3 3 5 5"),
                ("adverb_scan_min.k", "5 3 3 1 1"),
                ("adverb_scan_minus.k", "10 8 5 4"),
                ("adverb_scan_multiply.k", "1 2 6 24"),
                ("adverb_scan_plus.k", "1 3 6 10 15"),
                ("adverb_scan_power.k", "2 8 64"),
                ("adverb_scan_with_initialization_1.k", "1 3 6 10 15"),
                ("adverb_scan_with_initialization_2.k", "2 3 5 8 12"),
                ("adverb_scan_with_initialization_divide.k", "(2;2;1;0.3333333;0.08333333)"), // Test %\ scan with divide
                ("adverb_scan_with_initialization_minus.k", "2 1 -1 -4 -8"),
                ("adverb_scan_with_initialization.k", "2 2 4 12"),
                
                // Anonymous Function tests
                ("anonymous_function_double_param.k", "{[op1;op2] op1*op2}"),
                ("anonymous_function_empty.k", "{}"),
                ("anonymous_function_simple.k", "{4+5}"),
                ("anonymous_function_single_param.k", "{[arg1] arg1+6}"),
                
                // Atom tests
                ("atom_scalar.k", "1"),
                ("atom_vector.k", "0"),
                
                // Attribute Handle tests
                ("attribute_handle_symbol.k", "`a."),
                ("attribute_handle_vector.k", "`a.`b.`c."),
                
                // Character tests
                ("character_single.k", "\"f\""),
                ("character_vector.k", "\"hello\""),
                
                // Complex Function tests
                ("complex_function.k", "205"),
                
                // Count operator
                ("count_operator.k", "3"),
                
                // Cut vector
                ("cut_vector.k", "(0 1;2 3;4 5 6 7)"),
                
                // Dictionary tests
                ("dictionary_empty.k", ".()"),
                ("dictionary_index.k", "1"),
                ("dictionary_index_attr.k", ".((`c;3;);(`d;4;))"),
                ("dictionary_index_value.k", "1"),
                ("dictionary_index_value2.k", "2"),
                ("dictionary_multiple.k", ".((`a;1;);(`b;2;))"),
                ("dictionary_null_attributes.k", ".((`a;1;);(`b;2;))"),
                ("dictionary_single.k", ".,(`a;`b;)"),
                ("dictionary_type.k", "5"),
                ("dictionary_with_null_value.k", ".((`a;1;);(`b;;);(`c;3;))"),
                
                // Drop tests
                ("drop_negative.k", "0 1 2 3"),
                ("drop_positive.k", "4 5 6 7"),
                
                // Empty mixed vector
                ("empty_mixed_vector.k", "()"),
                
                // Enlist operator
                ("enlist_operator.k", ",5"),
                
                // Enumerate tests
                ("enumerate_empty_int.k", "!0"),
                ("enumerate_operator.k", "0 1 2 3 4"),
                
                // Equal operator
                ("equal_operator.k", "0"),
                
                // First operator
                ("first_operator.k", "1"),
                
                // Float tests
                ("float_decimal_point.k", "10.0"),
                ("float_exponential.k", "170.0"),
                ("float_exponential_large.k", "1e+015"),
                ("float_exponential_small.k", "1e-020"),
                ("float_types.k", "3.14"),
                
                // Function tests
                ("function_add7.k", "12"),
                ("function_call_anonymous.k", "13"),
                ("function_call_chain.k", "20"),
                ("function_call_double.k", "32"),
                ("function_call_simple.k", "12"),
                ("function_foo_chain.k", "20"),
                ("function_mul.k", "32"),
                
                // Generate operator
                ("generate_operator.k", "0 0 0 0"),
                
                // Grade operators
                ("grade_down_operator.k", "1 2 3 4 0"),
                ("grade_up_operator.k", "0 4 2 3 1"),
                
                // Greater than operator
                ("greater_than_operator.k", "0"),
                
                // Integer types
                ("integer_types_int.k", "42"),
                ("integer_types_long.k", "123456789j"),
                
                // Join operator
                ("join_operator.k", "3 5"),
                
                // Less than operator
                ("less_than_operator.k", "1"),
                
                // Math functions
                ("math_abs.k", "5.0"),
                ("math_exp.k", "7.389056"),
                ("math_log.k", "2.302585"),
                ("math_sin.k", "0.0"),
                ("math_sqrt.k", "4.0"),
                ("math_vector.k", "0.841471 0.9092974 0.14112"),
                
                // Maximum operator
                ("maximum_operator.k", "5"),
                
                // Minimum operator
                ("minimum_operator.k", "3"),
                
                // Mixed list with null
                ("mixed_list_with_null.k", "(1;;`test;42.5)"),
                
                // Mixed vector tests
                ("mixed_vector_empty_position.k", "(1;;2)"),
                ("mixed_vector_multiple_empty.k", "(1;;;3)"),
                ("mixed_vector_whitespace_position.k", "(1;;2)"),
                
                // Mod tests
                ("mod_integer.k", "1"),
                ("mod_rotate.k", "3 4 1 2"),
                ("mod_vector.k", "1 0 1 0"),
                ("modulus_operator.k", "1"),
                
                // Negate operator
                ("negate_operator.k", "1"),
                
                // Nested vector test
                ("nested_vector_test.k", "(1 2 3;4 5 6)"),
                
                // Overflow tests
                ("overflow_int_max_plus1.k", "0N"),
                ("overflow_int_neg_inf.k", "0N"),
                ("overflow_int_neg_inf_minus2.k", "0I"),
                ("overflow_int_null_minus1.k", "0I"),
                ("overflow_int_pos_inf.k", "0N"),
                ("overflow_int_pos_inf_plus2.k", "-0I"),
                ("overflow_long_max_plus1.k", "0Nj"),
                ("overflow_long_min_minus1.k", "0Ij"),
                ("overflow_long_neg_inf.k", "0Nj"),
                ("overflow_long_neg_inf_minus2.k", "0Ij"),
                ("overflow_long_pos_inf.k", "0Nj"),
                ("overflow_long_pos_inf_plus2.k", "-0Ij"),
                ("overflow_regular_int.k", "-2147483639"),
                ("underflow_regular_int.k", "2147483617"),
                
                // Parentheses tests
                ("parentheses_basic.k", "7"),
                ("parentheses_grouping.k", "9"),
                ("parentheses_nested.k", "7"),
                ("parentheses_nested1.k", "21"),
                ("parentheses_nested2.k", "0.7142857"),
                ("parentheses_nested3.k", "24"),
                ("parentheses_nested4.k", "15"),
                ("parentheses_nested5.k", "29"),
                ("parentheses_precedence.k", "7"),
                ("parenthesized_vector.k", "1 2 3 4"),
                
                // Power operator
                ("power_operator.k", "8"),
                
                // Precedence tests
                ("precedence_chain1.k", "1410"),
                ("precedence_chain2.k", "0.07092199"),
                ("precedence_complex1.k", "0.7142857"),
                ("precedence_complex2.k", "10.01667"),
                ("precedence_mixed1.k", "0"),
                ("precedence_mixed2.k", "25"),
                ("precedence_mixed3.k", "11"),
                ("precedence_power1.k", "128"),
                ("precedence_power2.k", "83"),
                ("precedence_spec1.k", "11.57143"),
                ("precedence_spec2.k", "6"),
                
                // Reciprocal operator
                ("reciprocal_operator.k", "0.25"),
                
                // Reverse operator
                ("reverse_operator.k", "3 2 1"),
                
                // Scalar vector tests
                ("scalar_vector_addition.k", "4 5"),
                ("scalar_vector_multiplication.k", "3 6"),
                
                // Shape operator tests
                ("shape_operator.k", ",3"),
                ("shape_operator_empty_vector.k", ",0"),
                ("shape_operator_jagged.k", ",3"),
                ("shape_operator_jagged_3d.k", "2 2"),
                ("shape_operator_jagged_matrix.k", ",3"),
                ("shape_operator_matrix.k", "3 3"),
                ("shape_operator_matrix_2x3.k", "2 3"),
                ("shape_operator_matrix_3x3.k", "3 3"),
                ("shape_operator_scalar.k", "!0"),
                ("shape_operator_tensor_2x2x3.k", "2 2 3"),
                ("shape_operator_tensor_3d.k", "3 2 2"),
                ("shape_operator_vector.k", ",5"),
                
                // Simple arithmetic tests
                ("simple_addition.k", "3"),
                ("divide_float.k", "0.6"),
                ("divide_integer.k", "2.5"),
                ("simple_multiplication.k", "12"),
                ("simple_nested_test.k", "1 2 3"),
                ("minus_integer.k", "2"),
                
                // Special values tests
                ("special_float_neg_inf.k", "-0i"),
                ("special_float_null.k", "0n"),
                ("special_float_pos_inf.k", "0i"),
                ("special_int_neg_inf.k", "-0I"),
                ("special_int_null.k", "0N"),
                ("special_int_pos_inf.k", "0I"),
                ("special_long_neg_inf.k", "-0Ij"),
                ("special_long_null.k", "0Nj"),
                ("special_long_pos_inf.k", "0Ij"),
                ("special_null.k", "_n"),
                
                // Additional special value arithmetic tests (separated from special_values_arithmetic.k)
                ("special_int_pos_inf_plus_1.k", "0N"),
                ("special_int_null_plus_1.k", "-0I"),
                ("special_int_neg_inf_plus_1.k", "-2147483646"),
                ("special_float_null_plus_1.k", "0n"),
                ("special_1_plus_int_pos_inf.k", "0N"),
                ("special_1_plus_int_null.k", "-0I"),
                ("special_int_vector.k", "0I 0N -0I"),
                ("special_float_vector.k", "0i 0n -0i"),
                
                // Square bracket tests
                ("square_bracket_function.k", "2"),
                ("square_bracket_vector_multiple.k", "14 16"),
                ("square_bracket_vector_single.k", "14"),
                
                // String representation tests
                ("string_representation_int.k", "\"42\""),
                ("string_representation_mixed.k", "\"(1;2.5;\\\"a\\\")\""),
                ("string_representation_symbol.k", "\"`symbol\""),
                ("string_representation_vector.k", "\"1 2 3\""),
                
                // Symbol tests
                ("symbol_quoted.k", "`\"a symbol\""),
                ("symbol_simple.k", "`foo"),
                ("symbol_vector_compact.k", "`a`b`c"),
                ("symbol_vector_spaces.k", "`a`b`c"),
                
                // Take operator tests
                ("take_operator_basic.k", "1 2 3"),
                ("take_operator_empty_float.k", "0#0.0"),
                ("take_operator_empty_symbol.k", "0#`"),
                ("take_operator_overflow.k", "1 2 3 1 2 3 1 2 3 1"),
                ("take_operator_scalar.k", "42 42 42"),
                
                // Test division rules
                ("division_float_4_2.0.k", "2.0"),
                ("division_float_5_2.5.k", "2.0"),
                ("division_int_4_2.k", "2"),
                ("division_int_5_2.k", "2.5"),
                ("division_rules_10_3.k", "3.333333"),
                ("division_rules_12_4.k", "3"),
                ("division_rules_4_2.k", "2"),
                ("division_rules_5_2.k", "2.5"),
                
                // Test enumerate
                ("enumerate.k", "0 1"),
                
                // Test grade operators
                ("grade_down_no_parens.k", "1 2 3 4 0"),
                ("grade_up_no_parens.k", "0 4 2 3 1"),
                
                // Test mixed types
                ("mixed_types.k", "(42;3.14;\"hello\";`symbol)"),
                
                // Test multiline function
                ("multiline_function_single.k", "20"),
                
                // Test null vector
                ("null_vector.k", "(;1;2)"),
                
                // Test scoping
                ("scoping_single.k", "60"),
                
                // Test semicolon tests
                ("semicolon_simple.k", "(7;11;-20.45)"),
                ("semicolon_vars.k", "30 200 -10"),
                ("semicolon_vector.k", "(7;3 4;-20.45)"),
                
                // Test simple scalar div
                ("simple_scalar_div.k", "2.5"),
                
                // Test single no semicolon
                ("single_no_semicolon.k", "42"),
                
                // Test smart division
                ("smart_division1.k", "2.5 5.0"),
                ("smart_division2.k", "2 4"),
                ("smart_division3.k", "2 4 6"),
                
                // Test special values
                ("special_0i_plus_1.k", "0N"),
                ("special_0n_plus_1.k", "-0I"),
                ("special_1_plus_neg0i.k", "-2147483646"),
                ("special_neg0i_plus_1.k", "-2147483646"),
                ("special_underflow.k", "2147483622"),
                ("special_underflow_2.k", "2147483549"),
                ("special_underflow_3.k", "2147482649"),
                
                // Test type operators
                ("type_char.k", "3"),
                ("type_float.k", "2"),
                ("type_null.k", "6"),
                ("type_space.k", "3"),
                ("type_symbol.k", "4"),
                ("type_vector.k", "-1"),
                
                // Test vector
                ("vector.k", "1 2 3"),
                
                // Type operator tests
                ("type_operator_char.k", "3"),
                ("type_operator_float.k", "2"),
                ("type_operator_null.k", "6"),
                ("type_operator_symbol.k", "4"),
                ("type_operator_vector_char.k", "-3"),
                ("type_operator_vector_float.k", "-2"),
                ("type_operator_vector_int.k", "-1"),
                ("type_operator_vector_mixed.k", "0"),
                ("type_operator_vector_symbol.k", "-3"),
                
                // Type promotion tests
                ("type_promotion_float_int.k", "3.5"),
                ("type_promotion_float_long.k", "2.5"),
                ("type_promotion_int_float.k", "3.5"),
                ("type_promotion_int_long.k", "3j"),
                ("type_promotion_long_float.k", "2.5"),
                ("type_promotion_long_int.k", "3j"),
                
                // Unary minus operator
                ("unary_minus_operator.k", "-5"),
                
                // Unique operator
                ("unique_operator.k", "1 2 3"),
                
                // Variable tests
                ("amend_item_simple_no_semicolon.k", "1 12 3"),
                ("variable_assignment.k", "7"),
                ("variable_reassignment.k", "7.2 4.5"),
                ("variable_scoping_global_access.k", "150"),
                ("variable_scoping_global_assignment.k", "10"),
                ("variable_scoping_global_unchanged.k", "100"),
                ("variable_scoping_local_hiding.k", "60"),
                ("variable_scoping_nested_functions.k", "25"),
                ("variable_usage.k", "30"),
                ("dot_execute.k", "4"),
                ("dot_execute_context.k", "8"),
                ("dictionary_enumerate.k", "`a`b"),
                
                // New spec features
                ("null_operations.k", "7"),
                ("dictionary_dot_apply.k", "1"),
                
                // $ operator tests - monadic format
                ("monadic_format_basic.k", ",\"1\""),
                ("monadic_format_types.k", "\"42.5\""),
                ("monadic_format_vector.k", "(,\"1\";,\"2\";,\"3\")"),
                ("monadic_format_string_hello.k", "\"hello\""),
                ("monadic_format_string_a.k", ",\"a\""),
                ("monadic_format_symbol_hello.k", "\"hello\""),
                ("monadic_format_symbol_simple.k", "\"test\""),
                ("monadic_format_dictionary.k", "\".((`a;1;);(`b;2;);(`c;3;))\""),
                ("monadic_format_nested_list.k", "((,\"1\";,\"2\";,\"3\");(,\"4\";,\"5\";,\"6\"))"),
                ("monadic_format_integer.k", "\"42\""),
                ("monadic_format_float.k", "\"3.14\""),
                ("monadic_format_vector_simple.k", "(,\"1\";,\"2\";,\"3\")"),
                
                // $ operator tests - binary form/type conversion
                ("format_integer.k", "\"\""),
                ("format_float_numeric.k", ",\"1\""),
                ("form_long.k", "42j"),
                ("format_numeric.k", "\"    1\""),
                ("form_string_pad_left.k", "\"  hello\""),
                ("format_symbol_pad_left.k", "\"     hello\""),
                ("format_symbol_pad_left_8.k", "\"   hello\""),
                ("format_pad_left.k", "\"   42\""),
                ("format_pad_right.k", "\"42   \""),
                ("format_float_width_precision.k", "\"      3.14\""),
                ("format_float_precision.k", "\"    3.14\""),
                
                // Additional format tests
                ("format_0_1.k", "\"\""),
                ("format_1_1.k", ",\"1\""),
                ("format_symbol_string_mixed_vector.k", "`hello`world`test"),
                ("form_integer_charvector.k", "42"),
                ("form_character_charvector.k", "\"aaa\""),
                ("dot_execute_variables.k", "0.6"),
                ("form_braces_expressions.k", "8 15 2"),
                ("form_braces_nested_expr.k", "(12 20;(7;3.333333))"),
                ("form_braces_complex.k", "8.25 12.0 9.0"),
                ("form_braces_string.k", "(\"John\";\"is\";25;\"years old\")"),
                ("form_braces_mixed_type.k", "(42;\"hello\";`test;47;\"helloworld\")"),
                ("form_braces_simple.k", "8"),
                ("form_braces_arith.k", "8 15 2 12 20 5"),
                ("form_braces_nested_arith.k", "(7 10;(2;2.5);5 -1)"),
                ("form_braces_float.k", "4.0 3.75 0.6 -1.0"),
                ("form_braces_mixed_arith.k", "17.5 32.5 4.5"),
                ("form_braces_example.k", "8 9"),
                ("form_braces_function_calls.k", "5 20 12"),
                ("form_braces_nested_function_calls.k", "5 8 25"),
                
                // Test underscore functions
                ("log.k", "2.302585"),
                ("time_t.k", ".((`type;1;);(`shape;!0;))"),
                ("rand_draw_select.k", ".((`type;-1;);(`shape;,10;))"),
                ("rand_draw_deal.k", ".((`type;-1;);(`shape;,4;);(`allitemsunique;1;))"),
                ("rand_draw_probability.k", ".((`type;-2;);(`shape;,10;))"),
                ("rand_draw_vector_select.k", ".((`type;0;);(`shape;,2;))"),
                ("rand_draw_vector_deal.k", ".((`type;0;);(`shape;,2;);(`allitemsunique;1;))"),
                ("rand_draw_vector_probability.k", ".((`type;0;);(`shape;,2;))"),
                ("draw.k", "Error - _draw (random number generation) operation reserved for future use"),
                ("time_gtime.k", "20350101 0"),
                ("time_lt.k", "-18000"),
                ("time_jd.k", "0N"),
                ("time_dj.k", "20350101"),
                ("time_ltime.k", "20341231 190000"),
                ("in.k", "4"),
                
                // List operations tests
                ("list_dv_basic.k", "3 5"),
                ("list_dv_nomatch.k", "3 4 4 5"),
                ("list_di_basic.k", "3 4 5"),
                ("list_di_multiple.k", "3 4"),
                ("list_sv_base10.k", "1995"),
                ("list_sv_base2.k", "9"),
                ("list_sv_mixed.k", "1995"),
                
                // Environment and file system tests
                ("list_getenv.k", "\"C:\\Program Files\\Git\\mingw64\\bin;C:\\Program Files\\Git\\usr\\bin;C:\\Users\\euseb\\bin;C:\\Program Files\\Microsoft MPI\\Bin\\;C:\\Program Files\\Eclipse Adoptium\\jdk-8.0.382.5-hotspot\\bin;C:\\Program Files (x86)\\Common Files\\Intel\\Shared Files\\cpp\\bin\\ia32;C:\\Program Files (x86)\\Common Files\\Intel\\Shared Files\\cpp\\bin\\Intel64;C:\\Program Files (x86)\\Common Files\\Intel\\Shared Libraries\\redist\\ia32_win\\compiler;C:\\Program Files (x86)\\Common Files\\Intel\\Shared Libraries\\redist\\intel64_win\\compiler;C:\\Program Files (x86)\\Common Files\\Intel\\Shared Libraries\\redist\\ia32\\compiler;C:\\Program Files (x86)\\Common Files\\Intel\\Shared Libraries\\redist\\intel64\\compiler;C:\\Program Files (x86)\\NVIDIA Corporation\\PhysX\\Common;C:\\Windows\\system32;C:\\Windows;C:\\Windows\\System32\\Wbem;C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\;C:\\Windows\\System32\\OpenSSH\\;C:\\Program Files (x86)\\HP\\HP Performance Advisor;C:\\Program Files (x86)\\QuickTime\\QTSystem\\;C:\\Program Files (x86)\\Common Files\\Propellerhead Software\\ReWire\\;C:\\Program Files\\Common Files\\Propellerhead Software\\ReWire\\;C:\\Program Files\\Microsoft SQL Server\\130\\Tools\\Binn\\;C:\\Program Files\\Microsoft SQL Server\\Client SDK\\ODBC\\170\\Tools\\Binn\\;C:\\Program Files (x86)\\Common Files\\GRM\\SpaceVRData\\Libs;C:\\Program Files\\Common Files\\GRM\\SpaceVRData\\Libs;C:\\Program Files\\CMake\\bin;C:\\Program Files\\dotnet\\;C:\\Program Files (x86)\\Microsoft Emulator Manager\\1.0\\;C:\\Program Files (x86)\\Microsoft ASP.NET\\ASP.NET Web Pages\\v1.0\\;C:\\Program Files\\Microsoft SQL Server\\110\\Tools\\Binn\\;C:\\Users\\euseb\\.dnx\\bin;C:\\Program Files\\Microsoft DNX\\Dnvm\\;C:\\Program Files\\Microsoft SQL Server\\120\\Tools\\Binn\\;C:\\Program Files (x86)\\nodejs\\;C:\\Program Files\\Common Files\\Avid\\Eucon;C:\\Program Files (x86)\\Common Files\\Avid\\Eucon;C:\\Program Files\\Avid\\Avid Link\\jre\\bin;C:\\Program Files (x86)\\dotnet\\;c:\\Windows\\SysWOW64\\;c:\\Windows\\Sysnative\\;c:\\program files (x86)\\iracing\\d3dgear;C:\\Program Files (x86)\\Common Files\\DivX Shared\\DesktopService;C:\\Program Files\\Microsoft SQL Server\\150\\Tools\\Binn\\;C:\\Program Files\\gs\\gs10.00.0\\bin;C:\\Program Files (x86)\\Windows Kits\\10\\Microsoft Application Virtualization\\Sequencer\\;C:\\Program Files\\Calibre2\\;C:\\Program Files\\NVIDIA Corporation\\Nsight Compute 2020.1.1\\;C:\\Program Files\\NVIDIA Corporation\\NVIDIA app\\NvDLISR;C:\\Program Files\\Git LFS;C:\\Program Files\\GitHub CLI\\;C:\\Program Files (x86)\\Incredibuild;C:\\Program Files (x86)\\Windows Kits\\10\\Windows Performance Toolkit\\;C:\\Program Files\\Acustica\\Medusa Framework\\;C:\\Program Files\\Git\\cmd;C:\\Users\\euseb\\.local\\bin;C:\\Progra~1\\NVIDIA~2\\CUDA\\v11.0\\libnvvp;C:\\Progra~1\\NVIDIA~2\\CUDA\\v11.0\\bin;C:\\Progra~2\\Common~1\\Intel\\Shared~1\\redist\\intel6~1\\compiler;C:\\Users\\euseb\\AppData\\Local\\Microsoft\\WindowsApps;C:\\Program Files\\Imaginando\\DLYM;C:\\Users\\euseb\\.dotnet\\tools;C:\\Users\\euseb\\AppData\\Roaming\\npm;C:\\Users\\euseb\\AppData\\Local\\Programs\\Microsoft VS Code\\bin;C:\\Program Files\\Imaginando\\K7D;C:\\Program Files\\Imaginando\\FRMS;C:\\Program Files\\Imaginando\\DRC;C:\\Users\\euseb\\AppData\\Local\\Programs\\Ollama;C:\\Users\\euseb\\.local\\bin;C:\\Users\\euseb\\.dotnet\\tools\""), // PATH environment variable
                ("list_setenv.k", "`TESTVAR"), // _setenv returns the variable name (not the value) - this is the current behavior
                ("list_size.k", "Error - _size: error accessing file 'README.md': _size: file 'README.md' not found"), // File doesn't exist in test directory
                ("list_size_existing.k", "872.0"), // Test with existing project file using absolute path
                ("test_ci_basic.k", "\"A\""),
                ("test_ci_vector.k", "\"ABC\""),
                ("test_ic_basic.k", "65"),
                ("test_vs_dyadic.k", "1 9 9 5"),
                ("test_ic_vector.k", "65 66 67"),
                ("test_monadic_colon.k", "42"),
                ("test_sm_basic.k", "1"),
                ("test_sm_simple.k", "1"),
                ("test_ss_basic.k", "7"),
                ("test_semicolon.k", "1 2 3 4"),
                
                // New search function tests
                ("search_in_basic.k", "4"),
                ("search_in_notfound.k", "0"),
                ("search_bin_basic.k", "1"),
                ("search_binl_eachleft.k", "0 1 1 2 2"),
                ("search_lin_intersection.k", "1 1 1 0 0"),
                
                // Amend Item tests - only valid cases with 3+ arguments
                ("amend_item_basic.k", "1 12 3"),
                ("amend_item_multiple.k", "11 2 13"),
                ("amend_item_monadic.k", "1 4 3"),
                
                // Existing amend tests - only valid cases with 3+ arguments
                ("amend_test.k", "1 12 3 4 5"),
                ("amend_simple.k", "1 12 3 4 5"),
                
                // Find operator tests
                ("find_basic.k", "2"),
                ("find_notfound.k", "7"),
                
                ("bin.k", "Error - _bin (binary search) operation reserved for future use"),
                ("lsq.k", "Error - _lsq (least squares) operation reserved for future use"),
                
                // Form specifiers on mixed vectors
                ("format_float_precision_vector_simple.k", "(\"       1.5\";\"       2.5\")"),
                ("format_float_precision_mixed_vector.k", "(\"   1.50\";\"   2.70\";\"   3.14\";\"   4.20\")"),
                ("format_pad_mixed_vector.k", "(\"         1\";\"         2\";\"         3\")"),
                ("format_pad_negative_mixed_vector.k", "(\"1         \";\"2         \";\"3         \")"),
                
// Vector notation tests
("vector_notation_empty.k", "()"),
("vector_notation_functions.k", "10 20 30"),
("vector_notation_mixed_types.k", "(42;3.14;\"hello\";`symbol)"),
("vector_notation_nested.k", "3 7 11"),
("vector_notation_semicolon.k", "(7;11;-20.45)"),
("vector_notation_single_group.k", "42"),
("vector_notation_space.k", "1 2 3 4 5"),
("vector_notation_variables.k", "30 200 -10"),
                ("vector_notation_semicolon.k", "(7;11;-20.45)"),
                ("vector_notation_single_group.k", "42"),
                ("vector_notation_space.k", "1 2 3 4 5"),
                ("vector_notation_variables.k", "30 200 -10"),
                
                // Vector operations
                ("vector_addition.k", "4 6"),
                ("vector_division.k", "0.3333333 0.5"),
                ("vector_index_duplicate.k", "5 5"),
                ("vector_index_first.k", "5"),
                ("vector_index_multiple.k", "8 9"),
                ("vector_index_reverse.k", "9 8"),
                ("vector_index_single.k", "4"),
                ("vector_multiplication.k", "3 8"),
                ("vector_subtraction.k", "-2 -2"),
                ("vector_with_null.k", "(;1;2)"),
                ("vector_with_null_middle.k", "(1;;3)"),
                
                // Where operator
                ("where_operator.k", "0 2 3"),
                ("where_vector_counts.k", "0 0 0 1 1 2"),
                
                // Floor operator
                ("floor_operator.k", "3.0"),
                
                // Missing adverb tests
                ("adverb_backslash_colon_basic.k", "(5 6 7;6 7 8;7 8 9)"),
                ("adverb_slash_colon_basic.k", "(5 6 7;6 7 8;7 8 9)"),
                ("adverb_tick_colon_basic.k", "4 1 3 8"),
                
                // Missing amend tests
                ("amend_apply.k", "(1 2 13 4 5\n6 7 8 9 10)"),
                ("amend_dot_test.k", "1 12 3 4 5"),
                ("amend_item_simple.k", "@[1 2 3]"),
                ("amend_item_single.k", "@[1 2 3]"),
                ("amend_minimal.k", "@[1 2 3]"),
                ("amend_parenthesized.k", "@[1 2 3]"),
                ("amend_test_anonymous_func.k", "1 12 3 4 5"),
                
                // More missing tests
                ("amend_test_func_var.k", "11 2 3"),
                ("conditional_bracket_test.k", ""),
                ("conditional_false.k", ""),
                ("conditional_simple_test.k", ""),
                ("conditional_true.k", ""),
                ("dictionary_null_index.k", "1 2"),
                ("dictionary_unmake.k", "((`a;1;);(`b;2;))"),
                ("do_bracket_test.k", ""),
                ("do_loop.k", ""),
                ("do_simple.k", ""),
                
                // Dyadic bracket tests
                ("dyadic_divide_bracket.k", "5"),
                ("dyadic_minus_bracket.k", "7"),
                ("dyadic_multiply_bracket.k", "24"),
                ("dyadic_plus_bracket.k", "8"),
                
                // Empty brackets tests
                ("empty_brackets_dictionary.k", "1 2 3"),
                ("empty_brackets_vector.k", "1 2 3"),
                
                // Format tests
                ("form_braces_complex_expressions.k", "14 20 10"),
                ("format_float_precision_complex_mixed.k", "(\"   1.50\";\"   2.70\";\"   3.14\";\"   4.20\")"),
                ("format_float_vector.k", "(\"       1.5\";\"       2.5\")"),
                ("format_int_vector.k", "(\"         1\";\"         2\";\"         3\")"),
                
                // Final missing tests
                ("format_long_vector.k", "\"\""),
                ("format_string_pad_left.k", "\"     hello\""),
                ("format_string_pad_right.k", "\"test      \""),
                ("format_vector_int.k", "(\"1\";\"2\";\"3\")"),
                ("group_operator.k", "(0 1 6;2 7 16;3 5 12 15 17;,4;8 9;,10;,11;,13;14 18;,19)"),
                ("if_bracket_test.k", ""),
                ("if_simple_test.k", ""),
                ("if_true.k", ""),
                ("in_basic.k", "4"),
                ("in_notfound.k", "0"),
                ("monadic_format_basic.k", ",\"1\""),
                ("monadic_format_types.k", "\"42.5\""),
                ("monadic_format_vector.k", "(,\"1\";,\"2\";,\"3\")"),
                ("monadic_format_string_hello.k", "\"hello\""),
                ("monadic_format_string_a.k", ",\"a\""),
                ("monadic_format_symbol_hello.k", "\"hello\""),
                ("monadic_format_symbol_simple.k", "\"test\""),
                ("monadic_format_dictionary.k", ".((`a;1;);(`b;2;);(`c;3;))"),
                ("monadic_format_nested_list.k", "((,\"1\";,\"2\";,\"3\");(,\"4\";,\"5\";,\"6\"))"),
                ("monadic_format_integer.k", "\"42\""),
                ("monadic_format_float.k", "\"3.14\""),
                ("monadic_format_vector_simple.k", "(,\"1\";,\"2\";,\"3\")"),
                
                // Final remaining tests
                ("in_simple.k", "0"),
                ("isolated.k", "0.6"),
                ("modulo.k", "0.6"),
                ("monadic_format_mixed_vector.k", "(,\"1\";\"2.5\";\"hello\";\"symbol\")"),
                ("over_plus_empty.k", "0"),
                ("simple_division.k", "4"),
                ("simple_subtraction.k", "2"),
                ("string_parse.k", "30"),
                ("take_operator_empty_float.k", "0#0.0"),
                ("take_operator_empty_symbol.k", "0#`"),
                ("take_operator_overflow.k", "1 2 3 1 2 3 1 2 3 1"),
                
                // K Tree tests - Following One Test Per File principle
                ("k_tree_assignment_absolute_foo.k", "42"), // Absolute path assignment to foo
                ("k_tree_retrieve_absolute_foo.k", "42"),  // Absolute path retrieval from foo
                ("k_tree_retrieval_relative.k", "42"),         // Relative path retrieval only
                ("k_tree_enumerate.k", "`k`t"),     // Root enumeration - compact symbol vector format
                ("k_tree_current_branch.k", "`.k"),           // Current branch command - returns K tree branch name
                ("k_tree_dictionary_indexing.k", "42"),       // Dictionary indexing
                ("k_tree_nested_indexing.k", "2"),          // Nested indexing
                ("k_tree_verify_root.k", ""),               // Root verification - null displays as empty string
                ("k_tree_flip_dictionary.k", ".((`a;1;);(`b;2;);(`c;3;))"), // Test flip + make dictionary - matches k.exe
                
                // Proper single-test files following BEST.md principles
                ("k_tree_null_to_dict_conversion.k", ".,(`foo;42;)"), // Test .k converts from null to dict (single-item dict list)
                ("k_tree_dictionary_assignment.k", ".((`a;1;);(`b;2;);(`c;3;))"), // Test dictionary assignment in K tree (triplets format)
                ("k_tree_test_bracket_indexing.k", "2"),   // Test bracket indexing with regular dictionary
                ("k_tree_flip_test.k", "((`a;1);(`b;2);(`c;3))"), // Test flip operation - matches k.exe
                
                // Final remaining tests
                ("vector_null_index.k", "1 2 3 4"),
                ("while_bracket_test.k", ""),
                ("while_safe_test.k", ""),
                ("while_simple_test.k", ""),
                
                // K Serialization tests (based on actual implementation output)
                ("serialization_bd_db_integer.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000*\\000\\000\\000\""),
                ("serialization_bd_db_float.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000n\\206\\033\\360\\371!\\t@\""),
                ("serialization_bd_db_character.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000a\\000\\000\\000\""),
                ("serialization_bd_db_symbol.k", "\"\\001\\000\\000\\000\\013\\000\\000\\000\\004\\000\\000\\000symbol\\000\""),
                ("serialization_bd_db_null.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_db_integervector.k", "\"\\001\\000\\000\\000\\024\\000\\000\\000\\377\\377\\377\\377\\003\\000\\000\\000\\001\\000\\000\\000\\002\\000\\000\\000\\003\\000\\000\\000\""),
                ("serialization_bd_db_floatvector.k", "\"\\001\\000\\000\\000 \\000\\000\\000\\376\\377\\377\\377\\003\\000\\000\\000\\232\\231\\231\\231\\231\\231\\361?\\232\\231\\231\\231\\231\\231\\001@ffffff\\n@\""),
                ("serialization_bd_db_charactervector.k", "\"\\001\\000\\000\\000\\016\\000\\000\\000\\375\\377\\377\\377\\005\\000\\000\\000hello\\000\""),
                ("serialization_bd_db_symbolvector.k", "\"\\001\\000\\000\\000\\016\\000\\000\\000\\374\\377\\377\\377\\003\\000\\000\\000a\\000b\\000c\\000\""),
                ("serialization_bd_db_list.k", "\"\\001\\000\\000\\000(\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\001\\000\\000\\000\\001\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\004@\\003\\000\\000\\000a\\000\\000\\000\""),
                ("serialization_bd_db_dictionary.k", "\"\\001\\000\\000\\000H\\000\\000\\000\\005\\000\\000\\000\\002\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\004\\000\\000\\000a\\000\\000\\000\\004\\000\\000\\0001\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\004\\000\\000\\000b\\000\\000\\000\\004\\000\\000\\0002\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_db_anonymousfunction.k", "\"\\001\\000\\000\\000\\017\\000\\000\\000\\n\\000\\000\\000\\000{[x] x+1}\\000\""),
                ("serialization_bd_db_roundtrip_integer.k", "42"),
                
                // Comprehensive _bd serialization tests - Edge cases and random examples
                ("serialization_bd_null_edge_0.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_integer_edge_0.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_integer_edge_1.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\001\\000\\000\\000\""),
                ("serialization_bd_integer_edge_-1.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\377\\377\\377\\377\""),
                ("serialization_bd_integer_edge_2147483647.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\377\\377\\377\\177\""),
                ("serialization_bd_integer_edge_-2147483648.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\001\\000\\000\\200\""),
                ("serialization_bd_integer_edge_0N.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\200\""),
                ("serialization_bd_integer_edge_0I.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\377\\377\\377\\177\""),
                ("serialization_bd_integer_edge_-0I.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\001\\000\\000\\000\\001\\000\\000\\200\""),
                ("serialization_bd_float_edge_0.0.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_float_edge_1.0.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\360?\""),
                ("serialization_bd_float_edge_-1.0.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\360\\277\""),
                ("serialization_bd_float_edge_0.5.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\340?\""),
                ("serialization_bd_float_edge_-0.5.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\340\\277\""),
                ("serialization_bd_float_edge_0n.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\370\\377\""),
                ("serialization_bd_float_edge_0i.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\360\\177\""),
                ("serialization_bd_float_edge_-0i.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\000\\360\\377\""),
                ("serialization_bd_symbol_edge_a.k", "\"\\001\\000\\000\\000\\006\\000\\000\\000\\004\\000\\000\\000a\\000\""),
                ("serialization_bd_symbol_edge_symbol.k", "\"\\001\\000\\000\\000\\013\\000\\000\\000\\004\\000\\000\\000symbol\\000\""),
                ("serialization_bd_symbol_edge_test123.k", "\"\\001\\000\\000\\000\\014\\000\\000\\000\\004\\000\\000\\000test123\\000\""),
                ("serialization_bd_symbol_edge_underscore.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\004\\000\\000\\000_underscore\\000\""),
                ("serialization_bd_symbol_edge_hello.k", "\"\\001\\000\\000\\000\\n\\000\\000\\000\\004\\000\\000\\000hello\\000\""),
                ("serialization_bd_symbol_edge_newline_tab.k", "\"\\001\\000\\000\\000\\007\\000\\000\\000\\004\\000\\000\\000\\n\\t\\000\""),
                ("serialization_bd_symbol_edge_001.k", "\"\\001\\000\\000\\000\\006\\000\\000\\000\\004\\000\\000\\000\\001\\000\""),
                ("serialization_bd_symbol_edge_empty.k", "\"\\001\\000\\000\\000\\005\\000\\000\\000\\004\\000\\000\\000\\000\""),
                ("serialization_bd_character_edge_a.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000a\\000\\000\\000\""),
                ("serialization_bd_character_edge_b.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000b\\000\\000\\000\""),
                ("serialization_bd_character_edge_z.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000z\\000\\000\\000\""),
                ("serialization_bd_character_edge_A_upper.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000A\\000\\000\\000\""),
                ("serialization_bd_character_edge_Z_upper.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000Z\\000\\000\\000\""),
                ("serialization_bd_character_edge_0.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\0000\\000\\000\\000\""),
                ("serialization_bd_character_edge_9.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\0009\\000\\000\\000\""),
                ("serialization_bd_character_edge_space.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000 \\000\\000\\000\""),
                ("serialization_bd_character_edge_newline.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\n\\000\\000\\000\""),
                ("serialization_bd_character_edge_tab.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\t\\000\\000\\000\""),
                ("serialization_bd_character_edge_carriage.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\r\\000\\000\\000\""),
                ("serialization_bd_character_edge_null.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_character_edge_001.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\001\\000\\000\\000\""),
                ("serialization_bd_character_edge_377.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\377\\000\\000\\000\""),
                ("serialization_bd_character_edge_backspace.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000\\b\\000\\000\\000\""),
                ("serialization_bd_charactervector_edge_empty.k", "\"\\001\\000\\000\\000\\t\\000\\000\\000\\375\\377\\377\\377\\000\\000\\000\\000\\000\""),
                ("serialization_bd_charactervector_edge_a.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\003\\000\\000\\000a\\000\\000\\000\""),
                ("serialization_bd_charactervector_edge_hello.k", "\"\\001\\000\\000\\000\\016\\000\\000\\000\\375\\377\\377\\377\\005\\000\\000\\000hello\\000\""),
                ("serialization_bd_charactervector_edge_whitespace.k", "\"\\001\\000\\000\\000\\014\\000\\000\\000\\375\\377\\377\\377\\003\\000\\000\\000\\n\\t\\r\\000\""),
                ("serialization_bd_integervector_edge_empty.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\377\\377\\377\\377\\000\\000\\000\\000\""),
                ("serialization_bd_integervector_edge_single.k", "\"\\001\\000\\000\\000\\014\\000\\000\\000\\377\\377\\377\\377\\001\\000\\000\\000\\001\\000\\000\\000\""),
                ("serialization_bd_integervector_edge_123.k", "\"\\001\\000\\000\\000\\024\\000\\000\\000\\377\\377\\377\\377\\003\\000\\000\\000\\001\\000\\000\\000\\002\\000\\000\\000\\003\\000\\000\\000\""),
                ("serialization_bd_integervector_edge_special.k", "\"\\001\\000\\000\\000\\024\\000\\000\\000\\377\\377\\377\\377\\003\\000\\000\\000\\000\\000\\000\\200\\377\\377\\377\\177\\001\\000\\000\\200\""),
                ("serialization_bd_list_edge_empty.k", "\"\\001\\000\\000\\000\\b\\000\\000\\000\\000\\000\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_list_edge_null.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\000\\000\\000\\000\\001\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_list_edge_mixed.k", "\"\\001\\000\\000\\000(\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\001\\000\\000\\000\\001\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\000\\004@\\003\\000\\000\\000a\\000\\000\\000\""),
                ("serialization_bd_list_edge_complex.k", "\"\\001\\000\\000\\0000\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\\000\\004\\000\\000\\000symbol\\000\\000\\000\\001\\000\\000\\000\\n\\000\\000\\000\\000{[]}\\000\\000\\000\\001\\000\\000\\000\""),
                ("serialization_bd_list_edge_nested.k", "\"\\001\\000\\000\\000(\\000\\000\\000\\000\\000\\000\\000\\002\\000\\000\\000\\377\\377\\377\\377\\002\\000\\000\\000\\001\\000\\000\\000\\002\\000\\000\\000\\377\\377\\377\\377\\002\\000\\000\\000\\003\\000\\000\\000\\004\\000\\000\\000\""),
                ("serialization_bd_list_edge_dicts.k", "\"\\001\\000\\000\\000X\\000\\000\\000\\000\\000\\000\\000\\002\\000\\000\\000\\005\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\004\\000\\000\\000a\\000.\\002\\001\\000\\000\\000\\001\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\\000\\005\\000\\000\\000\\001\\000\\000\\000\\000\\000\\000\\000\\003\\000\\000\\000\\004\\000\\000\\000b\\000.\\002\\001\\000\\000\\002\\000\\000\\000\\006\\000\\000\\000\\000\\000\\000\""),
                ("serialization_bd_anonymousfunction_random_1.k", "\"\\001\\000\\000\\000\\030\\000\\000\\000\\n\\000\\000\\000\\000{[x] x+7;x$1;x<=2}\\000\""),
                ("serialization_bd_anonymousfunction_random_2.k", "\"\\001\\000\\000\\000\\022\\000\\000\\000\\n\\000\\000\\000\\000{[] 0|4;0&3}\\000\""),
                ("serialization_bd_anonymousfunction_random_3.k", "\"\\001\\000\\000\\000\\024\\000\\000\\000\\n\\000\\000\\000.k\\000{[xyz] xy|3}\\000\""),
                ("serialization_bd_floatvector_random_1.k", "\"\\001\\000\\000\\000(\\000\\000\\000\\376\\377\\377\\377\\004\\000\\000\\000sj\\325/\\312\\006\\bAl\\315\\227\\234\\217\\353\\023\\301\\026\\270Yp\\027/\\n\\301Z`\\257\\331\\350\\303\\306@\""),
                ("serialization_bd_floatvector_random_2.k", "\"\\001\\000\\000\\000\\020\\000\\000\\000\\002\\000\\000\\000\\001\\000\\000\\000:\\321\\254\\250b*\\002A\""),
                ("serialization_bd_floatvector_random_3.k", "\"\\001\\000\\000\\000(\\000\\000\\000\\376\\377\\377\\377\\004\\000\\000\\0002\\377\\004(g\\334!A\\253p.\\342\\211b!\\301\\317\\3376\\002\\n*\\342@zIO\\0230;\\030A\""),
                ("serialization_bd_symbolvector_random_1.k", "\"\\001\\000\\000\\000*\\000\\000\\000\\374\\377\\377\\377\\006\\000\\000\\000qzUM7\\000g8X6P\\000\\303\\256\\036\\302\\255\\302\\245\\000KgNQ5i\\000<\\013+\\000b5\\000\""),
                ("serialization_bd_symbolvector_random_2.k", "\"\\001\\000\\000\\0008\\000\\000\\000\\374\\377\\377\\377\\n\\000\\000\\000\\303\\224\\0070\\000D\\000qCBI1b\\000*H \\000\\303\\202\\302\\211\\302\\251\\000ULsyI\\000F~\\302\\224\\000C\\000Mont\\000O25B\\000\""),
                ("serialization_bd_symbolvector_random_3.k", "\"\\001\\000\\000\\000'\\000\\000\\000\\374\\377\\377\\377\\006\\000\\000\\000\\303\\265\\302\\263\\000EE5ijP\\000trD0LuE\\000\\303\\223W\\000\\302\\270\\302\\202\\000y\\000\""),
                ("serialization_bd_db_anonymousfunction.k", "\"\\001\\000\\000\\000\\017\\000\\000\\000\\n\\000\\000\\000\\000{[x] x+1}\\000\""),
                ("serialization_bd_db_roundtrip_integer.k", "42"),
            };

            var testResults = new List<TestResult>();
            var testScriptsPath = Path.Combine(Directory.GetCurrentDirectory(), "TestScripts");

            // Validate test count vs actual test files
            var actualTestFiles = Directory.GetFiles(testScriptsPath, "*.k", SearchOption.AllDirectories)
                .Select(Path.GetFileName)
                .OrderBy(f => f)
                .ToList();
            
            var expectedTestFiles = tests.Select(t => t.Item1).ToList();
            var missingFromRunner = actualTestFiles.Except(expectedTestFiles).ToList();
            var extraInRunner = expectedTestFiles.Except(actualTestFiles).ToList();

            Console.WriteLine($"Test File Validation:");
            Console.WriteLine($"  Expected tests: {tests.Length}");
            Console.WriteLine($"  Actual .k files: {actualTestFiles.Count}");
            
            if (missingFromRunner.Any())
            {
                Console.WriteLine($"  ❌ MISSING FROM RUNNER ({missingFromRunner.Count}):");
                foreach (var missing in missingFromRunner.Take(10))
                {
                    Console.WriteLine($"    - {missing}");
                }
                if (missingFromRunner.Count > 10)
                {
                    Console.WriteLine($"    ... and {missingFromRunner.Count - 10} more");
                }
                Console.WriteLine();
                Console.WriteLine("ERROR: Test files exist but are not included in test runner!");
                Console.WriteLine("Please add missing test cases to the test runner or remove duplicate files.");
                return;
            }
            
            if (extraInRunner.Any())
            {
                Console.WriteLine($"  ⚠️  EXTRA IN RUNNER ({extraInRunner.Count}):");
                foreach (var extra in extraInRunner)
                {
                    Console.WriteLine($"    - {extra}");
                }
                Console.WriteLine();
            }

            if (missingFromRunner.Count == 0 && extraInRunner.Count == 0)
            {
                Console.WriteLine($"  ✅ Test counts match perfectly!");
            }
            Console.WriteLine("=========================");

            Console.WriteLine($"Running K3CSharp Tests...");
            Console.WriteLine($"Total tests: {tests.Length}");
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
                    var lines = script.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                    
                    var evaluator = new Evaluator();
                    
                    // Reset K tree before each test to ensure isolation
                    evaluator.ResetKTree();
                    
                    K3Value? lastResult = null;
                    
                    // Process each line in the script
                    foreach (var line in lines)
                    {
                        var trimmedLine = line.Trim();
                        if (string.IsNullOrEmpty(trimmedLine)) continue;
                        
                        // Handle REPL commands (starting with \)
                        if (trimmedLine.StartsWith("\\"))
                        {
                            // Handle REPL command directly
                            var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            
                            switch (parts[0])
                            {
                                case "\\r":
                                    // Handle random seed get/set
                                    if (parts.Length == 1)
                                    {
                                        // Display current random seed (no output for test)
                                    }
                                    else if (parts.Length == 2)
                                    {
                                        // Set random seed
                                        if (int.TryParse(parts[1], out int newSeed))
                                        {
                                            Evaluator.RandomSeed = newSeed;
                                        }
                                    }
                                    break;
                                default:
                                    // Ignore other REPL commands for now
                                    break;
                            }
                        }
                        else
                        {
                            // Handle regular K expressions
                            var lexer = new Lexer(trimmedLine);
                            var tokens = lexer.Tokenize();
                            var parser = new Parser(tokens);
                            var ast = parser.Parse();
                            
                            lastResult = evaluator.Evaluate(ast);
                        }
                    }
                    
                    var actualOutput = (lastResult ?? new NullValue()).ToString().Trim();
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
            Console.WriteLine($"Test Results: {passedCount}/{totalCount} passed ({(passedCount * 100.0 / totalCount):F1}%)");
            
            WriteResultsTable(testResults);
        }
    }
}
