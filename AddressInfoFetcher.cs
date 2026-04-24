using System.Net.Http.Json;

namespace LighterTest;

public class AddressInfoFetcher
{
    private readonly string _layer1Address;
    private readonly HttpClient _httpClient;

    public AddressInfoFetcher(string layer1Address, string url)
    {
        if (string.IsNullOrWhiteSpace(layer1Address))
        {
            throw new ArgumentException("Layer1 address is required.", nameof(layer1Address));
        }

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException("Url is required.", nameof(url));
        }

        _layer1Address = layer1Address;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(url, UriKind.Absolute)
        };
    }

    public async Task<long> GetAccountIndexAsync(CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/v1/account?by=l1_address&value={Uri.EscapeDataString(_layer1Address)}";
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        var accountsInfo = await response.Content.ReadFromJsonAsync<Models.LighterAccountsInfo>(cancellationToken);
        if (accountsInfo?.Accounts == null || accountsInfo.Accounts.Count == 0)
        {
            throw new Exception("No accounts were returned.");
        }

        foreach (var account in accountsInfo.Accounts)
        {
            if (string.Equals(account.L1Address, _layer1Address, StringComparison.OrdinalIgnoreCase))
            {
                return account.AccountIndex;
            }
        }

        throw new Exception("Lighter Account Index not found.");
    }

    public async Task<short> GetApiKeyIndexAsync(long accountIndex, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/v1/apikeys?account_index={accountIndex}";
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        var apiKeysInfo = await response.Content.ReadFromJsonAsync<Models.LighterApiKeysInfo>(cancellationToken);
        if (apiKeysInfo?.ApiKeys == null || apiKeysInfo.ApiKeys.Count == 0)
        {
            throw new Exception("Lighter Api Key Index not found.");
        }

        return apiKeysInfo.ApiKeys[0].ApiKeyIndex;
    }
}
