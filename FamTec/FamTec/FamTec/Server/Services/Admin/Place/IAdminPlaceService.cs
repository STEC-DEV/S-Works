using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Services.Admin.Place
{
    public interface IAdminPlaceService
    {
        /// <summary>
        /// 사업장에 포함되어있지 않은 관리자 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<ManagerListDTO>?> NotContainManagerList(HttpContext? context, int? placeid);

        /// <summary>
        /// 해당 관리자가 가지고 있지 않은 사업장 List 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<AdminPlaceDTO>?> NotContainPlaceList(HttpContext? context, int? adminid);


        /// <summary>
        /// 로그인한 관리자ID 인덱스가 속해있는 사업장 리스트 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksService(int? adminid);

        /// <summary>
        /// 관리자가 일반 로그인시 선택할 수 있는 PlaceList 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ValueTask<ResponseList<AdminPlaceDTO>> GetMyWorksList(HttpContext? context);

        /// <summary>
        /// 전체 사업장 조회 - 매니저는 자기것가 할당된것만 조회됨
        /// </summary>
        /// <returns></returns>
        public ValueTask<ResponseList<AllPlaceDTO>?> GetAllWorksService(HttpContext? context);

        /// <summary>
        /// 전체 관리자리스트 반환
        /// </summary>
        /// <returns></returns>
        public ValueTask<ResponseList<ManagerListDTO>?> GetAllManagerListService();

        /// <summary>
        /// 사업장 등록
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> AddPlaceService(HttpContext? context, AddPlaceDTO? dto);

        /// <summary>
        /// 사업장 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        ValueTask<ResponseUnit<UpdatePlaceDTO>?> UpdatePlaceService(HttpContext? context, UpdatePlaceDTO? dto);

        /// <summary>
        /// 사업장 번호로 사업장 모델 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<PlaceDetailDTO>?> GetPlaceService(int? placeid);

        /// <summary>
        /// 사업장에 관리자 추가 서비스
        /// </summary>
        /// <param name="placemanager"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool>> AddPlaceManagerService(HttpContext? context, AddPlaceManagerDTO<ManagerListDTO> placemanager);

        /// <summary>
        /// 관리자생성후 사업장 추가 서비스
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool>> AddManagerPlaceSerivce(HttpContext? context, AddManagerPlaceDTO? dto);

        /// <summary>
        /// 사업장에 할당된 관리자 삭제
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<int?>> DeleteManagerPlaceService(HttpContext? context, AddPlaceManagerDTO<ManagerListDTO>? dto);


        /// <summary>
        /// 사업장 삭제 --- 여기맞나..
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<bool>> DeletePlaceService(HttpContext? context, List<int> placeidx);


    }
}
