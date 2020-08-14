using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class ReportsModel
    {
        public Guid ReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportEmbedUrl { get; set; }
        public string PowerBIAccessToken { get; set; }
        public List<UsersModel> Users { get; set; }
        public List<ReportsModel> Reports { get; set; }
    }
}
