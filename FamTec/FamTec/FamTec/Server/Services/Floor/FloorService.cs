using FamTec.Server.Repository.Floor;
using FamTec.Server.Repository.Place;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Floor;
using System.Collections.Generic;

namespace FamTec.Server.Services.Floor
{
    public class FloorService : IFloorService
    {
        private readonly IFloorInfoRepository FloorInfoRepository;


        ResponseOBJ<string> strResponse;
        Func<string, string, int, ResponseModel<string>> FuncResponseSTR;

        public FloorService(IFloorInfoRepository _floorinforepository)
        {
            this.FloorInfoRepository = _floorinforepository;

            strResponse = new ResponseOBJ<string>();
            FuncResponseSTR = strResponse.RESPMessage;

        }

        /// <summary>
        /// 건물에 속해있는 층 리스트 반환
        /// </summary>
        /// <param name="buildingtbid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<FloorDTO>?> GetFloorListService(int? buildingtbid)
        {
            try
            {
                if(buildingtbid is not null)
                {
                    List<FloorTb>? model = await FloorInfoRepository.GetFloorList(buildingtbid);

                    if(model is [_, ..])
                    {
                        return new ResponseList<FloorDTO>()
                        {
                            message = "요청이 정상 처리되었습니다.",
                            data = model.Select(e => new FloorDTO
                            {
                                FloorID = e.Id,
                                Name = e.Name,
                                BuildingTBID = e.BuildingTbId
                            }).ToList(),
                            code = 200
                        };
                    }
                    else
                    {
                        return new ResponseList<FloorDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<FloorDTO>(), code = 200 };
                    }
                }
                else
                {
                    return new ResponseList<FloorDTO>() { message = "잘못된 요청입니다.", data = new List<FloorDTO>(), code = 404 };
                }
            }
            catch(Exception ex)
            {
                return new ResponseList<FloorDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<FloorDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 층삭제
        /// </summary>
        /// <param name="index"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseModel<string>?> DeleteFloorService(List<int>? index, SessionInfo? session)
        {
            try
            {
                if(index is [_, ..])
                {
                    int count = 0;

                    for (int i = 0; i < index.Count; i++)
                    {
                        FloorTb? model = await FloorInfoRepository.GetFloorInfo(index[i]);

                        if(model is not null)
                        {
                            model.DelYn = true;
                            model.DelDt = DateTime.Now;
                            model.DelUser = session.Name;

                            bool? result = await FloorInfoRepository.DeleteFloorInfo(model);

                            if(result == true)
                            {
                                count++;
                            }
                        }
                    }
                    return FuncResponseSTR("데이터 삭제 완료", count.ToString(), 200);
                }
                else
                {
                    return FuncResponseSTR("잘못된 요청 입니다.", null, 404);
                }
            }
            catch(Exception ex)
            {
                return FuncResponseSTR("서버에서 요청을 처리하지 못하였습니다.", null, 500);
            }
        }
    }
}





