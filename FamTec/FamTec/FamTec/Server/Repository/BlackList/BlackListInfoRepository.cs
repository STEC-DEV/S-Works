using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.BlackList
{
    public class BlackListInfoRepository : IBlackListInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public BlackListInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 블랙리스트 추가
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public async ValueTask<BlacklistTb?> AddAsync(BlacklistTb model)
        {
            try
            {
                await context.BlacklistTbs.AddAsync(model);
                
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
        /// 블랙리스트 전체조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<List<BlacklistTb>?> GetBlackList()
        {
            try
            {
                List<BlacklistTb>? model = await context.BlacklistTbs
                    .Where(m => m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .ToListAsync();

                if(model is not null && model.Any())
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
        /// 블랙리스트 페이지네이션 조회
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<BlacklistTb>?> GetBlackListPaceNationList(int pagenumber, int pagesize)
        {
            try
            {
                List<BlacklistTb>? model = await context.BlacklistTbs
                    .Where(m => m.DelYn != true)
                    .OrderBy(m => m.CreateDt)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .ToListAsync();

                if (model is not null && model.Any())
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
        /// 블랙리스트 개수 반환
        /// </summary>
        /// <returns></returns>
        public async ValueTask<int> GetBlackListCount()
        {
            try
            {
                int count = await context.BlacklistTbs
                    .Where(m => m.DelYn != true)
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
        /// 블랙리스트 전화번호로 조회
        /// </summary>
        /// <param name="PhoneNumber"></param>
        /// <returns></returns>
        public async ValueTask<BlacklistTb?> GetBlackListInfo(string PhoneNumber)
        {
            try
            {
                BlacklistTb? model = await context.BlacklistTbs
                    .FirstOrDefaultAsync(m => m.Phone == PhoneNumber && m.DelYn != true);
                
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
        /// 블랙리스트 ID로 추가
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<BlacklistTb?> GetBlackListInfo(int id)
        {
            try
            {
                BlacklistTb? model = await context.BlacklistTbs
                    .FirstOrDefaultAsync(m => m.Id == id && m.DelYn != true);

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
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public async ValueTask<bool?> DeleteBlackList(List<int> delIdx, string deleter)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    foreach(int BlackListID in delIdx)
                    {
                        BlacklistTb? BlackListTB = await context.BlacklistTbs
                            .FirstOrDefaultAsync(m => m.DelYn != true && m.Id == BlackListID);
                        
                        if(BlackListTB is not null)
                        {
                            // 삭제시에는 해당명칭 다시사용을 위해 원래이름_ID 로 명칭을 변경하도록 함.
                            BlackListTB.Phone = $"{BlackListTB.Phone}_{BlackListTB.Id}";
                            BlackListTB.DelYn = true;
                            BlackListTB.DelDt = DateTime.Now;
                            BlackListTB.DelUser = deleter;

                            context.BlacklistTbs.Update(BlackListTB);
                        }
                        else
                        {
                            // 조회결과가 없음
                            return null;
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
                catch(Exception ex)
                {
                    LogService.LogMessage(ex.ToString());
                    throw new ArgumentNullException();
                }
            }
        }

        /// <summary>
        /// 블랙리스트 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateBlackList(BlacklistTb model)
        {
            try
            {
                context.BlacklistTbs.Update(model);
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
