# K3CSharp Parser Failures

**Generated:** 2026-03-21 03:22:47
**Test Results:** 810/839 passed (96.5%)

## Executive Summary

**Total Tests:** 839
**Passed Tests:** 810
**Failed Tests:** 29
**Success Rate:** 96.5%

**LRS Parser Statistics:**
- NULL Results: 217
- Incorrect Results: 0
- LRS Success Rate: 74.1%

**Top Failure Patterns:**
- After INTEGER (position 3/4): 57
- After CHARACTER_VECTOR (position 3/4): 23
- After FLOAT (position 3/4): 15
- After INTEGER (position 6/7): 13
- After RIGHT_BRACKET (position 4/5): 9
- After RIGHT_PAREN (position 9/10): 7
- After INTEGER (position 7/8): 7
- After CHARACTER_VECTOR (position 2/3): 7
- After RIGHT_BRACKET (position 6/7): 6
- After SYMBOL (position 3/4): 6

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
9. **mixed_list_with_null.k**:
```k
(1;_n;`test;42.5)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
10. **mixed_vector_empty_position.k**:
```k
(1;;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
11. **mixed_vector_multiple_empty.k**:
```k
(1;;;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
12. **mixed_vector_whitespace_position.k**:
```k
(1; ;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
13. **nested_vector_test.k**:
```k
((1 2 3);(4 5 6))
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
14. **parenthesized_vector.k**:
```k
(1;2;3;4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
15. **divide_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
16. **divide_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
17. **divide_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
18. **divide_integer.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
19. **simple_nested_test.k**:
```k
(1 2 3)
```
After RIGHT_PAREN (position 5/6)
-------------------------------------------------
20. **minus_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
21. **minus_integer.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
22. **special_int_vector.k**:
```k
0I 0N -0I
```
After INTEGER (position 3/4)
-------------------------------------------------
23. **special_float_vector.k**:
```k
0i 0n -0i
```
After FLOAT (position 3/4)
-------------------------------------------------
24. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
25. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
26. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
27. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
28. **square_bracket_vector_single.k**:
```k
v[4]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
29. **symbol_vector_compact.k**:
```k
`a`b`c
```
After SYMBOL (position 3/4)
-------------------------------------------------
30. **symbol_vector_spaces.k**:
```k
`a `b `c
```
After SYMBOL (position 3/4)
-------------------------------------------------
31. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
32. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
33. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
34. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
35. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
36. **null_vector.k**:
```k
(;1;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
37. **scoping_single.k**:
```k
globalVar: 100
```
After INTEGER (position 3/4)
-------------------------------------------------
38. **semicolon_vars.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
39. **semicolon_vars.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
40. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
41. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
42. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
43. **single_no_semicolon.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
44. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
45. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
46. **variable_assignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
47. **variable_reassignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
48. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
49. **variable_scoping_global_access.k**:
```k
globalVar: 100  // Test function accessing global variable
```
After INTEGER (position 3/4)
-------------------------------------------------
50. **variable_scoping_global_assignment.k**:
```k
globalVar: 100  // Test global assignment from nested function
```
After INTEGER (position 3/4)
-------------------------------------------------
51. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100  // Test verify global variable unchanged
```
After INTEGER (position 3/4)
-------------------------------------------------
52. **variable_scoping_local_hiding.k**:
```k
globalVar: 100  // Test function with local variable hiding global
```
After INTEGER (position 3/4)
-------------------------------------------------
53. **variable_scoping_nested_functions.k**:
```k
globalVar: 100  // Test nested functions
```
After INTEGER (position 3/4)
-------------------------------------------------
54. **variable_usage.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
55. **variable_usage.k**:
```k
y:20
```
After INTEGER (position 3/4)
-------------------------------------------------
56. **dot_execute_context.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
57. **dot_execute_variables.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
58. **dot_execute_variables.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
59. **format_braces_expressions.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
60. **format_braces_expressions.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
61. **format_braces_nested_expr.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
62. **format_braces_nested_expr.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
63. **format_braces_nested_expr.k**:
```k
z:3
```
After INTEGER (position 3/4)
-------------------------------------------------
64. **format_braces_complex.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
65. **format_braces_complex.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
66. **format_braces_complex.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
67. **format_braces_string.k**:
```k
name:"John"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
68. **format_braces_string.k**:
```k
age:25
```
After INTEGER (position 3/4)
-------------------------------------------------
69. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
After INTEGER (position 3/27)
-------------------------------------------------
70. **format_braces_simple.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
71. **format_braces_simple.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
72. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
After INTEGER (position 3/33)
-------------------------------------------------
73. **format_braces_nested_arith.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
74. **format_braces_nested_arith.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
75. **format_braces_nested_arith.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
76. **format_braces_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
77. **format_braces_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
78. **format_braces_float.k**:
```k
c:3.0
```
After FLOAT (position 3/4)
-------------------------------------------------
79. **format_braces_mixed_arith.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
80. **format_braces_mixed_arith.k**:
```k
y:3
```
After INTEGER (position 3/4)
-------------------------------------------------
81. **format_braces_mixed_arith.k**:
```k
z:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
82. **format_braces_example.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
83. **format_braces_example.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
84. **time_t.k**:
```k
r:_t
```
After TIME (position 3/4)
-------------------------------------------------
85. **time_ltime.k**:
```k
_ltime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
86. **vector_notation_empty.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
87. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
88. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
89. **vector_notation_single_group.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
90. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
91. **vector_notation_variables.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
92. **vector_notation_variables.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
93. **vector_with_null.k**:
```k
(_n;1;2)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
94. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
95. **do_loop.k**:
```k
i: 0; do[3; i+: 1]  // Do loop - increment i 3 times
```
After INTEGER (position 3/13)
-------------------------------------------------
96. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
97. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
98. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
99. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
100. **if_simple_test.k**:
```k
if[3; 42]  // If function with bracket notation
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
101. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]  // If statement - condition true
```
After INTEGER (position 3/15)
-------------------------------------------------
102. **isolated.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
103. **isolated.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
104. **modulo.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
105. **modulo.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
106. **string_parse.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
107. **string_parse.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
108. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
109. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
110. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]  // Test while function with bracket notation
```
After INTEGER (position 3/15)
-------------------------------------------------
111. **while_safe_test.k**:
```k
i: 0
```
After INTEGER (position 3/4)
-------------------------------------------------
112. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
113. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
114. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
115. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
116. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
117. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
118. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
119. **ffi_type_marshalling.k**:
```k
3.14159 _hint `float
```
After SYMBOL (position 3/4)
-------------------------------------------------
120. **ffi_type_marshalling.k**:
```k
"hello" _hint `string
```
After SYMBOL (position 3/4)
-------------------------------------------------
121. **ffi_type_marshalling.k**:
```k
1 2 3 4 5 _hint `list
```
After SYMBOL (position 7/8)
-------------------------------------------------
122. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
After CHARACTER_VECTOR (position 3/12)
-------------------------------------------------
123. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
124. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
125. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
126. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
127. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
128. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
129. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
130. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
131. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
132. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
133. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
134. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
135. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
136. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
137. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
138. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
139. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
140. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
141. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
142. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
143. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
144. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
145. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
146. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
147. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
148. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
149. **idioms_01_388_drop_rows.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
150. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
151. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
152. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
153. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
154. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
155. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
156. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
157. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
158. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
159. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
160. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
161. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
162. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
163. **idioms_01_434_replace_first.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
164. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
165. **idioms_01_433_replace_last.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
166. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
167. **idioms_01_406_add_last.k**:
```k
y:100
```
After INTEGER (position 3/4)
-------------------------------------------------
168. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
169. **idioms_01_449_limit_between.k**:
```k
l:30
```
After INTEGER (position 3/4)
-------------------------------------------------
170. **idioms_01_449_limit_between.k**:
```k
h:70
```
After INTEGER (position 3/4)
-------------------------------------------------
171. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
172. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
173. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
174. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
175. **idioms_01_504_replace_satisfying.k**:
```k
g:" "
```
After CHARACTER (position 3/4)
-------------------------------------------------
176. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
177. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
178. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
179. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
180. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
181. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
182. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
183. **idioms_01_509_remove_y.k**:
```k
y:"a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
184. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
185. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
186. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
187. **idioms_01_177_string_search.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
188. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
189. **idioms_01_45_binary_representation.k**:
```k
x:16
```
After INTEGER (position 3/4)
-------------------------------------------------
190. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
191. **idioms_01_129_arctangent.k**:
```k
x:_sqrt[3]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
192. **idioms_01_129_arctangent.k**:
```k
y:1
```
After INTEGER (position 3/4)
-------------------------------------------------
193. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
194. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
After INTEGER (position 3/4)
-------------------------------------------------
195. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
196. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
197. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
198. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
199. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
200. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
201. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
After INTEGER (position 3/4)
-------------------------------------------------
202. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
203. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
204. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
205. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
206. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
207. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
208. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
209. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
210. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
211. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
212. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
213. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
214. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
215. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------
216. **eval_dot_parse_and_eval.k**:
```k
a:7
```
After INTEGER (position 3/4)
-------------------------------------------------
217. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
