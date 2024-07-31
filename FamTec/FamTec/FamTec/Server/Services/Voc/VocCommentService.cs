using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.EMMA;
using FamTec.Client.Pages.Admin.Place.PlaceMain;
using FamTec.Server.Repository.Voc;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Voc;

namespace FamTec.Server.Services.Voc
{
    public class VocCommentService : IVocCommentService
    {
        private readonly IVocCommentRepository VocCommentRepository;
        private readonly ILogService LogService;

        // 파일디렉터리
        private DirectoryInfo? di;
        private string? VocCommentFileFolderPath;

        public VocCommentService(IVocCommentRepository _voccommentrepository, ILogService _logservice)
        {
            this.VocCommentRepository = _voccommentrepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 민원 댓글 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddVocCommentDTO>> AddVocCommentService(HttpContext? context, AddVocCommentDTO? dto, List<IFormFile> files)
        {
            try
            {
                string? FileName = String.Empty;
                string? FileExtenstion = String.Empty;

                if (context is null)
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };

                string? placeId = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeId))
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };

                string? Useridx = Convert.ToString(context.Items["UserIdx"]);
                if (String.IsNullOrWhiteSpace(Useridx))
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };

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
                if(files is [_, ..])
                {
                    for (int i = 0; i < files.Count(); i++)
                    {
                        string newFileName = $"{Guid.NewGuid()}{Path.GetExtension(files[i].FileName)}";
                        string filePath = Path.Combine(VocCommentFileFolderPath, newFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await files[i].CopyToAsync(fileStream);

                            if (i == 0)
                                comment.Image1 = newFileName;
                            if (i == 1)
                                comment.Image2 = newFileName;
                            if (i == 2)
                                comment.Image2 = newFileName;
                        }
                    }
                }

                CommentTb? model = await VocCommentRepository.AddAsync(comment);

                if (model is not null)
                {
                    // 등록했으면 - 전송
                    /*
                        알림톡 들어올자리
                     */
                    return new ResponseUnit<AddVocCommentDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddVocCommentDTO()
                    {
                        Content = model.Content,
                        Status = model.Status,
                        VocTbId = model.VocTbId
                    }, code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddVocCommentDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddVocCommentDTO(), code = 500 };
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
                if(model is not null)
                {
                    VocCommentDetailDTO dto = new VocCommentDetailDTO();
                    dto.VocCommentId = model.Id; // VOC 댓글 ID
                    dto.Content = model.Content; // VOC 댓글 내용
                    dto.Status = model.Status; // VOC 댓글 상태
                    dto.CreateDT = model.CreateDt; // VOC 댓글 작성시간
                    dto.CreateUser = model.CreateUser; // 댓글 작성자
                    dto.Userid = model.UserTbId; // 작성자 인덱스

                    if(!String.IsNullOrWhiteSpace(model.Image1))
                    {
                        VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, placeId);
                        string[] FileList = Directory.GetFiles(VocCommentFileFolderPath);
                        if(FileList is [_, ..])
                        {
                            foreach(var file in FileList)
                            {
                                if(file.Contains(model.Image1))
                                {
                                    string ContentType = Path.GetExtension(model.Image1);
                                    ContentType = ContentType.Substring(1, ContentType.Length - 1);
                                    byte[] ImageBytes = File.ReadAllBytes(file);
                                    dto.Images!.Add($"data:{ContentType};base64,{Convert.ToBase64String(ImageBytes)}");
                                }
                            }
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(model.Image2))
                    {
                        VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, placeId);
                        string[] FileList = Directory.GetFiles(VocCommentFileFolderPath);
                        if (FileList is [_, ..])
                        {
                            foreach (var file in FileList)
                            {
                                if (file.Contains(model.Image2))
                                {
                                    string ContentType = Path.GetExtension(model.Image2);
                                    ContentType = ContentType.Substring(1, ContentType.Length - 1);
                                    byte[] ImageBytes = File.ReadAllBytes(file);
                                    dto.Images!.Add($"data:{ContentType};base64,{Convert.ToBase64String(ImageBytes)}");
                                }
                            }
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(model.Image3))
                    {
                        VocCommentFileFolderPath = String.Format(@"{0}\\{1}\\VocComment", Common.FileServer, placeId);
                        string[] FileList = Directory.GetFiles(VocCommentFileFolderPath);
                        if (FileList is [_, ..])
                        {
                            foreach (var file in FileList)
                            {
                                if (file.Contains(model.Image3))
                                {
                                    string ContentType = Path.GetExtension(model.Image3);
                                    ContentType = ContentType.Substring(1, ContentType.Length - 1);
                                    byte[] ImageBytes = File.ReadAllBytes(file);
                                    dto.Images!.Add($"data:{ContentType};base64,{Convert.ToBase64String(ImageBytes)}");
                                }
                            }
                        }
                    }

                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<VocCommentDetailDTO?>() { message = "조회결과가 없습니다.", data = new VocCommentDetailDTO(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<VocCommentDetailDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }


    }
}
