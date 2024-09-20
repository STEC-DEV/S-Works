using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Store;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Store
{
    public class StoreInfoRepository : IStoreInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public StoreInfoRepository(WorksContext _context,
            ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        public async ValueTask<StoreTb?> AddAsync(StoreTb model)
        {
            try
            {
                await context.StoreTbs.AddAsync(model);
                
                bool AddResult =await context.SaveChangesAsync() > 0 ? true : false;
                
                if (AddResult)
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

        /// <summary>
        /// 입출고 이력 조회
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<List<InOutHistoryListDTO>?> GetInOutList(int placeid)
        {
            try
            {
                List<StoreTb>? StoreList = await context.StoreTbs
                    .Where(m => m.DelYn != true && 
                    m.PlaceTbId == placeid)
                    .OrderByDescending(m => m.InoutDate)
                    .ToListAsync();


                List<InOutHistoryListDTO> model = (from Store in StoreList
                                                   join Material in context.MaterialTbs.Where(m => m.DelYn != true)
                                                   on Store.MaterialTbId equals Material.Id
                                                   join Room in context.RoomTbs.Where(m => m.DelYn != true)
                                                   on Store.RoomTbId equals Room.Id
                                                   select new InOutHistoryListDTO
                                                   {
                                                       ID = Store.Id,
                                                       INOUT = Store.Inout,
                                                       InOutDate = Store.InoutDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                       MaterialCode = Material.Code, // 품목코드
                                                       RoomID = Room.Id,
                                                       RoomName = Room.Name,
                                                       MaterialID = Store.MaterialTbId,
                                                       MaterialName = Material.Name,
                                                       MaterialUnit = Material.Unit,
                                                       Num = Store.Num,
                                                       UnitPrice = Store.UnitPrice,
                                                       ToTalPrice = Store.TotalPrice,
                                                       Note = Store.Note
                                                   }).ToList();
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

        /// <summary>
        /// 입출고 이력 조회 - 페이지 네이션
        /// </summary>
        /// <param name="placeid"></param>
        /// <param name="pagenum"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<List<InOutHistoryListDTO>?> GetInOutPageNationList(int placeid, int pagenumber, int pagesize)
        {
            try
            {
                List<StoreTb>? StoreList = await context.StoreTbs
                    .Where(m => m.DelYn != true &&
                    m.PlaceTbId == placeid)
                    .Skip((pagenumber - 1) * pagesize)
                    .Take(pagesize)
                    .OrderByDescending(m => m.InoutDate)
                    .ToListAsync();


                List<InOutHistoryListDTO> model = (from Store in StoreList
                                                   join Material in context.MaterialTbs.Where(m => m.DelYn != true)
                                                   on Store.MaterialTbId equals Material.Id
                                                   join Room in context.RoomTbs.Where(m => m.DelYn != true)
                                                   on Store.RoomTbId equals Room.Id
                                                   select new InOutHistoryListDTO
                                                   {
                                                       ID = Store.Id,
                                                       INOUT = Store.Inout,
                                                       InOutDate = Store.InoutDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                       MaterialCode = Material.Code, // 품목코드
                                                       RoomID = Room.Id,
                                                       RoomName = Room.Name,
                                                       MaterialID = Store.MaterialTbId,
                                                       MaterialName = Material.Name,
                                                       MaterialUnit = Material.Unit,
                                                       Num = Store.Num,
                                                       UnitPrice = Store.UnitPrice,
                                                       ToTalPrice = Store.TotalPrice,
                                                       Note = Store.Note
                                                   }).ToList();
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

        /// <summary>
        /// 사업장의 입출고 총 개수 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<int> GetPlaceInOutCount(int placeid)
        {
            try
            {
                int count = await context.StoreTbs
                    .Where(m => m.PlaceTbId == placeid &&
                           m.DelYn != true)
                    .CountAsync();

                return count;
            }
            catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

    }
}
