using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MySqlConnector;
using System.Diagnostics;

namespace FamTec.Server.Repository.Material
{
    public class MaterialInfoRepository : IMaterialInfoRepository
    {
        private readonly WorksContext context;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<MaterialInfoRepository> CreateBuilderLogger;

        public MaterialInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<MaterialInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 대쉬보드용 안전재고와 가까운순 TOP 10
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<MaterialCountDTO>?> GetDashBoardMaterialCount(int placeid)
        {
            try
            {
                #region 쿼리
                /*
                 // 없는거 제회 기본 쿼리
                 SELECT inv.material_tb_id, 
                       SUM(inv.num) AS total_num, 
                       mat.safe_num,
                       ABS(COALESCE(mat.safe_num, 0) - SUM(inv.num)) AS distance
                FROM inventory_tb inv
                INNER JOIN material_tb mat 
                    ON inv.material_tb_id = mat.id
                WHERE mat.del_yn != true 
                  AND mat.place_tb_id = 1
                GROUP BY inv.material_tb_id, mat.safe_num
                ORDER BY distance ASC
                LIMIT 10;

                // 없는거 포함 기본 쿼리
                SELECT mat.id AS material_tb_id,
                       mat.name AS material_name,
                       COALESCE(SUM(inv.num), 0) AS total_num, 
                       COALESCE(mat.safe_num, 0) AS safe_num,
                       ABS(COALESCE(mat.safe_num, 0) - COALESCE(SUM(inv.num), 0)) AS distance
                FROM material_tb mat
                LEFT JOIN inventory_tb inv 
                    ON mat.id = inv.material_tb_id
                WHERE mat.del_yn != true 
                  AND mat.place_tb_id = 1
                GROUP BY mat.id, mat.safe_num, mat.name
                ORDER BY distance ASC
                LIMIT 10;
                 */
                #endregion

                List<MaterialTb> MaterialCheck = await context.MaterialTbs.Where(m => m.DelYn != true && m.PlaceTbId == placeid).ToListAsync();
                
                if (MaterialCheck is [_, ..])
                {

                    List<MaterialCountDTO> model = await (
                     from mat in context.MaterialTbs
                     join inv in context.InventoryTbs
                         on mat.Id equals inv.MaterialTbId into invGroup // LEFT JOIN
                     where mat.DelYn != true
                           && mat.PlaceTbId == placeid // MaterialTbs의 조건
                           && mat.SafeNum != null      // SafeNum이 null이 아닌 항목
                           && mat.SafeNum > 0          // SafeNum이 0보다 큰 항목
                     let filteredInvGroup = invGroup.Where(inv => inv.DelYn != true) // InventoryTbs의 조건
                     let totalNum = filteredInvGroup.Sum(inv => (int?)inv.Num) ?? 0 // InventoryTb의 Num 합계, 없으면 0
                     let safeNum = mat.SafeNum ?? 0 // SafeNum null이면 0
                     let distance = Math.Abs(safeNum - totalNum) // SafeNum과 totalNum 차이 계산
                     orderby distance // Distance 기준으로 정렬
                     select new MaterialCountDTO
                     {
                         MaterialId = mat.Id,       // MaterialTb의 Id
                         MaterialName = mat.Name,   // MaterialTb의 Name
                         TotalNum = totalNum,       // InventoryTb의 Num 합계
                         SafeNum = safeNum,         // MaterialTb의 SafeNum
                         Distance = distance        // SafeNum과 합계 차이의 절대값
                     })
                     .Take(10) // 상위 10개 결과 가져오기
                     .ToListAsync();

                    return model;
                }
                else
                {
                    return new List<MaterialCountDTO>();
                }
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
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async Task<bool?> DelMaterialCheck(int materialid)
        {
            try
            {
                bool InventoryCheck = await context.InventoryTbs
                    .AnyAsync(m => m.MaterialTbId == materialid && m.DelYn != true).ConfigureAwait(false);
                bool StoreCheck = await context.StoreTbs
                    .AnyAsync(m => m.MaterialTbId == materialid && m.DelYn != true).ConfigureAwait(false);
                bool UseMaterialCheck = await context.UseMaintenenceMaterialTbs
                    .AnyAsync(m => m.MaterialTbId == materialid && m.DelYn != true).ConfigureAwait(false);

                return InventoryCheck || StoreCheck || UseMaterialCheck;
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
        /// 자재 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<MaterialTb?> AddAsync(MaterialTb model)
        {
            try
            {
                await context.MaterialTbs.AddAsync(model).ConfigureAwait(false);
                bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                
                if (AddResult)
                    return model;
                else
                    return null;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
        /// 엑셀 IMPORT
        /// </summary>
        /// <param name="MaterialList"></param>
        /// <returns></returns>
        public async Task<bool?> AddMaterialList(List<MaterialTb> MaterialList)
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
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        await context.MaterialTbs.AddRangeAsync(MaterialList).ConfigureAwait(false);

                        bool AddResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (AddResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
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
        /// 해당 품목코드 사업장에 있는지 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<bool?> GetPlaceMaterialCheck(int placeid, string code)
        {
            try
            {
                MaterialTb? MaterialTB = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.DelYn != true && 
                                              m.Code == code && 
                                              m.PlaceTbId == placeid)
                    .ConfigureAwait(false);

                if (MaterialTB is null)
                    return true;
                else
                    return false;
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
        /// 사업장에 속해있는 자재 리스트들 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<List<MaterialTb>?> GetPlaceAllMaterialList(int placeid)
        {
            try
            {
                List<MaterialTb>? model = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is [_, ..])
                    return model;
                else
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
        /// 사업장에 속해있는 자재 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async Task<int> GetPlaceAllMaterialCount(int placeid)
        {
            try
            {
                int count = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && 
                                m.DelYn != true)
                    .CountAsync()
                    .ConfigureAwait(false);

                return count;
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
        /// 사업장에 속해있는 자재리스트 페이지네이션 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<List<MaterialTb>?> GetPlaceAllMaterialPageNationList(int placeid,int pagenumber, int pagesize)
        {
            try
            {
                List<MaterialTb>? model = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (model is not null && model.Any())
                    return model;
                else
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
        /// 자재 인덱스에 해당하는 모델클래스 반환
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public async Task<MaterialTb?> GetDetailMaterialInfo(int placeid, int materialId)
        {
            try
            {
                MaterialTb? model = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == materialId && 
                                              m.PlaceTbId == placeid &&
                                              m.DelYn != true)
                    .ConfigureAwait(false);

                if (model is not null)
                    return model;
                else
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
        /// 자재정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateMaterialInfo(MaterialTb model)
        {
            try
            {
                context.MaterialTbs.Update(model);
                return await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
            }
            catch (DbUpdateException ex)
            {
                LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                throw;
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
        /// 자재정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool?> DeleteMaterialInfo(List<int> delidx, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            DateTime ThisDate = DateTime.Now;

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음.
                Debugger.Break();
#endif

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    try
                    {
                        foreach (int delId in delidx)
                        {
                            MaterialTb? MaterialTB = await context.MaterialTbs
                                .FirstOrDefaultAsync(m => m.Id == delId && m.DelYn != true)
                                .ConfigureAwait(false);

                            if (MaterialTB is not null)
                            {
                                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                MaterialTB.Code = $"{MaterialTB.Code}_{MaterialTB.Id}";
                                MaterialTB.DelYn = true;
                                MaterialTB.DelDt = ThisDate;
                                MaterialTB.DelUser = deleter;

                                context.MaterialTbs.Update(MaterialTB);
                            }
                            else
                            {
                                // 잘못된 조회결과 (롤백)
                                await transaction.RollbackAsync().ConfigureAwait(false);
                                return (bool?)null;
                            }
                        }

                        bool MaterialResult = await context.SaveChangesAsync().ConfigureAwait(false) > 0 ? true : false;
                        if (MaterialResult)
                        {
                            await transaction.CommitAsync().ConfigureAwait(false);
                            return true;
                        }
                        else
                        {
                            // 업데이트 실패시 롤백
                            await transaction.RollbackAsync().ConfigureAwait(false);
                            return false;
                        }
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
        /// 품목 검색
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        public async Task<List<MaterialSearchListDTO>> GetMaterialSearchInfo(int placeid, string searchData)
        {
            try
            {
                var materialList = await (from m in context.MaterialTbs
                                          join r in context.RoomTbs on m.RoomTbId equals r.Id // MaterialTb와 RoomTb 조인
                                          join f in context.FloorTbs on r.FloorTbId equals f.Id // RoomTb와 FloorTb 조인
                                          join b in context.BuildingTbs on f.BuildingTbId equals b.Id // FloorTb와 BuildingTb 조인
                                          where m.DelYn != true &&
                                                m.PlaceTbId == placeid &&
                                                (m.Code.Contains(searchData) ||
                                                 m.ManufacturingComp.Contains(searchData) ||
                                                 m.Name.Contains(searchData) ||
                                                 m.Standard.Contains(searchData))
                                          group new { m, r, f, b } by m.Id into g
                                          select g.First()).ToListAsync()
                          .ConfigureAwait(false);

                if (materialList is not null && materialList.Any())
                {
                    List<MaterialSearchListDTO> model = materialList.Select(e => new MaterialSearchListDTO
                    {
                        Id = e.m.Id,
                        Code = e.m.Code,
                        Name = e.m.Name,
                        Unit = e.m.Unit,
                        Standard = e.m.Standard,
                        Mfr = e.m.ManufacturingComp,
                        RoomId = e.m.RoomTbId,
                        RoomName = e.r.Name,
                        BuildingId = e.b.Id // BuildingTb의 Name
                    }).ToList();

                    return model;
                }
                else
                {
                    return new List<MaterialSearchListDTO>();
                }
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
