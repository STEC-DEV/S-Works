using FamTec.Server.Databases;
using FamTec.Server.Services;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Material;
using Microsoft.EntityFrameworkCore;

namespace FamTec.Server.Repository.Material
{
    public class MaterialInfoRepository : IMaterialInfoRepository
    {
        private readonly WorksContext context;
        private ILogService LogService;

        public MaterialInfoRepository(WorksContext _context, ILogService _logservice)
        {
            this.context = _context;
            this.LogService = _logservice;
        }

        /// <summary>
        /// 자재 추가
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async ValueTask<MaterialTb?> AddAsync(MaterialTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.MaterialTbs.Add(model);
                    await context.SaveChangesAsync();
                    return model;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 사업장에 속해있는 자재 리스트들 반환
        /// </summary>
        /// <param name="placeid"></param>
        /// <returns></returns>
        public async ValueTask<List<MaterialTb>?> GetPlaceAllMaterialList(int? placeid)
        {
            try
            {
                List<MaterialTb>? model = await context.MaterialTbs
                    .Where(m => m.PlaceTbId == placeid && m.DelYn != true)
                    .ToListAsync();

                if(model is [_, ..])
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
        /// 자재 인덱스에 해당하는 모델클래스 반환
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public async ValueTask<MaterialTb?> GetDetailMaterialInfo(int? placeid, int? materialId)
        {
            try
            {
                MaterialTb? model = await context.MaterialTbs
                    .FirstOrDefaultAsync(m => m.Id == materialId && m.PlaceTbId == placeid && m.DelYn != true);

                if (model is not null)
                    return model;
                else
                    return null;
            }
            catch (Exception ex)
            {
                LogService.LogMessage(ex.ToString());
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// 자재정보 수정
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async ValueTask<bool?> UpdateMaterialInfo(MaterialTb? model)
        {
            try
            {
                if(model is not null)
                {
                    context.MaterialTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
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

        /// <summary>
        /// 자재정보 삭제
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async ValueTask<bool?> DeleteMaterialInfo(MaterialTb? model)
        {
            try
            {
                if (model is not null)
                {
                    context.MaterialTbs.Update(model);
                    return await context.SaveChangesAsync() > 0 ? true : false;
                }
                else
                {
                    return null;
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
