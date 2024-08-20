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
        ValueTask<UsersTb?> AddAsync(UsersTb model);

        /// <summary>
        /// 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> AddUserAsync(UsersTb model);

        /// <summary>
        /// 엑셀 IMPORT 추가
        /// </summary>
        /// <param name="UserList"></param>
        /// <returns></returns>
        ValueTask<bool?> AddUserList(List<UsersTb> UserList);

        /// <summary>
        /// 유저 INDEX로 유저테이블 검색
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        ValueTask<UsersTb?> GetUserIndexInfo(int useridx);

        /// <summary>
        /// 유저ID로 유저테이블 검색 (삭제유무와 관계없는 모든 사용자)
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        ValueTask<UsersTb?> GetNotFilterUserInfo(int useridx);

        /// <summary>
        /// 삭제 쿼리
        /// </summary>
        /// <param name="Useridx"></param>
        /// <returns></returns>
        ValueTask<int?> DeleteUserList(List<int> Useridx, string Name);

        /// <summary>
        /// USERID + PASSWORD에 해당하는 모델반환
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        ValueTask<UsersTb?> GetUserInfo(string userid, string password);

        /// <summary>
        /// USERID 검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        ValueTask<UsersTb?> UserIdCheck(string userid);

        /// <summary>
        /// 유저 테이블 내용 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<bool?> DeleteUserInfo(List<int> delIdx, string deleter);

        /// <summary>
        /// 유저 테이블 내용 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ValueTask<UsersTb?> UpdateUserInfo(UsersTb model);

        /// <summary>
        /// 해당 사업장의 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetPlaceUserList(int placeidx);

        /// <summary>
        /// 해당 사업장의 기계 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocMachineList(int placeidx);

        /// <summary>
        /// 해당 사업장의 전기 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocElecList(int placeidx);

        /// <summary>
        /// 해당 사업장의 승강 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocLiftList(int placeidx);

        /// <summary>
        /// 해당 사업장의 소방 voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocFireList(int placeidx);

        /// <summary>
        /// 해당 사업장의 건축 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocConstructList(int placeidx);

        /// <summary>
        /// 해당 사업장의 통신 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocNetWorkList(int placeidx);

        /// <summary>
        /// 해당 사업장의 미화 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocBeautyList(int placeidx);

        /// <summary>
        /// 해당 사업장의 보안 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocSecurityList(int placeidx);

        /// <summary>
        /// 해당 사업장의 기타 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetVocDefaultList(int placeidx);

        /// <summary>
        /// 테이블 전체 사용자 반환
        /// </summary>
        /// <returns></returns>
        ValueTask<List<UsersTb>?> GetAllUserList();

    }
}
