﻿using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.UseMaintenence
{
    public class UseMaintenenceInfoRepository : IUseMaintenenceInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<UseMaintenenceInfoRepository> CreateBuilderLogger;

        public UseMaintenenceInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<UseMaintenenceInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        public async Task<int> UpdateUseMaintance(UpdateMaintancematerialDTO dto, int placeid, string updater)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;
            bool SaveResult = false;

            // 20이 줄어드는 원인찾아야함.

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                Debugger.Break();
#endif
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        MaintenenceHistoryTb? maintanceHistoryTB = await context.MaintenenceHistoryTbs
                            .FirstOrDefaultAsync(m => m.Id == dto.MaintanceID &&
                                                      m.DelYn != true)
                            .ConfigureAwait(false);

                        if (maintanceHistoryTB is null)
                            return -1;

                        // 원본
                        List<UseMaintenenceMaterialTb> UseMaintanceList = await context.UseMaintenenceMaterialTbs
                            .Where(m => m.DelYn != true &&
                                        m.MaintenanceTbId == dto.MaintanceID)
                            .ToListAsync()
                            .ConfigureAwait(false);

                        List<int> UseMaintanceIdxList = UseMaintanceList.Select(m => m.Id).ToList();

                        // 똑같이있는거 -- 넘어온 DTO랑 Num 수량 비교
                        List<UseMaintenenceMaterialTb> updatelist= UseMaintanceList.Where(u => dto.UpdateUsematerialDTO.Any(update => update.UseID == u.Id)).ToList();

                        if (updatelist is [_, ..])
                        {
                            /* 재입고 - 추가출고에 대한 로직건 */
                            foreach (UseMaintenenceMaterialTb UpdateTB in updatelist)
                            {
                                UpdateUseMaterialDTO? UpdateModel = dto.UpdateUsematerialDTO.FirstOrDefault(m => m.UseID.HasValue && m.UseID!.Value == UpdateTB.Id);
                                if (UpdateModel is null)
                                    return -2;

                                if (UpdateTB.Num > UpdateModel.Num) /* 입고 */
                                {
                                    // UpdateTB.Num - UpdateModel.NUM 개수만큼 다시입고 해야함.
                                    int InOutNum = UpdateTB.Num - UpdateModel.Num; // 재입고 수량
                                    if (InOutNum < 0)
                                        return -2; // 잘못된 요청

                                    float MaxUnitPrice = await context.StoreTbs.Where(m => m.PlaceTbId == placeid &&
                                                                                            m.DelYn != true &&
                                                                                            m.MaintenenceHistoryTbId == maintanceHistoryTB.Id &&
                                                                                            m.MaintenenceMaterialTbId == UpdateTB.Id &&
                                                                                            m.RoomTbId == UpdateTB.RoomTbId &&
                                                                                            m.Inout == 0)
                                                                                            .MaxAsync(m => m.UnitPrice)
                                                                                            .ConfigureAwait(false);

                                    // 얘를 먼저 넣어야 Store에 CurrentNum을 채울 수 있음.
                                    InventoryTb NewInventoryTB = new InventoryTb();
                                    NewInventoryTB.MaterialTbId = UpdateTB.MaterialTbId;
                                    NewInventoryTB.RoomTbId = UpdateTB.RoomTbId;
                                    NewInventoryTB.Num = InOutNum; // 계산한 재입고 수량
                                    NewInventoryTB.UnitPrice = MaxUnitPrice; // 출고건 최대 금액으로 단가 설정
                                    NewInventoryTB.CreateDt = ThisDate;
                                    NewInventoryTB.CreateUser = updater;
                                    NewInventoryTB.UpdateDt = ThisDate;
                                    NewInventoryTB.UpdateUser = updater;
                                    NewInventoryTB.PlaceTbId = placeid;

                                    await context.InventoryTbs.AddAsync(NewInventoryTB).ConfigureAwait(false);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
                                        return -3;
                                    }

                                    int Available = await context.InventoryTbs
                                                .Where(m => m.DelYn != true &&
                                                            m.PlaceTbId == placeid &&
                                                            m.MaterialTbId == UpdateTB.MaterialTbId)
                                                .SumAsync(m => m.Num)
                                                .ConfigureAwait(false);

                                    StoreTb NewStoreTB = new StoreTb();
                                    NewStoreTB.Inout = 1; // 입고
                                    NewStoreTB.InoutDate = ThisDate;
                                    NewStoreTB.Num = InOutNum; // 재입고 수량
                                    NewStoreTB.CurrentNum = Available; // 현재 총 수량
                                    NewStoreTB.UnitPrice = MaxUnitPrice; // 최고 단가
                                    NewStoreTB.TotalPrice = InOutNum * MaxUnitPrice; // 총 금액
                                    NewStoreTB.CreateDt = ThisDate;
                                    NewStoreTB.CreateUser = updater;
                                    NewStoreTB.UpdateDt = ThisDate;
                                    NewStoreTB.UpdateUser = updater;
                                    NewStoreTB.PlaceTbId = placeid;
                                    NewStoreTB.RoomTbId = UpdateTB.RoomTbId;
                                    NewStoreTB.MaterialTbId = UpdateTB.MaterialTbId;
                                    NewStoreTB.MaintenenceHistoryTbId = maintanceHistoryTB.Id;
                                    NewStoreTB.MaintenenceMaterialTbId = UpdateTB.Id;
                                    //NewStoreTB.Note = UpdateModel.Note;

                                    await context.StoreTbs.AddAsync(NewStoreTB).ConfigureAwait(false);

                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    UpdateTB.Num = UpdateTB.Num - InOutNum; // 수량변경
                                    UpdateTB.Totalprice = UpdateTB.Num * MaxUnitPrice;
                                    UpdateTB.UpdateDt = ThisDate;
                                    UpdateTB.UpdateUser = updater;
                                    UpdateTB.Note = UpdateModel.Note;

                                    context.UseMaintenenceMaterialTbs.Update(UpdateTB);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    // 총 금액 변경사항 반영
                                    List<UseMaintenenceMaterialTb>? UseMaintenenceList = await context.UseMaintenenceMaterialTbs
                                    .Where(m => m.DelYn != true && m.MaintenanceTbId == maintanceHistoryTB.Id)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

                                    float maintenence_totalprice = UseMaintanceList.Sum(m => m.Totalprice);
                                    maintanceHistoryTB.UpdateDt = ThisDate;
                                    maintanceHistoryTB.TotalPrice = maintenence_totalprice;
                                    context.MaintenenceHistoryTbs.Update(maintanceHistoryTB);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }
                                }
                                else if (UpdateTB.Num < UpdateModel.Num) /*================================= 출고 ===========================*/
                                {
                                    // UpdateModel.Num - UpdateTB.Num 만큼 추가로 출고해야함
                                    int InOutNum = UpdateModel.Num - UpdateTB.Num; // 추가출고 수량
                                    if (InOutNum < 0)
                                        return -2; // 잘못된 요청

                                    // 수량체크
                                    List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, UpdateModel.RoomID, UpdateModel.MaterialID, InOutNum).ConfigureAwait(false);

                                    if (InventoryList is null || !InventoryList.Any())
                                        return -2; // 수량이 없음

                                    if (InventoryList.Sum(i => i.Num) < InOutNum)
                                        return -2; // 수량이 부족함

                                    List<InventoryTb> OutModel = new List<InventoryTb>(); // 출고 모델에 담는다
                                    int result = 0; // 개수만큼 담기위한 임시변수
                                    foreach (InventoryTb? inventory in InventoryList)
                                    {
                                        if (result <= InOutNum)
                                        {
                                            OutModel.Add(inventory);
                                            result += inventory.Num;
                                            if (result == InOutNum)
                                            {
                                                break; // 개수가 같으면 종료
                                            }
                                        }
                                        else // 개수보다 크면 종료
                                        {
                                            break;
                                        }
                                    }


                                    List<int> StoreID = new List<int>(); // 외래키 박기위해서 리스트에 담음
                                    float this_TotalPrice = 0; // 총금액 계산용 임시변수

                                    if (OutModel.Sum(i => i.Num) < InOutNum)
                                        return -2;

                                    if (OutModel is [_, ..])
                                    {
                                        // 출고 개수가 충분할때만 동작
                                        if (result >= InOutNum)
                                        {
                                            // 넘어온 수량이 실제로 빠지는 수량이랑 같은지 검사하는 CHECK SUM
                                            int checksum = 0;

                                            // 개수만큼 빼주면 됨
                                            int outresult = 0;

                                            foreach (InventoryTb OutInventoryTb in OutModel)
                                            {
                                                outresult += OutInventoryTb.Num;
                                                if (InOutNum > outresult) // 하나의 InventoryTb의 Row를 다 뺏는데 최종 빼야할 수량보다 담은 수량이 작으면
                                                {
                                                    int OutStoreEA = OutInventoryTb.Num;
                                                    checksum += OutInventoryTb.Num;

                                                    OutInventoryTb.Num -= OutInventoryTb.Num;
                                                    OutInventoryTb.UpdateDt = ThisDate;
                                                    OutInventoryTb.UpdateUser = updater;

                                                    if (OutInventoryTb.Num == 0)
                                                    {
                                                        OutInventoryTb.DelYn = true;
                                                        OutInventoryTb.DelDt = ThisDate;
                                                        OutInventoryTb.DelUser = updater;
                                                    }

                                                    context.Update(OutInventoryTb);
                                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                    if (!SaveResult)
                                                    {
                                                        // 트랜잭션 에러
                                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                        return -3;
                                                    }

                                                    int ThisCurrentNum = await context.InventoryTbs
                                                                                    .Where(m => m.DelYn != true &&
                                                                                                m.MaterialTbId == UpdateModel.MaterialID &&
                                                                                                m.PlaceTbId == placeid)
                                                                                    .SumAsync(m => m.Num)
                                                                                    .ConfigureAwait(false);

                                                    StoreTb StoreTB = new StoreTb();
                                                    StoreTB.Inout = 0; // 출고
                                                    StoreTB.Num = OutStoreEA; // 해당건 출고수
                                                    StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                                    StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                                    StoreTB.InoutDate = ThisDate;
                                                    StoreTB.CreateDt = ThisDate;
                                                    StoreTB.CreateUser = updater;
                                                    StoreTB.UpdateDt = ThisDate;
                                                    StoreTB.UpdateUser = updater;
                                                    StoreTB.RoomTbId = UpdateTB.RoomTbId;
                                                    StoreTB.MaterialTbId = UpdateModel.MaterialID;
                                                    StoreTB.CurrentNum = ThisCurrentNum;
                                                    StoreTB.PlaceTbId = placeid;
                                                    StoreTB.MaintenenceHistoryTbId = maintanceHistoryTB.Id;

                                                    await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);

                                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                    if (!SaveResult)
                                                    {
                                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                        return -3;
                                                    }
                                                    StoreID.Add(StoreTB.Id);

                                                    this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                                }
                                                else
                                                {
                                                    int OutStoreEA = InOutNum - (outresult - OutInventoryTb.Num);

                                                    checksum += InOutNum - (outresult - OutInventoryTb.Num);

                                                    outresult -= InOutNum;
                                                    OutInventoryTb.Num = outresult;
                                                    OutInventoryTb.UpdateDt = ThisDate;
                                                    OutInventoryTb.UpdateUser = updater;

                                                    if (OutInventoryTb.Num == 0)
                                                    {
                                                        OutInventoryTb.DelYn = true;
                                                        OutInventoryTb.DelDt = ThisDate;
                                                        OutInventoryTb.DelUser = updater;
                                                    }

                                                    context.Update(OutInventoryTb);
                                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;

                                                    if (!SaveResult)
                                                    {
                                                        await transaction.RollbackAsync().ConfigureAwait(false); // 트랜잭션 에러
#if DEBUG
                                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                        return -3;
                                                    }

                                                    int ThisCurrentNum = await context.InventoryTbs
                                                                                  .Where(m => m.DelYn != true &&
                                                                                              m.MaterialTbId == UpdateModel.MaterialID &&
                                                                                              m.PlaceTbId == placeid)
                                                                                  .SumAsync(m => m.Num)
                                                                                  .ConfigureAwait(false);

                                                    StoreTb StoreTB = new StoreTb();
                                                    StoreTB.Inout = 0; // 출고
                                                    StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                                    StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                                    StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                                    StoreTB.InoutDate = ThisDate;
                                                    StoreTB.CreateDt = ThisDate;
                                                    StoreTB.CreateUser = updater;
                                                    StoreTB.UpdateDt = ThisDate;
                                                    StoreTB.UpdateUser = updater;
                                                    StoreTB.RoomTbId = UpdateTB.RoomTbId;
                                                    StoreTB.MaterialTbId = UpdateModel.MaterialID;
                                                    StoreTB.CurrentNum = ThisCurrentNum;
                                                    StoreTB.PlaceTbId = placeid;
                                                    StoreTB.MaintenenceHistoryTbId = maintanceHistoryTB.Id;

                                                    await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);

                                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                    if (!SaveResult)
                                                    {
                                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                        return -3; // 트랜잭션 에러
                                                    }

                                                    StoreID.Add(StoreTB.Id);
                                                    this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                                }
                                            }

                                            if (checksum != InOutNum)
                                            {
                                                /* 출고하고자 하는 개수와 실제 개수가 다름. */
#if DEBUG
                                                CreateBuilderLogger.ConsoleText("예상 결과가 다름.");
#endif
                                                await transaction.RollbackAsync().ConfigureAwait(false);
                                                return -3;
                                            }

                                        }
                                    }

                                    UpdateTB.Num = UpdateModel.Num; // 총수량
                                    UpdateTB.Totalprice = UpdateTB.Totalprice + this_TotalPrice; // 총금액
                                    UpdateTB.UpdateDt = ThisDate;
                                    UpdateTB.UpdateUser = updater;
                                    UpdateTB.Note = UpdateModel.Note; // 비고
                                    context.UseMaintenenceMaterialTbs.Update(UpdateTB);

                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        // 저장실패 트랜잭션
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    foreach (int ID in StoreID)
                                    {
                                        StoreTb? UpdateStoreInfo = await context.StoreTbs.FirstOrDefaultAsync(m => m.DelYn != true && 
                                                                                                                   m.Id == ID)
                                        .ConfigureAwait(false);

                                        if (UpdateStoreInfo is null)
                                        {
                                            await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                            CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                            return -3;
                                        }

                                        UpdateStoreInfo.MaintenenceMaterialTbId = UpdateTB.Id;
                                        context.Update(UpdateStoreInfo);
                                    }

                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    // - 총금액 변경사항 반영
                                    // 빼는게 아니라 덧셈으로 토탈금액 구해서 유지보수 TotalPrice에 넣어야할듯.
                                    List<UseMaintenenceMaterialTb>? UseMaintenenceList = await context.UseMaintenenceMaterialTbs
                                    .Where(m => m.DelYn != true &&
                                                m.MaintenanceTbId == maintanceHistoryTB.Id)
                                                    .ToListAsync()
                                                    .ConfigureAwait(false);

                                    float maintenence_totalprice = UseMaintenenceList.Sum(m => m.Totalprice);

                                    maintanceHistoryTB.UpdateDt = ThisDate;
                                    maintanceHistoryTB.UpdateUser = updater;
                                    maintanceHistoryTB.TotalPrice = maintenence_totalprice;

                                    context.MaintenenceHistoryTbs.Update(maintanceHistoryTB);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -1;
                                    }
                                }
                            }
                        }

                        // -------------------------------------------
                        // 신규추가건
                        List<UpdateUseMaterialDTO> AddList = dto.UpdateUsematerialDTO.Where(m => m.UseID == 0).ToList();

                        if (AddList is [_, ..])
                        {
                            foreach (UpdateUseMaterialDTO AddModel in AddList)
                            {
                                int InOutNum = AddModel.Num;

                                List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, AddModel.RoomID, AddModel.MaterialID, InOutNum).ConfigureAwait(false);

                                if (InventoryList is null || !InventoryList.Any())
                                    return -2; // 수량이 없음

                                if (InventoryList.Sum(i => i.Num) < InOutNum)
                                    return -2; // 수량이 부족합

                                List<InventoryTb> OutModel = new List<InventoryTb>(); // 출고 모델에 담는다.
                                int result = 0; // 개수만큼 담기위한 임시변수
                                foreach (InventoryTb? inventory in InventoryList)
                                {
                                    if (result <= InOutNum)
                                    {
                                        OutModel.Add(inventory);
                                        result += inventory.Num;
                                        if (result == InOutNum)
                                        {
                                            break; // 개수가 같으면 종료
                                        }
                                    }
                                    else // 개수보다 크면 종료
                                    {
                                        break;
                                    }
                                }

                                List<int> StoreID = new List<int>();
                                float this_TotalPrice = 0; // 총금액 게산용 임시변수

                                if (OutModel.Sum(i => i.Num) < InOutNum)
                                    return -2;

                                if (OutModel is [_, ..])
                                {
                                    // 출고 개수가 충분할때만 동작
                                    if (result >= InOutNum)
                                    {
                                        // 넘어온 수량이 실제로 빠지는 수량이랑 같은지 검사하는 CHECK SUM
                                        int checksum = 0;

                                        // 개수만큼 빼주면 됨
                                        int outresult = 0;

                                        foreach (InventoryTb OutInventoryTb in OutModel)
                                        {
                                            outresult += OutInventoryTb.Num;
                                            if (InOutNum > outresult) // 하나의 InventoryTb의 Row를 다 뺏는데 최종 빼야할 수량보다 담은 수량이 작으면
                                            {
                                                int OutStoreEA = OutInventoryTb.Num;
                                                checksum += OutInventoryTb.Num;

                                                OutInventoryTb.Num -= OutInventoryTb.Num;
                                                OutInventoryTb.UpdateDt = ThisDate;
                                                OutInventoryTb.UpdateUser = updater;

                                                if (OutInventoryTb.Num == 0)
                                                {
                                                    OutInventoryTb.DelYn = true;
                                                    OutInventoryTb.DelDt = ThisDate;
                                                    OutInventoryTb.DelUser = updater;
                                                }

                                                context.Update(OutInventoryTb);
                                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                if (!SaveResult)
                                                {
                                                    // 트랜잭션 에러
                                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                    return -3;
                                                }

                                                int ThisCurrentNum = await context.InventoryTbs.Where(m => m.DelYn != true &&
                                                                                                           m.MaterialTbId == AddModel.MaterialID &&
                                                                                                           m.PlaceTbId == placeid)
                                                .SumAsync(m => m.Num)
                                                .ConfigureAwait(false);

                                                StoreTb StoreTB = new StoreTb();
                                                StoreTB.Inout = 0; // 출고
                                                StoreTB.Num = OutStoreEA; // 해당건 출고수
                                                StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                                StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                                StoreTB.InoutDate = ThisDate;
                                                StoreTB.CreateDt = ThisDate;
                                                StoreTB.CreateUser = updater;
                                                StoreTB.UpdateDt = ThisDate;
                                                StoreTB.UpdateUser = updater;
                                                StoreTB.RoomTbId = AddModel.RoomID;
                                                StoreTB.MaterialTbId = AddModel.MaterialID;
                                                StoreTB.CurrentNum = ThisCurrentNum;
                                                StoreTB.PlaceTbId = placeid;
                                                StoreTB.MaintenenceHistoryTbId = maintanceHistoryTB.Id;

                                                await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);

                                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                if (!SaveResult)
                                                {
                                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                    return -3; // 트랜잭션 에러
                                                }

                                                StoreID.Add(StoreTB.Id);
                                                this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                            }
                                            else
                                            {
                                                int OutStoreEA = InOutNum - (outresult - OutInventoryTb.Num);

                                                checksum += InOutNum - (outresult - OutInventoryTb.Num);

                                                outresult -= InOutNum;
                                                OutInventoryTb.Num = outresult;
                                                OutInventoryTb.UpdateDt = ThisDate;
                                                OutInventoryTb.UpdateUser = updater;

                                                if (OutInventoryTb.Num == 0)
                                                {
                                                    OutInventoryTb.DelYn = true;
                                                    OutInventoryTb.DelUser = updater;
                                                    OutInventoryTb.DelDt = ThisDate;
                                                }

                                                context.Update(OutInventoryTb);
                                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;

                                                if (!SaveResult)
                                                {
                                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                    return -3;
                                                }

                                                int ThisCurrentNum = await context.InventoryTbs
                                                     .Where(m => m.DelYn != true &&
                                                                 m.MaterialTbId == AddModel.MaterialID &&
                                                                 m.PlaceTbId == placeid)
                                                     .SumAsync(m => m.Num)
                                                     .ConfigureAwait(false);

                                                StoreTb StoreTB = new StoreTb();
                                                StoreTB.Inout = 0; // 출고
                                                StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                                StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                                StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                                StoreTB.InoutDate = ThisDate;
                                                StoreTB.CreateDt = ThisDate;
                                                StoreTB.CreateUser = updater;
                                                StoreTB.UpdateDt = ThisDate;
                                                StoreTB.UpdateUser = updater;
                                                StoreTB.RoomTbId = AddModel.RoomID;
                                                StoreTB.MaterialTbId = AddModel.MaterialID;
                                                StoreTB.CurrentNum = ThisCurrentNum;
                                                StoreTB.PlaceTbId = placeid;
                                                StoreTB.MaintenenceHistoryTbId = maintanceHistoryTB.Id;

                                                await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);

                                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                                if (!SaveResult)
                                                {
                                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                                    return -3; // 트랜잭션 에러
                                                }

                                                StoreID.Add(StoreTB.Id);
                                                this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액

                                            }
                                        }

                                        if (checksum != InOutNum)
                                        {
                                            /* 출고하고자 하는 개수와 실제 개수가 다름. */
                                            Console.WriteLine("결과가 다름 RollBack");

                                            await transaction.RollbackAsync().ConfigureAwait(false);
                                            return -3;
                                        }
                                    }
                                }

                                UseMaintenenceMaterialTb UseMaintenenceMaterialTB = new UseMaintenenceMaterialTb();
                                UseMaintenenceMaterialTB.Num = InOutNum;
                                UseMaintenenceMaterialTB.Totalprice = this_TotalPrice; // 총금액
                                UseMaintenenceMaterialTB.MaterialTbId = AddModel.MaterialID;
                                UseMaintenenceMaterialTB.RoomTbId = AddModel.RoomID;
                                UseMaintenenceMaterialTB.MaintenanceTbId = maintanceHistoryTB.Id;
                                UseMaintenenceMaterialTB.CreateDt = ThisDate;
                                UseMaintenenceMaterialTB.CreateUser = updater;
                                UseMaintenenceMaterialTB.UpdateDt = ThisDate;
                                UseMaintenenceMaterialTB.UpdateUser = updater;
                                UseMaintenenceMaterialTB.PlaceTbId = placeid;
                                UseMaintenenceMaterialTB.Note = AddModel.Note; // 비고

                                await context.UseMaintenenceMaterialTbs.AddAsync(UseMaintenenceMaterialTB).ConfigureAwait(false);
                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                if (!SaveResult)
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                    return -3;
                                }

                                foreach (int ID in StoreID)
                                {
                                    StoreTb? UpdateStoreInfo = await context.StoreTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == ID).ConfigureAwait(false);

                                    if (UpdateStoreInfo is null)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    UpdateStoreInfo.MaintenenceMaterialTbId = UseMaintenenceMaterialTB.Id; // 사용자재
                                    context.Update(UpdateStoreInfo);
                                }

                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                if (!SaveResult)
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                    return -3;
                                }

                                // 총금액 변경사항 반영
                                List<UseMaintenenceMaterialTb>? UseMaintenenceList = await context.UseMaintenenceMaterialTbs
                                                                        .Where(m => m.DelYn != true &&
                                                                                    m.MaintenanceTbId == maintanceHistoryTB.Id)
                                                                                        .ToListAsync()
                                                                                        .ConfigureAwait(false);

                                float maintenence_totalprice = UseMaintenenceList.Sum(m => m.Totalprice);

                                maintanceHistoryTB.UpdateDt = ThisDate;
                                maintanceHistoryTB.UpdateUser = updater;
                                maintanceHistoryTB.TotalPrice = maintenence_totalprice;

                                context.MaintenenceHistoryTbs.Update(maintanceHistoryTB);
                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                if (!SaveResult)
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                    return -3;
                                }
                            }
                        }

                        // List에는 있는데 DTO에는 없는거 -- 삭제 원래로 돌려놔야함.
                        List<int> delList = UseMaintanceIdxList.Except(dto.UpdateUsematerialDTO.Where(m => m.UseID.HasValue).Select(m => m.UseID.Value)).ToList();
                        if (delList is [_, ..])
                        {
                            foreach (int delID in delList)
                            {
                                UseMaintenenceMaterialTb? UseMaintenenceTB = await context.UseMaintenenceMaterialTbs.FirstOrDefaultAsync(m => m.DelYn != true && m.Id == delID).ConfigureAwait(false);
                                if (UseMaintenenceTB is null)
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                    return -3;
                                }

                                UseMaintenenceTB.DelYn = true;
                                UseMaintenenceTB.DelDt = ThisDate;
                                UseMaintenenceTB.DelUser = updater;

                                context.UseMaintenenceMaterialTbs.Update(UseMaintenenceTB);
                                SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                if (!SaveResult)
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                    return -3;
                                }

                                List<StoreTb>? StoreList = await context.StoreTbs.Where(m => m.DelYn != true && m.MaintenenceMaterialTbId == UseMaintenenceTB.Id).ToListAsync().ConfigureAwait(false);
                                if (StoreList is null || !StoreList.Any())
                                {
                                    await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                    CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                    return -3;
                                }

                                foreach (StoreTb StoreTB in StoreList)
                                {
                                    StoreTB.DelDt = ThisDate;
                                    StoreTB.DelYn = true;
                                    StoreTB.DelUser = updater;
                                    StoreTB.Note2 = $"[유지보수 취소건 삭제]";

                                    context.StoreTbs.Update(StoreTB);

                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    InventoryTb NewInventoryTB = new InventoryTb();
                                    NewInventoryTB.Num = StoreTB.Num;
                                    NewInventoryTB.UnitPrice = StoreTB.UnitPrice;
                                    NewInventoryTB.CreateDt = ThisDate;
                                    NewInventoryTB.CreateUser = updater;
                                    NewInventoryTB.UpdateDt = ThisDate;
                                    NewInventoryTB.UpdateUser = updater;
                                    NewInventoryTB.PlaceTbId = placeid;
                                    NewInventoryTB.RoomTbId = StoreTB.RoomTbId;
                                    NewInventoryTB.MaterialTbId = StoreTB.MaterialTbId;

                                    await context.InventoryTbs.AddAsync(NewInventoryTB).ConfigureAwait(false);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }

                                    int thisCurrentNum = await context.InventoryTbs.Where(m => m.DelYn != true &&
                                                                                                m.MaterialTbId == StoreTB.MaterialTbId &&
                                                                                                m.PlaceTbId == placeid)
                                                                                    .SumAsync(m => m.Num)
                                                                                    .ConfigureAwait(false);

                                    // 새로 재입고 로그
                                    StoreTb NewStoreTB = new StoreTb();
                                    NewStoreTB.Inout = 1;
                                    NewStoreTB.Num = StoreTB.Num; // 출고된 수량만큼 입고
                                    NewStoreTB.UnitPrice = StoreTB.UnitPrice; // 출고된 단가로 입고
                                    NewStoreTB.TotalPrice = StoreTB.Num * StoreTB.UnitPrice;
                                    NewStoreTB.InoutDate = ThisDate;
                                    NewStoreTB.CreateDt = ThisDate;
                                    NewStoreTB.CreateUser = updater;
                                    NewStoreTB.UpdateDt = ThisDate;
                                    NewStoreTB.UpdateUser = updater;
                                    NewStoreTB.CurrentNum = thisCurrentNum;
                                    NewStoreTB.PlaceTbId = placeid;
                                    NewStoreTB.RoomTbId = StoreTB.RoomTbId;
                                    NewStoreTB.MaterialTbId = StoreTB.MaterialTbId;

                                    await context.StoreTbs.AddAsync(NewStoreTB).ConfigureAwait(false);
                                    SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                    if (!SaveResult)
                                    {
                                        await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                        CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                        return -3;
                                    }
                                }
                            }

                            // 유지보수 이력의 총금액 변경해야함.
                            List<UseMaintenenceMaterialTb>? UseMaterialList = await context.UseMaintenenceMaterialTbs.Where(m => m.DelYn != true && m.MaintenanceTbId == maintanceHistoryTB.Id).ToListAsync().ConfigureAwait(false);

                            float totalPrice = UseMaterialList.Sum(m => m.Totalprice);

                            maintanceHistoryTB.UpdateDt = ThisDate;
                            maintanceHistoryTB.UpdateUser = updater;
                            maintanceHistoryTB.TotalPrice = totalPrice;

                            context.MaintenenceHistoryTbs.Update(maintanceHistoryTB);
                            SaveResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                            if (!SaveResult)
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
#if DEBUG
                                CreateBuilderLogger.ConsoleText("ASPlog > transaction error");
