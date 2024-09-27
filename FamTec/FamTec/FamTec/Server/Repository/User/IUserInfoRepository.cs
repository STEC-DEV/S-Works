using FamTec.Shared.Model;

namespace FamTec.Server.Repository.User
{
    public interface IUserInfoRepository
    {

        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool?> DelUserCheck(int id);

        /// <summary>
        /// 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UsersTb?> AddAsync(UsersTb model);

        /// <summary>
        /// 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> AddUserAsync(UsersTb model);

        /// <summary>
        /// 엑셀 IMPORT 추가
        /// </summary>
        /// <param name="UserList"></param>
        /// <returns></returns>
        Task<bool?> AddUserList(List<UsersTb> UserList);

        /// <summary>
        /// 유저 INDEX로 유저테이블 검색
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        Task<UsersTb?> GetUserIndexInfo(int useridx);

        /// <summary>
        /// 유저ID로 유저테이블 검색 (삭제유무와 관계없는 모든 사용자)
        /// </summary>
        /// <param name="useridx"></param>
        /// <returns></returns>
        Task<UsersTb?> GetNotFilterUserInfo(int useridx);

        /// <summary>
        /// 삭제 쿼리
        /// </summary>
        /// <param name="Useridx"></param>
        /// <returns></returns>
        Task<int?> DeleteUserList(List<int> Useridx, string Name);

        /// <summary>
        /// USERID + PASSWORD에 해당하는 모델반환
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UsersTb?> GetUserInfo(string userid, string password);

        /// <summary>
        /// USERID 검사
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<UsersTb?> UserIdCheck(string userid);

        /// <summary>
        /// 유저 테이블 내용 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteUserInfo(List<int> delIdx, string deleter);

        /// <summary>
        /// 유저 테이블 내용 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UsersTb?> UpdateUserInfo(UsersTb model);

        /// <summary>
        /// 해당 사업장의 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetPlaceUserList(int placeidx);

        /// <summary>
        /// 해당 사업장의 기계 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocMachineList(int placeidx);

        /// <summary>
        /// 해당 사업장의 전기 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocElecList(int placeidx);

        /// <summary>
        /// 해당 사업장의 승강 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocLiftList(int placeidx);

        /// <summary>
        /// 해당 사업장의 소방 voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocFireList(int placeidx);

        /// <summary>
        /// 해당 사업장의 건축 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocConstructList(int placeidx);

        /// <summary>
        /// 해당 사업장의 통신 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocNetWorkList(int placeidx);

        /// <summary>
        /// 해당 사업장의 미화 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocBeautyList(int placeidx);

        /// <summary>
        /// 해당 사업장의 보안 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocSecurityList(int placeidx);

        /// <summary>
        /// 해당 사업장의 기타 Voc 권한 가진 사용자 리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        Task<List<UsersTb>?> GetVocDefaultList(int placeidx);

        /// <summary>
        /// 테이블 전체 사용자 반환
        /// </summary>
        /// <returns></returns>
        Task<List<UsersTb>?> GetAllUserList();

    }
}
