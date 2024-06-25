using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Room;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Facility;
using FamTec.Shared.Server.DTO.Floor;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Services.Facility
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        private readonly IRoomInfoRepository RoomInfoRepository;

        public FacilityService(IFacilityInfoRepository _facilityinforepository, IRoomInfoRepository _roominforepository)
        {
            this.FacilityInfoRepository = _facilityinforepository;
            this.RoomInfoRepository = _roominforepository;
        }

        /// <summary>
        /// 설비 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddFacilityDTO>?> AddFacilityService(HttpContext? context, AddFacilityDTO? dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                if (dto is null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                RoomTb? tokenck = await RoomInfoRepository.GetRoomInfo(dto.RoomTbId);
                if (tokenck is null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                string? creator = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creator))
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };

                FacilityTb? model = new FacilityTb()
                {
                    Category = dto.Category,
                    Name = dto.Name,
                    Type = dto.Type,
                    Ea = dto.Ea,
                    StandardCapacity = dto.Standard_capacity,
                    FacCreateDt = dto.FacCreateDT,
                    Lifespan = dto.LifeSpan,
                    StandardCapacityUnit = dto.Standard_unit,
                    FacUpdateDt = dto.FacUpdateDT,
                    CreateDt = DateTime.Now,
                    CreateUser = creator,
                    UpdateDt = DateTime.Now,
                    UpdateUser = creator,
                    RoomTbid = dto.RoomTbId
                };

                FacilityTb? result = await FacilityInfoRepository.AddAsync(model);
                if (result is not null)
                    return new ResponseUnit<AddFacilityDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseUnit<AddFacilityDTO>() { message = "잘못된 요청입니다.", data = new AddFacilityDTO(), code = 404 };
            }
            catch(Exception ex)
            {
                return new ResponseUnit<AddFacilityDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddFacilityDTO(), code = 500 };
            }

        }

        /// <summary>
        /// 해당 사업장의 모든 설비 리스트 조회
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<FacilityListDTO>?> GetFacilityListService(HttpContext? context)
        {
            try
            {
                if (context is null)
                    return new ResponseList<FacilityListDTO>() { message = "잘못된 요청입니다.", data = new List<FacilityListDTO>(), code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<FacilityListDTO>() { message = "잘못된 요청입니다.", data = new List<FacilityListDTO>(), code = 404 };

                List<FacilityListDTO>? model = await FacilityInfoRepository.GetFacilityList(Int32.Parse(placeid));

                if (model is [_, ..])
                    return new ResponseList<FacilityListDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<FacilityListDTO>() { message = "해당하는 데이터가 없습니다.", data = new List<FacilityListDTO>(), code = 200 };
            }
            catch(Exception ex)
            {
                return new ResponseList<FacilityListDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<FacilityListDTO>(), code = 500 };
            }
        }

        public async ValueTask<ResponseUnit<FacilityDetailDTO>?> GetDetailService(int? facilityId)
        {
            try
            {
                if (facilityId is null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "잘못된 요청입니다.", data = new FacilityDetailDTO(), code = 404 };

                FacilityDetailDTO? model = await FacilityInfoRepository.GetDetailInfo(facilityId);

                if (model is not null)
                    return new ResponseUnit<FacilityDetailDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseUnit<FacilityDetailDTO>() { message = "해당하는 데이터가 없습니다.", data = new FacilityDetailDTO(), code = 200 };
            }
            catch(Exception ex)
            {
                return new ResponseUnit<FacilityDetailDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new FacilityDetailDTO(), code = 508 };
            }
        }

    }
}