#endif
                                return -3;
                            }
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                        return 1;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return -1;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                }
            });
        }

        /// <summary>
        /// 사용자재 세부 이력 리스트
        /// </summary>
        /// <param name="usematerialid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<UseMaterialDetailDTO?> GetDetailUseStoreList(int usematerialid, int placeid)
        {
            try
            {
                UseMaintenenceMaterialTb? UseMaterialTB = await context.UseMaintenenceMaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == usematerialid && m.DelYn != true)
                    .ConfigureAwait(false);
                
                if (UseMaterialTB is null)
                    return null;

                RoomTb? RoomTB = await context.RoomTbs
                    .FirstOrDefaultAsync(m => m.Id == UseMaterialTB.RoomTbId && m.DelYn != true)
                    .ConfigureAwait(false);

                if (RoomTB is null)
                    return null;

                MaterialTb? MaterialTB = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == UseMaterialTB.MaterialTbId && m.DelYn != true)
                    .ConfigureAwait(false);

                if (MaterialTB is null)
                    return null;

                int Available = await context.InventoryTbs
                    .Where(m => m.DelYn != true && 
                                m.PlaceTbId == placeid && 
                                m.RoomTbId == UseMaterialTB.RoomTbId && 
                                m.MaterialTbId == UseMaterialTB.MaterialTbId)
                    .SumAsync(m => m.Num)
                    .ConfigureAwait(false);


                UseMaterialDetailDTO dto = new UseMaterialDetailDTO();
                dto.UseMaterialId = UseMaterialTB.Id; // 유지보수 사용자재 ID
                dto.Num = UseMaterialTB.Num; // 수량
                dto.TotalPrice = UseMaterialTB.Totalprice; // 총 금액
                dto.RoomId = UseMaterialTB.RoomTbId; // 공간ID
                dto.RoomName = RoomTB.Name; // 공간명칭
                dto.MaterialId = UseMaterialTB.MaterialTbId; // 사용자재 ID
                dto.MaterialName = MaterialTB.Name; // 사용자재 명칭
                dto.MaintenanceId = UseMaterialTB.MaintenanceTbId; // 유지보수 ID
                dto.TotalAvailableInventory = Available; // 공간 가용재고

                List<StoreTb>? StoreList = await context.StoreTbs
                    .Where(m => m.MaintenenceMaterialTbId == UseMaterialTB.Id && m.DelYn != true)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (StoreList is [_, ..])
                {
                    foreach(StoreTb StoreTB in StoreList)
                    {
                        dto.UseList.Add(new UseDetailStoreDTO
                        {
                            Id = StoreTB.Id,
                            InOut = StoreTB.Inout,
                            Num = StoreTB.Num,
                            UnitPrice = StoreTB.UnitPrice,
                            TotalPrice = StoreTB.TotalPrice
                        });
                    }
                }
                return dto;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 사용자재 세부이력 - 신규추가용
        ///     - 가용자재를 반환하면됨
        /// </summary>
        /// <param name="materialid"></param>
        /// <param name="roomid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<UseMaterialDetailDTO?> New_GetDetailUseStoreList(int materialid, int roomid, int placeid)
        {
            try
            {
                RoomTb? RoomTB = await context.RoomTbs
                  .FirstOrDefaultAsync(m => m.Id == roomid && m.DelYn != true)
                  .ConfigureAwait(false);

                if (RoomTB is null)
                    return null;
                MaterialTb? MaterialTB = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == materialid && m.DelYn != true)
                    .ConfigureAwait(false);

                if (MaterialTB is null)
                    return null;

                int Available = await context.InventoryTbs
                    .Where(m => m.DelYn != true &&
                    m.PlaceTbId == placeid &&
                    m.RoomTbId == roomid &&
                                m.MaterialTbId == materialid)
                    .SumAsync(m => m.Num)
                    .ConfigureAwait(false);

                UseMaterialDetailDTO dto = new UseMaterialDetailDTO();
                dto.RoomId = RoomTB.Id; // 공간ID
                dto.RoomName = RoomTB.Name; // 공간명칭
                dto.MaterialId = MaterialTB.Id; // 사용자재 ID
                dto.MaterialName = MaterialTB.Name; // 사용자재 명칭
                dto.TotalAvailableInventory = Available; // 공간 가용재고

                return dto;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 가용한 재고 수량 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<int?> UseAvailableMaterialNum(int placeid, int roomid, int materialid)
        {
            try
            {
                int? Available = await context.InventoryTbs
                                 .Where(m => m.DelYn != true &&
                                 m.PlaceTbId == placeid &&
                                             m.RoomTbId == roomid &&
                                             m.MaterialTbId == materialid)
                                 .SumAsync(m => m.Num)
                                 .ConfigureAwait(false);

                return Available;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 해당건 사용자재테이블에서 사용된 품목의 출고개수 반환 
        ///     -- 입고냐 출고냐 정해야 함.
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="usemaintanceid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int?> UseThisMaterialNum(int placeid, int maintenanceid, int roomid, int materialid)
        {
            try
            {
                // 해당 유지보수건에 대해 해당공간 + 해당품목에 출고 총 개수 조회
                int? ThisUseMaterialNum = await context.UseMaintenenceMaterialTbs
                    .Where(m => m.DelYn != true && 
                                m.MaintenanceTbId == maintenanceid && 
                                m.RoomTbId == roomid &&
                                m.MaterialTbId == materialid)
                    .SumAsync(m => m.Num)
                    .ConfigureAwait(false);

                return ThisUseMaterialNum;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 가지고있는 개수가 요청 개수보다 많은지 검사
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<List<InventoryTb>?> GetMaterialCount(int placeid, int roomid, int materialid, int delCount)
        {
            try
            {
                List<InventoryTb>? model = await context.InventoryTbs
                    .Where(m => m.MaterialTbId == materialid &&
                                m.RoomTbId == roomid &&
                                m.PlaceTbId == placeid &&
                                m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                // 개수가 뭐라도 있으면
                if (model is [_, ..])
                {
                    int? result = 0;

                    foreach(InventoryTb Inventory in model)
                    {
                        result += Inventory.Num; // 개수누적
                    }

                    if (result >= delCount) // 개수가 됨
                        return model;
                    else // 개수가 안됨
                        return null;
                }
                else // 개수 조회결과가 아에없음
                    return null;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 사용자재 수정 - 출고
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int?> UseMaintanceOutput(int placeid, string updater, UpdateMaintenanceMaterialDTO dto)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                bool UpdateResult = false;
                DateTime ThisDate = DateTime.Now;

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        //// 실제 추가출고 개수를 알아내야함.
                        MaintenenceHistoryTb? maintanceHistoryTB = await context.MaintenenceHistoryTbs
                        .FirstOrDefaultAsync(m => m.Id == dto.MaintanceID && m.DelYn != true)
                        .ConfigureAwait(false);

                        if (maintanceHistoryTB is null)
                            return -2; // 이력 자체가 없음. 잘못된 요청


                        UseMaintenenceMaterialTb? UseMaintenenceMaterialTB = await context.UseMaintenenceMaterialTbs
                        .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == dto.UseMaintanceID)
                        .ConfigureAwait(false);

                        if (UseMaintenenceMaterialTB is null)
                            return -2; // 얘는 수정이 아니라 입고등록으로 해야함. 잘못된 요청

                        // ** 넘어온 수량  -현재 수량 => 추가출고 수량
                        int InOutNum = dto.Num - UseMaintenenceMaterialTB.Num;
                        if (InOutNum < 0)
                            return -2; // 잘못된 요청

                        // 수량체크
                        List<InventoryTb>? InventoryList = await GetMaterialCount(placeid, UseMaintenenceMaterialTB.RoomTbId, UseMaintenenceMaterialTB.MaterialTbId, InOutNum)
                        .ConfigureAwait(false);

                        if (InventoryList is null || !InventoryList.Any())
                            return -1; // 수량이 아에 없음.

                        if (InventoryList.Sum(i => i.Num) < InOutNum)
                            return -1; // 수량이 부족함.

                        List<InventoryTb> OutModel = new List<InventoryTb>(); // 출고 모델에 담는다.
                        int result = 0; // 개수만큼만 담기위한 임시 변수
                        foreach (InventoryTb? inventory in InventoryList)
                        {
                            if (result <= InOutNum)
                            {
                                OutModel.Add(inventory);
                                result += inventory.Num;
                                if (result == InOutNum)
                                {
                                    break; // 개수가 같으면 종료
                                }
                            }
                            else // 개수보다 크면 종료
                                break;
                        }

                        List<int> StoreID = new List<int>(); // 외래키 박기위해서 리스트에 담음
                        float this_TotalPrice = 0; // 총금액 계산용 임시변수

                        // OutModel 담은 총 개수가 실제 출고할 개수보다 작으면 여기까지 오면 안되는 로직임
                        if (OutModel.Sum(i => i.Num) < InOutNum)
                            return -1; // 서버에서 처리하지 못함.

                        if (OutModel is [_, ..])
                        {
                            // 출고 개수가 충분할때만 동작
                            if (result >= InOutNum)
                            {
                                // 넘어온 수량이랑 실제로 빠지는 수량이랑 같은지 검사하는 CHECK SUM
                                int checksum = 0;

                                // 개수만큼 빼주면 됨
                                int outresult = 0;
                                foreach (InventoryTb OutInventoryTb in OutModel)
                                {
                                    outresult += OutInventoryTb.Num;
                                    if (InOutNum > outresult) // 하나의 InventoryTb의 Row를 다 뺏는데 최종 빼야할 수량보다 담은 수량이 작으면
                                    {
                                        int OutStoreEA = OutInventoryTb.Num;
                                        checksum += OutInventoryTb.Num;

                                        OutInventoryTb.Num -= OutInventoryTb.Num;
                                        OutInventoryTb.UpdateDt = ThisDate;
                                        OutInventoryTb.UpdateUser = updater;

                                        if (OutInventoryTb.Num == 0) // 무조껀 0일것 같은데 일단 체크전이라서 남겨둠 -------1
                                        {
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = ThisDate;
                                            OutInventoryTb.DelUser = updater;
                                        }

                                        context.Update(OutInventoryTb);
                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                        if (!UpdateResult)
                                        {
                                            // 트랜잭션 에러
                                            await transaction.RollbackAsync().ConfigureAwait(false);
                                            return -1;
                                        }

                                        int thisCurrentNum = await context.InventoryTbs
                                                                .Where(m => m.DelYn != true &&
                                                                            m.MaterialTbId == UseMaintenenceMaterialTB.MaterialTbId &&
                                                                            m.PlaceTbId == placeid)
                                                                .SumAsync(m => m.Num)
                                                                .ConfigureAwait(false);

                                        StoreTb StoreTB = new StoreTb();
                                        StoreTB.Inout = 0; // 출고
                                        StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                        StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                        StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                        StoreTB.InoutDate = ThisDate;
                                        StoreTB.CreateDt = ThisDate;
                                        StoreTB.CreateUser = updater;
                                        StoreTB.UpdateDt = ThisDate;
                                        StoreTB.UpdateUser = updater;
                                        StoreTB.RoomTbId = UseMaintenenceMaterialTB.RoomTbId;
                                        StoreTB.MaterialTbId = UseMaintenenceMaterialTB.MaterialTbId;
                                        StoreTB.CurrentNum = thisCurrentNum;
                                        //StoreTB.Note = model.AddStore.Note;
                                        StoreTB.PlaceTbId = placeid;
                                        StoreTB.MaintenenceHistoryTbId = UseMaintenenceMaterialTB.MaintenanceTbId;

                                        await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false); ;

                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                        if (!UpdateResult)
                                        {
                                            await transaction.RollbackAsync().ConfigureAwait(false);
                                            return -1;
                                        }
                                        StoreID.Add(StoreTB.Id);

                                        this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                    }
                                    else
                                    {
                                        int OutStoreEA = InOutNum - (outresult - OutInventoryTb.Num);

                                        checksum += InOutNum - (outresult - OutInventoryTb.Num);

                                        outresult -= InOutNum;
                                        OutInventoryTb.Num = outresult;
                                        OutInventoryTb.UpdateDt = ThisDate;
                                        OutInventoryTb.UpdateUser = updater;

                                        if (OutInventoryTb.Num == 0)
                                        {
                                            OutInventoryTb.DelYn = true;
                                            OutInventoryTb.DelDt = ThisDate;
                                            OutInventoryTb.DelUser = updater;
                                        }

                                        context.Update(OutInventoryTb);
                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                        if (!UpdateResult)
                                        {
                                            await transaction.RollbackAsync().ConfigureAwait(false);
                                            return -1;
                                        }

                                        int thisCurrentNum = await context.InventoryTbs
                                                                .Where(m => m.DelYn != true &&
                                                                            m.MaterialTbId == UseMaintenenceMaterialTB.MaterialTbId &&
                                                                            m.PlaceTbId == placeid)
                                                                .SumAsync(m => m.Num)
                                                                .ConfigureAwait(false);


                                        StoreTb StoreTB = new StoreTb();
                                        StoreTB.Inout = 0; // 출고
                                        StoreTB.Num = OutStoreEA; // 해당건 출고 수
                                        StoreTB.UnitPrice = OutInventoryTb.UnitPrice; // 단가
                                        StoreTB.TotalPrice = OutStoreEA * OutInventoryTb.UnitPrice; // 총금액
                                        StoreTB.InoutDate = ThisDate;
                                        StoreTB.CreateDt = ThisDate;
                                        StoreTB.CreateUser = updater;
                                        StoreTB.UpdateDt = ThisDate;
                                        StoreTB.UpdateUser = updater;
                                        StoreTB.RoomTbId = UseMaintenenceMaterialTB.RoomTbId;
                                        StoreTB.MaterialTbId = UseMaintenenceMaterialTB.MaterialTbId;
                                        StoreTB.CurrentNum = thisCurrentNum;
                                        //StoreTB.Note = model.AddStore.Note;
                                        StoreTB.PlaceTbId = placeid;
                                        StoreTB.MaintenenceHistoryTbId = UseMaintenenceMaterialTB.MaintenanceTbId;


                                        await context.StoreTbs.AddAsync(StoreTB).ConfigureAwait(false);

                                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                                        if (!UpdateResult)
                                        {
                                            await transaction.RollbackAsync().ConfigureAwait(false);
                                            return -1;
                                        }
                                        StoreID.Add(StoreTB.Id);

                                        this_TotalPrice += OutStoreEA * OutInventoryTb.UnitPrice; // 해당건 총 금액
                                    }
                                }

                                if (checksum != InOutNum)
                                {
                                    /* 출고하고자 하는 개수와 실제 개수가 다름. (동시성에서 누가 먼저 뺏을경우 발생함.) */
                                    
#if DEBUG
                                    CreateBuilderLogger.ConsoleText("예상결과가 다름");
#endif
                                    await transaction.RollbackAsync().ConfigureAwait(false);
                                    return -1;
                                }

                                await context.SaveChangesAsync().ConfigureAwait(false);
                            }
                        }


                        UseMaintenenceMaterialTB.Num = dto.Num; // 총수량
                        UseMaintenenceMaterialTB.Totalprice = UseMaintenenceMaterialTB.Totalprice + this_TotalPrice;  // 총금액
                        UseMaintenenceMaterialTB.MaterialTbId = UseMaintenenceMaterialTB.MaterialTbId;
                        UseMaintenenceMaterialTB.RoomTbId = UseMaintenenceMaterialTB.RoomTbId;
                        UseMaintenenceMaterialTB.MaintenanceTbId = maintanceHistoryTB.Id;
                        //UseMaintenenceMaterialTB.Note = model.AddStore.Note;
                        UseMaintenenceMaterialTB.CreateDt = ThisDate;
                        UseMaintenenceMaterialTB.CreateUser = updater;
                        UseMaintenenceMaterialTB.UpdateDt = ThisDate;
                        UseMaintenenceMaterialTB.UpdateUser = updater;
                        UseMaintenenceMaterialTB.PlaceTbId = placeid;
                        context.UseMaintenenceMaterialTbs.Update(UseMaintenenceMaterialTB);
                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            /* 저장 실패 - (트랜잭션) */
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }

                        foreach (int ID in StoreID)
                        {
                            StoreTb? UpdateStoreInfo = await context.StoreTbs
                            .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == ID)
                            .ConfigureAwait(false);

                            if (UpdateStoreInfo is null)
                            {
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return -1;
                            }

                            UpdateStoreInfo.MaintenenceMaterialTbId = UseMaintenenceMaterialTB.Id;
                            context.Update(UpdateStoreInfo);
                        }

                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }

                        // - 총금액 변경사항 반영
                        // 빼는게 아니라 덧셈으로 토탈금액 구해서 유지보수 TotalPrice에 넣어야할듯.
                        List<UseMaintenenceMaterialTb>? UseMaintenenceList = await context.UseMaintenenceMaterialTbs
                                        .Where(m => m.DelYn != true &&
                                                    m.MaintenanceTbId == maintanceHistoryTB.Id)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                        float maintenence_totalprice = UseMaintenenceList.Sum(m => m.Totalprice);

                        maintanceHistoryTB.UpdateDt = ThisDate;
                        maintanceHistoryTB.UpdateUser = updater;
                        maintanceHistoryTB.TotalPrice = maintenence_totalprice;
                        context.MaintenenceHistoryTbs.Update(maintanceHistoryTB);
                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }

                        /* 여기까지 오면 출고완료 */
                        await transaction.CommitAsync().ConfigureAwait(false);

                        /* 저장된 유지보수 테이블 ID를 반환한다. */
                        return maintanceHistoryTB.Id;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return -1;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return -1;
                    }
                }
            });
        }

        /// <summary>
        /// 사용자재 수정 - 입고
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int?> UseMatintanceInput(int placeid,string updater, UpdateMaintenanceMaterialDTO dto)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅포인트 잡음.
                Debugger.Break();
