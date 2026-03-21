# K3CSharp Parser Failures

**Generated:** 2026-03-21 02:45:08
**Test Results:** 810/839 passed (96.5%)

## Executive Summary

**Total Tests:** 839
**Passed Tests:** 810
**Failed Tests:** 29
**Success Rate:** 96.5%

**LRS Parser Statistics:**
- NULL Results: 288
- Incorrect Results: 0
- LRS Success Rate: 65.7%

**Top Failure Patterns:**
- After INTEGER (position 3/4): 68
- After CHARACTER_VECTOR (position 3/4): 28
- After INTEGER (position 6/7): 19
- After FLOAT (position 3/4): 17
- After SYMBOL (position 3/4): 12
- After INTEGER (position 7/8): 10
- After RIGHT_BRACKET (position 4/5): 9
- After RIGHT_PAREN (position 9/10): 7
- After CHARACTER_VECTOR (position 2/3): 7
- After INTEGER (position 2/3): 6

## LRS Parser Failures

1. **anonymous_function_empty.k**:
```k
{}
```
After RIGHT_BRACE (position 2/3)
-------------------------------------------------
2. **lrs_atomic_parser_basic.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
3. **lrs_adverb_parser_basic.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
4. **empty_list.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
5. **empty_mixed_vector.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
6. **lambda_string_assign.k**:
```k
{a:"hello";a}[]
```
After RIGHT_BRACKET (position 9/10)
-------------------------------------------------
7. **lambda_string_literal.k**:
```k
{"hello"}[]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
8. **lambda_symbol_literal.k**:
```k
{`abc}[]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
9. **math_exp.k**:
```k
_exp 2
```
After INTEGER (position 2/3)
-------------------------------------------------
10. **math_log.k**:
```k
_log 10
```
After INTEGER (position 2/3)
-------------------------------------------------
11. **math_exp_basic.k**:
```k
_exp 1
```
After INTEGER (position 2/3)
-------------------------------------------------
12. **math_log_negative.k**:
```k
_log -1
```
After INTEGER (position 2/3)
-------------------------------------------------
13. **math_log_zero.k**:
```k
_log 0
```
After INTEGER (position 2/3)
-------------------------------------------------
14. **math_mul_matrix_2x2.k**:
```k
((1 2);(3 4)) _mul ((5 6);(7 8))
```
After RIGHT_PAREN (position 23/24)
-------------------------------------------------
15. **math_mul_matrix_2x3_3x2.k**:
```k
((1 2 3);(4 5 6)) _mul ((7 8);(9 10);(11 12))
```
After RIGHT_PAREN (position 30/31)
-------------------------------------------------
16. **math_mul_matrix_3x3.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) _mul ((9 8 7);(6 5 4);(3 2 1))
```
After RIGHT_PAREN (position 39/40)
-------------------------------------------------
17. **math_mul_matrix_4x2_2x4.k**:
```k
((1 2);(3 4);(5 6);(7 8)) _mul ((9 10 11 12);(13 14 15 16))
```
After RIGHT_PAREN (position 37/38)
-------------------------------------------------
18. **mixed_list_with_null.k**:
```k
(1;_n;`test;42.5)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
19. **mixed_vector_empty_position.k**:
```k
(1;;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
20. **mixed_vector_multiple_empty.k**:
```k
(1;;;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
21. **mixed_vector_whitespace_position.k**:
```k
(1; ;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
22. **nested_vector_test.k**:
```k
((1 2 3);(4 5 6))
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
23. **parenthesized_vector.k**:
```k
(1;2;3;4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
24. **divide_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
25. **divide_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
26. **divide_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
27. **divide_integer.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
28. **simple_nested_test.k**:
```k
(1 2 3)
```
After RIGHT_PAREN (position 5/6)
-------------------------------------------------
29. **minus_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
30. **minus_integer.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
31. **special_int_vector.k**:
```k
0I 0N -0I
```
After INTEGER (position 3/4)
-------------------------------------------------
32. **special_float_vector.k**:
```k
0i 0n -0i
```
After FLOAT (position 3/4)
-------------------------------------------------
33. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
34. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
35. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
36. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
37. **square_bracket_vector_single.k**:
```k
v[4]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
38. **symbol_vector_compact.k**:
```k
`a`b`c
```
After SYMBOL (position 3/4)
-------------------------------------------------
39. **symbol_vector_spaces.k**:
```k
`a `b `c
```
After SYMBOL (position 3/4)
-------------------------------------------------
40. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
41. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
42. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
43. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
44. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
45. **null_vector.k**:
```k
(;1;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
46. **scoping_single.k**:
```k
globalVar: 100
```
After INTEGER (position 3/4)
-------------------------------------------------
47. **semicolon_vars.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
48. **semicolon_vars.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
49. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
50. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
51. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
52. **single_no_semicolon.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
53. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
54. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
55. **variable_assignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
56. **variable_reassignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
57. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
58. **variable_scoping_global_access.k**:
```k
globalVar: 100  // Test function accessing global variable
```
After INTEGER (position 3/4)
-------------------------------------------------
59. **variable_scoping_global_assignment.k**:
```k
globalVar: 100  // Test global assignment from nested function
```
After INTEGER (position 3/4)
-------------------------------------------------
60. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100  // Test verify global variable unchanged
```
After INTEGER (position 3/4)
-------------------------------------------------
61. **variable_scoping_local_hiding.k**:
```k
globalVar: 100  // Test function with local variable hiding global
```
After INTEGER (position 3/4)
-------------------------------------------------
62. **variable_scoping_nested_functions.k**:
```k
globalVar: 100  // Test nested functions
```
After INTEGER (position 3/4)
-------------------------------------------------
63. **variable_usage.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
64. **variable_usage.k**:
```k
y:20
```
After INTEGER (position 3/4)
-------------------------------------------------
65. **dot_execute_context.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
66. **ci_adverb_vector.k**:
```k
_ci' 97 94 80
```
After INTEGER (position 5/6)
-------------------------------------------------
67. **dot_execute_variables.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
68. **dot_execute_variables.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
69. **format_braces_expressions.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
70. **format_braces_expressions.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
71. **format_braces_nested_expr.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
72. **format_braces_nested_expr.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
73. **format_braces_nested_expr.k**:
```k
z:3
```
After INTEGER (position 3/4)
-------------------------------------------------
74. **format_braces_complex.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
75. **format_braces_complex.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
76. **format_braces_complex.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
77. **format_braces_string.k**:
```k
name:"John"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
78. **format_braces_string.k**:
```k
age:25
```
After INTEGER (position 3/4)
-------------------------------------------------
79. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
After INTEGER (position 3/27)
-------------------------------------------------
80. **format_braces_simple.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
81. **format_braces_simple.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
82. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
After INTEGER (position 3/33)
-------------------------------------------------
83. **format_braces_nested_arith.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
84. **format_braces_nested_arith.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
85. **format_braces_nested_arith.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
86. **format_braces_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
87. **format_braces_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
88. **format_braces_float.k**:
```k
c:3.0
```
After FLOAT (position 3/4)
-------------------------------------------------
89. **format_braces_mixed_arith.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
90. **format_braces_mixed_arith.k**:
```k
y:3
```
After INTEGER (position 3/4)
-------------------------------------------------
91. **format_braces_mixed_arith.k**:
```k
z:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
92. **format_braces_example.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
93. **format_braces_example.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
94. **log.k**:
```k
_log 10
```
After INTEGER (position 2/3)
-------------------------------------------------
95. **time_t.k**:
```k
r:_t
```
After TIME (position 3/4)
-------------------------------------------------
96. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
97. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 5/36)
-------------------------------------------------
98. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
99. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
100. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 6/37)
-------------------------------------------------
101. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
102. **list_dv_basic.k**:
```k
3 4 4 5 _dv 4
```
After INTEGER (position 6/7)
-------------------------------------------------
103. **list_dv_nomatch.k**:
```k
3 4 4 5 _dv 6
```
After INTEGER (position 6/7)
-------------------------------------------------
104. **list_di_basic.k**:
```k
3 2 4 5 _di 1
```
After INTEGER (position 6/7)
-------------------------------------------------
105. **list_di_multiple.k**:
```k
3 2 4 5 _di 1 3
```
After INTEGER (position 7/8)
-------------------------------------------------
106. **list_sv_base10.k**:
```k
10 _sv 1 9 9 5
```
After INTEGER (position 6/7)
-------------------------------------------------
107. **list_sv_base2.k**:
```k
2 _sv 1 0 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
108. **list_sv_mixed.k**:
```k
10 10 10 10 _sv 1 9 9 5
```
After INTEGER (position 9/10)
-------------------------------------------------
109. **list_setenv.k**:
```k
`TESTVAR _setenv "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
110. **test_vs_dyadic.k**:
```k
10 _vs 1995
```
After INTEGER (position 3/4)
-------------------------------------------------
111. **test_sm_basic.k**:
```k
`foo _sm `foo
```
After SYMBOL (position 3/4)
-------------------------------------------------
112. **test_sm_simple.k**:
```k
`a _sm `a
```
After SYMBOL (position 3/4)
-------------------------------------------------
113. **test_ss_basic.k**:
```k
"hello world" _ss "world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
114. **search_bin_basic.k**:
```k
3 4 5 6 _bin 4
```
After INTEGER (position 6/7)
-------------------------------------------------
115. **search_binl_eachleft.k**:
```k
1 3 5 _binl 1 2 3 4 5
```
After INTEGER (position 9/10)
-------------------------------------------------
116. **search_lin_intersection.k**:
```k
1 3 5 7 9 _lin 1 2 3 4 5
```
After INTEGER (position 11/12)
-------------------------------------------------
117. **vector_notation_empty.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
118. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
119. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
120. **vector_notation_single_group.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
121. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
122. **vector_notation_variables.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
123. **vector_notation_variables.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
124. **vector_with_null.k**:
```k
(_n;1;2)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
125. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
126. **do_loop.k**:
```k
i: 0; do[3; i+: 1]  // Do loop - increment i 3 times
```
After INTEGER (position 3/13)
-------------------------------------------------
127. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
128. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
129. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
130. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
131. **if_simple_test.k**:
```k
if[3; 42]  // If function with bracket notation
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
132. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]  // If statement - condition true
```
After INTEGER (position 3/15)
-------------------------------------------------
133. **isolated.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
134. **isolated.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
135. **modulo.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
136. **modulo.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
137. **string_parse.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
138. **string_parse.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
139. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
140. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
141. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]  // Test while function with bracket notation
```
After INTEGER (position 3/15)
-------------------------------------------------
142. **while_safe_test.k**:
```k
i: 0
```
After INTEGER (position 3/4)
-------------------------------------------------
143. **serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
144. **serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
After SYMBOL (position 3/4)
-------------------------------------------------
145. **db_basic_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
146. **db_float.k**:
```k
_db _bd 3.14
```
After FLOAT (position 3/4)
-------------------------------------------------
147. **db_symbol.k**:
```k
_db _bd `test
```
After SYMBOL (position 3/4)
-------------------------------------------------
148. **db_int_vector.k**:
```k
_db _bd 1 2 3
```
After INTEGER (position 5/6)
-------------------------------------------------
149. **db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
After SYMBOL (position 5/6)
-------------------------------------------------
150. **db_char_vector.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
151. **db_null.k**:
```k
_db _bd 0N
```
After INTEGER (position 3/4)
-------------------------------------------------
152. **db_character.k**:
```k
_db _bd "a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
153. **db_float_simple.k**:
```k
_db _bd 1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
154. **db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
155. **db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
After FLOAT (position 5/6)
-------------------------------------------------
156. **db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
157. **db_symbol_simple.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
158. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
159. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
160. **db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
After FLOAT (position 7/8)
-------------------------------------------------
161. **db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
After INTEGER (position 12/13)
-------------------------------------------------
162. **db_string_hello.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
163. **db_symbol_hello.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
164. **db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
After SYMBOL (position 5/6)
-------------------------------------------------
165. **math_and_basic.k**:
```k
5 _and 3
```
After INTEGER (position 3/4)
-------------------------------------------------
166. **math_and_vector.k**:
```k
(5 6 3) _and (1 2 3)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
167. **math_div_float.k**:
```k
7 _div 2
```
After INTEGER (position 3/4)
-------------------------------------------------
168. **math_div_integer.k**:
```k
7 _div 3
```
After INTEGER (position 3/4)
-------------------------------------------------
169. **math_div_vector.k**:
```k
(7 14 21) _div 3
```
After INTEGER (position 7/8)
-------------------------------------------------
170. **math_lsq_non_square.k**:
```k
(7 8 9) _lsq (1 2 3;4 5 6)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
171. **math_lsq_high_rank.k**:
```k
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
```
After RIGHT_PAREN (position 18/19)
-------------------------------------------------
172. **math_lsq_complex.k**:
```k
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
173. **math_lsq_regression.k**:
```k
(1 2 3.0) _lsq (1 1 1.0;1 2 4.0)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
174. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
175. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
176. **math_or_basic.k**:
```k
5 _or 3
```
After INTEGER (position 3/4)
-------------------------------------------------
177. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
178. **math_rot_basic.k**:
```k
8 _rot 2
```
After INTEGER (position 3/4)
-------------------------------------------------
179. **math_shift_basic.k**:
```k
8 _shift 2
```
After INTEGER (position 3/4)
-------------------------------------------------
180. **math_shift_vector.k**:
```k
(8 16 32 64) _shift 2
```
After INTEGER (position 8/9)
-------------------------------------------------
181. **math_xor_basic.k**:
```k
5 _xor 3
```
After INTEGER (position 3/4)
-------------------------------------------------
182. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
183. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
184. **ffi_type_marshalling.k**:
```k
3.14159 _hint `float
```
After SYMBOL (position 3/4)
-------------------------------------------------
185. **ffi_type_marshalling.k**:
```k
"hello" _hint `string
```
After SYMBOL (position 3/4)
-------------------------------------------------
186. **ffi_type_marshalling.k**:
```k
1 2 3 4 5 _hint `list
```
After SYMBOL (position 7/8)
-------------------------------------------------
187. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
After CHARACTER_VECTOR (position 3/12)
-------------------------------------------------
188. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
189. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
190. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
191. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
192. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
193. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
194. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
195. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
196. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
197. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
198. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
199. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
200. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
201. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
202. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
203. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
204. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
205. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
206. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
207. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
208. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
209. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
210. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
211. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
212. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
213. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
214. **idioms_01_388_drop_rows.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
215. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
216. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
217. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
218. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
219. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
220. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
221. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
222. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
223. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
224. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
225. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
226. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
227. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
228. **idioms_01_434_replace_first.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
229. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
230. **idioms_01_433_replace_last.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
231. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
232. **idioms_01_406_add_last.k**:
```k
y:100
```
After INTEGER (position 3/4)
-------------------------------------------------
233. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
234. **idioms_01_449_limit_between.k**:
```k
l:30
```
After INTEGER (position 3/4)
-------------------------------------------------
235. **idioms_01_449_limit_between.k**:
```k
h:70
```
After INTEGER (position 3/4)
-------------------------------------------------
236. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
237. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
238. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
239. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
240. **idioms_01_504_replace_satisfying.k**:
```k
g:" "
```
After CHARACTER (position 3/4)
-------------------------------------------------
241. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
242. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
243. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
244. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
245. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
246. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
247. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
248. **idioms_01_509_remove_y.k**:
```k
y:"a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
249. **idioms_01_509_remove_y.k**:
```k
x _dv y
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
250. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
251. **idioms_01_510_remove_blanks.k**:
```k
x _dv " "
```
After CHARACTER (position 3/4)
-------------------------------------------------
252. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
253. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
254. **idioms_01_177_string_search.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
255. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
256. **idioms_01_177_string_search.k**:
```k
y _ss x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
257. **idioms_01_45_binary_representation.k**:
```k
x:16
```
After INTEGER (position 3/4)
-------------------------------------------------
258. **idioms_01_45_binary_representation.k**:
```k
2 _vs x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
259. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
260. **idioms_01_84_scalar_boolean.k**:
```k
2 _sv x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
261. **idioms_01_129_arctangent.k**:
```k
x:_sqrt[3]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
262. **idioms_01_129_arctangent.k**:
```k
y:1
```
After INTEGER (position 3/4)
-------------------------------------------------
263. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
264. **idioms_01_241_sum_subsets.k**:
```k
x _mul y
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
265. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
After INTEGER (position 3/4)
-------------------------------------------------
266. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
267. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
268. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
269. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
270. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
271. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
272. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
After INTEGER (position 3/4)
-------------------------------------------------
273. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
274. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
275. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
276. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
277. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
278. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
279. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
280. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
281. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
282. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
283. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
284. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
285. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
286. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------
287. **eval_dot_parse_and_eval.k**:
```k
a:7
```
After INTEGER (position 3/4)
-------------------------------------------------
288. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
