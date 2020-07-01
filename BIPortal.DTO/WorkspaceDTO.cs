using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.DTO
{
    public class WorkspaceDTO
    {
        public string WorkSpaceId { get; set; }
        public string WorkSpaceName { get; set; }
        public string Filter { get; set; }
        public string WorkSpaceUser { get; set; }
        public int ReportCount { get; set; }
        //public IEnumerable<ReportsDTO> Reports { get; set; }
        public string ReportId { get; set; }
        public string ReportName { get; set; }
    }
}
