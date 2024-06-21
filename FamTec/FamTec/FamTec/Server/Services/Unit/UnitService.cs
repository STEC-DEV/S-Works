using FamTec.Server.Repository.Unit;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.Unit;
using Newtonsoft.Json.Linq;

namespace FamTec.Server.Services.Unit
{
    public class UnitService : IUnitService
    {
        private readonly IUnitInfoRepository UnitInfoRepository;

        ResponseOBJ<string> strResponse;
        Func<string, string, int, ResponseModel<string>> FuncResponseSTR;

        public UnitService(IUnitInfoRepository _unitinforepository)
        {
            this.UnitInfoRepository = _unitinforepository;

            strResponse = new ResponseOBJ<string>();
            FuncResponseSTR = strResponse.RESPMessage;
        }

        /// <summary>
        /// 단위정보 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<UnitsDTO>> AddUnitService(HttpContext? context,UnitsDTO? dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };
                if (dto is null)
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };
                
                if (context is not null && dto is not null)
                {
                    UnitTb? model = new UnitTb
                    {
                        Unit = dto.Unit,
                        CreateDt = DateTime.Now,
                        CreateUser = context.Items["Name"].ToString(),
                        UpdateDt = DateTime.Now,
                        UpdateUser = context.Items["Name"].ToString(),
                        PlaceTbId = Int32.Parse(context.Items["PlaceIdx"].ToString())
                    };

                    UnitTb? result = await UnitInfoRepository.AddAsync(model);

                    if(result is not null)
                    {
                        return new ResponseUnit<UnitsDTO>() {
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
                else
                {
                    return new ResponseUnit<UnitsDTO>() { message = "잘못된 요청입니다.", data = new UnitsDTO(), code = 404 };
                }
            }
            catch(Exception ex)
            {
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

                
                List<UnitTb>? model = await UnitInfoRepository.GetUnitList(Int32.Parse(context.Items["PlaceIdx"].ToString()));

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
                return new ResponseList<UnitsDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<UnitsDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 단위정보 삭제
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="sessioninfo"></param>
        /// <returns></returns>
        public async ValueTask<ResponseModel<string>?> DeleteUnitService(UnitsDTO? dto, SessionInfo? sessioninfo)
        {
            try
            {
                if(dto is not null && sessioninfo is not null)
                {
                    UnitTb? model = await UnitInfoRepository.GetUnitInfo(dto.Id);

                    if(model is not null)
                    {
                        model.DelYn = 1;
                        model.DelDt = DateTime.Now;
                        model.DelUser = sessioninfo.Name;

                        bool? result = await UnitInfoRepository.DeleteUnitInfo(model);
                        if(result == true)
                        {
                            return FuncResponseSTR("데이터 삭제 완료.", "1", 200);
                        }
                        else
                        {
                            return FuncResponseSTR("데이터 삭제 실패.", null, 200);
                        }
                    }
                    else
                    {
                        return FuncResponseSTR("데이터가 존재하지 않습니다.", null, 200);
                    }
                }
                else
                {
                    return FuncResponseSTR("요청이 잘못되었습니다.", null, 404);
                }
            }
            catch(Exception ex)
            {
                return FuncResponseSTR("서버에서 요청을 처리하지 못하였습니다.", null, 500);
            }
        }

    }
}
