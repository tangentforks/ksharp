# K3CSharp Parser Failures

**Generated:** 2026-03-25 00:52:35
**Test Results:** 813/844 passed (96.3%)

## Executive Summary

**Total Tests:** 844
**Passed Tests:** 813
**Failed Tests:** 31
**Success Rate:** 96.3%

**LRS Parser Statistics:**
- NULL Results: 365
- Incorrect Results: 0
- LRS Success Rate: 56.8%

**Top Failure Patterns:**
- After INTEGER (position 3/4): 61
- Start of expression: 34
- After CHARACTER_VECTOR (position 3/4): 23
- After INTEGER (position 6/7): 19
- After FLOAT (position 3/4): 15
- After INTEGER (position 7/8): 13
- After INTEGER (position 5/6): 10
- After RIGHT_BRACE (position 12/13): 10
- After RIGHT_BRACKET (position 4/5): 9
- After CHARACTER_VECTOR (position 2/3): 9

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
distance:{[d0;v;a;t] d1:v*t; d2:a*t*t%2; d0+d1+d2}
```
After RIGHT_BRACE (position 34/35)
-------------------------------------------------
5. **dictionary_period_index_all_attributes.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
6. **test_minimal_dict.k**:
```k
d: .,(`a;1;.,(`x;2));d[.]
```
After RIGHT_PAREN (position 17/23)
-------------------------------------------------
7. **test_simple_period.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[.]
```
After RIGHT_PAREN (position 31/37)
-------------------------------------------------
8. **test_attr_access.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[`b.]
```
After RIGHT_PAREN (position 31/37)
-------------------------------------------------
9. **test_dict_create.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d
```
After RIGHT_PAREN (position 31/34)
-------------------------------------------------
10. **test_show_dict.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d
```
After RIGHT_PAREN (position 88/91)
-------------------------------------------------
11. **test_specific_attr.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
12. **test_specific_attr_fixed.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
13. **empty_list.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
14. **empty_mixed_vector.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
15. **function_add7.k**:
```k
add7:{[arg1] arg1+7}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
16. **function_call_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
17. **function_call_chain.k**:
```k
foo: (mul) . (8 4)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
18. **function_call_double.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
19. **function_call_simple.k**:
```k
add7:{[arg1] arg1+7}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
20. **function_foo_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
21. **function_foo_chain.k**:
```k
foo: (mul) . (8 4)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
22. **function_mul.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
23. **lambda_string_assign.k**:
```k
{a:"hello";a}[]
```
After RIGHT_BRACKET (position 9/10)
-------------------------------------------------
24. **lambda_string_literal.k**:
```k
{"hello"}[]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
25. **lambda_symbol_literal.k**:
```k
{`abc}[]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
26. **named_function_over.k**:
```k
f:{x*%y};f/10 20 30
```
After RIGHT_BRACE (position 8/15)
-------------------------------------------------
27. **named_function_scan.k**:
```k
f:{x*%y};f\10 20 30
```
After RIGHT_BRACE (position 8/15)
-------------------------------------------------
28. **mixed_list_with_null.k**:
```k
(1;_n;`test;42.5)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
29. **mixed_vector_empty_position.k**:
```k
(1;;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
30. **mixed_vector_multiple_empty.k**:
```k
(1;;;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
31. **mixed_vector_whitespace_position.k**:
```k
(1; ;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
32. **nested_vector_test.k**:
```k
((1 2 3);(4 5 6))
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
33. **parenthesized_vector.k**:
```k
(1;2;3;4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
34. **divide_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
35. **divide_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
36. **divide_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
37. **divide_integer.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
38. **simple_nested_test.k**:
```k
(1 2 3)
```
After RIGHT_PAREN (position 5/6)
-------------------------------------------------
39. **minus_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
40. **minus_integer.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
41. **test_triadic_dot.k**:
```k
.[1 2 3;1;-:]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
42. **special_int_vector.k**:
```k
0I 0N -0I
```
After INTEGER (position 3/4)
-------------------------------------------------
43. **special_float_vector.k**:
```k
0i 0n -0i
```
After FLOAT (position 3/4)
-------------------------------------------------
44. **square_bracket_function.k**:
```k
div:{[op1;op2] op1%op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
45. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
46. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
47. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
48. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
49. **square_bracket_vector_single.k**:
```k
v[4]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
50. **symbol_vector_compact.k**:
```k
`a`b`c
```
After SYMBOL (position 3/4)
-------------------------------------------------
51. **symbol_vector_spaces.k**:
```k
`a `b `c
```
After SYMBOL (position 3/4)
-------------------------------------------------
52. **io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
53. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
54. **io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
55. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
56. **io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
57. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
58. **io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
59. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
60. **mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
61. **multiline_function_single.k**:
```k
test: {[x] a:x*2; a+10}
```
After RIGHT_BRACE (position 16/17)
-------------------------------------------------
62. **null_vector.k**:
```k
(;1;2)
```
After RIGHT_PAREN (position 6/7)
-------------------------------------------------
63. **scoping_single.k**:
```k
globalVar: 100
```
After INTEGER (position 3/4)
-------------------------------------------------
64. **scoping_single.k**:
```k
test2: {[x] globalVar: x * 2; globalVar + 10}
```
After RIGHT_BRACE (position 16/17)
-------------------------------------------------
65. **scoping_single.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
66. **semicolon_vars.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
67. **semicolon_vars.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
68. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
69. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
70. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
71. **single_no_semicolon.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
72. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
73. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
74. **variable_assignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
75. **variable_reassignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
76. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
77. **variable_scoping_global_access.k**:
```k
globalVar: 100  // Test function accessing global variable
```
After INTEGER (position 3/4)
-------------------------------------------------
78. **variable_scoping_global_access.k**:
```k
test1: {[x] globalVar + x}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
79. **variable_scoping_global_access.k**:
```k
result1: test1 . 50
```
After INTEGER (position 5/6)
-------------------------------------------------
80. **variable_scoping_global_assignment.k**:
```k
globalVar: 100  // Test global assignment from nested function
```
After INTEGER (position 3/4)
-------------------------------------------------
81. **variable_scoping_global_assignment.k**:
```k
test5: {[x]
inner: {[y] globalVar :: x + y}
inner . x * 2
globalVar}
```
After RIGHT_BRACE (position 28/29)
-------------------------------------------------
82. **variable_scoping_global_assignment.k**:
```k
result5: test5 . 10
```
After INTEGER (position 5/6)
-------------------------------------------------
83. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100  // Test verify global variable unchanged
```
After INTEGER (position 3/4)
-------------------------------------------------
84. **variable_scoping_global_unchanged.k**:
```k
test2: {[x]
globalVar: x * 2
globalVar + 10
}
```
After RIGHT_BRACE (position 18/19)
-------------------------------------------------
85. **variable_scoping_global_unchanged.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
86. **variable_scoping_local_hiding.k**:
```k
globalVar: 100  // Test function with local variable hiding global
```
After INTEGER (position 3/4)
-------------------------------------------------
87. **variable_scoping_local_hiding.k**:
```k
test2: {[x]
globalVar: x * 2
globalVar + 10}
```
After RIGHT_BRACE (position 17/18)
-------------------------------------------------
88. **variable_scoping_local_hiding.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
89. **variable_scoping_nested_functions.k**:
```k
globalVar: 100  // Test nested functions
```
After INTEGER (position 3/4)
-------------------------------------------------
90. **variable_scoping_nested_functions.k**:
```k
outer: {[x]
inner: {[y] globalVar + x + y}
inner . x}
```
After RIGHT_BRACE (position 24/25)
-------------------------------------------------
91. **variable_scoping_nested_functions.k**:
```k
result4: outer . 20
```
After INTEGER (position 5/6)
-------------------------------------------------
92. **variable_usage.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
93. **variable_usage.k**:
```k
y:20
```
After INTEGER (position 3/4)
-------------------------------------------------
94. **variable_usage.k**:
```k
z:x+y
```
After IDENTIFIER (position 5/6)
-------------------------------------------------
95. **dot_execute_context.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
96. **dictionary_enumerate.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
97. **dictionary_dot_apply.k**:
```k
d: .((`a;1;.());(`b;2;.()))
```
After RIGHT_PAREN (position 24/25)
-------------------------------------------------
98. **adverb_each_count.k**:
```k
#:' (1 2 3;1 2 3 4 5; 1 2)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
99. **dot_execute_variables.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
100. **dot_execute_variables.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
101. **format_braces_expressions.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
102. **format_braces_expressions.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
103. **format_braces_nested_expr.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
104. **format_braces_nested_expr.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
105. **format_braces_nested_expr.k**:
```k
z:3
```
After INTEGER (position 3/4)
-------------------------------------------------
106. **format_braces_complex.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
107. **format_braces_complex.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
108. **format_braces_complex.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
109. **format_braces_string.k**:
```k
name:"John"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
110. **format_braces_string.k**:
```k
age:25
```
After INTEGER (position 3/4)
-------------------------------------------------
111. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
After INTEGER (position 3/27)
-------------------------------------------------
112. **format_braces_simple.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
113. **format_braces_simple.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
114. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
After INTEGER (position 3/33)
-------------------------------------------------
115. **format_braces_nested_arith.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
116. **format_braces_nested_arith.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
117. **format_braces_nested_arith.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
118. **format_braces_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
119. **format_braces_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
120. **format_braces_float.k**:
```k
c:3.0
```
After FLOAT (position 3/4)
-------------------------------------------------
121. **format_braces_mixed_arith.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
122. **format_braces_mixed_arith.k**:
```k
y:3
```
After INTEGER (position 3/4)
-------------------------------------------------
123. **format_braces_mixed_arith.k**:
```k
z:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
124. **format_braces_example.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
125. **format_braces_example.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
126. **format_braces_function_calls.k**:
```k
sum:{[a;b] a+b}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
127. **format_braces_function_calls.k**:
```k
product:{[x;y] x*y}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
128. **format_braces_function_calls.k**:
```k
double:{[x] x*2}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
129. **format_braces_nested_function_calls.k**:
```k
sum:{[a;b] a+b}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
130. **format_braces_nested_function_calls.k**:
```k
double:{[x] x*2}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
131. **format_braces_nested_function_calls.k**:
```k
square:{[x] x*x}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
132. **time_t.k**:
```k
r:_t
```
After TIME (position 3/4)
-------------------------------------------------
133. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
134. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 5/36)
-------------------------------------------------
135. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
136. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
137. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 6/37)
-------------------------------------------------
138. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
139. **time_gtime.k**:
```k
_gtime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
140. **time_ltime.k**:
```k
_ltime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
141. **assignment_lrs_return_value.k**:
```k
b:2*a:47;(a;b)
```
After INTEGER (position 7/14)
-------------------------------------------------
142. **list_getenv.k**:
```k
_getenv "PROMPT"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
143. **list_size_existing.k**:
```k
_size "C:\\Windows\\System32\\write.exe"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
144. **test_monadic_colon.k**:
```k
: 42
```
After INTEGER (position 2/3)
-------------------------------------------------
145. **statement_assignment_basic.k**:
```k
a: 42
```
After INTEGER (position 3/4)
-------------------------------------------------
146. **statement_assignment_inline.k**:
```k
1 + a: 42
```
After INTEGER (position 5/6)
-------------------------------------------------
147. **statement_do_basic.k**:
```k
i:0;do[3;i+:1];i
```
After INTEGER (position 3/15)
-------------------------------------------------
148. **statement_do_simple.k**:
```k
i:0;do[3;i+:1]
```
After INTEGER (position 3/13)
-------------------------------------------------
149. **semicolon_vars_test.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
150. **apply_and_assign_simple.k**:
```k
i:0;i+:1;i
```
After INTEGER (position 3/10)
-------------------------------------------------
151. **apply_and_assign_multiline.k**:
```k
i:0
```
After INTEGER (position 3/4)
-------------------------------------------------
152. **apply_and_assign_multiline.k**:
```k
i+:1
```
After INTEGER (position 3/4)
-------------------------------------------------
153. **vector_notation_empty.k**:
```k
()
```
After RIGHT_PAREN (position 2/3)
-------------------------------------------------
154. **vector_notation_functions.k**:
```k
double: {[x] x * 2}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
155. **vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
156. **vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
157. **vector_notation_single_group.k**:
```k
(42)
```
After RIGHT_PAREN (position 3/4)
-------------------------------------------------
158. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
159. **vector_notation_variables.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
160. **vector_notation_variables.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
161. **vector_with_null.k**:
```k
(_n;1;2)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
162. **vector_with_null_middle.k**:
```k
(1;_n;3)
```
After RIGHT_PAREN (position 7/8)
-------------------------------------------------
163. **amend_test_anonymous_func.k**:
```k
f:{x+y}
```
After RIGHT_BRACE (position 7/8)
-------------------------------------------------
164. **amend_test_func_var.k**:
```k
f:{x+y}
```
After RIGHT_BRACE (position 7/8)
-------------------------------------------------
165. **dictionary_null_index.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
166. **dictionary_unmake.k**:
```k
d: .((`a;1);(`b;2)); result:. d; result
```
After RIGHT_PAREN (position 16/24)
-------------------------------------------------
167. **do_loop.k**:
```k
i: 0; do[3; i+: 1]  // Do loop - increment i 3 times
```
After INTEGER (position 3/13)
-------------------------------------------------
168. **empty_brackets_dictionary.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
169. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
170. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
171. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
172. **format_braces_complex_expressions.k**:
```k
sum:{[a;b] a+b}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
173. **format_braces_complex_expressions.k**:
```k
product:{[x;y] x*y}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
174. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
175. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]  // If statement - condition true
```
After INTEGER (position 3/15)
-------------------------------------------------
176. **isolated.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
177. **isolated.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
178. **modulo.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
179. **modulo.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
180. **string_parse.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
181. **string_parse.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
182. **k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
183. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
184. **k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
185. **k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
186. **k_tree_nested_indexing.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
187. **k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
188. **k_tree_dictionary_assignment.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
189. **k_tree_test_bracket_indexing.k**:
```k
d: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 22/23)
-------------------------------------------------
190. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
191. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
192. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]  // Test while function with bracket notation
```
After INTEGER (position 3/15)
-------------------------------------------------
193. **while_safe_test.k**:
```k
i: 0
```
After INTEGER (position 3/4)
-------------------------------------------------
194. **serialization_bd_db_integer.k**:
```k
// Test _bd and _db for Integer serialization
```
Start of expression
-------------------------------------------------
195. **serialization_bd_db_character.k**:
```k
// Test _bd and _db for Character serialization
```
Start of expression
-------------------------------------------------
196. **serialization_bd_db_symbol.k**:
```k
// Test _bd and _db for Symbol serialization
```
Start of expression
-------------------------------------------------
197. **serialization_bd_db_null.k**:
```k
// Test _bd and _db for Null serialization
```
Start of expression
-------------------------------------------------
198. **serialization_bd_db_roundtrip_integer.k**:
```k
// Test _bd and _db round-trip serialization
```
Start of expression
-------------------------------------------------
199. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
200. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
201. **math_ceil_basic.k**:
```k
_ceil 4.7
```
After FLOAT (position 2/3)
-------------------------------------------------
202. **math_ceil_integer.k**:
```k
_ceil 5
```
After INTEGER (position 2/3)
-------------------------------------------------
203. **math_ceil_negative.k**:
```k
_ceil -3.2
```
After FLOAT (position 2/3)
-------------------------------------------------
204. **math_ceil_vector.k**:
```k
_ceil 1.2 2.7 3.5
```
After FLOAT (position 4/5)
-------------------------------------------------
205. **math_lsq_non_square.k**:
```k
// Non-square matrix: 2x3 matrix (more columns than rows)
```
Start of expression
-------------------------------------------------
206. **math_lsq_non_square.k**:
```k
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
```
Start of expression
-------------------------------------------------
207. **math_lsq_non_square.k**:
```k
// y = [1 2 3; 4 5 6], so y^T is 3x2
```
Start of expression
-------------------------------------------------
208. **math_lsq_non_square.k**:
```k
// x = [7; 8; 9] (length 3)
```
Start of expression
-------------------------------------------------
209. **math_lsq_non_square.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
210. **math_lsq_high_rank.k**:
```k
// Rectangular matrix: 2x4 matrix (more columns than rows)
```
Start of expression
-------------------------------------------------
211. **math_lsq_high_rank.k**:
```k
// Solving: y^T * w = x where y is 2x4, x is length 4, w is length 2
```
Start of expression
-------------------------------------------------
212. **math_lsq_high_rank.k**:
```k
// y = [1 2 3 4; 2 3 4 5], so y^T is 4x2
```
Start of expression
-------------------------------------------------
213. **math_lsq_high_rank.k**:
```k
// x = [10; 11; 12; 13] (length 4)
```
Start of expression
-------------------------------------------------
214. **math_lsq_high_rank.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
215. **math_lsq_complex.k**:
```k
// Complex non-square case with mixed values
```
Start of expression
-------------------------------------------------
216. **math_lsq_complex.k**:
```k
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
```
Start of expression
-------------------------------------------------
217. **math_lsq_complex.k**:
```k
// y = [1.5 2.0 3.0; 4.5 5.5 6.0], so y^T is 3x2
```
Start of expression
-------------------------------------------------
218. **math_lsq_complex.k**:
```k
// x = [7.5; 8.0; 9.5] (length 3)
```
Start of expression
-------------------------------------------------
219. **math_lsq_complex.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
220. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
221. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
222. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
223. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
224. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
225. **ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
After SYMBOL (position 5/8)
-------------------------------------------------
226. **ffi_assembly_load.k**:
```k
// Test basic FFI functionality
```
Start of expression
-------------------------------------------------
227. **ffi_assembly_load.k**:
```k
// Load System.Private.CoreLib assembly and access String type
```
Start of expression
-------------------------------------------------
228. **ffi_type_marshalling.k**:
```k
f:3.14159;f _sethint `float;s:"hello";s _sethint `string;l:1 2 3 4 5;l: _sethint `list // This needs to be split at some point
```
After FLOAT (position 3/29)
-------------------------------------------------
229. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
After CHARACTER_VECTOR (position 3/12)
-------------------------------------------------
230. **ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
231. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
232. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
233. **ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
234. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
235. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
236. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
237. **ffi_complete_workflow.k**:
```k
// Complete FFI Workflow Test
```
Start of expression
-------------------------------------------------
238. **ffi_complete_workflow.k**:
```k
// This test covers the full FFI functionality from assembly loading to method invocation
```
Start of expression
-------------------------------------------------
239. **ffi_complete_workflow.k**:
```k
// Step 1: Load .NET assembly with Complex type
```
Start of expression
-------------------------------------------------
240. **ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
241. **ffi_complete_workflow.k**:
```k
// Step 2: Create constructor function
```
Start of expression
-------------------------------------------------
242. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
243. **ffi_complete_workflow.k**:
```k
// Step 3: Create object instance using constructor
```
Start of expression
-------------------------------------------------
244. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
245. **ffi_complete_workflow.k**:
```k
// Step 4: Call instance method (Abs - magnitude)
```
Start of expression
-------------------------------------------------
246. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
247. **ffi_complete_workflow.k**:
```k
// Step 5: Call property getter methods (not working)
```
Start of expression
-------------------------------------------------
248. **ffi_complete_workflow.k**:
```k
// real: c1[`get_Real][]
```
Start of expression
-------------------------------------------------
249. **ffi_complete_workflow.k**:
```k
// imag: c1[`get_Imaginary][]
```
Start of expression
-------------------------------------------------
250. **ffi_complete_workflow.k**:
```k
// Step 6: Call static method (Conjugate) (not working)
```
Start of expression
-------------------------------------------------
251. **ffi_complete_workflow.k**:
```k
conj_func: ._dotnet.System.Numerics.Complex.Conjugate
```
After IDENTIFIER (position 12/13)
-------------------------------------------------
252. **ffi_complete_workflow.k**:
```k
//conj_result: conj_func[c1]
```
Start of expression
-------------------------------------------------
253. **ffi_complete_workflow.k**:
```k
// Step 7: Display object information
```
Start of expression
-------------------------------------------------
254. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
255. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
256. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
257. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
258. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
259. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
260. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
261. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
262. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
263. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
264. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
265. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
266. **idioms_01_624_zero_array.k**:
```k
x:2 3#99
```
After INTEGER (position 6/7)
-------------------------------------------------
267. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
268. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
269. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
270. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
271. **idioms_01_411_number_rows.k**:
```k
x:2 7#" "
```
After CHARACTER (position 6/7)
-------------------------------------------------
272. **idioms_01_445_number_columns.k**:
```k
x:4 3#!12
```
After INTEGER (position 7/8)
-------------------------------------------------
273. **test_parse_verb.k**:
```k
_parse "1 + 2"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
274. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
275. **idioms_01_388_drop_rows.k**:
```k
x:6 3#!18
```
After INTEGER (position 7/8)
-------------------------------------------------
276. **idioms_01_388_drop_rows.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
277. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
278. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
279. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
280. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
281. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
282. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
283. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
284. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
285. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
286. **idioms_01_88_name_variable.k**:
```k
y:2 3#!6
```
After INTEGER (position 7/8)
-------------------------------------------------
287. **idioms_01_96_conditional_execution.k**:
```k
@[+/;!6;:]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
288. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
289. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
290. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
291. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
292. **idioms_01_434_replace_first.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
293. **idioms_01_434_replace_first.k**:
```k
@[x;0;:;y]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
294. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
295. **idioms_01_433_replace_last.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
296. **idioms_01_433_replace_last.k**:
```k
@[x;-1+#x;:;y]
```
After RIGHT_BRACKET (position 13/14)
-------------------------------------------------
297. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
298. **idioms_01_406_add_last.k**:
```k
y:100
```
After INTEGER (position 3/4)
-------------------------------------------------
299. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
300. **idioms_01_449_limit_between.k**:
```k
l:30
```
After INTEGER (position 3/4)
-------------------------------------------------
301. **idioms_01_449_limit_between.k**:
```k
h:70
```
After INTEGER (position 3/4)
-------------------------------------------------
302. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
303. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
304. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
305. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
306. **idioms_01_504_replace_satisfying.k**:
```k
g:" "
```
After CHARACTER (position 3/4)
-------------------------------------------------
307. **idioms_01_504_replace_satisfying.k**:
```k
@[y;&x;:;g]
```
After RIGHT_BRACKET (position 11/12)
-------------------------------------------------
308. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
309. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
310. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
311. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
312. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
313. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
314. **idioms_01_616_scalar_from_vector.k**:
```k
x:,8
```
After INTEGER (position 4/5)
-------------------------------------------------
315. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
316. **idioms_01_509_remove_y.k**:
```k
y:"a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
317. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
318. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
319. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
320. **idioms_01_177_string_search.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
321. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
322. **idioms_01_45_binary_representation.k**:
```k
x:16
```
After INTEGER (position 3/4)
-------------------------------------------------
323. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
324. **idioms_01_129_arctangent.k**:
```k
x:_sqrt[3]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
325. **idioms_01_129_arctangent.k**:
```k
y:1
```
After INTEGER (position 3/4)
-------------------------------------------------
326. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
327. **idioms_01_241_sum_subsets.k**:
```k
x:1+3 4#!12
```
After INTEGER (position 9/10)
-------------------------------------------------
328. **idioms_01_241_sum_subsets.k**:
```k
y:4 3#1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
329. **idioms_01_61_cyclic_counter.k**:
```k
x:!10
```
After INTEGER (position 4/5)
-------------------------------------------------
330. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
After INTEGER (position 3/4)
-------------------------------------------------
331. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
332. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
333. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
334. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
335. **idioms_01_447_conditional_drop.k**:
```k
x:4 3#!12
```
After INTEGER (position 7/8)
-------------------------------------------------
336. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
337. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
338. **idioms_01_448_conditional_drop_last.k**:
```k
x:4 3#!12
```
After INTEGER (position 7/8)
-------------------------------------------------
339. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
After INTEGER (position 3/4)
-------------------------------------------------
340. **ktree_enumerate_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
341. **ktree_enumerate_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
342. **ktree_enumerate_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
343. **ktree_indexing_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
344. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
345. **ktree_indexing_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
346. **ktree_indexing_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
347. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
348. **ktree_indexing_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
349. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
350. **ktree_dot_apply_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); d . `keyA
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
351. **ktree_dot_apply_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
352. **ktree_dot_apply_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `d . `keyA
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
353. **ktree_dot_apply_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `.k.d . `keyB
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
354. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
355. **test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
356. **parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
357. **parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
358. **parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
359. **parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
360. **parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
361. **eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
362. **eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
After RIGHT_PAREN (position 14/15)
-------------------------------------------------
363. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------
364. **eval_dot_parse_and_eval.k**:
```k
a:7
```
After INTEGER (position 3/4)
-------------------------------------------------
365. **test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
