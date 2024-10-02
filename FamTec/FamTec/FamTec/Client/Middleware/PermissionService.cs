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
            return (isAdmin || userPerm == 2);
        }

        public async Task<string> HasAdminModeEditPerm()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            string role = await authProvider.AdminRole();
            return role;
        }

        public async Task<bool> IsAdmin()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            bool isAdmin = await authProvider.IsAdminAsync();
            return isAdmin;
        }

        //사업장 아이디 조회
        public async Task<int> GetPlaceIdx()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            int placeIdx = await authProvider.GetPlaceIdx();
            return placeIdx;
        }

        public async Task<bool> IsLogin()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            bool isLogin = await authProvider.HasJwtTokenAsync();
            return isLogin;
        }

        public async Task<string> GetPlaceName()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            string name = await authProvider.GetPlaceName();
            return name;
        }
        
    }
}
