using FamTec.Server.Repository.Material;
using FamTec.Server.Repository.Store;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Store;

namespace FamTec.Server.Services.Store
{
    public class InStoreService : IInStoreService
    {
        private readonly IStoreInfoRepository StoreInfoRepository;
        private readonly IMaterialInfoRepository MaterialInfoRepository;

        private ILogService LogService;

        public InStoreService(IStoreInfoRepository _storeinforepository,
            IMaterialInfoRepository _materialinforepository,
            ILogService _logservice)
        {
            this.StoreInfoRepository = _storeinforepository;
            this.MaterialInfoRepository = _materialinforepository;
            this.LogService = _logservice;
        }

        // 입고
        public async ValueTask<ResponseUnit<AddStoreDTO>?> AddInStoreService(HttpContext? context, AddStoreDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                int? materialId = dto.MaterialID;
                if (materialId is null)
                    return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };



                MaterialTb? material = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), materialId);
                if (material is null)
                    return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                foreach (StoreDTO Store in dto.StoreList)
                {
                    StoreTb model = new StoreTb();
                    model.Inout = 1; // 입고
                    model.Num = Store.Num; // 수량
                    model.UnitPrice = Store.UnitPrice; // 단가
                    model.TotalPrice = Store.Num * Store.UnitPrice; // 입출고 가격
                    model.InoutDate = Store.InOutDate; // 입출고날짜
                    model.CreateDt = DateTime.Now;
                    model.CreateUser = creater;
                    model.UpdateDt = DateTime.Now;
                    model.UpdateUser = creater;
                    model.MaterialTbId = materialId; // 품목ID
                    //model.RoomTbId = Store.RoomID; // 공간ID

                    StoreTb? AddStore = await StoreInfoRepository.AddAsync(model);
                    if(AddStore is null)
                    {
                        return new ResponseUnit<AddStoreDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                    }
                }
                return new ResponseUnit<AddStoreDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddStoreDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    }
}
