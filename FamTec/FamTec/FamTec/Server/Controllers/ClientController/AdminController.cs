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
    public class AdminController : ControllerBase
    {
        private readonly WorksContext _workContext;

        public AdminController(WorksContext workContext)
        {
            _workContext = workContext;
        }

        /// <summary>
        /// 매니저 목록 조회
        /// </summary>
        /// <returns></returns>
    
        [HttpGet]
        [Route("allmanager")]
        public async Task<IActionResult> FindAllManager()
        {
            try
            {
                Console.WriteLine("매니저 전체조회");
                List<ManagerDTO> res = await _workContext.AdminTbs
                    .Where(a => a.DelYn != true)
                    .Include(a => a.UserTb)
                    .Include(a => a.DepartmentTb)
                    .Select(a => new ManagerDTO
                    {
                        Id = a.Id,
                        UserId = a.UserTb.UserId,
                        Name = a.UserTb.Name,
                        Department = a.DepartmentTb.Name
                    }).ToListAsync();

                return Ok(new ResponseObj<ManagerDTO> { message = "매니저 전체조회 성공", data = res, code = 200 });

            }
            catch (Exception ex)
            {
                Console.WriteLine("[Admin][Controller] 매니저 전체 조회 에러!!\n " + ex);
                return Problem("[Admin][Controller] 매니저 전체 조회 에러!!\n" + ex);
            }
        }
     

        //매니저추가
    
        [HttpPost]
        [Route("addmanager")]
        public async Task<IActionResult> AddManager([FromBody] AddManagerDTO manager)
        {
            using var transaction = await _workContext.Database.BeginTransactionAsync();
            try
            {

                //사용자 테이블 생성
                //생성 id를 포함한 데이터로 관리자db 생성
                Console.WriteLine("매니저 추가");
                UserTb userTb = new()
                {
                    UserId = manager.UserId,
                    Name = manager.Name,
                    Password = manager.Password,
                    Email = manager.Email,
                    Phone = manager.Phone,
                    PermBasic = 2,
                    PermMachine = 2,
                    PermLift = 2,
                    PermFire = 2,
                    PermConstruct = 2,
                    PermNetwork = 2,
                    PermBeauty = 2,
                    PermSecurity = 2,
                    PermMaterial = 2,
                    PermEnergy = 2,
                    PermUser = 2,
                    PermVoc = 2,
                    AdminYn = 1,
                    Status = 1,
                };
                UserTb resUsertb = _workContext.UserTbs.Add(userTb).Entity;
                await _workContext.SaveChangesAsync();

                if (resUsertb == null)
                {
                    return BadRequest("회원가입 되지않음");
                }

                AdminTb adminTb = new()
                {
                    Type = manager.Type,
                    UserTbId = resUsertb.Id,
                    DepartmentTbId = manager.DepartmentId
                };
                _workContext.AdminTbs.Add(adminTb);

                await _workContext.SaveChangesAsync();
                await transaction.CommitAsync();


                return Ok(new ResponseObj<AddManagerDTO> { message = "매니저 등록 완료", code = 200 });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Admin][Controller] 매니저 추가 에러!!\n " + ex);
                return Problem("[Admin][Controller] 매니저 추가 에러!!\n" + ex);
            }
        }
        [HttpPut]
        [Route("deletemanager")]
        public async Task<IActionResult> DeleteManager([FromBody] List<int> adminIds)
        {
            try
            {
                Console.WriteLine("[매니저 삭제 컨트롤러 시작]");




                return Ok();
            }
            catch(Exception ex)
            {
                Console.WriteLine("[Admin][Controller][Delete] 매니저 삭제 에러!!\n " + ex);
                return Problem("[Admin][Controller][Delete] 매니저 삭제 에러!!\n" + ex);
            }
        }

        

        /// <summary>
        /// 부서 전체 조회
        /// </summary>
        /// <returns></returns>
        
        [HttpGet]
        [Route("alldepartment")]
        public async Task<IActionResult> FindAllDepartment()
        {
            try
            {
                Console.WriteLine("부서 전체 조회");
                List<DepartmentTb> res = await _workContext.DepartmentTbs
                    .Where(d => d.DelYn != true)
                    .ToListAsync();
                List<DepartmentDTO> departmentList = res.Select(departTb => new DepartmentDTO
                {
                    Id = departTb.Id,
                    Name = departTb.Name,
                }).ToList();


                return Ok(new ResponseObj<DepartmentDTO> { message = "부서 조회 성공", data = departmentList, code = 200 });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Admin][Controller] 부서 전체 조회 에러!!\n " + ex);
                return Problem("[Admin][Controller] 부서 전체 조회 에러!!\n" + ex);
            }
        }
    

        //부서 추가
     
        [HttpPost]
        [Route("adddepartment")]
        public async Task<IActionResult> AddDepartment([FromBody] AddDepartmentDTO department)
        {
            try
            {
                Console.WriteLine("부서 추가");

                if (department.Name == null)
                {
                    return BadRequest("부서명이 공백입니다.");
                }

                DepartmentTb newDepartment = new()
                {
                    Name = department.Name,
                };

                var resData = _workContext.DepartmentTbs.Add(newDepartment).Entity;
                await _workContext.SaveChangesAsync();

                List<DepartmentTb> data = new List<DepartmentTb> { resData };

                return Ok(new ResponseObj<DepartmentTb> { message = "부서 추가 완료", data = data, code = 200 });

                //return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Admin][Controller] 부서 추가 에러!!\n " + ex);
                return Problem("[Admin][Controller] 부서 추가 에러!!\n" + ex);
            }
        }
       

        /// <summary>
        /// 부서 삭제
        /// </summary>
        /// <param name="delData"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("deletedepartment")]
        public async Task<IActionResult> DeleteDepartment([FromBody] List<int> delData)
        {
            try
            {
                /*
                 * 1. req data 셀렉트 검증
                 * 1-1 없는 db 있을 시 return
                 * 1-2 성공 시 del_yn y로 변경
                 */
                Console.WriteLine(delData);
                var departments = await _workContext.DepartmentTbs
                    .Where(d => delData.Contains(d.Id))
                    .ToListAsync();

                if (departments == null || !departments.Any())
                {
                    return NotFound("Departments not found");
                }
                foreach (var department in departments)
                {
                    department.DelYn = true;
                }

                await _workContext.SaveChangesAsync();


                return Ok(new { message = "삭제 성공" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Admin][Department][Delete] 부서 삭제 에러!!\n " + ex);
                return Problem("[Admin][Department][Delete] 부서 삭제 에러!!\n" + ex);
            }
        }

        /*
        [HttpPost]
        [Route("addplacemanager")]
        public async Task<IActionResult> AddPlaceManager([FromBody] AddPlaceManagerDTO<ManagerDTO> placemanager)
        {
            try
            {
                Console.WriteLine("사업장 매니저 추가");

                int placeId = placemanager.PlaceId;
                List<ManagerDTO> placeManagers = placemanager.PlaceManager;

                // 각 관리자 정보를 AdminPlaceTb에 추가
                foreach (var manager in placeManagers)
                {
                    AdminPlaceTb adminPlace = new AdminPlaceTb
                    {
                        AdminTbId = manager.Id, // 관리자 ID 설정
                        PlaceId = placeId
                        // 나머지 AdminPlaceTb 속성 설정
                    };

                    // AdminPlaceTb를 데이터베이스에 추가
                    _workContext.AdminPlaceTbs.Add(adminPlace);
                }

                // 변경 사항을 저장
                await _workContext.SaveChangesAsync();

                return Ok("관리자 추가 완료");

                return Ok();
            }catch (Exception ex)
            {
                Console.WriteLine("[Admin][AdminPlace][Add] 사업장 매니저 추가 에러!!\n " + ex);
                return Problem("[Admin][AdminPlace][Delete] 사업장 매니저 추가 에러!!\n" + ex);
            }
        }
        */
    }
}
