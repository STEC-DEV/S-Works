using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Place
{
    public class PlaceInfoRepository : IPlaceInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public PlaceInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사업장 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask<PlaceTb?> AddPlaceInfo(PlaceTb model)
        {
            try
            {
                await context.PlaceTbs.AddAsync(model);
                
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
        /// 전체조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<PlaceTb>?> GetAllList()
        {
            try
            {
                List<PlaceTb>? model = await context.PlaceTbs
                    .Where(m => m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (model is [_, ..])
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
        /// 사업장인덱스로 사업장 조회
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<PlaceTb?> GetByPlaceInfo(int id)
        {
            try
            {
                PlaceTb? model = await context.PlaceTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(id) &&
                    m.DelYn != true);

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
        /// 사업장 코드로 사업장 검색
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public async ValueTask<bool?> PlaceUKCheck(string Code)
        {
            try
            {
                PlaceTb? PlaceTB = await context.PlaceTbs
                    .FirstOrDefaultAsync(m => m.PlaceCd == Code && m.DelYn != true);

                if (PlaceTB is null)
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
        /// 삭제할 사업장 인덱스 조회 - 동시다발 삭제때문에 DelYN 적용안함.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<PlaceTb?> GetDeletePlaceInfo(int id)
        {
            try
            {
                PlaceTb? model = await context.PlaceTbs
                    .FirstOrDefaultAsync(m => m.Id.Equals(id) && m.DelYn != true);

                if (model is not null)
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
        /// 건물ID로 사업장정보 조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<PlaceTb?> GetBuildingPlace(int buildingid)
        {
            try
            {
                BuildingTb? builingTB = await context.BuildingTbs
                    .FirstOrDefaultAsync(m => m.Id == buildingid && m.DelYn != true);

                if(builingTB is not null)
                {
                    PlaceTb? PlaceTB = await context.PlaceTbs
                        .FirstOrDefaultAsync(m => m.Id == builingTB.PlaceTbId && m.DelYn != true);

                    if (PlaceTB is not null)
                        return PlaceTB;
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
        /// 삭제
        /// </summary>
        /// <param name="placecd"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeletePlace(PlaceTb model)
        {
            try
            {
                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                model.PlaceCd = $"{model.PlaceCd}_{model.Id}";
                context.PlaceTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 삭제 테스트 해야함.
        /// </summary>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<bool?> DeletePlaceList(string Name, List<int> placeidx)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int PlaceID in placeidx)
                    {
                        PlaceTb? PlaceTB = await context.PlaceTbs.FirstOrDefaultAsync(m => m.Id.Equals(PlaceID) && m.DelYn != true);
                        if(PlaceTB is not null)
                        {
                            // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                            PlaceTB.PlaceCd = $"{PlaceTB.PlaceCd}_{PlaceTB.Id}";
                            PlaceTB.DelYn = true;
                            PlaceTB.DelDt = DateTime.Now;
                            PlaceTB.DelUser = Name;

                            context.PlaceTbs.Update(PlaceTB);
                        }
                        else
                        {
                            await transaction.RollbackAsync();
                            return false;
                        }
                    }

                    bool DeleteResult = await context.SaveChangesAsync() > 0 ? true : false;
                    if(DeleteResult)
                    {
                        await transaction.CommitAsync();
                        return true;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 수정
        /// </summary>
        /// <param name="model"></param>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> EditPlaceInfo(PlaceTb model)
        {
            try
            {
                context.PlaceTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


    }
}
