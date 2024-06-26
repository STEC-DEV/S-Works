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
        public async ValueTask<ResponseUnit<AddVocCommentDTO>> AddVocCommentService(HttpContext? context, AddVocCommentDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddVocCommentDTO>() { message = "잘못된 요청입니다.", data = new AddVocCommentDTO(), code = 404 };

                CommentTb? comment = new CommentTb()
                {
                    Content = dto.Content,
                    Status = dto.Status,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    VocTbid = dto.VocTbId
                };

                CommentTb? model = await VocCommentRepository.AddAsync(comment);

                if (model is not null)
                {
                    return new ResponseUnit<AddVocCommentDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddVocCommentDTO()
                    {
                        Content = model.Content,
                        Status = model.Status,
                        VocTbId = model.VocTbid
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
        /// 해당 민원의 댓글 상세정보
        /// </summary>
        /// <param name="vocid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<VocCommentListDTO>> GetVocCommentList(int? vocid)
        {
            try
            {
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
                            VocTbId = e.VocTbid
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
    }
}
