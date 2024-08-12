using FamTec.Server.Repository.Building.SubItem.Group;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;
using FamTec.Shared.Server.DTO.Building.Group;
using FamTec.Shared.Server.DTO.Building.Group.Key;
using Microsoft.JSInterop.Infrastructure;

namespace FamTec.Server.Services.Building.Key
{
    public class BuildingKeyService : IBuildingKeyService
    {
        private readonly IBuildingGroupItemInfoRepository BuildingGroupItemInfoRepository;
        private readonly IBuildingItemKeyInfoRepository BuildingItemKeyInfoRepository;
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;

        private ILogService LogService;

        public BuildingKeyService(
            IBuildingGroupItemInfoRepository _buildinggroupiteminforepository,
            IBuildingItemKeyInfoRepository _buildingItemkeyinforepository,
            IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            ILogService _logservice)
        {
            this.BuildingGroupItemInfoRepository = _buildinggroupiteminforepository;
            this.BuildingItemKeyInfoRepository = _buildingItemkeyinforepository;
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            
            
            this.LogService = _logservice;
        }

        /// <summary>
        /// 키 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddKeyDTO>> AddKeyService(HttpContext context, AddKeyDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                BuildingItemGroupTb? GroupTb = await BuildingGroupItemInfoRepository.GetGroupInfo(dto.GroupID!.Value);
                if(GroupTb is null) // 기존의 GroupTB 이 존재하는지 Check
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                BuildingItemKeyTb KeyTb = new BuildingItemKeyTb();
                KeyTb.Name = dto.Name!; // 키명칭
                KeyTb.Unit = dto.Unit!; // 단위
                KeyTb.CreateDt = DateTime.Now;
                KeyTb.CreateUser = creater;
                KeyTb.UpdateDt = DateTime.Now;
                KeyTb.UpdateUser = creater;
                KeyTb.BuildingGroupTbId = dto.GroupID.Value;

                BuildingItemKeyTb? AddkeyResult = await BuildingItemKeyInfoRepository.AddAsync(KeyTb);
                if(AddkeyResult is not null)
                {
                    if (dto.ItemValues is [_, ..])
                    {
                        foreach (AddGroupItemValueDTO GroupDTO in dto.ItemValues)
                        {
                            BuildingItemValueTb ValueTB = new BuildingItemValueTb();
                            ValueTB.ItemValue = GroupDTO.Values!;
                            ValueTB.CreateDt = DateTime.Now;
                            ValueTB.CreateUser = creater;
                            ValueTB.UpdateDt = DateTime.Now;
                            ValueTB.UpdateUser = creater;
                            ValueTB.BuildingKeyTbId = AddkeyResult.Id;

                            BuildingItemValueTb? result = await BuildingItemValueInfoRepository.AddAsync(ValueTB);
                            if(result is null)
                            {
                                return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
                            }
                        }
                    }

                    return new ResponseUnit<AddKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UpdateKeyDTO>> UpdateKeyService(HttpContext context, UpdateKeyDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                
                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(dto.ID!.Value);
                if(KeyTB is not null)
                {
                    bool? UpdateResult = await BuildingItemKeyInfoRepository.UpdateKeyInfo(dto, creater);
                    if(UpdateResult == true)
                    {
                        return new ResponseUnit<UpdateKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else if(UpdateResult == false)
                    {
                        return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                    }
                    else
                    {
                        return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UpdateKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> DeleteKeyService(HttpContext context, int KeyId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(KeyId);

                if(KeyTB is not null)
                {
                    KeyTB.DelDt = DateTime.Now;
                    KeyTB.DelUser = creater;
                    KeyTB.DelYn = true;

                    bool? DeleteKeyResult = await BuildingItemKeyInfoRepository.DeleteKeyInfo(KeyTB);

                    if(DeleteKeyResult != true)
                    {
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }

                    List<BuildingItemValueTb>? ItemTB = await BuildingItemValueInfoRepository.GetAllValueList(KeyId);
                    if(ItemTB is [_, ..])
                    {
                        foreach(BuildingItemValueTb Item in ItemTB)
                        {
                            Item.DelDt = DateTime.Now;
                            Item.DelUser = creater;
                            Item.DelYn = true;

                            bool? DeleteValueResult = await BuildingItemValueInfoRepository.DeleteValueInfo(Item);
                            if(DeleteValueResult != true)
                            {
                                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                            }
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
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
