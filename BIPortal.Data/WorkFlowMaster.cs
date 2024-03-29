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
    
    public partial class WorkFlowMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WorkFlowMaster()
        {
            this.WorkFlowDetails = new HashSet<WorkFlowDetail>();
        }
    
        public int RequestID { get; set; }
        public string WorkspaceID { get; set; }
        public string WorkspaceName { get; set; }
        public Nullable<int> OwnerID { get; set; }
        public string RequestFor { get; set; }
        public string RequestedBy { get; set; }
        public System.DateTime RequestedDate { get; set; }
        public Nullable<System.DateTime> ProcessedDate { get; set; }
        public string Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WorkFlowDetail> WorkFlowDetails { get; set; }
        public virtual UserMaster UserMaster { get; set; }
    }
}
