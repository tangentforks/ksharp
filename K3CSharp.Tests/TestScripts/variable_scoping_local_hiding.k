// Test function with local variable hiding global
globalVar: 100
test2: {[x]
    globalVar: x * 2
    globalVar + 10}
result2: test2 . 25
result2
