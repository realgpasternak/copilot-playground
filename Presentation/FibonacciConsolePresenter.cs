namespace FibonacciCalculator.Presentation;

/// <summary>
/// Handles console-based user interaction for Fibonacci calculations.
/// </summary>
public class FibonacciConsolePresenter
{
    private readonly IFibonacciCalculator _calculator;
    private readonly IPerformanceTracker _metrics;

    public FibonacciConsolePresenter(IFibonacciCalculator calculator, IPerformanceTracker metrics)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    public void DisplayWelcome()
    {
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║   Fibonacci Sequence Calculator v2.0   ║");
        Console.WriteLine("╚════════════════════════════════════════╝\n");
    }

    public long PromptForInput()
    {
        while (true)
        {
            try
            {
                Console.Write("Enter the maximum number for Fibonacci sequence: ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("❌ Input cannot be empty. Please try again.\n");
                    continue;
                }

                if (long.TryParse(input, out var result))
                {
                    if (result < 0)
                    {
                        Console.WriteLine("❌ Number must be non-negative. Please try again.\n");
                        continue;
                    }

                    return result;
                }

                Console.WriteLine("❌ Invalid input. Please enter a valid number.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An error occurred: {ex.Message}\n");
            }
        }
    }

    public void DisplaySequence(long limit)
    {
        try
        {
            Console.WriteLine("\n📊 Fibonacci Sequence:");
            Console.WriteLine(new string('─', 50));

            var sequence = _calculator.CalculateUpTo(limit).ToList();

            if (!sequence.Any())
            {
                Console.WriteLine("(No values)");
                return;
            }

            for (int i = 0; i < sequence.Count; i++)
            {
                Console.Write($"{sequence[i]:N0}");
                if (i < sequence.Count - 1)
                    Console.Write(", ");

                if ((i + 1) % 5 == 0)
                    Console.WriteLine();
            }

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error displaying sequence: {ex.Message}");
        }
    }

    public void DisplayStatistics(long limit, int sequenceLength)
    {
        Console.WriteLine("📈 Statistics:");
        Console.WriteLine(new string('─', 50));
        Console.WriteLine($"Limit: {limit:N0}");
        Console.WriteLine($"Sequence Length: {sequenceLength} numbers");
        Console.WriteLine($"Execution Time: {_metrics.ExecutionTimeMs}ms");

        if (_metrics is PerformanceMetrics advancedMetrics)
        {
            Console.WriteLine($"Cache Hits: {advancedMetrics.CacheHits}");
            Console.WriteLine($"Cache Misses: {advancedMetrics.CacheMisses}");
        }

        Console.WriteLine();
    }

    public bool PromptContinue()
    {
        Console.Write("Calculate another sequence? (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        return response == "y" || response == "yes";
    }
}
