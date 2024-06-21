using FamTec.Shared.Model;

namespace FamTec.Server.Repository.User
{
    public interface IUserInfoRepository
    {
        /// <summary>
        /// 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<UserTb?> AddAsync(UserTb? model);

        /// <summary>
        /// 유저 INDEX로 유저테이블 검색
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        ValueTask<UserTb?> GetUserIndexInfo(int? useridx);

        /// <summary>
        /// USERID + PASSWORD에 해당하는 모델반환
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ValueTask<UserTb?> GetUserInfo(string? userid, string? password);

        /// <summary>
        /// USERID 검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        ValueTask<UserTb?> UserIdCheck(string? userid);

        /// <summary>
        /// 유저 테이블 내용 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteUserInfo(UserTb? model);

        /// <summary>
        /// 해당 사업장의 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetPlaceUserList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 기계 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocMachineList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 전기 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocElecList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 승강 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocLiftList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 소방 voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocFireList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 건축 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocConstructList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 통신 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocNetWorkList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 미화 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocBeautyList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 보안 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocSecurityList(int? placeidx);

        /// <summary>
        /// 해당 사업장의 기타 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UserTb>?> GetVocDefaultList(int? placeidx);

    }
}
