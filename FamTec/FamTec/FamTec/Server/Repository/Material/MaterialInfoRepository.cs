using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Material;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;

namespace FamTec.Server.Repository.Material
{
    public class MaterialInfoRepository : IMaterialInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;
        
        public MaterialInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 삭제가능여부 체크
        ///     참조하는게 하나라도 있으면 true 반환
        ///     아니면 false 반환
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DelMaterialCheck(int materialid)
        {
            try
            {
                bool InventoryCheck = await context.InventoryTbs
                    .AnyAsync(m => m.MaterialTbId == materialid && m.DelYn != true);
                bool StoreCheck = await context.StoreTbs
                    .AnyAsync(m => m.MaterialTbId == materialid && m.DelYn != true);
                bool UseMaterialCheck = await context.UseMaintenenceMaterialTbs
                    .AnyAsync(m => m.MaterialTbId == materialid && m.DelYn != true);

                return InventoryCheck || StoreCheck || UseMaterialCheck;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 자재 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<MaterialTb?> AddAsync(MaterialTb model)
        {
            try
            {
                await context.MaterialTbs.AddAsync(model);
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
        /// 엑셀 IMPORT
        /// </summary>
        /// <param name="MaterialList"></param>
        /// <returns></returns>
        public async ValueTask<bool?> AddMaterialList(List<MaterialTb> MaterialList)
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
                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        await context.MaterialTbs.AddRangeAsync(MaterialList);

                        bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (AddResult)
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
            });
        }

        /// <summary>
        /// 해당 품목코드 사업장에 있는지 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async ValueTask<bool?> GetPlaceMaterialCheck(int placeid, string code)
        {
            try
            {
                MaterialTb? MaterialTB = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.DelYn != true && 
                                              m.Code == code && 
                                              m.PlaceTbId == placeid);

                if (MaterialTB is null)
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
        /// 사업장에 속해있는 자재 리스트들 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<MaterialTb>?> GetPlaceAllMaterialList(int placeid)
        {
            try
            {
                List<MaterialTb>? model = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if(model is [_, ..])
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
        /// 사업장에 속해있는 자재 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<int> GetPlaceAllMaterialCount(int placeid)
        {
            try
            {
                int count = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && 
                                m.DelYn != true)
                    .CountAsync();

                return count;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
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
        public async ValueTask<List<MaterialTb>?> GetPlaceAllMaterialPageNationList(int placeid,int pagenumber, int pagesize)
        {
            try
            {
                List<MaterialTb>? model = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync();

                if (model is not null && model.Any())
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
        /// 자재 인덱스에 해당하는 모델클래스 반환
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public async ValueTask<MaterialTb?> GetDetailMaterialInfo(int placeid, int materialId)
        {
            try
            {
                MaterialTb? model = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == materialId && 
                                              m.PlaceTbId == placeid &&
                                              m.DelYn != true);

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
        /// 자재정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateMaterialInfo(MaterialTb model)
        {
            try
            {
                context.MaterialTbs.Update(model);
                return await context.SaveChangesAsync() > 0 ? true : false;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 자재정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<bool?> DeleteMaterialInfo(List<int> delidx, string deleter)
        {
            // ExecutionStrategy 생성
            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            // ExecutionStrategy를 통해 트랜잭션 재시도 가능
            return await strategy.ExecuteAsync(async () =>
            {
#if DEBUG
                // 강제로 디버깅 포인트 잡음.
                Debugger.Break();
#endif

                using (IDbContextTransaction transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 교착상태 방지용 타임아웃
                        context.Database.SetCommandTimeout(TimeSpan.FromSeconds(30));

                        foreach (int delId in delidx)
                        {
                            MaterialTb? MaterialTB = await context.MaterialTbs
                                .FirstOrDefaultAsync(m => m.Id == delId && m.DelYn != true);

                            if (MaterialTB is not null)
                            {
                                // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                                MaterialTB.Code = $"{MaterialTB.Code}_{MaterialTB.Id}";
                                MaterialTB.DelYn = true;
                                MaterialTB.DelDt = DateTime.Now;
                                MaterialTB.DelUser = deleter;

                                context.MaterialTbs.Update(MaterialTB);
                            }
                            else
                            {
                                // 잘못된 조회결과 (롤백)
                                await transaction.RollbackAsync();
                                return (bool?)null;
                            }
                        }

                        bool MaterialResult = await context.SaveChangesAsync() > 0 ? true : false;
                        if (MaterialResult)
                        {
                            await transaction.CommitAsync();
                            return true;
                        }
                        else
                        {
                            // 업데이트 실패시 롤백
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
        }

        /// <summary>
        /// 품목 검색
        /// </summary>
        /// <param name="searchData"></param>
        /// <returns></returns>
        public async ValueTask<List<MaterialSearchListDTO>> GetMaterialSearchInfo(int placeid, string searchData)
        {
            try
            {
                List<MaterialTb>? MaterialList = await context.MaterialTbs
                    .Where(m => m.DelYn != true &&
                               (m.Code == searchData ||
                                m.ManufacturingComp == searchData ||
                                m.Name == searchData ||
                                m.Standard == searchData) &&
                               m.PlaceTbId == placeid)
                    .GroupBy(m => m.Id) // ID 기준으로 그룹화
                    .Select(g => g.First()) // 각 그룹에서 첫 번째 항목 선택
                    .ToListAsync();

                if(MaterialList is not null && MaterialList.Any())
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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }


    }
}
