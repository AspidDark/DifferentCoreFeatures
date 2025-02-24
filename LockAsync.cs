using System;

public class LockAsync
{
    public static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    public async Task DoWorkAsync()
    {
        await semaphoreSlim.WaitAsync(TimeSpan.FromSeconds(2));

        try
        {
            await Task.Delay(1000);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            semaphoreSlim?.Release();
        }
    }
}
