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

        private readonly ILogService LogService;
        private readonly ILogger<BuildingKeyService> BuilderLogger;

        public BuildingKeyService(
            IBuildingGroupItemInfoRepository _buildinggroupiteminforepository,
            IBuildingItemKeyInfoRepository _buildingItemkeyinforepository,
            IBuildingItemValueInfoRepository _buildingitemvalueinforepository,
            ILogService _logservice,
            ILogger<BuildingKeyService> _builderlogger)
        {
            this.BuildingGroupItemInfoRepository = _buildinggroupiteminforepository;
            this.BuildingItemKeyInfoRepository = _buildingItemkeyinforepository;
            this.BuildingItemValueInfoRepository = _buildingitemvalueinforepository;
            
            this.LogService = _logservice;
            this.BuilderLogger = _builderlogger;
        }

        /// <summary>
        /// ASP - 빌드로그
        /// </summary>
        /// <param name="ex"></param>
        private void CreateBuilderLogger(Exception ex)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black; // 배경색 설정
                Console.ForegroundColor = ConsoleColor.Red; // 텍스트 색상 설정
                BuilderLogger.LogError($"ASPlog {ex.Source}\n {ex.StackTrace}");
                Console.ResetColor();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 키 추가
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<AddKeyDTO>> AddKeyService(HttpContext context, AddKeyDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

                DateTime ThisTime = DateTime.Now;

                BuildingItemGroupTb? GroupTb = await BuildingGroupItemInfoRepository.GetGroupInfo(dto.GroupID!.Value).ConfigureAwait(false);
                if (GroupTb is null) // 기존의 GroupTB 이 존재하는지 Check
                    return new ResponseUnit<AddKeyDTO>() { message = "잘못된 요청입니다.", data = new AddKeyDTO(), code = 404 };

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
                    return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };

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
                            return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
                    }
                }

                return new ResponseUnit<AddKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<AddKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new AddKeyDTO(), code = 500 };
            }
        }

        /// <summary>
        /// 키 - value 업데이트 (키-Value) 단일 묶음 업데이트
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseUnit<UpdateKeyDTO>> UpdateKeyService(HttpContext context, UpdateKeyDTO dto)
        {
            try
            {
                if (context is null || dto is null)
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if(String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                
                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(dto.ID!.Value).ConfigureAwait(false);
                if(KeyTB is null)
                    return new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 };

                bool? UpdateResult = await BuildingItemKeyInfoRepository.UpdateKeyInfo(dto, creater).ConfigureAwait(false);

                return UpdateResult switch
                {
                    true => new ResponseUnit<UpdateKeyDTO>() { message = "요청이 정상 처리되었습니다.", data = dto, code = 200 },
                    false => new ResponseUnit<UpdateKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 },
                    _ => new ResponseUnit<UpdateKeyDTO>() { message = "잘못된 요청입니다.", data = new UpdateKeyDTO(), code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<UpdateKeyDTO>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = new UpdateKeyDTO(), code = 500 };
            }
        }

        public async Task<ResponseUnit<bool?>> DeleteKeyListService(HttpContext context, List<int> KeyId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };
                
                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                bool? DeleteResult = await BuildingItemKeyInfoRepository.DeleteKeyList(KeyId, creater);
                
                return DeleteResult switch
                {
                    true => new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 },
                    false => new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = false, code = 500 },
                    _ => new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 }
                };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }

        public async Task<ResponseUnit<bool?>> DeleteKeyService(HttpContext context, int KeyId)
        {
            try
            {
                if (context is null)
                    return new ResponseUnit<bool?>() { message = "요청이 잘못되었습니다.", data = null, code = 404 };

                string? creater = Convert.ToString(context.Items["Name"]);
                if (String.IsNullOrWhiteSpace(creater))
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                BuildingItemKeyTb? KeyTB = await BuildingItemKeyInfoRepository.GetKeyInfo(KeyId).ConfigureAwait(false);

                DateTime ThisTime = DateTime.Now;

                if(KeyTB is null)
                    return new ResponseUnit<bool?>() { message = "잘못된 요청입니다.", data = null, code = 404 };

                KeyTB.DelDt = ThisTime;
                KeyTB.DelUser = creater;
                KeyTB.DelYn = true;

                bool? DeleteKeyResult = await BuildingItemKeyInfoRepository.DeleteKeyInfo(KeyTB).ConfigureAwait(false);

                if(DeleteKeyResult != true)
                    return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };

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
                            return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
                    }
                }

                return new ResponseUnit<bool?>() { message = "요청이 정상 처리되었습니다.", data = true, code = 200 };
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
#if DEBUG
                CreateBuilderLogger(ex);
#endif
                return new ResponseUnit<bool?>() { message = "서버에서 요청을 처리하지 못하였습니다.", data = null, code = 500 };
            }
        }
    
    }
}
