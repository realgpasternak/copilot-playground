using System;

class FibonacciCalculator
{
    static void Main(string[] args)
    {
        Console.WriteLine("Fibonacci Number Calculator");
        Console.WriteLine("============================");
        Console.Write("Enter the number of Fibonacci numbers to calculate: ");
        
        if (!int.TryParse(Console.ReadLine(), out int count) || count < 0)
        {
            Console.WriteLine("Please enter a valid non-negative integer.");
            return;
        }

        Console.WriteLine($"\nFirst {count} Fibonacci numbers:");
        CalculateAndDisplayFibonacci(count);
    }

    static void CalculateAndDisplayFibonacci(int count)
    {
        if (count == 0)
            return;

        long a = 0, b = 1;
        
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"{i + 1}: {a}");
            
            long temp = a + b;
            a = b;
            b = temp;
        }
    }
}
