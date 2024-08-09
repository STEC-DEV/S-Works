using FamTec.Server.Repository.Admin.Departmnet;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;

namespace FamTec.Server.Services.Admin.Department
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentInfoRepository DepartmentInfoRepository;
        private ILogService LogService;


        public DepartmentService(IDepartmentInfoRepository _departmentinforepository,
            ILogService _logservice)
        {
            this.DepartmentInfoRepository = _departmentinforepository;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 부서추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<AddDepartmentDTO>> AddDepartmentService(HttpContext context, AddDepartmentDTO dto)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<AddDepartmentDTO> { message = "잘못된 요청입니다..", data = null, code = 404 };
                if(dto is null)
                    return new ResponseUnit<AddDepartmentDTO> { message = "잘못된 요청입니다..", data = null, code = 404 };
                
                string? Creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(Creater))
                    return new ResponseUnit<AddDepartmentDTO> { message = "잘못된 요청입니다.", data = null, code = 404 };

                DepartmentsTb? AlreadyCheck = await DepartmentInfoRepository.GetDepartmentInfo(dto.Name!);
                if (AlreadyCheck is not null)
                    return new ResponseUnit<AddDepartmentDTO>() { message = "이미 존재하는 부서명입니다.", data = null, code = 204 };

                DepartmentsTb? DepartmentTB = new DepartmentsTb();
                DepartmentTB.Name = dto.Name!;
                DepartmentTB.CreateDt = DateTime.Now;
                DepartmentTB.CreateUser = Creater;
                DepartmentTB.UpdateDt = DateTime.Now;
                DepartmentTB.UpdateUser = Creater;
                DepartmentTB.ManagementYn = dto.ManagerYN!.Value;

                DepartmentsTb? result = await DepartmentInfoRepository.AddAsync(DepartmentTB);

                if (result is not null)
                {
                    return new ResponseUnit<AddDepartmentDTO>
                    {
                        message = "데이터가 정상 처리되었습니다.",
                        data = new AddDepartmentDTO
                        {
                            Name = result.Name,
                            ManagerYN = result.ManagementYn
                        },
                        code = 200
                    };
                }
                else
                {
                    return new ResponseUnit<AddDepartmentDTO> { message = "데이터가 처리되지 않았습니다.", data = new AddDepartmentDTO(), code = 404 };
                }
               
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<AddDepartmentDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddDepartmentDTO(), code = 404 };
            }
        }



        /// <summary>
        /// 부서 전체조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<ResponseList<DepartmentDTO>> GetAllDepartmentService()
        {
            try
            {
                List<DepartmentsTb>? model = await DepartmentInfoRepository.GetAllList();

                if (model is [_, ..])
                {
                    return new ResponseList<DepartmentDTO>
                    {
                        message = "데이터가 정상 처리되었습니다.",
                        data = model.Select(e => new DepartmentDTO
                        {
                            Id = e.Id,
                            Name = e.Name,
                            ManageYN = e.ManagementYn
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<DepartmentDTO> { message = "데이터가 존재하지 않습니다.", data = new List<DepartmentDTO>(), code = 200 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<DepartmentDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<DepartmentDTO>(), code = 500 };
            }
        }


        /// <summary>
        /// 관리부서 전체조회
        /// </summary>
        /// <returns></returns>
        public async ValueTask<ResponseList<DepartmentDTO>> ManageDepartmentService()
        {
            try
            {
                List<DepartmentsTb>? model = await DepartmentInfoRepository.GetManageDepartmentList();

                if(model is [_, ..])
                {
                    return new ResponseList<DepartmentDTO>
                    {
                        message = "데이터가 정상 처리되었습니다.",
                        data = model.Select(e => new DepartmentDTO
                        {
                            Id = e.Id,
                            Name = e.Name,
                            ManageYN = e.ManagementYn
                        }).ToList(),
                        code = 200
                    };
                }
                else
                {
                    return new ResponseList<DepartmentDTO>() { message = "데이터가 존재하지 않습니다.", data = new List<DepartmentDTO>(), code = 204 };
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<DepartmentDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = new List<DepartmentDTO>(), code = 500 };
            }
        }

        /// <summary>
        /// 부서삭제
        /// </summary>
        /// <param name="index"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool>> DeleteDepartmentService(HttpContext context, List<int> departmentidx)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                if (departmentidx is null)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                 
                if (departmentidx.Count() == 0)
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };

                // 해당 부서 인덱스와 AdminTb의 DepartmentID 외래키로 검색해서 있는지 검사 - 삭제조건 [1]
                List<AdminTb>? admintb = await DepartmentInfoRepository.SelectDepartmentAdminList(departmentidx);
                if (admintb is not null)
                    return new ResponseUnit<bool>() { message = "해당 부서에 할당되어있는 관리자가 있어 삭제가 불가능합니다.", data = false, code = 200 };

                // 부서존재하는지 + 시스템 부서가 있는지 검사
                foreach(int DepartmentID in departmentidx)
                {
                    DepartmentsTb? CheckTB = await DepartmentInfoRepository.GetDeleteDepartmentInfo(DepartmentID);
                    if(CheckTB is null)
                    {
                        return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                    }
                    else
                    {
                        if (CheckTB.Name.Equals("에스텍시스템"))
                        {
                            return new ResponseUnit<bool>() { message = "시스템 부서는 삭제 불가능합니다.", data = false, code = 404 };
                        }
                    }
                }

                // 부서삭제
                bool? DeleteResult = await DepartmentInfoRepository.DeleteDepartmentInfo(departmentidx, creater);
                if (DeleteResult == true)
                {
                    return new ResponseUnit<bool>() { message = "요청이 정상 처리되었습니다", data = true, code = 200 };
                }
                else if (DeleteResult == false)
                {
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }
                else
                {
                    return new ResponseUnit<bool>() { message = "잘못된 요청입니다.", data = false, code = 404 };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool> { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 404 };
            }
        }

        /// <summary>
        /// 부서수정
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<DepartmentDTO>> UpdateDepartmentService(HttpContext context,DepartmentDTO dto)
        {
            try
            {
                if(dto is null)
                    return new ResponseUnit<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                if(context is null)
                    return new ResponseUnit<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? updater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(updater))
                    return new ResponseUnit<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };
                
                DepartmentsTb? AlreadyCheck = await DepartmentInfoRepository.GetDepartmentInfo(dto.Name!);
                if (AlreadyCheck is not null)
                    return new ResponseUnit<DepartmentDTO>() { message = "이미 존재하는 부서명입니다.", data = null, code = 204 };

                DepartmentsTb? DepartmentTB = await DepartmentInfoRepository.GetDepartmentInfo(dto.Id!.Value);
                if (DepartmentTB is not null)
                {
                    DepartmentTB.Name = dto.Name!;
                    DepartmentTB.UpdateUser = updater;
                    DepartmentTB.UpdateDt = DateTime.Now;
                    DepartmentTB.ManagementYn = dto.ManageYN!.Value;

                    bool? result = await DepartmentInfoRepository.UpdateDepartmentInfo(DepartmentTB);
                    if (result == true)
                    {
                        return new ResponseUnit<DepartmentDTO>() { message = "요청이 정상 처리되었습니다.", data = new DepartmentDTO { Id = DepartmentTB.Id, Name = dto.Name }, code = 200 };
                    }
                    else
                    {
                        return new ResponseUnit<DepartmentDTO>() { message = "요청이 처리되지 않았습니다.", data = null, code = 200 };
                    }
                }
                else
                {
                    return new ResponseUnit<DepartmentDTO>() { message = "해당 부서가 존재하지 않습니다.", data = null, code = 404 };
                }   
              
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DepartmentDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new DepartmentDTO(), code = 500 };
            }
        }

    
    }
}
