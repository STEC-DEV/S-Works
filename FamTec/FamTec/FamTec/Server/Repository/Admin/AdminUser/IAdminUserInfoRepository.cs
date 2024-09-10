using FamTec.Shared;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Admin;
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
        ValueTask<AdminTb?> AddAdminUserInfo(AdminTb model);

        /// <summary>
        /// 해당 사업장 관리자로 선택가능한 사업장 리스트 출력
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        ValueTask<List<ManagerListDTO>?> GetNotContainsAdminList(int placeid);

        /// <summary>
        /// 매개변수의 USERID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> GetAdminUserInfo(int usertbid);

        /// <summary>
        /// 매개변수의 ADMINID에 해당하는 관리자모델 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> GetAdminIdInfo(int adminid);

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
        ValueTask<bool?> DeleteAdminInfo(AdminTb model);

        /// <summary>
        /// 관리자 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteAdminsInfo(List<int> idx, string deleter);

        /// <summary>
        /// 관리자 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<AdminTb?> UpdateAdminInfo(AdminTb model);

        ValueTask<bool?> UpdateAdminInfo(UpdateManagerDTO dto, string UserIdx, string creater);
        ValueTask<bool?> UpdateAdminImageInfo(int id, IFormFile? files);
    }
}
