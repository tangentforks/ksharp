# K3CSharp Parser Failures

**Generated:** 2026-03-28 01:56:17
**Test Results:** 820/852 passed (96.2%)

## Executive Summary

**Total Tests:** 852
**Passed Tests:** 820
**Failed Tests:** 32
**Success Rate:** 96.2%

**LRS Parser Statistics:**
- NULL Results: 44
- Incomplete Token Consumption: 944
- Total Fallbacks to Legacy: 988
- Incorrect Results: 0
- LRS Success Rate: -16.0%

**Top Failure Patterns:**
- Incomplete consumption (position 3/4): 264
- Incomplete consumption (position 2/3): 123
- Incomplete consumption (position 7/8): 77
- Incomplete consumption (position 5/6): 73
- Incomplete consumption (position 4/5): 60
- Incomplete consumption (position 6/7): 54
- Incomplete consumption (position 1/2): 39
- Incomplete consumption (position 9/10): 36
- Incomplete consumption (position 10/11): 22
- Incomplete consumption (position 8/9): 22

## LRS Parser Failures

### NULL Results (LRS returned NULL)

1. **complex_function.k**:
```k
distance:{[d0;v;a;t] d1:v*t; d2:a*t*t%2; d0+d1+d2}
```
After RIGHT_BRACE (position 34/35)
-------------------------------------------------
2. **lambda_string_assign.k**:
```k
{a:"hello";a}[]
```
After RIGHT_BRACKET (position 9/10)
-------------------------------------------------
3. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
4. **multiline_function_single.k**:
```k
test: {[x] a:x*2; a+10}
```
After RIGHT_BRACE (position 16/17)
-------------------------------------------------
5. **scoping_single.k**:
```k
test2: {[x] globalVar: x * 2; globalVar + 10}
```
After RIGHT_BRACE (position 16/17)
-------------------------------------------------
6. **variable_scoping_global_assignment.k**:
```k
test5: {[x]
inner: {[y] globalVar :: x + y}
inner . x * 2
globalVar}
```
After RIGHT_BRACE (position 28/29)
-------------------------------------------------
7. **variable_scoping_global_unchanged.k**:
```k
test2: {[x]
globalVar: x * 2
globalVar + 10
}
```
After RIGHT_BRACE (position 18/19)
-------------------------------------------------
8. **variable_scoping_local_hiding.k**:
```k
test2: {[x]
globalVar: x * 2
globalVar + 10}
```
After RIGHT_BRACE (position 17/18)
-------------------------------------------------
9. **variable_scoping_nested_functions.k**:
```k
outer: {[x]
inner: {[y] globalVar + x + y}
inner . x}
```
After RIGHT_BRACE (position 24/25)
-------------------------------------------------
10. **adverb_each_count.k**:
```k
#:' (1 2 3;1 2 3 4 5; 1 2)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
11. **time_gtime.k**:
```k
_gtime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
12. **time_ltime.k**:
```k
_ltime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
13. **assignment_lrs_return_value.k**:
```k
b:2*a:47;(a;b)
```
After INTEGER (position 7/14)
-------------------------------------------------
14. **list_getenv.k**:
```k
_getenv "PROMPT"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
15. **list_size_existing.k**:
```k
_size "C:\\Windows\\System32\\write.exe"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
16. **statement_assignment_inline.k**:
```k
1 + a: 42
```
After INTEGER (position 5/6)
-------------------------------------------------
17. **k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
18. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
19. **k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
20. **k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
21. **k_tree_nested_indexing.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
22. **k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
23. **k_tree_dictionary_assignment.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
24. **math_ceil_basic.k**:
```k
_ceil 4.7
```
After FLOAT (position 2/3)
-------------------------------------------------
25. **math_ceil_integer.k**:
```k
_ceil 5
```
After INTEGER (position 2/3)
-------------------------------------------------
26. **math_ceil_negative.k**:
```k
_ceil -3.2
```
After FLOAT (position 2/3)
-------------------------------------------------
27. **math_ceil_vector.k**:
```k
_ceil 1.2 2.7 3.5
```
After FLOAT (position 4/5)
-------------------------------------------------
28. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
29. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
30. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
31. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
32. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
33. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
34. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
35. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
36. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
37. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
38. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
39. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
40. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
41. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
42. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
43. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
44. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
### Incomplete Token Consumption (LRS returned result but didn't consume all tokens)

1. **adverb_each_vector_minus.k**:
```k
(10 20 30) -' (1 2 3)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
2. **adverb_each_vector_multiply.k**:
```k
(1 2 3) *' (4 5 6)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
3. **adverb_each_vector_plus.k**:
```k
(1 2 3 4) +' (5 6 7 8)
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
4. **adverb_over_divide.k**:
```k
%/ 100 2 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
5. **adverb_over_max.k**:
```k
|/ 1 3 2 5 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
6. **adverb_over_min.k**:
```k
&/ 5 3 4 1 2
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
7. **adverb_over_minus.k**:
```k
-/ 10 2 3 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
8. **adverb_over_multiply.k**:
```k
*/ 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
9. **adverb_over_plus.k**:
```k
+/ 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
10. **plus_over_empty.k**:
```k
+/!0
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
11. **multiply_over_empty.k**:
```k
*/!0
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
12. **adverb_over_power.k**:
```k
^/ 2 3 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
13. **adverb_over_with_initialization_1.k**:
```k
1 +/ 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
14. **adverb_over_with_initialization_2.k**:
```k
2 +/ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
15. **adverb_scan_divide.k**:
```k
%\ 100 2 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
16. **adverb_scan_max.k**:
```k
|\ 1 3 2 5 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
17. **adverb_scan_min.k**:
```k
&\ 5 3 4 1 2
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
18. **adverb_scan_minus.k**:
```k
-\ 10 2 3 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
19. **adverb_scan_multiply.k**:
```k
*\ 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
20. **adverb_scan_plus.k**:
```k
+\ 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
21. **adverb_scan_power.k**:
```k
^\ 2 3 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
22. **adverb_scan_with_initialization_1.k**:
```k
1 +\ 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
23. **adverb_scan_with_initialization_2.k**:
```k
2 +\ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
24. **adverb_scan_with_initialization_divide.k**:
```k
2 %\ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
25. **adverb_scan_with_initialization_minus.k**:
```k
2 -\ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
26. **adverb_scan_with_initialization.k**:
```k
2 *\ 1 2 3
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
27. **anonymous_function_double_param.k**:
```k
{[op1;op2] op1 * op2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
28. **anonymous_function_empty.k**:
```k
{}
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
29. **anonymous_function_over_adverb.k**:
```k
{x*%y}/10 20 30
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
30. **anonymous_function_scan_adverb.k**:
```k
{x*%y}\10 20 30
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
31. **test_projected_function.k**:
```k
%
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
32. **atom_scalar.k**:
```k
@42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
33. **atom_vector.k**:
```k
@1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
34. **lrs_atomic_parser_basic.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
35. **lrs_adverb_parser_each.k**:
```k
(1 2 3) %\: 2
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
36. **lrs_adverb_parser_basic.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
37. **lrs_expression_processor_test.k**:
```k
1 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
38. **lrs_parser_validation.k**:
```k
1 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
39. **attribute_handle_symbol.k**:
```k
~`a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
40. **attribute_handle_vector.k**:
```k
~(`a`b`c)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
41. **character_vector.k**:
```k
"hello"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
42. **complex_function.k**:
```k
(distance) . (5 10 2 10)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
43. **count_operator.k**:
```k
# (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
44. **cut_vector.k**:
```k
(0 2 4) _ (0 1 2 3 4 5 6 7)
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
45. **dictionary_empty.k**:
```k
.()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
46. **dictionary_index.k**:
```k
(.((`a;1);(`b;2))) @ `a
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
47. **dictionary_index_attr.k**:
```k
(.((`a;1);(`b;2;.((`c;3);(`d;4))))) @ `b.
```
Incomplete consumption (position 33/34) (consumed 33/34)
-------------------------------------------------
48. **dictionary_index_value.k**:
```k
(.((`a;1);(`b;2))) @ `a
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
49. **dictionary_index_value2.k**:
```k
(.((`a;1);(`b;2))) @ `b
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
50. **dictionary_make_symbol_vector.k**:
```k
.,`a`b
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
51. **dictionary_multiple.k**:
```k
.((`a;1);(`b;2))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
52. **dictionary_null_attributes.k**:
```k
.((`a;1);(`b;2))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
53. **dictionary_single.k**:
```k
.,(`a;`b)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
54. **dictionary_type.k**:
```k
4:.((`a;1);(`b;2))
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
55. **dictionary_with_null_value.k**:
```k
.((`a;1);(`b;_n);(`c;3))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
56. **dictionary_period_index_all_attributes.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[.]
```
Incomplete consumption (position 88/94) (consumed 88/94)
-------------------------------------------------
57. **test_minimal_dict.k**:
```k
d: .,(`a;1;.,(`x;2));d[.]
```
Incomplete consumption (position 17/23) (consumed 17/23)
-------------------------------------------------
58. **test_simple_period.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[.]
```
Incomplete consumption (position 31/37) (consumed 31/37)
-------------------------------------------------
59. **test_attr_access.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[`b.]
```
Incomplete consumption (position 31/37) (consumed 31/37)
-------------------------------------------------
60. **test_dict_create.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d
```
Incomplete consumption (position 31/34) (consumed 31/34)
-------------------------------------------------
61. **test_dict_with_attr.k**:
```k
.((`a;1);(`b;2;.((`x;2);(`y;3))))
```
Incomplete consumption (position 29/30) (consumed 29/30)
-------------------------------------------------
62. **test_show_dict.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d
```
Incomplete consumption (position 88/91) (consumed 88/91)
-------------------------------------------------
63. **test_simple_dict_create.k**:
```k
.,(`a;1)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
64. **test_specific_attr.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
Incomplete consumption (position 88/94) (consumed 88/94)
-------------------------------------------------
65. **test_specific_attr_fixed.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
Incomplete consumption (position 88/94) (consumed 88/94)
-------------------------------------------------
66. **empty_char_vector.k**:
```k
0#"abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
67. **empty_float_vector_test.k**:
```k
0#1.0 2.0 3.0
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
68. **empty_symbol_atomic.k**:
```k
0#`test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
69. **empty_dictionary.k**:
```k
.()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
70. **empty_list.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
71. **test_symbol_take.k**:
```k
0 # `test`
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
72. **test_symbol_parsing.k**:
```k
0#`test`
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
73. **drop_negative.k**:
```k
-4 _ 0 1 2 3 4 5 6 7
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
74. **drop_positive.k**:
```k
4 _ 0 1 2 3 4 5 6 7
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
75. **empty_mixed_vector.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
76. **enlist_operator.k**:
```k
,5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
77. **enumerate_empty_int.k**:
```k
!0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
78. **enumerate_operator.k**:
```k
!5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
79. **equal_operator.k**:
```k
3 = 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
80. **char_vector_equal.k**:
```k
"abc" = "abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
81. **char_vector_different.k**:
```k
"abc" = "abz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
82. **char_vector_match_equal.k**:
```k
"abc" ~ "abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
83. **char_vector_match_different.k**:
```k
"abc" ~ "abz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
84. **float_infinity_match_equal.k**:
```k
0i ~ 0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
85. **float_neg_infinity_match_equal.k**:
```k
-0i ~ -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
86. **float_infinity_match_different.k**:
```k
0i ~ -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
87. **float_infinity_equal.k**:
```k
0i = 0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
88. **float_neg_infinity_equal.k**:
```k
-0i = -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
89. **float_infinity_equal_different.k**:
```k
0i = -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
90. **first_operator.k**:
```k
* (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
91. **float_decimal_point.k**:
```k
10.
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
92. **float_exponential.k**:
```k
0.17e03
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
93. **float_exponential_large.k**:
```k
1e15
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
94. **float_exponential_small.k**:
```k
1e-20
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
95. **float_types.k**:
```k
3.14
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
96. **function_add7.k**:
```k
add7:{[arg1] arg1+7}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
97. **function_add7.k**:
```k
(add7) . (5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
98. **function_call_anonymous.k**:
```k
{[arg1] arg1+6} . 7
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
99. **function_call_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
100. **function_call_chain.k**:
```k
foo: (mul) . (8 4)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
101. **function_call_chain.k**:
```k
foo - 12
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
102. **function_call_double.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
103. **function_call_double.k**:
```k
(mul) . (8 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
104. **function_call_simple.k**:
```k
add7:{[arg1] arg1+7}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
105. **function_call_simple.k**:
```k
(add7) . (5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
106. **function_foo_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
107. **function_foo_chain.k**:
```k
foo: (mul) . (8 4)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
108. **function_foo_chain.k**:
```k
foo - 12
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
109. **function_mul.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
110. **function_mul.k**:
```k
(mul) . (8 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
111. **lambda_string_literal.k**:
```k
{"hello"}[]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
112. **lambda_symbol_literal.k**:
```k
{`abc}[]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
113. **named_function_over.k**:
```k
f:{x*%y};f/10 20 30
```
Incomplete consumption (position 8/15) (consumed 8/15)
-------------------------------------------------
114. **named_function_scan.k**:
```k
f:{x*%y};f\10 20 30
```
Incomplete consumption (position 8/15) (consumed 8/15)
-------------------------------------------------
115. **where_generate_scalar.k**:
```k
&4
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
116. **grade_down_operator.k**:
```k
> (3 11 9 9 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
117. **grade_up_operator.k**:
```k
< (3 11 9 9 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
118. **greater_than_operator.k**:
```k
3 > 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
119. **integer_types_int.k**:
```k
42
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
120. **integer_types_long.k**:
```k
123456789j
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
121. **join_operator.k**:
```k
3 , 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
122. **less_than_operator.k**:
```k
3 < 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
123. **math_abs.k**:
```k
_abs -5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
124. **math_exp.k**:
```k
_exp 2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
125. **math_log.k**:
```k
_log 10
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
126. **math_sin.k**:
```k
_sin 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
127. **math_sqrt.k**:
```k
_sqrt 16
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
128. **math_vector.k**:
```k
_sin 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
129. **math_exp_basic.k**:
```k
_exp 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
130. **math_floor_nan.k**:
```k
_ 0n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
131. **math_floor_negative_infinity.k**:
```k
_ -0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
132. **math_floor_special_values.k**:
```k
_ 0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
133. **math_hyperbolic_basic.k**:
```k
_sinh 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
134. **math_inv_matrix_2x2.k**:
```k
_inv ((1 2);(3 4))
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
135. **math_inv_matrix_3x3.k**:
```k
_inv ((1 2 3);(0 1 4);(5 6 0))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
136. **math_inv_matrix_identity_3x3.k**:
```k
_inv ((1 0 0);(0 1 0);(0 0 1))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
137. **math_log_negative.k**:
```k
_log -1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
138. **math_log_zero.k**:
```k
_log 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
139. **math_mul_matrix_2x2.k**:
```k
((1 2);(3 4)) _mul ((5 6);(7 8))
```
Incomplete consumption (position 23/24) (consumed 23/24)
-------------------------------------------------
140. **math_mul_matrix_2x3_3x2.k**:
```k
((1 2 3);(4 5 6)) _mul ((7 8);(9 10);(11 12))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
141. **math_mul_matrix_3x3.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) _mul ((9 8 7);(6 5 4);(3 2 1))
```
Incomplete consumption (position 39/40) (consumed 39/40)
-------------------------------------------------
142. **math_mul_matrix_4x2_2x4.k**:
```k
((1 2);(3 4);(5 6);(7 8)) _mul ((9 10 11 12);(13 14 15 16))
```
Incomplete consumption (position 37/38) (consumed 37/38)
-------------------------------------------------
143. **math_trig_basic.k**:
```k
_sin 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
144. **math_trig_pi.k**:
```k
_cos 3.141592653589793
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
145. **maximum_operator.k**:
```k
3 | 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
146. **minimum_operator.k**:
```k
3 & 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
147. **mixed_list_with_null.k**:
```k
(1;_n;`test;42.5)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
148. **mixed_vector_empty_position.k**:
```k
(1;;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
149. **mixed_vector_multiple_empty.k**:
```k
(1;;;3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
150. **mixed_vector_whitespace_position.k**:
```k
(1; ;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
151. **mod_integer.k**:
```k
7!3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
152. **mod_rotate.k**:
```k
2 ! 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
153. **mod_vector.k**:
```k
1 2 3 4 ! 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
154. **modulus_operator.k**:
```k
7 ! 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
155. **negate_operator.k**:
```k
~0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
156. **nested_vector_test.k**:
```k
((1 2 3);(4 5 6))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
157. **overflow_int_max_plus1.k**:
```k
2147483647 + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
158. **overflow_int_neg_inf.k**:
```k
-0I - 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
159. **overflow_int_neg_inf_minus2.k**:
```k
-0I - 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
160. **overflow_int_null_minus1.k**:
```k
0N - 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
161. **overflow_int_pos_inf.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
162. **overflow_int_pos_inf_plus2.k**:
```k
0I + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
163. **overflow_long_max_plus1.k**:
```k
9223372036854775807j + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
164. **overflow_long_min_minus1.k**:
```k
-9223372036854775808j - 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
165. **overflow_regular_int.k**:
```k
2147483637 + 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
166. **underflow_regular_int.k**:
```k
-2147483639 - 40
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
167. **parentheses_basic.k**:
```k
1 + 2 * 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
168. **parentheses_grouping.k**:
```k
(1 + 2) * 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
169. **debug_mult_only.k**:
```k
3 * 7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
170. **debug_no_outer_paren.k**:
```k
1 + 2 * 3 + 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
171. **debug_left_paren.k**:
```k
(1 + 2)
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
172. **debug_simple_mult.k**:
```k
(1 + 2) * (3 + 4)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
173. **debug_double_nested.k**:
```k
((1+2)*(3+4))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
174. **parentheses_nested.k**:
```k
(1 + (2 * 3))
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
175. **parentheses_nested1.k**:
```k
((1 + 2) * (3 + 4))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
176. **parentheses_nested2.k**:
```k
(10 % (2 + (3 * 4)))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
177. **parentheses_nested3.k**:
```k
(((1 + 2) + 3) * 4)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
178. **parentheses_nested4.k**:
```k
(1 + (2 + (3 + (4 + 5))))
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
179. **parentheses_nested5.k**:
```k
((1 * 2) + (3 * (4 + 5)))
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
180. **parentheses_precedence.k**:
```k
1 + (2 * 3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
181. **parenthesized_vector.k**:
```k
(1;2;3;4)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
182. **list_null_consecutive_semicolons.k**:
```k
(1;;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
183. **list_null_multiple_semicolons.k**:
```k
(;;)
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
184. **list_null_empty_parens.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
185. **power_operator.k**:
```k
2 ^ 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
186. **precedence_chain1.k**:
```k
10 + 20 * 30 + 40
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
187. **precedence_chain2.k**:
```k
100 % 10 + 20 * 30 + 40
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
188. **precedence_complex1.k**:
```k
10 % 2 + 3 * 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
189. **precedence_complex2.k**:
```k
10 + 20 % 30 * 40
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
190. **precedence_mixed1.k**:
```k
5 - 2 + 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
191. **precedence_mixed2.k**:
```k
5 * 2 + 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
192. **precedence_mixed3.k**:
```k
5 + 2 * 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
193. **precedence_power1.k**:
```k
2 ^ 3 + 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
194. **precedence_power2.k**:
```k
2 + 3 ^ 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
195. **precedence_spec1.k**:
```k
81 % 1 + 2 * 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
196. **precedence_spec2.k**:
```k
120 % 4 * 2 + 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
197. **reciprocal_operator.k**:
```k
%4
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
198. **reverse_operator.k**:
```k
| (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
199. **scalar_vector_addition.k**:
```k
1 2 + 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
200. **scalar_vector_multiplication.k**:
```k
1 2 * 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
201. **shape_operator.k**:
```k
^ (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
202. **shape_operator_empty_vector.k**:
```k
^ ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
203. **shape_operator_jagged.k**:
```k
^ ((1 2 3); (4 5); (6 7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
204. **dyadic_plus_vector_vector.k**:
```k
1 2 3 + 4 5 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
205. **dyadic_plus_atom_vector.k**:
```k
5 + 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
206. **dyadic_plus_vector_atom.k**:
```k
1 2 3 + 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
207. **dyadic_minus_vector_vector.k**:
```k
5 6 7 - 1 2 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
208. **dyadic_minus_atom_vector.k**:
```k
10 - 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
209. **dyadic_minus_vector_atom.k**:
```k
5 6 7 - 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
210. **dyadic_times_vector_vector.k**:
```k
2 3 4 * 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
211. **dyadic_times_atom_vector.k**:
```k
3 * 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
212. **dyadic_times_vector_atom.k**:
```k
1 2 3 * 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
213. **dyadic_divide_vector_vector.k**:
```k
6 8 10 % 2 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
214. **dyadic_divide_atom_vector.k**:
```k
12 % 2 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
215. **dyadic_divide_vector_atom.k**:
```k
4 6 8 % 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
216. **dyadic_min_vector_vector.k**:
```k
5 8 3 & 2 9 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
217. **dyadic_min_atom_vector.k**:
```k
3 & 5 2 8
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
218. **dyadic_min_vector_atom.k**:
```k
5 2 8 & 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
219. **dyadic_max_vector_vector.k**:
```k
5 8 3 | 2 9 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
220. **dyadic_max_atom_vector.k**:
```k
6 | 3 8 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
221. **dyadic_max_vector_atom.k**:
```k
3 8 2 | 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
222. **dyadic_less_vector_vector.k**:
```k
3 5 2 < 4 2 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
223. **dyadic_less_atom_vector.k**:
```k
3 < 4 2 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
224. **dyadic_less_vector_atom.k**:
```k
3 5 2 < 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
225. **dyadic_more_vector_vector.k**:
```k
3 5 2 > 2 4 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
226. **dyadic_more_atom_vector.k**:
```k
4 > 2 5 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
227. **dyadic_more_vector_atom.k**:
```k
3 5 2 > 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
228. **dyadic_equal_vector_vector.k**:
```k
3 5 2 = 3 4 2
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
229. **dyadic_equal_atom_vector.k**:
```k
3 = 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
230. **dyadic_equal_vector_atom.k**:
```k
3 4 5 = 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
231. **dyadic_power_vector_vector.k**:
```k
2 3 4 ^ 3 2 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
232. **dyadic_power_atom_vector.k**:
```k
2 ^ 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
233. **dyadic_power_vector_atom.k**:
```k
2 3 4 ^ 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
234. **shape_operator_jagged_3d.k**:
```k
^ (((1 2); (3 4 5)); ((6 7); (8 9 10)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
235. **shape_operator_jagged_matrix.k**:
```k
^ ((1 2 3); (4 5); (6 7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
236. **shape_operator_matrix.k**:
```k
^ ((1 2 3); (4 5 6); (7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
237. **shape_operator_matrix_2x3.k**:
```k
^ ((1 2 3); (4 5 6))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
238. **shape_operator_matrix_3x3.k**:
```k
^ ((1 2 3); (4 5 6); (7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
239. **shape_operator_scalar.k**:
```k
^ 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
240. **shape_operator_tensor_2x2x3.k**:
```k
^ (((1 2 3); (4 5 6)); ((7 8 9); (10 11 12)))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
241. **shape_operator_tensor_3d.k**:
```k
^ (((1 2); (3 4)); ((5 6); (7 8)); ((9 10); (11 12)))
```
Incomplete consumption (position 38/39) (consumed 38/39)
-------------------------------------------------
242. **shape_operator_vector.k**:
```k
^ (1 2 3 4 5)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
243. **simple_addition.k**:
```k
1 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
244. **divide_float.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
245. **divide_float.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
246. **divide_float.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
247. **divide_integer.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
248. **divide_integer.k**:
```k
b:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
249. **divide_integer.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
250. **simple_multiplication.k**:
```k
4 * 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
251. **simple_nested_test.k**:
```k
(1 2 3)
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
252. **minus_integer.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
253. **minus_integer.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
254. **minus_integer.k**:
```k
a-c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
255. **test_adverb_aware_evaluation.k**:
```k
+/ 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
256. **special_float_neg_inf.k**:
```k
-0i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
257. **special_float_null.k**:
```k
0n
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
258. **special_float_pos_inf.k**:
```k
0i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
259. **special_int_neg_inf.k**:
```k
-0I
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
260. **special_int_null.k**:
```k
0N
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
261. **special_int_pos_inf.k**:
```k
0I
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
262. **special_null.k**:
```k
_n
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
263. **special_int_pos_inf_plus_1.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
264. **special_int_null_plus_1.k**:
```k
0N + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
265. **special_int_neg_inf_plus_1.k**:
```k
-0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
266. **special_float_null_plus_1.k**:
```k
0n + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
267. **special_1_plus_int_pos_inf.k**:
```k
1 + 0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
268. **special_1_plus_int_null.k**:
```k
1 + 0N
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
269. **special_int_vector.k**:
```k
0I 0N -0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
270. **special_float_vector.k**:
```k
0i 0n -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
271. **square_bracket_function.k**:
```k
div:{[op1;op2] op1%op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
272. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
273. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
274. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
275. **square_bracket_vector_single.k**:
```k
v[4]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
276. **string_representation_int.k**:
```k
5:42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
277. **string_representation_symbol.k**:
```k
5:`symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
278. **string_representation_vector.k**:
```k
5:(1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
279. **symbol_quoted.k**:
```k
`"a symbol"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
280. **symbol_simple.k**:
```k
`foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
281. **symbol_vector_compact.k**:
```k
`a`b`c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
282. **symbol_vector_spaces.k**:
```k
`a `b `c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
283. **symbol_period_foo.k**:
```k
`foo.
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
284. **symbol_period_foobar.k**:
```k
`foo.bar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
285. **symbol_period_dotbar.k**:
```k
`.bar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
286. **symbol_period_dotk.k**:
```k
`.k
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
287. **io_read_int.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\int.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
288. **io_read_float.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
289. **io_read_symbol.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\symbol.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
290. **io_read_intvec.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
291. **io_read_debug.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
292. **io_write_int.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_write.l" 1: 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
293. **io_roundtrip.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l" 1: (1;2.5;"hello")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
294. **io_roundtrip.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
295. **io_monadic_1_int_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
296. **io_monadic_1_float_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
297. **io_monadic_1_char_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
298. **io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
299. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
300. **io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
301. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
302. **io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
303. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
304. **io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
305. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
306. **io_monadic_1_vs_2_int_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
307. **io_monadic_1_vs_2_float_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
308. **io_monadic_1_vs_2_char_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
309. **io_monadic_1_symbol_fallback.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\symbol.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
310. **match_simple_vectors.k**:
```k
5 6 7 ~ 5 6 7
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
311. **take_operator_basic.k**:
```k
3#1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
312. **take_operator_empty_float.k**:
```k
0#0.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
313. **take_operator_empty_symbol.k**:
```k
0#``
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
314. **take_operator_overflow.k**:
```k
10#1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
315. **take_operator_scalar.k**:
```k
3#42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
316. **reshape_basic.k**:
```k
3 4 # !12
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
317. **division_float_4_2.0.k**:
```k
4 % 2.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
318. **division_float_5_2.5.k**:
```k
5 % 2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
319. **division_int_4_2.k**:
```k
4 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
320. **division_int_5_2.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
321. **division_rules_10_3.k**:
```k
10 % 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
322. **division_rules_12_4.k**:
```k
12 % 4
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
323. **division_rules_4_2.k**:
```k
4 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
324. **division_rules_5_2.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
325. **enumerate.k**:
```k
! 2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
326. **grade_down_no_parens.k**:
```k
> 3 11 9 9 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
327. **grade_up_no_parens.k**:
```k
< 3 11 9 9 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
328. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
329. **multiline_function_single.k**:
```k
test . 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
330. **null_vector.k**:
```k
(;1;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
331. **scoping_single.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
332. **scoping_single.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
333. **scoping_single.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
334. **semicolon_simple.k**:
```k
(3 + 4; 5 + 6; -20.45)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
335. **semicolon_vars.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
336. **semicolon_vars.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
337. **semicolon_vars.k**:
```k
(a + b; a * b; a - b)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
338. **semicolon_vector.k**:
```k
a: 1 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
339. **semicolon_vector.k**:
```k
b: 3 4
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
340. **semicolon_vector.k**:
```k
(3 + 4; b; -20.45)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
341. **test_semicolon.k**:
```k
1 2; 3 4
```
Incomplete consumption (position 2/6) (consumed 2/6)
-------------------------------------------------
342. **simple_scalar_div.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
343. **single_no_semicolon.k**:
```k
(42)
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
344. **smart_division1.k**:
```k
5 10 % 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
345. **smart_division2.k**:
```k
4 8 % 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
346. **smart_division3.k**:
```k
6 12 18 % 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
347. **special_0i_plus_1.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
348. **special_0n_plus_1.k**:
```k
0N + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
349. **special_1_plus_neg0i.k**:
```k
1 + -0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
350. **special_neg0i_plus_1.k**:
```k
-0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
351. **special_underflow.k**:
```k
-0I - 27
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
352. **special_underflow_2.k**:
```k
-0I - 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
353. **special_underflow_3.k**:
```k
-0I - 1000
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
354. **type_float.k**:
```k
4: 3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
355. **type_null.k**:
```k
4: _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
356. **type_symbol.k**:
```k
4: `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
357. **type_vector.k**:
```k
4: (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
358. **vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
359. **type_operator_float.k**:
```k
4: 3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
360. **type_operator_null.k**:
```k
4: _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
361. **type_operator_symbol.k**:
```k
4: `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
362. **type_operator_vector_char.k**:
```k
4: ("abc")
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
363. **type_operator_vector_float.k**:
```k
4: (1.0 2.0 3.0)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
364. **type_operator_vector_int.k**:
```k
4: (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
365. **type_operator_vector_symbol.k**:
```k
4: "abc"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
366. **type_promotion_float_int.k**:
```k
1.5 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
367. **type_promotion_float_long.k**:
```k
1.5 + 1j
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
368. **type_promotion_int_float.k**:
```k
2 + 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
369. **type_promotion_int_long.k**:
```k
2 + 1j
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
370. **type_promotion_long_float.k**:
```k
1j + 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
371. **type_promotion_long_int.k**:
```k
1j + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
372. **unary_minus_operator.k**:
```k
- 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
373. **unique_operator.k**:
```k
? (1 2 1 3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
374. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
375. **variable_assignment.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
376. **variable_reassignment.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
377. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
378. **variable_reassignment.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
379. **variable_scoping_global_access.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
380. **variable_scoping_global_access.k**:
```k
test1: {[x] globalVar + x}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
381. **variable_scoping_global_access.k**:
```k
result1: test1 . 50
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
382. **variable_scoping_global_access.k**:
```k
result1
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
383. **variable_scoping_global_assignment.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
384. **variable_scoping_global_assignment.k**:
```k
result5: test5 . 10
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
385. **variable_scoping_global_assignment.k**:
```k
result5
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
386. **variable_scoping_global_assignment.k**:
```k
globalVar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
387. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
388. **variable_scoping_global_unchanged.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
389. **variable_scoping_global_unchanged.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
390. **variable_scoping_global_unchanged.k**:
```k
globalVar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
391. **variable_scoping_local_hiding.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
392. **variable_scoping_local_hiding.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
393. **variable_scoping_local_hiding.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
394. **variable_scoping_nested_functions.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
395. **variable_scoping_nested_functions.k**:
```k
result4: outer . 20
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
396. **variable_scoping_nested_functions.k**:
```k
result4
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
397. **variable_usage.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
398. **variable_usage.k**:
```k
y:20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
399. **variable_usage.k**:
```k
z:x+y
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
400. **variable_usage.k**:
```k
z
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
401. **dot_execute.k**:
```k
."2+2"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
402. **dot_execute_context.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
403. **dot_execute_context.k**:
```k
."foo:8"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
404. **dot_execute_context.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
405. **dictionary_enumerate.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
406. **dictionary_enumerate.k**:
```k
!d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
407. **null_operations.k**:
```k
_n@7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
408. **dictionary_dot_apply.k**:
```k
d: .((`a;1;.());(`b;2;.()))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
409. **dictionary_dot_apply.k**:
```k
d@`a
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
410. **monadic_format_basic.k**:
```k
$1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
411. **monadic_format_types.k**:
```k
$42.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
412. **monadic_format_vector.k**:
```k
$1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
413. **monadic_format_string_hello.k**:
```k
$"hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
414. **monadic_format_symbol_hello.k**:
```k
$`hello
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
415. **monadic_format_symbol_simple.k**:
```k
$`test
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
416. **monadic_format_dictionary.k**:
```k
$.((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
417. **monadic_format_nested_list.k**:
```k
$((1;2;3);(4;5;6))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
418. **monadic_format_integer.k**:
```k
$42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
419. **monadic_format_float.k**:
```k
$3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
420. **monadic_format_vector_simple.k**:
```k
$1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
421. **format_integer.k**:
```k
0$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
422. **ci_adverb_vector.k**:
```k
_ci' 97 94 80
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
423. **format_float_numeric.k**:
```k
0.0$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
424. **form_long.k**:
```k
0j$"42"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
425. **format_numeric.k**:
```k
5$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
426. **form_string_pad_left.k**:
```k
7$"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
427. **format_symbol_pad_left.k**:
```k
10$`hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
428. **format_symbol_pad_left_8.k**:
```k
8$`hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
429. **format_pad_left.k**:
```k
5$42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
430. **format_pad_right.k**:
```k
-5$42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
431. **format_float_width_precision.k**:
```k
10.2$3.14159
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
432. **format_float_precision.k**:
```k
8.2$3.14159
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
433. **format_0_1.k**:
```k
0$1.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
434. **format_1_1.k**:
```k
1$1.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
435. **format_symbol_string_mixed_vector.k**:
```k
`$("hello";"world";"test")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
436. **form_integer_charvector.k**:
```k
0$"42"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
437. **dot_execute_variables.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
438. **dot_execute_variables.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
439. **dot_execute_variables.k**:
```k
."a%b"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
440. **format_braces_expressions.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
441. **format_braces_expressions.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
442. **format_braces_nested_expr.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
443. **format_braces_nested_expr.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
444. **format_braces_nested_expr.k**:
```k
z:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
445. **format_braces_complex.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
446. **format_braces_complex.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
447. **format_braces_complex.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
448. **format_braces_string.k**:
```k
name:"John"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
449. **format_braces_string.k**:
```k
age:25
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
450. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
Incomplete consumption (position 3/27) (consumed 3/27)
-------------------------------------------------
451. **format_braces_simple.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
452. **format_braces_simple.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
453. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
Incomplete consumption (position 3/33) (consumed 3/33)
-------------------------------------------------
454. **format_braces_nested_arith.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
455. **format_braces_nested_arith.k**:
```k
b:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
456. **format_braces_nested_arith.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
457. **format_braces_float.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
458. **format_braces_float.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
459. **format_braces_float.k**:
```k
c:3.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
460. **format_braces_mixed_arith.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
461. **format_braces_mixed_arith.k**:
```k
y:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
462. **format_braces_mixed_arith.k**:
```k
z:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
463. **format_braces_example.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
464. **format_braces_example.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
465. **format_braces_function_calls.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
466. **format_braces_function_calls.k**:
```k
product:{[x;y] x*y}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
467. **format_braces_function_calls.k**:
```k
double:{[x] x*2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
468. **format_braces_nested_function_calls.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
469. **format_braces_nested_function_calls.k**:
```k
double:{[x] x*2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
470. **format_braces_nested_function_calls.k**:
```k
square:{[x] x*x}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
471. **log.k**:
```k
_log 10
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
472. **time_t.k**:
```k
.((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
473. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 5/23) (consumed 5/23)
-------------------------------------------------
474. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
Incomplete consumption (position 5/36) (consumed 5/36)
-------------------------------------------------
475. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 5/23) (consumed 5/23)
-------------------------------------------------
476. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 6/24) (consumed 6/24)
-------------------------------------------------
477. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
Incomplete consumption (position 6/37) (consumed 6/37)
-------------------------------------------------
478. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 6/24) (consumed 6/24)
-------------------------------------------------
479. **time_lt.k**:
```k
_lt 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
480. **time_jd.k**:
```k
_jd 20260206
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
481. **time_dj.k**:
```k
_dj 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
482. **in.k**:
```k
4 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
483. **list_dv_basic.k**:
```k
3 4 4 5 _dv 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
484. **list_dv_nomatch.k**:
```k
3 4 4 5 _dv 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
485. **list_di_basic.k**:
```k
3 2 4 5 _di 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
486. **list_di_multiple.k**:
```k
3 2 4 5 _di 1 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
487. **list_sv_base10.k**:
```k
10 _sv 1 9 9 5
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
488. **list_sv_base2.k**:
```k
2 _sv 1 0 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
489. **list_sv_mixed.k**:
```k
10 10 10 10 _sv 1 9 9 5
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
490. **list_setenv.k**:
```k
`TESTVAR _setenv "hello world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
491. **test_ci_basic.k**:
```k
_ci 65
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
492. **test_ci_vector.k**:
```k
_ci 65 66 67
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
493. **test_vs_dyadic.k**:
```k
10 _vs 1995
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
494. **test_ic_vector.k**:
```k
_ic "ABC"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
495. **test_monadic_colon.k**:
```k
: 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
496. **test_sm_basic.k**:
```k
`foo _sm `foo
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
497. **test_sm_simple.k**:
```k
`a _sm `a
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
498. **test_ss_basic.k**:
```k
"hello world" _ss "world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
499. **statement_assignment_basic.k**:
```k
a: 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
500. **statement_conditional_basic.k**:
```k
:[1;2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
501. **statement_do_basic.k**:
```k
i:0;do[3;i+:1];i
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
502. **statement_do_simple.k**:
```k
i:0;do[3;i+:1]
```
Incomplete consumption (position 3/13) (consumed 3/13)
-------------------------------------------------
503. **semicolon_vars_test.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
504. **apply_and_assign_simple.k**:
```k
i:0;i+:1;i
```
Incomplete consumption (position 3/10) (consumed 3/10)
-------------------------------------------------
505. **apply_and_assign_multiline.k**:
```k
i:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
506. **apply_and_assign_multiline.k**:
```k
i+:1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
507. **apply_and_assign_multiline.k**:
```k
i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
508. **io_read_basic.k**:
```k
0:`test
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
509. **io_write_basic.k**:
```k
`testW 0: ("line1";"line2";"line3");0:`testW
```
Incomplete consumption (position 9/13) (consumed 9/13)
-------------------------------------------------
510. **io_append_simple.k**:
```k
`testfile 5: "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
511. **io_append_basic.k**:
```k
`test 5: "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
512. **io_append_multiple.k**:
```k
`test 5: "hello"; `test 5: "world"; `test 5: 1 2 3
```
Incomplete consumption (position 3/14) (consumed 3/14)
-------------------------------------------------
513. **io_read_bytes_basic.k**:
```k
`test 0:,"hello";6:`test
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
514. **io_read_bytes_empty.k**:
```k
`empty 0:(); 6:`empty
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
515. **io_write_bytes_basic.k**:
```k
`test 6:,"ABC"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
516. **io_write_bytes_overwrite.k**:
```k
`test 6:,"ABC"; `test 6:,"XYZ"
```
Incomplete consumption (position 4/10) (consumed 4/10)
-------------------------------------------------
517. **io_write_bytes_binary.k**:
```k
`test 6:(0 1 2 255)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
518. **search_in_basic.k**:
```k
4 _in 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
519. **search_in_notfound.k**:
```k
6 _in 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
520. **search_bin_basic.k**:
```k
3 4 5 6 _bin 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
521. **search_binl_eachleft.k**:
```k
1 3 5 _binl 1 2 3 4 5
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
522. **search_lin_intersection.k**:
```k
1 3 5 7 9 _lin 1 2 3 4 5
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
523. **amend_test.k**:
```k
(.) . (((1 2 3 4 5);(6 7 8 9 10)); 0 2; +; 10)
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
524. **find_basic.k**:
```k
9 8 7 6 5 4 3 ? 7
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
525. **find_notfound.k**:
```k
9 8 7 6 5 4 3 ? 1
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
526. **format_float_precision_vector_simple.k**:
```k
10.1$(1.5;2.5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
527. **format_float_precision_mixed_vector.k**:
```k
7.2$(1.5;2.7;3.14159;4.2)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
528. **format_pad_mixed_vector.k**:
```k
10$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
529. **format_pad_negative_mixed_vector.k**:
```k
-10$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
530. **vector_notation_empty.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
531. **vector_notation_functions.k**:
```k
double: {[x] x * 2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
532. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
533. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
534. **vector_notation_nested.k**:
```k
((1 + 2); (3 + 4); (5 + 6))
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
535. **vector_notation_semicolon.k**:
```k
(3 + 4; 5 + 6; -20.45)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
536. **vector_notation_single_group.k**:
```k
(42)
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
537. **vector_notation_space.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
538. **vector_notation_variables.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
539. **vector_notation_variables.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
540. **vector_notation_variables.k**:
```k
(a + b; a * b; a - b)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
541. **vector_addition.k**:
```k
1 2 + 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
542. **vector_division.k**:
```k
1 2 % 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
543. **vector_index_duplicate.k**:
```k
5 8 4 9 @ 0 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
544. **vector_index_first.k**:
```k
5 8 4 9 @ 0
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
545. **vector_index_multiple.k**:
```k
5 8 4 9 @ 1 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
546. **vector_index_reverse.k**:
```k
5 8 4 9 @ 3 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
547. **vector_index_single.k**:
```k
5 8 4 9 @ 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
548. **vector_multiplication.k**:
```k
1 2 * 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
549. **vector_subtraction.k**:
```k
1 2 - 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
550. **vector_with_null.k**:
```k
(_n;1;2)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
551. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
552. **where_operator.k**:
```k
& (1 0 1 1 0)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
553. **where_vector_counts.k**:
```k
& (3 2 1)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
554. **floor_operator.k**:
```k
_3.7
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
555. **adverb_backslash_colon_basic.k**:
```k
1 2 3 +\: 4 5 6
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
556. **adverb_slash_colon_basic.k**:
```k
1 2 3 +/: 4 5 6
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
557. **adverb_tick_colon_basic.k**:
```k
-': 4 8 9 12 20
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
558. **amend_apply.k**:
```k
(.) . (((1 2 3 4 5);(6 7 8 9 10)); 0 2; +; 10)
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
559. **amend_parenthesized.k**:
```k
(.) . (1 2 3; 0; +; 10)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
560. **amend_test_anonymous_func.k**:
```k
f:{x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
561. **amend_test_anonymous_func.k**:
```k
(.).((1 2 3); 0; f; 10)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
562. **amend_test_func_var.k**:
```k
f:{x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
563. **amend_test_func_var.k**:
```k
(.).((1 2 3); 0; f; 10)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
564. **conditional_bracket_test.k**:
```k
:[1 < 2; "true"; "false"]
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
565. **conditional_false.k**:
```k
:[0; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
566. **conditional_simple_test.k**:
```k
:[1; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
567. **conditional_true.k**:
```k
:[1; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
568. **dictionary_null_index.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
569. **dictionary_null_index.k**:
```k
d@_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
570. **dictionary_unmake.k**:
```k
d: .((`a;1);(`b;2)); result:. d; result
```
Incomplete consumption (position 16/24) (consumed 16/24)
-------------------------------------------------
571. **do_loop.k**:
```k
i: 0; do[3; i+: 1]
```
Incomplete consumption (position 3/13) (consumed 3/13)
-------------------------------------------------
572. **dyadic_divide_bracket.k**:
```k
%[20; 4]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
573. **dyadic_minus_bracket.k**:
```k
-[10; 3]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
574. **dyadic_multiply_bracket.k**:
```k
*[4; 6]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
575. **dyadic_plus_bracket.k**:
```k
+[3; 5]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
576. **dyadic_divide_dot_apply.k**:
```k
(%) . (20; 4)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
577. **dyadic_minus_dot_apply.k**:
```k
(-) . (10; 3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
578. **dyadic_multiply_dot_apply.k**:
```k
(*) . (4; 6)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
579. **dyadic_plus_dot_apply.k**:
```k
(+) . (3; 5)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
580. **empty_brackets_dictionary.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
581. **empty_brackets_dictionary.k**:
```k
d[]
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
582. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
583. **empty_brackets_vector.k**:
```k
v[]
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
584. **format_braces_complex_expressions.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
585. **format_braces_complex_expressions.k**:
```k
product:{[x;y] x*y}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
586. **format_float_precision_complex_mixed.k**:
```k
10.3$(1.234;2.567;3.890;4.123)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
587. **format_float_vector.k**:
```k
0.0$(1;2.5;3.14;42)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
588. **format_int_vector.k**:
```k
0$(1;2.5;3.14;42)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
589. **form_0_string.k**:
```k
0$"123"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
590. **form_0_vector.k**:
```k
0$("123";"456")
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
591. **form_0_float_string.k**:
```k
0.0$"3.14"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
592. **form_0_float_vector.k**:
```k
0.0$("3.14";"1e48";"1.4e-27")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
593. **form_symbol_string.k**:
```k
`$"abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
594. **form_symbol_vector.k**:
```k
`$("abc";"de";"f")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
595. **format_string_pad_left.k**:
```k
10$"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
596. **format_string_pad_right.k**:
```k
-10$"test"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
597. **format_vector_int.k**:
```k
0$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
598. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
Incomplete consumption (position 22/23) (consumed 22/23)
-------------------------------------------------
599. **group_operator.k**:
```k
=a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
600. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
601. **in_basic.k**:
```k
4 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
602. **in_notfound.k**:
```k
10 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
603. **in_simple.k**:
```k
5 _in 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
604. **isolated.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
605. **isolated.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
606. **modulo.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
607. **modulo.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
608. **modulo.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
609. **monadic_format_mixed_vector.k**:
```k
$(1;2.5;"hello";`symbol)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
610. **over_plus_empty.k**:
```k
+/!0
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
611. **simple_division.k**:
```k
8 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
612. **simple_subtraction.k**:
```k
5 - 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
613. **string_parse.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
614. **string_parse.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
615. **string_parse.k**:
```k
."a+b"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
616. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
617. **k_tree_retrieval_relative.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
618. **k_tree_enumerate.k**:
```k
!`
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
619. **k_tree_current_branch.k**:
```k
_d
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
620. **k_tree_dictionary_indexing.k**:
```k
.k[`foo]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
621. **k_tree_nested_indexing.k**:
```k
.k.dd[`b]
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
622. **k_tree_verify_root.k**:
```k
.k
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
623. **k_tree_flip_dictionary.k**:
```k
.+(`a`b`c;1 2 3)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
624. **k_tree_null_to_dict_conversion.k**:
```k
.k
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
625. **k_tree_dictionary_assignment.k**:
```k
.k.dd
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
626. **k_tree_test_bracket_indexing.k**:
```k
d: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 22/23) (consumed 22/23)
-------------------------------------------------
627. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
628. **k_tree_flip_test.k**:
```k
+(`a`b`c;1 2 3)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
629. **vector_null_index.k**:
```k
v: 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
630. **vector_null_index.k**:
```k
v@_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
631. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
632. **while_safe_test.k**:
```k
i: 0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
633. **serialization_bd_db_integer.k**:
```k
_bd 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
634. **serialization_bd_db_float.k**:
```k
_bd 3.14159
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
635. **serialization_bd_db_symbol.k**:
```k
_bd `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
636. **serialization_bd_db_null.k**:
```k
_bd _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
637. **serialization_bd_db_integervector.k**:
```k
_bd 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
638. **serialization_bd_db_floatvector.k**:
```k
_bd 1.1 2.2 3.3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
639. **serialization_bd_db_charactervector.k**:
```k
_bd "hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
640. **serialization_bd_db_symbolvector.k**:
```k
_bd `a`b`c
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
641. **serialization_bd_db_dictionary.k**:
```k
_bd .((`a;`"1");(`b;`"2"))
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
642. **serialization_bd_db_anonymousfunction.k**:
```k
_bd {[x] x+1}
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
643. **serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
644. **serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
645. **db_basic_integer.k**:
```k
_db _bd 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
646. **db_float.k**:
```k
_db _bd 3.14
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
647. **db_symbol.k**:
```k
_db _bd `test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
648. **db_int_vector.k**:
```k
_db _bd 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
649. **db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
650. **db_char_vector.k**:
```k
_db _bd "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
651. **db_list_simple.k**:
```k
_db _bd (1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
652. **db_dict_simple.k**:
```k
_db _bd .((`a;1);(`b;2);(`c;3;))
```
Incomplete consumption (position 23/24) (consumed 23/24)
-------------------------------------------------
653. **db_function_simple.k**:
```k
_db _bd {+}
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
654. **db_function_params.k**:
```k
_db _bd {x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
655. **db_null.k**:
```k
_db _bd 0N
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
656. **db_empty_list.k**:
```k
_db _bd ()
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
657. **db_float_simple.k**:
```k
_db _bd 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
658. **db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
659. **db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
660. **db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
661. **db_symbol_simple.k**:
```k
_db _bd `hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
662. **db_list_longer.k**:
```k
_db _bd (1;2;3;4;5)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
663. **db_list_mixed_types.k**:
```k
_db _bd (1;`test;3.14;"hello")
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
664. **db_function_complex.k**:
```k
_db _bd {[x;y] x*y+z}
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
665. **db_function_simple_math.k**:
```k
_db _bd {x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
666. **db_nested_dict_vectors.k**:
```k
_db _bd .((`a;(1;2;3));(`b;(`hello;`world;`test)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
667. **db_nested_lists.k**:
```k
_db _bd ((1;2;3);(`hello;`world;`test);(4.5;6.7))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
668. **db_mixed_list.k**:
```k
_db _bd (1;`test;3.14)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
669. **db_dict_single_entry.k**:
```k
_db _bd .,(`a;1)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
670. **db_dict_symbol_2char.k**:
```k
_db _bd .,(`ab;1 2)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
671. **db_dict_symbol_8char.k**:
```k
_db _bd .,(`abcdefgh;1 2 3 4 5 6 7 8)
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
672. **db_dict_multi_entry.k**:
```k
_db _bd .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
673. **db_dict_five_entries.k**:
```k
_db _bd .((`a;1);(`b;2);(`c;3);(`d;4);(`e;5))
```
Incomplete consumption (position 34/35) (consumed 34/35)
-------------------------------------------------
674. **db_dict_complex_attributes.k**:
```k
_db _bd .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))))
```
Incomplete consumption (position 88/89) (consumed 88/89)
-------------------------------------------------
675. **db_dict_empty.k**:
```k
_db _bd .()
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
676. **db_dict_with_null_attrs.k**:
```k
_db _bd .,(`a;1;.())
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
677. **db_dict_with_empty_attrs.k**:
```k
_db _bd .((`a;1);(`b;2;.()))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
678. **db_enlist_single_int.k**:
```k
_db _bd ,5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
679. **db_enlist_single_symbol.k**:
```k
_db _bd ,`test
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
680. **db_enlist_single_string.k**:
```k
_db _bd ,"hello"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
681. **db_enlist_vector.k**:
```k
_db _bd ,(1 2 3)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
682. **serialization_bd_null_edge_0.k**:
```k
_bd _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
683. **serialization_bd_integer_edge_0.k**:
```k
_bd 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
684. **serialization_bd_integer_edge_1.k**:
```k
_bd 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
685. **serialization_bd_integer_edge_-1.k**:
```k
_bd -1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
686. **serialization_bd_integer_edge_2147483647.k**:
```k
_bd 2147483647
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
687. **serialization_bd_integer_edge_-2147483648.k**:
```k
_bd -2147483648
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
688. **serialization_bd_integer_edge_0N.k**:
```k
_bd 0N
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
689. **serialization_bd_integer_edge_0I.k**:
```k
_bd 0I
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
690. **serialization_bd_integer_edge_-0I.k**:
```k
_bd -0I
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
691. **serialization_bd_float_edge_0.0.k**:
```k
_bd 0.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
692. **serialization_bd_float_edge_1.0.k**:
```k
_bd 1.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
693. **serialization_bd_float_edge_-1.0.k**:
```k
_bd -1.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
694. **serialization_bd_float_edge_0.5.k**:
```k
_bd 0.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
695. **serialization_bd_float_edge_-0.5.k**:
```k
_bd -0.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
696. **serialization_bd_float_edge_0n.k**:
```k
_bd 0n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
697. **serialization_bd_float_edge_0i.k**:
```k
_bd 0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
698. **serialization_bd_float_edge_-0i.k**:
```k
_bd -0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
699. **serialization_bd_symbol_edge_a.k**:
```k
_bd `a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
700. **serialization_bd_symbol_edge_symbol.k**:
```k
_bd `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
701. **serialization_bd_symbol_edge_test123.k**:
```k
_bd `test123
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
702. **serialization_bd_symbol_edge_underscore.k**:
```k
_bd `_underscore
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
703. **serialization_bd_symbol_edge_hello.k**:
```k
_bd `"hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
704. **serialization_bd_symbol_edge_newline_tab.k**:
```k
_bd `"\n\t"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
705. **serialization_bd_symbol_edge_001.k**:
```k
_bd `"\001"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
706. **serialization_bd_symbol_edge_empty.k**:
```k
_bd `
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
707. **serialization_bd_charactervector_edge_empty.k**:
```k
_bd ""
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
708. **serialization_bd_charactervector_edge_hello.k**:
```k
_bd "hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
709. **serialization_bd_charactervector_edge_whitespace.k**:
```k
_bd "\n\t\r"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
710. **serialization_bd_integervector_edge_empty.k**:
```k
_bd !0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
711. **serialization_bd_integervector_edge_single.k**:
```k
_bd ,1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
712. **serialization_bd_integervector_edge_123.k**:
```k
_bd 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
713. **serialization_bd_integervector_edge_special.k**:
```k
_bd 0N 0I -0I
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
714. **serialization_bd_list_edge_empty.k**:
```k
_bd ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
715. **serialization_bd_list_edge_null.k**:
```k
_bd ,_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
716. **serialization_bd_list_edge_complex.k**:
```k
_bd (_n;`symbol;{[]})
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
717. **serialization_bd_list_edge_nested.k**:
```k
_bd ((1;2);(3;4))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
718. **serialization_bd_list_edge_dicts.k**:
```k
_bd (.,(`a;1);.,(`b;2))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
719. **serialization_bd_anonymousfunction_random_3.k**:
```k
_bd {[xyz] xy|3}
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
720. **serialization_bd_floatvector_random_1.k**:
```k
_bd 196825.27335627712 -326371.90292283031 -214498.92985862558 11655.819143220946
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
721. **serialization_bd_floatvector_random_2.k**:
```k
_bd 148812.33236087282
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
722. **serialization_bd_floatvector_random_3.k**:
```k
_bd 585267.57816312299 -569668.94176055992 37200.312770306708 397004.01885714347
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
723. **serialization_bd_symbolvector_random_1.k**:
```k
_bd `qzUM7 `g8X6P `"iay" `KgNQ5i `"< +" `b5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
724. **serialization_bd_symbolvector_random_2.k**:
```k
_bd `"O 0" `D `qCBI1b `"*H " `"SS" `ULsyI `"F~" `"C" `Mont `O25B
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
725. **serialization_bd_symbolvector_random_3.k**:
```k
_bd `o3 `EE5ijP `trD0LuE `OW `"." `y
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
726. **test_quoted_symbol.k**:
```k
`"."
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
727. **test_quoted_symbol_serialization.k**:
```k
_bd `"."
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
728. **test_simple_symbol.k**:
```k
`a `"." `b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
729. **test_single_quoted_symbol.k**:
```k
`"."
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
730. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
731. **serialization_bd_dictionary_with_symbol_vectors.k**:
```k
_bd .((`colA;`a `b `c);(`colB;`dd `eee `ffff))
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
732. **serialization_bd_dictionary_with_vectors.k**:
```k
_bd .((`col1;1 2 3 4);(`col2;5 6 7 8))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
733. **serialization_bd_list_with_explicit_nulls.k**:
```k
_bd ((`a;`"1";);(`b;`"2";))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
734. **serialization_bd_list_with_vectors.k**:
```k
_bd ((`col1;1 2 3 4);(`col2;5 6 7 8))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
735. **serialization_bd_list_with_symbol_vectors.k**:
```k
_bd ((`colA;`a `b `c);(`colB;`dd `eee `ffff))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
736. **bd_dict_single_entry.k**:
```k
_bd .,(`a;1)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
737. **db_dict_larger.k**:
```k
_db _bd .((`a;1);(`b;2;);(`c;3;);(`d;4;))
```
Incomplete consumption (position 31/32) (consumed 31/32)
-------------------------------------------------
738. **db_dict_mixed_types.k**:
```k
_db _bd .((`key1;`value1;);(`key2;42;);(`key3;3.14))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
739. **db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
740. **db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
741. **db_nested_structures.k**:
```k
_db _bd .((`a;(1;2;3));(`b;(4;5;6)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
742. **db_string_hello.k**:
```k
_db _bd "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
743. **db_symbol_hello.k**:
```k
_db _bd `hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
744. **db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
745. **test_dict_larger.k**:
```k
.((`a;1;);(`b;2;);(`c;3;);(`d;4;))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
746. **test_dict_simple.k**:
```k
.((`a;1);(`b;2);(`c;3;))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
747. **symbol_special_chars.k**:
```k
`"hello-world!"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
748. **type_empty_int_vector.k**:
```k
4: !0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
749. **bd_empty_list.k**:
```k
_bd ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
750. **bd_enlist_single_int.k**:
```k
_bd ,5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
751. **bd_enlist_single_string.k**:
```k
_bd ,"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
752. **bd_symbol_vector_longer.k**:
```k
_bd `hello`world`test
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
753. **bd_enlist_single_symbol.k**:
```k
_bd ,`test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
754. **math_and_basic.k**:
```k
5 _and 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
755. **math_and_vector.k**:
```k
(5 6 3) _and (1 2 3)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
756. **math_div_float.k**:
```k
7 _div 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
757. **math_div_integer.k**:
```k
7 _div 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
758. **math_div_vector.k**:
```k
(7 14 21) _div 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
759. **math_dot_basic.k**:
```k
1 2 3 _dot 4 5 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
760. **math_dot_matrix_matrix.k**:
```k
(1 2 3;4 5 6) _dot (7 8 9;10 11 12)
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
761. **math_dot_matrix_2x2.k**:
```k
(1 2;3 4) _dot (5 6;7 8)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
762. **math_dot_vector_each_left.k**:
```k
(1 2) _dot\: (3 4;5 6)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
763. **adverb_complex_vector_each_right.k**:
```k
10 11 12 +/: ((1 2 3);(4 5 6);(7 8 9))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
764. **adverb_complex_matrix_each_right.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) +/: 10 11 12
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
765. **adverb_chaining_join_each_left.k**:
```k
((1 2 3);(4 5 6);(7 8 9)),/:\:((9 8 7);(6 5 4);(3 2 1))
```
Incomplete consumption (position 41/42) (consumed 41/42)
-------------------------------------------------
766. **adverb_complex_string_each_left.k**:
```k
(("hello";"world.");("It's";"me";"ksharp.");("Have";"fun";"with";"me!")),/:\:"  "
```
Incomplete consumption (position 29/30) (consumed 29/30)
-------------------------------------------------
767. **join_each_left.k**:
```k
(1 2 3),\: (4 5 6)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
768. **test_nested_adverb.k**:
```k
1 2 3 ,/:\: 4 5 6
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
769. **math_lsq_non_square.k**:
```k
(7 8 9) _lsq (1 2 3;4 5 6)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
770. **math_lsq_high_rank.k**:
```k
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
771. **math_lsq_complex.k**:
```k
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
772. **math_lsq_regression.k**:
```k
(1 2 3.0) _lsq (1 1 1.0;1 2 4.0)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
773. **math_mul_basic.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
774. **math_not_basic.k**:
```k
_not 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
775. **math_not_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
776. **math_or_basic.k**:
```k
5 _or 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
777. **math_or_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
778. **math_rot_basic.k**:
```k
8 _rot 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
779. **math_shift_basic.k**:
```k
8 _shift 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
780. **math_shift_vector.k**:
```k
(8 16 32 64) _shift 2
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
781. **math_xor_basic.k**:
```k
5 _xor 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
782. **math_xor_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
783. **ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
Incomplete consumption (position 5/8) (consumed 5/8)
-------------------------------------------------
784. **ffi_assembly_load.k**:
```k
"System.Private.CoreLib" 2: `System.String
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
785. **ffi_type_marshalling.k**:
```k
f:3.14159;f _sethint `float;s:"hello";s _sethint `string;l:1 2 3 4 5;l: _sethint `list
```
Incomplete consumption (position 3/29) (consumed 3/29)
-------------------------------------------------
786. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
Incomplete consumption (position 3/12) (consumed 3/12)
-------------------------------------------------
787. **ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
788. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
789. **ffi_constructor.k**:
```k
.((`real;c1[`Real]);(`imag;c1[`Imaginary]);(`instance;!c1);(`type;!complex))
```
Incomplete consumption (position 34/35) (consumed 34/35)
-------------------------------------------------
790. **ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
791. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
792. **ffi_dispose.k**:
```k
c1 @ `_this
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
793. **ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
794. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
795. **ffi_complete_workflow.k**:
```k
conj_func: ._dotnet.System.Numerics.Complex.Conjugate
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
796. **ffi_complete_workflow.k**:
```k
.((`real;c1[`Real]);(`imag;c1[`Imaginary]);(`magnitude;magnitude);(`instance;!c1);(`type;!complex))
```
Incomplete consumption (position 40/41) (consumed 40/41)
-------------------------------------------------
797. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
798. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
799. **idioms_01_575_kronecker_delta.k**:
```k
x=y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
800. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
801. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
802. **idioms_01_571_xbutnoty.k**:
```k
x>y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
803. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
804. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
805. **idioms_01_570_implies.k**:
```k
~x>y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
806. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
807. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
808. **idioms_01_573_exclusive_or.k**:
```k
~x=y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
809. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
810. **idioms_01_41_indices_ones.k**:
```k
&x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
811. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
812. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
813. **idioms_01_516_multiply_columns.k**:
```k
x*y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
814. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
815. **idioms_01_566_zero_boolean.k**:
```k
0&x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
816. **idioms_01_624_zero_array.k**:
```k
x:2 3#99
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
817. **idioms_01_624_zero_array.k**:
```k
x*0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
818. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
819. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
820. **idioms_01_622_retain_marked.k**:
```k
x*y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
821. **idioms_01_331_identity_max.k**:
```k
-1e100|-0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
822. **idioms_01_337_identity_min.k**:
```k
1e100&0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
823. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
824. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
825. **idioms_01_357_match.k**:
```k
x~y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
826. **idioms_01_328_number_items.k**:
```k
#"abcd"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
827. **idioms_01_411_number_rows.k**:
```k
#x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
828. **idioms_01_445_number_columns.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
829. **idioms_01_445_number_columns.k**:
```k
*|^x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
830. **test_eval_verb.k**:
```k
_eval ("+", 1, 2)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
831. **idioms_01_388_drop_rows.k**:
```k
x:6 3#!18
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
832. **idioms_01_388_drop_rows.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
833. **idioms_01_388_drop_rows.k**:
```k
y _ x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
834. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
835. **idioms_01_154_range.k**:
```k
?x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
836. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
837. **idioms_01_70_remove_duplicates.k**:
```k
?x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
838. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
839. **idioms_01_143_indices_distinct.k**:
```k
=x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
840. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
841. **idioms_01_228_is_row.k**:
```k
x?"yyy"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
842. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
843. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
844. **idioms_01_232_is_row_in.k**:
```k
y _in x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
845. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
846. **idioms_01_559_first_marker.k**:
```k
x?1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
847. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
848. **idioms_01_78_eval_number.k**:
```k
. x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
849. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
850. **idioms_01_88_name_variable.k**:
```k
y:2 3#!6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
851. **idioms_01_88_name_variable.k**:
```k
. "var",($x),":y"
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
852. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
853. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
854. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
855. **idioms_01_493_choose_boolean.k**:
```k
:[g;x;y]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
856. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
857. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
858. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
859. **idioms_01_406_add_last.k**:
```k
y:100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
860. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89)
```
Incomplete consumption (position 38/39) (consumed 38/39)
-------------------------------------------------
861. **idioms_01_449_limit_between.k**:
```k
l:30
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
862. **idioms_01_449_limit_between.k**:
```k
h:70
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
863. **idioms_01_449_limit_between.k**:
```k
l|h&x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
864. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
865. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
866. **idioms_01_495_indices_occurrences.k**:
```k
&x _lin y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
867. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
868. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
869. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
870. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
871. **idioms_01_569_change_to_one.k**:
```k
y^~x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
872. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
873. **idioms_01_556_all_indices.k**:
```k
!#x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
874. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
875. **idioms_01_535_avoid_parentheses.k**:
```k
|1,#x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
876. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
877. **idioms_01_591_reshape_2column.k**:
```k
((_ 0.5*#x),2)#x
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
878. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
879. **idioms_01_595_one_row_matrix.k**:
```k
,x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
880. **idioms_01_616_scalar_from_vector.k**:
```k
x:,8
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
881. **idioms_01_616_scalar_from_vector.k**:
```k
*x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
882. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
883. **idioms_01_509_remove_y.k**:
```k
x _dv y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
884. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
885. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
886. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
887. **idioms_01_496_remove_punctuation.k**:
```k
x _dvl y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
888. **idioms_01_177_string_search.k**:
```k
x:"st"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
889. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
890. **idioms_01_177_string_search.k**:
```k
y _ss x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
891. **idioms_01_45_binary_representation.k**:
```k
x:16
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
892. **idioms_01_45_binary_representation.k**:
```k
2 _vs x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
893. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
894. **idioms_01_84_scalar_boolean.k**:
```k
2 _sv x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
895. **idioms_01_129_arctangent.k**:
```k
y:1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
896. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
897. **idioms_01_561_numeric_code.k**:
```k
_ic[x]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
898. **idioms_01_241_sum_subsets.k**:
```k
x:1+3 4#!12
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
899. **idioms_01_241_sum_subsets.k**:
```k
y:4 3#1 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
900. **idioms_01_241_sum_subsets.k**:
```k
x _mul y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
901. **idioms_01_61_cyclic_counter.k**:
```k
x:!10
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
902. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
903. **idioms_01_61_cyclic_counter.k**:
```k
1+x!y
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
904. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
905. **idioms_01_384_drop_1st_postpend.k**:
```k
1 _ x,0
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
906. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
907. **idioms_01_385_drop_last_prepend.k**:
```k
-1 _ 0,x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
908. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
909. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
910. **idioms_01_178_first_occurrence.k**:
```k
*y _ss x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
911. **idioms_01_447_conditional_drop.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
912. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
913. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
914. **idioms_01_447_conditional_drop.k**:
```k
(y*g) _ x
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
915. **idioms_01_448_conditional_drop_last.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
916. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
917. **idioms_01_448_conditional_drop_last.k**:
```k
(-y) _ x
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
918. **ktree_enumerate_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
919. **ktree_enumerate_relative_name.k**:
```k
!d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
920. **ktree_enumerate_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
921. **ktree_enumerate_relative_path.k**:
```k
!`d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
922. **ktree_enumerate_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
923. **ktree_enumerate_absolute_path.k**:
```k
!(.k.d)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
924. **ktree_enumerate_root.k**:
```k
!`
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
925. **ktree_indexing_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
926. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
927. **ktree_indexing_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
928. **ktree_indexing_absolute_name.k**:
```k
.k.d[`keyA]
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
929. **ktree_indexing_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
930. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
931. **ktree_indexing_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
932. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
933. **ktree_dot_apply_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); d . `keyA
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
934. **ktree_dot_apply_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
935. **ktree_dot_apply_absolute_name.k**:
```k
.k.d . `keyB
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
936. **ktree_dot_apply_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `d . `keyA
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
937. **ktree_dot_apply_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `.k.d . `keyB
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
938. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
939. **eval_dot_execute_path.k**:
```k
v:`e`f
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
940. **eval_dot_execute_path.k**:
```k
_eval (`",";,`a`b`c;(`",";,`d;`v))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
941. **eval_dot_repl_dir.k**:
```k
."\\d ^";.d:_d;."\\d .k";(.d;_d)
```
Incomplete consumption (position 2/18) (consumed 2/18)
-------------------------------------------------
942. **eval_dot_parse_and_eval.k**:
```k
a:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
943. **eval_dot_parse_and_eval.k**:
```k
. "a+4"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
944. **test_eval_monadic_star_atomic.k**:
```k
_eval (`"*:";,1)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
