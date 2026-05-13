using System.Diagnostics;
using System.Text.Json;

namespace LighterTest;

public class GoProcessClient
{
    private readonly Process _process;
    private readonly StreamWriter _stdin;
    private readonly StreamReader _stdout;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public GoProcessClient(string executablePath)
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = executablePath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        _process.Start();
        _stdin = _process.StandardInput;
        _stdout = _process.StandardOutput;
    }

    public async Task<T?> CallAsync<T>(string method, object? param = null)
    {
        await _lock.WaitAsync(); // one request at a time
        try
        {
            var request = JsonSerializer.Serialize(new { method, args = param });
            await _stdin.WriteLineAsync(request);
            await _stdin.FlushAsync();

            var responseLine = await _stdout.ReadLineAsync()
                               ?? throw new Exception("Go process closed unexpectedly");

            using var doc = JsonDocument.Parse(responseLine);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var err) && err.GetString() is { Length: > 0 } errMsg)
                throw new Exception($"Go error: {errMsg}");

            return root.GetProperty("result").Deserialize<T>();
        }
        finally
        {
            _lock.Release();
        }
    }

    public void Dispose()
    {
        _stdin.Close();
        _process.WaitForExit(3000);
        _process.Dispose();
    }
}