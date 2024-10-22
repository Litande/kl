using System.Collections.Concurrent;

namespace KL.Statistics.Application.Common.Extensions;

public static class QueueExtensions
{
    public static IEnumerable<T> DequeueChunk<T>(this ConcurrentQueue<T> queue, int chunkSize)
    {
        var result = new List<T>();
        for (var i = 0; i < chunkSize && queue.Count > 0; i++)
        {
            if (queue.TryDequeue(out var message))
            {
                result.Add(message);
            }
        }

        return result;
    }
}