using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Building
{
    public class BuildingInfoRepository : IBuildingInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BuildingInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장에 속한 건물 총 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<int?> TotalBuildingCount(int placeid)
        {
            try
            {
                int totalCount = await context.BuildingTbs
                    .CountAsync(m => m.PlaceTbId == placeid && m.DelYn != true);

                return totalCount;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 건물추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<BuildingTb?> AddAsync(BuildingTb model)
        {
            try
            {
                await context.BuildingTbs.AddAsync(model);
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


        /// <summary>
        /// 해당사업장 건물 전체조회
        /// </summary>
        /// <param name="placeid">로그인한 사업장정보</param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> GetAllBuildingList(int placeid)
        {
            try
            {
                List<BuildingTb>? model = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (model is [_, ..])
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
        /// 해당 사업장의 건물조회 - 페이지네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> GetAllBuildingPageList(int placeid, int skip, int take)
        {
            try
            {
                List<BuildingTb>? model = await context.BuildingTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip(skip) // 건너뛸 개수
                    .Take(take) // 출력할 개수
                    .ToListAsync();

                if (model is [_, ..])
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
        /// 빌딩 인덱스로 빌딩 검색
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async ValueTask<BuildingTb?> GetBuildingInfo(int buildingId)
        {
            try
            {
                BuildingTb? model = await context.BuildingTbs
                    .FirstOrDefaultAsync(m => m.Id == buildingId && m.DelYn != true);

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

        /// <summary>
        /// 사용가능한 건물코드인지 검사
        /// </summary>
        /// <param name="buildingcode"></param>
        /// <returns></returns>
        public async ValueTask<bool?> CheckBuildingCD(string buildingcode)
        {
            try
            {
                BuildingTb? model = await context.BuildingTbs.FirstOrDefaultAsync(m => m.BuildingCd.Equals(buildingcode));
                if (model is null)
                    return true;
                else
                    return false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 건물정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateBuildingInfo(BuildingTb model)
        {
            try
            {
                context.BuildingTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 삭제할 리스트 조회
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<List<BuildingTb>?> GetDeleteList(List<int> buildingId)
        {
            try
            {
                List<BuildingTb>? buildinglist = await context.BuildingTbs.Where(m => buildingId.Contains(m.Id) && m.DelYn != true).ToListAsync();

                if (buildinglist is [_, ..])
                {
                    // 층 Table의 DelYN = true
                    List<BuildingTb> BuildingTb = (from buildingtb in buildinglist
                                                    join floortb in context.FloorTbs.Where(m => m.DelYn == true)
                                                    on buildingtb.Id equals floortb.BuildingTbId
                                                    select buildingtb).ToList();
                        
                    // 건물 Table의 층이 할당안된 테이블
                    List<BuildingTb>? deleteList = buildinglist.Where(a => !context.FloorTbs.Any(b => b.BuildingTbId == a.Id)).ToList();
                        
                    // Merge
                    deleteList.AddRange(BuildingTb);

                    if (deleteList is [_, ..])
                        return deleteList;
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
        /// 제네릭에 해당하는 빌딩정보들 반환
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> GetBuildings(List<int> buildingid)
        {
            try
            {
                List<BuildingTb>? model = await context.BuildingTbs
                    .Where(m => buildingid.Contains(m.Id))
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (model is [_, ..])
                {
                    return model;
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

        public async ValueTask<bool?> DeleteBuildingInfo(BuildingTb model)
        {
            try
            {
                context.BuildingTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 선택된 사업장에 포함되어있는 건물리스트 반환
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<List<BuildingTb>?> SelectPlaceBuildingList(List<int> placeidx)
        {
            try
            {
                List<BuildingTb>? buildingtb = await context.BuildingTbs
                    .Where(m => placeidx.Contains(Convert.ToInt32(m.PlaceTbId)) && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if(buildingtb is [_, ..])
                {
                    return buildingtb;
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
        /// 건물정보 삭제
        /// </summary>
        /// <param name="buildingid"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteBuildingList(List<int> buildingid, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (buildingid is [_, ..])
                    {
                        foreach(int bdid in buildingid)
                        {
                            BuildingTb? BuildingTB = await context.BuildingTbs
                                .FirstOrDefaultAsync(m => m.Id == bdid && m.DelYn != true);

                            if(BuildingTB is not null)
                            {
                                BuildingTB.DelYn = true;
                                BuildingTB.DelDt = DateTime.Now;
                                BuildingTB.DelUser = deleter;

                                context.BuildingTbs.Update(BuildingTB);
                                bool BuildingResult = await context.SaveChangesAsync() > 0 ? true : false;
                                if(!BuildingResult)
                                {
                                    // 업데이트 실패시 롤백
                                    await context.Database.RollbackTransactionAsync(); // 롤백
                                    return false;
                                }

                                List<BuildingItemGroupTb>? GroupList = await context.BuildingItemGroupTbs
                                    .Where(m => m.BuildingTbId == BuildingTB.Id && m.DelYn != true)
                                    .ToListAsync();

                                if(GroupList is [_, ..])
                                {
                                    foreach(BuildingItemGroupTb GroupTB in GroupList)
                                    {
                                        GroupTB.DelYn = true;
                                        GroupTB.DelDt = DateTime.Now;
                                        GroupTB.DelUser = deleter;

                                        context.BuildingItemGroupTbs.Update(GroupTB);
                                        bool GroupResult = await context.SaveChangesAsync() > 0 ? true : false;
                                        if(!GroupResult)
                                        {
                                            // 업데이트 실패시 롤백
                                            await context.Database.RollbackTransactionAsync();
                                            return false;
                                        }
                                        List<BuildingItemKeyTb>? KeyList = await context.BuildingItemKeyTbs
                                            .Where(m => m.BuildingGroupTbId == GroupTB.Id && m.DelYn != true)
                                            .ToListAsync();

                                        if(KeyList is [_, ..])
                                        {
                                            foreach(BuildingItemKeyTb KeyTB in KeyList)
                                            {
                                                KeyTB.DelYn = true;
                                                KeyTB.DelDt = DateTime.Now;
                                                KeyTB.DelUser = deleter;

                                                context.BuildingItemKeyTbs.Update(KeyTB);
                                                bool KeyResult = await context.SaveChangesAsync() > 0 ? true : false;
                                                if(!KeyResult)
                                                {
                                                    // 업데이트 실패시 롤백
                                                    await context.Database.RollbackTransactionAsync();
                                                    return false;
                                                }

                                                List<BuildingItemValueTb>? ValueList = await context.BuildingItemValueTbs.Where(m => m.BuildingKeyTbId == KeyTB.Id && m.DelYn != true).ToListAsync();
                                                if(ValueList is [_, ..])
                                                {
                                                    foreach(BuildingItemValueTb ValueTB in ValueList)
                                                    {
                                                        ValueTB.DelYn = true;
                                                        ValueTB.DelDt = DateTime.Now;
                                                        ValueTB.DelUser = deleter;

                                                        context.BuildingItemValueTbs.Update(ValueTB);
                                                        bool ValueResult = await context.SaveChangesAsync() > 0 ? true : false;
                                                        if(!ValueResult)
                                                        {
                                                            // 업데이트 실패시 롤백
                                                            await context.Database.RollbackTransactionAsync();
                                                            return false;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                            else
                            {
                                await context.Database.RollbackTransactionAsync(); // 롤백
                                return null;
                            }
                        }
                        await context.Database.CommitTransactionAsync(); // 커밋
                        return true;
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
        }

    
    }
}
