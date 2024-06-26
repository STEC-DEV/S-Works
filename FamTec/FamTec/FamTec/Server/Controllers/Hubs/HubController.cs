using FamTec.Server.Hubs;
using FamTec.Server.Services.Voc;
using FamTec.Shared.Client.DTO;
using FamTec.Shared.Server;
using FamTec.Shared.Server.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System.Net;

namespace FamTec.Server.Controllers.Hubs
{
    [Route("api/[controller]")]
    [ApiController]
    public class HubController : ControllerBase
    {
        private readonly IHubContext<BroadcastHub> HubContext;

        private readonly IWebHostEnvironment _env;

        private IVocService VocService;


        public HubController(IHubContext<BroadcastHub> _hubcontext, IVocService _vocservice, IWebHostEnvironment env)
        {
            this.HubContext = _hubcontext;
            this.VocService = _vocservice;
            this._env = env;

        }

        [HttpGet]
        [Route("Temp")]
        public async ValueTask<IActionResult> FileDownload()
        {
            
            string filepath = "C:\\Users\\kyw\\Pictures\\Screenshots\\"; //파일경로
            string filename = "스크린샷 2024-02-26 091117.png";
            string path = filepath + filename;
            // 파일을 바이트로 읽음
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return Ok(bytes);
          
        }

        [HttpPost]
        [Route("Files")]
        public async Task<IActionResult> UploadFile([FromForm]string obj, [FromForm]List<IFormFile> files)
        {
            //await HubContext.Clients.Group("35_BeautyRoom").SendAsync("ReceiveVoc", "보냄");
            //return Ok("asdasd");

            
            JObject? jobj = JObject.Parse(obj);

            ResponseUnit<string>? model = await VocService.AddVocService(obj, files);
            string? title = Convert.ToString(jobj["Title"]);
            
            if (model is not null)
            {
                if (model.code == 200)
                {
                    int Voctype = Int32.Parse(jobj["Type"]!.ToString()); // 종류
                    int PlaceId = Int32.Parse(jobj["PlaceIdx"]!.ToString()); // 사업장ID

                    switch (Voctype)
                    {
                        // 기계
                        case 1:
                            // obj에서 사업장+Room Name으로 Group항목에 넣어야함.
                            await HubContext.Clients.Group($"{PlaceId}_MachineRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 전기
                        case 2:
                            await HubContext.Clients.Group($"{PlaceId}_ElectricityRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 승강
                        case 3:
                            await HubContext.Clients.Group($"{PlaceId}_LiftRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 소방
                        case 4:
                            await HubContext.Clients.Group($"{PlaceId}_FireRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 건축
                        case 5:
                            await HubContext.Clients.Group($"{PlaceId}_ConstructRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 통신
                        case 6:
                            await HubContext.Clients.Group($"{PlaceId}_NetworkRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 미화
                        case 7:
                            await HubContext.Clients.Group($"{PlaceId}_BeautyRoom").SendAsync("ReceiveVoc", title);
                            return Ok("123");
                        // 보안
                        case 8:
                            await HubContext.Clients.Group($"{PlaceId}_SecurityRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        // 기타
                        case 9:
                            await HubContext.Clients.Group($"{PlaceId}_DefaultRoom").SendAsync("ReceiveVoc", title);
                            return Ok(model);
                        default:
                            return BadRequest();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
           
            
        }

    }
}
