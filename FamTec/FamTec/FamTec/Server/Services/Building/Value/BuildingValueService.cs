using FamTec.Server.Helpers;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;

namespace FamTec.Server.Services.Building.Value
{
    public class BuildingValueService : IBuildingValueService
    {
        private readonly IBuildingItemKeyInfoRepository BuildingItemKeyInfoRepository;
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<BuildingValueService> CreateBuilderLogger; /* 콘솔로그 */

        public BuildingValueService(IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            IBuildingItemKeyInfoRepository _buildingitemkeyinforepository,
            IHttpContextAccessor _httpcontextaccessor,
            ILogService _logservice,
            ConsoleLogService<BuildingValueService> _createbuilderlogger)
        {
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            this.BuildingItemKeyInfoRepository = _buildingitemkeyinforepository;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        public async Task<ResponseModel<AddValueDTO>> AddValueService(AddValueDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 400 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 400 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemKeyTb? KeyTb = await BuildingItemKeyInfoRepository.GetKeyInfo(dto.KeyID!.Value).ConfigureAwait(false);
                if (KeyTb is null) // 기존의 KEYTB가 존재하는지 Check
                    return new ResponseModel<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };

                BuildingItemValueTb ValueTb = new BuildingItemValueTb()
                {
                    ItemValue = dto.Value!,
                    CreateDt = ThisTime,
                    CreateUser = creater,
                    UpdateDt = ThisTime,
                    UpdateUser = creater,
                    BuildingKeyTbId = dto.KeyID.Value
                };

                BuildingItemValueTb? AddValueResult = await BuildingItemValueInfoRepository.AddAsync(ValueTb).ConfigureAwait(false);
                if(AddValueResult is not null)
                {
                    return new ResponseModel<AddValueDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseModel<AddValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddValueDTO(), code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<AddValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseModel<UpdateValueDTO>> UpdateValueService(UpdateValueDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemValueTb? ItemValueTb = await BuildingItemValueInfoRepository.GetValueInfo(dto.ID!.Value).ConfigureAwait(false);
                if(ItemValueTb is null)
                    return new ResponseModel<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                ItemValueTb.ItemValue = dto.ItemValue!;
                ItemValueTb.UpdateDt = ThisTime;
                ItemValueTb.UpdateUser = creater;

                bool? UpdateValueResult = await BuildingItemValueInfoRepository.UpdateValueInfo(ItemValueTb).ConfigureAwait(false);
                return UpdateValueResult switch
                {
                    true => new ResponseModel<UpdateValueDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 },
                    false => new ResponseModel<UpdateValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseModel<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<UpdateValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseModel<bool?>> DeleteValueService(int valueid)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemValueTb? ItemValueTb = await BuildingItemValueInfoRepository.GetValueInfo(valueid).ConfigureAwait(false);
                if(ItemValueTb is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                ItemValueTb.DelDt = ThisTime;
                ItemValueTb.DelUser = creater;
                ItemValueTb.DelYn = true;

                bool? UpdateValueResult = await BuildingItemValueInfoRepository.DeleteValueInfo(ItemValueTb).ConfigureAwait(false);
                return UpdateValueResult switch
                {
                    true => new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
      
    }
}
