using DocumentFormat.OpenXml.Drawing;
using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FamTec.Server.Repository.Store
{
    public class StoreInfoRepository : IStoreInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public StoreInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        public async ValueTask<StoreTb?> AddAsync(StoreTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.StoreTbs.Add(model);
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

        public async ValueTask<List<InOutHistoryListDTO>?> GetInOutList(int? placeid)
        {
            try
            {
                if(placeid is not null)
                {
                    List<InOutHistoryListDTO> model = (from Store in context.StoreTbs.Where(m => m.DelYn != true)
                                                     join Material in context.MaterialTbs.Where(m => m.DelYn != true)
                                                     on Store.MaterialTbId equals Material.Id
                                                     join Room in context.RoomTbs.Where(m => m.DelYn != true)
                                                     on Store.Location equals Room.Id
                                                     select new InOutHistoryListDTO()
                                                     {
                                                         ID = Store.Id,
                                                         INOUT = Store.Inout,
                                                         InOutDate =Store.InoutDate,
                                                         RoomID = Room.Id,
                                                         RoomName = Room.Name,
                                                         MaterialID = Store.MaterialTbId,
                                                         MaterialName =Material.Name,
                                                         MaterialUnit = Material.Unit,
                                                         Num = Store.Num,
                                                         UnitPrice = Store.UnitPrice,
                                                         ToTalPrice = Store.TotalPrice,
                                                         Note = Store.Note
                                                     }).ToList().OrderByDescending(m => m.InOutDate).ToList();

                    if (model is [_, ..])
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

       

        public async ValueTask<List<InventoryTb>?> GetOutList(int? placeid, int? roomid)
        {
            try
            {
                int row = 22;

                List<InventoryTb>? model = await context.InventoryTbs.Where(m => m.Num > 0 && m.DelYn != true).ToListAsync();

                return null;

                //for (int i = 0; i < model.Count; i++)
                //{
                //    string GUID = Guid.NewGuid().ToString();

                //    using (var context = new WorksContext())
                //    using (var transaction = context.Database.BeginTransaction())
                //    {
                //        var sql = @"SELECT * FROM inventory_tb WHERE Id = @p0 FOR UPDATE";
                //        var entity = context.InventoryTbs.FromSqlRaw(sql, GUID).Single();

                //        // ... 여기에서 업데이트를 수행합니다 ...

                //        context.SaveChanges();

                //        transaction.Commit();
                //    }
                //}
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }
    }
}
