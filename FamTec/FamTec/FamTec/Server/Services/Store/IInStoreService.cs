using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Services.Store
{
    public interface IInStoreService
    {
        public ValueTask<ResponseUnit<AddStoreDTO>?> AddInStoreService(HttpContext? context, AddStoreDTO? dto);
    }
}


// 품목코드
// 품목이름
// 제조사
// 규격
