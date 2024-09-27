using FamTec.Server.Repository.Facility.Group;
using FamTec.Server.Repository.Facility.ItemKey;
using FamTec.Server.Repository.Facility.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FamTec.Server.Services.Facility.Key
{
    public class FacilityKeyService : IFacilityKeyService
    {
        private readonly IFacilityGroupItemInfoRepository FacilityGroupItemInfoRepository;
        private readonly IFacilityItemKeyInfoRepository FacilityItemKeyInfoRepository;
        private readonly IFacilityItemValueInfoRepository FacilityItemValueInfoRepository;

        private ILogService LogService;

        public FacilityKeyService(
            IFacilityGroupItemInfoRepository _facilitygroupiteminforepository,
            IFacilityItemKeyInfoRepository _facilityitemkeyinforepository,
            IFacilityItemValueInfoRepository _facilityitemvalueinforepository,
            ILogService _logservice)
        {
            this.FacilityGroupItemInfoRepository = _facilitygroupiteminforepository;
            this.FacilityItemKeyInfoRepository = _facilityitemkeyinforepository;
            this.FacilityItemValueInfoRepository = _facilityitemvalueinforepository;

            this.LogService = _logservice;
        }


        public async ValueTask<ResponseUnit<AddKeyDTO>> AddKeyService(HttpContext context, AddKeyDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                DateTime ThisTime = DateTime.Now;

                FacilityItemGroupTb? GroupTb = await FacilityGroupItemInfoRepository.GetGroupInfo(dto.GroupID.Value).ConfigureAwait(false);
                if(GroupTb is null) // 기존의 GroupTB가 존재하는지 Check
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                FacilityItemKeyTb KeyTb = new FacilityItemKeyTb();
                KeyTb.Name = dto.Name!.Trim()!; // 키 명칭
                KeyTb.Unit = dto.Unit!.Trim(); // 단위
                KeyTb.CreateDt = ThisTime;
                KeyTb.CreateUser = creater;
                KeyTb.UpdateDt = ThisTime;
                KeyTb.UpdateUser = creater;
                KeyTb.FacilityItemGroupTbId = dto.GroupID.Value;

                FacilityItemKeyTb? AddKeyResult = await FacilityItemKeyInfoRepository.AddAsync(KeyTb).ConfigureAwait(false);
                if(AddKeyResult is null)
                    return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };

                if(dto.ItemValues is [_, ..])
                {
                    foreach(AddGroupItemValueDTO GroupDTO in dto.ItemValues)
                    {
                        FacilityItemValueTb ValueTB = new FacilityItemValueTb();
                        ValueTB.ItemValue = GroupDTO.Values!;
                        ValueTB.CreateDt = ThisTime;
                        ValueTB.CreateUser = creater;
                        ValueTB.UpdateDt = ThisTime;
                        ValueTB.UpdateUser = creater;
                        ValueTB.FacilityItemKeyTbId = AddKeyResult.Id;

                        FacilityItemValueTb? result = await FacilityItemValueInfoRepository.AddAsync(ValueTB).ConfigureAwait(false);
                        if(result is null)
                        {
                            return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
                        }
                    }
                }

                return new ResponseUnit<AddKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<UpdateKeyDTO>> UpdateKeyService(HttpContext context, UpdateKeyDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                FacilityItemKeyTb? KeyTB = await FacilityItemKeyInfoRepository.GetKeyInfo(dto.ID!.Value).ConfigureAwait(false);
                if(KeyTB is null)
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                
                bool? UpdateResult = await FacilityItemKeyInfoRepository.UpdateKeyInfo(dto, creater).ConfigureAwait(false);
                return UpdateResult switch
                {
                    true => new ResponseUnit<UpdateKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 },
                    false => new ResponseUnit<UpdateKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 },
                    _ => new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 }
                };
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

                DateTime ThisTime = DateTime.Now;

                FacilityItemKeyTb? KeyTB = await FacilityItemKeyInfoRepository.GetKeyInfo(KeyId).ConfigureAwait(false);
                if(KeyTB is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                KeyTB.DelDt = ThisTime;
                KeyTB.DelUser = creater;
                KeyTB.DelYn = true;

                bool? DeleteKeyResult = await FacilityItemKeyInfoRepository.DeleteKeyInfo(KeyTB).ConfigureAwait(false);

                if(DeleteKeyResult != true)
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }

                List<FacilityItemValueTb>? ItemTB = await FacilityItemValueInfoRepository.GetAllValueList(KeyId).ConfigureAwait(false);
                if(ItemTB is [_, ..])
                {
                    foreach(FacilityItemValueTb Item in ItemTB)
                    {
                        Item.DelDt = ThisTime;
                        Item.DelUser = creater;
                        Item.DelYn = true;

                        bool? DeleteValueResult = await FacilityItemValueInfoRepository.DeleteValueInfo(Item).ConfigureAwait(false);
                        if(DeleteValueResult != true)
                        {
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }
                    }
                }
                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 키 List - Value 삭제 리스트 묶음 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="KeyId"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeletKeyListService(HttpContext? context, List<int> KeyId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? DeleteResult = await FacilityItemKeyInfoRepository.DeleteKeyList(KeyId, creater).ConfigureAwait(false);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
