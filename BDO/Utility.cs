using System.Collections.Concurrent;

namespace BDO_Project.BDO
{
    /// <summary>
    /// Utility class for this project.
    /// </summary>
    public class Utility
    {

        public static readonly ParallelOptions MaxParallelism = new() { MaxDegreeOfParallelism = Environment.ProcessorCount / 4 + 1 };

        /// <summary>
        /// To speedup processing of items.
        /// </summary>
        public static Exception[]? ProcessParallel<T>(T[] items, Action<T> action)
        {
            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            ConcurrentQueue<Exception>? exceptions = null;
            try
            {
                var res = Parallel.For(0, items.Length, MaxParallelism, index =>
                {
                     try
                    {
                        var item = items[index];
                        if (item != null)
                            action(item);
                    }
                    catch (Exception e)
                    {
                       if (exceptions == null)
                            exceptions = new ConcurrentQueue<Exception>();
                        exceptions.Enqueue(e);
                    }
                });
            }
            catch (Exception ex)
            {
                if (exceptions == null)
                    exceptions = new ConcurrentQueue<Exception>();
                exceptions.Enqueue(ex);
            }

            return exceptions != null && !exceptions.IsEmpty ? exceptions.ToArray() : null;
        }
    }
}
