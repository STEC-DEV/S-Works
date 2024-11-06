using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamTec.Shared.Client.Class
{
    public class FilterItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; } = true;
    }
}
