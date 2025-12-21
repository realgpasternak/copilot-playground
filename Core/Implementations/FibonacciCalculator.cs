namespace FibonacciCalculator.Core.Implementations;

/// <summary>
/// Performance metrics tracker for Fibonacci calculations.
/// </summary>
public class PerformanceMetrics : IPerformanceTracker
{
    private long _executionTimeMs;
    private int _cacheHits;
    private int _cacheMisses;

    public long ExecutionTimeMs => _executionTimeMs;
    public int CacheHits => _cacheHits;
    public int CacheMisses => _cacheMisses;

    internal void RecordCacheHit() => Interlocked.Increment(ref _cacheHits);
    internal void RecordCacheMiss() => Interlocked.Increment(ref _cacheMisses);
    internal void RecordExecutionTime(long ms) => _executionTimeMs = ms;

    public void Reset()
    {
        _executionTimeMs = 0;
        _cacheHits = 0;
        _cacheMisses = 0;
    }
}

/// <summary>
/// Implements Fibonacci calculation with memoization and performance tracking.
/// </summary>
public class MemoizedFibonacciCalculator : IFibonacciCalculator
{
    private readonly Dictionary<int, long> _cache = new();
    private readonly PerformanceMetrics _metrics;

    public IPerformanceTracker Metrics => _metrics;

    public MemoizedFibonacciCalculator()
    {
        _metrics = new PerformanceMetrics();
        InitializeBaseValues();
    }

    private void InitializeBaseValues()
    {
        _cache[0] = 0;
        _cache[1] = 1;
    }

    public long GetFibonacci(int n)
    {
        if (n < 0)
            throw new ArgumentException("Fibonacci index cannot be negative", nameof(n));

        if (_cache.TryGetValue(n, out var cachedValue))
        {
            _metrics.RecordCacheHit();
            return cachedValue;
        }

        _metrics.RecordCacheMiss();
        var result = ComputeFibonacci(n);
        _cache[n] = result;
        return result;
    }

    private long ComputeFibonacci(int n)
    {
        return n switch
        {
            0 => 0,
            1 => 1,
            _ => GetFibonacci(n - 1) + GetFibonacci(n - 2)
        };
    }

    public IEnumerable<long> CalculateUpTo(long limit)
    {
        if (limit < 0)
            throw new ArgumentException("Limit cannot be negative", nameof(limit));

        var startTime = DateTime.UtcNow;

        int index = 0;
        long current = 0;
        long next = 1;

        if (current <= limit)
            yield return current;

        while (next <= limit)
        {
            yield return next;
            var temp = next;
            next += current;
            current = temp;
            index++;
        }

        _metrics.RecordExecutionTime((long)(DateTime.UtcNow - startTime).TotalMilliseconds);
    }

    public void ClearCache()
    {
        _cache.Clear();
        InitializeBaseValues();
        _metrics.Reset();
    }
}

/// <summary>
/// Implements iterative Fibonacci calculation with sequence analysis.
/// </summary>
public class IterativeFibonacciCalculator : IFibonacciCalculator
{
    private readonly PerformanceMetrics _metrics = new();

    public IPerformanceTracker Metrics => _metrics;

    public long GetFibonacci(int n)
    {
        if (n < 0)
            throw new ArgumentException("Fibonacci index cannot be negative", nameof(n));

        return n switch
        {
            0 => 0,
            1 => 1,
            _ => CalculateIterative(n)
        };
    }

    private long CalculateIterative(int n)
    {
        long prev = 0, current = 1;

        for (int i = 2; i <= n; i++)
        {
            var next = prev + current;
            prev = current;
            current = next;
        }

        return current;
    }

    public IEnumerable<long> CalculateUpTo(long limit)
    {
        if (limit < 0)
            throw new ArgumentException("Limit cannot be negative", nameof(limit));

        var startTime = DateTime.UtcNow;

        long current = 0, next = 1;

        while (current <= limit)
        {
            yield return current;
            var temp = next;
            next += current;
            current = temp;
        }

        _metrics.RecordExecutionTime((long)(DateTime.UtcNow - startTime).TotalMilliseconds);
    }

    public void ClearCache()
    {
        _metrics.Reset();
    }
}
