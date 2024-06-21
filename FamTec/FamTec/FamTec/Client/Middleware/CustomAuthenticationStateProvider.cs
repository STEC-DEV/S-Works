﻿using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FamTec.Client.Middleware
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        public CustomAuthenticationStateProvider(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _sessionStorage.GetItemAsStringAsync("SWORKSSESSION");
            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity(token) 
                : new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),"jwt");

            var user = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(user));
        }

        public async Task<string> GetTokenAsync()
        {
            return await _sessionStorage.GetItemAsStringAsync("SWORKSSESSION");
        }

        public async Task UpdateTokenAsync(string newToken)
        {
            await _sessionStorage.SetItemAsStringAsync("SWORKSSESSION", newToken);
            NotifyUserAuthentication(newToken);
        }

        public void NotifyUserAuthentication(string token)
        {
            var identity = new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt");
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void NotifyUserLogout()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
