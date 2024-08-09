using FamTec.Shared.Server.DTO;
using FamTec.Shared.Server.DTO.Maintenence;

namespace FamTec.Server.Services.Admin.Maintenance
{
    public interface IMaintanceService
    {
        public ValueTask<ResponseUnit<bool?>> AddMaintanceService(HttpContext context, AddMaintanceDTO dto);
    }
}
