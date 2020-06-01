using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIPortal.Models
{
    public class WorkspaceModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Filter { get; set; }
        public string User { get; set; }
    }
}
