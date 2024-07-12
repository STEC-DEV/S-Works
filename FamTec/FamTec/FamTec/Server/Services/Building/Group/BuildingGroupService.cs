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

        public async ValueTask<ResponseUnit<AddGroupDTO?>> AddBuildingGroupService(HttpContext? context, AddGroupDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddGroupDTO?>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };

                if (dto is null)
                    return new ResponseUnit<AddGroupDTO?>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddGroupDTO?>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };

                BuildingItemGroupTb GroupTB = new BuildingItemGroupTb();
                GroupTB.Name = dto.Name; // 그룹이름
                GroupTB.CreateDt = DateTime.Now;
                GroupTB.CreateUser = creater;
                GroupTB.UpdateDt = DateTime.Now;
                GroupTB.UpdateUser = creater;
                GroupTB.BuildingTbId = dto.BuildingIdx; // 빌딩인덱스

                BuildingItemGroupTb? AddGroupTable = await BuildingGroupItemInfoRepository.AddAsync(GroupTB);
                if (AddGroupTable is not null)
                {
                    foreach (AddGroupItemKeyDTO KeyDTO in dto.AddGroupKey)
                    {
                        BuildingItemKeyTb KeyTB = new BuildingItemKeyTb();
                        KeyTB.Name = KeyDTO.Name;
                        KeyTB.CreateDt = DateTime.Now;
                        KeyTB.CreateUser = creater;
                        KeyTB.UpdateDt = DateTime.Now;
                        KeyTB.UpdateUser = creater;
                        KeyTB.BuildingGroupTbId = AddGroupTable.Id;

                        BuildingItemKeyTb? AddKeyTable = await BuildingItemKeyInfoRepository.AddAsync(KeyTB);
                        if (AddKeyTable is not null)
                        {
                            if (KeyDTO.ItemValues is [_, ..])
                            {
                                foreach (AddGroupItemValueDTO ValueDTO in KeyDTO.ItemValues)
                                {
                                    BuildingItemValueTb ValueTB = new BuildingItemValueTb();
                                    ValueTB.ItemValue = ValueDTO.Values;
                                    ValueTB.Unit = ValueDTO.Unit;
                                    ValueTB.BuildingKeyTbId = AddKeyTable.Id;

                                    BuildingItemValueTb? AddValueTable = await BuildingItemValueInfoRepository.AddAsync(ValueTB);
                                    if (AddValueTable is null)
                                    {
                                        return new ResponseUnit<AddGroupDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddGroupDTO(), code = 500 };
                                    }
                                }
                            }
                        }
                        else
                        {
                            return new ResponseUnit<AddGroupDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddGroupDTO(), code = 500 };
                        }
                    }

                    return new ResponseUnit<AddGroupDTO?>() { message = "요청이 정상 처리되었습니다", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddGroupDTO?>() { message = "잘못된 요청입니다.", data = new AddGroupDTO(), code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddGroupDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddGroupDTO(), code = 500 };
            }
        }



        // detail -- buildingid --> GroupList랑 ItemList 전체다 한번에?
        public async ValueTask<ResponseList<GroupListDTO?>> GetBuildingGroupListService(HttpContext? context, int? buildingId)
        {
            try
            {
                if (context is null)
                    return new ResponseList<GroupListDTO?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (buildingId is null)
                    return new ResponseList<GroupListDTO?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<GroupListDTO?> GroupList = new List<GroupListDTO?>(); // 그룹 [1]
                List<GroupKeyListDTO?> GroupKeyList = new List<GroupKeyListDTO?>(); // 키 [2]
                List<GroupValueListDTO?> GroupValueList = new List<GroupValueListDTO?>(); // 값 [3]


                List<BuildingItemGroupTb>? GroupListTB = await BuildingGroupItemInfoRepository.GetAllGroupList(buildingId);
                if (GroupListTB is [_, ..])
                {
                    foreach (BuildingItemGroupTb Group in GroupListTB)
                    {
                        List<BuildingItemKeyTb>? GroupKeyTB = await BuildingItemKeyInfoRepository.GetAllKeyList(Group.Id);
                        if (GroupKeyTB is [_, ..])
                        {
                            foreach (BuildingItemKeyTb Key in GroupKeyTB)
                            {
                                List<BuildingItemValueTb>? GroupValueTB = await BuildingItemValueInfoRepository.GetAllValueList(Key.Id);
                                if (GroupValueTB is [_, ..])
                                {
                                    foreach (BuildingItemValueTb Value in GroupValueTB)
                                    {
                                        GroupValueList.Add(new GroupValueListDTO
                                        {
                                            ID = Value.Id,
                                            ItemValue = Value.ItemValue,
                                            Unit = Value.Unit
                                        });
                                    }

                                    GroupKeyList.Add(new GroupKeyListDTO()
                                    {
                                        ID = Key.Id,
                                        ItemKey = Key.Name,
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

                    return new ResponseList<GroupListDTO?>() { message = "요청이 정상 처리되었습니다", data = GroupList, code = 200 };
                }
                else
                {
                    return new ResponseList<GroupListDTO?>() { message = "요청이 정상 처리되었습니다", data = GroupList, code = 200 };
                }
            }
            catch (Exception ex)
            {
                return new ResponseList<GroupListDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다", data = null, code = 500 };
            }
        }


        // update group 명칭만 변경
        public async ValueTask<ResponseUnit<bool?>> UpdateGroupNameService(HttpContext? context, UpdateGroupDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                BuildingItemGroupTb? GroupTb = await BuildingGroupItemInfoRepository.GetGroupInfo(dto.GroupId);

                if (GroupTb is not null)
                {
                    GroupTb.Name = dto.GroupName;
                    GroupTb.UpdateDt = DateTime.Now;
                    GroupTb.UpdateUser = creater;

                    bool? UpdateGroupResult = await BuildingGroupItemInfoRepository.UpdateGroupInfo(GroupTb);
                    if (UpdateGroupResult == true)
                    {
                        return new ResponseUnit<bool?>() { message = "수정이 완료되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 처리되지 않았습니다.", data = false, code = 500 };
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
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }

        }

        // delete group  = group - itemkey - itemvalue 삭제

        public async ValueTask<ResponseUnit<bool?>> DeleteGroupService(HttpContext? context, int? groupid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (groupid is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                BuildingItemGroupTb? GroupTb = await BuildingGroupItemInfoRepository.GetGroupInfo(groupid);

                if (GroupTb is not null)
                {
                    GroupTb.DelDt = DateTime.Now;
                    GroupTb.DelUser = creater;
                    GroupTb.DelYn = true;

                    bool? DeleteGroupResult = await BuildingGroupItemInfoRepository.DeleteGroupInfo(GroupTb);

                    if (DeleteGroupResult != true)
                        return new ResponseUnit<bool?>() { message = "요청이 처리되지 않았습니다.", data = false, code = 500 };

                    List<BuildingItemKeyTb>? KeyTb = await BuildingItemKeyInfoRepository.GetAllKeyList(groupid);
                    if (KeyTb is [_, ..])
                    {
                        foreach (BuildingItemKeyTb KeyModel in KeyTb)
                        {
                            KeyModel.DelDt = DateTime.Now;
                            KeyModel.DelUser = creater;
                            KeyModel.DelYn = true;

                            bool? DeleteKeyResult = await BuildingItemKeyInfoRepository.DeleteKeyInfo(KeyModel);

                            if (DeleteKeyResult != true)
                            {
                                return new ResponseUnit<bool?>() { message = "요청이 처리되지 않았습니다.", data = false, code = 500 };
                            }

                            List<BuildingItemValueTb>? ValueTb = await BuildingItemValueInfoRepository.GetAllValueList(KeyModel.Id);
                            if (ValueTb is [_, ..])
                            {
                                foreach (BuildingItemValueTb ValueModel in ValueTb)
                                {
                                    ValueModel.DelDt = DateTime.Now;
                                    ValueModel.DelUser = creater;
                                    ValueModel.DelYn = true;

                                    bool? DeleteValueResult = await BuildingItemValueInfoRepository.DeleteValueInfo(ValueModel);
                                    if (DeleteValueResult != true)
                                    {
                                        return new ResponseUnit<bool?>() { message = "요청이 처리되지 않았습니다.", data = false, code = 500 };
                                    }
                                }
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
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }


    }
}
