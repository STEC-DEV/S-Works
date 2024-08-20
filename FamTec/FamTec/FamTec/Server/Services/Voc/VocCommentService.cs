using FamTec.Server.Repository.BlackList;
using FamTec.Server.Repository.Building;
using FamTec.Server.Repository.KakaoLog;
using FamTec.Server.Repository.Place;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.KakaoLog;
using FamTec.Shared.Server.DTO.Voc;
using System.Text;

namespace FamTec.Server.Services.Voc
{
    public class VocCommentService : IVocCommentService
    {
        private readonly IVocCommentRepository VocCommentRepository;
        private readonly IVocInfoRepository VocInfoRepository;
        private readonly IBlackListInfoRepository BlackListInfoRepository;
        private readonly IPlaceInfoRepository PlaceInfoRepository;
        private readonly IKakaoLogInfoRepository KakaoLogInfoRepository;
        private readonly IBuildingInfoRepository BuildingInfoRepository;

        private readonly ILogService LogService;
        private readonly IFileService FileService;
        private readonly IKakaoService KakaoService;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocCommentFileFolderPath;

        public VocCommentService(IVocCommentRepository _voccommentrepository,
            IVocInfoRepository _vocinforepository,
            IBlackListInfoRepository _blacklistinforepository,
            IPlaceInfoRepository _placeinforepository,
            IKakaoLogInfoRepository _kakaologinforepository,
            IBuildingInfoRepository _buildinginforepository,
            IKakaoService _kakaoservice,
            ILogService _logservice,
            IFileService _fileservice)
        {
            this.VocCommentRepository = _voccommentrepository;
            this.VocInfoRepository = _vocinforepository;
            this.BlackListInfoRepository = _blacklistinforepository;
            this.PlaceInfoRepository = _placeinforepository;
            this.KakaoLogInfoRepository = _kakaologinforepository;
            this.BuildingInfoRepository = _buildinginforepository;

            this.KakaoService = _kakaoservice;
            this.LogService = _logservice;
            this.FileService = _fileservice;
        }

        /// <summary>
        /// 민원 댓글 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddVocCommentDTO?>> AddVocCommentService(HttpContext context, AddVocCommentDTO dto, List<IFormFile> files)
        {
            try
            {
                List<string> newFileName = new List<string>();

                if (context is null)
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Useridx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(Useridx))
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                // 파일명 생성
                if (files is not null)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        newFileName.Add(FileService.SetNewFileName(Useridx, files[i]));
                    }
                }

