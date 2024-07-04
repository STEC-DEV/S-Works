using FamTec.Shared;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin.Place;

namespace FamTec.Server.Repository.Admin.AdminUser
{
    public interface IAdminUserInfoRepository
    {
        /// <summary>
        /// 관리자 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> AddAdminUserInfo(AdminTb? model);

        /// <summary>
        /// 매개변수의 USERID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> GetAdminUserInfo(int? usertbid);

        /// <summary>
        /// 매개변수의 ADMINID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> GetAdminIdInfo(int? adminid);

        /// <summary>
        /// 관리자리스트 
        /// </summary>
        /// <returns></returns>
        ValueTask<List<ManagerListDTO>?> GetAllAdminUserList();


        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteAdminInfo(AdminTb? model);

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> UpdateAdminInfo(AdminTb? model);
        

    }
}
