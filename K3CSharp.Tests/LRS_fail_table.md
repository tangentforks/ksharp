# K3CSharp Parser Failures

**Generated:** 2026-03-26 13:06:31
**Test Results:** 821/852 passed (96.4%)

## Executive Summary

**Total Tests:** 852
**Passed Tests:** 821
**Failed Tests:** 31
**Success Rate:** 96.4%

**LRS Parser Statistics:**
- NULL Results: 136
- Incorrect Results: 0
- LRS Success Rate: 84.0%

**Top Failure Patterns:**
- Start of expression: 34
- After INTEGER (position 6/7): 18
- After RIGHT_BRACKET (position 4/5): 9
- After INTEGER (position 3/4): 7
- After INTEGER (position 7/8): 7
- After INTEGER (position 5/6): 4
- After RIGHT_BRACKET (position 6/7): 4
- After SYMBOL (position 3/4): 4
- After INTEGER (position 2/3): 4
- After RIGHT_BRACKET (position 8/9): 4

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
3. **test_triadic_dot.k**:
```k
.[1 2 3;1;-:]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
4. **special_int_vector.k**:
```k
0I 0N -0I
```
After INTEGER (position 3/4)
-------------------------------------------------
5. **special_float_vector.k**:
```k
0i 0n -0i
```
After FLOAT (position 3/4)
-------------------------------------------------
6. **square_bracket_function.k**:
```k
div[8;4]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
7. **square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
8. **square_bracket_vector_multiple.k**:
```k
v[4 6]
```
After RIGHT_BRACKET (position 5/6)
-------------------------------------------------
9. **square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
After INTEGER (position 9/10)
-------------------------------------------------
10. **square_bracket_vector_single.k**:
```k
v[4]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
11. **symbol_vector_compact.k**:
```k
`a`b`c
```
After SYMBOL (position 3/4)
-------------------------------------------------
12. **symbol_vector_spaces.k**:
```k
`a `b `c
```
After SYMBOL (position 3/4)
-------------------------------------------------
13. **io_monadic_1_int_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
14. **io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
15. **io_monadic_1_char_vector_index.k**:
```k
result[0]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
16. **io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
17. **semicolon_vector.k**:
```k
a: 1 2
```
After INTEGER (position 4/5)
-------------------------------------------------
18. **semicolon_vector.k**:
```k
b: 3 4
```
After INTEGER (position 4/5)
-------------------------------------------------
19. **test_semicolon.k**:
```k
1 2; 3 4
```
After INTEGER (position 2/6)
-------------------------------------------------
20. **vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
21. **amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
After INTEGER (position 3/4)
-------------------------------------------------
22. **variable_reassignment.k**:
```k
foo:7.2 4.5
```
After FLOAT (position 4/5)
-------------------------------------------------
23. **adverb_each_count.k**:
```k
#:' (1 2 3;1 2 3 4 5; 1 2)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
24. **time_gtime.k**:
```k
_gtime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
25. **time_ltime.k**:
```k
_ltime 0
```
After INTEGER (position 2/3)
-------------------------------------------------
26. **list_getenv.k**:
```k
_getenv "PROMPT"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
27. **list_size_existing.k**:
```k
_size "C:\\Windows\\System32\\write.exe"
```
After CHARACTER_VECTOR (position 2/3)
-------------------------------------------------
28. **test_monadic_colon.k**:
```k
: 42
```
After INTEGER (position 2/3)
-------------------------------------------------
29. **statement_assignment_inline.k**:
```k
1 + a: 42
```
After INTEGER (position 5/6)
-------------------------------------------------
30. **vector_notation_space.k**:
```k
1 2 3 4 5
```
After INTEGER (position 5/6)
-------------------------------------------------
31. **empty_brackets_dictionary.k**:
```k
d[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
32. **empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
33. **empty_brackets_vector.k**:
```k
v[]
```
After RIGHT_BRACKET (position 3/4)
-------------------------------------------------
34. **group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
After INTEGER (position 22/23)
-------------------------------------------------
35. **k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
36. **k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
37. **k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
38. **k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
39. **k_tree_nested_indexing.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
40. **k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
After INTEGER (position 6/7)
-------------------------------------------------
41. **k_tree_dictionary_assignment.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
42. **k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
43. **vector_null_index.k**:
```k
v: 1 2 3 4
```
After INTEGER (position 6/7)
-------------------------------------------------
44. **serialization_bd_db_integer.k**:
```k
// Test _bd and _db for Integer serialization
```
Start of expression
-------------------------------------------------
45. **serialization_bd_db_character.k**:
```k
// Test _bd and _db for Character serialization
```
Start of expression
-------------------------------------------------
46. **serialization_bd_db_symbol.k**:
```k
// Test _bd and _db for Symbol serialization
```
Start of expression
-------------------------------------------------
47. **serialization_bd_db_null.k**:
```k
// Test _bd and _db for Null serialization
```
Start of expression
-------------------------------------------------
48. **serialization_bd_db_roundtrip_integer.k**:
```k
// Test _bd and _db round-trip serialization
```
Start of expression
-------------------------------------------------
49. **test_simple_symbol.k**:
```k
`a `"." `b
```
After SYMBOL (position 3/4)
-------------------------------------------------
50. **test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
After SYMBOL (position 4/5)
-------------------------------------------------
51. **math_ceil_basic.k**:
```k
_ceil 4.7
```
After FLOAT (position 2/3)
-------------------------------------------------
52. **math_ceil_integer.k**:
```k
_ceil 5
```
After INTEGER (position 2/3)
-------------------------------------------------
53. **math_ceil_negative.k**:
```k
_ceil -3.2
```
After FLOAT (position 2/3)
-------------------------------------------------
54. **math_ceil_vector.k**:
```k
_ceil 1.2 2.7 3.5
```
After FLOAT (position 4/5)
-------------------------------------------------
55. **math_lsq_non_square.k**:
```k
// Non-square matrix: 2x3 matrix (more columns than rows)
```
Start of expression
-------------------------------------------------
56. **math_lsq_non_square.k**:
```k
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
```
Start of expression
-------------------------------------------------
57. **math_lsq_non_square.k**:
```k
// y = [1 2 3; 4 5 6], so y^T is 3x2
```
Start of expression
-------------------------------------------------
58. **math_lsq_non_square.k**:
```k
// x = [7; 8; 9] (length 3)
```
Start of expression
-------------------------------------------------
59. **math_lsq_non_square.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
60. **math_lsq_high_rank.k**:
```k
// Rectangular matrix: 2x4 matrix (more columns than rows)
```
Start of expression
-------------------------------------------------
61. **math_lsq_high_rank.k**:
```k
// Solving: y^T * w = x where y is 2x4, x is length 4, w is length 2
```
Start of expression
-------------------------------------------------
62. **math_lsq_high_rank.k**:
```k
// y = [1 2 3 4; 2 3 4 5], so y^T is 4x2
```
Start of expression
-------------------------------------------------
63. **math_lsq_high_rank.k**:
```k
// x = [10; 11; 12; 13] (length 4)
```
Start of expression
-------------------------------------------------
64. **math_lsq_high_rank.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
65. **math_lsq_complex.k**:
```k
// Complex non-square case with mixed values
```
Start of expression
-------------------------------------------------
66. **math_lsq_complex.k**:
```k
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
```
Start of expression
-------------------------------------------------
67. **math_lsq_complex.k**:
```k
// y = [1.5 2.0 3.0; 4.5 5.5 6.0], so y^T is 3x2
```
Start of expression
-------------------------------------------------
68. **math_lsq_complex.k**:
```k
// x = [7.5; 8.0; 9.5] (length 3)
```
Start of expression
-------------------------------------------------
69. **math_lsq_complex.k**:
```k
// Result w should be length 2
```
Start of expression
-------------------------------------------------
70. **math_mul_basic.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
71. **math_not_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
72. **math_or_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
73. **math_xor_vector.k**:
```k
1 2 3
```
After INTEGER (position 3/4)
-------------------------------------------------
74. **ffi_hint_system.k**:
```k
42 _sethint `uint
```
After SYMBOL (position 3/4)
-------------------------------------------------
75. **ffi_assembly_load.k**:
```k
// Test basic FFI functionality
```
Start of expression
-------------------------------------------------
76. **ffi_assembly_load.k**:
```k
// Load System.Private.CoreLib assembly and access String type
```
Start of expression
-------------------------------------------------
77. **ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
78. **ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
79. **ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
80. **ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
81. **ffi_dispose.k**:
```k
_dispose c1
```
After IDENTIFIER (position 2/3)
-------------------------------------------------
82. **ffi_complete_workflow.k**:
```k
// Complete FFI Workflow Test
```
Start of expression
-------------------------------------------------
83. **ffi_complete_workflow.k**:
```k
// This test covers the full FFI functionality from assembly loading to method invocation
```
Start of expression
-------------------------------------------------
84. **ffi_complete_workflow.k**:
```k
// Step 1: Load .NET assembly with Complex type
```
Start of expression
-------------------------------------------------
85. **ffi_complete_workflow.k**:
```k
// Step 2: Create constructor function
```
Start of expression
-------------------------------------------------
86. **ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
After RIGHT_BRACKET (position 6/7)
-------------------------------------------------
87. **ffi_complete_workflow.k**:
```k
// Step 3: Create object instance using constructor
```
Start of expression
-------------------------------------------------
88. **ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
89. **ffi_complete_workflow.k**:
```k
// Step 4: Call instance method (Abs - magnitude)
```
Start of expression
-------------------------------------------------
90. **ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
After RIGHT_BRACKET (position 8/9)
-------------------------------------------------
91. **ffi_complete_workflow.k**:
```k
// Step 5: Call property getter methods (not working)
```
Start of expression
-------------------------------------------------
92. **ffi_complete_workflow.k**:
```k
// real: c1[`get_Real][]
```
Start of expression
-------------------------------------------------
93. **ffi_complete_workflow.k**:
```k
// imag: c1[`get_Imaginary][]
```
Start of expression
-------------------------------------------------
94. **ffi_complete_workflow.k**:
```k
// Step 6: Call static method (Conjugate) (not working)
```
Start of expression
-------------------------------------------------
95. **ffi_complete_workflow.k**:
```k
//conj_result: conj_func[c1]
```
Start of expression
-------------------------------------------------
96. **ffi_complete_workflow.k**:
```k
// Step 7: Display object information
```
Start of expression
-------------------------------------------------
97. **idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
98. **idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
99. **idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
100. **idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
101. **idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
102. **idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
103. **idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
After INTEGER (position 6/7)
-------------------------------------------------
104. **idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
After INTEGER (position 6/7)
-------------------------------------------------
105. **idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
106. **idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
After RIGHT_PAREN (position 17/18)
-------------------------------------------------
107. **idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
After INTEGER (position 4/5)
-------------------------------------------------
108. **idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
After INTEGER (position 13/14)
-------------------------------------------------
109. **idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
After INTEGER (position 7/8)
-------------------------------------------------
110. **idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
After INTEGER (position 7/8)
-------------------------------------------------
111. **idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
112. **idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
113. **idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
After RIGHT_PAREN (position 15/16)
-------------------------------------------------
114. **idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
115. **idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
116. **idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
After INTEGER (position 12/13)
-------------------------------------------------
117. **idioms_01_96_conditional_execution.k**:
```k
@[+/;!6;:]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
118. **idioms_01_434_replace_first.k**:
```k
@[x;0;:;y]
```
After RIGHT_BRACKET (position 10/11)
-------------------------------------------------
119. **idioms_01_433_replace_last.k**:
```k
@[x;-1+#x;:;y]
```
After RIGHT_BRACKET (position 13/14)
-------------------------------------------------
120. **idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
121. **idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89) // 5 6 _draw 100
```
After RIGHT_PAREN (position 38/39)
-------------------------------------------------
122. **idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
After INTEGER (position 12/13)
-------------------------------------------------
123. **idioms_01_504_replace_satisfying.k**:
```k
@[y;&x;:;g]
```
After RIGHT_BRACKET (position 11/12)
-------------------------------------------------
124. **idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
After INTEGER (position 7/8)
-------------------------------------------------
125. **idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
After INTEGER (position 7/8)
-------------------------------------------------
126. **idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
After INTEGER (position 6/7)
-------------------------------------------------
127. **idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
128. **idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
After INTEGER (position 7/8)
-------------------------------------------------
129. **idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
After INTEGER (position 10/11)
-------------------------------------------------
130. **idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
131. **idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
After INTEGER (position 6/7)
-------------------------------------------------
132. **ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
133. **ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
134. **ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
After RIGHT_BRACKET (position 4/5)
-------------------------------------------------
135. **test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
136. **eval_dot_execute_path.k**:
```k
v:`e`f
```
After SYMBOL (position 4/5)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
