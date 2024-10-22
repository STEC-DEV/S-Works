using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FamTec.Server.Repository.Building.SubItem.ItemValue
{
    public class BuildingItemValueInfoRepository : IBuildingItemValueInfoRepository
    {
        private readonly WorksContext context;

        private readonly ILogService LogService;
        private ConsoleLogService<BuildingItemValueInfoRepository> CreateBuilderLogger;

        public BuildingItemValueInfoRepository(WorksContext _context,
            ILogService _logservice,
            ConsoleLogService<BuildingItemValueInfoRepository> _createbuilderlogger)
        {
            this.context = _context;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 아이템 KEY에 대한 Value 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<BuildingItemValueTb?> AddAsync(BuildingItemValueTb model)
        {
            try
            {
                await context.BuildingItemValueTbs.AddAsync(model).ConfigureAwait(false);

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
        /// 아이템 Value 리스트 상세검색 keyid로 검색
        /// </summary>
        /// <param name="keyid"></param>
        /// <returns></returns>
        public async Task<List<BuildingItemValueTb>?> GetAllValueList(int keyid)
        {
            try
            {
                List<BuildingItemValueTb>? model = await context.BuildingItemValueTbs
                    .Where(m => m.BuildingKeyTbId == keyid && 
                                m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
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
        /// 아이템 Value 상세검색 valueid로 검색
        /// </summary>
        /// <param name="valueid"></param>
        /// <returns></returns>
        public async Task<BuildingItemValueTb?> GetValueInfo(int valueid)
        {
            try
            {
                BuildingItemValueTb? model = await context.BuildingItemValueTbs
                    .FirstOrDefaultAsync(m => m.Id == valueid && 
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
        /// 아이템 Value 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> UpdateValueInfo(BuildingItemValueTb model)
        {
            try
            {
                context.BuildingItemValueTbs.Update(model);
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
        /// 아이템 Value 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool?> DeleteValueInfo(BuildingItemValueTb model)
        {
            try
            {
                context.BuildingItemValueTbs.Update(model);
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

        public async Task<List<BuildingItemValueTb>?> ContainsKeyList(List<int> KeyitemId)
        {
            try
            {
                List<BuildingItemValueTb>? keytb = await context.BuildingItemValueTbs
                    .Where(e => KeyitemId.Contains(Convert.ToInt32(e.BuildingKeyTbId)) && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (keytb is not null && keytb.Any())
                    return keytb;
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

        public async Task<List<BuildingItemValueTb>?> NotContainsKeyList(List<int> KeyitemId)
        {
            try
            {
                List<BuildingItemValueTb>? keytb = await context.BuildingItemValueTbs
                    .Where(e => !KeyitemId.Contains(Convert.ToInt32(e.BuildingKeyTbId)) && e.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync()
                    .ConfigureAwait(false);

                if (keytb is not null && keytb.Any())
                    return keytb;
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
    }
}
