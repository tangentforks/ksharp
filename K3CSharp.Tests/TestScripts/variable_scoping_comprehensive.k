// Test comprehensive variable scoping
globalVar: 100

// Test 1: Function accessing global variable
test1: {[x] globalVar + x}
result1: test1 . 50
result1

// Test 2: Function with local variable hiding global
test2: {[x]
    globalVar: x * 2
    globalVar + 10
}
result2: test2 . 25
result2

// Test 3: Verify global variable unchanged
globalVar

// Test 4: Nested functions
outer: {[x]
    inner: {[y] globalVar + x + y}
    inner . x
}
result4: outer . 20
result4

// Test 5: Global assignment from nested function
test5: {[x]
    inner: {[y] globalVar :: x + y}
    inner . x * 2
    globalVar
}
result5: test5 . 10
result5
globalVar
