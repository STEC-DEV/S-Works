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


        //READ WRITE 권한
        protected bool BASIC { get; private set; } = false;
        protected bool MACHINE { get; private set; } = false;
        protected bool ELEC { get; private set; } = false;
        protected bool LIFT { get; private set; } = false;
        protected bool FIRE { get; private set; } = false;
        protected bool CONSTRUCT { get; private set; } = false;
        protected bool NETWORK { get; private set; } = false;
        protected bool BEAUTY { get; private set; } = false;
        protected bool SECURITY { get; private set; } = false;
        protected bool MATERIAL { get; private set; } = false;
        protected bool ENERGY { get; private set; } = false;
        protected bool USER { get; private set; } = false;
        protected bool VOC { get; private set; } = false;


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
            await LoadAllUserPerms();
        }

        //일반 권한
        protected virtual async Task LoadPermissions()
        {
            USEREDIT = await PermissionService.HasEditPermission(RequiredPermission);
        }

        //UserPerm 전체 권한 조회
        protected virtual async Task LoadAllUserPerms()
        {
            var permMappings = new Dictionary<string,Action<bool>>() {
                {"UserPerm_Basic", value => BASIC = value} ,
                {"UserPerm_Machine", value => MACHINE = value} ,
                {"UserPerm_Elec", value => ELEC = value} ,
                {"UserPerm_Lift", value => LIFT = value} ,
                {"UserPerm_Fire", value => FIRE = value} ,
                {"UserPerm_Construct", value => CONSTRUCT= value} ,
                {"UserPerm_Network", value => NETWORK = value} ,
                {"UserPerm_Beauty", value => BEAUTY = value} ,
                {"UserPerm_Security", value => SECURITY = value} ,
                {"UserPerm_Material", value => MATERIAL = value} ,
                {"UserPerm_Energy", value => ENERGY = value} ,
                {"UserPerm_User", value => USER = value} ,
                {"UserPerm_Voc", value => VOC = value} ,
            };


            foreach(var perm in permMappings)
            {
                int permValue = await PermissionService.GetUserPerm(perm.Key);
                if(permValue > 1)
                {
                    perm.Value(true);
                }
            }
        }

        //관리자 권한
        protected virtual async Task LoadAdminModePermissions()
        {
            string _role = await PermissionService.HasAdminModeEditPerm();
          
            if ( _role == "SystemManager" || _role == "Master")
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
