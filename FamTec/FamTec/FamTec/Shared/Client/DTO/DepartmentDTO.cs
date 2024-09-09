using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.DTO
{
    public class DepartmentDTO
    {
        public bool IsSelect { get; set; } = false;
        public int Id { get; set; }
        
        public string? Name { get; set; }
        public bool ManagerYN { get; set; } = false;
        public string? Description { get; set; } = null;
    }
}
