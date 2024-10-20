using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FamTec.Server.Repository.Meter.Contract
{
    public class ContractInfoRepository : IContractInfoRepository
    {
        private readonly WorksContext context;
        private readonly ILogService LogService;
        private readonly ILogger<ContractInfoRepository> BuilderLogger;

        public ContractInfoRepository(WorksContext _context,
            ILogService _logservice,
            ILogger<ContractInfoRepository> _builderlogger)
        {
            this.context = _context;
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

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
        /// 계약종류 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ContractTypeTb?> AddAsync(ContractTypeTb model)
        {
            try
            {
                await context.ContractTypeTbs.AddAsync(model);
                bool AddResult = await context.SaveChangesAsync() > 0 ? true : false;

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
        /// 사업장에 속한 계약종류 전체반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<ContractTypeTb>?> GetAllContractList(int placeid)
        {
            try
            {
                List<ContractTypeTb>? model = await context.ContractTypeTbs
                    .Where(m => m.PlaceTbId == placeid &&
                                m.DelYn != true)
                    .ToListAsync();

                if (model is not null && model.Any())
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
        /// 계약 명칭으로 검색
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<ContractTypeTb?> GetContractName(int placeid, string name)
        {
            try
            {
                ContractTypeTb? model = await context.ContractTypeTbs
                    .FirstOrDefaultAsync(m => m.PlaceTbId == placeid &&
                                              m.DelYn != true &&
                                              m.Name == name);

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
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 계약종류 ID에 해당하는 계약종류정보 반환
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ContractTypeTb?> GetContractInfo(int id)
        {
            try
            {
                ContractTypeTb? model = await context.ContractTypeTbs
                    .FirstOrDefaultAsync(m => m.Id == id && 
                                              m.DelYn != true);

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

  
    }
}