                // VOC관련한 폴더 없으면 만들기
                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, placeId, dto.VocTbId!.Value);

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                CommentTb? comment = new CommentTb();
                comment.Content = dto.Content!;
                comment.Status = dto.Status!.Value;
                comment.CreateDt = DateTime.Now;
                comment.CreateUser = Creater;
                comment.UpdateDt = DateTime.Now;
                comment.UpdateUser = Creater;
                comment.VocTbId = dto.VocTbId!.Value;
                comment.UserTbId = Convert.ToInt32(Useridx);

                // 파일이 있으면
                if (files is [_, ..])
                {
                    // 이미지 저장
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if (i is 0)
                            comment.Image1 = newFileName[i];
                        if (i is 1)
                            comment.Image2 = newFileName[i];
                        if (i is 2)
                            comment.Image3 = newFileName[i];
                    }
                }

                CommentTb? model = await VocCommentRepository.AddAsync(comment);
                if (model is not null)
                {
                    if (files is not null)
                    {
                        for (int i = 0; i < files.Count; i++)
                        {
                            bool? AddFile = await FileService.AddImageFile(newFileName[i], VocCommentFileFolderPath, files[i]);
                        }
                    }

                    // 등록했으면 원래꺼 상태변경
                    VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId);
                    if (VocTB is not null)
                    {
                        if(model.Status == 2) // 처리완료
                        {
                            VocTB.CompleteDt = model.CreateDt; // 코맨트 등록시간을 VOC 완료시간으로
                            VocTB.DurationDt = (model.CreateDt - VocTB.CreateDt).ToString(); // 댓글등록시간 - VOC 등록시간 ==> 소요시간
                        }

                        VocTB.Status = dto.Status.Value;
                        VocTB.UpdateDt = DateTime.Now;
                        VocTB.UpdateUser = Creater;
                        bool VocUpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB);
                        
                        if (VocUpdateResult)
                        {
                            // 만약 처리결과를 받는다고 했었으면.
                            if(VocTB.ReplyYn == true)
                            {
                                BlacklistTb? BlackListTB = await BlackListInfoRepository.GetBlackListInfo(VocTB.Phone);

                                if(BlackListTB is null) // 블랙리스트가 아니면
                                {
                                    PlaceTb? placeTB = await PlaceInfoRepository.GetBuildingPlace(VocTB.BuildingTbId);

                                    if (placeTB is null)
                                        return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                                    // 전화번호
                                    string receiver = VocTB.Phone!;

                                    // 사업장 전화번호
                                    string placetel = placeTB.Tel!;

                                    // URL 파라미터 인코딩
                                    byte[] bytes = Encoding.Unicode.GetBytes(VocTB.Id.ToString());
                                    string base64 = Convert.ToBase64String(bytes);

                                    string url = $"https://123.2.156.148/vocinfo?vocid={base64}";

                                    string StatusResult = string.Empty;
                                    if(model.Status == 1)
                                    {
                                        StatusResult = "처리중";
                                    }
                                    else if(model.Status == 2)
                                    {
                                        StatusResult = "처리완료";
                                    }

                                    // 카카오 API 전송
                                    AddKakaoLogDTO? LogDTO = await KakaoService.UpdateVocAnswer(VocTB.Code!, StatusResult, receiver, url, placetel);

                                    if(LogDTO is not null)
                                    {
                                        // 카카오 메시지 성공
                                        KakaoLogTb LogTB = new KakaoLogTb();
                                        LogTB.Code = LogDTO.Code!;
                                        LogTB.Message = LogDTO.Message!;
                                        LogTB.CreateDt = DateTime.Now;
                                        LogTB.CreateUser = Creater;
                                        LogTB.UpdateDt = DateTime.Now;
                                        LogTB.UpdateUser = Creater;
                                        LogTB.VocTbId = VocTB.Id; // 민원ID
                                        LogTB.PlaceTbId = placeTB.Id; // 사업장ID
                                        LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물ID

                                        // 카카오API 로그 테이블에 쌓아야함
                                        KakaoLogTb? LogResult = await KakaoLogInfoRepository.AddAsync(LogTB);
                                    }
                                    else
                                    {
                                        // 카카오 메시지 에러
                                        KakaoLogTb LogTB = new KakaoLogTb();
                                        LogTB.Code = "ERROR";
                                        LogTB.Message = "ERROR";
                                        LogTB.CreateDt = DateTime.Now;
                                        LogTB.CreateUser = Creater;
                                        LogTB.UpdateDt = DateTime.Now;
                                        LogTB.UpdateUser = Creater;
                                        LogTB.VocTbId = VocTB.Id; // 민원ID
                                        LogTB.PlaceTbId = placeTB.Id; // 사업장ID
                                        LogTB.BuildingTbId = VocTB.BuildingTbId; // 건물ID

                                        // 카카오API 로그 테이블에 쌓아야함
                                        KakaoLogTb? LogResult = await KakaoLogInfoRepository.AddAsync(LogTB);
                                    }
                                }
                            }

                            return new ResponseUnit<AddVocCommentDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto , code = 200 };
                        }
                        else
                        {
                            return new ResponseUnit<AddVocCommentDTO?>() { message = "요청을 처리하지 못하였습니다.", data = null, code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<AddVocCommentDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddVocCommentDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddVocCommentDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 민원에 대한 댓글 리스트 조회
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<VocCommentListDTO>> GetVocCommentList(HttpContext context, int vocid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(vocid);
                if (VocTB is null)
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = null, code = 404 };

                List<CommentTb>? model = await VocCommentRepository.GetCommentList(vocid);
                if(model is [_, ..])
                {
                    BuildingTb? BuildingTB = await BuildingInfoRepository.GetBuildingInfo(VocTB.BuildingTbId);
                    if(BuildingTB is null)
                        return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                    VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, BuildingTB!.PlaceTbId, VocTB.Id);

                    List<VocCommentListDTO> dto = new List<VocCommentListDTO>();
                    for (int i = 0; i < model.Count; i++)
                    {
                        VocCommentListDTO dtoModel = new VocCommentListDTO();
                        dtoModel.Id = model[i].Id;
                        dtoModel.status = model[i].Status; // 댓글상태
                        dtoModel.Comment = model[i].Content; // 내용
                        dtoModel.CreateDT = model[i].CreateDt.ToString("yyyy-MM-dd HH:mm:ss");
                        dtoModel.CreateUser = model[i].CreateUser;
                        dtoModel.VocTbId = model[i].VocTbId;

                        if (!String.IsNullOrWhiteSpace(model[i].Image1))
                        {
                            byte[]? images = await FileService.GetImageFile(VocCommentFileFolderPath, model[i].Image1);
                            dtoModel.Images.Add(images);
                        }

                        if (!String.IsNullOrWhiteSpace(model[i].Image2))
                        {
                            byte[]? images = await FileService.GetImageFile(VocCommentFileFolderPath, model[i].Image2);
                            dtoModel.Images.Add(images);
                        }

                        if (!String.IsNullOrWhiteSpace(model[i].Image3))
                        {
                            byte[]? images = await FileService.GetImageFile(VocCommentFileFolderPath, model[i].Image3);
                            dtoModel.Images.Add(images);
                        }
                        dto.Add(dtoModel);
                    }

                    return new ResponseList<VocCommentListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseList<VocCommentListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<VocCommentListDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<VocCommentListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<VocCommentListDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 해당 민원에 대한 댓글 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="commentid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(HttpContext context, int commentid)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                CommentTb? model = await VocCommentRepository.GetCommentInfo(commentid);
                if(model is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId);
                if (VocTB is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocCommentDetailDTO dto = new VocCommentDetailDTO();
                dto.VocCommentId = model.Id; // VOC 댓글 ID
                dto.Content = model.Content; // VOC 댓글 내용
                dto.Status = model.Status; // VOC 댓글 상태
                dto.CreateDT = model.CreateDt; // VOC 댓글 작성시간
                dto.CreateUser = model.CreateUser; // 댓글 작성자

                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, placeId, VocTB.Id);
                if (!String.IsNullOrWhiteSpace(model.Image1))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image1);
                    dto.Images!.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(model.Image2))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image2);
                    dto.Images!.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(model.Image3))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image3);
                    dto.Images!.Add(ImageBytes);
                }

                return new ResponseUnit<VocCommentDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocCommentDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 차후 바꿈
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateCommentService(HttpContext context, VocCommentDetailDTO dto, List<IFormFile>? files)
        {
            try
            {
                List<string> NewFileName = new List<string>();
                List<string> deleteFileName = new List<string>();

                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? Useridx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(Useridx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (files is [_, ..]) // 넘어온 파일이 있다면
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        NewFileName.Add(FileService.SetNewFileName(Useridx, files[i])); // 새로운 파일명 생성
                    }
                }

                // 내가 쓴건지 확인
                CommentTb? model = await VocCommentRepository.GetCommentInfo(dto.VocCommentId!.Value);
                if(model!.UserTbId != Convert.ToInt32(Useridx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Content = dto.Content!;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creater;

                // VOC관련한 폴더 경로
                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\Voc\\{2}\\VocComment", Common.FileServer, placeId, model.VocTbId);

                if(files is not null) // 파일이 공백이 아닌 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image1)) // DB에 파일이 있을 경우
                    {
                        deleteFileName.Add(model.Image1); // 삭제할 이름을 넣는다.
                    }
                    if(!String.IsNullOrWhiteSpace(model.Image2))
                    {
                        deleteFileName.Add(model.Image2); // 삭제할 이름을 넣는다.
                    }
                    if(!String.IsNullOrWhiteSpace(model.Image3))
                    {
                        deleteFileName.Add(model.Image3); // 삭제할 이름을 넣는다.
                    }

                    for (int i = 0; i < files.Count(); i++) // 새 파일명을 넣는다.
                    {
                        if(i is 0)
                        {
                            model.Image1 = NewFileName[i];
                        }
                        if(i is 1)
                        {
                            model.Image2 = NewFileName[i];
                        }
                        if(i is 2)
                        {
                            model.Image3 = NewFileName[i];
                        }
                    }
                }
                else // 파일이 공백인 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image1))
                    {
                        deleteFileName.Add(model.Image1); // 모델의 파일명을 삭제 명단에 넣는다.
                        model.Image1 = null; // 모델의 파일명을 비운다.
                    }
                    if (!String.IsNullOrWhiteSpace(model.Image2)) 
                    {
                        deleteFileName.Add(model.Image2); // 모델의 파일명을 삭제 명단에 넣는다.
                        model.Image2 = null; // 모델의 파일명을 비운다
                    }
                    if (!String.IsNullOrWhiteSpace(model.Image3))
                    {
                        deleteFileName.Add(model.Image3); // 모델의 파일명을 삭제 명단에 넣는다.
                        model.Image3 = null; // 모델의 파일명을 비운다.
                    }
                }

                bool? UpdateResult = await VocCommentRepository.UpdateCommentInfo(model);
                if(UpdateResult == true)
                {
                    // 파일이 공백이 아닌경우
                    if (files is not null)
                    {
                        for (int i = 0; i < files.Count(); i++) // 파일 넣는다.
                        {
                            if(i is 0)
                            {
                                bool? AddFile = await FileService.AddImageFile(model.Image1!, VocCommentFileFolderPath, files[i]);
                            }
                            if(i is 1)
                            {
                                bool? AddFile = await FileService.AddImageFile(model.Image2!, VocCommentFileFolderPath, files[i]);
                            }
                            if(i is 2)
                            {
                                bool? AddFile = await FileService.AddImageFile(model.Image3!, VocCommentFileFolderPath, files[i]);
                            }
                        }

                        for (int i = 0; i < deleteFileName.Count; i++) // 삭제할거
                        {
                            bool DeleteFile = FileService.DeleteImageFile(VocCommentFileFolderPath, deleteFileName[i]); // 삭제
                        }
                    }
                    else // 파일이 공백인 경우
                    {
                        for (int i = 0; i < deleteFileName.Count; i++) // 삭제
                        {
                            bool DeleteFile = FileService.DeleteImageFile(VocCommentFileFolderPath, deleteFileName[i]);
                        }
                    }
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "요청을 처리하지 못하였습니다.", data = true, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}