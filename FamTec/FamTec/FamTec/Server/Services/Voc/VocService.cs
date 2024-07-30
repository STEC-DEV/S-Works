using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Alarm;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace FamTec.Server.Services.Voc
{
    public class VocService : IVocService
    {
        private readonly IVocInfoRpeository VocInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;
        private readonly IUserInfoRepository UserInfoRepository;
        private readonly IAlarmInfoRepository AlarmInfoRepository;
        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IAdminPlacesInfoRepository AdminPlaceInfoRepository;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocFileFolderPath;

        private ILogService LogService;

        public VocService(IVocInfoRpeository _vocinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IUserInfoRepository _userinforepository,
            IAlarmInfoRepository _alarminforepository,
            IAdminPlacesInfoRepository _adminplaceinforepository,
            IPlaceInfoRepository _placeinforepository,
            ILogService _logservice)
        {
            this.VocInfoRepository = _vocinforepository;
            this.BuildingInfoRepository = _buildinginforepository;
            this.UserInfoRepository = _userinforepository;
            this.AlarmInfoRepository = _alarminforepository;
            this.AdminPlaceInfoRepository = _adminplaceinforepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.LogService = _logservice;
        }


        /// <summary>
        /// 민원 추가
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> AddVocService(AddVocDTO? dto, List<IFormFile>? files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;


                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if(String.IsNullOrWhiteSpace(dto.Title))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if(String.IsNullOrWhiteSpace(dto.Contents))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (String.IsNullOrWhiteSpace(dto.Name))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto.Buildingid is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto.Placeid is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };


                BuildingTb? buildingtb = await BuildingInfoRepository.GetBuildingInfo(dto.Buildingid);
                if(buildingtb is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // VOC관련한 폴더 없으면 만들기
                VocFileFolderPath = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, dto.Placeid);

                di = new DirectoryInfo(VocFileFolderPath);
                if (!di.Exists) di.Create();


                VocTb? model = new VocTb();
                model.Title = dto.Title;
                model.Content = dto.Contents;
                model.Type = 0; // 처음은 미분류
                model.Phone = dto.PhoneNumber; // 전화번호;
                model.Status = 0; // 미처리

                if (dto.PhoneNumber is null) // 전화번호 있으면 회신True / 없으면False
                    model.ReplyYn = false;
                else
                    model.ReplyYn = true;
                model.CreateDt = DateTime.Now;
                model.CreateUser = dto.Name; // 작성자
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = dto.Name; // 작성자
                model.BuildingTbId = dto.Buildingid;

                if (files is [_, ..])
                {
                    // 이미지 저장
                    for (int i = 0; i < files.Count(); i++)
                    {
                        string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                        string filePath = Path.Combine(VocFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await files[i].CopyToAsync(fileStream);

                            if (i == 0)
                                model.Image1 = newFileName;
                            if (i == 1)
                                model.Image2 = newFileName;
                            if (i == 2)
                                model.Image3 = newFileName;
                        }
                    }
                }

                VocTb? result = await VocInfoRepository.AddAsync(model);
                if(result is not null)
                {
                    List<UsersTb>? userlist = await UserInfoRepository.GetVocDefaultList(buildingtb.PlaceTbId);
                    if(userlist is [_, ..])
                    {
                        bool AlarmResult = await SetMessage(userlist, dto.Name, result.Id);
                        if(AlarmResult)
                        {
                            // 성공
                            return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                        }
                        else
                        {
                            // 실패
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                        }
                    }
                    else // 보낼대상이 없음
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }
                    
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

     

        /// <summary>
        /// 해당 사업장의 선택된 일자의 VOC LIST 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<VocListDTO>?> GetVocList(HttpContext? context, DateTime? startdate, DateTime? enddate, int? type, int? status,int? buildingid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (startdate is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (enddate is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (type is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (status is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };
                if (buildingid is null)
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<VocListDTO>() { message = "잘못된 요청입니다.", data = new List<VocListDTO>(), code = 404 };


                List<VocListDTO>? model = await VocInfoRepository.GetVocList(Convert.ToInt32(PlaceIdx), startdate, enddate, type, status, buildingid);
                if (model is [_, ..])
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<VocListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
             
                
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<VocListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocListDTO>(), code = 500 };
            }
        }

      


        /// <summary>
        /// VOC 알람메시지 처리
        /// </summary>
        /// <param name="userlist"></param>
        /// <param name="VocName"></param>
        /// <param name="VocTableIdx"></param>
        /// <returns></returns>
        public async ValueTask<bool> SetMessage(List<UsersTb>? userlist, string Creater, int VocTableIdx)
        {
            try
            {
                if (userlist is [_, ..])
                {
                    foreach (var user in userlist)
                    {
                        // 사용자 수만큼 Alarm 테이블에 Insert
                        AlarmTb alarm = new AlarmTb()
                        {
                            CreateDt = DateTime.Now,
                            CreateUser = Creater,
                            UpdateDt = DateTime.Now,
                            UpdateUser = Creater,
                            UsersTbId = user.Id,
                            VocTbId = VocTableIdx
                        };

                        AlarmTb? alarm_result = await AlarmInfoRepository.AddAsync(alarm);
                        if (alarm_result is null)
                            return false;
                    }
                    return true;
                }
                else
                    return true;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return false;
            }
        }


        /// <summary>
        /// VOC 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<VocDetailDTO?>> GetVocDetail(HttpContext? context, int? vocid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<VocDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (vocid is null)
                    return new ResponseUnit<VocDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseUnit<VocDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? model = await VocInfoRepository.GetDetailVoc(vocid);
                if(model is not null)
                {
                    BuildingTb? building = await BuildingInfoRepository.GetBuildingInfo(model.BuildingTbId);
                    if(building is not null)
                    {
                        VocDetailDTO dto = new VocDetailDTO();
                        dto.Id = model.Id; // 민원 인덱스
                        dto.CreateDT = model.CreateDt; // 민원 신청일
                        dto.Status = model.Status; // 민원상태
                        dto.BuildingName = building.Name; // 건물명
                        dto.Type = model.Type; // 민원유형
                        dto.Title = model.Title; // 민원제목
                        dto.Contents = model.Content; // 민원내용
                        dto.CreateUser = model.CreateUser; // 민원인
                        dto.Phone = model.Phone; // 민원인 전화번호
                        dto.ReplyYN = model.ReplyYn; // 처리결과 반환유무

                        if(!String.IsNullOrWhiteSpace(model.Image1))
                        {
                            string VocFileName = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, PlaceIdx);
                            string[] FileList = Directory.GetFiles(VocFileName);
                            if(FileList is [_, ..])
                            {
                                foreach(var file in FileList)
                                {
                                    if(file.Contains(model.Image1))
                                    {
                                        string ContentType = Path.GetExtension(model.Image1);
                                        ContentType = ContentType.Substring(1, ContentType.Length - 1);
                                        byte[] ImageBytes = File.ReadAllBytes(file);
                                        dto.Images.Add($"data:{ContentType};base64,{Convert.ToBase64String(ImageBytes)}");
                                    }
                                }
                            }
                        }
                        
                        if (!String.IsNullOrWhiteSpace(model.Image2))
                        {
                            string VocFileName = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, PlaceIdx);
                            string[] FileList = Directory.GetFiles(VocFileName);
                            if (FileList is [_, ..])
                            {
                                foreach (var file in FileList)
                                {
                                    if (file.Contains(model.Image2))
                                    {
                                        string ContentType = Path.GetExtension(model.Image2);
                                        ContentType = ContentType.Substring(1, ContentType.Length - 1);
                                        byte[] ImageBytes = File.ReadAllBytes(file);
                                        dto.Images.Add($"data:{ContentType};base64,{Convert.ToBase64String(ImageBytes)}");
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrWhiteSpace(model.Image3))
                        {
                            string VocFileName = String.Format(@"{0}\\{1}\\Voc", Common.FileServer, PlaceIdx);
                            string[] FileList = Directory.GetFiles(VocFileName);
                            if (FileList is [_, ..])
                            {
                                foreach (var file in FileList)
                                {
                                    if (file.Contains(model.Image3))
                                    {
                                        string ContentType = Path.GetExtension(model.Image3);
                                        ContentType = ContentType.Substring(1, ContentType.Length - 1);
                                        byte[] ImageBytes = File.ReadAllBytes(file);
                                        dto.Images.Add($"data:{ContentType};base64,{Convert.ToBase64String(ImageBytes)}");
                                    }
                                }
                            }
                        }
                        

                        return new ResponseUnit<VocDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<VocDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                    }
                }
                else
                {
                    return new ResponseUnit<VocDetailDTO?>() { message = "데이터가 존재하지 않습니다.", data = null, code = 200 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
