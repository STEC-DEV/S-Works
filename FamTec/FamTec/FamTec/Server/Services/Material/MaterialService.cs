using FamTec.Server.Repository.Material;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Services.Material
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialInfoRepository MaterialInfoRepository;
        private ILogService LogService;

        public MaterialService(IMaterialInfoRepository _materialinforepository, ILogService _logservice)
        {
            this.MaterialInfoRepository = _materialinforepository;
            this.LogService = _logservice;

        }

        public async ValueTask<ResponseUnit<bool>> AddMaterialService(HttpContext? context, AddMaterialDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                MaterialTb matertialtb = new MaterialTb
                {
                    Name = dto.Name,
                    Unit = dto.Unit,
                    DefaultLocation = dto.Default_Location,
                    Standard = dto.Standard,
                    Mfr = dto.Mfr,
                    SafeNum = dto.SafeNum,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    PlaceTbId = Int32.Parse(placeidx),
                    BuildingTbId = dto.BuildingId
                };

                MaterialTb? model = await MaterialInfoRepository.AddAsync(matertialtb);
                
                if(model is not null)
                {
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "요청이 처리되지 않았습니다.", data = true, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        public ValueTask<ResponseList<MaterialListDTO>?> GetPlaceMaterialListService(HttpContext? context)
        {
            throw new NotImplementedException();
        }
    }
}
