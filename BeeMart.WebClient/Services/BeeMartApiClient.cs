using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BeeMart.WebClient.Models;
using BeeMart.WebClient.Models.Auth;
using BeeMart.WebClient.Models.Catalog;
using BeeMart.WebClient.Models.Transactions;

namespace BeeMart.WebClient.Services;

/// <summary>Result wrapper so controllers can surface API error messages to the UI.</summary>
public class ApiResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }

    public static ApiResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ApiResult<T> Fail(string error) => new() { Success = false, Error = error };
}

/// <summary>
/// Typed client for the Backend API. The WebClient talks ONLY through this —
/// no direct DB access (spec architecture rule). JWT is attached from session.
/// </summary>
public class BeeMartApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _ctx;
    private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    public BeeMartApiClient(HttpClient http, IHttpContextAccessor ctx)
    {
        _http = http;
        _ctx = ctx;
    }

    private void AttachToken()
    {
        var token = _ctx.HttpContext?.Session.GetToken();
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>Extract the API's { "message": "..." } error body, falling back to the status.</summary>
    private static async Task<string> ReadErrorAsync(HttpResponseMessage resp)
    {
        try
        {
            var doc = await resp.Content.ReadFromJsonAsync<JsonElement>();
            if (doc.ValueKind == JsonValueKind.Object && doc.TryGetProperty("message", out var m))
                return m.GetString() ?? $"Error {(int)resp.StatusCode}";
        }
        catch { /* non-JSON body */ }
        return $"Error {(int)resp.StatusCode}";
    }

    // ---- Auth ----
    public async Task<ApiResult<AuthResponse>> LoginAsync(LoginRequest req)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/auth/login", req, JsonOpts);
        if (!resp.IsSuccessStatusCode) return ApiResult<AuthResponse>.Fail(await ReadErrorAsync(resp));
        return ApiResult<AuthResponse>.Ok((await resp.Content.ReadFromJsonAsync<AuthResponse>(JsonOpts))!);
    }

    public async Task<ApiResult<AuthResponse>> RegisterAsync(RegisterRequest req)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/auth/register", req, JsonOpts);
        if (!resp.IsSuccessStatusCode) return ApiResult<AuthResponse>.Fail(await ReadErrorAsync(resp));
        return ApiResult<AuthResponse>.Ok((await resp.Content.ReadFromJsonAsync<AuthResponse>(JsonOpts))!);
    }

    // ---- Catalog ----
    public async Task<ApiResult<List<ProductDto>>> SearchProductsAsync(string keyword)
    {
        AttachToken();
        var resp = await _http.GetAsync($"api/v1/products/search?keyword={Uri.EscapeDataString(keyword)}");
        if (!resp.IsSuccessStatusCode) return ApiResult<List<ProductDto>>.Fail(await ReadErrorAsync(resp));
        return ApiResult<List<ProductDto>>.Ok((await resp.Content.ReadFromJsonAsync<List<ProductDto>>(JsonOpts)) ?? new());
    }

    public async Task<ApiResult<List<PaymentMethodDto>>> GetPaymentMethodsAsync()
    {
        AttachToken();
        var resp = await _http.GetAsync("api/v1/payment-methods");
        if (!resp.IsSuccessStatusCode) return ApiResult<List<PaymentMethodDto>>.Fail(await ReadErrorAsync(resp));
        return ApiResult<List<PaymentMethodDto>>.Ok((await resp.Content.ReadFromJsonAsync<List<PaymentMethodDto>>(JsonOpts)) ?? new());
    }

    // ---- Transactions ----
    public async Task<ApiResult<TransactionResponse>> CreateTransactionAsync(CreateTransactionRequest req)
    {
        AttachToken();
        var resp = await _http.PostAsJsonAsync("api/v1/transactions", req, JsonOpts);
        if (!resp.IsSuccessStatusCode) return ApiResult<TransactionResponse>.Fail(await ReadErrorAsync(resp));
        return ApiResult<TransactionResponse>.Ok((await resp.Content.ReadFromJsonAsync<TransactionResponse>(JsonOpts))!);
    }

    public async Task<ApiResult<PagedResult<TransactionHistoryItem>>> GetHistoryAsync(DateTime date, int page)
    {
        AttachToken();
        var resp = await _http.GetAsync($"api/v1/transactions/history?date={date:yyyy-MM-dd}&page={page}");
        if (!resp.IsSuccessStatusCode) return ApiResult<PagedResult<TransactionHistoryItem>>.Fail(await ReadErrorAsync(resp));
        return ApiResult<PagedResult<TransactionHistoryItem>>.Ok(
            (await resp.Content.ReadFromJsonAsync<PagedResult<TransactionHistoryItem>>(JsonOpts))!);
    }
}
