# K3CSharp Parser Failures

**Generated:** 2026-03-26 01:45:00
**Test Results:** 821/852 passed (96.4%)

## Executive Summary

**Total Tests:** 852
**Passed Tests:** 821
**Failed Tests:** 31
**Success Rate:** 96.4%

**LRS Parser Statistics:**
- NULL Results: 333
- Incorrect Results: 0
- LRS Success Rate: 60.9%

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
- After RIGHT_PAREN (position 21/22): 8

## LRS Parser Failures

1. **lrs_atomic_parser_basic.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
2. **lrs_adverb_parser_basic.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
3. **complex_function.k**:
```k
distance:{[d0;v;a;t] d1:v*t; d2:a*t*t%2; d0+d1+d2}
```
After RIGHT_BRACE (position 34/35)
-------------------------------------------------
4. **dictionary_period_index_all_attributes.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
5. **test_minimal_dict.k**:
```k
d: .,(`a;1;.,(`x;2));d[.]
```
After RIGHT_PAREN (position 17/23)
-------------------------------------------------
6. **test_simple_period.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[.]
```
After RIGHT_PAREN (position 31/37)
-------------------------------------------------
7. **test_attr_access.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[`b.]
```
After RIGHT_PAREN (position 31/37)
-------------------------------------------------
8. **test_dict_create.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d
```
After RIGHT_PAREN (position 31/34)
-------------------------------------------------
9. **test_show_dict.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d
```
After RIGHT_PAREN (position 88/91)
-------------------------------------------------
10. **test_specific_attr.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
11. **test_specific_attr_fixed.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
After RIGHT_PAREN (position 88/94)
-------------------------------------------------
12. **function_add7.k**:
```k
add7:{[arg1] arg1+7}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
13. **function_call_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
14. **function_call_chain.k**:
```k
foo: (mul) . (8 4)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
15. **function_call_double.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
16. **function_call_simple.k**:
```k
add7:{[arg1] arg1+7}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
17. **function_foo_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
18. **function_foo_chain.k**:
```k
foo: (mul) . (8 4)
```
After RIGHT_PAREN (position 10/11)
-------------------------------------------------
19. **function_mul.k**:
```k
mul:{[op1;op2] op1 * op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
20. **named_function_over.k**:
```k
f:{x*%y};f/10 20 30
```
After RIGHT_BRACE (position 8/15)
-------------------------------------------------
21. **named_function_scan.k**:
```k
f:{x*%y};f\10 20 30
```
After RIGHT_BRACE (position 8/15)
-------------------------------------------------
22. **divide_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
23. **divide_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
24. **divide_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
25. **divide_integer.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
26. **minus_integer.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
27. **minus_integer.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
28. **test_triadic_dot.k**:
```k
.[1 2 3;1;-:]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
29. **special_int_vector.k**:
```k
0I 0N -0I
```
After INTEGER (position 3/4)
-------------------------------------------------
30. **special_float_vector.k**:
```k
0i 0n -0i
```
After FLOAT (position 3/4)
-------------------------------------------------
31. **square_bracket_function.k**:
```k
div:{[op1;op2] op1%op2}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
32. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
33. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
34. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
35. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
36. **square_bracket_vector_single.k**:
```k
v[4]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
37. **symbol_vector_compact.k**:
```k
`a`b`c
```
After SYMBOL (position 3/4)
-------------------------------------------------
38. **symbol_vector_spaces.k**:
```k
`a `b `c
```
After SYMBOL (position 3/4)
-------------------------------------------------
39. **io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
40. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
41. **io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
42. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
43. **io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
44. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
45. **io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
After CHARACTER_VECTOR (position 4/5)
-------------------------------------------------
46. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
47. **multiline_function_single.k**:
```k
test: {[x] a:x*2; a+10}
```
After RIGHT_BRACE (position 16/17)
-------------------------------------------------
48. **scoping_single.k**:
```k
globalVar: 100
```
After INTEGER (position 3/4)
-------------------------------------------------
49. **scoping_single.k**:
```k
test2: {[x] globalVar: x * 2; globalVar + 10}
```
After RIGHT_BRACE (position 16/17)
-------------------------------------------------
50. **scoping_single.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
51. **semicolon_vars.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
52. **semicolon_vars.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
53. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
54. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
55. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
56. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
57. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
58. **variable_assignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
59. **variable_reassignment.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
60. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
61. **variable_scoping_global_access.k**:
```k
globalVar: 100  // Test function accessing global variable
```
After INTEGER (position 3/4)
-------------------------------------------------
62. **variable_scoping_global_access.k**:
```k
test1: {[x] globalVar + x}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
63. **variable_scoping_global_access.k**:
```k
result1: test1 . 50
```
After INTEGER (position 5/6)
-------------------------------------------------
64. **variable_scoping_global_assignment.k**:
```k
globalVar: 100  // Test global assignment from nested function
```
After INTEGER (position 3/4)
-------------------------------------------------
65. **variable_scoping_global_assignment.k**:
```k
test5: {[x]
inner: {[y] globalVar :: x + y}
inner . x * 2
globalVar}
```
After RIGHT_BRACE (position 28/29)
-------------------------------------------------
66. **variable_scoping_global_assignment.k**:
```k
result5: test5 . 10
```
After INTEGER (position 5/6)
-------------------------------------------------
67. **variable_scoping_global_unchanged.k**:
```k
globalVar: 100  // Test verify global variable unchanged
```
After INTEGER (position 3/4)
-------------------------------------------------
68. **variable_scoping_global_unchanged.k**:
```k
test2: {[x]
globalVar: x * 2
globalVar + 10
}
```
After RIGHT_BRACE (position 18/19)
-------------------------------------------------
69. **variable_scoping_global_unchanged.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
70. **variable_scoping_local_hiding.k**:
```k
globalVar: 100  // Test function with local variable hiding global
```
After INTEGER (position 3/4)
-------------------------------------------------
71. **variable_scoping_local_hiding.k**:
```k
test2: {[x]
globalVar: x * 2
globalVar + 10}
```
After RIGHT_BRACE (position 17/18)
-------------------------------------------------
72. **variable_scoping_local_hiding.k**:
```k
result2: test2 . 25
```
After INTEGER (position 5/6)
-------------------------------------------------
73. **variable_scoping_nested_functions.k**:
```k
globalVar: 100  // Test nested functions
```
After INTEGER (position 3/4)
-------------------------------------------------
74. **variable_scoping_nested_functions.k**:
```k
outer: {[x]
inner: {[y] globalVar + x + y}
inner . x}
```
After RIGHT_BRACE (position 24/25)
-------------------------------------------------
75. **variable_scoping_nested_functions.k**:
```k
result4: outer . 20
```
After INTEGER (position 5/6)
-------------------------------------------------
76. **variable_usage.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
77. **variable_usage.k**:
```k
y:20
```
After INTEGER (position 3/4)
-------------------------------------------------
78. **variable_usage.k**:
```k
z:x+y
```
After IDENTIFIER (position 5/6)
-------------------------------------------------
79. **dot_execute_context.k**:
```k
foo:7
```
After INTEGER (position 3/4)
-------------------------------------------------
80. **dictionary_enumerate.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
81. **dictionary_dot_apply.k**:
```k
d: .((`a;1;.());(`b;2;.()))
```
After RIGHT_PAREN (position 24/25)
-------------------------------------------------
82. **adverb_each_count.k**:
```k
#:' (1 2 3;1 2 3 4 5; 1 2)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
83. **dot_execute_variables.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
84. **dot_execute_variables.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
85. **format_braces_expressions.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
86. **format_braces_expressions.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
87. **format_braces_nested_expr.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
88. **format_braces_nested_expr.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
89. **format_braces_nested_expr.k**:
```k
z:3
```
After INTEGER (position 3/4)
-------------------------------------------------
90. **format_braces_complex.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
91. **format_braces_complex.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
92. **format_braces_complex.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
93. **format_braces_string.k**:
```k
name:"John"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
94. **format_braces_string.k**:
```k
age:25
```
After INTEGER (position 3/4)
-------------------------------------------------
95. **format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
After INTEGER (position 3/27)
-------------------------------------------------
96. **format_braces_simple.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
97. **format_braces_simple.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
98. **format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
After INTEGER (position 3/33)
-------------------------------------------------
99. **format_braces_nested_arith.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
100. **format_braces_nested_arith.k**:
```k
b:2
```
After INTEGER (position 3/4)
-------------------------------------------------
101. **format_braces_nested_arith.k**:
```k
c:3
```
After INTEGER (position 3/4)
-------------------------------------------------
102. **format_braces_float.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
103. **format_braces_float.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
104. **format_braces_float.k**:
```k
c:3.0
```
After FLOAT (position 3/4)
-------------------------------------------------
105. **format_braces_mixed_arith.k**:
```k
x:10
```
After INTEGER (position 3/4)
-------------------------------------------------
106. **format_braces_mixed_arith.k**:
```k
y:3
```
After INTEGER (position 3/4)
-------------------------------------------------
107. **format_braces_mixed_arith.k**:
```k
z:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
108. **format_braces_example.k**:
```k
a:5
```
After INTEGER (position 3/4)
-------------------------------------------------
109. **format_braces_example.k**:
```k
b:3
```
After INTEGER (position 3/4)
-------------------------------------------------
110. **format_braces_function_calls.k**:
```k
sum:{[a;b] a+b}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
111. **format_braces_function_calls.k**:
```k
product:{[x;y] x*y}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
112. **format_braces_function_calls.k**:
```k
double:{[x] x*2}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
113. **format_braces_nested_function_calls.k**:
```k
sum:{[a;b] a+b}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
114. **format_braces_nested_function_calls.k**:
```k
double:{[x] x*2}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
115. **format_braces_nested_function_calls.k**:
```k
square:{[x] x*x}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
116. **time_t.k**:
```k
r:_t
```
After TIME (position 3/4)
-------------------------------------------------
117. **rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
118. **rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 5/36)
-------------------------------------------------
119. **rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 5/23)
-------------------------------------------------
120. **rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
121. **rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
After INTEGER (position 6/37)
-------------------------------------------------
122. **rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
After INTEGER (position 6/24)
-------------------------------------------------
123. **time_gtime.k**:
```k
_gtime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
124. **time_ltime.k**:
```k
_ltime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
125. **assignment_lrs_return_value.k**:
```k
b:2*a:47;(a;b)
```
After INTEGER (position 7/14)
-------------------------------------------------
126. **list_getenv.k**:
```k
_getenv "PROMPT"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
127. **list_size_existing.k**:
```k
_size "C:\\Windows\\System32\\write.exe"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
128. **test_monadic_colon.k**:
```k
: 42
```
After INTEGER (position 2/3)
-------------------------------------------------
129. **statement_assignment_basic.k**:
```k
a: 42
```
After INTEGER (position 3/4)
-------------------------------------------------
130. **statement_assignment_inline.k**:
```k
1 + a: 42
```
After INTEGER (position 5/6)
-------------------------------------------------
131. **statement_do_basic.k**:
```k
i:0;do[3;i+:1];i
```
After INTEGER (position 3/15)
-------------------------------------------------
132. **statement_do_simple.k**:
```k
i:0;do[3;i+:1]
```
After INTEGER (position 3/13)
-------------------------------------------------
133. **semicolon_vars_test.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
134. **apply_and_assign_simple.k**:
```k
i:0;i+:1;i
```
After INTEGER (position 3/10)
-------------------------------------------------
135. **apply_and_assign_multiline.k**:
```k
i:0
```
After INTEGER (position 3/4)
-------------------------------------------------
136. **apply_and_assign_multiline.k**:
```k
i+:1
```
After INTEGER (position 3/4)
-------------------------------------------------
137. **vector_notation_functions.k**:
```k
double: {[x] x * 2}
```
After RIGHT_BRACE (position 10/11)
-------------------------------------------------
138. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
139. **vector_notation_variables.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
140. **vector_notation_variables.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
141. **amend_test_anonymous_func.k**:
```k
f:{x+y}
```
After RIGHT_BRACE (position 7/8)
-------------------------------------------------
142. **amend_test_func_var.k**:
```k
f:{x+y}
```
After RIGHT_BRACE (position 7/8)
-------------------------------------------------
143. **dictionary_null_index.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
144. **dictionary_unmake.k**:
```k
d: .((`a;1);(`b;2)); result:. d; result
```
After RIGHT_PAREN (position 16/24)
-------------------------------------------------
145. **do_loop.k**:
```k
i: 0; do[3; i+: 1]  // Do loop - increment i 3 times
```
After INTEGER (position 3/13)
-------------------------------------------------
146. **empty_brackets_dictionary.k**:
```k
d: .((`a;1);(`b;2))
```
After RIGHT_PAREN (position 16/17)
-------------------------------------------------
147. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
148. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
149. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
150. **format_braces_complex_expressions.k**:
```k
sum:{[a;b] a+b}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
151. **format_braces_complex_expressions.k**:
```k
product:{[x;y] x*y}
```
After RIGHT_BRACE (position 12/13)
-------------------------------------------------
152. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
153. **if_true.k**:
```k
a: 10; if[1 < 2; a: 20]  // If statement - condition true
```
After INTEGER (position 3/15)
-------------------------------------------------
154. **isolated.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
155. **isolated.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
156. **modulo.k**:
```k
a:1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
157. **modulo.k**:
```k
b:2.5
```
After FLOAT (position 3/4)
-------------------------------------------------
158. **string_parse.k**:
```k
a: 10
```
After INTEGER (position 3/4)
-------------------------------------------------
159. **string_parse.k**:
```k
b: 20
```
After INTEGER (position 3/4)
-------------------------------------------------
160. **k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
161. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
162. **k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
163. **k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
164. **k_tree_nested_indexing.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
165. **k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
166. **k_tree_dictionary_assignment.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
167. **k_tree_test_bracket_indexing.k**:
```k
d: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 22/23)
-------------------------------------------------
168. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
169. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
170. **while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]  // Test while function with bracket notation
```
After INTEGER (position 3/15)
-------------------------------------------------
171. **while_safe_test.k**:
```k
i: 0
```
After INTEGER (position 3/4)
-------------------------------------------------
172. **serialization_bd_db_integer.k**:
```k
// Test _bd and _db for Integer serialization
```
Start of expression
-------------------------------------------------
173. **serialization_bd_db_character.k**:
```k
// Test _bd and _db for Character serialization
```
Start of expression
-------------------------------------------------
174. **serialization_bd_db_symbol.k**:
```k
// Test _bd and _db for Symbol serialization
```
Start of expression
-------------------------------------------------
175. **serialization_bd_db_null.k**:
```k
// Test _bd and _db for Null serialization
```
Start of expression
-------------------------------------------------
176. **serialization_bd_db_roundtrip_integer.k**:
```k
// Test _bd and _db round-trip serialization
```
Start of expression
-------------------------------------------------
177. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
178. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
179. **math_ceil_basic.k**:
```k
_ceil 4.7
```
After FLOAT (position 2/3)
-------------------------------------------------
180. **math_ceil_integer.k**:
```k
_ceil 5
```
After INTEGER (position 2/3)
-------------------------------------------------
181. **math_ceil_negative.k**:
```k
_ceil -3.2
```
After FLOAT (position 2/3)
-------------------------------------------------
182. **math_ceil_vector.k**:
```k
_ceil 1.2 2.7 3.5
```
After FLOAT (position 4/5)
-------------------------------------------------
183. **math_lsq_non_square.k**:
```k
// Non-square matrix: 2x3 matrix (more columns than rows)
```
Start of expression
-------------------------------------------------
184. **math_lsq_non_square.k**:
```k
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
```
Start of expression
-------------------------------------------------
185. **math_lsq_non_square.k**:
```k
// y = [1 2 3; 4 5 6], so y^T is 3x2
```
Start of expression
-------------------------------------------------
186. **math_lsq_non_square.k**:
```k
// x = [7; 8; 9] (length 3)
```
Start of expression
-------------------------------------------------
187. **math_lsq_non_square.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
188. **math_lsq_high_rank.k**:
```k
// Rectangular matrix: 2x4 matrix (more columns than rows)
```
Start of expression
-------------------------------------------------
189. **math_lsq_high_rank.k**:
```k
// Solving: y^T * w = x where y is 2x4, x is length 4, w is length 2
```
Start of expression
-------------------------------------------------
190. **math_lsq_high_rank.k**:
```k
// y = [1 2 3 4; 2 3 4 5], so y^T is 4x2
```
Start of expression
-------------------------------------------------
191. **math_lsq_high_rank.k**:
```k
// x = [10; 11; 12; 13] (length 4)
```
Start of expression
-------------------------------------------------
192. **math_lsq_high_rank.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
193. **math_lsq_complex.k**:
```k
// Complex non-square case with mixed values
```
Start of expression
-------------------------------------------------
194. **math_lsq_complex.k**:
```k
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
```
Start of expression
-------------------------------------------------
195. **math_lsq_complex.k**:
```k
// y = [1.5 2.0 3.0; 4.5 5.5 6.0], so y^T is 3x2
```
Start of expression
-------------------------------------------------
196. **math_lsq_complex.k**:
```k
// x = [7.5; 8.0; 9.5] (length 3)
```
Start of expression
-------------------------------------------------
197. **math_lsq_complex.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
198. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
199. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
200. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
201. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
202. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
203. **ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
After SYMBOL (position 5/8)
-------------------------------------------------
204. **ffi_assembly_load.k**:
```k
// Test basic FFI functionality
```
Start of expression
-------------------------------------------------
205. **ffi_assembly_load.k**:
```k
// Load System.Private.CoreLib assembly and access String type
```
Start of expression
-------------------------------------------------
206. **ffi_type_marshalling.k**:
```k
f:3.14159;f _sethint `float;s:"hello";s _sethint `string;l:1 2 3 4 5;l: _sethint `list // This needs to be split at some point
```
After FLOAT (position 3/29)
-------------------------------------------------
207. **ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
After CHARACTER_VECTOR (position 3/12)
-------------------------------------------------
208. **ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
209. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
210. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
211. **ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
212. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
213. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
214. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
215. **ffi_complete_workflow.k**:
```k
// Complete FFI Workflow Test
```
Start of expression
-------------------------------------------------
216. **ffi_complete_workflow.k**:
```k
// This test covers the full FFI functionality from assembly loading to method invocation
```
Start of expression
-------------------------------------------------
217. **ffi_complete_workflow.k**:
```k
// Step 1: Load .NET assembly with Complex type
```
Start of expression
-------------------------------------------------
218. **ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.19\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
After SYMBOL (position 5/6)
-------------------------------------------------
219. **ffi_complete_workflow.k**:
```k
// Step 2: Create constructor function
```
Start of expression
-------------------------------------------------
220. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
221. **ffi_complete_workflow.k**:
```k
// Step 3: Create object instance using constructor
```
Start of expression
-------------------------------------------------
222. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
223. **ffi_complete_workflow.k**:
```k
// Step 4: Call instance method (Abs - magnitude)
```
Start of expression
-------------------------------------------------
224. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
225. **ffi_complete_workflow.k**:
```k
// Step 5: Call property getter methods (not working)
```
Start of expression
-------------------------------------------------
226. **ffi_complete_workflow.k**:
```k
// real: c1[`get_Real][]
```
Start of expression
-------------------------------------------------
227. **ffi_complete_workflow.k**:
```k
// imag: c1[`get_Imaginary][]
```
Start of expression
-------------------------------------------------
228. **ffi_complete_workflow.k**:
```k
// Step 6: Call static method (Conjugate) (not working)
```
Start of expression
-------------------------------------------------
229. **ffi_complete_workflow.k**:
```k
conj_func: ._dotnet.System.Numerics.Complex.Conjugate
```
After IDENTIFIER (position 12/13)
-------------------------------------------------
230. **ffi_complete_workflow.k**:
```k
//conj_result: conj_func[c1]
```
Start of expression
-------------------------------------------------
231. **ffi_complete_workflow.k**:
```k
// Step 7: Display object information
```
Start of expression
-------------------------------------------------
232. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
233. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
234. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
235. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
236. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
237. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
238. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
239. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
240. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
241. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
242. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
243. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
244. **idioms_01_624_zero_array.k**:
```k
x:2 3#99
```
After INTEGER (position 6/7)
-------------------------------------------------
245. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
246. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
247. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
248. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
249. **idioms_01_411_number_rows.k**:
```k
x:2 7#" "
```
After CHARACTER (position 6/7)
-------------------------------------------------
250. **idioms_01_445_number_columns.k**:
```k
x:4 3#!12
```
After INTEGER (position 7/8)
-------------------------------------------------
251. **test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
After CHARACTER_VECTOR (position 4/8)
-------------------------------------------------
252. **idioms_01_388_drop_rows.k**:
```k
x:6 3#!18
```
After INTEGER (position 7/8)
-------------------------------------------------
253. **idioms_01_388_drop_rows.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
254. **idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
255. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
256. **idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
257. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
258. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
259. **idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
260. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
261. **idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
262. **idioms_01_88_name_variable.k**:
```k
x:"test"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
263. **idioms_01_88_name_variable.k**:
```k
y:2 3#!6
```
After INTEGER (position 7/8)
-------------------------------------------------
264. **idioms_01_96_conditional_execution.k**:
```k
@[+/;!6;:]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
265. **idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
266. **idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
267. **idioms_01_493_choose_boolean.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
268. **idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
269. **idioms_01_434_replace_first.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
270. **idioms_01_434_replace_first.k**:
```k
@[x;0;:;y]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
271. **idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
272. **idioms_01_433_replace_last.k**:
```k
y:"t"
```
After CHARACTER (position 3/4)
-------------------------------------------------
273. **idioms_01_433_replace_last.k**:
```k
@[x;-1+#x;:;y]
```
After RIGHT_BRACKET (position 13/14)
-------------------------------------------------
274. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
275. **idioms_01_406_add_last.k**:
```k
y:100
```
After INTEGER (position 3/4)
-------------------------------------------------
276. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
277. **idioms_01_449_limit_between.k**:
```k
l:30
```
After INTEGER (position 3/4)
-------------------------------------------------
278. **idioms_01_449_limit_between.k**:
```k
h:70
```
After INTEGER (position 3/4)
-------------------------------------------------
279. **idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
280. **idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
281. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
282. **idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
283. **idioms_01_504_replace_satisfying.k**:
```k
g:" "
```
After CHARACTER (position 3/4)
-------------------------------------------------
284. **idioms_01_504_replace_satisfying.k**:
```k
@[y;&x;:;g]
```
After RIGHT_BRACKET (position 11/12)
-------------------------------------------------
285. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
286. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
287. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
288. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
289. **idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
290. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
291. **idioms_01_616_scalar_from_vector.k**:
```k
x:,8
```
After INTEGER (position 4/5)
-------------------------------------------------
292. **idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
293. **idioms_01_509_remove_y.k**:
```k
y:"a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
294. **idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
295. **idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
296. **idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
297. **idioms_01_177_string_search.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
298. **idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
299. **idioms_01_45_binary_representation.k**:
```k
x:16
```
After INTEGER (position 3/4)
-------------------------------------------------
300. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
301. **idioms_01_129_arctangent.k**:
```k
x:_sqrt[3]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
302. **idioms_01_129_arctangent.k**:
```k
y:1
```
After INTEGER (position 3/4)
-------------------------------------------------
303. **idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
304. **idioms_01_241_sum_subsets.k**:
```k
x:1+3 4#!12
```
After INTEGER (position 9/10)
-------------------------------------------------
305. **idioms_01_241_sum_subsets.k**:
```k
y:4 3#1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
306. **idioms_01_61_cyclic_counter.k**:
```k
x:!10
```
After INTEGER (position 4/5)
-------------------------------------------------
307. **idioms_01_61_cyclic_counter.k**:
```k
y:8
```
After INTEGER (position 3/4)
-------------------------------------------------
308. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
309. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
310. **idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
311. **idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
312. **idioms_01_447_conditional_drop.k**:
```k
x:4 3#!12
```
After INTEGER (position 7/8)
-------------------------------------------------
313. **idioms_01_447_conditional_drop.k**:
```k
y:2
```
After INTEGER (position 3/4)
-------------------------------------------------
314. **idioms_01_447_conditional_drop.k**:
```k
g:0
```
After INTEGER (position 3/4)
-------------------------------------------------
315. **idioms_01_448_conditional_drop_last.k**:
```k
x:4 3#!12
```
After INTEGER (position 7/8)
-------------------------------------------------
316. **idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
After INTEGER (position 3/4)
-------------------------------------------------
317. **ktree_enumerate_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
318. **ktree_enumerate_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
319. **ktree_enumerate_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
320. **ktree_indexing_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
321. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
322. **ktree_indexing_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
323. **ktree_indexing_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
324. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
325. **ktree_indexing_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
326. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
327. **ktree_dot_apply_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); d . `keyA
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
328. **ktree_dot_apply_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
After RIGHT_PAREN (position 21/22)
-------------------------------------------------
329. **ktree_dot_apply_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `d . `keyA
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
330. **ktree_dot_apply_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `.k.d . `keyB
```
After RIGHT_PAREN (position 21/26)
-------------------------------------------------
331. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
332. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------
333. **eval_dot_parse_and_eval.k**:
```k
a:7
```
After INTEGER (position 3/4)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
