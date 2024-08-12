using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Facility.Group;
using FamTec.Server.Repository.Facility.ItemKey;
using FamTec.Server.Repository.Facility.ItemValue;
using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Model;

namespace FamTec.Server.Services.Facility.Type.Lift
{
    public class LiftFacilityService : ILiftFacilityService
    {
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        private readonly IFacilityGroupItemInfoRepository FacilityGroupItemInfoRepository;
        private readonly IFacilityItemKeyInfoRepository FacilityItemKeyInfoRepository;
        private readonly IFacilityItemValueInfoRepository FacilityItemValueInfoRepository;

        private readonly ILogService LogService;

        private DirectoryInfo? di;
        private string? LiftFileFolderPath;

        public LiftFacilityService(
           IBuildingInfoRepository _buildinginforepository,
           IFacilityInfoRepository _facilityinforepository,
           IRoomInfoRepository _roominforepository,
           IFloorInfoRepository _floorinforepository,
           IFacilityGroupItemInfoRepository _facilitygroupiteminforepository,
           IFacilityItemKeyInfoRepository _facilityitemkeyinforepository,
           IFacilityItemValueInfoRepository _facilityitemvalueinforepository,
           ILogService _logService)
        {
            FacilityInfoRepository = _facilityinforepository;
            RoomInfoRepository = _roominforepository;

            FacilityGroupItemInfoRepository = _facilitygroupiteminforepository;
            FacilityItemKeyInfoRepository = _facilityitemkeyinforepository;
            FacilityItemValueInfoRepository = _facilityitemvalueinforepository;

            LogService = _logService;
        }

        /// <summary>
        /// 승강설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<FacilityDTO>?> AddLiftFacilityService(HttpContext? context, FacilityDTO? dto, IFormFile? files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;

