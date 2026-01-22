// Test 4: type operator with all K3 types

// Test integers
4: 42
4: -5
4: 0

// Test longs  
4: 42L
4: -999999999L

// Test floats
4: 3.14
4: -2.71
4: 0.0

// Test characters
4: "a"
4: "Z"
4: "1"

// Test symbols
4: `symbol
4: `"test symbol"

// Test null
4: _n

// Test vectors
4: 1 2 3 4
4: "a" "b" "c"
4: `x`y`z

// Test mixed vector
4: 1 2.5 "c" `sym

// Test functions
func: {[x] x + 1}
4: func

// Test special values
4: 0I
4: 0N
4: -0I
