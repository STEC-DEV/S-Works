using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Repository.Voc
{
    public class VocInfoRepository : IVocInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public VocInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        public async ValueTask<VocTb?> AddAsync(VocTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.VocTbs.Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 로그인한 사업장에 속한 건물들에 대한 모든 민원리스트 반환
        /// </summary>
        /// <param name="buildinglist"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<VocListDTO>?> GetVocList(int? placeid, DateTime? StartDate, DateTime? EndDate, int? Type, int? status,int? BuildingID)
        {
            try
            {
                if (placeid is null)
                    return null;
                if(StartDate is null)
                    return null;
                if(EndDate is null)
                    return null;
                if(Type is null)
                    return null;
                if(BuildingID is null)
                    return null;

                List<int>? buildingidx = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid)
                    .Select(m => m.Id)
                    .ToListAsync();

                if(buildingidx is [_, ..])
                {
                    // 해당 사업장에 넘어온 건물인덱스의 건물이 있는지?
                    bool buildingChk = buildingidx.Contains(BuildingID.Value);
                    if(buildingChk)
                    {
                        List<VocTb>? VocModel = null;
                        // 건물이 있음
                        VocModel = await context.VocTbs
                        .Where(m => m.DelYn != true &&
                        m.BuildingTbId == BuildingID &&
                        m.CreateDt >= StartDate &&
                        m.CreateDt <= EndDate &&
                        m.Status == status &&
                        m.Type == Type)
                        .OrderBy(m => m.CreateDt)
                        .ToListAsync();

                        if(VocModel is [_, ..]) // 해당조건의 모델이 있는지?
                        {
                            // DTO JOIN
                            List<VocListDTO> dto = (from Voc in VocModel
                                                join Building in context.BuildingTbs.Where(m => m.DelYn != true && m.Id == BuildingID)
                                                on Voc.BuildingTbId equals Building.Id
                                                select new VocListDTO
                                                {
                                                    CreateDT = Voc.CreateDt, // 요청일시
                                                    Id = Voc.Id, // VOC ID
                                                    BuildingName = Building.Name, // 건물명
                                                    Type = Voc.Type, // 유형
                                                    Title = Voc.Title, // 민원제목
                                                    Status = Voc.Status, // 민원상태
                                                    CompleteDT = Voc.CompleteDt, // 처리일시
                                                    DurationDT = Voc.DurationDt // 소요시간
                                                }).ToList();

                            if (dto is [_, ..])
                                return dto;
                            else
                                return null;
                        }
                        else
                        {
                            // 없음
                            return null;
                        }
                    }
                    else
                    {
                        // 건물이 없음
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        // 민원 상세조회
        public async ValueTask<VocTb?> GetDetailVoc(int? vocid)
        {
            try
            {
                if (vocid is null)
                    return null;

                VocTb? model = await context.VocTbs.FirstOrDefaultAsync(m => m.Id == vocid && m.DelYn != true);
                if(model is not null)
                    return model;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<bool> UpdateVocInfo(VocTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.VocTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
