using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Model;

namespace FamTec.Server.Services.Facility.Type.Security
{
    public class SecurityFacilityService : ISecurityFacilityService
    {
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        private readonly ILogService LogService;
        private IFileService FileService;

        private DirectoryInfo? di;
        private string? SecurityFileFolderPath;

        public SecurityFacilityService(
           IFacilityInfoRepository _facilityinforepository,
           IRoomInfoRepository _roominforepository,
           IFileService _fileservice,
           ILogService _logService)
        {
            this.FacilityInfoRepository = _facilityinforepository;
            this.RoomInfoRepository = _roominforepository;

            this.FileService = _fileservice;
            this.LogService = _logService;
        }

        public async ValueTask<ResponseUnit<FacilityDTO>> AddSecurityFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                RoomTb? RoomInfo = await RoomInfoRepository.GetRoomInfo(dto.RoomTbId!.Value);
                if (RoomInfo is null)
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creator) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? NewFileName = String.Empty;
                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                // 전기설비 관련한 폴더 없으면 만들기 
                SecurityFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Security", Common.FileServer, placeidx);

                di = new DirectoryInfo(SecurityFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = new FacilityTb()
                {
                    Category = "보안", //  카테고리 - 보안
                    Name = dto.Name!, // 설비명칭
                    Type = dto.Type, // 형식
                    Num = dto.Num, // 개수
                    Unit = dto.Unit, // 단위
                    EquipDt = dto.EquipDT, // 설치년월
                    Lifespan = dto.LifeSpan, // 내용연수
                    StandardCapacity = dto.Standard_capacity, // 규격용량
                    ChangeDt = dto.ChangeDT, // 교체년월
                    CreateDt = DateTime.Now,
                    CreateUser = creator,
                    UpdateDt = DateTime.Now,
                    UpdateUser = creator,
                    RoomTbId = dto.RoomTbId.Value, // 공간 ID
                    Image = NewFileName
                };

                FacilityTb? result = await FacilityInfoRepository.AddAsync(model);
                if (result is not null)
                {
                    if(files is not null)
                    {
                        // 파일 넣기
                        bool? AddFile = await FileService.AddResizeImageFile(NewFileName, SecurityFileFolderPath, files);
                    }
                    return new ResponseUnit<FacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                    return new ResponseUnit<FacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDTO(), code = 500 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDTO(), code = 500 };
            }
        }
        
        /// <summary>
        /// 사업장에 등록되어있는 보안설비 List 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<FacilityListDTO>> GetSecurityFacilityListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<FacilityListDTO>? model = await FacilityInfoRepository.GetPlaceSecurityFacilityList(Convert.ToInt32(placeidx));

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

        public async ValueTask<ResponseUnit<FacilityDetailDTO>> GetSecurityDetailFacilityService(HttpContext context, int facilityId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(facilityId);
                if(model is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

                RoomTb? room = await RoomInfoRepository.GetRoomInfo(model.RoomTbId);
                if(room is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };

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

                SecurityFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Security", Common.FileServer, placeid);
                        
                if(!String.IsNullOrWhiteSpace(model.Image))
                {
                    dto.ImageName = model.Image; // 이미지 파일명
                    dto.Image = await FileService.GetImageFile(SecurityFileFolderPath, model.Image); // 이미지 Byte[]
                }

                return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
     
        public async ValueTask<ResponseUnit<bool?>> UpdateSecurityFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files)
        {
            try
            {
                // 파일처리 준비
                string? NewFileName = String.Empty;
                string? deleteFileName = String.Empty;

                // 수정실패 시 돌려놓을 FormFile
                IFormFile? AddTemp = default;
                string RemoveTemp = String.Empty;

                if (context is null || dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 이미지 변경 or 삭제
                SecurityFileFolderPath = String.Format(@"{0}\\{1}\\Facility\\Security", Common.FileServer, placeid);
                di = new DirectoryInfo(SecurityFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(dto.ID!.Value);
                if(model is null || model.Category.Trim() != "보안")
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Category = "보안"; // 카테고리
                model.Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!; // 설비명칭
                model.Type = dto.Type; // 타입
                model.Num = dto.Num; // 개수
                model.Unit = dto.Unit; // 단위
                model.EquipDt = dto.EquipDT; // 설치년월
                model.Lifespan = dto.LifeSpan; // 내용연수
                model.StandardCapacity = dto.Standard_capacity;
                model.ChangeDt = dto.ChangeDT;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creater;
                model.RoomTbId = dto.RoomTbId!.Value;


                if (files is not null) // 파일이 공백이 아닌 경우
                {
                    if (files.FileName != model.Image) // 넘어온 이미지의 이름과 DB에 저장된 이미지의 이름이 다르면.
                    {
                        if (!String.IsNullOrWhiteSpace(model.Image))
                        {
                            deleteFileName = model.Image;
                        }

                        // 새로운 파일명 설정
                        string newFileName = FileService.SetNewFileName(UserIdx, files);
                        NewFileName = newFileName; // 파일명 리스트에 추가
                        model.Image = newFileName; // DB Image명칭 업데이트

                        RemoveTemp = newFileName; // 실패시 삭제명단에 넣어야함.
                    }
                }
                else // 파일이 공백인 경우
                {
                    if (!String.IsNullOrWhiteSpace(model.Image)) // DB의 이미지가 공백이 아니면
                    {
                        deleteFileName = model.Image; // 기존 파일 삭제 목록에 추가
                        model.Image = null; // 모델의 파일명 비우기
                    }
                }

                // 먼저 파일 삭제 처리
                // DB 실패했을경우 대비해서 해당파일을 미리 뽑아서 iFormFile로 변환하여 가지고 있어야함.
                byte[]? ImageBytes = null;
                if (!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    ImageBytes = await FileService.GetImageFile(SecurityFileFolderPath, deleteFileName);
                }

                // - DB 실패했을경우 iFormFile을 바이트로 변환하여 DB의 해당명칭으로 다시 저장해야함.
                if (ImageBytes is not null)
                {
                    AddTemp = FileService.ConvertFormFiles(ImageBytes, deleteFileName);
                }

                // 삭제할 파일명단에 들어와있으면 파일삭제
                if (!String.IsNullOrWhiteSpace(deleteFileName))
                {
                    FileService.DeleteImageFile(SecurityFileFolderPath, deleteFileName);
                }

                // 새 파일 저장
                if (files is not null)
                {
                    if (String.IsNullOrWhiteSpace(model.Image) || files.FileName != model.Image)
                    {
                        // Image가 없거나 혹은 기존 파일명과 다른 경우에만 파일 저장
                        await FileService.AddResizeImageFile(model.Image!, SecurityFileFolderPath, files);
                    }
                }

                // 이후 데이터베이스 업데이트
                bool? updateBuilding = await FacilityInfoRepository.UpdateFacilityInfo(model);
                if (updateBuilding == true)
                {
                    // 성공했으면 그걸로 끝
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    // 실패했으면 파일을 원래대로 돌려놔야함.
                    if (AddTemp is not null)
                    {
                        try
                        {
                            if (FileService.IsFileExists(SecurityFileFolderPath, AddTemp.FileName) == false)
                            {
                                // 파일을 저장하는 로직
                                await FileService.AddResizeImageFile(AddTemp.FileName, SecurityFileFolderPath, files);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 복원실패 : {ex.Message}");
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(RemoveTemp))
                    {
                        try
                        {
                            FileService.DeleteImageFile(SecurityFileFolderPath, RemoveTemp);
                        }
                        catch (Exception ex)
                        {
                            LogService.LogMessage($"파일 삭제실패 : {ex.Message}");
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> DeleteSecurityFacilityService(HttpContext context, List<int> delIdx)
        {
            try
            {
                if (context is null || delIdx is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]); // 토큰 사업장 검사

                if (String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 삭제가능여부 체크
                foreach (int id in delIdx) 
                {
                    bool? DelCheck = await FacilityInfoRepository.DelFacilityCheck(id);
                    if (DelCheck == true)
                        return new ResponseUnit<bool?>() { message = "참조하고있는 하위 정보가 있어 삭제가 불가능합니다.", data = null, code = 200 };
                }

                bool? DeleteResult = await FacilityInfoRepository.DeleteFacilityInfo(delIdx, creater);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


    }
}
