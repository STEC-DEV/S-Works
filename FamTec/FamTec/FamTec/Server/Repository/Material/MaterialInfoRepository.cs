using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
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
        private readonly ILogger<MaterialInfoRepository> BuilderLogger;

        public MaterialInfoRepository(WorksContext _context,
            ILogService _logservice,
            ILogger<MaterialInfoRepository> _builderlogger)
        {
            this.context = _context;
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        /// <summary>
        /// ASP - 빌드로그
        /// </summary>
        /// <param name="ex"></param>
        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor();
            }
            catch (Exception)
            {
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                        CreateBuilderLogger(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (MySqlException ex)
            {
                LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
                        CreateBuilderLogger(ex);
#endif
                        throw; // ExecutionStrategy가 자동으로 재시도 처리
                    }
                    catch (DbUpdateException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"데이터베이스 업데이트 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (MySqlException ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage($"MariaDB 오류 발생: {ex}");
#if DEBUG
                        CreateBuilderLogger(ex);
#endif
                        throw;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync().ConfigureAwait(false);
                        LogService.LogMessage(ex.ToString());
#if DEBUG
                        CreateBuilderLogger(ex);
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
                List<MaterialTb>? MaterialList = await context.MaterialTbs
                    .Where(m => m.DelYn != true &&
                               (m.Code.Contains(searchData) ||
                                m.ManufacturingComp.Contains(searchData) ||
                                m.Name.Contains(searchData) ||
                                m.Standard.Contains(searchData) &&
                               m.PlaceTbId == placeid))
                    .GroupBy(m => m.Id) // ID 기준으로 그룹화
                    .Select(g => g.First()) // 각 그룹에서 첫 번째 항목 선택
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (MaterialList is not null && MaterialList.Any())
                {
                    List<MaterialSearchListDTO> model = MaterialList.Select(e => new MaterialSearchListDTO
                    {
                        Id = e.Id,
                        Code = e.Code,
                        Name = e.Name,
                        Unit = e.Unit,
                        Standard = e.Standard,
                        Mfr = e.ManufacturingComp
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
                CreateBuilderLogger(ex);
#endif
                throw;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
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
