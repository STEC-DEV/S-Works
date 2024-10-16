using FamTec.Server.Repository.Building.SubItem.Group;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;
using FamTec.Shared.Server.DTO.Building.Group;

namespace FamTec.Server.Services.Building.Group
{
    public class BuildingGroupService : IBuildingGroupService
    {
        private readonly IBuildingGroupItemInfoRepository BuildingGroupItemInfoRepository;
        private readonly IBuildingItemKeyInfoRepository BuildingItemKeyInfoRepository;
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;

        private ILogService LogService;

        public BuildingGroupService(IBuildingGroupItemInfoRepository _buildinggroupiteminforepository,
            IBuildingItemKeyInfoRepository _buildingitemkeyinforepository,
            IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            ILogService _logservice)
        {
            this.BuildingGroupItemInfoRepository = _buildinggroupiteminforepository;
            this.BuildingItemKeyInfoRepository = _buildingitemkeyinforepository;
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;

            this.LogService = _logservice;
        }

        /// <summary>
        /// 그룹 - 키 - 값 추가서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool>> AddBuildingGroupService(HttpContext context, List<AddGroupDTO> dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                
                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };


                int Result = await BuildingGroupItemInfoRepository.AddGroupAsync(dto, creater, Convert.ToInt32(placeid));
                if(Result == 1)
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다", data = true, code = 200 };
                else if(Result == -1)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                else if(Result == 0)
                    return new ResponseUnit<bool>() { message = "이미 처리된 작업이 존재합니다.", data = false, code = 201 };
                else
                    return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 그룹 만 추가 서비스
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddGroupInfoDTO>> AddBuildingGroupInfoService(HttpContext? context, AddGroupInfoDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemGroupTb GroupTb = new BuildingItemGroupTb()
                {
                    Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!,
                    CreateDt = ThisTime,
                    CreateUser = creater,
                    UpdateDt = ThisTime,
                    UpdateUser = creater,
                    BuildingTbId = dto.BuildingIdx!.Value
                };

                BuildingItemGroupTb? AddResult = await BuildingGroupItemInfoRepository.AddAsync(GroupTb).ConfigureAwait(false);
                if (AddResult is not null)
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<AddGroupInfoDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddGroupInfoDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// detail -- buildingid --> GroupList랑 ItemList 전체다 한번에 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async Task<ResponseList<GroupListDTO?>> GetBuildingGroupListService(HttpContext context, int buildingId)
        {
            try
            {
                // Validate the context object
                if (context is null)
                    return new ResponseList<GroupListDTO?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                // Initialize lists for groups, keys, and values
                List<GroupListDTO?> groupList = new List<GroupListDTO?>(); // 그룹 [1]

                // Fetch all groups for the specified building ID
                List<BuildingItemGroupTb>? groupListTB = await BuildingGroupItemInfoRepository.GetAllGroupList(buildingId).ConfigureAwait(false);

                if (groupListTB is not null && groupListTB.Any())
                {
                    foreach (BuildingItemGroupTb group in groupListTB)
                    {
                        List<GroupKeyListDTO?> groupKeyList = new List<GroupKeyListDTO?>(); // 키 [2]

                        // Fetch all keys for each group
                        List<BuildingItemKeyTb>? groupKeyTB = await BuildingItemKeyInfoRepository.GetAllKeyList(group.Id).ConfigureAwait(false);
                        if (groupKeyTB is not null && groupKeyTB.Any())
                        {
                            foreach (BuildingItemKeyTb key in groupKeyTB)
                            {
                                List<GroupValueListDTO> groupValueList = new List<GroupValueListDTO>(); // 값 [3]

                                // Fetch all values for each key
                                List<BuildingItemValueTb>? groupValueTB = await BuildingItemValueInfoRepository.GetAllValueList(key.Id).ConfigureAwait(false);
                                if (groupValueTB is not null && groupValueTB.Any())
                                {
                                    groupValueList = groupValueTB.Select(value => new GroupValueListDTO
                                    {
                                        ID = value.Id,
                                        ItemValue = value.ItemValue
                                    }).ToList();
                                }

                                groupKeyList.Add(new GroupKeyListDTO
                                {
                                    ID = key.Id,
                                    ItemKey = key.Name,
                                    Unit = key.Unit,
                                    ValueList = groupValueList
                                });
                            }
                        }

                        groupList.Add(new GroupListDTO
                        {
                            ID = group.Id,
                            Name = group.Name,
                            KeyListDTO = groupKeyList!
                        });
                    }

                    return new ResponseList<GroupListDTO?>() { message = "요청이 정상 처리되었습니다.", data = groupList, code = 200 };
                }
                else
                {
                    return new ResponseList<GroupListDTO?>() { message = "요청이 정상 처리되었습니다.", data = groupList, code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<GroupListDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// update group 명칭만 변경
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<bool?>> UpdateGroupNameService(HttpContext context, UpdateGroupDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemGroupTb? GroupTb = await BuildingGroupItemInfoRepository.GetGroupInfo(dto.GroupId!.Value).ConfigureAwait(false);
                if (GroupTb is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                GroupTb.Name = !String.IsNullOrWhiteSpace(dto.GroupName) ? dto.GroupName.Trim() : dto.GroupName!;
                GroupTb.UpdateDt = ThisTime;
                GroupTb.UpdateUser = creater;

                bool? UpdateGroupResult = await BuildingGroupItemInfoRepository.UpdateGroupInfo(GroupTb).ConfigureAwait(false);
                return UpdateGroupResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "수정이 완료되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// delete group  = group - itemkey - itemvalue 삭제
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

                bool? DeleteResult = await BuildingGroupItemInfoRepository.DeleteGroupInfo(groupid, creater).ConfigureAwait(false);

                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        
    }
}
