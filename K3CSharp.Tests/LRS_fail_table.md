# K3CSharp Parser Failures

**Generated:** 2026-03-30 14:15:49
**Test Results:** 820/852 passed (96.2%)

## Executive Summary

**Total Tests:** 852
**Passed Tests:** 820
**Failed Tests:** 32
**Success Rate:** 96.2%

**LRS Parser Statistics:**
- NULL Results: 14
- Incomplete Token Consumption: 966
- Total Fallbacks to Legacy: 980
- Incorrect Results: 0
- LRS Success Rate: -15.0%

**Top Failure Patterns:**
- Incomplete consumption (position 3/4): 264
- Incomplete consumption (position 2/3): 130
- Incomplete consumption (position 7/8): 77
- Incomplete consumption (position 5/6): 74
- Incomplete consumption (position 4/5): 61
- Incomplete consumption (position 6/7): 60
- Incomplete consumption (position 1/2): 39
- Incomplete consumption (position 9/10): 36
- Incomplete consumption (position 8/9): 25
- Incomplete consumption (position 10/11): 22

## LRS Parser Failures

### NULL Results (LRS returned NULL)

1. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
2. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
3. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
4. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
5. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
6. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
7. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
8. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
9. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
10. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
11. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
12. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
13. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
14. **test_eval_monadic_star.k**:
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
272. **square_bracket_function.k**:
```k
div[8;4]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
273. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
274. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
275. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
276. **square_bracket_vector_single.k**:
```k
v[4]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
277. **string_representation_int.k**:
```k
5:42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
278. **string_representation_symbol.k**:
```k
5:`symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
279. **string_representation_vector.k**:
```k
5:(1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
280. **symbol_quoted.k**:
```k
`"a symbol"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
281. **symbol_simple.k**:
```k
`foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
282. **symbol_vector_compact.k**:
```k
`a`b`c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
283. **symbol_vector_spaces.k**:
```k
`a `b `c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
284. **symbol_period_foo.k**:
```k
`foo.
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
285. **symbol_period_foobar.k**:
```k
`foo.bar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
286. **symbol_period_dotbar.k**:
```k
`.bar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
287. **symbol_period_dotk.k**:
```k
`.k
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
288. **io_read_int.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\int.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
289. **io_read_float.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
290. **io_read_symbol.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\symbol.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
291. **io_read_intvec.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
292. **io_read_debug.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
293. **io_write_int.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_write.l" 1: 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
294. **io_roundtrip.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l" 1: (1;2.5;"hello")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
295. **io_roundtrip.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
296. **io_monadic_1_int_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
297. **io_monadic_1_float_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
298. **io_monadic_1_char_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
299. **io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
300. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
301. **io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
302. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
303. **io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
304. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
305. **io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
306. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
307. **io_monadic_1_vs_2_int_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
308. **io_monadic_1_vs_2_float_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
309. **io_monadic_1_vs_2_char_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
310. **io_monadic_1_symbol_fallback.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\symbol.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
311. **match_simple_vectors.k**:
```k
5 6 7 ~ 5 6 7
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
312. **take_operator_basic.k**:
```k
3#1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
313. **take_operator_empty_float.k**:
```k
0#0.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
314. **take_operator_empty_symbol.k**:
```k
0#``
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
315. **take_operator_overflow.k**:
```k
10#1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
316. **take_operator_scalar.k**:
```k
3#42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
317. **reshape_basic.k**:
```k
3 4 # !12
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
318. **division_float_4_2.0.k**:
```k
4 % 2.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
319. **division_float_5_2.5.k**:
```k
5 % 2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
320. **division_int_4_2.k**:
```k
4 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
321. **division_int_5_2.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
322. **division_rules_10_3.k**:
```k
10 % 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
323. **division_rules_12_4.k**:
```k
12 % 4
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
324. **division_rules_4_2.k**:
```k
4 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
325. **division_rules_5_2.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
326. **enumerate.k**:
```k
! 2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
327. **grade_down_no_parens.k**:
```k
> 3 11 9 9 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
328. **grade_up_no_parens.k**:
```k
< 3 11 9 9 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
329. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
330. **multiline_function_single.k**:
```k
test . 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
331. **null_vector.k**:
```k
(;1;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
332. **scoping_single.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
333. **scoping_single.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
334. **scoping_single.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
335. **semicolon_simple.k**:
```k
(3 + 4; 5 + 6; -20.45)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
336. **semicolon_vars.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
337. **semicolon_vars.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
338. **semicolon_vars.k**:
```k
(a + b; a * b; a - b)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
339. **semicolon_vector.k**:
```k
a: 1 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
340. **semicolon_vector.k**:
```k
b: 3 4
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
341. **semicolon_vector.k**:
```k
(3 + 4; b; -20.45)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
342. **test_semicolon.k**:
```k
1 2; 3 4
```
Incomplete consumption (position 2/6) (consumed 2/6)
-------------------------------------------------
343. **simple_scalar_div.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
344. **single_no_semicolon.k**:
```k
(42)
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
345. **smart_division1.k**:
```k
5 10 % 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
346. **smart_division2.k**:
```k
4 8 % 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
347. **smart_division3.k**:
```k
6 12 18 % 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
348. **special_0i_plus_1.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
349. **special_0n_plus_1.k**:
```k
0N + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
350. **special_1_plus_neg0i.k**:
```k
1 + -0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
351. **special_neg0i_plus_1.k**:
```k
-0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
352. **special_underflow.k**:
```k
-0I - 27
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
353. **special_underflow_2.k**:
```k
-0I - 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
354. **special_underflow_3.k**:
```k
-0I - 1000
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
355. **type_float.k**:
```k
4: 3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
356. **type_null.k**:
```k
4: _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
357. **type_symbol.k**:
```k
4: `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
358. **type_vector.k**:
```k
4: (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
359. **vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
360. **type_operator_float.k**:
```k
4: 3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
361. **type_operator_null.k**:
```k
4: _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
362. **type_operator_symbol.k**:
```k
4: `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
363. **type_operator_vector_char.k**:
```k
4: ("abc")
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
364. **type_operator_vector_float.k**:
```k
4: (1.0 2.0 3.0)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
365. **type_operator_vector_int.k**:
```k
4: (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
366. **type_operator_vector_symbol.k**:
```k
4: "abc"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
367. **type_promotion_float_int.k**:
```k
1.5 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
368. **type_promotion_float_long.k**:
```k
1.5 + 1j
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
369. **type_promotion_int_float.k**:
```k
2 + 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
370. **type_promotion_int_long.k**:
```k
2 + 1j
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
371. **type_promotion_long_float.k**:
```k
1j + 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
372. **type_promotion_long_int.k**:
```k
1j + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
373. **unary_minus_operator.k**:
```k
- 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
374. **unique_operator.k**:
```k
? (1 2 1 3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
375. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
376. **variable_assignment.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
377. **variable_reassignment.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
378. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
379. **variable_reassignment.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
380. **variable_scoping_global_access.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
381. **variable_scoping_global_access.k**:
```k
test1: {[x] globalVar + x}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
382. **variable_scoping_global_access.k**:
```k
result1: test1 . 50
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
383. **variable_scoping_global_access.k**:
```k
result1
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
384. **variable_scoping_global_assignment.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
385. **variable_scoping_global_assignment.k**:
```k
result5: test5 . 10
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
386. **variable_scoping_global_assignment.k**:
```k
result5
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
387. **variable_scoping_global_assignment.k**:
```k
globalVar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
388. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
389. **variable_scoping_global_unchanged.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
390. **variable_scoping_global_unchanged.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
391. **variable_scoping_global_unchanged.k**:
```k
globalVar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
392. **variable_scoping_local_hiding.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
393. **variable_scoping_local_hiding.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
394. **variable_scoping_local_hiding.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
395. **variable_scoping_nested_functions.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
396. **variable_scoping_nested_functions.k**:
```k
result4: outer . 20
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
397. **variable_scoping_nested_functions.k**:
```k
result4
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
398. **variable_usage.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
399. **variable_usage.k**:
```k
y:20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
400. **variable_usage.k**:
```k
z:x+y
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
401. **variable_usage.k**:
```k
z
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
402. **dot_execute.k**:
```k
."2+2"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
403. **dot_execute_context.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
404. **dot_execute_context.k**:
```k
."foo:8"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
405. **dot_execute_context.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
406. **dictionary_enumerate.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
407. **dictionary_enumerate.k**:
```k
!d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
408. **null_operations.k**:
```k
_n@7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
409. **dictionary_dot_apply.k**:
```k
d: .((`a;1;.());(`b;2;.()))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
410. **dictionary_dot_apply.k**:
```k
d@`a
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
411. **monadic_format_basic.k**:
```k
$1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
412. **monadic_format_types.k**:
```k
$42.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
413. **monadic_format_vector.k**:
```k
$1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
414. **monadic_format_string_hello.k**:
```k
$"hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
415. **monadic_format_symbol_hello.k**:
```k
$`hello
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
416. **monadic_format_symbol_simple.k**:
```k
$`test
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
417. **monadic_format_dictionary.k**:
```k
$.((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
418. **monadic_format_nested_list.k**:
```k
$((1;2;3);(4;5;6))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
419. **monadic_format_integer.k**:
```k
$42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
420. **monadic_format_float.k**:
```k
$3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
421. **monadic_format_vector_simple.k**:
```k
$1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
422. **format_integer.k**:
```k
0$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
423. **ci_adverb_vector.k**:
```k
_ci' 97 94 80
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
424. **adverb_each_count.k**:
```k
#:' (1 2 3;1 2 3 4 5; 1 2)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
425. **format_float_numeric.k**:
```k
0.0$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
426. **form_long.k**:
```k
0j$"42"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
427. **format_numeric.k**:
```k
5$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
428. **form_string_pad_left.k**:
```k
7$"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
429. **format_symbol_pad_left.k**:
```k
10$`hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
430. **format_symbol_pad_left_8.k**:
```k
8$`hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
431. **format_pad_left.k**:
```k
5$42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
432. **format_pad_right.k**:
```k
-5$42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
433. **format_float_width_precision.k**:
```k
10.2$3.14159
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
434. **format_float_precision.k**:
```k
8.2$3.14159
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
435. **format_0_1.k**:
```k
0$1.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
436. **format_1_1.k**:
```k
1$1.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
437. **format_symbol_string_mixed_vector.k**:
```k
`$("hello";"world";"test")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
438. **form_integer_charvector.k**:
```k
0$"42"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
439. **dot_execute_variables.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
440. **dot_execute_variables.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
441. **dot_execute_variables.k**:
```k
."a%b"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
442. **format_braces_expressions.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
443. **format_braces_expressions.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
444. **format_braces_nested_expr.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
445. **format_braces_nested_expr.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
446. **format_braces_nested_expr.k**:
```k
z:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
447. **format_braces_complex.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
448. **format_braces_complex.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
449. **format_braces_complex.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
450. **format_braces_string.k**:
```k
name:"John"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
451. **format_braces_string.k**:
```k
age:25
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
452. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
Incomplete consumption (position 3/27) (consumed 3/27)
-------------------------------------------------
453. **format_braces_simple.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
454. **format_braces_simple.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
455. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
Incomplete consumption (position 3/33) (consumed 3/33)
-------------------------------------------------
456. **format_braces_nested_arith.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
457. **format_braces_nested_arith.k**:
```k
b:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
458. **format_braces_nested_arith.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
459. **format_braces_float.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
460. **format_braces_float.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
461. **format_braces_float.k**:
```k
c:3.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
462. **format_braces_mixed_arith.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
463. **format_braces_mixed_arith.k**:
```k
y:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
464. **format_braces_mixed_arith.k**:
```k
z:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
465. **format_braces_example.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
466. **format_braces_example.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
467. **format_braces_function_calls.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
468. **format_braces_function_calls.k**:
```k
product:{[x;y] x*y}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
469. **format_braces_function_calls.k**:
```k
double:{[x] x*2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
470. **format_braces_nested_function_calls.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
471. **format_braces_nested_function_calls.k**:
```k
double:{[x] x*2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
472. **format_braces_nested_function_calls.k**:
```k
square:{[x] x*x}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
473. **log.k**:
```k
_log 10
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
474. **time_t.k**:
```k
.((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
475. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 5/23) (consumed 5/23)
-------------------------------------------------
476. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
Incomplete consumption (position 5/36) (consumed 5/36)
-------------------------------------------------
477. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 5/23) (consumed 5/23)
-------------------------------------------------
478. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 6/24) (consumed 6/24)
-------------------------------------------------
479. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
Incomplete consumption (position 6/37) (consumed 6/37)
-------------------------------------------------
480. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 6/24) (consumed 6/24)
-------------------------------------------------
481. **time_gtime.k**:
```k
_gtime 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
482. **time_lt.k**:
```k
_lt 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
483. **time_jd.k**:
```k
_jd 20260206
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
484. **time_dj.k**:
```k
_dj 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
485. **time_ltime.k**:
```k
_ltime 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
486. **in.k**:
```k
4 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
487. **assignment_lrs_return_value.k**:
```k
b:2*a:47;(a;b)
```
Incomplete consumption (position 7/14) (consumed 7/14)
-------------------------------------------------
488. **list_dv_basic.k**:
```k
3 4 4 5 _dv 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
489. **list_dv_nomatch.k**:
```k
3 4 4 5 _dv 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
490. **list_di_basic.k**:
```k
3 2 4 5 _di 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
491. **list_di_multiple.k**:
```k
3 2 4 5 _di 1 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
492. **list_sv_base10.k**:
```k
10 _sv 1 9 9 5
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
493. **list_sv_base2.k**:
```k
2 _sv 1 0 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
494. **list_sv_mixed.k**:
```k
10 10 10 10 _sv 1 9 9 5
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
495. **list_getenv.k**:
```k
_getenv "PROMPT"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
496. **list_setenv.k**:
```k
`TESTVAR _setenv "hello world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
497. **list_size_existing.k**:
```k
_size "C:\\Windows\\System32\\write.exe"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
498. **test_ci_basic.k**:
```k
_ci 65
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
499. **test_ci_vector.k**:
```k
_ci 65 66 67
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
500. **test_vs_dyadic.k**:
```k
10 _vs 1995
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
501. **test_ic_vector.k**:
```k
_ic "ABC"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
502. **test_monadic_colon.k**:
```k
: 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
503. **test_sm_basic.k**:
```k
`foo _sm `foo
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
504. **test_sm_simple.k**:
```k
`a _sm `a
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
505. **test_ss_basic.k**:
```k
"hello world" _ss "world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
506. **statement_assignment_basic.k**:
```k
a: 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
507. **statement_assignment_inline.k**:
```k
1 + a: 42
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
508. **statement_conditional_basic.k**:
```k
:[1;2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
509. **statement_do_basic.k**:
```k
i:0;do[3;i+:1];i
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
510. **statement_do_simple.k**:
```k
i:0;do[3;i+:1]
```
Incomplete consumption (position 3/13) (consumed 3/13)
-------------------------------------------------
511. **semicolon_vars_test.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
512. **apply_and_assign_simple.k**:
```k
i:0;i+:1;i
```
Incomplete consumption (position 3/10) (consumed 3/10)
-------------------------------------------------
513. **apply_and_assign_multiline.k**:
```k
i:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
514. **apply_and_assign_multiline.k**:
```k
i+:1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
515. **apply_and_assign_multiline.k**:
```k
i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
516. **io_read_basic.k**:
```k
0:`test
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
517. **io_write_basic.k**:
```k
`testW 0: ("line1";"line2";"line3");0:`testW
```
Incomplete consumption (position 9/13) (consumed 9/13)
-------------------------------------------------
518. **io_append_simple.k**:
```k
`testfile 5: "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
519. **io_append_basic.k**:
```k
`test 5: "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
520. **io_append_multiple.k**:
```k
`test 5: "hello"; `test 5: "world"; `test 5: 1 2 3
```
Incomplete consumption (position 3/14) (consumed 3/14)
-------------------------------------------------
521. **io_read_bytes_basic.k**:
```k
`test 0:,"hello";6:`test
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
522. **io_read_bytes_empty.k**:
```k
`empty 0:(); 6:`empty
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
523. **io_write_bytes_basic.k**:
```k
`test 6:,"ABC"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
524. **io_write_bytes_overwrite.k**:
```k
`test 6:,"ABC"; `test 6:,"XYZ"
```
Incomplete consumption (position 4/10) (consumed 4/10)
-------------------------------------------------
525. **io_write_bytes_binary.k**:
```k
`test 6:(0 1 2 255)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
526. **search_in_basic.k**:
```k
4 _in 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
527. **search_in_notfound.k**:
```k
6 _in 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
528. **search_bin_basic.k**:
```k
3 4 5 6 _bin 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
529. **search_binl_eachleft.k**:
```k
1 3 5 _binl 1 2 3 4 5
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
530. **search_lin_intersection.k**:
```k
1 3 5 7 9 _lin 1 2 3 4 5
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
531. **amend_test.k**:
```k
(.) . (((1 2 3 4 5);(6 7 8 9 10)); 0 2; +; 10)
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
532. **find_basic.k**:
```k
9 8 7 6 5 4 3 ? 7
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
533. **find_notfound.k**:
```k
9 8 7 6 5 4 3 ? 1
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
534. **format_float_precision_vector_simple.k**:
```k
10.1$(1.5;2.5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
535. **format_float_precision_mixed_vector.k**:
```k
7.2$(1.5;2.7;3.14159;4.2)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
536. **format_pad_mixed_vector.k**:
```k
10$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
537. **format_pad_negative_mixed_vector.k**:
```k
-10$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
538. **vector_notation_empty.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
539. **vector_notation_functions.k**:
```k
double: {[x] x * 2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
540. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
541. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
542. **vector_notation_nested.k**:
```k
((1 + 2); (3 + 4); (5 + 6))
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
543. **vector_notation_semicolon.k**:
```k
(3 + 4; 5 + 6; -20.45)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
544. **vector_notation_single_group.k**:
```k
(42)
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
545. **vector_notation_space.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
546. **vector_notation_variables.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
547. **vector_notation_variables.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
548. **vector_notation_variables.k**:
```k
(a + b; a * b; a - b)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
549. **vector_addition.k**:
```k
1 2 + 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
550. **vector_division.k**:
```k
1 2 % 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
551. **vector_index_duplicate.k**:
```k
5 8 4 9 @ 0 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
552. **vector_index_first.k**:
```k
5 8 4 9 @ 0
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
553. **vector_index_multiple.k**:
```k
5 8 4 9 @ 1 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
554. **vector_index_reverse.k**:
```k
5 8 4 9 @ 3 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
555. **vector_index_single.k**:
```k
5 8 4 9 @ 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
556. **vector_multiplication.k**:
```k
1 2 * 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
557. **vector_subtraction.k**:
```k
1 2 - 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
558. **vector_with_null.k**:
```k
(_n;1;2)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
559. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
560. **where_operator.k**:
```k
& (1 0 1 1 0)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
561. **where_vector_counts.k**:
```k
& (3 2 1)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
562. **floor_operator.k**:
```k
_3.7
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
563. **adverb_backslash_colon_basic.k**:
```k
1 2 3 +\: 4 5 6
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
564. **adverb_slash_colon_basic.k**:
```k
1 2 3 +/: 4 5 6
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
565. **adverb_tick_colon_basic.k**:
```k
-': 4 8 9 12 20
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
566. **amend_apply.k**:
```k
(.) . (((1 2 3 4 5);(6 7 8 9 10)); 0 2; +; 10)
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
567. **amend_parenthesized.k**:
```k
(.) . (1 2 3; 0; +; 10)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
568. **amend_test_anonymous_func.k**:
```k
f:{x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
569. **amend_test_anonymous_func.k**:
```k
(.).((1 2 3); 0; f; 10)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
570. **amend_test_func_var.k**:
```k
f:{x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
571. **amend_test_func_var.k**:
```k
(.).((1 2 3); 0; f; 10)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
572. **conditional_bracket_test.k**:
```k
:[1 < 2; "true"; "false"]
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
573. **conditional_false.k**:
```k
:[0; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
574. **conditional_simple_test.k**:
```k
:[1; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
575. **conditional_true.k**:
```k
:[1; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
576. **dictionary_null_index.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
577. **dictionary_null_index.k**:
```k
d@_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
578. **dictionary_unmake.k**:
```k
d: .((`a;1);(`b;2)); result:. d; result
```
Incomplete consumption (position 16/24) (consumed 16/24)
-------------------------------------------------
579. **do_loop.k**:
```k
i: 0; do[3; i+: 1]
```
Incomplete consumption (position 3/13) (consumed 3/13)
-------------------------------------------------
580. **dyadic_divide_bracket.k**:
```k
%[20; 4]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
581. **dyadic_minus_bracket.k**:
```k
-[10; 3]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
582. **dyadic_multiply_bracket.k**:
```k
*[4; 6]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
583. **dyadic_plus_bracket.k**:
```k
+[3; 5]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
584. **dyadic_divide_dot_apply.k**:
```k
(%) . (20; 4)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
585. **dyadic_minus_dot_apply.k**:
```k
(-) . (10; 3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
586. **dyadic_multiply_dot_apply.k**:
```k
(*) . (4; 6)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
587. **dyadic_plus_dot_apply.k**:
```k
(+) . (3; 5)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
588. **empty_brackets_dictionary.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
589. **empty_brackets_dictionary.k**:
```k
d[]
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
590. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
591. **empty_brackets_vector.k**:
```k
v[]
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
592. **format_braces_complex_expressions.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
593. **format_braces_complex_expressions.k**:
```k
product:{[x;y] x*y}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
594. **format_float_precision_complex_mixed.k**:
```k
10.3$(1.234;2.567;3.890;4.123)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
595. **format_float_vector.k**:
```k
0.0$(1;2.5;3.14;42)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
596. **format_int_vector.k**:
```k
0$(1;2.5;3.14;42)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
597. **form_0_string.k**:
```k
0$"123"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
598. **form_0_vector.k**:
```k
0$("123";"456")
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
599. **form_0_float_string.k**:
```k
0.0$"3.14"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
600. **form_0_float_vector.k**:
```k
0.0$("3.14";"1e48";"1.4e-27")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
601. **form_symbol_string.k**:
```k
`$"abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
602. **form_symbol_vector.k**:
```k
`$("abc";"de";"f")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
603. **format_string_pad_left.k**:
```k
10$"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
604. **format_string_pad_right.k**:
```k
-10$"test"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
605. **format_vector_int.k**:
```k
0$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
606. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
Incomplete consumption (position 22/23) (consumed 22/23)
-------------------------------------------------
607. **group_operator.k**:
```k
=a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
608. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
609. **in_basic.k**:
```k
4 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
610. **in_notfound.k**:
```k
10 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
611. **in_simple.k**:
```k
5 _in 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
612. **isolated.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
613. **isolated.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
614. **modulo.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
615. **modulo.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
616. **modulo.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
617. **monadic_format_mixed_vector.k**:
```k
$(1;2.5;"hello";`symbol)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
618. **over_plus_empty.k**:
```k
+/!0
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
619. **simple_division.k**:
```k
8 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
620. **simple_subtraction.k**:
```k
5 - 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
621. **string_parse.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
622. **string_parse.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
623. **string_parse.k**:
```k
."a+b"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
624. **k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
625. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
626. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
627. **k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
628. **k_tree_retrieval_relative.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
629. **k_tree_enumerate.k**:
```k
!`
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
630. **k_tree_current_branch.k**:
```k
_d
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
631. **k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
632. **k_tree_dictionary_indexing.k**:
```k
.k[`foo]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
633. **k_tree_nested_indexing.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
634. **k_tree_nested_indexing.k**:
```k
.k.dd[`b]
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
635. **k_tree_verify_root.k**:
```k
.k
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
636. **k_tree_flip_dictionary.k**:
```k
.+(`a`b`c;1 2 3)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
637. **k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
638. **k_tree_null_to_dict_conversion.k**:
```k
.k
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
639. **k_tree_dictionary_assignment.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
640. **k_tree_dictionary_assignment.k**:
```k
.k.dd
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
641. **k_tree_test_bracket_indexing.k**:
```k
d: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 22/23) (consumed 22/23)
-------------------------------------------------
642. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
643. **k_tree_flip_test.k**:
```k
+(`a`b`c;1 2 3)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
644. **vector_null_index.k**:
```k
v: 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
645. **vector_null_index.k**:
```k
v@_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
646. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
647. **while_safe_test.k**:
```k
i: 0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
648. **serialization_bd_db_integer.k**:
```k
_bd 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
649. **serialization_bd_db_float.k**:
```k
_bd 3.14159
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
650. **serialization_bd_db_symbol.k**:
```k
_bd `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
651. **serialization_bd_db_null.k**:
```k
_bd _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
652. **serialization_bd_db_integervector.k**:
```k
_bd 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
653. **serialization_bd_db_floatvector.k**:
```k
_bd 1.1 2.2 3.3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
654. **serialization_bd_db_charactervector.k**:
```k
_bd "hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
655. **serialization_bd_db_symbolvector.k**:
```k
_bd `a`b`c
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
656. **serialization_bd_db_dictionary.k**:
```k
_bd .((`a;`"1");(`b;`"2"))
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
657. **serialization_bd_db_anonymousfunction.k**:
```k
_bd {[x] x+1}
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
658. **serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
659. **serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
660. **db_basic_integer.k**:
```k
_db _bd 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
661. **db_float.k**:
```k
_db _bd 3.14
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
662. **db_symbol.k**:
```k
_db _bd `test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
663. **db_int_vector.k**:
```k
_db _bd 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
664. **db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
665. **db_char_vector.k**:
```k
_db _bd "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
666. **db_list_simple.k**:
```k
_db _bd (1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
667. **db_dict_simple.k**:
```k
_db _bd .((`a;1);(`b;2);(`c;3;))
```
Incomplete consumption (position 23/24) (consumed 23/24)
-------------------------------------------------
668. **db_function_simple.k**:
```k
_db _bd {+}
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
669. **db_function_params.k**:
```k
_db _bd {x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
670. **db_null.k**:
```k
_db _bd 0N
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
671. **db_empty_list.k**:
```k
_db _bd ()
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
672. **db_float_simple.k**:
```k
_db _bd 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
673. **db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
674. **db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
675. **db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
676. **db_symbol_simple.k**:
```k
_db _bd `hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
677. **db_list_longer.k**:
```k
_db _bd (1;2;3;4;5)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
678. **db_list_mixed_types.k**:
```k
_db _bd (1;`test;3.14;"hello")
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
679. **db_function_complex.k**:
```k
_db _bd {[x;y] x*y+z}
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
680. **db_function_simple_math.k**:
```k
_db _bd {x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
681. **db_nested_dict_vectors.k**:
```k
_db _bd .((`a;(1;2;3));(`b;(`hello;`world;`test)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
682. **db_nested_lists.k**:
```k
_db _bd ((1;2;3);(`hello;`world;`test);(4.5;6.7))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
683. **db_mixed_list.k**:
```k
_db _bd (1;`test;3.14)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
684. **db_dict_single_entry.k**:
```k
_db _bd .,(`a;1)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
685. **db_dict_symbol_2char.k**:
```k
_db _bd .,(`ab;1 2)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
686. **db_dict_symbol_8char.k**:
```k
_db _bd .,(`abcdefgh;1 2 3 4 5 6 7 8)
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
687. **db_dict_multi_entry.k**:
```k
_db _bd .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
688. **db_dict_five_entries.k**:
```k
_db _bd .((`a;1);(`b;2);(`c;3);(`d;4);(`e;5))
```
Incomplete consumption (position 34/35) (consumed 34/35)
-------------------------------------------------
689. **db_dict_complex_attributes.k**:
```k
_db _bd .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))))
```
Incomplete consumption (position 88/89) (consumed 88/89)
-------------------------------------------------
690. **db_dict_empty.k**:
```k
_db _bd .()
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
691. **db_dict_with_null_attrs.k**:
```k
_db _bd .,(`a;1;.())
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
692. **db_dict_with_empty_attrs.k**:
```k
_db _bd .((`a;1);(`b;2;.()))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
693. **db_enlist_single_int.k**:
```k
_db _bd ,5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
694. **db_enlist_single_symbol.k**:
```k
_db _bd ,`test
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
695. **db_enlist_single_string.k**:
```k
_db _bd ,"hello"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
696. **db_enlist_vector.k**:
```k
_db _bd ,(1 2 3)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
697. **serialization_bd_null_edge_0.k**:
```k
_bd _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
698. **serialization_bd_integer_edge_0.k**:
```k
_bd 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
699. **serialization_bd_integer_edge_1.k**:
```k
_bd 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
700. **serialization_bd_integer_edge_-1.k**:
```k
_bd -1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
701. **serialization_bd_integer_edge_2147483647.k**:
```k
_bd 2147483647
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
702. **serialization_bd_integer_edge_-2147483648.k**:
```k
_bd -2147483648
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
703. **serialization_bd_integer_edge_0N.k**:
```k
_bd 0N
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
704. **serialization_bd_integer_edge_0I.k**:
```k
_bd 0I
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
705. **serialization_bd_integer_edge_-0I.k**:
```k
_bd -0I
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
706. **serialization_bd_float_edge_0.0.k**:
```k
_bd 0.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
707. **serialization_bd_float_edge_1.0.k**:
```k
_bd 1.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
708. **serialization_bd_float_edge_-1.0.k**:
```k
_bd -1.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
709. **serialization_bd_float_edge_0.5.k**:
```k
_bd 0.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
710. **serialization_bd_float_edge_-0.5.k**:
```k
_bd -0.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
711. **serialization_bd_float_edge_0n.k**:
```k
_bd 0n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
712. **serialization_bd_float_edge_0i.k**:
```k
_bd 0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
713. **serialization_bd_float_edge_-0i.k**:
```k
_bd -0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
714. **serialization_bd_symbol_edge_a.k**:
```k
_bd `a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
715. **serialization_bd_symbol_edge_symbol.k**:
```k
_bd `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
716. **serialization_bd_symbol_edge_test123.k**:
```k
_bd `test123
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
717. **serialization_bd_symbol_edge_underscore.k**:
```k
_bd `_underscore
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
718. **serialization_bd_symbol_edge_hello.k**:
```k
_bd `"hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
719. **serialization_bd_symbol_edge_newline_tab.k**:
```k
_bd `"\n\t"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
720. **serialization_bd_symbol_edge_001.k**:
```k
_bd `"\001"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
721. **serialization_bd_symbol_edge_empty.k**:
```k
_bd `
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
722. **serialization_bd_charactervector_edge_empty.k**:
```k
_bd ""
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
723. **serialization_bd_charactervector_edge_hello.k**:
```k
_bd "hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
724. **serialization_bd_charactervector_edge_whitespace.k**:
```k
_bd "\n\t\r"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
725. **serialization_bd_integervector_edge_empty.k**:
```k
_bd !0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
726. **serialization_bd_integervector_edge_single.k**:
```k
_bd ,1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
727. **serialization_bd_integervector_edge_123.k**:
```k
_bd 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
728. **serialization_bd_integervector_edge_special.k**:
```k
_bd 0N 0I -0I
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
729. **serialization_bd_list_edge_empty.k**:
```k
_bd ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
730. **serialization_bd_list_edge_null.k**:
```k
_bd ,_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
731. **serialization_bd_list_edge_complex.k**:
```k
_bd (_n;`symbol;{[]})
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
732. **serialization_bd_list_edge_nested.k**:
```k
_bd ((1;2);(3;4))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
733. **serialization_bd_list_edge_dicts.k**:
```k
_bd (.,(`a;1);.,(`b;2))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
734. **serialization_bd_anonymousfunction_random_3.k**:
```k
_bd {[xyz] xy|3}
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
735. **serialization_bd_floatvector_random_1.k**:
```k
_bd 196825.27335627712 -326371.90292283031 -214498.92985862558 11655.819143220946
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
736. **serialization_bd_floatvector_random_2.k**:
```k
_bd 148812.33236087282
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
737. **serialization_bd_floatvector_random_3.k**:
```k
_bd 585267.57816312299 -569668.94176055992 37200.312770306708 397004.01885714347
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
738. **serialization_bd_symbolvector_random_1.k**:
```k
_bd `qzUM7 `g8X6P `"iay" `KgNQ5i `"< +" `b5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
739. **serialization_bd_symbolvector_random_2.k**:
```k
_bd `"O 0" `D `qCBI1b `"*H " `"SS" `ULsyI `"F~" `"C" `Mont `O25B
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
740. **serialization_bd_symbolvector_random_3.k**:
```k
_bd `o3 `EE5ijP `trD0LuE `OW `"." `y
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
741. **test_quoted_symbol.k**:
```k
`"."
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
742. **test_quoted_symbol_serialization.k**:
```k
_bd `"."
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
743. **test_simple_symbol.k**:
```k
`a `"." `b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
744. **test_single_quoted_symbol.k**:
```k
`"."
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
745. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
746. **serialization_bd_dictionary_with_symbol_vectors.k**:
```k
_bd .((`colA;`a `b `c);(`colB;`dd `eee `ffff))
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
747. **serialization_bd_dictionary_with_vectors.k**:
```k
_bd .((`col1;1 2 3 4);(`col2;5 6 7 8))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
748. **serialization_bd_list_with_explicit_nulls.k**:
```k
_bd ((`a;`"1";);(`b;`"2";))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
749. **serialization_bd_list_with_vectors.k**:
```k
_bd ((`col1;1 2 3 4);(`col2;5 6 7 8))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
750. **serialization_bd_list_with_symbol_vectors.k**:
```k
_bd ((`colA;`a `b `c);(`colB;`dd `eee `ffff))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
751. **bd_dict_single_entry.k**:
```k
_bd .,(`a;1)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
752. **db_dict_larger.k**:
```k
_db _bd .((`a;1);(`b;2;);(`c;3;);(`d;4;))
```
Incomplete consumption (position 31/32) (consumed 31/32)
-------------------------------------------------
753. **db_dict_mixed_types.k**:
```k
_db _bd .((`key1;`value1;);(`key2;42;);(`key3;3.14))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
754. **db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
755. **db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
756. **db_nested_structures.k**:
```k
_db _bd .((`a;(1;2;3));(`b;(4;5;6)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
757. **db_string_hello.k**:
```k
_db _bd "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
758. **db_symbol_hello.k**:
```k
_db _bd `hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
759. **db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
760. **test_dict_larger.k**:
```k
.((`a;1;);(`b;2;);(`c;3;);(`d;4;))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
761. **test_dict_simple.k**:
```k
.((`a;1);(`b;2);(`c;3;))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
762. **symbol_special_chars.k**:
```k
`"hello-world!"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
763. **type_empty_int_vector.k**:
```k
4: !0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
764. **bd_empty_list.k**:
```k
_bd ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
765. **bd_enlist_single_int.k**:
```k
_bd ,5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
766. **bd_enlist_single_string.k**:
```k
_bd ,"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
767. **bd_symbol_vector_longer.k**:
```k
_bd `hello`world`test
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
768. **bd_enlist_single_symbol.k**:
```k
_bd ,`test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
769. **math_and_basic.k**:
```k
5 _and 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
770. **math_and_vector.k**:
```k
(5 6 3) _and (1 2 3)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
771. **math_ceil_basic.k**:
```k
_ceil 4.7
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
772. **math_ceil_integer.k**:
```k
_ceil 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
773. **math_ceil_negative.k**:
```k
_ceil -3.2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
774. **math_ceil_vector.k**:
```k
_ceil 1.2 2.7 3.5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
775. **math_div_float.k**:
```k
7 _div 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
776. **math_div_integer.k**:
```k
7 _div 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
777. **math_div_vector.k**:
```k
(7 14 21) _div 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
778. **math_dot_basic.k**:
```k
1 2 3 _dot 4 5 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
779. **math_dot_matrix_matrix.k**:
```k
(1 2 3;4 5 6) _dot (7 8 9;10 11 12)
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
780. **math_dot_matrix_2x2.k**:
```k
(1 2;3 4) _dot (5 6;7 8)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
781. **math_dot_vector_each_left.k**:
```k
(1 2) _dot\: (3 4;5 6)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
782. **adverb_complex_vector_each_right.k**:
```k
10 11 12 +/: ((1 2 3);(4 5 6);(7 8 9))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
783. **adverb_complex_matrix_each_right.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) +/: 10 11 12
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
784. **adverb_chaining_join_each_left.k**:
```k
((1 2 3);(4 5 6);(7 8 9)),/:\:((9 8 7);(6 5 4);(3 2 1))
```
Incomplete consumption (position 41/42) (consumed 41/42)
-------------------------------------------------
785. **adverb_complex_string_each_left.k**:
```k
(("hello";"world.");("It's";"me";"ksharp.");("Have";"fun";"with";"me!")),/:\:"  "
```
Incomplete consumption (position 29/30) (consumed 29/30)
-------------------------------------------------
786. **join_each_left.k**:
```k
(1 2 3),\: (4 5 6)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
787. **test_nested_adverb.k**:
```k
1 2 3 ,/:\: 4 5 6
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
788. **math_lsq_non_square.k**:
```k
(7 8 9) _lsq (1 2 3;4 5 6)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
789. **math_lsq_high_rank.k**:
```k
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
790. **math_lsq_complex.k**:
```k
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
791. **math_lsq_regression.k**:
```k
(1 2 3.0) _lsq (1 1 1.0;1 2 4.0)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
792. **math_mul_basic.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
793. **math_not_basic.k**:
```k
_not 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
794. **math_not_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
795. **math_or_basic.k**:
```k
5 _or 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
796. **math_or_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
797. **math_rot_basic.k**:
```k
8 _rot 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
798. **math_shift_basic.k**:
```k
8 _shift 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
799. **math_shift_vector.k**:
```k
(8 16 32 64) _shift 2
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
800. **math_xor_basic.k**:
```k
5 _xor 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
801. **math_xor_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
802. **ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
Incomplete consumption (position 5/8) (consumed 5/8)
-------------------------------------------------
803. **ffi_assembly_load.k**:
```k
"System.Private.CoreLib" 2: `System.String
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
804. **ffi_type_marshalling.k**:
```k
f:3.14159;f _sethint `float;s:"hello";s _sethint `string;l:1 2 3 4 5;l: _sethint `list
```
Incomplete consumption (position 3/29) (consumed 3/29)
-------------------------------------------------
805. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
Incomplete consumption (position 3/12) (consumed 3/12)
-------------------------------------------------
806. **ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
807. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
808. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
809. **ffi_constructor.k**:
```k
.((`real;c1[`Real]);(`imag;c1[`Imaginary]);(`instance;!c1);(`type;!complex))
```
Incomplete consumption (position 34/35) (consumed 34/35)
-------------------------------------------------
810. **ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
811. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
812. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
813. **ffi_dispose.k**:
```k
c1 @ `_this
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
814. **ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
815. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
816. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
817. **ffi_complete_workflow.k**:
```k
conj_func: ._dotnet.System.Numerics.Complex.Conjugate
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
818. **ffi_complete_workflow.k**:
```k
.((`real;c1[`Real]);(`imag;c1[`Imaginary]);(`magnitude;magnitude);(`instance;!c1);(`type;!complex))
```
Incomplete consumption (position 40/41) (consumed 40/41)
-------------------------------------------------
819. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
820. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
821. **idioms_01_575_kronecker_delta.k**:
```k
x=y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
822. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
823. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
824. **idioms_01_571_xbutnoty.k**:
```k
x>y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
825. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
826. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
827. **idioms_01_570_implies.k**:
```k
~x>y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
828. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
829. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
830. **idioms_01_573_exclusive_or.k**:
```k
~x=y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
831. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
832. **idioms_01_41_indices_ones.k**:
```k
&x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
833. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
834. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
835. **idioms_01_516_multiply_columns.k**:
```k
x*y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
836. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
837. **idioms_01_566_zero_boolean.k**:
```k
0&x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
838. **idioms_01_624_zero_array.k**:
```k
x:2 3#99
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
839. **idioms_01_624_zero_array.k**:
```k
x*0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
840. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
841. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
842. **idioms_01_622_retain_marked.k**:
```k
x*y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
843. **idioms_01_331_identity_max.k**:
```k
-1e100|-0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
844. **idioms_01_337_identity_min.k**:
```k
1e100&0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
845. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
846. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
847. **idioms_01_357_match.k**:
```k
x~y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
848. **idioms_01_328_number_items.k**:
```k
#"abcd"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
849. **idioms_01_411_number_rows.k**:
```k
#x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
850. **idioms_01_445_number_columns.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
851. **idioms_01_445_number_columns.k**:
```k
*|^x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
852. **test_eval_verb.k**:
```k
_eval ("+", 1, 2)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
853. **idioms_01_388_drop_rows.k**:
```k
x:6 3#!18
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
854. **idioms_01_388_drop_rows.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
855. **idioms_01_388_drop_rows.k**:
```k
y _ x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
856. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
857. **idioms_01_154_range.k**:
```k
?x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
858. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
859. **idioms_01_70_remove_duplicates.k**:
```k
?x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
860. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
861. **idioms_01_143_indices_distinct.k**:
```k
=x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
862. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
863. **idioms_01_228_is_row.k**:
```k
x?"yyy"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
864. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
865. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
866. **idioms_01_232_is_row_in.k**:
```k
y _in x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
867. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
868. **idioms_01_559_first_marker.k**:
```k
x?1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
869. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
870. **idioms_01_78_eval_number.k**:
```k
. x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
871. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
872. **idioms_01_88_name_variable.k**:
```k
y:2 3#!6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
873. **idioms_01_88_name_variable.k**:
```k
. "var",($x),":y"
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
874. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
875. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
876. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
877. **idioms_01_493_choose_boolean.k**:
```k
:[g;x;y]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
878. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
879. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
880. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
881. **idioms_01_406_add_last.k**:
```k
y:100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
882. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89)
```
Incomplete consumption (position 38/39) (consumed 38/39)
-------------------------------------------------
883. **idioms_01_449_limit_between.k**:
```k
l:30
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
884. **idioms_01_449_limit_between.k**:
```k
h:70
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
885. **idioms_01_449_limit_between.k**:
```k
l|h&x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
886. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
887. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
888. **idioms_01_495_indices_occurrences.k**:
```k
&x _lin y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
889. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
890. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
891. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
892. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
893. **idioms_01_569_change_to_one.k**:
```k
y^~x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
894. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
895. **idioms_01_556_all_indices.k**:
```k
!#x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
896. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
897. **idioms_01_535_avoid_parentheses.k**:
```k
|1,#x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
898. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
899. **idioms_01_591_reshape_2column.k**:
```k
((_ 0.5*#x),2)#x
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
900. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
901. **idioms_01_595_one_row_matrix.k**:
```k
,x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
902. **idioms_01_616_scalar_from_vector.k**:
```k
x:,8
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
903. **idioms_01_616_scalar_from_vector.k**:
```k
*x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
904. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
905. **idioms_01_509_remove_y.k**:
```k
x _dv y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
906. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
907. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
908. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
909. **idioms_01_496_remove_punctuation.k**:
```k
x _dvl y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
910. **idioms_01_177_string_search.k**:
```k
x:"st"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
911. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
912. **idioms_01_177_string_search.k**:
```k
y _ss x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
913. **idioms_01_45_binary_representation.k**:
```k
x:16
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
914. **idioms_01_45_binary_representation.k**:
```k
2 _vs x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
915. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
916. **idioms_01_84_scalar_boolean.k**:
```k
2 _sv x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
917. **idioms_01_129_arctangent.k**:
```k
y:1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
918. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
919. **idioms_01_561_numeric_code.k**:
```k
_ic[x]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
920. **idioms_01_241_sum_subsets.k**:
```k
x:1+3 4#!12
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
921. **idioms_01_241_sum_subsets.k**:
```k
y:4 3#1 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
922. **idioms_01_241_sum_subsets.k**:
```k
x _mul y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
923. **idioms_01_61_cyclic_counter.k**:
```k
x:!10
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
924. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
925. **idioms_01_61_cyclic_counter.k**:
```k
1+x!y
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
926. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
927. **idioms_01_384_drop_1st_postpend.k**:
```k
1 _ x,0
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
928. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
929. **idioms_01_385_drop_last_prepend.k**:
```k
-1 _ 0,x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
930. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
931. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
932. **idioms_01_178_first_occurrence.k**:
```k
*y _ss x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
933. **idioms_01_447_conditional_drop.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
934. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
935. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
936. **idioms_01_447_conditional_drop.k**:
```k
(y*g) _ x
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
937. **idioms_01_448_conditional_drop_last.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
938. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
939. **idioms_01_448_conditional_drop_last.k**:
```k
(-y) _ x
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
940. **ktree_enumerate_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
941. **ktree_enumerate_relative_name.k**:
```k
!d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
942. **ktree_enumerate_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
943. **ktree_enumerate_relative_path.k**:
```k
!`d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
944. **ktree_enumerate_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
945. **ktree_enumerate_absolute_path.k**:
```k
!(.k.d)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
946. **ktree_enumerate_root.k**:
```k
!`
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
947. **ktree_indexing_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
948. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
949. **ktree_indexing_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
950. **ktree_indexing_absolute_name.k**:
```k
.k.d[`keyA]
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
951. **ktree_indexing_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
952. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
953. **ktree_indexing_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
954. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
955. **ktree_dot_apply_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); d . `keyA
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
956. **ktree_dot_apply_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
957. **ktree_dot_apply_absolute_name.k**:
```k
.k.d . `keyB
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
958. **ktree_dot_apply_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `d . `keyA
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
959. **ktree_dot_apply_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `.k.d . `keyB
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
960. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
961. **eval_dot_execute_path.k**:
```k
v:`e`f
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
962. **eval_dot_execute_path.k**:
```k
_eval (`",";,`a`b`c;(`",";,`d;`v))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
963. **eval_dot_repl_dir.k**:
```k
."\\d ^";.d:_d;."\\d .k";(.d;_d)
```
Incomplete consumption (position 2/18) (consumed 2/18)
-------------------------------------------------
964. **eval_dot_parse_and_eval.k**:
```k
a:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
965. **eval_dot_parse_and_eval.k**:
```k
. "a+4"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
966. **test_eval_monadic_star_atomic.k**:
```k
_eval (`"*:";,1)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
