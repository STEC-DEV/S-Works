﻿using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Services.Voc
{
    public interface IVocService
    {
        /// <summary>
        /// 사업장별 VOC 월간 전체보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <param name="division"></param>
        /// <param name="searchDate"></param>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetMonthVocSearchList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, string searchDate);

        /// <summary>
        /// 사업장별 VOC 기간 전체보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="buildingid"></param>
        /// <param name="division"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetDateVocSearchList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, DateTime StartDate, DateTime EndDate);

        /// <summary>
        /// 사업장별 VOC 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        //public Task<ResponseList<AllVocListDTO>> GetVocList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division, string searchdate);
        public Task<ResponseList<AllVocListDTO>> GetVocList(HttpContext context, List<int> type, List<int> status, List<int> buildingid, List<int> division);

        /// <summary>
        /// 조건별 민원 리스트 조회
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Task<ResponseList<VocListDTO>> GetVocFilterList(HttpContext context, DateTime startdate, DateTime enddate, List<int> type, List<int> status, List<int> buildingid, List<int> division);

        /// <summary>
        /// VOC 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocEmployeeDetailDTO>> GetVocDetail(HttpContext context, int vocid, bool isMobile);

        /// <summary>
        /// VOC 유형 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<ResponseUnit<bool?>> UpdateVocTypeService(HttpContext context, UpdateVocDTO dto);

        /// <summary>
        /// DashBoard용 금일 처리유형별 발생건수
        /// </summary>
        /// <param name="conteext"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocDaysStatusCountDTO>?> GetVocDaysStatusDataService(HttpContext context);

        /// <summary>
        /// DashBoard용 일주일치 처리유형별 발생건수
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<VocWeekStatusCountDTO>?> GetVocWeeksStatusDataService(HttpContext context);

        /// <summary>
        /// DashBoard용 하루치 각 타입별 카운트
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseUnit<VocDaysCountDTO>?> GetVocDashBoardDaysDataService(HttpContext context);
        

        /// <summary>
        /// DashBoard용 일주일치 민원 각 타입별 카운트
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task<ResponseList<VocWeekCountDTO>?> GetVocDashBoardWeeksDataService(HttpContext context);
    }
}
