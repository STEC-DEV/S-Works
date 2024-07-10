using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Building.SubItem.Group;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Server.Repository.Floor;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building;

namespace FamTec.Server.Services.Building
{
    public class BuildingService : IBuildingService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IGroupItemInfoRepository GroupItemInfoRepository;
        private readonly IItemKeyInfoRepository ItemKeyInfoRepository;
        private readonly IItemValueInfoRepository ItemValueInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepsitory;

        private ILogService LogService;

        // 파일디렉토리
        private DirectoryInfo? di;
        private string PlaceFileFolderPath;

      

        public BuildingService(
            IBuildingInfoRepository _buildinginforepository,
            IGroupItemInfoRepository _groupiteminforepository,
            IItemKeyInfoRepository _itemkeyinforepository,
            IItemValueInfoRepository _itemvalueinforepository,
            IFloorInfoRepository _floorinforepository,
            ILogService _logservice)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.GroupItemInfoRepository = _groupiteminforepository;

            this.ItemKeyInfoRepository = _itemkeyinforepository;
            this.ItemValueInfoRepository = _itemvalueinforepository;

            this.FloorInfoRepsitory = _floorinforepository;

            this.LogService = _logservice;
        }

        /// <summary>
        /// 해당 사업장에 건물추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="placeidx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> AddBuildingService(HttpContext? context, AddBuildingDTO? dto, IFormFile? files)
        {
            try
            {
                string FileName = String.Empty;
                string FileExtenstion = String.Empty;

                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                if (dto is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };


               // 건물 관련한 폴더 없으면 만들기
                PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeidx.ToString()); // 사업장

               di = new DirectoryInfo(PlaceFileFolderPath);
               if (!di.Exists) di.Create();


                BuildingTb? model = new BuildingTb();
                model.BuildingCd = dto.BuildingCD;
                model.Name = dto.Name;
                model.Address = dto.Address;
                model.Tel = dto.Tel;
                model.Usage = dto.Usage;
                model.ConstComp = dto.ConstComp;
                model.CompletionDt = dto.CompletionDT;
                model.BuildingStruct = dto.BuildingStruct;
                model.RoofStruct = dto.RoofStruct;

              
                model.CreateDt = DateTime.Now;
                model.CreateUser = Creater;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = Creater;
                model.PlaceTbId = Int32.Parse(placeidx);

                if (files is not null)
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);

                    if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<bool>() { message = "올바르지 않은 파일형식입니다.", data = false, code = 404 };
                    }
                    else
                    {
                        // 이미지경로
                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}"; // 암호화된 파일명

                        string? newFilePath = Path.Combine(PlaceFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await files.CopyToAsync(fileStream);
                            model.Image = newFileName;
                        }
                    }
                }
                else
                {
                    model.Image = null;
                }

                BuildingTb? buildingtb = await BuildingInfoRepository.AddAsync(model);
                
                if(buildingtb is null)
                    return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
                
                if(dto.SubItem is not null)
                {
                    foreach(var item in dto.SubItem)
                    {
                        AddGroupDTO? groupdto = item;

                        if (groupdto is not null)
                        {
                            // GROUP 추가
                            GroupitemTb group = new GroupitemTb();
                            group.Name = groupdto.Name!;
                            group.Type = 0; // 건물
                            group.CreateDt = DateTime.Now;
                            group.CreateUser = Creater;
                            group.UpdateDt = DateTime.Now;
                            group.UpdateUser = Creater;
                            group.BuildingId = buildingtb.Id;

                            GroupitemTb? GroupItemResult = await GroupItemInfoRepository.AddAsync(group);
                            if(GroupItemResult is null)
                                return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };

                            // KEY 추가
                            foreach (var itemkey in groupdto.AddGroupKey)
                            {
                                ItemkeyTb key = new ItemkeyTb();
                                key.Itemkey = itemkey.Name!;
                                key.CreateDt = DateTime.Now;
                                key.CreateUser = Creater;
                                key.UpdateDt = DateTime.Now;
                                key.UpdateUser = Creater;
                                key.GroupItemId = GroupItemResult.Id;
                                
                                ItemkeyTb? ItemKeyResult = await ItemKeyInfoRepository.AddAsync(key);
                                
                                if (ItemKeyResult is null)
                                    return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };

                                foreach (var itemValue in itemkey.ItemValues!)
                                {
                                    ItemvalueTb value = new ItemvalueTb();
                                    value.Itemvalue = itemValue.Values!;
                                    value.Unit = itemValue.Unit;
                                    value.CreateDt = DateTime.Now;
                                    value.CreateUser = Creater;
                                    value.UpdateDt = DateTime.Now;
                                    value.UpdateUser = Creater;
                                    value.ItemKeyId = ItemKeyResult.Id;

                                    ItemvalueTb? result = await ItemValueInfoRepository.AddAsync(value);
                                    if (result == null)
                                        return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = false, code = 404 };
                                };

                            }

                        }

                    }
                    return new ResponseUnit<bool>() { message = "요청이 정상처리되었습니다.", data = true, code = 200 };
                }
                return new ResponseUnit<bool>() { message = "요청이 정상처리되었습니다.", data = true, code = 200 };

                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 등록되어있는 건물리스트 출력
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<BuildinglistDTO>> GetBuilidngListService(HttpContext? context)
        {
            try
            {
                if(context is null)
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = new List<BuildinglistDTO>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 잘못되었습니다.", data = new List<BuildinglistDTO>(), code = 404 };

                List<BuildingTb>? model = await BuildingInfoRepository.GetAllBuildingList(Int32.Parse(placeidx));

                if (model is [_, ..])
                {
                    return new ResponseList<BuildinglistDTO>()
                    {
                        message = "요청이 정상적으로 처리되었습니다.",
                        data = model.Select(e => new BuildinglistDTO
                        {
                            ID = e.Id,
                            BuildingCD = e.BuildingCd,
                            Name = e.Name,
                            Address = e.Address,
                            CompletionDT = e.CompletionDt,
                            CreateDT = e.CreateDt
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<BuildinglistDTO>() { message = "요청이 정상적으로 처리되었습니다.", data = new List<BuildinglistDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<BuildinglistDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<BuildinglistDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 건물 상세정보 보기
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<DetailBuildingDTO>?> GetDetailBuildingService(HttpContext? context, int? buildingId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };
                if (buildingId is null)
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 잘못되었습니다.", data = new DetailBuildingDTO(), code = 404 };

                BuildingTb? model = await BuildingInfoRepository.GetBuildingInfo(buildingId);

                if (model is not null)
                {
                    DetailBuildingDTO dto = new DetailBuildingDTO();
                    dto.Id = model.Id;
                    dto.BuildingCD = model.BuildingCd;
                    dto.Name = model.Name;
                    dto.Address = model.Address;
                    dto.Tel = model.Tel;
                    dto.Usage = model.Usage;
                    dto.ConstComp = model.ConstComp;
                    dto.CompletionDT = model.CompletionDt;
                    dto.BuildingStruct = model.BuildingStruct;
                    dto.RoofStruct = model.RoofStruct;

                    string? Image = model.Image;
                    if(!String.IsNullOrWhiteSpace(Image))
                    {
                        string PlaceFileName = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString());
                        string[] FileList = Directory.GetFiles(PlaceFileName);
                        if(FileList is [_, ..])
                        {
                            foreach(var file in FileList)
                            {
                                if (file.Contains(Image))
                                {
                                    byte[] ImageBytes = File.ReadAllBytes(file);
                                    dto.Image = Convert.ToBase64String(ImageBytes);
                                }
                            }
                        }
                        else
                        {
                            dto.Image = model.Image;
                        }
                    }
                    else
                    {
                        dto.Image = model.Image;
                    }
                    

                    List<GroupitemTb>? GROUP_TB = await GroupItemInfoRepository.GetAllGroupList(model.Id);

                    dto.GroupItem = new List<GroupListDTO>(); // GROUP

                    if (GROUP_TB is [_, ..])
                    {
                        foreach(GroupitemTb groupItem in GROUP_TB)
                        {
                            
                            GroupListDTO groupdto = new GroupListDTO();
                            groupdto.ID = groupItem.Id;
                            groupdto.Name = groupItem.Name;

                            List<ItemkeyTb>? KEY_TB = await ItemKeyInfoRepository.GetAllKeyList(groupItem.Id);
                            if(KEY_TB is null)
                            {
                                dto.GroupItem.Add(groupdto);
                                return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                            }

                            foreach(ItemkeyTb? keyItem in KEY_TB)
                            {
                                GroupKeyListDTO keydto = new GroupKeyListDTO();
                                keydto.ID = keyItem.Id;
                                keydto.ItemKey = keyItem.Itemkey;
                                
                                groupdto.KeyListDTO!.Add(keydto);
                                
                                List<ItemvalueTb>? VALUE_TB = await ItemValueInfoRepository.GetAllValueList(keyItem.Id);
                                foreach(ItemvalueTb? valueItem in VALUE_TB)
                                {
                                    GroupValueListDTO valuedto = new GroupValueListDTO();
                                    valuedto.ID = valueItem.Id;
                                    valuedto.ItemValue = valueItem.Itemvalue;
                                    valuedto.Unit = valueItem.Unit;

                                    keydto.ValueList.Add(valuedto);
                                }
                            }
                            dto.GroupItem.Add(groupdto);
                        }
                    }

                    return new ResponseUnit<DetailBuildingDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };

                }
                else
                {
                    return new ResponseUnit<DetailBuildingDTO>() { message = "데이터가 존재하지 않습니다.", data = new DetailBuildingDTO(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailBuildingDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DetailBuildingDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 건물정보 수정
        /// LEVEL_1 (GROUPID) --> 없다 : 아에처음
        /// LEVEL_2 (UPDATE, INSERT)
        /// LEVEL_3 (UPDATE, INSERT) - 2에 종속 2가 INSERT면 무조껀 INSERT
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateBuildingService(HttpContext? context, DetailBuildingDTO? dto, IFormFile? files)
        {
            string? FileName = null;
            string? FileExtenstion = null;
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };


                int ThisBuildingId = Convert.ToInt32(dto.Id);

                // 해당 건물의 빌딩 검색
                BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(ThisBuildingId);
                buildingtb.BuildingCd = dto.BuildingCD;
                buildingtb.Name = dto.Name;
                buildingtb.Address = dto.Address;
                buildingtb.Tel = dto.Tel;
                buildingtb.Usage = dto.Usage;
                buildingtb.ConstComp = dto.ConstComp;
                buildingtb.CompletionDt = dto.CompletionDT;
                buildingtb.BuildingStruct = dto.BuildingStruct;
                buildingtb.RoofStruct = dto.RoofStruct;
                buildingtb.UpdateDt = DateTime.Now;
                buildingtb.UpdateUser = creater;


                if (files is not null) // 파일이 공백이 아닌경우 - 삭제 - 업데이트 or insert
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);
                    if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<bool?>() { message = "이미지의 형식이 올바르지 않습니다.", data = false, code = 404 };
                    }

                    // DB 파일 삭제
                    string? filePath = buildingtb.Image;
                    PlaceFileFolderPath = String.Format(@"{0}\\{1}\\Building", Common.FileServer, placeid.ToString()); // 사업장

                    if (!String.IsNullOrWhiteSpace(filePath))
                    {
                        FileName = String.Format("{0}\\{1}", PlaceFileFolderPath, filePath);
                        if (File.Exists(FileName))
                        {
                            File.Delete(FileName);
                        }
                    }

                    string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}"; // 암호화된 파일명

                    //string SavePath = String.Format(@"{0}\\{1}", Common.FileServer, placeidx.ToString());
                    string? newFilePath = Path.Combine(PlaceFileFolderPath, newFileName);

                    using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                    {
                        await files.CopyToAsync(fileStream);
                        buildingtb.Image = newFileName;
                    }

                }
                else // 파일이 공백인경우 db에 해당 데이터 값이 있으면 삭제
                {
                    string? filePath = buildingtb.Image;
                    if (!String.IsNullOrWhiteSpace(filePath))
                    {
                        PlaceFileFolderPath = String.Format(@"{0}\\{1}", Common.FileServer, placeid.ToString()); // 사업장
                        FileName = String.Format("{0}\\{1}", PlaceFileFolderPath, filePath);
                        if (File.Exists(FileName))
                        {
                            File.Delete(FileName);
                            buildingtb.Image = null;
                        }
                    }
                }

                bool? buildingupdate = await BuildingInfoRepository.UpdateBuildingInfo(buildingtb);
                if (buildingupdate != true)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if (dto.GroupItem is null)
                    return new ResponseUnit<bool?>() { message = "수정이 완료되었습니다.", data = null, code = 200 };

                // 삭제먼저 진행 -- GroupTB
                #region 삭제 잠시
                // 전체 GroupTable
                List<GroupitemTb>? AllGroupItemTb = await GroupItemInfoRepository.GetAllGroupList(ThisBuildingId);
                List<int>? AllGroupIdx = new List<int>();
                if (AllGroupItemTb is [_, ..])
                {
                    AllGroupIdx = AllGroupItemTb.Select(m => m.Id).ToList();
                }

                // 전체 ItemTB
                List<ItemkeyTb>? AllItemKeyTb = await ItemKeyInfoRepository.ContainsKeyList(AllGroupIdx);
                List<int>? AllItemKeyIdx = new List<int>();
                if (AllItemKeyTb is [_, ..])
                {
                    AllItemKeyIdx = AllItemKeyTb.Select(m => m.Id).ToList();
                }

                // 전체 Value TB
                List<ItemvalueTb>? AllItemValueTb = await ItemValueInfoRepository.ContainsKeyList(AllItemKeyIdx);
                List<int>? AllItemValueIdx = new List<int>();
                if (AllItemValueTb is [_, ..])
                {
                    AllItemValueIdx = AllItemValueTb.Select(m => m.Id).ToList();
                }

                List<GroupListDTO>? GroupListDTO = dto.GroupItem.Where(m => m.ID != null).ToList();
                List<int>? GroupList = new List<int>();
                if (GroupListDTO is [_, ..])
                {
                    GroupList = GroupListDTO.Select(m => Convert.ToInt32(m.ID)).ToList();
                }
                List<int>? DelGroupList = new List<int>();
                if (AllGroupIdx is [_, ..])
                {
                    DelGroupList = AllGroupIdx.Except(GroupList).ToList();
                }

                List<GroupKeyListDTO> KeyListDTO = new List<GroupKeyListDTO>();
                if(GroupListDTO is [_, ..])
                {
                    KeyListDTO = GroupListDTO.SelectMany(m => m.KeyListDTO).Where(e => e.ID != null).ToList();
                }

                List<int>? KeyList = new List<int>();
                if(KeyListDTO is [_, ..])
                {
                    KeyList = KeyListDTO.Select(m => Convert.ToInt32(m.ID)).ToList();
                }

                List<int>? DelKeyList = new List<int>();
                if(AllItemKeyIdx is [_, ..])
                {
                    DelKeyList = AllItemKeyIdx.Except(KeyList).ToList();
                }

                List<GroupValueListDTO>? ValueListDTO = new List<GroupValueListDTO>();
                if (KeyListDTO is [_, ..])
                {
                    ValueListDTO = KeyListDTO.SelectMany(m => m.ValueList).Where(e => e.ID != null).ToList();
                }

                List<int>? ValueList = new List<int>();
                if(ValueListDTO is [_, ..])
                {
                    ValueList = ValueListDTO.Select(m => Convert.ToInt32(m.ID)).ToList();
                }

                List<int>? DelValueList = new List<int>();
                if(AllItemValueIdx is [_, ..])
                {
                    DelValueList = AllItemValueIdx.Except(ValueList).ToList();
                }


                for (int i = 0; i < DelGroupList.Count(); i++)
                {
                    GroupitemTb? delGroup = await GroupItemInfoRepository.GetGroupInfo(DelGroupList[i]);
                    if(delGroup is not null)
                    {
                        delGroup.DelDt = DateTime.Now;
                        delGroup.DelUser = creater;
                        delGroup.DelYn = true;

                        bool? delGroupResult = await GroupItemInfoRepository.DeleteGroupInfo(delGroup);
                        if(delGroupResult != true)
                        {
                            // 실패
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }
                    }
                }

                for (int i = 0; i < DelKeyList.Count(); i++)
                {
                    ItemkeyTb? delKey = await ItemKeyInfoRepository.GetKeyInfo(DelKeyList[i]);
                    delKey.DelDt = DateTime.Now;
                    delKey.DelUser = creater;
                    delKey.DelYn = true;

                    bool? delKeyResult = await ItemKeyInfoRepository.DeleteKeyInfo(delKey);
                    if(delKeyResult != true)
                    {
                        // 실패
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }

                for (int i = 0; i < DelValueList.Count(); i++)
                {
                    ItemvalueTb? delValue = await ItemValueInfoRepository.GetValueInfo(DelValueList[i]);
                    delValue.DelDt = DateTime.Now;
                    delValue.DelUser = creater;
                    delValue.DelYn = true;

                    bool? delValueResult = await ItemValueInfoRepository.DeleteValueInfo(delValue);
                    if(delValueResult != true)
                    {
                        // 실패
                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }
                #endregion


                foreach (GroupListDTO group in dto.GroupItem)
                {
                    int? GroupId = group.ID;
                    
                    if (GroupId is not null) // 넘어온 DTO의 GroupID가 Null이 아니다
                    {
                        // GroupTable Update
                        GroupitemTb? GroupTB = await GroupItemInfoRepository.GetGroupInfo(GroupId);
                        if (GroupTB is null)
                            return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                        GroupTB.Name = group.Name;
                        GroupTB.UpdateDt = DateTime.Now;
                        GroupTB.UpdateUser = creater;

                        bool? GroupUpdateResult = await GroupItemInfoRepository.UpdateGroupInfo(GroupTB);
                        if(GroupUpdateResult != true)
                        {
                            // 업데이트 실패
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }

                        if (group.KeyListDTO is [_, ..])
                        {
                            foreach (var key in group.KeyListDTO)
                            {
                                int? KeyId = key.ID;

                                if(KeyId is not null) // Key 업데이트
                                {
                                    ItemkeyTb? KeyTB = await ItemKeyInfoRepository.GetKeyInfo(KeyId);
                                    if (KeyTB is null)
                                        return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                                    KeyTB.Itemkey = key.ItemKey;
                                    KeyTB.UpdateDt = DateTime.Now;
                                    KeyTB.UpdateUser = creater;

                                    bool? KeyUpdateResult = await ItemKeyInfoRepository.UpdateKeyInfo(KeyTB);
                                    if(KeyUpdateResult != true)
                                    {
                                        // 업데이트 실패
                                        return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                                    }


                                    foreach(var value in key.ValueList)
                                    {
                                        int? ValueId = value.ID;
                                        
                                        if(ValueId is not null) // Key Insert
                                        {
                                            ItemvalueTb? ValueTB = await ItemValueInfoRepository.GetValueInfo(ValueId);
                                            if (ValueTB is null)
                                                return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                                            ValueTB.Itemvalue = value.ItemValue;
                                            ValueTB.Unit = value.Unit;
                                            ValueTB.UpdateDt = DateTime.Now;
                                            ValueTB.UpdateUser = creater;

                                            bool? ValueUpdateResult = await ItemValueInfoRepository.UpdateValueInfo(ValueTB);
                                            if(ValueUpdateResult != true)
                                            {
                                                // 업데이트 실패
                                                return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                                            }
                                        }
                                        else // value Insert
                                        {
                                            ItemvalueTb ValueTB = new ItemvalueTb();
                                            ValueTB.Itemvalue = value.ItemValue;
                                            ValueTB.Unit = value.Unit;
                                            ValueTB.CreateDt = DateTime.Now;
                                            ValueTB.CreateUser = creater;
                                            ValueTB.UpdateDt = DateTime.Now;
                                            ValueTB.UpdateUser = creater;
                                            ValueTB.ItemKeyId = KeyId;

                                            ItemvalueTb? InsertValueTB = await ItemValueInfoRepository.AddAsync(ValueTB);
                                            if(InsertValueTB is null)
                                            {
                                                // 실패
                                                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                            }
                                        }
                                    }

                                }
                                else // Key Insert
                                {
                                    ItemkeyTb KeyTB = new ItemkeyTb();
                                    KeyTB.Itemkey = key.ItemKey;
                                    KeyTB.CreateDt = DateTime.Now;
                                    KeyTB.CreateUser = creater;
                                    KeyTB.UpdateDt = DateTime.Now;
                                    KeyTB.UpdateUser = creater;
                                    KeyTB.GroupItemId = GroupId;

                                    ItemkeyTb? InsertKeyTB = await ItemKeyInfoRepository.AddAsync(KeyTB);
                                    if(InsertKeyTB is null)
                                    {
                                        // 실패
                                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                    }

                                    foreach(var value in key.ValueList)
                                    {
                                        ItemvalueTb? ValueTB = new ItemvalueTb();
                                        ValueTB.Itemvalue = value.ItemValue;
                                        ValueTB.Unit = value.Unit;
                                        ValueTB.CreateDt = DateTime.Now;
                                        ValueTB.CreateUser = creater;
                                        ValueTB.UpdateDt = DateTime.Now;
                                        ValueTB.UpdateUser = creater;
                                        ValueTB.ItemKeyId = InsertKeyTB.Id;

                                        ItemvalueTb? InsertValueTB = await ItemValueInfoRepository.AddAsync(ValueTB);
                                        if (InsertValueTB is null)
                                        {
                                            // 실패
                                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                        }
                                    }


                                }
                            }
                        }
                    }
                    else // 넘어온 DTO의 GroupID가 Null이다. -- 전부 INSERT
                    {
                        GroupitemTb InsertGroupTB = new GroupitemTb();
                        InsertGroupTB.Name = dto.Name;
                        InsertGroupTB.Type = 0;
                        InsertGroupTB.CreateDt = DateTime.Now;
                        InsertGroupTB.CreateUser = creater;
                        InsertGroupTB.UpdateDt = DateTime.Now;
                        InsertGroupTB.UpdateUser = creater;
                        InsertGroupTB.BuildingId = dto.Id;

                        GroupitemTb? ResultGroupTB = await GroupItemInfoRepository.AddAsync(InsertGroupTB);
                        if(ResultGroupTB is not null) // Add하고 받은값이 Null이 아니다 - 정상 INSERT
                        {
                            foreach(GroupKeyListDTO key in group.KeyListDTO)
                            {
                                ItemkeyTb InsertKeyTB = new ItemkeyTb();
                                InsertKeyTB.Itemkey = key.ItemKey;
                                InsertKeyTB.CreateDt = DateTime.Now;
                                InsertKeyTB.CreateUser = creater;
                                InsertKeyTB.UpdateDt = DateTime.Now;
                                InsertKeyTB.UpdateUser = creater;
                                InsertKeyTB.GroupItemId = ResultGroupTB.Id;

                                ItemkeyTb? ResultKeyTB = await ItemKeyInfoRepository.AddAsync(InsertKeyTB);
                                if(ResultKeyTB is not null) // Key를 ADD하고 받은값이 Null이 아니다 - 정상 Insert
                                {
                                    foreach(GroupValueListDTO value in key.ValueList)
                                    {
                                        ItemvalueTb InsertValueTB = new ItemvalueTb();
                                        InsertValueTB.Itemvalue = value.ItemValue;
                                        InsertValueTB.Unit = value.Unit;
                                        InsertValueTB.CreateDt = DateTime.Now;
                                        InsertValueTB.CreateUser = creater;
                                        InsertValueTB.UpdateDt = DateTime.Now;
                                        InsertValueTB.UpdateUser = creater;
                                        InsertValueTB.ItemKeyId = ResultKeyTB.Id;

                                        ItemvalueTb? ResultValueTB = await ItemValueInfoRepository.AddAsync(InsertValueTB);
                                        
                                        if(ResultValueTB is null)
                                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                    }
                                }
                                else
                                {
                                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                }
                            }
                        }
                        else
                        {
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }

                    }
                }

                return new ResponseUnit<bool?>() { message = "수정이 완료되었습니다.", data = true, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// 건물정보 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<int?>> DeleteBuildingService(HttpContext? context, List<int>? buildingid)
        {
            try
            {
                int delCount = 0;

                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (buildingid is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]); // 토큰 로그인 사용자 검사
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]); // 토큰 사업장 검사
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                // - 층이 있으면 삭제안됨
                // - 건물 삭제시 하위 추가내용들 전체삭제
                for (int i = 0; i < buildingid.Count(); i++)
                {
                    List<FloorTb>? floortb = await FloorInfoRepsitory.GetFloorList(buildingid[i]);

                    if (floortb is [_, ..])
                        return new ResponseUnit<int?> { message = "해당 건물에 속한 층정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                for (int i = 0; i < buildingid.Count(); i++) 
                {
                    BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(buildingid[i]);
                    buildingtb.DelYn = true;
                    buildingtb.DelDt = DateTime.Now;
                    buildingtb.DelUser = creater;
                    bool? delBuilding  = await BuildingInfoRepository.DeleteBuildingInfo(buildingtb);
                    if (delBuilding == true)
                    {
                        delCount++;
                    }

                    List<GroupitemTb>? grouptb = await GroupItemInfoRepository.GetAllGroupList(buildingid[i]);
                    if (grouptb is [_, ..])
                    {
                        foreach(GroupitemTb group in grouptb)
                        {
                            group.DelYn = true;
                            group.DelDt = DateTime.Now;
                            group.DelUser = creater;

                            bool? delGroup = await GroupItemInfoRepository.DeleteGroupInfo(group);
                            if(delGroup == true)
                            {
                                List<ItemkeyTb>? keytb = await ItemKeyInfoRepository.GetAllKeyList(group.Id);
                                if (keytb is [_, ..])
                                {
                                    foreach (ItemkeyTb key in keytb)
                                    {
                                        key.DelYn = true;
                                        key.DelDt = DateTime.Now;
                                        key.DelUser = creater;

                                        bool? delKey = await ItemKeyInfoRepository.DeleteKeyInfo(key);
                                        if (delKey == true)
                                        {
                                            List<ItemvalueTb>? valuetb = await ItemValueInfoRepository.GetAllValueList(key.Id);
                                            if (valuetb is [_, ..])
                                            {
                                                foreach (ItemvalueTb value in valuetb)
                                                {
                                                    value.DelYn = true;
                                                    value.DelDt = DateTime.Now;
                                                    value.DelUser = creater;

                                                    bool? delValue = await ItemValueInfoRepository.DeleteValueInfo(value);
                                                    if (delValue != true)
                                                    {
                                                        return new ResponseUnit<int?>() { message = "이미 삭제된 데이터입니다.", data = null, code = 404 };
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return new ResponseUnit<int?>() { message = "이미 삭제된 데이터입니다.", data = null, code = 404 };
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return new ResponseUnit<int?>() { message = "이미 삭제된 데이터입니다.", data = null, code = 404 };
                            }
                        }
                    }
                }

                return new ResponseUnit<int?>() { message = $"{delCount}건 삭제 성공.", data = delCount, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

      

    }
}
