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

        public async Task<string> HasAdminModeEditPerm()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            string role = await authProvider.AdminRole();
            await Console.Out.WriteLineAsync(role);
            return role;
        }

        //사업장 아이디 조회
        public async Task<int> GetPlaceIdx()
        {
            var authProvider = _authStateProvider as CustomAuthProvider;
            int placeIdx = await authProvider.GetPlaceIdx();
            await Console.Out.WriteLineAsync("사업장인덱스"+placeIdx);
            return placeIdx;
        }
    }
}
