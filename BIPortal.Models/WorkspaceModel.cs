using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class WorkspaceModel
    {
        public Guid WorkSpaceId { get; set; }
        public string WorkSpaceName { get; set; }
        public string Filter { get; set; }
        public string WorkSpaceUser { get; set; }
        public int ReportCount { get; set; }
        public IEnumerable<ReportsModel> Reports { get; set; }
    }
}
