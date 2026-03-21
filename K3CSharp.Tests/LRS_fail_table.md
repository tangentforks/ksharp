# K3CSharp Parser Failures

**Generated:** 2026-03-21 02:30:18
**Test Results:** 783/841 passed (93.1%)

## Executive Summary

**Total Tests:** 841
**Passed Tests:** 783
**Failed Tests:** 58
**Success Rate:** 93.1%

**LRS Parser Statistics:**
- NULL Results: 307
- Incorrect Results: 0
- LRS Success Rate: 63.5%

**Top Failure Patterns:**
- After INTEGER (position 3/4): 69
- After CHARACTER_VECTOR (position 3/4): 31
- After INTEGER (position 6/7): 19
- After FLOAT (position 3/4): 17
- After SYMBOL (position 3/4): 13
- After INTEGER (position 7/8): 10
- After RIGHT_BRACKET (position 4/5): 9
- After RIGHT_PAREN (position 9/10): 8
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
40. **io_write_int.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_write.l" 1: 42
```
After INTEGER (position 3/4)
-------------------------------------------------
41. **io_roundtrip.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l" 1: (1;2.5;"hello")
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
42. **io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
43. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
44. **io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
45. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
46. **io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
47. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
48. **io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
49. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
50. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
51. **null_vector.k**:
```k
(;1;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
52. **scoping_single.k**:
```k
globalVar: 100
```
After INTEGER (position 3/4)
-------------------------------------------------
53. **semicolon_vars.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
54. **semicolon_vars.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
55. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
56. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
57. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
58. **single_no_semicolon.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
59. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
60. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
61. **variable_assignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
62. **variable_reassignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
63. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
64. **variable_scoping_global_access.k**:
```k
globalVar: 100  // Test function accessing global variable
```
After INTEGER (position 3/4)
-------------------------------------------------
65. **variable_scoping_global_assignment.k**:
```k
globalVar: 100  // Test global assignment from nested function
```
After INTEGER (position 3/4)
-------------------------------------------------
66. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100  // Test verify global variable unchanged
```
After INTEGER (position 3/4)
-------------------------------------------------
67. **variable_scoping_local_hiding.k**:
```k
globalVar: 100  // Test function with local variable hiding global
```
After INTEGER (position 3/4)
-------------------------------------------------
68. **variable_scoping_nested_functions.k**:
```k
globalVar: 100  // Test nested functions
```
After INTEGER (position 3/4)
-------------------------------------------------
69. **variable_usage.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
70. **variable_usage.k**:
```k
y:20
```
After INTEGER (position 3/4)
-------------------------------------------------
71. **dot_execute_context.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
72. **ci_adverb_vector.k**:
```k
_ci' 97 94 80
```
After INTEGER (position 5/6)
-------------------------------------------------
73. **dot_execute_variables.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
74. **dot_execute_variables.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
75. **format_braces_expressions.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
76. **format_braces_expressions.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
77. **format_braces_nested_expr.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
78. **format_braces_nested_expr.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
79. **format_braces_nested_expr.k**:
```k
z:3
```
After INTEGER (position 3/4)
-------------------------------------------------
80. **format_braces_complex.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
81. **format_braces_complex.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
82. **format_braces_complex.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
83. **format_braces_string.k**:
```k
name:"John"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
84. **format_braces_string.k**:
```k
age:25
```
After INTEGER (position 3/4)
-------------------------------------------------
85. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
After INTEGER (position 3/27)
-------------------------------------------------
86. **format_braces_simple.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
87. **format_braces_simple.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
88. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
After INTEGER (position 3/33)
-------------------------------------------------
89. **format_braces_nested_arith.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
90. **format_braces_nested_arith.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
91. **format_braces_nested_arith.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
92. **format_braces_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
93. **format_braces_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
94. **format_braces_float.k**:
```k
c:3.0
```
After FLOAT (position 3/4)
-------------------------------------------------
95. **format_braces_mixed_arith.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
96. **format_braces_mixed_arith.k**:
```k
y:3
```
After INTEGER (position 3/4)
-------------------------------------------------
97. **format_braces_mixed_arith.k**:
```k
z:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
98. **format_braces_example.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
99. **format_braces_example.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
100. **log.k**:
```k
_log 10
```
After INTEGER (position 2/3)
-------------------------------------------------
101. **time_t.k**:
```k
r:_t
```
After TIME (position 3/4)
-------------------------------------------------
102. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
103. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 5/36)
-------------------------------------------------
104. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
105. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
106. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 6/37)
-------------------------------------------------
107. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
108. **list_dv_basic.k**:
```k
3 4 4 5 _dv 4
```
After INTEGER (position 6/7)
-------------------------------------------------
109. **list_dv_nomatch.k**:
```k
3 4 4 5 _dv 6
```
After INTEGER (position 6/7)
-------------------------------------------------
110. **list_di_basic.k**:
```k
3 2 4 5 _di 1
```
After INTEGER (position 6/7)
-------------------------------------------------
111. **list_di_multiple.k**:
```k
3 2 4 5 _di 1 3
```
After INTEGER (position 7/8)
-------------------------------------------------
112. **list_sv_base10.k**:
```k
10 _sv 1 9 9 5
```
After INTEGER (position 6/7)
-------------------------------------------------
113. **list_sv_base2.k**:
```k
2 _sv 1 0 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
114. **list_sv_mixed.k**:
```k
10 10 10 10 _sv 1 9 9 5
```
After INTEGER (position 9/10)
-------------------------------------------------
115. **list_setenv.k**:
```k
`TESTVAR _setenv "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
116. **test_vs_dyadic.k**:
```k
10 _vs 1995
```
After INTEGER (position 3/4)
-------------------------------------------------
117. **test_sm_basic.k**:
```k
`foo _sm `foo
```
After SYMBOL (position 3/4)
-------------------------------------------------
118. **test_sm_simple.k**:
```k
`a _sm `a
```
After SYMBOL (position 3/4)
-------------------------------------------------
119. **test_ss_basic.k**:
```k
"hello world" _ss "world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
120. **io_write_basic.k**:
```k
`testW 0: ("line1";"line2";"line3");0:`testW
```
After RIGHT_PAREN (position 9/13)
-------------------------------------------------
121. **io_lrs_test.k**:
```k
`test 5: "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
122. **io_lrs_complex.k**:
```k
`test1 5: "hello"; `test2 6: "ABC"; `test3 0:,"world"; 6:`test3
```
After CHARACTER_VECTOR (position 3/16)
-------------------------------------------------
123. **io_append_simple.k**:
```k
`testfile 5: "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
124. **io_append_basic.k**:
```k
`test 5: "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
125. **io_append_multiple.k**:
```k
`test 5: "hello"; `test 5: "world"; `test 5: 1 2 3
```
After CHARACTER_VECTOR (position 3/14)
-------------------------------------------------
126. **io_read_bytes_empty.k**:
```k
`empty 0:(); 6:`empty
```
After RIGHT_PAREN (position 4/8)
-------------------------------------------------
127. **io_write_bytes_binary.k**:
```k
`test 6:(0 1 2 255)
```
After RIGHT_PAREN (position 8/9)
-------------------------------------------------
128. **search_bin_basic.k**:
```k
3 4 5 6 _bin 4
```
After INTEGER (position 6/7)
-------------------------------------------------
129. **search_binl_eachleft.k**:
```k
1 3 5 _binl 1 2 3 4 5
```
After INTEGER (position 9/10)
-------------------------------------------------
130. **search_lin_intersection.k**:
```k
1 3 5 7 9 _lin 1 2 3 4 5
```
After INTEGER (position 11/12)
-------------------------------------------------
131. **vector_notation_empty.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
132. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
133. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
134. **vector_notation_single_group.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
135. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
136. **vector_notation_variables.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
137. **vector_notation_variables.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
138. **vector_with_null.k**:
```k
(_n;1;2)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
139. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
140. **do_loop.k**:
```k
i: 0; do[3; i+: 1]  // Do loop - increment i 3 times
```
After INTEGER (position 3/13)
-------------------------------------------------
141. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
142. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
143. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
144. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
145. **if_simple_test.k**:
```k
if[3; 42]  // If function with bracket notation
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
146. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]  // If statement - condition true
```
After INTEGER (position 3/15)
-------------------------------------------------
147. **isolated.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
148. **isolated.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
149. **modulo.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
150. **modulo.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
151. **string_parse.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
152. **string_parse.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
153. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
154. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
155. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]  // Test while function with bracket notation
```
After INTEGER (position 3/15)
-------------------------------------------------
156. **while_safe_test.k**:
```k
i: 0
```
After INTEGER (position 3/4)
-------------------------------------------------
157. **serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
158. **serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
After SYMBOL (position 3/4)
-------------------------------------------------
159. **db_basic_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
160. **db_float.k**:
```k
_db _bd 3.14
```
After FLOAT (position 3/4)
-------------------------------------------------
161. **db_symbol.k**:
```k
_db _bd `test
```
After SYMBOL (position 3/4)
-------------------------------------------------
162. **db_int_vector.k**:
```k
_db _bd 1 2 3
```
After INTEGER (position 5/6)
-------------------------------------------------
163. **db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
After SYMBOL (position 5/6)
-------------------------------------------------
164. **db_char_vector.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
165. **db_null.k**:
```k
_db _bd 0N
```
After INTEGER (position 3/4)
-------------------------------------------------
166. **db_character.k**:
```k
_db _bd "a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
167. **db_float_simple.k**:
```k
_db _bd 1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
168. **db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
169. **db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
After FLOAT (position 5/6)
-------------------------------------------------
170. **db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
171. **db_symbol_simple.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
172. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
173. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
174. **db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
After FLOAT (position 7/8)
-------------------------------------------------
175. **db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
After INTEGER (position 12/13)
-------------------------------------------------
176. **db_string_hello.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
177. **db_symbol_hello.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
178. **db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
After SYMBOL (position 5/6)
-------------------------------------------------
179. **math_and_basic.k**:
```k
5 _and 3
```
After INTEGER (position 3/4)
-------------------------------------------------
180. **math_and_vector.k**:
```k
(5 6 3) _and (1 2 3)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
181. **math_div_float.k**:
```k
7 _div 2
```
After INTEGER (position 3/4)
-------------------------------------------------
182. **math_div_integer.k**:
```k
7 _div 3
```
After INTEGER (position 3/4)
-------------------------------------------------
183. **math_div_vector.k**:
```k
(7 14 21) _div 3
```
After INTEGER (position 7/8)
-------------------------------------------------
184. **math_lsq_non_square.k**:
```k
(7 8 9) _lsq (1 2 3;4 5 6)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
185. **math_lsq_high_rank.k**:
```k
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
```
After RIGHT_PAREN (position 18/19)
-------------------------------------------------
186. **math_lsq_complex.k**:
```k
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
187. **math_lsq_regression.k**:
```k
(1 2 3.0) _lsq (1 1 1.0;1 2 4.0)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
188. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
189. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
190. **math_or_basic.k**:
```k
5 _or 3
```
After INTEGER (position 3/4)
-------------------------------------------------
191. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
192. **math_rot_basic.k**:
```k
8 _rot 2
```
After INTEGER (position 3/4)
-------------------------------------------------
193. **math_shift_basic.k**:
```k
8 _shift 2
```
After INTEGER (position 3/4)
-------------------------------------------------
194. **math_shift_vector.k**:
```k
(8 16 32 64) _shift 2
```
After INTEGER (position 8/9)
-------------------------------------------------
195. **math_xor_basic.k**:
```k
5 _xor 3
```
After INTEGER (position 3/4)
-------------------------------------------------
196. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
197. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
198. **ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
After SYMBOL (position 5/8)
-------------------------------------------------
199. **ffi_assembly_load.k**:
```k
"System.Private.CoreLib" 2: `System.String
```
After SYMBOL (position 3/4)
-------------------------------------------------
200. **ffi_type_marshalling.k**:
```k
3.14159 _hint `float
```
After SYMBOL (position 3/4)
-------------------------------------------------
201. **ffi_type_marshalling.k**:
```k
"hello" _hint `string
```
After SYMBOL (position 3/4)
-------------------------------------------------
202. **ffi_type_marshalling.k**:
```k
1 2 3 4 5 _hint `list
```
After SYMBOL (position 7/8)
-------------------------------------------------
203. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
After CHARACTER_VECTOR (position 3/12)
-------------------------------------------------
204. **ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
205. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
206. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
207. **ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
208. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
209. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
210. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
211. **ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
212. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
213. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
214. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
215. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
216. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
217. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
218. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
219. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
220. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
221. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
222. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
223. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
224. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
225. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
226. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
227. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
228. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
229. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
230. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
231. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
232. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
233. **idioms_01_388_drop_rows.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
234. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
235. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
236. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
237. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
238. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
239. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
240. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
241. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
242. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
243. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
244. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
245. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
246. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
247. **idioms_01_434_replace_first.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
248. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
249. **idioms_01_433_replace_last.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
250. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
251. **idioms_01_406_add_last.k**:
```k
y:100
```
After INTEGER (position 3/4)
-------------------------------------------------
252. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
253. **idioms_01_449_limit_between.k**:
```k
l:30
```
After INTEGER (position 3/4)
-------------------------------------------------
254. **idioms_01_449_limit_between.k**:
```k
h:70
```
After INTEGER (position 3/4)
-------------------------------------------------
255. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
256. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
257. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
258. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
259. **idioms_01_504_replace_satisfying.k**:
```k
g:" "
```
After CHARACTER (position 3/4)
-------------------------------------------------
260. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
261. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
262. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
263. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
264. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
265. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
266. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
267. **idioms_01_509_remove_y.k**:
```k
y:"a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
268. **idioms_01_509_remove_y.k**:
```k
x _dv y
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
269. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
270. **idioms_01_510_remove_blanks.k**:
```k
x _dv " "
```
After CHARACTER (position 3/4)
-------------------------------------------------
271. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
272. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
273. **idioms_01_177_string_search.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
274. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
275. **idioms_01_177_string_search.k**:
```k
y _ss x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
276. **idioms_01_45_binary_representation.k**:
```k
x:16
```
After INTEGER (position 3/4)
-------------------------------------------------
277. **idioms_01_45_binary_representation.k**:
```k
2 _vs x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
278. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
279. **idioms_01_84_scalar_boolean.k**:
```k
2 _sv x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
280. **idioms_01_129_arctangent.k**:
```k
x:_sqrt[3]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
281. **idioms_01_129_arctangent.k**:
```k
y:1
```
After INTEGER (position 3/4)
-------------------------------------------------
282. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
283. **idioms_01_241_sum_subsets.k**:
```k
x _mul y
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
284. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
After INTEGER (position 3/4)
-------------------------------------------------
285. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
286. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
287. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
288. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
289. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
290. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
291. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
After INTEGER (position 3/4)
-------------------------------------------------
292. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
293. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
294. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
295. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
296. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
297. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
298. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
299. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
300. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
301. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
302. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
303. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
304. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
305. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------
306. **eval_dot_parse_and_eval.k**:
```k
a:7
```
After INTEGER (position 3/4)
-------------------------------------------------
307. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
