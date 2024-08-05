using FamTec.Server.Repository.Unit;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.Unit;

namespace FamTec.Server.Services.Unit
{
    public class UnitService : IUnitService
    {
        private readonly IUnitInfoRepository UnitInfoRepository;
        private ILogService LogService;

        public UnitService(IUnitInfoRepository _unitinforepository, ILogService _logservice)
        {
            this.UnitInfoRepository = _unitinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UnitsDTO>> AddUnitService(HttpContext? context, UnitsDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };

                UnitTb? model = new UnitTb();
                model.Unit = dto.Unit;
                model.CreateDt = DateTime.Now;
                model.CreateUser = creator;
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = creator;
                model.PlaceTbId = Int32.Parse(placeidx);

                UnitTb? result = await UnitInfoRepository.AddAsync(model);

                if (result is not null)
                {
                    return new ResponseUnit<UnitsDTO>()
                    {
                        message = "데이터가 정상 처리되었습니다.",
                        data = new UnitsDTO()
                        {
                            Id = result.Id,
                            Unit = result.Unit,
                        },
                        code = 200
                    };
                }
                else
                {
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };
                }
               
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UnitsDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UnitsDTO(), code = 500 };
            }
        }



        /// <summary>
        /// 해당 사업장의 단위리스트 조회
        /// </summary>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<UnitsDTO>?> GetUnitList(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<UnitsDTO>() { message = "잘못된 요청입니다.", data = new List<UnitsDTO>(), code = 404 };

                string? placeidx = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeidx))
                    return new ResponseList<UnitsDTO>() { message = "잘못된 요청입니다.", data = new List<UnitsDTO>(), code = 404 };

                List<UnitTb>? model = await UnitInfoRepository.GetUnitList(Int32.Parse(placeidx));

                if(model is [_, ..])
                {
                    return new ResponseList<UnitsDTO>()
                    {
                        message = "요청이 정상 처리되었습니다.",
                        data = model.Select(e => new UnitsDTO
                        {
                            Id = e.Id,
                            Unit = e.Unit
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<UnitsDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<UnitsDTO>(), code = 200 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<UnitsDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<UnitsDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<string?>> DeleteUnitService(HttpContext? context, int? unitid)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                if (unitid is null)
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<string?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };


                UnitTb? model = await UnitInfoRepository.GetUnitInfo(unitid);

                if(model is not null)
                {
                    model.DelYn = true;
                    model.DelDt = DateTime.Now;
                    model.DelUser = creater;

                    bool? result = await UnitInfoRepository.DeleteUnitInfo(model);
                    if(result == true)
                    {
                        return new ResponseUnit<string?>() { message = "요청이 정상 처리되었습니다.", data = model.Unit, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<string?>() { message = "요청을 처리하지 못하였습니다.", data = model.Unit, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<string?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<string?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 404 };
            }
        }

        /// <summary>
        /// 단위정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UnitsDTO?>> UpdateUnitService(HttpContext? context, UnitsDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<UnitsDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                if (dto is null)
                    return new ResponseUnit<UnitsDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<UnitsDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                UnitTb? model = await UnitInfoRepository.GetUnitInfo(dto.Id);
                if (model is not null)
                {
                    model.Unit = dto.Unit;
                    model.UpdateDt = DateTime.Now;
                    model.UpdateUser = creator;

                    bool? result = await UnitInfoRepository.UpdateUnitInfo(model);

                    if (result != true)
                    {
                        return new ResponseUnit<UnitsDTO?>() { message = "요청을 처리하지 못하였습니다.", data = new UnitsDTO(), code = 200 };
                    }

                    return new ResponseUnit<UnitsDTO?>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                }
                else
                {
                    return new ResponseUnit<UnitsDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<UnitsDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }

        }
    }
}
