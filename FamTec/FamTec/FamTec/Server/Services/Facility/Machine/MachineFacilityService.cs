using DocumentFormat.OpenXml.Office.Word;
using FamTec.Client.Pages.Admin.Place.PlaceMain;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using Microsoft.JSInterop.Infrastructure;

namespace FamTec.Server.Services.Facility.Machine
{
    public class MachineFacilityService : IMachineFacilityService
    {
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        
        private readonly IRoomInfoRepository RoomInfoRepository;
        private readonly IFloorInfoRepository FloorInfoRepository;


        private readonly ILogService LogService;

        private DirectoryInfo? di;
        private string? MachineFileFolderPath;

        public MachineFacilityService(
            IBuildingInfoRepository _buildinginforepository,
            IFacilityInfoRepository _facilityinforepository,
            IRoomInfoRepository _roominforepository,
            IFloorInfoRepository _floorinforepository,
            ILogService _logService)
        {
            this.BuildingInfoRepository = _buildinginforepository;
            this.FacilityInfoRepository = _facilityinforepository;
            this.RoomInfoRepository = _roominforepository;
            this.FloorInfoRepository = _floorinforepository;

            this.LogService = _logService;
        }

        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddFacilityDTO>?> AddMachineFacilityService(HttpContext? context, AddFacilityDTO? dto, IFormFile? files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;

                if (context is null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                if (dto is null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                RoomTb? tokenck = await RoomInfoRepository.GetRoomInfo(dto.RoomTbId);
                if (tokenck is null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (string.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                // 기계설비 관련한 폴더 없으면 만들기
                MachineFileFolderPath = String.Format(@"{0}\\{1}\\Facility\\Machine", Common.FileServer, placeidx.ToString());

                di = new DirectoryInfo(MachineFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = new FacilityTb();
                model.Category = dto.Category; // 카테고리 - 기계
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
                model.RoomTbId = dto.RoomTbId; // 공간 ID

                if(files is not null)
                {
                    FileName = files.FileName;
                    FileExtenstion = Path.GetExtension(FileName);

                    if (!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                    {
                        return new ResponseUnit<AddFacilityDTO>() { message = "올바르지 않는 파일형식입니다.", data = null, code = 404 };
                    }
                    else
                    {
                        // 이미지 경로
                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}"; // 암호화된 파일명

                        string? newFilePath = Path.Combine(MachineFileFolderPath, newFileName);

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
                    return new ResponseUnit<AddFacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddFacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddFacilityDTO(), code = 500 };
            }

        }

        /// <summary>
        /// 사업장에 등록되어있는 기계설비 List 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MachineFacilityListDTO>?> GetMachineFacilityListService(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MachineFacilityListDTO>() { message = "요청이 잘못되었습니다.", data = new List<MachineFacilityListDTO>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<MachineFacilityListDTO>() { message = "요청이 잘못되었습니다.", data = new List<MachineFacilityListDTO>(), code = 404 };

                List<MachineFacilityListDTO>? model = await FacilityInfoRepository.GetPlaceMachineFacilityList(Convert.ToInt32(placeidx));

                if (model is [_, ..])
                {
                    return new ResponseList<MachineFacilityListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<MachineFacilityListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<MachineFacilityListDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MachineFacilityListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<FacilityDetailDTO?>> GetMachineDetailFacilityService(HttpContext? context, int? facilityId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };
                
                if (facilityId is null)
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<FacilityDetailDTO?>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(facilityId);
                
                if(model is not null)
                {
                    RoomTb? room = await RoomInfoRepository.GetRoomInfo(model.RoomTbId);
                    if(room is not null)
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

                        string? Image = model.Image;
                        if(!String.IsNullOrWhiteSpace(Image))
                        {
                            string placeFilePath = String.Format(@"{0}\\{1}\\Facility\\Machine", Common.FileServer, placeid);
                            string[] FileList = Directory.GetFiles(placeFilePath);
                            if(FileList is [_, ..])
                            {
                                foreach(var file in FileList)
                                {
                                    if(file.Contains(Image))
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        public async ValueTask<ResponseUnit<bool?>> UpdateMachineFacilityService(HttpContext? context, UpdateFacilityDTO? dto, IFormFile? files)
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
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(dto.Id);
                if(model is not null)
                {
                    model.Category = dto.Category; // 카테고리
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
                    model.RoomTbId = dto.RoomId;

                    // 이미지 변경 or 삭제
                    if(files is not null) // 파일이 공백이 아닌경우 - 삭제 업데이트 or insert
                    {
                        FileName = files.FileName;
                        FileExtenstion = Path.GetExtension(FileName);
                        if(!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                        {
                            return new ResponseUnit<bool?>() { message = "이미지의 형식이 올바르지 않습니다.", data = null, code = 404 };
                        }

                        // DB 파일 삭제
                        string? filePath = model.Image;
                        placeFilePath = String.Format(@"{0}\\{1}\\Facility\\Machine", Common.FileServer, placeid);

                        if(!String.IsNullOrWhiteSpace(filePath))
                        {
                            FileName = String.Format("{0}\\{1}", placeFilePath, filePath);
                            if(File.Exists(FileName))
                            {
                                File.Delete(FileName);
                            }
                        }

                        


                        string? newFileName = $"{Guid.NewGuid()}{Path.GetExtension(FileName)}";

                        string? newFilePath = Path.Combine(placeFilePath, newFileName);

                        using (var fileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write))
                        {
                            await files.CopyToAsync(fileStream);
                            model.Image = newFileName;
                        }
                    }
                    else // 파일이 공백인 경우 db에 해당 값이 있으면 삭제
                    {
                        string? filePath = model.Image;
                        if(!String.IsNullOrWhiteSpace(filePath))
                        {
                            placeFilePath = String.Format(@"{0}\\{1}\\Facility\\Machine", Common.FileServer, placeid);
                            FileName = String.Format("{0}\\{1}", placeFilePath, filePath);
                            if(File.Exists(FileName))
                            {
                                File.Delete(FileName);
                                model.Image = null;
                            }
                        }
                    }

                    bool? updateBuilding = await FacilityInfoRepository.UpdateFacilityInfo(model);
                    if(updateBuilding == true)
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
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

        // 그룹까지 삭제해야해서 나중에
        public async ValueTask<ResponseUnit<int?>> DeleteMachineFacilityService(HttpContext? context, List<int>? delIdx)
        {
            try
            {
                int delCount = 0;

                if (context is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(delIdx is null)
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<int?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                return null;

            }
            catch(Exception ex)
            {
                return null;
            }
        }



      
    }
}
