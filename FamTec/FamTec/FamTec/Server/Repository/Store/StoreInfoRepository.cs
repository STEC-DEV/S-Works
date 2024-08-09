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

        public async ValueTask<StoreTb?> AddAsync(StoreTb model)
        {
            try
            {
                context.StoreTbs.Add(model);
                bool AddResult =await context.SaveChangesAsync() > 0 ? true : false;
                if (AddResult)
                {
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

        public async ValueTask<List<InOutHistoryListDTO>?> GetInOutList(int placeid)
        {
            try
            {
                List<InOutHistoryListDTO> model = (from Store in context.StoreTbs.Where(m => m.DelYn != true)
                                                    join Material in context.MaterialTbs.Where(m => m.DelYn != true)
                                                    on Store.MaterialTbId equals Material.Id
                                                    join Room in context.RoomTbs.Where(m => m.DelYn != true)
                                                    on Store.RoomTbId equals Room.Id
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
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        
    }
}
