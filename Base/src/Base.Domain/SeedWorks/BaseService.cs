namespace Base.Domain.SeedWorks;

public abstract class BaseService : IDisposable
{
    private bool disposed;

    public virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // 釋放受控資源
            }

            // 釋放非受控資源（如果有）

            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~BaseService() // the finalizer
    {
        Dispose(false);
    }

}
