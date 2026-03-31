# K3CSharp Parser Failures

**Generated:** 2026-03-30 22:49:40
**Test Results:** 820/852 passed (96.2%)

## Executive Summary

**Total Tests:** 852
**Passed Tests:** 820
**Failed Tests:** 32
**Success Rate:** 96.2%

**LRS Parser Statistics:**
- NULL Results: 0
- Incomplete Token Consumption: 976
- Total Fallbacks to Legacy: 976
- Incorrect Results: 0
- LRS Success Rate: -14.6%

**Top Failure Patterns:**
- Incomplete consumption (position 3/4): 265
- Incomplete consumption (position 2/3): 138
- Incomplete consumption (position 7/8): 77
- Incomplete consumption (position 5/6): 74
- Incomplete consumption (position 6/7): 60
- Incomplete consumption (position 4/5): 58
- Incomplete consumption (position 1/2): 39
- Incomplete consumption (position 9/10): 37
- Incomplete consumption (position 8/9): 25
- Incomplete consumption (position 10/11): 22

## LRS Parser Failures

### Incomplete Token Consumption (LRS returned result but didn't consume all tokens)

**adverb_each_vector_minus.k**:
```k
(10 20 30) -' (1 2 3)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**adverb_each_vector_multiply.k**:
```k
(1 2 3) *' (4 5 6)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**adverb_each_vector_plus.k**:
```k
(1 2 3 4) +' (5 6 7 8)
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**adverb_over_divide.k**:
```k
%/ 100 2 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**adverb_over_max.k**:
```k
|/ 1 3 2 5 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_over_min.k**:
```k
&/ 5 3 4 1 2
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_over_minus.k**:
```k
-/ 10 2 3 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**adverb_over_multiply.k**:
```k
*/ 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**adverb_over_plus.k**:
```k
+/ 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_over_power.k**:
```k
^/ 2 3 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**adverb_over_with_initialization_1.k**:
```k
1 +/ 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_over_with_initialization_2.k**:
```k
2 +/ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_divide.k**:
```k
%\ 100 2 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**adverb_scan_max.k**:
```k
|\ 1 3 2 5 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_min.k**:
```k
&\ 5 3 4 1 2
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_minus.k**:
```k
-\ 10 2 3 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**adverb_scan_multiply.k**:
```k
*\ 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**adverb_scan_plus.k**:
```k
+\ 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_power.k**:
```k
^\ 2 3 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**adverb_scan_with_initialization_1.k**:
```k
1 +\ 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_with_initialization_2.k**:
```k
2 +\ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_with_initialization_divide.k**:
```k
2 %\ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_with_initialization_minus.k**:
```k
2 -\ 1 2 3 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**adverb_scan_with_initialization.k**:
```k
2 *\ 1 2 3
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**anonymous_function_double_param.k**:
```k
{[op1;op2] op1 * op2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**anonymous_function_empty.k**:
```k
{}
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**anonymous_function_over_adverb.k**:
```k
{x*%y}/10 20 30
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**anonymous_function_scan_adverb.k**:
```k
{x*%y}\10 20 30
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**test_projected_function.k**:
```k
%
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**atom_scalar.k**:
```k
@42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**atom_vector.k**:
```k
@1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**lrs_atomic_parser_basic.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**lrs_adverb_parser_each.k**:
```k
(1 2 3) %\: 2
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**lrs_adverb_parser_basic.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**lrs_expression_processor_test.k**:
```k
1 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**lrs_parser_validation.k**:
```k
1 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**attribute_handle_symbol.k**:
```k
~`a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**attribute_handle_vector.k**:
```k
~(`a`b`c)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**character_vector.k**:
```k
"hello"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**complex_function.k**:
```k
(distance) . (5 10 2 10)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**count_operator.k**:
```k
# (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**cut_vector.k**:
```k
(0 2 4) _ (0 1 2 3 4 5 6 7)
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**dictionary_empty.k**:
```k
.()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dictionary_index.k**:
```k
(.((`a;1);(`b;2))) @ `a
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**dictionary_index_attr.k**:
```k
(.((`a;1);(`b;2;.((`c;3);(`d;4))))) @ `b.
```
Incomplete consumption (position 33/34) (consumed 33/34)
-------------------------------------------------
**dictionary_index_value.k**:
```k
(.((`a;1);(`b;2))) @ `a
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**dictionary_index_value2.k**:
```k
(.((`a;1);(`b;2))) @ `b
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**dictionary_make_symbol_vector.k**:
```k
.,`a`b
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**dictionary_multiple.k**:
```k
.((`a;1);(`b;2))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**dictionary_null_attributes.k**:
```k
.((`a;1);(`b;2))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**dictionary_single.k**:
```k
.,(`a;`b)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dictionary_type.k**:
```k
4:.((`a;1);(`b;2))
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**dictionary_with_null_value.k**:
```k
.((`a;1);(`b;_n);(`c;3))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**dictionary_period_index_all_attributes.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[.]
```
Incomplete consumption (position 88/94) (consumed 88/94)
-------------------------------------------------
**test_minimal_dict.k**:
```k
d: .,(`a;1;.,(`x;2));d[.]
```
Incomplete consumption (position 17/23) (consumed 17/23)
-------------------------------------------------
**test_simple_period.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[.]
```
Incomplete consumption (position 31/37) (consumed 31/37)
-------------------------------------------------
**test_attr_access.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d[`b.]
```
Incomplete consumption (position 31/37) (consumed 31/37)
-------------------------------------------------
**test_dict_create.k**:
```k
d: .((`a;1);(`b;2;.((`x;2);(`y;3))));d
```
Incomplete consumption (position 31/34) (consumed 31/34)
-------------------------------------------------
**test_dict_with_attr.k**:
```k
.((`a;1);(`b;2;.((`x;2);(`y;3))))
```
Incomplete consumption (position 29/30) (consumed 29/30)
-------------------------------------------------
**test_show_dict.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d
```
Incomplete consumption (position 88/91) (consumed 88/91)
-------------------------------------------------
**test_simple_dict_create.k**:
```k
.,(`a;1)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**test_specific_attr.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
Incomplete consumption (position 88/94) (consumed 88/94)
-------------------------------------------------
**test_specific_attr_fixed.k**:
```k
d: .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))));d[`col01.]
```
Incomplete consumption (position 88/94) (consumed 88/94)
-------------------------------------------------
**empty_char_vector.k**:
```k
0#"abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**empty_float_vector_test.k**:
```k
0#1.0 2.0 3.0
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**empty_symbol_atomic.k**:
```k
0#`test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**empty_dictionary.k**:
```k
.()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**empty_list.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_symbol_take.k**:
```k
0 # `test`
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**test_symbol_parsing.k**:
```k
0#`test`
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**drop_negative.k**:
```k
-4 _ 0 1 2 3 4 5 6 7
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**drop_positive.k**:
```k
4 _ 0 1 2 3 4 5 6 7
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**empty_mixed_vector.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**enlist_operator.k**:
```k
,5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**enumerate_empty_int.k**:
```k
!0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**enumerate_operator.k**:
```k
!5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**equal_operator.k**:
```k
3 = 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**char_vector_equal.k**:
```k
"abc" = "abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**char_vector_different.k**:
```k
"abc" = "abz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**char_vector_match_equal.k**:
```k
"abc" ~ "abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**char_vector_match_different.k**:
```k
"abc" ~ "abz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**float_infinity_match_equal.k**:
```k
0i ~ 0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**float_neg_infinity_match_equal.k**:
```k
-0i ~ -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**float_infinity_match_different.k**:
```k
0i ~ -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**float_infinity_equal.k**:
```k
0i = 0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**float_neg_infinity_equal.k**:
```k
-0i = -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**float_infinity_equal_different.k**:
```k
0i = -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**first_operator.k**:
```k
* (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**float_decimal_point.k**:
```k
10.
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**float_exponential.k**:
```k
0.17e03
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**float_exponential_large.k**:
```k
1e15
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**float_exponential_small.k**:
```k
1e-20
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**float_types.k**:
```k
3.14
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**function_add7.k**:
```k
add7:{[arg1] arg1+7}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**function_add7.k**:
```k
(add7) . (5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**function_call_anonymous.k**:
```k
{[arg1] arg1+6} . 7
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**function_call_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**function_call_chain.k**:
```k
foo: (mul) . (8 4)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**function_call_chain.k**:
```k
foo - 12
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**function_call_double.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**function_call_double.k**:
```k
(mul) . (8 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**function_call_simple.k**:
```k
add7:{[arg1] arg1+7}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**function_call_simple.k**:
```k
(add7) . (5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**function_foo_chain.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**function_foo_chain.k**:
```k
foo: (mul) . (8 4)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**function_foo_chain.k**:
```k
foo - 12
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**function_mul.k**:
```k
mul:{[op1;op2] op1 * op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**function_mul.k**:
```k
(mul) . (8 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**lambda_string_literal.k**:
```k
{"hello"}[]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**lambda_symbol_literal.k**:
```k
{`abc}[]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**named_function_over.k**:
```k
f:{x*%y};f/10 20 30
```
Incomplete consumption (position 8/15) (consumed 8/15)
-------------------------------------------------
**named_function_scan.k**:
```k
f:{x*%y};f\10 20 30
```
Incomplete consumption (position 8/15) (consumed 8/15)
-------------------------------------------------
**where_generate_scalar.k**:
```k
&4
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**grade_down_operator.k**:
```k
> (3 11 9 9 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**grade_up_operator.k**:
```k
< (3 11 9 9 4)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**greater_than_operator.k**:
```k
3 > 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**integer_types_int.k**:
```k
42
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**integer_types_long.k**:
```k
123456789j
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**join_operator.k**:
```k
3 , 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**less_than_operator.k**:
```k
3 < 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_abs.k**:
```k
_abs -5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_exp.k**:
```k
_exp 2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_log.k**:
```k
_log 10
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_sin.k**:
```k
_sin 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_sqrt.k**:
```k
_sqrt 16
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_vector.k**:
```k
_sin 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**math_exp_basic.k**:
```k
_exp 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_floor_nan.k**:
```k
_ 0n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_floor_negative_infinity.k**:
```k
_ -0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_floor_special_values.k**:
```k
_ 0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_hyperbolic_basic.k**:
```k
_sinh 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_inv_matrix_2x2.k**:
```k
_inv ((1 2);(3 4))
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**math_inv_matrix_3x3.k**:
```k
_inv ((1 2 3);(0 1 4);(5 6 0))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**math_inv_matrix_identity_3x3.k**:
```k
_inv ((1 0 0);(0 1 0);(0 0 1))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**math_log_negative.k**:
```k
_log -1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_log_zero.k**:
```k
_log 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_mul_matrix_2x2.k**:
```k
((1 2);(3 4)) _mul ((5 6);(7 8))
```
Incomplete consumption (position 23/24) (consumed 23/24)
-------------------------------------------------
**math_mul_matrix_2x3_3x2.k**:
```k
((1 2 3);(4 5 6)) _mul ((7 8);(9 10);(11 12))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
**math_mul_matrix_3x3.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) _mul ((9 8 7);(6 5 4);(3 2 1))
```
Incomplete consumption (position 39/40) (consumed 39/40)
-------------------------------------------------
**math_mul_matrix_4x2_2x4.k**:
```k
((1 2);(3 4);(5 6);(7 8)) _mul ((9 10 11 12);(13 14 15 16))
```
Incomplete consumption (position 37/38) (consumed 37/38)
-------------------------------------------------
**math_trig_basic.k**:
```k
_sin 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_trig_pi.k**:
```k
_cos 3.141592653589793
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**maximum_operator.k**:
```k
3 | 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**minimum_operator.k**:
```k
3 & 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**mixed_list_with_null.k**:
```k
(1;_n;`test;42.5)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**mixed_vector_empty_position.k**:
```k
(1;;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**mixed_vector_multiple_empty.k**:
```k
(1;;;3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**mixed_vector_whitespace_position.k**:
```k
(1; ;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**mod_integer.k**:
```k
7!3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**mod_rotate.k**:
```k
2 ! 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**mod_vector.k**:
```k
1 2 3 4 ! 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**modulus_operator.k**:
```k
7 ! 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**negate_operator.k**:
```k
~0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**nested_vector_test.k**:
```k
((1 2 3);(4 5 6))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**overflow_int_max_plus1.k**:
```k
2147483647 + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_int_neg_inf.k**:
```k
-0I - 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_int_neg_inf_minus2.k**:
```k
-0I - 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_int_null_minus1.k**:
```k
0N - 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_int_pos_inf.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_int_pos_inf_plus2.k**:
```k
0I + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_long_max_plus1.k**:
```k
9223372036854775807j + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_long_min_minus1.k**:
```k
-9223372036854775808j - 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**overflow_regular_int.k**:
```k
2147483637 + 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**underflow_regular_int.k**:
```k
-2147483639 - 40
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**parentheses_basic.k**:
```k
1 + 2 * 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**parentheses_grouping.k**:
```k
(1 + 2) * 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**debug_mult_only.k**:
```k
3 * 7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**debug_no_outer_paren.k**:
```k
1 + 2 * 3 + 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**debug_left_paren.k**:
```k
(1 + 2)
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**debug_simple_mult.k**:
```k
(1 + 2) * (3 + 4)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**debug_double_nested.k**:
```k
((1+2)*(3+4))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**parentheses_nested.k**:
```k
(1 + (2 * 3))
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**parentheses_nested1.k**:
```k
((1 + 2) * (3 + 4))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**parentheses_nested2.k**:
```k
(10 % (2 + (3 * 4)))
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**parentheses_nested3.k**:
```k
(((1 + 2) + 3) * 4)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**parentheses_nested4.k**:
```k
(1 + (2 + (3 + (4 + 5))))
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
**parentheses_nested5.k**:
```k
((1 * 2) + (3 * (4 + 5)))
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
**parentheses_precedence.k**:
```k
1 + (2 * 3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**parenthesized_vector.k**:
```k
(1;2;3;4)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**list_null_consecutive_semicolons.k**:
```k
(1;;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**list_null_multiple_semicolons.k**:
```k
(;;)
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**list_null_empty_parens.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**power_operator.k**:
```k
2 ^ 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**precedence_chain1.k**:
```k
10 + 20 * 30 + 40
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**precedence_chain2.k**:
```k
100 % 10 + 20 * 30 + 40
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**precedence_complex1.k**:
```k
10 % 2 + 3 * 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**precedence_complex2.k**:
```k
10 + 20 % 30 * 40
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**precedence_mixed1.k**:
```k
5 - 2 + 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**precedence_mixed2.k**:
```k
5 * 2 + 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**precedence_mixed3.k**:
```k
5 + 2 * 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**precedence_power1.k**:
```k
2 ^ 3 + 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**precedence_power2.k**:
```k
2 + 3 ^ 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**precedence_spec1.k**:
```k
81 % 1 + 2 * 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**precedence_spec2.k**:
```k
120 % 4 * 2 + 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**reciprocal_operator.k**:
```k
%4
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**reverse_operator.k**:
```k
| (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**scalar_vector_addition.k**:
```k
1 2 + 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**scalar_vector_multiplication.k**:
```k
1 2 * 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**shape_operator.k**:
```k
^ (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**shape_operator_empty_vector.k**:
```k
^ ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**shape_operator_jagged.k**:
```k
^ ((1 2 3); (4 5); (6 7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**dyadic_plus_vector_vector.k**:
```k
1 2 3 + 4 5 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_plus_atom_vector.k**:
```k
5 + 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_plus_vector_atom.k**:
```k
1 2 3 + 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_minus_vector_vector.k**:
```k
5 6 7 - 1 2 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_minus_atom_vector.k**:
```k
10 - 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_minus_vector_atom.k**:
```k
5 6 7 - 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_times_vector_vector.k**:
```k
2 3 4 * 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_times_atom_vector.k**:
```k
3 * 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_times_vector_atom.k**:
```k
1 2 3 * 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_divide_vector_vector.k**:
```k
6 8 10 % 2 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_divide_atom_vector.k**:
```k
12 % 2 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_divide_vector_atom.k**:
```k
4 6 8 % 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_min_vector_vector.k**:
```k
5 8 3 & 2 9 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_min_atom_vector.k**:
```k
3 & 5 2 8
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_min_vector_atom.k**:
```k
5 2 8 & 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_max_vector_vector.k**:
```k
5 8 3 | 2 9 4
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_max_atom_vector.k**:
```k
6 | 3 8 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_max_vector_atom.k**:
```k
3 8 2 | 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_less_vector_vector.k**:
```k
3 5 2 < 4 2 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_less_atom_vector.k**:
```k
3 < 4 2 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_less_vector_atom.k**:
```k
3 5 2 < 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_more_vector_vector.k**:
```k
3 5 2 > 2 4 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_more_atom_vector.k**:
```k
4 > 2 5 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_more_vector_atom.k**:
```k
3 5 2 > 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_equal_vector_vector.k**:
```k
3 5 2 = 3 4 2
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_equal_atom_vector.k**:
```k
3 = 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_equal_vector_atom.k**:
```k
3 4 5 = 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_power_vector_vector.k**:
```k
2 3 4 ^ 3 2 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**dyadic_power_atom_vector.k**:
```k
2 ^ 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**dyadic_power_vector_atom.k**:
```k
2 3 4 ^ 2
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**shape_operator_jagged_3d.k**:
```k
^ (((1 2); (3 4 5)); ((6 7); (8 9 10)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
**shape_operator_jagged_matrix.k**:
```k
^ ((1 2 3); (4 5); (6 7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**shape_operator_matrix.k**:
```k
^ ((1 2 3); (4 5 6); (7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**shape_operator_matrix_2x3.k**:
```k
^ ((1 2 3); (4 5 6))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**shape_operator_matrix_3x3.k**:
```k
^ ((1 2 3); (4 5 6); (7 8 9))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**shape_operator_scalar.k**:
```k
^ 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**shape_operator_tensor_2x2x3.k**:
```k
^ (((1 2 3); (4 5 6)); ((7 8 9); (10 11 12)))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
**shape_operator_tensor_3d.k**:
```k
^ (((1 2); (3 4)); ((5 6); (7 8)); ((9 10); (11 12)))
```
Incomplete consumption (position 38/39) (consumed 38/39)
-------------------------------------------------
**shape_operator_vector.k**:
```k
^ (1 2 3 4 5)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**simple_addition.k**:
```k
1 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**divide_float.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**divide_float.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**divide_float.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**divide_integer.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**divide_integer.k**:
```k
b:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**divide_integer.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**simple_multiplication.k**:
```k
4 * 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**simple_nested_test.k**:
```k
(1 2 3)
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**minus_integer.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**minus_integer.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**minus_integer.k**:
```k
a-c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**test_adverb_aware_evaluation.k**:
```k
+/ 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**special_float_neg_inf.k**:
```k
-0i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_float_null.k**:
```k
0n
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_float_pos_inf.k**:
```k
0i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_int_neg_inf.k**:
```k
-0I
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_int_null.k**:
```k
0N
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_int_pos_inf.k**:
```k
0I
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_null.k**:
```k
_n
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**special_int_pos_inf_plus_1.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_int_null_plus_1.k**:
```k
0N + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_int_neg_inf_plus_1.k**:
```k
-0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_float_null_plus_1.k**:
```k
0n + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_1_plus_int_pos_inf.k**:
```k
1 + 0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_1_plus_int_null.k**:
```k
1 + 0N
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_int_vector.k**:
```k
0I 0N -0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_float_vector.k**:
```k
0i 0n -0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**square_bracket_function.k**:
```k
div:{[op1;op2] op1%op2}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**square_bracket_function.k**:
```k
div[8;4]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**square_bracket_vector_multiple.k**:
```k
v:10 11 12 13 14 15 16
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**square_bracket_vector_multiple.k**:
```k
v[4 6]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**square_bracket_vector_single.k**:
```k
v:10 11 12 13 14 15 16
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**square_bracket_vector_single.k**:
```k
v[4]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**string_representation_int.k**:
```k
5:42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**string_representation_symbol.k**:
```k
5:`symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**string_representation_vector.k**:
```k
5:(1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**symbol_quoted.k**:
```k
`"a symbol"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**symbol_simple.k**:
```k
`foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**symbol_vector_compact.k**:
```k
`a`b`c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**symbol_vector_spaces.k**:
```k
`a `b `c
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**symbol_period_foo.k**:
```k
`foo.
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**symbol_period_foobar.k**:
```k
`foo.bar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**symbol_period_dotbar.k**:
```k
`.bar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**symbol_period_dotk.k**:
```k
`.k
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**io_read_int.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\int.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_read_float.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_read_symbol.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\symbol.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_read_intvec.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_read_debug.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_write_int.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_write.l" 1: 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**io_roundtrip.k**:
```k
"T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l" 1: (1;2.5;"hello")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**io_roundtrip.k**:
```k
2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\test_roundtrip.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_monadic_1_int_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_monadic_1_float_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_monadic_1_char_vector.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_monadic_1_int_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_int_vector_index.k**:
```k
result[0]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_int_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_int_vector_last_index.k**:
```k
result[2]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_char_vector_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_char_vector_index.k**:
```k
result[0]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_char_vector_last_index.k**:
```k
result: 1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_char_vector_last_index.k**:
```k
result[10]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_monadic_1_vs_2_int_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\intvec.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**io_monadic_1_vs_2_float_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\float.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**io_monadic_1_vs_2_char_vector.k**:
```k
(1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l") ~ (2: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\charvec.l")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**io_monadic_1_symbol_fallback.k**:
```k
1: "T:\\_src\\github.com\\ERufian\\ksharp\\TestFiles\\symbol.l"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**match_simple_vectors.k**:
```k
5 6 7 ~ 5 6 7
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**take_operator_basic.k**:
```k
3#1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**take_operator_empty_float.k**:
```k
0#0.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**take_operator_empty_symbol.k**:
```k
0#``
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**take_operator_overflow.k**:
```k
10#1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**take_operator_scalar.k**:
```k
3#42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**reshape_basic.k**:
```k
3 4 # !12
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**division_float_4_2.0.k**:
```k
4 % 2.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_float_5_2.5.k**:
```k
5 % 2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_int_4_2.k**:
```k
4 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_int_5_2.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_rules_10_3.k**:
```k
10 % 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_rules_12_4.k**:
```k
12 % 4
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_rules_4_2.k**:
```k
4 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**division_rules_5_2.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**enumerate.k**:
```k
! 2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**grade_down_no_parens.k**:
```k
> 3 11 9 9 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**grade_up_no_parens.k**:
```k
< 3 11 9 9 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**multiline_function_single.k**:
```k
test . 5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**null_vector.k**:
```k
(;1;2)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**scoping_single.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**scoping_single.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**scoping_single.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**semicolon_simple.k**:
```k
(3 + 4; 5 + 6; -20.45)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**semicolon_vars.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**semicolon_vars.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**semicolon_vars.k**:
```k
(a + b; a * b; a - b)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**semicolon_vector.k**:
```k
a: 1 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**semicolon_vector.k**:
```k
b: 3 4
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**semicolon_vector.k**:
```k
(3 + 4; b; -20.45)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**test_semicolon.k**:
```k
1 2; 3 4
```
Incomplete consumption (position 2/6) (consumed 2/6)
-------------------------------------------------
**simple_scalar_div.k**:
```k
5 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**single_no_semicolon.k**:
```k
(42)
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**smart_division1.k**:
```k
5 10 % 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**smart_division2.k**:
```k
4 8 % 2
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**smart_division3.k**:
```k
6 12 18 % 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**special_0i_plus_1.k**:
```k
0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_0n_plus_1.k**:
```k
0N + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_1_plus_neg0i.k**:
```k
1 + -0I
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_neg0i_plus_1.k**:
```k
-0I + 1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_underflow.k**:
```k
-0I - 27
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_underflow_2.k**:
```k
-0I - 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**special_underflow_3.k**:
```k
-0I - 1000
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_float.k**:
```k
4: 3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_null.k**:
```k
4: _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_symbol.k**:
```k
4: `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_vector.k**:
```k
4: (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_operator_float.k**:
```k
4: 3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_operator_null.k**:
```k
4: _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_operator_symbol.k**:
```k
4: `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_operator_vector_char.k**:
```k
4: ("abc")
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**type_operator_vector_float.k**:
```k
4: (1.0 2.0 3.0)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**type_operator_vector_int.k**:
```k
4: (1 2 3)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**type_operator_vector_symbol.k**:
```k
4: "abc"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**type_promotion_float_int.k**:
```k
1.5 + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_promotion_float_long.k**:
```k
1.5 + 1j
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_promotion_int_float.k**:
```k
2 + 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_promotion_int_long.k**:
```k
2 + 1j
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_promotion_long_float.k**:
```k
1j + 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**type_promotion_long_int.k**:
```k
1j + 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**unary_minus_operator.k**:
```k
- 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**unique_operator.k**:
```k
? (1 2 1 3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**amend_item_simple_no_semicolon.k**:
```k
1 12 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_assignment.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_reassignment.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_reassignment.k**:
```k
foo:7.2 4.5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**variable_reassignment.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_global_access.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_scoping_global_access.k**:
```k
test1: {[x] globalVar + x}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**variable_scoping_global_access.k**:
```k
result1: test1 . 50
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**variable_scoping_global_access.k**:
```k
result1
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_global_assignment.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_scoping_global_assignment.k**:
```k
result5: test5 . 10
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**variable_scoping_global_assignment.k**:
```k
result5
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_global_assignment.k**:
```k
globalVar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_global_unchanged.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_scoping_global_unchanged.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**variable_scoping_global_unchanged.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_global_unchanged.k**:
```k
globalVar
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_local_hiding.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_scoping_local_hiding.k**:
```k
result2: test2 . 25
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**variable_scoping_local_hiding.k**:
```k
result2
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_scoping_nested_functions.k**:
```k
globalVar: 100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_scoping_nested_functions.k**:
```k
result4: outer . 20
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**variable_scoping_nested_functions.k**:
```k
result4
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**variable_usage.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_usage.k**:
```k
y:20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**variable_usage.k**:
```k
z:x+y
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**variable_usage.k**:
```k
z
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**dot_execute.k**:
```k
."2+2"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**dot_execute_context.k**:
```k
foo:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dot_execute_context.k**:
```k
."foo:8"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**dot_execute_context.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**dictionary_enumerate.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**dictionary_enumerate.k**:
```k
!d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**null_operations.k**:
```k
_n@7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dictionary_dot_apply.k**:
```k
d: .((`a;1;.());(`b;2;.()))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
**dictionary_dot_apply.k**:
```k
d@`a
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**monadic_format_basic.k**:
```k
$1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_types.k**:
```k
$42.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_vector.k**:
```k
$1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**monadic_format_string_hello.k**:
```k
$"hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_symbol_hello.k**:
```k
$`hello
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_symbol_simple.k**:
```k
$`test
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_dictionary.k**:
```k
$.((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**monadic_format_nested_list.k**:
```k
$((1;2;3);(4;5;6))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**monadic_format_integer.k**:
```k
$42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_float.k**:
```k
$3.14
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**monadic_format_vector_simple.k**:
```k
$1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**format_integer.k**:
```k
0$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**ci_adverb_vector.k**:
```k
_ci' 97 94 80
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**adverb_each_count.k**:
```k
#:' (1 2 3;1 2 3 4 5; 1 2)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
**format_float_numeric.k**:
```k
0.0$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**form_long.k**:
```k
0j$"42"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_numeric.k**:
```k
5$1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**form_string_pad_left.k**:
```k
7$"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_symbol_pad_left.k**:
```k
10$`hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_symbol_pad_left_8.k**:
```k
8$`hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_pad_left.k**:
```k
5$42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_pad_right.k**:
```k
-5$42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_float_width_precision.k**:
```k
10.2$3.14159
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_float_precision.k**:
```k
8.2$3.14159
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_0_1.k**:
```k
0$1.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_1_1.k**:
```k
1$1.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_symbol_string_mixed_vector.k**:
```k
`$("hello";"world";"test")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**form_integer_charvector.k**:
```k
0$"42"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dot_execute_variables.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dot_execute_variables.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dot_execute_variables.k**:
```k
."a%b"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**format_braces_expressions.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_expressions.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_nested_expr.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_nested_expr.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_nested_expr.k**:
```k
z:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_complex.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_complex.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_complex.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_string.k**:
```k
name:"John"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_string.k**:
```k
age:25
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_mixed_type.k**:
```k
num:42;txt:"hello";sym:`test;{}$("num";"txt";"sym";"num+5";"txt,\"world\"")
```
Incomplete consumption (position 3/27) (consumed 3/27)
-------------------------------------------------
**format_braces_simple.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_simple.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_arith.k**:
```k
a:5;b:3;x:10;y:2;{}$("a+b";"a*b";"a-b";"x+y";"x*y";"x%b")
```
Incomplete consumption (position 3/33) (consumed 3/33)
-------------------------------------------------
**format_braces_nested_arith.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_nested_arith.k**:
```k
b:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_nested_arith.k**:
```k
c:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_float.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_float.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_float.k**:
```k
c:3.0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_mixed_arith.k**:
```k
x:10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_mixed_arith.k**:
```k
y:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_mixed_arith.k**:
```k
z:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_example.k**:
```k
a:5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_example.k**:
```k
b:3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_function_calls.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**format_braces_function_calls.k**:
```k
product:{[x;y] x*y}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**format_braces_function_calls.k**:
```k
double:{[x] x*2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**format_braces_nested_function_calls.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**format_braces_nested_function_calls.k**:
```k
double:{[x] x*2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**format_braces_nested_function_calls.k**:
```k
square:{[x] x*x}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**log.k**:
```k
_log 10
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**time_t.k**:
```k
.((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**rand_draw_select.k**:
```k
r:10 _draw 4; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 5/23) (consumed 5/23)
-------------------------------------------------
**rand_draw_deal.k**:
```k
r:4 _draw -4; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
Incomplete consumption (position 5/36) (consumed 5/36)
-------------------------------------------------
**rand_draw_probability.k**:
```k
r:10 _draw 0; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 5/23) (consumed 5/23)
-------------------------------------------------
**rand_draw_vector_select.k**:
```k
r:2 3 _draw 4; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 6/24) (consumed 6/24)
-------------------------------------------------
**rand_draw_vector_deal.k**:
```k
r:2 3 _draw -10; .((`type;4:r);(`shape;^r);(`allitemsunique;(#r)=#?r))
```
Incomplete consumption (position 6/37) (consumed 6/37)
-------------------------------------------------
**rand_draw_vector_probability.k**:
```k
r:2 3 _draw 0; .((`type;4:r);(`shape;^r))
```
Incomplete consumption (position 6/24) (consumed 6/24)
-------------------------------------------------
**time_gtime.k**:
```k
_gtime 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**time_lt.k**:
```k
_lt 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**time_jd.k**:
```k
_jd 20260206
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**time_dj.k**:
```k
_dj 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**time_ltime.k**:
```k
_ltime 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**in.k**:
```k
4 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**assignment_lrs_return_value.k**:
```k
b:2*a:47;(a;b)
```
Incomplete consumption (position 7/14) (consumed 7/14)
-------------------------------------------------
**list_dv_basic.k**:
```k
3 4 4 5 _dv 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**list_dv_nomatch.k**:
```k
3 4 4 5 _dv 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**list_di_basic.k**:
```k
3 2 4 5 _di 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**list_di_multiple.k**:
```k
3 2 4 5 _di 1 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**list_sv_base10.k**:
```k
10 _sv 1 9 9 5
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**list_sv_base2.k**:
```k
2 _sv 1 0 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**list_sv_mixed.k**:
```k
10 10 10 10 _sv 1 9 9 5
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**list_getenv.k**:
```k
_getenv "PROMPT"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**list_setenv.k**:
```k
`TESTVAR _setenv "hello world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**list_size_existing.k**:
```k
_size "C:\\Windows\\System32\\write.exe"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_ci_basic.k**:
```k
_ci 65
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_ci_vector.k**:
```k
_ci 65 66 67
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**test_vs_dyadic.k**:
```k
10 _vs 1995
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**test_ic_vector.k**:
```k
_ic "ABC"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_monadic_colon.k**:
```k
: 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_sm_basic.k**:
```k
`foo _sm `foo
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**test_sm_simple.k**:
```k
`a _sm `a
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**test_ss_basic.k**:
```k
"hello world" _ss "world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**statement_assignment_basic.k**:
```k
a: 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**statement_assignment_inline.k**:
```k
1 + a: 42
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**statement_conditional_basic.k**:
```k
:[1;2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**statement_do_basic.k**:
```k
i:0;do[3;i+:1];i
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
**statement_do_simple.k**:
```k
i:0;do[3;i+:1]
```
Incomplete consumption (position 3/13) (consumed 3/13)
-------------------------------------------------
**semicolon_vars_test.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**apply_and_assign_simple.k**:
```k
i:0;i+:1;i
```
Incomplete consumption (position 3/10) (consumed 3/10)
-------------------------------------------------
**apply_and_assign_multiline.k**:
```k
i:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**apply_and_assign_multiline.k**:
```k
i+:1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**apply_and_assign_multiline.k**:
```k
i
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**io_read_basic.k**:
```k
0:`test
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**io_write_basic.k**:
```k
`testW 0: ("line1";"line2";"line3");0:`testW
```
Incomplete consumption (position 9/13) (consumed 9/13)
-------------------------------------------------
**io_append_simple.k**:
```k
`testfile 5: "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**io_append_basic.k**:
```k
`test 5: "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**io_append_multiple.k**:
```k
`test 5: "hello"; `test 5: "world"; `test 5: 1 2 3
```
Incomplete consumption (position 3/14) (consumed 3/14)
-------------------------------------------------
**io_read_bytes_basic.k**:
```k
`test 0:,"hello";6:`test
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
**io_read_bytes_empty.k**:
```k
`empty 0:(); 6:`empty
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
**io_write_bytes_basic.k**:
```k
`test 6:,"ABC"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**io_write_bytes_overwrite.k**:
```k
`test 6:,"ABC"; `test 6:,"XYZ"
```
Incomplete consumption (position 4/10) (consumed 4/10)
-------------------------------------------------
**io_write_bytes_binary.k**:
```k
`test 6:(0 1 2 255)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**search_in_basic.k**:
```k
4 _in 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**search_in_notfound.k**:
```k
6 _in 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**search_bin_basic.k**:
```k
3 4 5 6 _bin 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**search_binl_eachleft.k**:
```k
1 3 5 _binl 1 2 3 4 5
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**search_lin_intersection.k**:
```k
1 3 5 7 9 _lin 1 2 3 4 5
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**amend_test.k**:
```k
(.) . (((1 2 3 4 5);(6 7 8 9 10)); 0 2; +; 10)
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
**find_basic.k**:
```k
9 8 7 6 5 4 3 ? 7
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**find_notfound.k**:
```k
9 8 7 6 5 4 3 ? 1
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**format_float_precision_vector_simple.k**:
```k
10.1$(1.5;2.5)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**format_float_precision_mixed_vector.k**:
```k
7.2$(1.5;2.7;3.14159;4.2)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**format_pad_mixed_vector.k**:
```k
10$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**format_pad_negative_mixed_vector.k**:
```k
-10$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**vector_notation_empty.k**:
```k
()
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**vector_notation_functions.k**:
```k
double: {[x] x * 2}
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**vector_notation_functions.k**:
```k
(double 5; double 10; double 15)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**vector_notation_mixed_types.k**:
```k
(42; 3.14; "hello"; `symbol)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**vector_notation_nested.k**:
```k
((1 + 2); (3 + 4); (5 + 6))
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
**vector_notation_semicolon.k**:
```k
(3 + 4; 5 + 6; -20.45)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**vector_notation_single_group.k**:
```k
(42)
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**vector_notation_space.k**:
```k
1 2 3 4 5
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**vector_notation_variables.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**vector_notation_variables.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**vector_notation_variables.k**:
```k
(a + b; a * b; a - b)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**vector_addition.k**:
```k
1 2 + 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**vector_division.k**:
```k
1 2 % 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**vector_index_duplicate.k**:
```k
5 8 4 9 @ 0 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**vector_index_first.k**:
```k
5 8 4 9 @ 0
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**vector_index_multiple.k**:
```k
5 8 4 9 @ 1 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**vector_index_reverse.k**:
```k
5 8 4 9 @ 3 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**vector_index_single.k**:
```k
5 8 4 9 @ 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**vector_multiplication.k**:
```k
1 2 * 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**vector_subtraction.k**:
```k
1 2 - 3 4
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**vector_with_null.k**:
```k
(_n;1;2)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**vector_with_null_middle.k**:
```k
(1;_n;3)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**where_operator.k**:
```k
& (1 0 1 1 0)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**where_vector_counts.k**:
```k
& (3 2 1)
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**floor_operator.k**:
```k
_3.7
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**adverb_backslash_colon_basic.k**:
```k
1 2 3 +\: 4 5 6
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**adverb_slash_colon_basic.k**:
```k
1 2 3 +/: 4 5 6
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**adverb_tick_colon_basic.k**:
```k
-': 4 8 9 12 20
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**amend_apply.k**:
```k
(.) . (((1 2 3 4 5);(6 7 8 9 10)); 0 2; +; 10)
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
**amend_parenthesized.k**:
```k
(.) . (1 2 3; 0; +; 10)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**amend_test_anonymous_func.k**:
```k
f:{x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**amend_test_anonymous_func.k**:
```k
(.).((1 2 3); 0; f; 10)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
**amend_test_func_var.k**:
```k
f:{x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**amend_test_func_var.k**:
```k
(.).((1 2 3); 0; f; 10)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
**conditional_bracket_test.k**:
```k
:[1 < 2; "true"; "false"]
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**conditional_false.k**:
```k
:[0; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**conditional_simple_test.k**:
```k
:[1; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**conditional_true.k**:
```k
:[1; "true"; "false"]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**dictionary_null_index.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**dictionary_null_index.k**:
```k
d@_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**dictionary_unmake.k**:
```k
d: .((`a;1);(`b;2)); result:. d; result
```
Incomplete consumption (position 16/24) (consumed 16/24)
-------------------------------------------------
**do_loop.k**:
```k
i: 0; do[3; i+: 1]
```
Incomplete consumption (position 3/13) (consumed 3/13)
-------------------------------------------------
**dyadic_divide_bracket.k**:
```k
%[20; 4]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**dyadic_minus_bracket.k**:
```k
-[10; 3]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**dyadic_multiply_bracket.k**:
```k
*[4; 6]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**dyadic_plus_bracket.k**:
```k
+[3; 5]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**dyadic_divide_dot_apply.k**:
```k
(%) . (20; 4)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**dyadic_minus_dot_apply.k**:
```k
(-) . (10; 3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**dyadic_multiply_dot_apply.k**:
```k
(*) . (4; 6)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**dyadic_plus_dot_apply.k**:
```k
(+) . (3; 5)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**empty_brackets_dictionary.k**:
```k
d: .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**empty_brackets_dictionary.k**:
```k
d[]
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**empty_brackets_vector.k**:
```k
v: 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**empty_brackets_vector.k**:
```k
v[]
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_braces_complex_expressions.k**:
```k
sum:{[a;b] a+b}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**format_braces_complex_expressions.k**:
```k
product:{[x;y] x*y}
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**format_float_precision_complex_mixed.k**:
```k
10.3$(1.234;2.567;3.890;4.123)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**format_float_vector.k**:
```k
0.0$(1;2.5;3.14;42)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**format_int_vector.k**:
```k
0$(1;2.5;3.14;42)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**form_0_string.k**:
```k
0$"123"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**form_0_vector.k**:
```k
0$("123";"456")
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**form_0_float_string.k**:
```k
0.0$"3.14"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**form_0_float_vector.k**:
```k
0.0$("3.14";"1e48";"1.4e-27")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**form_symbol_string.k**:
```k
`$"abc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**form_symbol_vector.k**:
```k
`$("abc";"de";"f")
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**format_string_pad_left.k**:
```k
10$"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_string_pad_right.k**:
```k
-10$"test"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**format_vector_int.k**:
```k
0$(1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**group_operator.k**:
```k
a: 3 3 8 7 5 7 3 8 4 4 9 2 7 6 0 7 8 7 0 1
```
Incomplete consumption (position 22/23) (consumed 22/23)
-------------------------------------------------
**group_operator.k**:
```k
=a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**if_true.k**:
```k
a: 10; if[1 < 2; a: 20]
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
**in_basic.k**:
```k
4 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**in_notfound.k**:
```k
10 _in 1 7 2 4 6 3
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**in_simple.k**:
```k
5 _in 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**isolated.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**isolated.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**modulo.k**:
```k
a:1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**modulo.k**:
```k
b:2.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**modulo.k**:
```k
a%b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**monadic_format_mixed_vector.k**:
```k
$(1;2.5;"hello";`symbol)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**simple_division.k**:
```k
8 % 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**simple_subtraction.k**:
```k
5 - 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**string_parse.k**:
```k
a: 10
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**string_parse.k**:
```k
b: 20
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**string_parse.k**:
```k
."a+b"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**k_tree_assignment_absolute_foo.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**k_tree_retrieve_absolute_foo.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**k_tree_retrieve_absolute_foo.k**:
```k
.k.foo
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**k_tree_retrieval_relative.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**k_tree_retrieval_relative.k**:
```k
foo
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**k_tree_enumerate.k**:
```k
!`
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**k_tree_current_branch.k**:
```k
_d
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**k_tree_dictionary_indexing.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**k_tree_dictionary_indexing.k**:
```k
.k[`foo]
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**k_tree_nested_indexing.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
**k_tree_nested_indexing.k**:
```k
.k.dd[`b]
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**k_tree_verify_root.k**:
```k
.k
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**k_tree_flip_dictionary.k**:
```k
.+(`a`b`c;1 2 3)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**k_tree_null_to_dict_conversion.k**:
```k
.k.foo: 42
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**k_tree_null_to_dict_conversion.k**:
```k
.k
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**k_tree_dictionary_assignment.k**:
```k
.k.dd: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
**k_tree_dictionary_assignment.k**:
```k
.k.dd
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**k_tree_test_bracket_indexing.k**:
```k
d: .((`a;1);(`b;2);(`c;3))
```
Incomplete consumption (position 22/23) (consumed 22/23)
-------------------------------------------------
**k_tree_test_bracket_indexing.k**:
```k
d[`b]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**k_tree_flip_test.k**:
```k
+(`a`b`c;1 2 3)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**vector_null_index.k**:
```k
v: 1 2 3 4
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**vector_null_index.k**:
```k
v@_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**while_bracket_test.k**:
```k
i: 0; while[i < 3; i+: 1]
```
Incomplete consumption (position 3/15) (consumed 3/15)
-------------------------------------------------
**while_safe_test.k**:
```k
i: 0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**serialization_bd_db_integer.k**:
```k
_bd 42
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_db_float.k**:
```k
_bd 3.14159
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_db_symbol.k**:
```k
_bd `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_db_null.k**:
```k
_bd _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_db_integervector.k**:
```k
_bd 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**serialization_bd_db_floatvector.k**:
```k
_bd 1.1 2.2 3.3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**serialization_bd_db_charactervector.k**:
```k
_bd "hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_db_symbolvector.k**:
```k
_bd `a`b`c
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**serialization_bd_db_dictionary.k**:
```k
_bd .((`a;`"1");(`b;`"2"))
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**serialization_bd_db_anonymousfunction.k**:
```k
_bd {[x] x+1}
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_basic_integer.k**:
```k
_db _bd 42
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_float.k**:
```k
_db _bd 3.14
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_symbol.k**:
```k
_db _bd `test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_int_vector.k**:
```k
_db _bd 1 2 3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**db_char_vector.k**:
```k
_db _bd "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_list_simple.k**:
```k
_db _bd (1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**db_dict_simple.k**:
```k
_db _bd .((`a;1);(`b;2);(`c;3;))
```
Incomplete consumption (position 23/24) (consumed 23/24)
-------------------------------------------------
**db_function_simple.k**:
```k
_db _bd {+}
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**db_function_params.k**:
```k
_db _bd {x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**db_null.k**:
```k
_db _bd 0N
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_empty_list.k**:
```k
_db _bd ()
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**db_float_simple.k**:
```k
_db _bd 1.5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_symbol_simple.k**:
```k
_db _bd `hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_list_longer.k**:
```k
_db _bd (1;2;3;4;5)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**db_list_mixed_types.k**:
```k
_db _bd (1;`test;3.14;"hello")
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**db_function_complex.k**:
```k
_db _bd {[x;y] x*y+z}
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**db_function_simple_math.k**:
```k
_db _bd {x+y}
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**db_nested_dict_vectors.k**:
```k
_db _bd .((`a;(1;2;3));(`b;(`hello;`world;`test)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
**db_nested_lists.k**:
```k
_db _bd ((1;2;3);(`hello;`world;`test);(4.5;6.7))
```
Incomplete consumption (position 25/26) (consumed 25/26)
-------------------------------------------------
**db_mixed_list.k**:
```k
_db _bd (1;`test;3.14)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**db_dict_single_entry.k**:
```k
_db _bd .,(`a;1)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**db_dict_symbol_2char.k**:
```k
_db _bd .,(`ab;1 2)
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**db_dict_symbol_8char.k**:
```k
_db _bd .,(`abcdefgh;1 2 3 4 5 6 7 8)
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**db_dict_multi_entry.k**:
```k
_db _bd .((`a;1);(`b;2))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**db_dict_five_entries.k**:
```k
_db _bd .((`a;1);(`b;2);(`c;3);(`d;4);(`e;5))
```
Incomplete consumption (position 34/35) (consumed 34/35)
-------------------------------------------------
**db_dict_complex_attributes.k**:
```k
_db _bd .((`col01; 11 12 13 14 15;.((`format;,`"n");(`name;`ID)));(`col02; `yellow`white`blue`red`black;.((`format;,`"c");(`name;`Color)));(`col03; ("Home Depot";"Lowes";"Ace";"Neighborhood Paints";"Supply Co.");.((`format;,`"c");(`name;`Retailer))))
```
Incomplete consumption (position 88/89) (consumed 88/89)
-------------------------------------------------
**db_dict_empty.k**:
```k
_db _bd .()
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**db_dict_with_null_attrs.k**:
```k
_db _bd .,(`a;1;.())
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**db_dict_with_empty_attrs.k**:
```k
_db _bd .((`a;1);(`b;2;.()))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**db_enlist_single_int.k**:
```k
_db _bd ,5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**db_enlist_single_symbol.k**:
```k
_db _bd ,`test
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**db_enlist_single_string.k**:
```k
_db _bd ,"hello"
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**db_enlist_vector.k**:
```k
_db _bd ,(1 2 3)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**serialization_bd_null_edge_0.k**:
```k
_bd _n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_0.k**:
```k
_bd 0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_1.k**:
```k
_bd 1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_-1.k**:
```k
_bd -1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_2147483647.k**:
```k
_bd 2147483647
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_-2147483648.k**:
```k
_bd -2147483648
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_0N.k**:
```k
_bd 0N
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_0I.k**:
```k
_bd 0I
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integer_edge_-0I.k**:
```k
_bd -0I
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_0.0.k**:
```k
_bd 0.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_1.0.k**:
```k
_bd 1.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_-1.0.k**:
```k
_bd -1.0
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_0.5.k**:
```k
_bd 0.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_-0.5.k**:
```k
_bd -0.5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_0n.k**:
```k
_bd 0n
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_0i.k**:
```k
_bd 0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_float_edge_-0i.k**:
```k
_bd -0i
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_a.k**:
```k
_bd `a
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_symbol.k**:
```k
_bd `symbol
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_test123.k**:
```k
_bd `test123
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_underscore.k**:
```k
_bd `_underscore
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_hello.k**:
```k
_bd `"hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_newline_tab.k**:
```k
_bd `"\n\t"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_001.k**:
```k
_bd `"\001"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_symbol_edge_empty.k**:
```k
_bd `
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_charactervector_edge_empty.k**:
```k
_bd ""
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_charactervector_edge_hello.k**:
```k
_bd "hello"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_charactervector_edge_whitespace.k**:
```k
_bd "\n\t\r"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_integervector_edge_empty.k**:
```k
_bd !0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**serialization_bd_integervector_edge_single.k**:
```k
_bd ,1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**serialization_bd_integervector_edge_123.k**:
```k
_bd 1 2 3
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**serialization_bd_integervector_edge_special.k**:
```k
_bd 0N 0I -0I
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**serialization_bd_list_edge_empty.k**:
```k
_bd ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**serialization_bd_list_edge_null.k**:
```k
_bd ,_n
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**serialization_bd_list_edge_complex.k**:
```k
_bd (_n;`symbol;{[]})
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**serialization_bd_list_edge_nested.k**:
```k
_bd ((1;2);(3;4))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**serialization_bd_list_edge_dicts.k**:
```k
_bd (.,(`a;1);.,(`b;2))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**serialization_bd_anonymousfunction_random_3.k**:
```k
_bd {[xyz] xy|3}
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**serialization_bd_floatvector_random_1.k**:
```k
_bd 196825.27335627712 -326371.90292283031 -214498.92985862558 11655.819143220946
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**serialization_bd_floatvector_random_2.k**:
```k
_bd 148812.33236087282
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**serialization_bd_floatvector_random_3.k**:
```k
_bd 585267.57816312299 -569668.94176055992 37200.312770306708 397004.01885714347
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**serialization_bd_symbolvector_random_1.k**:
```k
_bd `qzUM7 `g8X6P `"iay" `KgNQ5i `"< +" `b5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**serialization_bd_symbolvector_random_2.k**:
```k
_bd `"O 0" `D `qCBI1b `"*H " `"SS" `ULsyI `"F~" `"C" `Mont `O25B
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**serialization_bd_symbolvector_random_3.k**:
```k
_bd `o3 `EE5ijP `trD0LuE `OW `"." `y
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**test_quoted_symbol.k**:
```k
`"."
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**test_quoted_symbol_serialization.k**:
```k
_bd `"."
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_simple_symbol.k**:
```k
`a `"." `b
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**test_single_quoted_symbol.k**:
```k
`"."
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**test_symbol_vector_with_quoted.k**:
```k
`a `b `"." `c
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**serialization_bd_dictionary_with_symbol_vectors.k**:
```k
_bd .((`colA;`a `b `c);(`colB;`dd `eee `ffff))
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
**serialization_bd_dictionary_with_vectors.k**:
```k
_bd .((`col1;1 2 3 4);(`col2;5 6 7 8))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**serialization_bd_list_with_explicit_nulls.k**:
```k
_bd ((`a;`"1";);(`b;`"2";))
```
Incomplete consumption (position 16/17) (consumed 16/17)
-------------------------------------------------
**serialization_bd_list_with_vectors.k**:
```k
_bd ((`col1;1 2 3 4);(`col2;5 6 7 8))
```
Incomplete consumption (position 20/21) (consumed 20/21)
-------------------------------------------------
**serialization_bd_list_with_symbol_vectors.k**:
```k
_bd ((`colA;`a `b `c);(`colB;`dd `eee `ffff))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**bd_dict_single_entry.k**:
```k
_bd .,(`a;1)
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**db_dict_larger.k**:
```k
_db _bd .((`a;1);(`b;2;);(`c;3;);(`d;4;))
```
Incomplete consumption (position 31/32) (consumed 31/32)
-------------------------------------------------
**db_dict_mixed_types.k**:
```k
_db _bd .((`key1;`value1;);(`key2;42;);(`key3;3.14))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
**db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**db_nested_structures.k**:
```k
_db _bd .((`a;(1;2;3));(`b;(4;5;6)))
```
Incomplete consumption (position 28/29) (consumed 28/29)
-------------------------------------------------
**db_string_hello.k**:
```k
_db _bd "hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_symbol_hello.k**:
```k
_db _bd `hello
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**test_dict_larger.k**:
```k
.((`a;1;);(`b;2;);(`c;3;);(`d;4;))
```
Incomplete consumption (position 30/31) (consumed 30/31)
-------------------------------------------------
**test_dict_simple.k**:
```k
.((`a;1);(`b;2);(`c;3;))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**symbol_special_chars.k**:
```k
`"hello-world!"
```
Incomplete consumption (position 1/2) (consumed 1/2)
-------------------------------------------------
**type_empty_int_vector.k**:
```k
4: !0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**bd_empty_list.k**:
```k
_bd ()
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**bd_enlist_single_int.k**:
```k
_bd ,5
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**bd_enlist_single_string.k**:
```k
_bd ,"hello"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**bd_symbol_vector_longer.k**:
```k
_bd `hello`world`test
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**bd_enlist_single_symbol.k**:
```k
_bd ,`test
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_and_basic.k**:
```k
5 _and 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_and_vector.k**:
```k
(5 6 3) _and (1 2 3)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**math_ceil_basic.k**:
```k
_ceil 4.7
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_ceil_integer.k**:
```k
_ceil 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_ceil_negative.k**:
```k
_ceil -3.2
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_ceil_vector.k**:
```k
_ceil 1.2 2.7 3.5
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**math_div_float.k**:
```k
7 _div 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_div_integer.k**:
```k
7 _div 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_div_vector.k**:
```k
(7 14 21) _div 3
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**math_dot_basic.k**:
```k
1 2 3 _dot 4 5 6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**math_dot_matrix_matrix.k**:
```k
(1 2 3;4 5 6) _dot (7 8 9;10 11 12)
```
Incomplete consumption (position 19/20) (consumed 19/20)
-------------------------------------------------
**math_dot_matrix_2x2.k**:
```k
(1 2;3 4) _dot (5 6;7 8)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**math_dot_vector_each_left.k**:
```k
(1 2) _dot\: (3 4;5 6)
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**adverb_complex_vector_each_right.k**:
```k
10 11 12 +/: ((1 2 3);(4 5 6);(7 8 9))
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
**adverb_complex_matrix_each_right.k**:
```k
((1 2 3);(4 5 6);(7 8 9)) +/: 10 11 12
```
Incomplete consumption (position 24/25) (consumed 24/25)
-------------------------------------------------
**adverb_chaining_join_each_left.k**:
```k
((1 2 3);(4 5 6);(7 8 9)),/:\:((9 8 7);(6 5 4);(3 2 1))
```
Incomplete consumption (position 41/42) (consumed 41/42)
-------------------------------------------------
**adverb_complex_string_each_left.k**:
```k
(("hello";"world.");("It's";"me";"ksharp.");("Have";"fun";"with";"me!")),/:\:"  "
```
Incomplete consumption (position 29/30) (consumed 29/30)
-------------------------------------------------
**join_each_left.k**:
```k
(1 2 3),\: (4 5 6)
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**test_nested_adverb.k**:
```k
1 2 3 ,/:\: 4 5 6
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**math_lsq_non_square.k**:
```k
(7 8 9) _lsq (1 2 3;4 5 6)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**math_lsq_high_rank.k**:
```k
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**math_lsq_complex.k**:
```k
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**math_lsq_regression.k**:
```k
(1 2 3.0) _lsq (1 1 1.0;1 2 4.0)
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**math_mul_basic.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_not_basic.k**:
```k
_not 5
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**math_not_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_or_basic.k**:
```k
5 _or 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_or_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_rot_basic.k**:
```k
8 _rot 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_shift_basic.k**:
```k
8 _shift 2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_shift_vector.k**:
```k
(8 16 32 64) _shift 2
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**math_xor_basic.k**:
```k
5 _xor 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**math_xor_vector.k**:
```k
1 2 3
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**ffi_hint_system.k**:
```k
42 _sethint `uint
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**ffi_simple_assembly.k**:
```k
str:"System.Private.CoreLib" 2: `System.String; str
```
Incomplete consumption (position 5/8) (consumed 5/8)
-------------------------------------------------
**ffi_assembly_load.k**:
```k
"System.Private.CoreLib" 2: `System.String
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**ffi_type_marshalling.k**:
```k
f:3.14159;f _sethint `float;s:"hello";s _sethint `string;l:1 2 3 4 5;l: _sethint `list
```
Incomplete consumption (position 3/29) (consumed 3/29)
-------------------------------------------------
**ffi_object_management.k**:
```k
str: "hello";str _sethint `object; str . ToUpper
```
Incomplete consumption (position 3/12) (consumed 3/12)
-------------------------------------------------
**ffi_constructor.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**ffi_constructor.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**ffi_constructor.k**:
```k
c1:complex_new[2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**ffi_constructor.k**:
```k
.((`real;c1[`Real]);(`imag;c1[`Imaginary]);(`instance;!c1);(`type;!complex))
```
Incomplete consumption (position 34/35) (consumed 34/35)
-------------------------------------------------
**ffi_dispose.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**ffi_dispose.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**ffi_dispose.k**:
```k
c1:complex_new[2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**ffi_dispose.k**:
```k
_dispose c1
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**ffi_dispose.k**:
```k
c1 @ `_this
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**ffi_complete_workflow.k**:
```k
complex:"C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\8.0.25\\System.Runtime.Numerics.dll" 2: `System.Numerics.Complex
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**ffi_complete_workflow.k**:
```k
complex_new:complex[`constructor]
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**ffi_complete_workflow.k**:
```k
c1:complex_new[2;3]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**ffi_complete_workflow.k**:
```k
magnitude: c1[`Abs][]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**ffi_complete_workflow.k**:
```k
conj_func: ._dotnet.System.Numerics.Complex.Conjugate
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**ffi_complete_workflow.k**:
```k
.((`real;c1[`Real]);(`imag;c1[`Imaginary]);(`magnitude;magnitude);(`instance;!c1);(`type;!complex))
```
Incomplete consumption (position 40/41) (consumed 40/41)
-------------------------------------------------
**idioms_01_575_kronecker_delta.k**:
```k
x:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_575_kronecker_delta.k**:
```k
y:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_575_kronecker_delta.k**:
```k
x=y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_571_xbutnoty.k**:
```k
x:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_571_xbutnoty.k**:
```k
y:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_571_xbutnoty.k**:
```k
x>y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_570_implies.k**:
```k
x:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_570_implies.k**:
```k
y:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_570_implies.k**:
```k
~x>y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_573_exclusive_or.k**:
```k
x:0 0 1 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_573_exclusive_or.k**:
```k
y:0 1 0 1
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_573_exclusive_or.k**:
```k
~x=y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_41_indices_ones.k**:
```k
x:0 0 1 0 1 0 0 0 1 0
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**idioms_01_41_indices_ones.k**:
```k
&x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_516_multiply_columns.k**:
```k
x:(1 2 3 4 5 6;7 8 9 10 11 12)
```
Incomplete consumption (position 17/18) (consumed 17/18)
-------------------------------------------------
**idioms_01_516_multiply_columns.k**:
```k
y:10 100
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_516_multiply_columns.k**:
```k
x*y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_566_zero_boolean.k**:
```k
x:0 1 0 1 1 0 0 1 1 1 0
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**idioms_01_566_zero_boolean.k**:
```k
0&x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_624_zero_array.k**:
```k
x:2 3#99
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_624_zero_array.k**:
```k
x*0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_622_retain_marked.k**:
```k
x:3 7 15 1 292
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_622_retain_marked.k**:
```k
y:1 0 1 1 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_622_retain_marked.k**:
```k
x*y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_331_identity_max.k**:
```k
-1e100|-0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_337_identity_min.k**:
```k
1e100&0i
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_357_match.k**:
```k
x:("abc";`sy;1 3 -7)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**idioms_01_357_match.k**:
```k
y:("abc";`sy;1 3 -7)
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**idioms_01_357_match.k**:
```k
x~y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_328_number_items.k**:
```k
#"abcd"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_411_number_rows.k**:
```k
#x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_445_number_columns.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_445_number_columns.k**:
```k
*|^x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**test_parse_verb.k**:
```k
_parse "1 + 2"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_parse_eval_together.k**:
```k
parse_tree: _parse "1 + 2"; _eval parse_tree
```
Incomplete consumption (position 4/8) (consumed 4/8)
-------------------------------------------------
**idioms_01_388_drop_rows.k**:
```k
x:6 3#!18
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_388_drop_rows.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_388_drop_rows.k**:
```k
y _ x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_154_range.k**:
```k
x:"wirlsisl"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_154_range.k**:
```k
?x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_70_remove_duplicates.k**:
```k
x:("to";"be";"or";"not";"to";"be")
```
Incomplete consumption (position 15/16) (consumed 15/16)
-------------------------------------------------
**idioms_01_70_remove_duplicates.k**:
```k
?x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_143_indices_distinct.k**:
```k
x:"ajhajhja"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_143_indices_distinct.k**:
```k
=x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_228_is_row.k**:
```k
x:("xxx";"yyy";"zzz";"yyy")
```
Incomplete consumption (position 11/12) (consumed 11/12)
-------------------------------------------------
**idioms_01_228_is_row.k**:
```k
x?"yyy"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_232_is_row_in.k**:
```k
x:("aaa";"bbb";"ooo";"ppp";"kkk")
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**idioms_01_232_is_row_in.k**:
```k
y:"ooo"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_232_is_row_in.k**:
```k
y _in x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_559_first_marker.k**:
```k
x:0 0 1 0 1 0 0 1 1 0
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**idioms_01_559_first_marker.k**:
```k
x?1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_78_eval_number.k**:
```k
x:"1998 51"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_78_eval_number.k**:
```k
. x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_88_name_variable.k**:
```k
x:"test"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_88_name_variable.k**:
```k
y:2 3#!6
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_88_name_variable.k**:
```k
. "var",($x),":y"
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**idioms_01_493_choose_boolean.k**:
```k
x:"abcdef"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_493_choose_boolean.k**:
```k
y:"xyz"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_493_choose_boolean.k**:
```k
g:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_493_choose_boolean.k**:
```k
:[g;x;y]
```
Incomplete consumption (position 8/9) (consumed 8/9)
-------------------------------------------------
**idioms_01_434_replace_first.k**:
```k
x:"abbccdefcdab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_433_replace_last.k**:
```k
x:"abbccdefcdab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_406_add_last.k**:
```k
x:1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_406_add_last.k**:
```k
y:100
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_449_limit_between.k**:
```k
x:(58 9 37 84 39 99;60 30 45 97 77 35;49 87 82 79 8 30;46 61 20 51 12 34;31 51 29 35 17 89)
```
Incomplete consumption (position 38/39) (consumed 38/39)
-------------------------------------------------
**idioms_01_449_limit_between.k**:
```k
l:30
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_449_limit_between.k**:
```k
h:70
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_449_limit_between.k**:
```k
l|h&x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**idioms_01_495_indices_occurrences.k**:
```k
x:"abcdefgab"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_495_indices_occurrences.k**:
```k
y:"afc*"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_495_indices_occurrences.k**:
```k
&x _lin y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_504_replace_satisfying.k**:
```k
x:1 0 0 0 1 0 1 1 0 1
```
Incomplete consumption (position 12/13) (consumed 12/13)
-------------------------------------------------
**idioms_01_504_replace_satisfying.k**:
```k
y:"abcdefghij"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_569_change_to_one.k**:
```k
y:10 5 7 12 20
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_569_change_to_one.k**:
```k
x:0 1 0 1 1
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_569_change_to_one.k**:
```k
y^~x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_556_all_indices.k**:
```k
x:2 2 2 2
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_556_all_indices.k**:
```k
!#x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_535_avoid_parentheses.k**:
```k
x:1 2 3 4 5
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_535_avoid_parentheses.k**:
```k
|1,#x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**idioms_01_591_reshape_2column.k**:
```k
x:"abcdefgh"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_591_reshape_2column.k**:
```k
((_ 0.5*#x),2)#x
```
Incomplete consumption (position 13/14) (consumed 13/14)
-------------------------------------------------
**idioms_01_595_one_row_matrix.k**:
```k
x:2 3 5 7 11
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_595_one_row_matrix.k**:
```k
,x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_616_scalar_from_vector.k**:
```k
x:,8
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_616_scalar_from_vector.k**:
```k
*x
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**idioms_01_509_remove_y.k**:
```k
x:"abcdeabc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_509_remove_y.k**:
```k
x _dv y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_510_remove_blanks.k**:
```k
x:" bcde bc"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_496_remove_punctuation.k**:
```k
x:"oh! no, stop it. you will?"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_496_remove_punctuation.k**:
```k
y:",;:.!?"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_496_remove_punctuation.k**:
```k
x _dvl y
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_177_string_search.k**:
```k
x:"st"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_177_string_search.k**:
```k
y:"indices of start of string x in string y"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_177_string_search.k**:
```k
y _ss x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_45_binary_representation.k**:
```k
x:16
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_45_binary_representation.k**:
```k
2 _vs x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_84_scalar_boolean.k**:
```k
x:1 0 0 1 1 1 0 1
```
Incomplete consumption (position 10/11) (consumed 10/11)
-------------------------------------------------
**idioms_01_84_scalar_boolean.k**:
```k
2 _sv x
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_129_arctangent.k**:
```k
y:1
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_561_numeric_code.k**:
```k
x:" aA0"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_561_numeric_code.k**:
```k
_ic[x]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_241_sum_subsets.k**:
```k
x:1+3 4#!12
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**idioms_01_241_sum_subsets.k**:
```k
y:4 3#1 0
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_241_sum_subsets.k**:
```k
x _mul y
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_61_cyclic_counter.k**:
```k
x:!10
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_61_cyclic_counter.k**:
```k
y:8
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_61_cyclic_counter.k**:
```k
1+x!y
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**idioms_01_384_drop_1st_postpend.k**:
```k
x:3 4 5 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_384_drop_1st_postpend.k**:
```k
1 _ x,0
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**idioms_01_385_drop_last_prepend.k**:
```k
x:3 4 5 6
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**idioms_01_385_drop_last_prepend.k**:
```k
-1 _ 0,x
```
Incomplete consumption (position 5/6) (consumed 5/6)
-------------------------------------------------
**idioms_01_178_first_occurrence.k**:
```k
x:"st"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_178_first_occurrence.k**:
```k
y:"index of first occurrence of string x in string y"
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_178_first_occurrence.k**:
```k
*y _ss x
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**idioms_01_447_conditional_drop.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_447_conditional_drop.k**:
```k
y:2
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_447_conditional_drop.k**:
```k
g:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_447_conditional_drop.k**:
```k
(y*g) _ x
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_448_conditional_drop_last.k**:
```k
x:4 3#!12
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**idioms_01_448_conditional_drop_last.k**:
```k
y:0
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**idioms_01_448_conditional_drop_last.k**:
```k
(-y) _ x
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**ktree_enumerate_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_enumerate_relative_name.k**:
```k
!d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**ktree_enumerate_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_enumerate_relative_path.k**:
```k
!`d
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**ktree_enumerate_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_enumerate_absolute_path.k**:
```k
!(.k.d)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**ktree_enumerate_root.k**:
```k
!`
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**ktree_indexing_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_indexing_relative_name.k**:
```k
d[`keyB]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**ktree_indexing_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_indexing_absolute_name.k**:
```k
.k.d[`keyA]
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------
**ktree_indexing_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_indexing_relative_path.k**:
```k
`d[`keyA]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**ktree_indexing_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_indexing_absolute_path.k**:
```k
`.k.d[`keyB]
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**ktree_dot_apply_relative_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); d . `keyA
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
**ktree_dot_apply_absolute_name.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5))
```
Incomplete consumption (position 21/22) (consumed 21/22)
-------------------------------------------------
**ktree_dot_apply_absolute_name.k**:
```k
.k.d . `keyB
```
Incomplete consumption (position 6/7) (consumed 6/7)
-------------------------------------------------
**ktree_dot_apply_relative_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `d . `keyA
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
**ktree_dot_apply_absolute_path.k**:
```k
d:.((`keyA;1 2 3;);(`keyB;1 3 5)); `.k.d . `keyB
```
Incomplete consumption (position 21/26) (consumed 21/26)
-------------------------------------------------
**test_semicolon_parsing.k**:
```k
x: (1;2;3)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**test_parse_monadic_star.k**:
```k
_parse "*1 2 3 4"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**parse_atomic_value_no_verb.k**:
```k
_parse "`a"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**parse_projection_dyadic_plus.k**:
```k
_parse "(+)"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**parse_projection_dyadic_plus_fixed_left.k**:
```k
_parse "1+"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**parse_projection_dyadic_plus_fixed_right.k**:
```k
_parse "+[;2]"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**parse_monadic_shape_atomic.k**:
```k
_parse "^,`a"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**eval_dyadic_plus.k**:
```k
_eval (`"+";5 6 7 8;1 2 3 4)
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**eval_monadic_star_nested.k**:
```k
_eval (`"*";2;(`"+";4;7))
```
Incomplete consumption (position 14/15) (consumed 14/15)
-------------------------------------------------
**eval_dot_execute_path.k**:
```k
v:`e`f
```
Incomplete consumption (position 4/5) (consumed 4/5)
-------------------------------------------------
**eval_dot_execute_path.k**:
```k
_eval (`",";,`a`b`c;(`",";,`d;`v))
```
Incomplete consumption (position 18/19) (consumed 18/19)
-------------------------------------------------
**eval_dot_repl_dir.k**:
```k
."\\d ^";.d:_d;."\\d .k";(.d;_d)
```
Incomplete consumption (position 2/18) (consumed 2/18)
-------------------------------------------------
**eval_dot_parse_and_eval.k**:
```k
a:7
```
Incomplete consumption (position 3/4) (consumed 3/4)
-------------------------------------------------
**eval_dot_parse_and_eval.k**:
```k
. "a+4"
```
Incomplete consumption (position 2/3) (consumed 2/3)
-------------------------------------------------
**test_eval_monadic_star.k**:
```k
_eval (`"*:";1 2 3 4)
```
Incomplete consumption (position 9/10) (consumed 9/10)
-------------------------------------------------
**test_eval_monadic_star_atomic.k**:
```k
_eval (`"*:";,1)
```
Incomplete consumption (position 7/8) (consumed 7/8)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
