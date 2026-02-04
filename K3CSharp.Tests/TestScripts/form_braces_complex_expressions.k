sum:{[a;b] a+b}
product:{[x;y] x*y}
{}$("sum[3;4]*2";"product[sum[2;3];4]";"sum[product[2;3];4]")  // Complex expressions mixing arithmetic and function calls
