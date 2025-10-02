namespace Base.Infrastructure.Toolkits.Utilities;

public static class PollingRetryUtility
{

    public static async Task<T?> PollingRetry<T>(Func<Task<T>> operation, Func<T, bool> condition, int maxRetries = 3, int pollingInterval = 2000, CancellationToken cancellationToken = default)
    {
        Exception? lastException = null;
        int attempt = 0;
        while (attempt < maxRetries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                T result = await operation();
                if (condition(result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
                // 如果是最後一次嘗試，就拋出異常
                if (attempt == maxRetries - 1)
                    throw;
            }

            attempt++;
            if (attempt < maxRetries)
            {
                await Task.Delay(pollingInterval, cancellationToken);
            }
        }

        return default;
    }

}
