globalVar: 100  // Test nested functions
outer: {[x]
    inner: {[y] globalVar + x + y}
    inner . x}
result4: outer . 20
result4
