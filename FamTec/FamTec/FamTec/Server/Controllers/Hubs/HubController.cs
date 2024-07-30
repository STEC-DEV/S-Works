using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using FamTec.Server.Hubs;
using FamTec.Server.Services;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.User;
using FamTec.Shared.Server.DTO.Voc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Data;

namespace FamTec.Server.Controllers.Hubs
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubContext<BroadcastHub> HubContext;
        private IVocService VocService;
        private ILogService LogService;
        //private readonly IWebHostEnvironment _env;


        /*
        public HubController(IHubContext<BroadcastHub> _hubcontext, IVocService _vocservice, IWebHostEnvironment env)
        {
            this.HubContext = _hubcontext;
            this.VocService = _vocservice;
            this._env = env;
        }
        */

        public HubController(IHubContext<BroadcastHub> _hubcontext,
            IVocService _vocservice,
            ILogService _logservice)
        {
            this.HubContext = _hubcontext;
            this.VocService = _vocservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 민원접수
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddVoc")]
        public async Task<IActionResult> AddVoc([FromForm] AddVocDTO dto, [FromForm] List<IFormFile>? files)
        {
            try 
            {
                // 확장자 검사
                if(files is [_, ..])
                {
                    foreach(IFormFile file in files)
                    {
                        string? FileName = file.Name;
                        string? FileExtenstion = Path.GetExtension(file.FileName);

                        // VOC 이미지는 2MB 제한
                        if(file.Length > 2097152)
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "이미지 업로드는 2MB 이하만 가능합니다.", data = null, code = 200 });
                        }

                        if(!Common.ImageAllowedExtensions.Contains(FileExtenstion))
                        {
                            return Ok(new ResponseUnit<bool?>() { message = "올바르지 않은 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }


                ResponseUnit<bool?> model = await VocService.AddVocService(dto, files);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                {
                    // 미분류로 알람전송
                    await HubContext.Clients.Group($"{dto.Placeid}_ETCRoom").SendAsync("ReceiveVoc", dto.Title);
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
                

                //if (model is not null)
                //{
                //    if (model.code == 200)
                //    {
                //        int Voctype = Int32.Parse(jobj["Type"]!.ToString()); // 종류
                //        int PlaceId = Int32.Parse(jobj["PlaceIdx"]!.ToString()); // 사업장ID

                //        switch (Voctype)
                //        {
                //            // 기계
                //            case 1:
                //                // obj에서 사업장+Room Name으로 Group항목에 넣어야함.
                //                await HubContext.Clients.Group($"{PlaceId}_MachineRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 전기
                //            case 2:
                //                await HubContext.Clients.Group($"{PlaceId}_ElectricityRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 승강
                //            case 3:
                //                await HubContext.Clients.Group($"{PlaceId}_LiftRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 소방
                //            case 4:
                //                await HubContext.Clients.Group($"{PlaceId}_FireRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 건축
                //            case 5:
                //                await HubContext.Clients.Group($"{PlaceId}_ConstructRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 통신
                //            case 6:
                //                await HubContext.Clients.Group($"{PlaceId}_NetworkRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 미화
                //            case 7:
                //                await HubContext.Clients.Group($"{PlaceId}_BeautyRoom").SendAsync("ReceiveVoc", title);
                //                return Ok("123");
                //            // 보안
                //            case 8:
                //                await HubContext.Clients.Group($"{PlaceId}_SecurityRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            // 기타
                //            case 9:
                //                await HubContext.Clients.Group($"{PlaceId}_DefaultRoom").SendAsync("ReceiveVoc", title);
                //                return Ok(model);
                //            default:
                //                return BadRequest();
                //        }
                //    }
                //    else
                //    {
                //        return BadRequest();
                //    }
                //}
                //else
                //{
                //    return BadRequest();
                //}
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리하지 못함", statusCode: 500);
            }
        }

       



    }
}
