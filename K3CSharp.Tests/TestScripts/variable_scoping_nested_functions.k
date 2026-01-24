// Test nested functions
globalVar: 100
outer: {[x]
    inner: {[y] globalVar + x + y}
    inner . x}
result4: outer . 20
result4
