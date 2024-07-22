using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace FamTec.Server.Repository.Inventory
{
    public class InventoryInfoRepository : IInventoryInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public InventoryInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        public async ValueTask<InventoryTb?> AddAsync(InventoryTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.InventoryTbs.Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<List<InventoryTb>?> GetMaterialCount(int? placeid, int? roomid, int? materialId)
        {
            try
            {
                if (materialId is null)
                    return null;
                if (roomid is null)
                    return null;
                if (placeid is null)
                    return null;

                List<InventoryTb>? model = await context.InventoryTbs
                .Where(m => m.MaterialTbId == materialId && 
                        m.RoomTbId == roomid && 
                        m.PlaceTbId == placeid &&
                        m.Occupant == null &&
                        m.DelYn != true).OrderByDescending(m => m.CreateDt).ToListAsync();

                if (model is [_, ..])
                    return model;
                else
                    return null;

            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<InventoryTb?> GetInventoryInfo(int? id)
        {
            try
            {
                if(id is not null)
                {
                    InventoryTb? model = await context.InventoryTbs.FirstOrDefaultAsync(m => m.Id == id);
                    if (model is not null)
                        return model;
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        public async ValueTask<bool?> AvailableCheck(int? placeid, int? roomid, int? materialid, string? guid)
        {
            try
            {
                if (placeid is null)
                    return false;
                if (roomid is null)
                    return false;
                if (materialid is null)
                    return false;
                if (guid is null)
                    return false;

                InventoryTb? model = await context.InventoryTbs.FirstOrDefaultAsync(m =>
                m.PlaceTbId == placeid &&
                m.RoomTbId == roomid &&
                m.MaterialTbId == materialid &&
                m.Occupant != null &&
                m.DelYn != true);

                if(model is not null)
                {
                    return false;
                }
                else
                {
                    List<InventoryTb>? Occupant = await context.InventoryTbs
                        .Where(m => m.PlaceTbId == placeid &&
                        m.RoomTbId == roomid &&
                        m.MaterialTbId == materialid &&
                        m.DelYn != true).ToListAsync();

                    foreach (InventoryTb OccModel in Occupant)
                    {
                        OccModel.Occupant = guid;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }



       

    }
}
