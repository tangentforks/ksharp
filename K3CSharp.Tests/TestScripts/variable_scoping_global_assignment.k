globalVar: 100  // Test global assignment from nested function
test5: {[x]
    inner: {[y] globalVar :: x + y}
    inner . x * 2
    globalVar}
result5: test5 . 10
result5
globalVar
