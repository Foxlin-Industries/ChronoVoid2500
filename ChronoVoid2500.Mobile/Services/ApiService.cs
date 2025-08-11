using System.Net.Http.Json;
using System.Text.Json;
using ChronoVoid2500.Mobile.Models;

namespace ChronoVoid2500.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUrl = "https://localhost:7001/api"; // Update this to your API URL
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/auth/register", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<RealmDto>?> GetRealmsAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"Requesting realms from: {_baseUrl}/realm");
            var response = await _httpClient.GetAsync($"{_baseUrl}/realm");
            
            System.Diagnostics.Debug.WriteLine($"Response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response content: {content}");
                
                var realms = await response.Content.ReadFromJsonAsync<List<RealmDto>>();
                System.Diagnostics.Debug.WriteLine($"Parsed {realms?.Count ?? 0} realms");
                
                return realms;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"API error response: {errorContent}");
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get realms error: {ex}");
            return null;
        }
    }

    public async Task<JoinRealmResponse?> JoinRealmAsync(JoinRealmRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/auth/join-realm", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<JoinRealmResponse>();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Join realm error: {ex.Message}");
            return null;
        }
    }

    public async Task<NodeDetailDto?> GetNodeDetailsAsync(int nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/navigation/node/{nodeId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<NodeDetailDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Get node details error: {ex.Message}");
            return null;
        }
    }

    public async Task<NavigationResultDto?> MoveToNodeAsync(MoveRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/navigation/move", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<NavigationResultDto>();
            }
            return null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Move to node error: {ex.Message}");
            return null;
        }
    }
}