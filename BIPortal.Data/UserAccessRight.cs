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
    
    public partial class UserAccessRight
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string WorkspaceID { get; set; }
        public string WorkspaceName { get; set; }
        public string ReportID { get; set; }
        public string ReportName { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
    
        public virtual UserMaster UserMaster { get; set; }
    }
}
