﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Store;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.DashBoard;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Store
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IInVentoryService InStoreService;
        
        private readonly ILogService LogService;
        private readonly ConsoleLogService<StoreController> CreateBuilderLogger;

        public StoreController(IInVentoryService _instoreservice,
            ILogService _logservice,
            ConsoleLogService<StoreController> _createbuilderlogger)
        {
            this.InStoreService = _instoreservice;
            
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

// ############################### 대쉬보드

        /// <summary>
        /// 대쉬보드용 금일 입출고내역 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetToDayInOutCount")]
        public async Task<IActionResult> GetToDayInOutCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<InOutListDTO?> model = await InStoreService.GetDashBoardInOutListData(HttpContext);
                
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 대쉬보드용 품목별 재고 현황
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/v2/GetInventoryAmount")]
        public async Task<IActionResult> GetInventoryAmount([FromQuery]List<int> MaterialIdx)
        {
            try
            {

                if (HttpContext is null)
                    return BadRequest();

                if (MaterialIdx is null || MaterialIdx.Count == 0)
                    return NoContent();

                ResponseList<InventoryAmountDTO>? model = await InStoreService.GetDashBoardInvenAmountData(HttpContext, MaterialIdx);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        #region Regacy

        /*
        /// <summary>
        /// 자재별 일주일치 입출고 카운트 -- 대쉬보드
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetInOutWeekCount")]
        public async Task<IActionResult> GetInOutWeekCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<MaterialWeekCountDTO>? model = await InStoreService.GetInoutDashBoardDataService(HttpContext);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }
        */

        #endregion

        // ############################### 대쉬보드


        /// <summary>
        /// 입고 등록 - 수정완료
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddInStore")]
        public async Task<IActionResult> AddInStore([FromBody] List<InOutInventoryDTO> dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                foreach(InOutInventoryDTO InOutDTO in dto)
                {
                    if (InOutDTO.InOut is null)
                        return NoContent();
                    if (InOutDTO.MaterialID is null)
                        return NoContent();
                    if(InOutDTO.AddStore!.RoomID is null)
                        return NoContent();
                    if(InOutDTO.AddStore!.Num is null)
                        return NoContent();
                }

                ResponseUnit<int?> model = await InStoreService.AddInStoreService(HttpContext, dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 출고 등록 - 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/OutInventory")]
        public async Task<IActionResult> OutInventoryService([FromBody] List<InOutInventoryDTO> dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                foreach (InOutInventoryDTO InOutDTO in dto)
                {
                    if (InOutDTO.InOut is null)
                        return NoContent();
                    if (InOutDTO.MaterialID is null)
                        return NoContent();
                    if (InOutDTO.AddStore!.RoomID is null)
                        return NoContent();
                    if (InOutDTO.AddStore!.Num is null)
                        return NoContent();
                }

                ResponseUnit<FailResult?> model = await InStoreService.OutInventoryService(HttpContext, dto).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 422)
                    return Ok(model);
                else if (model.code == 409)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }

        }

        /// <summary>
        /// 입출고 이력 전체 조회
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetHistory")]
        public async Task<IActionResult> GetInoutHistory()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<InOutHistoryListDTO>? model = await InStoreService.GetInOutHistoryService(HttpContext).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 입출고 이력 페이지네이션 조회
        /// </summary>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPageNationHistory")]
        public async Task<IActionResult> GetInoutPageNationHistory([FromQuery] int pagenum, [FromQuery] int pagesize)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (pagenum == 0)
                    return NoContent();

                if(pagesize == 0 || pagesize > 100)
                    return NoContent();

                ResponseList<InOutHistoryListDTO>? model = await InStoreService.GetInoutPageNationHistoryService(HttpContext, pagenum, pagesize).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장의 입-출고 이력 개수 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetAllInoutPlaceCount")]
        public async Task<IActionResult> GetAllInoutPlaceCount()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await InStoreService.GetPlaceInOutCountService(HttpContext).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 별 폼목별 재고 현황 
        ///     - true (재고가 없는것도) : false: 재고가 하나라도 있는것만
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceInventoryStatus")]
        public async Task<IActionResult> GetPlaceInventoryStatus([FromQuery]List<int> materialid, [FromQuery]bool type)
        {
            try
            {
                //List<int> materialid = new List<int>() { 10,11 };
                //bool type = false;

                if (HttpContext is null)
                    return BadRequest();

                if (materialid is null)
                    return NoContent();

                if(materialid.Count() == 0)
                    return NoContent();

                ResponseList<MaterialHistory>? model = await InStoreService.GetPlaceInventoryRecordService(HttpContext, materialid, type).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 기간별 입출고 내역
        /// </summary>
        /// <param name="materialid"></param>
        /// <param name="Startdate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPeriodicRecord")]
        public async Task<IActionResult> PeriodicRecord([FromQuery] List<int> materialid, [FromQuery]DateTime Startdate, [FromQuery]DateTime EndDate)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<PeriodicDTO>? model = await InStoreService.PeriodicInventoryRecordService(HttpContext, materialid, Startdate, EndDate).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 공간의 재고수량 Return
        /// </summary>
        /// <param name="materialid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetLocationMaterial")]
        public async Task<IActionResult> GetLocationMaterial([FromQuery]int materialid, [FromQuery]int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (materialid is 0)
                    return NoContent();

                ResponseList<InOutLocationDTO> model = await InStoreService.GetMaterialRoomNumService(HttpContext, materialid, buildingid).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 공간의 재고수량 Return
        /// </summary>
        /// <param name="materialid"></param>
        /// <param name="roomid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetLocationMaterialNum")]
        public async Task<IActionResult> GetLocationMaterialNum([FromQuery]int materialid, [FromQuery]int roomid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (materialid is 0)
                    return NoContent();

                if (roomid is 0)
                    return NoContent();

                ResponseUnit<InOutLocationDTO> model = await InStoreService.GetMaterialRoomInventoryNumService(HttpContext, materialid, roomid).ConfigureAwait(false);
                
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }
        

        /// <summary>
        /// 출고 리스트에 담음 - FRONT 용
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="materialid"></param>
        /// <param name="outcount"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/AddOutStoreList")]
        public async Task<IActionResult> AddOutStoreList([FromQuery]int roomid, [FromQuery]int materialid, [FromQuery]int outcount)
        {
            try
            {
                //int roomid = 3;
                //int materialid = 10;
                //int outcount = 120;

                if (HttpContext is null)
                    return BadRequest();

                if (roomid is 0)
                    return NoContent();
                if(materialid is 0)
                    return NoContent();
                if(outcount is 0)
                    return NoContent();

                ResponseUnit<InOutInventoryDTO>? model = await InStoreService.AddOutStoreList(HttpContext, roomid, materialid, outcount).ConfigureAwait(false);
                if (model is null)
                    return BadRequest();

#if DEBUG
                CreateBuilderLogger.ConsoleText($"{model.code.ToString()} --> {HttpContext.Request.Path.Value}");
#endif

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
