﻿using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Facility;

namespace FamTec.Server.Repository.Facility
{
    public interface IFacilityInfoRepository
    {
        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="Facilityid"></param>
        /// <returns></returns>
        Task<bool?> DelFacilityCheck(int Facilityid);

        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<FacilityTb?> AddAsync(FacilityTb model);

        /// <summary>
        /// 설비 리스트 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> AddFacilityList(List<FacilityTb> model);

        /// <summary>
        /// 사업장에 속한 기계설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceMachineFacilityList(int placeid);
        
        /// <summary>
        /// 사업장에 속한 전기설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceElectronicFacilityList(int placeid);

        /// <summary>
        /// 사업장에 속한 승강설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceLiftFacilityList(int placeid);
        
        /// <summary>
        /// 사업장에 속한 소방설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceFireFacilityList(int placeid);
        
        /// <summary>
        /// 사업장에 속한 건축설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceConstructFacilityList(int placeid);

        /// <summary>
        /// 사업장에 속한 통신설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceNetworkFacilityList(int placeid);

        /// <summary>
        /// 사업장에 속한 미화설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceBeautyFacilityList(int placeid);

        /// <summary>
        /// 사업장에 속한 보안설비 List 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        Task<List<FacilityListDTO>?> GetPlaceSecurityFacilityList(int placeid);


        /// <summary>
        /// 공간ID에 포함되어있는 전체 설비LIST 조회
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        Task<List<FacilityTb>?> GetAllFacilityList(int roomid);

        /// <summary>
        /// 설비ID로 단일 설비모델 조회
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<FacilityTb?> GetFacilityInfo(int id);

        /// <summary>
        /// 설비 정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateFacilityInfo(FacilityTb model);

        /// <summary>
        /// 설비 정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteFacilityInfo(List<int> facilityid, string deleter);

    }
}
