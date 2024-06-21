using FamTec.Server.Databases;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server
{
    public class WorksSetting : DbContext
    {
        private readonly WorksContext context;



        enum LevelCode : ushort
        {
            시스템관리자 = 100,
            마스터 = 200,
            매니저 = 300,
        }

        public WorksSetting()
        {
            this.context = new WorksContext();
        }

        public async ValueTask DefaultSetting()
        {
            // 파일서버 경로
            DirectoryInfo di = new DirectoryInfo(CommPath.FileServer);
            if(!di.Exists) di.Create();
            
            // VOC 파일 이미지서버 경로
            di = new DirectoryInfo(CommPath.VocFileImages);
            if (!di.Exists)di.Create();
            


            DepartmentTb? department = new DepartmentTb();
            department.Name = "에스텍시스템";
            department.CreateDt = DateTime.Now;
            department.CreateUser = LevelCode.시스템관리자.ToString();
            department.UpdateDt = DateTime.Now;
            department.UpdateUser = LevelCode.시스템관리자.ToString();
            department.DelYn = false;

            DepartmentTb? selectDepartment = await context.DepartmentTbs
                .FirstOrDefaultAsync(m =>
                m.Name!.Equals("에스텍시스템") && 
                m.DelYn != true);

            if(selectDepartment is null)
            {
                context.DepartmentTbs.Add(department);
                await context.SaveChangesAsync();
                selectDepartment = await context.DepartmentTbs.FirstOrDefaultAsync(m => m.Name!.Equals("에스텍시스템") && m.DelYn != true);
            }
            else
            {
                if (department.Name != selectDepartment.Name)
                {
                    selectDepartment.Name = department.Name;
                    selectDepartment.UpdateDt = DateTime.Now;
                    selectDepartment.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if (department.DelYn != selectDepartment.DelYn)
                {
                    selectDepartment.DelYn = department.DelYn;
                    selectDepartment.UpdateDt = DateTime.Now;
                    selectDepartment.UpdateUser = LevelCode.시스템관리자.ToString();
                }

                context.DepartmentTbs.Update(selectDepartment);
                await context.SaveChangesAsync();

                selectDepartment = await context.DepartmentTbs.FirstOrDefaultAsync(m => m.Name!.Equals("에스텍시스템") && m.DelYn != true);
            }

            UserTb? user = new UserTb();
            user.UserId = "Admin";
            user.Password = "stecdev1234!";
            user.Name = "시스템개발파트";
            user.Email = "stecdev@s-tec.co.kr";
            user.Phone = "010-0000-0000";
            user.PermBasic = 2; // 기본정보등록 권한 (수정권한)
            user.PermMachine = 2; // 설비권한 (수정권한)
            user.PermLift = 2; // 승강권한 (수정권한)
            user.PermFire = 2; // 소방권한
            user.PermConstruct = 2; // 건축권한
            user.PermNetwork = 2; // 통신권한
            user.PermBeauty = 2; // 미화권한
            user.PermSecurity = 2; // 보안권한
            user.PermMaterial = 2; // 자재권한
            user.PermEnergy = 2; // 에너지권한
            user.PermUser = 2; // 사용자 설정 권한
            user.PermVoc = 2; // VOC권한

            user.AdminYn = 1;
            user.AlramYn = 1;
            user.Status = 1; // 0 : 퇴직 / 1 : 재직

            user.CreateDt = DateTime.Now;
            user.CreateUser = LevelCode.시스템관리자.ToString();
            user.UpdateDt = DateTime.Now;
            user.UpdateUser = LevelCode.시스템관리자.ToString();
            user.DelYn = false;
            user.Job = LevelCode.시스템관리자.ToString();

            

            UserTb? selectUser = await context.UserTbs.FirstOrDefaultAsync(m => m.UserId!.Equals(user.UserId) && m.Password!.Equals(user.Password));
            if(selectUser is null)
            {
                
                context.UserTbs.Add(user);
                await context.SaveChangesAsync();
                
                selectUser = await context.UserTbs.FirstOrDefaultAsync(m => m.UserId!.Equals(user.UserId) && m.Password!.Equals(user.Password));
            }
            else
            {
                if (user.UserId != selectUser.UserId) // 사용자ID
                {
                    selectUser.UserId = user.UserId;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if (user.Name != selectUser.Name) // 이름
                {
                    selectUser.Name = user.Name;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.Email != selectUser.Email) // 이메일
                {
                    selectUser.Email = user.Email;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.Phone != selectUser.Phone) // 전화번호
                {
                    selectUser.Phone = user.Phone;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermBasic != selectUser.PermBasic) // 기본정보등록 권한
                {
                    selectUser.PermBasic = user.PermBasic;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermMachine != selectUser.PermMachine) // 설비 권한
                {
                    selectUser.PermMachine = user.PermMachine;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermLift != selectUser.PermLift) // 승강 권한
                {
                    selectUser.PermLift = user.PermLift;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermFire != selectUser.PermFire) // 소방권한
                {
                    selectUser.PermFire = user.PermFire;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermConstruct != selectUser.PermConstruct) // 건축권한
                {
                    selectUser.PermConstruct = user.PermConstruct;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermNetwork != selectUser.PermNetwork) // 통신권한
                {
                    selectUser.PermNetwork = user.PermNetwork;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermBeauty != selectUser.PermBeauty) // 미화권한
                {
                    selectUser.PermBeauty = user.PermBeauty;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermSecurity != selectUser.PermSecurity) // 보안권한
                {
                    selectUser.PermSecurity = user.PermSecurity;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermMaterial != selectUser.PermMaterial) // 자재권한
                {
                    selectUser.PermMaterial = user.PermMaterial;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermEnergy != selectUser.PermEnergy) // 에너지권한
                {
                    selectUser.PermEnergy = user.PermEnergy;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermUser != selectUser.PermUser) // 사용자 설정 권한
                {
                    selectUser.PermUser = user.PermUser;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.PermVoc != selectUser.PermVoc) // VOC 권한
                {
                    selectUser.PermVoc = user.PermVoc;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }

                if(user.AdminYn != selectUser.AdminYn)
                {
                    selectUser.AdminYn = user.AdminYn;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.AlramYn != selectUser.AlramYn)
                {
                    selectUser.AlramYn = user.AlramYn;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.DelYn != selectUser.DelYn)
                {
                    selectUser.DelYn = user.DelYn;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(user.Job != selectUser.Job)
                {
                    selectUser.Job = user.Job;
                    selectUser.UpdateDt = DateTime.Now;
                    selectUser.UpdateUser = LevelCode.시스템관리자.ToString();
                }

                context.UserTbs.Update(selectUser);
                await context.SaveChangesAsync();

                selectUser = await context.UserTbs.FirstOrDefaultAsync(m => m.UserId!.Equals(user.UserId) && m.Password!.Equals(user.Password));
            }

            AdminTb? admin = new AdminTb();
            admin.Type = LevelCode.시스템관리자.ToString();
            admin.CreateDt = DateTime.Now;
            admin.CreateUser = LevelCode.시스템관리자.ToString();
            admin.UpdateDt = DateTime.Now;
            admin.UpdateUser = LevelCode.시스템관리자.ToString();
            admin.UserTbId = selectUser!.Id;

            admin.DepartmentTbId = selectDepartment!.Id;

            AdminTb? selectAdmin = await context.AdminTbs.FirstOrDefaultAsync(m => m.UserTbId.Equals(selectUser.Id));
            
            if(selectAdmin is null)
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
                    selectAdmin.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(selectAdmin.UserTbId != selectUser.Id)
                {
                    selectAdmin.UserTbId = selectUser.Id;
                    selectAdmin.UpdateDt = DateTime.Now;
                    selectAdmin.UpdateUser = LevelCode.시스템관리자.ToString();
                }
                if(selectAdmin.DepartmentTbId != selectDepartment.Id)
                {
                    selectAdmin.DepartmentTbId = selectDepartment.Id;
                    selectAdmin.UpdateDt = DateTime.Now;
                    selectAdmin.UpdateUser = LevelCode.시스템관리자.ToString();
                }

                context.UserTbs.Update(selectUser);
                await context.SaveChangesAsync();

            }




        }
    }
}

/*
 INSERT INTO unit_tb(ID, UNIT, CREATE_DT, CREATE_USER, UPDATE_DT, UPDATE_USER, DEL_YN, DEL_DT, DEL_USER, PLACE_TB_ID)
VALUES(null,'㎀',Now(),'시스템관리자',NOW(),'시스템관리자',NULL,NULL,NULL,NULL)
(null,'㎁', NOW(),'시스템관리자', NOW(),'시스템관리자', NULL, NULL, NULL, NULL),
(null, '㎂', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(null, '㎃', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(null, '㎄', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'KB', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'MB', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'GB', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎈', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎉', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎊', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎋', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎌', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎍', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎎', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎏', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎐', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎑', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎒', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎒', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎓', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎔', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎙', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎚', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎛', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎜', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎝', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎞', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎟', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎠', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎡', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎢', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎣', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎤', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎤', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎥', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎦', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎨', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎩', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎪', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎫', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎬', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏂', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏘', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎭', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎮', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎯', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎰', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎱', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎲', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎳', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎕', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎖', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎗', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'ℓ', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎘', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎴', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎵', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎶', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎷', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎸', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎹', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎺', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎻', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎼', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎽', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎾', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㎿', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'Ω', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏀', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏁', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏃', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏄', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏅', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏆', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏇', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏈', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏉', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏊', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏋', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏌', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏍', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏎', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏏', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏐', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏑', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏒', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏓', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏔', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏕', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏖', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏗', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏙', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏚', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏛', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏜', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏝', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏞', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㏟', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㍱', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㍲', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㍳', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㍴', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㍵', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '㍶', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, '℉', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'K', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL),
(NULL, 'µ', NOW(), '시스템관리자', NOW(), '시스템관리자', NULL, NULL, NULL, NULL);
 */