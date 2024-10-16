﻿using FamTec.Server.Middleware;
using FamTec.Server.Services;
using FamTec.Server.Services.Building.Group;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FamTec.Server.Controllers.Building.Group
{
    [ServiceFilter(typeof(SlidingWindowPolicyFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class BuildingGroupController : ControllerBase
    {
        private IBuildingGroupService GroupService;
        private ILogService LogService;

        public BuildingGroupController(IBuildingGroupService _groupservice,
            ILogService _logservice)
        {
            this.GroupService = _groupservice;
            this.LogService = _logservice;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign/AddGroup")]
        public async Task<IActionResult> AddGroup([FromBody] AddGroupInfoDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.BuildingIdx is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.Name))
                    return NoContent();

                ResponseUnit<AddGroupInfoDTO> model = await GroupService.AddBuildingGroupInfoService(HttpContext, dto).ConfigureAwait(false);
                
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
        //[HttpGet]
        [HttpPost]
        [Route("sign/AddBuildingGroup")]
        //public async Task<IActionResult> AddBuildingGroup()
        public async Task<IActionResult> AddBuildingGroup([FromBody] List<AddGroupDTO> dto)
        {
            try
            {
                //List<AddGroupDTO> dto = new List<AddGroupDTO>();

                //AddGroupDTO GroupDTO = new AddGroupDTO();
                //GroupDTO.BuildingIdx = 1;
                //GroupDTO.Name = "B그룹";

                //// AddGroupKey가 null일 수 있으므로 초기화
                //GroupDTO.AddGroupKey = new List<AddGroupItemKeyDTO>();
                //GroupDTO.AddGroupKey.Add(new AddGroupItemKeyDTO
                //{
                //    Name = "B항목",
                //    Unit = "B단위",
                //    // ItemValues 리스트가 null일 수 있으므로 초기화
                //    ItemValues = new List<AddGroupItemValueDTO>()
                //});

                //// ItemValues에 새로운 AddGroupItemValueDTO 객체 추가
                //GroupDTO.AddGroupKey[0].ItemValues.Add(new AddGroupItemValueDTO
                //{
                //    Values = "값1"
                //});
                //GroupDTO.AddGroupKey[0].ItemValues.Add(new AddGroupItemValueDTO
                //{
                //    Values = "값2"
                //});
                //dto.Add(GroupDTO);

                //GroupDTO = new AddGroupDTO();
                //GroupDTO.BuildingIdx = 1;
                //GroupDTO.Name = "C그룹";

                //// AddGroupKey가 null일 수 있으므로 초기화
                //GroupDTO.AddGroupKey = new List<AddGroupItemKeyDTO>();
                //GroupDTO.AddGroupKey.Add(new AddGroupItemKeyDTO
                //{
                //    Name = "C항목",
                //    Unit = "C단위",
                //    // ItemValues 리스트가 null일 수 있으므로 초기화
                //    ItemValues = new List<AddGroupItemValueDTO>()
                //});

                //// ItemValues에 새로운 AddGroupItemValueDTO 객체 추가
                //GroupDTO.AddGroupKey[0].ItemValues.Add(new AddGroupItemValueDTO
                //{
                //    Values = "값1"
                //});
                //GroupDTO.AddGroupKey[0].ItemValues.Add(new AddGroupItemValueDTO
                //{
                //    Values = "값2"
                //});
                //dto.Add(GroupDTO);

                // ------------- DTO 검사
                if (dto is null)
                    return NoContent();

                foreach(AddGroupDTO group in dto)
                {
                    if(group.BuildingIdx is null || group.BuildingIdx == 0)
                        return NoContent();
                    if(String.IsNullOrWhiteSpace(group.Name))
                        return NoContent();

                    if (group.AddGroupKey is [_, ..])
                    {
                        foreach (var key in group.AddGroupKey)
                        {
                            if (String.IsNullOrWhiteSpace(key.Name))
                                return NoContent();
                            if (String.IsNullOrWhiteSpace(key.Unit))
                                return NoContent();

                            if (key.ItemValues is [_, ..])
                            {
                                foreach (var value in key.ItemValues)
                                {
                                    if (String.IsNullOrWhiteSpace(value.Values))
                                        return NoContent();
                                }
                            }
                        }
                    }
                }


                if (HttpContext is null)
                    return BadRequest();

                ResponseUnit<bool> model = await GroupService.AddBuildingGroupService(HttpContext, dto).ConfigureAwait(false);

                if (model is null)
                    return BadRequest();

                if (model.code == 200)
                    return Ok(model);
                else if(model.code == 201)
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
        /// 건물의 하위정보 전체조회
        /// </summary>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("sign/GetBuildingGroup")]
        public async Task<IActionResult> GetDetailBuilding(int buildingid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                ResponseList<GroupListDTO?> model = await GroupService.GetBuildingGroupListService(HttpContext, buildingid).ConfigureAwait(false);
                
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
        /// 건물 그룹정보 수정
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPut]
        [Route("sign/UpdateGroup")]
        public async Task<IActionResult> UpdateBuildingGroup([FromBody] UpdateGroupDTO dto)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();

                if (dto.GroupId is null)
                    return NoContent();

                if (String.IsNullOrWhiteSpace(dto.GroupName))
                    return NoContent();

                ResponseUnit<bool?> model = await GroupService.UpdateGroupNameService(HttpContext, dto).ConfigureAwait(false);
                
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
        [HttpPost]
        [Route("sign/DeleteGroup")]
        public async Task<IActionResult> DeleteBuildingGroup([FromBody]int groupid)
        {
            try
            {
                if (HttpContext is null)
                    return BadRequest();
                
                ResponseUnit<bool?> model = await GroupService.DeleteGroupService(HttpContext, groupid).ConfigureAwait(false);
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


    }
}
