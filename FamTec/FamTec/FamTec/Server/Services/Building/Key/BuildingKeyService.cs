using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;
using FamTec.Shared.Server.DTO.Building.Group.Key;
using System.Linq;

namespace FamTec.Server.Services.Building.Key
{
    public class BuildingKeyService : IBuildingKeyService
    {
        private readonly IBuildingItemKeyInfoRepository BuildingItemKeyInfoRepository;
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;

        private ILogService LogService;

        public BuildingKeyService(IBuildingItemKeyInfoRepository _buildingItemkeyinforepository,
            IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            ILogService _logservice)
        {
            this.BuildingItemKeyInfoRepository = _buildingItemkeyinforepository;
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            
            
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<UpdateKeyDTO?>> UpdateKeyService(HttpContext? context, UpdateKeyDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(dto.ID);
                if(KeyTB is not null)
                {
                    KeyTB.Name = dto.Itemkey;
                    KeyTB.UpdateDt = DateTime.Now;
                    KeyTB.UpdateUser = creater;

                    bool? UpdateKeyResult = await BuildingItemKeyInfoRepository.UpdateKeyInfo(KeyTB);
                    if(UpdateKeyResult != true)
                    {
                        return new ResponseUnit<UpdateKeyDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 };
                    }

                    List<BuildingItemValueTb>? ValueTB = await BuildingItemValueInfoRepository.GetAllValueList(KeyTB.Id);
                    if(ValueTB is [_, ..])
                    {
                        if(ValueTB.Count() != dto.ValueList.Count())
                        {
                            // 개수가 다름 요청이 잘못됨
                            return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                        }
                        
                        // 혹시모를 DTO안의 ID 개수와 내용이 실제 DB의 ID 개수와 내용과 같은지
                        List<int> dtoList = dto.ValueList.Select(m => m.ID).Where(x=>x.HasValue).Select(x => x.Value).ToList();
                        dtoList.Sort();
                        List<int> dbList = ValueTB.Select(m => m.Id).ToList();
                        dbList.Sort();

                        bool equals = dtoList.SequenceEqual(dbList);
                        if (equals)
                        {
                            foreach(GroupValueListDTO value in dto.ValueList)
                            {
                                BuildingItemValueTb? valuetb = await BuildingItemValueInfoRepository.GetValueInfo(value.ID);
                                if(valuetb is not null)
                                {
                                    valuetb.ItemValue = value.ItemValue;
                                    valuetb.Unit = value.Unit;
                                    valuetb.UpdateDt = DateTime.Now;
                                    valuetb.UpdateUser = creater;

                                    bool? ValueUpdateResult = await BuildingItemValueInfoRepository.UpdateValueInfo(valuetb);
                                    if(ValueUpdateResult != true)
                                    {
                                        return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                                    }
                                }
                                else
                                {
                                    return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                                }
                            }

                            
                        }
                    }

                    return new ResponseUnit<UpdateKeyDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<UpdateKeyDTO?>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UpdateKeyDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> DeleteKeyService(HttpContext? context, int? KeyId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (KeyId is null)
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
