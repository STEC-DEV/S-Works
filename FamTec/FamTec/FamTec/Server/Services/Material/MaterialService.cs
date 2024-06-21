using FamTec.Server.Repository.Material;
using FamTec.Shared;
using FamTec.Shared.DTO;
using FamTec.Shared.Model;
using FamTec.Shared.Server.DTO.Material;

namespace FamTec.Server.Services.Material
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialInfoRepository MaterialInfoRepository;


        public MaterialService(IMaterialInfoRepository _materialinforepository)
        {
            this.MaterialInfoRepository = _materialinforepository;

        }


    
    }
}
