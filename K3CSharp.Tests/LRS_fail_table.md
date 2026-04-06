# K3CSharp Parser Failures

**Generated:** 2026-04-06 11:01:32
**Test Results:** 725/861 passed (84.2%)

## Executive Summary

**Total Tests:** 861
**Passed Tests:** 725
**Failed Tests:** 136
**Success Rate:** 84.2%

**LRS Parser Statistics:**
- NULL Results: 40
- Incomplete Token Consumption: 1
- Total Fallbacks to Legacy: 41
- Incorrect Results: 0
- LRS is handling: 95.2%

**Top Failure Patterns:**
- After INTEGER (position 3/4): 7
- After SYMBOL (position 3/4): 5
- After CHARACTER_VECTOR (position 3/4): 4
- After RIGHT_PAREN (position 9/10): 3
- After FLOAT (position 3/4): 2
- After SYMBOL (position 5/6): 2
- After RIGHT_BRACE (position 7/8): 2
- Incomplete consumption (position 8/11): 1
- After INTEGER (position 4/5): 1
- After INTEGER (position 5/6): 1

## LRS Parser Failures

### NULL Results (LRS returned NULL)

1. **apply_and_assign_multiline.k**:
```k
i+:1
```
After INTEGER (position 4/5)
-------------------------------------------------
2. **serialization_bd_db_roundtrip_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
3. **serialization_bd_db_roundtrip_list.k**:
```k
_db _bd (1;2.5;"a")
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
4. **serialization_bd_ic_symbol.k**:
```k
_ic _bd `A
```
After SYMBOL (position 3/4)
-------------------------------------------------
5. **db_basic_integer.k**:
```k
_db _bd 42
```
After INTEGER (position 3/4)
-------------------------------------------------
6. **db_float.k**:
```k
_db _bd 3.14
```
After FLOAT (position 3/4)
-------------------------------------------------
7. **db_symbol.k**:
```k
_db _bd `test
```
After SYMBOL (position 3/4)
-------------------------------------------------
8. **db_int_vector.k**:
```k
_db _bd 1 2 3
```
After INTEGER (position 5/6)
-------------------------------------------------
9. **db_symbol_vector.k**:
```k
_db _bd `a`b`c
```
After SYMBOL (position 5/6)
-------------------------------------------------
10. **db_char_vector.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
11. **db_list_simple.k**:
```k
_db _bd (1;2;3)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
12. **db_function_simple.k**:
```k
_db _bd {+}
```
After RIGHT_BRACE (position 5/6)
-------------------------------------------------
13. **db_function_params.k**:
```k
_db _bd {x+y}
```
After RIGHT_BRACE (position 7/8)
-------------------------------------------------
14. **db_null.k**:
```k
_db _bd 0N
```
After INTEGER (position 3/4)
-------------------------------------------------
15. **db_empty_list.k**:
```k
_db _bd ()
```
After RIGHT_PAREN (position 4/5)
-------------------------------------------------
16. **db_character.k**:
```k
_db _bd "a"
```
After CHARACTER (position 3/4)
-------------------------------------------------
17. **db_float_simple.k**:
```k
_db _bd 1.5
```
After FLOAT (position 3/4)
-------------------------------------------------
18. **db_int_vector_long.k**:
```k
_db _bd 1 2 3 4 5
```
After INTEGER (position 7/8)
-------------------------------------------------
19. **db_float_vector.k**:
```k
_db _bd 1.1 2.2 3.3
```
After FLOAT (position 5/6)
-------------------------------------------------
20. **db_char_vector_sentence.k**:
```k
_db _bd "hello world"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
21. **db_symbol_simple.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
22. **db_list_longer.k**:
```k
_db _bd (1;2;3;4;5)
```
After RIGHT_PAREN (position 13/14)
-------------------------------------------------
23. **db_list_mixed_types.k**:
```k
_db _bd (1;`test;3.14;"hello")
```
After RIGHT_PAREN (position 11/12)
-------------------------------------------------
24. **db_function_complex.k**:
```k
_db _bd {[x;y] x*y+z}
```
After RIGHT_BRACE (position 14/15)
-------------------------------------------------
25. **db_function_simple_math.k**:
```k
_db _bd {x+y}
```
After RIGHT_BRACE (position 7/8)
-------------------------------------------------
26. **db_nested_lists.k**:
```k
_db _bd ((1;2;3);(`hello;`world;`test);(4.5;6.7))
```
After RIGHT_PAREN (position 25/26)
-------------------------------------------------
27. **db_mixed_list.k**:
```k
_db _bd (1;`test;3.14)
```
After RIGHT_PAREN (position 9/10)
-------------------------------------------------
28. **serialization_bd_integervector_edge_empty.k**:
```k
_bd !0
```
After INTEGER (position 3/4)
-------------------------------------------------
29. **serialization_bd_integervector_edge_single.k**:
```k
_bd ,1
```
After INTEGER (position 3/4)
-------------------------------------------------
30. **serialization_bd_list_edge_null.k**:
```k
_bd ,_n
```
After NULL (position 3/4)
-------------------------------------------------
31. **db_float_vector_longer.k**:
```k
_db _bd 1.1 2.2 3.3 4.4 5.5
```
After FLOAT (position 7/8)
-------------------------------------------------
32. **db_int_vector_longer.k**:
```k
_db _bd 1 2 3 4 5 6 7 8 9 10
```
After INTEGER (position 12/13)
-------------------------------------------------
33. **db_string_hello.k**:
```k
_db _bd "hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
34. **db_symbol_hello.k**:
```k
_db _bd `hello
```
After SYMBOL (position 3/4)
-------------------------------------------------
35. **db_symbol_vector_longer.k**:
```k
_db _bd `hello`world`test
```
After SYMBOL (position 5/6)
-------------------------------------------------
36. **type_empty_int_vector.k**:
```k
4: !0
```
After INTEGER (position 3/4)
-------------------------------------------------
37. **bd_enlist_single_int.k**:
```k
_bd ,5
```
After INTEGER (position 3/4)
-------------------------------------------------
38. **bd_enlist_single_string.k**:
```k
_bd ,"hello"
```
After CHARACTER_VECTOR (position 3/4)
-------------------------------------------------
39. **bd_enlist_single_symbol.k**:
```k
_bd ,`test
```
After SYMBOL (position 3/4)
-------------------------------------------------
40. **idioms_01_556_all_indices.k**:
```k
!#x
```
After IDENTIFIER (position 3/4)
-------------------------------------------------
### Incomplete Token Consumption (LRS returned result but didn't consume all tokens)

**apply_and_assign_simple.k**:
```k
i:0;i+:1;i
```
Incomplete consumption (position 8/11) (consumed 8/11)
-------------------------------------------------

---

*Report generated by K3CSharp Parser Analysis System*
