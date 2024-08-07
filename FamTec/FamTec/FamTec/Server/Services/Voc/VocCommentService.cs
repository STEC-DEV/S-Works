using FamTec.Server.Repository.User;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Services.Voc
{
    public class VocCommentService : IVocCommentService
    {
        private readonly IVocCommentRepository VocCommentRepository;
        private readonly IVocInfoRepository VocInfoRepository;
        private readonly IUserInfoRepository UserInfoRepository;
        private readonly ILogService LogService;
        private readonly IFileService FileService;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocCommentFileFolderPath;

        public VocCommentService(IVocCommentRepository _voccommentrepository,
            IVocInfoRepository _vocinforepository,
            IUserInfoRepository _userinforepository,
            ILogService _logservice,
            IFileService _fileservice)
        {
            this.VocCommentRepository = _voccommentrepository;
            this.VocInfoRepository = _vocinforepository;
            this.UserInfoRepository = _userinforepository;
            this.LogService = _logservice;
            this.FileService = _fileservice;
        }

        /// <summary>
        /// 민원 댓글 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddVocCommentDTO?>> AddVocCommentService(HttpContext? context, AddVocCommentDTO? dto, List<IFormFile> files)
        {
            try
            {
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

                // VOC관련한 폴더 없으면 만들기
                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, placeId);

                di = new DirectoryInfo(VocCommentFileFolderPath);
                if (!di.Exists) di.Create();

                CommentTb? comment = new CommentTb();
                comment.Content = dto.Content;
                comment.Status = dto.Status;
                comment.CreateDt = DateTime.Now;
                comment.CreateUser = Creater;
                comment.UpdateDt = DateTime.Now;
                comment.UpdateUser = Creater;
                comment.VocTbId = dto.VocTbId;
                comment.UserTbId = Convert.ToInt32(Useridx);

                // 파일이 있으면
                if (files is [_, ..])
                {
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if (i is 0)
                            comment.Image1 = await FileService.AddImageFile(VocCommentFileFolderPath, files[i]);
                        if (i is 1)
                            comment.Image2 = await FileService.AddImageFile(VocCommentFileFolderPath, files[i]);
                        if (i is 2)
                            comment.Image3 = await FileService.AddImageFile(VocCommentFileFolderPath, files[i]);
                    }
                }


                CommentTb? model = await VocCommentRepository.AddAsync(comment);

                if (model is not null)
                {
                    // 등록했으면 원래꺼 상태변경
                    VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId);
                    if (VocTB is not null)
                    {
                        if(model.Status == 2) // 처리완료
                        {
                            VocTB.CompleteDt = model.CreateDt; // 코맨트 등록시간을 VOC 완료시간으로
                            VocTB.DurationDt = (model.CreateDt - VocTB.CreateDt).ToString(); // 댓글등록시간 - VOC 등록시간 ==> 소요시간
                        }

                        VocTB.Status = dto.Status;
                        VocTB.UpdateDt = DateTime.Now;
                        VocTB.UpdateUser = Creater;
                        bool VocUpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB);
                        
                        if (VocUpdateResult)
                        {
                            // 카카오 알림톡 (진행상태가 변경되는거임) - 민원자에게 민원현황 알려주기용
                            // 등록했으면 - 전송 (해당VOC가 Reply -- Y 인경우) + 블랙리스트가 아닌경우
                            /*
                                알림톡 들어올자리
                             */
                            return new ResponseUnit<AddVocCommentDTO?>() { message = "요청이 정상 처리되었습니다.", data = new AddVocCommentDTO(), code = 200 };

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
        public async ValueTask<ResponseList<VocCommentListDTO>> GetVocCommentList(HttpContext? context, int? vocid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = new List<VocCommentListDTO>(), code = 404 };

                if(vocid is null)
                    return new ResponseList<VocCommentListDTO>() { message = "잘못된 요청입니다.", data = new List<VocCommentListDTO>(), code = 404 };

                List<CommentTb>? model = await VocCommentRepository.GetCommentList(vocid);
                if(model is [_, ..])
                {
                    return new ResponseList<VocCommentListDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = model.Select(e => new VocCommentListDTO
                        {
                            Id = e.Id,
                            Comment = e.Content,
                            CreateDT = e.CreateDt,
                            CreateUser = e.CreateUser,
                            VocTbId = e.VocTbId
                        }).ToList(),
                        code = 200
                    };
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
        public async ValueTask<ResponseUnit<VocCommentDetailDTO?>> GetVocCommentDetail(HttpContext? context, int? commentid)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (commentid is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = new VocCommentDetailDTO(), code = 404 };

                CommentTb? model = await VocCommentRepository.GetCommentInfo(commentid);
                if(model is null)
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                VocCommentDetailDTO dto = new VocCommentDetailDTO();
                dto.VocCommentId = model.Id; // VOC 댓글 ID
                dto.Content = model.Content; // VOC 댓글 내용
                dto.Status = model.Status; // VOC 댓글 상태
                dto.CreateDT = model.CreateDt; // VOC 댓글 작성시간
                dto.CreateUser = model.CreateUser; // 댓글 작성자

                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, placeId);
                if (!String.IsNullOrWhiteSpace(model.Image1))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image1);
                    dto.Images.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(model.Image2))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image2);
                    dto.Images.Add(ImageBytes);
                }
                if (!String.IsNullOrWhiteSpace(model.Image3))
                {
                    byte[]? ImageBytes = await FileService.GetImageFile(VocCommentFileFolderPath, model.Image3);
                    dto.Images.Add(ImageBytes);
                }

                return new ResponseUnit<VocCommentDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocCommentDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<bool?>> UpdateCommentService(HttpContext? context, VocCommentDetailDTO? dto, List<IFormFile>? files)
        {
            try
            {
                //string? FileName = String.Empty;
                //string? FileExtenstion = String.Empty;

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

                // 내가 쓴건지 확인
                CommentTb? model = await VocCommentRepository.GetCommentInfo(dto.VocCommentId);
                if(model!.UserTbId != Convert.ToInt32(Useridx))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Content = dto.Content;
                model.Status = dto.Status;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creater;

                // VOC관련한 폴더 경로
                VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, placeId);

                if (files is [_, ..])
                {
                    // 디렉터리에서 파일 삭제
                    if (model.Image1 is not null)
                    {
                        // DB에서 파일 검색해서 삭제
                        bool result = FileService.DeleteImageFile(VocCommentFileFolderPath, model.Image1);
                        if (result)
                            model.Image1 = null;
                    }
                    
                    if (model.Image2 is not null)
                    {
                        // DB에서 파일 검색해서 삭제
                        bool result = FileService.DeleteImageFile(VocCommentFileFolderPath, model.Image2);
                        if(result)
                            model.Image2 = null;
                    }
                    if (model.Image3 is not null)
                    {
                        // DB에서 파일 검색해서 삭제
                        bool result = FileService.DeleteImageFile(VocCommentFileFolderPath, model.Image3);
                        if (result)
                            model.Image3 = null;
                    }

                    // 파일 생성 AND DB Update
                    for (int i = 0; i < files.Count(); i++)
                    {
                        if(i is 0)
                        {
                            model.Image1 = await FileService.AddImageFile(VocCommentFileFolderPath, files[i]);
                        }
                        if(i is 1)
                        {
                            model.Image2 = await FileService.AddImageFile(VocCommentFileFolderPath, files[i]);
                        }
                        if(i is 2)
                        {
                            model.Image3 = await FileService.AddImageFile(VocCommentFileFolderPath, files[i]);
                        }
                    }
                }
                else
                {
                    // 넘어온 파일이 공백인경우 DB에 파일이 있으면 삭제
                    if(model.Image1 is not null)
                    {
                        bool result = FileService.DeleteImageFile(VocCommentFileFolderPath, model.Image1);
                        if (result)
                            model.Image1 = null;
                    }
                    if(model.Image2 is not null)
                    {
                        bool result = FileService.DeleteImageFile(VocCommentFileFolderPath, model.Image2);
                        if (result)
                            model.Image2 = null;
                    }
                    if(model.Image3 is not null)
                    {
                        bool result = FileService.DeleteImageFile(VocCommentFileFolderPath, model.Image3);
                        if (result)
                            model.Image3 = null;
                    }
                }

                bool? UpdateResult = await VocCommentRepository.UpdateCommentInfo(model);
                if(UpdateResult == true)
                {
                    // 여기서 Voc 원본 조회해서 값 변경해줘야함!.
                    VocTb? VocTB = await VocInfoRepository.GetVocInfoById(model.VocTbId);
                    if (VocTB is not null)
                    {
                        VocTB.Status = dto.Status;
                        VocTB.UpdateDt = DateTime.Now;
                        VocTB.UpdateUser = creater;
                        bool VocUpdateResult = await VocInfoRepository.UpdateVocInfo(VocTB);
                        if (VocUpdateResult)
                        {
                            // 카카오 알림톡 (진행상태가 변경되는거임) - 민원자에게 민원현황 알려주기용

                            return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                        }
                        else 
                        {
                            return new ResponseUnit<bool?>() { message = "요청을 처리하지 못하였습니다.", data = true, code = 404 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<bool?>() { message = "요청을 처리하지 못하였습니다.", data = true, code = 404 };
                    }     
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
