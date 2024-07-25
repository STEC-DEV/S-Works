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
        /// 해당 사업장의 선택된 일자의 VOC LIST 반환
        /// </summary>
        /// <param name="context"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<VocDTO>?> GetVocList(HttpContext? context, string? date)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };
                if (date is null)
                    return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                string? UserType = Convert.ToString(context.Items["UserType"]);
                if (String.IsNullOrWhiteSpace(UserType))
                    return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                string? PlaceIdx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(PlaceIdx))
                    return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                // 관리자 일때
                if (UserType.Equals("ADMIN"))
                {
                    // 관리자테이블 인덱스 가져와야함.
                    string? AdminIdx = Convert.ToString(context.Items["AdminIdx"]);
                    if (String.IsNullOrWhiteSpace(AdminIdx))
                        return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                    // 받아온 AdminIdx의 사업장 리스트를 받아온다.
                    List<AdminPlaceTb>? placelist = await AdminPlaceInfoRepository.GetMyWorksList(Int32.Parse(AdminIdx));

                    // 해당 관리자가 사업장을 관리하는지 여부
                    if (placelist is not [_, ..])
                        return new ResponseList<VocDTO>() { message = "해당 관리자의 사업장이 존재하지 않습니다.", data = new List<VocDTO>(), code = 200 };

                    // 그 사업장이 로그인한 관리자가 관리하는 곳인지?
                    AdminPlaceTb? adminplace = placelist.FirstOrDefault(m => m.PlaceTbId == Int32.Parse(PlaceIdx));
                    if (adminplace is null)
                        return new ResponseList<VocDTO>() { message = "해당 관리자는 해당 사업장의 권한이 없습니다.", data = new List<VocDTO>(), code = 401 };

                    // 사업장의 정보를 받아옴.
                    PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(adminplace.PlaceTbId);
                    if (placetb is null)
                        return new ResponseList<VocDTO>() { message = "실제 존재하지 않는 사업장입니다.", data = new List<VocDTO>(), code = 401 };

                    List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(placetb.Id);

                    // 건물 테이블을 조회해서 --> 건물테이블의 placeid가 매개변수 placeid인 list들을 반환
                    if (buildinglist is [_, ..])
                    {
                        // Voc테이블에 위의 건물테이블의 list들을 던진다.
                        List<VocTb>? voclist = await VocInfoRepository.GetVocList(buildinglist, date);

                        if(voclist is [_, ..])
                        {
                            List<VocDTO> dto = (from bd in buildinglist
                                                join voc in voclist
                                                on bd.Id equals voc.BuildingTbId
                                                select new VocDTO
                                                {
                                                    Id = voc.Id, // VOC ID
                                                    Location = bd.Name, // 위치
                                                    Type = voc.Type, // VOC 유형
                                                    Writer = voc.CreateUser, // 작성자
                                                    Status = voc.Status, // VOC 처리상태
                                                    Tel = voc.Phone, // 전화번호
                                                    Title = voc.Title, // 제목
                                                    Content = voc.Content, // 내용
                                                    CreateDT = voc.CreateDt, // 작성일
                                                    CompleteDT = voc.CompleteDt, // 완료일
                                                    DurationDT = voc.DurationDt // 소요시간
                                                }).ToList();
                            // VOC 리스트 반환
                            if (dto is [_, ..])
                                return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                            else
                                return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                        }
                        else
                        {
                            return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<VocDTO>(), code = 200 };
                        }
                    }
                    else
                    {
                        return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<VocDTO>(), code = 200 };
                    }
                }
                else // 일반 유저일때
                {
                    // USERIDX 토큰 검색
                    string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                    if (String.IsNullOrWhiteSpace(UserIdx))
                        return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                    // USER_VOCPERM 토큰 검색
                    string? VocPerm = Convert.ToString(context.Items["UserPerm_Voc"]);
                    if (String.IsNullOrWhiteSpace(VocPerm))
                        return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                    // VOC 권한 검사
                    if (Int32.Parse(VocPerm) == 0)
                        return new ResponseList<VocDTO>() { message = "VOC 권한이 없습니다.", data = new List<VocDTO>(), code = 200 };

                    // 해당 USERIDX 로 유저테이블 검색
                    UsersTb? usermodel = await UserInfoRepository.GetUserIndexInfo(Int32.Parse(UserIdx));
                    if (usermodel is null)
                        return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                    if (usermodel.PermVoc == 0)
                        return new ResponseList<VocDTO>() { message = "VOC 권한이 없습니다.", data = new List<VocDTO>(), code = 401 };

                    // 해당 유저의 사업장 검색
                    PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(usermodel.PlaceTbId);
                    if (placetb is null)
                        return new ResponseList<VocDTO>() { message = "잘못된 요청입니다.", data = new List<VocDTO>(), code = 401 };

                    // 해당 사업장에 포함되어있는 건물 리스트 출력
                    List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(placetb.Id);
                    if (buildinglist is not [_, ..]) // 해당 사업장에 포함된 건물이 없으면
                        return new ResponseList<VocDTO>() { message = "해당 사업장에 속한 건물이 없습니다.", data = new List<VocDTO>(), code = 200 };

                    // VOC LIST 검색
                    List<VocTb>? voclist = await VocInfoRepository.GetVocList(buildinglist, date);

                    if (voclist is [_, ..])
                    {
                        List<VocDTO> dto = (from bd in buildinglist
                                            join voc in voclist
                                            on bd.Id equals voc.BuildingTbId
                                            select new VocDTO
                                            {
                                                Id = voc.Id, // VOC ID
                                                Location = bd.Name, // 위치
                                                Type = voc.Type, // VOC 유형
                                                Writer = voc.CreateUser, // 작성자
                                                Status = voc.Status, // VOC 처리상태
                                                Tel = voc.Phone, // 전화번호
                                                Title = voc.Title,
                                                Content = voc.Content,
                                                CreateDT = voc.CreateDt,
                                                CompleteDT = voc.CompleteDt,
                                                DurationDT = voc.DurationDt
                                            }).ToList();
                        // VOC 리스트 반환
                        if (dto is [_, ..])
                            return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                        else
                            return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                    }
                    else
                    {
                        return new ResponseList<VocDTO>() { message = "요청이 정상 처리되었습니다.", data = new List<VocDTO>(), code = 200 };
                    }
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<VocDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 민원 추가
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string>?> AddVocService(string obj, List<IFormFile> image)
        {
            try
            {
                
                JObject? jobj = JObject.Parse(obj);

                string? Voctype = Convert.ToString(jobj["Type"]);
                if(String.IsNullOrWhiteSpace(Voctype))
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? VocName = Convert.ToString(jobj["Name"]);
                if(String.IsNullOrWhiteSpace(VocName))
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? VocPhoneNumber = Convert.ToString(jobj["PhoneNumber"]);
                if(String.IsNullOrWhiteSpace(VocPhoneNumber))
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? VocTitle = Convert.ToString(jobj["Title"]);
                if(String.IsNullOrWhiteSpace(VocTitle))
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? VocContents = Convert.ToString(jobj["Contents"]);
                if(String.IsNullOrWhiteSpace(VocContents))
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? Vocbuildingidx = Convert.ToString(jobj["buildingidx"]);
                if(String.IsNullOrWhiteSpace(Vocbuildingidx))
                    return new ResponseUnit<string>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                BuildingTb? buildingck = await BuildingInfoRepository.GetBuildingInfo(Int32.Parse(Vocbuildingidx)); // 넘어온 해당 건물이 있는지 먼저 CHECK / 해당 건물이 속해있는 사업장 INDEX 반환

                VocTb? model = new VocTb();
                model.Type = Convert.ToInt32(Voctype); // 종류
                model.CreateUser = VocName; // 이름
                model.Phone = VocPhoneNumber; // 전화번호
                model.Title = VocTitle; // 제목
                model.Content = VocContents; // 내용
                model.BuildingTbId = Int32.Parse(Vocbuildingidx); // 건물 인덱스
               
                if (image is [_, ..])
                {
                    // 확장자 검사
                    for (int i = 0; i < image.Count(); i++)
                    {
                        string tempName = image[i].FileName;
                        string tempextenstion = Path.GetExtension(tempName);

                        string[] allowedExtensions = { ".jpg", ".png", ".bmp", ".jpeg" };

                        if (!allowedExtensions.Contains(tempextenstion))
                            return new ResponseUnit<string>() { message = "파일 형식이 잘못되었습니다.", data = null, code = 200 };
                    }
                }

                if (buildingck is not null)
                {
                   





                    // 이미지 저장
                    for (int i = 0; i < image.Count(); i++)
                    {
                        string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(image[i].FileName)}";
                        string filePath = Path.Combine(Common.VocFileImages, newFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await image[i].CopyToAsync(fileStream);

                            if (i == 0)
                                model.Image1 = newFileName;
                            if (i == 1)
                                model.Image2 = newFileName;
                            if (i == 2)
                                model.Image3 = newFileName;
                        }
                    }
                    
                    VocTb? result = await VocInfoRepository.AddAsync(model);
                    
                    if (result is not null)
                    {
                        List<UsersTb>? userlist;
                        // 알람 발생시키는곳
                        switch (Int32.Parse(Voctype))
                        {
                            case 1: // 기계
                                userlist = await UserInfoRepository.GetVocMachineList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };

                            case 2: // 전기
                                userlist = await UserInfoRepository.GetVocElecList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };

                            case 3: // 승강
                                userlist = await UserInfoRepository.GetVocLiftList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };

                            case 4: // 소방
                                userlist = await UserInfoRepository.GetVocFireList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };

                            case 5: // 건축
                                userlist = await UserInfoRepository.GetVocConstructList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };

                            case 6: // 통신
                                userlist = await UserInfoRepository.GetVocNetWorkList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };

                            case 7: // 미화
                                userlist = await UserInfoRepository.GetVocBeautyList(buildingck.PlaceTbId);
                             
                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                            case 8: // 보안
                                userlist = await UserInfoRepository.GetVocSecurityList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                            case 9: //기타
                                userlist = await UserInfoRepository.GetVocDefaultList(buildingck.PlaceTbId);

                                if (userlist is [_, ..])
                                {
                                    bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                    if (!AlarmResult)
                                    {
                                        return new ResponseUnit<string>() { message = "요청이 처리되지 않았습니다.", data = VocTitle, code = 200 };
                                    }
                                    return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                                }
                                return new ResponseUnit<string>() { message = "요청이 정상 처리되었습니다.", data = VocTitle, code = 200 };
                            default: // 잘못된거
                                return new ResponseUnit<string>() { message = "잘못된 요청입니다.", data = VocTitle, code = 404 };
                        }

                    }
                }
                return new ResponseUnit<string>() { message = "잘못된 요청입니다.", data = VocTitle, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


        /// <summary>
        /// VOC 알람메시지 처리
        /// </summary>
        /// <param name="userlist"></param>
        /// <param name="VocName"></param>
        /// <param name="VocTableIdx"></param>
        /// <returns></returns>
        public async ValueTask<bool> SetMessage(List<UsersTb>? userlist, string VocName, int VocTableIdx)
        {
            if (userlist is [_, ..])
            {
                foreach (var user in userlist)
                {
                    // 사용자 수만큼 Alarm 테이블에 Insert
                    AlarmTb alarm = new AlarmTb()
                    {
                        CreateDt = DateTime.Now,
                        CreateUser = VocName,
                        UpdateDt = DateTime.Now,
                        UpdateUser = VocName,
                        UsersTbId = user.Id,
                        VocTbId = VocTableIdx
                    };

                    AlarmTb? alarm_result = await AlarmInfoRepository.AddAsync(alarm);
                    
                    if(alarm_result is null)
                        return false;
                }
                return true;
            }
            else
                return false;
        }

    }
}
