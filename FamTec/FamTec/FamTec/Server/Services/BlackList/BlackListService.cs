using FamTec.Server.Repository.BlackList;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.BlackList;

namespace FamTec.Server.Services.BlackList
{
    public class BlackListService : IBlackListService
    {
        private readonly IBlackListInfoRepository BlackListInfoRepository;
        private readonly ILogService LogService;
        private readonly ConsoleLogService<BlackListService> CreateBuilderLogger;

        public BlackListService(IBlackListInfoRepository _blacklistinforepository,
            ILogService _logservice,
            ConsoleLogService<BlackListService> _createbuilderlogger
        )
        {
            this.BlackListInfoRepository = _blacklistinforepository;
            
            this.CreateBuilderLogger = _createbuilderlogger;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 블랙리스트 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddBlackListDTO>> AddBlackList(HttpContext context, AddBlackListDTO dto)
        {
            try
            {
                if(context is null || dto is null)
                    return new ResponseUnit<AddBlackListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddBlackListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                BlacklistTb model = new BlacklistTb()
                {
                    Phone = dto.PhoneNumber!,
                    CreateDt = ThisTime,
                    CreateUser = creater,
                    UpdateDt = ThisTime,
                    UpdateUser = creater
                };

                BlacklistTb? AddResult = await BlackListInfoRepository.AddAsync(model).ConfigureAwait(false);
                if(AddResult is not null)
                {
                    return new ResponseUnit<AddBlackListDTO>() 
                    { 
                        message = "요청이 정상 처리되었습니다.",
                        data = new AddBlackListDTO 
                        { 
                            PhoneNumber = model.Phone 
                        },
                        code = 200 
                    };
                }
                else
                {
                    return new ResponseUnit<AddBlackListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddBlackListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 전체 조회
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseList<BlackListDTO>> GetAllBlackList(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<BlackListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<BlacklistTb>? model = await BlackListInfoRepository.GetBlackList().ConfigureAwait(false);
                if(model is not null && model.Any())
                {
                    List<BlackListDTO> dto = model.Select(e => new BlackListDTO
                    {
                        ID = e.Id,
                        PhoneNumber = e.Phone
                    }).ToList();

                    return new ResponseList<BlackListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto!, code = 200 };
                }
                else
                {
                    return new ResponseList<BlackListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<BlackListDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<BlackListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 개수 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<int?>> GetBlackListCountService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<int?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                int count = await BlackListInfoRepository.GetBlackListCount().ConfigureAwait(false);
                return new ResponseUnit<int?>() { message = "요청이 정상 처리되었습니다.", data = count, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 페이지네이션 조회
        /// </summary>
        /// <param name="pagenumber"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public async Task<ResponseList<BlackListDTO>> GetAllBlackListPageNation(HttpContext context, int pagenumber, int pagesize)
        {
            try
            {
                if (context is null)
                    return new ResponseList<BlackListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<BlacklistTb>? model = await BlackListInfoRepository.GetBlackListPaceNationList(pagenumber, pagesize).ConfigureAwait(false);
                if (model is not null && model.Any())
                {
                    List<BlackListDTO> dto = model.Select(e => new BlackListDTO
                    {
                        ID = e.Id,
                        PhoneNumber = e.Phone
                    }).ToList();

                    return new ResponseList<BlackListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto!, code = 200 };
                }
                else
                {
                    return new ResponseList<BlackListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<BlackListDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseList<BlackListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateBlackList(HttpContext context, BlackListDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? updater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(updater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                BlacklistTb? model = await BlackListInfoRepository.GetBlackListInfo(dto.ID!.Value).ConfigureAwait(false);
                if (model is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Phone = dto.PhoneNumber!;
                model.UpdateDt = ThisTime;
                model.UpdateUser = updater;

                bool? UpdateResult = await BlackListInfoRepository.UpdateBlackList(model).ConfigureAwait(false);
                
                return UpdateResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> DeleteBlackList(HttpContext context, List<int> delIdx)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (delIdx is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (delIdx.Count == 0)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? deleter = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? DeleteResult = await BlackListInfoRepository.DeleteBlackList(delIdx, deleter).ConfigureAwait(false);

                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
   
    }
}
