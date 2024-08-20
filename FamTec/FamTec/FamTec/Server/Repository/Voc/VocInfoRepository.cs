using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// VOC 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<VocTb?> AddAsync(VocTb model)
        {
            try
            {
                context.VocTbs.Add(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                if (AddResult)
                {
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
        /// 사업장별 민원 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<AllVocListDTO>?> GetVocList(int placeid)
        {
            try
            {
                List<int>? buildingidx = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid)
                    .Select(m => m.Id)
                    .ToListAsync();

                if (buildingidx is [_, ..])
                {
                    List<VocTb>? VocModel = await context.VocTbs
                        .Where(m => buildingidx.Contains(m.BuildingTbId) && m.DelYn != true)
                        .ToListAsync();

                    if(VocModel is [_, ..])
                    {
                        var grouplist = VocModel.GroupBy(voc => new
                        {
                            voc.CreateDt.Year,
                            voc.CreateDt.Month
                        }).Select(group => new
                        {
                            Year = group.Key.Year,
                            Month = group.Key.Month,
                            VocList = group.ToList()
                        })
                        .ToList();

                        if (grouplist is not [_, ..])
                            return null;

                        List<AllVocListDTO> AllVocList = new List<AllVocListDTO>();
                        foreach(var Group in grouplist)
                        {
                            AllVocListDTO VocItem = new AllVocListDTO();
                            VocItem.Years = Group.Year;
                            VocItem.Month = Group.Month;
                            VocItem.Dates = $"{Group.Year}-{Group.Month}";

                            List<VocListDTO> dto = (from VocTB in Group.VocList
                                                    join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                                    on VocTB.BuildingTbId equals BuildingTB.Id
                                                    select new VocListDTO
                                                    {
                                                        Id = VocTB.Id, // VOCID
                                                        BuildingName = BuildingTB.Name, // 건물명
                                                        Type = VocTB.Type, // 유형
                                                        Title = VocTB.Title, // 제목
                                                        Status = VocTB.Status, // 처리상태
                                                        CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 민원 요청시간
                                                        CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 민원처리 완료시간 -- .ToString() 에러
                                                        DurationDT = VocTB.DurationDt // 민원처리 소요시간
                                                    }).OrderBy(m => m.CreateDT).ToList();

                            VocItem.VocList = dto;
                            AllVocList.Add(VocItem);
                        }

                        if (AllVocList is [_, ..])
                            return AllVocList;
                        else
                            return null;
                    }
                    else
                    {
                        return null;
                    }

                }
                else
                {
                    // 건물이 없음
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 조건별 민원 리스트 조회
        /// </summary>
        /// <param name="buildinglist"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async ValueTask<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, int Type, int status,int BuildingID)
        {
            try
            {
                List<int>? buildingidx = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid)
                    .Select(m => m.Id)
                    .ToListAsync();

                if(buildingidx is [_, ..])
                {
                    // 해당 사업장에 넘어온 건물인덱스의 건물이 있는지?
                    bool buildingChk = buildingidx.Contains(BuildingID);
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
                                                        CreateDT = Voc.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                        Id = Voc.Id, // VOC ID
                                                        BuildingName = Building.Name, // 건물명
                                                        Type = Voc.Type, // 유형
                                                        Title = Voc.Title, // 민원제목
                                                        Status = Voc.Status, // 민원상태
                                                        CompleteDT = Voc.CompleteDt?.ToString("yyyy-MM-ddd HH:mm:ss"), // 처리일시
                                                        DurationDT = Voc.DurationDt // 소요시간
                                                    }).OrderBy(m => m.CreateDT).ToList();
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

    


        /// <summary>
        /// ID로 민원 상세조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async ValueTask<VocTb?> GetVocInfoById(int vocid)
        {
            try
            {

                VocTb? model = await context.VocTbs
                    .FirstOrDefaultAsync(m => m.Id == vocid && m.DelYn != true);

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

       

        /// <summary>
        /// Code로 민원 상세조회
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<VocTb?> GetVocInfoByCode(string code)
        {
            try
            {

                VocTb? model = await context.VocTbs
                    .FirstOrDefaultAsync(m => m.Code == code && m.DelYn != true);

                if (model is not null)
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

        public async ValueTask<bool> UpdateVocInfo(VocTb model)
        {
            try
            {
                context.VocTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

    }
}
