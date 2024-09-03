using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO.Normal.Material.Detail
{
    public class DetailMaterialListDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public List<InventoryRecordDTO> InventoryList { get; set; } = new List<InventoryRecordDTO>();

    }
}
