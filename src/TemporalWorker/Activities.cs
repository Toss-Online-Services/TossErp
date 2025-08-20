namespace TemporalWorker;

public static class Activities
{
    public static async Task<string> Echo(string input)
    {
        await Task.Delay(10);
        return $"echo:{input}";
    }
}
