using FamTec.Server.Helpers;
using FamTec.Server.Repository.Building.SubItem.Group;
using FamTec.Server.Repository.Building.SubItem.ItemKey;
using FamTec.Server.Repository.Building.SubItem.ItemValue;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Building.Group;
using FamTec.Shared.Server.DTO.Building.Group.Key;

namespace FamTec.Server.Services.Building.Key
{
    public class BuildingKeyService : IBuildingKeyService
    {
        private readonly IBuildingGroupItemInfoRepository BuildingGroupItemInfoRepository;
        private readonly IBuildingItemKeyInfoRepository BuildingItemKeyInfoRepository;
        private readonly IBuildingItemValueInfoRepository BuildingItemValueInfoRepository;
        private readonly IHttpContextAccessor HttpContextAccessor; /* HttpContext 의존성 주입 */
        private readonly ILogService LogService; /* 파일로그 */
        private readonly ConsoleLogService<BuildingKeyService> CreateBuilderLogger; /* 콘솔로그 */

        public BuildingKeyService(IBuildingGroupItemInfoRepository _buildinggroupiteminforepository,
            IBuildingItemKeyInfoRepository _buildingItemkeyinforepository,
            IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            IHttpContextAccessor _httpcontextaccessor,
            ILogService _logservice,
            ConsoleLogService<BuildingKeyService> _createbuilderlogger)
        {
            this.BuildingGroupItemInfoRepository = _buildinggroupiteminforepository;
            this.BuildingItemKeyInfoRepository = _buildingItemkeyinforepository;
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            this.HttpContextAccessor = _httpcontextaccessor;
            this.LogService = _logservice;
            this.CreateBuilderLogger = _createbuilderlogger;
        }

        /// <summary>
        /// 키 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<AddKeyDTO>> AddKeyService(AddKeyDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemGroupTb? GroupTb = await BuildingGroupItemInfoRepository.GetGroupInfo(dto.GroupID!.Value).ConfigureAwait(false);
                if (GroupTb is null) // 기존의 GroupTB 이 존재하는지 Check
                    return new ResponseModel<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                BuildingItemKeyTb KeyTb = new BuildingItemKeyTb()
                {
                    Name = !String.IsNullOrWhiteSpace(dto.Name) ? dto.Name.Trim() : dto.Name!, /* 키 명칭 */
                    Unit = dto.Unit, /* 단위 */
                    CreateDt = ThisTime,
                    CreateUser = creater,
                    UpdateDt = ThisTime,
                    UpdateUser = creater,
                    BuildingGroupTbId = dto.GroupID.Value
                };

                BuildingItemKeyTb? AddkeyResult = await BuildingItemKeyInfoRepository.AddAsync(KeyTb).ConfigureAwait(false);
                if(AddkeyResult is null)
                    return new ResponseModel<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };

                if (dto.ItemValues is not null && dto.ItemValues.Any())
                {
                    foreach (AddGroupItemValueDTO GroupDTO in dto.ItemValues)
                    {
                        BuildingItemValueTb ValueTB = new BuildingItemValueTb()
                        {
                            ItemValue = !String.IsNullOrWhiteSpace(GroupDTO.Values) ? GroupDTO.Values.Trim() : GroupDTO.Values!,
                            CreateDt = ThisTime,
                            CreateUser = creater,
                            UpdateDt = ThisTime,
                            UpdateUser = creater,
                            BuildingKeyTbId = AddkeyResult.Id
                        };

                        BuildingItemValueTb? result = await BuildingItemValueInfoRepository.AddAsync(ValueTB).ConfigureAwait(false);
                        if(result is null)
                            return new ResponseModel<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
                    }
                }

                return new ResponseModel<AddKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel<UpdateKeyDTO>> UpdateKeyService(UpdateKeyDTO dto)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null || dto is null)
                    return new ResponseModel<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                
                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(dto.ID!.Value).ConfigureAwait(false);
                if(KeyTB is null)
                    return new ResponseModel<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                bool? UpdateResult = await BuildingItemKeyInfoRepository.UpdateKeyInfo(dto, creater).ConfigureAwait(false);

                return UpdateResult switch
                {
                    true => new ResponseModel<UpdateKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 },
                    false => new ResponseModel<UpdateKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 },
                    _ => new ResponseModel<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<UpdateKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 };
            }
        }

        public async Task<ResponseModel<bool?>> DeleteKeyListService(List<int> KeyId)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? DeleteResult = await BuildingItemKeyInfoRepository.DeleteKeyList(KeyId, creater).ConfigureAwait(false);
                
                return DeleteResult switch
                {
                    true => new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseModel<bool?>> DeleteKeyService(int KeyId)
        {
            try
            {
                var context = HttpContextAccessor.HttpContext;

                if (context is null)
                    return new ResponseModel<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(KeyId).ConfigureAwait(false);

                DateTime ThisTime = DateTime.Now;

                if(KeyTB is null)
                    return new ResponseModel<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                KeyTB.DelDt = ThisTime;
                KeyTB.DelUser = creater;
                KeyTB.DelYn = true;

                bool? DeleteKeyResult = await BuildingItemKeyInfoRepository.DeleteKeyInfo(KeyTB).ConfigureAwait(false);

                if(DeleteKeyResult != true)
                    return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

                List<BuildingItemValueTb>? ItemTB = await BuildingItemValueInfoRepository.GetAllValueList(KeyId).ConfigureAwait(false);
                if(ItemTB is [_, ..])
                {
                    foreach(BuildingItemValueTb Item in ItemTB)
                    {
                        Item.DelDt = ThisTime;
                        Item.DelUser = creater;
                        Item.DelYn = true;

                        bool? DeleteValueResult = await BuildingItemValueInfoRepository.DeleteValueInfo(Item).ConfigureAwait(false);
                        if(DeleteValueResult != true)
                            return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }

                return new ResponseModel<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger.ConsoleLog(ex);
#endif
                return new ResponseModel<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    
    }
}
