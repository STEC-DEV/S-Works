using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Room;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Diagnostics;

namespace FamTec.Server.Repository.Room
{
    public class RoomInfoRepository : IRoomInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public RoomInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        
        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false반환
        /// </summary>
        /// <param name="roomid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<bool?> DelRoomCheck(int roomid)
        {
            try
            {
                bool FacilityCheck = await context.FacilityTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true);
                bool InventoryCheck = await context.InventoryTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true);
                bool MaterialCheck = await context.MaterialTbs.AnyAsync(m => m.DefaultLocation == roomid && m.DelYn != true);
                bool StoreCheck = await context.StoreTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true);
                bool UseMaintenenceMartialCheck = await context.UseMaintenenceMaterialTbs.AnyAsync(m => m.RoomTbId == roomid && m.DelYn != true);

                return FacilityCheck || InventoryCheck || MaterialCheck || StoreCheck || UseMaintenenceMartialCheck;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 공간정보 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<RoomTb?> AddAsync(RoomTb model)
        {
            try
            {
                await context.RoomTbs.AddAsync(model);
                
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
             
                if (AddResult)
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
        /// 층에 해당하는 공간 List 반환
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<List<RoomTb>?> GetRoomList(int flooridx)
        {
            try
            {
                List<RoomTb>? model = await context.RoomTbs
                    .Where(m => m.DelYn != true && m.FloorTbId == flooridx)
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
        /// 층 List에 해당하는 공간 List 정보 조회
        /// </summary>
        /// <param name="FloorList"></param>
        /// <returns></returns>
        public async ValueTask<List<RoomTb>?> GetFloorRoomList(List<FloorTb> FloorList)
        {
            try
            {
                // 층 INDEX 뽑아냄
                List<int> FloorIdx = FloorList.Select(m => m.Id)
                    .ToList();

                List<RoomTb>? RoomList = await context.RoomTbs
                    .Where(m => FloorIdx.Contains(m.FloorTbId) && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if (RoomList is [_, ..])
                    return RoomList;
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
        /// 공간 인덱스로 공간 검색
        /// </summary>
        /// <param name="roomidx"></param>
        /// <returns></returns>
        public async ValueTask<RoomTb?> GetRoomInfo(int roomidx)
        {
            try
            {
                RoomTb? model = await context.RoomTbs
                    .FirstOrDefaultAsync(m => m.Id == roomidx && m.DelYn != true);

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
        /// 공간정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateRoomInfo(RoomTb model)
        {
            try
            {
                context.RoomTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteRoomInfo(RoomTb model)
        {
            try
            {
                context.RoomTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 공간정보 삭제
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteRoomInfo(List<int> idx, string deleter)
        {
            var strategy = context.Database.CreateExecutionStrategy();
            bool? result = await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 디버깅 포인트를 강제로 잡음.
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        if (idx is null || !idx.Any())
                            return (bool?)null;

                        foreach (int roomid in idx)
                        {
                            RoomTb? RoomTB = await context.RoomTbs
                                .FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true);

                            if (RoomTB is not null)
                            {
                                RoomTB.DelYn = true;
                                RoomTB.DelDt = DateTime.Now;
                                RoomTB.DelUser = deleter;

                                context.RoomTbs.Update(RoomTB);
                            }
                            else
                            {
                                // 공간정보 없음 -- 데이터가 잘못됨 (롤백)
                                await transaction.RollbackAsync();
                                return false;
                            }
                        }

                        bool UpdateResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (UpdateResult) // 성공시 커밋
                        {
                            await transaction.CommitAsync();
                            return true;
                        }
                        else // 실패시 롤백
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
            });
            return result;
        }


        /// <summary>
        /// 공간 삭제 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> RoomDeleteCheck(int placeid, int roomidx)
        {
            try
            {
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.DelYn != true &&
                    m.PlaceTbId == placeid &&
                    m.RoomTbId == roomidx)
                    .ToListAsync();

                if (model is [_, ..]) // 사업장ID + 공간ID로 자재테이블 검색해서 나오는게 1개라도 있으면 false
                    return false;
                else // 사업장ID + 공간ID로 자재테이블 검색해서 나오는게 1개라도 있으면 true
                    return true;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사업장에 해당하는 전체건물 - 전체층 - 전체공간 Group
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<PlaceRoomListDTO>?> GetPlaceAllGroupRoomInfo(int placeid)
        {
            try
            {
                List<BuildingTb>? BuildingList = await context.BuildingTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync();
                if (BuildingList is null || !BuildingList.Any())
                    return null;

                List<FloorTb>? FloorList = await context.FloorTbs.Where(m => BuildingList.Select(b => b.Id).Contains(m.BuildingTbId) && m.DelYn != true).ToListAsync();
                if (FloorList is null || !FloorList.Any())
                    return null;

                List<RoomTb>? RoomList = await context.RoomTbs.Where(m => FloorList.Select(f => f.Id).Contains(m.FloorTbId) && m.DelYn != true).ToListAsync();
                if (RoomList is null || !RoomList.Any())
                    return null;

                List<PlaceRoomListDTO>? model = BuildingList.Select(building => new PlaceRoomListDTO
                {
                    Id = building.Id,
                    Name = building.Name,
                    FloorList = FloorList.Where(floor => floor.BuildingTbId == building.Id)
                         .Select(floor => new BuildingFloor
                         {
                             Id = floor.Id,
                             Name = floor.Name,
                             RoomList = RoomList.Where(room => room.FloorTbId == floor.Id)
                                                .Select(room => new FloorRoom
                                                {
                                                    Id = room.Id,
                                                    Name = room.Name
                                                }).ToList()
                         }).ToList()
                }).ToList();

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

    }
}
