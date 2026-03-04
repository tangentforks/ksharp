// Non-square matrix: 2x3 matrix (more columns than rows)
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
// y = [1 2 3; 4 5 6], so y^T is 3x2
// x = [7; 8; 9] (length 3)
// Result w should be length 2
(7 8 9) _lsq (1 2 3;4 5 6)
