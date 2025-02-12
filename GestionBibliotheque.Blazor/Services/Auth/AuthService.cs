using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestionBibliotheque.Blazor.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Net.Http.Json;

namespace GestionBibliotheque.Blazor.Services.Auth
{
    public class AuthService: IAuthService
    {
         private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> Login(LoginDTO loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("login", loginDto);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            await _localStorage.SetItemAsync("authToken", result.Token);
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Token);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
            return true;
        }
        return false;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
    }
}