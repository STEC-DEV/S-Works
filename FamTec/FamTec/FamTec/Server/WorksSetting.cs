using FamTec.Server.Databases;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server
{
    public class WorksSetting : DbContext
    {
        private readonly WorksContext context;
        private List<string> defaultUnit;

        public WorksSetting()
        {
            context = new WorksContext();
            defaultUnit = new List<string>()
            {
                "㎀","㎁","㎂","㎃","KB", "MB", "GB", "㎈", "㎉", "㎊", "㎋", "㎌", "㎍", "㎎",
                "㎏", "㎐", "㎑", "㎒", "㎒", "㎓", "㎔", "㎙", "㎚", "㎛", "㎜", "㎝", "㎞", "㎟", "㎠", "㎡", "㎢", "㎣", "㎤",
                "㎥", "㎦", "㎨", "㎩", "㎪", "㎫", "㎬", "㏂", "㏘", "㎭", "㎮", "㎯", "㎰", "㎱", "㎲", "㎳", "㎕", "㎖", "㎗",
                "ℓ", "㎘", "㎴", "㎵", "㎶", "㎷", "㎸", "㎹", "㎺", "㎻", "㎼", "㎽", "㎾", "㎿", "Ω", "㏀", "㏁", "㏃", "㏄",
                "㏅", "㏆", "㏇", "㏈", "㏉", "㏊", "㏋", "㏌", "㏍", "㏎", "㏏", "㏐", "㏑", "㏒", "㏓", "㏔", "㏕", "㏖", "㏗",
                "㏙", "㏚", "㏛", "㏜", "㏝", "㏞", "㏟", "㍱", "㍲", "㍳", "㍴", "㍵", "㍶", "℉", "K" ,"µ"
            };
        }

        public async Task DefaultSetting()
        {
            List<string>? unittb = await context.UnitTbs
                .Where(m => m.DelYn != true &&
                       m.PlaceTbId == null)
                .Select(m => m.Unit)
                .ToListAsync();

            List<string>? compare = defaultUnit.Except(unittb).ToList();

            for (int i = 0; i < compare.Count; i++)
            {
                UnitTb? model = new UnitTb();
                model.Unit = compare[i];
                model.CreateDt = DateTime.Now;
                model.CreateUser = "시스템관리자";
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = "시스템관리자";
                model.DelYn = false;

                context.UnitTbs.Add(model);
            }
            await context.SaveChangesAsync();


            // 파일서버 경로
            DirectoryInfo di = new DirectoryInfo(Common.FileServer);
            if (!di.Exists) di.Create();

            DepartmentsTb? department = new DepartmentsTb();
            department.Name = "에스텍시스템";
            department.CreateDt = DateTime.Now;
            department.CreateUser = "시스템관리자";
            department.UpdateDt = DateTime.Now;
            department.UpdateUser = "시스템관리자";
            department.DelYn = false;
            department.ManagementYn = true;


            DepartmentsTb? selectDepartment = await context.DepartmentsTbs
                .FirstOrDefaultAsync(m =>
                m.Name!.Equals("에스텍시스템") &&
                m.DelYn != true);

            if (selectDepartment is null)
            {
                context.DepartmentsTbs.Add(department);
                await context.SaveChangesAsync();
                selectDepartment = await context.DepartmentsTbs.FirstOrDefaultAsync(m => m.Name!.Equals("에스텍시스템") && m.DelYn != true);
            }
            else
            {
                if (department.Name != selectDepartment.Name)
                {
                    selectDepartment.Name = department.Name;
                    selectDepartment.UpdateDt = DateTime.Now;
                    selectDepartment.UpdateUser = "시스템관리자";
                }
                if (department.DelYn != selectDepartment.DelYn)
                {
                    selectDepartment.DelYn = department.DelYn;
                    selectDepartment.UpdateDt = DateTime.Now;
                    selectDepartment.UpdateUser = "시스템관리자";
                }

                context.DepartmentsTbs.Update(selectDepartment);
                await context.SaveChangesAsync();

                selectDepartment = await context.DepartmentsTbs.FirstOrDefaultAsync(m => m.Name!.Equals("에스텍시스템") && m.DelYn != true);
            }

            UsersTb? user = new UsersTb()
            {
                UserId = "Admin",
                Password = "stecdev1234!",
                Name = "시스템관리자",
                Email = "stecdev@s-tec.co.kr",
                Phone = "1577-0722",
                PermBasic = 2, // 기본정보관리메뉴 권한
                PermMachine = 2, // 기계메뉴 권한
                PermElec = 2, // 전기메뉴 권한
                PermLift = 2, // 승강메뉴 권한
                PermFire = 2, // 소방메뉴 권한
                PermConstruct = 2, // 건축메뉴 권한
                PermNetwork = 2, // 통신메뉴 권한
                PermBeauty = 2, // 미화메뉴 권한
                PermSecurity = 2, // 보안메뉴 권한
                PermMaterial = 2, // 자재관리메뉴 권한
                PermEnergy = 2, // 에너지관리메뉴 권한
                PermUser = 2, // 사용자관리메뉴 권한
                PermVoc = 2, // 민원관리메뉴 권한
                VocMachine = true, // 기계민원 처리권한
                VocElec = true, // 전기민원 처리권한
                VocLift = true, // 승강민원 처리권한
                VocFire = true, // 소방민원 처리권한
                VocConstruct = true, // 건축민원 처리권한
                VocNetwork = true, // 통신민원 처리권한
                VocBeauty = true, // 미화민원 처리권한
                VocSecurity = true, // 보안민원 처리권한
                VocEtc = true, // 기타민원 처리권한
                AdminYn = true, // 관리자유무
                AlarmYn = true, // 알람유무
                Status = 2, // 재직유무
                CreateDt = DateTime.Now,
                CreateUser = "시스템관리자",
                UpdateDt = DateTime.Now,
                UpdateUser = "시스템관리자",
                DelYn = false,
                Job = "시스템관리자"
            };

            UsersTb? selectUser = await context.UsersTbs.FirstOrDefaultAsync(m => m.UserId!.Equals(user.UserId) && m.Password!.Equals(user.Password));
            if (selectUser is null)
            {
                context.UsersTbs.Add(user);
                await context.SaveChangesAsync();
                selectUser = await context.UsersTbs.FirstOrDefaultAsync(m => m.UserId!.Equals(user.UserId) && m.Password!.Equals(user.Password));
            }
            else
            {
                if (user.UserId != selectUser.UserId) // 사용자ID
                {
                    selectUser.UserId = user.UserId;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.Name != selectUser.Name) // 이름
                {
                    selectUser.Name = user.Name;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.Email != selectUser.Email) // 이메일
                {
                    selectUser.Email = user.Email;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.Phone != selectUser.Phone) // 전화번호
                {
                    selectUser.Phone = user.Phone;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermBasic != selectUser.PermBasic) // 기본정보등록 권한
                {
                    selectUser.PermBasic = user.PermBasic;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermMachine != selectUser.PermMachine) // 설비 권한
                {
                    selectUser.PermMachine = user.PermMachine;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermLift != selectUser.PermLift) // 승강 권한
                {
                    selectUser.PermLift = user.PermLift;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermFire != selectUser.PermFire) // 소방권한
                {
                    selectUser.PermFire = user.PermFire;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermConstruct != selectUser.PermConstruct) // 건축권한
                {
                    selectUser.PermConstruct = user.PermConstruct;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermNetwork != selectUser.PermNetwork) // 통신권한
                {
                    selectUser.PermNetwork = user.PermNetwork;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermBeauty != selectUser.PermBeauty) // 미화권한
                {
                    selectUser.PermBeauty = user.PermBeauty;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermSecurity != selectUser.PermSecurity) // 보안권한
                {
                    selectUser.PermSecurity = user.PermSecurity;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermMaterial != selectUser.PermMaterial) // 자재권한
                {
                    selectUser.PermMaterial = user.PermMaterial;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermEnergy != selectUser.PermEnergy) // 에너지권한
                {
                    selectUser.PermEnergy = user.PermEnergy;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermUser != selectUser.PermUser) // 사용자 설정 권한
                {
                    selectUser.PermUser = user.PermUser;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.PermVoc != selectUser.PermVoc) // VOC 권한
                {
                    selectUser.PermVoc = user.PermVoc;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }

                if (user.AdminYn != selectUser.AdminYn)
                {
                    selectUser.AdminYn = user.AdminYn;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.AlarmYn != selectUser.AlarmYn)
                {
                    selectUser.AlarmYn = user.AlarmYn;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.DelYn != selectUser.DelYn)
                {
                    selectUser.DelYn = user.DelYn;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }
                if (user.Job != selectUser.Job)
                {
                    selectUser.Job = user.Job;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = "시스템관리자";
                }

                context.UsersTbs.Update(selectUser);
                await context.SaveChangesAsync();

                selectUser = await context.UsersTbs.FirstOrDefaultAsync(m => m.UserId!.Equals(user.UserId) && m.Password!.Equals(user.Password));
            }

            AdminTb? admin = new AdminTb();
            admin.Type = "시스템관리자";
            admin.CreateDt = DateTime.Now;
            admin.CreateUser = "시스템관리자";
            admin.UpdateDt = DateTime.Now;
            admin.UpdateUser = "시스템관리자";
            admin.UserTbId = selectUser!.Id;

            admin.DepartmentTbId = selectDepartment!.Id;

            AdminTb? selectAdmin = await context.AdminTbs.FirstOrDefaultAsync(m => m.UserTbId.Equals(selectUser.Id));

            if (selectAdmin is null)
            {
                context.AdminTbs.Add(admin);
                await context.SaveChangesAsync();
            }
            else
            {
                if (selectAdmin.Type != admin.Type)
                {
                    selectAdmin.Type = admin.Type;
                    selectAdmin.UpdateDt = DateTime.Now;
                    selectAdmin.UpdateUser = "시스템관리자";
                }
                if (selectAdmin.UserTbId != selectUser.Id)
                {
                    selectAdmin.UserTbId = selectUser.Id;
                    selectAdmin.UpdateDt = DateTime.Now;
                    selectAdmin.UpdateUser = "시스템관리자";
                }
                if (selectAdmin.DepartmentTbId != selectDepartment.Id)
                {
                    selectAdmin.DepartmentTbId = selectDepartment.Id;
                    selectAdmin.UpdateDt = DateTime.Now;
                    selectAdmin.UpdateUser = "시스템관리자";
                }

                context.UsersTbs.Update(selectUser);
                await context.SaveChangesAsync();
            }
        }
    }
}