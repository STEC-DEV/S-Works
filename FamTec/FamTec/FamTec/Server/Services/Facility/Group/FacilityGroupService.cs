using FamTec.Server.Repository.Facility.Group;
using FamTec.Server.Repository.Facility.ItemKey;
using FamTec.Server.Repository.Facility.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility.Group;

namespace FamTec.Server.Services.Facility.Group
{
    public class FacilityGroupService : IFacilityGroupService
    {
        private readonly IFacilityGroupItemInfoRepository FacilityGroupItemInfoRepository;
        private readonly IFacilityItemKeyInfoRepository FacilityItemKeyInfoRepository;
        private readonly IFacilityItemValueInfoRepository FacilityItemValueInfoRepository;

        private ILogService LogService;

        public FacilityGroupService(IFacilityGroupItemInfoRepository _facilitygroupiteminforepository,
            IFacilityItemKeyInfoRepository _facilityitemkeyinforepository,
            IFacilityItemValueInfoRepository _facilityitemvalueinforepository,
            ILogService _logservice)
        {
            FacilityGroupItemInfoRepository = _facilitygroupiteminforepository;
            FacilityItemKeyInfoRepository = _facilityitemkeyinforepository;
            FacilityItemValueInfoRepository = _facilityitemvalueinforepository;

            LogService = _logservice;
        }

        /// <summary>
        /// 그룹 - 키 - 값 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddGroupDTO>> AddFacilityGroupService(HttpContext context, AddGroupDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddGroupDTO>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (string.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddGroupDTO>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };

                DateTime ThisTime = DateTime.Now;

                FacilityItemGroupTb GroupTB = new FacilityItemGroupTb()
                {
                    Name = dto.Name!, // 그룹이름
                    CreateDt = ThisTime,
                    CreateUser = creater,
                    UpdateDt = ThisTime,
                    UpdateUser = creater,
                    FacilityTbId = dto.FacilityIdx!.Value // 설비 인덱스
                };

                FacilityItemGroupTb? AddGroupTable = await FacilityGroupItemInfoRepository.AddAsync(GroupTB).ConfigureAwait(false);
                if(AddGroupTable is null)
                    return new ResponseUnit<AddGroupDTO>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };


                if (dto.AddGroupKey is not { Count: > 0 })
                {
                    return new ResponseUnit<AddGroupDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }

                foreach (AddGroupItemKeyDTO KeyDTO in dto.AddGroupKey)
                {
                    FacilityItemKeyTb KeyTB = new FacilityItemKeyTb();
                    KeyTB.Name = KeyDTO.Name!; // 키의 명칭
                    KeyTB.Unit = KeyDTO.Unit!; // 단위
                    KeyTB.CreateDt = ThisTime;
                    KeyTB.CreateUser = creater;
                    KeyTB.UpdateDt = ThisTime;
                    KeyTB.UpdateUser = creater;
                    KeyTB.FacilityItemGroupTbId = AddGroupTable.Id;

                    FacilityItemKeyTb? AddKeyTable = await FacilityItemKeyInfoRepository.AddAsync(KeyTB).ConfigureAwait(false);
                    if(AddKeyTable is null)
                        return new ResponseUnit<AddGroupDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddGroupDTO(), code = 404 };

