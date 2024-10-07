using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Admin.Place;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Admin;
using FamTec.Shared.Server.DTO.Admin.Place;
using FamTec.Shared.Server.DTO.Place;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Admin.AdminPlaces
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminPlaceController : ControllerBase
    {
        private IAdminPlaceService AdminPlaceService;
        private ILogService LogService;


        public AdminPlaceController(IAdminPlaceService _adminplaceservice,
            ILogService _logservice)
        {
            this.AdminPlaceService = _adminplaceservice;
            this.LogService = _logservice;
        }

        /*
        [HttpPost]
        [Route("temp")]
        public async Task<IActionResult> Temp(IFormFile files)
        {
            string str = "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Uk0AAHomAACAhAAA+gAAAIDoAAB1MAAA6mAAADqYAAAXcJy6UTwAAAAJcEhZcwAACxMAAAsTAQCanBgAACeJSURBVHic7d17lN1lYe//z55bZiZDEiQJFxGttjbgiVZDxdCzArVKY7S42tLoz+NCSyxWEY0EFE8TREgVo7CweCn0QLWlFqKtLcU08kN/MR4JiDm2h1uKgjGIYAwsJZOZzHX//pgkMJnJZZKZZ89MXq+1ZmXx/e7Z+9nJWsx+z/f7PE+lq6urGgAAgALqaj0AAADgyCFAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAAChGgAAAAMUIEAAAoBgBAgAAFCNAAACAYgQIAABQjAABAACKESAAAEAxAgQAACimodYDADiSPfjgg7ntttty55135oEHHkiSVKvVGo9qcqlUKkmSl73sZXnd616Xs88+O6ecckqNRwVw5Kp0dXX5SQdQA9/+9rezYsWK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gJETIMCEcPrppxd5nSVLlmTJkiVFXutQ7Os3+snA2CdigOyOqhtuuCFbt24dcn7p0qVpa2vLmWeemdbW1tLDA2CUCRAAitu2bVv+9V//NTfeeOMBH3vttdcmGYivhQsXZvHixZkzZ84Yj3D/1qxZs98YLOmmm26q+d8HwEgIEIBJYvv27bUewkE5nA/va9euzdq1a7N48eKcf/75rogATECW4QWYILZt27bf84899lihkRy6G2+8cVSuHKxevTorVqxIR0fHKIwKgJIECMAE8eCDD+73/IYNG8b1B/LVq1cf1C1XB2vDhg1ZsWLFqD0fAGW4BQtggli3bt1BPWbRokVjP5gR2rJly565HMOZM2dOzjnnnCFjX79+fdatW5e1a9cO+30bNmzImjVrxuV7BmB4AgSYEO66664DPmbjxo258MIL93n+tttuy8yZM0dzWMVs2bJlnx/Cn+uGG24Yl6tF/fM///M+z+1v5bEFCxZkwYIFOfPMM3PppZcO+5gbbrhBgABMIAIEmDR+/vOf7/f8tm3bJmyAfOYznzmox23dujW33357Fi9ePMYjGpl9Xb2ZM2fOQS17vGDBgixZsmTYW7i2bt2aLVu25KSTTjrcYR60RYsWFY2eK6644qACFGAiMAcEmDTa29v3e/7RRx8tNJLRtXr16mzYsOGgH3/ttddm48aNYziikdmyZcuw+3skAzuSH6zXv/71+zx3//33j3hcANSGKyDApHHPPffs9/z3v//9/f7Wejz+lnn9+vX7nTuxLxdeeGFuueWWolcF9mV/E+OPO+64g36e8XZbGQCHxhUQYFLYsmXLAa8SrF27dlyvErW3NWvW7HPeQ5IDbj731re+NZs2bRrtYY2qA121AmDyESDApHD33Xcf1OMOZiWpWuvo6Mi11157wP0yLrjggixdunS/jznvvPOyevXqURzdyO3vKszDDz980M+zv2WIjz322BGNCYDacQsWMOF1dHTky1/+8kE99oYbbsirX/3qcTsZfc2aNbnhhhv2OWdit6VLl2bevHmZN29e7rnnnv1e/bn22muzdu3aXHDBBZk3b95oD/mAWltbM3/+/GHHuHr16vzRH/3RAW8V6+joyBe/+MV9nj/55JMPd5gjcji7uQMc6VwBASa8f/zHfzzgB/bdtm7dmptvvnmMRzRya9asyXnnnZeVK1ce8L0s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";
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            // byte[]를 메모리 스트림으로 변환
            var stream = new MemoryStream(byteArray);

            string fileName = "파일명.png";

            // IFormFile로 변환
            // IFormFile 생성
            IFormFile formFile = new FormFile(stream, 0, byteArray.Length, "files", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png", // 파일 타입에 맞게 수정
                ContentDisposition = $"form-data; name=\"files\"; filename=\"{fileName}\"; filename*=UTF-8''{Uri.EscapeDataString(fileName)}"
            };

            string filePath = Path.Combine("C:\\Users\\kyw\\Documents\\카카오톡 받은 파일", fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await files.CopyToAsync(fileStream);
            }

            Console.WriteLine("asdgasg");
            return Ok(formFile);
        }
        */

        /// <summary>
        /// 전체 사업장 리스트 조회 [OK]
        /// [매니저는 본인이 할당된 것 만 출력]
        /// [토큰 적용완료]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetAllWorksList")]
        public async Task<IActionResult> GetAllWorksList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AllPlaceDTO> model = await AdminPlaceService.GetAllWorksService(HttpContext).ConfigureAwait(false);
                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return Ok(model);

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 관리자정보 전체조회 [OK]
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/GetAllManagerList")]
        public async Task<IActionResult> GetAllManagerList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ManagerListDTO> model = await AdminPlaceService.GetAllManagerListService().ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 선택된 매니저가 관리하는 사업장 LIST반환
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/MyWorks")]
        public async Task<IActionResult> GetMyWorks([FromQuery] int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.GetMyWorksService(adminid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();

            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }

        }

        /// <summary>
        /// 사업장 상세정보
        /// </summary>
        /// <param name="placeid">사업장ID</param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/DetailWorks")]
        public async Task<IActionResult> DetailWorks([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<PlaceDetailDTO> model = await AdminPlaceService.GetPlaceService(placeid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 등록
        /// </summary>
        /// <param name="dto">추가할 사업장정보 DTO</param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddWorks")]
        public async Task<IActionResult> AddWorks([FromBody]AddPlaceDTO dto)
        {
            try
            {
                
                //AddPlaceDTO dto = new AddPlaceDTO();
                //dto.PlaceCd = "AB000000004"; // 사업장코드
                //dto.Name = "C사업장"; // 사업장명
                //dto.Tel = "02-0000-0000"; // 사업장 전화번호
                //dto.Address = "서울시 강서구"; // 사업장 주소
                //dto.ContractNum = "00054487"; // 계약번호
                //dto.ContractDT = DateTime.Now; // 계약일자
                //dto.PermMachine = true; // 설비메뉴 권한
                //dto.PermLift = true; // 승강메뉴 권한
                //dto.PermFire = true; // 소방메뉴 권한
                //dto.PermConstruct = true; // 건축메뉴 권한
                //dto.PermNetwork = true; // 통신메뉴 권한
                //dto.PermBeauty = false; // 미화메뉴 권한
                //dto.PermSecurity = false; // 보안메뉴 권한
                //dto.PermMaterial = false; // 자재메뉴 권한
                //dto.PermEnergy = false; // 에너지 메뉴 권한
                //dto.PermVoc = false; // VOC 권한
                //dto.Status = true; // 계약상태
                //dto.Note = "테스트데이터";
                

                if (String.IsNullOrWhiteSpace(dto.PlaceCd))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Tel))
                    return NoContent();

                if (dto.PermMachine == null)
                    return NoContent();

                if (dto.PermLift == null)
                    return NoContent();

                if (dto.PermFire == null)
                    return NoContent();

                if (dto.PermElec == null)
                    return NoContent();

                if (dto.PermConstruct == null)
                    return NoContent();

                if (dto.PermNetwork == null)
                    return NoContent();

                if (dto.PermBeauty == null)
                    return NoContent();

                if (dto.PermSecurity == null)
                    return NoContent();

                if (dto.PermMaterial == null)
                    return NoContent();

                if (dto.PermEnergy == null)
                    return NoContent();

                if (dto.PermVoc == null)
                    return NoContent();
                

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<int?> model = await AdminPlaceService.AddPlaceService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();
                
                // 성공
                if (model.code == 200) 
                    return Ok(model);
                // 이미 해당 코드가 사용한 이력이 있을때
                else if (model.code == 202) 
                    return Ok(model);
                // 실패
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장 삭제
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeleteWorks")]
        
        public async Task<IActionResult> DeleteWorks([FromBody]List<int> placeidx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                if (placeidx is null)
                    return NoContent();
                if (placeidx.Count == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await AdminPlaceService.DeletePlaceService(HttpContext, placeidx).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        
        /// <summary>
        /// 사업장 정보 수정
        /// </summary>
        /// <param name="dto">
        ///     수정할 DTO
        ///         - dto.PlaceInfo.Id  * 필수값
        ///         - dto.PlaceInfo.PlaceCd * 필수값
        ///         - dto.PlaceInfo.Name * 필수값
        ///         - dto.PlaceInfo.Tel * 필수값
        ///         - dto.PlaceInfo.Status * 계약상태
        ///         - dto.PlaceInfo.DepartmentID * 부서ID
        ///         
        ///         - dto.PlacePerm * 필수값
        /// </param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/UpdateWorks")]
        public async Task<IActionResult> UpdateWorks([FromBody]UpdatePlaceDTO dto)
        {
            try
            {
                /*
                UpdatePlaceDTO dto = new UpdatePlaceDTO();
                dto.PlaceInfo.Id = 3;
                dto.PlaceInfo.Name = "A수정사업장";
                dto.PlaceInfo.PlaceCd = "P001";
                dto.PlaceInfo.ContractNum = "ABC123";
                dto.PlaceInfo.Tel = "02-123-1234";
                dto.PlaceInfo.DepartmentID = 5;
                dto.PlaceInfo.Status = true;

                dto.PlacePerm.PermMachine = true;
                dto.PlacePerm.PermElec = true;
                dto.PlacePerm.PermLift = true;
                dto.PlacePerm.PermFire = true;
                dto.PlacePerm.PermConstruct = true;
                dto.PlacePerm.PermNetwork = true;
                dto.PlacePerm.PermBeauty = true;
                dto.PlacePerm.PermSecurity = true;
                dto.PlacePerm.PermMaterial = true;
                dto.PlacePerm.PermEnergy = true;
                dto.PlacePerm.PermVoc = true;
                */



                if (dto.PlaceInfo.Id is null)
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.PlaceCd))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.Name))
                    return NoContent();

                if(String.IsNullOrWhiteSpace(dto.PlaceInfo.Tel))
                    return NoContent();

                if(dto.PlaceInfo.Status is null)
                    return NoContent();

                if(dto.PlacePerm.PermMachine is null)
                    return NoContent();

                if(dto.PlacePerm.PermElec is null)
                    return NoContent();

                if(dto.PlacePerm.PermLift is null)
                    return NoContent();

                if(dto.PlacePerm.PermFire is null)
                    return NoContent();

                if(dto.PlacePerm.PermConstruct is null)
                    return NoContent();

                if(dto.PlacePerm.PermNetwork is null)
                    return NoContent();

                if(dto.PlacePerm.PermBeauty is null)
                    return NoContent();

                if(dto.PlacePerm.PermSecurity is null)
                    return NoContent();

                if(dto.PlacePerm.PermMaterial is null)
                    return NoContent();

                if(dto.PlacePerm.PermEnergy is null)
                    return NoContent();

                if(dto.PlacePerm.PermVoc is null)
                    return NoContent();


                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UpdatePlaceDTO> model = await AdminPlaceService.UpdatePlaceService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 204)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 포함되어있지 않은 관리자 리스트 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainManagerList")]
        public async Task<IActionResult> NotContainManagerList([FromQuery]int placeid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (placeid is 0)
                    return NoContent();

                ResponseList<ManagerListDTO> model = await AdminPlaceService.NotContainManagerList(HttpContext, placeid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 해당 관리자가 가지고 있지 않은 사업장 List 조회
        /// </summary>
        /// <param name="adminid"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpGet]
        [Route("sign/NotContainPlaceList")]
        public async Task<IActionResult> NotContainPlaceList([FromQuery]int adminid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (adminid is 0)
                    return NoContent();

                ResponseList<AdminPlaceDTO> model = await AdminPlaceService.NotContainPlaceList(HttpContext, adminid).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest();
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에 관리자 추가
        /// </summary>
        /// <param name="placemanager"></param>
        /// <returns></returns>
        [Authorize(Roles = "SystemManager, Master, Manager")]
        [HttpPost]
        [Route("sign/AddPlaceManager")]
        public async Task<IActionResult> AddPlaceManager([FromBody]AddPlaceManagerDTO<ManagerListDTO> placemanager)
        {
            try
            {
                //AddPlaceManagerDTO<ManagerListDTO> placemanager = new AddPlaceManagerDTO<ManagerListDTO>();
                //placemanager.PlaceId = 3;
                //placemanager.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 15
                //});
                //placemanager.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 16
                //});

                if(placemanager.PlaceId is null)
                    return NoContent();
                
                if(placemanager.PlaceManager is null)
                    return NoContent();
                
                foreach (ManagerListDTO ManagerInfo in placemanager.PlaceManager)
                {
                    if(ManagerInfo.Id is null)
                        return NoContent();
                }

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminPlaceService.AddPlaceManagerService(HttpContext, placemanager).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);

                if (model.code == 200)
                    return Ok(model);
                else if (model.code == 202) // 이미 포함되어있는 관리자
                    return Ok(model);
                else
                    return BadRequest(model);
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

        /// <summary>
        /// 사업장에서 관리자 삭제
        /// </summary>
        /// <param name="DeletePlace"></param>
        /// <returns></returns>
        [Authorize(Roles ="SystemManager, Master, Manager")]
        [HttpPut]
        [Route("sign/DeletePlaceManager")]
        public async Task<IActionResult> DeleteWorks([FromBody] AddPlaceManagerDTO<ManagerListDTO> dto)
        {
            try
            {
                //AddPlaceManagerDTO<ManagerListDTO> dto = new AddPlaceManagerDTO<ManagerListDTO>();
                //dto.PlaceId = 3;
                //dto.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 15
                //});
                //dto.PlaceManager.Add(new ManagerListDTO
                //{
                //    Id = 16
                //});

                if (dto.PlaceId is null)
                    return NoContent();
                
                if(dto.PlaceManager is null)
                    return NoContent();
                
                foreach (ManagerListDTO ManagerList in dto.PlaceManager)
                {
                    if(ManagerList.Id is null)
                        return NoContent();
                }

                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool?> model = await AdminPlaceService.DeleteManagerPlaceService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest(model);
                
                if (model.code == 200)
                    return Ok(model);
                else
                    return BadRequest(model);

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

      

        

    }
}
