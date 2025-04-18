﻿using FamTec.Shared.Model;

namespace FamTec.Server.Repository.Facility.ItemValue
{
    public interface IFacilityItemValueInfoRepository
    {
        /// <summary>
        /// 아이템 KEY에 대한 VALUE 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<FacilityItemValueTb?> AddAsync(FacilityItemValueTb model);

        /// <summary>
        /// KEY에 해당하는 VALUE LIST 조회
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        Task<List<FacilityItemValueTb>?> GetAllValueList(int keyid);

        /// <summary>
        /// VALUE ID에 해당하는 Value모델 조회
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        Task<FacilityItemValueTb?> GetValueInfo(int valueid);

        /// <summary>
        /// 아이템 Value 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> UpdateValueInfo(FacilityItemValueTb model);

        /// <summary>
        /// 아이템 Vaue 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool?> DeleteValueInfo(FacilityItemValueTb model);

    }
}
