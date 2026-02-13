// Debug test for character vector length
using K3CSharp.Serialization;
using System;

var serializer = new KSerializer();
var result = serializer.SerializeCharacterVector("");
Console.WriteLine($"Length: {result.Length}");
Console.WriteLine($"Bytes: {BitConverter.ToString(result).Replace("-", "")}");
