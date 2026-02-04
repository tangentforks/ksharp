globalVar: 100  // Test verify global variable unchanged
test2: {[x]
    globalVar: x * 2
    globalVar + 10
}
result2: test2 . 25
result2
globalVar
