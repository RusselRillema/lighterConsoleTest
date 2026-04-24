using LighterTest;
using System.Text.Json;

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

void WriteDebugLine(string message)
{
    if (!returnTokenOnly)
    {
        Console.WriteLine(message);
    }
}

WriteDebugLine(LighterNativeLinux.DllName);
WriteDebugLine(Environment.Is64BitProcess.ToString());

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
long deadline = 28200; //7 hours and 50 minutes
try
{
    await Task.Delay(1000);
    var client = LighterSignerLinux.CreateClient(url, keySecret, chainId, apiKeyIndex, accountIndex);

    await Task.Delay(1000);

    token = LighterSignerLinux.CreateAuthToken(deadline, apiKeyIndex, accountIndex);

    await Task.Delay(1000);
    
    Console.WriteLine(token);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

await Task.Delay(1000);

if (returnTokenOnly)
{
    return;
}

try
{
    var depositHistory = await addressInfoFetcher.GetDepositHistoryAsync(accountIndex, token);
    WriteDebugLine($"DepositHistory: {depositHistory}");
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

int count = 0;

_ = Task.Run(async () =>
{
    while (true)
    {
        await Task.Delay(5000);
        Console.WriteLine($"Still running {count++}...");

        if (count % 10 == 0)
        {
            Console.WriteLine("Recreating token...");
            token = LighterSignerLinux.CreateAuthToken(deadline, apiKeyIndex, accountIndex);
            Console.WriteLine(token);
        }
    }
});

for (int i = 0; i < 20; i++)
{
    _ = Task.Run(() =>
    {
        while (true)
        {
        }
    });
}

while (true) ;

sealed class LocalTestConfig
{
    public string? l1address { get; set; }
    public string? apiSecret { get; set; }
    public string? apiPublic { get; set; }
    public string? url { get; set; }
    public int? chainId { get; set; }
}
