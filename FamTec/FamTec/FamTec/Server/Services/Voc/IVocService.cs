using FamTec.Shared.Client.DTO.Normal.Voc;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace FamTec.Server.Services.Voc
{
    public interface IVocService
    {
        /// <summary>
        /// 민원추가
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public ValueTask<ResponseUnit<string>?> AddVocService(string obj, List<IFormFile> image);

        /// <summary>
        /// Voc List 출력
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="placeidx"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public ValueTask<ResponseList<ListVoc>?> GetVocList(HttpContext context, string? date);
    }
}
