using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Word;
using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FamTec.Server.Repository.Maintenence
{
    /// <summary>
    /// 유지보수 이력을 하려면 자재부터 해야함.
    /// </summary>
    public class MaintanceRepository : IMaintanceRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public MaintanceRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 유지보수 사용자재 등록 -- 출고등록과 비슷하다 보면됨.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddMaintanceAsync(AddMaintanceDTO? dto, string? creater, int? placeid, string? GUID)
        {
            try
            {
                if (dto is null)
                    return null;
                if(placeid is null)
                    return null;
                if (String.IsNullOrWhiteSpace(creater))
                    return null;
                if (String.IsNullOrWhiteSpace(GUID))
                    return null;

                // [1]. 토큰체크
                foreach(InOutInventoryDTO InventoryDTO in dto.Inventory)
                {
                    List<InventoryTb>? Occupant = await context.InventoryTbs
                        .Where(m => m.PlaceTbId == placeid &&
                        m.RoomTbId == InventoryDTO.AddStore.RoomID &&
                        m.DelYn != true).ToListAsync();

                    List<InventoryTb>? check = Occupant
                        .Where(m =>
                        !String.IsNullOrWhiteSpace(m.Occupant) ||
                        !String.IsNullOrWhiteSpace(m.TimeStamp.ToString()))
                        .ToList().Where(m => m.Occupant != GUID).ToList();

                    if(check is [_, ..]) // 다른곳에서 해당항목 사용중
                    {
                        Console.WriteLine("다른곳에서 해당 품목을 사용중입니다.");
                        return false;
                    }
                }

                // [2]. 수량체크
                foreach (InOutInventoryDTO model in dto.Inventory)
                {
                    int? result = 0;

                    // 출고할게 여러곳에 있으니 전체 Check 개수 Check
                    List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore.RoomID, model.MaterialID, model.AddStore.Num, GUID);
                    if (InventoryList is [_, ..]) // 여기에 들어오면 개수는 통과한거임.
                    {
                        foreach(InventoryTb? inventory in InventoryList)
                        {
                            if(result <= model.AddStore.Num)
                            {
                                result += inventory.Num;
                            }
                        }

                        if(result < model.AddStore.Num)
                        {
                            Console.WriteLine("수량이 부족합니다.");
                            await RoolBackOccupant(GUID);
                            return null;
                        }
                    }
                }


                using var transaction = context.Database.BeginTransaction();

                MaintenenceHistoryTb? MaintenenceHistory = new MaintenenceHistoryTb();
                MaintenenceHistory.Name = dto.Name; // 작업명
                MaintenenceHistory.Type = dto.Type; // 작업구분 (자체작업 / 외주작업 ..)
                MaintenenceHistory.Worker = dto.Worker; // 작업자
                MaintenenceHistory.UnitPrice = dto.UnitPrice; // 단가
                MaintenenceHistory.Num = dto.Num; // 수량
                MaintenenceHistory.TotalPrice = dto.TotalPrice; // 소요비용
                MaintenenceHistory.CreateDt = DateTime.Now; // 생성일자
                MaintenenceHistory.CreateUser = creater; // 생성자
                MaintenenceHistory.UpdateDt = DateTime.Now; // 수정일자
                MaintenenceHistory.UpdateUser = creater; // 수정자
                MaintenenceHistory.FacilityTbId = dto.FacilityID; // 설비 ID
                
                context.MaintenenceHistoryTbs.Add(MaintenenceHistory);
                bool AddHistoryResult = await context.SaveChangesAsync() > 0 ? true : false; // 저장

                if (!AddHistoryResult)
                {
                    await RoolBackOccupant(GUID);
                    transaction.Rollback();
                    return null;
                }

                foreach(InOutInventoryDTO model in dto.Inventory)
                {
                    List<InventoryTb> OutModel = new List<InventoryTb>();
                    int? result = 0;

                    // 출고시킬 LIST를 만든다 = 사업장ID + ROOMID + MATERIAL ID + 삭제수량 + GUID로 검색
                    List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, model.AddStore.RoomID, model.MaterialID, model.AddStore.Num, GUID);
                    if(InventoryList is [_, ..])
                    {
                        foreach(InventoryTb? inventory in InventoryList)
                        {
                            if(result <= model.AddStore.Num)
                            {
                                OutModel.Add(inventory);
                                result += inventory.Num;
                                if(result == model.AddStore.Num)
                                {
                                    // 반복문 종료
                                    break;
                                }
                            }
                            else
                            {
                                // 반복문 종료
                                break;
                            }
                        }

                        if(OutModel is [_, ..])
                        {
                            if(result >= model.AddStore.Num) // 출고개수가 충분할때만 동작.
                            {
                                // 개수만큼 - 빼주면 됨
                                int? outresult = 0;
                                foreach(InventoryTb OutInventoryTb in OutModel)
                                {
                                    outresult += OutInventoryTb.Num;
                                    if(model.AddStore.Num > outresult)
                                    {
                                        OutInventoryTb.Num -= OutInventoryTb.Num;
                                        OutInventoryTb.UpdateDt = DateTime.Now;
                                        OutInventoryTb.UpdateUser = creater;

                                        if(OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.TimeStamp = null;
                                            OutInventoryTb.Occupant = null;
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = DateTime.Now;
                                            OutInventoryTb.DelUser = creater;
                                        }

                                        context.Update(OutInventoryTb);
                                    }
                                    else
                                    {
                                        outresult -= model.AddStore.Num;
                                        OutInventoryTb.Num = outresult;
                                        OutInventoryTb.UpdateDt = DateTime.Now;
                                        OutInventoryTb.UpdateUser = creater;

                                        if(OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.TimeStamp = null;
                                            OutInventoryTb.Occupant = null;
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = DateTime.Now;
                                            OutInventoryTb.DelUser = creater;
                                        }
                                        context.Update(OutInventoryTb);
                                    }
                                }
                            }
                        }

                        bool InventoryResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(!InventoryResult)
                        {
                            await RoolBackOccupant(GUID);
                            transaction.Rollback();
                            return null;
                        }

                        // Inventory 테이블에서 해당 품목의 개수 Sum
                        int? thisCurrentNum = context.InventoryTbs.Where(m =>
                        m.DelYn != true &&
                        m.MaterialTbId == model.MaterialID &&
                        m.RoomTbId == model.AddStore.RoomID &&
                        m.PlaceTbId == placeid)
                            .Sum(m => m.Num);


                        if (thisCurrentNum == null)
                            thisCurrentNum = 0;

                        StoreTb store = new StoreTb();
                        store.Inout = model.InOut;
                        store.Num = model.AddStore.Num;
                        store.UnitPrice = model.AddStore.UnitPrice;
                        store.TotalPrice = model.AddStore.TotalPrice;
                        store.InoutDate = model.AddStore.InOutDate;
                        store.CreateDt = DateTime.Now;
                        store.CreateUser = creater;
                        store.UpdateDt = DateTime.Now;
                        store.UpdateUser = creater;
                        store.RoomTbId = model.AddStore.RoomID;
                        store.MaterialTbId = model.MaterialID;
                        store.CurrentNum = thisCurrentNum;
                        store.Note = model.AddStore.Note;
                        store.PlaceTbId = placeid;
                        store.MaintenenceHistoryTbId = MaintenenceHistory.Id;

                        context.StoreTbs.Add(store);
                        bool StoreResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if(!StoreResult)
                        {
                            await RoolBackOccupant(GUID);
                            transaction.Rollback();
                            return null;
                        }
                    }
                    else // 출고개수가 부족함
                    {
                        await RoolBackOccupant(GUID);
                        transaction.Rollback();
                        return null;
                    }
                }

                Console.WriteLine("출고완료");
                transaction.Commit();
                return true;
            }
            catch (DBConcurrencyException ex)
            {
                // 해당 GUID 찾아서 TiemStamp / 토큰 null 해줘야함.
                await RoolBackOccupant(GUID);
                LogService.LogMessage($"동시성 에러 {ex.Message}");
                throw new ArgumentNullException();
            }
            catch (Exception ex)
            {
                await RoolBackOccupant(GUID);
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 유지보수이력 설비ID에 해당하는거 전체조회
        /// </summary>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public async ValueTask<List<MaintenenceHistoryTb>?> GetFacilityHistoryList(int? facilityid)
        {
            try
            {
                if(facilityid is not null)
                {
                    List<MaintenenceHistoryTb>? model = await context.MaintenenceHistoryTbs
                        .Where(m => m.FacilityTbId == facilityid && m.DelYn != true)
                        .ToListAsync();

                    if (model is [_, ..])
                        return model;
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
        /// 가지고있는 개수가 삭제할 개수보다 많은지 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="delCount"></param>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int? placeid, int? roomid, int? materialid, int? delCount, string? Guid)
        {
            try
            {
                if (materialid is null)
                    return null;
                if (roomid is null)
                    return null;
                if (placeid is null)
                    return null;
                if (delCount is null)
                    return null;
                if (String.IsNullOrWhiteSpace(Guid))
                    return null;

                // 선입선출
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.MaterialTbId == materialid &&
                    m.RoomTbId == roomid &&
                    m.PlaceTbId == placeid &&
                    m.Occupant == Guid &&
                    m.DelYn != true).OrderBy(m => m.CreateDt).ToListAsync();
                
                // 개수가 뭐라도 있으면
                if (model is [_, ..]) 
                {
                    int? result = 0;
                    
                    foreach(InventoryTb Inventory in model)
                    {
                        result += Inventory.Num; // 개수누적
                    }

                    if(result >= delCount) // 개수가됨
                    {
                        return model;
                    }
                    else // 개수가안됨 ROLLBACK
                    {
                        await RoolBackOccupant(Guid);
                        return null;
                    }
                }
                else // 개수 조회결과가 아에없음
                {
                    await RoolBackOccupant(Guid); // 롤백
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
        /// SET 토큰
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> SetOccupantToken(int? placeid, AddMaintanceDTO? dto, string? guid)
        {
            try
            {
                if (placeid is null)
                    return null;
                if (dto is null)
                    return null;
                if (String.IsNullOrWhiteSpace(guid))
                    return null;

                using var transaction = context.Database.BeginTransaction();

                foreach(InOutInventoryDTO inventoryDTO in dto.Inventory)
                {
                    List<InventoryTb>? Occupant = await context.InventoryTbs
                    .Where(m => m.PlaceTbId == placeid &&
                    m.MaterialTbId == inventoryDTO.MaterialID &&
                    m.RoomTbId == inventoryDTO.AddStore.RoomID &&
                    m.DelYn != true).ToListAsync();

                    if(Occupant is [_, ..]) // 여기는 TRUE 여야 됨.
                    {
                        List<InventoryTb>? check = Occupant
                        .Where(m =>
                        !String.IsNullOrWhiteSpace(m.Occupant) ||
                        !String.IsNullOrWhiteSpace(m.TimeStamp.ToString()))
                        .ToList()
                        .Where(m => m.Occupant != guid).ToList();

                        if (check is [_, ..])
                        {
                            return false; // 다른데서 품목 사용중
                        }
                        else // 여기는 FALSE 여야 됨.
                        {
                            foreach (InventoryTb OccModel in Occupant)
                            {
                                OccModel.TimeStamp = DateTime.Now;
                                OccModel.Occupant = guid;
                                context.Update(OccModel);
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                bool result = await context.SaveChangesAsync() > 0 ? true : false;
                if (result)
                {
                    transaction.Commit();
                    return true;
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
            }
            catch (DbUpdateConcurrencyException ex) // 동시성 에러
            {
                // 해당 GUID 찾아서 TimeStamp / 토큰 null 해줘야함.
                await RoolBackOccupant(guid);
                LogService.LogMessage($"동시성 에러 {ex.Message}");
                throw new ArgumentNullException();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<Task?> RoolBackOccupant(string GUID)
        {
            try
            {
                if (GUID is null)
                    return null;

                List<InventoryTb>? Occupant = await context.InventoryTbs
                    .Where(m => m.DelYn != true && m.Occupant == GUID)
                    .ToListAsync();

                if(Occupant is [_, ..])
                {
                    foreach(InventoryTb model in Occupant)
                    {
                        model.Occupant = null;
                        model.TimeStamp = null;

                        context.InventoryTbs.Update(model);
                    }
                }

                await context.SaveChangesAsync();
                return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return null;
            }
        }



        /// <summary>
        /// 유지보수 이력 사업장별 날짜기간 전체
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ValueTask<List<MaintenenceHistoryTb>?> GetDateHistoryList(int? placeid, string? date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 유지보수이력 ID에 해당하는 상세정보 검색
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ValueTask<MaintenenceHistoryTb>? GetDetailHistoryInfo(int? id)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 유지보수이력 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MaintenenceHistoryTb>? UpdateHistoryInfo(MaintenenceHistoryTb? model)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 유지보수이력 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ValueTask<MaintenenceHistoryTb>? DeleteHistoryInfo(MaintenenceHistoryTb? model)
        {
            throw new NotImplementedException();
        }
    }
}
