﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.DTO
{
    public class WorkspaceDTO
    {
        public Guid WorkSpaceId { get; set; }
        public string WorkSpaceName { get; set; }
        public string Filter { get; set; }
        public string WorkSpaceUser { get; set; }

        public Guid ReportId { get; set; }
        public string ReportName { get; set; }
        public int ReportCount { get; set; }
    }
}
