using Microsoft.AspNetCore.Mvc;
using FamTec.Shared.Server.DTO.User;
using FamTec.Server.Services.User;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Client.DTO.Normal.Users;
using Microsoft.AspNetCore.Authorization;
using FamTec.Server.Services;
using Microsoft.JSInterop.Infrastructure;

namespace FamTec.Server.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService UserService;
        private IFileService FileService;
        private ILogService LogService;

        public UserController(IUserService _userservice,
            IFileService _fileservice,
            ILogService _logservice)
        {
            this.UserService = _userservice;
            this.FileService = _fileservice;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 로그인한 사업장의 모든 사용자 반환
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetPlaceUsers")]
        public async ValueTask<IActionResult> GetUserList()
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<ListUser>? model = await UserService.GetPlaceUserList(HttpContext);

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
        /// 사용자 추가
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddUser")]
        public async ValueTask<IActionResult> AddUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                //UsersDTO dto = new UsersDTO();
                //dto.USERID = "TESTUSER0003";
                //dto.PASSWORD = "123";
                //dto.NAME = "테스트사용자";
                //dto.PERM_BASIC = 2; // 기본정보메뉴 권한
                //dto.PERM_MACHINE = 2; // 기계메뉴 권한
                //dto.PERM_ELEC = 2; // 전기메뉴 권한
                //dto.PERM_LIFT = 2; // 승강메뉴 권한
                //dto.PERM_FIRE = 2; // 소방메뉴 권한
                //dto.PERM_CONSTRUCT = 2; // 건축메뉴 권한
                //dto.PERM_NETWORK = 2;
                //dto.PERM_BEAUTY = 2;
                //dto.PERM_SECURITY = 2;
                //dto.PERM_MATERIAL = 2;
                //dto.PERM_ENERGY = 2;
                //dto.PERM_USER = 2;
                //dto.PERM_VOC = 2;
                //dto.ALRAM_YN = true;
                //dto.STATUS = 2;
                //dto.VOC_MACHINE = true;
                //dto.VOC_ELEC = false;
                //dto.VOC_FIRE = true;
                //dto.VOC_CONSTRUCT = true;
                //dto.VOC_NETWORK = true;
                //dto.VOC_BEAUTY = true;
                //dto.VOC_SECURITY = true;
                //dto.VOC_ETC = true;
           
                if (HttpContext is null)
                    return BadRequest();


                if (String.IsNullOrWhiteSpace(dto.USERID)) return NoContent(); // 사용자ID
                if(String.IsNullOrWhiteSpace(dto.PASSWORD)) return NoContent(); // 사용자 비밀번호
                if(dto.PERM_BASIC is null) return NoContent(); // 기본정보메뉴 권한
                if(dto.PERM_MACHINE is null) return NoContent(); // 기계메뉴 권한
                if(dto.PERM_ELEC is null) return NoContent(); // 전기메뉴 권한
                if(dto.PERM_LIFT is null) return NoContent(); // 승강메뉴 권한
                if(dto.PERM_FIRE is null) return NoContent(); // 소방메뉴 권한
                if(dto.PERM_CONSTRUCT is null) return NoContent(); // 건축메뉴 권한
                if(dto.PERM_NETWORK is null) return NoContent(); // 통신메뉴 권한
                if(dto.PERM_BEAUTY is null) return NoContent(); // 미화메뉴 권한
                if(dto.PERM_SECURITY is null) return NoContent(); // 보안메뉴 권한
                if(dto.PERM_MATERIAL is null) return NoContent(); // 자재메뉴 권한
                if(dto.PERM_ENERGY is null) return NoContent(); // 에너지메뉴 권한
                if(dto.PERM_USER is null) return NoContent(); // 사용자메뉴 권한
                if(dto.PERM_VOC is null) return NoContent(); // VOC메뉴 권한
                if(dto.ALRAM_YN is null) return NoContent(); // 알람여부
                if(dto.STATUS is null) return NoContent(); // 재직여부
                if(dto.VOC_MACHINE is null) return NoContent(); // VOC 기계권한
                if(dto.VOC_ELEC is null) return NoContent(); // VOC 전기권한
                if(dto.VOC_LIFT is null) return NoContent(); // VOC 승강권한
                if(dto.VOC_FIRE is null) return NoContent(); // VOC 소방권한
                if(dto.VOC_CONSTRUCT is null) return NoContent(); // VOC 건축권한
                if(dto.VOC_NETWORK is null) return NoContent(); // VOC 통신권한
                if(dto.VOC_BEAUTY is null) return NoContent(); // VOC 미화권한
                if (dto.VOC_SECURITY is null) return NoContent(); // VOC 보안권한
                if (dto.VOC_ETC is null) return NoContent(); // VOC 기타권한
                
                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<int?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }

                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<UsersDTO> model = await UserService.AddUserService(HttpContext, dto, files);

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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DetailUser")]
        public async ValueTask<IActionResult> DetailUser([FromQuery]int id)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<UsersDTO> model = await UserService.GetUserDetails(HttpContext, id);
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

        [AllowAnonymous]
        [HttpGet]
        [Route("sign/DeleteUser")]
        public async ValueTask<IActionResult> DeleteUser([FromQuery] List<int> delIdx)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                
                if (delIdx is null)
                    return NoContent();

                if (delIdx.Count == 0)
                    return NoContent();

                ResponseUnit<bool?> model = await UserService.DeleteUserService(HttpContext, delIdx);

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

        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateUser")]
        public async ValueTask<IActionResult> UpdateUser([FromForm]UsersDTO dto, [FromForm]IFormFile? files)
        {
            try
            {
                //UsersDTO dto = new UsersDTO();
                //dto.ID = 50;
                //dto.USERID = "TESTUSER0003_1";
                //dto.PASSWORD = "123";
                //dto.NAME = "테스트사용자";
                //dto.PERM_BASIC = 2; // 기본정보메뉴 권한
                //dto.PERM_MACHINE = 2; // 기계메뉴 권한
                //dto.PERM_ELEC = 2; // 전기메뉴 권한
                //dto.PERM_LIFT = 2; // 승강메뉴 권한
                //dto.PERM_FIRE = 2; // 소방메뉴 권한
                //dto.PERM_CONSTRUCT = 2; // 건축메뉴 권한
                //dto.PERM_NETWORK = 2;
                //dto.PERM_BEAUTY = 2;
                //dto.PERM_SECURITY = 2;
                //dto.PERM_MATERIAL = 2;
                //dto.PERM_ENERGY = 2;
                //dto.PERM_USER = 2;
                //dto.PERM_VOC = 2;
                //dto.ALRAM_YN = true;
                //dto.STATUS = 2;
                //dto.VOC_MACHINE = true;
                //dto.VOC_ELEC = false;
                //dto.VOC_FIRE = true;
                //dto.VOC_CONSTRUCT = true;
                //dto.VOC_NETWORK = true;
                //dto.VOC_BEAUTY = true;
                //dto.VOC_SECURITY = true;
                //dto.VOC_ETC = true;


                if (HttpContext is null)
                    return BadRequest();

                if (dto.ID is null) return NoContent();
                if (String.IsNullOrWhiteSpace(dto.USERID)) return NoContent(); // 사용자ID
                if (String.IsNullOrWhiteSpace(dto.PASSWORD)) return NoContent(); // 사용자 비밀번호
                if (dto.PERM_BASIC is null) return NoContent(); // 기본정보메뉴 권한
                if (dto.PERM_MACHINE is null) return NoContent(); // 기계메뉴 권한
                if (dto.PERM_ELEC is null) return NoContent(); // 전기메뉴 권한
                if (dto.PERM_LIFT is null) return NoContent(); // 승강메뉴 권한
                if (dto.PERM_FIRE is null) return NoContent(); // 소방메뉴 권한
                if (dto.PERM_CONSTRUCT is null) return NoContent(); // 건축메뉴 권한
                if (dto.PERM_NETWORK is null) return NoContent(); // 통신메뉴 권한
                if (dto.PERM_BEAUTY is null) return NoContent(); // 미화메뉴 권한
                if (dto.PERM_SECURITY is null) return NoContent(); // 보안메뉴 권한
                if (dto.PERM_MATERIAL is null) return NoContent(); // 자재메뉴 권한
                if (dto.PERM_ENERGY is null) return NoContent(); // 에너지메뉴 권한
                if (dto.PERM_USER is null) return NoContent(); // 사용자메뉴 권한
                if (dto.PERM_VOC is null) return NoContent(); // VOC메뉴 권한
                if (dto.ALRAM_YN is null) return NoContent(); // 알람여부
                if (dto.STATUS is null) return NoContent(); // 재직여부
                if (dto.VOC_MACHINE is null) return NoContent(); // VOC 기계권한
                if (dto.VOC_ELEC is null) return NoContent(); // VOC 전기권한
                if (dto.VOC_LIFT is null) return NoContent(); // VOC 승강권한
                if (dto.VOC_FIRE is null) return NoContent(); // VOC 소방권한
                if (dto.VOC_CONSTRUCT is null) return NoContent(); // VOC 건축권한
                if (dto.VOC_NETWORK is null) return NoContent(); // VOC 통신권한
                if (dto.VOC_BEAUTY is null) return NoContent(); // VOC 미화권한
                if (dto.VOC_SECURITY is null) return NoContent(); // VOC 보안권한
                if (dto.VOC_ETC is null) return NoContent(); // VOC 기타권한

                if (files is not null)
                {
                    if (files.Length > Common.MEGABYTE_1)
                    {
                        return Ok(new ResponseUnit<int?>() { message = "이미지 업로드는 1MB 이하만 가능합니다.", data = null, code = 200 });
                    }

                    string? extension = FileService.GetExtension(files);
                    if (String.IsNullOrWhiteSpace(extension))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        bool extensioncheck = Common.ImageAllowedExtensions.Contains(extension);
                        if (!extensioncheck)
                        {
                            return Ok(new ResponseUnit<int?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                        }
                    }
                }

                ResponseUnit<UsersDTO>? model = await UserService.UpdateUserService(HttpContext, dto, files);

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
        /// 엑셀 IMPORT
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("sign/ImportUser")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile files)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (files is null)
                    return NoContent();

                if (files.Length == 0)
                    return NoContent();

                string? extension = FileService.GetExtension(files); // 파일 확장자 추출
                if(String.IsNullOrWhiteSpace(extension))
                {
                    return BadRequest();
                }
                else
                {
                    bool extensioncheck = Common.XlsxAllowedExtensions.Contains(extension); // 파일 확장자 검사 xlsx or xls 만 허용
                    if(!extensioncheck)
                    {
                        return Ok(new ResponseUnit<string?>() { message = "지원하지 않는 파일형식입니다.", data = null, code = 200 });
                    }
                }

                ResponseUnit<string?> model = await UserService.ImportUserService(HttpContext, files);
                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                {
                    return Ok(model);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.Message);
                return Problem("서버에서 처리할 수 없는 요청입니다.", statusCode: 500);
            }
        }

    }
}
