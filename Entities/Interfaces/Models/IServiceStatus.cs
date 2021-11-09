using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
    public abstract class IServiceStatus
    {
        bool successful = true;
        public bool Successful { get { return successful; } set { this.successful = value; } }
        public string Message { get; set; }
    }
}
