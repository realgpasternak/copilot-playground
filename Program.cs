using FibonacciCalculator.Core.Implementations;
using FibonacciCalculator.Presentation;

class Program
{
    static void Main()
    {
        var calculator = new MemoizedFibonacciCalculator();
        var metrics = (calculator as MemoizedFibonacciCalculator)?.Metrics 
            ?? throw new InvalidOperationException("Unable to access metrics");

        var presenter = new FibonacciConsolePresenter(calculator, metrics);

        presenter.DisplayWelcome();

        bool continueCalculating = true;

        while (continueCalculating)
        {
            try
            {
                long limit = presenter.PromptForInput();
                
                var sequence = calculator.CalculateUpTo(limit).ToList();
                
                presenter.DisplaySequence(limit);
                presenter.DisplayStatistics(limit, sequence.Count);

                continueCalculating = presenter.PromptContinue();
                Console.WriteLine();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Invalid input: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ An unexpected error occurred: {ex.Message}\n");
            }
        }

        Console.WriteLine("\n👋 Thank you for using Fibonacci Calculator!");
    }
}
