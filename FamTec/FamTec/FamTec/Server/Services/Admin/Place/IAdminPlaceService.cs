using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;

namespace FamTec.Server.Services.Admin.Place
{
    public interface IAdminPlaceService
    {
        /// <summary>
        /// 사업장에 포함되어있지 않은 관리자 리스트 조회
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="placeid">사업장 ID</param>
        /// <returns></returns>
        public ValueTask<ResponseList<ManagerListDTO>> NotContainManagerList(HttpContext context, int placeid);

        /// <summary>
        /// 해당 관리자가 가지고 있지 않은 사업장 List 조회
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="adminid">관리자 ID</param>
        /// <returns></returns>
        public ValueTask<ResponseList<AdminPlaceDTO>> NotContainPlaceList(HttpContext context, int adminid);


        /// <summary>
        /// 로그인한 관리자ID 인덱스가 속해있는 사업장 리스트 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksService(int adminid);

        /// <summary>
        /// 관리자가 일반 로그인시 선택할 수 있는 PlaceList 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksList(HttpContext context);

        /// <summary>
        /// 전체 사업장 조회
        ///      - 시스템관리자, 관리자 => 전체 사업장 조회
        ///      - 매니저 => 할당된 사업장 조회
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <returns>ResponseList<AllPlaceDTO></returns>
        public ValueTask<ResponseList<AllPlaceDTO>> GetAllWorksService(HttpContext context);

        /// <summary>
        /// 관리자정보 전체 조회
        /// </summary>
        /// <returns></returns>
        public ValueTask<ResponseList<ManagerListDTO>> GetAllManagerListService();

        /// <summary>
        /// 사업장 등록
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="dto">사업장 추가 DTO</param>
        /// <returns>추가된 사업장ID</returns>
        public ValueTask<ResponseUnit<int?>> AddPlaceService(HttpContext context, AddPlaceDTO dto);

        /// <summary>
        /// 사업장 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        ValueTask<ResponseUnit<UpdatePlaceDTO>> UpdatePlaceService(HttpContext context, UpdatePlaceDTO dto);

        /// <summary>
        /// 사업장 번호로 사업장 모델 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<PlaceDetailDTO>> GetPlaceService(int placeid);

        /// <summary>
        /// 사업장에 관리자 추가
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="placemanager">추가할 관리자 정보 DTO</param>
        public ValueTask<ResponseUnit<bool?>> AddPlaceManagerService(HttpContext context, AddPlaceManagerDTO<ManagerListDTO> placemanager);

        /// <summary>
        /// 관리자생성후 사업장 추가 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> AddManagerPlaceSerivce(HttpContext context, AddManagerPlaceDTO dto);

        /// <summary>
        /// 사업장에 할당된 관리자 삭제
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool?>> DeleteManagerPlaceService(HttpContext context, AddPlaceManagerDTO<ManagerListDTO> dto);


        /// <summary>
        /// 사업장 자체를 삭제
        ///     - 사업장에 할당된 매니저가 있으면 삭제안됨
        ///     - 사업장에 할당된 건물이 있으면 삭제안됨
        /// </summary>
        /// <param name="context">JWT 토큰</param>
        /// <param name="placeidx">삭제할 사업장 인덱스</param>
        /// <returns>TRUE & FALSE</returns>
        public ValueTask<ResponseUnit<bool?>> DeletePlaceService(HttpContext context, List<int> placeidx);

        /// <summary>
        /// 사업장 이름 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<string?>> GetPlaceName(int placeid);
    }
}
