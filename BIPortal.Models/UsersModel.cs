﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BIPortal.Models
{
    public class UsersModel
    {
        public int UserID { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool Active { get; set; }
        public int PermissionID { get; set; }
        public PermissionMasterModel PermissionMaster { get; set; }
        

    }
}
