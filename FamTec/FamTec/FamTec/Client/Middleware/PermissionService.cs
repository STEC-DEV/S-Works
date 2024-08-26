using FamTec.Client.Shared.Provider;
using Microsoft.AspNetCore.Components.Authorization;

namespace FamTec.Client.Middleware
{
    public class PermissionService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        public PermissionService(AuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        public async Task<bool> HasEditPermission(string permName)
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            if (authProvider == null) return false;

            bool isAdmin = await authProvider.IsAdminAsync();
            int userPerm = await authProvider.GetUserPermission(permName);
            Console.WriteLine("미들웨어 권한 확인" + (isAdmin || userPerm == 2));
            return (isAdmin || userPerm == 2);
        }
    }
}
