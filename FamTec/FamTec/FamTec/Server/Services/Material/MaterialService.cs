using FamTec.Server.Repository.Inventory;
using FamTec.Server.Repository.Material;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Services.Material
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialInfoRepository MaterialInfoRepository;
        private readonly IInventoryInfoRepository InventoryInfoRepository;
        private IFileService FileService;
        private ILogService LogService;

        private DirectoryInfo? di;
        private string? MaterialFileFolderPath;

        public MaterialService(IMaterialInfoRepository _materialinforepository,
            IInventoryInfoRepository _inventoryinforepository,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.MaterialInfoRepository = _materialinforepository;
            this.InventoryInfoRepository = _inventoryinforepository;
            this.FileService = _fileservice;
            this.LogService = _logservice;

        }
        
        /// <summary>
        /// 자재추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddMaterialDTO>?> AddMaterialService(HttpContext? context, AddMaterialDTO? dto, IFormFile? files)
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

                MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeidx);

                di = new DirectoryInfo(MaterialFileFolderPath);
                if (!di.Exists) di.Create();

                MaterialTb matertialtb = new MaterialTb();
                matertialtb.Name = dto.Name; // 자재명
                matertialtb.Unit = dto.Unit; // 단위
                matertialtb.Standard = dto.Standard; // 규격
                matertialtb.ManufacturingComp = dto.ManufacturingComp; // 제조사
                matertialtb.SafeNum = dto.SafeNum; // 안전재고수량
                matertialtb.DefaultLocation = dto.DefaultLocation; // 공간위치 인덱스
                matertialtb.CreateDt = DateTime.Now;
                matertialtb.CreateUser = Creater;
                matertialtb.UpdateDt = DateTime.Now;
                matertialtb.UpdateUser = Creater;
                matertialtb.PlaceTbId = Int32.Parse(placeidx); // 사업장ID

                if(files is not null)
                {
                    matertialtb.Image = await FileService.AddImageFile(MaterialFileFolderPath, files);
                }
                else
                {
                    matertialtb.Image = null;
                }
               
                MaterialTb? model = await MaterialInfoRepository.AddAsync(matertialtb);
                
                if(model is not null)
                {
                    return new ResponseUnit<AddMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = new AddMaterialDTO()
                    {
                        Name = model.Name, // 자재명
                        Unit = model.Unit, // 단위
                        Standard = model.Standard, // 규격
                        ManufacturingComp = model.ManufacturingComp, // 제조사
                        SafeNum = model.SafeNum, // 안전재고수량
                        DefaultLocation = model.DefaultLocation, // 기본위치
                    }, code = 200 };
                }
                else
                {
                    if(!String.IsNullOrWhiteSpace(matertialtb.Image))
                    {
                        bool result = FileService.DeleteImageFile(MaterialFileFolderPath, matertialtb.Image); // 파일삭제
                    }
                    return new ResponseUnit<AddMaterialDTO>() { message = "요청이 처리되지 않았습니다.", data = new AddMaterialDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddMaterialDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 출력
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
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
                            Standard = e.Standard,
                            ManufacturingComp = e.ManufacturingComp,
                            SafeNum = e.SafeNum
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<MaterialListDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<MaterialListDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaterialListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<MaterialListDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 자재 상세정보 보기
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<ResponseUnit<DetailMaterialDTO>?> GetDetailMaterialService(HttpContext? context, int? materialid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                if (materialid is null)
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailMaterialDTO>() { message = "잘못된 요청입니다.", data = new DetailMaterialDTO(), code = 404 };

                MaterialTb? model = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), materialid);
                if(model is not null)
                {
                    DetailMaterialDTO dto = new DetailMaterialDTO();
                    dto.Id = model.Id; // 품목 ID
                    dto.Name = model.Name; // 품목명
                    dto.Unit = model.Unit; // 품목단위
                    dto.Standard = model.Standard; // 규격
                    dto.ManufacturingComp = model.ManufacturingComp; // 제조사
                    dto.SafeNum = model.SafeNum; // 안전재고 수량
                    dto.RoomID = model.DefaultLocation; // 기본위치

                    MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);

                    if(!String.IsNullOrWhiteSpace(model.Image))
                    {
                        dto.Image = await FileService.GetImageFile(MaterialFileFolderPath, model.Image);
                    }

                    return new ResponseUnit<DetailMaterialDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<DetailMaterialDTO>() { message = "데이터가 존재하지 않습니다.", data = new DetailMaterialDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailMaterialDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DetailMaterialDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<ResponseUnit<bool?>> UpdateMaterialService(HttpContext? context, UpdateMaterialDTO? dto, IFormFile? files)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                MaterialTb? model = await MaterialInfoRepository.GetDetailMaterialInfo(Int32.Parse(placeid), dto.Id);
                if(model is not null)
                {
                    model.Name = dto.Name; // 품목명
                    model.Unit = dto.Unit; // 단위
                    model.Standard = dto.Standard; // 규격
                    model.ManufacturingComp = dto.ManufacturingComp; // 제조사
                    model.SafeNum = dto.SafeNum; // 안전재고수량
                    model.DefaultLocation = dto.RoomID; // 공간위치
                    model.UpdateDt = DateTime.Now;
                    model.UpdateUser = creater;

                    MaterialFileFolderPath = String.Format(@"{0}\\{1}\\Material", Common.FileServer, placeid);
                    
                    if(files is not null)
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는 경우
                        {
                            bool result = FileService.DeleteImageFile(MaterialFileFolderPath, model.Image);
                            model.Image = await FileService.AddImageFile(MaterialFileFolderPath, files);
                        }
                        else // DB엔 없는 경우
                        {
                            model.Image = await FileService.AddImageFile(MaterialFileFolderPath, files);
                        }
                    }
                    else // 파일이 공백인 경우
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는 경우
                        {
                            bool result = FileService.DeleteImageFile(MaterialFileFolderPath, model.Image);
                            if (result)
                                model.Image = null;
                        }
                    }

                    bool? updateMaterial = await MaterialInfoRepository.UpdateMaterialInfo(model);
                    if(updateMaterial == true)
                    {
                        return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                    }
                    else
                    {
                        if (!String.IsNullOrWhiteSpace(model.Image))
                        {
                            bool result = FileService.DeleteImageFile(MaterialFileFolderPath, model.Image);
                        }

                        return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }


        /// <summary>
        /// 품목 삭제
        /// </summary>
        /// <param name="delIdx"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteMaterialService(HttpContext? context, List<int>? delIdx)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (delIdx is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (delIdx.Count() == 0)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                for (int i = 0; i < delIdx.Count(); i++)
                {
                    List<InventoryTb>? Inventory = await InventoryInfoRepository.GetPlaceMaterialInventoryList(Convert.ToInt32(placeid), delIdx[i]);

                    if (Inventory is [_, ..])
                    {
                        List<InventoryTb>? CheckModel = Inventory.Where(m => m.Num > 0).ToList();
                        if(CheckModel is [_, ..])
                        {
                            // 조건걸림
                            return new ResponseUnit<bool?>() { message = "남아있는 재고가 있는 품목이 있어 삭제가 불가능합니다.", data = null, code = 200 };
                        }
                    }
                }

                //  품목삭제 하면됨
                bool? DeleteResult = await MaterialInfoRepository.DeleteMaterialInfo(delIdx, creater);
                if(DeleteResult == true)
                {
                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else if(DeleteResult == false)
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 };
            }
        }

        
    }
}
