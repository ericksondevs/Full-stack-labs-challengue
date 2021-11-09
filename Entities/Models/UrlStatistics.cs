using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class UrlStatistics : IServiceStatus
    {
        public UrlStatistics()
        {
            Urls = new List<Url>();
        }
        public int UrlStatId { get; set; }
        [DisplayName("User Agent")]
        public string UserAgent { get; set; }
        [DisplayName("Host Address")]
        public string Browser { get; set; }
        [DisplayName("Browser Version")]
        public DateTime Date { get; set; }
        public int Id { get; set; }
        public virtual Url Url { get; set; }
        public virtual ICollection<Url> Urls { get; set; }

    }
}
