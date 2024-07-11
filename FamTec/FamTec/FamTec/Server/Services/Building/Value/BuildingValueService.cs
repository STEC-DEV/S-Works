using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group.Key.Value;

namespace FamTec.Server.Services.Building.Value
{
    public class BuildingValueService : IBuildingValueService
    {
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;

        private ILogService LogService;

        public BuildingValueService(IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            ILogService _logservice)
        {
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<UpdateValueDTO?>> UpdateValueService(HttpContext? context, UpdateValueDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UpdateValueDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<UpdateValueDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateValueDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                BuildingItemvalueTb? ItemValueTb = await BuildingItemValueInfoRepository.GetValueInfo(dto.ID);
                if(ItemValueTb is not null)
                {
                    ItemValueTb.Itemvalue = dto.ItemValue;
                    ItemValueTb.Unit = dto.Unit;
                    ItemValueTb.UpdateDt = DateTime.Now;
                    ItemValueTb.UpdateUser = creater;

                    bool? UpdateValueResult = await BuildingItemValueInfoRepository.UpdateValueInfo(ItemValueTb);
                    if(UpdateValueResult == true)
                    {
                        return new ResponseUnit<UpdateValueDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<UpdateValueDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<UpdateValueDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UpdateValueDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> DeleteValueService(HttpContext? context, int? valueid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (valueid is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                BuildingItemvalueTb? ItemValueTb = await BuildingItemValueInfoRepository.GetValueInfo(valueid);
                if (ItemValueTb is not null)
                {
                    ItemValueTb.DelDt = DateTime.Now;
                    ItemValueTb.DelUser = creater;
                    ItemValueTb.DelYn = true;

                    bool? UpdateValueResult = await BuildingItemValueInfoRepository.DeleteValueInfo(ItemValueTb);
                    if (UpdateValueResult == true)
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

      
    }
}
