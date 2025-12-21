namespace FibonacciCalculator.Core.Abstractions;

/// <summary>
/// Defines the contract for calculating Fibonacci sequences.
/// </summary>
public interface IFibonacciCalculator
{
    /// <summary>
    /// Calculates Fibonacci numbers up to the specified limit.
    /// </summary>
    /// <param name="limit">The maximum number to calculate Fibonacci values for</param>
    /// <returns>An enumerable of Fibonacci numbers</returns>
    IEnumerable<long> CalculateUpTo(long limit);

    /// <summary>
    /// Calculates the nth Fibonacci number.
    /// </summary>
    /// <param name="n">The position in the sequence (0-based)</param>
    /// <returns>The nth Fibonacci number</returns>
    long GetFibonacci(int n);

    /// <summary>
    /// Clears any cached values.
    /// </summary>
    void ClearCache();
}

/// <summary>
/// Defines performance metrics for calculations.
/// </summary>
public interface IPerformanceTracker
{
    long ExecutionTimeMs { get; }
    int CacheHits { get; }
    int CacheMisses { get; }
    void Reset();
}
