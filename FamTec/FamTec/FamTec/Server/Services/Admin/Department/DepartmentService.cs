using FamTec.Server.Helpers;
using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;

namespace FamTec.Server.Services.Admin.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentInfoRepository DepartmentInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<DepartmentService> CreateBuilderLogger; /* 콘솔로그 */

        public DepartmentService(IDepartmentInfoRepository _departmentinforepository,
            IHttpContextAccessor _httpcontextaccessor,
            ILogService _logservice,
            ConsoleLogService<DepartmentService> _createbuilderlogger)
        {
            this.DepartmentInfoRepository = _departmentinforepository;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 부서추가
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<AddDepartmentDTO>> AddDepartmentService(AddDepartmentDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<AddDepartmentDTO> { message = "잘못된 요청입니다..", data = null, code = 404 };
                
                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseModel<AddDepartmentDTO> { message = "잘못된 요청입니다.", data = null, code = 404 };

                DepartmentsTb? AlreadyCheck = await DepartmentInfoRepository.GetDepartmentInfo(dto.Name!).ConfigureAwait(false);
                if (AlreadyCheck is not null)
                    return new ResponseModel<AddDepartmentDTO>() { message = "이미 존재하는 부서명입니다.", data = null, code = 204 };

                DateTime ThisTime = DateTime.Now;

                DepartmentsTb? DepartmentTB = new DepartmentsTb
                {
                    Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!,
                    CreateDt = ThisTime,
                    CreateUser = !String.IsNullOrWhiteSpace(Creater) ? Creater.Trim() : Creater,
                    UpdateDt = ThisTime,
                    UpdateUser = !String.IsNullOrWhiteSpace(Creater) ? Creater.Trim() : Creater,
                    ManagementYn = dto.ManagerYN
                };

                DepartmentsTb? result = await DepartmentInfoRepository.AddAsync(DepartmentTB).ConfigureAwait(false);

                if (result is not null)
                {
                    var DepartmentData = new AddDepartmentDTO
                    {
                        Name = !String.IsNullOrWhiteSpace(result.Name) ? result.Name.Trim() : result.Name,
                        ManagerYN = result.ManagementYn
                    };

                    return new ResponseModel<AddDepartmentDTO> { message = "데이터가 정상 처리되었습니다.", data = DepartmentData, code = 200 };
                }
                else
                {
                    return new ResponseModel<AddDepartmentDTO> { message = "데이터가 처리되지 않았습니다.", data = new AddDepartmentDTO(), code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<AddDepartmentDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddDepartmentDTO(), code = 404 };
            }
        }

        /// <summary>
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<DepartmentDTO>>> GetAllDepartmentService()
        {
            try
            {
                List<DepartmentsTb>? model = await DepartmentInfoRepository.GetAllList().ConfigureAwait(false);

                if (model is not null && model.Any())
                {
                    var departmentData = model.Select(e => new DepartmentDTO
                    {
                        Id = e.Id,
                        Name = !String.IsNullOrWhiteSpace(e.Name) ? e.Name.Trim() : e.Name,
                        ManagerYN = e.ManagementYn
                    }).ToList();

                    return new ResponseModel<List<DepartmentDTO>> { message = "데이터가 정상 처리되었습니다.", data = departmentData, code = 200 };
                }
                else
                {
                    return new ResponseModel<List<DepartmentDTO>> { message = "데이터가 존재하지 않습니다.", data = new List<DepartmentDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<DepartmentDTO>> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<DepartmentDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 관리부서 전체조회
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<DepartmentDTO>>> ManageDepartmentService()
        {
            try
            {
                List<DepartmentsTb>? model = await DepartmentInfoRepository.GetManageDepartmentList().ConfigureAwait(false);

                if(model is not null && model.Any())
                {
                    var DepartmentData = model.Select(e => new DepartmentDTO()
                    {
                        Id = e.Id,
                        Name = !String.IsNullOrWhiteSpace(e.Name) ? e.Name.Trim() : e.Name,
                        ManagerYN = e.ManagementYn
                    }).ToList();

                    return new ResponseModel<List<DepartmentDTO>> { message = "데이터가 정상 처리되었습니다.", data = DepartmentData, code = 200 };
                }
                else
                {
                    return new ResponseModel<List<DepartmentDTO>>() { message = "데이터가 존재하지 않습니다.", data = new List<DepartmentDTO>(), code = 204 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<List<DepartmentDTO>> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<DepartmentDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="index"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool?>> DeleteDepartmentService(List<int> departmentidx)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || departmentidx is null || !departmentidx.Any())
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                // 해당 부서 인덱스와 AdminTb의 DepartmentID 외래키로 검색해서 있는지 검사 - 삭제조건 [1]
                List<AdminTb>? admintb = await DepartmentInfoRepository.SelectDepartmentAdminList(departmentidx).ConfigureAwait(false);
                if (admintb is not null && admintb.Any())
                    return new ResponseModel<bool?>() { message = "해당 부서에 할당되어있는 관리자가 있어 삭제가 불가능합니다.", data = false, code = 400 };

                // 부서존재하는지 + 시스템 부서가 있는지 검사
                foreach(int DepartmentID in departmentidx)
                {
                    bool? DelCheck = await DepartmentInfoRepository.DelDepartmentCheck(DepartmentID).ConfigureAwait(false);
                    if (DelCheck == true)
                        return new ResponseModel<bool?>() { message = "참조하는 하위 정보가 있어 삭제가 불가능합니다.", data = false, code = 400 };

                    DepartmentsTb? CheckTB = await DepartmentInfoRepository.GetDeleteDepartmentInfo(DepartmentID).ConfigureAwait(false);
                    if(CheckTB is null)
                        return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                    
                    if (CheckTB.Name.Trim().Equals("에스텍시스템"))
                        return new ResponseModel<bool?>() { message = "시스템 부서는 삭제 불가능합니다.", data = false, code = 400 };
                }

                // 부서삭제
                bool? DeleteResult = await DepartmentInfoRepository.DeleteDepartmentInfo(departmentidx, creater).ConfigureAwait(false);
                return DeleteResult switch
                {
                    true => new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 404 };
            }
        }

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async Task<ResponseModel<DepartmentDTO>> UpdateDepartmentService(DepartmentDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (dto is null || context is null)
                    return new ResponseModel<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? updater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(updater))
                    return new ResponseModel<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                DepartmentsTb? AlreadyCheck = await DepartmentInfoRepository.GetDepartmentInfo(dto.Name!).ConfigureAwait(false);
                if (AlreadyCheck is not null)
                    return new ResponseModel<DepartmentDTO>() { message = "이미 존재하는 부서명입니다.", data = null, code = 204 };

                DepartmentsTb? DepartmentTB = await DepartmentInfoRepository.GetDepartmentInfo(dto.Id!.Value).ConfigureAwait(false);
                if(DepartmentTB is null)
                    return new ResponseModel<DepartmentDTO>() { message = "해당 부서가 존재하지 않습니다.", data = null, code = 404 };

                DepartmentTB.Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!;
                DepartmentTB.UpdateUser = !String.IsNullOrWhiteSpace(updater) ? updater.Trim() : updater;
                DepartmentTB.UpdateDt = DateTime.Now;
                DepartmentTB.ManagementYn = dto.ManagerYN!.Value;

                bool? result = await DepartmentInfoRepository.UpdateDepartmentInfo(DepartmentTB).ConfigureAwait(false);
                return result switch
                {
                    true => new ResponseModel<DepartmentDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 },
                    false => new ResponseModel<DepartmentDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 },
                    _ => new ResponseModel<DepartmentDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<DepartmentDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DepartmentDTO(), code = 500 };
            }
        }
    
    }
}
