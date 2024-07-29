using System.Text.Json;

/*
 * Using array manipulations
 */

Console.WriteLine("Array manipulations:");

string[] array = ["A", "B", "C"];
Console.WriteLine(JsonSerializer.Serialize(array));

Console.WriteLine("Get last element from an array and remove it");

var last = array.Last();
array = array[..^1];

Console.WriteLine($"Last Element: {last}");
Console.WriteLine($"Remains: {JsonSerializer.Serialize(array)}");

Console.WriteLine("Get first element from an array and remove it");

var first = array.First();
array = array[1..];

Console.WriteLine($"First Element: {first}");
Console.WriteLine($"Remains: {JsonSerializer.Serialize(array)}");