                if (context is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                if (dto is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (string.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                RoomTb? tokenck = await RoomInfoRepository.GetRoomInfo(dto.RoomTbId!.Value);
                if (tokenck is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (string.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                // 기계설비 관련한 폴더 없으면 만들기
                LiftFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Lift", Common.FileServer, placeidx.ToString());

                di = new DirectoryInfo(LiftFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = new FacilityTb();
                model.Category = "승강"; // 카테고리 - 기계
                model.Name = dto.Name; // 설비명칭
                model.Type = dto.Type; // 형식
                model.Num = dto.Num; // 개수
                model.Unit = dto.Unit; // 단위
                model.EquipDt = dto.EquipDT; // 설치년월
                model.Lifespan = dto.LifeSpan; // 내용연수
                model.StandardCapacity = dto.Standard_capacity; //규격용량
                model.ChangeDt = dto.ChangeDT; // 교체년월
                model.CreateDt = DateTime.Now;
                model.CreateUser = creator;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creator;
                model.RoomTbId = dto.RoomTbId.Value; // 공간 ID

                if (files is not null)
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);

                    if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<FacilityDTO>() { message = "올바르지 않는 파일형식입니다.", data = null, code = 404 };
                    }
                    else
                    {
                        // 이미지 경로
                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}"; // 암호화된 파일명

                        string? newFilePath = Path.Combine(LiftFileFolderPath, newFileName);

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

                FacilityTb? result = await FacilityInfoRepository.AddAsync(model);
                if (result is not null)
                    return new ResponseUnit<FacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDTO(), code = 500 };
            }
        }

        public async ValueTask<ResponseList<FacilityListDTO>?> GetLiftFacilityListService(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = new List<FacilityListDTO>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (string.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = new List<FacilityListDTO>(), code = 404 };

                List<FacilityListDTO>? model = await FacilityInfoRepository.GetPlaceLiftFacilityList(Convert.ToInt32(placeidx));

                if (model is [_, ..])
                {
                    return new ResponseList<FacilityListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<FacilityListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<FacilityListDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<FacilityListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<FacilityDetailDTO?>> GetLiftDetailFacilityService(HttpContext? context, int? facilityId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                if (facilityId is null)
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (string.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(facilityId.Value);

                if (model is not null)
                {
                    RoomTb? room = await RoomInfoRepository.GetRoomInfo(model.RoomTbId);
                    if (room is not null)
                    {
                        FacilityDetailDTO dto = new FacilityDetailDTO();
                        dto.Id = model.Id;
                        dto.Category = model.Category;
                        dto.Name = model.Name;
                        dto.Type = model.Type;
                        dto.Num = model.Num;
                        dto.Unit = model.Unit;
                        dto.EquipDT = model.EquipDt;
                        dto.LifeSpan = model.Lifespan;
                        dto.Standard_capacity = model.StandardCapacity;
                        dto.ChangeDT = model.ChangeDt;
                        dto.RoomId = model.RoomTbId;
                        dto.RoomName = room.Name;
                        /*
                        string? Image = model.Image;
                        if (!string.IsNullOrWhiteSpace(Image))
                        {
                            LiftFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Lift", Common.FileServer, placeid);
                            string[] FileList = Directory.GetFiles(LiftFileFolderPath);
                            if (FileList is [_, ..])
                            {
                                foreach (var file in FileList)
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
                        */
                        return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

       

        public async ValueTask<ResponseUnit<bool?>> UpdateLiftFacilityService(HttpContext? context, FacilityDTO? dto, IFormFile? files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;
                string? placeFilePath = String.Empty;

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(dto.ID.Value);
                
                if (model is not null)
                {
                    if (model!.Category != "승강")
                        return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    model.Category = "승강"; // 카테고리
                    model.Name = dto.Name; // 설비명칭
                    model.Type = dto.Type; // 타입
                    model.Num = dto.Num; // 개수
                    model.Unit = dto.Unit; // 단위
                    model.EquipDt = dto.EquipDT; // 설치년월
                    model.Lifespan = dto.LifeSpan; // 내용연수
                    model.StandardCapacity = dto.Standard_capacity;
                    model.ChangeDt = dto.ChangeDT;
                    model.UpdateDt = DateTime.Now;
                    model.UpdateUser = creater;
                    model.RoomTbId = dto.RoomTbId.Value;

                    // 이미지 변경 or 삭제
                    if (files is not null)
                    {
                        FileName = files.FileName;
                        FileExtenstion = Path.GetExtension(FileName);

                        if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                        {
                            return new ResponseUnit<bool?>() { message = "이미지 형식이 올바르지 않습니다.", data = null, code = 404 };
                        }


                        // DB 파일 삭제
                        string? filePath = model.Image;
                        LiftFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Lift", Common.FileServer, placeid);
                        di = new DirectoryInfo(LiftFileFolderPath);
                        if (di.Exists)
                        {
                            if (!String.IsNullOrWhiteSpace(filePath))
                            {
                                FileName = String.Format("{0}\\{1}", LiftFileFolderPath, filePath);
                                if (File.Exists(FileName))
                                {
                                    File.Delete(FileName);
                                }
                            }
                        }
                        else
                        {
                            di.Create();
                        }

                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}";

                        string? newFilePath = Path.Combine(LiftFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await files.CopyToAsync(fileStream);
                            model.Image = newFileName;
                        }
                    }
                    else // 파일이 공백인 경우 db에 해당 값이 있으면 삭제
                    {
                        string? filePath = model.Image;
                        if (!String.IsNullOrWhiteSpace(filePath))
                        {
                            LiftFileFolderPath = String.Format(@"{0}\\{1}\\Facility\\Lift", Common.FileServer, placeid);
                            FileName = String.Format("{0}\\{1}", LiftFileFolderPath, filePath);

                            if (File.Exists(FileName))
                            {
                                File.Delete(FileName);
                            }
                            model.Image = null;
                        }
                    }

                    bool? updateBuilding = await FacilityInfoRepository.UpdateFacilityInfo(model);
                    if (updateBuilding == true)
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

        public async ValueTask<ResponseUnit<int?>> DeleteLiftFacilityService(HttpContext? context, List<int> delIdx)
        {
            try
            {
                return null;

                /*
                int delCount = 0;

                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (delIdx is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (string.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]); // 토큰 사업장 검사
                if (string.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                for (int i = 0; i < delIdx.Count(); i++)
                {
                    FacilityTb? FacilityTB = await FacilityInfoRepository.GetFacilityInfo(delIdx[i]);

                    if (FacilityTB is not null)
                    {
                        FacilityTB.DelDt = DateTime.Now;
                        FacilityTB.DelUser = creater;
                        FacilityTB.DelYn = true;

                        bool? delFacility = await FacilityInfoRepository.DeleteFacilityInfo(FacilityTB);
                        if (delFacility != true)
                        {
                            return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                        }

                        List<FacilityItemGroupTb>? GroupTBList = await FacilityGroupItemInfoRepository.GetAllGroupList(delIdx[i]);
                        if (GroupTBList is [_, ..])
                        {
                            foreach (FacilityItemGroupTb GroupTB in GroupTBList)
                            {
                                GroupTB.DelDt = DateTime.Now;
                                GroupTB.DelUser = creater;
                                GroupTB.DelYn = true;

                                bool? delGroup = await FacilityGroupItemInfoRepository.DeleteGroupInfo(GroupTB);
                                if (delGroup != true)
                                {
                                    return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                }

                                List<FacilityItemKeyTb>? GroupKeyList = await FacilityItemKeyInfoRepository.GetAllKeyList(GroupTB.Id);

                                if (GroupKeyList is [_, ..])
                                {
                                    foreach (FacilityItemKeyTb KeyTB in GroupKeyList)
                                    {
                                        KeyTB.DelDt = DateTime.Now;
                                        KeyTB.DelUser = creater;
                                        KeyTB.DelYn = true;

                                        bool? delKey = await FacilityItemKeyInfoRepository.DeleteKeyInfo(KeyTB);
                                        if (delKey != true)
                                        {
                                            return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                        }

                                        List<FacilityItemValueTb>? GroupValueList = await FacilityItemValueInfoRepository.GetAllValueList(KeyTB.Id);
                                        if (GroupValueList is [_, ..])
                                        {
                                            foreach (FacilityItemValueTb ValueTB in GroupValueList)
                                            {
                                                ValueTB.DelDt = DateTime.Now;
                                                ValueTB.DelUser = creater;
                                                ValueTB.DelYn = true;

                                                bool? delValue = await FacilityItemValueInfoRepository.DeleteValueInfo(ValueTB);

                                                if (delValue != true)
                                                {
                                                    return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                    delCount++;
                }
                return new ResponseUnit<int?>() { message = $"{delCount}건 삭제 성공", data = delCount, code = 200 };
                */
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<int?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
