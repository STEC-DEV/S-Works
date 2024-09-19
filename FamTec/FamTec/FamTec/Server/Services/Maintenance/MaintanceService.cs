using FamTec.Server.Repository.Facility;
using FamTec.Server.Repository.Maintenence;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Services.Maintenance
{
    public class MaintanceService : IMaintanceService
    {
        private readonly IMaintanceRepository MaintanceRepository;
        private readonly IFacilityInfoRepository FacilityInfoRepository;
        
        private IFileService FileService;
        private ILogService LogService;
        
        private string? MaintanceFileFolderPath;

        public MaintanceService(IMaintanceRepository _maintancerepository,
            IFacilityInfoRepository _facilityinforepository,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.MaintanceRepository = _maintancerepository;
            this.FacilityInfoRepository = _facilityinforepository;
            
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        public async ValueTask<ResponseUnit<bool?>> AddMaintanceImageService(HttpContext context, int id, IFormFile? files)
        {
            try
            {
                if(context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? ImageAddResult = await MaintanceRepository.AddMaintanceImageAsync(id, Int32.Parse(placeid), files);
                return ImageAddResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 유지보수 출고등록
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<FailResult?>> AddMaintanceService(HttpContext context, AddMaintenanceDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? creater = Convert.ToString(context.Items["Name"]);
                string? userid = Convert.ToString(context.Items["UserIdx"]);

                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(creater) || String.IsNullOrWhiteSpace(userid))
                    return new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (dto.Type is 0) // 자체작업
                {
                    FailResult? MaintanceId = await MaintanceRepository.AddSelfMaintanceAsync(dto, creater, userid, Convert.ToInt32(placeid));
                    return MaintanceId!.ReturnResult switch
                    {
                        > 0 => new ResponseUnit<FailResult?>() { message = "요청이 정상 처리되었습니다.", data = MaintanceId, code = 200 },
                        0 => new ResponseUnit<FailResult?>() { message = "출고시킬 수량이 실제수량보다 부족합니다.", data = MaintanceId, code = 422 },
                        -1 => new ResponseUnit<FailResult?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = MaintanceId, code = 409 },
                        -2 => new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = MaintanceId, code = 404 },
                        _ => new ResponseUnit<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = MaintanceId, code = 500 }
                    };
                }
                else // 외주작업
                {
                    if (dto.TotalPrice is 0)
                        return new ResponseUnit<FailResult?> { message = "잘못된 요청입니다.", data = null, code = 404 };

                    FailResult? MaintanceId = await MaintanceRepository.AddOutSourcingMaintanceAsync(dto, creater, userid, Convert.ToInt32(placeid));

                    return MaintanceId!.ReturnResult switch
                    {
                        > 0 => new ResponseUnit<FailResult?>() { message = "요청이 정상 처리되었습니다.", data = MaintanceId, code = 200 },
                        -2 => new ResponseUnit<FailResult?>() { message = "잘못된 요청입니다.", data = MaintanceId, code = 404 },
                        _ => new ResponseUnit<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = MaintanceId, code = 500 }
                    };
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<FailResult?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
        
        /// <summary>
        /// 해당 유지보수의 출고내역 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteMaintenanceStoreRecordService(HttpContext context, DeleteMaintanceDTO dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? deleter = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                bool? DeleteResult = await MaintanceRepository.deleteMaintenanceStoreRecord(dto, Int32.Parse(placeid), deleter);
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = false, code = 200 },
                    _ => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 유지보수 자체를 삭제
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> DeleteMaintenanceRecordService(HttpContext context, DeleteMaintanceDTO2 dto)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? deleter = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(placeid) || String.IsNullOrWhiteSpace(deleter))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다", data = null, code = 404 };

                bool? DeleteResult = await MaintanceRepository.deleteMaintenanceRecord(dto, Int32.Parse(placeid), deleter);

                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "다른곳에서 해당 품목을 사용중입니다.", data = null, code = 200 },
                    _ => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 해당 설비의 유지보수 이력 조회
        /// </summary>
        /// <param name="context"></param>
        /// <param name="facilityid"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaintanceListDTO>> GetMaintanceHistoryService(HttpContext context, int facilityid)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                // 여기 더 추가해야함
                
                FacilityTb? VaildFacility = await FacilityInfoRepository.GetFacilityInfo(facilityid);
                if(VaildFacility is null)
                    return new ResponseList<MaintanceListDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceListDTO>? dto = await MaintanceRepository.GetFacilityHistoryList(facilityid, Int32.Parse(placeid));
                

                if (dto is not null && dto.Any())
                    return new ResponseList<MaintanceListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
                else
                    return new ResponseList<MaintanceListDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaintanceListDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 속한 사업장 유지보수 이력 날짜기간 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<MaintanceHistoryDTO>?> GetDateHisotryList(HttpContext context, DateTime StartDate, DateTime EndDate, List<string> category, List<int> type)
        {
            try
            {
                if (context is null)
                    return new ResponseList<MaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if(String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<MaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<MaintanceHistoryDTO>? model = await MaintanceRepository.GetDateHistoryList(Convert.ToInt32(placeid), StartDate, EndDate, category, type);

                if (model is not null && model.Any())
                    return new ResponseList<MaintanceHistoryDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<MaintanceHistoryDTO>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<MaintanceHistoryDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 속한 사업장 유지보수 이력 전체
        /// </summary>
        /// <param name="context"></param>
        /// <param name="category"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async ValueTask<ResponseList<AllMaintanceHistoryDTO>?> GetAllHistoryList(HttpContext context, List<string> category, List<int> type)
        {
            try
            {
                if (context is null)
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                List<AllMaintanceHistoryDTO>? model = await MaintanceRepository.GetAllHistoryList(Convert.ToInt32(placeid), category, type);

                if (model is not null && model.Any())
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseList<AllMaintanceHistoryDTO>() { message = "데이터가 존재하지 않습니다.", data = model, code = 200 };

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseList<AllMaintanceHistoryDTO> { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 설비의 유지보수리스트중 하나 상세보기
        /// </summary>
        /// <param name="context"></param>
        /// <param name="MaintanceID"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<DetailMaintanceDTO?>> GetDetailService(HttpContext context, int MaintanceID)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                if (String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                DetailMaintanceDTO? model = await MaintanceRepository.DetailMaintanceList(MaintanceID, Int32.Parse(placeid));
                if (model is not null)
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "요청이 정상 처리되었습니다.", data = model, code = 200 };
                else
                    return new ResponseUnit<DetailMaintanceDTO?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<DetailMaintanceDTO?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        /// <summary>
        /// 유지보수 정보 수정
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async ValueTask<ResponseUnit<bool?>> UpdateMaintenanceService(HttpContext context, UpdateMaintenanceDTO dto, IFormFile? files)
        {
            try
            {
                string NewFileName = String.Empty;
                string deleteFileName = String.Empty;

                
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                string? placeid = Convert.ToString(context.Items["PlaceIdx"]);
                string? UserIdx = Convert.ToString(context.Items["UserIdx"]);
                string? updater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(UserIdx) || String.IsNullOrWhiteSpace(updater) || String.IsNullOrWhiteSpace(placeid))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                if (files is not null)
                {
                    NewFileName = FileService.SetNewFileName(UserIdx, files);
                }

                MaintenenceHistoryTb? model = await MaintanceRepository.GetMaintenanceInfo(dto.Id);
                if(model is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                model.Name = dto.Name!; // 유지보수명
                model.Worker = dto.Worker!; // 작업자
                model.UpdateDt = DateTime.Now;
                model.UpdateUser = updater;

                MaintanceFileFolderPath = String.Format(@"{0}\\{1}\\Maintance", Common.FileServer, placeid.ToString());
                if(files is not null) // 파일이 공백이 아닌 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있을경우
                    {
                        deleteFileName = model.Image; // 삭제할 이름에 넣는다.
                        model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                    }
                    else // DB엔 없을 경우
                    {
                        model.Image = NewFileName; // 새 파일명을 모델에 넣는다.
                    }
                }
                else // 파일이 공백인 경우
                {
                    if(!String.IsNullOrWhiteSpace(model.Image)) // DB에 파일이 있는경우
                    {
                        deleteFileName = model.Image; // 모델의 파일명을 삭제 명단에 넣는다.
                        model.Image = null; // 모델의 파일명을 비운다.
                    }
                }

                bool? updateMaintance = await MaintanceRepository.UpdateMaintenanceInfo(model);
                if(updateMaintance == true)
                {
                    if(files is not null) // 파일이 공백이 아닌경우
                    {
                        if(!String.IsNullOrWhiteSpace(model.Image))
                        {
                            // 파일 넣기
                            await FileService.AddResizeImageFile(NewFileName, MaintanceFileFolderPath, files);
                        }
                        if(!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            // 파일삭제
                            FileService.DeleteImageFile(MaintanceFileFolderPath, deleteFileName);
                        }
                    } // 파일이 공백인 경우
                    else
                    {
                        if(!String.IsNullOrWhiteSpace(deleteFileName))
                        {
                            // 삭제할거
                            FileService.DeleteImageFile(MaintanceFileFolderPath, deleteFileName);
                        }
                    }

                    return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
                }
                else
                {
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                }

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

    }
}
