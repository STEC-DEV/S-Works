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

        public async ValueTask<ResponseUnit<AddMaterialDTO>?> AddMaterialService(HttpContext? context, AddMaterialDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                string? Creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddMaterialDTO>() { message = "잘못된 요청입니다.", data = new AddMaterialDTO(), code = 404 };

                MaterialTb matertialtb = new MaterialTb
                {
                    Name = dto.Name,
                    Unit = dto.Unit,
                    DefaultLocation = dto.Default_Location,
                    Standard = dto.Standard,
                    ManufacturingComp = dto.Mfr,
                    SafeNum = dto.SafeNum,
                    CreateDt = DateTime.Now,
                    CreateUser = Creater,
                    UpdateDt = DateTime.Now,
                    UpdateUser = Creater,
                    PlaceTbId = Int32.Parse(placeidx),
                    BuildingTbId = Convert.ToInt32(dto.BuildingId)
                };

                MaterialTb? model = await MaterialInfoRepository.AddAsync(matertialtb);
                
                if(model is not null)
                {
                    return new ResponseUnit<AddMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddMaterialDTO()
                    {
                        Name = model.Name,
                        Unit = model.Unit,
                        Default_Location = model.DefaultLocation,
                        Standard = model.Standard,
                        SafeNum = model.SafeNum,
                        Mfr = model.ManufacturingComp,
                        BuildingId = model.BuildingTbId
                    }, code = 200 };
                }
                else
                {
                    return new ResponseUnit<AddMaterialDTO>() { message = "요청이 처리되지 않았습니다.", data = new AddMaterialDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddMaterialDTO(), code = 500 };
            }
        }

        public async ValueTask<ResponseList<MaterialListDTO>?> GetPlaceMaterialListService(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaterialListDTO>() { message = "잘못된 요청입니다.", data = new List<MaterialListDTO>(), code = 404 };

                List<MaterialTb>? model = await MaterialInfoRepository.GetPlaceAllMaterialList(Int32.Parse(placeid));

                if (model is [_, ..])
                {
                    return new ResponseList<MaterialListDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = model.Select(e => new MaterialListDTO
                        {
                            ID = e.Id,
                            Name = e.Name,
                            Unit = e.Unit,
                            SafeNum = e.SafeNum,
                            ManufacturingCompany = e.ManufacturingComp,
                            Standard = e.Standard
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<MaterialListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<MaterialListDTO>(), code = 200 };
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaterialListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<MaterialListDTO>(), code = 500 };
            }
        }
    }
}
