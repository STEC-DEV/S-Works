﻿using FamTec.Server.Repository.Admin.Departmnet;
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
        public async ValueTask<ResponseUnit<AddDepartmentDTO>> AddDepartmentService(HttpContext? context, AddDepartmentDTO? dto)
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

                DepartmentTb? model = await DepartmentInfoRepository.GetDepartmentInfo(dto.Name);

                if (model is null)
                {
                    DepartmentTb? tb = new DepartmentTb
                    {
                        Name = dto.Name,
                        CreateDt = DateTime.Now,
                        CreateUser = Creater,
                        UpdateDt = DateTime.Now,
                        UpdateUser = Creater
                    };

                    DepartmentTb? result = await DepartmentInfoRepository.AddAsync(tb);

                    if (result is not null)
                    {
                        return new ResponseUnit<AddDepartmentDTO>
                        {
                            message = "데이터가 정상 처리되었습니다.",
                            data = new AddDepartmentDTO
                            {
                                Name = result.Name!
                            },
                            code = 200
                        };

                    }
                    else
                    {
                        return new ResponseUnit<AddDepartmentDTO> { message = "데이터가 처리되지 않았습니다.", data = new AddDepartmentDTO(), code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<AddDepartmentDTO> { message = "이미 해당 부서가 존재합니다.", data = new AddDepartmentDTO(), code = 404 };
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
                List<DepartmentTb>? model = await DepartmentInfoRepository.GetAllList();

                if(model is [_, ..])
                {
                    return new ResponseList<DepartmentDTO>
                    {
                        message = "데이터가 정상 처리되었습니다.",
                        data = model.Select(e => new DepartmentDTO
                        {
                            Id = e.Id,
                            Name = e.Name
                        }).ToList(),
                        code = 200};
                }
                else
                {
                    return new ResponseList<DepartmentDTO> { message = "데이터가 존재하지 않습니다.", data = new List<DepartmentDTO>(), code = 200 };
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
        public async ValueTask<ResponseUnit<bool>> DeleteDepartmentService(List<int?> index)
        {
            try
            {
                if(index is [_, ..])
                {
                    bool? result = await DepartmentInfoRepository.DeleteDepartmentInfo(index);

                    if(result == true)
                    {
                        return new ResponseUnit<bool> { message = "데이터 삭제완료.", data = true, code = 200 };
                    }
                    else if(result == false)
                    {
                        return new ResponseUnit<bool> { message = "데이터 삭제 실패.", data = false, code = 404 };
                    }
                    else
                    {
                        return new ResponseUnit<bool> { message = "요청이 잘못되었습니다.", data = false, code = 404 };
                    }
                }
                else
                {
                    return new ResponseUnit<bool> { message = "요청이 잘못되었습니다.", data = false, code = 404 };
                }

            }
            catch(Exception ex)
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
        public async ValueTask<ResponseUnit<DepartmentDTO>?> UpdateDepartmentService(HttpContext? context,DepartmentDTO? dto)
        {
            try
            {
                if(dto is null)
                    return new ResponseUnit<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = new DepartmentDTO(), code = 404 };

                if(context is null)
                    return new ResponseUnit<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = new DepartmentDTO(), code = 404 };

                string? updater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(updater))
                    return new ResponseUnit<DepartmentDTO>() { message = "요청이 잘못되었습니다.", data = new DepartmentDTO(), code = 404 };
                
                DepartmentTb? model = await DepartmentInfoRepository.GetDepartmentInfo(dto.Id);
                    
                if(model is not null)
                {
                    DepartmentTb? duplechk = await DepartmentInfoRepository.GetDepartmentInfo(dto.Name);
                        
                    if(duplechk is null)
                    {
                        model.Name = dto.Name;
                        model.UpdateUser = updater;
                        model.UpdateDt = DateTime.Now;

                        bool? result = await DepartmentInfoRepository.UpdateDepartmentInfo(model);
                        if (result == true)
                        {
                            return new ResponseUnit<DepartmentDTO>() { message = "요청이 정상 처리되었습니다.", data = new DepartmentDTO { Id = model.Id, Name = dto.Name }, code = 200 };
                        }
                        else
                        {
                            return new ResponseUnit<DepartmentDTO>() { message = "요청이 처리되지 않았습니다.", data = new DepartmentDTO(), code = 200 };
                        }
                    }
                    else
                    {
                        return new ResponseUnit<DepartmentDTO>() { message = "입력하신 부서명이 존재합니다.", data = new DepartmentDTO(), code = 200 };
                    }   
                }
                else
                {
                    return new ResponseUnit<DepartmentDTO>() { message = "해당 부서가 존재하지 않습니다.", data = new DepartmentDTO(), code = 200 };
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
