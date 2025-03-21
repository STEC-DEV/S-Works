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


                                        //int thisCurrentNum = await context.InventoryTbs
                                        //                        .Where(m => m.DelYn != true &&
                                        //                                    m.MaterialTbId == UseMaintenenceMaterialTB.MaterialTbId &&
                                        //                                    m.RoomTbId == UseMaintenenceMaterialTB.RoomTbId &&
                                        //                                    m.PlaceTbId == placeid)
                                        //                        .SumAsync(m => m.Num)
                                        //                        .ConfigureAwait(false);

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

                                        //int thisCurrentNum = await context.InventoryTbs
                                        //                        .Where(m => m.DelYn != true &&
                                        //                                    m.MaterialTbId == UseMaintenenceMaterialTB.MaterialTbId &&
                                        //                                    m.RoomTbId == UseMaintenenceMaterialTB.RoomTbId &&
                                        //                                    m.PlaceTbId == placeid)
                                        //                        .SumAsync(m => m.Num)
                                        //                        .ConfigureAwait(false);

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
                                    Console.WriteLine("결과가 다름 RollBack!");
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
                        return -1;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException dbEx)
                    {
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                        throw;
                    }
                    catch (MySqlException mysqlEx)
                    {
                        LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        LogService.LogMessage(ex.ToString());
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

                        //int Available = await context.InventoryTbs
                        //    .Where(m => m.DelYn != true &&
                        //    m.PlaceTbId == placeid &&
                        //                m.RoomTbId == UseMaintenenceMaterialTB.RoomTbId &&
                        //                m.MaterialTbId == UseMaintenenceMaterialTB.MaterialTbId)
                        //    .SumAsync(m => m.Num)
                        //    .ConfigureAwait(false);

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
                        return -1;
                    }
                    catch (Exception ex) when (IsDeadlockException(ex))
                    {
                        LogService.LogMessage($"데드락이 발생했습니다. 재시도 중: {ex}");
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException dbEx)
                    {
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {dbEx}");
                        throw;
                    }
                    catch (MySqlException mysqlEx)
                    {
                        LogService.LogMessage($"MariaDB 오류 발생: {mysqlEx}");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
                        return -1;
                    }
                }
            });
        }