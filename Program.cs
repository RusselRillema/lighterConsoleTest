using LighterTest;

string defaultUrl = "https://mainnet.zklighter.elliot.ai/";
int defaultChainId = 304;

string layer1Address;
while (true)
{
    Console.WriteLine("Enter layer1Address:");
    var layer1AddressInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(layer1AddressInput))
    {
        layer1Address = layer1AddressInput;
        break;
    }

    Console.WriteLine("layer1Address is required.");
}

Console.WriteLine($"Enter url (default: {defaultUrl}):");
var urlInput = Console.ReadLine();
var url = string.IsNullOrWhiteSpace(urlInput) ? defaultUrl : urlInput;

int chainId;
while (true)
{
    Console.WriteLine($"Enter chainId (default: {defaultChainId}):");
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

    Console.WriteLine("Invalid chainId. Please enter a valid integer.");
}

string keySecret;
while (true)
{
    Console.WriteLine("Enter keySecret:");
    var keySecretInput = Console.ReadLine();

    if (!string.IsNullOrWhiteSpace(keySecretInput))
    {
        keySecret = keySecretInput;
        break;
    }

    Console.WriteLine("keySecret is required.");
}

var addressInfoFetcher = new AddressInfoFetcher(layer1Address, url);
long accountIndex = await addressInfoFetcher.GetAccountIndexAsync();
short apiKeyIndex = await addressInfoFetcher.GetApiKeyIndexAsync(accountIndex);

Console.WriteLine($"AccountIndex: {accountIndex}");
Console.WriteLine($"ApiKeyIndex: {apiKeyIndex}");

var client = LighterSignerLinux.CreateClient(url, keySecret, chainId, apiKeyIndex, accountIndex);

while (true)
{
    await Task.Delay(5000);
    Console.WriteLine("Still running...");
}
