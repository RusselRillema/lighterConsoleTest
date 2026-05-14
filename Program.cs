using System.Runtime.InteropServices;
using LighterTest;
using System.Text.Json;


//NativeLibrary.Load(LighterNativeLinux.DllName);

try
{
    string defaultUrl = "https://mainnet.zklighter.elliot.ai/";
    int defaultChainId = 304;
    string localConfigPath = "local.test.config.json";
    bool returnTokenOnly = false;

    if (args.Length > 0 && bool.TryParse(args[0], out var parsedReturnTokenOnly))
    {
        returnTokenOnly = parsedReturnTokenOnly;
    }

    string layer1Address;
    string url;
    int chainId;
    string keySecret;
    string keyPublic;
    bool linuxTest = System.OperatingSystem.IsLinux();

    if (linuxTest)
    {
        Console.WriteLine("Running singer test for Linux.");
        Console.WriteLine($"Expected Dll name: {LighterNativeLinux.DllName}.");
    }
    else
    {
        Console.WriteLine("Running singer test for Windows.");
        Console.WriteLine($"Expected Dll name: {LighterNativeWindows.DllName}.");
    }
    Console.WriteLine($"Is 64 bit proccess: {Environment.Is64BitProcess}.\n\n");

    if (File.Exists(localConfigPath))
    {
        var configJson = await File.ReadAllTextAsync(localConfigPath);
        var config = JsonSerializer.Deserialize<LocalTestConfig>(configJson);

        if (config is null ||
            string.IsNullOrWhiteSpace(config.l1address) ||
            string.IsNullOrWhiteSpace(config.apiSecret) ||
            string.IsNullOrWhiteSpace(config.apiPublic))
        {
            throw new InvalidOperationException(
                $"Invalid config in '{localConfigPath}'. 'l1address' and 'apiSecret' are required.");
        }

        layer1Address = config.l1address;
        keySecret = config.apiSecret;
        keyPublic = config.apiPublic;
        url = string.IsNullOrWhiteSpace(config.url) ? defaultUrl : config.url;
        chainId = config.chainId ?? defaultChainId;

        WriteDebugLine($"Loaded config from {localConfigPath}");
    }
    else
    {
        if (returnTokenOnly)
        {
            throw new InvalidOperationException(
                $"Config file '{localConfigPath}' is required when running in token-only mode.");
        }

        while (true)
        {
            WriteDebugLine("Enter layer1Address:");
            var layer1AddressInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(layer1AddressInput))
            {
                layer1Address = layer1AddressInput;
                break;
            }

            WriteDebugLine("layer1Address is required.");
        }

        WriteDebugLine($"Enter url (default: {defaultUrl}):");
        var urlInput = Console.ReadLine();
        url = string.IsNullOrWhiteSpace(urlInput) ? defaultUrl : urlInput;

        while (true)
        {
            WriteDebugLine($"Enter chainId (default: {defaultChainId}):");
            var chainIdInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(chainIdInput))
            {
                chainId = defaultChainId;
                break;
            }

            if (int.TryParse(chainIdInput, out chainId))
            {
                break;
            }

            WriteDebugLine("Invalid chainId. Please enter a valid integer.");
        }

        while (true)
        {
            WriteDebugLine("Enter keySecret:");
            var keySecretInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(keySecretInput))
            {
                keySecret = keySecretInput;
                break;
            }

            WriteDebugLine("keySecret is required.");
        }

        while (true)
        {
            WriteDebugLine("Enter keyPublic:");
            var keyPublicInput = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(keyPublicInput))
            {
                keyPublic = keyPublicInput;
                break;
            }

            WriteDebugLine("keyPublic is required.");
        }
    }

    var addressInfoFetcher = new AddressInfoFetcher(layer1Address, url);
    long accountIndex = await addressInfoFetcher.GetAccountIndexAsync();
    short apiKeyIndex = await addressInfoFetcher.GetApiKeyIndexAsync(accountIndex, keyPublic);

    WriteDebugLine($"AccountIndex: {accountIndex}");
    WriteDebugLine($"ApiKeyIndex: {apiKeyIndex}");


    string token = "";
    var expiryTime = DateTime.UtcNow.AddSeconds(28200); //7 hours and 50 minutes
    long deadline = Convert.ToInt64((expiryTime - new DateTime(1970, 01, 01)).TotalSeconds);
    try
    {
        if (linuxTest)
        {
            var client = LighterSignerLinux.CreateClient(url, keySecret, chainId, (byte)apiKeyIndex, accountIndex);
            LighterSignerLinux.InitSignerThread();
            token = LighterSignerLinux.CreateAuthToken(deadline, apiKeyIndex, accountIndex);
        }
        else
        {
            var client = LighterSignerWindows.CreateClient(url, keySecret, chainId, (byte)apiKeyIndex, accountIndex);
            token = LighterSignerWindows.CreateAuthToken(deadline, apiKeyIndex, accountIndex);
        }
        Console.WriteLine("Generated Auth Token:");
        Console.WriteLine(token);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Caught exception while attempting to generate auth token: {e}");
        throw;
    }

    await Task.Delay(1000);

    if (returnTokenOnly)
    {
        return;
    }

    try
    {
        Console.WriteLine($"Testing endpoint - DepositHistory:");
        var depositHistory = await addressInfoFetcher.GetDepositHistoryAsync(accountIndex, token);
        Console.WriteLine($"DepositHistory: {depositHistory}");
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }

    int count = 0;

    _ = Task.Run(async () =>
    {
        try
        {
            while (true)
            {
                await Task.Delay(1000);
                Console.WriteLine($"Still running {count++}...");
                if (count % 10 == 0)
                {
                    Console.WriteLine("Recreating token...");
                    if (linuxTest)
                    {
                        LighterSignerLinux.InitSignerThread();
                        token = LighterSignerLinux.CreateAuthToken(deadline, apiKeyIndex, accountIndex);
                    }
                    else
                    {
                        token = LighterSignerWindows.CreateAuthToken(deadline, apiKeyIndex, accountIndex);
                    }
                    Console.WriteLine(token);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Caught exception while attempting to generate auth token on background thread: {ex}");
        }
    });

    for (int i = 0; i < 10; i++)
    {
        _ = Task.Run(() =>
        {
            try
            {
                int number = 2;
                List<int> primes = new List<int>();
                List<int> notPrimes = new List<int>();
                while (true)
                {
                    if (IsPrime(number))
                        primes.Add(number);
                    else
                        notPrimes.Add(number);
                    number++;
                }
            }
            catch (Exception e)
            {
                //Ensure an exception in one of these calculations doesn't crash the app
                Console.WriteLine($"Caught exception in calculation: {e}");
            }
        });
    }

    while (true)
    {
        Console.WriteLine($"\nTest app will continue testing the signer background.\nYou may press any key to close the app early.\n");
        var input = Console.ReadLine();

        if (input != null)
            break;
    }

    #region Helper methods
    void WriteDebugLine(string message)
    {
        if (!returnTokenOnly)
        {
            Console.WriteLine(message);
        }
    }

    static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;

        int boundary = (int)Math.Sqrt(n);

        for (int i = 3; i <= boundary; i += 2)
        {
            if (n % i == 0) return false;
        }

        return true;
    } 
    #endregion

}
catch (Exception ex)
{
    Console.WriteLine($"Caught exception in console test: {ex}");
}

sealed class LocalTestConfig
{
    public string? l1address { get; set; }
    public string? apiSecret { get; set; }
    public string? apiPublic { get; set; }
    public string? url { get; set; }
    public int? chainId { get; set; }
}
