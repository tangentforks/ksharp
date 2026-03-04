// Complex non-square case with mixed values
// Solving: y^T * w = x where y is 2x3, x is length 3, w is length 2
// y = [1.5 2.0 3.0; 4.5 5.5 6.0], so y^T is 3x2
// x = [7.5; 8.0; 9.5] (length 3)
// Result w should be length 2
(7.5 8.0 9.5) _lsq (1.5 2.0 3.0;4.5 5.5 6.0)