                    if (KeyDTO.ItemValues is [_, ..])
                    {
                        foreach (AddGroupItemValueDTO ValueDTO in KeyDTO.ItemValues)
                        {
                            FacilityItemValueTb ValueTB = new FacilityItemValueTb();
                            ValueTB.ItemValue = ValueDTO.Values!;
                            ValueTB.CreateDt = ThisTime;
                            ValueTB.CreateUser = creater;
                            ValueTB.UpdateDt = ThisTime;
                            ValueTB.UpdateUser = creater;
                            ValueTB.FacilityItemKeyTbId = AddKeyTable.Id;

                            FacilityItemValueTb? AddValueTable = await FacilityItemValueInfoRepository.AddAsync(ValueTB).ConfigureAwait(false);
                            if (AddValueTable is null)
                            {
                                return new ResponseUnit<AddGroupDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddGroupDTO(), code = 404 };
                            }
                        }
                    }
                }

                return new ResponseUnit<AddGroupDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddGroupDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 그룹 만 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddGroupInfoDTO>> AddFacilityGroupInfoService(HttpContext context, AddGroupInfoDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                FacilityItemGroupTb GroupTb = new FacilityItemGroupTb()
                {
                    Name = dto.Name!,
                    CreateDt = ThisTime,
                    CreateUser = creater,
                    UpdateDt = ThisTime,
                    UpdateUser = creater,
                    FacilityTbId = dto.FacilityIdx!.Value
                };

                FacilityItemGroupTb? AddResult = await FacilityGroupItemInfoRepository.AddAsync(GroupTb).ConfigureAwait(false);
                if(AddResult is not null)
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddGroupInfoDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseList<GroupListDTO>> GetFacilityGroupListService(HttpContext context, int facilityid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<GroupListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                

                List<GroupListDTO?> GroupList = new List<GroupListDTO?>(); // 그룹 [1]
                List<GroupKeyListDTO?> GroupKeyList = new List<GroupKeyListDTO?>(); // 그룹 [2]
                List<GroupValueListDTO?> GroupValueList = new List<GroupValueListDTO?>(); // 그룹 [3]


                List<FacilityItemGroupTb>? GroupListTB = await FacilityGroupItemInfoRepository.GetAllGroupList(facilityid).ConfigureAwait(false);
                if (GroupListTB is [_, ..])
                {
                    foreach (FacilityItemGroupTb Group in GroupListTB)
                    {
                        List<FacilityItemKeyTb>? GroupKeyTB = await FacilityItemKeyInfoRepository.GetAllKeyList(Group.Id).ConfigureAwait(false);
                        if (GroupKeyTB is [_, ..])
                        {
                            foreach (FacilityItemKeyTb Key in GroupKeyTB)
                            {
                                List<FacilityItemValueTb>? GroupValueTB = await FacilityItemValueInfoRepository.GetAllValueList(Key.Id).ConfigureAwait(false);

                                if (GroupValueTB is [_, ..])
                                {
                                    foreach (FacilityItemValueTb Value in GroupValueTB)
                                    {
                                        GroupValueList.Add(new GroupValueListDTO()
                                        {
                                            ID = Value.Id,
                                            ItemValue = Value.ItemValue,
                                        });
                                    }

                                    GroupKeyList.Add(new GroupKeyListDTO()
                                    {
                                        ID = Key.Id,
                                        ItemKey = Key.Name,
                                        Unit = Key.Unit,
                                        ValueList = GroupValueList
                                    });
                                    GroupValueList = new List<GroupValueListDTO?>();
                                }
                            }
                        }

                        GroupList.Add(new GroupListDTO()
                        {
                            ID = Group.Id,
                            Name = Group.Name,
                            KeyListDTO = GroupKeyList
                        });
                        GroupKeyList = new List<GroupKeyListDTO?>();
                    }

                    return new ResponseList<GroupListDTO>() { message = "요청이 정상 처리되었습니다.", data = GroupList, code = 200 };
                }
                else
                {
                    return new ResponseList<GroupListDTO>() { message = "요청이 정상 처리되었습니다", data = GroupList, code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<GroupListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<GroupListDTO?>(), code = 500 };
            }
        }

        public async Task<ResponseUnit<bool?>> UpdateGroupNameService(HttpContext context, UpdateGroupDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (string.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                FacilityItemGroupTb? GroupTb = await FacilityGroupItemInfoRepository.GetGroupInfo(dto.GroupId.Value).ConfigureAwait(false);

                if (GroupTb is not null)
                {
                    GroupTb.Name = dto.GroupName!;
                    GroupTb.UpdateDt = ThisTime;
                    GroupTb.UpdateUser = creater;

                    bool? UpdateGroupResult = await FacilityGroupItemInfoRepository.UpdateGroupInfo(GroupTb).ConfigureAwait(false);
                    if (UpdateGroupResult == true)
                    {
                        return new ResponseUnit<bool?>() { message = "수정이 완료되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = true, code = 500 };
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

        /// <summary>
        /// 설비 그룹 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> DeleteGroupService(HttpContext context, int groupid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                bool? DeleteResult = await FacilityGroupItemInfoRepository.DeleteGroupInfo(groupid, creater).ConfigureAwait(false);
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
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
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
