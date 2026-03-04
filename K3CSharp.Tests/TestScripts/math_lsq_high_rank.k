// Rectangular matrix: 2x4 matrix (more columns than rows)
// Solving: y^T * w = x where y is 2x4, x is length 4, w is length 2
// y = [1 2 3 4; 2 3 4 5], so y^T is 4x2
// x = [10; 11; 12; 13] (length 4)
// Result w should be length 2
(10 11 12 13) _lsq (1 2 3 4;2 3 4 5)
