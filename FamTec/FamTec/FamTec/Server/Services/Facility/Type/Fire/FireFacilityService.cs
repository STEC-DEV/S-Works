using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;

namespace FamTec.Server.Services.Facility.Type.Fire
{
    public class FireFacilityService : IFireFacilityService
    {
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        private IFileService FileService;
        private readonly ILogService LogService;

        private DirectoryInfo? di;
        private string? FireFileFolderPath;

        public FireFacilityService(
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


        public async ValueTask<ResponseUnit<FacilityDTO>> AddFireFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files)
        {
            try
            {
                string? NewFileName = String.Empty;

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
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<FacilityDTO>() { message = "잘못된 요청입니다.", data = new FacilityDTO(), code = 404 };

                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                // 전기설비 관련한 폴더 없으면 만들기 
                FireFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Fire", Common.FileServer, placeidx);

                di = new DirectoryInfo(FireFileFolderPath);
                if (!di.Exists) di.Create();

                FacilityTb? model = new FacilityTb();
                model.Category = "소방"; //  카테고리 - 소방
                model.Name = dto.Name!; // 설비명칭
                model.Type = dto.Type; // 형식
                model.Num = dto.Num; // 개수
                model.Unit = dto.Unit; // 단위
                model.EquipDt = dto.EquipDT; // 설치년월
                model.Lifespan = dto.LifeSpan; // 내용연수
                model.StandardCapacity = dto.Standard_capacity; // 규격용량
                model.ChangeDt = dto.ChangeDT; // 교체년월
                model.CreateDt = DateTime.Now;
                model.CreateUser = creator;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creator;
                model.RoomTbId = dto.RoomTbId.Value; // 공간 ID

                if (files is not null)
                {
                    model.Image = NewFileName;
                }
                else
                {
                    model.Image = null;
                }

                FacilityTb? result = await FacilityInfoRepository.AddAsync(model);
                if (result is not null)
                {
                    if(files is not null)
                    {
                        // 파일 넣기
                        bool? AddFile = await FileService.AddImageFile(NewFileName, FireFileFolderPath, files);
                    }

                    return new ResponseUnit<FacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                    return new ResponseUnit<FacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = new FacilityDTO(), code = 404 };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 등록되어있는 소방설비 List 반환
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<FacilityListDTO>> GetFireFacilityListService(HttpContext context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<FacilityListDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                List<FacilityListDTO>? model = await FacilityInfoRepository.GetPlaceFireFacilityList(Convert.ToInt32(placeidx));

                if (model is [_, ..])
                {
                    return new ResponseList<FacilityListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                }
                else
                {
                    return new ResponseList<FacilityListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<FacilityListDTO>(), code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<FacilityListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<FacilityDetailDTO>> GetFireDetailFacilityService(HttpContext context, int facilityId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(facilityId);
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

                        FireFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Fire", Common.FileServer, placeid);

                        if(!String.IsNullOrWhiteSpace(model.Image))
                        {
                            dto.Image = await FileService.GetImageFile(FireFileFolderPath, model.Image);
                        }

                        return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 잘못되었습니다", data = null, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FacilityDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> UpdateFireFacilityService(HttpContext context, FacilityDTO dto, IFormFile? files)
        {
            try
            {
                string? NewFileName = String.Empty;
                string? deleteFileName = String.Empty;

            
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

                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                if(String.IsNullOrWhiteSpace(UserIdx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                FacilityTb? model = await FacilityInfoRepository.GetFacilityInfo(dto.ID!.Value);

                if (model is not null)
                {
                    if (model!.Category != "소방")
                        return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    model.Category = "소방"; // 카테고리
                    model.Name = dto.Name!; // 설비명칭
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

                    // 이미지 변경 or 삭제
                    FireFileFolderPath = string.Format(@"{0}\\{1}\\Facility\\Fire", Common.FileServer, placeid);

                    if(files is not null) // 파일이 공백이 아닌 경우
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는 경우
                        {
                            deleteFileName = model.Image; // 삭제할 이름을 넣는다.
                            model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                        }
                        else // DB엔 없는 경우
                        {
                            model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                        }
                    }
                    else // 파일이 공백인 경우
                    {
                        if (!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는경우
                        {
                            deleteFileName = model.Image; // 모델의 파일명을 삭제 명단에 넣는다.
                            model.Image = null; // 모델의 파일명을 비운다.
                        }
                    }


                    bool? updateBuilding = await FacilityInfoRepository.UpdateFacilityInfo(model);
                    if (updateBuilding == true)
                    {
                        if (files is not null) // 파일이 공백이 아닌경우
                        {
                            if (!String.IsNullOrWhiteSpace(model.Image))
                            {
                                // 파일넣기
                                bool? AddFile = await FileService.AddImageFile(NewFileName, FireFileFolderPath, files);
                            }
                            if (!String.IsNullOrWhiteSpace(deleteFileName))
                            {
                                // 파일삭제
                                bool DeleteFile = FileService.DeleteImageFile(FireFileFolderPath, deleteFileName);
                            }
                        }
                        else // 파일이 공백인 경우
                        {
                            if (!String.IsNullOrWhiteSpace(deleteFileName))
                            {
                                // 삭제할거
                                bool DeleteFile = FileService.DeleteImageFile(FireFileFolderPath, deleteFileName);
                            }
                        }

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

        public async ValueTask<ResponseUnit<bool?>> DeleteFireFacilityService(HttpContext context, List<int> delIdx)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (delIdx is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (string.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]); // 토큰 사업장 검사
                if (string.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? DeleteResult = await FacilityInfoRepository.DeleteFacilityInfo(delIdx, creater);
                if (DeleteResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if (DeleteResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
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

   

  
    }
}
