sum:{[a;b] a+b}
product:{[x;y] x*y}

// Complex expressions mixing arithmetic and function calls
{}$("sum[3;4]*2";"product[sum[2;3];4]";"sum[product[2;3];4]")
