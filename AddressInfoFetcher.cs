using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LighterTest;

public class AddressInfoFetcher
{
    private readonly string _layer1Address;
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters = { new FlexibleStringConverter() }
    };

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

        var accountsInfo = await response.Content.ReadFromJsonAsync<Models.LighterAccountsInfo>(_jsonSerializerOptions, cancellationToken);
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

    public async Task<short> GetApiKeyIndexAsync(long accountIndex,string apiPublicKey, CancellationToken cancellationToken = default)
    {
        var endpoint = $"/api/v1/apikeys?account_index={accountIndex}";
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        var apiKeysInfo = await response.Content.ReadFromJsonAsync<Models.LighterApiKeysInfo>(_jsonSerializerOptions, cancellationToken);
        if (apiKeysInfo?.ApiKeys == null || apiKeysInfo.ApiKeys.Count == 0)
            throw new Exception("Lighter Api Key Index not found.");
        
        foreach (var apiKey in apiKeysInfo.ApiKeys)
        {
            if(apiKey.PublicKey == apiPublicKey)
                return apiKey.ApiKeyIndex;
        }

        throw new Exception("Lighter Api Key Index not found.");
    }

    public async Task<string> GetDepositHistoryAsync(long accountIndex, string authToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(authToken))
            {
                throw new ArgumentException("Auth token is required.", nameof(authToken));
            }

            var endpoint = $"/api/v1/deposit/history?account_index={accountIndex}&l1_address={Uri.EscapeDataString(_layer1Address)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.TryAddWithoutValidation("authorization", authToken);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private sealed class FlexibleStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString(),
                JsonTokenType.Number => JsonDocument.ParseValue(ref reader).RootElement.GetRawText(),
                JsonTokenType.True => "true",
                JsonTokenType.False => "false",
                JsonTokenType.Null => null,
                _ => throw new JsonException($"Unexpected token parsing string. Token: {reader.TokenType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
