using System;
using System.Threading;

public abstract record ThreadSafeConfiguration : IDisposable
{
    private ReaderWriterLockSlim @lock;
    private bool isDisposed;

    protected ThreadSafeConfiguration()
    {
        @lock = new ReaderWriterLockSlim();
    }

    public void Dispose()
    {
        if (isDisposed) return;

        isDisposed = true;

        ReleaseResources();

        @lock.Dispose();
    }

    /// <summary>
    /// </summary>
    /// <remarks>Called on dispose</remarks>
    protected abstract void ReleaseResources();

    protected TProperty GetThreadSafe<TProperty>(ref TProperty property)
    {
        @lock.EnterReadLock();

        try 
        {
            return property;
        }
        finally
        {
            @lock.ExitReadLock();
        }
    }

    protected void SetThreadSafe<TProperty>(ref TProperty property, TProperty value, Func<TProperty, TProperty, bool> eqFun = null, EventHandler<TProperty> propertyChanged = null)
    {
        @lock.EnterWriteLock();

        try
        {
            if (eqFun is not null && eqFun(property, value)) return;

            property = value;

            propertyChanged?.Invoke(this, property);
        }
        finally
        {
            @lock.ExitWriteLock();
        }
    }
}