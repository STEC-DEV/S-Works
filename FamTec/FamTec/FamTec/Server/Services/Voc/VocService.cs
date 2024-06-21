using FamTec.Server.Repository.Admin.AdminPlaces;
using FamTec.Server.Repository.Alarm;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.User;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Client.DTO;
using FamTec.Shared.Client.DTO.Normal.Users;
using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Http.Features;
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

        public async ValueTask<ResponseList<ListVoc>?> GetVocList(HttpContext? context, string? date)
        {
            if (context is null)
                return null;
            if (date is null)
                return null;

            string? UserType = context.Items["UserType"].ToString();

            if (UserType is null)
            {
                return new ResponseList<ListVoc>()
                {
                    message = "잘못된 요청입니다.",
                    data = new List<ListVoc>(),
                    code = 401
                };
            }

            // 관리자 일때
            if(UserType.Equals("ADMIN"))
            {
                int? AdminIdx = Int32.Parse(context.Items["AdminIdx"].ToString());

                if(AdminIdx is null)
                {
                    return new ResponseList<ListVoc>()
                    {
                        message = "잘못된 요청입니다.",
                        data = new List<ListVoc>(),
                        code = 401
                    };
                }

                List<AdminPlaceTb>? placelist = await AdminPlaceInfoRepository.GetMyWorksList(AdminIdx);

                if(placelist is [_, ..])
                {
                    
                    AdminPlaceTb? adminplace = placelist.FirstOrDefault(m => m.PlaceId == Int32.Parse(context.Items["PlaceIdx"].ToString()));

                    if(adminplace is not null)
                    {
                        PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(adminplace.PlaceId);

                        if(placetb is not null) // 사업장이 있다 -- 실제로직
                        {
                            List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(placetb.Id);
                            
                            // 건물 테이블을 조회해서 --> 건물테이블의 placeid가 매개변수 placeid인 list들을 반환
                            if (buildinglist is [_, ..])
                            {
                                // Voc테이블에 위의 건물테이블의 list들을 던진다.
                                List<ListVoc>? voctb = await VocInfoRepository.GetVocList(buildinglist,date);
                                // VocTb가 왔음.

                                if(voctb is [_, ..])
                                {
                                    return new ResponseList<ListVoc>()
                                    {
                                        message = "요청이 정상 처리되었습니다.",
                                        data = voctb,
                                        code = 200
                                    };
                                }
                                else
                                {
                                    return new ResponseList<ListVoc>()
                                    {
                                        message = "요청이 정상 처리되었습니다.",
                                        data = voctb,
                                        code = 200
                                    };
                                }
                            }
                            else
                            {
                                return new ResponseList<ListVoc>()
                                {
                                    message = "요청이 정상 처리되었습니다.",
                                    data = new List<ListVoc>(),
                                    code = 200
                                };
                            }
                        }
                        else
                        {
                            return new ResponseList<ListVoc>()
                            {
                                message = "잘못된 요청입니다.",
                                data = new List<ListVoc>(),
                                code = 401
                            };
                        }
                    }
                    else
                    {
                        return new ResponseList<ListVoc>()
                        {
                            message = "잘못된 요청입니다.",
                            data = new List<ListVoc>(),
                            code = 401
                        };
                    }
                }
                else
                {
                    return new ResponseList<ListVoc>()
                    {
                        message = "잘못된 요청입니다.",
                        data = new List<ListVoc>(),
                        code = 401
                    };
                }
            }
            else // 일반 유저일때
            {
                int? UserIdx = Int32.Parse(context.Items["UserIdx"].ToString());

                
                int? VocPerm = Int32.Parse(context.Items["User_PermVoc"].ToString());
                

                if (UserIdx is null)
                {
                    return new ResponseList<ListVoc>()
                    {
                        message = "잘못된 요청입니다.",
                        data = new List<ListVoc>(),
                        code = 401
                    };
                }

                // 토큰의 User_PermVoc 0이 아닌거 Check
                if (VocPerm != 0 && VocPerm is not null)
                {
                    UserTb? usermodel = await UserInfoRepository.GetUserIndexInfo(UserIdx);

                    if (usermodel is not null)
                    {
                        if (usermodel.PermVoc != 0) // 실제 디비에 권한이 있는사람
                        {
                            PlaceTb? placetb = await PlaceInfoRepository.GetByPlaceInfo(usermodel.PlaceTbId);

                            if(placetb is not null) // 실제 해당 사업장이 있는지 검사
                            {
                                List<BuildingTb>? buildinglist = await BuildingInfoRepository.GetAllBuildingList(placetb.Id);

                                if(buildinglist is [_, ..]) // 해당사업장의 건물 LIST 출력
                                {
                                    List<ListVoc>? voctb = await VocInfoRepository.GetVocList(buildinglist, date);
                                    
                                    if (voctb is [_, ..])
                                    {
                                        return new ResponseList<ListVoc>()
                                        {
                                            message = "요청이 정상 처리되었습니다.",
                                            data = voctb,
                                            code = 200
                                        };
                                    }
                                    else
                                    {
                                        return new ResponseList<ListVoc>()
                                        {
                                            message = "요청이 정상 처리되었습니다.",
                                            data = voctb,
                                            code = 200
                                        };
                                    }
                                }
                                else
                                {
                                    return new ResponseList<ListVoc>()
                                    {
                                        message = "요청이 정상 처리되었습니다.",
                                        data = new List<ListVoc>(),
                                        code = 200
                                    };
                                }
                            }
                            else
                            {
                                return new ResponseList<ListVoc>()
                                {
                                    message = "잘못된 요청입니다.",
                                    data = new List<ListVoc>(),
                                    code = 401
                                };
                            }
                        }
                        else
                        {
                            return new ResponseList<ListVoc>()
                            {
                                message = "권한이 없습니다",
                                data = new List<ListVoc>(),
                                code = 401
                            };
                        }
                    }
                    else
                    {
                        return new ResponseList<ListVoc>()
                        {
                            message = "잘못된 요청입니다.",
                            data = new List<ListVoc>(),
                            code = 401
                        };
                    }
                }
                else
                {
                    return new ResponseList<ListVoc>()
                    {
                        message = "권한이 없습니다",
                        data = new List<ListVoc>(),
                        code = 401
                    };
                }
            }
        }



        public async ValueTask<ResponseUnit<string>?> AddVocService(string obj, List<IFormFile> image)
        {
            JObject? jobj = JObject.Parse(obj);
            int Voctype = Int32.Parse(jobj["Type"]!.ToString()); // 종류
            string VocName = jobj["Name"]!.ToString(); // 이름
            string VocPhoneNumber = jobj["PhoneNumber"]!.ToString(); // 전화번호
            string VocTitle = jobj["Title"]!.ToString(); // 제목
            string VocContents = jobj["Contents"]!.ToString(); // 내용
            int Vocbuildingidx = Int32.Parse(jobj["buildingidx"]!.ToString()); // 건물 인덱스

            BuildingTb? buildingck = await BuildingInfoRepository.GetBuildingInfo(Vocbuildingidx); // 넘어온 해당 건물이 있는지 먼저 CHECK / 해당 건물이 속해있는 사업장 INDEX 반환

            VocTb? model = new VocTb()
            {
                Type = Voctype, // 종류
                Name = VocName, // 이름
                Phone = VocPhoneNumber, // 전화번호
                Title = VocTitle, // 제목
                Content = VocContents, // 내용
                BuildingTbId = Vocbuildingidx
            };

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
            
            if(buildingck is not null)
            {
                for (int i = 0; i < image.Count(); i++)
                {
                    string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(image[i].FileName)}";
                    string filePath = Path.Combine(CommPath.VocFileImages, newFileName);

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
                    List<UserTb>? userlist;
                    // 알람 발생시키는곳
                    switch (Voctype)
                    {
                        case 1: // 기계
                            userlist = await UserInfoRepository.GetVocMachineList(buildingck.PlaceTbId);

                            if (userlist is [_, ..])
                            {
                                bool AlarmResult = await SetMessage(userlist, VocName, result.Id);
                                if(!AlarmResult)
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


        /// <summary>
        /// VOC 알람메시지 처리
        /// </summary>
        /// <param name="userlist"></param>
        /// <param name="VocName"></param>
        /// <param name="VocTableIdx"></param>
        /// <returns></returns>
        public async ValueTask<bool> SetMessage(List<UserTb>? userlist, string VocName, int VocTableIdx)
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
                        UserTbId = user.Id,
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
