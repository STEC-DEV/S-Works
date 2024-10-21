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

        public DepartmentDTO() { }
        public DepartmentDTO(DepartmentDTO soruce)
        {
            Id = soruce.Id;
            IsSelect = soruce.IsSelect;
            Name = soruce.Name;
            Description = soruce.Description;
            ManagerYN = soruce.ManagerYN;

        }

    }
}
