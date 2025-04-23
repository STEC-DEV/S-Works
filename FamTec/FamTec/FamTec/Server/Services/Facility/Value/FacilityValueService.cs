using FamTec.Server.Repository.Facility.ItemKey;
using FamTec.Server.Repository.Facility.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Value
{
    public class FacilityValueService : IFacilityValueService
    {
        private readonly IFacilityItemKeyInfoRepository FacilityItemKeyInfoRepository;
        private readonly IFacilityItemValueInfoRepository FacilityItemValueInfoRepository;
        private readonly ILogService LogService;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly ConsoleLogService<FacilityValueService> CreateBuilderLogger;

        public FacilityValueService(IFacilityItemKeyInfoRepository _facilityitemkeyinforepository,
            IFacilityItemValueInfoRepository _facilityitemvalueinforepository,
            ILogService _logservice,
            IHttpContextAccessor _httpcontextaccessor,
            ConsoleLogService<FacilityValueService> _createbuilderlogger)
        {
            this.FacilityItemKeyInfoRepository = _facilityitemkeyinforepository;
            this.FacilityItemValueInfoRepository = _facilityitemvalueinforepository;
            this.LogService = _logservice;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.CreateBuilderLogger = _createbuilderlogger;            
        }

        public async Task<ResponseUnit<AddValueDTO>> AddValueService(AddValueDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };

                DateTime ThisDate = DateTime.Now;

                FacilityItemKeyTb? KeyTb = await FacilityItemKeyInfoRepository.GetKeyInfo(dto.KeyID!.Value).ConfigureAwait(false);
                if (KeyTb is null)
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };

                FacilityItemValueTb ValueTb = new FacilityItemValueTb();
                ValueTb.ItemValue = dto.Value!;
                ValueTb.CreateDt = ThisDate;
                ValueTb.CreateUser = creater;
                ValueTb.UpdateDt = ThisDate;
                ValueTb.UpdateUser = creater;
                ValueTb.FacilityItemKeyTbId = dto.KeyID.Value;

                FacilityItemValueTb? AddValueResult = await FacilityItemValueInfoRepository.AddAsync(ValueTb).ConfigureAwait(false);
                if(AddValueResult is not null)
                {
                    return new ResponseUnit<AddValueDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddValueDTO(), code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<AddValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddValueDTO(), code = 500 };
            }
        }

        public async Task<ResponseUnit<UpdateValueDTO>> UpdateValueService(UpdateValueDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다", data = null, code = 404 };

                DateTime ThisDate = DateTime.Now;

                FacilityItemValueTb? ItemValueTb = await FacilityItemValueInfoRepository.GetValueInfo(dto.ID!.Value).ConfigureAwait(false);
                if(ItemValueTb is null)
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                ItemValueTb.ItemValue = dto.ItemValue!;
                ItemValueTb.UpdateDt = ThisDate;
                ItemValueTb.UpdateUser = creater;

                bool? UpdateValueResult = await FacilityItemValueInfoRepository.UpdateValueInfo(ItemValueTb).ConfigureAwait(false);
                return UpdateValueResult switch
                {
                    true => new ResponseUnit<UpdateValueDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 },
                    false => new ResponseUnit<UpdateValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseUnit<UpdateValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseUnit<bool?>> DeleteValueService(int valueid)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisDate = DateTime.Now;

                FacilityItemValueTb? ItemValueTb = await FacilityItemValueInfoRepository.GetValueInfo(valueid).ConfigureAwait(false);
                if(ItemValueTb is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                ItemValueTb.DelDt = ThisDate;
                ItemValueTb.DelUser = creater;
                ItemValueTb.DelYn = true;

                bool? UpdateValueResult = await FacilityItemValueInfoRepository.DeleteValueInfo(ItemValueTb).ConfigureAwait(false);
                return UpdateValueResult switch
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
