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

        private ILogService LogService;

        public BuildingValueService(IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            IBuildingItemKeyInfoRepository _buildingitemkeyinforepository,
            ILogService _logservice)
        {
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            this.BuildingItemKeyInfoRepository = _buildingitemkeyinforepository;
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<AddValueDTO>> AddValueService(HttpContext context, AddValueDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };

                BuildingItemKeyTb? KeyTb = await BuildingItemKeyInfoRepository.GetKeyInfo(dto.KeyID!.Value);
                if (KeyTb is null) // 기존의 KEYTB가 존재하는지 Check
                    return new ResponseUnit<AddValueDTO>() { message = "잘못된 요청입니다.", data = new AddValueDTO(), code = 404 };

                BuildingItemValueTb ValueTb = new BuildingItemValueTb();
                ValueTb.ItemValue = dto.Value!;
                ValueTb.CreateDt = DateTime.Now;
                ValueTb.CreateUser = creater;
                ValueTb.UpdateDt = DateTime.Now;
                ValueTb.UpdateUser = creater;
                ValueTb.BuildingKeyTbId = dto.KeyID.Value;

                BuildingItemValueTb? AddValueResult = await BuildingItemValueInfoRepository.AddAsync(ValueTb);
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
                return new ResponseUnit<AddValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<UpdateValueDTO>> UpdateValueService(HttpContext context, UpdateValueDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                BuildingItemValueTb? ItemValueTb = await BuildingItemValueInfoRepository.GetValueInfo(dto.ID.Value);
                if(ItemValueTb is not null)
                {
                    ItemValueTb.ItemValue = dto.ItemValue!;
                    ItemValueTb.UpdateDt = DateTime.Now;
                    ItemValueTb.UpdateUser = creater;

                    bool? UpdateValueResult = await BuildingItemValueInfoRepository.UpdateValueInfo(ItemValueTb);
                    if(UpdateValueResult == true)
                    {
                        return new ResponseUnit<UpdateValueDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<UpdateValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<UpdateValueDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UpdateValueDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> DeleteValueService(HttpContext context, int valueid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                BuildingItemValueTb? ItemValueTb = await BuildingItemValueInfoRepository.GetValueInfo(valueid);
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
