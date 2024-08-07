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

        public BlackListService(IBlackListInfoRepository _blacklistinforepository,
            ILogService _logservice)
        {
            this.BlackListInfoRepository = _blacklistinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 블랙리스트 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddBlackListDTO?>> AddBlackList(HttpContext? context, AddBlackListDTO? dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<AddBlackListDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (dto is null)
                    return new ResponseUnit<AddBlackListDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddBlackListDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BlacklistTb model = new BlacklistTb();
                model.Phone = dto.PhoneNumber;
                model.CreateDt = DateTime.Now;
                model.CreateUser = creater;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creater;

                BlacklistTb? AddResult = await BlackListInfoRepository.AddAsync(model);
                if(AddResult is not null)
                {
                    return new ResponseUnit<AddBlackListDTO?>() 
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
                    return new ResponseUnit<AddBlackListDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddBlackListDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 전체 조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<ResponseList<BlackListDTO?>> GetAllBlackList(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<BlackListDTO?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<BlacklistTb>? model = await BlackListInfoRepository.GetBlackList();
                if(model is [_, ..])
                {
                    List<BlackListDTO> dto = model.Select(e => new BlackListDTO
                    {
                        ID = e.Id,
                        PhoneNumber = e.Phone
                    }).ToList();
                    return new ResponseList<BlackListDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto!, code = 200 };
                }
                else
                {
                    return new ResponseList<BlackListDTO?>() { message = "데이터가 존재하지 않습니다.", data = new List<BlackListDTO?>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<BlackListDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateBlackList(HttpContext? context, BlackListDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? updater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(updater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BlacklistTb? model = await BlackListInfoRepository.GetBlackListInfo(dto.ID);
                if (model is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Phone = dto.PhoneNumber;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = updater;

                bool? UpdateResult = await BlackListInfoRepository.UpdateBlackList(model);
                if(UpdateResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(UpdateResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 블랙리스트 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <param name="deleter"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteBlackList(HttpContext? context, List<int>? delIdx)
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

                bool? DeleteResult = await BlackListInfoRepository.DeleteBlackList(delIdx, deleter);
                
                if (DeleteResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if (DeleteResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

 

   
    }
}
