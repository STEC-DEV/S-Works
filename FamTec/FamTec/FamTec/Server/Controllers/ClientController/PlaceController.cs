using FamTec.Server.Databases;
using FamTec.Shared.Client.DTO;
using FamTec.Shared.Client.DTO.Place;
using FamTec.Shared.Model;
using FamTec.Shared.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Controllers.ClientController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly WorksContext _workContext;

        public PlaceController(WorksContext workContext)
        {
            _workContext = workContext;
        }

        /*
        [HttpGet]
        [Route("detail")]
        public async Task<IActionResult> FindOnePlace([FromQuery] int id)
        {
            try
            {
                Console.WriteLine("사업장 상세조회");

                var placeInfo = await _workContext.PlaceTbs
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if(placeInfo == null)
                {
                    return NotFound();
                }
                var placeManager = await _workContext.AdminPlaceTbs
                     .Include(ap => ap.AdminTb)
                        .ThenInclude(a => a.DepartmentTb)
                     .Include(ap => ap.AdminTb)
                        .ThenInclude(a => a.UserTb)
                   
                     .Where(p => p.PlaceId == id)
                     .ToListAsync();

                // 관리자 목록을 담을 리스트
                var managerList = new List<ManagerDTO>();

                // 관리자 목록에 데이터 추가
                foreach (var manager in placeManager)
                {
                    // 관리자 정보 가져오기
                    var adminTb = manager.AdminTb;
                    if (adminTb != null)
                    {
                        // 관리자 DTO에 관련 정보 추가
                        var managerDto = new ManagerDTO
                        {
                            Id = adminTb.Id,
                            UserId = adminTb.UserTb?.UserId,
                            Name = adminTb.UserTb?.Name,
                            Department = adminTb.DepartmentTb?.Name
                        };

                        // 리스트에 추가
                        managerList.Add(managerDto);
                    }
                }


                PlaceDetail2DTO placeDetail = new PlaceDetail2DTO
                {
                    PlaceInfo = new PlaceInfo
                    {
                        Id = placeInfo.Id,
                        PlaceCd = placeInfo.PlaceCd,
                        Name = placeInfo.Name,
                        Tel = placeInfo.Tel,
                        ContractNum = placeInfo.ContractNum,
                        ContractDt = placeInfo.ContractDt,
                        CancelDt = placeInfo.CancelDt,
                        Status = placeInfo.Status,
                        Note = placeInfo.Note
                    },

                    PlacePerm = new PlacePerm
                    {
                        Id = placeInfo.Id,
                        PermMachine = placeInfo.PermMachine,
                        PermLift = placeInfo.PermLift,
                        PermFire = placeInfo.PermFire,
                        PermConstruct = placeInfo.PermConstruct,
                        PermNetwork = placeInfo.PermNetwrok,
                        PermBeauty = placeInfo.PermBeauty,
                        PermSecurity = placeInfo.PermSecurity,
                        PermMaterial = placeInfo.PermMaterial,
                        PermEnergy = placeInfo.PermEnergy,
                        PermVoc = placeInfo.PermVoc
                    },
                   ManagerList = managerList

                };

                return Ok(new ResponseObj<PlaceDetail2DTO> { message = "상세 조회 성공", data = new List<PlaceDetail2DTO> { placeDetail }, code = 200 });
            }catch (Exception ex)
            {
                Console.WriteLine("[Place][Controller] 사업장 상세조회 에러!!\n " + ex);
                return Problem("[Place][Controller] 사업장 상세조회 에러!!\n" + ex);
            }
        }
        */

        /*
        [HttpPost]
        [Route("addplace")]
        public async Task<IActionResult> AddPlace([FromBody] AddPlaceDTO place)
        {
            try
            {
                PlaceTb placeTb = new()
                {
                    PlaceCd = place.PlaceCd,
                    Name = place.Name,
                    //Tell
                    Address = place.Address,
                    ContractNum = place.ContractNum,
                    ContractDt = Convert.ToDateTime(place.ContractDt),
                    PermMachine = place.PermMachine,
                    PermLift = place.PermLift,
                    PermFire = place.PermFire,
                    PermConstruct = place.PermConstruct,
                    PermNetwrok = place.PermNetwork,
                    PermBeauty = place.PermBeauty,
                    PermSecurity = place.PermSecurity,
                    PermMaterial = place.PermMaterial,
                    PermEnergy = place.PermEnergy,
                    PermVoc = place.PermVoc,
                };

                var resPlaceTb = _workContext.PlaceTbs.Add(placeTb).Entity;
                await _workContext.SaveChangesAsync();
                if(resPlaceTb == null)
                {
                    return BadRequest("사업장 생성 오류");
                }

                return Ok(new ResponseObj<int>{ message="사업장 생성 완료", data = new List<int> { resPlaceTb.Id }, code = 200});
            }catch (Exception ex)
            {
                Console.WriteLine("[Place][Controller] 사업장 생성 에러!!\n " + ex);
                return Problem("[Place][Controller] 사업장 생성 에러!!\n" + ex);
            }
        }
        */

    }
}
