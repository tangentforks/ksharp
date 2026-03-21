# K3CSharp Parser Failures

**Generated:** 2026-03-21 02:01:29
**Test Results:** 782/839 passed (93.2%)

## Executive Summary

**Total Tests:** 839
**Passed Tests:** 782
**Failed Tests:** 57
**Success Rate:** 93.2%

**LRS Parser Statistics:**
- NULL Results: 411
- Incorrect Results: 0
- LRS Success Rate: 51.0%

**Top Failure Patterns:**
- After INTEGER (position 3/4): 75
- After CHARACTER_VECTOR (position 3/4): 32
- After INTEGER (position 6/7): 27
- After FLOAT (position 3/4): 18
- After SYMBOL (position 3/4): 17
- After INTEGER (position 7/8): 15
- After INTEGER (position 5/6): 13
- After RIGHT_BRACKET (position 4/5): 9
- After RIGHT_PAREN (position 9/10): 8
- After RIGHT_PAREN (position 21/22): 8

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
4. **complex_function.k**:
```k
(distance) . (5 10 2 10)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
5. **cut_vector.k**:
```k
(0 2 4) _ (0 1 2 3 4 5 6 7)
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
6. **dictionary_index.k**:
```k
(.((`a;1);(`b;2))) @ `a
```
After SYMBOL (position 18/19)
-------------------------------------------------
7. **dictionary_index_attr.k**:
```k
(.((`a;1);(`b;2;.((`c;3);(`d;4))))) @ `b.
```
After SYMBOL (position 33/34)
-------------------------------------------------
8. **dictionary_index_value.k**:
```k
(.((`a;1);(`b;2))) @ `a
```
After SYMBOL (position 18/19)
-------------------------------------------------
9. **dictionary_index_value2.k**:
```k
(.((`a;1);(`b;2))) @ `b
```
After SYMBOL (position 18/19)
-------------------------------------------------
10. **dictionary_make_symbol_vector.k**:
```k
.,`a`b
```
After SYMBOL (position 4/5)
-------------------------------------------------
11. **dictionary_period_index_all_attributes.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
12. **test_minimal_dict.k**:
```k
d: .,(`a;1;.,(`x;2));d[.]
```
After RIGHT_PAREN (position 17/23)
-------------------------------------------------
13. **test_simple_period.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[.]
```
After RIGHT_PAREN (position 31/37)
-------------------------------------------------
14. **test_attr_access.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[`b.]
```
After RIGHT_PAREN (position 31/37)
-------------------------------------------------
15. **test_dict_create.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d
```
After RIGHT_PAREN (position 31/34)
-------------------------------------------------
16. **test_show_dict.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d
```
After RIGHT_PAREN (position 88/91)
-------------------------------------------------
17. **test_specific_attr.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
18. **test_specific_attr_fixed.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
19. **empty_char_vector.k**:
```k
0#"abc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
20. **empty_float_vector_test.k**:
```k
0#1.0 2.0 3.0
```
After FLOAT (position 5/6)
-------------------------------------------------
21. **empty_symbol_atomic.k**:
```k
0#`test
```
After SYMBOL (position 3/4)
-------------------------------------------------
22. **empty_list.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
23. **test_symbol_take.k**:
```k
0 # `test`
```
After SYMBOL (position 4/5)
-------------------------------------------------
24. **test_symbol_parsing.k**:
```k
0#`test`
```
After SYMBOL (position 4/5)
-------------------------------------------------
25. **drop_negative.k**:
```k
-4 _ 0 1 2 3 4 5 6 7
```
After INTEGER (position 10/11)
-------------------------------------------------
26. **drop_positive.k**:
```k
4 _ 0 1 2 3 4 5 6 7
```
After INTEGER (position 10/11)
-------------------------------------------------
27. **empty_mixed_vector.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
28. **function_add7.k**:
```k
(add7) . (5)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
29. **function_call_chain.k**:
```k
foo: (mul) . (8 4)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
30. **function_call_double.k**:
```k
(mul) . (8 4)
```
After RIGHT_PAREN (position 8/9)
-------------------------------------------------
31. **function_call_simple.k**:
```k
(add7) . (5)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
32. **function_foo_chain.k**:
```k
foo: (mul) . (8 4)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
33. **function_mul.k**:
```k
(mul) . (8 4)
```
After RIGHT_PAREN (position 8/9)
-------------------------------------------------
34. **lambda_string_assign.k**:
```k
{a:"hello";a}[]
```
After RIGHT_BRACKET (position 9/10)
-------------------------------------------------
35. **lambda_string_literal.k**:
```k
{"hello"}[]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
36. **lambda_symbol_literal.k**:
```k
{`abc}[]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
37. **join_operator.k**:
```k
3 , 5
```
After INTEGER (position 3/4)
-------------------------------------------------
38. **math_exp.k**:
```k
_exp 2
```
After INTEGER (position 2/3)
-------------------------------------------------
39. **math_log.k**:
```k
_log 10
```
After INTEGER (position 2/3)
-------------------------------------------------
40. **math_exp_basic.k**:
```k
_exp 1
```
After INTEGER (position 2/3)
-------------------------------------------------
41. **math_log_negative.k**:
```k
_log -1
```
After INTEGER (position 2/3)
-------------------------------------------------
42. **math_log_zero.k**:
```k
_log 0
```
After INTEGER (position 2/3)
-------------------------------------------------
43. **math_mul_matrix_2x2.k**:
```k
((1 2);(3 4)) _mul ((5 6);(7 8))
```
After RIGHT_PAREN (position 23/24)
-------------------------------------------------
44. **math_mul_matrix_2x3_3x2.k**:
```k
((1 2 3);(4 5 6)) _mul ((7 8);(9 10);(11 12))
```
After RIGHT_PAREN (position 30/31)
-------------------------------------------------
45. **math_mul_matrix_3x3.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) _mul ((9 8 7);(6 5 4);(3 2 1))
```
After RIGHT_PAREN (position 39/40)
-------------------------------------------------
46. **math_mul_matrix_4x2_2x4.k**:
```k
((1 2);(3 4);(5 6);(7 8)) _mul ((9 10 11 12);(13 14 15 16))
```
After RIGHT_PAREN (position 37/38)
-------------------------------------------------
47. **mixed_list_with_null.k**:
```k
(1;_n;`test;42.5)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
48. **mixed_vector_empty_position.k**:
```k
(1;;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
49. **mixed_vector_multiple_empty.k**:
```k
(1;;;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
50. **mixed_vector_whitespace_position.k**:
```k
(1; ;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
51. **nested_vector_test.k**:
```k
((1 2 3);(4 5 6))
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
52. **parenthesized_vector.k**:
```k
(1;2;3;4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
53. **divide_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
54. **divide_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
55. **divide_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
56. **divide_integer.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
57. **simple_nested_test.k**:
```k
(1 2 3)
```
After RIGHT_PAREN (position 5/6)
-------------------------------------------------
58. **minus_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
59. **minus_integer.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
60. **special_int_vector.k**:
```k
0I 0N -0I
```
After INTEGER (position 3/4)
-------------------------------------------------
61. **special_float_vector.k**:
```k
0i 0n -0i
```
After FLOAT (position 3/4)
-------------------------------------------------
62. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
63. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
64. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
65. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
66. **square_bracket_vector_single.k**:
```k
v[4]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
67. **symbol_vector_compact.k**:
```k
`a`b`c
```
After SYMBOL (position 3/4)
-------------------------------------------------
68. **symbol_vector_spaces.k**:
```k
`a `b `c
```
After SYMBOL (position 3/4)
-------------------------------------------------
69. **io_write_int.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_write.l" 1: 42
```
After INTEGER (position 3/4)
-------------------------------------------------
70. **io_roundtrip.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l" 1: (1;2.5;"hello")
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
71. **io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
72. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
73. **io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
74. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
75. **io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
76. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
77. **io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
78. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
79. **take_operator_basic.k**:
```k
3#1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
80. **take_operator_empty_float.k**:
```k
0#0.0
```
After FLOAT (position 3/4)
-------------------------------------------------
81. **take_operator_empty_symbol.k**:
```k
0#``
```
After SYMBOL (position 4/5)
-------------------------------------------------
82. **take_operator_overflow.k**:
```k
10#1 2 3
```
After INTEGER (position 5/6)
-------------------------------------------------
83. **take_operator_scalar.k**:
```k
3#42
```
After INTEGER (position 3/4)
-------------------------------------------------
84. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
85. **multiline_function_single.k**:
```k
test . 5
```
After INTEGER (position 3/4)
-------------------------------------------------
86. **null_vector.k**:
```k
(;1;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
87. **scoping_single.k**:
```k
globalVar: 100
```
After INTEGER (position 3/4)
-------------------------------------------------
88. **scoping_single.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
89. **semicolon_vars.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
90. **semicolon_vars.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
91. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
92. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
93. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
94. **single_no_semicolon.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
95. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
96. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
97. **variable_assignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
98. **variable_reassignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
99. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
100. **variable_scoping_global_access.k**:
```k
globalVar: 100  // Test function accessing global variable
```
After INTEGER (position 3/4)
-------------------------------------------------
101. **variable_scoping_global_access.k**:
```k
result1: test1 . 50
```
After INTEGER (position 5/6)
-------------------------------------------------
102. **variable_scoping_global_assignment.k**:
```k
globalVar: 100  // Test global assignment from nested function
```
After INTEGER (position 3/4)
-------------------------------------------------
103. **variable_scoping_global_assignment.k**:
```k
result5: test5 . 10
```
After INTEGER (position 5/6)
-------------------------------------------------
104. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100  // Test verify global variable unchanged
```
After INTEGER (position 3/4)
-------------------------------------------------
105. **variable_scoping_global_unchanged.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
106. **variable_scoping_local_hiding.k**:
```k
globalVar: 100  // Test function with local variable hiding global
```
After INTEGER (position 3/4)
-------------------------------------------------
107. **variable_scoping_local_hiding.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
108. **variable_scoping_nested_functions.k**:
```k
globalVar: 100  // Test nested functions
```
After INTEGER (position 3/4)
-------------------------------------------------
109. **variable_scoping_nested_functions.k**:
```k
result4: outer . 20
```
After INTEGER (position 5/6)
-------------------------------------------------
110. **variable_usage.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
111. **variable_usage.k**:
```k
y:20
```
After INTEGER (position 3/4)
-------------------------------------------------
112. **dot_execute_context.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
113. **dictionary_enumerate.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
114. **null_operations.k**:
```k
_n@7
```
After INTEGER (position 3/4)
-------------------------------------------------
115. **dictionary_dot_apply.k**:
```k
d: .((`a;1;.());(`b;2;.()))
```
After RIGHT_PAREN (position 24/25)
-------------------------------------------------
116. **dictionary_dot_apply.k**:
```k
d@`a
```
After SYMBOL (position 3/4)
-------------------------------------------------
117. **ci_adverb_vector.k**:
```k
_ci' 97 94 80
```
After INTEGER (position 5/6)
-------------------------------------------------
118. **dot_execute_variables.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
119. **dot_execute_variables.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
120. **format_braces_expressions.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
121. **format_braces_expressions.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
122. **format_braces_nested_expr.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
123. **format_braces_nested_expr.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
124. **format_braces_nested_expr.k**:
```k
z:3
```
After INTEGER (position 3/4)
-------------------------------------------------
125. **format_braces_complex.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
126. **format_braces_complex.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
127. **format_braces_complex.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
128. **format_braces_string.k**:
```k
name:"John"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
129. **format_braces_string.k**:
```k
age:25
```
After INTEGER (position 3/4)
-------------------------------------------------
130. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
After INTEGER (position 3/27)
-------------------------------------------------
131. **format_braces_simple.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
132. **format_braces_simple.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
133. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
After INTEGER (position 3/33)
-------------------------------------------------
134. **format_braces_nested_arith.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
135. **format_braces_nested_arith.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
136. **format_braces_nested_arith.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
137. **format_braces_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
138. **format_braces_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
139. **format_braces_float.k**:
```k
c:3.0
```
After FLOAT (position 3/4)
-------------------------------------------------
140. **format_braces_mixed_arith.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
141. **format_braces_mixed_arith.k**:
```k
y:3
```
After INTEGER (position 3/4)
-------------------------------------------------
142. **format_braces_mixed_arith.k**:
```k
z:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
143. **format_braces_example.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
144. **format_braces_example.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
145. **log.k**:
```k
_log 10
```
After INTEGER (position 2/3)
-------------------------------------------------
146. **time_t.k**:
```k
r:_t
```
After TIME (position 3/4)
-------------------------------------------------
147. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
148. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 5/36)
-------------------------------------------------
149. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
150. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
151. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 6/37)
-------------------------------------------------
152. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
153. **list_dv_basic.k**:
```k
3 4 4 5 _dv 4
```
After INTEGER (position 6/7)
-------------------------------------------------
154. **list_dv_nomatch.k**:
```k
3 4 4 5 _dv 6
```
After INTEGER (position 6/7)
-------------------------------------------------
155. **list_di_basic.k**:
```k
3 2 4 5 _di 1
```
After INTEGER (position 6/7)
-------------------------------------------------
156. **list_di_multiple.k**:
```k
3 2 4 5 _di 1 3
```
After INTEGER (position 7/8)
-------------------------------------------------
157. **list_sv_base10.k**:
```k
10 _sv 1 9 9 5
```
After INTEGER (position 6/7)
-------------------------------------------------
158. **list_sv_base2.k**:
```k
2 _sv 1 0 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
159. **list_sv_mixed.k**:
```k
10 10 10 10 _sv 1 9 9 5
```
After INTEGER (position 9/10)
-------------------------------------------------
160. **list_setenv.k**:
```k
`TESTVAR _setenv "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
161. **test_vs_dyadic.k**:
```k
10 _vs 1995
```
After INTEGER (position 3/4)
-------------------------------------------------
162. **test_sm_basic.k**:
```k
`foo _sm `foo
```
After SYMBOL (position 3/4)
-------------------------------------------------
163. **test_sm_simple.k**:
```k
`a _sm `a
```
After SYMBOL (position 3/4)
-------------------------------------------------
164. **test_ss_basic.k**:
```k
"hello world" _ss "world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
165. **io_write_basic.k**:
```k
`testW 0: ("line1";"line2";"line3");0:`testW
```
After RIGHT_PAREN (position 9/13)
-------------------------------------------------
166. **io_append_simple.k**:
```k
`testfile 5: "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
167. **io_append_basic.k**:
```k
`test 5: "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
168. **io_append_multiple.k**:
```k
`test 5: "hello"; `test 5: "world"; `test 5: 1 2 3
```
After CHARACTER_VECTOR (position 3/14)
-------------------------------------------------
169. **io_read_bytes_basic.k**:
```k
`test 0:,"hello";6:`test
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
170. **io_read_bytes_empty.k**:
```k
`empty 0:(); 6:`empty
```
After RIGHT_PAREN (position 4/8)
-------------------------------------------------
171. **io_write_bytes_basic.k**:
```k
`test 6:,"ABC"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
172. **io_write_bytes_overwrite.k**:
```k
`test 6:,"ABC"; `test 6:,"XYZ"
```
After CHARACTER_VECTOR (position 4/10)
-------------------------------------------------
173. **io_write_bytes_binary.k**:
```k
`test 6:(0 1 2 255)
```
After RIGHT_PAREN (position 8/9)
-------------------------------------------------
174. **search_bin_basic.k**:
```k
3 4 5 6 _bin 4
```
After INTEGER (position 6/7)
-------------------------------------------------
175. **search_binl_eachleft.k**:
```k
1 3 5 _binl 1 2 3 4 5
```
After INTEGER (position 9/10)
-------------------------------------------------
176. **search_lin_intersection.k**:
```k
1 3 5 7 9 _lin 1 2 3 4 5
```
After INTEGER (position 11/12)
-------------------------------------------------
177. **vector_notation_empty.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
178. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
179. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
180. **vector_notation_single_group.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
181. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
182. **vector_notation_variables.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
183. **vector_notation_variables.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
184. **vector_index_duplicate.k**:
```k
5 8 4 9 @ 0 0
```
After INTEGER (position 7/8)
-------------------------------------------------
185. **vector_index_first.k**:
```k
5 8 4 9 @ 0
```
After INTEGER (position 6/7)
-------------------------------------------------
186. **vector_index_multiple.k**:
```k
5 8 4 9 @ 1 3
```
After INTEGER (position 7/8)
-------------------------------------------------
187. **vector_index_reverse.k**:
```k
5 8 4 9 @ 3 1
```
After INTEGER (position 7/8)
-------------------------------------------------
188. **vector_index_single.k**:
```k
5 8 4 9 @ 2
```
After INTEGER (position 6/7)
-------------------------------------------------
189. **vector_with_null.k**:
```k
(_n;1;2)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
190. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
191. **amend_test_anonymous_func.k**:
```k
(.).((1 2 3); 0; f; 10)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
192. **amend_test_func_var.k**:
```k
(.).((1 2 3); 0; f; 10)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
193. **dictionary_null_index.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
194. **dictionary_null_index.k**:
```k
d@_n
```
After NULL (position 3/4)
-------------------------------------------------
195. **dictionary_unmake.k**:
```k
d: .((`a;1);(`b;2)); result:. d; result
```
After RIGHT_PAREN (position 16/24)
-------------------------------------------------
196. **do_loop.k**:
```k
i: 0; do[3; i+: 1]  // Do loop - increment i 3 times
```
After INTEGER (position 3/13)
-------------------------------------------------
197. **empty_brackets_dictionary.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
198. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
199. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
200. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
201. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
202. **if_simple_test.k**:
```k
if[3; 42]  // If function with bracket notation
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
203. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]  // If statement - condition true
```
After INTEGER (position 3/15)
-------------------------------------------------
204. **isolated.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
205. **isolated.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
206. **modulo.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
207. **modulo.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
208. **string_parse.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
209. **string_parse.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
210. **k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
211. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
212. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo
```
After IDENTIFIER (position 4/5)
-------------------------------------------------
213. **k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
214. **k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
215. **k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
216. **k_tree_dictionary_assignment.k**:
```k
.k.dd
```
After IDENTIFIER (position 4/5)
-------------------------------------------------
217. **k_tree_test_bracket_indexing.k**:
```k
d: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 22/23)
-------------------------------------------------
218. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
219. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
220. **vector_null_index.k**:
```k
v@_n
```
After NULL (position 3/4)
-------------------------------------------------
221. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]  // Test while function with bracket notation
```
After INTEGER (position 3/15)
-------------------------------------------------
222. **while_safe_test.k**:
```k
i: 0
```
After INTEGER (position 3/4)
-------------------------------------------------
223. **serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
224. **serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
After SYMBOL (position 3/4)
-------------------------------------------------
225. **db_basic_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
226. **db_float.k**:
```k
_db _bd 3.14
```
After FLOAT (position 3/4)
-------------------------------------------------
227. **db_symbol.k**:
```k
_db _bd `test
```
After SYMBOL (position 3/4)
-------------------------------------------------
228. **db_int_vector.k**:
```k
_db _bd 1 2 3
```
After INTEGER (position 5/6)
-------------------------------------------------
229. **db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
After SYMBOL (position 5/6)
-------------------------------------------------
230. **db_char_vector.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
231. **db_null.k**:
```k
_db _bd 0N
```
After INTEGER (position 3/4)
-------------------------------------------------
232. **db_character.k**:
```k
_db _bd "a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
233. **db_float_simple.k**:
```k
_db _bd 1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
234. **db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
235. **db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
After FLOAT (position 5/6)
-------------------------------------------------
236. **db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
237. **db_symbol_simple.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
238. **db_enlist_single_int.k**:
```k
_db _bd ,5
```
After INTEGER (position 4/5)
-------------------------------------------------
239. **db_enlist_single_symbol.k**:
```k
_db _bd ,`test
```
After SYMBOL (position 4/5)
-------------------------------------------------
240. **db_enlist_single_string.k**:
```k
_db _bd ,"hello"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
241. **serialization_bd_integervector_edge_single.k**:
```k
_bd ,1
```
After INTEGER (position 3/4)
-------------------------------------------------
242. **serialization_bd_list_edge_null.k**:
```k
_bd ,_n
```
After NULL (position 3/4)
-------------------------------------------------
243. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
244. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
245. **db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
After FLOAT (position 7/8)
-------------------------------------------------
246. **db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
After INTEGER (position 12/13)
-------------------------------------------------
247. **db_string_hello.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
248. **db_symbol_hello.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
249. **db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
After SYMBOL (position 5/6)
-------------------------------------------------
250. **bd_enlist_single_int.k**:
```k
_bd ,5
```
After INTEGER (position 3/4)
-------------------------------------------------
251. **bd_enlist_single_string.k**:
```k
_bd ,"hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
252. **bd_enlist_single_symbol.k**:
```k
_bd ,`test
```
After SYMBOL (position 3/4)
-------------------------------------------------
253. **math_and_basic.k**:
```k
5 _and 3
```
After INTEGER (position 3/4)
-------------------------------------------------
254. **math_and_vector.k**:
```k
(5 6 3) _and (1 2 3)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
255. **math_div_float.k**:
```k
7 _div 2
```
After INTEGER (position 3/4)
-------------------------------------------------
256. **math_div_integer.k**:
```k
7 _div 3
```
After INTEGER (position 3/4)
-------------------------------------------------
257. **math_div_vector.k**:
```k
(7 14 21) _div 3
```
After INTEGER (position 7/8)
-------------------------------------------------
258. **adverb_chaining_join_each_left.k**:
```k
((1 2 3);(4 5 6);(7 8 9)),/:\:((9 8 7);(6 5 4);(3 2 1))
```
After RIGHT_PAREN (position 41/42)
-------------------------------------------------
259. **adverb_complex_string_each_left.k**:
```k
(("hello";"world.");("It's";"me";"ksharp.");("Have";"fun";"with";"me!")),/:\:"  "
```
After CHARACTER_VECTOR (position 29/30)
-------------------------------------------------
260. **join_each_left.k**:
```k
(1 2 3),\: (4 5 6)
```
After RIGHT_PAREN (position 12/13)
-------------------------------------------------
261. **test_nested_adverb.k**:
```k
1 2 3 ,/:\: 4 5 6
```
After INTEGER (position 9/10)
-------------------------------------------------
262. **math_lsq_non_square.k**:
```k
(7 8 9) _lsq (1 2 3;4 5 6)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
263. **math_lsq_high_rank.k**:
```k
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
```
After RIGHT_PAREN (position 18/19)
-------------------------------------------------
264. **math_lsq_complex.k**:
```k
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
265. **math_lsq_regression.k**:
```k
(1 2 3.0) _lsq (1 1 1.0;1 2 4.0)
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
266. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
267. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
268. **math_or_basic.k**:
```k
5 _or 3
```
After INTEGER (position 3/4)
-------------------------------------------------
269. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
270. **math_rot_basic.k**:
```k
8 _rot 2
```
After INTEGER (position 3/4)
-------------------------------------------------
271. **math_shift_basic.k**:
```k
8 _shift 2
```
After INTEGER (position 3/4)
-------------------------------------------------
272. **math_shift_vector.k**:
```k
(8 16 32 64) _shift 2
```
After INTEGER (position 8/9)
-------------------------------------------------
273. **math_xor_basic.k**:
```k
5 _xor 3
```
After INTEGER (position 3/4)
-------------------------------------------------
274. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
275. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
276. **ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
After SYMBOL (position 5/8)
-------------------------------------------------
277. **ffi_assembly_load.k**:
```k
"System.Private.CoreLib" 2: `System.String
```
After SYMBOL (position 3/4)
-------------------------------------------------
278. **ffi_type_marshalling.k**:
```k
3.14159 _hint `float
```
After SYMBOL (position 3/4)
-------------------------------------------------
279. **ffi_type_marshalling.k**:
```k
"hello" _hint `string
```
After SYMBOL (position 3/4)
-------------------------------------------------
280. **ffi_type_marshalling.k**:
```k
1 2 3 4 5 _hint `list
```
After SYMBOL (position 7/8)
-------------------------------------------------
281. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
After CHARACTER_VECTOR (position 3/12)
-------------------------------------------------
282. **ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
283. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
284. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
285. **ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
286. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
287. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
288. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
289. **ffi_dispose.k**:
```k
c1 @ `_this
```
After SYMBOL (position 3/4)
-------------------------------------------------
290. **ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
291. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
292. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
293. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
294. **ffi_complete_workflow.k**:
```k
conj_func: ._dotnet.System.Numerics.Complex.Conjugate
```
After IDENTIFIER (position 12/13)
-------------------------------------------------
295. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
296. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
297. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
298. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
299. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
300. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
301. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
302. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
303. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
304. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
305. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
306. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
307. **idioms_01_624_zero_array.k**:
```k
x:2 3#99
```
After INTEGER (position 6/7)
-------------------------------------------------
308. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
309. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
310. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
311. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
312. **idioms_01_411_number_rows.k**:
```k
x:2 7#" "
```
After CHARACTER (position 6/7)
-------------------------------------------------
313. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
314. **test_eval_verb.k**:
```k
_eval ("+", 1, 2)
```
After RIGHT_PAREN (position 8/9)
-------------------------------------------------
315. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
316. **idioms_01_388_drop_rows.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
317. **idioms_01_388_drop_rows.k**:
```k
y _ x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
318. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
319. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
320. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
321. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
322. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
323. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
324. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
325. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
326. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
327. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
328. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
329. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
330. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
331. **idioms_01_434_replace_first.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
332. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
333. **idioms_01_433_replace_last.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
334. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
335. **idioms_01_406_add_last.k**:
```k
y:100
```
After INTEGER (position 3/4)
-------------------------------------------------
336. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
337. **idioms_01_449_limit_between.k**:
```k
l:30
```
After INTEGER (position 3/4)
-------------------------------------------------
338. **idioms_01_449_limit_between.k**:
```k
h:70
```
After INTEGER (position 3/4)
-------------------------------------------------
339. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
340. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
341. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
342. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
343. **idioms_01_504_replace_satisfying.k**:
```k
g:" "
```
After CHARACTER (position 3/4)
-------------------------------------------------
344. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
345. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
346. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
347. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
348. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
349. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
350. **idioms_01_616_scalar_from_vector.k**:
```k
x:,8
```
After INTEGER (position 4/5)
-------------------------------------------------
351. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
352. **idioms_01_509_remove_y.k**:
```k
y:"a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
353. **idioms_01_509_remove_y.k**:
```k
x _dv y
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
354. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
355. **idioms_01_510_remove_blanks.k**:
```k
x _dv " "
```
After CHARACTER (position 3/4)
-------------------------------------------------
356. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
357. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
358. **idioms_01_496_remove_punctuation.k**:
```k
x _dvl y
```
After IDENTIFIER (position 4/5)
-------------------------------------------------
359. **idioms_01_177_string_search.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
360. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
361. **idioms_01_177_string_search.k**:
```k
y _ss x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
362. **idioms_01_45_binary_representation.k**:
```k
x:16
```
After INTEGER (position 3/4)
-------------------------------------------------
363. **idioms_01_45_binary_representation.k**:
```k
2 _vs x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
364. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
365. **idioms_01_84_scalar_boolean.k**:
```k
2 _sv x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
366. **idioms_01_129_arctangent.k**:
```k
x:_sqrt[3]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
367. **idioms_01_129_arctangent.k**:
```k
y:1
```
After INTEGER (position 3/4)
-------------------------------------------------
368. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
369. **idioms_01_241_sum_subsets.k**:
```k
y:4 3#1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
370. **idioms_01_241_sum_subsets.k**:
```k
x _mul y
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
371. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
After INTEGER (position 3/4)
-------------------------------------------------
372. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
373. **idioms_01_384_drop_1st_postpend.k**:
```k
1 _ x,0
```
After INTEGER (position 5/6)
-------------------------------------------------
374. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
375. **idioms_01_385_drop_last_prepend.k**:
```k
-1 _ 0,x
```
After IDENTIFIER (position 5/6)
-------------------------------------------------
376. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
377. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
378. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
379. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
380. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
After INTEGER (position 3/4)
-------------------------------------------------
381. **ktree_enumerate_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
382. **ktree_enumerate_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
383. **ktree_enumerate_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
384. **ktree_indexing_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
385. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
386. **ktree_indexing_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
387. **ktree_indexing_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
388. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
389. **ktree_indexing_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
390. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
391. **ktree_dot_apply_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); d . `keyA
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
392. **ktree_dot_apply_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
393. **ktree_dot_apply_absolute_name.k**:
```k
.k.d . `keyB
```
After SYMBOL (position 6/7)
-------------------------------------------------
394. **ktree_dot_apply_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `d . `keyA
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
395. **ktree_dot_apply_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `.k.d . `keyB
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
396. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
397. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
398. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
399. **test_eval_monadic_star_atomic.k**:
```k
_eval (`"*:";,1)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
400. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
401. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
402. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
403. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
404. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
405. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
406. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
407. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------
408. **eval_dot_execute_path.k**:
```k
_eval (`",";,`a`b`c;(`",";,`d;`v)) // `v is interpreted as a path into the current K tree
```
After RIGHT_PAREN (position 18/19)
-------------------------------------------------
409. **eval_dot_parse_and_eval.k**:
```k
a:7
```
After INTEGER (position 3/4)
-------------------------------------------------
410. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
411. **test_eval_monadic_star_atomic.k**:
```k
_eval (`"*:";,1)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
