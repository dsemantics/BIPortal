using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.DTO
{
    public class ReportsDTO
    {
        public string ReportId { get; set; }
        public string ReportName { get; set; }
        public List<UsersDTO> Users { get; set; }
        public List<ReportsDTO> Reports { get; set; }
    }
}