#endif
                bool UpdateResult = false;
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        MaintenenceHistoryTb? maintanceHistoryTB = await context.MaintenenceHistoryTbs
                        .FirstOrDefaultAsync(m => m.Id == dto.MaintanceID && m.DelYn != true)
                        .ConfigureAwait(false);

                        if (maintanceHistoryTB is null)
                            return -2; // 이력 자체가 없음. 잘못된 요청


                        UseMaintenenceMaterialTb? UseMaintenenceMaterialTB = await context.UseMaintenenceMaterialTbs
                        .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == dto.UseMaintanceID)
                        .ConfigureAwait(false);

                        if (UseMaintenenceMaterialTB is null)
                            return -2; // 얘는 수정이 아니라 입고등록으로 해야함. 잘못된 요청

                        int InOutNum = UseMaintenenceMaterialTB.Num - dto.Num; // 현재 수량 - 넘어온 수량 => 재입고 수량
                        if (InOutNum < 0)
                            return -2; // 잘못된 요청

                        // 여기서 단가를 위한 계산이 ++ 해야할듯..
                        // 출고시에 는 해당 뺴는 금액이 그대로 들어가면되고
                        // 입고시에는 유지보수1건에대해 같은품목에 대해서 금액이 다른게 있을수도 있으니
                        // 거기서 가격이 가장 높은 금액대로 뽑아서 계산한 수량만큼 재입고 넣어주면될듯
                        // 출고단가에서 가장 비싼금액으로 입고시켜야 할듯.
                        float MaxUnitPrice = await context.StoreTbs.Where(m => m.PlaceTbId == placeid &&
                                                                                m.DelYn != true &&
                                                                                m.MaintenenceHistoryTbId == maintanceHistoryTB.Id &&
                                                                                m.MaintenenceMaterialTbId == UseMaintenenceMaterialTB.Id &&
                                                                                m.RoomTbId == UseMaintenenceMaterialTB.RoomTbId &&
                                                                                m.Inout == 0)
                                                                                .MaxAsync(m => m.UnitPrice)
                                                                                .ConfigureAwait(false);


                        // 얘를 먼저넣어야 Store에 CurrentNum을 채울 수 있음.
                        InventoryTb NewInventoryTB = new InventoryTb();
                        NewInventoryTB.MaterialTbId = UseMaintenenceMaterialTB.MaterialTbId;
                        NewInventoryTB.RoomTbId = UseMaintenenceMaterialTB.RoomTbId;
                        NewInventoryTB.Num = InOutNum; // 계산한 재입고 수량
                        NewInventoryTB.UnitPrice = MaxUnitPrice; // 출고건 최대 금액으로 단가 설정
                        NewInventoryTB.CreateDt = ThisDate;
                        NewInventoryTB.CreateUser = updater;
                        NewInventoryTB.UpdateDt = ThisDate;
                        NewInventoryTB.UpdateUser = updater;
                        NewInventoryTB.PlaceTbId = placeid;

                        context.InventoryTbs.Add(NewInventoryTB);
                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            // 업데이트 실패 - 동시성
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }

                        int Available = await context.InventoryTbs
                            .Where(m => m.DelYn != true &&
                            m.PlaceTbId == placeid &&
                                        m.MaterialTbId == UseMaintenenceMaterialTB.MaterialTbId)
                            .SumAsync(m => m.Num)
                            .ConfigureAwait(false);

                        StoreTb NewStoreTB = new StoreTb();
                        NewStoreTB.Inout = 1; // 입고
                        NewStoreTB.InoutDate =ThisDate;
                        NewStoreTB.Num = InOutNum; // 재입고 수량
                        NewStoreTB.CurrentNum = Available; // 현재 총 수량
                        NewStoreTB.UnitPrice = MaxUnitPrice; // 받은 단가
                        NewStoreTB.TotalPrice = InOutNum * MaxUnitPrice; // 총 금액
                        NewStoreTB.CreateDt = ThisDate;
                        NewStoreTB.CreateUser = updater;
                        NewStoreTB.UpdateDt = ThisDate;
                        NewStoreTB.UpdateUser = updater;
                        NewStoreTB.PlaceTbId = placeid;
                        NewStoreTB.RoomTbId = UseMaintenenceMaterialTB.RoomTbId;
                        NewStoreTB.MaterialTbId = UseMaintenenceMaterialTB.MaterialTbId;
                        NewStoreTB.MaintenenceHistoryTbId = maintanceHistoryTB.Id;
                        NewStoreTB.MaintenenceMaterialTbId = UseMaintenenceMaterialTB.Id;

                        context.StoreTbs.Add(NewStoreTB);

                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            // 업데이트 실패 - 동시성
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }

                        UseMaintenenceMaterialTB.Num = UseMaintenenceMaterialTB.Num - InOutNum; // 수량변경
                                                                                                //UseMaintenenceMaterialTB.Totalprice = UseMaintenenceMaterialTB.Totalprice - (dto.Num * MaxUnitPrice);
                        UseMaintenenceMaterialTB.Totalprice = UseMaintenenceMaterialTB.Num * MaxUnitPrice;

                        UseMaintenenceMaterialTB.UpdateDt = ThisDate; // 현재시간
                        UseMaintenenceMaterialTB.UpdateUser = updater; // 수정자
                        context.UseMaintenenceMaterialTbs.Update(UseMaintenenceMaterialTB);
                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            // 업데이트 실패 - 동시성
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }


                        // - 총금액 변경사항 반영
                        // 빼는게 아니라 덧셈으로 토탈금액 구해서 유지보수 TotalPrice에 넣어야할듯. 
                        List<UseMaintenenceMaterialTb>? UseMaintenenceList = await context.UseMaintenenceMaterialTbs
                                        .Where(m => m.DelYn != true &&
                                                    m.MaintenanceTbId == maintanceHistoryTB.Id)
                                        .ToListAsync()
                                        .ConfigureAwait(false);

                        float maintenence_totalprice = UseMaintenenceList.Sum(m => m.Totalprice);

                        maintanceHistoryTB.UpdateDt = DateTime.Now;
                        maintanceHistoryTB.UpdateUser = updater;
                        maintanceHistoryTB.TotalPrice = maintenence_totalprice;
                        context.MaintenenceHistoryTbs.Update(maintanceHistoryTB);
                        UpdateResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (!UpdateResult)
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return -1;
                        }

                        await transaction.CommitAsync().ConfigureAwait(false);
                        return 1;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return -1;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger.ConsoleLog(ex);
#endif
                        return -1;
                    }
                }
            });
        }

        /// <summary>
        /// 사용자재 이력 테이블 조회
        /// </summary>
        /// <param name="useid"></param>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<UseMaintenenceMaterialTb?> GetUseMaintanceInfo(int useid, int placeid)
        {
            try
            {
                UseMaintenenceMaterialTb? UseMaintenceMaterialTB = await context.UseMaintenenceMaterialTbs.FirstOrDefaultAsync(m => m.Id == useid && m.PlaceTbId == placeid && m.DelYn != true);

                if (UseMaintenceMaterialTB is not null)
                    return UseMaintenceMaterialTB;
                else
                    return null;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
            }
        }

        /// <summary>
        /// 데드락 감지코드
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool IsDeadlockException(Exception ex)
        {
            // MySqlException 및 MariaDB의 교착 상태 오류 코드는 일반적으로 1213입니다.
            if (ex is MySqlException mysqlEx && mysqlEx.Number == 1213)
                return true;

            // InnerException에도 동일한 확인 로직을 적용
            if (ex.InnerException is MySqlException innerMySqlEx && innerMySqlEx.Number == 1213)
                return true;

            return false;
        }
        
    }
}
