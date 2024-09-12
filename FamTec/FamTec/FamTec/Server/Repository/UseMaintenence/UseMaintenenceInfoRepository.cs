using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.UseMaintenenceMaterial;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.UseMaintenence
{
    public class UseMaintenenceInfoRepository : IUseMaintenenceInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public UseMaintenenceInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 사용자재 세부 이력 리스트
        /// </summary>
        /// <param name="usematerialid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<UseMaterialDetailDTO?> GetDetailUseStoreList(int usematerialid, int placeid)
        {
            try
            {
                UseMaintenenceMaterialTb? UseMaterialTB = await context.UseMaintenenceMaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == usematerialid && m.DelYn != true);
                
                if (UseMaterialTB is null)
                    return null;

                RoomTb? RoomTB = await context.RoomTbs
                    .FirstOrDefaultAsync(m => m.Id == UseMaterialTB.RoomTbId && m.DelYn != true);
                
                if (RoomTB is null)
                    return null;

                MaterialTb? MaterialTB = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == UseMaterialTB.MaterialTbId && m.DelYn != true);
                
                if (MaterialTB is null)
                    return null;

                int Available = await context.InventoryTbs
                    .Where(m => m.DelYn != true && 
                                m.PlaceTbId == placeid && 
                                m.RoomTbId == UseMaterialTB.RoomTbId && 
                                m.MaterialTbId == UseMaterialTB.MaterialTbId)
                    .SumAsync(m => m.Num);


                UseMaterialDetailDTO dto = new UseMaterialDetailDTO();
                dto.UseMaterialId = UseMaterialTB.Id; // 유지보수 사용자재 ID
                dto.Num = UseMaterialTB.Num; // 수량
                dto.TotalPrice = UseMaterialTB.Totalprice; // 총 금액
                dto.RoomId = UseMaterialTB.RoomTbId; // 공간ID
                dto.RoomName = RoomTB.Name; // 공간명칭
                dto.MaterialId = UseMaterialTB.MaterialTbId; // 사용자재 ID
                dto.MaterialName = MaterialTB.Name; // 사용자재 명칭
                dto.MaintenanceId = UseMaterialTB.MaintenanceTbId; // 유지보수 ID
                dto.TotalAvailableInventory = Available; // 공간 가용재고

                List<StoreTb>? StoreList = await context.StoreTbs.Where(m => m.MaintenenceMaterialTbId == UseMaterialTB.Id && m.DelYn != true).ToListAsync();
                if(StoreList is [_, ..])
                {
                    foreach(StoreTb StoreTB in StoreList)
                    {
                        dto.UseList.Add(new UseDetailStoreDTO
                        {
                            Id = StoreTB.Id,
                            Num = StoreTB.Num,
                            UnitPrice = StoreTB.UnitPrice,
                            TotalPrice = StoreTB.TotalPrice
                        });
                    }
                }
                return dto;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

    }
}
