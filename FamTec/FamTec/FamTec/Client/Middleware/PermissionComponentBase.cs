using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FamTec.Client.Middleware
{
    public class PermissionComponentBase : ComponentBase
    {
        [Inject] protected PermissionService PermissionService { get; set; }

        protected bool USEREDIT { get; private set; } = false;

        // 각 페이지에서 오버라이드할 속성
        protected virtual string RequiredPermission => "UserPerm_Basic";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadPermissions();
        }

        protected virtual async Task LoadPermissions()
        {
            USEREDIT = await PermissionService.HasEditPermission(RequiredPermission);
        }

    }
}
