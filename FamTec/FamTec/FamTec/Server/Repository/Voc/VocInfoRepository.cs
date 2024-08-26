using FamTec.Client.Pages.Normal.User.UserAdd;
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
                await context.VocTbs.AddAsync(model);
             
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
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
        public async ValueTask<List<AllVocListDTO>?> GetVocList(int placeid, List<int> type, List<int> status, List<int> buildingid)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs.Where(m => type.Contains(m.Type) &&
                    status.Contains(m.Status) && 
                    buildingid.Contains(m.BuildingTbId))
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if(VocList is [_, ..])
                {
                    var grouplist = VocList.GroupBy(voc => new
                    {
                        voc.CreateDt.Year,
                        voc.CreateDt.Month
                    }).Select(group => new
                    {
                        Year = group.Key.Year,
                        Month = group.Key.Month,
                        VocList = group.ToList()
                    }).ToList();

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
        public async ValueTask<List<VocListDTO>?> GetVocFilterList(int placeid, DateTime StartDate, DateTime EndDate, List<int> Type, List<int> status,List<int> BuildingID)
        {
            try
            {
                List<VocTb>? VocList = await context.VocTbs
                    .Where(m => Type.Contains(m.Type) &&
                    status.Contains(m.Status) && 
                    BuildingID.Contains(m.BuildingTbId) &&
                    m.CreateDt >= StartDate && m.CreateDt <= EndDate)
                    .ToListAsync();

                if (VocList is [_, ..])
                {
                    List<VocListDTO>? dto = (from VocTB in VocList
                                            join BuildingTB in context.BuildingTbs.Where(m => m.DelYn != true).ToList()
                                            on VocTB.BuildingTbId equals BuildingTB.Id
                                            select new VocListDTO
                                            {
                                                CreateDT = VocTB.CreateDt.ToString("yyyy-MM-dd HH:mm:ss"), // 요청일시
                                                Id = VocTB.Id, // VOC ID
                                                BuildingName = BuildingTB.Name, // 건물명
                                                Type = VocTB.Type, // 유형
                                                Title = VocTB.Title, // 민원제목
                                                Status = VocTB.Status, // 민원상태
                                                CompleteDT = VocTB.CompleteDt?.ToString("yyyy-MM-dd HH:mm:ss"), // 처리일시
                                                DurationDT = VocTB.DurationDt // 소요시간
                                            }).OrderBy(m => m.CreateDT).ToList();

                    return dto;
                }
                else
                {
                    // VocList 조회결과가 없음.
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
