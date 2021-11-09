using DataAccessLayer.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models
{
    public class Url : IServiceStatus
    {
        public int Id { get; set; }

        [Required]
        public string LongUrl { get; set; }
        public string ShortUrl { get; set; }
        public int Count { get; set; }
        public DateTime Date { get; set; }

        public int UrlStatId { get; set; }
        public  UrlStatistics UrlStatistics { get; set; }

    }
}
