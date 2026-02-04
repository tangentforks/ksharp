globalVar: 100  // Test function accessing global variable
test1: {[x] globalVar + x}
result1: test1 . 50
result1
