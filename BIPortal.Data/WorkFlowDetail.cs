//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BIPortal.Data
{
    using System;
    using System.Collections.Generic;
    
    public class WorkFlowDetail
    {
        public int RequestDetailID { get; set; }
        public Nullable<int> RequestID { get; set; }
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public Nullable<System.DateTime> RequestedDate { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
        public string Status { get; set; }        
        public WorkFlowMaster WorkFlowMaster { get; set; }
    }
}