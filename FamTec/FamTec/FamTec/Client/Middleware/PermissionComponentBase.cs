using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace FamTec.Client.Middleware
{
    public class PermissionComponentBase : ComponentBase
    {
        [Inject] protected PermissionService PermissionService { get; set; }

        protected bool USEREDIT { get; private set; } = false;
        protected bool ADMINEDIT { get; private set; } = false;

        protected string? ADMINJOB { get; private set; }

        protected int? PLACEIDX { get; private set; }
        protected bool ISADMIN { get; private set; }
        protected bool ISLOGIN { get; private set; }

        protected string PLACENAME { get; private set; }
        // 각 페이지에서 오버라이드할 속성
        protected virtual string RequiredPermission => "UserPerm_Basic";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadPermissions();
            await LoadAdminModePermissions();
            await LoadAdminJob();
            await LoadPlaceIdx();
            await LoadIsAdmin();
            await LoadIsLogin();
            //await LoadPlaceName();
        }

        //일반 권한
        protected virtual async Task LoadPermissions()
        {
            USEREDIT = await PermissionService.HasEditPermission(RequiredPermission);
        }

        //관리자 권한
        protected virtual async Task LoadAdminModePermissions()
        {
            string _role = await PermissionService.HasAdminModeEditPerm();
            if (_role == "Master" || _role == "SystemManager")
            {
                ADMINEDIT = true;
                return;
            }
            ADMINEDIT = false;
        }

        //사업장 아이디
        protected virtual  async Task LoadPlaceIdx()
        {
            int _placeIdx = await PermissionService.GetPlaceIdx();
            PLACEIDX = _placeIdx;
        }

        //관리자 여부
        protected virtual async Task LoadIsAdmin()
        {
            ISADMIN = await PermissionService.IsAdmin();
        }

        protected virtual async Task LoadAdminJob()
        {
            ADMINJOB= await PermissionService.HasAdminModeEditPerm();
        }

        protected virtual async Task LoadIsLogin()
        {
            ISLOGIN = await PermissionService.IsLogin();
        }

        protected virtual async Task LoadPlaceName()
        {
            PLACENAME= await PermissionService.GetPlaceName();
        }
    }
}
